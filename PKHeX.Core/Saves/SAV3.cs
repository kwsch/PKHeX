using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 3 <see cref="SaveFile"/> object.
/// </summary>
public abstract class SAV3 : SaveFile, ILangDeviantSave, IEventFlag37, IBoxDetailName, IBoxDetailWallpaper, IDaycareStorage, IDaycareEggState, IDaycareExperience
{
    protected internal sealed override string ShortSummary => $"{OT} ({Version}) - {PlayTimeString}";
    public sealed override string Extension => ".sav";

    public int SaveRevision => Japanese ? 0 : 1;
    public string SaveRevisionString => Japanese ? "J" : "U";
    public bool Japanese { get; }
    public bool Korean => false;

    // Similar to future games, the Generation 3 Mainline save files are comprised of separate objects:
    // Object 1 - Small, containing misc configuration data & the Pokédex.
    // Object 2 - Large, containing everything else that isn't PC Storage system data.
    // Object 3 - Storage, containing all the data for the PC storage system.

    // When the objects are serialized to the savedata, the game fragments each object and saves it to a sector.
    // The main save data for a save file occupies 14 sectors; there are a total of two serialized main saves.
    // After the serialized main save data, there is "extra data", for stuff like Hall of Fame and battle videos.
    // Extra data is always at the same sector, while the main sectors rotate sectors within their region (on each successive save?).

    private const int SIZE_SECTOR = 0x1000;
    public const int SIZE_SECTOR_USED = 0xF80;
    private const int COUNT_MAIN = 14; // sectors worth of data
  //private const int COUNT_BACKUP = COUNT_MAIN; // sectors worth of data
    private const int COUNT_EXTRA = 4; // sectors worth of data
    private const int SIZE_MAIN = COUNT_MAIN * SIZE_SECTOR;

    // There's no harm having buffers larger than their actual size (per format).
    // A checksum consuming extra zeroes does not change the prior checksum result.
    public readonly byte[] Small = new byte[1 * SIZE_SECTOR_USED]; //  [0x890 RS, 0xf24 FR/LG, 0xf2c E]
    public readonly byte[] Large = new byte[4 * SIZE_SECTOR_USED]; //3+[0xc40 RS, 0xee8 FR/LG, 0xf08 E]
    public readonly byte[] Storage = new byte[9 * SIZE_SECTOR_USED]; //  [0x83D0]

    private readonly int ActiveSlot;
    public sealed override int Language { get; set; }

    protected SAV3(bool japanese) => Japanese = japanese;

    protected SAV3(byte[] data) : base(data)
    {
        // Copy sector data to the allocated location
        ReadSectors(data, ActiveSlot = GetActiveSlot(data));

        // OT name is the first 8 bytes of Small. The game fills any unused characters with 0xFF.
        // Japanese games are limited to 5 character OT names; INT 7 characters. +1 0xFF terminator.
        // Since JPN games don't touch the last 2 bytes (alignment), they end up as zeroes!
        Japanese = ReadInt16LittleEndian(Small.AsSpan(0x6)) == 0;
    }

    private void ReadSectors(ReadOnlySpan<byte> data, int group)
    {
        int start = group * SIZE_MAIN;
        int end = start + SIZE_MAIN;
        for (int ofs = start; ofs < end; ofs += SIZE_SECTOR)
        {
            // Get the sector ID for the serialized savedata block, and copy the chunk into the corresponding object.
            var id = ReadInt16LittleEndian(data[(ofs + 0xFF4)..]);
            switch (id)
            {
                case >= 5: data.Slice(ofs, SIZE_SECTOR_USED).CopyTo(Storage.AsSpan((id - 5) * SIZE_SECTOR_USED)); break;
                case >= 1: data.Slice(ofs, SIZE_SECTOR_USED).CopyTo(Large.AsSpan((id - 1) * SIZE_SECTOR_USED)); break;
                default: data.Slice(ofs, SIZE_SECTOR_USED).CopyTo(Small.AsSpan(0)); break;
            }
        }
    }

