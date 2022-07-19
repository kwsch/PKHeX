using System;

namespace PKHeX.Core;

/// <summary>
/// Generation 3 Base <see cref="PKM"/> Class
/// </summary>
public abstract class G3PKM : PKM, IRibbonSetEvent3, IRibbonSetCommon3, IRibbonSetUnique3, IRibbonSetOnly3, IContestStats, IContestStatsMutable
{
    protected G3PKM(byte[] data) : base(data) { }
    protected G3PKM(int size) : base(size) { }

    // Maximums
    public sealed override int MaxMoveID => Legal.MaxMoveID_3;
    public sealed override int MaxSpeciesID => Legal.MaxSpeciesID_3;
    public sealed override int MaxAbilityID => Legal.MaxAbilityID_3;
    public sealed override int MaxItemID => Legal.MaxItemID_3;
    public sealed override int MaxBallID => Legal.MaxBallID_3;
    public sealed override int MaxGameID => Legal.MaxGameID_3;
    public sealed override int MaxIV => 31;
    public sealed override int MaxEV => 255;
    public sealed override int OTLength => 7;
    public sealed override int NickLength => 10;

    // Generated Attributes
    public sealed override int PSV => (int)(((PID >> 16) ^ (PID & 0xFFFF)) >> 3);
    public sealed override int TSV => (TID ^ SID) >> 3;
    public sealed override bool Japanese => Language == (int)LanguageID.Japanese;

    public sealed override int Ability { get => ((PersonalInfo3)PersonalInfo).GetAbility(AbilityBit); set { } }
    public sealed override uint EncryptionConstant { get => PID; set { } }
    public sealed override int Nature { get => (int)(PID % 25); set { } }
    public sealed override bool IsNicknamed { get => SpeciesName.IsNicknamed(Species, Nickname, Language, 3); set { } }
    public sealed override int Gender { get => EntityGender.GetFromPID(Species, PID); set { } }
    public sealed override int Characteristic => -1;
    public sealed override int CurrentFriendship { get => OT_Friendship; set => OT_Friendship = value; }
    public sealed override int CurrentHandler { get => 0; set { } }
    public sealed override int Egg_Location { get => 0; set { } }
    public override int MarkingCount => 4;

    public override int GetMarking(int index)
    {
        if ((uint)index >= MarkingCount)
            throw new ArgumentOutOfRangeException(nameof(index));
        return (MarkValue >> index) & 1;
    }

    public override void SetMarking(int index, int value)
    {
        if ((uint)index >= MarkingCount)
            throw new ArgumentOutOfRangeException(nameof(index));
        MarkValue = (MarkValue & ~(1 << index)) | ((value & 1) << index);
    }

    public abstract ushort SpeciesID3 { get; set; } // raw access

    public sealed override int Form
    {
        get => Species == (int)Core.Species.Unown ? EntityPID.GetUnownForm3(PID) : 0;
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
        AbilityBit = n == 1 && ((PersonalInfo3)PersonalInfo).HasSecondAbility;
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
    public abstract int RibbonCountG3Cool { get; set; }
    public abstract int RibbonCountG3Beauty { get; set; }
    public abstract int RibbonCountG3Cute { get; set; }
    public abstract int RibbonCountG3Smart { get; set; }
    public abstract int RibbonCountG3Tough { get; set; }
    public abstract bool RibbonWorld { get; set; }
    public abstract bool Unused1 { get; set; }
    public abstract bool Unused2 { get; set; }
    public abstract bool Unused3 { get; set; }
    public abstract bool Unused4 { get; set; }

    public abstract byte CNT_Cool   { get; set; }
    public abstract byte CNT_Beauty { get; set; }
    public abstract byte CNT_Cute   { get; set; }
    public abstract byte CNT_Smart  { get; set; }
    public abstract byte CNT_Tough  { get; set; }
    public abstract byte CNT_Sheen  { get; set; }

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

    protected static byte GetGBAVersionID(byte gc) => (byte)((GCVersion)gc).GetG3VersionID();
    protected static byte GetGCVersionID(int gba) => (byte)((GameVersion)gba).GetCXDVersionID();

    /// <summary>
    /// Interconversion for Generation 3 <see cref="PKM"/> formats.
    /// </summary>
    /// <typeparam name="T">Generation 3 format to convert to</typeparam>
    /// <returns>New object with transferred properties.</returns>
    protected T ConvertTo<T>() where T : G3PKM, new() => new()
    {
        SpeciesID3 = SpeciesID3,
        Language = Language,
        PID = PID,
        TID = TID,
        SID = SID,
        EXP = EXP,
        HeldItem = HeldItem,
        AbilityBit = AbilityBit,
        IsEgg = IsEgg,
        FatefulEncounter = FatefulEncounter,

        Met_Location = Met_Location,
        Met_Level = Met_Level,
        Version = Version,
        Ball = Ball,

        Nickname = Nickname,
        OT_Name = OT_Name,
        OT_Gender = OT_Gender,
        OT_Friendship = OT_Friendship,

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
        CNT_Cool = CNT_Cool,
        CNT_Beauty = CNT_Beauty,
        CNT_Cute = CNT_Cute,
        CNT_Smart = CNT_Smart,
        CNT_Tough = CNT_Tough,
        CNT_Sheen = CNT_Sheen,

        PKRS_Days = PKRS_Days,
        PKRS_Strain = PKRS_Strain,

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
