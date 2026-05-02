namespace ChildhoodAdventure.RetroSystems.AppleII;

public static partial class AppleIISprites
{
    public static readonly byte[][][] Head0 =   // basic round head
    [
        [
            [ 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0 ],   // hair crown
            [ 0, 0, 2, 2, 1, 1, 1, 1, 1, 1, 2, 2, 0, 0 ],   // hair sides + face
            [ 0, 0, 1, 1, 4, 4, 1, 1, 4, 4, 1, 1, 0, 0 ],   // face + two eyes
            [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // chin
        ],
    ];

    public static readonly byte[][][] Head1 =   // cap / hat
    [
        [
            [ 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0 ],   // hat top
            [ 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0 ],   // hat brim
            [ 0, 0, 1, 1, 4, 4, 1, 1, 4, 4, 1, 1, 0, 0 ],   // face + eyes
            [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // chin
        ],
    ];

    public static readonly byte[][][] Head2 =   // long / full hair
    [
        [
            [ 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0 ],   // full hair top
            [ 0, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 0 ],   // hair sides + face
            [ 0, 2, 2, 1, 4, 4, 1, 1, 4, 4, 1, 2, 2, 0 ],   // hair sides + eyes + face
            [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // chin
        ],
    ];

    public static readonly byte[][][] Head0Back =
    [[
        [ 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0 ],   // hair crown
        [ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],   // hair back (no eyes)
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // upper neck
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // neck
    ]];

    public static readonly byte[][][] Head1Back =
    [[
        [ 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0 ],   // hat top (full brim from behind)
        [ 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0 ],   // hat brim
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // upper neck
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // neck
    ]];

    public static readonly byte[][][] Head2Back =
    [[
        [ 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0 ],   // full hair
        [ 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0 ],   // full hair back
        [ 0, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 0 ],   // hair sides + neck
        [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // neck
    ]];
}
