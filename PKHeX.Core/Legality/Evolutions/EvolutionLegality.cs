using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    internal static class EvolutionLegality
    {
        internal static readonly HashSet<int> FutureEvolutionsGen1 = new()
        {
            (int)Species.Crobat,
            (int)Species.Bellossom,
            (int)Species.Politoed,
            (int)Species.Espeon,
            (int)Species.Umbreon,
            (int)Species.Slowking,
            (int)Species.Steelix,
            (int)Species.Scizor,
            (int)Species.Kingdra,
            (int)Species.Porygon2,
            (int)Species.Blissey,

            (int)Species.Magnezone,
            (int)Species.Lickilicky,
            (int)Species.Rhyperior,
            (int)Species.Tangrowth,
            (int)Species.Electivire,
            (int)Species.Magmortar,
            (int)Species.Leafeon,
            (int)Species.Glaceon,
            (int)Species.PorygonZ,

            (int)Species.Sylveon,
        };

        private static readonly HashSet<int> FutureEvolutionsGen2 = new()
        {
            (int)Species.Ambipom,
            (int)Species.Mismagius,
            (int)Species.Honchkrow,
            (int)Species.Weavile,
            (int)Species.Magnezone,
            (int)Species.Lickilicky,
            (int)Species.Rhyperior,
            (int)Species.Tangrowth,
            (int)Species.Electivire,
            (int)Species.Magmortar,
            (int)Species.Togekiss,
            (int)Species.Yanmega,
            (int)Species.Leafeon,
            (int)Species.Glaceon,
            (int)Species.Gliscor,
            (int)Species.Mamoswine,
            (int)Species.PorygonZ,

            (int)Species.Sylveon,
        };

        private static readonly HashSet<int> FutureEvolutionsGen3 = new()
        {
            (int)Species.Roserade,
            (int)Species.Ambipom,
            (int)Species.Mismagius,
            (int)Species.Honchkrow,
            (int)Species.Weavile,
            (int)Species.Magnezone,
            (int)Species.Lickilicky,
            (int)Species.Rhyperior,
            (int)Species.Tangrowth,
            (int)Species.Electivire,
            (int)Species.Magmortar,
            (int)Species.Togekiss,
            (int)Species.Yanmega,
            (int)Species.Leafeon,
            (int)Species.Glaceon,
            (int)Species.Gliscor,
            (int)Species.Mamoswine,
            (int)Species.PorygonZ,
            (int)Species.Gallade,
            (int)Species.Probopass,
            (int)Species.Dusknoir,
            (int)Species.Froslass,

            (int)Species.Sylveon,
        };

        private static readonly int[] FutureEvolutionsGen4 =
        {
            (int)Species.Sylveon,
        };

        private static readonly int[] FutureEvolutionsGen5 = FutureEvolutionsGen4;

        internal static ICollection<int> GetFutureGenEvolutions(int generation) => generation switch
        {
            1 => FutureEvolutionsGen1,
            2 => FutureEvolutionsGen2,
            3 => FutureEvolutionsGen3,
            4 => FutureEvolutionsGen4,
            5 => FutureEvolutionsGen5,
            _ => Array.Empty<int>()
        };
    }
}
