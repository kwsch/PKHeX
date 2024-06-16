using System;
using System.Collections.Generic;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 2 <see cref="SaveFile"/> object.
/// </summary>
public sealed class SAV2 : SaveFile, ILangDeviantSave, IEventFlagArray, IEventWorkArray<byte>, IBoxDetailName, IDaycareStorage, IDaycareEggState
{
    protected internal override string ShortSummary => $"{OT} ({Version}) - {PlayTimeString}";
    public override string Extension => ".sav";
    public bool IsVirtualConsole => State.Exportable && Metadata.FileName is { } s && s.StartsWith("sav", StringComparison.Ordinal) && s.Contains(".dat"); // default to GB-Era for non-exportable

    public int SaveRevision => Japanese ? 0 : !Korean ? 1 : 2;
    public string SaveRevisionString => (Japanese ? "J" : !Korean ? "U" : "K") + (IsVirtualConsole ? "VC" : "GB");
    public bool Japanese { get; }
    public bool Korean { get; }
    public override int Language { get; set; }

    private readonly byte[] Reserved = new byte[0x8000];

    protected override Span<byte> BoxBuffer => Reserved;
    protected override Span<byte> PartyBuffer => Reserved;

    private readonly SAV2Offsets Offsets;

    public override PersonalTable2 Personal { get; }
    public override ReadOnlySpan<ushort> HeldItems => Legal.HeldItems_GSC;

    public override IReadOnlyList<string> PKMExtensions => Array.FindAll(PKM.Extensions, f =>
    {
        int gen = f[^1] - 0x30;
        if (Korean)
            return gen == 2;
        return gen is 1 or 2;
    });

    public SAV2(GameVersion version = GameVersion.C, LanguageID language = LanguageID.English) : base(SaveUtil.SIZE_G2RAW_J)
    {
        Version = version;
        switch (language)
        {
            case LanguageID.Japanese:
                Japanese = true;
                Language = 1;
                break;
            case LanguageID.Korean:
                Korean = true;
                Language = (int)LanguageID.Korean;
                break;
            default: // otherwise, both false
                Language = -1;
                break;
        }
        Offsets = new SAV2Offsets(this);
        Personal = Version == GameVersion.C ? PersonalTable.C : PersonalTable.GS;
        Initialize();
        ClearBoxes();
    }

    public SAV2(byte[] data, GameVersion versionOverride = GameVersion.Any) : base(data)
    {
        Version = versionOverride != GameVersion.Any ? versionOverride : SaveUtil.GetIsG2SAV(Data);
        Japanese = SaveUtil.GetIsG2SAVJ(Data) != GameVersion.Invalid;
        if (Version != GameVersion.C && !Japanese)
            Korean = SaveUtil.GetIsG2SAVK(Data) != GameVersion.Invalid;
        Language = Japanese ? 1 : Korean ? (int)LanguageID.Korean : -1;

        Offsets = new SAV2Offsets(this);
        Personal = Version == GameVersion.C ? PersonalTable.C : PersonalTable.GS;
        Initialize();
    }

    private void Initialize()
    {
        Box = Data.Length;
        Party = GetPartyOffset(0);

        // Stash boxes after the save file's end.
        int splitAtIndex = (Japanese ? 6 : 7);
        int stored = SIZE_BOX_LIST;
        var capacity = BoxSlotCount;
        for (int i = 0; i < BoxCount; i++)
        {
            int ofs = GetBoxRawDataOffset(i, splitAtIndex);
            var src = Data.AsSpan(ofs, stored);
            var dest = BoxBuffer[(i * SIZE_BOX_AS_SINGLES)..];
            PokeList2.Unpack(src, dest, StringLength, capacity, false);
        }

        // Don't treat the CurrentBox segment as valid; Stadium ignores it and will de-synchronize it.
        // The main box data segment is the truth, the CurrentBox copy is not always up-to-date.
        // When exporting an updated save file in this program, be nice and re-synchronize.

        // Stash party immediately after.
        {
            var ofs = Offsets.Party;
            var src = Data.AsSpan(ofs, SIZE_PARTY_LIST);
            var dest = PartyBuffer[GetPartyOffset(0)..];
            PokeList2.Unpack(src, dest, StringLength, 6, true);
        }

        if (Offsets.Daycare >= 0)
        {
            var dc0 = PartyBuffer[GetDaycareSlotOffset(0)..];
            PokeList2.UnpackNOB(Data.AsSpan(GetRawDaycareSlotOffset(0)), dc0, StringLength);

            var dc1 = PartyBuffer[GetDaycareSlotOffset(1)..];
            PokeList2.UnpackNOB(Data.AsSpan(GetRawDaycareSlotOffset(1)), dc1, StringLength);

            var egg = PartyBuffer[GetDaycareSlotOffset(2)..];
            PokeList2.UnpackNOB(Data.AsSpan(GetRawDaycareSlotOffset(2)), egg, StringLength);
            if (egg[2] != PokeList2.SlotEmpty)
                egg[1] = PokeList2.SlotEgg;
        }
    }

