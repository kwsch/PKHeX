using System;
using System.Diagnostics.CodeAnalysis;

namespace PKHeX.Core;

/// <summary>
/// Generation 3 Base <see cref="PKM"/> Class
/// </summary>
public abstract class G3PKM : PKM, IRibbonSetEvent3, IRibbonSetCommon3, IRibbonSetUnique3, IRibbonSetOnly3, IRibbonSetRibbons, IContestStats, IAppliedMarkings3
{
    protected G3PKM(byte[] data) : base(data) { }
    protected G3PKM([ConstantExpected] int size) : base(size) { }

    public abstract override PersonalInfo3 PersonalInfo { get; }

    // Maximums
    public sealed override ushort MaxMoveID => Legal.MaxMoveID_3;
    public sealed override ushort MaxSpeciesID => Legal.MaxSpeciesID_3;
    public sealed override int MaxAbilityID => Legal.MaxAbilityID_3;
    public sealed override int MaxItemID => Legal.MaxItemID_3;
    public sealed override int MaxBallID => Legal.MaxBallID_3;
    public sealed override GameVersion MaxGameID => Legal.MaxGameID_3;
    public sealed override int MaxIV => 31;
    public sealed override int MaxEV => EffortValues.Max255;
    public sealed override int MaxStringLengthTrainer => 7;
    public sealed override int MaxStringLengthNickname => 10;

    // Generated Attributes
    public sealed override uint PSV => ((PID >> 16) ^ (PID & 0xFFFF)) >> 3;
    public sealed override uint TSV => (uint)(TID16 ^ SID16) >> 3;
    public sealed override bool Japanese => Language == (int)LanguageID.Japanese;

    public sealed override int Ability { get => PersonalInfo.GetAbility(AbilityBit); set { } }
    public sealed override uint EncryptionConstant { get => PID; set { } }
    public sealed override Nature Nature { get => (Nature)(PID % 25); set { } }
    public sealed override bool IsNicknamed { get => SpeciesName.IsNicknamed(Species, Nickname, Language, 3); set { } }
    public sealed override byte Gender { get => EntityGender.GetFromPID(Species, PID); set { } }
    public sealed override int Characteristic => -1;
    public sealed override byte CurrentFriendship { get => OriginalTrainerFriendship; set => OriginalTrainerFriendship = value; }
    public sealed override byte CurrentHandler { get => 0; set { } }
    public sealed override ushort EggLocation { get => 0; set { } }
    public int MarkingCount => 4;
    public abstract byte MarkingValue { get; set; }

    public bool GetMarking(int index)
    {
        if ((uint)index >= MarkingCount)
            throw new ArgumentOutOfRangeException(nameof(index));
        return ((MarkingValue >> index) & 1) != 0;
    }

    public void SetMarking(int index, bool value)
    {
        if ((uint)index >= MarkingCount)
            throw new ArgumentOutOfRangeException(nameof(index));
        MarkingValue = (byte)((MarkingValue & ~(1 << index)) | ((value ? 1 : 0) << index));
    }

    public bool MarkingCircle   { get => GetMarking(0); set => SetMarking(0, value); }
    public bool MarkingTriangle { get => GetMarking(1); set => SetMarking(1, value); } // Purposefully reverse because we swap bits already and want to match Gen4+
    public bool MarkingSquare   { get => GetMarking(2); set => SetMarking(2, value); }
    public bool MarkingHeart    { get => GetMarking(3); set => SetMarking(3, value); }

    public abstract ushort SpeciesInternal { get; set; } // raw access

    public sealed override byte Form
    {
        get => Species == (int)Core.Species.Unown ? EntityPID.GetUnownForm3(PID) : (byte)0;
        set
        {
            if (Species != (int)Core.Species.Unown)
                return;
            var rnd = Util.Rand;
            while (EntityPID.GetUnownForm3(PID) != value)
                PID = rnd.Rand32();
        }
    }

    public sealed override int AbilityNumber { get => 1 << (AbilityBit ? 1 : 0); set => AbilityBit = value > 1; } // [0,1]->[1,2] ; [1,x]->[0,1]
    public abstract bool AbilityBit { get; set; }

    public sealed override void RefreshAbility(int n)
    {
        if (Species is (int)Core.Species.Granbull or (int)Core.Species.Vibrava or (int)Core.Species.Flygon)
            return;
        AbilityBit = n == 1 && PersonalInfo.HasSecondAbility;
    }

    public override bool Valid { get => true; set { } }
    public override void RefreshChecksum() { }
    public override bool ChecksumValid => Valid;

    public abstract bool RibbonEarth { get; set; }
    public abstract bool RibbonNational { get; set; }
    public abstract bool RibbonCountry { get; set; }
    public abstract bool RibbonChampionBattle { get; set; }
    public abstract bool RibbonChampionRegional { get; set; }
    public abstract bool RibbonChampionNational { get; set; }
    public abstract bool RibbonChampionG3 { get; set; }
    public abstract bool RibbonArtist { get; set; }
    public abstract bool RibbonEffort { get; set; }
    public abstract bool RibbonWinning { get; set; }
    public abstract bool RibbonVictory { get; set; }
    public abstract byte RibbonCountG3Cool { get; set; }
    public abstract byte RibbonCountG3Beauty { get; set; }
    public abstract byte RibbonCountG3Cute { get; set; }
    public abstract byte RibbonCountG3Smart { get; set; }
    public abstract byte RibbonCountG3Tough { get; set; }
    public abstract bool RibbonWorld { get; set; }
    public abstract bool Unused1 { get; set; }
    public abstract bool Unused2 { get; set; }
    public abstract bool Unused3 { get; set; }
    public abstract bool Unused4 { get; set; }
    public abstract int RibbonCount { get; }

