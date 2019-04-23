using System.Collections.Generic;

namespace PKHeX.Core
{
    public static partial class Extensions
    {
        public static PKM GetPKM(this SaveFile sav, StorageSlotOffset slot) => slot.IsPartyFormat
            ? sav.GetPartySlot(slot.Offset)
            : sav.GetStoredSlot(slot.Offset);

        public static void SetPKM(this SaveFile sav, StorageSlotOffset slot, PKM pkm)
        {
            if (slot.IsPartyFormat)
                sav.SetPartySlot(pkm, slot.Offset);
            else
                sav.SetStoredSlot(pkm, slot.Offset);
        }
        public static IReadOnlyList<PKM> GetAllPKM(this SaveFile sav)
        {
            var pkms = new List<PKM>();
            if (sav.HasBox)
                pkms.AddRange(sav.BoxData);
            if (sav.HasParty)
                pkms.AddRange(sav.PartyData);
            if (sav.HasBattleBox)
                pkms.AddRange(sav.BattleBoxData);

            var extra = sav.GetExtraPKM();
            pkms.AddRange(extra);
            pkms.RemoveAll(z => z.Species == 0);
            return pkms;
        }

        public static PKM[] GetExtraPKM(this SaveFile sav) => sav.GetExtraPKM(sav.GetExtraSlots());

        public static PKM[] GetExtraPKM(this SaveFile sav, IList<StorageSlotOffset> slots)
        {
            slots = slots ?? sav.GetExtraSlots();
            var arr = new PKM[slots.Count];
            for (int i = 0; i < slots.Count; i++)
                arr[i] = sav.GetPKM(slots[i]);
            return arr;
        }

        public static List<StorageSlotOffset> GetExtraSlots(this SaveFile sav, bool all = false)
        {
            var slots = GetExtraSlotsUnsafe(sav, all);
            for (int i = 0; i < slots.Count;)
            {
                if (slots[i].Offset < 0)
                    slots.RemoveAt(i);
                else
                    ++i;
            }
            return slots;
        }

        private static readonly List<StorageSlotOffset> None = new List<StorageSlotOffset>();

        private static List<StorageSlotOffset> GetExtraSlotsUnsafe(SaveFile sav, bool all)
        {
            switch (sav)
            {
                default: return None;
                case SAV2 sav2: return GetExtraSlots2(sav2);
                case SAV3 sav3: return GetExtraSlots3(sav3);
                case SAV4 sav4: return GetExtraSlots4(sav4);
                case SAV5 sav5: return GetExtraSlots5(sav5);
                case SAV6 sav6: return GetExtraSlots6(sav6);
                case SAV7 sav7: return GetExtraSlots7(sav7, all);
            }
        }

        private static List<StorageSlotOffset> GetExtraSlots2(SAV2 sav)
        {
            return new List<StorageSlotOffset>
            {
                new StorageSlotOffset {Type = StorageSlotType.Daycare, Offset = sav.GetDaycareSlotOffset(0, 2)} // egg
            };
        }

        private static List<StorageSlotOffset> GetExtraSlots3(SAV3 sav)
        {
            if (!sav.FRLG)
                return None;
            return new List<StorageSlotOffset>
            {
                new StorageSlotOffset {Type = StorageSlotType.Daycare, Offset = sav.GetBlockOffset(4) + 0xE18}
            };
        }

        private static List<StorageSlotOffset> GetExtraSlots4(SAV4 sav)
        {
            return new List<StorageSlotOffset>
            {
                new StorageSlotOffset {Type = StorageSlotType.GTS, Offset = sav.GTS},
            };
        }

        private static List<StorageSlotOffset> GetExtraSlots5(SAV5 sav)
        {
            return new List<StorageSlotOffset>
            {
                new StorageSlotOffset {Type = StorageSlotType.GTS, Offset = sav.GTS},
                new StorageSlotOffset {Type = StorageSlotType.Fused, Offset = sav.Fused}
            };
        }

        private static List<StorageSlotOffset> GetExtraSlots6(SAV6 sav)
        {
            if (sav.ORASDEMO)
                return None;
            var list = new List<StorageSlotOffset>
            {
                new StorageSlotOffset {Type = StorageSlotType.GTS, Offset = sav.GTS},
                new StorageSlotOffset {Type = StorageSlotType.Fused, Offset = sav.Fused}
            };
            if (sav.XY)
                list.Add(new StorageSlotOffset{Type = StorageSlotType.Misc, Offset = sav.SUBE});
            return list;
        }

        private static List<StorageSlotOffset> GetExtraSlots7(SAV7 sav, bool all)
        {
            var list = new List<StorageSlotOffset>
            {
                new StorageSlotOffset {Type = StorageSlotType.GTS, Offset = sav.GTS},
                new StorageSlotOffset {Type = StorageSlotType.Fused, Offset = sav.GetFusedSlotOffset(0)}
            };
            if (sav.USUM)
            {
                list.AddRange(new[]
               {
                    new StorageSlotOffset {Type = StorageSlotType.Fused, Offset = sav.GetFusedSlotOffset(1)},
                    new StorageSlotOffset {Type = StorageSlotType.Fused, Offset = sav.GetFusedSlotOffset(2)},
                });
            }

            if (!all)
                return list;

            for (int i = 0; i < SAV7.ResortCount; i++)
                list.Add(new StorageSlotOffset { Type = StorageSlotType.Resort, Offset = sav.GetResortSlotOffset(i) });
            return list;
        }
    }
}