    private int GetRawDaycareSlotOffset(int index)
    {
        int offset = Offsets.Daycare;

        offset++; // Breeding Status flags
        if (index == 0)
            return offset;

        offset += (StringLength * 2) + PokeCrypto.SIZE_2STORED; // nick/ot/pk
        offset++; // Lady Status flags
        offset++; // Steps until Egg
        offset++; // Ditto Parent state
        if (index == 1)
            return offset;

        offset += (StringLength * 2) + PokeCrypto.SIZE_2STORED; // nick/ot/pk
        return offset;
    }

    public override bool HasPokeDex => true;

    private int EventFlag => Offsets.EventFlag;
    private int EventWork => Offsets.EventWork;

    private int GetBoxRawDataOffset(int i, int splitAtIndex)
    {
        if (i < splitAtIndex)
            return 0x4000 + (i * (SIZE_BOX_LIST + 2));
        return 0x6000 + ((i - splitAtIndex) * (SIZE_BOX_LIST + 2));
    }

    protected override byte[] GetFinalData()
    {
        int splitAtIndex = (Japanese ? 6 : 7);
        int boxListLength = SIZE_BOX_LIST;
        var boxSlotCount = BoxSlotCount;
        for (int i = 0; i < BoxCount; i++)
        {
            int ofs = GetBoxRawDataOffset(i, splitAtIndex);
            var dest = Data.AsSpan(ofs, boxListLength);
            var src = BoxBuffer.Slice(i * SIZE_BOX_AS_SINGLES, SIZE_BOX_AS_SINGLES);

            bool written = PokeList2.MergeSingles(src, dest, StringLength, boxSlotCount, false);
            if (written && i == CurrentBox)
                dest.CopyTo(Data.AsSpan(Offsets.CurrentBox));
        }

        // Write Party
        {
            int ofs = Offsets.Party;
            var dest = Data.AsSpan(ofs, SIZE_PARTY_LIST);
            var src = PartyBuffer[GetPartyOffset(0)..];

            PokeList2.MergeSingles(src, dest, StringLength, 6, true);
        }

        // Write Daycare

        if (Offsets.Daycare >= 0)
        {
            var dc0 = PartyBuffer[GetDaycareSlotOffset(0)..];
            PokeList2.PackNOB(dc0, Data.AsSpan(GetRawDaycareSlotOffset(0)), StringLength);

            var dc1 = PartyBuffer[GetDaycareSlotOffset(1)..];
            PokeList2.PackNOB(dc1, Data.AsSpan(GetRawDaycareSlotOffset(1)), StringLength);

            var egg = PartyBuffer[GetDaycareSlotOffset(2)..];
            PokeList2.PackNOB(egg, Data.AsSpan(GetRawDaycareSlotOffset(2)), StringLength);
        }

        SetChecksums();
        if (Japanese)
        {
            switch (Version)
            {
                case GameVersion.C:
                    Data.AsSpan(Offsets.Trainer1, 0xADA).CopyTo(Data.AsSpan(0x7209)); break;
                default:
                    Data.AsSpan(Offsets.Trainer1, 0xC83).CopyTo(Data.AsSpan(0x7209)); break;
            }
        }
        else if (Korean)
        {
            // Calculate oddball checksum
            ushort sum = 0;
            Span<(ushort, ushort)> offsetpairs =
            [
                (0x106B, 0x1533),
                (0x1534, 0x1A12),
                (0x1A13, 0x1C38),
                (0x3DD8, 0x3F79),
                (0x7E39, 0x7E6A),
            ];
            foreach (var p in offsetpairs)
            {
                for (int i = p.Item1; i < p.Item2; i++)
                    sum += Data[i];
            }
            WriteUInt16LittleEndian(Data.AsSpan(0x7E6B), sum);
        }
        else
        {
            switch (Version)
            {
                case GameVersion.C:
                    Array.Copy(Data, 0x2009, Data, 0x1209, 0xB7A);
                    break;
                default:
                    Array.Copy(Data, 0x2009, Data, 0x15C7, 0x222F - 0x2009);
                    Array.Copy(Data, 0x222F, Data, 0x3D69, 0x23D9 - 0x222F);
                    Array.Copy(Data, 0x23D9, Data, 0x0C6B, 0x2856 - 0x23D9);
                    Array.Copy(Data, 0x2856, Data, 0x7E39, 0x288A - 0x2856);
                    Array.Copy(Data, 0x288A, Data, 0x10E8, 0x2D69 - 0x288A);
                    break;
            }
        }
        return Data;
    }

