using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TileEngine.MiniGames;
using TileEngine.Rendering;

namespace Battleshoot;

/// <summary>
/// "Battleshoot" — a deliberately faithful homage to Atari 2600 Combat (1977).
/// Two tanks, one screen, four walls forming the classic centre-pinch maze;
/// one bullet at a time per tank, first to <see cref="ScoreToWin"/> wins.
///
/// Implements <see cref="IEmbeddedMiniGame"/> so it can run as a standalone
/// executable (via <see cref="Program"/> and <see cref="StandaloneHost"/>) or
/// be hosted inside ChildhoodAdventure as a <see cref="MiniGameScene"/>.
/// </summary>
public sealed class BattleshootGame :	IEmbeddedMiniGame
{
	// ── Constants — Atari 2600 native NTSC playfield is 160×192 ──────────────
	// Internal game coordinates use this 160-wide logical space so gameplay
	// constants (speeds, sizes, wall positions) stay Atari-authentic.
	public const int FieldWidth	=	160;
	public const int FieldHeight	=	192;

	// Atari 2600 pixels are NOT square: each logical pixel covers two
	// horizontal screen pixels on a real display. On a square-pixel monitor
	// the playfield must be presented as 320×192 with every column doubled.
	// The Draw routine handles the X→screen mapping; <see cref="DisplayWidth"/>
	// is what the host should letterbox to.
	public const int PixelAspectXMultiplier	=	2;
	public const int DisplayWidth	=	FieldWidth * PixelAspectXMultiplier;	// 320

	private const int HudHeight =	16;
	private const int PlayHeight =	FieldHeight - HudHeight;

	private const float TankSpeed =	36f;	// pixels per second
	private const float BulletSpeed =	120f;
	private const float FireCooldown =	0.25f;
	private const float TankSize =	10f;
	private const float BulletSize =	3f;
	private const float MaxBulletDistance =	200f;
	private const int ScoreToWin =	5;
	private const float HitFreezeSeconds =	0.6f;
	private const float WinHoldSeconds =	2.5f;

	// Atari 2600 NTSC palette samples (close enough for the homage).
	// Combat tanks-mode field is a deep brick red, walls are cream-yellow.
	private static readonly Color BgColor =	new(162, 78, 44);		// Combat brick-red field
	private static readonly Color WallColor =	new(228, 188, 110);	// cream-yellow walls
	private static readonly Color HudColor =	new(20, 20, 20);
	private static readonly Color HudText =	new(236, 236, 236);
	private static readonly Color P1Color =	new(212, 80, 60);		// red tank
	private static readonly Color P2Color =	new(80, 156, 220);	// blue tank
	private static readonly Color BulletColor =	new(252, 224, 168);

	// ── State ────────────────────────────────────────────────────────────────
	private GraphicsDevice _gd =	null!;
	private Texture2D _pixel =	null!;

	private Tank _p1 =	null!;
	private Tank _p2 =	null!;
	private readonly List<Bullet>	_bullets =	new();
	private readonly List<Rectangle>	_walls =	new();
	private float _hitFreezeTimer;
	private int _winner;	// 0 = none, 1 = P1, 2 = P2
	private float _winHoldTimer;
	private bool _finished;
	private readonly Random _rng =	new();

	// ── IEmbeddedMiniGame ────────────────────────────────────────────────────
	public string Title =>	"Battleshoot";
	// Native display is 320×192 SQUARE pixels (logical 160×192 doubled
	// horizontally to honour Atari 2600 double-wide pixels). Hosts letterbox
	// this aspect.
	public Point NativeResolution =>	new(DisplayWidth, FieldHeight);
	public bool IsFinished =>	_finished;

	public void Initialize(GraphicsDevice graphicsDevice, ContentManager content)
	{
		_gd =	graphicsDevice;
		_pixel =	new Texture2D(_gd, 1, 1);
		_pixel.SetData(new[] { Color.White });

		BuildWalls();
		ResetRound(initial:	true);
	}

