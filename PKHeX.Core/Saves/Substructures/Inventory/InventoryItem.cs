using System.Collections.Generic;

namespace PKHeX.Core
{
    public class InventoryItem
    {
        public bool New;
        public bool FreeSpace;
        public int Index, Count;
        public InventoryItem Clone() => (InventoryItem) MemberwiseClone();

        // Check Pouch Compatibility
        public bool Valid(IList<ushort> LegalItems, bool HaX, int MaxItemID)
        {
            if (Index == 0)
                return true;
            if ((uint) Index > MaxItemID)
                return false;
            return HaX || LegalItems.Contains((ushort)Index);
        }

        public void Clear()
        {
            Index = Count = 0;
            New = FreeSpace = false;
        }
    }
}
