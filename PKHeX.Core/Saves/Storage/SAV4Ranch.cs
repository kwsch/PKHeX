using System;
using System.Collections.Generic;
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

    public override int SlotCount => RanchLevel.GetSlotCount(CurrentRanchLevelIndex);
    public override int BoxCount => (int)Math.Ceiling((decimal)SlotCount / SlotsPerBox);
    public int MiiCount { get; }
    public int TrainerMiiCount { get; }
    public int MaxToys => RanchLevel.GetMaxToys(CurrentRanchLevelIndex);
    public int MaxMiiCount => RanchLevel.GetMaxMiis(CurrentRanchLevelIndex);

    public override IPersonalTable Personal => PersonalTable.Pt;
    public override IReadOnlyList<ushort> HeldItems => Legal.HeldItems_Pt;
    protected override SaveFile CloneInternal() => new SAV4Ranch((byte[])Data.Clone());
    protected internal override string ShortSummary => $"{OT} {PlayTimeString}";
    public override string Extension => ".bin";

    protected override PKM GetPKM(byte[] data) => new RK4(data);
    public override StorageSlotSource GetSlotFlags(int index) => index >= SlotCount ? StorageSlotSource.Locked : StorageSlotSource.None;
    protected override bool IsSlotSwapProtected(int box, int slot) => IsSlotOverwriteProtected(box, slot);
    public override bool IsPKMPresent(ReadOnlySpan<byte> data) => EntityDetection.IsPresentSAV4Ranch(data);

    public SAV4Ranch(byte[] data) : base(data, typeof(RK4), 0)
    {
        Version = Data.Length == SaveUtil.SIZE_G4RANCH_PLAT ? GameVersion.Pt : GameVersion.DP;

        OT = GetString(0x770, 0x12);

        // 0x18 starts the header table: [u32 BlockID, u32 Offset]
        // Block 00, Offset = Metadata object
        // Block 01, Offset = Mii Data Array object
        // Block 02, Offset = Mii Link Data Array object
        // Block 03, Offset = Pokemon Data Array object
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

    public byte CurrentRanchLevelIndex { get => Data[0x5A]; set => Data[0x5A] = value; }
    public byte PlannedRanchLevelIndex { get => Data[0x5B]; set => Data[0x5B] = value; } // tomorrow's level

    public uint SecondsSince2000 { get => ReadUInt32BigEndian(Data.AsSpan(0x5C)); set => WriteUInt32BigEndian(Data.AsSpan(0x5C), value); }
    public uint TotalSeconds { get => ReadUInt32BigEndian(Data.AsSpan(0x60)); set => WriteUInt32BigEndian(Data.AsSpan(0x60), value); }
    public ushort NextHayleyBringNationalDex { get => ReadUInt16LittleEndian(Data.AsSpan(0x6A)); set => WriteUInt16LittleEndian(Data.AsSpan(0x6A), value); }

    public RanchToy GetRanchToy(int index)
    {
        if ((uint)index >= MaxToys)
            throw new ArgumentOutOfRangeException(nameof(index));

        int toyOffset = ToyBaseOffset + (RanchToy.SIZE * index);
        var data = Data.Slice(toyOffset, RanchToy.SIZE);
        return new RanchToy(data);
    }

    public void SetRanchToy(RanchToy toy, int index)
    {
        if ((uint)index >= MaxToys)
            throw new ArgumentOutOfRangeException(nameof(index));

        int toyOffset = ToyBaseOffset + (RanchToy.SIZE * index);
        SetData(Data, toy.Data, toyOffset);
    }

    public RanchMii GetRanchMii(int index)
    {
        if ((uint)index >= MiiCount)
            throw new ArgumentOutOfRangeException(nameof(index));

        int offset = MiiDataOffset + (RanchMii.SIZE * index);
        var data = Data.Slice(offset, RanchMii.SIZE);
        return new RanchMii(data);
    }

    public void SetRanchMii(RanchMii trainer, int index)
    {
        if ((uint)index >= MiiCount)
            throw new ArgumentOutOfRangeException(nameof(index));

        int offset = MiiDataOffset + (RanchMii.SIZE * index);
        SetData(Data, trainer.Data, offset);
    }

    public RanchTrainerMii GetRanchTrainerMii(int index)
    {
        if ((uint)index >= TrainerMiiCount)
            throw new ArgumentOutOfRangeException(nameof(index));

        int offset = TrainerMiiDataOffset + (RanchTrainerMii.SIZE * index);
        var data = Data.Slice(offset, RanchTrainerMii.SIZE);
        return new RanchTrainerMii(data);
    }

    public void SetRanchTrainerMii(RanchTrainerMii mii, int index)
    {
        if ((uint)index >= TrainerMiiCount)
            throw new ArgumentOutOfRangeException(nameof(index));

        int offset = TrainerMiiDataOffset + (RanchTrainerMii.SIZE * index);
        SetData(Data, mii.Data, offset);
    }

    protected override void SetChecksums()
    {
        // ensure the final data is written if the user screws stuff up
        WriteInt32BigEndian(Data.AsSpan(DataEndMarkerOffset), DataEndMarker);
        var goodlen = (DataEndMarkerOffset + 4);
        Array.Clear(Data, goodlen, Data.Length - goodlen);

        // 20 byte SHA checksum at the top of the file, which covers all data that follows.
        using var hash = SHA1.Create();
        var result = hash.ComputeHash(Data, 20, Data.Length - 20);
        SetData(result, 0);
    }

    protected override byte[] DecryptPKM(byte[] data)
    {
        var pokeData = PokeCrypto.DecryptArray45(data.Slice(0, PokeCrypto.SIZE_4STORED));
        var ranchData = data.AsSpan(PokeCrypto.SIZE_4STORED, 0x1C);
        var finalData = new byte[SIZE_STORED];

        pokeData.CopyTo(finalData, 0);
        ranchData.CopyTo(finalData.AsSpan(PokeCrypto.SIZE_4STORED));
        return finalData;
    }

    public void WriteBoxSlotInternal(PKM pk, Span<byte> data, int offset, string htName = "", ushort htTID = 0, ushort htSID = 0, RanchOwnershipType type = RanchOwnershipType.Hayley)
    {
        RK4 rk = (RK4)this.GetCompatiblePKM(pk);
        rk.OwnershipType = type;
        rk.HT_TID = htTID;
        rk.HT_SID = htSID;
        rk.HT_Name = htName;

        WriteBoxSlot(rk, data, offset);
    }

    public override void WriteBoxSlot(PKM pk, Span<byte> data, int offset)
    {
        bool isBlank = pk.Data.SequenceEqual(BlankPKM.Data);
        if (pk is not RK4 rk4)
        {
            WriteBoxSlotInternal(pk, data, offset);
            return;
        }

        if (!isBlank && rk4.OwnershipType == RanchOwnershipType.None)
            rk4.OwnershipType = RanchOwnershipType.Hayley; // Pokemon without an Ownership type get erased when the save is loaded. Hayley is considered 'default'.

        base.WriteBoxSlot(rk4, data, offset);
        if ((offset + SIZE_STORED) > DataEndMarkerOffset)
        {
            DataEndMarkerOffset = (offset + SIZE_STORED);
            WriteInt32BigEndian(Data.AsSpan(0x3C), DataEndMarkerOffset);
            WriteInt32BigEndian(Data.AsSpan(DataEndMarkerOffset), DataEndMarker);
        }

        int pkStart = PokemonCountOffset + 4;
        int pkEnd = DataEndMarkerOffset;
        int pkCount = (pkEnd - pkStart) / SIZE_STORED;
        WriteInt32BigEndian(Data.AsSpan(PokemonCountOffset), pkCount);
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

    public override string GetString(ReadOnlySpan<byte> data) => StringConverter4GC.GetStringUnicode(data);

    public override int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength, StringConverterOption option)
    {
        return StringConverter4GC.SetStringUnicode(value, destBuffer, maxLength, option);
    }
}