	public void Update(GameTime gameTime, IMiniGameInput input)
	{
		float dt =	(float)gameTime.ElapsedGameTime.TotalSeconds;

		// Host-driven exit (ESC). Honour immediately if no game is in progress;
		// otherwise the player has to finish the match.
		if (input.ExitRequested)	{ _finished =	true; return; }

		// Win banner timing
		if (_winner != 0)
		{
			_winHoldTimer -=	dt;
			if (_winHoldTimer <= 0f)	{ _finished =	true; }
			return;
		}

		// Brief freeze on hit so the impact reads
		if (_hitFreezeTimer > 0f)
		{
			_hitFreezeTimer -=	dt;
			if (_hitFreezeTimer <= 0f)	ResetRound(initial:	false);
			return;
		}

		UpdateTank(_p1, input.GetDirection(0), input.IsFirePressed(0), dt);
		UpdateTankAi(_p2, _p1, dt);

		UpdateBullets(dt);
		ResolveCollisions();
		CheckWin();
	}

	public void Draw(SpriteBatch spriteBatch, RectangleF viewport)
	{
		// Fit the 320×192 display rectangle into the host viewport. Px() then
		// converts logical (160-wide) coordinates to screen pixels by
		// stretching X by PixelAspectXMultiplier — the Atari 2600
		// double-wide-pixel rule applied at draw time so internal gameplay
		// stays in clean 160×192 logical space.
		float scale =	MathF.Min(viewport.Width / DisplayWidth, viewport.Height / FieldHeight);
		float ox =	viewport.X + (viewport.Width  - DisplayWidth * scale)	* 0.5f;
		float oy =	viewport.Y + (viewport.Height - FieldHeight  * scale)	* 0.5f;
		Rectangle Px(float x, float y, float w, float h)	=>
			new(
				(int)(ox + x * PixelAspectXMultiplier * scale),
				(int)(oy + y * scale),
				(int)MathF.Ceiling(w * PixelAspectXMultiplier * scale),
				(int)MathF.Ceiling(h * scale));

		// Background
		spriteBatch.Draw(_pixel, Px(0, HudHeight, FieldWidth, PlayHeight), BgColor);
		spriteBatch.Draw(_pixel, Px(0, 0, FieldWidth, HudHeight), HudColor);

		// Walls
		foreach (var w in _walls)
			spriteBatch.Draw(_pixel, Px(w.X, w.Y, w.Width, w.Height), WallColor);

		// Tanks
		DrawTank(spriteBatch, _p1, P1Color, Px);
		DrawTank(spriteBatch, _p2, P2Color, Px);

		// Bullets
		foreach (var b in _bullets)
		{
			spriteBatch.Draw(_pixel,
				Px(b.Position.X - BulletSize/2, b.Position.Y - BulletSize/2, BulletSize, BulletSize),
				BulletColor);
		}

		// Score (chunky 5×7 digits drawn from rectangles — no font dependency)
		DrawScore(spriteBatch, _p1.Score, leftAligned: true,  Px);
		DrawScore(spriteBatch, _p2.Score, leftAligned: false, Px);

		if (_winner != 0)
		{
			DrawWinnerBanner(spriteBatch, Px);
		}
	}

	public void Shutdown()
	{
		_pixel?.Dispose();
	}

