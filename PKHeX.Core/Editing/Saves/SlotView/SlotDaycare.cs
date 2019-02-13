using System.Collections.Generic;

namespace PKHeX.Core
{
    public class SlotDaycare : SlotView
    {
        private readonly List<StorageSlotOffset> Slots = new List<StorageSlotOffset>();

        public SlotDaycare(SaveFile sav, bool locked)
            : base(sav, 2, locked)
        {
            ReloadDaycareSlots();
        }

        private void ReloadDaycareSlots()
        {
            var s1 = SAV.GetDaycareSlotOffset(CurrentDaycare, 0);
            if (s1 < 0)
                return;
            Slots.Add(new StorageSlotOffset { IsPartyFormat = false, Offset = s1, Type = StorageSlotType.Daycare });
            var s2 = SAV.GetDaycareSlotOffset(CurrentDaycare, 1);
            if (s2 < 0)
                return;
            Slots.Add(new StorageSlotOffset { IsPartyFormat = false, Offset = s2, Type = StorageSlotType.Daycare });
            Capacity = Slots.Count;
        }

        public int CurrentDaycare { get; private set; }
        public bool CanSwitch => SAV.HasTwoDaycares;

        public void SwitchCurrent()
        {
            CurrentDaycare ^= 1;
            ReloadDaycareSlots();
        }

        protected override int GetOffset(int index) => Slots[index].Offset;
    }
}