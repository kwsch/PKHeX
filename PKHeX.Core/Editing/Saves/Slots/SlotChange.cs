namespace PKHeX.Core
{
    public class SlotChange : StorageSlotOffset
    {
        /// <summary> Parent of the object that initiated the slot change. </summary>
        public object Parent { get; set; }

        public int Slot { get; set; } = -1;
        public int Box { get; set; } = -1;
        public PKM PKM { get; set; }

        public bool IsParty => IsPartyFormat;
        public bool IsValid => Slot > -1 && (Box > -1 || IsParty);
        public bool Editable { get; set; }

        public SlotChange() { }

        public SlotChange(SlotChange info, SaveFile sav)
        {
            Box = info.Box;
            Slot = info.Slot;
            Offset = info.Offset;
            PKM = sav.GetStoredSlot(info.Offset);
        }

        public SlotChange GetInverseData(SaveFile sav) => new SlotChange(this, sav);
    }
}