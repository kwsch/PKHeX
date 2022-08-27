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
    public static uint GetRandomPID(Random rnd, ushort species, int gender, int origin, int nature, byte form, uint oldPID)
    {
        // Gen6+ (and VC) PIDs do not tie PID to Nature/Gender/Ability
        if (origin >= 24)
            return rnd.Rand32();

        // Below logic handles Gen3-5.
        // No need to get form specific entry, as Gen3-5 do not have that feature.
        int gt = PKX.Personal[species].Gender;
        bool g34 = origin <= 15;
        uint abilBitVal = g34 ? oldPID & 0x0000_0001 : oldPID & 0x0001_0000;

        bool g3unown = origin <= 5 && species == (int)Species.Unown;
        bool singleGender = PersonalInfo.IsSingleGender(gt); // single gender, skip gender check
        while (true) // Loop until we find a suitable PID
        {
            uint pid = rnd.Rand32();

            // Gen 3/4: Nature derived from PID
            if (g34 && pid % 25 != nature)
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
