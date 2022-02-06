namespace PKHeX.Core;

/// <summary>
/// Contains logic for the Generation 8 (Legends: Arceus) overworld spawns that walk around the overworld.
/// </summary>
public static class Overworld8aRNG
{
    public static uint AdaptPID(PKM pk, Shiny shiny, uint pid)
    {
        if (shiny == Shiny.Never)
        {
            if (GetIsShiny(pk.TID, pk.SID, pid))
                pid ^= 0x1000_0000;
        }
        else if (shiny != Shiny.Random)
        {
            if (!GetIsShiny(pk.TID, pk.SID, pid))
                pid = GetShinyPID(pk.TID, pk.SID, pid, 0);
        }
        return pid;
    }

    private static uint GetShinyPID(int tid, int sid, uint pid, int type)
    {
        return (uint)(((tid ^ sid ^ (pid & 0xFFFF) ^ type) << 16) | (pid & 0xFFFF));
    }

    private static bool GetIsShiny(int tid, int sid, uint pid)
    {
        return GetShinyXor(pid, (uint)((sid << 16) | tid)) < 16;
    }

    private static uint GetShinyXor(uint pid, uint oid)
    {
        var xor = pid ^ oid;
        return (xor ^ (xor >> 16)) & 0xFFFF;
    }
}
