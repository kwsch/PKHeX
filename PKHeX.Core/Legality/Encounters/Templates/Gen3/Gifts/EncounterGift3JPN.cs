using System;

namespace PKHeX.Core;

/// <summary>
/// Generation 3 Event Gift
/// </summary>
/// <remarks>Specialized for the PCJP gift distribution machines.</remarks>
public sealed class EncounterGift3JPN(ushort Species, Distribution3JPN Distribution)
    : IEncounterable, IEncounterMatch, IRandomCorrelation, IFixedTrainer
{
    public ushort Species { get; } = Species;
    public Distribution3JPN Distribution { get; } = Distribution;
    public const byte Level = 10;

    private const PIDType Method = PIDType.BACD_U_AX;
    public PIDType GetSuggestedCorrelation() => Method;
    public byte Form => 0;
    public GameVersion Version => GameVersion.R;
    public byte Generation => 3;
    public bool IsShiny => false;
    public byte LevelMin => Level;
    public byte LevelMax => Level;
    public ushort Location => 255;
    public ushort EggLocation => 0;
    public AbilityPermission Ability => AbilityPermission.Any12;
    public Ball FixedBall => Ball.Poke;
    public Shiny Shiny => Shiny.Never;
    public EntityContext Context => EntityContext.Gen3;
    public bool IsEgg => false;
    public bool IsFixedTrainer => true;
    public string Name => "PCNY Gift";
    public string LongName => Name;

    public bool IsCompatible(PIDType val, PKM pk) => val is Method;
    public EncounterMatchRating GetMatchRating(PKM pk) => EncounterMatchRating.Match; // checked in explicit match
    public bool IsTrainerMatch(PKM pk, ReadOnlySpan<char> trainer, int language) => true; // checked in explicit match

    #region Generating
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);

    public PK3 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK3 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        var pi = PersonalTable.RS[Species];
        var pivot = Util.Rand.Next(0, 65535);
        PK3 pk = new()
        {
            Species = Species,
            MetLevel = Level,
            MetLocation = Location,
            Ball = 4,
            Version = Version,
            EXP = Experience.GetEXP(Level, pi.EXPGrowth),
            OriginalTrainerGender = tr.Gender,
            Language = (int)LanguageID.Japanese,
            ID32 = Distribution.GetTrainerID(),
            OriginalTrainerName = Distribution.GetTrainerName((ushort)pivot),
            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, (int)LanguageID.Japanese, 3),
            OriginalTrainerFriendship = pi.BaseFriendship,
        };

        // Generate PIDIV
        SetPINGA(pk, criteria);
        EncounterUtil.SetEncounterMoves(pk, Version, LevelMin);
        pk.RefreshChecksum();
        return pk;
    }

    private static void SetPINGA(PK3 pk, EncounterCriteria _)
    {
        var seed = Util.Rand32();
        PIDGenerator.SetValuesFromSeed(pk, Method, seed);
        pk.RefreshAbility((int)(pk.EncryptionConstant & 1));
    }
    #endregion

    private bool IsValidTrainerID(ushort value) => value == Distribution.GetTrainerID();

    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        // Gen3 Version MUST match.
        if (Version != 0 && !Version.Contains(pk.Version))
            return false;

        if (pk.SID16 != 0) return false;
        if (!IsValidTrainerID(pk.TID16)) return false;

        Span<char> trainerName = stackalloc char[pk.TrashCharCountTrainer];
        int len = pk.LoadString(pk.OriginalTrainerTrash, trainerName);
        if (len <= 2)
            return false;

        if (!Distribution.IsTrainerNameValid(trainerName[..len]))
            return false;

        if (Form != evo.Form && !FormInfo.IsFormChangeable(Species, Form, pk.Form, Context, pk.Context))
            return false;

        if (pk.Language == (int)LanguageID.Japanese)
            return false;

        if ((byte)FixedBall != pk.Ball)
            return false;

        if (pk.Format == 3)
        {
            if (Level != pk.MetLevel)
                return false;
            if (Location != pk.MetLocation)
                return false;
        }
        else
        {
            if (pk.IsEgg)
                return false;
            if (Level > pk.MetLevel)
                return false;
            if (pk.EggLocation != LocationEdits.GetNoneLocation(pk.Context))
                return false;
        }
        return true;
    }
}
