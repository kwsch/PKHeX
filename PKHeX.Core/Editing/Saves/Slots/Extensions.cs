using System.Collections.Generic;

namespace PKHeX.Core
{
    public static partial class Extensions
    {
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

        public static PKM[] GetExtraPKM(this SaveFile sav, IList<SlotInfoMisc> slots)
        {
            slots ??= sav.GetExtraSlots();
            var arr = new PKM[slots.Count];
            for (int i = 0; i < slots.Count; i++)
                arr[i] = slots[i].Read(sav);
            return arr;
        }

        public static List<SlotInfoMisc> GetExtraSlots(this SaveFile sav, bool all = false)
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

        private static readonly List<SlotInfoMisc> None = new List<SlotInfoMisc>();

        private static List<SlotInfoMisc> GetExtraSlotsUnsafe(SaveFile sav, bool all)
        {
            return sav switch
            {
                SAV2 sav2 => GetExtraSlots2(sav2),
                SAV3 sav3 => GetExtraSlots3(sav3),
                SAV4 sav4 => GetExtraSlots4(sav4),
                SAV5 sav5 => GetExtraSlots5(sav5),
                SAV6XY xy => GetExtraSlots6XY(xy),
                SAV6AO xy => GetExtraSlots6AO(xy),
                SAV7 sav7 => GetExtraSlots7(sav7, all),
                _ => None
            };
        }

        private static List<SlotInfoMisc> GetExtraSlots2(SAV2 sav)
        {
            return new List<SlotInfoMisc>
            {
                new SlotInfoMisc(0, sav.GetDaycareSlotOffset(0, 2)) {Type = StorageSlotType.Daycare } // egg
            };
        }

        private static List<SlotInfoMisc> GetExtraSlots3(SAV3 sav)
        {
            if (!sav.FRLG)
                return None;
            return new List<SlotInfoMisc>
            {
                new SlotInfoMisc(0, sav.GetBlockOffset(4) + 0xE18) {Type = StorageSlotType.Daycare }
            };
        }

        private static List<SlotInfoMisc> GetExtraSlots4(SAV4 sav)
        {
            return new List<SlotInfoMisc>
            {
                new SlotInfoMisc(0, sav.GTS) {Type = StorageSlotType.GTS },
            };
        }

        private static List<SlotInfoMisc> GetExtraSlots5(SAV5 sav)
        {
            return new List<SlotInfoMisc>
            {
                new SlotInfoMisc(0, sav.GTS) {Type = StorageSlotType.GTS},
                new SlotInfoMisc(0, sav.Fused) {Type = StorageSlotType.Fused}
            };
        }

        private static List<SlotInfoMisc> GetExtraSlots6XY(SAV6XY sav)
        {
            return new List<SlotInfoMisc>
            {
                new SlotInfoMisc(0, sav.GTS) {Type = StorageSlotType.GTS},
                new SlotInfoMisc(0, sav.Fused) {Type = StorageSlotType.Fused},
                new SlotInfoMisc(0, sav.SUBE) {Type = StorageSlotType.Misc},
            };
        }

        private static List<SlotInfoMisc> GetExtraSlots6AO(SAV6AO sav)
        {
            return new List<SlotInfoMisc>
            {
                new SlotInfoMisc(0, sav.GTS) {Type = StorageSlotType.GTS},
                new SlotInfoMisc(0, sav.Fused) {Type = StorageSlotType.Fused}
            };
        }

        private static List<SlotInfoMisc> GetExtraSlots7(SAV7 sav, bool all)
        {
            var list = new List<SlotInfoMisc>
            {
                new SlotInfoMisc(0, sav.GTS) {Type = StorageSlotType.GTS},
                new SlotInfoMisc(0, sav.GetFusedSlotOffset(0)) {Type = StorageSlotType.Fused}
            };
            if (sav is SAV7USUM)
            {
                list.AddRange(new[]
               {
                    new SlotInfoMisc(1, sav.GetFusedSlotOffset(1)) {Type = StorageSlotType.Fused},
                    new SlotInfoMisc(2, sav.GetFusedSlotOffset(2)) {Type = StorageSlotType.Fused},
                });
            }

            if (!all)
                return list;

            for (int i = 0; i < ResortSave7.ResortCount; i++)
                list.Add(new SlotInfoMisc(i, sav.ResortSave.GetResortSlotOffset(i)) { Type = StorageSlotType.Resort });
            return list;
        }
    }
}