    // Configuration
    protected override SAV2 CloneInternal() => new(Write(), Version) { Language = Language };

    protected override int SIZE_STORED => Japanese ? PokeCrypto.SIZE_2JLIST : PokeCrypto.SIZE_2ULIST;
    protected override int SIZE_PARTY => SIZE_STORED;
    public override PK2 BlankPKM => new(jp: Japanese);
    public override Type PKMType => typeof(PK2);

    private int SIZE_BOX_AS_SINGLES => BoxSlotCount * SIZE_STORED;
    private int SIZE_BOX_LIST => (((StringLength * 2) + PokeCrypto.SIZE_2STORED + 1) * BoxSlotCount) + 2;
    private int SIZE_PARTY_LIST => (((StringLength * 2) + PokeCrypto.SIZE_2PARTY + 1) * 6) + 2;

    public override ushort MaxMoveID => Legal.MaxMoveID_2;
    public override ushort MaxSpeciesID => Legal.MaxSpeciesID_2;
    public override int MaxAbilityID => Legal.MaxAbilityID_2;
    public override int MaxItemID => Legal.MaxItemID_2;
    public override int MaxBallID => 0; // unused
    public override GameVersion MaxGameID => GameVersion.Gen2; // unused
    public override int MaxMoney => 999999;
    public override int MaxCoins => 9999;

    public override bool IsPKMPresent(ReadOnlySpan<byte> data) => EntityDetection.IsPresentGB(data);

    public int EventWorkCount => 0x100;
    public int EventFlagCount => 2000;

    public override int BoxCount => Japanese ? 9 : 14;
    public override int MaxEV => EffortValues.Max12;
    public override int MaxIV => 15;
    public override byte Generation => 2;
    public override EntityContext Context => EntityContext.Gen2;
    public override int MaxStringLengthTrainer => Japanese || Korean ? 5 : 7;
    public override int MaxStringLengthNickname => Japanese || Korean ? 5 : 10;
    public override int BoxSlotCount => Japanese ? 30 : 20;

    public override bool HasParty => true;
    private int StringLength => Japanese ? GBPKML.StringLengthJapanese : GBPKML.StringLengthNotJapan;

    // Checksums
    private ushort GetChecksum()
    {
        ushort sum = 0;
        for (int i = Offsets.Trainer1; i <= Offsets.AccumulatedChecksumEnd; i++)
            sum += Data[i];
        return sum;
    }

    protected override void SetChecksums()
    {
        ushort accum = GetChecksum();
        WriteUInt16LittleEndian(Data.AsSpan(Offsets.OverallChecksumPosition), accum);
        WriteUInt16LittleEndian(Data.AsSpan(Offsets.OverallChecksumPosition2), accum);
    }

    public override bool ChecksumsValid => !ChecksumInfo.Contains("Invalid");

    public override string ChecksumInfo
    {
        get
        {
            ushort accum = GetChecksum();
            ushort actual = ReadUInt16LittleEndian(Data.AsSpan(Offsets.OverallChecksumPosition));
            ushort actual2 = ReadUInt16LittleEndian(Data.AsSpan(Offsets.OverallChecksumPosition2));

            bool checksum1Valid = (accum == actual);
            bool checksum2Valid = (accum == actual2);
            static string valid(bool s) => s ? "Valid" : "Invalid";
            return $"Checksum 1 {valid(checksum1Valid)}, Checksum 2 {valid(checksum2Valid)}.";
        }
    }

