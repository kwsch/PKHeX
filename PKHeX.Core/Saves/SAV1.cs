using System;
using System.Collections.Generic;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 1 <see cref="SaveFile"/> object.
/// </summary>
public sealed class SAV1 : SaveFile, ILangDeviantSave, IEventFlagArray, IEventWorkArray<byte>, IBoxDetailNameRead, IDaycareStorage
{
    protected internal override string ShortSummary => $"{OT} ({Version}) - {PlayTimeString}";
    public override string Extension => ".sav";
    public bool IsVirtualConsole => State.Exportable && Metadata.FileName is { } s && s.StartsWith("sav", StringComparison.Ordinal) && s.Contains(".dat"); // default to GB-Era for non-exportable

    public int SaveRevision => Japanese ? 0 : 1;
    public string SaveRevisionString => (Japanese ? "J" : "U") + (IsVirtualConsole ? "VC" : "GB");
    public bool Japanese { get; }
    public bool Korean => false;
    public override int Language { get; set; }

    private readonly byte[] Reserved = new byte[SIZE_RESERVED];

    private const int SIZE_RESERVED = 0x8000; // unpacked box data
    protected override Span<byte> BoxBuffer => Reserved;
    protected override Span<byte> PartyBuffer => Reserved;
    private readonly SAV1Offsets Offsets;

    public override PersonalTable1 Personal { get; }

    public override ReadOnlySpan<ushort> HeldItems => [];

    public override IReadOnlyList<string> PKMExtensions => Array.FindAll(PKM.Extensions, f =>
    {
        int gen = f[^1] - 0x30;
        return gen is 1 or 2;
    });

    public SAV1(GameVersion version = GameVersion.RBY, LanguageID language = LanguageID.English) : base(SaveUtil.SIZE_G1RAW)
    {
        Version = version;
        Japanese = language == LanguageID.Japanese;
        Language = (int)language;
        Offsets = Japanese ? SAV1Offsets.JPN : SAV1Offsets.INT;
        Personal = version == GameVersion.YW ? PersonalTable.Y : PersonalTable.RB;
        Initialize(version);
        ClearBoxes();
    }

    public SAV1(byte[] data, GameVersion versionOverride = GameVersion.Any, LanguageID language = LanguageID.English) : base(data)
    {
        Japanese = SaveUtil.GetIsG1SAVJ(Data);
        Language = (int)language;
        Offsets = Japanese ? SAV1Offsets.JPN : SAV1Offsets.INT;

        Version = versionOverride != GameVersion.Any ? versionOverride : SaveUtil.GetIsG1SAV(data);
        Personal = Version == GameVersion.YW ? PersonalTable.Y : PersonalTable.RB;
        if (Version == GameVersion.Invalid)
            return;

        Initialize(versionOverride);
    }

    private void Initialize(GameVersion versionOverride)
    {
        // see if RBY can be differentiated
        if (versionOverride is not (GameVersion.RB or GameVersion.YW))
        {
            if (Starter != 0)
                Version = Yellow ? GameVersion.YW : GameVersion.RB;
            else
                Version = Data[Offsets.PikaFriendship] != 0 ? GameVersion.YW : GameVersion.RB;
        }

        Box = 0;
        Party = GetPartyOffset(0);

        int stored = SIZE_BOX_LIST;
        var capacity = BoxSlotCount;
        var current = CurrentBox;
        if (BoxesInitialized) // Current box has flushed to box storage at least once, box contents are trustworthy.
        {
            for (int i = 0; i < BoxCount; i++)
            {
                if (i == current)
                    continue; // Use the current box data instead, loaded a little later.
                int ofs = GetBoxRawDataOffset(i);
                var src = Data.AsSpan(ofs, stored);
                var dest = BoxBuffer[(i * SIZE_BOX_AS_SINGLES)..];
                PokeList1.Unpack(src, dest, StringLength, capacity, false);
            }
        }
        if (current < BoxCount) // Load Current Box
        {
            var src = Data.AsSpan(Offsets.CurrentBox, stored);
            var dest = BoxBuffer[(current * SIZE_BOX_AS_SINGLES)..];
            PokeList1.Unpack(src, dest, StringLength, capacity, false);
        }

        // Stash party immediately after.
        {
            var ofs = Offsets.Party;
            var src = Data.AsSpan(ofs, SIZE_PARTY_LIST);
            var dest = PartyBuffer[GetPartyOffset(0)..];
            PokeList1.Unpack(src, dest, StringLength, 6, true);
        }

        var dc = Data.AsSpan(Offsets.Daycare, 0x38);
        PokeList1.UnpackNOB(dc[1..], PartyBuffer[DaycareOffset..], StringLength);
    }