    private void WriteSectors(Span<byte> data, int group)
    {
        int start = group * SIZE_MAIN;
        int end = start + SIZE_MAIN;
        for (int ofs = start; ofs < end; ofs += SIZE_SECTOR)
        {
            // Get the sector ID for the serialized savedata block, and copy the corresponding chunk of object data into it.
            var id = ReadInt16LittleEndian(data[(ofs + 0xFF4)..]);
            switch (id)
            {
                case >= 5: Storage.AsSpan((id - 5) * SIZE_SECTOR_USED, SIZE_SECTOR_USED).CopyTo(data[ofs..]); break;
                case >= 1: Large.AsSpan((id - 1) * SIZE_SECTOR_USED, SIZE_SECTOR_USED).CopyTo(data[ofs..]); break;
                default: Small.AsSpan(0, SIZE_SECTOR_USED).CopyTo(data[ofs..]); break;
            }
        }
    }

    /// <summary>
    /// Checks the input data to see if all required sectors for the main save data are present for the <see cref="slot"/>.
    /// </summary>
    /// <param name="data">Data to check</param>
    /// <param name="slot">Which main to check (primary or secondary)</param>
    /// <param name="sector0">Offset of the sector that has the small object data</param>
    public static bool IsAllMainSectorsPresent(ReadOnlySpan<byte> data, int slot, out int sector0)
    {
        System.Diagnostics.Debug.Assert(slot is 0 or 1);
        int start = SIZE_MAIN * slot;
        int end = start + SIZE_MAIN;
        int bitTrack = 0; // bit flags for each sector, 1 if present
        sector0 = 0;
        for (int ofs = start; ofs < end; ofs += SIZE_SECTOR)
        {
            var span = data[ofs..];
            var id = ReadInt16LittleEndian(span[0xFF4..]);
            if ((uint)id >= COUNT_MAIN)
                return false; // invalid sector ID
            bitTrack |= (1 << id);
            if (id == 0)
                sector0 = ofs;
        }
        // all 14 fragments present
        return bitTrack == 0b_0011_1111_1111_1111;
    }

    private static int GetActiveSlot(ReadOnlySpan<byte> data)
    {
        if (data.Length == SaveUtil.SIZE_G3RAWHALF) // misconfigured emulator FLASH size
            return 0; // not enough data for a secondary save

        var v0 = IsAllMainSectorsPresent(data, 0, out var sectorZero0);
        var v1 = IsAllMainSectorsPresent(data, 1, out var sectorZero1);
        if (!v0)
            return v1 ? 1 : 0;
        if (!v1)
            return 0;

        return SAV3BlockDetection.CompareFooters(data, sectorZero0, sectorZero1);
    }

    protected sealed override byte[] GetFinalData()
    {
        // Copy Box data back
        WriteSectors(Data, ActiveSlot);
        return base.GetFinalData();
    }

    /// <summary>
    /// Writes the active save data to both save slots (0 and 1).
    /// </summary>
    /// <param name="data">Destination to write to. Usually want to pass in the <see cref="SaveFile.Data"/>.</param>
    /// <remarks>Slot 1 is not written if the binary does not contain it.</remarks>
    public void WriteBothSaveSlots(Span<byte> data)
    {
        WriteSectors(data, 0);
        SetSlotChecksums(data, 0);

        if (data.Length < SaveUtil.SIZE_G3RAW) // don't update second half if it doesn't exist
            return;

        WriteSectors(data, 1);
        SetSlotChecksums(data, 1);
    }

    protected sealed override int SIZE_STORED => PokeCrypto.SIZE_3STORED;
    protected sealed override int SIZE_PARTY => PokeCrypto.SIZE_3PARTY;
    public sealed override PK3 BlankPKM => new();
    public sealed override Type PKMType => typeof(PK3);

    public sealed override ushort MaxMoveID => Legal.MaxMoveID_3;
    public sealed override ushort MaxSpeciesID => Legal.MaxSpeciesID_3;
    public sealed override int MaxAbilityID => Legal.MaxAbilityID_3;
    public override int MaxItemID => Legal.MaxItemID_3;
    public sealed override int MaxBallID => Legal.MaxBallID_3;
    public sealed override GameVersion MaxGameID => Legal.MaxGameID_3;

