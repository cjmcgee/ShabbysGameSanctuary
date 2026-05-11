using System.Runtime.InteropServices;
using static TileEngine.MiniGames.Libretro.LibretroNative;

namespace TileEngine.MiniGames.Libretro
{
	/// <summary>
	/// Loads a libretro core shared library at runtime and drives its
	/// lifecycle (init → load_game → run* → unload_game → deinit).
	///
	/// Callers register handlers for the four "frontend implements" hooks
	/// (video, audio, input poll, input state) before <see cref="LoadGame"/>
	/// — the core then calls those during <see cref="Run"/>. The environment
	/// callback is handled internally with sensible defaults; subclasses or
	/// callers can override individual cmd codes via
	/// <see cref="EnvironmentOverride"/>.
	///
	/// Cross-platform: uses <c>NativeLibrary</c>, so the same code works for
	/// <c>.so</c> (Linux), <c>.dll</c> (Windows), and <c>.dylib</c> (macOS).
	/// </summary>
	public sealed class LibretroCore :	IDisposable
	{
		// ── Function pointers loaded from the core ──────────────────────────
		private IntPtr _lib;
		private retro_init_t					_retroInit =	null!;
		private retro_deinit_t					_retroDeinit =	null!;
		private retro_get_system_info_t			_getSystemInfo =	null!;
		private retro_get_system_av_info_t		_getSystemAvInfo =	null!;
		private retro_set_environment_t			_setEnvironment =	null!;
		private retro_set_video_refresh_t		_setVideoRefresh =	null!;
		private retro_set_audio_sample_t		_setAudioSample =	null!;
		private retro_set_audio_sample_batch_t	_setAudioSampleBatch =	null!;
		private retro_set_input_poll_t			_setInputPoll =	null!;
		private retro_set_input_state_t			_setInputState =	null!;
		private retro_load_game_t				_loadGame =	null!;
		private retro_unload_game_t				_unloadGame =	null!;
		private retro_run_t						_run =	null!;
		private retro_reset_t?					_reset;

		// ── Callback delegates (held to prevent GC from sweeping them out
		// from under the unmanaged core mid-call). Each is built once and
		// passed to the core via retro_set_*. ────────────────────────────────
		private retro_environment_t			_envCb =	null!;
		private retro_video_refresh_t		_videoCb =	null!;
		private retro_audio_sample_t		_audioCb =	null!;
		private retro_audio_sample_batch_t	_audioBatchCb =	null!;
		private retro_input_poll_t			_inputPollCb =	null!;
		private retro_input_state_t			_inputStateCb =	null!;

		// ── Caller-supplied handlers ────────────────────────────────────────

		/// <summary>Called every time the core renders a frame.</summary>
		public Action<IntPtr, uint, uint, uint>?	OnVideoRefresh { get; set; }

		/// <summary>Called once per frame for input state queries.</summary>
		public Action?	OnInputPoll { get; set; }

		/// <summary>Returns input state (1=pressed, 0=not) for a button.</summary>
		public Func<uint, uint, uint, uint, short>?	OnInputState { get; set; }

		/// <summary>Optional: called for each pair of audio samples (left, right).</summary>
		public Action<short, short>?	OnAudioSample { get; set; }

		/// <summary>Optional: called with a batch of stereo frames.</summary>
		public Func<IntPtr, ulong, ulong>?	OnAudioBatch { get; set; }

		/// <summary>
		/// Lets a caller override or extend environment-callback handling.
		/// Return true to indicate the cmd was handled; false falls through
		/// to the built-in handling.
		/// </summary>
		public Func<uint, IntPtr, bool?>?	EnvironmentOverride { get; set; }

		// ── Cached system-supplied paths (returned via env callback) ────────

		/// <summary>Path passed to the core via <c>RETRO_ENVIRONMENT_GET_SYSTEM_DIRECTORY</c>.</summary>
		public string SystemDirectory { get; set; } =	"";

