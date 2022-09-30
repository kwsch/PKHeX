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

    public override int BoxCount { get; }
    public override int SlotCount { get; }
    public int MiiCount { get; }
    public int TrainerMiiCount { get; }
    public int MaxToys { get; } = 6;

    public override IPersonalTable Personal => PersonalTable.Pt;
    public override IReadOnlyList<ushort> HeldItems => Legal.HeldItems_Pt;
    protected override SaveFile CloneInternal() => new SAV4Ranch((byte[])Data.Clone());
    public override string PlayTimeString => $"{Checksums.CRC16Invert(Data):X4}";
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

        // 0x18 starts the header table
        // Block 00, Offset = ???
        // Block 01, Offset = Mii Data
        // Block 02, Offset = Mii Link Data
        // Block 03, Offset = Pokemon Data
        // Block 04, Offset = ??

        // Unpack the binary a little:
        // size, count, Mii data[count]
        // size, count, Mii Link data[count]
        // size, count, Pokemon (PK4 + metadata)[count]
        // size, count, ???

        /* ====Metadata====
         * uint8_t poke_type;// 01 trainer, 04 hayley, 05 traded
         * uint8_t tradeable;// 02 is tradeable, normal 00
         * uint16_t tid;
         * uint16_t sid;
         * uint32_t name1;
         * uint32_t name2;
         * uint32_t name3;
         * uint32_t name4;
         */

        MiiCountOffset = ReadInt32BigEndian(Data.AsSpan(0x24)) + 4;
        TrainerMiiCountOffset = ReadInt32BigEndian(Data.AsSpan(0x2C)) + 4;
        MiiCount = ReadInt32BigEndian(Data.AsSpan(MiiCountOffset));
        TrainerMiiCount = ReadInt32BigEndian(Data.AsSpan(TrainerMiiCountOffset));

        MiiDataOffset = MiiCountOffset + 4;
        TrainerMiiDataOffset = TrainerMiiCountOffset + 4;

        PokemonCountOffset = ReadInt32BigEndian(Data.AsSpan(0x34)) + 4;
        SlotCount = GetRanchLevel().MaxPkm;
        BoxCount = (int)Math.Ceiling((decimal)SlotCount / SlotsPerBox);
        Box = PokemonCountOffset + 4;

        DataEndMarkerOffset = ReadInt32BigEndian(Data.AsSpan(0x3C));
        DataEndMarker = ReadInt32BigEndian(Data.AsSpan(DataEndMarkerOffset));
    }

    public RanchLevel GetRanchLevel()
    {
        int ranchLevelIndex = Data[0x5A];
        return new RanchLevel(ranchLevelIndex);
    }

    public void SetRanchLevel(byte levelIndex)
    {
        Data[0x5A] = levelIndex;
    }

    public RanchLevel GetPlannedRanchLevel()
    {
        int ranchLevelIndex = Data[0x5B];
        return new RanchLevel(ranchLevelIndex);
    }

    public void SetPlannedRanchLevel(byte levelIndex)
    {
        Data[0x5B] = levelIndex;
    }

    public RanchToy GetRanchToy(int index)
    {
        if (index >= MaxToys)
            throw new ArgumentOutOfRangeException(nameof(index));

        int baseOffset = 0x227B;
        int toyOffset = baseOffset + (RanchToy.SIZE * index);
        byte[] toyData = Data.Slice(toyOffset, RanchToy.SIZE);
        return new RanchToy(toyData);
    }

    public void SetRanchToy(RanchToy toy, int index)
    {
        if (index >= MaxToys)
            throw new ArgumentOutOfRangeException(nameof(index));

        int baseOffset = 0x227B;
        int toyOffset = baseOffset + (RanchToy.SIZE * index);
        SetData(Data, toy.Data, toyOffset);
    }

    public RanchMii GetRanchMii(int index)
    {
        if (index >= MiiCount)
            throw new ArgumentOutOfRangeException(nameof(index));

        int offset = MiiDataOffset + (RanchMii.SIZE * index);
        byte[] miiData = Data.Slice(offset, RanchMii.SIZE);
        return new RanchMii(miiData);
    }

    public void SetRanchMii(RanchMii mii, int index)
    {
        if (index >= MiiCount)
            throw new ArgumentOutOfRangeException(nameof(index));

        int offset = MiiDataOffset + (RanchMii.SIZE * index);
        SetData(Data, mii.Data, offset);
    }

    public RanchTrainerMii GetRanchTrainerMii(int index)
    {
        if (index >= TrainerMiiCount)
            throw new ArgumentOutOfRangeException(nameof(index));

        int offset = TrainerMiiDataOffset + (RanchTrainerMii.SIZE * index);
        byte[] trainerMiiData = Data.Slice(offset, RanchTrainerMii.SIZE);
        return new RanchTrainerMii(trainerMiiData);
    }

    public void SetRanchTrainerMii(RanchTrainerMii trainerMii, int index)
    {
        if (index >= TrainerMiiCount)
            throw new ArgumentOutOfRangeException(nameof(index));

        int offset = TrainerMiiDataOffset + (RanchTrainerMii.SIZE * index);
        SetData(Data, trainerMii.Data, offset);
    }

    private readonly int DataEndMarker;
    private int DataEndMarkerOffset;
    private readonly int MiiDataOffset;
    private readonly int MiiCountOffset;
    private readonly int TrainerMiiDataOffset;
    private readonly int TrainerMiiCountOffset;
    private readonly int PokemonCountOffset;

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
        byte[] pokeData = PokeCrypto.DecryptArray45(data.Slice(0, PokeCrypto.SIZE_4STORED));
        byte[] ranchData = data.Slice(PokeCrypto.SIZE_4STORED, 0x1C);
        byte[] finalData = new byte[SIZE_STORED];
        pokeData.CopyTo(finalData, 0);
        ranchData.CopyTo(finalData, PokeCrypto.SIZE_4STORED);
        return finalData;
    }

    public void WriteBoxSlot(PKM pk, RanchPkOwnershipType ownershipType, ushort linkedGameTid, ushort linkedGameSid, string linkedGameTrainerName, Span<byte> data, int offset)
    {
        RK4 rk = (RK4) this.GetCompatiblePKM(pk);
        rk.OwnershipType = ownershipType;
        rk.LinkedGame_TID = linkedGameTid;
        rk.LinkedGame_SID = linkedGameSid;
        rk.LinkedGame_TrainerName = linkedGameTrainerName;

        WriteBoxSlot(rk, data, offset);
    }

    public override void WriteBoxSlot(PKM pk, Span<byte> data, int offset)
    {
        bool isBlank = pk.Data.SequenceEqual(BlankPKM.Data);
        if (pk is not RK4)
        {
            WriteBoxSlot(pk, RanchPkOwnershipType.Hayley, 0x00, 0x00, "", data, offset);
            return;
        }

        if (!isBlank && (((RK4)pk).OwnershipType == RanchPkOwnershipType.None))
            ((RK4)pk).OwnershipType = RanchPkOwnershipType.Hayley; // Pokemon without an Ownership type get erased when the save is loaded. Hayley is considered 'default'.

        base.WriteBoxSlot(pk, data, offset);
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

    public override string GetString(ReadOnlySpan<byte> data) => StringConverter4GC.GetStringUnicode(data);

    public override int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength, StringConverterOption option)
    {
        return StringConverter4GC.SetStringUnicode(value, destBuffer, maxLength, option);
    }
}
