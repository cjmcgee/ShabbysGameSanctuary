namespace ChildhoodAdventure.RetroSystems.NES;

public static partial class NESSprites
{
    public const int CharWidth = 16;
    public const int BodyRows = 9;

    public static BodyPalette[] BodyPalettes { get; } =
        [
            new("green",
                Skin:           NESPalette.SkinFair,
                Shirt:          NESPalette.NesGreen,   // NES green
                ShirtHighlight: NESPalette.Aqua,
                Buttons:        NESPalette.NearWhite,
                Accessory:      NESPalette.NesBlue),
            new("blue",
                Skin:           NESPalette.SkinFair,
                Shirt:          NESPalette.NesBlue,   // NES blue
                ShirtHighlight: NESPalette.Aqua,
                Buttons:        NESPalette.WallGray,
                Accessory:      NESPalette.BrightYellow),
            new("red",
                Skin:           NESPalette.SkinFair,
                Shirt:          NESPalette.DarkRed,   // NES dark red
                ShirtHighlight: NESPalette.VividRed,
                Buttons:        NESPalette.NearWhite,
                Accessory:      NESPalette.BrightYellow),
            new("white/light",
                Skin:           NESPalette.SkinFair,
                Shirt:          NESPalette.WallGray,
                ShirtHighlight: NESPalette.NearWhite,
                Buttons:        NESPalette.NesBlue,
                Accessory:      NESPalette.NesGreen),
            new("teal",
                Skin:           NESPalette.SkinFair,
                Shirt:          NESPalette.Aqua,   // NES aqua
                ShirtHighlight: NESPalette.PaleCyan,
                Buttons:        NESPalette.NearWhite,
                Accessory:      NESPalette.MidMagenta),
        ];

