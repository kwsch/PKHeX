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
                SAV8SWSH ss => GetExtraSlots8(ss),
                _ => None
            };
        }

        private static List<SlotInfoMisc> GetExtraSlots2(SAV2 sav)
        {
            return new List<SlotInfoMisc>
            {
                new SlotInfoMisc(sav.Data, 0, sav.GetDaycareSlotOffset(0, 2)) {Type = StorageSlotType.Daycare } // egg
            };
        }

        private static List<SlotInfoMisc> GetExtraSlots3(SAV3 sav)
        {
            if (!sav.FRLG)
                return None;
            return new List<SlotInfoMisc>
            {
                new SlotInfoMisc(sav.Data, 0, sav.GetBlockOffset(4) + 0xE18) {Type = StorageSlotType.Daycare }
            };
        }

        private static List<SlotInfoMisc> GetExtraSlots4(SAV4 sav)
        {
            return new List<SlotInfoMisc>
            {
                new SlotInfoMisc(sav.General, 0, sav.GTS) {Type = StorageSlotType.GTS },
            };
        }

        private static List<SlotInfoMisc> GetExtraSlots5(SAV5 sav)
        {
            return new List<SlotInfoMisc>
            {
                new SlotInfoMisc(sav.Data, 0, sav.GTS) {Type = StorageSlotType.GTS},
                new SlotInfoMisc(sav.Data, 0, sav.Fused) {Type = StorageSlotType.Fused},

                new SlotInfoMisc(sav.Data, 0, sav.GetBattleBoxSlot(0)) {Type = StorageSlotType.BattleBox},
                new SlotInfoMisc(sav.Data, 1, sav.GetBattleBoxSlot(1)) {Type = StorageSlotType.BattleBox},
                new SlotInfoMisc(sav.Data, 2, sav.GetBattleBoxSlot(2)) {Type = StorageSlotType.BattleBox},
                new SlotInfoMisc(sav.Data, 3, sav.GetBattleBoxSlot(3)) {Type = StorageSlotType.BattleBox},
                new SlotInfoMisc(sav.Data, 4, sav.GetBattleBoxSlot(4)) {Type = StorageSlotType.BattleBox},
                new SlotInfoMisc(sav.Data, 5, sav.GetBattleBoxSlot(5)) {Type = StorageSlotType.BattleBox},
            };
        }

        private static List<SlotInfoMisc> GetExtraSlots6XY(SAV6XY sav)
        {
            return new List<SlotInfoMisc>
            {
                new SlotInfoMisc(sav.Data, 0, sav.GTS) {Type = StorageSlotType.GTS},
                new SlotInfoMisc(sav.Data, 0, sav.Fused) {Type = StorageSlotType.Fused},
                new SlotInfoMisc(sav.Data, 0, sav.SUBE.Offset + 0x90) {Type = StorageSlotType.Misc}, // Old Man

                new SlotInfoMisc(sav.Data, 0, sav.GetBattleBoxSlot(0)) {Type = StorageSlotType.BattleBox},
                new SlotInfoMisc(sav.Data, 1, sav.GetBattleBoxSlot(1)) {Type = StorageSlotType.BattleBox},
                new SlotInfoMisc(sav.Data, 2, sav.GetBattleBoxSlot(2)) {Type = StorageSlotType.BattleBox},
                new SlotInfoMisc(sav.Data, 3, sav.GetBattleBoxSlot(3)) {Type = StorageSlotType.BattleBox},
                new SlotInfoMisc(sav.Data, 4, sav.GetBattleBoxSlot(4)) {Type = StorageSlotType.BattleBox},
                new SlotInfoMisc(sav.Data, 5, sav.GetBattleBoxSlot(5)) {Type = StorageSlotType.BattleBox},
            };
        }

        private static List<SlotInfoMisc> GetExtraSlots6AO(SAV6AO sav)
        {
            return new List<SlotInfoMisc>
            {
                new SlotInfoMisc(sav.Data, 0, sav.GTS) {Type = StorageSlotType.GTS},
                new SlotInfoMisc(sav.Data, 0, sav.Fused) {Type = StorageSlotType.Fused},

                new SlotInfoMisc(sav.Data, 0, sav.GetBattleBoxSlot(0)) {Type = StorageSlotType.BattleBox},
                new SlotInfoMisc(sav.Data, 1, sav.GetBattleBoxSlot(1)) {Type = StorageSlotType.BattleBox},
                new SlotInfoMisc(sav.Data, 2, sav.GetBattleBoxSlot(2)) {Type = StorageSlotType.BattleBox},
                new SlotInfoMisc(sav.Data, 3, sav.GetBattleBoxSlot(3)) {Type = StorageSlotType.BattleBox},
                new SlotInfoMisc(sav.Data, 4, sav.GetBattleBoxSlot(4)) {Type = StorageSlotType.BattleBox},
                new SlotInfoMisc(sav.Data, 5, sav.GetBattleBoxSlot(5)) {Type = StorageSlotType.BattleBox},
            };
        }

        private static List<SlotInfoMisc> GetExtraSlots7(SAV7 sav, bool all)
        {
            var list = new List<SlotInfoMisc>
            {
                new SlotInfoMisc(sav.Data, 0, sav.AllBlocks[07].Offset) {Type = StorageSlotType.GTS},
                new SlotInfoMisc(sav.Data, 0, sav.GetFusedSlotOffset(0)) {Type = StorageSlotType.Fused}
            };
            if (sav is SAV7USUM)
            {
                list.AddRange(new[]
               {
                    new SlotInfoMisc(sav.Data, 1, sav.GetFusedSlotOffset(1)) {Type = StorageSlotType.Fused},
                    new SlotInfoMisc(sav.Data, 2, sav.GetFusedSlotOffset(2)) {Type = StorageSlotType.Fused},
                });
            }

            if (!all)
                return list;

            for (int i = 0; i < ResortSave7.ResortCount; i++)
                list.Add(new SlotInfoMisc(sav.Data, i, sav.ResortSave.GetResortSlotOffset(i)) { Type = StorageSlotType.Resort });
            return list;
        }

        private static List<SlotInfoMisc> GetExtraSlots8(ISaveBlock8Main sav)
        {
            var fused = sav.Fused;
            var dc = sav.Daycare;
            return new List<SlotInfoMisc>
            {
                new SlotInfoMisc(fused.Data, 0, Fused8.GetFusedSlotOffset(0), true),
                new SlotInfoMisc(fused.Data, 0, Fused8.GetFusedSlotOffset(0), true),
                new SlotInfoMisc(fused.Data, 0, Fused8.GetFusedSlotOffset(0), true),

                new SlotInfoMisc(dc.Data, 0, Daycare8.GetDaycareSlotOffset(0, 0)),
                new SlotInfoMisc(dc.Data, 0, Daycare8.GetDaycareSlotOffset(0, 1)),
                new SlotInfoMisc(dc.Data, 0, Daycare8.GetDaycareSlotOffset(1, 0)),
                new SlotInfoMisc(dc.Data, 0, Daycare8.GetDaycareSlotOffset(1, 1)),
            };
        }
    }
}