    private int DaycareOffset => GetPartyOffset(6);
    public override bool HasPokeDex => true;

    // Event Flags
    public int EventFlagCount => 0xA00; // 320 * 8
    public bool GetEventFlag(int flagNumber)
    {
        if ((uint)flagNumber >= EventFlagCount)
            throw new ArgumentOutOfRangeException(nameof(flagNumber), $"Event Flag to get ({flagNumber}) is greater than max ({EventFlagCount}).");
        return GetFlag(Offsets.EventFlag + (flagNumber >> 3), flagNumber & 7);
    }

    public void SetEventFlag(int flagNumber, bool value)
    {
        if ((uint)flagNumber >= EventFlagCount)
            throw new ArgumentOutOfRangeException(nameof(flagNumber), $"Event Flag to set ({flagNumber}) is greater than max ({EventFlagCount}).");
        SetFlag(Offsets.EventFlag + (flagNumber >> 3), flagNumber & 7, value);
    }

    // Event Work
    public int EventWorkCount => 0x100;
    public byte GetWork(int index) => Data[Offsets.EventWork + index];
    public void SetWork(int index, byte value) => Data[Offsets.EventWork + index] = value;

    protected override byte[] GetFinalData()
    {
        int boxListLength = SIZE_BOX_LIST;
        var boxSlotCount = BoxSlotCount;
        bool boxInitialized = BoxesInitialized;
        var current = CurrentBox;
        if (!boxInitialized)
        {
            // Check if any box has content in it.
            bool newContent = AnyBoxSlotSpeciesPresent(current, boxSlotCount);
            if (newContent)
                BoxesInitialized = boxInitialized = true;
            else
                current = CurrentBox = 0; // No content, reset to box 1.
        }

        for (int i = 0; i < BoxCount; i++)
        {
            int ofs = GetBoxRawDataOffset(i);
            var dest = Data.AsSpan(ofs, boxListLength);
            var src = GetUnpackedBoxSpan(i);

            bool written = PokeList1.MergeSingles(src, dest, StringLength, boxSlotCount, false, boxInitialized);
            if (written && i == current) // Ensure the current box is mirrored in the box buffer; easier than having dest be CurrentBox.
                dest.CopyTo(Data.AsSpan(Offsets.CurrentBox));
        }

        // Write Party
        {
            int ofs = Offsets.Party;
            var dest = Data.AsSpan(ofs, SIZE_PARTY_LIST);
            var src = PartyBuffer[GetPartyOffset(0)..];

            PokeList1.MergeSingles(src, dest, StringLength, 6, true);
        }

        // Daycare is read-only, but in case it ever becomes editable, copy it back in.
        PokeList1.PackNOB(PartyBuffer[DaycareOffset..], Data.AsSpan(Offsets.Daycare, 0x38)[1..], StringLength);

        SetChecksums();
        return Data;
    }

    private Span<byte> GetUnpackedBoxSpan(int box)
    {
        var size = SIZE_BOX_AS_SINGLES;
        return BoxBuffer.Slice(box * size, size);
    }

    private bool AnyBoxSlotSpeciesPresent(int current, int boxSlotCount)
    {
        bool newContent = false;
        for (int i = 0; i < BoxCount; i++)
        {
            if (i == current) // Exclude current box in the check.
                continue;

            var src = GetUnpackedBoxSpan(i);
            int count = PokeList1.CountPresent(src, boxSlotCount);
            if (count == 0)
                continue;

            newContent = true;
            break;
        }

        return newContent;
    }

    private int GetBoxRawDataOffset(int box)
    {
        if (box < BoxCount / 2)
            return 0x4000 + (box * SIZE_BOX_LIST);
        return 0x6000 + ((box - (BoxCount / 2)) * SIZE_BOX_LIST);
    }

    // Configuration
    protected override SAV1 CloneInternal() => new(Write(), Version) { Language = Language };

    protected override int SIZE_STORED => Japanese ? PokeCrypto.SIZE_1JLIST : PokeCrypto.SIZE_1ULIST;
    protected override int SIZE_PARTY => SIZE_STORED;
    private int SIZE_BOX_AS_SINGLES => BoxSlotCount * SIZE_STORED;
    private int SIZE_BOX_LIST => (((StringLength * 2) + PokeCrypto.SIZE_1STORED + 1) * BoxSlotCount) + 2;
    private int SIZE_PARTY_LIST => (((StringLength * 2) + PokeCrypto.SIZE_1PARTY + 1) * 6) + 2;

