using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TileEngine.Rendering;
using static TileEngine.MiniGames.Libretro.LibretroNative;

namespace TileEngine.MiniGames.Libretro
{
	/// <summary>
	/// Adapter that lets any libretro core run inside the same
	/// <see cref="IEmbeddedMiniGame"/> slot the native mini-games use
	/// (e.g. Battleshoot). Loads the core, loads a ROM, then on each
	/// <see cref="Update"/> pumps one libretro frame, captures the framebuffer
	/// into a Texture2D, and on <see cref="Draw"/> blits that texture into
	/// the host viewport.
	///
	/// Audio is piped through MonoGame's <see cref="DynamicSoundEffectInstance"/>
	/// at the core's advertised sample rate. Stella reports 31440 Hz stereo
	/// (mono samples duplicated to L/R), which the audio device accepts directly.
	/// </summary>
	public sealed class LibretroMiniGame :	IEmbeddedMiniGame, IDisposable
	{
		private readonly string _corePath;
		private readonly string _romPath;
		private readonly string _systemDirectory;

		private LibretroCore _core =	null!;
		private GraphicsDevice _gd =	null!;

		// Latest frame metadata captured during the env callback.
		private uint _frameWidth;
		private uint _frameHeight;
		private uint _framePitchBytes;
		private byte[]	_frameBytes =	Array.Empty<byte>();
		private bool _frameDirty;

		private Texture2D?	_frameTexture;
		private int _texWidth;
		private int _texHeight;

		// Latest input state, refreshed by KeyboardMiniGameInput each tick.
		private bool[]	_joypad =	new bool[16];

		private bool _finished;
		private bool _disposed;

		// Time-since-last-Run accumulator. Decoupled from the host's render
		// rate so a 144 Hz monitor doesn't run a 60 Hz NTSC core at 2.4× speed.
		private double _runAccumulator;

		// Audio pipeline. Created in Initialize once the core's sample rate is
		// known. The callback marshals samples on whatever thread retro_run
		// is running on — DynamicSoundEffectInstance.SubmitBuffer is safe to
		// call from any thread.
		private DynamicSoundEffectInstance?	_audio;
		private byte[]	_audioBuf =	Array.Empty<byte>();

		// ── IEmbeddedMiniGame ────────────────────────────────────────────────
		public string Title { get; }

		/// <summary>
		/// The logical display size used for letterboxing. NOT the framebuffer
		/// size — for systems with non-square pixels they differ. Stella, for
		/// instance, hands us a 160-wide framebuffer where each pixel is meant
		/// to be drawn double-wide (TIA pixels are 2:1), so the framebuffer is
		/// 160×228 but the rendered display is 320×228 (close to 4:3). The
		/// core's retro_get_system_av_info reports the right display size as
		/// base_width × base_height — we cache those in LibretroCore. Falling
		/// back to the raw framebuffer dims would render Atari games at ~2:3,
		/// way too tall.
		/// </summary>
		public Point NativeResolution
		{
			get
			{
				if (_core != null && _core.BaseWidth > 0 && _core.BaseHeight > 0)
					return new Point((int)_core.BaseWidth, (int)_core.BaseHeight);
				if (_frameWidth > 0)
					return new Point((int)_frameWidth, (int)_frameHeight);
				return new Point(320, 192);	// Atari 2600 visible area, 4:3
			}
		}
		public bool IsFinished =>	_finished;

		public LibretroMiniGame(string corePath, string romPath, string title = "", string systemDirectory = "")
		{
			_corePath =	corePath;
			_romPath =	romPath;
			_systemDirectory =	systemDirectory;
			Title =	string.IsNullOrEmpty(title)
				?	System.IO.Path.GetFileNameWithoutExtension(romPath)
				:	title;
		}

