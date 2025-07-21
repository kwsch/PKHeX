using System;
using System.Buffers;
using System.Collections.Generic;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 3 <see cref="SaveFile"/> object for Pokémon XD saves.
/// </summary>
public sealed class SAV3XD : SaveFile, IGCSaveFile, IBoxDetailName, IDaycareStorage, IDaycareExperience, IGCRegion
{
    protected internal override string ShortSummary => $"{OT} ({Version}) {PlayTimeString}";
    public override string Extension => this.GCExtension();
    public SAV3GCMemoryCard? MemoryCard { get; init; }

    private readonly int SaveCount;
    private readonly int SaveIndex;
    private int Config;
    private int Trainer1;
    private int Memo;
    private int Shadow;
    private readonly StrategyMemo StrategyMemo;
    private readonly ShadowInfoTableXD ShadowInfo;
    public int MaxShadowID => ShadowInfo.Count;
    private int OFS_PouchHeldItem, OFS_PouchKeyItem, OFS_PouchBalls, OFS_PouchTMHM, OFS_PouchBerry, OFS_PouchCologne, OFS_PouchDisc;
    private readonly int[] subOffsets = new int[16];
    private readonly Memory<byte> Container;
    private int DaycareOffset;

    public SAV3XD(bool japanese = false) : base(SaveUtil.SIZE_G3XD)
    {
        Container = Memory<byte>.Empty;
        CurrentRegion = OriginalRegion = japanese ? GCRegion.NTSC_J : GCRegion.NTSC_U;
        // create fake objects
        StrategyMemo = new StrategyMemo();
        ShadowInfo = new ShadowInfoTableXD(japanese);
        Config = 0xA8;
        Trainer1 = 0xCCD8;
        Party = 0xCD08;
        Box = 0x10E08;
        DaycareOffset = 0x1CA68;
        Memo = 0xF678;
        Shadow = 0x1CB48;
        Initialize();
        ClearBoxes();
    }

    public SAV3XD(Memory<byte> data) : this(XDCrypto.DetectLatest(data.Span), data) { }
    private SAV3XD((int Index, int Count) latest, Memory<byte> data, bool decrypt = true) : base(XDCrypto.GetSlot(data, latest.Index))
    {
        if (decrypt)
            XDCrypto.DecryptSlot(Data);
        (SaveIndex, SaveCount) = latest;
        Container = data;
        Japanese = InitializeData(out StrategyMemo, out ShadowInfo);
        Initialize();
    }

    public override PersonalTable3 Personal => PersonalTable.RS;
    public override ReadOnlySpan<ushort> HeldItems => Legal.HeldItems_RS;

    private readonly bool Japanese;

    private bool InitializeData(out StrategyMemo memo, out ShadowInfoTableXD info)
    {
        // Get Offset Info
        Span<ushort> subLength = stackalloc ushort[16];
        for (int i = 0; i < 16; i++)
        {
            subLength[i] = ReadUInt16BigEndian(Data[(0x20 + (2 * i))..]);
            subOffsets[i] = ReadUInt16BigEndian(Data[(0x40 + (4 * i))..]) | (ReadUInt16BigEndian(Data[(0x40 + (4 * i) + 2)..]) << 16);
        }

        // Offsets are displaced by the 0xA8 savedata region
        Config = subOffsets[0] + 0xA8;
        Trainer1 = subOffsets[1] + 0xA8;
        Party = Trainer1 + 0x30;
        Box = subOffsets[2] + 0xA8;
        DaycareOffset = subOffsets[4] + 0xA8;
        Memo = subOffsets[5] + 0xA8;
        Shadow = subOffsets[7] + 0xA8;
        // Purifier = subOffsets[14] + 0xA8;

        bool jp = subLength[7] == 0x1E00;
        memo = new StrategyMemo(Buffer.Slice(Memo, subLength[5]), xd: true);
        info = new ShadowInfoTableXD(Data.Slice(Shadow, subLength[7]), jp);
        return jp;
    }

