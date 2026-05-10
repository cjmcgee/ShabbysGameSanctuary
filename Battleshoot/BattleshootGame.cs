using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TileEngine.MiniGames;
using TileEngine.Rendering;

namespace Battleshoot;

/// <summary>
/// "Battleshoot" — a faithful homage to Atari 2600 Combat (1977, Larry Wagner).
///
/// Recreates Combat's signature mechanics from the decompiled ROM
/// ("Combat (decomp).asm" in Notes/src):
///   • <b>Sprite</b>: the 8 stored rotation frames from <c>TankShape</c>, plus
///     180° flips (TIA <c>REFP0</c> + reverse byte iteration) to fill 16
///     directions at 22.5° each.
///   • <b>Movement</b>: 16-direction <c>DIRECTN</c>, forward-only thrust
///     (Combat's tank joystick has UP=thrust, LEFT/RIGHT=turn, DOWN does
///     nothing — see <c>CTRLTBL</c> in the ROM). Turns are paced by
///     <c>TurnTimer</c> at $0F frames per 22.5° step.
///   • <b>Map</b>: the Complex Maze decoded directly from <c>PF0_0</c>,
///     <c>PF1_0</c>, <c>PF2_0</c> in the ROM, with TIA's horizontal
///     reflection (REFLECT bit in <c>CTRLPF</c>) and Combat's kernel
///     vertical reflection around scanline $80.
///   • <b>Stir on hit</b>: the loser's tank rotates while the winner pauses
///     (Combat's <c>StirTimer</c> + <c>RushTank</c> behaviour, simplified).
///
/// Aspect: every Atari logical pixel is two screen pixels wide on a real
/// display. The internal 160×192 playfield is shown as 320×192 square pixels;
/// the Draw routine handles the stretch.
///
/// Implements <see cref="IEmbeddedMiniGame"/> so this same class runs either
/// in <see cref="StandaloneHost"/> (Battleshoot.exe) or inside ChildhoodAdventure
/// as a <see cref="TileEngine.MiniGames.MiniGameScene"/>.
/// </summary>
public sealed class BattleshootGame :	IEmbeddedMiniGame
{
	// ── Native NTSC playfield (Atari logical pixels) ─────────────────────────
	public const int FieldWidth	=	160;
	public const int FieldHeight	=	192;
	public const int PixelAspectXMultiplier	=	2;
	public const int DisplayWidth	=	FieldWidth * PixelAspectXMultiplier;	// 320

	private const int HudHeight =	16;
	private const int PlayHeight =	FieldHeight - HudHeight;

	// ── Combat constants (ported from ROM) ──────────────────────────────────
	private const int TankSpriteW =	8;	// tank shape is 8 bits wide
	private const int TankSpriteH =	16;	// 8 stored rows × 2 scanlines per row
	private const int TankCollisionH =	12;	// forgive 2 px top + 2 px bottom

	// Per-frame motion (60 fps target). Combat ran ~60Hz; HMPx offsets give
	// ~1–3 pixel/frame depending on diagonal/axis-aligned.
	private const float TankSpeedPerFrame =	1.0f;	// ~60 logical px/sec forward
	private const float BulletSpeedPerFrame =	2.5f;	// ~150 logical px/sec
	private const int TurnFrames =	10;	// TurnTimer init in ROM = $0F=15. 10 feels less sluggish.
	private const int MisLifeFrames =	63;	// MisLife init in ROM = $3F
	private const int FireCooldownFrames =	8;
	private const int ScoreToWin =	9;	// single BCD digit, matches score display
	private const int StirFrames =	36;	// ~0.6s loser-spin before reset
	private const int WinHoldFrames =	150;	// ~2.5s win banner before exit

	// ── Colours (Atari NTSC palette samples — tank-mode red & cream) ────────
	private static readonly Color BgColor =	new(162, 78, 44);
	private static readonly Color WallColor =	new(228, 188, 110);
	private static readonly Color HudColor =	new(20, 20, 20);
	private static readonly Color HudText =	new(236, 236, 236);
	private static readonly Color P1Color =	new(212, 80, 60);		// orange-red tank
	private static readonly Color P2Color =	new(80, 156, 220);	// blue tank
	private static readonly Color BulletColor =	new(252, 224, 168);

