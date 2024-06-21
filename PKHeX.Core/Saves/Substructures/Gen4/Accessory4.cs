using System;

namespace PKHeX.Core;

public enum Accessory4 : byte
{
    WhiteFluff,
    YellowFluff,
    PinkFluff,
    BrownFluff,
    BlackFluff,
    OrangeFluff,
    RoundPebble,
    GlitterBoulder,
    SnaggyPebble,
    JaggedBoulder,
    BlackPebble,
    MiniPebble,
    PinkScale,
    BlueScale,
    GreenScale,
    PurpleScale,
    BigScale,
    NarrowScale,
    BlueFeather,
    RedFeather,
    YellowFeather,
    WhiteFeather,
    BlackMoustache,
    WhiteMoustache,
    BlackBeard,
    WhiteBeard,
    SmallLeaf,
    BigLeaf,
    NarrowLeaf,
    ShedClaw,
    ShedHorn,
    ThinMushroom,
    ThickMushroom,
    Stump,
    PrettyDewdrop,
    SnowCrystal,
    Sparks,
    ShimmeringFire,
    MysticFire,
    Determination,
    PeculiarSpoon,
    PuffySmoke,
    PoisonExtract,
    WealthyCoin,
    EerieThing,
    Spring,
    Seashell,
    HummingNote,
    ShinyPowder,
    GlitterPowder,
    RedFlower,
    PinkFlower,
    WhiteFlower,
    BlueFlower,
    OrangeFlower,
    YellowFlower,
    GooglySpecs,
    BlackSpecs,
    GorgeousSpecs,
    SweetCandy,
    Confetti,

    // For accessories below this point, only 1 copy can be owned at once
    ColoredParasol,
    OldUmbrella,
    Spotlight,
    Cape,
    StandingMike,
    Surfboard,
    Carpet,
    RetroPipe,
    FluffyBed,
    MirrorBall,
    PhotoBoard,
    PinkBarrette,
    RedBarrette,
    BlueBarrette,
    YellowBarrette,
    GreenBarrette,
    PinkBalloon,
    RedBalloons,
    BlueBalloons,
    YellowBalloon,
    GreenBalloons,
    LaceHeadress,
    TopHat,
    SilkVeil,
    HeroicHeadband,
    ProfessorHat,
    FlowerStage,
    GoldPedestal,
    GlassStage,
    AwardPodium,
    CubeStage,
    TURTWIGMask,
    CHIMCHARMask,
    PIPLUPMask,
    BigTree,
    Flag,
    Crown,
    Tiara,

    // Unreleased
    Comet,
}

public static class AccessoryInfo
{
    public const int Count = 100;
    public const int MaxMulti = (int)Accessory4.Confetti;
    public const int MaxLegal = (int)Accessory4.Tiara;
    public const byte AccessoryMaxCount = 9;

    public static bool IsMultiple(this Accessory4 acc) => (uint)acc <= MaxMulti;
    public static bool IsSingle(this Accessory4 acc) => (uint)acc > MaxMulti;

    public static int GetSingleBitIndex(this Accessory4 acc)
    {
        var index = (int)acc - MaxMulti - 1;
        ArgumentOutOfRangeException.ThrowIfLessThan(index, 0, nameof(acc));
        return index;
    }
}
