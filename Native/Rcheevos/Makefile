# ─────────────────────────────────────────────────────────────────────────
# rcheevos shared library build
#
# Compiles the value/lboard/richpresence core of rcheevos into a single
# shared library that ChildhoodAdventure P/Invokes for score-formula
# evaluation. The rcheevos submodule itself is not modified — we just
# point at its sources from here.
#
# Defines:
#   RC_SHARED            → rc_export.h emits the right visibility/dllexport
#                          attributes for every RC_EXPORT public function
#   -fvisibility=hidden  → everything else stays hidden (transitive deps,
#                          internal helpers in rcheevos), so the exported
#                          surface is exactly the public rcheevos API.
#
# Cross-compile: pass CC=x86_64-w64-mingw32-gcc and the output naming
# auto-adjusts (or override OUTPUT yourself). See ChildhoodAdventure.csproj's
# BuildRcheevosWindowsLib target for the publish-time invocation.
# ─────────────────────────────────────────────────────────────────────────

RC_DIR     := ../../rcheevos
SRC_DIR    := $(RC_DIR)/src
INC_DIR    := $(RC_DIR)/include

CC         ?= cc
AR         ?= ar

# Probe the toolchain. mingw-w64 builds produce a .dll; otherwise pick by
# host OS. The csproj overrides OUTPUT when cross-building so we never
# need to second-guess from inside the Makefile.
ifeq ($(findstring mingw,$(CC)),mingw)
  OUTPUT ?= rcheevos.dll
  LDFLAGS_SHARED := -shared -Wl,--out-implib,rcheevos.lib
else
  UNAME_S := $(shell uname -s)
  ifeq ($(UNAME_S),Darwin)
    OUTPUT ?= librcheevos.dylib
    LDFLAGS_SHARED := -dynamiclib -install_name @rpath/$(OUTPUT)
  else
    OUTPUT ?= librcheevos.so
    LDFLAGS_SHARED := -shared -Wl,-soname,$(OUTPUT)
  endif
endif

CFLAGS := -O2 -fPIC -fvisibility=hidden -DRC_SHARED \
          -I$(INC_DIR) -I$(SRC_DIR) -I$(SRC_DIR)/rcheevos

# Just enough of rcheevos for value / leaderboard / rich-presence
# evaluation. Deliberately skipping:
#   rapi/*                     — HTTP client; we hit RA's API offline only
#   rhash/*                    — MD5 / rom-hashing; not used at runtime
#   rc_client*.c               — high-level client w/ session management
#   rc_libretro.c              — libretro-frontend integration helper
# All of those add transitive deps (zlib, libcurl, etc.) and we'd never
# call into them.
SOURCES := \
    $(SRC_DIR)/rc_compat.c \
    $(SRC_DIR)/rc_util.c \
    $(SRC_DIR)/rc_version.c \
    $(SRC_DIR)/rcheevos/alloc.c \
    $(SRC_DIR)/rcheevos/condition.c \
    $(SRC_DIR)/rcheevos/condset.c \
    $(SRC_DIR)/rcheevos/consoleinfo.c \
    $(SRC_DIR)/rcheevos/format.c \
    $(SRC_DIR)/rcheevos/lboard.c \
    $(SRC_DIR)/rcheevos/memref.c \
    $(SRC_DIR)/rcheevos/operand.c \
    $(SRC_DIR)/rcheevos/rc_validate.c \
    $(SRC_DIR)/rcheevos/richpresence.c \
    $(SRC_DIR)/rcheevos/runtime.c \
    $(SRC_DIR)/rcheevos/runtime_progress.c \
    $(SRC_DIR)/rcheevos/trigger.c \
    $(SRC_DIR)/rcheevos/value.c

OBJECTS := $(SOURCES:.c=.o)

$(OUTPUT): $(OBJECTS)
	$(CC) $(LDFLAGS_SHARED) $(CFLAGS) -o $@ $^

%.o: %.c
	$(CC) -c $(CFLAGS) -o $@ $<

clean:
	rm -f $(OBJECTS) $(OUTPUT) rcheevos.lib

.PHONY: clean