	// ── Setup ────────────────────────────────────────────────────────────────
	private void BuildWalls()
	{
		_walls.Clear();
		// No outer-border walls — playfield bounds are enforced in
		// CollidesWithWalls. Layout matches Atari Combat "tanks": scattered
		// cream shapes on an open red field. Spacing is hand-tuned so every
		// horizontal-traversal channel is ≥12 px tall, leaving at least 2 px
		// of clearance for a 10×10 tank.
		//
		// Vertical channel budget through the play area (y=16..192):
		//   y=16-18   2 px sliver above top finger
		//   y=18-30   12 px top finger
		//   y=30-38   8 px channel  (route around finger laterally)
		//   y=38-44   6 px top H bar
		//   y=44-56   12 px channel ✓
		//   y=56-66   10 px upper L (H 6 + V 10 with overlap)
		//   y=66-78   12 px channel ✓
		//   y=78-84   6 px bracket top arm
		//   y=84-96   12 px channel ✓
		//   y=96-108  12 px inside block
		//   y=108-122 14 px channel ✓
		//   y=122-128 6 px bracket bottom arm
		//   y=128-140 12 px channel ✓
		//   y=140-150 10 px lower L (V 10 + H 6 with overlap)
		//   y=150-164 14 px channel ✓
		//   y=164-170 6 px bottom H bar
		//   y=170-176 6 px channel  (route around bottom finger)
		//   y=176-188 12 px bottom finger

		// Top finger (vertical bar, top centre)
		_walls.Add(new Rectangle(76, 18, 8, 12));

		// Top horizontal bars (left & right). Inset 4 px from the playfield
		// edges so the outer-edge corridor stays a passable 12 px wide.
		_walls.Add(new Rectangle( 12, 38, 24, 6));
		_walls.Add(new Rectangle(124, 38, 24, 6));

		// Upper L-corners (notch facing inward — horizontal arm + downward stub)
		_walls.Add(new Rectangle(28, 56, 16, 6));	// upper-left  H
		_walls.Add(new Rectangle(38, 56,  6, 10));	// upper-left  V (down from right edge of H)
		_walls.Add(new Rectangle(116, 56, 16, 6));	// upper-right H
		_walls.Add(new Rectangle(116, 56,  6, 10));	// upper-right V (down from left edge of H)

		// Big bracket "[" on the left (vertical + top arm + bottom arm).
		// Spine inset 4 px so the outer-edge corridor (x=0..12) is a passable
		// 12 px wide rather than the impassable 8 it was before.
		_walls.Add(new Rectangle( 12, 78,  6, 50));	// left vertical spine
		_walls.Add(new Rectangle( 12, 78, 24,  6));	// left top arm
		_walls.Add(new Rectangle( 12,122, 24,  6));	// left bottom arm

		// Big bracket "]" on the right (mirror — spine at x=142..148, leaving
		// the x=148..160 outer corridor 12 px wide)
		_walls.Add(new Rectangle(142, 78,  6, 50));	// right vertical spine
		_walls.Add(new Rectangle(124, 78, 24,  6));	// right top arm
		_walls.Add(new Rectangle(124,122, 24,  6));	// right bottom arm

		// Two solid blocks framing the central corridor (slimmer than 16 tall
		// so the gap to the bracket bottom arm stays >12 px).
		_walls.Add(new Rectangle(48, 96, 16, 12));
		_walls.Add(new Rectangle(96, 96, 16, 12));

		// Lower L-corners (mirror of upper, opening upward). NOT strictly
		// vertically-symmetric to the upper Ls — pushed a few pixels further
		// from the bracket so the y=128..140 channel stays passable.
		_walls.Add(new Rectangle(38,140,  6, 10));	// lower-left  V (up from right edge of H)
		_walls.Add(new Rectangle(28,144, 16,  6));	// lower-left  H
		_walls.Add(new Rectangle(116,140, 6, 10));	// lower-right V
		_walls.Add(new Rectangle(116,144,16,  6));	// lower-right H

		// Bottom horizontal bars (mirror — same 4 px inset as the top bars)
		_walls.Add(new Rectangle( 12,164, 24, 6));
		_walls.Add(new Rectangle(124,164, 24, 6));

		// Bottom finger (mirror of top)
		_walls.Add(new Rectangle(76,176,  8, 12));
	}

	private void ResetRound(bool initial)
	{
		_bullets.Clear();
		_hitFreezeTimer =	0f;

		_p1 ??=	new Tank();
		_p2 ??=	new Tank();

		// Diagonally-opposite spawns in the open lanes between top/bottom
		// horizontals and the upper/lower L-corners, mirroring how Combat's
		// tank-mode opens. P1 faces east, P2 faces west.
		_p1.Position =	new Vector2(36, 30);
		_p1.Facing =	new Vector2(1, 0);
		_p1.FireCooldown =	0f;

		_p2.Position =	new Vector2(124, 178);
		_p2.Facing =	new Vector2(-1, 0);
		_p2.FireCooldown =	0f;

		if (initial)
		{
			_p1.Score =	0;
			_p2.Score =	0;
			_winner =	0;
		}
	}

	// ── Tank update ──────────────────────────────────────────────────────────
	private void UpdateTank(Tank tank, Vector2 dir, bool firePressed, float dt)
	{
		if (dir.LengthSquared() > 0.001f)
		{
			tank.Facing =	NormaliseToCardinal(dir);
			var attempt =	tank.Position + tank.Facing * TankSpeed * dt;
			if (!CollidesWithWalls(attempt, TankSize))	tank.Position =	attempt;
		}

		tank.FireCooldown =	Math.Max(0f, tank.FireCooldown - dt);
		if (firePressed && tank.FireCooldown <= 0f && _bullets.All(b => b.Owner != tank))
		{
			_bullets.Add(new Bullet
			{
				Owner =	tank,
				Position =	tank.Position + tank.Facing * (TankSize / 2 + 2),
				Velocity =	tank.Facing * BulletSpeed,
			});
			tank.FireCooldown =	FireCooldown;
		}
	}