	// ── Combat ROM tank sprite ($F64F..$F68E) ───────────────────────────────
	// 8 stored frames × 8 bytes; bit 7 = leftmost pixel. Each byte renders
	// for 2 scanlines, so the visible sprite is 8×16. Frame 0 faces East;
	// frames advance 22.5° counter-clockwise. Directions 8..15 are computed
	// at draw time by 180°-rotating frames 0..7 (horizontal flip + reverse
	// byte order) — exactly what the ROM's <c>ROT</c> routine does via
	// <c>REFP0</c> and the carry-flag-controlled DEY/INY trick.
	private static readonly byte[][]	TankFrames =	new[]
	{
		new byte[] { 0x00, 0xFC, 0xFC, 0x38, 0x3F, 0x38, 0xFC, 0xFC },	// 0  East
		new byte[] { 0x1C, 0x78, 0xFB, 0x7C, 0x1C, 0x1F, 0x3E, 0x18 },	// 1  ENE
		new byte[] { 0x19, 0x3A, 0x7C, 0xFF, 0xDF, 0x0E, 0x1C, 0x18 },	// 2  NE
		new byte[] { 0x24, 0x64, 0x79, 0xFF, 0xFF, 0x4E, 0x0E, 0x04 },	// 3  NNE
		new byte[] { 0x08, 0x08, 0x6B, 0x7F, 0x7F, 0x7F, 0x63, 0x63 },	// 4  North
		new byte[] { 0x24, 0x26, 0x9E, 0xFF, 0xFF, 0x72, 0x70, 0x20 },	// 5  NNW
		new byte[] { 0x98, 0x5C, 0x3E, 0xFF, 0xFB, 0x70, 0x38, 0x18 },	// 6  NW
		new byte[] { 0x38, 0x1E, 0xDF, 0x3E, 0x38, 0xF8, 0x7C, 0x18 },	// 7  WNW
	};

	// ── Combat ROM Complex Maze: PF0_0, PF1_0, PF2_0 ────────────────────────
	// 11 rows (the ROM has 12; we drop the solid-border row 0 so the top of
	// the playfield is open — the rest of the maze, including the left/right
	// edge "spines" generated by PF0=$10, stays). Each row is 8 scanlines.
	// TIA semantics for decoding into 20 cells:
	//   PF0 high nibble (bits 4..7) → cells 0..3, MSB-first
	//   PF1 all 8 bits (bits 7..0) → cells 4..11, MSB-first
	//   PF2 all 8 bits (bits 0..7) → cells 12..19, LSB-first (reversed)
	// Then the right half (cells 20..39) is left half mirrored, courtesy of
	// CTRLPF's REFLECT bit. The kernel also vertically mirrors the field
	// around scanline $80; we replicate that by drawing each row twice.
	private static readonly (byte pf0, byte pf1, byte pf2)[]	MazePfData =
	{
		( 0x10, 0x00, 0x80 ),	// PF row 1 (Combat $F77A/$F786/$F792)
		( 0x10, 0x00, 0x80 ),	// PF row 2
		( 0x10, 0x00, 0x00 ),	// PF row 3
		( 0x10, 0x38, 0x00 ),	// PF row 4
		( 0x10, 0x00, 0x00 ),	// PF row 5
		( 0x10, 0x00, 0x1C ),	// PF row 6
		( 0x10, 0x00, 0x04 ),	// PF row 7
		( 0x10, 0x60, 0x00 ),	// PF row 8
		( 0x10, 0x20, 0x00 ),	// PF row 9
		( 0x10, 0x20, 0x00 ),	// PF row 10
		( 0x10, 0x23, 0x00 ),	// PF row 11
	};

	// ── State ────────────────────────────────────────────────────────────────
	private GraphicsDevice _gd =	null!;
	private Texture2D _pixel =	null!;

	private Tank _p1 =	null!;
	private Tank _p2 =	null!;
	private readonly List<Bullet>	_bullets =	new();
	private readonly List<Rectangle>	_walls =	new();
	private int _stirTimer;
	private Tank?	_stirLoser;
	private int _winner;
	private int _winHoldTimer;
	private bool _finished;

	// ── IEmbeddedMiniGame ────────────────────────────────────────────────────
	public string Title =>	"Battleshoot";
	public Point NativeResolution =>	new(DisplayWidth, FieldHeight);
	public bool IsFinished =>	_finished;