		public void Initialize(GraphicsDevice graphicsDevice, ContentManager content)
		{
			_gd =	graphicsDevice;
			_core =	new LibretroCore
			{
				SystemDirectory =	_systemDirectory,
				OnVideoRefresh =	OnVideoRefresh,
				OnInputPoll =		OnInputPoll,
				OnInputState =		OnInputState,
				OnAudioSample =		OnAudioSample,
				OnAudioBatch =		OnAudioBatch,
			};
			_core.Open(_corePath);
			_core.Init();
			if (!_core.LoadGame(_romPath))
				throw new InvalidOperationException(
					$"Libretro core '{_corePath}' refused to load ROM '{_romPath}'.");

			// Pre-size the frame buffer to the maximum the core advertises so
			// the first few frames don't trigger reallocations.
			int maxBytes =	(int)(_core.MaxWidth * _core.MaxHeight * 4);
			if (maxBytes > 0)	_frameBytes =	new byte[maxBytes];

			// Audio sink. SampleRate comes from retro_get_system_av_info, so it's
			// known by the time we land here. DynamicSoundEffectInstance accepts
			// non-standard rates (Stella is 31440 Hz). Play immediately — the
			// device idles silently until SubmitBuffer is called.
			int sr =	_core.SampleRate > 0 ? (int)_core.SampleRate :	44100;
			try
			{
				_audio =	new DynamicSoundEffectInstance(sr, AudioChannels.Stereo);
				_audio.Play();
			}
			catch (Exception ex)
			{
				// Some hosts refuse weird sample rates. Falling back to silent
				// is better than crashing the whole mini-game.
				Console.Error.WriteLine($"[LibretroMiniGame] audio init failed: {ex.Message} — running silent.");
				_audio =	null;
			}
		}

		public void Update(GameTime gameTime, IMiniGameInput input)
		{
			if (input.ExitRequested)	{ _finished =	true; return; }
			MapKeyboardToJoypad(input);

			// Run the core at its advertised frame rate, not the host's render
			// rate. Cap catch-up at 3 frames per host tick — if we fall further
			// behind we drop the surplus rather than spiral-of-death the CPU.
			double targetFrameSeconds =	_core.Fps > 0 ? 1.0 / _core.Fps :	1.0 / 60.0;
			_runAccumulator +=	gameTime.ElapsedGameTime.TotalSeconds;
			int runs =	0;
			while (_runAccumulator >= targetFrameSeconds && runs < 3)
			{
				_core.Run();
				_runAccumulator -=	targetFrameSeconds;
				runs++;
			}
			if (_runAccumulator > targetFrameSeconds * 4)	_runAccumulator =	0;
		}

		public void Draw(SpriteBatch spriteBatch, RectangleF viewport)
		{
			if (_frameDirty)	UploadFrame();

			if (_frameTexture == null)	return;

			// Letterbox by the LOGICAL display size (NativeResolution), not
			// the raw framebuffer dims. The texture stretches horizontally to
			// match — for Atari that means the 160×228 framebuffer is drawn
			// into a 320×228-aspect destination rectangle.
			var fit =	LetterboxFit(NativeResolution,
				new Point((int)viewport.Width, (int)viewport.Height));
			var dst =	new Rectangle(
				(int)(viewport.X + fit.X),
				(int)(viewport.Y + fit.Y),
				(int)fit.Width,
				(int)fit.Height);

			spriteBatch.Draw(_frameTexture, dst,
				new Rectangle(0, 0, (int)_frameWidth, (int)_frameHeight),
				Color.White);
		}

		public void Shutdown()	=>	Dispose();

		public void Dispose()
		{
			if (_disposed)	return;
			_disposed =	true;
			if (_audio != null)
			{
				try		{ _audio.Stop(); }
				catch	{ }
				_audio.Dispose();
				_audio =	null;
			}
			_frameTexture?.Dispose();
			_core?.Dispose();
		}

