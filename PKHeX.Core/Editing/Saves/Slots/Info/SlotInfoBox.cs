namespace PKHeX.Core
{
    /// <summary>
    /// Box Data <see cref="ISlotInfo"/>
    /// </summary>
    public sealed record SlotInfoBox(int Box, int Slot) : ISlotInfo
    {
        public SlotOrigin Origin => SlotOrigin.Box;
        public bool CanWriteTo(SaveFile sav) => sav.HasBox && !sav.IsSlotLocked(Box, Slot);
        public WriteBlockedMessage CanWriteTo(SaveFile sav, PKM pkm) => WriteBlockedMessage.None;

        public bool WriteTo(SaveFile sav, PKM pkm, PKMImportSetting setting = PKMImportSetting.UseDefault)
        {
            sav.SetBoxSlotAtIndex(pkm, Box, Slot, setting, setting);
            return true;
        }

        public PKM Read(SaveFile sav) => sav.GetBoxSlotAtIndex(Box, Slot);
    }
}
