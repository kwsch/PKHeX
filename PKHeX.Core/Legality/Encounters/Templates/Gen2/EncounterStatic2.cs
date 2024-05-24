namespace PKHeX.Core;

/// <summary>
/// Generation 2 Static Encounter
/// </summary>
public sealed record EncounterStatic2(ushort Species, byte Level, GameVersion Version)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PK2>, IHatchCycle, IFixedGender, IMoveset, IFixedIVSet, IEncounterTime
{
    public byte Generation => 2;
    public EntityContext Context => EntityContext.Gen2;
    public byte Form => 0;
    public byte EggCycles => DizzyPunchEgg ? (byte)20 : (byte)0;
    public bool DizzyPunchEgg => IsEgg && Moves.HasMoves;

    public Ball FixedBall => Ball.Poke;
    ushort ILocation.Location => Location;
    public ushort EggLocation => 0;
    public bool IsShiny => Shiny == Shiny.Always;
    public AbilityPermission Ability => Species != (int)Core.Species.Koffing ? AbilityPermission.OnlyHidden : AbilityPermission.OnlyFirst;
    public bool Roaming => Species is (int)Core.Species.Entei or (int)Core.Species.Raikou or (int)Core.Species.Suicune && Location != 23;

    public Shiny Shiny { get; init; } = Shiny.Random;
    public byte Location { get; init; }
    public byte Gender { get; init; } = FixedGenderUtil.GenderRandom;
    public IndividualValueSet IVs { get; init; }
    public Moveset Moves { get; init; }
    public bool IsEgg { get; init; }

    public string Name => "Static Encounter";
    public string LongName => Name;
    public byte LevelMin => Level;
    public byte LevelMax => Level;

    #region Generating

    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    public PK2 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK2 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        var version = this.GetCompatibleVersion(tr.Version);
        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language, version);
        var pi = PersonalTable.C[Species];
        var pk = new PK2
        {
            Species = Species,
            CurrentLevel = LevelMin,

            TID16 = tr.TID16,
            OriginalTrainerName = tr.OT,

            OriginalTrainerFriendship = pi.BaseFriendship,

            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation),
        };

        if (IsEgg)
        {
            if (DizzyPunchEgg) // Fixed EXP value instead of exactly Level 5
                pk.EXP = 125;
        }
        else if (Version == GameVersion.C || (Version == GameVersion.GSC && tr.Version == GameVersion.C))
        {
            pk.OriginalTrainerGender = tr.Gender;
            pk.MetLevel = LevelMin;
            pk.MetLocation = Location;
            pk.MetTimeOfDay = GetRandomTime();
        }

        if (Moves.HasMoves)
            pk.SetMoves(Moves);
        else
            EncounterUtil.SetEncounterMoves(pk, version, LevelMin);

        if (IVs.IsSpecified)
            criteria.SetRandomIVs(pk, IVs);
        else
            criteria.SetRandomIVs(pk);

        pk.ResetPartyStats();

        return pk;
    }

    #endregion

    #region Matching
    public EncounterMatchRating GetMatchRating(PKM pk) => EncounterMatchRating.Match;

    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (Shiny == Shiny.Always && !pk.IsShiny)
            return false;
        if (IsEgg && Moves.HasMoves) // Odd Egg
        {
            if (pk.Format > 2)
                return false; // Can't be transferred to Gen7+
            if (!pk.HasMove((int)Move.DizzyPunch))
                return false;

            // EXP is a fixed starting value for eggs
            if (pk.IsEgg)
            {
                if (pk.EXP != 125)
                    return false;
            }
            else
            {
                if (pk.EXP < 125)
                    return false;
            }
        }

        if (!IsMatchEggLocation(pk))
            return false;
        if (!IsMatchLocation(pk))
            return false;
        if (!IsMatchLevel(pk, evo))
            return false;
        if (IVs.IsSpecified)
        {
            if (Shiny == Shiny.Always && !pk.IsShiny)
                return false;
            if (Shiny == Shiny.Never && pk.IsShiny)
                return false;
            if (pk.Format <= 2)
            {
                if (!Legal.GetIsFixedIVSequenceValidNoRand(IVs, pk))
                    return false;
            }
            else
            {
                if (Gender != FixedGenderUtil.GenderRandom && pk.Gender != Gender)
                    return false;
            }
        }
        if (Form != evo.Form && !FormInfo.IsFormChangeable(Species, Form, pk.Form, Context, pk.Context))
            return false;
        return true;
    }

    private bool IsMatchEggLocation(PKM pk)
    {
        if (pk is not ICaughtData2 c2)
        {
            var expect = pk is PB8 ? Locations.Default8bNone : EggLocation;
            return pk.EggLocation == expect;
        }

        if (pk.IsEgg)
        {
            if (!IsEgg)
                return false;
            if (c2.MetLocation != 0 && c2.MetLevel != 0)
                return false;
        }
        else
        {
            switch (c2.MetLevel)
            {
                case 0 when c2.MetLocation != 0:
                    return false;
                case 1: // 0 = second floor of every Pokémon Center, valid
                    return true;
                default:
                    if (pk.MetLocation == 0 && c2.MetLevel != 0)
                        return false;
                    break;
            }
        }

        return true;
    }

    private bool IsMatchLevel(PKM pk, EvoCriteria evo)
    {
        if (evo.LevelMax < Level)
            return false;
        if (pk is ICaughtData2 { CaughtData: not 0 })
            return pk.MetLevel == (IsEgg ? 1 : Level);
        return true;
    }

    // Routes 29-46, except 40 & 41; total 16.
    // 02, 04, 05, 08, 11, 15, 18, 20,
    // 21, 25, 26, 34, 37, 39, 43, 45,
    private const ulong RoamLocations = 0b10_1000_1010_0100_0000_0110_0011_0100_1000_1001_0011_0100;

    private bool IsMatchLocation(PKM pk)
    {
        if (IsEgg)
            return true;
        if (pk is not ICaughtData2 c2)
            return true;
        if (c2.CaughtData is 0 && Version != GameVersion.C)
            return true; // GS

        if (Roaming)
        {
            // Gen2 met location is always u8
            var loc = c2.MetLocation;
            return loc <= 45 && ((RoamLocations & (1UL << loc)) != 0);
        }
        if (Version is GameVersion.C or GameVersion.GSC)
        {
            if (c2.CaughtData is not 0)
                return Location == pk.MetLocation;
            if (pk.Species == (int)Core.Species.Celebi)
                return false; // Cannot reset the Met data
        }
        return true;
    }

    #endregion

    public EncounterTime EncounterTime => EncounterTime.Any;

    public int GetRandomTime()
    {
        if (IsEgg)
            return 0;
        return EncounterTime.RandomValidTime();
    }
}