	private void UpdateTankAi(Tank tank, Tank target, float dt)
	{
		// Simple pursue-and-shoot AI: move toward the player, fire when roughly aligned.
		var toTarget =	target.Position - tank.Position;
		if (toTarget.LengthSquared() < 0.001f)	return;

		// Choose dominant axis to keep movement on cardinals (Atari joystick feel).
		Vector2 dir;
		if (Math.Abs(toTarget.X) > Math.Abs(toTarget.Y))
			dir =	new Vector2(Math.Sign(toTarget.X), 0);
		else
			dir =	new Vector2(0, Math.Sign(toTarget.Y));

		// 8% chance per second to "rethink" and pick a random cardinal — keeps the AI from
		// wedging itself permanently against a wall.
		if (_rng.NextDouble() < 0.08 * dt * 60)
		{
			dir =	_rng.Next(4) switch
			{
				0 => new Vector2(1, 0),
				1 => new Vector2(-1, 0),
				2 => new Vector2(0, 1),
				_ => new Vector2(0, -1),
			};
		}

		tank.Facing =	dir;
		var attempt =	tank.Position + dir * (TankSpeed * 0.85f) * dt;
		if (!CollidesWithWalls(attempt, TankSize))	tank.Position =	attempt;

		// Fire when roughly aligned with target on facing axis.
		bool aligned =	(Math.Abs(dir.X) > 0 && Math.Abs(toTarget.Y) < TankSize)
					|| (Math.Abs(dir.Y) > 0 && Math.Abs(toTarget.X) < TankSize);
		if (aligned && tank.FireCooldown <= 0f && _bullets.All(b => b.Owner != tank))
		{
			_bullets.Add(new Bullet
			{
				Owner =	tank,
				Position =	tank.Position + tank.Facing * (TankSize / 2 + 2),
				Velocity =	tank.Facing * BulletSpeed,
			});
			tank.FireCooldown =	FireCooldown * 1.5f;
		}
		tank.FireCooldown =	Math.Max(0f, tank.FireCooldown - dt);
	}

	private static Vector2 NormaliseToCardinal(Vector2 v)
	{
		if (Math.Abs(v.X) > Math.Abs(v.Y))	return new Vector2(Math.Sign(v.X), 0);
		return new Vector2(0, Math.Sign(v.Y));
	}

	// ── Bullets ──────────────────────────────────────────────────────────────
	private void UpdateBullets(float dt)
	{
		for (int i = _bullets.Count - 1; i >= 0; i--)
		{
			var b =	_bullets[i];
			b.Position +=	b.Velocity * dt;
			b.Travelled +=	BulletSpeed * dt;
			_bullets[i]	=	b;
			bool kill =	b.Travelled >	MaxBulletDistance
						|| CollidesWithWalls(b.Position, BulletSize);
			if (kill)	_bullets.RemoveAt(i);
		}
	}

	private void ResolveCollisions()
	{
		for (int i = _bullets.Count - 1; i >= 0; i--)
		{
			var b =	_bullets[i];
			Tank?	hit =	null;
			if (b.Owner != _p1 && Hits(b, _p1))	hit =	_p1;
			else if (b.Owner != _p2 && Hits(b, _p2))	hit =	_p2;
			if (hit != null)
			{
				_bullets.RemoveAt(i);
				if (hit == _p1)	_p2.Score++;
				else _p1.Score++;
				_hitFreezeTimer =	HitFreezeSeconds;
				return;
			}
		}
	}

	private static bool Hits(Bullet b, Tank t)
	{
		var d =	b.Position - t.Position;
		return Math.Abs(d.X) < TankSize / 2 && Math.Abs(d.Y) < TankSize / 2;
	}

