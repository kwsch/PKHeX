using System.Collections.Generic;

namespace PKHeX.Core
{
    public abstract class MyItem : SaveBlock
    {
        public abstract IReadOnlyList<InventoryPouch> Inventory { get; set; }
        protected MyItem(SaveFile SAV) : base(SAV) { }
        protected MyItem(SaveFile SAV, byte[] data) : base(SAV, data) { }
    }
}