    public override PK1 BlankPKM => new(Japanese);
    public override Type PKMType => typeof(PK1);

    public override ushort MaxMoveID => Legal.MaxMoveID_1;
    public override ushort MaxSpeciesID => Legal.MaxSpeciesID_1;
    public override int MaxAbilityID => Legal.MaxAbilityID_1;
    public override int MaxItemID => Legal.MaxItemID_1;
    public override int MaxBallID => 0; // unused
    public override GameVersion MaxGameID => GameVersion.RBY; // unused
    public override int MaxMoney => 999999;
    public override int MaxCoins => 9999;

    public override int BoxCount => Japanese ? 8 : 12;
    public override int MaxEV => EffortValues.Max12;
    public override int MaxIV => 15;
    public override byte Generation => 1;
    public override EntityContext Context => EntityContext.Gen1;
    public override int MaxStringLengthTrainer => Japanese ? 5 : 7;
    public override int MaxStringLengthNickname => Japanese ? 5 : 10;
    public override int BoxSlotCount => Japanese ? 30 : 20;

    public override bool HasParty => true;
    private int StringLength => Japanese ? GBPKML.StringLengthJapanese : GBPKML.StringLengthNotJapan;

    public override bool IsPKMPresent(ReadOnlySpan<byte> data) => EntityDetection.IsPresentGB(data);

    // Checksums
    protected override void SetChecksums() => Data[Offsets.ChecksumOfs] = GetRBYChecksum(Offsets.OT, Offsets.ChecksumOfs);
    public override bool ChecksumsValid => Data[Offsets.ChecksumOfs] == GetRBYChecksum(Offsets.OT, Offsets.ChecksumOfs);
    public override string ChecksumInfo => ChecksumsValid ? "Checksum valid." : "Checksum invalid";

    private byte GetRBYChecksum(int start, int end)
    {
        var span = Data.AsSpan(start, end - start);
        byte result = 0;
        foreach (ref var b in span)
            result += b;
        return (byte)~result;
    }

    // Trainer Info
    public override GameVersion Version { get; set; }

    public override string OT
    {
        get => GetString(Data.AsSpan(Offsets.OT, MaxStringLengthTrainer));
        set => SetString(Data.AsSpan(Offsets.OT, MaxStringLengthTrainer + 1), value, MaxStringLengthTrainer, StringConverterOption.ClearZero);
    }

    public Span<byte> OriginalTrainerTrash { get => Data.AsSpan(Offsets.OT, StringLength); set { if (value.Length == StringLength) value.CopyTo(Data.AsSpan(Offsets.OT)); } }

    public override byte Gender
    {
        get => 0;
        set { }
    }

    public override uint ID32
    {
        get => TID16;
        set => TID16 = (ushort)value;
    }

    public override ushort TID16
    {
        get => ReadUInt16BigEndian(Data.AsSpan(Offsets.TID16));
        set => WriteUInt16BigEndian(Data.AsSpan(Offsets.TID16), value);
    }

    public override ushort SID16 { get => 0; set { } }

    public string Rival
    {
        get => GetString(Data.AsSpan(Offsets.Rival, MaxStringLengthTrainer));
        set => SetString(Data.AsSpan(Offsets.Rival, MaxStringLengthTrainer), value, MaxStringLengthTrainer, StringConverterOption.Clear50);
    }

    public Span<byte> RivalTrash { get => Data.AsSpan(Offsets.Rival, StringLength); set { if (value.Length == StringLength) value.CopyTo(Data.AsSpan(Offsets.Rival)); } }

    public byte RivalStarter { get => Data[Offsets.Starter - 2]; set => Data[Offsets.Starter - 2] = value; }
    public bool Yellow => Starter == 0x54; // Pikachu
    public byte Starter { get => Data[Offsets.Starter]; set => Data[Offsets.Starter] = value; }

    public ref byte WramD72E => ref Data[Offsets.Starter + 0x17]; // offset relative to player starter

    // bit0 of d72e
    public bool IsSilphLaprasReceived { get => (WramD72E & 1) != 0; set => WramD72E = (byte)((WramD72E & 0xFE) | (value ? 1 : 0)); }

    public byte PikaFriendship
    {
        get => Data[Offsets.PikaFriendship];
        set => Data[Offsets.PikaFriendship] = value;
    }

