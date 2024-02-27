using System;

namespace PKHeX.Core;

/// <summary>
/// Contains a collection of methods that mutate the input Pokémon object, usually to obtain a <see cref="PIDType"/> correlation.
/// </summary>
public static class PIDGenerator
{
    private static void SetValuesFromSeedLCRNG(PKM pk, PIDType type, uint seed)
    {
        var A = LCRNG.Next(seed);
        var B = LCRNG.Next(A);
        var skipBetweenPID = type is PIDType.Method_3 or PIDType.Method_3_Unown;
        if (skipBetweenPID) // VBlank skip between PID rand() [RARE]
            B = LCRNG.Next(B);

        var swappedPIDHalves = type is >= PIDType.Method_1_Unown and <= PIDType.Method_4_Unown;
        if (swappedPIDHalves) // switched order of PID halves, "BA**"
            pk.PID = (A & 0xFFFF0000) | (B >> 16);
        else
            pk.PID = (B & 0xFFFF0000) | (A >> 16);

        var C = LCRNG.Next(B);
        var skipIV1Frame = type is PIDType.Method_2 or PIDType.Method_2_Unown;
        if (skipIV1Frame) // VBlank skip after PID
            C = LCRNG.Next(C);

        var D = LCRNG.Next(C);
        var skipIV2Frame = type is PIDType.Method_4 or PIDType.Method_4_Unown;
        if (skipIV2Frame) // VBlank skip between IVs
            D = LCRNG.Next(D);

        Span<int> IVs = stackalloc int[6];
        MethodFinder.GetIVsInt32(IVs, C >> 16, D >> 16);
        if (type == PIDType.Method_1_Roamer)
        {
            // Only store lowest 8 bits of IV data; zero out the other bits.
            IVs[1] &= 7;
            for (int i = 2; i < 6; i++)
                IVs[i] = 0;
        }
        pk.SetIVs(IVs);
    }

    private static void SetValuesFromSeedBACD(PKM pk, PIDType type, uint seed)
    {
        bool shiny = type is PIDType.BACD_R_S or PIDType.BACD_U_S;
        uint X = shiny ? LCRNG.Next(seed) : seed;
        var A = LCRNG.Next(X);
        var B = LCRNG.Next(A);
        var C = LCRNG.Next(B);
        var D = LCRNG.Next(C);

        if (shiny)
        {
            uint PID = (X & 0xFFFF0000) | ((X >> 16) ^ pk.TID16 ^ pk.SID16);
            PID &= 0xFFFFFFF8;
            PID |= (B >> 16) & 0x7; // lowest 3 bits

            pk.PID = PID;
        }
        else if (type is PIDType.BACD_R_AX or PIDType.BACD_U_AX)
        {
            uint low = B >> 16;
            pk.PID = ((A & 0xFFFF0000) ^ ((low ^ pk.TID16 ^ pk.SID16) << 16)) | low;
        }
        else
        {
            pk.PID = (A & 0xFFFF0000) | (B >> 16);
        }

        Span<int> IVs = stackalloc int[6];
        MethodFinder.GetIVsInt32(IVs, C >> 16, D >> 16);
        pk.SetIVs(IVs);

        bool antishiny = type is PIDType.BACD_R_A or PIDType.BACD_U_A;
        while (antishiny && pk.IsShiny)
            pk.PID = unchecked(pk.PID + 1);
    }

