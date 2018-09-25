namespace PKHeX.Core
{
    public class StorageSlotOffset
    {
        public int Offset { get; set; } = -1;
        public bool IsPartyFormat { get; set; } = false;
        public StorageSlotType Type { get; set; } = StorageSlotType.Misc;
    }
}