    public abstract int EventFlagCount { get; }
    public abstract int EventWorkCount { get; }
    protected abstract int EventFlag { get; }
    protected abstract int EventWork { get; }

    /// <summary>
    /// Force loads a new <see cref="SAV3"/> object to the requested <see cref="version"/>.
    /// </summary>
    /// <param name="version"> Version to retrieve for</param>
    /// <returns>New <see cref="SaveFile"/> object.</returns>
    public SAV3 ForceLoad(GameVersion version) => version switch
    {
        GameVersion.R or GameVersion.S or GameVersion.RS => new SAV3RS(Data),
        GameVersion.E => new SAV3E(Data),
        GameVersion.FR or GameVersion.LG or GameVersion.FRLG => new SAV3FRLG(Data),
        _ => throw new ArgumentOutOfRangeException(nameof(version)),
    };

    public sealed override ReadOnlySpan<ushort> HeldItems => Legal.HeldItems_RS;

    public sealed override int BoxCount => 14;
    public sealed override int MaxEV => EffortValues.Max255;
    public sealed override byte Generation => 3;
    public sealed override EntityContext Context => EntityContext.Gen3;
    public sealed override int MaxStringLengthTrainer => 7;
    public sealed override int MaxStringLengthNickname => 10;
    public sealed override int MaxMoney => 999999;

    public sealed override bool HasParty => true;

    public sealed override bool IsPKMPresent(ReadOnlySpan<byte> data) => EntityDetection.IsPresentGBA(data);
    protected sealed override PK3 GetPKM(byte[] data) => new(data);
    protected sealed override byte[] DecryptPKM(byte[] data) => PokeCrypto.DecryptArray3(data);

    protected sealed override Span<byte> BoxBuffer => Storage;
    protected sealed override Span<byte> PartyBuffer => Large;

    private const int COUNT_BOX = 14;
    private const int COUNT_SLOTSPERBOX = 30;

    // Checksums
    private static void SetSlotChecksums(Span<byte> data, int slot)
    {
        int start = slot * SIZE_MAIN;
        int end = start + SIZE_MAIN;
        for (int ofs = start; ofs < end; ofs += SIZE_SECTOR)
        {
            var sector = data.Slice(ofs, SIZE_SECTOR);
            ushort chk = Checksums.CheckSum32(sector[..SIZE_SECTOR_USED]);
            WriteUInt16LittleEndian(sector[0xFF6..], chk);
        }
    }

    protected sealed override void SetChecksums()
    {
        SetSlotChecksums(Data, ActiveSlot);

        if (Data.Length < SaveUtil.SIZE_G3RAW) // don't update HoF for half-sizes
            return;

        for (int i = 0; i < COUNT_EXTRA; i++)
            SetSectorValidExtra(0x1C000 + (i * SIZE_SECTOR));
    }

    public sealed override bool ChecksumsValid
    {
        get
        {
            for (int i = 0; i < COUNT_MAIN; i++)
            {
                if (!IsSectorValid(i))
                    return false;
            }

            if (Data.Length < SaveUtil.SIZE_G3RAW) // don't check HoF for half-sizes
                return true;

            for (int i = 0; i < COUNT_EXTRA; i++)
            {
                if (!IsSectorValidExtra(0x1C000 + (i * SIZE_SECTOR)))
                    return false;
            }

            return true;
        }
    }

    private void SetSectorValidExtra(int offset)
    {
        var sector = Data.AsSpan(offset, SIZE_SECTOR);
        if (IsSectorUninitialized(sector))
            return;
        var expect = Checksums.CheckSum32(sector[..SIZE_SECTOR_USED]);
        WriteUInt16LittleEndian(sector[0xFF4..], expect);
    }

    private bool IsSectorValidExtra(int offset)
    {
        var sector = Data.AsSpan(offset, SIZE_SECTOR);
        if (IsSectorUninitialized(sector))
            return true;
        var expect = Checksums.CheckSum32(sector[..SIZE_SECTOR_USED]);
        var actual = ReadUInt16LittleEndian(sector[0xFF4..]);
        return expect == actual;
    }

