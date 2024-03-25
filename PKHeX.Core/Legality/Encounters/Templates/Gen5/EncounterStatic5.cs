namespace PKHeX.Core;

/// <summary>
/// Generation 5 Static Encounter
/// </summary>
public sealed record EncounterStatic5(GameVersion Version)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PK5>, IFixedGender
{
    public byte Generation => 5;
    public EntityContext Context => EntityContext.Gen5;
    public bool Roaming { get; init; }
    ushort ILocation.Location => Location;
    ushort ILocation.EggLocation => EggLocation;
    public bool IsShiny => false;
    public bool IsEgg => EggLocation != 0;
    private bool Gift => FixedBall == Ball.Poke;

    public Ball FixedBall { get; init; }

    public required ushort Species { get; init; }
    public required byte Level { get; init; }
    public required byte Location { get; init; }
    public AbilityPermission Ability { get; init; }
    public byte Form { get; init; }
    public Shiny Shiny { get; init; }
    public ushort EggLocation { get; init; }
    public byte Gender { get; init; } = FixedGenderUtil.GenderRandom;

    public string Name => "Static Encounter";
    public string LongName => Name;
    public byte LevelMin => Level;
    public byte LevelMax => Level;
    public bool IsWildCorrelationPID => !Roaming && Shiny == Shiny.Random && Species != (int)Core.Species.Crustle && !Gift && Ability != AbilityPermission.OnlyHidden;

    #region Generating

    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    public PK5 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK5 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        var version = this.GetCompatibleVersion(tr.Version);
        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language, version);
        var pi = PersonalTable.B2W2[Species];
        var pk = new PK5
        {
            Species = Species,
            Form = Form,
            CurrentLevel = LevelMin,
            MetLocation = Location,
            MetLevel = LevelMin,
            MetDate = EncounterDate.GetDateNDS(),
            Ball = (byte)(FixedBall is Ball.None ? Ball.Poke : FixedBall),

            ID32 = tr.ID32,
            Version = version,
            Language = lang,
            OriginalTrainerGender = tr.Gender,
            OriginalTrainerName = tr.OT,

            OriginalTrainerFriendship = pi.BaseFriendship,

            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation),
        };

        if (IsEgg)
        {
            // Fake as hatched.
            pk.MetLocation = Locations.HatchLocation5;
            pk.MetLevel = EggStateLegality.EggMetLevel;
            pk.EggLocation = EggLocation;
            pk.EggMetDate = pk.MetDate;
        }

        EncounterUtil.SetEncounterMoves(pk, version, LevelMin);

        SetPINGA(pk, criteria, pi);
        pk.ResetPartyStats();

        return pk;
    }

    private void SetPINGA(PK5 pk, EncounterCriteria criteria, PersonalInfo5B2W2 pi)
    {
        var gender = criteria.GetGender(Gender, pi);
        var nature = criteria.GetNature();
        var ability = criteria.GetAbilityFromNumber(Ability);
        var type = Shiny == Shiny.Always ? PIDType.G5MGShiny : PIDType.None;
        PIDGenerator.SetRandomWildPID5(pk, nature, ability, gender, type);
        criteria.SetRandomIVs(pk);
        if (Shiny == Shiny.Always)
            return;
        if (pk.IsShiny)
        {
            if ((Shiny == Shiny.Random && !criteria.Shiny.IsShiny()) || Shiny == Shiny.Never)
            {
                var pid = pk.PID;
                pid ^= 0x1000_0000;
                var result = (pid & 1) ^ (pid >> 31) ^ (pk.TID16 & 1) ^ (pk.SID16 & 1);
                if (result == 1)
                    pid ^= 1;
                pk.PID = pid;
            }
        }
        else
        {
            if (Shiny == Shiny.Random && criteria.Shiny.IsShiny())
            {
                var pid = pk.PID;
                var low = ((pid >> 16) & 1) | (pid & 0xFFFE);
                uint idx = (uint)pk.TID16 ^ pk.SID16;
                if ((idx & 1) == 1)
                    low ^= 1;
                pid = ((low ^ idx) << 16) | low;
                var result = (pid & 1) ^ (pid >> 31) ^ (pk.TID16 & 1) ^ (pk.SID16 & 1);
                if (result == 1)
                    pid ^= 1;
                pk.PID = pid;
            }
        }
    }

    #endregion

    #region Matching
    public EncounterMatchRating GetMatchRating(PKM pk)
    {
        if (IsMatchPartial(pk))
            return EncounterMatchRating.PartialMatch;
        return EncounterMatchRating.Match;
    }

    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (!IsMatchEggLocation(pk))
            return false;
        if (!IsMatchLocation(pk))
            return false;
        if (pk.MetLevel != Level)
            return false;
        if (Gender != FixedGenderUtil.GenderRandom && pk.Gender != Gender)
            return false;
        if (Form != evo.Form && !FormInfo.IsFormChangeable(Species, Form, pk.Form, Context, pk.Context))
            return false;
        return true;
    }

    private bool IsMatchPartial(PKM pk)
    {
        // B2/W2 has a static encounter Jellicent with Hidden Ability. Collides with wild surf slots.
        if (Ability == AbilityPermission.OnlyHidden && pk.AbilityNumber != 4 && pk.Format <= 7)
            return true;
        if (EggLocation == Locations.Daycare5 && pk.RelearnMove1 != 0)
            return true;
        if (pk is { AbilityNumber: 4 } && this.IsPartialMatchHidden(pk.Species, Species))
            return true;
        return false;
    }

    private bool IsMatchLocation(PKM pk)
    {
        var met = pk.MetLocation;
        if (IsEgg)
            return true;
        if (!Roaming)
            return met == Location;
        return IsRoamerMet(met);
    }

    private bool IsMatchEggLocation(PKM pk)
    {
        if (!IsEgg)
        {
            var expect = pk is PB8 ? Locations.Default8bNone : EggLocation;
            return pk.EggLocation == expect;
        }

        var eggLoc = pk.EggLocation;
        if (!pk.IsEgg) // hatched
            return eggLoc == EggLocation || eggLoc == Locations.LinkTrade5;

        // Unhatched:
        if (eggLoc != EggLocation)
            return false;
        if (pk.MetLocation is not (0 or Locations.LinkTrade5))
            return false;
        return true;
    }

    // 25,26,27,28, // Route 12, 13, 14, 15 Night latter half
    // 15,16,31,    // Route 2, 3, 18 Morning
    // 17,18,29,    // Route 4, 5, 16 Daytime
    // 19,20,21,    // Route 6, 7, 8 Evening
    // 22,23,24,    // Route 9, 10, 11 Night former half
    private static bool IsRoamerMet(ushort location)
    {
        if ((uint)location >= 32)
            return false;
        return (0b10111111111111111000000000000000 & (1 << location)) != 0;
    }

    #endregion
}
