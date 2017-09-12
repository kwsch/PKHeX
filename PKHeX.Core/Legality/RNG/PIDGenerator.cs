namespace PKHeX.Core
{
    public static class PIDGenerator
    {
        private static void SetValuesFromSeedLCRNG(PKM pk, PIDType type, uint seed)
        {
            var rng = RNG.LCRNG;
            var A = rng.Next(seed);
            var B = rng.Next(A);
            pk.PID = B & 0xFFFF0000 | A >> 16;

            var skipIV1Frame = type == PIDType.Method_2 || type == PIDType.Method_2_Unown;
            if (skipIV1Frame)
                B = rng.Next(B);
            var C = rng.Next(B);
            var D = rng.Next(C);

            var skipIV2Frame = type == PIDType.Method_4 || type == PIDType.Method_4_Unown;
            if (skipIV2Frame)
                D = rng.Next(D);

            pk.IVs = MethodFinder.GetIVsInt32(C >> 16, D >> 16);
        }
        private static void SetValuesFromSeedBACD(PKM pk, PIDType type, uint seed)
        {
            var rng = RNG.LCRNG;
            bool shiny = type == PIDType.BACD_R_S || type == PIDType.BACD_U_S;
            uint X = shiny ? rng.Next(seed) : seed;
            var A = rng.Next(X);
            var B = rng.Next(A);
            var C = rng.Next(B);
            var D = rng.Next(C);

            if (shiny)
            {
                uint PID;
                PID = X & 0xFFFF0000 | (uint)pk.SID ^ (uint)pk.TID ^ X >> 16;
                PID &= 0xFFFFFFF8;
                PID |= B >> 16 & 0x7; // lowest 3 bits

                pk.PID = PID;
            }
            else if (type == PIDType.BACD_R_AX || type == PIDType.BACD_U_AX)
            {
                uint low = B >> 16;
                pk.PID = A & 0xFFFF0000 ^ (((uint)pk.TID ^ (uint)pk.SID ^ low) << 16) | low;
            }
            else
                pk.PID = A & 0xFFFF0000 | B >> 16;

            pk.IVs = MethodFinder.GetIVsInt32(C >> 16, D >> 16);

            bool antishiny = type == PIDType.BACD_R_A || type == PIDType.BACD_U_A;
            while (antishiny && pk.IsShiny)
                pk.PID = unchecked(pk.PID + 1);
        }
        private static void SetValuesFromSeedXDRNG(PKM pk, uint seed)
        {
            var rng = RNG.XDRNG;
            var A = rng.Next(seed); // IV1
            var B = rng.Next(A); // IV2
            var C = rng.Next(B); // Ability?
            var D = rng.Next(C); // PID
            var E = rng.Next(D); // PID

            pk.PID = D & 0xFFFF0000 | E >> 16;
            pk.IVs = MethodFinder.GetIVsInt32(A >> 16, B >> 16);
        }
        private static void SetValuesFromSeedChannel(PKM pk, uint seed)
        {
            var rng = RNG.XDRNG;
            var O = rng.Next(seed); // SID
            var A = rng.Next(O); // PID
            var B = rng.Next(A); // PID
            var C = rng.Next(B); // Held Item
            var D = rng.Next(C); // Version
            var E = rng.Next(D); // OT Gender

            var TID = 40122;
            var SID = (int)(O >> 16);
            var pid1 = A >> 16;
            var pid2 = B >> 16;
            pk.TID = TID;
            pk.SID = SID;
            var pid = pid1 << 16 | pid2;
            if ((pid2 > 7 ? 0 : 1) != (pid1 ^ SID ^ TID))
                pid ^= 0x80000000;
            pk.PID = pid;
            pk.HeldItem = (int)(C >> 31) + 169; // 0-Ganlon, 1-Salac
            pk.Version = (int)(D >> 31) + 1; // 0-Sapphire, 1-Ruby
            pk.OT_Gender = (int)(E >> 31);
            pk.IVs = rng.GetSequentialIVsInt32(E);
        }

        public static void SetValuesFromSeed(PKM pk, PIDType type, uint seed)
        {
            switch (type)
            {
                case PIDType.Channel:
                    SetValuesFromSeedChannel(pk, seed);
                    break;
                case PIDType.CXD:
                    SetValuesFromSeedXDRNG(pk, seed);
                    break;

                case PIDType.Method_1:
                case PIDType.Method_2:
                case PIDType.Method_4:
                    SetValuesFromSeedLCRNG(pk, type, seed);
                    break;

                case PIDType.BACD_R:
                case PIDType.BACD_R_A:
                case PIDType.BACD_R_S:
                    SetValuesFromSeedBACD(pk, type, seed);
                    break;
                case PIDType.BACD_U:
                case PIDType.BACD_U_A:
                case PIDType.BACD_U_S:
                    SetValuesFromSeedBACD(pk, type, seed);
                    break;

                // others: unimplemented
                case PIDType.ChainShiny:
                    break;
                case PIDType.Method_1_Unown:
                case PIDType.Method_2_Unown:
                case PIDType.Method_4_Unown:
                    break;
                case PIDType.Method_1_Roamer:
                    break;

                case PIDType.CuteCharm:
                    break;
                case PIDType.PokeSpot:
                    break;
                case PIDType.G4MGAntiShiny:
                    break;
                case PIDType.G5MGShiny:
                    break;
                case PIDType.Pokewalker:
                    break;
            }
        }
    }
}