    public uint PikaBeachScore
    {
        get => BinaryCodedDecimal.ReadUInt32LittleEndian(Data.AsSpan(Offsets.PikaBeachScore, 2));
        set => BinaryCodedDecimal.WriteUInt32LittleEndian(Data.AsSpan(Offsets.PikaBeachScore, 2), Math.Min(9999, value));
    }

    public override string PlayTimeString => !PlayedMaximum ? base.PlayTimeString : $"{base.PlayTimeString} {Checksums.CRC16_CCITT(Data):X4}";

    public override int PlayedHours
    {
        get => Data[Offsets.PlayTime + 0];
        set
        {
            if (value >= byte.MaxValue) // Set 255:00:00.00 and flag
            {
                PlayedMaximum = true;
                value = byte.MaxValue;
                PlayedMinutes = PlayedSeconds = PlayedFrames = 0;
            }
            Data[Offsets.PlayTime + 0] = (byte) value;
        }
    }

    public bool PlayedMaximum
    {
        get => Data[Offsets.PlayTime + 1] != 0;
        set => Data[Offsets.PlayTime + 1] = value ? (byte)1 : (byte)0;
    }

    public override int PlayedMinutes
    {
        get => Data[Offsets.PlayTime + 2];
        set => Data[Offsets.PlayTime + 2] = (byte)value;
    }

    public override int PlayedSeconds
    {
        get => Data[Offsets.PlayTime + 3];
        set => Data[Offsets.PlayTime + 3] = (byte)value;
    }

    public int PlayedFrames
    {
        get => Data[Offsets.PlayTime + 4];
        set => Data[Offsets.PlayTime + 4] = (byte)value;
    }

    public int Badges
    {
        get => Data[Offsets.Badges];
        set => Data[Offsets.Badges] = (byte)value;
    }

    private byte Options
    {
        get => Data[Offsets.Options];
        set => Data[Offsets.Options] = value;
    }

    public bool BattleEffects
    {
        get => (Options & 0x80) == 0;
        set => Options = (byte)((Options & 0x7F) | (value ? 0 : 0x80));
    }

    public bool BattleStyleSwitch
    {
        get => (Options & 0x40) == 0;
        set => Options = (byte)((Options & 0xBF) | (value ? 0 : 0x40));
    }

    public int Sound
    {
        get => (Options & 0x30) >> 4;
        set => Options = (byte)((Options & 0xCF) | ((value & 3) << 4));
    }

    public int TextSpeed
    {
        get => Options & 0x7;
        set => Options = (byte)((Options & 0xF8) | (value & 7));
    }

    // yellow only
    public byte GBPrinterBrightness { get => Data[Offsets.PrinterBrightness]; set => Data[Offsets.PrinterBrightness] = value; }

    public override uint Money
    {
        get => BinaryCodedDecimal.ReadUInt32BigEndian(Data.AsSpan(Offsets.Money, 3));
        set
        {
            value = (uint)Math.Min(value, MaxMoney);
            BinaryCodedDecimal.WriteUInt32BigEndian(Data.AsSpan(Offsets.Money, 3), value);
        }
    }

    public uint Coin
    {
        get => BinaryCodedDecimal.ReadUInt32BigEndian(Data.AsSpan(Offsets.Coin, 2));
        set
        {
            value = (ushort)Math.Min(value, MaxCoins);
            BinaryCodedDecimal.WriteUInt32BigEndian(Data.AsSpan(Offsets.Coin, 2), value);
        }
    }

    public override IReadOnlyList<InventoryPouch> Inventory
    {
        get
        {
            InventoryPouch[] pouch =
            [
                new InventoryPouchGB(InventoryType.Items, ItemStorage1.Instance, 99, Offsets.Items, 20),
                new InventoryPouchGB(InventoryType.PCItems, ItemStorage1.Instance, 99, Offsets.PCItems, 50),
            ];
            return pouch.LoadAll(Data);
        }
        set => value.SaveAll(Data);
    }

    public int DaycareSlotCount => 1;

