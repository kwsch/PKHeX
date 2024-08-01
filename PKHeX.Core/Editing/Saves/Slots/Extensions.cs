using System;
using System.Collections.Generic;

namespace PKHeX.Core;

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

    public static PKM[] GetExtraPKM(this SaveFile sav, IReadOnlyList<SlotInfoMisc> slots)
    {
        var arr = new PKM[slots.Count];
        for (int i = 0; i < slots.Count; i++)
            arr[i] = slots[i].Read(sav);
        return arr;
    }

    private static readonly List<SlotInfoMisc> None = [];

    public static List<SlotInfoMisc> GetExtraSlots(this SaveFile sav, bool all = false) => sav switch
    {
        SAV2 sav2 => GetExtraSlots2(sav2),
        SAV3 sav3 => GetExtraSlots3(sav3),
        SAV4 sav4 => GetExtraSlots4(sav4),
        SAV5 sav5 => GetExtraSlots5(sav5),
        SAV6XY xy => GetExtraSlots6XY(xy),
        SAV6AO xy => GetExtraSlots6AO(xy),
        SAV7 sav7 => GetExtraSlots7(sav7, all),
        SAV7b lgpe => GetExtraSlots7b(lgpe),
        SAV8SWSH ss => GetExtraSlots8(ss),
        SAV8BS bs => GetExtraSlots8b(bs),
        SAV8LA la => GetExtraSlots8a(la),
        SAV9SV sv => GetExtraSlots9(sv),
        _ => None,
    };

    private static List<SlotInfoMisc> GetExtraSlots2(SAV2 sav)
    {
        return
        [
            new(sav.GetDaycareEgg(), 0) {Type = StorageSlotType.Daycare }, // egg
        ];
    }

    private static List<SlotInfoMisc> GetExtraSlots3(SAV3 sav)
    {
        if (sav is not SAV3FRLG)
            return None;
        return
        [
            new(sav.Large.AsMemory(0x3C98), 0) {Type = StorageSlotType.Daycare},
        ];
    }

    private static List<SlotInfoMisc> GetExtraSlots4(SAV4 sav)
    {
        var list = new List<SlotInfoMisc>();
        if (sav.GTS > 0)
            list.Add(new SlotInfoMisc(sav.GeneralBuffer[sav.GTS..], 0) { Type = StorageSlotType.GTS });
        if (sav is SAV4HGSS hgss)
            list.Add(new SlotInfoMisc(hgss.GeneralBuffer[SAV4HGSS.WalkerPair..], 1) {Type = StorageSlotType.Misc});
        return list;
    }

    private static List<SlotInfoMisc> GetExtraSlots5(SAV5 sav)
    {
        var list = new List<SlotInfoMisc>
        {
            new(sav.GTS.Upload, 0) {Type = StorageSlotType.GTS},
            new(sav.GlobalLink.Upload, 0) { Type = StorageSlotType.Misc },

            new(sav.BattleBox[0], 0) {Type = StorageSlotType.BattleBox},
            new(sav.BattleBox[1], 1) {Type = StorageSlotType.BattleBox},
            new(sav.BattleBox[2], 2) {Type = StorageSlotType.BattleBox},
            new(sav.BattleBox[3], 3) {Type = StorageSlotType.BattleBox},
            new(sav.BattleBox[4], 4) {Type = StorageSlotType.BattleBox},
            new(sav.BattleBox[5], 5) {Type = StorageSlotType.BattleBox},
        };

        if (sav is SAV5B2W2 b2w2)
            list.Insert(1, new(b2w2.Forest.Fused, 0) { Type = StorageSlotType.FusedKyurem });

        return list;
    }

    private static List<SlotInfoMisc> GetExtraSlots6XY(SAV6XY sav)
    {
        return
        [
            new(sav.GTS.Upload, 0) {Type = StorageSlotType.GTS},
            new(sav.Fused[0], 0) {Type = StorageSlotType.FusedKyurem},
            new(sav.SUBE.GiveSlot, 0) {Type = StorageSlotType.Misc}, // Old Man

            new(sav.BattleBox[0], 0) {Type = StorageSlotType.BattleBox},
            new(sav.BattleBox[1], 1) {Type = StorageSlotType.BattleBox},
            new(sav.BattleBox[2], 2) {Type = StorageSlotType.BattleBox},
            new(sav.BattleBox[3], 3) {Type = StorageSlotType.BattleBox},
            new(sav.BattleBox[4], 4) {Type = StorageSlotType.BattleBox},
            new(sav.BattleBox[5], 5) {Type = StorageSlotType.BattleBox},
        ];
    }

    private static List<SlotInfoMisc> GetExtraSlots6AO(SAV6AO sav)
    {
        return
        [
            new(sav.GTS.Upload, 0) { Type = StorageSlotType.GTS },
            new(sav.Fused[0], 0) { Type = StorageSlotType.FusedKyurem },
            new(sav.SUBE.GiveSlot, 0) {Type = StorageSlotType.Misc},

            new(sav.BattleBox[0], 0) {Type = StorageSlotType.BattleBox},
            new(sav.BattleBox[1], 1) {Type = StorageSlotType.BattleBox},
            new(sav.BattleBox[2], 2) {Type = StorageSlotType.BattleBox},
            new(sav.BattleBox[3], 3) {Type = StorageSlotType.BattleBox},
            new(sav.BattleBox[4], 4) {Type = StorageSlotType.BattleBox},
            new(sav.BattleBox[5], 5) {Type = StorageSlotType.BattleBox},
        ];
    }

    private static List<SlotInfoMisc> GetExtraSlots7(SAV7 sav, bool all)
    {
        var list = new List<SlotInfoMisc>
        {
            new(sav.GTS.Upload, 0) {Type = StorageSlotType.GTS},
            new(sav.Fused[0], 0, PartyFormat: true) {Type = StorageSlotType.FusedKyurem},
        };
        if (sav is SAV7USUM uu)
        {
            list.AddRange(
            [
                new SlotInfoMisc(uu.Fused[1], 1, PartyFormat: true) {Type = StorageSlotType.FusedNecrozmaS},
                new SlotInfoMisc(uu.Fused[2], 2, PartyFormat: true) {Type = StorageSlotType.FusedNecrozmaM},
            ]);
            list.AddRange(
            [
                new SlotInfoMisc(uu.BattleAgency[0], 0) {Type = StorageSlotType.Misc},
                new SlotInfoMisc(uu.BattleAgency[1], 1) {Type = StorageSlotType.Misc},
                new SlotInfoMisc(uu.BattleAgency[2], 2) {Type = StorageSlotType.Misc},
            ]);
        }

        if (!all)
            return list;

        for (int i = 0; i < ResortSave7.ResortCount; i++)
            list.Add(new SlotInfoMisc(sav.ResortSave[i], i) { Type = StorageSlotType.Resort });
        return list;
    }

    private static List<SlotInfoMisc> GetExtraSlots7b(ISaveBlock7b sav)
    {
        return
        [
            new(sav.Daycare.Stored, 0) {Type = StorageSlotType.Daycare},
        ];
    }

    private static List<SlotInfoMisc> GetExtraSlots8(ISaveBlock8Main sav)
    {
        var fused = sav.Fused;
        var dc = sav.Daycare;
        var list = new List<SlotInfoMisc>
        {
            new(fused[0], 0, true) {Type = StorageSlotType.FusedKyurem},
            new(fused[1], 1, true) {Type = StorageSlotType.FusedNecrozmaS},
            new(fused[2], 2, true) {Type = StorageSlotType.FusedNecrozmaM},

            new(dc[0], 0) {Type = StorageSlotType.Daycare},
            new(dc[1], 1) {Type = StorageSlotType.Daycare},
            new(dc[2], 2) {Type = StorageSlotType.Daycare},
            new(dc[3], 3) {Type = StorageSlotType.Daycare},
        };

        if (sav is SAV8SWSH {SaveRevision: >= 2} s8)
        {
            var block = s8.Blocks.GetBlockSafe(SaveBlockAccessor8SWSH.KFusedCalyrex);
            var c = new SlotInfoMisc(block.Data, 3, true) {Type = StorageSlotType.FusedCalyrex};
            list.Insert(3, c);
        }

        return list;
    }

    private static List<SlotInfoMisc> GetExtraSlots8b(SAV8BS sav)
    {
        return
        [
            new(sav.UgSaveData[0], 0, true) { Type = StorageSlotType.Misc },
            new(sav.UgSaveData[1], 1, true) { Type = StorageSlotType.Misc },
            new(sav.UgSaveData[2], 2, true) { Type = StorageSlotType.Misc },
            new(sav.UgSaveData[3], 3, true) { Type = StorageSlotType.Misc },
            new(sav.UgSaveData[4], 4, true) { Type = StorageSlotType.Misc },
            new(sav.UgSaveData[5], 5, true) { Type = StorageSlotType.Misc },
            new(sav.UgSaveData[6], 6, true) { Type = StorageSlotType.Misc },
            new(sav.UgSaveData[7], 7, true) { Type = StorageSlotType.Misc },
            new(sav.UgSaveData[8], 8, true) { Type = StorageSlotType.Misc },
        ];
    }

    private static List<SlotInfoMisc> GetExtraSlots8a(SAV8LA _)
    {
        return [];
    }

    private static List<SlotInfoMisc> GetExtraSlots9(SAV9SV sav)
    {
        var afterBox = sav.GetBoxOffset(BoxLayout9.BoxCount);
        var list = new List<SlotInfoMisc>
        {
            // Ride Legend
            new(sav.BoxInfo.Raw.Slice(afterBox, PokeCrypto.SIZE_9PARTY), 0, true, Mutable: true) { Type = StorageSlotType.Ride },
        };

        var block = sav.Blocks.GetBlock(SaveBlockAccessor9SV.KFusedCalyrex);
        list.Add(new(block.Data, 0, true) { Type = StorageSlotType.FusedCalyrex });

        if (sav.Blocks.TryGetBlock(SaveBlockAccessor9SV.KFusedKyurem, out var kyurem))
            list.Add(new(kyurem.Data, 1, true) { Type = StorageSlotType.FusedKyurem });
        if (sav.Blocks.TryGetBlock(SaveBlockAccessor9SV.KFusedNecrozmaS, out var solgaleo))
            list.Add(new(solgaleo.Data, 2, true) { Type = StorageSlotType.FusedNecrozmaS });
        if (sav.Blocks.TryGetBlock(SaveBlockAccessor9SV.KFusedNecrozmaM, out var lunala))
            list.Add(new(lunala.Data, 3, true) { Type = StorageSlotType.FusedNecrozmaM });

        if (sav.Blocks.TryGetBlock(SaveBlockAccessor9SV.KSurpriseTrade, out var surprise))
        {
            var st = surprise.Data.AsMemory();
            list.Add(new(st[0x198..], 0) { Type = StorageSlotType.Misc }); // my upload
            list.Add(new(st[0x2C..], 1) { Type = StorageSlotType.Misc }); // received from others
        }
        return list;
    }
}
