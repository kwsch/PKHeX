using System;
using static System.Buffers.Binary.BinaryPrimitives;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core;

/// <summary> Pokémon HOME <see cref="PKM"/> format. </summary>
public sealed class PKH : PKM, IHandlerLanguage, IFormArgument, IHomeTrack, IBattleVersion, ITrainerMemories, IRibbonSetAffixed, IContestStats, IScaledSize,
    IRibbonSetRibbons, IRibbonSetMarks, IAppliedMarkings7
{
    public readonly GameDataCore Core;
    public GameDataPB7? DataPB7 { get; private set; }
    public GameDataPK8? DataPK8 { get; private set; }
    public GameDataPA8? DataPA8 { get; private set; }
    public GameDataPB8? DataPB8 { get; private set; }
    public GameDataPK9? DataPK9 { get; private set; }

    public override EntityContext Context => EntityContext.None;

    public PKH(byte[] data) : base(DecryptHome(data))
    {
        var mem = Data.AsMemory(HomeCrypto.SIZE_1HEADER + 2);
        var core = mem[..CoreDataSize];
        var side = mem.Slice(core.Length + 2, GameDataSize);

        Core = new GameDataCore(core);
        ReadGameData1(side);
    }

    public PKH() : base(HomeCrypto.SIZE_STORED)
    {
        CoreDataSize = HomeCrypto.SIZE_CORE;

        var mem = Data.AsMemory(HomeCrypto.SIZE_1HEADER + 2);
        var core = mem[..CoreDataSize];
        Core = new GameDataCore(core) { AffixedRibbon = -1 };
    }

    private void ReadGameData1(Memory<byte> data)
    {
        // Can potentially have no side-game data (GO imports)
        while (data.Length != 0)
        {
            var span = data.Span;
            var format = (HomeGameDataFormat)span[0];
            var length = ReadUInt16LittleEndian(span[1..]);
            data = data[HomeOptional1.HeaderSize..];
            var chunk = data[..length];
            data = data[chunk.Length..];

            _ = ReadGameData1(chunk, format);
        }
    }

    private IGameDataSide ReadGameData1(Memory<byte> chunk, HomeGameDataFormat format) => format switch
    {
        HomeGameDataFormat.PB7 => DataPB7 = new GameDataPB7(chunk),
        HomeGameDataFormat.PK8 => DataPK8 = new GameDataPK8(chunk),
        HomeGameDataFormat.PA8 => DataPA8 = new GameDataPA8(chunk),
        HomeGameDataFormat.PB8 => DataPB8 = new GameDataPB8(chunk),
        HomeGameDataFormat.PK9 => DataPK9 = new GameDataPK9(chunk),
        _ => throw new ArgumentException($"Unknown {nameof(HomeGameDataFormat)} {format}"),
    };

    private static byte[] DecryptHome(byte[] data)
    {
        HomeCrypto.DecryptIfEncrypted(ref data);
      //Array.Resize(ref data, HomeCrypto.SIZE_1STORED);
        return data;
    }

    public ushort DataVersion     { get => ReadUInt16LittleEndian(Data.AsSpan(0x00)); set => WriteUInt16LittleEndian(Data.AsSpan(0x00), value); }
    public ulong EncryptionSeed   { get => ReadUInt64LittleEndian(Data.AsSpan(0x02)); set => WriteUInt64LittleEndian(Data.AsSpan(0x02), value); }
    public uint Checksum          { get => ReadUInt32LittleEndian(Data.AsSpan(0x0A)); set => WriteUInt32LittleEndian(Data.AsSpan(0x0A), value); }
    public ushort EncodedDataSize { get => ReadUInt16LittleEndian(Data.AsSpan(0x0E)); set => WriteUInt16LittleEndian(Data.AsSpan(0x0E), value); }
    public ushort CoreDataSize    { get => ReadUInt16LittleEndian(Data.AsSpan(0x10)); set => WriteUInt16LittleEndian(Data.AsSpan(0x10), value); }
    public ushort GameDataSize    { get => ReadUInt16LittleEndian(Data.AsSpan(0x12 + CoreDataSize)); set => WriteUInt16LittleEndian(Data.AsSpan(0x12 + CoreDataSize), value); }

    private const int GameDataStart = HomeCrypto.SIZE_1HEADER + 2 + HomeCrypto.SIZE_CORE + 2;

    public override Span<byte> NicknameTrash => Core.NicknameTrash;
    public override Span<byte> OriginalTrainerTrash => Core.OriginalTrainerTrash;
    public override Span<byte> HandlingTrainerTrash => Core.HandlingTrainerTrash;
    public override int TrashCharCountTrainer => 13;
    public override int TrashCharCountNickname => 13;
    public override bool IsUntraded => ReadUInt16LittleEndian(HandlingTrainerTrash) == 0; // immediately terminated HandlingTrainerName data (\0)

    #region Core

    public ulong Tracker { get => Core.Tracker; set => Core.Tracker = value; }
    public override uint EncryptionConstant { get => Core.EncryptionConstant; set => Core.EncryptionConstant = value; }
    public bool IsBadEgg { get => Core.IsBadEgg; set => Core.IsBadEgg = value; }
    public override ushort Species { get => Core.Species; set => Core.Species = value; }
    public override uint ID32 { get => Core.ID32; set => Core.ID32 = value; }
    public override ushort TID16 { get => Core.TID16; set => Core.TID16 = value; }
    public override ushort SID16 { get => Core.SID16; set => Core.SID16 = value; }
    public override uint EXP { get => Core.EXP; set => Core.EXP = value; }
    public bool Favorite { get => Core.IsFavorite; set => Core.IsFavorite = value; }
    public ushort MarkingValue { get => Core.MarkingValue; set => Core.MarkingValue = value; }
    public override uint PID { get => Core.PID; set => Core.PID = value; }
    public override Nature Nature { get => Core.Nature; set => Core.Nature = value; }
    public override Nature StatNature { get => Core.StatNature; set => Core.StatNature = value; }
    public override bool FatefulEncounter { get => Core.FatefulEncounter; set => Core.FatefulEncounter = value; }
    public override byte Gender { get => Core.Gender; set => Core.Gender = value; }
    public override byte Form { get => Core.Form; set => Core.Form = value; }
    public override int EV_HP { get => Core.EV_HP; set => Core.EV_HP = value; }
    public override int EV_ATK { get => Core.EV_ATK; set => Core.EV_ATK = value; }
    public override int EV_DEF { get => Core.EV_DEF; set => Core.EV_DEF = value; }
    public override int EV_SPE { get => Core.EV_SPE; set => Core.EV_SPE = value; }
    public override int EV_SPA { get => Core.EV_SPA; set => Core.EV_SPA = value; }
    public override int EV_SPD { get => Core.EV_SPD; set => Core.EV_SPD = value; }
    public byte ContestCool { get => Core.ContestCool; set => Core.ContestCool = value; }
    public byte ContestBeauty { get => Core.ContestBeauty; set => Core.ContestBeauty = value; }
    public byte ContestCute { get => Core.ContestCute; set => Core.ContestCute = value; }
    public byte ContestSmart { get => Core.ContestSmart; set => Core.ContestSmart = value; }
    public byte ContestTough { get => Core.ContestTough; set => Core.ContestTough = value; }
    public byte ContestSheen { get => Core.ContestSheen; set => Core.ContestSheen = value; }
    public byte HeightScalar { get => Core.HeightScalar; set => Core.HeightScalar = value; }
    public byte WeightScalar { get => Core.WeightScalar; set => Core.WeightScalar = value; }
    public override int Stat_HPCurrent { get => Core.Stat_HPCurrent; set => Core.Stat_HPCurrent = value; }
    public override int IV_HP { get => Core.IV_HP; set => Core.IV_HP = value; }
    public override int IV_ATK { get => Core.IV_ATK; set => Core.IV_ATK = value; }
    public override int IV_DEF { get => Core.IV_DEF; set => Core.IV_DEF = value; }
    public override int IV_SPE { get => Core.IV_SPE; set => Core.IV_SPE = value; }
    public override int IV_SPA { get => Core.IV_SPA; set => Core.IV_SPA = value; }
    public override int IV_SPD { get => Core.IV_SPD; set => Core.IV_SPD = value; }
    public override bool IsEgg { get => Core.IsEgg; set => Core.IsEgg = value; }
    public override bool IsNicknamed { get => Core.IsNicknamed; set => Core.IsNicknamed = value; }
    public override int Status_Condition { get => Core.Status_Condition; set => Core.Status_Condition = value; }
    public override byte HandlingTrainerGender { get => Core.HandlingTrainerGender; set => Core.HandlingTrainerGender = value; }
    public byte HandlingTrainerLanguage { get => Core.HandlingTrainerLanguage; set => Core.HandlingTrainerLanguage = value; }
    public override byte CurrentHandler { get => Core.CurrentHandler; set => Core.CurrentHandler = value; }
    public ushort HandlingTrainerID { get => Core.HandlingTrainerID; set => Core.HandlingTrainerID = value; }
    public override byte HandlingTrainerFriendship { get => Core.HandlingTrainerFriendship; set => Core.HandlingTrainerFriendship = value; }
    public byte HandlingTrainerMemoryIntensity { get => Core.HandlingTrainerMemoryIntensity; set => Core.HandlingTrainerMemoryIntensity = value; }
    public byte HandlingTrainerMemory { get => Core.HandlingTrainerMemory; set => Core.HandlingTrainerMemory = value; }
    public byte HandlingTrainerMemoryFeeling { get => Core.HandlingTrainerMemoryFeeling; set => Core.HandlingTrainerMemoryFeeling = value; }
    public ushort HandlingTrainerMemoryVariable { get => Core.HandlingTrainerMemoryVariable; set => Core.HandlingTrainerMemoryVariable = value; }
    public override GameVersion Version { get => Core.Version; set => Core.Version = value; }
    public GameVersion BattleVersion { get => Core.BattleVersion; set => Core.BattleVersion = value; }
    public override int Language { get => Core.Language; set => Core.Language = value; }
    public uint FormArgument { get => Core.FormArgument; set => Core.FormArgument = value; }
    public byte FormArgumentRemain { get => Core.FormArgumentRemain; set => Core.FormArgumentRemain = value; }
    public byte FormArgumentElapsed { get => Core.FormArgumentElapsed; set => Core.FormArgumentElapsed = value; }
    public byte FormArgumentMaximum { get => Core.FormArgumentMaximum; set => Core.FormArgumentMaximum = value; }
    public sbyte AffixedRibbon { get => Core.AffixedRibbon; set => Core.AffixedRibbon = value; }
    public override byte OriginalTrainerFriendship { get => Core.OriginalTrainerFriendship; set => Core.OriginalTrainerFriendship = value; }
    public byte OriginalTrainerMemoryIntensity { get => Core.OriginalTrainerMemoryIntensity; set => Core.OriginalTrainerMemoryIntensity = value; }
    public byte OriginalTrainerMemory { get => Core.OriginalTrainerMemory; set => Core.OriginalTrainerMemory = value; }
    public ushort OriginalTrainerMemoryVariable { get => Core.OriginalTrainerMemoryVariable; set => Core.OriginalTrainerMemoryVariable = value; }
    public byte OriginalTrainerMemoryFeeling { get => Core.OriginalTrainerMemoryFeeling; set => Core.OriginalTrainerMemoryFeeling = value; }
    public override byte EggYear { get => Core.EggYear; set => Core.EggYear = value; }
    public override byte EggMonth { get => Core.EggMonth; set => Core.EggMonth = value; }
    public override byte EggDay { get => Core.EggDay; set => Core.EggDay = value; }
    public override byte MetYear { get => Core.MetYear; set => Core.MetYear = value; }
    public override byte MetMonth { get => Core.MetMonth; set => Core.MetMonth = value; }
    public override byte MetDay { get => Core.MetDay; set => Core.MetDay = value; }
    public override byte MetLevel { get => Core.MetLevel; set => Core.MetLevel = value; }
    public override byte OriginalTrainerGender { get => Core.OriginalTrainerGender; set => Core.OriginalTrainerGender = value; }
    public byte HyperTrainFlags { get => Core.HyperTrainFlags; set => Core.HyperTrainFlags = value; }
    public bool HT_HP { get => Core.HT_HP; set => Core.HT_HP = value; }
    public bool HT_ATK { get => Core.HT_ATK; set => Core.HT_ATK = value; }
    public bool HT_DEF { get => Core.HT_DEF; set => Core.HT_DEF = value; }
    public bool HT_SPA { get => Core.HT_SPA; set => Core.HT_SPA = value; }
    public bool HT_SPD { get => Core.HT_SPD; set => Core.HT_SPD = value; }
    public bool HT_SPE { get => Core.HT_SPE; set => Core.HT_SPE = value; }
    public override int HeldItem { get => Core.HeldItem; set => Core.HeldItem = value; }

    public override string Nickname { get => Core.Nickname; set => Core.Nickname = value; }
    public override string OriginalTrainerName { get => Core.OriginalTrainerName; set => Core.OriginalTrainerName = value; }
    public override string HandlingTrainerName { get => Core.HandlingTrainerName; set => Core.HandlingTrainerName = value; }

    public int MarkingCount => Core.MarkingCount;
    public int RibbonCount => Core.RibbonCount;
    public int MarkCount => Core.MarkCount;
    public int RibbonMarkCount => Core.RibbonMarkCount;
    public MarkingColor GetMarking(int index) => Core.GetMarking(index);
    public void SetMarking(int index, MarkingColor value) => Core.SetMarking(index, value);

    public MarkingColor MarkingCircle   { get => GetMarking(0); set => SetMarking(0, value); }
    public MarkingColor MarkingTriangle { get => GetMarking(1); set => SetMarking(1, value); }
    public MarkingColor MarkingSquare   { get => GetMarking(2); set => SetMarking(2, value); }
    public MarkingColor MarkingHeart    { get => GetMarking(3); set => SetMarking(3, value); }
    public MarkingColor MarkingStar     { get => GetMarking(4); set => SetMarking(4, value); }
    public MarkingColor MarkingDiamond  { get => GetMarking(5); set => SetMarking(5, value); }

    #endregion

    // Used to be in Core, now we just don't bother.
    public override int PokerusDays { get => 0; set { } }
    public override int PokerusStrain { get => 0; set { } }
    public override int Ability { get => 0; set { } }
    public override int AbilityNumber { get => 0; set { } }

    #region Calculated

    public override byte CurrentFriendship { get => CurrentHandler == 0 ? OriginalTrainerFriendship : HandlingTrainerFriendship; set { if (CurrentHandler == 0) OriginalTrainerFriendship = value; else HandlingTrainerFriendship = value; } }

    public override uint PSV => ((PID >> 16) ^ (PID & 0xFFFF)) >> 4;
    public override uint TSV => (uint)(TID16 ^ SID16) >> 4;
    public override int Characteristic => EntityCharacteristic.GetCharacteristic(EncryptionConstant, [IV_HP, IV_ATK, IV_DEF, IV_SPE, IV_SPA, IV_SPD]);

    #endregion

    #region Children

    public override ushort Move1       { get => LatestGameData.Move1      ; set => LatestGameData.Move1       = value; }
    public override ushort Move2       { get => LatestGameData.Move2      ; set => LatestGameData.Move2       = value; }
    public override ushort Move3       { get => LatestGameData.Move3      ; set => LatestGameData.Move3       = value; }
    public override ushort Move4       { get => LatestGameData.Move4      ; set => LatestGameData.Move4       = value; }
    public override int Move1_PP    { get => LatestGameData.Move1_PP   ; set => LatestGameData.Move1_PP    = value; }
    public override int Move2_PP    { get => LatestGameData.Move2_PP   ; set => LatestGameData.Move2_PP    = value; }
    public override int Move3_PP    { get => LatestGameData.Move3_PP   ; set => LatestGameData.Move3_PP    = value; }
    public override int Move4_PP    { get => LatestGameData.Move4_PP   ; set => LatestGameData.Move4_PP    = value; }
    public override int Move1_PPUps { get => LatestGameData.Move1_PPUps; set => LatestGameData.Move1_PPUps = value; }
    public override int Move2_PPUps { get => LatestGameData.Move2_PPUps; set => LatestGameData.Move2_PPUps = value; }
    public override int Move3_PPUps { get => LatestGameData.Move3_PPUps; set => LatestGameData.Move3_PPUps = value; }
    public override int Move4_PPUps { get => LatestGameData.Move4_PPUps; set => LatestGameData.Move4_PPUps = value; }

    public override byte Ball         { get => LatestGameData.Ball;         set => LatestGameData.Ball = value; }
    public override ushort MetLocation { get => LatestGameData.MetLocation; set => LatestGameData.MetLocation = value; }
    public override ushort EggLocation { get => LatestGameData.EggLocation; set => LatestGameData.EggLocation = value; }

    #endregion

    public override byte Stat_Level  { get => CurrentLevel; set => CurrentLevel = value; }
    public override int Stat_HPMax  { get => 0; set { } }
    public override int Stat_ATK    { get => 0; set { } }
    public override int Stat_DEF    { get => 0; set { } }
    public override int Stat_SPE    { get => 0; set { } }
    public override int Stat_SPA    { get => 0; set { } }
    public override int Stat_SPD    { get => 0; set { } }

    #region Maximums

    public override int MaxIV => 31;
    public override int MaxEV => EffortValues.Max252;
    public override int MaxStringLengthTrainer => 12;
    public override int MaxStringLengthNickname => 12;
    public override ushort MaxMoveID => Legal.MaxMoveID_8a;
    public override ushort MaxSpeciesID => Legal.MaxSpeciesID_8a;
    public override int MaxAbilityID => Legal.MaxAbilityID_8a;
    public override int MaxItemID => Legal.MaxItemID_8a;
    public override int MaxBallID => Legal.MaxBallID_8a;
    public override GameVersion MaxGameID => Legal.MaxGameID_HOME;

    #endregion

    public override int SIZE_PARTY => HomeCrypto.SIZE_3STORED;
    public override int SIZE_STORED => HomeCrypto.SIZE_3STORED;
    public override bool Valid { get => true; set { } }
    public override PersonalInfo PersonalInfo => LatestGameData.GetPersonalInfo(Species, Form);
    public override void RefreshChecksum() => Checksum = 0;
    public override bool ChecksumValid => true;

    protected override byte[] Encrypt()
    {
        var result = Rebuild();
        return HomeCrypto.Encrypt(result);
    }

    public byte[] Rebuild()
    {
        var length = WriteLength;

        // Handle PKCS7 manually
        var remainder = length & 0xF;
        if (remainder != 0) // pad to nearest 0x10, fill remainder bytes with value.
            remainder = 0x10 - remainder;
        var result = new byte[length + remainder];
        var span = result.AsSpan(0, length);
        result.AsSpan(length).Fill((byte)remainder);

        // Header and Core are already in the current byte array.
        // Write each part, starting with header and core.
        int ctr = HomeCrypto.SIZE_1HEADER + 2;
        ctr += Core.WriteTo(span[ctr..]);
        var gameDataLengthSpan = span[ctr..];
        int gameDataStart = (ctr += 2);
        if (DataPK8 is { } pk8) ctr += pk8.WriteTo(span[ctr..]);
        if (DataPB7 is { } pb7) ctr += pb7.WriteTo(span[ctr..]);
        if (DataPA8 is { } pa8) ctr += pa8.WriteTo(span[ctr..]);
        if (DataPB8 is { } pb8) ctr += pb8.WriteTo(span[ctr..]);
        if (DataPK9 is { } pk9) ctr += pk9.WriteTo(span[ctr..]);
        WriteUInt16LittleEndian(gameDataLengthSpan, GameDataSize = (ushort)(ctr - gameDataStart));

        // Update metadata to ensure we're a valid object.
        DataVersion = HomeCrypto.VersionLatest;
        EncodedDataSize = (ushort)(result.Length - HomeCrypto.SIZE_1HEADER);
        CoreDataSize = (ushort)Core.SerializedSize;
        Data.AsSpan(0, HomeCrypto.SIZE_1HEADER + 2).CopyTo(span); // Copy updated header & CoreData length.

        return result;
    }

    private int WriteLength
    {
        get
        {
            var length = GameDataStart;
            if (DataPK8 is {} k8) length += k8.SerializedSize;
            if (DataPB7 is {} b7) length += b7.SerializedSize;
            if (DataPA8 is {} a8) length += a8.SerializedSize;
            if (DataPB8 is {} b8) length += b8.SerializedSize;
            if (DataPK9 is {} k9) length += k9.SerializedSize;
            return length;
        }
    }

    public override PKH Clone() => new((byte[])Data.Clone())
    {
        DataPK9 = DataPK9?.Clone(),
        DataPK8 = DataPK8?.Clone(),
        DataPA8 = DataPA8?.Clone(),
        DataPB8 = DataPB8?.Clone(),
        DataPB7 = DataPB7?.Clone(),
    };

    public IGameDataSide LatestGameData => OriginalGameData() ?? GetFallbackGameData();

    private IGameDataSide GetFallbackGameData() => DataPB7
                                                ?? DataPK9
                                                ?? DataPB8
                                                ?? DataPA8
                                                ?? DataPK8 ?? CreateFallback();

    private IGameDataSide CreateFallback() => Version switch
    {
        GP or GE => DataPB7 ??= new(),
        BD or SP => DataPB8 ??= new(),
        PLA      => DataPA8 ??= new(),
        SL or VL => DataPK9 ??= new(),
        _                  => DataPK8 ??= new(),
    };

    private IGameDataSide? OriginalGameData() => Version switch
    {
        GameVersion.GO when DataPB7 is not null => DataPB7,
        GP or GE => DataPB7,
        BD or SP => DataPB8,
        PLA      => DataPA8,
        SL or VL => DataPK9,

        // SW/SH can be confused with others if we didn't seed with the original transfer data.
        SW or SH => DataPK8 switch
        {
            { MetLocation: LocationsHOME.SWLA } => DataPA8,
            { MetLocation: LocationsHOME.SWBD or LocationsHOME.SHSP } => DataPB8,
            { MetLocation: LocationsHOME.SWSL or LocationsHOME.SHVL } => DataPK9,
            _ => DataPK8,
        },

        _ => DataPK8, // Gen7 and below.
    };

    public PB7? ConvertToPB7() => DataPB7 is { } x ? x.ConvertToPKM(this) : (DataPB7 ??= GameDataPB7.TryCreate(this))?.ConvertToPKM(this);
    public PK8? ConvertToPK8() => DataPK8 is { } x ? x.ConvertToPKM(this) : (DataPK8 ??= GameDataPK8.TryCreate(this))?.ConvertToPKM(this);
    public PB8? ConvertToPB8() => DataPB8 is { } x ? x.ConvertToPKM(this) : (DataPB8 ??= GameDataPB8.TryCreate(this))?.ConvertToPKM(this);
    public PA8? ConvertToPA8() => DataPA8 is { } x ? x.ConvertToPKM(this) : (DataPA8 ??= GameDataPA8.TryCreate(this))?.ConvertToPKM(this);
    public PK9? ConvertToPK9() => DataPK9 is { } x ? x.ConvertToPKM(this) : (DataPK9 ??= GameDataPK9.TryCreate(this))?.ConvertToPKM(this);

    public void CopyTo(PKM pk) => Core.CopyTo(pk);

    public static HomeGameDataFormat GetType(Type type)
    {
        if (type == typeof(PB7)) return HomeGameDataFormat.PB7;
        if (type == typeof(PK8)) return HomeGameDataFormat.PK8;
        if (type == typeof(PB8)) return HomeGameDataFormat.PB8;
        if (type == typeof(PA8)) return HomeGameDataFormat.PA8;
        if (type == typeof(PK9)) return HomeGameDataFormat.PK9;
        return HomeGameDataFormat.None;
    }

    public PKM? ConvertToPKM(HomeGameDataFormat type) => type switch
    {
        HomeGameDataFormat.PB7 => ConvertToPB7(),
        HomeGameDataFormat.PK8 => ConvertToPK8(),
        HomeGameDataFormat.PB8 => ConvertToPB8(),
        HomeGameDataFormat.PA8 => ConvertToPA8(),
        HomeGameDataFormat.PK9 => ConvertToPK9(),
        _ => null,
    };

    public static PKH ConvertFromPKM(PKM pk)
    {
        var blank = new PKH();
        blank.CopyFrom(pk);
        blank.EnsureScaleSizeExists();
        if (blank.Species is (int)PKHeX.Core.Species.Arceus or (int)PKHeX.Core.Species.Silvally)
            blank.Form = 0;
        return blank;
    }

    public void CopyFrom(PKM pk)
    {
        Core.CopyFrom(pk);
             if (pk is PB7 pb7) (DataPB7 ??= new GameDataPB7()).CopyFrom(pb7, this);
        else if (pk is PK7 pk7) (DataPK8 ??= new GameDataPK8()).CopyFrom(pk7, this);
        else if (pk is PK8 pk8) (DataPK8 ??= new GameDataPK8()).CopyFrom(pk8, this);
        else if (pk is PB8 pb8) (DataPB8 ??= new GameDataPB8()).CopyFrom(pb8, this);
        else if (pk is PA8 pa8) (DataPA8 ??= new GameDataPA8()).CopyFrom(pa8, this);
        else if (pk is PK9 pk9) (DataPK9 ??= new GameDataPK9()).CopyFrom(pk9, this);
    }

    private IGameDataSide? FirstScaleData => DataPK9 ?? DataPA8 as IGameDataSide;

    private void EnsureScaleSizeExists()
    {
        if (Core.RibbonMarkAlpha)
        {
            // Fix for PLA static encounter Alphas with 127 scale.
            Core.HeightScalar = Core.WeightScalar = 255;
            if (DataPA8 is { Scale: not 255 } pa8)
                pa8.Scale = 255;
            if (DataPK9 is { Scale: not 255 } pk9)
                pk9.Scale = 255;
            return;
        }
        if (GO_HOME || FirstScaleData is IScaledSize3)
            return; // data exists for scale, keep values.
        while (HeightScalar == 0 && WeightScalar == 0)
        {
            var rnd = Util.Rand;
            HeightScalar = PokeSizeUtil.GetRandomScalar(rnd);
            WeightScalar = PokeSizeUtil.GetRandomScalar(rnd);
        }
    }

    public override string GetString(ReadOnlySpan<byte> data)
        => StringConverter8.GetString(data);
    public override int LoadString(ReadOnlySpan<byte> data, Span<char> destBuffer)
        => StringConverter8.LoadString(data, destBuffer);
    public override int SetString(Span<byte> destBuffer, ReadOnlySpan<char> value, int maxLength, StringConverterOption option)
        => StringConverter8.SetString(destBuffer, value, maxLength, option);
    public override int GetStringTerminatorIndex(ReadOnlySpan<byte> data)
        => TrashBytesUTF16.GetTerminatorIndex(data);
    public override int GetStringLength(ReadOnlySpan<byte> data)
        => TrashBytesUTF16.GetStringLength(data);
    public override int GetBytesPerChar() => 2;

    /// <summary>
    /// Revises the Handler details of a <see cref="PKM"/> to match the current <see cref="ITrainerInfo"/>.
    /// </summary>
    /// <remarks>Logic used starting in SW/SH games.</remarks>
    public static void UpdateHandler<T>(T pk, ITrainerInfo tr)
        where T : PKM, IHandlerLanguage, IMemoryHT
    {
        pk.CurrentHandler = 1;

        var gender = tr.Gender;
        var other = tr.OT;
        if (IsHandlerSame(pk, other, gender))
            return;

        // Clear the old Handler details and write our new ones.
        pk.ClearMemoriesHT();
        pk.HandlingTrainerName = other;
        pk.HandlingTrainerLanguage = (byte)tr.Language;
        pk.HandlingTrainerGender = gender;
        pk.HandlingTrainerFriendship = pk.PersonalInfo.BaseFriendship;
        // Memories are deferred to the game. SW/SH does not immediately set memories.
    }

    private static bool IsHandlerSame<T>(T pk, ReadOnlySpan<char> newHT, byte newGender)
        where T : PKM, IHandlerLanguage
    {
        if (newGender != pk.HandlingTrainerGender)
            return false;

        // Does not check Language or Version for equality! Can result in mismatches or empty (0) language value.
        // Check the trainer string for equality (most expensive of all last).

        Span<char> exist = stackalloc char[pk.TrashCharCountTrainer];
        var len = pk.LoadString(pk.HandlingTrainerTrash, exist);
        exist = exist[..len];

        return exist.SequenceEqual(newHT);
    }
}
