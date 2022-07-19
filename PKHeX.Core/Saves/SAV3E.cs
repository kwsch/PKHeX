using System;
using System.Collections.Generic;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 3 <see cref="SaveFile"/> object for <see cref="GameVersion.E"/>.
/// </summary>
/// <inheritdoc cref="SAV3" />
public sealed class SAV3E : SAV3, IGen3Hoenn, IGen3Joyful, IGen3Wonder
{
    // Configuration
    protected override SaveFile CloneInternal() => new SAV3E(Write());
    public override GameVersion Version { get => GameVersion.E; protected set { } }
    public override IPersonalTable Personal => PersonalTable.E;

    public override int EventFlagCount => 8 * 300;
    public override int EventWorkCount => 0x100;
    protected override int DaycareSlotSize => SIZE_STORED + 0x3C; // 0x38 mail + 4 exp
    public override int DaycareSeedSize => 8; // 32bit
    protected override int EggEventFlag => 0x86;
    protected override int BadgeFlagStart => 0x867;

    public SAV3E(byte[] data) : base(data) => Initialize();
    public SAV3E(bool japanese = false) : base(japanese) => Initialize();

    protected override int EventFlag => 0x1270;
    protected override int EventWork => 0x139C;

    private void Initialize()
    {
        // small
        PokeDex = 0x18;

        // large
        DaycareOffset = 0x3030;

        // storage
        Box = 0;
    }

    #region Small
    public override bool NationalDex
    {
        get => PokedexNationalMagicRSE == PokedexNationalUnlockRSE;
        set
        {
            PokedexMode = value ? (byte)1 : (byte)0; // mode
            PokedexNationalMagicRSE = value ? PokedexNationalUnlockRSE : (byte)0; // magic
            SetEventFlag(0x896, value);
            SetWork(0x46, PokedexNationalUnlockWorkRSE);
        }
    }

    public override uint SecurityKey
    {
        get => ReadUInt32LittleEndian(Small.AsSpan(0xAC));
        set => WriteUInt32LittleEndian(Small.AsSpan(0xAC), value);
    }

    public RTC3 ClockInitial
    {
        get => new(GetData(Small, 0x98, RTC3.Size));
        set => SetData(Small, value.Data, 0x98);
    }

    public RTC3 ClockElapsed
    {
        get => new(GetData(Small, 0xA0, RTC3.Size));
        set => SetData(Small, value.Data, 0xA0);
    }

    public ushort JoyfulJumpInRow           { get => ReadUInt16LittleEndian(Small.AsSpan(0x1FC)); set => WriteUInt16LittleEndian(Small.AsSpan(0x1FC), Math.Min((ushort)9999, value)); }
    // u16 field2;
    public ushort JoyfulJump5InRow          { get => ReadUInt16LittleEndian(Small.AsSpan(0x200)); set => WriteUInt16LittleEndian(Small.AsSpan(0x200), Math.Min((ushort)9999, value)); }
    public ushort JoyfulJumpGamesMaxPlayers { get => ReadUInt16LittleEndian(Small.AsSpan(0x202)); set => WriteUInt16LittleEndian(Small.AsSpan(0x202), Math.Min((ushort)9999, value)); }
    // u32 field8;
    public uint   JoyfulJumpScore           { get => ReadUInt16LittleEndian(Small.AsSpan(0x208)); set => WriteUInt32LittleEndian(Small.AsSpan(0x208), Math.Min(9999, value)); }

    public uint   JoyfulBerriesScore        { get => ReadUInt16LittleEndian(Small.AsSpan(0x20C)); set => WriteUInt32LittleEndian(Small.AsSpan(0x20C), Math.Min(9999, value)); }
    public ushort JoyfulBerriesInRow        { get => ReadUInt16LittleEndian(Small.AsSpan(0x210)); set => WriteUInt16LittleEndian(Small.AsSpan(0x210), Math.Min((ushort)9999, value)); }
    public ushort JoyfulBerries5InRow       { get => ReadUInt16LittleEndian(Small.AsSpan(0x212)); set => WriteUInt16LittleEndian(Small.AsSpan(0x212), Math.Min((ushort)9999, value)); }