		/// <summary>Path passed via <c>RETRO_ENVIRONMENT_GET_SAVE_DIRECTORY</c>.</summary>
		public string SaveDirectory { get; set; } =	"";

		/// <summary>Pixel format the core asks for (set during env callback).</summary>
		public int PixelFormat { get; private set; } =	RETRO_PIXEL_FORMAT_0RGB1555;

		// ── System info populated after load ────────────────────────────────
		public string LibraryName { get; private set; } =	"";
		public string LibraryVersion { get; private set; } =	"";
		public uint BaseWidth { get; private set; }
		public uint BaseHeight { get; private set; }
		public uint MaxWidth { get; private set; }
		public uint MaxHeight { get; private set; }
		public double Fps { get; private set; }
		public double SampleRate { get; private set; }

		private bool _gameLoaded;
		private bool _initCalled;

		// Pinned UTF-8 buffers backing the strings we expose to the core via
		// the environment callback (system dir, save dir). Pinned so the
		// pointers we hand the core stay valid as long as we're alive.
		private GCHandle _systemDirHandle;
		private GCHandle _saveDirHandle;

		// ── Public API ──────────────────────────────────────────────────────

		/// <summary>
		/// Open the core .so / .dll / .dylib at <paramref name="path"/> and
		/// resolve all the entry points the frontend uses.
		/// </summary>
		public void Open(string path)
		{
			if (!File.Exists(path))
				throw new FileNotFoundException($"Libretro core not found: {path}", path);

			_lib =	NativeLibrary.Load(path);

			_retroInit =			GetExport<retro_init_t>("retro_init");
			_retroDeinit =			GetExport<retro_deinit_t>("retro_deinit");
			_getSystemInfo =		GetExport<retro_get_system_info_t>("retro_get_system_info");
			_getSystemAvInfo =		GetExport<retro_get_system_av_info_t>("retro_get_system_av_info");
			_setEnvironment =		GetExport<retro_set_environment_t>("retro_set_environment");
			_setVideoRefresh =		GetExport<retro_set_video_refresh_t>("retro_set_video_refresh");
			_setAudioSample =		GetExport<retro_set_audio_sample_t>("retro_set_audio_sample");
			_setAudioSampleBatch =	GetExport<retro_set_audio_sample_batch_t>("retro_set_audio_sample_batch");
			_setInputPoll =			GetExport<retro_set_input_poll_t>("retro_set_input_poll");
			_setInputState =		GetExport<retro_set_input_state_t>("retro_set_input_state");
			_loadGame =				GetExport<retro_load_game_t>("retro_load_game");
			_unloadGame =			GetExport<retro_unload_game_t>("retro_unload_game");
			_run =					GetExport<retro_run_t>("retro_run");
			_reset =				TryGetExport<retro_reset_t>("retro_reset");

			// Probe library identity. Some cores set library_name in
			// retro_init only; this is just an informational read.
			_getSystemInfo(out var sysinfo);
			LibraryName = Marshal.PtrToStringUTF8(sysinfo.library_name)	?? "";
			LibraryVersion = Marshal.PtrToStringUTF8(sysinfo.library_version)	?? "";

			// Build callback delegates ONCE and keep them alive via the
			// fields. Passing them to the core retains them on the unmanaged
			// side; we still need the managed reference to prevent GC.
			_envCb =			HandleEnvironment;
			_videoCb =			HandleVideoRefresh;
			_audioCb =			HandleAudioSample;
			_audioBatchCb =		HandleAudioBatch;
			_inputPollCb =		HandleInputPoll;
			_inputStateCb =		HandleInputState;

			// IMPORTANT: env must be set BEFORE retro_init — cores often
			// call back during init to query / negotiate features.
			_setEnvironment(_envCb);
		}

		/// <summary>Call <c>retro_init</c>. Must follow <see cref="Open"/>.</summary>
		public void Init()
		{
			if (_initCalled)	return;
			_retroInit();
			_initCalled =	true;

			_setVideoRefresh(_videoCb);
			_setAudioSample(_audioCb);
			_setAudioSampleBatch(_audioBatchCb);
			_setInputPoll(_inputPollCb);
			_setInputState(_inputStateCb);
		}

