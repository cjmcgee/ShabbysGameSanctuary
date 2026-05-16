using ChildhoodAdventure.Demographics;
using ChildhoodAdventure.RetroSystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TileEngine.Components;
using TileEngine.Core;
using TileEngine.ECS;
using TileEngine.Npc;
using TileEngine.Rendering;

namespace ChildhoodAdventure.Scenes
{
	/// <summary>
	/// Base class for all ChildhoodAdventure scenes.
	/// Handles player movement, NPC interaction, dialogue input, and dialogue box
	/// rendering so concrete scenes only need to define their map, NPCs, and
	/// scene-transition logic.
	/// </summary>
	public abstract class AdventureScene : Scene
	{
		private Entity? _player;
		private KeyboardState _prevKeys;
		private Texture2D? _pixel;

		protected bool Transitioning;

		// ── Customization points ─────────────────────────────────────────────

		// All in tile-space units: speed in tiles/sec, radius in tiles.
		protected virtual float PlayerMaxSpeed		=> 5.6f;
		protected virtual float CameraFollowSpeed	=> 8f;
		protected virtual float InteractionRadius	=> 1.875f;

		// Dialog colours default to the active retro system's UI palette;
		// scenes override DialogueBorderColor with one of the system's house
		// tones to give the box per-scene flavour.
		protected abstract Color DialogueBorderColor { get; }
		protected virtual  Color DialogueSpeakerColor =>
			RetroSystemRegistry.Current.ScenePalette.UiAccent;

		// ── Lifecycle ────────────────────────────────────────────────────────

		protected override sealed void OnLoad()
		{
			Engine.DialogueSystem.EnsureYarnLoaded( GetType().Assembly, AdventureGame.DialogueBundleResource );
			Engine.DialogueSystem.RegisterCommandHandler( "flag",
				args =>	{ if (args.Length > 0) GameState.SetFlag( args[0] ); });
			OnSceneLoad();
		}

		protected abstract void OnSceneLoad();

		protected override sealed void OnUnload()
		{
			Engine.RenderSystem.LightingSystem.ClearLights();
			OnSceneUnload();
		}

		protected virtual void OnSceneUnload() { }

		// ── Entity helpers ───────────────────────────────────────────────────

		protected Entity SpawnPlayer( GraphicsDevice gd, Vector2 pos )
		{
			// Player is structurally an NPC: registered once in the registry,
			// re-spawned per scene with the same Npc data so position can be
			// snapshotted into Npc.WorldPosition on unload (saves use it).
			var npc =	Engine.NpcRegistry.Get( "Player" );
			if( npc == null )
			{
				npc =	new TileEngine.Npc.Npc( "Player", "Player" )
				{
					WorldPosition =	pos,
					CurrentScene =	Name,
				};
				Engine.NpcRegistry.Register( npc );
			}
			else
			{
				npc.CurrentScene =	Name;
				npc.WorldPosition =	pos;	// scene-driven (GameState supplies the right pos)
			}

			// Sprite scale derived from age + gender + adult height (see Demographics/).
			var profile = NpcProfiles.GetOrDefault( "Player", Gender.Male, fallbackAge: 9f );
			var sprite = SpriteFactory.BuildCharacter( gd, NpcAppearances.Player );
			sprite.FrameTileSize *=	profile.SpriteScale;

			var e =	NpcBuilder.Default( Engine.EntityWorld, npc )
				.WithSprite( sprite )
				.WithCollider( 0.625f, 0.375f, new Vector2( -0.3125f, -0.1875f ), solid: true )
				.With( new PlayerControlComponent() )
				.WithTag( "player" )
				.Configure( ent => ent.GetComponent<TransformComponent>()!.MaxSpeed = PlayerMaxSpeed )
				.Build();

			Engine.RenderSystem.Camera.FollowTarget = pos;
			Engine.RenderSystem.Camera.FollowSpeed = CameraFollowSpeed;
			Engine.RenderSystem.Camera.CenterOn( pos );
			_player = e;
			return e;
		}

