namespace ChildhoodAdventure.RetroSystems.Commodore64;

public static partial class C64Sprites
{
    public static readonly byte[][][] Head0 =   // basic — hair top, two separate eyes
    [
        [
            [ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],   // hair top
            [ 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 ],   // face
            [ 0, 0, 1, 1, 4, 4, 1, 1, 4, 4, 0, 0 ],   // L eye · skin · R eye
            [ 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 ],   // lower face
            [ 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0 ],   // chin
            [ 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0 ],   // neck
        ],
    ];

    public static readonly byte[][][] Head1 =   // cap / hat
    [
        [
            [ 0, 0, 5, 5, 5, 5, 5, 5, 5, 5, 0, 0 ],   // hat top
            [ 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 ],   // hat brim (full width)
            [ 0, 0, 1, 1, 4, 4, 1, 1, 4, 4, 0, 0 ],   // eyes
            [ 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 ],   // lower face
            [ 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0 ],   // chin
            [ 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0 ],   // neck
        ],
    ];

    public static readonly byte[][][] Head2 =   // long hair — flows left side
    [
        [
            [ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],   // hair all across top
            [ 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 ],   // hair left + face
            [ 2, 2, 1, 1, 4, 4, 1, 1, 4, 4, 0, 0 ],   // hair left + eyes
            [ 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 ],   // hair left + chin
            [ 2, 2, 2, 2, 1, 1, 1, 1, 0, 0, 0, 0 ],   // hair falling + narrow chin
            [ 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0 ],   // neck
        ],
    ];

    public static readonly byte[][][] Head0Back =   // basic — short hair, back view
    [
        [
            [ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],   // hair top
            [ 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],   // hair
            [ 0, 0, 2, 2, 1, 1, 1, 1, 2, 2, 0, 0 ],   // short hair — skin centre shows
            [ 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 ],   // back of neck
            [ 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0 ],   // chin
            [ 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0 ],   // neck
        ],
    ];

    public static readonly byte[][][] Head1Back =   // cap — back of hat
    [
        [
            [ 0, 0, 5, 5, 5, 5, 5, 5, 5, 5, 0, 0 ],   // hat back
            [ 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 ],   // hat brim
            [ 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 ],   // back of head
            [ 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 ],   // back of neck
            [ 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0 ],   // chin
            [ 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0 ],   // neck
        ],
    ];

    public static readonly byte[][][] Head2Back =   // long hair — flows out from back
    [
        [
            [ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],   // hair all across top
            [ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],   // hair
            [ 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0 ],   // hair (long — covers head)
            [ 2, 2, 1, 1, 1, 1, 1, 1, 2, 2, 0, 0 ],   // hair sides + neck centre
            [ 2, 2, 2, 2, 1, 1, 1, 1, 0, 0, 0, 0 ],   // hair + chin
            [ 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0 ],   // neck
        ],
    ];

    public static readonly byte[][][] Head0Side =   // basic — right-facing profile
    [
        [
            [ 0, 0, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],   // hair top
            [ 0, 0, 2, 2, 1, 1, 1, 1, 0, 0, 0, 0 ],   // hair trailing + face
            [ 0, 0, 1, 1, 1, 1, 4, 4, 0, 0, 0, 0 ],   // face + eye (forward side)
            [ 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // lower face
            [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0 ],   // chin + nose protrudes right
            [ 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0 ],   // neck
        ],
    ];

    public static readonly byte[][][] Head1Side =   // cap — profile with hat brim forward
    [
        [
            [ 0, 0, 5, 5, 5, 5, 5, 5, 0, 0, 0, 0 ],   // hat top
            [ 0, 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 ],   // hat brim extends forward (right)
            [ 0, 0, 1, 1, 1, 1, 4, 4, 0, 0, 0, 0 ],   // face + eye
            [ 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // lower face
            [ 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0 ],   // chin + nose
            [ 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0 ],   // neck
        ],
    ];

    public static readonly byte[][][] Head2Side =   // long hair — profile, hair trails left
    [
        [
            [ 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0 ],   // hair all across top
            [ 2, 2, 2, 2, 1, 1, 1, 1, 0, 0, 0, 0 ],   // hair trailing + face
            [ 2, 2, 1, 1, 1, 1, 4, 4, 0, 0, 0, 0 ],   // hair edge + face + eye
            [ 2, 2, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 ],   // hair + lower face
            [ 2, 2, 2, 2, 1, 1, 1, 1, 1, 1, 0, 0 ],   // hair + chin + nose
            [ 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0 ],   // neck
        ],
    ];
}
