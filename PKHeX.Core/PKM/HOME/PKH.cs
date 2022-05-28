using System;
using static System.Buffers.Binary.BinaryPrimitives;
using static PKHeX.Core.GameVersion;
using static PKHeX.Core.Locations;

namespace PKHeX.Core;

/// <summary> Generation 8 <see cref="PKM"/> format. </summary>
public class PKH : PKM, IHandlerLanguage, IFormArgument, IHomeTrack, IBattleVersion, ITrainerMemories, IRibbonSetAffixed
{
    private readonly GameDataCore _coreData;
    private GameDataPB7? _gameDataPB7;
    private GameDataPK8? _gameDataPK8;
    private GameDataPA8? _gameDataPA8;
    private GameDataPB8? _gameDataPB8;

    public override EntityContext Context => EntityContext.Invalid;

    public PKH(byte[] data) : base(DecryptHome(data))
    {
        _coreData = new GameDataCore(Data, 0x10);

        var baseOfs = 0x14 + CoreDataSize;
        var gameOfs = 0;
        while (gameOfs < GameDataSize)
        {
            var fmt = (HomeGameDataFormat)Data[baseOfs + gameOfs];
            switch (fmt)
            {
                case HomeGameDataFormat.PB7: _gameDataPB7 = new GameDataPB7(Data, baseOfs + gameOfs); break;
                case HomeGameDataFormat.PK8: _gameDataPK8 = new GameDataPK8(Data, baseOfs + gameOfs); break;
                case HomeGameDataFormat.PA8: _gameDataPA8 = new GameDataPA8(Data, baseOfs + gameOfs); break;
                case HomeGameDataFormat.PB8: _gameDataPB8 = new GameDataPB8(Data, baseOfs + gameOfs); break;
                default: throw new ArgumentException($"Unknown GameData {fmt}");
            }

            gameOfs += 3 + ReadUInt16LittleEndian(Data.AsSpan(baseOfs + gameOfs + 1));
        }
    }

    private static byte[] DecryptHome(byte[] data)
    {
        HomeCrypto.DecryptIfEncrypted(ref data);
        Array.Resize(ref data, HomeCrypto.SIZE_1STORED);
        return data;
    }

    public ushort DataVersion     { get => ReadUInt16LittleEndian(Data.AsSpan(0x00)); set => WriteUInt16LittleEndian(Data.AsSpan(0x00), value); }
    public ulong EncryptionSeed   { get => ReadUInt64LittleEndian(Data.AsSpan(0x02)); set => WriteUInt64LittleEndian(Data.AsSpan(0x02), value); }
    public uint Checksum          { get => ReadUInt32LittleEndian(Data.AsSpan(0x0A)); set => WriteUInt32LittleEndian(Data.AsSpan(0x0A), value); }
    public ushort EncodedDataSize { get => ReadUInt16LittleEndian(Data.AsSpan(0x0E)); set => WriteUInt16LittleEndian(Data.AsSpan(0x0E), value); }
    public ushort CoreDataSize    { get => ReadUInt16LittleEndian(Data.AsSpan(0x10)); set => WriteUInt16LittleEndian(Data.AsSpan(0x10), value); }
    public ushort GameDataSize    { get => ReadUInt16LittleEndian(Data.AsSpan(0x12 + CoreDataSize)); set => WriteUInt16LittleEndian(Data.AsSpan(0x12 + CoreDataSize), value); }

    public override Span<byte> Nickname_Trash => _coreData.Nickname_Trash;
    public override Span<byte> OT_Trash => _coreData.OT_Trash;
    public override Span<byte> HT_Trash => _coreData.HT_Trash;

    #region Core

