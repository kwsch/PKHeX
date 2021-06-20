namespace PKHeX.Core
{
    public sealed class SlotInfoFile : ISlotInfo
    {
        public readonly string Path;
        public SlotOrigin Origin => SlotOrigin.Party;
        public int Slot => 0;

        public SlotInfoFile(string path) => Path = path;
        public bool Equals(ISlotInfo other) => other is SlotInfoFile f && f.Path == Path;

        public bool CanWriteTo(SaveFile sav) => false;
        public WriteBlockedMessage CanWriteTo(SaveFile sav, PKM pkm) => WriteBlockedMessage.InvalidDestination;
        public bool WriteTo(SaveFile sav, PKM pkm, PKMImportSetting setting = PKMImportSetting.UseDefault) => false;
        public PKM Read(SaveFile sav) => sav.BlankPKM;
    }
}
