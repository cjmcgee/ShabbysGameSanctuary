namespace ChildhoodAdventure.RetroSystems.AppleII;

public static partial class AppleIITiles
{
    // White brick faces (3) with dotted blue (5) mortar lines — palette 1 mortar
    // Real mortar can't be a continuous blue run (would render white), so the
    // mortar courses are single dotted scanlines and brick rows separate via
    // single-column 0 grout lines.
    public static readonly byte[][] Wall =
    [
        [ 0, 3, 3, 3, 3, 3, 0, 3, 3, 3, 3, 3, 3, 0 ],   // brick row (col 6 grout)
        [ 0, 3, 3, 3, 3, 3, 0, 3, 3, 3, 3, 3, 3, 0 ],
        [ 0, 0, 5, 0, 5, 0, 5, 0, 5, 0, 5, 0, 5, 0 ],   // blue mortar (dotted)
        [ 0, 3, 3, 3, 3, 3, 3, 3, 3, 0, 3, 3, 3, 0 ],   // staggered (col 9 grout)
        [ 0, 3, 3, 3, 3, 3, 3, 3, 3, 0, 3, 3, 3, 0 ],
        [ 0, 0, 5, 0, 5, 0, 5, 0, 5, 0, 5, 0, 5, 0 ],   // mortar
        [ 0, 3, 3, 3, 0, 3, 3, 3, 3, 3, 3, 3, 3, 0 ],   // staggered (col 4 grout)
        [ 0, 3, 3, 3, 0, 3, 3, 3, 3, 3, 3, 3, 3, 0 ],
        [ 0, 0, 5, 0, 5, 0, 5, 0, 5, 0, 5, 0, 5, 0 ],   // mortar
        [ 0, 3, 3, 3, 3, 3, 0, 3, 3, 3, 3, 3, 3, 0 ],   // brick row (col 6 grout)
        [ 0, 3, 3, 3, 3, 3, 0, 3, 3, 3, 3, 3, 3, 0 ],
        [ 0, 0, 5, 0, 5, 0, 5, 0, 5, 0, 5, 0, 5, 0 ],   // mortar
        [ 0, 3, 3, 3, 3, 3, 3, 3, 3, 0, 3, 3, 3, 0 ],   // staggered
        [ 0, 3, 3, 3, 3, 3, 3, 3, 3, 0, 3, 3, 3, 0 ],
    ];

    // Black frame (0), violet (2) recessed panel dots, white knob (3) — palette 0
    // Violet "panels" are dotted (every even col with 0 between) — solid violet
    // is impossible under Apple II rules.
    public static readonly byte[][] Door =
    [
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],   // top frame
        [ 0, 0, 2, 0, 2, 0, 2, 0, 2, 0, 2, 0, 2, 0 ],   // upper panel (violet dots)
        [ 0, 0, 2, 0, 2, 0, 2, 0, 2, 0, 2, 0, 2, 0 ],
        [ 0, 0, 2, 0, 2, 0, 2, 0, 2, 0, 2, 0, 2, 0 ],
        [ 0, 0, 2, 0, 2, 0, 2, 0, 2, 0, 2, 0, 2, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],   // mid rail
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 3, 0, 0, 0 ],   // white knob (cols 9-10)
        [ 0, 0, 2, 0, 2, 0, 2, 0, 2, 0, 2, 0, 2, 0 ],   // lower panel
        [ 0, 0, 2, 0, 2, 0, 2, 0, 2, 0, 2, 0, 2, 0 ],
        [ 0, 0, 2, 0, 2, 0, 2, 0, 2, 0, 2, 0, 2, 0 ],
        [ 0, 0, 2, 0, 2, 0, 2, 0, 2, 0, 2, 0, 2, 0 ],
        [ 0, 0, 2, 0, 2, 0, 2, 0, 2, 0, 2, 0, 2, 0 ],
        [ 0, 0, 2, 0, 2, 0, 2, 0, 2, 0, 2, 0, 2, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],   // bottom frame
    ];

    // Black frame (0), blue (5) dotted glass panes — palette 1, two panes
    public static readonly byte[][] Window =
    [
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],   // top frame
        [ 0, 0, 5, 0, 5, 0, 0, 0, 5, 0, 5, 0, 0, 0 ],   // upper panes (blue dots)
        [ 0, 0, 5, 0, 5, 0, 0, 0, 5, 0, 5, 0, 0, 0 ],
        [ 0, 0, 5, 0, 5, 0, 0, 0, 5, 0, 5, 0, 0, 0 ],
        [ 0, 0, 5, 0, 5, 0, 0, 0, 5, 0, 5, 0, 0, 0 ],
        [ 0, 0, 5, 0, 5, 0, 0, 0, 5, 0, 5, 0, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],   // crossbar
        [ 0, 0, 5, 0, 5, 0, 0, 0, 5, 0, 5, 0, 0, 0 ],   // lower panes
        [ 0, 0, 5, 0, 5, 0, 0, 0, 5, 0, 5, 0, 0, 0 ],
        [ 0, 0, 5, 0, 5, 0, 0, 0, 5, 0, 5, 0, 0, 0 ],
        [ 0, 0, 5, 0, 5, 0, 0, 0, 5, 0, 5, 0, 0, 0 ],
        [ 0, 0, 5, 0, 5, 0, 0, 0, 5, 0, 5, 0, 0, 0 ],
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],   // bottom frame
        [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
    ];
}