		// ── Video ───────────────────────────────────────────────────────────
		// Called by the core during retro_run. We copy out of the core's
		// buffer because it may reuse that memory before our next Draw().
		private void OnVideoRefresh(IntPtr data, uint width, uint height, uint pitchBytes)
		{
			if (data == IntPtr.Zero || width == 0 || height == 0)	return;

			int needed =	(int)(pitchBytes * height);
			if (_frameBytes.Length < needed)	_frameBytes =	new byte[needed];

			Marshal.Copy(data, _frameBytes, 0, needed);
			_frameWidth =	width;
			_frameHeight =	height;
			_framePitchBytes =	pitchBytes;
			_frameDirty =	true;
		}

		// Convert the captured raw buffer into XNA Color[] and upload to
		// the texture (creating/resizing as needed).
		private void UploadFrame()
		{
			_frameDirty =	false;
			int w =	(int)_frameWidth;
			int h =	(int)_frameHeight;
			if (_frameTexture == null || _texWidth != w || _texHeight != h)
			{
				_frameTexture?.Dispose();
				_frameTexture =	new Texture2D(_gd, w, h, false, SurfaceFormat.Color);
				_texWidth =	w;
				_texHeight =	h;
			}

			var pixels =	new Color[w * h];
			int pitch =	(int)_framePitchBytes;
			switch (_core.PixelFormat)
			{
				case RETRO_PIXEL_FORMAT_XRGB8888:
					ConvertXrgb8888(_frameBytes, pitch, w, h, pixels);
					break;
				case RETRO_PIXEL_FORMAT_RGB565:
					ConvertRgb565(_frameBytes, pitch, w, h, pixels);
					break;
				case RETRO_PIXEL_FORMAT_0RGB1555:
				default:
					ConvertRgb1555(_frameBytes, pitch, w, h, pixels);
					break;
			}

			_frameTexture.SetData(pixels);
		}

		private static void ConvertXrgb8888(byte[] src, int pitch, int w, int h, Color[] dst)
		{
			// Source bytes are little-endian XRGB8888 → 0xAARRGGBB-ish in memory:
			// byte 0 = B, byte 1 = G, byte 2 = R, byte 3 = X. We discard X.
			for (int y = 0; y < h; y++)
			{
				int srcRow =	y * pitch;
				int dstRow =	y * w;
				for (int x = 0; x < w; x++)
				{
					int s =	srcRow + x * 4;
					byte b =	src[s + 0];
					byte g =	src[s + 1];
					byte r =	src[s + 2];
					dst[dstRow + x]	=	new Color(r, g, b);
				}
			}
		}

		private static void ConvertRgb565(byte[] src, int pitch, int w, int h, Color[] dst)
		{
			for (int y = 0; y < h; y++)
			{
				int srcRow =	y * pitch;
				int dstRow =	y * w;
				for (int x = 0; x < w; x++)
				{
					int s =	srcRow + x * 2;
					ushort px =	(ushort)(src[s + 0] | (src[s + 1] << 8));
					byte r =	(byte)(((px >> 11) & 0x1F) << 3);
					byte g =	(byte)(((px >>  5) & 0x3F) << 2);
					byte b =	(byte)(((px      ) & 0x1F) << 3);
					dst[dstRow + x]	=	new Color(r, g, b);
				}
			}
		}

		private static void ConvertRgb1555(byte[] src, int pitch, int w, int h, Color[] dst)
		{
			for (int y = 0; y < h; y++)
			{
				int srcRow =	y * pitch;
				int dstRow =	y * w;
				for (int x = 0; x < w; x++)
				{
					int s =	srcRow + x * 2;
					ushort px =	(ushort)(src[s + 0] | (src[s + 1] << 8));
					byte r =	(byte)(((px >> 10) & 0x1F) << 3);
					byte g =	(byte)(((px >>  5) & 0x1F) << 3);
					byte b =	(byte)(((px      ) & 0x1F) << 3);
					dst[dstRow + x]	=	new Color(r, g, b);
				}
			}
		}

