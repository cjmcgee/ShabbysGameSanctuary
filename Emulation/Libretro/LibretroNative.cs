using System.Runtime.InteropServices;

namespace Emulation.Libretro
{
	/// <summary>
	/// Constants, enums, structs, and delegate types mirroring the relevant
	/// subset of the libretro API
	/// (https://github.com/libretro/libretro-common — <c>libretro.h</c>).
	///
	/// We define here only what the frontend actually uses; libretro's full
	/// header is much larger and most of it is core-only.
	/// </summary>
	internal static class LibretroNative
	{
		// ── Pixel formats ────────────────────────────────────────────────────
		public const int RETRO_PIXEL_FORMAT_0RGB1555	=	0;	// 16-bit, default
		public const int RETRO_PIXEL_FORMAT_XRGB8888	=	1;	// 32-bit, easiest
		public const int RETRO_PIXEL_FORMAT_RGB565		=	2;	// 16-bit

		// ── Environment commands (frontend ↔ core hooks) ─────────────────────
		// We respond to the small set the Stella / Atari 2600 cores actually
		// use; everything else returns false (= "not supported").
		public const uint RETRO_ENVIRONMENT_GET_OVERSCAN			=	2;
		public const uint RETRO_ENVIRONMENT_GET_CAN_DUPE			=	3;
		public const uint RETRO_ENVIRONMENT_SET_MESSAGE				=	6;
		public const uint RETRO_ENVIRONMENT_SHUTDOWN				=	7;
		public const uint RETRO_ENVIRONMENT_SET_PERFORMANCE_LEVEL	=	8;
		public const uint RETRO_ENVIRONMENT_GET_SYSTEM_DIRECTORY	=	9;
		public const uint RETRO_ENVIRONMENT_SET_PIXEL_FORMAT		=	10;
		public const uint RETRO_ENVIRONMENT_SET_INPUT_DESCRIPTORS	=	11;
		public const uint RETRO_ENVIRONMENT_SET_KEYBOARD_CALLBACK	=	12;
		public const uint RETRO_ENVIRONMENT_SET_DISK_CONTROL_INTERFACE	=	13;
		public const uint RETRO_ENVIRONMENT_SET_HW_RENDER			=	14;
		public const uint RETRO_ENVIRONMENT_GET_VARIABLE			=	15;
		public const uint RETRO_ENVIRONMENT_SET_VARIABLES			=	16;
		public const uint RETRO_ENVIRONMENT_GET_VARIABLE_UPDATE		=	17;
		public const uint RETRO_ENVIRONMENT_SET_SUPPORT_NO_GAME		=	18;
		public const uint RETRO_ENVIRONMENT_GET_LIBRETRO_PATH		=	19;
		public const uint RETRO_ENVIRONMENT_GET_LOG_INTERFACE		=	27;
		public const uint RETRO_ENVIRONMENT_GET_SAVE_DIRECTORY		=	31;
		public const uint RETRO_ENVIRONMENT_GET_LANGUAGE			=	39;

		// Bit flag returned in cmd to signify "experimental / extension".
		public const uint RETRO_ENVIRONMENT_EXPERIMENTAL =	0x10000u;
		public const uint RETRO_ENVIRONMENT_PRIVATE      =	0x20000u;

		// ── Log levels (RETRO_LOG_*) ─────────────────────────────────────────
		public const int RETRO_LOG_DEBUG	=	0;
		public const int RETRO_LOG_INFO		=	1;
		public const int RETRO_LOG_WARN		=	2;
		public const int RETRO_LOG_ERROR	=	3;

		// retro_log_callback is a struct with a single function pointer field.
		[StructLayout(LayoutKind.Sequential)]
		public struct retro_log_callback
		{
			public IntPtr	log;	// retro_log_printf_t function pointer
		}

		// libretro's log callback is variadic at the C level:
		//   void log(enum retro_log_level level, const char *fmt, ...);
		// Stella's libretro_logger calls it with a fixed shape:
		//   log_cb(level, "%s\n", token);
		// So we declare just the two extra args we actually care about
		// (the format string and its single char* argument). Extra varargs
		// pushed by the caller live in registers we never read; cdecl is
		// caller-cleanup so ignoring them is safe across both x86 and x86-64
		// System V ABIs.
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void retro_log_printf_t(int level, IntPtr fmt, IntPtr arg1);

		// ── Input devices / IDs ──────────────────────────────────────────────
		public const uint RETRO_DEVICE_NONE     =	0;
		public const uint RETRO_DEVICE_JOYPAD   =	1;
		public const uint RETRO_DEVICE_MOUSE    =	2;
		public const uint RETRO_DEVICE_KEYBOARD =	3;