    private static bool IsSectorUninitialized(ReadOnlySpan<byte> sector) =>
        sector.IndexOfAnyExcept<byte>(0, 0xFF) == -1;

    private bool IsSectorValid(int sectorIndex)
    {
        int start = ActiveSlot * SIZE_MAIN;
        int ofs = start + (sectorIndex * SIZE_SECTOR);
        var sector = Data.AsSpan(ofs, SIZE_SECTOR);
        var expect = Checksums.CheckSum32(sector[..SIZE_SECTOR_USED]);
        var actual = ReadUInt16LittleEndian(sector[0xFF6..]);
        return expect == actual;
    }

    public sealed override string ChecksumInfo
    {
        get
        {
            var list = new List<string>();
            for (int i = 0; i < COUNT_MAIN; i++)
            {
                if (!IsSectorValid(i))
                    list.Add($"Sector {i} @ {i * SIZE_SECTOR:X5} invalid.");
            }

            if (Data.Length > SaveUtil.SIZE_G3RAW) // don't check HoF for half-sizes
            {
                if (!IsSectorValidExtra(0x1C000))
                    list.Add("HoF first sector invalid.");
                if (!IsSectorValidExtra(0x1D000))
                    list.Add("HoF second sector invalid.");
                if (!IsSectorValidExtra(0x1E000))
                    list.Add("e-Reader data invalid.");
                if (!IsSectorValidExtra(0x1F000))
                    list.Add("Final extra data invalid.");
            }
            return list.Count != 0 ? string.Join(Environment.NewLine, list) : "Checksums are valid.";
        }
    }

    public static bool IsMail(int itemID) => (uint)(itemID - 121) <= (132 - 121);

    protected override void SetPartyValues(PKM pk, bool isParty)
    {
        if (pk is not PK3 p3)
            return;

        // If no mail ID is set, ensure it is set to 0xFF for party and 0x00 for box format.
        // Box format doesn't store this value, but set it anyway for clarity.
        if (!IsMail(p3.HeldItem))
            p3.HeldMailID = isParty ? (sbyte)-1 : (sbyte)0;

        base.SetPartyValues(pk, isParty);
    }

    public abstract uint SecurityKey { get; set; }

    public Span<byte> OriginalTrainerTrash => Small.AsSpan(0, 8);

    public sealed override string OT
    {
        get => GetString(OriginalTrainerTrash);
        set
        {
            int len = Japanese ? 5 : MaxStringLengthTrainer;
            SetString(OriginalTrainerTrash[..len], value, len, StringConverterOption.ClearFF); // match the game-init FF terminating pattern
        }
    }

    public sealed override byte Gender
    {
        get => Small[8];
        set => Small[8] = value;
    }

    public sealed override uint ID32
    {
        get => ReadUInt32LittleEndian(Small.AsSpan(0x0A));
        set => WriteUInt32LittleEndian(Small.AsSpan(0x0A), value);
    }

    public sealed override ushort TID16
    {
        get => ReadUInt16LittleEndian(Small.AsSpan(0xA));
        set => WriteUInt16LittleEndian(Small.AsSpan(0xA), value);
    }

    public sealed override ushort SID16
    {
        get => ReadUInt16LittleEndian(Small.AsSpan(0xC));
        set => WriteUInt16LittleEndian(Small.AsSpan(0xC), value);
    }

    public sealed override int PlayedHours
    {
        get => ReadUInt16LittleEndian(Small.AsSpan(0xE));
        set => WriteUInt16LittleEndian(Small.AsSpan(0xE), (ushort)value);
    }

    public sealed override int PlayedMinutes
    {
        get => Small[0x10];
        set => Small[0x10] = (byte)value;
    }

    public sealed override int PlayedSeconds
    {
        get => Small[0x11];
        set => Small[0x11] = (byte)value;
    }

    public int PlayedFrames
    {
        get => Small[0x12];
        set => Small[0x12] = (byte)value;
    }

