using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 5 <see cref="SaveFile"/> object for <see cref="GameVersion.FRLG"/>.
/// </summary>
/// <inheritdoc cref="SAV3" />
public sealed class SAV3FRLG : SAV3, IGen3Joyful, IGen3Wonder, IDaycareRandomState<ushort>
{
    // Configuration
    protected override SAV3FRLG CloneInternal() => new(Write()) { Language = Language };
    public override GameVersion Version { get; set; } = GameVersion.FR; // allow mutation
    private PersonalTable3 _personal = PersonalTable.FR;
    public override PersonalTable3 Personal => _personal;

    public override int EventFlagCount => 8 * 288;
    public override int EventWorkCount => 0x100;
    protected override int DaycareSlotSize => SIZE_STORED + 0x3C; // 0x38 mail + 4 exp
    protected override int EggEventFlag => 0x266;
    protected override int BadgeFlagStart => 0x820;

    public SAV3FRLG(bool japanese = false) : base(japanese) => Initialize();

    public SAV3FRLG(byte[] data) : base(data)
    {
        Initialize();

        // Fix save files that have an overflow corruption with the Pokédex.
        // Future loads of this save file will cause it to be recognized as FR/LG correctly.
        if (IsCorruptPokedexFF())
            WriteUInt32LittleEndian(Small.AsSpan(0xAC), 1);
    }

    protected override int EventFlag => 0xEE0;
    protected override int EventWork => 0x1000;
    protected override int PokeDex => 0x18; // small
    protected override int DaycareOffset => 0x2F80; // large

    // storage
    private void Initialize() => Box = 0;

    public bool ResetPersonal(GameVersion g)
    {
        if (g is not (GameVersion.FR or GameVersion.LG))
            return false;
        _personal = g == GameVersion.FR ? PersonalTable.FR : PersonalTable.LG;
        Version = g;
        return true;
    }

    #region Small
    public override bool NationalDex
    {
        get => PokedexNationalMagicFRLG == PokedexNationalUnlockFRLG;
        set
        {
            PokedexNationalMagicFRLG = value ? PokedexNationalUnlockFRLG : (byte)0; // magic
            SetEventFlag(0x840, value);
            SetWork(0x4E, PokedexNationalUnlockWorkFRLG);
        }
    }

    public uint BerryPowder
    {
        get => ReadUInt32LittleEndian(Small.AsSpan(0xAF8)) ^ SecurityKey;
        set => WriteUInt32LittleEndian(Small.AsSpan(0xAF8), value ^ SecurityKey);
    }

    public ushort JoyfulJumpInRow           { get => ReadUInt16LittleEndian(Small.AsSpan(0xB00)); set => WriteUInt16LittleEndian(Small.AsSpan(0xB00), Math.Min((ushort)9999, value)); }
    // u16 field2;
    public ushort JoyfulJump5InRow          { get => ReadUInt16LittleEndian(Small.AsSpan(0xB04)); set => WriteUInt16LittleEndian(Small.AsSpan(0xB04), Math.Min((ushort)9999, value)); }
    public ushort JoyfulJumpGamesMaxPlayers { get => ReadUInt16LittleEndian(Small.AsSpan(0xB06)); set => WriteUInt16LittleEndian(Small.AsSpan(0xB06), Math.Min((ushort)9999, value)); }
    // u32 field8;
    public uint   JoyfulJumpScore           { get => ReadUInt16LittleEndian(Small.AsSpan(0xB0C)); set => WriteUInt32LittleEndian(Small.AsSpan(0xB0C), Math.Min(99990, value)); }

    public uint   JoyfulBerriesScore        { get => ReadUInt16LittleEndian(Small.AsSpan(0xB10)); set => WriteUInt32LittleEndian(Small.AsSpan(0xB10), Math.Min(99990, value)); }
    public ushort JoyfulBerriesInRow        { get => ReadUInt16LittleEndian(Small.AsSpan(0xB14)); set => WriteUInt16LittleEndian(Small.AsSpan(0xB14), Math.Min((ushort)9999, value)); }
    public ushort JoyfulBerries5InRow       { get => ReadUInt16LittleEndian(Small.AsSpan(0xB16)); set => WriteUInt16LittleEndian(Small.AsSpan(0xB16), Math.Min((ushort)9999, value)); }

    public override uint SecurityKey
    {
        get => ReadUInt32LittleEndian(Small.AsSpan(0xF20));
        set => WriteUInt32LittleEndian(Small.AsSpan(0xF20), value);
    }
    #endregion

    #region Large
    public override int PartyCount { get => Large[0x034]; protected set => Large[0x034] = (byte)value; }
    public override int GetPartyOffset(int slot) => 0x038 + (SIZE_PARTY * slot);

