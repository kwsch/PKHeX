namespace PKHeX.Core
{
    /// <summary>
    /// Miscellaneous origination <see cref="ISlotInfo"/>
    /// </summary>
    public sealed record SlotInfoMisc(byte[] Data, int Slot, int Offset, bool PartyFormat = false) : ISlotInfo
    {
        public SlotOrigin Origin => PartyFormat ? SlotOrigin.Party : SlotOrigin.Box;
        public bool CanWriteTo(SaveFile sav) => false;
        public WriteBlockedMessage CanWriteTo(SaveFile sav, PKM pkm) => WriteBlockedMessage.InvalidDestination;
        public StorageSlotType Type { get; init; }

        public SlotInfoMisc(SaveFile sav, int slot, int offset, bool party = false) : this(GetBuffer(sav), slot, offset, party) { }

        private static byte[] GetBuffer(SaveFile sav) => sav switch
        {
            SAV4 s => s.General,
            SAV3 s3 => s3.Large,
            _ => sav.Data,
        };

        public bool WriteTo(SaveFile sav, PKM pkm, PKMImportSetting setting = PKMImportSetting.UseDefault)
        {
            if (PartyFormat)
                sav.SetSlotFormatParty(pkm, Data, Offset, setting, setting);
            else
                sav.SetSlotFormatStored(pkm, Data, Offset, setting, setting);
            return true;
        }

        public PKM Read(SaveFile sav)
        {
            return PartyFormat ? sav.GetPartySlot(Data, Offset) : sav.GetStoredSlot(Data, Offset);
        }
    }
}