		/// <summary>
		/// Load a ROM from <paramref name="romPath"/>. Reads the file into a
		/// managed buffer, pins it, and hands the core a <c>retro_game_info</c>
		/// pointing at both the on-disk path and the buffer (cores choose).
		/// </summary>
		public bool LoadGame(string romPath)
		{
			if (!_initCalled)	throw new InvalidOperationException("Init() must be called before LoadGame().");

			byte[]	data =	File.ReadAllBytes(romPath);
			GCHandle dataHandle =	GCHandle.Alloc(data, GCHandleType.Pinned);

			IntPtr pathPtr =	Marshal.StringToCoTaskMemUTF8(romPath);
			try
			{
				var info =	new retro_game_info
				{
					path =	pathPtr,
					data =	dataHandle.AddrOfPinnedObject(),
					size =	(UIntPtr)data.Length,
					meta =	IntPtr.Zero,
				};

				bool ok =	_loadGame(ref info);
				if (!ok)	return false;
				_gameLoaded =	true;

				_getSystemAvInfo(out var av);
				BaseWidth =		av.geometry.base_width;
				BaseHeight =	av.geometry.base_height;
				MaxWidth =		av.geometry.max_width;
				MaxHeight =		av.geometry.max_height;
				Fps =			av.timing.fps;
				SampleRate =	av.timing.sample_rate;
				return true;
			}
			finally
			{
				if (pathPtr != IntPtr.Zero)	Marshal.FreeCoTaskMem(pathPtr);
				if (dataHandle.IsAllocated)	dataHandle.Free();
			}
		}

		/// <summary>Tick the core for one frame.</summary>
		public void Run()	=>	_run();

		/// <summary>Soft-reset the running game, if the core supports it.</summary>
		public void Reset()	=>	_reset?.Invoke();

		// ── IDisposable ─────────────────────────────────────────────────────

		public void Dispose()
		{
			try
			{
				if (_gameLoaded)	_unloadGame();
				if (_initCalled)	_retroDeinit();
			}
			catch
			{
				// We're going down anyway; don't leak exceptions out of Dispose.
			}
			if (_systemDirHandle.IsAllocated)	_systemDirHandle.Free();
			if (_saveDirHandle.IsAllocated)	_saveDirHandle.Free();
			if (_lib != IntPtr.Zero)
			{
				NativeLibrary.Free(_lib);
				_lib =	IntPtr.Zero;
			}
		}

		// ── Helpers ─────────────────────────────────────────────────────────

		private T GetExport<T>(string name)	where T :	Delegate
		{
			var p =	NativeLibrary.GetExport(_lib, name);
			return Marshal.GetDelegateForFunctionPointer<T>(p);
		}

		private T?	TryGetExport<T>(string name)	where T :	Delegate
		{
			return NativeLibrary.TryGetExport(_lib, name, out var p)
				? Marshal.GetDelegateForFunctionPointer<T>(p)
				:	null;
		}

