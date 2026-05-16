using TileEngine.Rendering;

namespace ChildhoodAdventure
{
	/// <summary>
	/// Programmatic sprite builders — no content pipeline needed.
	/// Delegates character sprite creation to the active <see cref="RetroSystem"/>,
	/// so every system produces proportionally correct, palette-appropriate sprites.
	/// </summary>
	public static class SpriteFactory
	{
		/// <summary>
		/// Creates a character sprite assembled from head, body, and legs parts,
		/// using the shape variants and palette indices specified by <paramref name="appearance"/>.
		/// </summary>
		public static AnimatedSprite BuildCharacter( GraphicsDevice gd, CharacterAppearance appearance )
			=>	RetroSystemRegistry.Current.BuildCharacterSprite( gd, appearance );

		/// <summary>
		/// Creates a static flat-colour sprite. <paramref name="pixelW"/>/<paramref name="pixelH"/>
		/// are texture dimensions; <paramref name="tileSize"/> is the world-space size in tiles.
		/// </summary>
		public static AnimatedSprite BuildStatic( GraphicsDevice gd, int pixelW, int pixelH, Vector2 tileSize, Color color )
		{
			var tex = new Texture2D( gd, pixelW, pixelH );
			tex.SetData( Enumerable.Repeat( color, pixelW * pixelH).ToArray() );

			var idle =	new SpriteAnimation( "idle", [new AnimationFrame( new Rectangle( 0, 0, pixelW, pixelH ), 1f )] );

			var sprite = new AnimatedSprite( tex, pixelW, pixelH, tileSize )
				{ 
					Origin = new Vector2( pixelW / 2f, pixelH / 2f ) 
				};
			sprite.AddAnimation( idle );
			sprite.Play( "idle" );
			return sprite;
		}
	}
}