    private static void SetValuesFromSeedXDRNG(PKM pk, uint seed)
    {
        var species = pk.Species;
        switch (species)
        {
            case (int)Species.Umbreon or (int)Species.Eevee: // Colo Umbreon, XD Eevee
                pk.TID16 = (ushort)XDRNG.Next16(ref seed);
                pk.SID16 = (ushort)XDRNG.Next16(ref seed);
                seed = XDRNG.Next2(seed); // fake PID
                break;
            case (int)Species.Espeon: // Colo Espeon
                var tid = pk.TID16 = (ushort)XDRNG.Next16(ref seed);
                var sid = pk.SID16 = (ushort)XDRNG.Next16(ref seed);
                LockFinder.SkipValidColoStarter(ref seed, tid, sid);
                seed = XDRNG.Next2(seed); // fake PID
                break;
        }
        var A = XDRNG.Next(seed); // IV1
        var B = XDRNG.Next(A); // IV2
        var C = XDRNG.Next(B); // Ability

        if (species is (int)Species.Umbreon or (int)Species.Espeon)
        {
            // Reuse existing logic.
            pk.PID = LockFinder.GenerateStarterPID(ref C, pk.TID16, pk.SID16);
        }
        else
        {
            // Generate PID.
            var D = XDRNG.Next(C); // PID
            var E = XDRNG.Next(D); // PID
            pk.PID = (D & 0xFFFF0000) | (E >> 16);
        }

        Span<int> IVs = stackalloc int[6];
        MethodFinder.GetIVsInt32(IVs, A >> 16, B >> 16);
        pk.SetIVs(IVs);
    }

    public static void SetValuesFromSeedXDRNG_EReader(PKM pk, uint seed)
    {
        var D = XDRNG.Prev3(seed); // PID
        var E = XDRNG.Next(D); // PID

        pk.PID = (D & 0xFFFF0000) | (E >> 16);
    }

    private static void SetValuesFromSeedChannel(PKM pk, uint seed)
    {
        var O = XDRNG.Next(seed); // SID16
        var A = XDRNG.Next(O); // PID
        var B = XDRNG.Next(A); // PID
        var C = XDRNG.Next(B); // Held Item
        var D = XDRNG.Next(C); // Version
        var E = XDRNG.Next(D); // OT Gender

        const ushort TID16 = 40122;
        pk.ID32 = (O & 0xFFFF0000) | TID16;
        var SID16 = O >> 16;
        var pid1 = A >> 16;
        var pid2 = B >> 16;
        var pid = (pid1 << 16) | pid2;
        if ((pid2 > 7 ? 0 : 1) != (pid1 ^ SID16 ^ TID16))
            pid ^= 0x80000000;
        pk.PID = pid;
        pk.HeldItem = (ushort)(C >> 31) + 169; // 0-Ganlon, 1-Salac
        pk.Version = GameVersion.S + (byte)(D >> 31); // 0-Sapphire, 1-Ruby
        pk.OriginalTrainerGender = (byte)(E >> 31);
        Span<int> ivs = stackalloc int[6];
        XDRNG.GetSequentialIVsInt32(E, ivs);
        pk.SetIVs(ivs);
    }

    public static void SetValuesFromSeed(PKM pk, PIDType type, uint seed)
    {
        var method = GetGeneratorMethod(type);
        method(pk, seed);
    }

    private static Action<PKM, uint> GetGeneratorMethod(PIDType t)
    {
        switch (t)
        {
            case PIDType.Channel:
                return SetValuesFromSeedChannel;
            case PIDType.CXD:
                return SetValuesFromSeedXDRNG;

            case PIDType.Method_1 or PIDType.Method_2 or PIDType.Method_3 or PIDType.Method_4:
            case PIDType.Method_1_Unown or PIDType.Method_2_Unown or PIDType.Method_3_Unown or PIDType.Method_4_Unown:
            case PIDType.Method_1_Roamer:
                return (pk, seed) => SetValuesFromSeedLCRNG(pk, t, seed);

            case PIDType.BACD_R:
            case PIDType.BACD_R_A:
            case PIDType.BACD_R_S:
            case PIDType.BACD_R_AX:
                return (pk, seed) => SetValuesFromSeedBACD(pk, t, seed & 0xFFFF);
            case PIDType.BACD_U:
            case PIDType.BACD_U_A:
            case PIDType.BACD_U_S:
            case PIDType.BACD_U_AX:
                return (pk, seed) => SetValuesFromSeedBACD(pk, t, seed);

            case PIDType.PokeSpot:
                return SetRandomPokeSpotPID;

            case PIDType.G5MGShiny:
                return SetValuesFromSeedMG5Shiny;

            case PIDType.Pokewalker:
                return (pk, seed) =>
                {
                    var pid = pk.PID = PokewalkerRNG.GetPID(pk.TID16, pk.SID16, seed % 24, pk.Gender, pk.PersonalInfo.Gender);
                    pk.RefreshAbility((int)(pid & 1));
                };

            // others: unimplemented
            case PIDType.CuteCharm:
                break;
            case PIDType.ChainShiny:
                return SetRandomChainShinyPID;
            case PIDType.G4MGAntiShiny:
                break;
        }
        return (_, _) => { };
    }