    public abstract byte ContestCool   { get; set; }
    public abstract byte ContestBeauty { get; set; }
    public abstract byte ContestCute   { get; set; }
    public abstract byte ContestSmart  { get; set; }
    public abstract byte ContestTough  { get; set; }
    public abstract byte ContestSheen  { get; set; }

    /// <summary>
    /// Swaps bits at a given position
    /// </summary>
    /// <param name="value">Value to swap bits for</param>
    /// <param name="p1">Position of first bit to be swapped</param>
    /// <param name="p2">Position of second bit to be swapped</param>
    /// <remarks>Generation 3 marking values are swapped (Square-Triangle, instead of Triangle-Square).</remarks>
    /// <returns>Swapped bits value</returns>
    protected static int SwapBits(int value, int p1, int p2)
    {
        int bit1 = (value >> p1) & 1;
        int bit2 = (value >> p2) & 1;
        int x = bit1 ^ bit2;
        x = (x << p1) | (x << p2);
        return value ^ x;
    }

    protected static GameVersion GetGBAVersionID(GCVersion gc) => gc.GetG3VersionID();
    protected static GCVersion GetGCVersionID(GameVersion gba) => gba.GetCXDVersionID();

    /// <summary>
    /// Interconversion for Generation 3 <see cref="PKM"/> formats.
    /// </summary>
    /// <typeparam name="T">Generation 3 format to convert to</typeparam>
    /// <returns>New object with transferred properties.</returns>
    protected T ConvertTo<T>() where T : G3PKM, new() => new()
    {
        SpeciesInternal = SpeciesInternal,
        Language = Language,
        PID = PID,
        TID16 = TID16,
        SID16 = SID16,
        EXP = EXP,
        HeldItem = HeldItem,
        AbilityBit = AbilityBit,
        IsEgg = IsEgg,
        FatefulEncounter = FatefulEncounter,

        MetLocation = MetLocation,
        MetLevel = MetLevel,
        Version = Version,
        Ball = Ball,

        // Handle string conversion in derived classes
        //Nickname = Nickname,
        //OriginalTrainerName = OriginalTrainerName,
        OriginalTrainerGender = OriginalTrainerGender,
        OriginalTrainerFriendship = OriginalTrainerFriendship,

        Move1_PPUps = Move1_PPUps,
        Move2_PPUps = Move2_PPUps,
        Move3_PPUps = Move3_PPUps,
        Move4_PPUps = Move4_PPUps,
        Move1 = Move1,
        Move2 = Move2,
        Move3 = Move3,
        Move4 = Move4,
        Move1_PP = Move1_PP,
        Move2_PP = Move2_PP,
        Move3_PP = Move3_PP,
        Move4_PP = Move4_PP,

        IV_HP = IV_HP,
        IV_ATK = IV_ATK,
        IV_DEF = IV_DEF,
        IV_SPE = IV_SPE,
        IV_SPA = IV_SPA,
        IV_SPD = IV_SPD,
        EV_HP = EV_HP,
        EV_ATK = EV_ATK,
        EV_DEF = EV_DEF,
        EV_SPE = EV_SPE,
        EV_SPA = EV_SPA,
        EV_SPD = EV_SPD,
        ContestCool = ContestCool,
        ContestBeauty = ContestBeauty,
        ContestCute = ContestCute,
        ContestSmart = ContestSmart,
        ContestTough = ContestTough,
        ContestSheen = ContestSheen,

        PokerusDays = PokerusDays,
        PokerusStrain = PokerusStrain,

        // Transfer Ribbons
        RibbonCountG3Cool = RibbonCountG3Cool,
        RibbonCountG3Beauty = RibbonCountG3Beauty,
        RibbonCountG3Cute = RibbonCountG3Cute,
        RibbonCountG3Smart = RibbonCountG3Smart,
        RibbonCountG3Tough = RibbonCountG3Tough,
        RibbonChampionG3 = RibbonChampionG3,
        RibbonWinning = RibbonWinning,
        RibbonVictory = RibbonVictory,
        RibbonArtist = RibbonArtist,
        RibbonEffort = RibbonEffort,
        RibbonChampionBattle = RibbonChampionBattle,
        RibbonChampionRegional = RibbonChampionRegional,
        RibbonChampionNational = RibbonChampionNational,
        RibbonCountry = RibbonCountry,
        RibbonNational = RibbonNational,
        RibbonEarth = RibbonEarth,
        RibbonWorld = RibbonWorld,
        Unused1 = Unused1,
        Unused2 = Unused2,
        Unused3 = Unused3,
        Unused4 = Unused4,
    };
}
