using System;
using System.Linq;
using System.Security.Cryptography;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 4 <see cref="SaveFile"/> object for My Pokémon Ranch saves.
/// </summary>
public sealed class SAV4Ranch : BulkStorage, ISaveFileRevision
{
    protected override int SIZE_STORED => PokeCrypto.SIZE_4RSTORED;
    protected override int SIZE_PARTY => PokeCrypto.SIZE_4RSTORED;
    public int MaxToyID => (int) ((SaveRevision == 0) ? RanchToyType.Poke_Ball : RanchToyType.Water);
    public int SaveRevision => Version == GameVersion.DP ? 0 : 1;
    public string SaveRevisionString => Version == GameVersion.DP ? "-DP" : "-Pt";

    // ReSharper disable PrivateFieldCanBeConvertedToLocalVariable
    private readonly int DataEndMarker;
    private int DataEndMarkerOffset;
    private readonly int MiiDataOffset;
    private readonly int MiiCountOffset;
    private readonly int TrainerMiiDataOffset;
    private readonly int TrainerMiiCountOffset;
    private readonly int PokemonCountOffset;

    public override int SlotCount => RanchLevel.GetSlotCount(CurrentRanchLevel);
    public override int BoxCount => (int)Math.Ceiling((decimal)SlotCount / SlotsPerBox);
    public int MiiCount { get; }
    public int TrainerMiiCount { get; }
    public int MaxToyCount => RanchLevel.GetMaxToyCount(CurrentRanchLevel);
    public int MaxMiiCount => RanchLevel.GetMaxMiiCount(CurrentRanchLevel);

    private readonly RanchToy BlankToy = new(new byte[RanchToy.SIZE]);

    public override PersonalTable4 Personal => PersonalTable.Pt;
    public override ReadOnlySpan<ushort> HeldItems => Legal.HeldItems_Pt;
    protected override SAV4Ranch CloneInternal() => new((byte[])Data.Clone());
    protected internal override string ShortSummary => $"{OT} {PlayTimeString}";
    public override string Extension => ".bin";

    protected override RK4 GetPKM(byte[] data) => new(data);
    public override StorageSlotSource GetBoxSlotFlags(int index) => index >= SlotCount ? StorageSlotSource.Locked : StorageSlotSource.None;
    protected override bool IsSlotSwapProtected(int box, int slot) => IsBoxSlotOverwriteProtected(box, slot);
    public override bool IsPKMPresent(ReadOnlySpan<byte> data) => EntityDetection.IsPresentSAV4Ranch(data);

    private readonly GameVersion _version;
    public override GameVersion Version { get => _version; set { } }
    public SAV4Ranch(byte[] data) : base(data, typeof(RK4), 0)
    {
        _version = Data.Length == SaveUtil.SIZE_G4RANCH_PLAT ? GameVersion.Pt : GameVersion.DP;

        OT = GetString(Data.AsSpan(0x770, 0x12));

        // 0x18 starts the header table: [u32 BlockID, u32 Offset]
        // Block 00, Offset = Metadata object
        // Block 01, Offset = Mii Data Array object
        // Block 02, Offset = Mii Link Data Array object
        // Block 03, Offset = Pokémon Data Array object
        // Block 04, Offset = reserved object

        // Unpack the binary a little:
        // 00: size, ???
        // 01: size, count, Mii data[count]
        // 02: size, count, Mii Link data[count]
        // 03: size, count, Pokemon (PK4 + metadata)[count]
        // 04: size, count, ???

        MiiCountOffset = ReadInt32BigEndian(Data.AsSpan(0x24)) + 4;
        TrainerMiiCountOffset = ReadInt32BigEndian(Data.AsSpan(0x2C)) + 4;
        MiiCount = ReadInt32BigEndian(Data.AsSpan(MiiCountOffset));
        TrainerMiiCount = ReadInt32BigEndian(Data.AsSpan(TrainerMiiCountOffset));

        MiiDataOffset = MiiCountOffset + 4;
        TrainerMiiDataOffset = TrainerMiiCountOffset + 4;

        PokemonCountOffset = ReadInt32BigEndian(Data.AsSpan(0x34)) + 4;
        Box = PokemonCountOffset + 4;

        DataEndMarkerOffset = ReadInt32BigEndian(Data.AsSpan(0x3C));
        DataEndMarker = ReadInt32BigEndian(Data.AsSpan(DataEndMarkerOffset));
    }

