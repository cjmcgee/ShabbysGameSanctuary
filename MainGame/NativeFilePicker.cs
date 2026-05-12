using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace ChildhoodAdventure;

/// <summary>
/// One entry in the file-type dropdown shown by <see cref="NativeFilePicker.PickFile"/>.
/// <para><see cref="Pattern"/> is a semicolon-separated list of globs in the
/// IFileOpenDialog format (Windows) — e.g. <c>"*.bin;*.a26;*.rom"</c>. The
/// helper translates that into the right form for the other platforms.</para>
/// </summary>
public sealed record FileFilter(string Description, string Pattern);

/// <summary>
/// Opens a native OS file- or folder-picker dialog and returns the
/// selected absolute path, or <c>null</c> if the user cancelled / the
/// platform has no available picker.
///
/// Windows uses an in-process P/Invoke into the Vista+ <c>IFileOpenDialog</c>
/// COM interface — no subprocess spawn, the dialog parents to the foreground
/// window so it stays on top of the game, and there's no PowerShell
/// execution-policy concern. macOS and Linux still shell out to the
/// platform-standard helpers (osascript, zenity / kdialog) since there's
/// no equivalent in-process API.
/// </summary>
public static class NativeFilePicker
{
	/// <summary>
	/// Folder-selection dialog. <paramref name="startDir"/> is the initial
	/// location; pass <c>null</c> to let the OS choose (typically the user's
	/// home directory).
	/// </summary>
	public static string? PickFolder(string title, string? startDir) =>
		Pick(title, startDir, filters: null, foldersOnly: true);

	/// <summary>
	/// File-selection dialog. <paramref name="filters"/> drives the file-
	/// type dropdown; pass <c>null</c> or an empty array to show all files.
	/// </summary>
	public static string? PickFile(string title, string? startDir, FileFilter[]? filters = null) =>
		Pick(title, startDir, filters, foldersOnly: false);

	private static string? Pick(string title, string? startDir,
		FileFilter[]? filters, bool foldersOnly)
	{
		try
		{
			if (OperatingSystem.IsWindows())	return WindowsDialog.Pick(title, startDir, filters, foldersOnly);
			if (OperatingSystem.IsMacOS())		return PickMac(title, startDir, filters, foldersOnly);
			if (OperatingSystem.IsLinux())		return PickLinux(title, startDir, filters, foldersOnly);
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine($"[NativeFilePicker] {ex.Message}");
		}
		return null;
	}

	// ── macOS ────────────────────────────────────────────────────────────
	//
	// `choose folder` / `choose file` are part of AppleScript Standard
	// Additions on every modern macOS. For file mode we map each filter's
	// globs to a bare extension list (`{"bin", "a26"}`); osascript doesn't
	// honour multiple filter groups so we collapse them into a single set.
	private static string? PickMac(string title, string? startDir,
		FileFilter[]? filters, bool foldersOnly)
	{
		string esc(string s) =>	s.Replace("\\", "\\\\").Replace("\"", "\\\"");

		string defaultLoc =	(!string.IsNullOrEmpty(startDir) && Directory.Exists(startDir))
			? $" default location POSIX file \"{esc(startDir!)}\""
			: "";

		string verb =	foldersOnly ? "choose folder" : "choose file";
		string typeClause =	"";
		if (!foldersOnly && filters != null && filters.Length > 0)
		{
			var exts =	ExtractExtensions(filters);
			if (exts.Length > 0)
			{
				var quoted =	string.Join(", ", exts.Select(e =>	$"\"{e}\""));
				typeClause =	$" of type {{{quoted}}}";
			}
		}

		string script =	$"POSIX path of ({verb} with prompt \"{esc(title)}\"{typeClause}{defaultLoc})";
		return RunCapture("osascript", $"-e '{script.Replace("'", "'\\''")}'");
	}