    public uint BP
    {
        get => ReadUInt16LittleEndian(Small.AsSpan(0xEB8));
        set
        {
            if (value > 9999)
                value = 9999;
            WriteUInt16LittleEndian(Small.AsSpan(0xEB8), (ushort)value);
        }
    }

    public uint BPEarned
    {
        get => ReadUInt16LittleEndian(Small.AsSpan(0xEBA));
        set
        {
            if (value > 65535)
                value = 65535;
            WriteUInt16LittleEndian(Small.AsSpan(0xEBA), (ushort)value);
        }
    }
    #endregion

    #region Large
    public override int PartyCount { get => Large[0x234]; protected set => Large[0x234] = (byte)value; }
    public override int GetPartyOffset(int slot) => 0x238 + (SIZE_PARTY * slot);

    public override uint Money
    {
        get => ReadUInt32LittleEndian(Large.AsSpan(0x0490)) ^ SecurityKey;
        set => WriteUInt32LittleEndian(Large.AsSpan(0x0490), value ^ SecurityKey);
    }

    public override uint Coin
    {
        get => (ushort)(ReadUInt16LittleEndian(Large.AsSpan(0x0494)) ^ SecurityKey);
        set => WriteUInt16LittleEndian(Large.AsSpan(0x0494), (ushort)(value ^ SecurityKey));
    }

    private const int OFS_PCItem = 0x0498;
    private const int OFS_PouchHeldItem = 0x0560;
    private const int OFS_PouchKeyItem = 0x05D8;
    private const int OFS_PouchBalls = 0x0650;
    private const int OFS_PouchTMHM = 0x0690;
    private const int OFS_PouchBerry = 0x0790;

    protected override InventoryPouch3[] GetItems()
    {
        const int max = 99;
        var PCItems = ArrayUtil.ConcatAll(Legal.Pouch_Items_RS, Legal.Pouch_Key_E, Legal.Pouch_Ball_RS, Legal.Pouch_TMHM_RS, Legal.Pouch_Berries_RS);
        return new InventoryPouch3[]
        {
            new(InventoryType.Items, Legal.Pouch_Items_RS, max, OFS_PouchHeldItem, (OFS_PouchKeyItem - OFS_PouchHeldItem) / 4),
            new(InventoryType.KeyItems, Legal.Pouch_Key_E, 1, OFS_PouchKeyItem, (OFS_PouchBalls - OFS_PouchKeyItem) / 4),
            new(InventoryType.Balls, Legal.Pouch_Ball_RS, max, OFS_PouchBalls, (OFS_PouchTMHM - OFS_PouchBalls) / 4),
            new(InventoryType.TMHMs, Legal.Pouch_TMHM_RS, max, OFS_PouchTMHM, (OFS_PouchBerry - OFS_PouchTMHM) / 4),
            new(InventoryType.Berries, Legal.Pouch_Berries_RS, 999, OFS_PouchBerry, 46),
            new(InventoryType.PCItems, PCItems, 999, OFS_PCItem, (OFS_PouchHeldItem - OFS_PCItem) / 4),
        };
    }

    public PokeBlock3Case PokeBlocks
    {
        get => new(Large, 0x848);
        set => SetData(Large, value.Write(), 0x848);
    }

    protected override int SeenOffset2 => 0x988;

    public DecorationInventory3 Decorations => new(Large.AsSpan(0x2734, DecorationInventory3.SIZE));

    public Swarm3 Swarm
    {
        get => new(Large.Slice(0x2B90, Swarm3.SIZE));
        set => SetData(Large, value.Data, 0x2B90);
    }

    private void ClearSwarm() => Large.AsSpan(0x2B90, Swarm3.SIZE).Clear();

    public IReadOnlyList<Swarm3> DefaultSwarms => Swarm3Details.Swarms_E;

    public int SwarmIndex
    {
        get => Array.FindIndex(Swarm3Details.Swarms_E, z => z.MapNum == Swarm.MapNum);
        set
        {
            var arr = DefaultSwarms;
            if ((uint)value >= arr.Count)
                ClearSwarm();
            else
                Swarm = arr[value];
        }
    }

