using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 3 <see cref="SaveFile"/> object for Pokémon Colosseum saves.
/// </summary>
public sealed class SAV3Colosseum : SaveFile, IGCSaveFile
{
    protected internal override string ShortSummary => $"{OT} ({Version}) - {PlayTimeString}";
    public override string Extension => this.GCExtension();
    public override PersonalTable3 Personal => PersonalTable.RS;
    public override ReadOnlySpan<ushort> HeldItems => Legal.HeldItems_RS;
    public SAV3GCMemoryCard? MemoryCard { get; init; }

    // 3 Save files are stored
    // 0x0000-0x6000 contains memory card data
    // 0x6000-0x60000 contains the 3 save slots
    // 0x5A000 / 3 = 0x1E000 per save slot
    // Checksum is SHA1 over 0-0x1DFD7, stored in the last 20 bytes of the save slot.
    // Another SHA1 hash is 0x1DFD8, for 20 bytes. Unknown purpose.
    // Checksum is used as the crypto key.

    private const int SLOT_SIZE = 0x1E000;
    private const int SLOT_START = 0x6000;
    private const int SLOT_COUNT = 3;

    private const int sha1HashSize = 20;

    private int SaveCount = -1;
    private int SaveIndex = -1;
    private readonly StrategyMemo StrategyMemo;
    public const int MaxShadowID = 0x80; // 128
    private int Memo;

    private readonly byte[] BAK;
    private readonly bool Japanese;

    public SAV3Colosseum(bool japanese = false) : base(SaveUtil.SIZE_G3COLO)
    {
        Japanese = japanese;
        BAK = [];
        StrategyMemo = Initialize();
        ClearBoxes();
    }

    public SAV3Colosseum(byte[] data) : base(data)
    {
        Japanese = data[0] == 0x83; // Japanese game name first character
        BAK = data;
        InitializeData();
        StrategyMemo = Initialize();
    }

    private StrategyMemo Initialize()
    {
        // Trainer1 = 0x00078;
        Party = 0x000A8;

        Box = 0x00B90;
        DaycareOffset = 0x08170;
        Memo = 0x082B0;

        // Since PartyCount is not stored in the save file,
        // Count up how many party slots are active.
        for (int i = 0; i < 6; i++)
        {
            var ofs = GetPartyOffset(i);
            var span = Data.AsSpan(ofs);
            if (ReadUInt16BigEndian(span) != 0) // species is at offset 0x00
                PartyCount++;
        }

        var memo = new StrategyMemo(Data.AsSpan(Memo), xd: false);
        return memo;
    }

    private void InitializeData()
    {
        // Scan all 3 save slots for the highest counter
        for (int i = 0; i < SLOT_COUNT; i++)
        {
            int slotOffset = SLOT_START + (i * SLOT_SIZE);
            int SaveCounter = ReadInt32BigEndian(Data.AsSpan(slotOffset + 4));
            if (SaveCounter <= SaveCount)
                continue;

            SaveCount = SaveCounter;
            SaveIndex = i;
        }

        // Decrypt most recent save slot
        {
            int slotOffset = SLOT_START + (SaveIndex * SLOT_SIZE);
            ReadOnlySpan<byte> slot = Data.AsSpan(slotOffset, SLOT_SIZE);
            Span<byte> digest = stackalloc byte[sha1HashSize];
            slot[^sha1HashSize..].CopyTo(digest);

            // Decrypt Slot
            Data = DecryptColosseum(slot, digest);
        }
    }

    protected override byte[] GetFinalData()
    {
        var newFile = GetInnerData();

        // Return the gci if Memory Card is not being exported
        if (MemoryCard is null)
            return newFile;

        MemoryCard.WriteSaveGameData(newFile);
        return MemoryCard.Data;
    }

    private byte[] GetInnerData()
    {
        StrategyMemo.Write().CopyTo(Data, Memo);
        SetChecksums();

        // Get updated save slot data
        ReadOnlySpan<byte> slot = Data;
        Span<byte> digest = stackalloc byte[sha1HashSize];
        slot[^sha1HashSize..].CopyTo(digest);
        byte[] newSAV = EncryptColosseum(slot, digest);

        // Put save slot back in original save data
        byte[] newFile = MemoryCard != null ? MemoryCard.ReadSaveGameData().ToArray() : (byte[])BAK.Clone();
        Array.Copy(newSAV, 0, newFile, SLOT_START + (SaveIndex * SLOT_SIZE), newSAV.Length);
        return newFile;
    }

