using Microsoft.Xna.Framework;

namespace ChildhoodAdventure
{
    /// <summary>All houses that can be entered as a neighbor's home.</summary>
    public enum HouseId { Chen, Devon, JakeAndEmma, Thompson, Santos, Petrov, Sam, Johnson }

    /// <summary>
    /// Global singleton state shared across scene transitions.
    /// Stores spawn positions, current interior target, and story flags.
    /// </summary>
    public static class GameState
    {
        // Where the player appears when entering the next scene.
        public static Vector2 PlayerSpawnPosition { get; set; }
            = new Vector2(12 * 16 + 8, 15 * 16 + 8);   // default: inside player's home

        // Which neighbor's interior to build when loading NeighborInteriorScene.
        public static HouseId? TargetInterior { get; set; }

        // Where the player re-appears in the neighborhood after exiting any interior.
        // Default: sidewalk in front of the player's own house (door at tile x=40, sidewalk y=19).
        public static Vector2 NeighborhoodReturnPosition { get; set; }
            = new Vector2(40 * 16 + 8, 19 * 16 + 8);

        // Story/conversation flags.
        private static readonly HashSet<string> _flags = new();

        public static bool HasFlag(string flag) => _flags.Contains(flag);
        public static void SetFlag(string flag)  => _flags.Add(flag);
        public static void ClearFlag(string flag) => _flags.Remove(flag);
    }
}
