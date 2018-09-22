using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    public static class EncounterLinkGenerator
    {
        public static IEnumerable<EncounterLink> GetPossible(PKM pkm)
        {
            if (!pkm.Gen6)
                return Enumerable.Empty<EncounterLink>();
            return Encounters6.LinkGifts6.Where(g => g.Species == pkm.Species);
        }

        public static IEnumerable<EncounterLink> GetPossible(PKM pkm, IReadOnlyList<DexLevel> vs)
        {
            if (!pkm.Gen6)
                return Enumerable.Empty<EncounterLink>();
            return Encounters6.LinkGifts6.Where(g => vs.Any(z => z.Species == g.Species));
        }

        public static IEnumerable<EncounterLink> GetValidLinkGifts(PKM pkm)
        {
            return GetPossible(pkm).Where(g => g.Level == pkm.Met_Level);
        }
    }
}
