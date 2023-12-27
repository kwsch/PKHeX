namespace PKHeX.Core;

internal sealed class SAV1Offsets
{
    public static readonly SAV1Offsets INT = GetINT();
    public static readonly SAV1Offsets JPN = GetJPN();

    private static SAV1Offsets GetINT() => new()
    {
        DexCaught = 0x25A3,
        DexSeen = 0x25B6,
        Items = 0x25C9,
        Money = 0x25F3,
        Rival = 0x25F6,
        Options = 0x2601,
        Badges = 0x2602,
        TID16 = 0x2605,
        PikaFriendship = 0x271C,
        PikaBeachScore = 0x2741,
        PrinterBrightness = 0x2744,
        PCItems = 0x27E6,
        CurrentBoxIndex = 0x284C,
        Coin = 0x2850,
        ObjectSpawnFlags = 0x2852, // 2 bytes after Coin
        EventWork = 0x289C,
        Starter = 0x29C3,
        EventFlag = 0x29F3,
        PlayTime = 0x2CED,
        Daycare = 0x2CF4,
        Party = 0x2F2C,
        CurrentBox = 0x30C0,
        ChecksumOfs = 0x3523,
    };

    private static SAV1Offsets GetJPN() => new()
    {
        DexCaught = 0x259E,
        DexSeen = 0x25B1,
        Items = 0x25C4,
        Money = 0x25EE,
        Rival = 0x25F1,
        Options = 0x25F7,
        Badges = 0x25F8,
        TID16 = 0x25FB,
        PikaFriendship = 0x2712,
        PikaBeachScore = 0x2737,
        PrinterBrightness = 0x273A,
        PCItems = 0x27DC,
        CurrentBoxIndex = 0x2842,
        Coin = 0x2846,
        ObjectSpawnFlags = 0x2848, // 2 bytes after Coin
        EventWork = 0x2892,
        Starter = 0x29B9,
        EventFlag = 0x29E9,
        PlayTime = 0x2CA0,
        Daycare = 0x2CA7,
        Party = 0x2ED5,
        CurrentBox = 0x302D,
        ChecksumOfs = 0x3594,
    };

    public int OT => 0x2598;
    public int DexCaught { get; private init; }
    public int DexSeen { get; private init; }
    public int Items { get; private init; }
    public int Money { get; private init; }
    public int Rival { get; private init; }
    public int Options { get; private init; }
    public int Badges { get; private init; }
    public int TID16 { get; private init; }
    public int PikaFriendship { get; private init; }
    public int PikaBeachScore { get; private init; }
    public int PrinterBrightness { get; private init; }
    public int PCItems { get; private init; }
    public int CurrentBoxIndex { get; private init; }
    public int Coin { get; private init; }
    public int ObjectSpawnFlags { get; private init; }
    public int Starter { get; private init; }
    public int EventFlag { get; private init; }
    public int EventWork { get; private init; }
    public int PlayTime { get; private init; }
    public int Daycare { get; private init; }
    public int Party { get; private init; }
    public int CurrentBox { get; private init; }
    public int ChecksumOfs { get; private init; }
}
