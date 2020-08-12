using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    public static class EncounterArea6XYFriendSafari
    {
        public static bool WasFriendSafari(PKM pkm)
        {
            if (!pkm.XY)
                return false;
            if (pkm.Met_Location != 148)
                return false;
            if (pkm.Met_Level != 30)
                return false;
            if (pkm.Egg_Location != 0)
                return false;
            return true;
        }

        public static IEnumerable<EncounterSlot> GetValidFriendSafari(PKM pkm)
        {
            var chain = EvolutionChain.GetValidPreEvolutions(pkm);
            return GetValidFriendSafari(chain);
        }

        public static IEnumerable<EncounterSlot> GetValidFriendSafari(IReadOnlyList<DexLevel> chain)
        {
            var valid = chain.Where(d => d.Level >= 30);
            return valid.SelectMany(z => Encounters6.FriendSafari[z.Species]);
        }
    }
}