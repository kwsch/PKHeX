using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Data representing info for an individual slot.
    /// </summary>
    public interface ISlotInfo : IEquatable<ISlotInfo>
    {
        int Slot { get; }
        bool CanWriteTo(SaveFile SAV);
        WriteBlockedMessage CanWriteTo(SaveFile SAV, PKM pkm);
        bool WriteTo(SaveFile sav, PKM pkm, PKMImportSetting setting = PKMImportSetting.UseDefault);
        PKM Read(SaveFile sav);
    }
}