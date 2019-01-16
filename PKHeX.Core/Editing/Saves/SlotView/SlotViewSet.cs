using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    public class SlotViewSet
    {
        public readonly SlotView Box = SlotView.Empty;
        public readonly SlotView Party = SlotView.Empty;
        public readonly SlotView BattleBox = SlotView.Empty;
        public readonly SlotView Daycare = SlotView.Empty;
        public readonly SlotView[] Other;

        public readonly SaveFile SAV;

        private const int StackMax = 10;
        private readonly List<SlotChange> Changes = new List<SlotChange>(StackMax);

        public SlotViewSet(SaveFile sav, bool HaX = false)
        {
            SAV = sav;

            if (sav.HasBox)
                Box = new SlotBoxes(sav) { Type = StorageSlotType.Box };

            if (sav.HasParty)
                Party = GetPartyList(sav);

            if (sav.HasBattleBox)
                BattleBox = GetBattleBox(sav);

            if (sav.HasDaycare)
                Daycare = new SlotDaycare(sav, !sav.BattleBoxLocked);
            Other = GetOtherSlots(sav, HaX);

            SetChangeOutput();
        }

        private void SetChangeOutput()
        {
            foreach (var v in Viewers)
                v.Changes = Changes;
        }

        private static SlotView[] GetOtherSlots(SaveFile sav, bool HaX)
        {
            return sav.GetExtraSlots(HaX).GroupBy(z => z.Type)
                .Select(z => new SlotArray(sav, z.ToArray(), true) { Type = z.Key })
                .Cast<SlotView>().ToArray();
        }

        private static SlotList GetBattleBox(SaveFile sav)
        {
            var party = Enumerable.Range(0, 6).Select(sav.GetBattleBoxOffset)
                .Select(z => GetStorageDetails(z, false, StorageSlotType.BattleBox))
                .ToArray();
            return new SlotList(sav, party, !sav.BattleBoxLocked) { Type = StorageSlotType.BattleBox };
        }

        private static SlotList GetPartyList(SaveFile sav)
        {
            var party = Enumerable.Range(0, 6).Select(sav.GetPartyOffset)
                .Select(z => GetStorageDetails(z, true, StorageSlotType.Party))
                .ToArray();
            return new SlotList(sav, party, false) { Type = StorageSlotType.Party };
        }

        private static StorageSlotOffset GetStorageDetails(int z, bool party, StorageSlotType type)
        {
            return new StorageSlotOffset {IsPartyFormat = party, Offset = z, Type = type};
        }

        public IEnumerable<SlotView> Viewers
        {
            get
            {
                yield return Box;
                yield return Party;
                yield return BattleBox;
                yield return Daycare;
                foreach (var o in Other)
                    yield return o;
            }
        }

        public IEnumerable<PKM> AllPKM => Viewers.SelectMany(z => z).Where(z => z.Species > 0 && z.ChecksumValid);
    }
}