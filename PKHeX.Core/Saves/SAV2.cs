using System;
using System.Collections.Generic;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 2 <see cref="SaveFile"/> object.
/// </summary>
public sealed class SAV2 : SaveFile, ILangDeviantSave, IEventFlagArray, IEventWorkArray<byte>
{
    protected internal override string ShortSummary => $"{OT} ({Version}) - {PlayTimeString}";
    public override string Extension => ".sav";
    public bool IsVirtualConsole => State.Exportable && Metadata.FileName is { } s && s.StartsWith("sav", StringComparison.Ordinal) && s.Contains(".dat"); // default to GB-Era for non-exportable

    public int SaveRevision => Japanese ? 0 : !Korean ? 1 : 2;
    public string SaveRevisionString => (Japanese ? "J" : !Korean ? "U" : "K") + (IsVirtualConsole ? "VC" : "GB");
    public bool Japanese { get; }
    public bool Korean { get; }

    public override IPersonalTable Personal { get; }
    public override IReadOnlyList<ushort> HeldItems => Legal.HeldItems_GSC;

    public override IReadOnlyList<string> PKMExtensions => Array.FindAll(PKM.Extensions, f =>
    {
        int gen = f[^1] - 0x30;
        if (Korean)
            return gen == 2;
        return gen is 1 or 2;
    });

    public SAV2(GameVersion version = GameVersion.C, LanguageID lang = LanguageID.English) : base(SaveUtil.SIZE_G2RAW_J)
    {
        Version = version;
        switch (lang)
        {
            case LanguageID.Japanese:
                Japanese = true;
                break;
            case LanguageID.Korean:
                Korean = true;
                break;
            // otherwise, both false
        }
        Offsets = new SAV2Offsets(this);
        Personal = Version == GameVersion.GS ? PersonalTable.GS : PersonalTable.C;
        Initialize();
        ClearBoxes();
    }

    public SAV2(byte[] data, GameVersion versionOverride = GameVersion.Any) : base(data)
    {
        Version = versionOverride != GameVersion.Any ? versionOverride : SaveUtil.GetIsG2SAV(Data);
        Japanese = SaveUtil.GetIsG2SAVJ(Data) != GameVersion.Invalid;
        if (Version != GameVersion.C && !Japanese)
            Korean = SaveUtil.GetIsG2SAVK(Data) != GameVersion.Invalid;

        Offsets = new SAV2Offsets(this);
        Personal = Version == GameVersion.GS ? PersonalTable.GS : PersonalTable.C;
        Initialize();
    }