		public const uint RETRO_DEVICE_ID_JOYPAD_B      =	0;
		public const uint RETRO_DEVICE_ID_JOYPAD_Y      =	1;
		public const uint RETRO_DEVICE_ID_JOYPAD_SELECT =	2;
		public const uint RETRO_DEVICE_ID_JOYPAD_START  =	3;
		public const uint RETRO_DEVICE_ID_JOYPAD_UP     =	4;
		public const uint RETRO_DEVICE_ID_JOYPAD_DOWN   =	5;
		public const uint RETRO_DEVICE_ID_JOYPAD_LEFT   =	6;
		public const uint RETRO_DEVICE_ID_JOYPAD_RIGHT  =	7;
		public const uint RETRO_DEVICE_ID_JOYPAD_A      =	8;
		public const uint RETRO_DEVICE_ID_JOYPAD_X      =	9;
		public const uint RETRO_DEVICE_ID_JOYPAD_L      =	10;
		public const uint RETRO_DEVICE_ID_JOYPAD_R      =	11;

		// ── Structs (libretro.h packing matches the platform default) ───────

		[StructLayout(LayoutKind.Sequential)]
		public struct retro_system_info
		{
			public IntPtr	library_name;		// const char*
			public IntPtr	library_version;	// const char*
			public IntPtr	valid_extensions;	// const char*
			[MarshalAs(UnmanagedType.U1)]	public bool	need_fullpath;
			[MarshalAs(UnmanagedType.U1)]	public bool	block_extract;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct retro_game_info
		{
			public IntPtr	path;		// const char* (UTF-8)
			public IntPtr	data;		// const void*
			public UIntPtr	size;		// size_t
			public IntPtr	meta;		// const char*
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct retro_game_geometry
		{
			public uint		base_width;
			public uint		base_height;
			public uint		max_width;
			public uint		max_height;
			public float	aspect_ratio;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct retro_system_timing
		{
			public double	fps;
			public double	sample_rate;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct retro_system_av_info
		{
			public retro_game_geometry	geometry;
			public retro_system_timing	timing;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct retro_variable
		{
			public IntPtr	key;	// const char*
			public IntPtr	value;	// const char*
		}

		// ── Delegate signatures ─────────────────────────────────────────────

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void retro_init_t();

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void retro_deinit_t();

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate uint retro_api_version_t();

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void retro_get_system_info_t(out retro_system_info info);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void retro_get_system_av_info_t(out retro_system_av_info info);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void retro_set_environment_t(retro_environment_t cb);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void retro_set_video_refresh_t(retro_video_refresh_t cb);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void retro_set_audio_sample_t(retro_audio_sample_t cb);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void retro_set_audio_sample_batch_t(retro_audio_sample_batch_t cb);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void retro_set_input_poll_t(retro_input_poll_t cb);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void retro_set_input_state_t(retro_input_state_t cb);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[return:	MarshalAs(UnmanagedType.U1)]
		public delegate bool retro_load_game_t(ref retro_game_info info);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void retro_unload_game_t();

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void retro_run_t();

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void retro_reset_t();

		// ── Memory access ────────────────────────────────────────────────────
		//
		// libretro exposes pointers into the core's emulated memory regions
		// via two entry points keyed by a numeric region id. We only need
		// SYSTEM_RAM for high-score reads (the Atari 2600's 128-byte RAM
		// block); the other ids are listed for completeness in case a
		// future core needs them.
		//
		// Pointer-stability rules per libretro.h: the returned pointer may
		// be invalidated by retro_load_game / retro_unload_game / retro_reset
		// / retro_serialize_state, plus retro_init / retro_deinit at the
		// session boundary. Between those events the pointer is stable, so
		// caching after retro_load_game is safe — we just re-resolve on Reset.
		public const uint RETRO_MEMORY_MASK			=	0xff;
		public const uint RETRO_MEMORY_SAVE_RAM		=	0;
		public const uint RETRO_MEMORY_RTC			=	1;
		public const uint RETRO_MEMORY_SYSTEM_RAM	=	2;
		public const uint RETRO_MEMORY_VIDEO_RAM	=	3;

		// void* retro_get_memory_data(unsigned id);
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IntPtr retro_get_memory_data_t(uint id);

		// size_t retro_get_memory_size(unsigned id);
		// size_t round-trips through UIntPtr / nuint so the binding is
		// correct on both 32- and 64-bit hosts. The A2600 size is small
		// (128 bytes) and trivially fits in uint, but writing the binding
		// correctly is free.
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate UIntPtr retro_get_memory_size_t(uint id);

		// ── Callback signatures (frontend → core) ───────────────────────────

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[return:	MarshalAs(UnmanagedType.U1)]
		public delegate bool retro_environment_t(uint cmd, IntPtr data);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void retro_video_refresh_t(IntPtr data, uint width, uint height, UIntPtr pitch);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void retro_audio_sample_t(short left, short right);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate UIntPtr retro_audio_sample_batch_t(IntPtr data, UIntPtr frames);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void retro_input_poll_t();

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate short retro_input_state_t(uint port, uint device, uint index, uint id);
	}
}
