namespace PKHeX.Core;

/// <summary>
/// Records data for <see cref="ISlotInfo"/> that originates from an external file.
/// </summary>
/// <param name="Path"></param>
public sealed record SlotInfoFile(string Path) : ISlotInfo
{
    public SlotOrigin Origin => SlotOrigin.Party;
    public int Slot => 0;

    public bool CanWriteTo(SaveFile sav) => false;
    public WriteBlockedMessage CanWriteTo(SaveFile sav, PKM pk) => WriteBlockedMessage.InvalidDestination;
    public bool WriteTo(SaveFile sav, PKM pk, PKMImportSetting setting = PKMImportSetting.UseDefault) => false;
    public PKM Read(SaveFile sav) => sav.BlankPKM;
}
