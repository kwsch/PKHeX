using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Data representing info for an individual slot.
    /// </summary>
    public interface ISlotInfo : IEquatable<ISlotInfo>
    {
        int Slot { get; }
        bool CanWriteTo(SaveFile sav);
        WriteBlockedMessage CanWriteTo(SaveFile sav, PKM pkm);
        bool WriteTo(SaveFile sav, PKM pkm, PKMImportSetting setting = PKMImportSetting.UseDefault);
        PKM Read(SaveFile sav);
    }
}