    #region Event Flag/Event Work
    public bool GetEventFlag(int flagNumber)
    {
        if ((uint)flagNumber >= EventFlagCount)
            throw new ArgumentOutOfRangeException(nameof(flagNumber), $"Event Flag to get ({flagNumber}) is greater than max ({EventFlagCount}).");
        return GetFlag(EventFlag + (flagNumber >> 3), flagNumber & 7);
    }

    public void SetEventFlag(int flagNumber, bool value)
    {
        if ((uint)flagNumber >= EventFlagCount)
            throw new ArgumentOutOfRangeException(nameof(flagNumber), $"Event Flag to set ({flagNumber}) is greater than max ({EventFlagCount}).");
        SetFlag(EventFlag + (flagNumber >> 3), flagNumber & 7, value);
    }

    public ushort GetWork(int index) => ReadUInt16LittleEndian(Large.AsSpan(EventWork + (index * 2)));
    public void SetWork(int index, ushort value) => WriteUInt16LittleEndian(Large.AsSpan(EventWork)[(index * 2)..], value);
    #endregion

    public sealed override bool GetFlag(int offset, int bitIndex) => GetFlag(Large, offset, bitIndex);
    public sealed override void SetFlag(int offset, int bitIndex, bool value) => SetFlag(Large, offset, bitIndex, value);

    protected abstract int BadgeFlagStart { get; }
    public abstract uint Coin { get; set; }

    public int Badges
    {
        get
        {
            int startFlag = BadgeFlagStart;
            int val = 0;
            for (int i = 0; i < 8; i++)
            {
                if (GetEventFlag(startFlag + i))
                    val |= 1 << i;
            }

            return val;
        }
        set
        {
            int startFlag = BadgeFlagStart;
            for (int i = 0; i < 8; i++)
                SetEventFlag(startFlag + i, (value & (1 << i)) != 0);
        }
    }

    public sealed override IReadOnlyList<InventoryPouch> Inventory
    {
        get
        {
            var pouch = GetItems();
            foreach (var p in pouch)
            {
                if (p.Type != InventoryType.PCItems)
                    p.SecurityKey = SecurityKey;
            }
            return pouch.LoadAll(Large);
        }
        set => value.SaveAll(Large);
    }

    protected abstract InventoryPouch3[] GetItems();
    protected abstract int PokeDex { get; }
    public override bool HasPokeDex => true;

    public int DaycareSlotCount => 2;
    protected abstract int DaycareSlotSize { get; }
    protected abstract int DaycareOffset { get; }
    protected abstract int GetDaycareEXPOffset(int slot);
    public Memory<byte> GetDaycareSlot(int slot) => Large.AsMemory(GetDaycareSlotOffset(slot), DaycareSlotSize);
    public uint GetDaycareEXP(int index) => ReadUInt32LittleEndian(Large.AsSpan(GetDaycareEXPOffset(index)));
    public void SetDaycareEXP(int index, uint value) => WriteUInt32LittleEndian(Large.AsSpan(GetDaycareEXPOffset(index)), value);
    public bool IsDaycareOccupied(int slot) => IsPKMPresent(Large.AsSpan(GetDaycareSlotOffset(slot)));
    public void SetDaycareOccupied(int slot, bool occupied) { /* todo */ }
    public int GetDaycareSlotOffset(int slot) => DaycareOffset + (slot * DaycareSlotSize);
    protected abstract int EggEventFlag { get; }
    public bool IsEggAvailable { get => GetEventFlag(EggEventFlag); set => SetEventFlag(EggEventFlag, value); }

    #region Storage
    public sealed override int GetBoxOffset(int box) => Box + 4 + (SIZE_STORED * box * COUNT_SLOTSPERBOX);

    public sealed override int CurrentBox
    {
        get => Storage[0];
        set => Storage[0] = (byte)value;
    }

    public int GetBoxWallpaper(int box)
    {
        if (box >= COUNT_BOX)
            return box;
        int offset = GetBoxWallpaperOffset(box);
        return Storage[offset];
    }

