using System;
using System.Collections.Generic;

namespace PKHeX.Core;

internal static class EvolutionLegality
{
    internal static readonly HashSet<ushort> FutureEvolutionsGen1 = new()
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

        (int)Species.Kleavor,
    };
}
