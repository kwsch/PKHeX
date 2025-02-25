namespace PKHeX.Core;

/// <summary>
/// Box Data <see cref="ISlotInfo"/>
/// </summary>
public sealed record SlotInfoBox(int Box, int Slot) : ISlotInfo
{
    public StorageSlotType Type => StorageSlotType.Box;
    public bool CanWriteTo(SaveFile sav) => sav.HasBox && !sav.IsBoxSlotLocked(Box, Slot);
    public WriteBlockedMessage CanWriteTo(SaveFile sav, PKM pk) => WriteBlockedMessage.None;

    public bool WriteTo(SaveFile sav, PKM pk, EntityImportSettings settings = default)
    {
        sav.SetBoxSlotAtIndex(pk, Box, Slot, settings);
        return true;
    }

    public PKM Read(SaveFile sav) => sav.GetBoxSlotAtIndex(Box, Slot);
}
