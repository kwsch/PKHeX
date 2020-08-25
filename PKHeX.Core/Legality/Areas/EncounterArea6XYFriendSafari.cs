using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    public class EncounterArea6XYFriendSafari : EncounterArea
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

        public static ILookup<int, EncounterSlot6XY> GetArea()
        {
            var area = new EncounterArea6XYFriendSafari { Location = 148, Type = SlotType.FriendSafari };
            EncounterSlot6XY FriendSafariSlot(int d)
            {
                return new EncounterSlot6XY
                {
                    Area = area,
                    Species = d,
                    LevelMin = 30,
                    LevelMax = 30,
                    Form = 0,
                    Version = GameVersion.XY,
                };
            }
            area.Slots = Legal.FriendSafari.Select(FriendSafariSlot).ToArray();
            return area.Slots.Cast<EncounterSlot6XY>().ToLookup(s => s.Species);
        }

        public static IEnumerable<EncounterSlot> GetValidSafariEncounters(PKM pkm)
        {
            var chain = EvolutionChain.GetValidPreEvolutions(pkm);
            return GetValidSafariEncounters(chain);
        }

        public static IEnumerable<EncounterSlot> GetValidSafariEncounters(IReadOnlyList<DexLevel> chain)
        {
            var valid = chain.Where(d => d.Level >= 30);
            return valid.SelectMany(z => Encounters6.FriendSafari[z.Species]);
        }
    }
}