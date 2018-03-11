using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    public static class EncounterLinkGenerator
    {
        // EncounterLink
        public static IEnumerable<EncounterLink> GetValidLinkGifts(PKM pkm)
        {
            if (pkm.GenNumber != 6)
                return Enumerable.Empty<EncounterLink>();
            var gifts = Encounters6.LinkGifts6.Where(g => g.Species == pkm.Species);
            return gifts.Where(g => g.Level == pkm.Met_Level);
        }
    }
}
