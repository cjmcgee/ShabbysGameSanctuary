namespace ChildhoodAdventure.RetroSystems.AppleII;

public static partial class AppleIISprites
{
    public static readonly byte[][][] Body0 =   // casual shirt
    [
        [
            [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // neck
            [ 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0 ],   // shoulders
            [ 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0 ],   // chest
            [ 0, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 2, 2, 0 ],   // highlight band
            [ 0, 2, 2, 2, 4, 4, 2, 2, 4, 4, 2, 2, 2, 0 ],   // two buttons
            [ 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0 ],   // lower
        ],
    ];

    public static readonly byte[][][] Body1 =   // collared / formal
    [
        [
            [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // neck
            [ 0, 5, 5, 5, 5, 2, 2, 2, 2, 5, 5, 5, 5, 0 ],   // lapels around tie
            [ 0, 5, 5, 5, 5, 4, 4, 4, 4, 5, 5, 5, 5, 0 ],   // lapels + tie buttons
            [ 0, 5, 5, 5, 5, 2, 2, 2, 2, 5, 5, 5, 5, 0 ],   // lapels lower
            [ 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0 ],   // torso
            [ 0, 2, 2, 2, 2, 4, 4, 4, 4, 2, 2, 2, 2, 0 ],   // lower buttons
        ],
    ];

    public static readonly byte[][][] Body2 =   // jacket / hoodie
    [
        [
            [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // neck
            [ 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0 ],   // jacket full
            [ 0, 5, 5, 5, 5, 2, 2, 2, 2, 5, 5, 5, 5, 0 ],   // open jacket + shirt
            [ 0, 5, 5, 5, 5, 3, 3, 4, 4, 3, 3, 5, 5, 0 ],   // highlight + button
            [ 0, 5, 5, 5, 5, 2, 2, 2, 2, 5, 5, 5, 5, 0 ],   // shirt
            [ 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0 ],   // jacket hem
        ],
    ];

    public static readonly byte[][][] Body0Back =
    [[
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // neck
        [ 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0 ],   // back shoulders
        [ 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0 ],   // back
        [ 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0 ],   // back (no buttons)
        [ 0, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 2, 2, 0 ],   // highlight band
        [ 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0 ],   // lower
    ]];

    public static readonly byte[][][] Body1Back =
    [[
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // neck
        [ 0, 5, 5, 5, 5, 2, 2, 2, 2, 5, 5, 5, 5, 0 ],   // collar back
        [ 0, 5, 5, 5, 5, 2, 2, 2, 2, 5, 5, 5, 5, 0 ],   // collar
        [ 0, 5, 5, 5, 5, 2, 2, 2, 2, 5, 5, 5, 5, 0 ],   // lapels back
        [ 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0 ],   // torso
        [ 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0 ],   // lower
    ]];

    public static readonly byte[][][] Body2Back =
    [[
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // neck
        [ 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0 ],   // jacket back
        [ 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0 ],
        [ 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0 ],
        [ 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0 ],
        [ 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0 ],
    ]];
}
