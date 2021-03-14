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
        public StorageSlotType Type { get; init; }

        private readonly byte[] Data; // buffer to r/w

        public SlotInfoMisc(SaveFile sav, int slot, int offset, bool party = false)
        {
            Slot = slot;
            Offset = offset;
            PartyFormat = party;
            Data = sav switch
            {
                SAV4 s => s.General,
                SAV3 s3 => s3.Large,
                _ => sav.Data
            };
        }

        public SlotInfoMisc(byte[] data, int slot, int offset, bool party = false)
        {
            Slot = slot;
            Offset = offset;
            PartyFormat = party;
            Data = data;
        }

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

        private bool Equals(SlotInfoMisc other) => Offset == other.Offset && Data == other.Data;
        public bool Equals(ISlotInfo other) => other is SlotInfoMisc p && Equals(p);
        public override bool Equals(object obj) => obj is SlotInfoMisc p && Equals(p);
        public override int GetHashCode() => Offset;
    }
}