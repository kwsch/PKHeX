namespace PKHeX.Core
{
    public class WormholeInfoReader
    {
        public readonly SAV7 SAV;
        public WormholeInfoReader(SAV7 sav) => SAV = sav;

        // Wormhole shininess & flags found by @PP-theSLAYER
        // https://projectpokemon.org/home/forums/topic/39433-gen-7-save-research-thread/?page=3&tab=comments#comment-239090
        public bool WormholeShininess // 0x4535 = Misc (0x4400 in USUM) + 0x0135
        {
            get => SAV.Data[SAV.Misc + 0x0135] == 1;
            set => SAV.Data[SAV.Misc + 0x0135] = (byte)(value ? 1 : 0);
        }

        // TODO: Find out if legendaries are in slots too
        public const int WormholeSlotMax = 4;

        public int WormholeSlot
        {
            get
            {
                //if (!InWormhole())
                //    return -1;
                for (int i = 0; i <= WormholeSlotMax; i++)
                {
                    if (!SAV.GetEventFlag(i + 11))
                        return i;
                }
                return -1;
            }
            set
            {
                if (value < 0 || value > WormholeSlotMax)
                    return;
                for (int i = 0; i <= WormholeSlotMax; i++)
                {
                    SAV.SetEventFlag(i + 11, value != i);
                }

                // TODO: Is there a better way to set individual consts while using the API?
                var consts = SAV.EventConsts;
                consts[851] = (ushort)(value + 38);
                SAV.EventConsts = consts;
            }
        }

        public static readonly int[] StandardWormholes =
        {
            256, // Red
            257, // Green
            258, // Yellow
            259, // Blue
        };

        public static readonly int[] WormholeSlotsRed =
        {
            334, // Altaria
            469, // Yanmega
            561, // Sigilyph
            581, // Swanna
            277, // Swellow
        };

        public static readonly int[] WormholeSlotsGreen =
        {
            542, // Drapion
            531, // Audino
            695, // Heliolisk
            274, // Nuzleaf
            326, // Grumpig
        };

        public static readonly int[] WormholeSlotsYellow =
        {
            460, // Abomasnow
            308, // Medicham
            450, // Hippowdon
            558, // Crustle
            219, // Magcargo
        };

        public static readonly int[] WormholeSlotsBlue =
        {
            689, // Barbaracle
            271, // Lombre
            618, // Stunfisk
            419, // Floatzel
            195, // Quagsire
        };

        public int WormholeSlotToPokemon(int mapid, int slot)
        {
            if (slot < 0 || slot > WormholeSlotMax)
                return -1;
            //if (!StandardWormholes.Contains(mapid))
            //    return -1;

            switch (mapid)
            {
                case 256: return WormholeSlotsRed[slot];
                case 257: return WormholeSlotsGreen[slot];
                case 258: return WormholeSlotsYellow[slot];
                case 259: return WormholeSlotsBlue[slot];
                default: return -1;
            }
        }
    }
}