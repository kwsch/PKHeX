using System;
using System.Diagnostics;

namespace PKHeX.Core;

/// <summary>
/// Legality mutations for egg hatching.
/// </summary>
public static class EggHatchLegality
{
    /// <summary>
    /// Returns a valid hatch location for the specified <see cref="EntityContext.Gen4"/> game.
    /// </summary>
    public static ushort GetHatchLocation4(GameVersion version) =>
        version is GameVersion.HG or GameVersion.SS
            ? Locations.HatchLocationHGSS
            : Locations.HatchLocationDPPt;

    /// <summary>
    /// Forces the specified <see cref="PK4"/> to hatch.
    /// </summary>
    /// <param name="pk">The Pokémon to hatch.</param>
    /// <param name="tr">The trainer information.</param>
    /// <param name="date">The date of hatching.</param>
    public static void ForceHatch(PK4 pk, ITrainerInfo tr, DateOnly date = default)
    {
        Debug.Assert(pk.IsEgg);
        if (!EncounterDate.IsValidDateNDS(date))
            date = EncounterDate.GetDateNDS();

        if (!tr.IsOriginalHandler(pk, checkGame: true))
            Trade(pk, date); // Must have been traded; trade now.

        // Hatching the egg basically regenerates it into a new PK4. We'll keep the same object, but be more advanced on how we treat it.
        // If a met location is present, transfer it to Egg Location (such as a link traded egg).
        if (pk.MetLocation != 0) // only true if Link Trade was set via Trade.
        {
            pk.EggLocation = pk.MetLocation;
            pk.EggMetDate = pk.MetDate;
        }

        // Version is copied, not updated; need to have a valid one for the hatching trainer to determine a valid hatch location.
        var version = tr.Version;
        if (version is not (GameVersion.HG or GameVersion.SS or GameVersion.D or GameVersion.P or GameVersion.Pt))
            version = GameVersion.HG;

        pk.NicknameTrash.Clear();
        pk.OriginalTrainerTrash.Clear();
        // Version is not re-applied on hatch in Gen4.
        pk.Language = (int)Language.GetSafeLanguage456((LanguageID)tr.Language);
        pk.ClearNickname(); // reset
        pk.IsEgg = false;

        pk.MetLocation = GetHatchLocation4(version);
        pk.MetLevel = 0;
        pk.MetDate = date;

        // Set new Trainer details on hatch.
        pk.OriginalTrainerGender = tr.Gender;
        pk.OriginalTrainerName = tr.OT;
        pk.OriginalTrainerFriendship = EggStateLegality.GetEggHatchFriendship(EntityContext.Gen4);

        // Order of operations issue: this should have been before the Met Location is moved; resulting in Ranger Manaphy being able to be shiny.
        var id32 = pk.ID32 = tr.ID32;
        if (pk is { EggLocation: Locations.Ranger4, Species: (ushort)Species.Manaphy, IsShiny: true })
        {
            var pid = pk.PID;
            var xor = ShinyUtil.GetShinyXor(id32) >> 3;
            while (true)
            {
                pid = ARNG.Next(pid);
                var newXor = ShinyUtil.GetShinyXor(pid) >> 3;
                if (newXor != xor)
                    break;
            }
            pk.PID = pid;
        }

        // Mimic the rest of the property copies; the property setters hide some per-game quirks.
        pk.Ball = (byte)Ball.Poke;// Hatching regenerates the egg, can clear/set the HG/SS ball value.
        pk.EggLocation = pk.EggLocation; // just to be replicating 100% and setting for the correct hatch game.
    }

    /// <summary>
    /// A trade in Gen4 updates the met location and date of the egg.
    /// </summary>
    /// <param name="pk">The Pokémon to check.</param>
    /// <param name="date">The date of the trade.</param>
    public static void Trade(PK4 pk, DateOnly date = default)
    {
        Debug.Assert(pk.IsEgg);
        if (!EncounterDate.IsValidDateNDS(date))
            date = EncounterDate.GetDateNDS();
        pk.MetLocation = Locations.LinkTrade4;
        pk.MetDate = date;
    }
}
