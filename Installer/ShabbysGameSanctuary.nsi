; ShabbysGameSanctuary Windows installer
;
; Compile with:
;   makensis -DSTAGE_DIR=<absolute path to publish output> \
;            -DOUTPUT=<absolute path of .exe to produce> \
;            [-DAPP_VERSION=1.0.0]                                  \
;            Installer/ShabbysGameSanctuary.nsi
;
; The MSBuild target BuildWindowsInstaller in
; MainGame/MainGame.csproj invokes this after a
; win-x64 publish — no manual step needed when using publish-all.sh.

Unicode true
SetCompressor /SOLID lzma

;--------------------------------------------------------------------
; Required defines (passed in by MSBuild). Fail loudly if missing so a
; manual makensis invocation gets a useful error instead of a broken exe.
;--------------------------------------------------------------------
!ifndef STAGE_DIR
  !error "STAGE_DIR not defined. Pass -DSTAGE_DIR=<path to publish output>"
!endif
!ifndef OUTPUT
  !error "OUTPUT not defined. Pass -DOUTPUT=<path to installer exe>"
!endif
!ifndef APP_VERSION
  !define APP_VERSION "1.0.0"
!endif

!define APP_NAME       "Shabby's Game Sanctuary"
!define APP_PUBLISHER  "Shabby's Game Sanctuary"
!define APP_EXE        "MainGame.exe"
!define APP_REG_KEY    "Software\Microsoft\Windows\CurrentVersion\Uninstall\ShabbysGameSanctuary"

Name      "${APP_NAME} ${APP_VERSION}"
OutFile   "${OUTPUT}"
InstallDir "$PROGRAMFILES64\ShabbysGameSanctuary"
InstallDirRegKey HKLM "Software\ShabbysGameSanctuary" "InstallDir"
RequestExecutionLevel admin
ShowInstDetails show
ShowUninstDetails show

; Embed Windows Explorer file-version metadata.
VIProductVersion "${APP_VERSION}.0"
VIAddVersionKey  "ProductName"   "${APP_NAME}"
VIAddVersionKey  "ProductVersion" "${APP_VERSION}"
VIAddVersionKey  "FileVersion"    "${APP_VERSION}"
VIAddVersionKey  "FileDescription" "${APP_NAME} installer"
VIAddVersionKey  "CompanyName"    "${APP_PUBLISHER}"
VIAddVersionKey  "LegalCopyright" "Copyright (c) ${APP_PUBLISHER}"

;--------------------------------------------------------------------
; Modern UI + helpers (FileFunc gives us ${GetSize})
;--------------------------------------------------------------------
!include "MUI2.nsh"
!include "FileFunc.nsh"

; (Optional) header / wizard images and an installer icon can be
; configured here once we ship art for them:
; !define MUI_ICON           "Installer\app.ico"
; !define MUI_UNICON         "Installer\app.ico"
; !define MUI_HEADERIMAGE
; !define MUI_HEADERIMAGE_BITMAP "Installer\header.bmp"

!define MUI_ABORTWARNING

!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_COMPONENTS
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES

!define MUI_FINISHPAGE_RUN "$INSTDIR\${APP_EXE}"
!define MUI_FINISHPAGE_RUN_TEXT "Launch ${APP_NAME}"
!insertmacro MUI_PAGE_FINISH

!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES

!insertmacro MUI_LANGUAGE "English"