		protected Entity SpawnNpc( GraphicsDevice gd, string name, Vector2 pos,
			CharacterAppearance appearance,	Action talkFn )
		{
			// Persistent identity: register-or-update an Npc by name. Existing
			// world position from the registry takes precedence on re-entry,
			// so an NPC who walked somewhere remembers it across scene reloads.
			var npc =	Engine.NpcRegistry.Get( name );
			if( npc == null )
			{
				npc =	new TileEngine.Npc.Npc( name, name )
				{
					WorldPosition =	pos,
					CurrentScene =	Name,
				};
				Engine.NpcRegistry.Register( npc );
			}
			else
			{
				npc.CurrentScene =	Name;
				if( npc.WorldPosition == Vector2.Zero ) npc.WorldPosition =	pos;
			}

			// Sprite scale comes from the NPC's demographic profile (gender +
			// age + adult height). Adults render close to scale 1.0; children
			// scale via the NPCSIZE.md growth curve.
			var profile = NpcProfiles.GetOrDefault( name );
			var sprite = SpriteFactory.BuildCharacter( gd, appearance );
			sprite.FrameTileSize *=	profile.SpriteScale;

			var e =	NpcBuilder.Default( Engine.EntityWorld, npc )
				.WithSprite( sprite )
				.WithCollider( 0.625f, 0.5f, new Vector2( -0.3125f, -0.25f ), solid: true )
				.With( new InteractableComponent( _ => talkFn(), range: InteractionRadius, prompt: "Talk" ) )
				.Build();

			return e;
		}

		// ── Update ────────────────────────────────────────────────────────────

		public override sealed void Update( GameTime gameTime )
		{
			var keys = Keyboard.GetState();
			bool wasDialogueActive = Engine.DialogueSystem.IsActive;

			HandleDialogueInput( keys );

			if( !Engine.DialogueSystem.IsActive && !Transitioning )
			{
				HandlePlayerMovement( keys, gameTime );
				if( !wasDialogueActive )
					HandleInteraction( keys );
				CheckSceneTransitions();
			}

			_prevKeys =	keys;
		}

		protected abstract void CheckSceneTransitions();

		private void HandleDialogueInput( KeyboardState keys )
		{
			if( !Engine.DialogueSystem.IsActive ) { return; }

			if( Engine.DialogueSystem.WaitingForChoice )
			{
				if( keys.IsKeyDown( Keys.Up ) && !_prevKeys.IsKeyDown( Keys.Up ) )
				{
					Engine.DialogueSystem.MoveChoiceSelection( -1 );
				}

				if( keys.IsKeyDown( Keys.Down ) && !_prevKeys.IsKeyDown( Keys.Down ) )
				{
					Engine.DialogueSystem.MoveChoiceSelection( 1 );
				}

				if( keys.IsKeyDown( Keys.E ) && !_prevKeys.IsKeyDown( Keys.E ) )
				{
					Engine.DialogueSystem.SelectChoice( Engine.DialogueSystem.SelectedChoiceIndex );
				}
			}
			else if (keys.IsKeyDown( Keys.E ) && !_prevKeys.IsKeyDown( Keys.E ))
			{
				Engine.DialogueSystem.Advance();
			}
		}

		private void HandlePlayerMovement( KeyboardState keys, GameTime gameTime )
		{
			if( _player == null ) { return; }

			var t  = _player.GetComponent<TransformComponent>();
			if (t == null)	return;

			var sc = _player.GetComponent<SpriteComponent>();

			var move = Vector2.Zero;
			if( keys.IsKeyDown( Keys.W ) || keys.IsKeyDown( Keys.Up ) )		move.Y -= 1;
			if( keys.IsKeyDown( Keys.S ) || keys.IsKeyDown( Keys.Down ) )	move.Y += 1;
			if( keys.IsKeyDown( Keys.A ) || keys.IsKeyDown( Keys.Left ) )	move.X -= 1;
			if( keys.IsKeyDown( Keys.D ) || keys.IsKeyDown( Keys.Right ) )	move.X += 1;

			if( move != Vector2.Zero )
			{
				var dir = Vector2.Normalize( move );
				t.Velocity = dir * t.MaxSpeed;
				t.Facing = dir;
				sc?.Play( "walk" );
			}
			else
			{
				t.Velocity = Vector2.Zero;
				sc?.Play( "idle" );
			}

			Engine.CollisionSystem.MoveAndSlide( _player, (float)gameTime.ElapsedGameTime.TotalSeconds );
			t.Velocity = Vector2.Zero;
			Engine.RenderSystem.Camera.FollowTarget = t.Position;
		}

