using System;
using System.Collections.Generic;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 3 <see cref="SaveFile"/> object for <see cref="GameVersion.E"/>.
/// </summary>
/// <inheritdoc cref="SAV3" />
public sealed class SAV3E : SAV3, IGen3Hoenn, IGen3Joyful, IGen3Wonder, IDaycareRandomState<uint>
{
    // Configuration
    protected override SAV3E CloneInternal() => new(Write()) { Language = Language };
    public override GameVersion Version { get => GameVersion.E; set { } }
    public override PersonalTable3 Personal => PersonalTable.E;

    public override int EventFlagCount => 8 * 300;
    public override int EventWorkCount => 0x100;
    protected override int DaycareSlotSize => SIZE_STORED + 0x3C; // 0x38 mail + 4 exp
    protected override int EggEventFlag => 0x86;
    protected override int BadgeFlagStart => 0x867;

    public SAV3E(byte[] data) : base(data) => Initialize();
    public SAV3E(bool japanese = false) : base(japanese) => Initialize();

    protected override int EventFlag => 0x1270;
    protected override int EventWork => 0x139C;
    public override int MaxItemID => Legal.MaxItemID_3_E;

    protected override int PokeDex => 0x18; // small
    protected override int DaycareOffset => 0x3030; // large

    // storage
    private void Initialize() => Box = 0;

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
        get => new(Small.AsSpan(0x98, RTC3.Size).ToArray());
        set => SetData(Small.AsSpan(0x98), value.Data);
    }

    public RTC3 ClockElapsed
    {
        get => new(Small.AsSpan(0xA0, RTC3.Size).ToArray());
        set => SetData(Small.AsSpan(0xA0), value.Data);
    }

    public uint BerryPowder
    {
        get => ReadUInt32LittleEndian(Small.AsSpan(0x1F4)) ^ SecurityKey;
        set => WriteUInt32LittleEndian(Small.AsSpan(0x1F4), value ^ SecurityKey);
    }

    public ushort JoyfulJumpInRow           { get => ReadUInt16LittleEndian(Small.AsSpan(0x1FC)); set => WriteUInt16LittleEndian(Small.AsSpan(0x1FC), Math.Min((ushort)9999, value)); }
    // u16 field2;
    public ushort JoyfulJump5InRow          { get => ReadUInt16LittleEndian(Small.AsSpan(0x200)); set => WriteUInt16LittleEndian(Small.AsSpan(0x200), Math.Min((ushort)9999, value)); }
    public ushort JoyfulJumpGamesMaxPlayers { get => ReadUInt16LittleEndian(Small.AsSpan(0x202)); set => WriteUInt16LittleEndian(Small.AsSpan(0x202), Math.Min((ushort)9999, value)); }
    // u32 field8;
    public uint   JoyfulJumpScore           { get => ReadUInt16LittleEndian(Small.AsSpan(0x208)); set => WriteUInt32LittleEndian(Small.AsSpan(0x208), Math.Min(99990, value)); }

    public uint   JoyfulBerriesScore        { get => ReadUInt16LittleEndian(Small.AsSpan(0x20C)); set => WriteUInt32LittleEndian(Small.AsSpan(0x20C), Math.Min(99990, value)); }
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
    private const int OFS_BerryBlenderRecord = 0x9BC;
    private const int OFS_TrendyWord = 0x2E20;
    private const int OFS_TrainerHillRecord = 0x3718;

    protected override InventoryPouch3[] GetItems()
    {
        const int max = 99;
        var info = ItemStorage3E.Instance;
        return
        [
            new(InventoryType.Items, info, max, OFS_PouchHeldItem, (OFS_PouchKeyItem - OFS_PouchHeldItem) / 4),
            new(InventoryType.KeyItems, info, 1, OFS_PouchKeyItem, (OFS_PouchBalls - OFS_PouchKeyItem) / 4),
            new(InventoryType.Balls, info, max, OFS_PouchBalls, (OFS_PouchTMHM - OFS_PouchBalls) / 4),
            new(InventoryType.TMHMs, info, max, OFS_PouchTMHM, (OFS_PouchBerry - OFS_PouchTMHM) / 4),
            new(InventoryType.Berries, info, 999, OFS_PouchBerry, 46),
            new(InventoryType.PCItems, info, 999, OFS_PCItem, (OFS_PouchHeldItem - OFS_PCItem) / 4),
        ];
    }

    private Span<byte> PokeBlockData => Large.AsSpan(0x848, PokeBlock3Case.SIZE);

    public PokeBlock3Case PokeBlocks
    {
        get => new(PokeBlockData);
        set => value.Write(PokeBlockData);
    }

    protected override int SeenOffset2 => 0x988;

    public DecorationInventory3 Decorations => new(Large.AsSpan(0x2734, DecorationInventory3.SIZE));

    private Span<byte> SwarmSpan => Large.AsSpan(0x2B90, Swarm3.SIZE);
    public Swarm3 Swarm
    {
        get => new(SwarmSpan.ToArray());
        set => SetData(SwarmSpan, value.Data);
    }

    private void ClearSwarm() => SwarmSpan.Clear();

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

    protected override int GetDaycareEXPOffset(int slot) => GetDaycareSlotOffset(slot + 1) - 4; // @ end of each pk slot
    uint IDaycareRandomState<uint>.Seed // after the 2 slots, before the step counter
    {
        get => ReadUInt32LittleEndian(Large.AsSpan(GetDaycareEXPOffset(2)));
        set => WriteUInt32LittleEndian(Large.AsSpan(GetDaycareEXPOffset(2)), value);
    }

    protected override int ExternalEventData => 0x31B3;

    /// <summary>
    /// Max RPM for 2, 3 and 4 players. Each value unit represents 0.01 RPM. Value 0 if no record.
    /// </summary>
    /// <remarks>2 players: index 0, 3 players: index 1, 4 players: index 2</remarks>
    public const int BerryBlenderRPMRecordCount = 3;

    private Span<byte> GetBlenderRPMSpan(int index)
    {
        if ((uint)index >= BerryBlenderRPMRecordCount)
            throw new ArgumentOutOfRangeException(nameof(index));
        return Large.AsSpan(OFS_BerryBlenderRecord + (index * 2));
    }

    public ushort GetBerryBlenderRPMRecord(int index) => ReadUInt16LittleEndian(GetBlenderRPMSpan(index));

    public void SetBerryBlenderRPMRecord(int index, ushort value)
    {
        WriteUInt16LittleEndian(GetBlenderRPMSpan(index), value);
        State.Edited = true;
    }

    public bool GetTrendyWordUnlocked(TrendyWord3E word)
    {
        return GetFlag(OFS_TrendyWord + ((byte)word >> 3), (byte)word & 7);
    }

    public void SetTrendyWordUnlocked(TrendyWord3E word, bool value)
    {
        SetFlag(OFS_TrendyWord + ((byte)word >> 3), (byte)word & 7, value);
        State.Edited = true;
    }

    /** Each value unit represents 1/60th of a second. Value 0 if no record. */
    public uint GetTrainerHillRecord(TrainerHillMode3E mode)
    {
        return ReadUInt32LittleEndian(Large.AsSpan(OFS_TrainerHillRecord + ((byte)mode * 4)));
    }

    public void SetTrainerHillRecord(TrainerHillMode3E mode, uint value)
    {
        WriteUInt32LittleEndian(Large.AsSpan(OFS_TrainerHillRecord + ((byte)mode * 4)), value);
        State.Edited = true;
    }

    #region eBerry
    private const int OFFSET_EBERRY = 0x31F8;
    private const int SIZE_EBERRY = 0x34;

    public override Span<byte> EReaderBerry() => Large.AsSpan(OFFSET_EBERRY, SIZE_EBERRY);
    #endregion

    #region eTrainer
    public override Span<byte> EReaderTrainer() => Small.AsSpan(0xBEC, 0xBC);
    #endregion

    public int WonderOffset => WonderNewsOffset;
    private const int WonderNewsOffset = 0x322C;
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

    private Span<byte> MysterySpan => Large.AsSpan(0x3728, MysteryEvent3.SIZE);
    public override Gen3MysteryData MysteryData
    {
        get => new MysteryEvent3(MysterySpan.ToArray());
        set => SetData(MysterySpan, value.Data);
    }

    private Span<byte> RecordMixingData => Large.AsSpan(0x3B14, RecordMixing3Gift.SIZE);
    public RecordMixing3Gift RecordMixingGift { get => new(RecordMixingData.ToArray()); set => SetData(RecordMixingData, value.Data); }

    protected override int SeenOffset3 => 0x3B24;

    private const int Walda = 0x3D70;
    public ushort WaldaBackgroundColor { get => ReadUInt16LittleEndian(Large.AsSpan(Walda + 0)); set => WriteUInt16LittleEndian(Large.AsSpan(Walda + 0), value); }
    public ushort WaldaForegroundColor { get => ReadUInt16LittleEndian(Large.AsSpan(Walda + 2)); set => WriteUInt16LittleEndian(Large.AsSpan(Walda + 2), value); }
    public byte WaldaIconID { get => Large[Walda + 0x14]; set => Large[Walda + 0x14] = value; }
    public byte WaldaPatternID { get => Large[Walda + 0x15]; set => Large[Walda + 0x15] = value; }
    public bool WaldaUnlocked { get => Large[Walda + 0x16] != 0; set => Large[Walda + 0x16] = (byte)(value ? 1 : 0); }
    #endregion

    private const uint EXTRADATA_SENTINEL = 0x0000B39D;
    public bool HasBattleVideo => Data.Length > SaveUtil.SIZE_G3RAWHALF && ReadUInt32LittleEndian(GetFinalExternalData().Span) == EXTRADATA_SENTINEL;
    public void SetExtraDataSentinelBattleVideo() => WriteUInt32LittleEndian(GetFinalExternalData().Span, EXTRADATA_SENTINEL);

    public Memory<byte> BattleVideoData => GetFinalExternalData().Slice(4, BattleVideo3.SIZE);
    public BattleVideo3 BattleVideo
    {
        // decouple from the save file object on get, as the consumer might not be aware that mutations will touch the save.
        get => HasBattleVideo ? new BattleVideo3(BattleVideoData.ToArray()) : new BattleVideo3();
        set => SetData(BattleVideoData.Span, value.Data);
    }
}