	public void Initialize(GraphicsDevice gd, ContentManager content)
	{
		_gd =	gd;
		_pixel =	new Texture2D(_gd, 1, 1);
		_pixel.SetData(new[] { Color.White });
		BuildMazeFromPfData();
		ResetRound(initial:	true);
	}

	public void Update(GameTime gameTime, IMiniGameInput input)
	{
		if (input.ExitRequested)	{ _finished =	true; return; }

		if (_winner != 0)
		{
			if (--_winHoldTimer <= 0)	_finished =	true;
			return;
		}

		if (_stirTimer > 0)
		{
			UpdateStir();
			return;
		}

		UpdatePlayer(_p1, input.GetDirection(0), input.IsFireDown(0));
		UpdateAi(_p2, _p1);
		UpdateBullets();
		ResolveCollisions();
		CheckWin();
	}

	// ── Player / AI control ──────────────────────────────────────────────────

	/// <summary>
	/// Combat's tank joystick model (see <c>CTRLTBL</c> in the ROM):
	///   UP = thrust forward at current heading
	///   LEFT  = rotate +22.5° (counter-clockwise)
	///   RIGHT = rotate -22.5° (clockwise)
	///   DOWN  = nothing (no reverse)
	/// Turns are paced by <see cref="Tank.TurnTimer"/> so you can't spin in place.
	/// </summary>
	private void UpdatePlayer(Tank t, Vector2 input, bool fire)
	{
		if (t.TurnTimer > 0)	t.TurnTimer--;
		else
		{
			if (input.X < -0.5f)		{ t.Direction =	(t.Direction + 1)	& 15; t.TurnTimer =	TurnFrames; }
			else if (input.X > 0.5f)	{ t.Direction =	(t.Direction + 15)	& 15; t.TurnTimer =	TurnFrames; }
		}

		if (input.Y < -0.5f)	TryThrust(t, TankSpeedPerFrame);

		if (t.FireCooldown > 0)	t.FireCooldown--;
		if (fire && t.FireCooldown == 0 && _bullets.All(b => b.Owner != t))
			FireBullet(t);
	}

	/// <summary>
	/// Simple AI: rotate toward target along the shorter arc, thrust forward
	/// continuously, fire whenever cannon is roughly aligned.
	/// </summary>
	private void UpdateAi(Tank t, Tank target)
	{
		var to =	target.Position - t.Position;
		if (to.LengthSquared() < 1f)	return;

		// Snap angle to nearest 22.5° step. Screen-space Y is down, so negate.
		float angle =	MathF.Atan2(-to.Y, to.X);
		int desired =	(int)Math.Round(angle / (MathF.PI / 8f));
		desired =	((desired % 16)	+ 16)	& 15;

		if (t.TurnTimer > 0)	t.TurnTimer--;
		else if (t.Direction != desired)
		{
			int cwSteps =	(t.Direction - desired + 16)	& 15;
			int ccwSteps =	(desired - t.Direction + 16)	& 15;
			t.Direction =	ccwSteps <= cwSteps
				? (t.Direction + 1)	& 15
				: (t.Direction + 15)	& 15;
			t.TurnTimer =	TurnFrames;
		}

		TryThrust(t, TankSpeedPerFrame * 0.85f);

		if (t.FireCooldown > 0)	t.FireCooldown--;
		int gap =	Math.Min((t.Direction - desired + 16) & 15, (desired - t.Direction + 16) & 15);
		if (gap <= 1 && t.FireCooldown == 0 && _bullets.All(b => b.Owner != t))
			FireBullet(t);
	}

	private void TryThrust(Tank t, float distance)
	{
		var v =	DirVector(t.Direction)	* distance;
		var attempt =	t.Position + v;
		if (!CollidesWithWalls(attempt, TankSpriteW, TankCollisionH))
		{
			t.Position =	attempt;
			return;
		}
		// Slide on the cleaner axis so tanks don't fully stick on corners.
		var ax =	new Vector2(attempt.X, t.Position.Y);
		if (!CollidesWithWalls(ax, TankSpriteW, TankCollisionH))	{ t.Position =	ax; return; }
		var ay =	new Vector2(t.Position.X, attempt.Y);
		if (!CollidesWithWalls(ay, TankSpriteW, TankCollisionH))	{ t.Position =	ay; }
	}

