using System;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core;

/// <summary>
/// An obtainable FR/LG encounter transferred directly to HOME from Nintendo Switch.
/// HOME replaces the original met details; only Switch games can be visited afterwards.
/// </summary>
public sealed record EncounterTransfer3HOME(IEncounterable Original, GameVersion Version, ushort Species, byte Form, byte Level)
    : IEncounterable, IEncounterMatch, IFatefulEncounterReadOnly, IMemoryOTReadOnly, IFixedOTFriendship
{
    public byte Generation => 8;
    public EntityContext Context => EntityContext.Gen8;
    public bool IsEgg => false;
    public bool IsShiny => false;
    public Shiny Shiny => Original.Shiny;
    public AbilityPermission Ability => Original.Ability;
    public Ball FixedBall => Original.FixedBall;
    public bool FatefulEncounter => Original is IFatefulEncounterReadOnly { FatefulEncounter: true };
    public ushort Location => LocationsHOME.Transfer3HOME;
    public ushort EggLocation => 0;
    public byte LevelMin => Level;
    public byte LevelMax => Level;
    public string Name => "HOME Transfer (FR/LG Switch)";
    public string LongName => $"{Name}: {Original.LongName}";

    // HOME clears OT friendship; the GBA games cannot set OT memories.
    public byte OriginalTrainerFriendship => 0;
    public byte OriginalTrainerMemory => 0;
    public byte OriginalTrainerMemoryIntensity => 0;
    public byte OriginalTrainerMemoryFeeling => 0;
    public ushort OriginalTrainerMemoryVariable => 0;

    public static bool IsOrigin(PKM pk) => (pk is PK8 or PA8 or PB8 or PK9 or PA9) &&
        (pk.Version is FR_NX or LG_NX || pk is PK8 { Version: SW or SH, MetLocation: LocationsHOME.SWFR or LocationsHOME.SHLG });

    internal static GameVersion GetOriginVersion(PKM pk) => pk.Version is FR_NX or LG_NX
        ? pk.Version
        : LocationsHOME.GetVersionSWSHOriginal(pk.MetLocation);

    public bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (!IsOrigin(pk) || GetOriginVersion(pk) != Version || pk.IsEgg || pk.MetLevel != Level || pk.CurrentLevel < Level)
            return false;
        var met = pk is PK8 ? LocationsHOME.GetMetSWSH(Location, Version) : Location;
        var egg = pk is PB8 ? Locations.Default8bNone : 0;
        return pk.MetLocation == met && pk.EggLocation == egg && evo.Species == Species && evo.Form == Form;
    }

    public EncounterMatchRating GetMatchRating(PKM pk)
    {
        if (pk.FatefulEncounter != FatefulEncounter)
            return EncounterMatchRating.PartialMatch;
        if (!BallVerifier.VerifyBall(Original, (Ball)pk.Ball, pk).IsValid)
            return EncounterMatchRating.PartialMatch;
        return EncounterMatchRating.Match;
    }

    public PKM ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);
    public PKM ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => throw new InvalidOperationException("Conversion not supported.");
}