    private void Initialize()
    {
        OFS_PouchHeldItem = Trainer1 + 0x4C8;
        OFS_PouchKeyItem = Trainer1 + 0x540;
        OFS_PouchBalls = Trainer1 + 0x5EC;
        OFS_PouchTMHM = Trainer1 + 0x62C;
        OFS_PouchBerry = Trainer1 + 0x72C;
        OFS_PouchCologne = Trainer1 + 0x7E4;
        OFS_PouchDisc = Trainer1 + 0x7F0;

        // Since PartyCount is not stored in the save file,
        // Count up how many party slots are active.
        for (int i = 0; i < 6; i++)
        {
            var ofs = GetPartyOffset(i);
            var span = Data[ofs..];
            if (IsPKMPresent(span))
                PartyCount++;
        }
    }

    protected override Memory<byte> GetFinalData()
    {
        var newFile = GetInnerData();

        // Return the gci if Memory Card is not being exported
        if (MemoryCard is null)
            return newFile;

        MemoryCard.WriteSaveGameData(newFile);
        return MemoryCard.Data.ToArray();
    }

    private byte[] GetInnerData()
    {
        // Set Memo Back
        StrategyMemo.Write(); // .CopyTo(Data, Memo);
        ShadowInfo.Write().CopyTo(Data[Shadow..]);
        SetChecksums();

        // Put save slot back in original save data, as a separate array.
        var result = Container.ToArray();
        var region = XDCrypto.GetSlot(result, SaveIndex);
        XDCrypto.EncryptSlot(region.Span);
        return result;
    }

    // Configuration
    protected override SAV3XD CloneInternal() => new((SaveIndex, SaveCount), Container.ToArray(), false) { MemoryCard = MemoryCard };

    protected override int SIZE_STORED => PokeCrypto.SIZE_3XSTORED;
    protected override int SIZE_PARTY => PokeCrypto.SIZE_3XSTORED; // unused
    public override XK3 BlankPKM => new();
    public override Type PKMType => typeof(XK3);

    public override ushort MaxMoveID => Legal.MaxMoveID_3;
    public override ushort MaxSpeciesID => Legal.MaxSpeciesID_3;
    public override int MaxAbilityID => Legal.MaxAbilityID_3;
    public override int MaxBallID => Legal.MaxBallID_3;
    public override int MaxItemID => Legal.MaxItemID_3_XD;
    public override GameVersion MaxGameID => Legal.MaxGameID_3;

    public override int MaxEV => EffortValues.Max255;
    public override byte Generation => 3;
    public override EntityContext Context => EntityContext.Gen3;
    public override int MaxStringLengthTrainer => 7;
    public override int MaxStringLengthNickname => 10;
    public override int MaxMoney => 9999999;

    public override int BoxCount => 8;

    public override bool IsPKMPresent(ReadOnlySpan<byte> data) => EntityDetection.IsPresentGC(data);

    // Checksums
    protected override void SetChecksums() => XDCrypto.SetChecksums(Data, subOffsets[0]);

    public override bool ChecksumsValid => !ChecksumInfo.Contains("Invalid");

    public override string ChecksumInfo
    {
        get
        {
            var pool = ArrayPool<byte>.Shared;
            var rent = pool.Rent(Data.Length);
            var data = rent.AsSpan(0, Data.Length);
            Data.CopyTo(data);
            XDCrypto.SetChecksums(data, subOffsets[0]);

            const int start = 0xA8; // 0x88 + 0x20
            int oldHC = ReadInt32BigEndian(Data[(start + subOffsets[0] + 0x38)..]);
            int newHC = ReadInt32BigEndian(data[(start + subOffsets[0] + 0x38)..]);
            bool header = newHC == oldHC;

            var oldCHK = Data.Slice(0x10, 0x10);
            var newCHK = data.Slice(0x10, 0x10);
            bool body = newCHK.SequenceEqual(oldCHK);

            data.Clear();
            pool.Return(rent);
            return $"Header Checksum {(header ? "V" : "Inv")}alid, Body Checksum {(body ? "V" : "Inv")}alid.";
        }
    }