	private void FireBullet(Tank t)
	{
		var v =	DirVector(t.Direction);
		_bullets.Add(new Bullet
		{
			Owner =	t,
			Position =	t.Position + v * (TankSpriteW * 0.5f + 2),
			Velocity =	v * BulletSpeedPerFrame,
			LifeFrames =	MisLifeFrames,
		});
		t.FireCooldown =	FireCooldownFrames;
	}

	// ── Bullets / collisions ────────────────────────────────────────────────
	private void UpdateBullets()
	{
		for (int i = _bullets.Count - 1; i >= 0; i--)
		{
			var b =	_bullets[i];
			b.Position +=	b.Velocity;
			b.LifeFrames--;
			_bullets[i]	=	b;
			if (b.LifeFrames <= 0 || CollidesWithWalls(b.Position, 3, 3))
				_bullets.RemoveAt(i);
		}
	}

	private void ResolveCollisions()
	{
		for (int i = _bullets.Count - 1; i >= 0; i--)
		{
			var b =	_bullets[i];
			Tank?	hit =	null;
			if (b.Owner != _p1 && HitsTank(b, _p1))	hit =	_p1;
			else if (b.Owner != _p2 && HitsTank(b, _p2))	hit =	_p2;
			if (hit != null)
			{
				_bullets.RemoveAt(i);
				if (hit == _p1)	_p2.Score++;
				else _p1.Score++;
				_stirLoser =	hit;
				_stirTimer =	StirFrames;
				return;
			}
		}
	}

	private void UpdateStir()
	{
		_stirTimer--;
		if (_stirLoser != null)
			_stirLoser.Direction =	(_stirLoser.Direction + 1)	& 15;
		if (_stirTimer <= 0)
		{
			_stirLoser =	null;
			ResetRound(initial:	false);
		}
	}

	private static bool HitsTank(Bullet b, Tank t)
	{
		var d =	b.Position - t.Position;
		return Math.Abs(d.X) < TankSpriteW * 0.5f
			&& Math.Abs(d.Y) < TankCollisionH * 0.5f;
	}

	private bool CollidesWithWalls(Vector2 pos, float w, float h)
	{
		var box =	new Rectangle(
			(int)(pos.X - w * 0.5f), (int)(pos.Y - h * 0.5f),
			(int)w, (int)h);
		if (box.X < 0 || box.Y < HudHeight
			|| box.X + box.Width  > FieldWidth
			|| box.Y + box.Height > FieldHeight)	return true;
		foreach (var wall in _walls)
			if (wall.Intersects(box))	return true;
		return false;
	}

	// ── 16-direction unit vectors ───────────────────────────────────────────
	private static Vector2 DirVector(int dir)
	{
		// dir 0 = East (+X). Each +1 = +22.5° CCW (in math). Screen Y is down.
		float a =	dir * (MathF.PI / 8f);
		return new Vector2(MathF.Cos(a), -MathF.Sin(a));
	}

	// ── Maze decoding (PF0/PF1/PF2 → wall rectangles) ───────────────────────
	private void BuildMazeFromPfData()
	{
		_walls.Clear();
		int rowCount =	MazePfData.Length;
		// Top half: rows 0..rowCount-1 at y=HudHeight + r*8.
		// Bottom half: mirrors of those rows at y=HudHeight + (2N-1-r)*8.
		// With rowCount=11, total 22 rows × 8 = 176 px = the play area.
		for (int r = 0; r < rowCount; r++)
		{
			var (pf0, pf1, pf2) =	MazePfData[r];
			AddRowRectangles(pf0, pf1, pf2, HudHeight + r * 8);
			AddRowRectangles(pf0, pf1, pf2, HudHeight + (2 * rowCount - 1 - r) * 8);
		}
	}

	private void AddRowRectangles(byte pf0, byte pf1, byte pf2, int y)
	{
		bool[]	cells =	new bool[40];
		// PF0 high nibble, bit 4 = leftmost
		for (int i = 0; i < 4; i++)
			cells[i]	=	(pf0 & (1 << (4 + i))) != 0;
		// PF1 MSB-first
		for (int i = 0; i < 8; i++)
			cells[4 + i]	=	(pf1 & (1 << (7 - i))) != 0;
		// PF2 LSB-first
		for (int i = 0; i < 8; i++)
			cells[12 + i]	=	(pf2 & (1 << i)) != 0;
		// Right half mirrors left (TIA REFLECT on)
		for (int c = 20; c < 40; c++)
			cells[c]	=	cells[39 - c];
		// Emit contiguous runs as merged rectangles (fewer collision checks)
		int run =	0;
		for (int c = 0; c <= 40; c++)
		{
			bool on =	c < 40 && cells[c];
			if (on)	run++;
			else if (run > 0)
			{
				int startCol =	c - run;
				_walls.Add(new Rectangle(startCol * 4, y, run * 4, 8));
				run =	0;
			}
		}
	}