    // Configuration
    protected override SAV3Colosseum CloneInternal() => new(GetInnerData()) { MemoryCard = MemoryCard };

    protected override int SIZE_STORED => PokeCrypto.SIZE_3CSTORED;
    protected override int SIZE_PARTY => PokeCrypto.SIZE_3CSTORED; // unused
    public override CK3 BlankPKM => new();
    public override Type PKMType => typeof(CK3);

    public override ushort MaxMoveID => Legal.MaxMoveID_3;
    public override ushort MaxSpeciesID => Legal.MaxSpeciesID_3;
    public override int MaxAbilityID => Legal.MaxAbilityID_3;
    public override int MaxBallID => Legal.MaxBallID_3;
    public override int MaxItemID => Legal.MaxItemID_3_COLO;
    public override int MaxGameID => Legal.MaxGameID_3;

    public override int MaxEV => EffortValues.Max255;
    public override int Generation => 3;
    public override EntityContext Context => EntityContext.Gen3;
    protected override int GiftCountMax => 1;
    public override int MaxStringLengthOT => 10; // as evident by Mattle Ho-Oh
    public override int MaxStringLengthNickname => 10;
    public override int MaxMoney => 9999999;

    public override int BoxCount => 3;
    public override bool IsPKMPresent(ReadOnlySpan<byte> data) => EntityDetection.IsPresentGC(data);

    private static byte[] EncryptColosseum(ReadOnlySpan<byte> input, Span<byte> digest)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(input.Length, SLOT_SIZE);

        byte[] output = input.ToArray();

        // NOT key
        for (int i = 0; i < digest.Length; i++)
            digest[i] = (byte)~digest[i];