		private void HandleInteraction( KeyboardState keys )
		{
			if( !keys.IsKeyDown( Keys.E ) || _prevKeys.IsKeyDown( Keys.E ) ) { return; }
			if( _player == null ) { return; }

			var pt = _player.GetComponent<TransformComponent>()?.Position ?? Vector2.Zero;

			foreach( var e in Engine.EntityWorld.GetWithComponent<InteractableComponent>() )
			{
				var ic = e.GetComponent<InteractableComponent>()!;
				var nt = e.GetComponent<TransformComponent>()?.Position ?? Vector2.Zero;
				float radius = ic.Range > 0 ? ic.Range : InteractionRadius;
				if( Vector2.Distance( pt, nt ) < radius )
				{
					ic.Trigger( _player );
					return;
				}
			}
		}

		// ── Scene-transition helper ──────────────────────────────────────────

		protected Vector2 PlayerPosition =>
			_player?.GetComponent<TransformComponent>()?.Position ?? Vector2.Zero;

		// ── Draw ──────────────────────────────────────────────────────────────

		public override sealed void Draw( SpriteBatch spriteBatch, GameTime gameTime )
		{
			Engine.RenderSystem.DrawScene( Engine.EntityWorld, gameTime, Color.Black );

			Engine.RenderSystem.BeginUI();
			if( Engine.DialogueSystem.IsActive )
			{
				DrawDialogueBox( Engine.RenderSystem.SpriteBatch );
			}
			Engine.RenderSystem.EndUI();
		}

		private void DrawDialogueBox( SpriteBatch sb )
		{
			const float scale =	2f;

			if( _pixel == null )
			{
				_pixel = new Texture2D( Engine.GraphicsDevice, 1, 1 );
				_pixel.SetData( [Color.White] );
			}

			var vp 			= Engine.GraphicsDevice.Viewport;
			var font		= Engine.RenderSystem.Font;
			var sp			= RetroSystemRegistry.Current.ScenePalette;
			float lineH		= PixelFont.CharH * scale;
			int boxH 		= 110;
			int boxY		= vp.Height - boxH - 8;
			int textX		= 20;
			int textMaxW	= vp.Width - 40;
			var border		= DialogueBorderColor;
			
			sb.Draw( _pixel, new Rectangle( 8,				boxY, 				vp.Width - 16,	boxH ),	sp.UiBackground * 0.85f );
			sb.Draw( _pixel, new Rectangle( 8,				boxY,				vp.Width - 16,	2 ),	border );
			sb.Draw( _pixel, new Rectangle( 8,				boxY + boxH - 2,	vp.Width - 16,	2 ),	border );
			sb.Draw( _pixel, new Rectangle( 8,				boxY,				2,				boxH ),	border );
			sb.Draw( _pixel, new Rectangle( vp.Width - 10,	boxY,				2,				boxH ),	border );

			var line = Engine.DialogueSystem.CurrentLine;
			if( line == null ) { return; }

			float cy =	boxY + 8;
			if( !string.IsNullOrEmpty( line.Speaker ) )
			{
				font.DrawText( sb, line.Speaker, new Vector2( textX, cy ), DialogueSpeakerColor, scale );
				cy += lineH + 2;
			}

			float bodyH = font.DrawWrappedText( sb, Engine.DialogueSystem.DisplayedText,
				new Vector2( textX, cy ), sp.UiText, textMaxW, scale );
			float afterBody = cy + bodyH + 4;

			if( Engine.DialogueSystem.WaitingForChoice )
			{
				var choices = Engine.DialogueSystem.VisibleChoices;
				for( int i = 0; i < choices.Length; i++ )
				{
					bool sel		= i == Engine.DialogueSystem.SelectedChoiceIndex;
					bool enabled	= choices[i].EnabledCondition?.Invoke()	?? true;
					Color color		= !enabled ? sp.UiDim :	(sel ? sp.UiAccent : sp.UiChoice);
					font.DrawText( sb, (sel && enabled ? "> " : "  ") + choices[i].Text,
						new Vector2( textX, afterBody + i * lineH ), color,	scale );
				}
			}
			else if( Engine.DialogueSystem.IsTextComplete )
			{
				if( (int)(Engine.PlayTime.TotalSeconds * 2) % 2 == 0 )
				{
					font.DrawText( sb, "[ E ]", new Vector2( vp.Width - 68, boxY + boxH - lineH - 6 ), sp.UiDim, scale );
				}
			}
		}
	}
}