	// ── Linux ────────────────────────────────────────────────────────────
	//
	// zenity (GTK / GNOME default) is the first choice; kdialog (KDE) the
	// fallback. Both accept a list of file filters; we map each FileFilter
	// onto the helper's filter syntax.
	private static string? PickLinux(string title, string? startDir,
		FileFilter[]? filters, bool foldersOnly)
	{
		if (HasOnPath("zenity"))
		{
			var args =	new List<string> { "--file-selection", $"--title=\"{title.Replace("\"", "\\\"")}\"" };
			if (foldersOnly)	args.Add("--directory");
			if (!string.IsNullOrEmpty(startDir) && Directory.Exists(startDir))
				args.Add($"--filename=\"{startDir!.TrimEnd('/')}/\"");	// trailing / → start inside dir
			if (!foldersOnly && filters != null)
			{
				foreach (var f in filters)
				{
					// zenity filter syntax: "Name | *.ext1 *.ext2"
					var globs =	f.Pattern.Replace(";", " ");
					args.Add($"--file-filter=\"{f.Description} | {globs}\"");
				}
			}
			return RunCapture("zenity", string.Join(" ", args));
		}

		if (HasOnPath("kdialog"))
		{
			string start =	(!string.IsNullOrEmpty(startDir) && Directory.Exists(startDir))	? startDir! :	".";
			string esc(string s) =>	s.Replace("\"", "\\\"");

			if (foldersOnly)
				return RunCapture("kdialog",
					$"--getexistingdirectory \"{esc(start)}\" --title \"{esc(title)}\"");

			// kdialog file filter: 'Name (*.ext1 *.ext2)|Other (*.foo)'
			string filterArg =	"";
			if (filters != null && filters.Length > 0)
			{
				var parts =	filters.Select(f =>	$"{f.Description} ({f.Pattern.Replace(";", " ")})");
				filterArg =	$" \"{esc(string.Join("|", parts))}\"";
			}
			return RunCapture("kdialog",
				$"--getopenfilename \"{esc(start)}\"{filterArg} --title \"{esc(title)}\"");
		}

		Console.Error.WriteLine(
			"[NativeFilePicker] Neither zenity nor kdialog found on PATH; install one of:\n" +
			"  Debian/Ubuntu: sudo apt install zenity\n" +
			"  Fedora       : sudo dnf install zenity");
		return null;
	}

	// Pull bare extensions out of FileFilter patterns. "*.bin;*.a26" → ["bin","a26"].
	// macOS `choose file` only accepts unique extensions, so we dedupe.
	private static string[] ExtractExtensions(FileFilter[] filters)
	{
		var set =	new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		foreach (var f in filters)
		{
			foreach (var token in f.Pattern.Split(';'))
			{
				var t =	token.Trim().TrimStart('*').TrimStart('.');
				if (t.Length > 0 && t != "*")	set.Add(t);
			}
		}
		return set.ToArray();
	}

	private static bool HasOnPath(string name)
	{
		// RunCapture returns null on non-zero exit — `which foo` exits 1
		// when foo is not on PATH, so a null return tells us "not found".
		// Reusing RunCapture means HasOnPath benefits from the same
		// Snap-env scrubbing applied elsewhere.
		return RunCapture("which", name) != null;
	}

	private static string? RunCapture(string exe, string args)
	{
		var psi =	new ProcessStartInfo
		{
			FileName =	exe,
			Arguments =	args,
			RedirectStandardOutput =	true,
			RedirectStandardError =	true,
			UseShellExecute =	false,
			CreateNoWindow =	true,
		};

		// Strip Snap-set linker / GTK variables before spawning system
		// helpers. When the parent process is launched from a Snap
		// (VS Code's Snap install, Snap dotnet, etc), the inherited
		// environment points the dynamic loader at /snap/.../lib first,
		// and system binaries like zenity die with
		//   "symbol lookup error: libpthread.so.0: undefined symbol:
		//    __libc_pthread_init, version GLIBC_PRIVATE"
		// because the Snap libpthread doesn't expose private symbols the
		// host glibc-linked binary needs. Removing the vars makes the
		// child fall back to the system loader, which works.
		//
		// Safe to clear unconditionally on Linux — we're spawning a system
		// tool that should use the system libraries, never the Snap ones.
		if (OperatingSystem.IsLinux())
		{
			foreach (var v in new[]
			{
				"LD_LIBRARY_PATH",
				"LD_PRELOAD",
				"GTK_PATH",
				"GTK_EXE_PREFIX",
				"GTK_DATA_PREFIX",
				"GTK_IM_MODULE_FILE",
				"GDK_PIXBUF_MODULE_FILE",
				"GDK_PIXBUF_MODULEDIR",
				"GIO_MODULE_DIR",
				"GSETTINGS_SCHEMA_DIR",
				"LOCPATH",
				"SNAP_LIBRARY_PATH",
			})
			{
				psi.Environment.Remove(v);
			}
		}

		using var p =	Process.Start(psi);
		if (p == null)	return null;

		string stdout =	p.StandardOutput.ReadToEnd();
		string stderr =	p.StandardError.ReadToEnd();
		p.WaitForExit();

		if (p.ExitCode != 0)
		{
			// zenity / kdialog / osascript all return 1 on user-cancel.
			// Anything else is a real failure — log the stderr so the
			// next person to hit this isn't staring at a silent picker.
			if (!string.IsNullOrWhiteSpace(stderr))
				Console.Error.WriteLine($"[NativeFilePicker] {exe} exit {p.ExitCode}: {stderr.Trim()}");
			return null;
		}

		string trimmed =	stdout.Trim();
		return string.IsNullOrEmpty(trimmed) ? null :	trimmed;
	}

	// ── Windows: IFileOpenDialog via P/Invoke ────────────────────────────
	//
	// We import just enough of the Vista+ Shell Common Item Dialog API to
	// drive a folder or file picker:
	//
	//   IFileDialog        — SetOptions / SetTitle / SetFolder / Show / GetResult
	//   IFileOpenDialog    — derived; the open variant we instantiate
	//   IShellItem         — the in/out path object
	//   SHCreateItemFromParsingName — turn a string path into an IShellItem
	//
	// Vtable order matters: every method on the interface (and on its
	// COM bases, up through IUnknown) must be declared in the right slot,
	// even if we never call it. Missing entries silently corrupt the call
	// site, so we declare the full surface up to GetResult and stop there
	// (the methods we don't use further down the vtable would only be a
	// problem if we tried to call them — we don't).
	[SupportedOSPlatform("windows")]
	private static class WindowsDialog
	{
		private static readonly Guid CLSID_FileOpenDialog =	new("DC1C5A9C-E88A-4DDE-A5A1-60F82A20AEF7");
		private static readonly Guid IID_IShellItem =			new("43826D1E-E718-42EE-BC55-A1E261C37BFE");

		[Flags]
		private enum FOS : uint
		{
			FORCEFILESYSTEM	= 0x40,
			PICKFOLDERS		= 0x20,
			PATHMUSTEXIST	= 0x800,
			FILEMUSTEXIST	= 0x1000,
			DONTADDTORECENT	= 0x2000000,
		}

		// SIGDN values for IShellItem.GetDisplayName. FILESYSPATH gives the
		// canonical absolute filesystem path of the chosen item.
		private const uint SIGDN_FILESYSPATH =	0x80058000;

		// HRESULT for a user-cancelled dialog. Show() returns this rather
		// than failing, so we treat it as "no selection" rather than error.
		private const int HRESULT_CANCELLED =	unchecked((int)0x800704C7);

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		private struct COMDLG_FILTERSPEC
		{
			[MarshalAs(UnmanagedType.LPWStr)]	public string pszName;
			[MarshalAs(UnmanagedType.LPWStr)]	public string pszSpec;
		}

		[ComImport]
		[Guid("42f85136-db7e-439c-85f1-e4075d135fc8")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		private interface IFileDialog
		{
			// IModalWindow.Show — PreserveSig so we can read the cancel HRESULT
			// directly instead of catching COMException.
			[PreserveSig] int Show(IntPtr parent);

			// IFileDialog
			void SetFileTypes(uint cFileTypes,
				[In, MarshalAs(UnmanagedType.LPArray)] COMDLG_FILTERSPEC[] rgFilterSpec);
			void SetFileTypeIndex(uint iFileType);
			void GetFileTypeIndex(out uint piFileType);
			void Advise(IntPtr pfde, out uint pdwCookie);
			void Unadvise(uint dwCookie);
			void SetOptions(FOS fos);
			void GetOptions(out FOS pfos);
			void SetDefaultFolder(IShellItem psi);
			void SetFolder(IShellItem psi);
			void GetFolder(out IShellItem ppsi);
			void GetCurrentSelection(out IShellItem ppsi);
			void SetFileName([MarshalAs(UnmanagedType.LPWStr)] string pszName);
			void GetFileName([MarshalAs(UnmanagedType.LPWStr)] out string pszName);
			void SetTitle([MarshalAs(UnmanagedType.LPWStr)] string pszTitle);
			void SetOkButtonLabel([MarshalAs(UnmanagedType.LPWStr)] string pszText);
			void SetFileNameLabel([MarshalAs(UnmanagedType.LPWStr)] string pszLabel);
			void GetResult(out IShellItem ppsi);
		}

		[ComImport]
		[Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		private interface IShellItem
		{
			void BindToHandler(IntPtr pbc, ref Guid bhid, ref Guid riid, out IntPtr ppv);
			void GetParent(out IShellItem ppsi);
			// Out value is a CoTaskMem-allocated LPWSTR; we receive it as
			// IntPtr and free it ourselves rather than relying on the
			// runtime's [Out, LPWStr] marshaling (which is inconsistent
			// across .NET versions for this specific case).
			void GetDisplayName(uint sigdnName, out IntPtr ppszName);
			void GetAttributes(uint sfgaoMask, out uint psfgaoAttribs);
			void Compare(IShellItem psi, uint hint, out int piOrder);
		}

		[DllImport("shell32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = false)]
		private static extern IShellItem SHCreateItemFromParsingName(
			[MarshalAs(UnmanagedType.LPWStr)] string pszPath,
			IntPtr pbc,
			[In] ref Guid riid);

		// Parent the dialog to whatever window is currently in front (the
		// game). This is what keeps it from getting buried behind a
		// fullscreen MonoGame window.
		[DllImport("user32.dll")]
		private static extern IntPtr GetForegroundWindow();

		/// <summary>
		/// Public entry point. The actual COM dance happens in
		/// <see cref="PickOnStaThread"/> on a dedicated STA thread —
		/// IFileOpenDialog requires single-threaded-apartment semantics to
		/// drive its modal message loop, but the game's main thread is MTA
		/// (the default — Program.Main has no <c>[STAThread]</c> attribute
		/// and MonoGame doesn't request one). Calling the dialog from MTA
		/// hangs: COM marshals the call to a thread without a real message
		/// pump and the modal loop never returns. Spinning up an STA worker
		/// is the standard workaround and lets the main thread block-and-wait
		/// just like it does for the zenity / osascript subprocesses on
		/// other platforms.
		/// </summary>
		public static string? Pick(string title, string? startDir,
			FileFilter[]? filters, bool foldersOnly)
		{
			string? result =	null;
			Exception? failure =	null;

			var thread =	new Thread(() =>
			{
				try		{ result =	PickOnStaThread(title, startDir, filters, foldersOnly); }
				catch (Exception ex)	{ failure =	ex; }
			})
			{
				IsBackground =	true,
				Name =	"NativeFilePicker.STA",
			};
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
			thread.Join();

			if (failure != null)
			{
				Console.Error.WriteLine($"[NativeFilePicker] {failure.Message}");
				return null;
			}
			return result;
		}

		private static string? PickOnStaThread(string title, string? startDir,
			FileFilter[]? filters, bool foldersOnly)
		{
			var clsidType =	Type.GetTypeFromCLSID(CLSID_FileOpenDialog)
				?? throw new InvalidOperationException("FileOpenDialog COM class not registered");
			var dialog =	(IFileDialog)Activator.CreateInstance(clsidType)!;

			try
			{
				dialog.GetOptions(out var opts);
				opts |=	FOS.FORCEFILESYSTEM | FOS.PATHMUSTEXIST | FOS.DONTADDTORECENT;
				if (foldersOnly)	opts |=	FOS.PICKFOLDERS;
				else				opts |=	FOS.FILEMUSTEXIST;
				dialog.SetOptions(opts);

				dialog.SetTitle(title);

				if (!foldersOnly && filters is { Length: > 0 })
				{
					var specs =	new COMDLG_FILTERSPEC[filters.Length];
					for (int i = 0; i < filters.Length; i++)
					{
						specs[i].pszName =	filters[i].Description;
						specs[i].pszSpec =	filters[i].Pattern;
					}
					dialog.SetFileTypes((uint)specs.Length, specs);
				}

				if (!string.IsNullOrEmpty(startDir) && Directory.Exists(startDir))
				{
					try
					{
						var iid =	IID_IShellItem;
						var item =	SHCreateItemFromParsingName(startDir!, IntPtr.Zero, ref iid);
						dialog.SetFolder(item);
					}
					catch (Exception ex)
					{
						// Bad path is non-fatal — the dialog opens at its default location.
						Console.Error.WriteLine($"[NativeFilePicker] SetFolder({startDir}) failed: {ex.Message}");
					}
				}

				int hr =	dialog.Show(GetForegroundWindow());
				if (hr == HRESULT_CANCELLED)	return null;
				if (hr != 0)
				{
					Console.Error.WriteLine($"[NativeFilePicker] IFileOpenDialog.Show returned 0x{hr:X8}");
					return null;
				}

				dialog.GetResult(out var result);
				result.GetDisplayName(SIGDN_FILESYSPATH, out var pszPath);
				try
				{
					return Marshal.PtrToStringUni(pszPath);
				}
				finally
				{
					Marshal.FreeCoTaskMem(pszPath);
				}
			}
			finally
			{
				Marshal.ReleaseComObject(dialog);
			}
		}
	}
}
