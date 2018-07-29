using System.Linq;

namespace PKHeX.Core
{
    public class InventoryItem
    {
        public bool New;
        public bool FreeSpace;
        public int Index, Count;
        public InventoryItem Clone() => (InventoryItem) MemberwiseClone();

        // Check Pouch Compatibility
        public bool Valid(ushort[] LegalItems, bool HaX, int MaxItemID)
        {
            if (Index == 0)
                return true;
            if (Index <= MaxItemID)
                return HaX || LegalItems.Contains((ushort)Index);
            return false;
        }
    }
}
