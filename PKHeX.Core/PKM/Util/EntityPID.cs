using System;

namespace PKHeX.Core;

public static class EntityPID
{
    /// <summary>
    /// Gets a random PID according to specifications.
    /// </summary>
    /// <param name="rnd">RNG to use</param>
    /// <param name="species">National Dex ID</param>
    /// <param name="gender">Current Gender</param>
    /// <param name="origin">Origin Generation</param>
    /// <param name="nature">Nature</param>
    /// <param name="form">Form</param>
    /// <param name="oldPID">Current PID</param>
    /// <remarks>Used to retain ability bits.</remarks>
    /// <returns>Rerolled PID.</returns>
    public static uint GetRandomPID(Random rnd, ushort species, byte gender, GameVersion origin, Nature nature, byte form, uint oldPID)
    {
        // Gen6+ (and VC) PIDs do not tie PID to Nature/Gender/Ability
        if (origin is 0 or >= GameVersion.X)
            return rnd.Rand32();

        // Below logic handles Gen3-5.
        // No need to get form specific entry, as Gen3-5 do not have that feature.
        var gt = PersonalTable.B2W2[species].Gender;
        bool g34 = origin <= GameVersion.CXD;
        uint abilBitVal = g34 ? oldPID & 0x0000_0001 : oldPID & 0x0001_0000;

        bool g3unown = origin is GameVersion.FR or GameVersion.LG && species == (int)Species.Unown;
        bool singleGender = PersonalInfo.IsSingleGender(gt); // single gender, skip gender check
        while (true) // Loop until we find a suitable PID
        {
            uint pid = rnd.Rand32();

            // Gen 3/4: Nature derived from PID
            if (g34 && pid % 25 != (byte)nature)
                continue;

            // Gen 3 Unown: Letter/form derived from PID
            if (g3unown)
            {
                var pidLetter = GetUnownForm3(pid);
                if (pidLetter != form)
                    continue;
            }
            else if (g34)
            {
                if (abilBitVal != (pid & 0x0000_0001)) // keep ability bits
                    continue;
            }
            else
            {
                if (abilBitVal != (pid & 0x0001_0000)) // keep ability bits
                    continue;
            }

            if (singleGender) // Set Gender(less)
                return pid; // PID can be anything

            // Gen 3/4/5: Gender derived from PID
            if (gender == EntityGender.GetFromPIDAndRatio(pid, gt))
                return pid;
        }
    }

    /// <summary>
    /// Gets the Unown Forme ID from PID.
    /// </summary>
    /// <param name="pid">Personality ID</param>
    /// <remarks>Should only be used for 3rd Generation origin specimens.</remarks>
    public static byte GetUnownForm3(uint pid)
    {
        var value = ((pid & 0x3000000) >> 18) | ((pid & 0x30000) >> 12) | ((pid & 0x300) >> 6) | (pid & 0x3);
        return (byte)(value % 28);
    }
}