    // Trainer Info
    public override GameVersion Version { get; set; }

    public override string OT
    {
        get => GetString(Data.AsSpan(Offsets.Trainer1 + 2, (Korean ? 2 : 1) * MaxStringLengthTrainer));
        set => SetString(Data.AsSpan(Offsets.Trainer1 + 2, (Korean ? 2 : 1) * MaxStringLengthTrainer), value, 8, StringConverterOption.Clear50);
    }

    public Span<byte> OriginalTrainerTrash
    {
        get => Data.AsSpan(Offsets.Trainer1 + 2, StringLength);
        set { if (value.Length == StringLength) value.CopyTo(Data.AsSpan(Offsets.Trainer1 + 2)); }
    }

    public string Rival
    {
        get => GetString(Data.AsSpan(Offsets.Rival, (Korean ? 2 : 1) * MaxStringLengthTrainer));
        set => SetString(Data.AsSpan(Offsets.Rival, (Korean ? 2 : 1) * MaxStringLengthTrainer), value, 8, StringConverterOption.Clear50);
    }

    public Span<byte> RivalTrash
    {
        get => Data.AsSpan(Offsets.Rival, StringLength);
        set { if (value.Length == StringLength) value.CopyTo(Data.AsSpan(Offsets.Rival)); }
    }

    public override byte Gender
    {
        get => Version == GameVersion.C ? Data[Offsets.Gender] : (byte)0;
        set
        {
            if (Version != GameVersion.C)
                return;
            Data[Offsets.Gender] = value;
            Data[Offsets.Palette] = value;
        }
    }

    public byte Palette
    {
        get => Data[Offsets.Palette];
        set => Data[Offsets.Palette] = value;
    }

    public override uint ID32
    {
        get => TID16;
        set => TID16 = (ushort)value;
    }

    public override ushort TID16
    {
        get => ReadUInt16BigEndian(Data.AsSpan(Offsets.Trainer1));
        set => WriteUInt16BigEndian(Data.AsSpan(Offsets.Trainer1), value);
    }

    public override ushort SID16 { get => 0; set { } }

    public override int PlayedHours
    {
        get => ReadUInt16BigEndian(Data.AsSpan(Offsets.TimePlayed));
        set => WriteUInt16BigEndian(Data.AsSpan(Offsets.TimePlayed), (ushort)value);
    }

    public override int PlayedMinutes
    {
        get => Data[Offsets.TimePlayed + 2];
        set => Data[Offsets.TimePlayed + 2] = (byte)value;
    }

    public override int PlayedSeconds
    {
        get => Data[Offsets.TimePlayed + 3];
        set => Data[Offsets.TimePlayed + 3] = (byte)value;
    }

    public int Badges
    {
        get => ReadUInt16LittleEndian(Data.AsSpan(Offsets.JohtoBadges));
        set { if (value < 0) return; WriteUInt16LittleEndian(Data.AsSpan(Offsets.JohtoBadges), (ushort)value); }
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
        set => Options = (byte)((Options & 0xCF) | ((value != 0 ? 2 : 0) << 4)); // Stereo 2, Mono 0
    }

    public int TextSpeed
    {
        get => Options & 0x7;
        set => Options = (byte)((Options & 0xF8) | (value & 7));
    }

    public bool SaveFileExists
    {
        get => Data[Offsets.Options + 1] == 1;
        set => Data[Offsets.Options + 1] = value ? (byte)1 : (byte)0;
    }

    public int TextBoxFrame // 3bits
    {
        get => Data[Offsets.Options + 2] & 0b0000_0111;
        set => Data[Offsets.Options + 2] = (byte)((Data[Offsets.Options + 2] & 0b1111_1000) | (value & 0b0000_0111));
    }

    public int TextBoxFlags { get => Data[Offsets.Options + 3]; set => Data[Offsets.Options + 3] = (byte)value; }

    public bool TextBoxFrameDelay1 // bit 0
    {
        get => (TextBoxFlags & 0x01) == 0x01;
        set => TextBoxFlags = (TextBoxFlags & ~0x01) | (value ? 0x01 : 0);
    }

