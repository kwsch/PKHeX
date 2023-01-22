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

        var tsv = s.Version == GameVersion.XD ? (uint)(pk.TID16 ^ pk.SID16) >> 3 : uint.MaxValue; // no xd shiny shadow mons
        return IsAllShadowLockValid(pv, teams, tsv);
    }

    /// <summary>
    /// Checks all <see cref="teams"/> to see if they can be reversed from the <see cref="pv"/>.
    /// </summary>
    /// <param name="pv">RNG result PID and IV seed state</param>
    /// <param name="teams">Possible team data setups the NPC trainer has that need to generate before the shadow.</param>
    /// <param name="tsv">Trainer shiny value that is disallowed in XD</param>
    public static bool IsAllShadowLockValid(PIDIV pv, IEnumerable<TeamLock> teams, uint tsv = uint.MaxValue)
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
        // pidiv reversed 2x yields SID16, 3x yields TID16. shift by 7 if another PKM is generated prior
        var SIDf = XDRNG.Prev2(seed);
        var TIDf = XDRNG.Prev(SIDf);
        return SIDf >> 16 == SID16 && TIDf >> 16 == TID16;
    }

    /// <summary>
    /// Checks if the Colosseum starter correlation can be obtained with the trainer's IDs.
    /// </summary>
    /// <param name="species">Species of the starter, to indicate Espeon vs Umbreon</param>
    /// <param name="seed">Seed the PID/IV is generated with</param>
    /// <param name="TID16">Trainer ID of the trainer</param>
    /// <param name="SID16">Secret ID of the trainer</param>
    /// <param name="pkPID">PID of the entity</param>
    /// <param name="IV1">First 3 IVs combined</param>
    /// <param name="IV2">Last 3 IVs combined</param>
    public static bool IsColoStarterValid(ushort species, ref uint seed, ushort TID16, ushort SID16, uint pkPID, uint IV1, uint IV2)
    {
        // reverse the seed the bare minimum
        uint SIDf = species == (int)Species.Espeon
            ? XDRNG.Prev9(seed)
            : XDRNG.Prev2(seed);

        // reverse until we find the TID16/SID16, then run the generation forward to see if it matches our inputs.
        int ctr = 0;
        uint temp;
        while ((temp = XDRNG.Prev(SIDf)) >> 16 != TID16 || SIDf >> 16 != SID16)
        {
            SIDf = temp;
            if (ctr > 32) // arbitrary
                return false;
            ctr++;
        }

        var next = XDRNG.Next(SIDf);

        // generate Umbreon
        var PIDIV = GenerateValidColoStarterPID(ref next, TID16, SID16);
        if (species == (int)Species.Espeon) // need Espeon, which is immediately next
            PIDIV = GenerateValidColoStarterPID(ref next, TID16, SID16);

        if (!PIDIV.Equals(pkPID, IV1, IV2))
            return false;
        seed = XDRNG.Prev2(SIDf);
        return true;
    }

    private readonly record struct PIDIVGroup(uint PID, uint IV1, uint IV2)
    {
        public bool Equals(uint pid, uint iv1, uint iv2) => PID == pid && IV1 == iv1 && IV2 == iv2;
    }

    private static PIDIVGroup GenerateValidColoStarterPID(ref uint uSeed, ushort TID16, ushort SID16)
    {
        uSeed = XDRNG.Next2(uSeed); // skip fakePID
        var IV1 = (uSeed >> 16) & 0x7FFF;
        uSeed = XDRNG.Next(uSeed);
        var IV2 = (uSeed >> 16) & 0x7FFF;
        uSeed = XDRNG.Next(uSeed);
        uSeed = XDRNG.Next(uSeed); // skip ability call
        var PID = GenerateStarterPID(ref uSeed, TID16, SID16);

        uSeed = XDRNG.Next2(uSeed); // PID calls consumed

        return new PIDIVGroup(PID, IV1, IV2);
    }

    private static bool IsShiny(ushort TID16, ushort SID16, uint PID) => ((PID >> 16) ^ TID16 ^ SID16 ^ (PID & 0xFFFF)) < 8;

    private static uint GenerateStarterPID(ref uint uSeed, ushort TID16, ushort SID16)
    {
        uint PID;
        const byte ratio = 0x1F; // 12.5% F (can't be female)
        while (true)
        {
            var next = XDRNG.Next(uSeed);
            PID = (uSeed & 0xFFFF0000) | (next >> 16);
            if ((PID & 0xFF) >= ratio && !IsShiny(TID16, SID16, PID))
                break;
            uSeed = XDRNG.Next(next);
        }

        return PID;
    }
}
