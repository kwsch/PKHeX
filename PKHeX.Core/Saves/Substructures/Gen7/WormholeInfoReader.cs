namespace PKHeX.Core
{
    public sealed class WormholeInfoReader
    {
        public readonly SAV7 SAV;
        public WormholeInfoReader(SAV7 sav) => SAV = sav;

        // Wormhole shininess & flags found by @PP-theSLAYER
        // https://projectpokemon.org/home/forums/topic/39433-gen-7-save-research-thread/?page=3&tab=comments#comment-239090
        public bool WormholeShininess // 0x4535 = Misc (0x4400 in USUM) + 0x0135
        {
            get => SAV.Data[SAV.Misc.Offset + 0x0135] == 1;
            set => SAV.Data[SAV.Misc.Offset + 0x0135] = value ? (byte)1 : (byte)0;
        }

        public const int WormholeSlotMax = 15;

        // Slots currently use digits 1 through 15 inclusively.
        // This follows the flag numbers used for slots.
        public int WormholeSlot
        {
            get
            {
                for (int i = 1; i <= WormholeSlotMax; i++)
                {
                    if (!SAV.GetEventFlag(i))
                        return i;
                }
                return -1;
            }
            set
            {
                if (value is < 1 or > WormholeSlotMax)
                    return;
                for (int i = 1; i <= WormholeSlotMax; i++)
                {
                    SAV.SetEventFlag(i, value != i);
                }
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
            -1, // filler used for indexing with slot number
            144, // Articuno
            145, // Zapdos
            146, // Moltres
            250, // Ho-Oh
            384, // Rayquaza
            488, // Cresselia
            641, // Tornadus
            642, // Thundurus
            645, // Landorus
            717, // Yveltal
            334, // Altaria
            469, // Yanmega
            561, // Sigilyph
            581, // Swanna
            277, // Swellow
        };

        public static readonly int[] WormholeSlotsGreen =
        {
            -1, // filler used for indexing with slot number
            150, // Mewtwo
            243, // Raikou
            244, // Entei
            483, // Dialga
            638, // Cobalion
            639, // Terrakion
            640, // Virizion
            643, // Reshiram
            644, // Zekrom
            716, // Xerneas
            452, // Drapion
            531, // Audino
            695, // Heliolisk
            274, // Nuzleaf
            326, // Grumpig
        };

        public static readonly int[] WormholeSlotsYellow =
        {
            -1, // filler used for indexing with slot number
            377, // Regirock
            378, // Regice
            379, // Registeel
            383, // Groudon
            485, // Heatran
            486, // Regigigas
            484, // Palkia
            487, // Giratina
            -1, // unused
            -1, // unused
            460, // Abomasnow
            308, // Medicham
            450, // Hippowdon
            558, // Crustle
            219, // Magcargo
        };

        public static readonly int[] WormholeSlotsBlue =
        {
            -1, // filler used for indexing with slot number
            245, // Suicune
            249, // Lugia
            380, // Latias
            381, // Latios
            382, // Kyogre
            480, // Uxie
            481, // Mesprit
            482, // Azelf
            646, // Kyurem
            -1, // unused
            689, // Barbaracle
            271, // Lombre
            618, // Stunfisk
            419, // Floatzel
            195, // Quagsire
        };

        public static int WormholeSlotToPokemon(int mapid, int slot)
        {
            if (slot is < 1 or > WormholeSlotMax)
                return -1;

            return mapid switch
            {
                256 => WormholeSlotsRed[slot],
                257 => WormholeSlotsGreen[slot],
                258 => WormholeSlotsYellow[slot],
                259 => WormholeSlotsBlue[slot],
                _ => -1
            };
        }
    }
}