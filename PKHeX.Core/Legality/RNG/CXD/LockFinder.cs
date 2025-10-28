using System;

namespace PKHeX.Core;

/// <summary>
/// Logic for finding the RNG criteria pattern in Colosseum/XD.
/// </summary>
public static class LockFinder
{
    /// <summary>
    /// Checks if the encounter template can be obtained with the resulting PID and IV detail.
    /// </summary>
    /// <param name="s">Encounter template with lock info</param>
    /// <param name="seed">RNG seed that generates the shadow PID and IV</param>
    /// <param name="pk">Entity to check</param>
    /// <returns>True if all valid.</returns>
    public static bool IsAllShadowLockValid<T>(IShadow3 s, uint seed, T pk) where T : IShiny, ITrainerID32
    {
        bool xd = s is EncounterShadow3XD;
        if (xd && pk.IsShiny)
            return false; // no xd shiny shadow mons
        var teams = s.PartyPrior;
        if (teams.Length == 0)
            return true;

        var tsv = xd ? (uint)(pk.TID16 ^ pk.SID16) >> 3 : uint.MaxValue; // no xd shiny shadow mons
        return IsAllShadowLockValid(teams.Span, seed, tsv);
    }

    /// <summary>
    /// Checks all <see cref="teams"/> to see if they can be reversed from the <see cref="seed"/>.
    /// </summary>
    /// <param name="teams">Possible team data setups the NPC trainer has that need to generate before the shadow.</param>
    /// <param name="seed">RNG seed that generates the shadow PID and IV</param>
    /// <param name="tsv">Trainer shiny value that is disallowed in XD</param>
    public static bool IsAllShadowLockValid(ReadOnlySpan<TeamLock> teams, uint seed, uint tsv = uint.MaxValue)
    {
        foreach (var t in teams)
        {
            var result = new TeamLockResult(t, seed, tsv);
            if (result.Valid)
                return true;
        }
        return false;
    }
}
