namespace PKHeX.Core
{
    /// <summary>
    /// Miscellaneous origination <see cref="ISlotInfo"/>
    /// </summary>
    public sealed class SlotInfoMisc : ISlotInfo
    {
        public int Slot { get; }
        public bool PartyFormat { get; }
        public int Offset { get; }
        public bool CanWriteTo(SaveFile sav) => false;
        public WriteBlockedMessage CanWriteTo(SaveFile sav, PKM pkm) => WriteBlockedMessage.InvalidDestination;
        public StorageSlotType Type { get; set; }

        public SlotInfoMisc(int slot, int offset, bool party = false)
        {
            Slot = slot;
            Offset = offset;
            PartyFormat = party;
        }

        public bool WriteTo(SaveFile sav, PKM pkm, PKMImportSetting setting = PKMImportSetting.UseDefault)
        {
            if (PartyFormat)
                sav.SetPartySlot(pkm, Offset, setting, setting);
            else
                sav.SetStoredSlot(pkm, Offset, setting, setting);
            return true;
        }

        public PKM Read(SaveFile sav)
        {
            return PartyFormat ? sav.GetPartySlot(Offset) : sav.GetStoredSlot(Offset);
        }

        private bool Equals(SlotInfoMisc other) => Offset == other.Offset;
        public bool Equals(ISlotInfo other) => other is SlotInfoMisc p && Equals(p);
        public override bool Equals(object obj) => obj is SlotInfoMisc p && Equals(p);
        public override int GetHashCode() => Offset;
    }
}