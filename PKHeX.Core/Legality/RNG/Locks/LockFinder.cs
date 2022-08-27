using System.Collections.Generic;

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
    public static bool IsAllShadowLockValid(EncounterStaticShadow s, PIDIV pv, PKM pk)
    {
        if (s.Version == GameVersion.XD && pk.IsShiny)
            return false; // no xd shiny shadow mons
        var teams = s.Locks;
        if (teams.Length == 0)
            return true;

        var tsv = s.Version == GameVersion.XD ? (pk.TID ^ pk.SID) >> 3 : -1; // no xd shiny shadow mons
        return IsAllShadowLockValid(pv, teams, tsv);
    }

    /// <summary>
    /// Checks all <see cref="teams"/> to see if they can be reversed from the <see cref="pv"/>.
    /// </summary>
    /// <param name="pv">RNG result PID and IV seed state</param>
    /// <param name="teams">Possible team data setups the NPC trainer has that need to generate before the shadow.</param>
    /// <param name="tsv">Trainer shiny value that is disallowed in XD</param>
    public static bool IsAllShadowLockValid(PIDIV pv, IEnumerable<TeamLock> teams, int tsv = -1)
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
    /// <param name="TID">Trainer ID</param>
    /// <param name="SID">Trainer Secret ID</param>
    /// <returns>True if the starter ID correlation is correct</returns>
    public static bool IsXDStarterValid(uint seed, int TID, int SID)
    {
        // pidiv reversed 2x yields SID, 3x yields TID. shift by 7 if another PKM is generated prior
        var SIDf = RNG.XDRNG.Reverse(seed, 2);
        var TIDf = RNG.XDRNG.Prev(SIDf);
        return SIDf >> 16 == SID && TIDf >> 16 == TID;
    }

    /// <summary>
    /// Checks if the Colosseum starter correlation can be obtained with the trainer's IDs.
    /// </summary>
    /// <param name="species">Species of the starter, to indicate Espeon vs Umbreon</param>
    /// <param name="seed">Seed the PID/IV is generated with</param>
    /// <param name="TID">Trainer ID of the trainer</param>
    /// <param name="SID">Secret ID of the trainer</param>
    /// <param name="pkPID">PID of the entity</param>
    /// <param name="IV1">First 3 IVs combined</param>
    /// <param name="IV2">Last 3 IVs combined</param>
    public static bool IsColoStarterValid(ushort species, ref uint seed, int TID, int SID, uint pkPID, uint IV1, uint IV2)
    {
        // reverse the seed the bare minimum
        int rev = 2;
        if (species == (int)Species.Espeon)
            rev += 7;

        // reverse until we find the TID/SID, then run the generation forward to see if it matches our inputs.
        var rng = RNG.XDRNG;
        var SIDf = rng.Reverse(seed, rev);
        int ctr = 0;
        uint temp;
        while ((temp = rng.Prev(SIDf)) >> 16 != TID || SIDf >> 16 != SID)
        {
            SIDf = temp;
            if (ctr > 32) // arbitrary
                return false;
            ctr++;
        }

        var next = rng.Next(SIDf);

        // generate Umbreon
        var PIDIV = GenerateValidColoStarterPID(ref next, TID, SID);
        if (species == (int)Species.Espeon) // need Espeon, which is immediately next
            PIDIV = GenerateValidColoStarterPID(ref next, TID, SID);

        if (!PIDIV.Equals(pkPID, IV1, IV2))
            return false;
        seed = rng.Reverse(SIDf, 2);
        return true;
    }

    private readonly record struct PIDIVGroup(uint PID, uint IV1, uint IV2)
    {
        public bool Equals(uint pid, uint iv1, uint iv2) => PID == pid && IV1 == iv1 && IV2 == iv2;
    }

    private static PIDIVGroup GenerateValidColoStarterPID(ref uint uSeed, int TID, int SID)
    {
        var rng = RNG.XDRNG;

        uSeed = rng.Advance(uSeed, 2); // skip fakePID
        var IV1 = (uSeed >> 16) & 0x7FFF;
        uSeed = rng.Next(uSeed);
        var IV2 = (uSeed >> 16) & 0x7FFF;
        uSeed = rng.Next(uSeed);
        uSeed = rng.Advance(uSeed, 1); // skip ability call
        var PID = GenerateStarterPID(ref uSeed, TID, SID);

        uSeed = rng.Advance(uSeed, 2); // PID calls consumed

        return new PIDIVGroup(PID, IV1, IV2);
    }

    private static bool IsShiny(int TID, int SID, uint PID) => (TID ^ SID ^ (PID >> 16) ^ (PID & 0xFFFF)) < 8;

    private static uint GenerateStarterPID(ref uint uSeed, int TID, int SID)
    {
        uint PID;
        const byte ratio = 0x1F; // 12.5% F (can't be female)
        while (true)
        {
            var next = RNG.XDRNG.Next(uSeed);
            PID = (uSeed & 0xFFFF0000) | (next >> 16);
            if ((PID & 0xFF) >= ratio && !IsShiny(TID, SID, PID))
                break;
            uSeed = RNG.XDRNG.Next(next);
        }

        return PID;
    }
}