    protected override int MailOffset => 0x2BE0;

    protected override int GetDaycareEXPOffset(int slot) => GetDaycareSlotOffset(0, slot + 1) - 4; // @ end of each pk slot
    public override string GetDaycareRNGSeed(int loc) => ReadUInt32LittleEndian(Large.AsSpan(GetDaycareSlotOffset(0, 2))).ToString("X8");  // after the 2 slots, before the step counter
    public override void SetDaycareRNGSeed(int loc, string seed) => WriteUInt32LittleEndian(Large.AsSpan(GetDaycareEXPOffset(2)), Util.GetHexValue(seed));

    protected override int ExternalEventData => 0x31B3;

    #region eBerry
    private const int OFFSET_EBERRY = 0x31F8;
    private const int SIZE_EBERRY = 0x134;

    public byte[] GetEReaderBerry() => Large.Slice(OFFSET_EBERRY, SIZE_EBERRY);
    public void SetEReaderBerry(ReadOnlySpan<byte> data) => data.CopyTo(Large.AsSpan(OFFSET_EBERRY));

    public override string EBerryName => GetString(Large.AsSpan(OFFSET_EBERRY, 7));
    public override bool IsEBerryEngima => Large[OFFSET_EBERRY] is 0 or 0xFF;
    #endregion

    public int WonderOffset => WonderNewsOffset;
    private const int WonderNewsOffset = 0x322C;
    private const int WonderCardOffset = WonderNewsOffset + WonderNews3.SIZE;
    private const int WonderCardExtraOffset = WonderCardOffset + WonderCard3.SIZE;

    public WonderNews3 WonderNews { get => new(Large.Slice(WonderNewsOffset, WonderNews3.SIZE)); set => SetData(Large, value.Data, WonderOffset); }
    public WonderCard3 WonderCard { get => new(Large.Slice(WonderCardOffset, WonderCard3.SIZE)); set => SetData(Large, value.Data, WonderCardOffset); }
    public WonderCard3Extra WonderCardExtra { get => new(Large.Slice(WonderCardExtraOffset, WonderCard3Extra.SIZE)); set => SetData(Large, value.Data, WonderCardExtraOffset); }
    // 0x338: 4 easy chat words
    // 0x340: news MENewsJisanStruct
    // 0x344: uint[5], uint[5] tracking?

    public override MysteryEvent3 MysteryEvent
    {
        get => new(Large.Slice(0x3728, MysteryEvent3.SIZE));
        set => SetData(Large, value.Data, 0x3728);
    }

    protected override int SeenOffset3 => 0x3B24;

    private const int Walda = 0x3D70;
    public ushort WaldaBackgroundColor { get => ReadUInt16LittleEndian(Large.AsSpan(Walda + 0)); set => WriteUInt16LittleEndian(Large.AsSpan(Walda + 0), value); }
    public ushort WaldaForegroundColor { get => ReadUInt16LittleEndian(Large.AsSpan(Walda + 2)); set => WriteUInt16LittleEndian(Large.AsSpan(Walda + 2), value); }
    public byte WaldaIconID { get => Large[Walda + 0x14]; set => Large[Walda + 0x14] = value; }
    public byte WaldaPatternID { get => Large[Walda + 0x15]; set => Large[Walda + 0x15] = value; }
    public bool WaldaUnlocked { get => Large[Walda + 0x16] != 0; set => Large[Walda + 0x16] = (byte)(value ? 1 : 0); }
    #endregion

    private const uint EXTRADATA_SENTINEL = 0x0000B39D;
    private const int OFS_BV = 31 * 0x1000; // last sector of the save
    public bool HasBattleVideo => Data.Length > SaveUtil.SIZE_G3RAWHALF && ReadUInt32LittleEndian(Data.AsSpan(OFS_BV)) == EXTRADATA_SENTINEL;

    public BV3 BattleVideo
    {
        get => !HasBattleVideo ? new BV3() : new BV3(Data.Slice(OFS_BV + 4, BV3.SIZE));
        set => SetData(Data, value.Data, OFS_BV + 4);
    }
}
