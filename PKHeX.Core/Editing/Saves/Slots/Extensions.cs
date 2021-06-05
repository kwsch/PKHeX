using System.Collections.Generic;

namespace PKHeX.Core
{
    public static partial class Extensions
    {
        public static IReadOnlyList<PKM> GetAllPKM(this SaveFile sav)
        {
            var result = new List<PKM>();
            if (sav.HasBox)
                result.AddRange(sav.BoxData);
            if (sav.HasParty)
                result.AddRange(sav.PartyData);

            var extra = sav.GetExtraPKM();
            result.AddRange(extra);
            result.RemoveAll(z => z.Species == 0);
            return result;
        }

        public static PKM[] GetExtraPKM(this SaveFile sav) => sav.GetExtraPKM(sav.GetExtraSlots());

        public static PKM[] GetExtraPKM(this SaveFile sav, IList<SlotInfoMisc> slots)
        {
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

        private static readonly List<SlotInfoMisc> None = new();

        private static List<SlotInfoMisc> GetExtraSlotsUnsafe(SaveFile sav, bool all) => sav switch
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

        private static List<SlotInfoMisc> GetExtraSlots2(SAV2 sav)
        {
            return new()
            {
                new SlotInfoMisc(sav.Data, 0, sav.GetDaycareSlotOffset(0, 2)) {Type = StorageSlotType.Daycare } // egg
            };
        }

        private static List<SlotInfoMisc> GetExtraSlots3(SAV3 sav)
        {
            if (sav is not SAV3FRLG)
                return None;
            return new List<SlotInfoMisc>
            {
                new(sav.Large, 0, 0x3C98) {Type = StorageSlotType.Daycare}
            };
        }

        private static List<SlotInfoMisc> GetExtraSlots4(SAV4 sav)
        {
            var list = new List<SlotInfoMisc>
            {
                new(sav.General, 0, sav.GTS) {Type = StorageSlotType.GTS},
            };
            if (sav is SAV4HGSS)
                list.Add(new SlotInfoMisc(sav.General, 1, SAV4HGSS.WalkerPair) {Type = StorageSlotType.Misc});
            return list;
        }

        private static List<SlotInfoMisc> GetExtraSlots5(SAV5 sav)
        {
            return new()
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
            return new()
            {
                new SlotInfoMisc(sav.Data, 0, sav.GTS) {Type = StorageSlotType.GTS},
                new SlotInfoMisc(sav.Data, 0, sav.Fused) {Type = StorageSlotType.Fused},
                new SlotInfoMisc(sav.Data, 0, sav.SUBE.Give) {Type = StorageSlotType.Misc}, // Old Man

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
            return new()
            {
                new SlotInfoMisc(sav.Data, 0, SAV6AO.GTS) {Type = StorageSlotType.GTS},
                new SlotInfoMisc(sav.Data, 0, SAV6AO.Fused) {Type = StorageSlotType.Fused},
                new SlotInfoMisc(sav.Data, 0, sav.SUBE.Give) {Type = StorageSlotType.Misc},

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
                new(sav.Data, 0, sav.AllBlocks[07].Offset) {Type = StorageSlotType.GTS},
                new(sav.Data, 0, sav.GetFusedSlotOffset(0)) {Type = StorageSlotType.Fused}
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
            var list = new List<SlotInfoMisc>
            {
                new(fused.Data, 0, Fused8.GetFusedSlotOffset(0), true) {Type = StorageSlotType.Fused},
                new(fused.Data, 1, Fused8.GetFusedSlotOffset(1), true) {Type = StorageSlotType.Fused},
                new(fused.Data, 2, Fused8.GetFusedSlotOffset(2), true) {Type = StorageSlotType.Fused},

                new(dc.Data, 0, Daycare8.GetDaycareSlotOffset(0, 0)) {Type = StorageSlotType.Daycare},
                new(dc.Data, 0, Daycare8.GetDaycareSlotOffset(0, 1)) {Type = StorageSlotType.Daycare},
                new(dc.Data, 0, Daycare8.GetDaycareSlotOffset(1, 0)) {Type = StorageSlotType.Daycare},
                new(dc.Data, 0, Daycare8.GetDaycareSlotOffset(1, 1)) {Type = StorageSlotType.Daycare},
            };

            if (sav is SAV8SWSH {SaveRevision: >= 2} s8)
            {
                var block = s8.Blocks.GetBlockSafe(SaveBlockAccessor8SWSH.KFusedCalyrex);
                var c = new SlotInfoMisc(block.Data, 3, 0, true) {Type = StorageSlotType.Fused};
                list.Insert(3, c);
            }

            return list;
        }
    }
}
