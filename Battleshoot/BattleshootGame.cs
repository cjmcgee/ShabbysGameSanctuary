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
	public const int FieldWidth	=	160;
	public const int FieldHeight	=	192;

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
	private static readonly Color BgColor =	new(76, 108, 56);		// dirt/grass green
	private static readonly Color WallColor =	new(204, 204, 96);	// barrier yellow
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
	public Point NativeResolution =>	new(FieldWidth, FieldHeight);
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
		// Compute the integer pixel scale that fits the host viewport.
		float scale =	MathF.Min(viewport.Width / FieldWidth, viewport.Height / FieldHeight);
		float ox =	viewport.X + (viewport.Width  - FieldWidth  * scale)	* 0.5f;
		float oy =	viewport.Y + (viewport.Height - FieldHeight * scale)	* 0.5f;
		Rectangle Px(float x, float y, float w, float h)	=>
			new((int)(ox + x * scale), (int)(oy + y * scale), (int)MathF.Ceiling(w * scale), (int)MathF.Ceiling(h * scale));

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
		// Outer border (4 walls). HUD is the top strip — playfield starts at HudHeight.
		const int border =	4;
		_walls.Add(new Rectangle(0,					HudHeight,				FieldWidth,	border));	// top
		_walls.Add(new Rectangle(0,					FieldHeight - border,	FieldWidth,	border));	// bottom
		_walls.Add(new Rectangle(0,					HudHeight,				border,		PlayHeight));	// left
		_walls.Add(new Rectangle(FieldWidth - border, HudHeight,				border,		PlayHeight));	// right

		// Centre-pinch barrier (Combat's classic L-blocks).
		_walls.Add(new Rectangle(40,	60,	8,		60));	// left vertical
		_walls.Add(new Rectangle(112,	60,	8,		60));	// right vertical
		_walls.Add(new Rectangle(40,	60,	36,	8));	// left horizontal
		_walls.Add(new Rectangle(84,	60,	36,	8));	// right horizontal (top of right L flipped)
		_walls.Add(new Rectangle(40,	112,	36,	8));	// left horizontal lower
		_walls.Add(new Rectangle(84,	112,	36,	8));	// right horizontal lower
	}

	private void ResetRound(bool initial)
	{
		_bullets.Clear();
		_hitFreezeTimer =	0f;

		float playMidY =	HudHeight + PlayHeight / 2f;
		_p1 ??=	new Tank();
		_p2 ??=	new Tank();
		_p1.Position =	new Vector2(16, playMidY);
		_p1.Facing =	new Vector2(1, 0);
		_p1.FireCooldown =	0f;

		_p2.Position =	new Vector2(FieldWidth - 16, playMidY);
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
