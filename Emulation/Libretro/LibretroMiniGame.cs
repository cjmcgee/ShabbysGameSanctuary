using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
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
	/// Audio is intentionally dropped (silent) for this v1 — most cores still
	/// run correctly without an audio sink. Adding audio means feeding samples
	/// through MonoGame's <c>DynamicSoundEffectInstance</c>; out of scope for
	/// the first cut.
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

		// ── IEmbeddedMiniGame ────────────────────────────────────────────────
		public string Title { get; }
		public Point NativeResolution =>	_frameWidth > 0
			? new Point((int)_frameWidth, (int)_frameHeight)
			:	new Point(160, 192);	// Atari 2600 default before first frame
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
				// Audio is silently consumed for v1 — no MonoGame audio sink
				// is hooked up. We still ACK every batch so the core's
				// `frames consumed` accounting is satisfied; otherwise some
				// cores stall or log overflow chatter. The samples are
				// dropped on the floor.
				OnAudioSample =	(l, r) => { _ = l; _ = r; },
				OnAudioBatch =	(_, frames) => frames,
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
		}

		public void Update(GameTime gameTime, IMiniGameInput input)
		{
			if (input.ExitRequested)	{ _finished =	true; return; }
			MapKeyboardToJoypad(input);
			_core.Run();
		}

		public void Draw(SpriteBatch spriteBatch, RectangleF viewport)
		{
			if (_frameDirty)	UploadFrame();

			if (_frameTexture == null)	return;

			var fit =	LetterboxFit(new Point(_texWidth, _texHeight),
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
			if (input.IsFireDown(0))	_joypad[RETRO_DEVICE_ID_JOYPAD_A] =	true;
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
