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
    protected override SAV3RS CloneInternal() => new(GetFinalData()) { Language = Language };

    public override GameVersion Version
    {
        get;
        set => field = value is GameVersion.RS or GameVersion.R or GameVersion.S ? value : GameVersion.RS;
    } = GameVersion.RS;

    public override PersonalTable3 Personal => PersonalTable.RS;

    public override int EventFlagCount => 8 * 288;
    public override int EventWorkCount => 0x100;
    protected override int DaycareSlotSize => SIZE_STORED;
    protected override int EggEventFlag => 0x86;
    protected override int BadgeFlagStart => 0x807;

    public SAV3RS(Memory<byte> data) : base(data) => Initialize();
    public SAV3RS(bool japanese = false) : base(japanese) => Initialize();

    public override PlayerBag3RS Inventory => new(this);

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
        get => new(Small.Slice(0x98, RTC3.Size).ToArray());
        set => SetData(Small.Slice(0x98, RTC3.Size), value.Data);
    }

    public RTC3 ClockElapsed
    {
        get => new(Small.Slice(0xA0, RTC3.Size).ToArray());
        set => SetData(Small.Slice(0xA0, RTC3.Size), value.Data);
    }
    #endregion

    #region Large
    public override int PartyCount { get => Large[0x234]; protected set => Large[0x234] = (byte)value; }
    public override int GetPartyOffset(int slot) => 0x238 + (SIZE_PARTY * slot);

    public override uint Money
    {
        get => ReadUInt32LittleEndian(Large[0x0490..]);
        set => WriteUInt32LittleEndian(Large[0x0490..], value);
    }

    public override uint Coin
    {
        get => ReadUInt16LittleEndian(Large[0x0494..]);
        set => WriteUInt16LittleEndian(Large[0x0494..], (ushort)(value));
    }

    private Span<byte> PokeBlockData => Large.Slice(0x7F8, PokeBlock3Case.SIZE);

    public PokeBlock3Case PokeBlocks
    {
        get => new(PokeBlockData);
        set => value.Write(PokeBlockData);
    }

    protected override int SeenOffset2 => 0x938;

    public DecorationInventory3 Decorations => new(Large.Slice(0x26A0, DecorationInventory3.SIZE));

    private Span<byte> SwarmData => Large.Slice(0x2AFC, Swarm3.SIZE);
    public Swarm3 Swarm
    {
        get => new(SwarmData.ToArray());
        set => SetData(SwarmData, value.Data);
    }

    private void ClearSwarm() => Large.Slice(0x2AFC, Swarm3.SIZE).Clear();

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
        get => ReadUInt16LittleEndian(Large[GetDaycareEXPOffset(2)..]);
        set => WriteUInt16LittleEndian(Large[GetDaycareEXPOffset(2)..], value);
    }

    protected override int ExternalEventData => 0x311B;

    #region eBerry
    private const int OFFSET_EBERRY = 0x3160;
    private const int SIZE_EBERRY = 0x530;

    public override Span<byte> EReaderBerry() => Large.Slice(OFFSET_EBERRY, SIZE_EBERRY);
    #endregion

    #region eTrainer
    public override Span<byte> EReaderTrainer() => Small.Slice(0x498, 0xBC);
    #endregion

    private Span<byte> MysterySpan => Large.Slice(0x3690, MysteryEvent3.SIZE);
    public override Gen3MysteryData MysteryData { get => new MysteryEvent3(MysterySpan.ToArray()); set => SetData(MysterySpan, value.Data); }

    private Span<byte> RecordSpan => Large.Slice(0x3A7C, RecordMixing3Gift.SIZE);
    public RecordMixing3Gift RecordMixingGift { get => new(RecordSpan.ToArray()); set => SetData(RecordSpan, value.Data); }

    protected override int SeenOffset3 => 0x3A8C;

    private Memory<byte> SecretBaseData => LargeBuffer.Slice(0x1A08, SecretBaseManager3.BaseCount * SecretBase3.SIZE);
    public SecretBaseManager3 SecretBases => new(SecretBaseData);

    private const int Painting = 0x2EFC;
    private const int CountPaintings = 5;
    private Span<byte> GetPaintingSpan(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, CountPaintings);
        return Large.Slice(Painting + (Paintings3.SIZE * index), Paintings3.SIZE * CountPaintings);
    }
    public Paintings3 GetPainting(int index) => new(GetPaintingSpan(index).ToArray(), Japanese);
    public void SetPainting(int index, Paintings3 value) => value.Data.CopyTo(GetPaintingSpan(index));
    #endregion
}