;--------------------------------------------------------------------
; Sections (= components)
;--------------------------------------------------------------------
Section "Core game (required)" SecCore
  SectionIn RO   ; read-only: user can't deselect

  SetOutPath "$INSTDIR"

  ; Copy everything in the publish directory. The /r flag handles the
  ; `runtimes\` subtree (MonoGame native deps live there).
  File /r "${STAGE_DIR}\*.*"

  ; Don't clobber a user-modified emulator-config.json on upgrade. The
  ; recursive File above will have overwritten it; restore from the
  ; pre-install backup we made if one exists. (See the preInstall
  ; section below.)
  IfFileExists "$INSTDIR\emulator-config.json.user" 0 +3
    Delete "$INSTDIR\emulator-config.json"
    Rename "$INSTDIR\emulator-config.json.user" "$INSTDIR\emulator-config.json"

  ; Uninstaller + registry entries (Add/Remove Programs).
  WriteUninstaller "$INSTDIR\Uninstall.exe"

  WriteRegStr HKLM "Software\ShabbysGameSanctuary" "InstallDir"      "$INSTDIR"
  WriteRegStr HKLM "Software\ShabbysGameSanctuary" "Version"         "${APP_VERSION}"

  WriteRegStr HKLM "${APP_REG_KEY}" "DisplayName"     "${APP_NAME}"
  WriteRegStr HKLM "${APP_REG_KEY}" "DisplayVersion"  "${APP_VERSION}"
  WriteRegStr HKLM "${APP_REG_KEY}" "Publisher"       "${APP_PUBLISHER}"
  WriteRegStr HKLM "${APP_REG_KEY}" "InstallLocation" "$INSTDIR"
  WriteRegStr HKLM "${APP_REG_KEY}" "DisplayIcon"     "$INSTDIR\${APP_EXE}"
  WriteRegStr HKLM "${APP_REG_KEY}" "UninstallString" "$\"$INSTDIR\Uninstall.exe$\""
  WriteRegStr HKLM "${APP_REG_KEY}" "QuietUninstallString" "$\"$INSTDIR\Uninstall.exe$\" /S"
  WriteRegDWORD HKLM "${APP_REG_KEY}" "NoModify" 1
  WriteRegDWORD HKLM "${APP_REG_KEY}" "NoRepair" 1

  ; Best-effort install size for Add/Remove Programs (KB).
  Push $0
  ${GetSize} "$INSTDIR" "/S=0K" $0 $1 $2
  WriteRegDWORD HKLM "${APP_REG_KEY}" "EstimatedSize" "$0"
  Pop $0
SectionEnd

Section "Start Menu shortcut" SecStartMenu
  CreateDirectory "$SMPROGRAMS\${APP_NAME}"
  CreateShortcut  "$SMPROGRAMS\${APP_NAME}\${APP_NAME}.lnk" "$INSTDIR\${APP_EXE}" "" "$INSTDIR\${APP_EXE}" 0
  CreateShortcut  "$SMPROGRAMS\${APP_NAME}\Uninstall ${APP_NAME}.lnk" "$INSTDIR\Uninstall.exe"
SectionEnd

Section "Desktop shortcut" SecDesktop
  CreateShortcut "$DESKTOP\${APP_NAME}.lnk" "$INSTDIR\${APP_EXE}" "" "$INSTDIR\${APP_EXE}" 0
SectionEnd

; Section descriptions surfaced in the Components page tooltip.
!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
  !insertmacro MUI_DESCRIPTION_TEXT ${SecCore}      "The game itself plus the bundled .NET runtime and emulator core."
  !insertmacro MUI_DESCRIPTION_TEXT ${SecStartMenu} "Add ${APP_NAME} to the Start Menu."
  !insertmacro MUI_DESCRIPTION_TEXT ${SecDesktop}   "Add a shortcut to ${APP_NAME} on the Desktop."
!insertmacro MUI_FUNCTION_DESCRIPTION_END

;--------------------------------------------------------------------
; Pre-install: stash any user-modified emulator-config.json so we can
; restore it after the File /r call clobbers it.
;--------------------------------------------------------------------
Function .onInit
  ; If a previous install exists, back up the config out of the way so
  ; the upgrade preserves user-tweaked ROM paths.
  IfFileExists "$INSTDIR\emulator-config.json" 0 noBackup
    CopyFiles /SILENT "$INSTDIR\emulator-config.json" "$INSTDIR\emulator-config.json.user"
  noBackup:
FunctionEnd

;--------------------------------------------------------------------
; Uninstaller
;--------------------------------------------------------------------
Section "Uninstall"
  ; Read the recorded install directory so we don't delete the wrong
  ; folder if a curious user changed the uninstaller's $INSTDIR.
  ReadRegStr $0 HKLM "Software\ShabbysGameSanctuary" "InstallDir"
  StrCmp $0 "" noInstDir
    StrCpy $INSTDIR $0
  noInstDir:

  Delete "$DESKTOP\${APP_NAME}.lnk"
  Delete "$SMPROGRAMS\${APP_NAME}\${APP_NAME}.lnk"
  Delete "$SMPROGRAMS\${APP_NAME}\Uninstall ${APP_NAME}.lnk"
  RMDir  "$SMPROGRAMS\${APP_NAME}"

  ; Wipe everything we installed. The recursive remove is bounded to
  ; the path we recorded above, which keeps a typo-on-INSTDIR from
  ; nuking something unrelated.
  RMDir /r "$INSTDIR"

  DeleteRegKey HKLM "${APP_REG_KEY}"
  DeleteRegKey HKLM "Software\ShabbysGameSanctuary"
SectionEnd