    public bool TextBoxFrameDelayNone // bit 4
    {
        get => (TextBoxFlags & 0x10) == 0x10;
        set => TextBoxFlags = (TextBoxFlags & ~0x10) | (value ? 0x10 : 0);
    }

    public byte GBPrinterBrightness { get => Data[Offsets.Options + 4]; set => Data[Offsets.Options + 4] = value; }

    public bool MenuAccountOn
    {
        get => Data[Offsets.Options + 5] == 1;
        set => Data[Offsets.Options + 5] = value ? (byte)1 : (byte)0;
    }

    // 3 bytes
    public override uint Money
    {
        get => ReadUInt32BigEndian(Data.AsSpan(Offsets.Money)) >> 8;
        set
        {
            var clamp = (uint)Math.Min(value, MaxMoney);
            var toWrite = (clamp << 8) | Data[Offsets.Money + 3];
            WriteUInt32BigEndian(Data.AsSpan(Offsets.Money), toWrite);
        }
    }

    public uint Coin
    {
        get => ReadUInt16BigEndian(Data.AsSpan(Offsets.Money + 7));
        set
        {
            var clamped = (ushort)Math.Min(value, MaxCoins);
            WriteUInt16BigEndian(Data.AsSpan(Offsets.Money + 7), clamped);
        }
    }

    public byte BlueCardPoints
    {
        get
        {
            int ofs = Offsets.BlueCardPoints;
            if (ofs == -1)
                return 0;
            return Data[ofs];
        }
        set
        {
            int ofs = Offsets.BlueCardPoints;
            if (ofs == -1)
                return;
            Data[ofs] = value;
        }
    }

    public byte MysteryGiftItem
    {
        get
        {
            int ofs = Offsets.MysteryGiftItem;
            if (ofs == -1)
                return 0;
            if (GetEventFlag(1809))
                return 0;
            return Data[ofs];
        }
        set
        {
            int ofs = Offsets.MysteryGiftItem;
            if (ofs == -1)
                return;

            SetEventFlag(1809, value == 0);
            Data[ofs] = value;
        }
    }

    public bool IsMysteryGiftUnlocked
    {
        get
        {
            int ofs = Offsets.MysteryGiftIsUnlocked;
            if (ofs == -1)
                return false;
            return (sbyte)Data[ofs] >= 0x00; // -1 is disabled; [0,5] is unlocked
        }
        set
        {
            int ofs = Offsets.MysteryGiftIsUnlocked;
            if (ofs == -1)
                return;

            Data[ofs] = (byte)(value ? 0x00 : 0xFF);
            Data[ofs + 2] = Data[ofs];
        }
    }

    public override IReadOnlyList<InventoryPouch> Inventory
    {
        get
        {
            var info = Version == GameVersion.C ? ItemStorage2.InstanceC : ItemStorage2.InstanceGS;
            InventoryPouch[] pouch =
            [
                new InventoryPouchGB(InventoryType.TMHMs, info, 99, Offsets.PouchTMHM, 57),
                new InventoryPouchGB(InventoryType.Items, info, 99, Offsets.PouchItem, 20),
                new InventoryPouchGB(InventoryType.KeyItems, info, 99, Offsets.PouchKey, 26),
                new InventoryPouchGB(InventoryType.Balls, info, 99, Offsets.PouchBall, 12),
                new InventoryPouchGB(InventoryType.PCItems, info, 99, Offsets.PouchPC, 50),
            ];
            return pouch.LoadAll(Data);
        }
        set => value.SaveAll(Data);
    }

    public ref byte DaycareFlagByte(int index)
    {
        var offset = GetDaycareOffset(index);
        return ref Data[offset];
    }

    private int GetDaycareOffset(int index)
    {
        int offset = Offsets.Daycare;
        if (index != 0)
            offset += (StringLength * 2) + PokeCrypto.SIZE_2STORED + 1;
        return offset;
    }