	// ── Reset ───────────────────────────────────────────────────────────────
	private void ResetRound(bool initial)
	{
		_bullets.Clear();
		_stirTimer =	0;
		_stirLoser =	null;

		_p1 ??=	new Tank();
		_p2 ??=	new Tank();

		// Spawn in the open lanes that Combat's complex maze leaves at the
		// top and bottom of the field. P1 faces East (dir 0), P2 faces West
		// (dir 8).
		_p1.Position =	new Vector2(14, HudHeight + 18);
		_p1.Direction =	0;
		_p1.TurnTimer =	0;
		_p1.FireCooldown =	0;

		_p2.Position =	new Vector2(FieldWidth - 14, FieldHeight - 18);
		_p2.Direction =	8;
		_p2.TurnTimer =	0;
		_p2.FireCooldown =	0;

		if (initial)
		{
			_p1.Score =	0;
			_p2.Score =	0;
			_winner =	0;
		}
	}

	private void CheckWin()
	{
		if (_p1.Score >= ScoreToWin)	{ _winner =	1; _winHoldTimer =	WinHoldFrames; }
		else if (_p2.Score >= ScoreToWin)	{ _winner =	2; _winHoldTimer =	WinHoldFrames; }
	}

	// ── Drawing ─────────────────────────────────────────────────────────────
	public void Draw(SpriteBatch sb, RectangleF viewport)
	{
		// 320×192 native, fit-to-viewport with letterbox, X stretched 2× to
		// honour the Atari's double-wide pixels.
		float scale =	MathF.Min(viewport.Width / DisplayWidth, viewport.Height / FieldHeight);
		float ox =	viewport.X + (viewport.Width  - DisplayWidth * scale)	* 0.5f;
		float oy =	viewport.Y + (viewport.Height - FieldHeight  * scale)	* 0.5f;
		Rectangle Px(float x, float y, float w, float h)	=>	new(
			(int)(ox + x * PixelAspectXMultiplier * scale),
			(int)(oy + y * scale),
			(int)MathF.Ceiling(w * PixelAspectXMultiplier * scale),
			(int)MathF.Ceiling(h * scale));

		sb.Draw(_pixel, Px(0, HudHeight, FieldWidth, PlayHeight), BgColor);
		sb.Draw(_pixel, Px(0, 0, FieldWidth, HudHeight), HudColor);

		foreach (var w in _walls)
			sb.Draw(_pixel, Px(w.X, w.Y, w.Width, w.Height), WallColor);

		DrawTankSprite(sb, _p1, P1Color, Px);
		DrawTankSprite(sb, _p2, P2Color, Px);

		foreach (var b in _bullets)
			sb.Draw(_pixel, Px(b.Position.X - 1.5f, b.Position.Y - 1.5f, 3, 3), BulletColor);

		DrawScore(sb, _p1.Score, leftAligned:	true,  Px);
		DrawScore(sb, _p2.Score, leftAligned:	false, Px);

		if (_winner != 0)
			DrawWinnerBanner(sb, Px);
	}

	/// <summary>
	/// Render Combat's tank sprite for the current 16-direction heading.
	/// Directions 0–7 use the stored frame directly; directions 8–15 are the
	/// same frame rotated 180° (horizontal flip + reverse row order), exactly
	/// what the ROM's <c>ROT</c> routine does at $F744 via the carry-flag
	/// DEY/INY trick + the REFP0 reflection register.
	/// </summary>
	private void DrawTankSprite(SpriteBatch sb, Tank t, Color c,
		Func<float, float, float, float, Rectangle>	Px)
	{
		int d =	t.Direction & 15;
		bool flip =	d >= 8;
		int frame =	flip ? d - 8 : d;
		var rows =	TankFrames[frame];

		float x0 =	t.Position.X - TankSpriteW * 0.5f;
		float y0 =	t.Position.Y - TankSpriteH * 0.5f;
		for (int row = 0; row < 8; row++)
		{
			int srcRow =	flip ? 7 - row : row;
			byte bits =	rows[srcRow];
			for (int bit = 0; bit < 8; bit++)
			{
				int srcBit =	flip ? bit : 7 - bit;	// bit 7 = leftmost pixel
				if ((bits & (1 << srcBit)) == 0)	continue;
				sb.Draw(_pixel, Px(x0 + bit, y0 + row * 2, 1, 2), c);
			}
		}
	}