    public static void SetRandomChainShinyPID(PKM pk, uint seed)
    {
        pk.PID = ClassicEraRNG.GetChainShinyPID(ref seed, pk.ID32);

        Span<int> IVs = stackalloc int[6];
        var rand1 = LCRNG.Next16(ref seed);
        var rand2 = LCRNG.Next16(ref seed);
        MethodFinder.GetIVsInt32(IVs, rand1, rand2);
        pk.SetIVs(IVs);
    }

    private static void SetRandomPokeSpotPID(PKM pk, uint seed)
    {
        var D = XDRNG.Next(seed); // PID
        var E = XDRNG.Next(D); // PID
        pk.PID = (D & 0xFFFF0000) | (E >> 16);
    }

    public static void SetRandomPokeSpotPID(PKM pk, Nature nature, byte gender, int ability, int slot)
    {
        var rnd = Util.Rand;
        while (true)
        {
            var seed = rnd.Rand32();
            if (!MethodFinder.IsPokeSpotActivation(slot, seed, out var newSeed))
                continue;

            SetRandomPokeSpotPID(pk, newSeed);
            pk.SetRandomIVs();

            if (!IsValidCriteria4(pk, nature, ability, gender))
                continue;

            return;
        }
    }

    public static uint GetMG5ShinyPID(uint gval, uint av, ushort TID16, ushort SID16)
    {
        uint PID = ((gval ^ TID16 ^ SID16) << 16) | gval;
        if ((PID & 0x10000) != av << 16)
            PID ^= 0x10000;
        return PID;
    }

    public static void SetValuesFromSeedMG5Shiny(PKM pk, uint seed)
    {
        var gv = seed >> 24;
        var av = seed & 1; // arbitrary choice
        pk.PID = GetMG5ShinyPID(gv, av, pk.TID16, pk.SID16);
        SetRandomIVs(pk);
    }

    public static uint SetRandomWildPID4(PKM pk, Nature nature, int ability, byte gender, PIDType type)
    {
        pk.RefreshAbility(ability);
        pk.Gender = gender;
        var method = GetGeneratorMethod(type);

        var rnd = Util.Rand;
        while (true)
        {
            var seed = rnd.Rand32();
            method(pk, seed);
            if (!IsValidCriteria4(pk, nature, ability, gender))
                continue;
            return seed;
        }
    }

    public static bool IsValidCriteria4(PKM pk, Nature nature, int ability, byte gender)
    {
        if (pk.GetSaneGender() != gender)
            return false;

        if (pk.Nature != nature)
            return false;

        if ((pk.EncryptionConstant & 1) != ability)
            return false;

        return true;
    }

    public static void SetRandomWildPID5(PKM pk, Nature nature, int ability, byte gender, PIDType specific = PIDType.None)
    {
        var tidbit = (pk.TID16 ^ pk.SID16) & 1;
        pk.RefreshAbility(ability);
        pk.Gender = gender;
        pk.Nature = nature;

        if (ability == 2)
            ability = 0;

        var rnd = Util.Rand;
        while (true)
        {
            uint seed = rnd.Rand32();
            if (specific == PIDType.G5MGShiny)
            {
                SetValuesFromSeedMG5Shiny(pk, seed);
                seed = pk.PID;
            }
            else
            {
                var bitxor = (seed >> 31) ^ (seed & 1);
                if (bitxor != tidbit)
                    seed ^= 1;
            }

            if (((seed >> 16) & 1) != ability)
                continue;

            pk.PID = seed;
            if (pk.GetSaneGender() != gender)
                continue;
            return;
        }
    }

    private static void SetRandomIVs(PKM pk)
    {
        pk.SetRandomIVs();
    }
}