    public override uint Money
    {
        get => ReadUInt32LittleEndian(Large.AsSpan(0x0290)) ^ SecurityKey;
        set => WriteUInt32LittleEndian(Large.AsSpan(0x0290), value ^ SecurityKey);
    }

    public override uint Coin
    {
        get => (ushort)(ReadUInt16LittleEndian(Large.AsSpan(0x0294)) ^ SecurityKey);
        set => WriteUInt16LittleEndian(Large.AsSpan(0x0294), (ushort)(value ^ SecurityKey));
    }

    private const int OFS_PCItem = 0x0298;
    private const int OFS_PouchHeldItem = 0x0310;
    private const int OFS_PouchKeyItem = 0x03B8;
    private const int OFS_PouchBalls = 0x0430;
    private const int OFS_PouchTMHM = 0x0464;
    private const int OFS_PouchBerry = 0x054C;

    protected override InventoryPouch3[] GetItems()
    {
        const int max = 999;
        var info = ItemStorage3FRLG.Instance;
        return
        [
            new(InventoryType.Items, info, max, OFS_PouchHeldItem, (OFS_PouchKeyItem - OFS_PouchHeldItem) / 4),
            new(InventoryType.KeyItems, info, 1, OFS_PouchKeyItem, (OFS_PouchBalls - OFS_PouchKeyItem) / 4),
            new(InventoryType.Balls, info, max, OFS_PouchBalls, (OFS_PouchTMHM - OFS_PouchBalls) / 4),
            new(InventoryType.TMHMs, info, max, OFS_PouchTMHM, (OFS_PouchBerry - OFS_PouchTMHM) / 4),
            new(InventoryType.Berries, info, 999, OFS_PouchBerry, 43),
            new(InventoryType.PCItems, info, 999, OFS_PCItem, (OFS_PouchHeldItem - OFS_PCItem) / 4),
        ];
    }

    protected override int SeenOffset2 => 0x5F8;
    protected override int MailOffset => 0x2CD0;

    protected override int GetDaycareEXPOffset(int slot) => GetDaycareSlotOffset(slot + 1) - 4; // @ end of each pk slot
    ushort IDaycareRandomState<ushort>.Seed
    {
        get => ReadUInt16LittleEndian(Large.AsSpan(GetDaycareEXPOffset(2)));
        set => WriteUInt16LittleEndian(Large.AsSpan(GetDaycareEXPOffset(2)), value);
    }

    protected override int ExternalEventData => 0x30A7;

    #region eBerry
    private const int OFFSET_EBERRY = 0x30EC;
    private const int SIZE_EBERRY = 0x34;

    public override Span<byte> EReaderBerry() => Large.AsSpan(OFFSET_EBERRY, SIZE_EBERRY);
    #endregion

    #region eTrainer
    public override Span<byte> EReaderTrainer() => Small.AsSpan(0x4A0, 0xBC);
    #endregion

    public int WonderOffset => WonderNewsOffset;
    private const int WonderNewsOffset = 0x3120;
    private int WonderCardOffset => WonderNewsOffset + (Japanese ? WonderNews3.SIZE_JAP : WonderNews3.SIZE);
    private int WonderCardExtraOffset => WonderCardOffset + (Japanese ? WonderCard3.SIZE_JAP : WonderCard3.SIZE);

    private Span<byte> WonderNewsData => Large.AsSpan(WonderNewsOffset, Japanese ? WonderNews3.SIZE_JAP : WonderNews3.SIZE);
    private Span<byte> WonderCardData => Large.AsSpan(WonderCardOffset, Japanese ? WonderCard3.SIZE_JAP : WonderCard3.SIZE);
    private Span<byte> WonderCardExtraData => Large.AsSpan(WonderCardExtraOffset, WonderCard3Extra.SIZE);

    public WonderNews3 WonderNews { get => new(WonderNewsData.ToArray()); set => SetData(WonderNewsData, value.Data); }
    public WonderCard3 WonderCard { get => new(WonderCardData.ToArray()); set => SetData(WonderCardData, value.Data); }
    public WonderCard3Extra WonderCardExtra { get => new(WonderCardExtraData.ToArray()); set => SetData(WonderCardExtraData, value.Data); }

    // 0x338: 4 easy chat words
    // 0x340: news MENewsJisanStruct
    // 0x344: uint[5], uint[5] tracking?

    private Span<byte> MysterySpan => Large.AsSpan(0x361C, MysteryEvent3.SIZE);
    public override Gen3MysteryData MysteryData { get => new MysteryEvent3(MysterySpan.ToArray()); set => SetData(MysterySpan, value.Data); }

    protected override int SeenOffset3 => 0x3A18;

    public string RivalName
    {
        get => GetString(Large.AsSpan(0x3A4C, 8));
        set => SetString(Large.AsSpan(0x3A4C, 8), value, 7, StringConverterOption.ClearZero);
    }

    #endregion
}