    private const int COUNT_BOXNAME = 8 + 1;

    public void SetBoxWallpaper(int box, int value)
    {
        if (box >= COUNT_BOX)
            return;
        int offset = GetBoxWallpaperOffset(box);
        Storage[offset] = (byte)value;
    }

    protected int GetBoxWallpaperOffset(int box)
    {
        int offset = GetBoxOffset(COUNT_BOX);
        offset += (COUNT_BOX * COUNT_BOXNAME) + box;
        return offset;
    }

    public string GetBoxName(int box)
    {
        int offset = GetBoxOffset(COUNT_BOX);
        return StringConverter3.GetString(Storage.AsSpan(offset + (box * COUNT_BOXNAME), COUNT_BOXNAME), Japanese);
    }

    public void SetBoxName(int box, ReadOnlySpan<char> value)
    {
        int offset = GetBoxOffset(COUNT_BOX);
        var dest = Storage.AsSpan(offset + (box * COUNT_BOXNAME), COUNT_BOXNAME);
        SetString(dest, value, COUNT_BOXNAME - 1, StringConverterOption.ClearZero);
    }
    #endregion

    #region Pokédex
    protected sealed override void SetDex(PKM pk)
    {
        ushort species = pk.Species;
        if (species is 0 or > Legal.MaxSpeciesID_3)
            return;
        if (pk.IsEgg)
            return;

        switch (species)
        {
            case (int)Species.Unown when !GetSeen(species): // Unown
                DexPIDUnown = pk.PID;
                break;
            case (int)Species.Spinda when !GetSeen(species): // Spinda
                DexPIDSpinda = pk.PID;
                break;
        }
        SetCaught(species, true);
        SetSeen(species, true);
    }

    public uint DexPIDUnown  { get => ReadUInt32LittleEndian(Small.AsSpan(PokeDex + 0x4)); set => WriteUInt32LittleEndian(Small.AsSpan(PokeDex + 0x4), value); }
    public uint DexPIDSpinda { get => ReadUInt32LittleEndian(Small.AsSpan(PokeDex + 0x8)); set => WriteUInt32LittleEndian(Small.AsSpan(PokeDex + 0x8), value); }
    public int DexUnownForm => EntityPID.GetUnownForm3(DexPIDUnown);

    public sealed override bool GetCaught(ushort species)
    {
        int bit = species - 1;
        int ofs = bit >> 3;
        int caughtOffset = PokeDex + 0x10;
        return FlagUtil.GetFlag(Small, caughtOffset + ofs, bit & 7);
    }

    public sealed override void SetCaught(ushort species, bool caught)
    {
        int bit = species - 1;
        int ofs = bit >> 3;
        int caughtOffset = PokeDex + 0x10;
        FlagUtil.SetFlag(Small, caughtOffset + ofs, bit & 7, caught);
    }

    public sealed override bool GetSeen(ushort species)
    {
        int bit = species - 1;
        int ofs = bit >> 3;
        int seenOffset = PokeDex + 0x44;
        return FlagUtil.GetFlag(Small, seenOffset + ofs, bit & 7);
    }

    protected abstract int SeenOffset2 { get; }
    protected abstract int SeenOffset3 { get; }

    public sealed override void SetSeen(ushort species, bool seen)
    {
        int bit = species - 1;
        int ofs = bit >> 3;

        int seenOffset = PokeDex + 0x44;
        FlagUtil.SetFlag(Small, seenOffset + ofs, bit & 7, seen);
        FlagUtil.SetFlag(Large, SeenOffset2 + ofs, bit & 7, seen);
        FlagUtil.SetFlag(Large, SeenOffset3 + ofs, bit & 7, seen);
    }

    public byte PokedexSort
    {
        get => Small[PokeDex + 0x01];
        set => Small[PokeDex + 0x01] = value;
    }

    public byte PokedexMode
    {
        get => Small[PokeDex + 0x01];
        set => Small[PokeDex + 0x01] = value;
    }

    public byte PokedexNationalMagicRSE
    {
        get => Small[PokeDex + 0x02];
        set => Small[PokeDex + 0x02] = value;
    }

