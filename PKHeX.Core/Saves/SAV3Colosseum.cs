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
    public override IPersonalTable Personal => PersonalTable.RS;
    public override IReadOnlyList<ushort> HeldItems => Legal.HeldItems_COLO;
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

    private int SaveCount = -1;
    private int SaveIndex = -1;
    private readonly StrategyMemo StrategyMemo;
    public const int MaxShadowID = 0x80; // 128
    private int Memo;

    private readonly byte[] BAK;

    public SAV3Colosseum() : base(SaveUtil.SIZE_G3COLO)
    {
        BAK = Array.Empty<byte>();
        StrategyMemo = Initialize();
        ClearBoxes();
    }

    public SAV3Colosseum(byte[] data) : base(data)
    {
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
            if (GetPartySlot(Data, GetPartyOffset(i)).Species != 0)
                PartyCount++;
        }

        var memo = new StrategyMemo(Data, Memo, xd: false);
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
            Span<byte> digest = stackalloc byte[20];
            slot[^20..].CopyTo(digest);

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
        Span<byte> digest = stackalloc byte[20];
        slot[^20..].CopyTo(digest);
        byte[] newSAV = EncryptColosseum(slot, digest);

        // Put save slot back in original save data
        byte[] newFile = MemoryCard != null ? MemoryCard.ReadSaveGameData() : (byte[])BAK.Clone();
        Array.Copy(newSAV, 0, newFile, SLOT_START + (SaveIndex * SLOT_SIZE), newSAV.Length);
        return newFile;
    }

    // Configuration
    protected override SaveFile CloneInternal()
    {
        var data = GetInnerData();
        return new SAV3Colosseum(data) { MemoryCard = MemoryCard };
    }

    protected override int SIZE_STORED => PokeCrypto.SIZE_3CSTORED;
    protected override int SIZE_PARTY => PokeCrypto.SIZE_3CSTORED; // unused
    public override PKM BlankPKM => new CK3();
    public override Type PKMType => typeof(CK3);

    public override int MaxMoveID => Legal.MaxMoveID_3;
    public override int MaxSpeciesID => Legal.MaxSpeciesID_3;
    public override int MaxAbilityID => Legal.MaxAbilityID_3;
    public override int MaxBallID => Legal.MaxBallID_3;
    public override int MaxItemID => Legal.MaxItemID_3_COLO;
    public override int MaxGameID => Legal.MaxGameID_3;

    public override int MaxEV => 255;
    public override int Generation => 3;
    public override EntityContext Context => EntityContext.Gen3;
    protected override int GiftCountMax => 1;
    public override int OTLength => 10; // as evident by Mattle Ho-Oh
    public override int NickLength => 10;
    public override int MaxMoney => 9999999;

    public override int BoxCount => 3;
    public override bool IsPKMPresent(ReadOnlySpan<byte> data) => EntityDetection.IsPresentGC(data);

    private static byte[] EncryptColosseum(ReadOnlySpan<byte> input, Span<byte> digest)
    {
        if (input.Length != SLOT_SIZE)
            throw new ArgumentException(nameof(input));

        byte[] output = input.ToArray();

        // NOT key
        for (int i = 0; i < 20; i++)
            digest[i] = (byte)~digest[i];

        using var sha1 = SHA1.Create();
        for (int i = 0x18; i < 0x1DFD8; i += 20)
        {
            for (int j = 0; j < 20; j++)
                output[i + j] ^= digest[j];
            byte[] key = sha1.ComputeHash(output, i, 20); // update digest
            key.AsSpan(0, 20).CopyTo(digest); // for use in next loop
        }
        return output;
    }

    private static byte[] DecryptColosseum(ReadOnlySpan<byte> input, Span<byte> digest)
    {
        if (input.Length != SLOT_SIZE)
            throw new ArgumentException(nameof(input));

        byte[] output = input.ToArray();

        // NOT key
        for (int i = 0; i < 20; i++)
            digest[i] = (byte)~digest[i];

        using var sha1 = SHA1.Create();
        for (int i = 0x18; i < 0x1DFD8; i += 20)
        {
            byte[] key = sha1.ComputeHash(output, i, 20); // update digest
            for (int j = 0; j < 20; j++)
                output[i + j] ^= digest[j];
            key.AsSpan(0, 20).CopyTo(digest); // for use in next loop
        }
        return output;
    }

    protected override void SetChecksums()
    {
        // Clear Header Checksum
        var headerCHK = Data.AsSpan(12);
        WriteInt32BigEndian(headerCHK, 0);
        // Compute checksum of data
        using var sha1 = SHA1.Create();
        byte[] checksum = sha1.ComputeHash(Data, 0, 0x1DFD8);
        // Set Checksum to end
        var checkSpan = checksum.AsSpan();
        checkSpan.CopyTo(Data.AsSpan(Data.Length - checkSpan.Length));

        // Compute new header checksum
        var header = Data.AsSpan(0, 0x20);
        int newHC = 0;
        for (int i = 0; i < 0x18; i += 4)
            newHC -= ReadInt32BigEndian(header[i..]);
        newHC -= ReadInt32BigEndian(header[0x18..]) ^ ~ReadInt32BigEndian(checkSpan);
        newHC -= ReadInt32BigEndian(header[0x1C..]) ^ ~ReadInt32BigEndian(checkSpan[4..]);

        // Set Header Checksum
        WriteInt32BigEndian(headerCHK, newHC);
    }

    public override bool ChecksumsValid => !ChecksumInfo.Contains("Invalid");

    public override string ChecksumInfo
    {
        get
        {
            byte[] data = (byte[])Data.Clone();
            var hc = data.AsSpan(12);
            int oldHC = ReadInt32BigEndian(hc);
            // Clear Header Checksum
            WriteUInt32BigEndian(hc, 0);
            using var sha1 = SHA1.Create();
            byte[] checksum = sha1.ComputeHash(data, 0, 0x1DFD8);
            var checkSpan = checksum.AsSpan();

            // Compute new header checksum
            var header = data.AsSpan(0, 0x20);
            int newHC = 0;
            for (int i = 0; i < 0x18; i += 4)
                newHC -= ReadInt32BigEndian(header[i..]);

            newHC -= ReadInt32BigEndian(header[0x18..]) ^ ~ReadInt32BigEndian(checkSpan);
            newHC -= ReadInt32BigEndian(header[0x1C..]) ^ ~ReadInt32BigEndian(checkSpan[4..]);

            var chk = data.AsSpan(data.Length - 20, 20);

            bool isHeaderValid = newHC == oldHC;
            bool isBodyValid = chk.SequenceEqual(checkSpan);
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

    public override string GetBoxName(int box)
    {
        return GetString(Box + (0x24A4 * box), 16);
    }

    public override void SetBoxName(int box, string value)
    {
        SetString(Data.AsSpan(Box + (0x24A4 * box), 16), value.AsSpan(), 8, StringConverterOption.ClearZero);
    }

    protected override PKM GetPKM(byte[] data)
    {
        if (data.Length != SIZE_STORED)
            Array.Resize(ref data, SIZE_STORED);
        return new CK3(data);
    }

    protected override byte[] DecryptPKM(byte[] data) => data;

    protected override void SetPKM(PKM pk, bool isParty = false)
    {
        if (pk is not CK3 ck3)
            return;

        ck3.CurrentRegion = (byte)CurrentRegion;
        ck3.OriginalRegion = (byte)OriginalRegion;
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
            entry.TID = pk.TID;
            entry.SID = pk.SID;
        }
        if (entry.Matches(pk.Species, pk.PID, pk.TID, pk.SID))
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
        get => (ushort)PlayedSpan.Hours + (PlayedSpan.Days * 24);
        set { var time = PlayedSpan; PlayedSpan = time - TimeSpan.FromHours(time.Hours) + TimeSpan.FromHours(value); }
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
    public override string OT { get => GetString(0x78, 20); set { SetString(Data.AsSpan(0x78, 20), value.AsSpan(), 10, StringConverterOption.ClearZero); OT2 = value; } }
    public string OT2 { get => GetString(0x8C, 20); set => SetString(Data.AsSpan(0x8C, 20), value.AsSpan(), 10, StringConverterOption.ClearZero); }
    public override int SID { get => ReadUInt16BigEndian(Data.AsSpan(0xA4)); set => WriteUInt16BigEndian(Data.AsSpan(0xA4), (ushort)value); }
    public override int TID { get => ReadUInt16BigEndian(Data.AsSpan(0xA6)); set => WriteUInt16BigEndian(Data.AsSpan(0xA6), (ushort)value); }

    public override int Gender { get => Data[0xAF8]; set => Data[0xAF8] = (byte)value; }
    public override uint Money { get => ReadUInt32BigEndian(Data.AsSpan(0xAFC)); set => WriteUInt32BigEndian(Data.AsSpan(0xAFC), value); }
    public uint Coupons { get => ReadUInt32BigEndian(Data.AsSpan(0xB00)); set => WriteUInt32BigEndian(Data.AsSpan(0xB00), value); }
    public string RUI_Name { get => GetString(0xB3A, 20); set => SetString(Data.AsSpan(0xB3A, 20), value.AsSpan(), 10, StringConverterOption.ClearZero); }

    public override IReadOnlyList<InventoryPouch> Inventory
    {
        get
        {
            InventoryPouch[] pouch =
            {
                new InventoryPouch3GC(InventoryType.Items, Legal.Pouch_Items_COLO, 99, 0x007F8, 20), // 20 COLO, 30 XD
                new InventoryPouch3GC(InventoryType.KeyItems, Legal.Pouch_Key_COLO, 1, 0x00848, 43),
                new InventoryPouch3GC(InventoryType.Balls, Legal.Pouch_Ball_RS, 99, 0x008F4, 16),
                new InventoryPouch3GC(InventoryType.TMHMs, Legal.Pouch_TM_RS, 99, 0x00934, 64), // no HMs
                new InventoryPouch3GC(InventoryType.Berries, Legal.Pouch_Berries_RS, 999, 0x00A34, 46),
                new InventoryPouch3GC(InventoryType.Medicine, Legal.Pouch_Cologne_COLO, 99, 0x00AEC, 3), // Cologne
            };
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
