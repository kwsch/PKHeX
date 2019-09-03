namespace PKHeX.Core
{
    /// <summary>
    /// Box Data <see cref="ISlotInfo"/>
    /// </summary>
    public sealed class SlotInfoBox : ISlotInfo
    {
        public int Box { get; }
        public int Slot { get; }
        public bool CanWriteTo(SaveFile sav) => sav.HasBox && !sav.IsSlotLocked(Box, Slot);
        public WriteBlockedMessage CanWriteTo(SaveFile sav, PKM pkm) => WriteBlockedMessage.None;

        public SlotInfoBox(int box, int slot)
        {
            Box = box;
            Slot = slot;
        }

        public bool WriteTo(SaveFile sav, PKM pkm, PKMImportSetting setting = PKMImportSetting.UseDefault)
        {
            sav.SetBoxSlotAtIndex(pkm, Box, Slot, setting, setting);
            return true;
        }

        public PKM Read(SaveFile sav) => sav.GetBoxSlotAtIndex(Box, Slot);

        private bool Equals(SlotInfoBox other) => Box == other.Box && Slot == other.Slot;
        public bool Equals(ISlotInfo other) => other is SlotInfoBox b && Equals(b);
        public override bool Equals(object obj) => obj is SlotInfoBox b && Equals(b);

        public override int GetHashCode() => (Box * 397) ^ Slot;
    }
}