    public static readonly byte[][][] Body0 =   // casual shirt
    [
        [
            [ 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0 ],   // neck skin
            [ 0, 0, 0, 1, 2, 2, 2, 2, 2, 2, 1, 0, 0, 0, 0, 0 ],   // shoulders
            [ 0, 0, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 0, 0, 0, 0 ],   // upper chest
            [ 0, 0, 2, 3, 2, 4, 4, 2, 2, 3, 2, 2, 0, 0, 0, 0 ],   // highlights + buttons
            [ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
            [ 0, 0, 2, 3, 2, 2, 4, 4, 2, 3, 2, 2, 0, 0, 0, 0 ],   // lower buttons
            [ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
            [ 0, 0, 3, 2, 2, 2, 2, 2, 2, 2, 2, 3, 0, 0, 0, 0 ],   // shadow sides
            [ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
        ],
    ];

    public static readonly byte[][][] Body1 =   // collared / formal
    [
        [
            [ 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0 ],   // neck skin
            [ 0, 0, 0, 1, 2, 5, 2, 2, 5, 2, 1, 0, 0, 0, 0, 0 ],   // lapels start
            [ 0, 0, 1, 2, 2, 5, 2, 2, 5, 2, 2, 1, 0, 0, 0, 0 ],   // lapels chest
            [ 0, 0, 2, 2, 2, 2, 4, 4, 2, 2, 2, 2, 0, 0, 0, 0 ],   // center buttons
            [ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
            [ 0, 0, 2, 3, 2, 2, 4, 4, 2, 2, 3, 2, 0, 0, 0, 0 ],   // more buttons
            [ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
            [ 0, 0, 3, 2, 2, 2, 2, 2, 2, 2, 2, 3, 0, 0, 0, 0 ],   // shadow sides
            [ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
        ],
    ];

    public static readonly byte[][][] Body2 =   // jacket / hoodie
    [
        [
            [ 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0 ],   // neck skin
            [ 0, 0, 5, 5, 5, 2, 2, 2, 5, 5, 5, 0, 0, 0, 0, 0 ],   // jacket outer
            [ 0, 0, 5, 2, 2, 2, 2, 2, 2, 2, 5, 0, 0, 0, 0, 0 ],   // open front
            [ 0, 0, 5, 2, 3, 4, 4, 3, 2, 2, 5, 0, 0, 0, 0, 0 ],   // highlights + buttons
            [ 0, 0, 5, 2, 2, 2, 2, 2, 2, 2, 5, 0, 0, 0, 0, 0 ],
            [ 0, 0, 5, 2, 3, 2, 4, 2, 3, 2, 5, 0, 0, 0, 0, 0 ],
            [ 0, 0, 5, 2, 2, 2, 2, 2, 2, 2, 5, 0, 0, 0, 0, 0 ],
            [ 0, 0, 5, 5, 2, 2, 2, 2, 2, 5, 5, 0, 0, 0, 0, 0 ],
            [ 0, 0, 5, 5, 2, 2, 2, 2, 2, 5, 5, 0, 0, 0, 0, 0 ],
        ],
    ];

    public static readonly byte[][][] Body0Back =
    [
        [
            [ 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0 ],   // neck
            [ 0, 0, 0, 1, 2, 2, 2, 2, 2, 2, 1, 0, 0, 0, 0, 0 ],   // shoulders
            [ 0, 0, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 0, 0, 0, 0 ],   // upper back
            [ 0, 0, 2, 3, 2, 2, 2, 2, 3, 2, 2, 2, 0, 0, 0, 0 ],   // highlights (no buttons)
            [ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
            [ 0, 0, 2, 3, 2, 2, 2, 2, 3, 2, 2, 2, 0, 0, 0, 0 ],   // lower back highlights
            [ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
            [ 0, 0, 3, 2, 2, 2, 2, 2, 2, 2, 2, 3, 0, 0, 0, 0 ],   // shadow sides
            [ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
        ],
    ];

    public static readonly byte[][][] Body1Back =
    [
        [
            [ 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0 ],   // neck
            [ 0, 0, 0, 1, 2, 5, 2, 2, 5, 2, 1, 0, 0, 0, 0, 0 ],   // collar from behind
            [ 0, 0, 1, 2, 2, 5, 2, 2, 5, 2, 2, 1, 0, 0, 0, 0 ],   // collar
            [ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],   // shirt back (no buttons)
            [ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
            [ 0, 0, 2, 3, 2, 2, 2, 2, 2, 3, 2, 2, 0, 0, 0, 0 ],
            [ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
            [ 0, 0, 3, 2, 2, 2, 2, 2, 2, 2, 2, 3, 0, 0, 0, 0 ],
            [ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],
        ],
    ];

    public static readonly byte[][][] Body2Back =
    [
        [
            [ 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0 ],   // neck
            [ 0, 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0, 0, 0, 0, 0 ],   // jacket back
            [ 0, 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0, 0, 0, 0, 0 ],   // jacket
            [ 0, 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0, 0, 0, 0, 0 ],
            [ 0, 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0, 0, 0, 0, 0 ],
            [ 0, 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0, 0, 0, 0, 0 ],
            [ 0, 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0, 0, 0, 0, 0 ],
            [ 0, 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0, 0, 0, 0, 0 ],
            [ 0, 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0, 0, 0, 0, 0 ],
        ],
    ];

    public static readonly byte[][][] Body0Side =
    [
        [
            [ 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0 ],   // neck (narrower)
            [ 0, 0, 0, 1, 2, 2, 2, 2, 1, 0, 0, 0, 0, 0, 0, 0 ],   // shoulder
            [ 0, 0, 1, 2, 2, 2, 2, 2, 1, 0, 0, 0, 0, 0, 0, 0 ],   // upper torso
            [ 0, 0, 2, 3, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0 ],   // highlight
            [ 0, 0, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 2, 3, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 3, 2, 2, 2, 2, 3, 0, 0, 0, 0, 0, 0, 0, 0 ],   // arm visible at front
            [ 0, 0, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
    ];

    public static readonly byte[][][] Body1Side =
    [
        [
            [ 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0 ],   // neck
            [ 0, 0, 0, 1, 2, 5, 2, 5, 0, 0, 0, 0, 0, 0, 0, 0 ],   // lapel side
            [ 0, 0, 1, 2, 2, 5, 2, 5, 0, 0, 0, 0, 0, 0, 0, 0 ],   // lapel
            [ 0, 0, 2, 2, 2, 2, 4, 2, 0, 0, 0, 0, 0, 0, 0, 0 ],   // button
            [ 0, 0, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 2, 3, 2, 2, 4, 2, 0, 0, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 3, 2, 2, 2, 2, 3, 0, 0, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
    ];

    public static readonly byte[][][] Body2Side =
    [
        [
            [ 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0 ],   // neck
            [ 0, 0, 5, 5, 5, 2, 5, 5, 0, 0, 0, 0, 0, 0, 0, 0 ],   // jacket side
            [ 0, 0, 5, 2, 2, 2, 2, 5, 0, 0, 0, 0, 0, 0, 0, 0 ],   // open front
            [ 0, 0, 5, 2, 3, 4, 2, 5, 0, 0, 0, 0, 0, 0, 0, 0 ],   // highlight + button
            [ 0, 0, 5, 2, 2, 2, 2, 5, 0, 0, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 5, 2, 3, 4, 2, 5, 0, 0, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 5, 2, 2, 2, 2, 5, 0, 0, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 5, 5, 2, 2, 5, 5, 0, 0, 0, 0, 0, 0, 0, 0 ],
            [ 0, 0, 5, 5, 2, 2, 5, 5, 0, 0, 0, 0, 0, 0, 0, 0 ],
        ],
    ];
}