    private const int ToyBaseOffset = 0x227B;

    public int CurrentRanchLevel { get => Data[0x5A] + 1; set => Data[0x5A] = (byte)(value - 1); }
    public int PlannedRanchLevel { get => Data[0x5B] + 1; set => Data[0x5B] = (byte)(value - 1); } // tomorrow's level

    public uint SecondsSince2000 { get => ReadUInt32BigEndian(Data.AsSpan(0x5C)); set => WriteUInt32BigEndian(Data.AsSpan(0x5C), value); }
    public uint TotalSeconds { get => ReadUInt32BigEndian(Data.AsSpan(0x60)); set => WriteUInt32BigEndian(Data.AsSpan(0x60), value); }
    public ushort NextHayleyBringNationalDex { get => ReadUInt16LittleEndian(Data.AsSpan(0x6A)); set => WriteUInt16LittleEndian(Data.AsSpan(0x6A), value); }

    public RanchToy GetRanchToy(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)MaxToyCount);

        int toyOffset = ToyBaseOffset + (RanchToy.SIZE * index);
        var data = Data.AsSpan(toyOffset, RanchToy.SIZE).ToArray();
        return new RanchToy(data);
    }

    public void SetRanchToy(RanchToy toy, int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)MaxToyCount);
        if (((int)toy.ToyType) > MaxToyID) // Ranch will throw "Corrupt Save" error if ToyId is > expected.
            toy = BlankToy;

        int toyOffset = ToyBaseOffset + (RanchToy.SIZE * index);
        SetData(Data.AsSpan(toyOffset), toy.Data);
    }

    public RanchMii GetRanchMii(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)MiiCount);

        int offset = MiiDataOffset + (RanchMii.SIZE * index);
        var data = Data.AsSpan(offset, RanchMii.SIZE).ToArray();
        return new RanchMii(data);
    }

    public void SetRanchMii(RanchMii trainer, int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)MiiCount);

        int offset = MiiDataOffset + (RanchMii.SIZE * index);
        SetData(Data.AsSpan(offset), trainer.Data);
    }

    public RanchTrainerMii GetRanchTrainerMii(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)TrainerMiiCount);

        int offset = TrainerMiiDataOffset + (RanchTrainerMii.SIZE * index);
        var data = Data.AsSpan(offset, RanchTrainerMii.SIZE).ToArray();
        return new RanchTrainerMii(data);
    }

    public void SetRanchTrainerMii(RanchTrainerMii mii, int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)index, (uint)TrainerMiiCount);

        int offset = TrainerMiiDataOffset + (RanchTrainerMii.SIZE * index);
        SetData(Data.AsSpan(offset), mii.Data);
    }

    private const int sha1HashSize = 20;

    protected override void SetChecksums()
    {
        var data = Data.AsSpan();
        var slotCount = GetOccupiedSlotCount();
        int pkStart = PokemonCountOffset + 4;
        UpdateMetadata(pkStart + (slotCount * SIZE_STORED));

        // 20 byte SHA checksum at the top of the file, which covers all data that follows.
        var hash = data[..sha1HashSize];
        var payload = data[sha1HashSize..];
        SHA1.HashData(payload, hash);
    }

    private int GetOccupiedSlotCount()
    {
        int count = SlotCount;
        for (int i = count - 1; i >= 0; i--)
        {
            var ofs = GetBoxSlotOffset(i);
            var span = Data.AsSpan(ofs, SIZE_STORED);
            var type = ReadUInt64LittleEndian(span[0x88..]);
            if (type != 0)
                return i + 1;
        }
        return 0;
    }

    protected override byte[] DecryptPKM(byte[] data)
    {
        var pokeData = PokeCrypto.DecryptArray45(data.AsSpan(0, PokeCrypto.SIZE_4STORED));
        var ranchData = data.AsSpan(PokeCrypto.SIZE_4STORED, 0x1C);
        var finalData = new byte[SIZE_STORED];

        pokeData.CopyTo(finalData, 0);
        ranchData.CopyTo(finalData.AsSpan(PokeCrypto.SIZE_4STORED));
        return finalData;
    }

    public void WriteBoxSlotInternal(PKM pk, Span<byte> data, string htName = "", ushort htTID = 0, ushort htSID = 0, RanchOwnershipType type = RanchOwnershipType.Hayley)
    {
        RK4 rk = (RK4)this.GetCompatiblePKM(pk);
        rk.OwnershipType = type;
        rk.HandlingTrainerTID = htTID;
        rk.HandlingTrainerSID = htSID;
        rk.HandlingTrainerName = htName;

        WriteBoxSlot(rk, data);
    }

    public override void WriteBoxSlot(PKM pk, Span<byte> data)
    {
        if (pk is not RK4 rk4)
        {
            WriteBoxSlotInternal(pk, data);
            return;
        }

        bool isBlank = pk.Data.SequenceEqual(BlankPKM.Data);
        if (!isBlank && rk4.OwnershipType == RanchOwnershipType.None)
            rk4.OwnershipType = RanchOwnershipType.Hayley; // Pokémon without an Ownership type get erased when the save is loaded. Hayley is considered 'default'.

        base.WriteBoxSlot(rk4, data);
    }

    private void UpdateMetadata(int pkEnd)
    {
        var data = Data.AsSpan();
        // ensure the final data is cleared if the user screws stuff up
        {
            DataEndMarkerOffset = pkEnd;
            WriteInt32BigEndian(data[0x3C..], pkEnd);
            WriteInt32BigEndian(data[pkEnd..], DataEndMarker);
            data[(pkEnd + 4)..].Clear();
        }

        int pkStart = PokemonCountOffset + 4;
        int pkCount = (pkEnd - pkStart) / SIZE_STORED;
        WriteInt32BigEndian(data[PokemonCountOffset..], pkCount);
    }

    private TimeSpan PlayedSpan
    {
        get => TimeSpan.FromSeconds(TotalSeconds);
        set => TotalSeconds = (uint)value.TotalSeconds;
    }

    public override int PlayedHours
    {
        get => (ushort)PlayedSpan.TotalHours;
        set { var time = PlayedSpan; PlayedSpan = time - TimeSpan.FromHours(time.TotalHours) + TimeSpan.FromHours(value); }
    }

    public override int PlayedMinutes
    {
        get => (byte)PlayedSpan.Minutes;
        set { var time = PlayedSpan; PlayedSpan = time - TimeSpan.FromMinutes(time.Minutes) + TimeSpan.FromMinutes(value); }
    }

    public override int PlayedSeconds
    {
        get => (byte)PlayedSpan.Seconds;
        set { var time = PlayedSpan; PlayedSpan = time - TimeSpan.FromSeconds(time.Seconds) + TimeSpan.FromSeconds(value); }
    }

    public override string GetString(ReadOnlySpan<byte> data)
        => StringConverter4GC.GetStringUnicode(data);
    public override int LoadString(ReadOnlySpan<byte> data, Span<char> destBuffer)
        => StringConverter4GC.LoadStringUnicode(data, destBuffer);
    public override int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength, StringConverterOption option)
        => StringConverter4GC.SetStringUnicode(value, destBuffer, maxLength, option);
}