        const int start = 0x18;
        const int end = (SLOT_SIZE - (2 * sha1HashSize));
        var crypt = output.AsSpan();
        for (int i = start; i < end; i += sha1HashSize)
        {
            var slice = crypt.Slice(i, digest.Length);
            for (int j = 0; j < digest.Length; j++)
                slice[j] ^= digest[j];
            SHA1.HashData(slice, digest); // update digest
        }
        return output;
    }

    private static byte[] DecryptColosseum(ReadOnlySpan<byte> input, Span<byte> digest)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(input.Length, SLOT_SIZE);

        byte[] output = input.ToArray();

        // NOT key
        for (int i = 0; i < digest.Length; i++)
            digest[i] = (byte)~digest[i];

        Span<byte> hash = stackalloc byte[sha1HashSize];
        const int start = 0x18;
        const int end = (SLOT_SIZE - (2 * sha1HashSize));
        var crypt = output.AsSpan();
        for (int i = start; i < end; i += sha1HashSize)
        {
            var slice = crypt.Slice(i, Math.Min(crypt.Length - i, sha1HashSize));
            SHA1.HashData(slice, hash); // update digest
            for (int j = 0; j < slice.Length; j++)
                slice[j] ^= digest[j];
            hash.CopyTo(digest); // for use in next loop
        }
        return output;
    }

    protected override void SetChecksums()
    {
        var data = Data.AsSpan();
        var header = data[..0x20];
        var payload = data[..(SLOT_SIZE - (2 * sha1HashSize))];
        var hash = data[^sha1HashSize..];
        var headerCHK = data[0x0C..];

        // Clear Header Checksum
        WriteInt32BigEndian(headerCHK, 0);
        // Compute checksum of data
        SHA1.HashData(payload, hash);

        // Compute new header checksum
        int newHC = ComputeHeaderChecksum(header, hash);

        // Set Header Checksum
        WriteInt32BigEndian(headerCHK, newHC);
    }

    private static int ComputeHeaderChecksum(Span<byte> header, Span<byte> hash)
    {
        int result = 0;
        for (int i = 0; i < 0x18; i += 4)
            result -= ReadInt32BigEndian(header[i..]);
        result -= ReadInt32BigEndian(header[0x18..]) ^ ~ReadInt32BigEndian(hash);
        result -= ReadInt32BigEndian(header[0x1C..]) ^ ~ReadInt32BigEndian(hash[4..]);
        return result;
    }

    public override bool ChecksumsValid => !ChecksumInfo.Contains("Invalid");

    public override string ChecksumInfo
    {
        get
        {
            var data = Data.AsSpan();
            var header = data[..0x20];
            var payload = data[..(SLOT_SIZE - (2 * sha1HashSize))];
            var storedHash = data[^sha1HashSize..];
            var hc = header[0x0C..];

            int oldHC = ReadInt32BigEndian(hc);
            // Clear Header Checksum
            WriteInt32BigEndian(hc, 0);
            Span<byte> currentHash = stackalloc byte[sha1HashSize];
            SHA1.HashData(payload, currentHash);

            // Compute new header checksum
            int newHC = ComputeHeaderChecksum(header, currentHash);

            // Restore old header checksum
            WriteInt32BigEndian(hc, oldHC);
            bool isHeaderValid = newHC == oldHC;
            bool isBodyValid = storedHash.SequenceEqual(currentHash);
            static string valid(bool s) => s ? "Valid" : "Invalid";
            return $"Header Checksum {valid(isHeaderValid)}, Body Checksum {valid(isBodyValid)}.";
        }
    }

    // Trainer Info
    public override GameVersion Version { get => GameVersion.COLO; protected set { } }

    // Storage
    public override int GetPartyOffset(int slot)
    {
        return Party + (SIZE_STORED * slot);
    }

    public override int GetBoxOffset(int box)
    {
        return Box + (((30 * SIZE_STORED) + 0x14)*box) + 0x14;
    }

    private Span<byte> GetBoxNameSpan(int box) => Data.AsSpan(Box + (0x24A4 * box), 16);

    public override string GetBoxName(int box)
    {
        return GetString(GetBoxNameSpan(box));
    }

    public override void SetBoxName(int box, ReadOnlySpan<char> value)
    {
        SetString(GetBoxNameSpan(box), value, 8, StringConverterOption.ClearZero);
    }

    protected override CK3 GetPKM(byte[] data)
    {
        if (data.Length != SIZE_STORED)
            Array.Resize(ref data, SIZE_STORED);
        return new(data);
    }

    protected override byte[] DecryptPKM(byte[] data) => data;

    protected override void SetPKM(PKM pk, bool isParty = false)
    {
        if (pk is not CK3 ck3)
            return;

        ck3.CurrentRegion = (byte)CurrentRegion;
        ck3.OriginalRegion = (byte)OriginalRegion;

        ck3.ForceCorrectFatefulState(Japanese, ck3.FatefulEncounter);
    }

    protected override void SetDex(PKM pk)
    {
        if (pk.Species is 0 or > Legal.MaxSpeciesID_3)
            return;
        if (pk.IsEgg)
            return;

        // Dex Related
        var entry = StrategyMemo.GetEntry(pk.Species);
        if (entry.IsEmpty) // Populate
        {
            entry.Species = pk.Species;
            entry.PID = pk.PID;
            entry.ID32 = pk.ID32;
        }
        if (entry.Matches(pk.Species, pk.PID, pk.ID32))
        {
            entry.Seen = true;
            entry.Owned = true;
        }
        StrategyMemo.SetEntry(entry);
    }

    // Config
    private const int Config = 0x08;

    public GCVersion GCGameIndex { get => (GCVersion)Data[Config + 0x00]; set => Data[Config + 0x00] = (byte)value; }
    public GCRegion CurrentRegion { get => (GCRegion)Data[Config + 0x01]; set => Data[Config + 0x01] = (byte)value; }
    public GCRegion OriginalRegion { get => (GCRegion)Data[Config + 0x02]; set => Data[Config + 0x02] = (byte)value; }
    public LanguageGC GCLanguage { get => (LanguageGC)Data[Config + 0x03]; set => Data[Config + 0x03] = (byte)value; }
    public override int Language { get => (int)GCLanguage.ToLanguageID(); set => GCLanguage = ((LanguageID)value).ToLanguageGC(); }

    private TimeSpan PlayedSpan
    {
        get => TimeSpan.FromSeconds(ReadSingleBigEndian(Data.AsSpan(Config + 0x20)));
        set => WriteSingleBigEndian(Data.AsSpan(Config + 0x20), (float)value.TotalSeconds);
    }

    public override int PlayedHours
    {
        get => (ushort)PlayedSpan.TotalHours;
        set { var time = PlayedSpan; PlayedSpan = time - TimeSpan.FromHours((int)time.TotalHours) + TimeSpan.FromHours(value); }
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

    // Trainer Info (offset 0x78, length 0xB18, end @ 0xB90)
    public override string OT { get => GetString(Data.AsSpan(0x78, 20)); set { SetString(Data.AsSpan(0x78, 20), value, 10, StringConverterOption.ClearZero); OT2 = value; } }
    public string OT2 { get => GetString(Data.AsSpan(0x8C, 20)); set => SetString(Data.AsSpan(0x8C, 20), value, 10, StringConverterOption.ClearZero); }

    public override uint ID32 { get => ReadUInt32BigEndian(Data.AsSpan(0xA4)); set => WriteUInt32BigEndian(Data.AsSpan(0xA4), value); }
    public override ushort SID16 { get => ReadUInt16BigEndian(Data.AsSpan(0xA4)); set => WriteUInt16BigEndian(Data.AsSpan(0xA4), value); }
    public override ushort TID16 { get => ReadUInt16BigEndian(Data.AsSpan(0xA6)); set => WriteUInt16BigEndian(Data.AsSpan(0xA6), value); }

    public override int Gender { get => Data[0xAF8]; set => Data[0xAF8] = (byte)value; }
    public override uint Money { get => ReadUInt32BigEndian(Data.AsSpan(0xAFC)); set => WriteUInt32BigEndian(Data.AsSpan(0xAFC), value); }
    public uint Coupons { get => ReadUInt32BigEndian(Data.AsSpan(0xB00)); set => WriteUInt32BigEndian(Data.AsSpan(0xB00), value); }
    public string RUI_Name { get => GetString(Data.AsSpan(0xB3A, 20)); set => SetString(Data.AsSpan(0xB3A, 20), value, 10, StringConverterOption.ClearZero); }

    public override IReadOnlyList<InventoryPouch> Inventory
    {
        get
        {
            var info = ItemStorage3Colo.Instance;
            InventoryPouch[] pouch =
            [
                new InventoryPouch3GC(InventoryType.Items, info, 99, 0x007F8, 20), // 20 COLO, 30 XD
                new InventoryPouch3GC(InventoryType.KeyItems, info, 1, 0x00848, 43),
                new InventoryPouch3GC(InventoryType.Balls, info, 99, 0x008F4, 16),
                new InventoryPouch3GC(InventoryType.TMHMs, info, 99, 0x00934, 64), // no HMs
                new InventoryPouch3GC(InventoryType.Berries, info, 999, 0x00A34, 46),
                new InventoryPouch3GC(InventoryType.Medicine, info, 99, 0x00AEC, 3), // Cologne
            ];
            return pouch.LoadAll(Data);
        }
        set => value.SaveAll(Data);
    }

    // Daycare Structure:
    // 0x00 -- Occupied
    // 0x01 -- Deposited Level
    // 0x02-0x03 -- unused?
    // 0x04-0x07 -- Initial EXP
    public override int GetDaycareSlotOffset(int loc, int slot) { return DaycareOffset + 8; }
    public override uint? GetDaycareEXP(int loc, int slot) { return null; }
    public override bool? IsDaycareOccupied(int loc, int slot) { return null; }
    public override void SetDaycareEXP(int loc, int slot, uint EXP) { }
    public override void SetDaycareOccupied(int loc, int slot, bool occupied) { }

    public override string GetString(ReadOnlySpan<byte> data) => StringConverter3GC.GetString(data);

    public override int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength, StringConverterOption option)
    {
        return StringConverter3GC.SetString(destBuffer, value, maxLength, option);
    }
}