    public int DaycareSlotCount => 2;
    private int GetDaycareSlotOffset(int slot) => GetPartyOffset(6 + slot);
    public Memory<byte> GetDaycareSlot(int slot) => Reserved.AsMemory(GetDaycareSlotOffset(slot), SIZE_STORED);
    public Memory<byte> GetDaycareEgg() => Reserved.AsMemory(GetDaycareSlotOffset(2), SIZE_STORED);
    public bool IsDaycareOccupied(int slot) => (DaycareFlagByte(slot) & 1) != 0;

    public void SetDaycareOccupied(int slot, bool occupied)
    {
        if (occupied)
            DaycareFlagByte(slot) |= 1;
        else
            DaycareFlagByte(slot) &= 0xFE;
    }

    // bit 6 of the first flag byte
    public bool IsEggAvailable { get => (DaycareFlagByte(0) & 0x40) != 0; set => DaycareFlagByte(0) = (byte)((DaycareFlagByte(0) & 0xBF) | (value ? 0x40 : 0)); }

    // Storage
    public override int PartyCount
    {
        get => Data[Offsets.Party]; protected set => Data[Offsets.Party] = (byte)value;
    }

    public override int GetBoxOffset(int box) => box * SIZE_BOX_AS_SINGLES;
    public override int GetPartyOffset(int slot) => (BoxCount * SIZE_BOX_AS_SINGLES) + (slot * SIZE_STORED);

    public override int CurrentBox
    {
        get => Data[Offsets.CurrentBoxIndex];
        set => Data[Offsets.CurrentBoxIndex] = (byte)value;
    }

    public string GetBoxName(int box)
    {
        var result = GetString(GetBoxNameSpan(box));
        if (!Korean)
            result = StringConverter2.InflateLigatures(result, Language);
        return result;
    }

    private Span<byte> GetBoxNameSpan(int box)
    {
        int len = Korean ? 17 : 9;
        return Data.AsSpan(Offsets.BoxNames + (box * len), len);
    }

    public void SetBoxName(int box, ReadOnlySpan<char> value)
    {
        var span = GetBoxNameSpan(box);
        const int maxLen = 8;
        Span<char> deflated = stackalloc char[maxLen];
        int len = StringConverter2.DeflateLigatures(value, deflated, Language);
        SetString(span, deflated[..len], maxLen, StringConverterOption.Clear50);
    }

    protected override PK2 GetPKM(byte[] data)
    {
        if (data.Length == SIZE_STORED)
            return PokeList2.ReadFromList(data, StringLength);
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
        if (species is 0 or > Legal.MaxSpeciesID_2)
            return;
        if (pk.IsEgg)
            return;

        SetCaught(pk.Species, true);
        SetSeen(pk.Species, true);
    }

    private void SetUnownFormFlags()
    {
        // Give all Unown caught to prevent a crash on Pokédex view
        for (int i = 1; i <= 26; i++)
            Data[Offsets.PokedexSeen + 0x1F + i] = (byte)i;
        if (UnownFirstSeen == 0) // Invalid
            UnownFirstSeen = 1; // A
    }

    /// <summary>
    /// Toggles the availability of Unown letter groups in the Wild
    /// </summary>
    /// <remarks>
    /// Max value of 0x0F, 4 bitflags
    /// 1 lsh 0: A, B, C, D, E, F, G, H, I, J, K
    /// 1 lsh 1: L, M, N, O, P, Q, R
    /// 1 lsh 2: S, T, U, V, W
    /// 1 lsh 3: X, Y, Z
    /// </remarks>
    public int UnownUnlocked
    {
        get => Data[Offsets.PokedexSeen + 0x1F + 27];
        set => Data[Offsets.PokedexSeen + 0x1F + 27] = (byte)value;
    }

    /// <summary>
    /// Unlocks all Unown letters/forms in the wild.
    /// </summary>
    public void UnownUnlockAll() => UnownUnlocked = 0x0F; // all 4 bitflags

    /// <summary>
    /// Flag that determines if Unown Letters are available in the wild: A, B, C, D, E, F, G, H, I, J, K
    /// </summary>
    public bool UnownUnlocked0
    {
        get => (UnownUnlocked & (1 << 0)) == 1 << 0;
        set => UnownUnlocked = (UnownUnlocked & ~(1 << 0)) | ((value ? 1 : 0) << 0);
    }