	private bool CollidesWithWalls(Vector2 pos, float size)
	{
		var box =	new Rectangle(
			(int)(pos.X - size / 2), (int)(pos.Y - size / 2),
			(int)size, (int)size);

		// Invisible playfield bounds — there are no drawn outer walls in the
		// Combat-style map, but tanks and bullets still can't leave the field.
		if (box.X < 0 || box.Y < HudHeight ||
			box.X + box.Width  > FieldWidth ||
			box.Y + box.Height > FieldHeight)	return true;

		foreach (var w in _walls)	if (w.Intersects(box))	return true;
		return false;
	}

	private void CheckWin()
	{
		if (_p1.Score >= ScoreToWin)	{ _winner =	1; _winHoldTimer =	WinHoldSeconds; }
		else if (_p2.Score >= ScoreToWin)	{ _winner =	2; _winHoldTimer =	WinHoldSeconds; }
	}

	// ── Drawing helpers ──────────────────────────────────────────────────────
	private void DrawTank(SpriteBatch sb, Tank t, Color c, Func<float, float, float, float, Rectangle> Px)
	{
		// Body
		sb.Draw(_pixel, Px(t.Position.X - TankSize / 2, t.Position.Y - TankSize / 2, TankSize, TankSize), c);
		// Cannon — short stub in facing direction
		float cx =	t.Position.X + t.Facing.X * (TankSize / 2);
		float cy =	t.Position.Y + t.Facing.Y * (TankSize / 2);
		float cw =	Math.Abs(t.Facing.X) > 0 ? 4 : 2;
		float ch =	Math.Abs(t.Facing.Y) > 0 ? 4 : 2;
		sb.Draw(_pixel, Px(cx - cw / 2, cy - ch / 2, cw, ch), c);
	}

	private void DrawScore(SpriteBatch sb, int score, bool leftAligned,
		Func<float, float, float, float, Rectangle> Px)
	{
		// Single-digit score, drawn as chunky 5-segment glyph (only digits 0-9 needed).
		int x =	leftAligned ? 8 :	FieldWidth - 8 - 5;
		Color c =	leftAligned ? P1Color :	P2Color;
		DrawDigit(sb, score, x, 4, c, Px);
	}

	private void DrawDigit(SpriteBatch sb, int digit, int x, int y, Color c,
		Func<float, float, float, float, Rectangle> Px)
	{
		// 3×5 pixel digits — readable at the 160×192 native resolution.
		bool[,]	glyph =	digit switch
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
		{
			for (int gx = 0; gx < 3; gx++)
			{
				if (!glyph[gy, gx])	continue;
				sb.Draw(_pixel, Px(x + gx * 2, y + gy * 2, 2, 2), c);
			}
		}
	}

	private void DrawWinnerBanner(SpriteBatch sb,
		Func<float, float, float, float, Rectangle> Px)
	{
		int bw =	100, bh =	24;
		int bx =	(FieldWidth - bw)	/ 2;
		int by =	(FieldHeight - bh)	/ 2;
		sb.Draw(_pixel, Px(bx, by, bw, bh), HudColor);
		sb.Draw(_pixel, Px(bx, by, bw, 2),  HudText);
		sb.Draw(_pixel, Px(bx, by + bh - 2, bw, 2), HudText);
		sb.Draw(_pixel, Px(bx, by, 2, bh),  HudText);
		sb.Draw(_pixel, Px(bx + bw - 2, by, 2, bh), HudText);

		// Big winning digit + colour bar
		var c =	_winner == 1 ? P1Color :	P2Color;
		sb.Draw(_pixel, Px(bx + 8, by + 8, 8, 8), c);
		DrawDigit(sb, _winner, bx + 24, by + 7, HudText, Px);

		// "WINS" — a tiny indicator block beside the digit (no font).
		sb.Draw(_pixel, Px(bx + 40, by + 9, 4, 6), HudText);
		sb.Draw(_pixel, Px(bx + 50, by + 9, 4, 6), HudText);
		sb.Draw(_pixel, Px(bx + 60, by + 9, 4, 6), HudText);
		sb.Draw(_pixel, Px(bx + 70, by + 9, 4, 6), HudText);
	}

	// ── Internal types ───────────────────────────────────────────────────────
	private sealed class Tank
	{
		public Vector2 Position;
		public Vector2 Facing =	new(1, 0);
		public float FireCooldown;
		public int Score;
	}

	private struct Bullet
	{
		public Tank Owner;
		public Vector2 Position;
		public Vector2 Velocity;
		public float Travelled;
	}
}