    private void Initialize()
    {
        Box = Data.Length;
        Array.Resize(ref Data, Data.Length + SIZE_RESERVED);
        Party = GetPartyOffset(0);

        // Stash boxes after the save file's end.
        int splitAtIndex = (Japanese ? 6 : 7);
        int stored = SIZE_STOREDBOX;
        int baseDest = Data.Length - SIZE_RESERVED;
        var capacity = Japanese ? PokeListType.StoredJP : PokeListType.Stored;
        for (int i = 0; i < BoxCount; i++)
        {
            int ofs = GetBoxRawDataOffset(i, splitAtIndex);
            var box = GetData(ofs, stored);
            var boxDest = baseDest + (i * SIZE_BOX);
            var boxPL = new PokeList2(box, capacity, Japanese);
            for (int j = 0; j < boxPL.Pokemon.Length; j++)
            {
                var dest = boxDest + (j * SIZE_STORED);
                var pkDat = (j < boxPL.Count)
                    ? new PokeList2(boxPL[j]).Write()
                    : new byte[PokeList2.GetDataLength(PokeListType.Single, Japanese)];
                pkDat.CopyTo(Data, dest);
            }
        }

        var current = GetData(Offsets.CurrentBox, stored);
        var curBoxPL = new PokeList2(current, capacity, Japanese);
        var curDest = baseDest + (CurrentBox * SIZE_BOX);
        for (int i = 0; i < curBoxPL.Pokemon.Length; i++)
        {
            var dest = curDest + (i * SIZE_STORED);
            var pkDat = i < curBoxPL.Count
                ? new PokeList2(curBoxPL[i]).Write()
                : new byte[PokeList2.GetDataLength(PokeListType.Single, Japanese)];
            pkDat.CopyTo(Data, dest);
        }

        var party = GetData(Offsets.Party, SIZE_STOREDPARTY);
        var partyPL = new PokeList2(party, PokeListType.Party, Japanese);
        for (int i = 0; i < partyPL.Pokemon.Length; i++)
        {
            var dest = GetPartyOffset(i);
            var pkDat = i < partyPL.Count
                ? new PokeList2(partyPL[i]).Write()
                : new byte[PokeList2.GetDataLength(PokeListType.Single, Japanese)];
            pkDat.CopyTo(Data, dest);
        }

        if (Offsets.Daycare >= 0)
        {
            int offset = Offsets.Daycare;

            DaycareFlags[0] = Data[offset];
            offset++;
            var pk1 = ReadPKMFromOffset(offset); // parent 1
            var daycare1 = new PokeList2(pk1);
            offset += (StringLength * 2) + 0x20; // nick/ot/pk
            DaycareFlags[1] = Data[offset];
            offset++;
            //byte steps = Data[offset];
            offset++;
            //byte BreedMotherOrNonDitto = Data[offset];
            offset++;
            var pk2 = ReadPKMFromOffset(offset); // parent 2
            var daycare2 = new PokeList2(pk2);
            offset += (StringLength * 2) + PokeCrypto.SIZE_2STORED; // nick/ot/pk
            var pk3 = ReadPKMFromOffset(offset); // egg!
            pk3.IsEgg = true;
            var daycare3 = new PokeList2(pk3);

            daycare1.Write().CopyTo(Data, GetPartyOffset(7 + (0 * 2)));
            daycare2.Write().CopyTo(Data, GetPartyOffset(7 + (1 * 2)));
            daycare3.Write().CopyTo(Data, GetPartyOffset(7 + (2 * 2)));
            DaycareOffset = Offsets.Daycare;
        }

        // Enable Pokedex editing
        PokeDex = 0;
    }

    private int EventFlag => Offsets.EventFlag;
    private int EventWork => Offsets.EventWork;

    private PK2 ReadPKMFromOffset(int offset)
    {
        var stringLength = StringLength;
        var span = Data.AsSpan(offset);

        var pkData = span.Slice(stringLength * 2, PokeCrypto.SIZE_2STORED).ToArray();
        var pk = new PK2(pkData, jp: Japanese);

        var nick = span[..stringLength];
        var ot = span.Slice(stringLength, stringLength);
        nick.CopyTo(pk.RawNickname);
        ot.CopyTo(pk.RawOT);

        return pk;
    }

    private const int SIZE_RESERVED = 0x8000; // unpacked box data
    private readonly SAV2Offsets Offsets;

    private int GetBoxRawDataOffset(int i, int splitAtIndex)
    {
        if (i < splitAtIndex)
            return 0x4000 + (i * (SIZE_STOREDBOX + 2));
        return 0x6000 + ((i - splitAtIndex) * (SIZE_STOREDBOX + 2));
    }