    public byte PokedexNationalMagicFRLG
    {
        get => Small[PokeDex + 0x03];
        set => Small[PokeDex + 0x03] = value;
    }

    protected const byte PokedexNationalUnlockRSE = 0xDA;
    protected const byte PokedexNationalUnlockFRLG = 0xB9;
    protected const ushort PokedexNationalUnlockWorkRSE = 0x0302;
    protected const ushort PokedexNationalUnlockWorkFRLG = 0x6258;

    public abstract bool NationalDex { get; set; }
    #endregion

    public sealed override string GetString(ReadOnlySpan<byte> data)
        => StringConverter3.GetString(data, Japanese);
    public override int LoadString(ReadOnlySpan<byte> data, Span<char> destBuffer)
        => StringConverter3.LoadString(data, destBuffer, Japanese);
    public sealed override int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength, StringConverterOption option)
        => StringConverter3.SetString(destBuffer, value, maxLength, Japanese, option);

    protected abstract int MailOffset { get; }
    public int GetMailOffset(int index) => (index * Mail3.SIZE) + MailOffset;

    public MailDetail GetMail(int mailIndex)
    {
        var ofs = GetMailOffset(mailIndex);
        var data = Large.AsSpan(ofs, Mail3.SIZE).ToArray();
        return new Mail3(data, ofs, Japanese);
    }

    #region eBerry
    public abstract Span<byte> EReaderBerry();
    public string EBerryName => GetString(EReaderBerry()[..7]);
    public bool IsEBerryEngima => EReaderBerry()[0] is 0 or 0xFF;
    #endregion

    #region eTrainer
    public abstract Span<byte> EReaderTrainer();
    #endregion

    public abstract Gen3MysteryData MysteryData { get; set; }

    /// <summary>
    /// Hall of Fame data is split across two sectors.
    /// </summary>
    /// <returns>New object containing both sectors merged together.</returns>
    public byte[] GetHallOfFameData()
    {
        Span<byte> savedata = Data;
        var sector1 = savedata.Slice(0x1C000, SIZE_SECTOR_USED);
        var sector2 = savedata.Slice(0x1D000, SIZE_SECTOR_USED);
        return [..sector1, ..sector2];
    }

    /// <summary>
    /// Unmerges the two sectors of Hall of Fame data.
    /// </summary>
    public void SetHallOfFameData(ReadOnlySpan<byte> value)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(value.Length, SIZE_SECTOR_USED * 2);
        Span<byte> savedata = Data;
        var sector1 = savedata.Slice(0x1C000, SIZE_SECTOR_USED);
        var sector2 = savedata.Slice(0x1D000, SIZE_SECTOR_USED);
        value[..SIZE_SECTOR_USED].CopyTo(sector1);
        value[SIZE_SECTOR_USED..].CopyTo(sector2);
    }

    /// <summary>
    /// Only used by Japanese Emerald games.
    /// </summary>
    public Memory<byte> GetEReaderData() => Data.AsMemory(0x1E000, SIZE_SECTOR_USED);

    /// <summary> Only used in Emerald. </summary>
    public Memory<byte> GetFinalExternalData() => Data.AsMemory(0x1F000, SIZE_SECTOR_USED);

    public bool IsCorruptPokedexFF() => MemoryMarshal.Read<ulong>(Small.AsSpan(0xAC)) == ulong.MaxValue;

    public override void CopyChangesFrom(SaveFile sav)
    {
        SetData(sav.Data, 0);
        var s3 = (SAV3)sav;
        SetData(Small, s3.Small);
        SetData(Large, s3.Large);
        SetData(Storage, s3.Storage);
    }

    #region External Connections
    protected abstract int ExternalEventData { get; }
    protected int ExternalEventFlags => ExternalEventData + 0x14;

    public uint ColosseumRaw1
    {
        get => ReadUInt32LittleEndian(Large.AsSpan(ExternalEventData + 7));
        set => WriteUInt32LittleEndian(Large.AsSpan(ExternalEventData + 7), value);
    }

    public uint ColosseumRaw2
    {
        get => ReadUInt32LittleEndian(Large.AsSpan(ExternalEventData + 11));
        set => WriteUInt32LittleEndian(Large.AsSpan(ExternalEventData + 11), value);
    }

    /// <summary>
    /// PokéCoupons stored by Pokémon Colosseum and XD from Mt. Battle runs. Earned PokéCoupons are also added to <see cref="ColosseumCouponsTotal"/>.
    /// </summary>
    /// <remarks>Colosseum/XD caps this at 9,999,999, but will read up to 16,777,215.</remarks>
    public uint ColosseumCoupons
    {
        get => ColosseumRaw1 >> 8;
        set => ColosseumRaw1 = (value << 8) | (ColosseumRaw1 & 0xFF);
    }

    /// <summary> Master Ball from JP Colosseum Bonus Disc; for reaching 30,000 <see cref="ColosseumCouponsTotal"/> </summary>
    public bool ColosseumPokeCouponTitleGold
    {
        get => (ColosseumRaw2 & (1 << 0)) != 0;
        set => ColosseumRaw2 = (ColosseumRaw2 & (1 << 0)) | ((value ? 1u : 0) << 0);
    }

    /// <summary> Light Ball Pikachu from JP Colosseum Bonus Disc; for reaching 5000 <see cref="ColosseumCouponsTotal"/> </summary>
    public bool ColosseumPokeCouponTitleSilver
    {
        get => (ColosseumRaw2 & (1 << 1)) != 0;
        set => ColosseumRaw2 = (ColosseumRaw2 & (1 << 1)) | ((value ? 1u : 0) << 1);
    }

    /// <summary> PP Max from JP Colosseum Bonus Disc; for reaching 2500 <see cref="ColosseumCouponsTotal"/> </summary>
    public bool ColosseumPokeCouponTitleBronze
    {
        get => (ColosseumRaw2 & (1 << 2)) != 0;
        set => ColosseumRaw2 = (ColosseumRaw2 & (1 << 2)) | ((value ? 1u : 0) << 2);
    }

    /// <summary> Received Celebi Gift from JP Colosseum Bonus Disc </summary>
    public bool ColosseumReceivedAgeto
    {
        get => (ColosseumRaw2 & (1 << 3)) != 0;
        set => ColosseumRaw2 = (ColosseumRaw2 & (1 << 3)) | ((value ? 1u : 0) << 3);
    }

    /// <summary>
    /// Used by the JP Colosseum bonus disc. Determines PokéCoupon rank to distribute rewards. Unread in International games.
    /// </summary>
    /// <remarks>
    /// Colosseum/XD caps this at 9,999,999.
    /// </remarks>
    public uint ColosseumCouponsTotal
    {
        get => ColosseumRaw2 >> 8;
        set => ColosseumRaw2 = (value << 8) | (ColosseumRaw2 & 0xFF);
    }

    /// <summary> Indicates if this save has connected to RSBOX and triggered the free False Swipe Swablu Egg giveaway. </summary>
    public bool HasUsedRSBOX
    {
        get => GetFlag(ExternalEventFlags + 0, 0);
        set => SetFlag(ExternalEventFlags + 0, 0, value);
    }

    /// <summary>
    /// 1 for ExtremeSpeed Zigzagoon (at 100 deposited), 2 for Pay Day Skitty (at 500 deposited), 3 for Surf Pichu (at 1499 deposited)
    /// </summary>
    public int RSBoxDepositEggsUnlocked
    {
        get => (Large[ExternalEventFlags] >> 1) & 3;
        set => Large[ExternalEventFlags] = (byte)((Large[ExternalEventFlags] & ~(3 << 1)) | ((value & 3) << 1));
    }

    /// <summary> Received Jirachi Gift from Colosseum Bonus Disc </summary>
    public bool HasReceivedWishmkrJirachi
    {
        get => GetFlag(ExternalEventFlags + 2, 0);
        set => SetFlag(ExternalEventFlags + 2, 0, value);
    }
    #endregion
}
