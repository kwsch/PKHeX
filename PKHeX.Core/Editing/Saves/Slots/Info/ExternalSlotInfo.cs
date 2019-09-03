using System;

namespace PKHeX.Core
{
    /// <summary>
    /// <see cref="ISlotInfo"/> originating from outside of a save file (e.g. a saved file on a hard drive).
    /// </summary>
    public sealed class ExternalSlotInfo : ISlotInfo
    {
        private readonly PKM Data;
        public ExternalSlotInfo(PKM pkm) => Data = pkm;

        public int Slot { get; } = -1;
        public bool Equals(ISlotInfo other) => false;
        public bool CanWriteTo(SaveFile SAV) => false;
        public WriteBlockedMessage CanWriteTo(SaveFile SAV, PKM pkm) => WriteBlockedMessage.InvalidDestination;
        public bool WriteTo(SaveFile sav, PKM pkm, PKMImportSetting setting = PKMImportSetting.UseDefault) => throw new InvalidOperationException();

        public PKM Read(SaveFile sav) => Data;
    }
}