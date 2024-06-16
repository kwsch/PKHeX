using System;
using System.Collections.Generic;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 3 <see cref="SaveFile"/> object for <see cref="GameVersion.RS"/>.
/// </summary>
/// <inheritdoc cref="SAV3" />
public sealed class SAV3RS : SAV3, IGen3Hoenn, IDaycareRandomState<ushort>
{
    // Configuration
    protected override SAV3RS CloneInternal() => new(Write()) { Language = Language };
    public override GameVersion Version { get; set; } = GameVersion.RS; // allow mutation
    public override PersonalTable3 Personal => PersonalTable.RS;

    public override int EventFlagCount => 8 * 288;
    public override int EventWorkCount => 0x100;
    protected override int DaycareSlotSize => SIZE_STORED;
    protected override int EggEventFlag => 0x86;
    protected override int BadgeFlagStart => 0x807;

    public SAV3RS(byte[] data) : base(data) => Initialize();
    public SAV3RS(bool japanese = false) : base(japanese) => Initialize();

    protected override int EventFlag => 0x1220;
    protected override int EventWork => 0x1340;

    protected override int PokeDex => 0x18; // small
    protected override int DaycareOffset => 0x2F9C; // large

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
            SetEventFlag(0x836, value);
            SetWork(0x46, PokedexNationalUnlockWorkRSE);
        }
    }

    public override uint SecurityKey { get => 0; set { } }

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
    #endregion

    #region Large
    public override int PartyCount { get => Large[0x234]; protected set => Large[0x234] = (byte)value; }
    public override int GetPartyOffset(int slot) => 0x238 + (SIZE_PARTY * slot);

    public override uint Money
    {
        get => ReadUInt32LittleEndian(Large.AsSpan(0x0490));
        set => WriteUInt32LittleEndian(Large.AsSpan(0x0490), value);
    }

    public override uint Coin
    {
        get => ReadUInt16LittleEndian(Large.AsSpan(0x0494));
        set => WriteUInt16LittleEndian(Large.AsSpan(0x0494), (ushort)(value));
    }

    private const int OFS_PCItem = 0x0498;
    private const int OFS_PouchHeldItem = 0x0560;
    private const int OFS_PouchKeyItem = 0x05B0;
    private const int OFS_PouchBalls = 0x0600;
    private const int OFS_PouchTMHM = 0x0640;
    private const int OFS_PouchBerry = 0x0740;

    protected override InventoryPouch3[] GetItems()
    {
        const int max = 99;
        var info = ItemStorage3RS.Instance;
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

    private Span<byte> PokeBlockData => Large.AsSpan(0x7F8, PokeBlock3Case.SIZE);

    public PokeBlock3Case PokeBlocks
    {
        get => new(PokeBlockData);
        set => value.Write(PokeBlockData);
    }

    protected override int SeenOffset2 => 0x938;

    public DecorationInventory3 Decorations => new(Large.AsSpan(0x26A0, DecorationInventory3.SIZE));

    private Span<byte> SwarmData => Large.AsSpan(0x2AFC, Swarm3.SIZE);
    public Swarm3 Swarm
    {
        get => new(SwarmData.ToArray());
        set => SetData(SwarmData, value.Data);
    }

    private void ClearSwarm() => Large.AsSpan(0x2AFC, Swarm3.SIZE).Clear();

    public IReadOnlyList<Swarm3> DefaultSwarms => Swarm3Details.Swarms_RS;

    public int SwarmIndex
    {
        get => Array.FindIndex(Swarm3Details.Swarms_RS, z => z.MapNum == Swarm.MapNum);
        set
        {
            var arr = DefaultSwarms;
            if ((uint)value >= arr.Count)
                ClearSwarm();
            else
                Swarm = arr[value];
        }
    }

    protected override int MailOffset => 0x2B4C;

    protected override int GetDaycareEXPOffset(int slot) => GetDaycareSlotOffset(2) + (2 * 0x38) + (4 * slot); // consecutive vals, after both consecutive slots & 2 mail
    ushort IDaycareRandomState<ushort>.Seed
    {
        get => ReadUInt16LittleEndian(Large.AsSpan(GetDaycareEXPOffset(2)));
        set => WriteUInt16LittleEndian(Large.AsSpan(GetDaycareEXPOffset(2)), value);
    }

    protected override int ExternalEventData => 0x311B;

    #region eBerry
    private const int OFFSET_EBERRY = 0x3160;
    private const int SIZE_EBERRY = 0x530;

    public override Span<byte> EReaderBerry() => Large.AsSpan(OFFSET_EBERRY, SIZE_EBERRY);
    #endregion

    #region eTrainer
    public override Span<byte> EReaderTrainer() => Small.AsSpan(0x498, 0xBC);
    #endregion

    private Span<byte> MysterySpan => Large.AsSpan(0x3690, MysteryEvent3.SIZE);
    public override Gen3MysteryData MysteryData { get => new MysteryEvent3(MysterySpan.ToArray()); set => SetData(MysterySpan, value.Data); }

    private Span<byte> RecordSpan => Large.AsSpan(0x3A7C, RecordMixing3Gift.SIZE);
    public RecordMixing3Gift RecordMixingGift { get => new(RecordSpan.ToArray()); set => SetData(RecordSpan, value.Data); }

    protected override int SeenOffset3 => 0x3A8C;
    #endregion
}
