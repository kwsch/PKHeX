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
    /// <param name="pv">RNG result PID and IV seed state</param>
    /// <param name="pk">Entity to check</param>
    /// <returns>True if all valid.</returns>
    public static bool IsAllShadowLockValid(IShadow3 s, PIDIV pv, PKM pk)
    {
        if (s.Version == GameVersion.XD && pk.IsShiny)
            return false; // no xd shiny shadow mons
        var teams = s.PartyPrior;
        if (teams.Length == 0)
            return true;

        var tsv = s.Version == GameVersion.XD ? (uint)(pk.TID16 ^ pk.SID16) >> 3 : uint.MaxValue; // no xd shiny shadow mons
        return IsAllShadowLockValid(pv, teams.Span, tsv);
    }

    /// <summary>
    /// Checks all <see cref="teams"/> to see if they can be reversed from the <see cref="pv"/>.
    /// </summary>
    /// <param name="pv">RNG result PID and IV seed state</param>
    /// <param name="teams">Possible team data setups the NPC trainer has that need to generate before the shadow.</param>
    /// <param name="tsv">Trainer shiny value that is disallowed in XD</param>
    public static bool IsAllShadowLockValid(PIDIV pv, ReadOnlySpan<TeamLock> teams, uint tsv = uint.MaxValue)
    {
        foreach (var t in teams)
        {
            var result = new TeamLockResult(t, pv.OriginSeed, tsv);
            if (result.Valid)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if the XD starter Eevee can be obtained with the trainer's IDs.
    /// </summary>
    /// <param name="seed">Seed that generated the PID and IV</param>
    /// <param name="TID16">Trainer ID</param>
    /// <param name="SID16">Trainer Secret ID</param>
    /// <returns>True if the starter ID correlation is correct</returns>
    public static bool IsXDStarterValid(uint seed, uint TID16, uint SID16)
    {
        // pidiv is right before the IV calls; need to unroll 2x for fake calls, then unroll for the TID/SID.
        // reversed 2x yields SID16, 3x yields TID16.
        var TIDf = XDRNG.Prev3(seed);
        if (TIDf >> 16 != TID16)
            return false;
        var SIDf = XDRNG.Next(TIDf);
        return SIDf >> 16 == SID16;
    }

    /// <summary>
    /// Checks if the Colosseum starter correlation can be obtained with the trainer's IDs.
    /// </summary>
    /// <param name="species">Species of the starter, to indicate Espeon vs Umbreon</param>
    /// <param name="origin">Seed the PID/IV is generated with</param>
    /// <param name="TID16">Trainer ID of the trainer</param>
    /// <param name="SID16">Secret ID of the trainer</param>
    /// <param name="pkPID">PID of the entity</param>
    /// <param name="IV1">First 3 IVs combined</param>
    /// <param name="IV2">Last 3 IVs combined</param>
    public static bool IsColoStarterValid(ushort species, ref uint origin, ushort TID16, ushort SID16, uint pkPID, uint IV1, uint IV2)
    {
        // Input seed is right after the TID/SID and 2x fake rolls. Reverse the seed to the first possible SID seed value.
        var seed = species == (int)Species.Espeon
            ? XDRNG.Prev12(origin)
            : XDRNG.Prev3(origin);

        // Reverse until we find the TID16/SID16, then run the generation forward to see if it matches our inputs.
        const int arbitraryLookback = 8;
        int ctr = 0;
        while (true)
        {
            if (seed >> 16 == SID16 && XDRNG.Prev(seed) >> 16 == TID16)
            {
                origin = XDRNG.Prev2(seed);
                break; // result!
            }
            if (++ctr == arbitraryLookback)
                return false; // no valid seed found
            seed = XDRNG.Prev2(seed);
        }

        // generate Umbreon
        var PIDIV = GenerateValidColoStarter(ref seed, TID16, SID16);
        if (species == (int)Species.Espeon) // need Espeon, which is immediately next
            PIDIV = GenerateValidColoStarter(ref seed, TID16, SID16);
        return PIDIV.Equals(pkPID, IV1, IV2);
    }

    private readonly record struct PIDIVGroup(uint PID, uint IV1, uint IV2)
    {
        public bool Equals(uint pid, uint iv1, uint iv2) => PID == pid && IV1 == iv1 && IV2 == iv2;
    }

    public static void SkipValidColoStarter(ref uint seed, ushort TID16, ushort SID16) => GenerateValidColoStarter(ref seed, TID16, SID16);

    private static PIDIVGroup GenerateValidColoStarter(ref uint seed, ushort TID16, ushort SID16)
    {
        seed = XDRNG.Next2(seed); // skip fakePID
        var IV1 = XDRNG.Next15(ref seed);
        var IV2 = XDRNG.Next15(ref seed);
        seed = XDRNG.Next(seed); // ability call
        var PID = GenerateStarterPID(ref seed, TID16, SID16);

        return new PIDIVGroup(PID, IV1, IV2);
    }

    private static bool IsShiny(ushort TID16, ushort SID16, uint PID) => ((PID >> 16) ^ TID16 ^ SID16 ^ (PID & 0xFFFF)) < 8;

    public static uint GenerateStarterPID(ref uint seed, ushort TID16, ushort SID16)
    {
        const byte ratio = 0x1F; // 12.5% F (can't be female)
        while (true)
        {
            var first = seed = XDRNG.Next(seed); // first PID roll
            var second = seed = XDRNG.Next(seed); // second PID roll
            var PID = (first & 0xFFFF0000) | (second >> 16);
            if ((PID & 0xFF) >= ratio && !IsShiny(TID16, SID16, PID))
                return PID;
        }
    }
}