	private void DrawScore(SpriteBatch sb, int score, bool leftAligned,
		Func<float, float, float, float, Rectangle>	Px)
	{
		int x =	leftAligned ? 8 : FieldWidth - 8 - 5;
		Color c =	leftAligned ? P1Color : P2Color;
		DrawDigit(sb, score, x, 4, c, Px);
	}

	private void DrawDigit(SpriteBatch sb, int digit, int x, int y, Color c,
		Func<float, float, float, float, Rectangle>	Px)
	{
		bool[,]	g =	digit switch
		{
			0 => new[,] { {true,true,true},{true,false,true},{true,false,true},{true,false,true},{true,true,true} },
			1 => new[,] { {false,true,false},{true,true,false},{false,true,false},{false,true,false},{true,true,true} },
			2 => new[,] { {true,true,true},{false,false,true},{true,true,true},{true,false,false},{true,true,true} },
			3 => new[,] { {true,true,true},{false,false,true},{true,true,true},{false,false,true},{true,true,true} },
			4 => new[,] { {true,false,true},{true,false,true},{true,true,true},{false,false,true},{false,false,true} },
			5 => new[,] { {true,true,true},{true,false,false},{true,true,true},{false,false,true},{true,true,true} },
			6 => new[,] { {true,true,true},{true,false,false},{true,true,true},{true,false,true},{true,true,true} },
			7 => new[,] { {true,true,true},{false,false,true},{false,false,true},{false,false,true},{false,false,true} },
			8 => new[,] { {true,true,true},{true,false,true},{true,true,true},{true,false,true},{true,true,true} },
			9 => new[,] { {true,true,true},{true,false,true},{true,true,true},{false,false,true},{true,true,true} },
			_ => new bool[5, 3],
		};
		for (int gy = 0; gy < 5; gy++)
		for (int gx = 0; gx < 3; gx++)
			if (g[gy, gx])
				sb.Draw(_pixel, Px(x + gx * 2, y + gy * 2, 2, 2), c);
	}

	private void DrawWinnerBanner(SpriteBatch sb,
		Func<float, float, float, float, Rectangle>	Px)
	{
		int bw =	100, bh =	24;
		int bx =	(FieldWidth - bw)	/ 2;
		int by =	(FieldHeight - bh)	/ 2;
		sb.Draw(_pixel, Px(bx, by, bw, bh), HudColor);
		sb.Draw(_pixel, Px(bx, by, bw, 2),  HudText);
		sb.Draw(_pixel, Px(bx, by + bh - 2, bw, 2), HudText);
		sb.Draw(_pixel, Px(bx, by, 2, bh),  HudText);
		sb.Draw(_pixel, Px(bx + bw - 2, by, 2, bh), HudText);
		var c =	_winner == 1 ? P1Color : P2Color;
		sb.Draw(_pixel, Px(bx + 8, by + 8, 8, 8), c);
		DrawDigit(sb, _winner, bx + 24, by + 7, HudText, Px);
		sb.Draw(_pixel, Px(bx + 40, by + 9, 4, 6), HudText);
		sb.Draw(_pixel, Px(bx + 50, by + 9, 4, 6), HudText);
		sb.Draw(_pixel, Px(bx + 60, by + 9, 4, 6), HudText);
		sb.Draw(_pixel, Px(bx + 70, by + 9, 4, 6), HudText);
	}

	public void Shutdown()
	{
		_pixel?.Dispose();
	}

	// ── Internal types ──────────────────────────────────────────────────────
	private sealed class Tank
	{
		public Vector2 Position;
		public int Direction;	// 0..15, each = 22.5°
		public int TurnTimer;
		public int FireCooldown;
		public int Score;
	}

	private struct Bullet
	{
		public Tank Owner;
		public Vector2 Position;
		public Vector2 Velocity;
		public int LifeFrames;
	}
}