    public ulong Tracker { get => _coreData.Tracker; set => _coreData.Tracker = value; }
    public override uint EncryptionConstant { get => _coreData.EncryptionConstant; set => _coreData.EncryptionConstant = value; }
    public bool IsBadEgg { get => _coreData.IsBadEgg; set => _coreData.IsBadEgg = value; }
    public override int Species { get => _coreData.Species; set => _coreData.Species = value; }
    public override int TID { get => _coreData.TID; set => _coreData.TID = value; }
    public override int SID { get => _coreData.SID; set => _coreData.SID = value; }
    public override uint EXP { get => _coreData.EXP; set => _coreData.EXP = value; }
    public override int Ability { get => _coreData.Ability; set => _coreData.Ability = value; }
    public override int AbilityNumber { get => _coreData.AbilityNumber; set => _coreData.AbilityNumber = value; }
    public bool Favorite { get => _coreData.Favorite; set => _coreData.Favorite = value; }
    public override int MarkValue { get => _coreData.MarkValue; protected set => _coreData.MarkValue = value; }
    public override uint PID { get => _coreData.PID; set => _coreData.PID = value; }
    public override int Nature { get => _coreData.Nature; set => _coreData.Nature = value; }
    public override int StatNature { get => _coreData.StatNature; set => _coreData.StatNature = value; }
    public override bool FatefulEncounter { get => _coreData.FatefulEncounter; set => _coreData.FatefulEncounter = value; }
    public override int Gender { get => _coreData.Gender; set => _coreData.Gender = value; }
    public override int Form { get => _coreData.Form; set => _coreData.Form = value; }
    public override int EV_HP { get => _coreData.EV_HP; set => _coreData.EV_HP = value; }
    public override int EV_ATK { get => _coreData.EV_ATK; set => _coreData.EV_ATK = value; }
    public override int EV_DEF { get => _coreData.EV_DEF; set => _coreData.EV_DEF = value; }
    public override int EV_SPE { get => _coreData.EV_SPE; set => _coreData.EV_SPE = value; }
    public override int EV_SPA { get => _coreData.EV_SPA; set => _coreData.EV_SPA = value; }
    public override int EV_SPD { get => _coreData.EV_SPD; set => _coreData.EV_SPD = value; }
    public byte CNT_Cool { get => _coreData.CNT_Cool; set => _coreData.CNT_Cool = value; }
    public byte CNT_Beauty { get => _coreData.CNT_Beauty; set => _coreData.CNT_Beauty = value; }
    public byte CNT_Cute { get => _coreData.CNT_Cute; set => _coreData.CNT_Cute = value; }
    public byte CNT_Smart { get => _coreData.CNT_Smart; set => _coreData.CNT_Smart = value; }
    public byte CNT_Tough { get => _coreData.CNT_Tough; set => _coreData.CNT_Tough = value; }
    public byte CNT_Sheen { get => _coreData.CNT_Sheen; set => _coreData.CNT_Sheen = value; }
    public override int PKRS_Days { get => _coreData.PKRS_Days; set => _coreData.PKRS_Days = value; }
    public override int PKRS_Strain { get => _coreData.PKRS_Strain; set => _coreData.PKRS_Strain = value; }
    public byte HeightScalar { get => _coreData.HeightScalar; set => _coreData.HeightScalar = value; }
    public byte WeightScalar { get => _coreData.WeightScalar; set => _coreData.WeightScalar = value; }
    public override int Stat_HPCurrent { get => _coreData.Stat_HPCurrent; set => _coreData.Stat_HPCurrent = value; }
    public override int IV_HP { get => _coreData.IV_HP; set => _coreData.IV_HP = value; }
    public override int IV_ATK { get => _coreData.IV_ATK; set => _coreData.IV_ATK = value; }
    public override int IV_DEF { get => _coreData.IV_DEF; set => _coreData.IV_DEF = value; }
    public override int IV_SPE { get => _coreData.IV_SPE; set => _coreData.IV_SPE = value; }
    public override int IV_SPA { get => _coreData.IV_SPA; set => _coreData.IV_SPA = value; }
    public override int IV_SPD { get => _coreData.IV_SPD; set => _coreData.IV_SPD = value; }
    public override bool IsEgg { get => _coreData.IsEgg; set => _coreData.IsEgg = value; }
    public override bool IsNicknamed { get => _coreData.IsNicknamed; set => _coreData.IsNicknamed = value; }
    public override int Status_Condition { get => _coreData.Status_Condition; set => _coreData.Status_Condition = value; }
    public override int HT_Gender { get => _coreData.HT_Gender; set => _coreData.HT_Gender = value; }
    public byte HT_Language { get => _coreData.HT_Language; set => _coreData.HT_Language = value; }
    public override int CurrentHandler { get => _coreData.CurrentHandler; set => _coreData.CurrentHandler = value; }
    public int HT_TrainerID { get => _coreData.HT_TrainerID; set => _coreData.HT_TrainerID = value; }
    public override int HT_Friendship { get => _coreData.HT_Friendship; set => _coreData.HT_Friendship = value; }
    public byte HT_Intensity { get => _coreData.HT_Intensity; set => _coreData.HT_Intensity = value; }
    public byte HT_Memory { get => _coreData.HT_Memory; set => _coreData.HT_Memory = value; }
    public byte HT_Feeling { get => _coreData.HT_Feeling; set => _coreData.HT_Feeling = value; }
    public ushort HT_TextVar { get => _coreData.HT_TextVar; set => _coreData.HT_TextVar = value; }
    public override int Version { get => _coreData.Version; set => _coreData.Version = value; }
    public byte BattleVersion { get => _coreData.BattleVersion; set => _coreData.BattleVersion = value; }
    public override int Language { get => _coreData.Language; set => _coreData.Language = value; }
    public uint FormArgument { get => _coreData.FormArgument; set => _coreData.FormArgument = value; }
    public byte FormArgumentRemain { get => _coreData.FormArgumentRemain; set => _coreData.FormArgumentRemain = value; }
    public byte FormArgumentElapsed { get => _coreData.FormArgumentElapsed; set => _coreData.FormArgumentElapsed = value; }
    public byte FormArgumentMaximum { get => _coreData.FormArgumentMaximum; set => _coreData.FormArgumentMaximum = value; }
    public sbyte AffixedRibbon { get => _coreData.AffixedRibbon; set => _coreData.AffixedRibbon = value; }
    public override int OT_Friendship { get => _coreData.OT_Friendship; set => _coreData.OT_Friendship = value; }
    public byte OT_Intensity { get => _coreData.OT_Intensity; set => _coreData.OT_Intensity = value; }
    public byte OT_Memory { get => _coreData.OT_Memory; set => _coreData.OT_Memory = value; }
    public ushort OT_TextVar { get => _coreData.OT_TextVar; set => _coreData.OT_TextVar = value; }
    public byte OT_Feeling { get => _coreData.OT_Feeling; set => _coreData.OT_Feeling = value; }
    public override int Egg_Year { get => _coreData.Egg_Year; set => _coreData.Egg_Year = value; }
    public override int Egg_Month { get => _coreData.Egg_Month; set => _coreData.Egg_Month = value; }
    public override int Egg_Day { get => _coreData.Egg_Day; set => _coreData.Egg_Day = value; }
    public override int Met_Year { get => _coreData.Met_Year; set => _coreData.Met_Year = value; }
    public override int Met_Month { get => _coreData.Met_Month; set => _coreData.Met_Month = value; }
    public override int Met_Day { get => _coreData.Met_Day; set => _coreData.Met_Day = value; }
    public override int Met_Level { get => _coreData.Met_Level; set => _coreData.Met_Level = value; }
    public override int OT_Gender { get => _coreData.OT_Gender; set => _coreData.OT_Gender = value; }
    public byte HyperTrainFlags { get => _coreData.HyperTrainFlags; set => _coreData.HyperTrainFlags = value; }
    public bool HT_HP { get => _coreData.HT_HP; set => _coreData.HT_HP = value; }
    public bool HT_ATK { get => _coreData.HT_ATK; set => _coreData.HT_ATK = value; }
    public bool HT_DEF { get => _coreData.HT_DEF; set => _coreData.HT_DEF = value; }
    public bool HT_SPA { get => _coreData.HT_SPA; set => _coreData.HT_SPA = value; }
    public bool HT_SPD { get => _coreData.HT_SPD; set => _coreData.HT_SPD = value; }
    public bool HT_SPE { get => _coreData.HT_SPE; set => _coreData.HT_SPE = value; }
    public override int HeldItem { get => _coreData.HeldItem; set => _coreData.HeldItem = value; }