    /// <summary>
    /// Flag that determines if Unown Letters are available in the wild: L, M, N, O, P, Q, R
    /// </summary>
    public bool UnownUnlocked1
    {
        get => (UnownUnlocked & (1 << 1)) == 1 << 1;
        set => UnownUnlocked = (UnownUnlocked & ~(1 << 1)) | ((value ? 1 : 0) << 1);
    }

    /// <summary>
    /// Flag that determines if Unown Letters are available in the wild: S, T, U, V, W
    /// </summary>
    public bool UnownUnlocked2
    {
        get => (UnownUnlocked & (1 << 2)) == 1 << 2;
        set => UnownUnlocked = (UnownUnlocked & ~(1 << 2)) | ((value ? 1 : 0) << 2);
    }

    /// <summary>
    /// Flag that determines if Unown Letters are available in the wild: X, Y, Z
    /// </summary>
    public bool UnownUnlocked3
    {
        get => (UnownUnlocked & (1 << 3)) == 1 << 3;
        set => UnownUnlocked = (UnownUnlocked & ~(1 << 3)) | ((value ? 1 : 0) << 3);
    }

    /// <summary>
    /// Chooses which Unown sprite to show in the regular Pokédex View
    /// </summary>
    public int UnownFirstSeen
    {
        get => Data[Offsets.PokedexSeen + 0x1F + 28];
        set => Data[Offsets.PokedexSeen + 0x1F + 28] = (byte)value;
    }

    public override bool GetSeen(ushort species) => GetDexFlag(Offsets.PokedexSeen, species);
    public override bool GetCaught(ushort species) => GetDexFlag(Offsets.PokedexCaught, species);
    public override void SetSeen(ushort species, bool seen) => SetDexFlag(Offsets.PokedexSeen, species, seen);

    public override void SetCaught(ushort species, bool caught)
    {
        SetDexFlag(Offsets.PokedexCaught, species, caught);
        if (caught && species == (int)Species.Unown)
            SetUnownFormFlags();
    }

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

    public byte GetWork(int index) => Data[EventWork + index];
    public void SetWork(int index, byte value) => Data[EventWork + index] = value;
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

    // Misc
    public ushort ResetKey => GetResetKey();

    private ushort GetResetKey()
    {
        ushort result = 0;
        foreach (var b in Data.AsSpan(Offsets.Money, 3))
            result += b;
        var tr = Data.AsSpan(Offsets.Trainer1, 7);
        var end = tr[2..].IndexOf(StringConverter1.TerminatorCode);
        if (end >= 0)
            tr = tr[..(end + 2)];
        foreach (var b in tr)
            result += b;
        return result;
    }

    /// <summary>
    /// Sets the "Time Not Set" flag to the RTC Flag list.
    /// </summary>
    public void ResetRTC() => Data[Offsets.RTCFlags] |= 0x80;

    public void UnlockAllDecorations()
    {
        for (int i = 676; i <= 721; i++)
            SetEventFlag(i, true);
    }

    public override string GetString(ReadOnlySpan<byte> data)
    {
        if (Korean)
            return StringConverter2KOR.GetString(data);
        return StringConverter2.GetString(data, Language);
    }

    public override int LoadString(ReadOnlySpan<byte> data, Span<char> text)
    {
        if (Korean)
            return StringConverter2KOR.LoadString(data, text);
        return StringConverter2.LoadString(data, text, Language);
    }

    public override int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength, StringConverterOption option)
    {
        if (Korean)
            return StringConverter2KOR.SetString(destBuffer, value, maxLength, option);
        return StringConverter2.SetString(destBuffer, value, maxLength, Language, option);
    }

    public bool IsGBMobileAvailable => Japanese && Version == GameVersion.C;
    public bool IsGBMobileEnabled => Japanese && Enum.IsDefined(typeof(GBMobileCableColor), GBMobileCable);

    public GBMobileCableColor GBMobileCable
    {
        get => (GBMobileCableColor) Data[0xE800];
        set
        {
            Data[0xE800] = (byte)value;
            Data[0x9000] = (byte)(0xFF - value);
        }
    }
}

public enum GBMobileCableColor : byte
{
    None = 0,
    Blue = 1,
    Yellow = 2,
    Green = 3,
    Red = 4,
    Purple = 5,
    Black = 6,
    Pink = 7,
    Gray = 8,
    Debug = 0x81,
    Disabled = 0xFF,
}