		// ── Environment callback ────────────────────────────────────────────
		// Returns true → "we handled this", false → "not supported".
		private bool HandleEnvironment(uint cmd, IntPtr data)
		{
			// Allow caller-side extension first.
			if (EnvironmentOverride != null)
			{
				bool? r =	EnvironmentOverride(cmd, data);
				if (r.HasValue)	return r.Value;
			}

			// Mask off the experimental / private flags so a normal switch matches.
			uint baseCmd =	cmd & ~(RETRO_ENVIRONMENT_EXPERIMENTAL | RETRO_ENVIRONMENT_PRIVATE);

			switch (baseCmd)
			{
				case RETRO_ENVIRONMENT_GET_OVERSCAN:
					if (data != IntPtr.Zero)	Marshal.WriteByte(data, 1);
					return true;

				case RETRO_ENVIRONMENT_GET_CAN_DUPE:
					if (data != IntPtr.Zero)	Marshal.WriteByte(data, 1);
					return true;

				case RETRO_ENVIRONMENT_SET_PIXEL_FORMAT:
					if (data == IntPtr.Zero)	return false;
					int fmt =	Marshal.ReadInt32(data);
					// Accept any of the three documented formats — caller maps.
					if (fmt == RETRO_PIXEL_FORMAT_XRGB8888
						|| fmt == RETRO_PIXEL_FORMAT_RGB565
						|| fmt == RETRO_PIXEL_FORMAT_0RGB1555)
					{
						PixelFormat =	fmt;
						return true;
					}
					return false;

				case RETRO_ENVIRONMENT_GET_SYSTEM_DIRECTORY:
					return WriteCachedPathPtr(data, SystemDirectory, ref _systemDirHandle);

				case RETRO_ENVIRONMENT_GET_SAVE_DIRECTORY:
					return WriteCachedPathPtr(data, SaveDirectory, ref _saveDirHandle);

				// RETRO_ENVIRONMENT_GET_LOG_INTERFACE intentionally NOT
				// handled (returns false via the default branch). Stella's
				// libretro.h declares the callback as variadic
				// (`void log(level, fmt, ...)`) and the shim makes both
				// 2-arg (no varargs) and 3-arg (one %s + char* arg) calls.
				// A managed delegate with a fixed signature can't safely
				// receive the 2-arg form: marshalling reads the 3rd
				// register slot regardless, and dereferencing the garbage
				// pointer that's in it (e.g. via PtrToStringAnsi) walks
				// invalid memory. So we let Stella's own fallback_log run
				// instead — verbose, but stable. The price is the
				// AudioQueue INFO chatter showing up on stderr.

				case RETRO_ENVIRONMENT_SHUTDOWN:
					return true;

				case RETRO_ENVIRONMENT_SET_PERFORMANCE_LEVEL:
					return true;

				case RETRO_ENVIRONMENT_SET_VARIABLES:
				case RETRO_ENVIRONMENT_GET_VARIABLE:
				case RETRO_ENVIRONMENT_GET_VARIABLE_UPDATE:
					// Treat all variables as unset / no updates. Most cores
					// fall back to default values when they get this answer.
					return false;

				case RETRO_ENVIRONMENT_SET_MESSAGE:
					return true;

				default:
					return false;
			}
		}

		// Pin a UTF-8 byte buffer for the path and write its pointer to *data.
		private static bool WriteCachedPathPtr(IntPtr data, string path, ref GCHandle handle)
		{
			if (data == IntPtr.Zero)	return false;
			if (string.IsNullOrEmpty(path))
			{
				Marshal.WriteIntPtr(data, IntPtr.Zero);
				return true;
			}
			if (!handle.IsAllocated)
			{
				var bytes =	System.Text.Encoding.UTF8.GetBytes(path + "\0");
				handle =	GCHandle.Alloc(bytes, GCHandleType.Pinned);
			}
			Marshal.WriteIntPtr(data, handle.AddrOfPinnedObject());
			return true;
		}

		private void HandleVideoRefresh(IntPtr data, uint width, uint height, UIntPtr pitch)
		{
			OnVideoRefresh?.Invoke(data, width, height, (uint)pitch.ToUInt64());
		}

		private void HandleAudioSample(short left, short right)	=>	OnAudioSample?.Invoke(left, right);

		private UIntPtr HandleAudioBatch(IntPtr data, UIntPtr frames)	=>
			(UIntPtr)(OnAudioBatch?.Invoke(data, frames.ToUInt64()) ?? frames.ToUInt64());

		private void HandleInputPoll()	=>	OnInputPoll?.Invoke();

		private short HandleInputState(uint port, uint device, uint index, uint id)	=>
			OnInputState?.Invoke(port, device, index, id) ?? 0;
	}
}