    public Memory<byte> GetDaycareSlot(int index)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(index, 0, nameof(index));
        return Reserved.AsMemory(DaycareOffset, SIZE_STORED);
    }

    public bool IsDaycareOccupied(int index)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(index, 0, nameof(index));
        return Data[Offsets.Daycare] == 0x01;
    }

    public void SetDaycareOccupied(int index, bool occupied)
    {
        ArgumentOutOfRangeException.ThrowIfNotEqual(index, 0, nameof(index));
        Data[Offsets.Daycare] = (byte)(occupied ? 0x01 : 0x00);
    }

    // Storage
    public override int PartyCount
    {
        get => Data[Offsets.Party];
        protected set => Data[Offsets.Party] = (byte)value;
    }

    public override int GetBoxOffset(int box) => box * SIZE_BOX_AS_SINGLES;
    public override int GetPartyOffset(int slot) => (BoxCount * SIZE_BOX_AS_SINGLES) + (slot * SIZE_STORED);

    public override int CurrentBox
    {
        get => Data[Offsets.CurrentBoxIndex] & 0x7F;
        set => Data[Offsets.CurrentBoxIndex] = (byte)((Data[Offsets.CurrentBoxIndex] & 0x80) | (value & 0x7F));
    }

    public bool BoxesInitialized
    {
        get => (Data[Offsets.CurrentBoxIndex] & 0x80) != 0;
        set => Data[Offsets.CurrentBoxIndex] = (byte)((Data[Offsets.CurrentBoxIndex] & 0x7F) | (byte)(value ? 0x80 : 0));
    }

    public string GetBoxName(int box)
    {
        if (Japanese)
            return BoxDetailNameExtensions.GetDefaultBoxNameJapanese(box);
        return BoxDetailNameExtensions.GetDefaultBoxName(box);
    }

    protected override PK1 GetPKM(byte[] data)
    {
        if (data.Length == SIZE_STORED)
            return PokeList1.ReadFromList(data, StringLength);
        return new(data);
    }

    protected override byte[] DecryptPKM(byte[] data)
    {
        return data;
    }

    // Pokédex
    protected override void SetDex(PKM pk)
    {
        ushort species = pk.Species;
        if (!CanSetDex(species))
            return;

        SetCaught(pk.Species, true);
        SetSeen(pk.Species, true);
    }

    private bool CanSetDex(ushort species)
    {
        if (species == 0)
            return false;
        if (species > MaxSpeciesID)
            return false;
        if (Version == GameVersion.Invalid)
            return false;
        return true;
    }

    public override bool GetSeen(ushort species) => GetDexFlag(Offsets.DexSeen, species);
    public override bool GetCaught(ushort species) => GetDexFlag(Offsets.DexCaught, species);
    public override void SetSeen(ushort species, bool seen) => SetDexFlag(Offsets.DexSeen, species, seen);
    public override void SetCaught(ushort species, bool caught) => SetDexFlag(Offsets.DexCaught, species, caught);

    private bool GetDexFlag(int region, ushort species)
    {
        int bit = species - 1;
        int ofs = bit >> 3;
        return GetFlag(region + ofs, bit & 7);
    }

    private void SetDexFlag(int region, ushort species, bool value)
    {
        int bit = species - 1;
        int ofs = bit >> 3;
        SetFlag(region + ofs, bit & 7, value);
    }

    public override void WriteSlotFormatStored(PKM pk, Span<byte> data)
    {
        // pk that have never been boxed have yet to save the 'current level' for box indication
        // set this value at this time
        ((PK1)pk).Stat_LevelBox = pk.CurrentLevel;
        base.WriteSlotFormatStored(pk, data);
    }

    public override void WriteBoxSlot(PKM pk, Span<byte> data)
    {
        // pk that have never been boxed have yet to save the 'current level' for box indication
        // set this value at this time
        ((PK1)pk).Stat_LevelBox = pk.CurrentLevel;
        base.WriteBoxSlot(pk, data);
    }

    private const int SpawnFlagCount = 0xF0;

    public bool[] EventSpawnFlags
    {
        get
        {
            // RB uses 0xE4 (0xE8) flags, Yellow uses 0xF0 flags. Just grab 0xF0
            bool[] data = new bool[SpawnFlagCount];
            for (int i = 0; i < data.Length; i++)
                data[i] = GetFlag(Offsets.ObjectSpawnFlags + (i >> 3), i & 7);
            return data;
        }
        set
        {
            if (value.Length != SpawnFlagCount)
                return;
            for (int i = 0; i < value.Length; i++)
                SetFlag(Offsets.ObjectSpawnFlags + (i >> 3), i & 7, value[i]);
        }
    }

    public override string GetString(ReadOnlySpan<byte> data)
        => StringConverter1.GetString(data, Japanese);
    public override int LoadString(ReadOnlySpan<byte> data, Span<char> destBuffer)
        => StringConverter1.LoadString(data, destBuffer, Japanese);
    public override int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength, StringConverterOption option)
        => StringConverter1.SetString(destBuffer, value, maxLength, Japanese, option);
}