		// ── Audio ───────────────────────────────────────────────────────────
		// Stella uses the batch path exclusively (one call per retro_run with
		// the frame's worth of samples), but we wire the single-sample path
		// too so other cores work without modification.

		private void OnAudioSample(short left, short right)
		{
			if (_audio == null)	return;
			Span<byte> frame =	stackalloc byte[4];
			frame[0] = (byte)(left  & 0xFF);
			frame[1] = (byte)((left  >> 8) & 0xFF);
			frame[2] = (byte)(right & 0xFF);
			frame[3] = (byte)((right >> 8) & 0xFF);
			// SubmitBuffer needs a byte[]; stackalloc is just for clarity.
			_audio.SubmitBuffer(frame.ToArray());
		}

		private ulong OnAudioBatch(IntPtr data, ulong frames)
		{
			if (_audio == null || data == IntPtr.Zero || frames == 0)	return frames;

			// Stereo 16-bit interleaved → 4 bytes per stereo frame.
			int bytes =	checked((int)(frames * 4UL));
			if (_audioBuf.Length < bytes)	_audioBuf =	new byte[bytes];
			Marshal.Copy(data, _audioBuf, 0, bytes);

			// If we've already buffered a lot of audio (host is lagging or the
			// core handed us a burst), drop this batch rather than pile up
			// latency. ~6 frames at 60 Hz ≈ 100 ms — past that the lip-sync
			// gets unpleasant.
			if (_audio.PendingBufferCount < 6)
				_audio.SubmitBuffer(_audioBuf, 0, bytes);

			return frames;
		}

		// ── Input ───────────────────────────────────────────────────────────

		// Update the joypad state from the host's logical input each frame.
		// Mapping: WASD/arrows → directions, Space → Joypad A (fire),
		// Enter → Start, Tab → Select.
		private void MapKeyboardToJoypad(IMiniGameInput input)
		{
			Array.Clear(_joypad, 0, _joypad.Length);
			var dir =	input.GetDirection(0);
			if (dir.X < -0.5f)	_joypad[RETRO_DEVICE_ID_JOYPAD_LEFT]  =	true;
			if (dir.X >  0.5f)	_joypad[RETRO_DEVICE_ID_JOYPAD_RIGHT] =	true;
			if (dir.Y < -0.5f)	_joypad[RETRO_DEVICE_ID_JOYPAD_UP]    =	true;
			if (dir.Y >  0.5f)	_joypad[RETRO_DEVICE_ID_JOYPAD_DOWN]  =	true;
			// Stella maps the Atari joystick's red fire button to JOYPAD_B
			// (not A — A is "Fire5", a Genesis-pad extension).
			if (input.IsFireDown(0))	_joypad[RETRO_DEVICE_ID_JOYPAD_B] =	true;
			if (input.IsKeyDown(Keys.Enter))	_joypad[RETRO_DEVICE_ID_JOYPAD_START]  =	true;
			if (input.IsKeyDown(Keys.Tab))      _joypad[RETRO_DEVICE_ID_JOYPAD_SELECT] =	true;
		}

		private void OnInputPoll()
		{
			// All input state is kept in _joypad; nothing to do here. Cores
			// call this once per retro_run() before any input_state queries.
		}

		private short OnInputState(uint port, uint device, uint index, uint id)
		{
			if (port != 0)	return 0;	// Only player 1 is wired up.
			if (device != RETRO_DEVICE_JOYPAD)	return 0;
			if (id >= _joypad.Length)	return 0;
			return (short)(_joypad[id]	? 1 :	0);
		}

		// ── Letterboxing helper ─────────────────────────────────────────────
		private static RectangleF LetterboxFit(Point native, Point container)
		{
			float scale =	MathF.Min(container.X / (float)native.X, container.Y / (float)native.Y);
			float w =	native.X * scale;
			float h =	native.Y * scale;
			float x =	(container.X - w)	* 0.5f;
			float y =	(container.Y - h)	* 0.5f;
			return new RectangleF(x, y, w, h);
		}
	}
}
