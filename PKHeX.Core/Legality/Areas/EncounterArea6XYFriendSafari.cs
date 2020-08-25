using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    public class EncounterArea6XYFriendSafari : EncounterArea
    {
        public EncounterArea6XYFriendSafari(ICollection<int> species)
        {
            Location = 148;
            Type = SlotType.FriendSafari;

            var slots = new EncounterSlot6XY[species.Count];
            int ctr = 0;
            foreach (var s in species)
                slots[ctr++] = new EncounterSlot6XY(this, s, 0, 30, 30, GameVersion.XY);
            Slots = slots;
        }

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

        public override IEnumerable<EncounterSlot> GetMatchingSlots(PKM pkm, IReadOnlyList<EvoCriteria> chain)
        {
            if (!WasFriendSafari(pkm))
                return Array.Empty<EncounterSlot>();
            return base.GetMatchingSlots(pkm, chain);
        }
    }
}