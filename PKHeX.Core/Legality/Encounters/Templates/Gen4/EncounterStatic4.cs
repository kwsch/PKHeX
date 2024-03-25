using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.GroundTileAllowed;

namespace PKHeX.Core;

/// <summary>
/// Generation 4 Static Encounter
/// </summary>
public sealed record EncounterStatic4(GameVersion Version)
    : IEncounterable, IEncounterMatch, IEncounterConvertible<PK4>, IMoveset, IGroundTypeTile, IFatefulEncounterReadOnly, IFixedGender, IRandomCorrelation, IFixedNature
{
    public byte Generation => 4;
    public EntityContext Context => EntityContext.Gen4;
    ushort ILocation.Location => Location;
    ushort ILocation.EggLocation => EggLocation;
    public bool IsShiny => false;
    public bool IsEgg => EggLocation != 0;
    private bool Gift => FixedBall == Ball.Poke;

    public Ball FixedBall { get; init; }
    public bool FatefulEncounter { get; init; }

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

    /// <summary> Indicates if the encounter is a Roamer (variable met location) </summary>
    public bool Roaming { get; init; }

    /// <summary> <see cref="PK4.GroundTile"/> values permitted for the encounter. </summary>
    public GroundTileAllowed GroundTile { get; init; } = None;

    public ushort HeldItem { get; init; }
    public Nature Nature { get; init; } = Nature.Random;
    public Moveset Moves { get; init; }

    #region Generating
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    public PK4 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PK4 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language);
        var version = this.GetCompatibleVersion(tr.Version);
        var pi = PersonalTable.HGSS[Species];
        var pk = new PK4
        {
            Species = Species,
            Form = Form,
            CurrentLevel = LevelMin,
            OriginalTrainerFriendship = pi.BaseFriendship,

            MetLocation = Location,
            MetLevel = LevelMin,
            Version = version,
            GroundTile = GroundTile.GetIndex(),
            MetDate = EncounterDate.GetDateNDS(),
            Ball = (byte)(FixedBall != Ball.None ? FixedBall : Ball.Poke),
            FatefulEncounter = FatefulEncounter,

            Language = lang,
            OriginalTrainerName = tr.OT,
            OriginalTrainerGender = tr.Gender,
            ID32 = tr.ID32,
            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, Generation),
        };

        if (IsEgg)
        {
            // Fake as hatched.
            pk.MetLocation = version is GameVersion.HG or GameVersion.SS ? Locations.HatchLocationHGSS : Locations.HatchLocationDPPt;
            pk.MetLevel = EggStateLegality.EggMetLevel34;
            pk.EggLocation = EggLocation;
            pk.EggMetDate = pk.MetDate;
        }
        else if (Species == (int)Core.Species.Giratina && Form == 1)
        {
            pk.HeldItem = 0112; // Griseous Orb
        }

        SetPINGA(pk, criteria, pi);
        if (Moves.HasMoves)
            pk.SetMoves(Moves);
        else
            EncounterUtil.SetEncounterMoves(pk, version, LevelMin);

        pk.ResetPartyStats();
        return pk;
    }

    private void SetPINGA(PK4 pk, EncounterCriteria criteria, PersonalInfo4 pi)
    {
        // Pichu is special -- use Pokewalker method
        if (Species == (int)Core.Species.Pichu)
        {
            var pid = pk.PID = PokewalkerRNG.GetPID(pk.ID32, (uint)Nature, pk.Gender = Gender, pi.Gender);
            pk.RefreshAbility((int)(pid & 1));
            criteria.SetRandomIVs(pk); // IVs are sufficiently random; set based on request.
            return;
        }

        var gender = criteria.GetGender(Gender, pi);
        var nature = criteria.GetNature(Nature);
        int ability = criteria.GetAbilityFromNumber(Ability);
        if (Shiny == Shiny.Always) // Chain Shiny
        {
            SetChainShiny(pk, pi.Gender, ability, gender, nature);
            return;
        }
        PIDType type = this is { Shiny: Shiny.Always } ? PIDType.ChainShiny : PIDType.Method_1;
        PIDGenerator.SetRandomWildPID4(pk, nature, ability, gender, type);
    }

    private static void SetChainShiny(PK4 pk, byte gr, int ability, byte gender, Nature nature)
    {
        pk.RefreshAbility(ability);
        pk.Gender = gender;
        var seed = Util.Rand32();
        var id32 = pk.ID32;
        while (true)
        {
            var pid = ClassicEraRNG.GetChainShinyPID(ref seed, id32);
            if ((Nature)(pid % 25) != nature)
                continue;
            if (EntityGender.GetFromPIDAndRatio(pid, gr) != gender)
                continue;
            if ((pid & 1) != ability)
                continue;

            pk.PID = pid;
            pk.IV32 = ClassicEraRNG.GetSequentialIVs(ref seed);
            break;
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
        if (!IsMatchLevel(pk, evo))
            return false;
        if (Gender != FixedGenderUtil.GenderRandom && pk.Gender != Gender)
            return false;
        if (Form != evo.Form && !FormInfo.IsFormChangeable(Species, Form, pk.Form, Context, pk.Context))
            return false;
        return true;
    }

    private bool IsMatchLocation(PKM pk)
    {
        // Met location is lost on transfer
        if (pk is not G4PKM pk4)
            return true;

        var met = pk4.MetLocation;
        if (IsEgg)
            return true;
        if (!Roaming)
            return met == Location;

        return pk4.GroundTile switch
        {
            GroundTileType.Grass => IsMatchLocationGrass(Location, met),
            GroundTileType.Water => IsMatchLocationWater(Location, met),
            _ => false,
        };
    }

    private bool IsMatchEggLocation(PKM pk)
    {
        if (!IsEgg)
        {
            var expect = pk is PB8 ? Locations.Default8bNone : EggLocation;
            return pk.EggLocation == expect;
        }

        var eggLoc = pk.EggLocation;
        // Transferring 4->5 clears Pt/HG/SS location value and keeps Faraway Place
        if (pk is not G4PKM pk4)
        {
            if (eggLoc == Locations.LinkTrade4)
                return true;
            var cmp = Locations.IsPtHGSSLocationEgg(EggLocation) ? Locations.Faraway4 : EggLocation;
            return eggLoc == cmp;
        }

        if (!pk4.IsEgg) // hatched
            return eggLoc == EggLocation || eggLoc == Locations.LinkTrade4;

        // Unhatched:
        if (eggLoc != EggLocation)
            return false;
        if (pk4.MetLocation is not (0 or Locations.LinkTrade4))
            return false;
        return true;
    }

    private static bool IsMatchLocationGrass(ushort location, ushort met) => location switch
    {
        FirstS => IsMatchRoamerLocation(PermitGrassS, met, FirstS),
        FirstJ => IsMatchRoamerLocation(PermitGrassJ, met, FirstJ),
        FirstH => IsMatchRoamerLocation(PermitGrassH, met, FirstH),
        _ => false,
    };

    private static bool IsMatchLocationWater(ushort location, ushort met) => location switch
    {
        FirstS => IsMatchRoamerLocation(PermitWaterS, met, FirstS),
        FirstJ => IsMatchRoamerLocation(PermitWaterJ, met, FirstJ),
        FirstH => IsMatchRoamerLocation(PermitWaterH, met, FirstH),
        _ => false,
    };

    private bool IsMatchLevel(PKM pk, EvoCriteria evo)
    {
        if (pk.Format != 4) // Met Level lost on PK4=>PK5
            return Level <= evo.LevelMax;

        return pk.MetLevel == (IsEgg ? 0 : Level);
    }

    private bool IsMatchPartial(PKM pk) => Gift && pk.Ball != (byte)FixedBall;

    public static bool IsMatchRoamerLocation([ConstantExpected] ulong permit, ushort location, int first)
    {
        var value = location - first;
        if ((uint)value >= 64)
            return false;
        return (permit & (1ul << value)) != 0;
    }

    public static bool IsMatchRoamerLocation([ConstantExpected] uint permit, ushort location, int first)
    {
        var value = location - first;
        if ((uint)value >= 32)
            return false;
        return (permit & (1u << value)) != 0;
    }

    // Merged all locations into a bitmask for quick computation.
    private const int FirstS = 16;
    private const ulong PermitGrassS = 0x2_8033FFFF; // 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33,         36, 37, 47, 49,
    private const ulong PermitWaterS = 0x2_803E3B9E; //         18, 19, 20,         23, 24, 25,     27, 28, 29,             33, 34, 35, 36, 37, 47, 49,

    private const int FirstJ = 177;
    private const uint PermitGrassJ = 0x0003E7FF; // 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187,                     190, 191, 192, 193, 194,
    private const uint PermitWaterJ = 0x0001E06E; //      178, 179, 180,      182, 183,                                         190, 191, 192, 193,

    private const int FirstH = 149;
    private const uint PermitGrassH = 0x0AB3FFFF; // 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166,           169, 170, 172,      174,      176, 
    private const uint PermitWaterH = 0x0ABC1B28; //                152,      154,           157, 158,      160, 161,                          167, 168, 169, 170, 172,      174,      176,

    #endregion

    public bool IsCompatible(PIDType val, PKM pk)
    {
        if (Species == (int)Core.Species.Pichu)
            return val == PIDType.Pokewalker;
        if (Shiny == Shiny.Always)
            return val == PIDType.ChainShiny;
        if (val is PIDType.Method_1)
            return true;
        if (val is PIDType.CuteCharm)
            return MethodFinder.IsCuteCharm4Valid(this, pk);
        return false;
    }

    public PIDType GetSuggestedCorrelation()
    {
        if (Species == (int)Core.Species.Pichu)
            return PIDType.Pokewalker;
        if (Shiny == Shiny.Always)
            return PIDType.ChainShiny;
        return PIDType.Method_1;
    }
}