    protected override byte[] GetFinalData()
    {
        int splitAtIndex = (Japanese ? 6 : 7);
        for (int i = 0; i < BoxCount; i++)
        {
            var boxPL = new PokeList2(Japanese ? PokeListType.StoredJP : PokeListType.Stored, Japanese);
            int slot = 0;
            for (int j = 0; j < boxPL.Pokemon.Length; j++)
            {
                PK2 boxPK = (PK2) GetPKM(GetData(GetBoxOffset(i) + (j * SIZE_STORED), SIZE_STORED));
                if (boxPK.Species > 0)
                    boxPL[slot++] = boxPK;
            }

            int src = GetBoxRawDataOffset(i, splitAtIndex);
            boxPL.Write().CopyTo(Data, src);
            if (i == CurrentBox)
                boxPL.Write().CopyTo(Data, Offsets.CurrentBox);
        }

        var partyPL = new PokeList2(PokeListType.Party, Japanese);
        int pSlot = 0;
        for (int i = 0; i < 6; i++)
        {
            PK2 partyPK = (PK2)GetPKM(GetData(GetPartyOffset(i), SIZE_STORED));
            if (partyPK.Species > 0)
                partyPL[pSlot++] = partyPK;
        }
        partyPL.Write().CopyTo(Data, Offsets.Party);

        SetChecksums();
        if (Japanese)
        {
            switch (Version)
            {
                case GameVersion.GS: Data.AsSpan(Offsets.Trainer1, 0xC83).CopyTo(Data.AsSpan(0x7209)); break;
                case GameVersion.C:  Data.AsSpan(Offsets.Trainer1, 0xADA).CopyTo(Data.AsSpan(0x7209)); break;
            }
        }
        else if (Korean)
        {
            // Calculate oddball checksum
            ushort sum = 0;
            Span<(ushort, ushort)> offsetpairs = stackalloc (ushort,ushort)[]
            {
                (0x106B, 0x1533),
                (0x1534, 0x1A12),
                (0x1A13, 0x1C38),
                (0x3DD8, 0x3F79),
                (0x7E39, 0x7E6A),
            };
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
                case GameVersion.GS:
                    Array.Copy(Data, 0x2009, Data, 0x15C7, 0x222F - 0x2009);
                    Array.Copy(Data, 0x222F, Data, 0x3D69, 0x23D9 - 0x222F);
                    Array.Copy(Data, 0x23D9, Data, 0x0C6B, 0x2856 - 0x23D9);
                    Array.Copy(Data, 0x2856, Data, 0x7E39, 0x288A - 0x2856);
                    Array.Copy(Data, 0x288A, Data, 0x10E8, 0x2D69 - 0x288A);
                    break;
                case GameVersion.C:
                    Array.Copy(Data, 0x2009, Data, 0x1209, 0xB7A);
                    break;
            }
        }
        byte[] outData = new byte[Data.Length - SIZE_RESERVED];
        Array.Copy(Data, outData, outData.Length);
        return outData;
    }

    // Configuration
    protected override SaveFile CloneInternal() => new SAV2(Write(), Version);

    protected override int SIZE_STORED => Japanese ? PokeCrypto.SIZE_2JLIST : PokeCrypto.SIZE_2ULIST;
    protected override int SIZE_PARTY => Japanese ? PokeCrypto.SIZE_2JLIST : PokeCrypto.SIZE_2ULIST;
    public override PKM BlankPKM => new PK2(jp: Japanese);
    public override Type PKMType => typeof(PK2);

    private int SIZE_BOX => BoxSlotCount*SIZE_STORED;
    private int SIZE_STOREDBOX => PokeList2.GetDataLength(Japanese ? PokeListType.StoredJP : PokeListType.Stored, Japanese);
    private int SIZE_STOREDPARTY => PokeList2.GetDataLength(PokeListType.Party, Japanese);

    public override int MaxMoveID => Legal.MaxMoveID_2;
    public override int MaxSpeciesID => Legal.MaxSpeciesID_2;
    public override int MaxAbilityID => Legal.MaxAbilityID_2;
    public override int MaxItemID => Legal.MaxItemID_2;
    public override int MaxBallID => 0; // unused
    public override int MaxGameID => 99; // unused
    public override int MaxMoney => 999999;
    public override int MaxCoins => 9999;

    public override bool IsPKMPresent(ReadOnlySpan<byte> data) => EntityDetection.IsPresentGB(data);

    public int EventWorkCount => 0x100;
    public int EventFlagCount => 2000;

    public override int BoxCount => Japanese ? 9 : 14;
    public override int MaxEV => 65535;
    public override int MaxIV => 15;
    public override int Generation => 2;
    public override EntityContext Context => EntityContext.Gen2;
    protected override int GiftCountMax => 0;
    public override int OTLength => Japanese || Korean ? 5 : 7;
    public override int NickLength => Japanese || Korean ? 5 : 10;
    public override int BoxSlotCount => Japanese ? 30 : 20;

    public override bool HasParty => true;
    public override bool HasNamableBoxes => true;
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

    public override bool ChecksumsValid
    {
        get
        {
            ushort accum = GetChecksum();
            ushort actual = ReadUInt16LittleEndian(Data.AsSpan(Offsets.OverallChecksumPosition));
            return accum == actual;
        }
    }

    public override string ChecksumInfo => ChecksumsValid ? "Checksum valid." : "Checksum invalid";

    // Trainer Info
    public override GameVersion Version { get; protected set; }

    public override string OT
    {
        get => GetString(Offsets.Trainer1 + 2, (Korean ? 2 : 1) * OTLength);
        set => SetString(Data.AsSpan(Offsets.Trainer1 + 2, (Korean ? 2 : 1) * OTLength), value.AsSpan(), 8, StringConverterOption.Clear50);
    }

    public Span<byte> OT_Trash
    {
        get => Data.AsSpan(Offsets.Trainer1 + 2, StringLength);
        set { if (value.Length == StringLength) value.CopyTo(Data.AsSpan(Offsets.Trainer1 + 2)); }
    }

    public string Rival
    {
        get => GetString(Offsets.Rival, (Korean ? 2 : 1) * OTLength);
        set => SetString(Data.AsSpan(Offsets.Rival, (Korean ? 2 : 1) * OTLength), value.AsSpan(), 8, StringConverterOption.Clear50);
    }

    public Span<byte> Rival_Trash
    {
        get => Data.AsSpan(Offsets.Rival, StringLength);
        set { if (value.Length == StringLength) value.CopyTo(Data.AsSpan(Offsets.Rival)); }
    }

    public override int Gender
    {
        get => Version == GameVersion.C ? Data[Offsets.Gender] : 0;
        set
        {
            if (Version != GameVersion.C)
                return;
            Data[Offsets.Gender] = (byte) value;
            Data[Offsets.Palette] = (byte) value;
        }
    }

    public override int TID
    {
        get => ReadUInt16BigEndian(Data.AsSpan(Offsets.Trainer1));
        set => WriteUInt16BigEndian(Data.AsSpan(Offsets.Trainer1), (ushort)value);
    }

    public override int SID { get => 0; set { } }

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

    private static ushort[] LegalItems => Legal.Pouch_Items_GSC;
    private ushort[] LegalKeyItems => Version == GameVersion.C? Legal.Pouch_Key_C : Legal.Pouch_Key_GS;
    private static ushort[] LegalBalls => Legal.Pouch_Ball_GSC;
    private static ushort[] LegalTMHMs => Legal.Pouch_TMHM_GSC;

    public override IReadOnlyList<InventoryPouch> Inventory
    {
        get
        {
            InventoryPouch[] pouch =
            {
                new InventoryPouchGB(InventoryType.TMHMs, LegalTMHMs, 99, Offsets.PouchTMHM, 57),
                new InventoryPouchGB(InventoryType.Items, LegalItems, 99, Offsets.PouchItem, 20),
                new InventoryPouchGB(InventoryType.KeyItems, LegalKeyItems, 99, Offsets.PouchKey, 26),
                new InventoryPouchGB(InventoryType.Balls, LegalBalls, 99, Offsets.PouchBall, 12),
                new InventoryPouchGB(InventoryType.PCItems, ArrayUtil.ConcatAll(LegalItems, LegalKeyItems, LegalBalls, LegalTMHMs), 99, Offsets.PouchPC, 50),
            };
            return pouch.LoadAll(Data);
        }
        set => value.SaveAll(Data);
    }

    private readonly byte[] DaycareFlags = new byte[2];
    public override int GetDaycareSlotOffset(int loc, int slot) => GetPartyOffset(7 + (slot * 2));
    public override uint? GetDaycareEXP(int loc, int slot) => null;
    public override bool? IsDaycareOccupied(int loc, int slot) => (DaycareFlags[slot] & 1) != 0;
    public override void SetDaycareEXP(int loc, int slot, uint EXP) { }
    public override void SetDaycareOccupied(int loc, int slot, bool occupied) { }

    // Storage
    public override int PartyCount
    {
        get => Data[Offsets.Party]; protected set => Data[Offsets.Party] = (byte)value;
    }

    public override int GetBoxOffset(int box)
    {
        return Data.Length - SIZE_RESERVED + (box * SIZE_BOX);
    }

    public override int GetPartyOffset(int slot)
    {
        return Data.Length - SIZE_RESERVED + (BoxCount * SIZE_BOX) + (slot * SIZE_STORED);
    }

    public override int CurrentBox
    {
        get => Data[Offsets.CurrentBoxIndex] & 0x7F;
        set => Data[Offsets.CurrentBoxIndex] = (byte)((Data[Offsets.CurrentBoxIndex] & 0x80) | (value & 0x7F));
    }

    public bool CurrentBoxChanged
    {
        get => (Data[Offsets.CurrentBoxIndex] & 0x80) != 0;
        set => Data[Offsets.CurrentBoxIndex] = (byte)((Data[Offsets.CurrentBoxIndex] & 0x7F) | (byte)(value ? 0x80 : 0));
    }

    public override string GetBoxName(int box)
    {
        int len = Korean ? 17 : 9;
        return GetString(Offsets.BoxNames + (box * len), len);
    }

    public override void SetBoxName(int box, string value)
    {
        int len = Korean ? 17 : 9;
        var span = Data.AsSpan(Offsets.BoxNames + (box * len), len);
        SetString(span, value.AsSpan(), 8, StringConverterOption.Clear50);
    }

    protected override PKM GetPKM(byte[] data)
    {
        if (data.Length == SIZE_STORED)
            return new PokeList2(data, PokeListType.Single, Japanese)[0];
        return new PK2(data);
    }

    protected override byte[] DecryptPKM(byte[] data)
    {
        return data;
    }

    // Pokédex
    protected override void SetDex(PKM pk)
    {
        int species = pk.Species;
        if (species is 0 or > Legal.MaxSpeciesID_2)
            return;
        if (pk.IsEgg)
            return;

        SetCaught(pk.Species, true);
        SetSeen(pk.Species, true);
    }

    private void SetUnownFormFlags()
    {
        // Give all Unown caught to prevent a crash on pokedex view
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

    public override bool GetSeen(int species) => GetDexFlag(Offsets.PokedexSeen, species);
    public override bool GetCaught(int species) => GetDexFlag(Offsets.PokedexCaught, species);
    public override void SetSeen(int species, bool seen) => SetDexFlag(Offsets.PokedexSeen, species, seen);

    public override void SetCaught(int species, bool caught)
    {
        SetDexFlag(Offsets.PokedexCaught, species, caught);
        if (caught && species == (int)Species.Unown)
            SetUnownFormFlags();
    }

    private bool GetDexFlag(int region, int species)
    {
        int bit = species - 1;
        int ofs = bit >> 3;
        return GetFlag(region + ofs, bit & 7);
    }

    private void SetDexFlag(int region, int species, bool value)
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
        var value = (TID >> 8) + (TID & 0xFF) + ((Money >> 16) & 0xFF) + ((Money >> 8) & 0xFF) + (Money & 0xFF);
        var ot = Data.AsSpan(Offsets.Trainer1 + 2, 5);
        var sum = 0;
        foreach (var b in ot)
        {
            if (b == StringConverter12.G1TerminatorCode)
                break;
            sum += b;
        }
        return (ushort)(value + sum);
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
        return StringConverter12.GetString(data, Japanese);
    }

    public override int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength, StringConverterOption option)
    {
        if (Korean)
            return StringConverter2KOR.SetString(destBuffer, value, maxLength, option);
        return StringConverter12.SetString(destBuffer, value, maxLength, Japanese, option);
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