    public override string Nickname { get => _coreData.Nickname; set => _coreData.Nickname = value; }
    public override string OT_Name { get => _coreData.OT_Name; set => _coreData.OT_Name = value; }
    public override string HT_Name { get => _coreData.HT_Name; set => _coreData.HT_Name = value; }

    #endregion

    #region Calculated

    public override int CurrentFriendship { get => CurrentHandler == 0 ? OT_Friendship : HT_Friendship; set { if (CurrentHandler == 0) OT_Friendship = value; else HT_Friendship = value; } }

    public override int PSV => (int)((PID >> 16 ^ (PID & 0xFFFF)) >> 4);
    public override int TSV => (TID ^ SID) >> 4;

    public override int Characteristic
    {
        get
        {
            int pm6 = (int)(EncryptionConstant % 6);
            int maxIV = MaximumIV;
            int pm6stat = 0;
            for (int i = 0; i < 6; i++)
            {
                pm6stat = (pm6 + i) % 6;
                if (GetIV(pm6stat) == maxIV)
                    break;
            }
            return (pm6stat * 5) + (maxIV % 5);
        }
    }

    #endregion

    #region Children

    public override int Move1       { get => LatestGameData.Move1      ; set => LatestGameData.Move1       = value; }
    public override int Move2       { get => LatestGameData.Move2      ; set => LatestGameData.Move2       = value; }
    public override int Move3       { get => LatestGameData.Move3      ; set => LatestGameData.Move3       = value; }
    public override int Move4       { get => LatestGameData.Move4      ; set => LatestGameData.Move4       = value; }
    public override int Move1_PP    { get => LatestGameData.Move1_PP   ; set => LatestGameData.Move1_PP    = value; }
    public override int Move2_PP    { get => LatestGameData.Move2_PP   ; set => LatestGameData.Move2_PP    = value; }
    public override int Move3_PP    { get => LatestGameData.Move3_PP   ; set => LatestGameData.Move3_PP    = value; }
    public override int Move4_PP    { get => LatestGameData.Move4_PP   ; set => LatestGameData.Move4_PP    = value; }
    public override int Move1_PPUps { get => LatestGameData.Move1_PPUps; set => LatestGameData.Move1_PPUps = value; }
    public override int Move2_PPUps { get => LatestGameData.Move2_PPUps; set => LatestGameData.Move2_PPUps = value; }
    public override int Move3_PPUps { get => LatestGameData.Move3_PPUps; set => LatestGameData.Move3_PPUps = value; }
    public override int Move4_PPUps { get => LatestGameData.Move4_PPUps; set => LatestGameData.Move4_PPUps = value; }

