using ChildhoodAdventure.RetroSystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TileEngine.Collision;
using TileEngine.Components;
using TileEngine.Core;
using TileEngine.ECS;
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
        private readonly List<(Entity Npc, Action TalkFn)> _npcTalks = new();
        private KeyboardState _prevKeys;
        private Texture2D? _pixel;

        protected bool Transitioning;

        // ── Customization points ─────────────────────────────────────────────

        // All in tile-space units: speed in tiles/sec, radius in tiles.
        protected virtual float PlayerMaxSpeed    => 5.6f;   // ~5.6 tiles/sec
        protected virtual float CameraFollowSpeed => 8f;
        protected virtual float InteractionRadius => 1.875f; // ~30 px at old 16-px scale

        protected abstract Color DialogueBorderColor { get; }
        protected virtual  Color DialogueSpeakerColor => Color.Yellow;

        // ── Lifecycle ────────────────────────────────────────────────────────

        protected override sealed void OnLoad()
        {
            Engine.DialogueSystem.EnsureYarnLoaded(GetType().Assembly, AdventureGame.DialogueBundleResource);
            Engine.DialogueSystem.RegisterCommandHandler("flag",
                args => { if (args.Length > 0) GameState.SetFlag(args[0]); });
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

        protected Entity SpawnPlayer(GraphicsDevice gd, Vector2 pos)
        {
            var e = Engine.EntityWorld.CreateEntity("Player");
            Engine.EntityWorld.RegisterTag("player", e);
            e.AddComponent(new TransformComponent(pos) { MaxSpeed = PlayerMaxSpeed });
            // Hitbox: 0.625 × 0.375 tiles centred on the entity position (was 10×6 px).
            e.AddComponent(new CollisionComponent(0.625f, 0.375f, new Vector2(-0.3125f, -0.1875f)));
            var sprite = SpriteFactory.BuildCharacter(gd, NpcAppearances.Player);
            sprite.FrameTileSize *= 0.5f;     // child-sized
            e.AddComponent(new SpriteComponent { Sprite = sprite });
            Engine.RenderSystem.Camera.FollowTarget = pos;
            Engine.RenderSystem.Camera.FollowSpeed  = CameraFollowSpeed;
            Engine.RenderSystem.Camera.CenterOn(pos);
            _player = e;
            return e;
        }

        protected Entity SpawnNpc(GraphicsDevice gd, string name, Vector2 pos,
            CharacterAppearance appearance, Action talkFn, float scale = 1f)
        {
            var e = Engine.EntityWorld.CreateEntity(name);
            e.AddComponent(new TransformComponent(pos));
            // Hitbox: 0.625 × 0.5 tiles centred on the entity position (was 10×8 px).
            e.AddComponent(new CollisionComponent(0.625f, 0.5f, new Vector2(-0.3125f, -0.25f)) { IsSolid = true });
            var sprite = SpriteFactory.BuildCharacter(gd, appearance);
            if (scale != 1f) sprite.FrameTileSize *= scale;
            e.AddComponent(new SpriteComponent { Sprite = sprite });
            _npcTalks.Add((e, talkFn));
            return e;
        }

        // ── Update ────────────────────────────────────────────────────────────

        public override sealed void Update(GameTime gameTime)
        {
            var keys = Keyboard.GetState();
            bool wasDialogueActive = Engine.DialogueSystem.IsActive;

            HandleDialogueInput(keys);

            if (!Engine.DialogueSystem.IsActive && !Transitioning)
            {
                HandlePlayerMovement(keys, gameTime);
                if (!wasDialogueActive)
                    HandleInteraction(keys);
                CheckSceneTransitions();
            }

            _prevKeys = keys;
        }

        protected abstract void CheckSceneTransitions();

        private void HandleDialogueInput(KeyboardState keys)
        {
            if (!Engine.DialogueSystem.IsActive) return;

            if (Engine.DialogueSystem.WaitingForChoice)
            {
                if (keys.IsKeyDown(Keys.Up)   && !_prevKeys.IsKeyDown(Keys.Up))
                    Engine.DialogueSystem.MoveChoiceSelection(-1);
                if (keys.IsKeyDown(Keys.Down) && !_prevKeys.IsKeyDown(Keys.Down))
                    Engine.DialogueSystem.MoveChoiceSelection(1);
                if (keys.IsKeyDown(Keys.E) && !_prevKeys.IsKeyDown(Keys.E))
                    Engine.DialogueSystem.SelectChoice(Engine.DialogueSystem.SelectedChoiceIndex);
            }
            else if (keys.IsKeyDown(Keys.E) && !_prevKeys.IsKeyDown(Keys.E))
            {
                Engine.DialogueSystem.Advance();
            }
        }

        private void HandlePlayerMovement(KeyboardState keys, GameTime gameTime)
        {
            if (_player == null) return;
            var t  = _player.GetComponent<TransformComponent>();
            var sc = _player.GetComponent<SpriteComponent>();
            if (t == null) return;

            var move = Vector2.Zero;
            if (keys.IsKeyDown(Keys.W) || keys.IsKeyDown(Keys.Up))    move.Y -= 1;
            if (keys.IsKeyDown(Keys.S) || keys.IsKeyDown(Keys.Down))  move.Y += 1;
            if (keys.IsKeyDown(Keys.A) || keys.IsKeyDown(Keys.Left))  move.X -= 1;
            if (keys.IsKeyDown(Keys.D) || keys.IsKeyDown(Keys.Right)) move.X += 1;

            if (move != Vector2.Zero)
            {
                var dir = Vector2.Normalize(move);
                t.Velocity = dir * t.MaxSpeed;
                t.Facing   = dir;
                sc?.Play("walk");
            }
            else
            {
                t.Velocity = Vector2.Zero;
                sc?.Play("idle");
            }

            Engine.CollisionSystem.MoveAndSlide(_player, (float)gameTime.ElapsedGameTime.TotalSeconds);
            t.Velocity = Vector2.Zero;
            Engine.RenderSystem.Camera.FollowTarget = t.Position;
        }

        private void HandleInteraction(KeyboardState keys)
        {
            if (!keys.IsKeyDown(Keys.E) || _prevKeys.IsKeyDown(Keys.E)) return;
            if (_player == null) return;

            var pt = _player.GetComponent<TransformComponent>()?.Position ?? Vector2.Zero;

            foreach (var (npc, talkFn) in _npcTalks)
            {
                var nt = npc.GetComponent<TransformComponent>()?.Position ?? Vector2.Zero;
                if (Vector2.Distance(pt, nt) < InteractionRadius)
                {
                    talkFn();
                    return;
                }
            }
        }

        // ── Scene-transition helper ──────────────────────────────────────────

        protected Vector2 PlayerPosition =>
            _player?.GetComponent<TransformComponent>()?.Position ?? Vector2.Zero;

        // ── Draw ──────────────────────────────────────────────────────────────

        public override sealed void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Engine.RenderSystem.DrawScene(Engine.EntityWorld, gameTime, Color.Black);

            Engine.RenderSystem.BeginUI();
            if (Engine.DialogueSystem.IsActive)
                DrawDialogueBox(Engine.RenderSystem.SpriteBatch);
            Engine.RenderSystem.EndUI();
        }

        private void DrawDialogueBox(SpriteBatch sb)
        {
            if (_pixel == null)
            {
                _pixel = new Texture2D(Engine.GraphicsDevice, 1, 1);
                _pixel.SetData(new[] { Color.White });
            }

            var vp    = Engine.GraphicsDevice.Viewport;
            var font  = Engine.RenderSystem.Font;
            const float scale = 2f;
            float lineH = PixelFont.CharH * scale;
            int boxH = 110, boxY = vp.Height - boxH - 8;
            int textX = 20, textMaxW = vp.Width - 40;

            var border = DialogueBorderColor;
            sb.Draw(_pixel, new Rectangle(8, boxY, vp.Width - 16, boxH), new Color(0, 0, 0, 215));
            sb.Draw(_pixel, new Rectangle(8, boxY,              vp.Width - 16, 2),    border);
            sb.Draw(_pixel, new Rectangle(8, boxY + boxH - 2,   vp.Width - 16, 2),    border);
            sb.Draw(_pixel, new Rectangle(8, boxY,              2,             boxH), border);
            sb.Draw(_pixel, new Rectangle(vp.Width - 10, boxY,  2,             boxH), border);

            var line = Engine.DialogueSystem.CurrentLine;
            if (line == null) return;

            float cy = boxY + 8;
            if (!string.IsNullOrEmpty(line.Speaker))
            {
                font.DrawText(sb, line.Speaker, new Vector2(textX, cy), DialogueSpeakerColor, scale);
                cy += lineH + 2;
            }

            float bodyH = font.DrawWrappedText(sb, Engine.DialogueSystem.DisplayedText,
                new Vector2(textX, cy), Color.White, textMaxW, scale);
            float afterBody = cy + bodyH + 4;

            if (Engine.DialogueSystem.WaitingForChoice)
            {
                var choices = Engine.DialogueSystem.VisibleChoices;
                for (int i = 0; i < choices.Length; i++)
                {
                    bool sel     = i == Engine.DialogueSystem.SelectedChoiceIndex;
                    bool enabled = choices[i].EnabledCondition?.Invoke() ?? true;
                    Color color  = !enabled ? Color.DarkGray
                                 : sel      ? Color.Yellow
                                            : Color.LightGray;
                    font.DrawText(sb, (sel && enabled ? "> " : "  ") + choices[i].Text,
                        new Vector2(textX, afterBody + i * lineH), color, scale);
                }
            }
            else if (Engine.DialogueSystem.IsTextComplete)
            {
                if ((int)(Engine.PlayTime.TotalSeconds * 2) % 2 == 0)
                    font.DrawText(sb, "[ E ]",
                        new Vector2(vp.Width - 68, boxY + boxH - lineH - 6), Color.Gray, scale);
            }
        }
    }
}
