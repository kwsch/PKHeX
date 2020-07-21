namespace PKHeX.Core
{
    public abstract class MyItem : SaveBlock
    {
        public abstract InventoryPouch[] Inventory { get; set; }
        protected MyItem(SaveFile SAV) : base(SAV) { }
        protected MyItem(SaveFile SAV, byte[] data) : base(SAV, data) { }
    }
}