    public override int Ball         { get => LatestGameData.Ball;         set => LatestGameData.Ball = value; }
    public override int Met_Location { get => LatestGameData.Met_Location; set => LatestGameData.Met_Location = value; }
    public override int Egg_Location { get => LatestGameData.Egg_Location; set => LatestGameData.Egg_Location = value; }

    #endregion

    public override int Stat_Level  { get => CurrentLevel; set => CurrentLevel = value; }
    public override int Stat_HPMax  { get => 0; set { } }
    public override int Stat_ATK    { get => 0; set { } }
    public override int Stat_DEF    { get => 0; set { } }
    public override int Stat_SPE    { get => 0; set { } }
    public override int Stat_SPA    { get => 0; set { } }
    public override int Stat_SPD    { get => 0; set { } }

    #region Maximums

    public override int MaxIV => 31;
    public override int MaxEV => 252;
    public override int OTLength => 12;
    public override int NickLength => 12;
    public override int MaxMoveID => Legal.MaxMoveID_8a;
    public override int MaxSpeciesID => Legal.MaxSpeciesID_8a;
    public override int MaxAbilityID => Legal.MaxAbilityID_8a;
    public override int MaxItemID => Legal.MaxItemID_8a;
    public override int MaxBallID => Legal.MaxBallID_8a;
    public override int MaxGameID => Legal.MaxGameID_8a;

    #endregion

    public override int SIZE_PARTY => HomeCrypto.SIZE_1STORED;
    public override int SIZE_STORED => HomeCrypto.SIZE_1STORED;
    public override bool Valid { get => true; set { } }
    public override PersonalInfo PersonalInfo => LatestGameData.GetPersonalInfo(Species, Form);
    protected override byte[] Encrypt() => throw new NotImplementedException();
    public override void RefreshChecksum() => throw new NotImplementedException();
    public override bool ChecksumValid => throw new NotImplementedException();

    public override PKM Clone() => new PKH((byte[])Data.Clone())
    {
        _gameDataPK8 = _gameDataPK8?.Clone(),
        _gameDataPA8 = _gameDataPA8?.Clone(),
        _gameDataPB8 = _gameDataPB8?.Clone(),
        _gameDataPB7 = _gameDataPB7?.Clone(),
    };

    public IGameDataSide LatestGameData => OriginalGameData() ?? GetFallbackGameData();

    private IGameDataSide GetFallbackGameData() => Version switch
    {
        (int)GP or (int)GE => _gameDataPB7 ??= new(),
        (int)BD or (int)SP => _gameDataPB8 ??= new(),
        (int)PLA           => _gameDataPA8 ??= new(),

        _                  => _gameDataPK8 ??= new(),
    };

    private IGameDataSide? OriginalGameData() => Version switch
    {
        (int)GP or (int)GE => _gameDataPB7,
        (int)BD or (int)SP => _gameDataPB8,
        (int)PLA           => _gameDataPA8,

        (int)SW or (int)SH when _gameDataPK8 is { Met_Location: HOME_SWLA }              => _gameDataPA8,
        (int)SW or (int)SH when _gameDataPK8 is { Met_Location: HOME_SWBD or HOME_SHSP } => _gameDataPB8,
        (int)SW or (int)SH                                                               => _gameDataPK8,

        _ => _gameDataPK8,
    };
}
