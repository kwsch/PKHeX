using System;
using static PKHeX.Core.PIDType;

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
        var skipBetweenPID = type is Method_3 or Method_3_Unown;
        if (skipBetweenPID) // VBlank skip between PID rand() [RARE]
            B = LCRNG.Next(B);

        var swappedPIDHalves = type is >= Method_1_Unown and <= Method_4_Unown;
        if (swappedPIDHalves) // switched order of PID halves, "BA**"
            pk.PID = (A & 0xFFFF0000) | (B >> 16);
        else
            pk.PID = (B & 0xFFFF0000) | (A >> 16);

        var C = LCRNG.Next(B);
        var skipIV1Frame = type is Method_2 or Method_2_Unown;
        if (skipIV1Frame) // VBlank skip after PID
            C = LCRNG.Next(C);

        var D = LCRNG.Next(C);
        var skipIV2Frame = type is Method_4 or Method_4_Unown;
        if (skipIV2Frame) // VBlank skip between IVs
            D = LCRNG.Next(D);

        Span<int> IVs = stackalloc int[6];
        MethodFinder.GetIVsInt32(IVs, C >> 16, D >> 16);
        if (type == Method_1_Roamer)
        {
            // Only store lowest 8 bits of IV data; zero out the other bits.
            IVs[1] &= 7;
            IVs[2..].Clear();
        }
        pk.SetIVs(IVs);
    }

    private static void SetValuesFromSeedBACD(PKM pk, PIDType type, uint seed)
    {
        uint idxor = pk.TID16 ^ (uint)pk.SID16;
        pk.PID = GetPIDFromSeedBACD(ref seed, type, idxor);
        SetIVsFromSeedSequentialLCRNG(ref seed, pk);
        pk.RefreshAbility((int)(pk.PID & 1));
    }

    private static uint GetPIDFromSeedBACD(ref uint seed, PIDType type, uint idXor) => type switch
    {
        BACD => CommonEvent3.GetRegular(ref seed),
        BACD_S => CommonEvent3.GetForceShiny(ref seed, idXor),
        BACD_AX  => CommonEvent3.GetAntishiny(ref seed, idXor),
        _ => CommonEvent3.GetRegularAntishiny(ref seed, idXor),
    };

    public static void SetIVsFromSeedSequentialLCRNG(ref uint seed, PKM pk)
    {
        var c16 = LCRNG.Next16(ref seed);
        var d16 = LCRNG.Next16(ref seed);
        Span<int> IVs = stackalloc int[6];
        MethodFinder.GetIVsInt32(IVs, c16, d16);
        pk.SetIVs(IVs);
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
        var a16 = XDRNG.Next15(ref seed); // IV1
        var b16 = XDRNG.Next15(ref seed); // IV2

        Span<int> IVs = stackalloc int[6];
        MethodFinder.GetIVsInt32(IVs, a16, b16);
        pk.SetIVs(IVs);

        _ = XDRNG.Next16(ref seed); // Ability

        if (species is (int)Species.Umbreon or (int)Species.Espeon)
        {
            // Reuse existing logic.
            pk.PID = LockFinder.GenerateStarterPID(ref seed, pk.TID16, pk.SID16);
        }
        else
        {
            // Generate PID.
            var d16 = XDRNG.Next16(ref seed); // PID
            var e16 = XDRNG.Next16(ref seed); // PID
            pk.PID = (d16 << 16) | e16;
        }
    }

    public static void SetValuesFromSeedXDRNG_EReader(PKM pk, uint seed)
    {
        var D = XDRNG.Prev3(seed); // PID
        var E = XDRNG.Next(D); // PID

        pk.PID = (D & 0xFFFF0000) | (E >> 16);
    }

    private static void SetValuesFromSeedChannel(PKM pk, uint seed)
    {
        const ushort TID16 = 40122;
        var sid = XDRNG.Next16(ref seed);
        pk.ID32 = (sid << 16) | TID16;

        var pid1 = XDRNG.Next16(ref seed);
        var pid2 = XDRNG.Next16(ref seed);
        var pid = (pid1 << 16) | pid2;
        if ((pid2 > 7 ? 0 : 1) != (pid1 ^ sid ^ TID16))
            pid ^= 0x80000000;
        pk.PID = pid;
        pk.HeldItem = (ushort)(XDRNG.Next16(ref seed) >> 15) + 169; // 0-Ganlon, 1-Salac
        pk.Version = GameVersion.S + (byte)(XDRNG.Next16(ref seed) >> 15); // 0-Sapphire, 1-Ruby
        pk.OriginalTrainerGender = (byte)(XDRNG.Next16(ref seed) >> 15);
        Span<int> ivs = stackalloc int[6];
        XDRNG.GetSequentialIVsInt32(seed, ivs);
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
            case Channel:
                return SetValuesFromSeedChannel;
            case CXD:
                return SetValuesFromSeedXDRNG;

            case Method_1 or Method_2 or Method_3 or Method_4:
            case Method_1_Unown or Method_2_Unown or Method_3_Unown or Method_4_Unown:
            case Method_1_Roamer:
                return (pk, seed) => SetValuesFromSeedLCRNG(pk, t, seed);

            case BACD:
            case BACD_S:
            case BACD_A:
            case BACD_AX:
                return (pk, seed) => SetValuesFromSeedBACD(pk, t, seed);

            case PokeSpot:
                return SetRandomPokeSpotPID;

            case G5MGShiny:
                return SetValuesFromSeedMG5Shiny;

            case Pokewalker:
                return (pk, seed) =>
                {
                    var pid = pk.PID = PokewalkerRNG.GetPID(pk.TID16, pk.SID16, seed % 24, pk.Gender, pk.PersonalInfo.Gender);
                    pk.RefreshAbility((int)(pid & 1));
                };

            // others: unimplemented
            case CuteCharm:
                break;
            case ChainShiny:
                return SetRandomChainShinyPID;
            case G4MGAntiShiny:
                break;
        }
        return (_, _) => { };
    }

    public static void SetRandomChainShinyPID(PKM pk, uint seed)
    {
        pk.PID = ClassicEraRNG.GetChainShinyPID(ref seed, pk.ID32);
        SetIVsFromSeedSequentialLCRNG(ref seed, pk);
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

    public static void SetRandomWildPID5(PKM pk, Nature nature, int ability, byte gender, PIDType specific = None)
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
            if (specific == G5MGShiny)
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