    // Config
    public GCVersion GCGameIndex   { get => (GCVersion)Data[Config + 0x00];  set => Data[Config + 0x00] = (byte)value; }
    public GCRegion CurrentRegion  { get => (GCRegion)Data[Config + 0x01];   set => Data[Config + 0x01] = (byte)value; }
    public GCRegion OriginalRegion { get => (GCRegion)Data[Config + 0x02];   set => Data[Config + 0x02] = (byte)value; }
    public LanguageGC GCLanguage   { get => (LanguageGC)Data[Config + 0x03]; set => Data[Config + 0x03] = (byte)value; }
    public override int Language { get => (int)GCLanguage.ToLanguageID(); set => GCLanguage = ((LanguageID)value).ToLanguageGC(); }

    private TimeSpan PlayedSpan
    {
        get => TimeSpan.FromSeconds(TotalSeconds);
        set => TotalSeconds = value.TotalSeconds;
    }

    private double TotalSeconds
    {
        get
        {
            if (Japanese)
                return ReadSingleBigEndian(Data[(Config + 0x20)..]);
            return ReadDoubleBigEndian(Data[(Config + 0x30)..]);
        }
        set
        {
            if (Japanese)
                WriteSingleBigEndian(Data[(Config + 0x20)..], (float)value);
            else
                WriteDoubleBigEndian(Data[(Config + 0x30)..], value);
        }
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

    // Trainer Info
    public override GameVersion Version { get => GameVersion.XD; set { } }
    public override string OT { get => GetString(Data.Slice(Trainer1 + 0x00, 20)); set => SetString(Data.Slice(Trainer1 + 0x00, 20), value, 10, StringConverterOption.ClearZero); }
    public override uint ID32 { get => ReadUInt32BigEndian(Data[(Trainer1 + 0x2C)..]); set => WriteUInt32BigEndian(Data[(Trainer1 + 0x2C)..], value); }
    public override ushort SID16 { get => ReadUInt16BigEndian(Data[(Trainer1 + 0x2C)..]); set => WriteUInt16BigEndian(Data[(Trainer1 + 0x2C)..], value); }
    public override ushort TID16 { get => ReadUInt16BigEndian(Data[(Trainer1 + 0x2E)..]); set => WriteUInt16BigEndian(Data[(Trainer1 + 0x2E)..], value); }

    public override byte Gender { get => Data[Trainer1 + 0x8E0]; set => Data[Trainer1 + 0x8E0] = value; }
    public override uint Money { get => ReadUInt32BigEndian(Data[(Trainer1 + 0x8E4)..]); set => WriteUInt32BigEndian(Data[(Trainer1 + 0x8E4)..], value); }
    public uint Coupons { get => ReadUInt32BigEndian(Data[(Trainer1 + 0x8E8)..]); set => WriteUInt32BigEndian(Data[(Trainer1 + 0x8E8)..], value); }

    // Storage
    public override int GetPartyOffset(int slot) => Party + (SIZE_STORED * slot);
    private int GetBoxInfoOffset(int box) => Box + (((30 * SIZE_STORED) + 0x14) * box);
    public override int GetBoxOffset(int box) => GetBoxInfoOffset(box) + 20;
    public string GetBoxName(int box) => GetString(Data.Slice(GetBoxInfoOffset(box), 16));

    public void SetBoxName(int box, ReadOnlySpan<char> value)
    {
        SetString(Data.Slice(GetBoxInfoOffset(box), 20), value, 8, StringConverterOption.ClearZero);
    }

    protected override XK3 GetPKM(byte[] data)
    {
        if (data.Length != SIZE_STORED)
            Array.Resize(ref data, SIZE_STORED);
        return new(data);
    }

    protected override byte[] DecryptPKM(byte[] data) => data;
    public override XK3 GetPartySlot(ReadOnlySpan<byte> data) => GetStoredSlot(data);

    public override XK3 GetStoredSlot(ReadOnlySpan<byte> data)
    {
        // Get Shadow Data
        var pk = (XK3)base.GetStoredSlot(data);

        // Get Shadow Data from save
        var id = pk.ShadowID;
        if (id == 0 || id >= ShadowInfo.Count)
            return pk;

        var entry = ShadowInfo[pk.ShadowID];
        pk.Purification = entry.Purification;
        pk.IsShadow = !entry.IsPurified;
        return pk;
    }

    protected override void SetPKM(PKM pk, bool isParty = false)
    {
        if (pk is not XK3 xk3)
            return; // shouldn't ever hit

        var oldRegion = xk3.CurrentRegion;
        xk3.CurrentRegion = CurrentRegion;
        xk3.OriginalRegion = OriginalRegion;

        StringConverter3GC.RemapGlyphsBetweenRegions3GC(xk3.NicknameTrash, oldRegion, xk3.CurrentRegion, xk3.Language);
        StringConverter3GC.RemapGlyphsBetweenRegions3GC(xk3.OriginalTrainerTrash, oldRegion, xk3.CurrentRegion, xk3.Language);
        xk3.ResetNicknameDisplay();

        // Set Shadow Data back to save
        var id = xk3.ShadowID;
        if (id == 0 || id >= ShadowInfo.Count)
            return;

        var entry = ShadowInfo[id];
        entry.Purification = xk3.Purification;
      //entry.IsPurified = !xk3.IsShadow;
      //entry.Species = xk3.Species;
        entry.PID = xk3.PID;
        entry.IV_HP  = xk3.IV_HP ;
        entry.IV_ATK = xk3.IV_ATK;
        entry.IV_DEF = xk3.IV_DEF;
        entry.IV_SPA = xk3.IV_SPA;
        entry.IV_SPD = xk3.IV_SPD;
        entry.IV_SPE = xk3.IV_SPE;
    }

    protected override void SetDex(PKM pk)
    {
        /*
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
            entry.TID16 = pk.TID16;
            entry.SID16 = pk.SID16;
        }
        if (entry.Matches(pk.Species, pk.PID, pk.TID16, pk.SID16))
        {
            entry.Seen = true;
            entry.Owned = true;
        }
        StrategyMemo.SetEntry(entry);
        */
    }

    public override IReadOnlyList<InventoryPouch> Inventory
    {
        get
        {
            var info = ItemStorage3XD.Instance;
            InventoryPouch[] pouch =
            [
                new InventoryPouch3GC(InventoryType.Items, info, 999, OFS_PouchHeldItem, 30), // 20 COLO, 30 XD
                new InventoryPouch3GC(InventoryType.KeyItems, info, 1, OFS_PouchKeyItem, 43),
                new InventoryPouch3GC(InventoryType.Balls, info, 999, OFS_PouchBalls, 16),
                new InventoryPouch3GC(InventoryType.TMHMs, info, 999, OFS_PouchTMHM, 64),
                new InventoryPouch3GC(InventoryType.Berries, info, 999, OFS_PouchBerry, 46),
                new InventoryPouch3GC(InventoryType.Medicine, info, 999, OFS_PouchCologne, 3), // Cologne
                new InventoryPouch3GC(InventoryType.BattleItems, info, 1, OFS_PouchDisc, 60),
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
    public int DaycareSlotCount => 1;
    public bool IsDaycareOccupied(int slot) => Data[DaycareOffset] != 0;
    public void SetDaycareOccupied(int slot, bool occupied) => Data[DaycareOffset] = (byte)(occupied ? 1 : 0);
    public byte DaycareDepositLevel { get => Data[DaycareOffset + 1]; set => Data[DaycareOffset + 1] = value; }
    public uint GetDaycareEXP(int index) => ReadUInt32BigEndian(Data[(DaycareOffset + 4)..]);
    public void SetDaycareEXP(int index, uint value) => WriteUInt32BigEndian(Data[(DaycareOffset + 4)..], value);
    public Memory<byte> GetDaycareSlot(int slot) => Buffer.Slice(DaycareOffset + 8, PokeCrypto.SIZE_3XSTORED);

    public override string GetString(ReadOnlySpan<byte> data)
        => StringConverter3GC.GetString(data);
    public override int LoadString(ReadOnlySpan<byte> data, Span<char> destBuffer)
        => StringConverter3GC.LoadString(data, destBuffer);
    public override int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength, StringConverterOption option)
        => StringConverter3GC.SetString(destBuffer, value, maxLength, option);
}
