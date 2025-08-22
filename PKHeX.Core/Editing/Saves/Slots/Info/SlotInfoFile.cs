namespace PKHeX.Core;

/// <inheritdoc cref="SlotInfoFileMulti(string, int, int)"/>
public sealed record SlotInfoFileSingle(string Path) : SlotInfoFileMulti(Path, 0, 1);

/// <summary>
/// Records data for <see cref="ISlotInfo"/> that originates from a readonly external file.
/// </summary>
/// <remarks>
/// The file is read-only, so no writing is possible.
/// </remarks>
/// <param name="Path">Path the file was loaded from.</param>
/// <param name="Slot">The slot index within the file.</param>
/// <param name="Count">The number of slots inside the file.</param>
public record SlotInfoFileMulti(string Path, int Slot, int Count) : ISlotInfo
{
    public StorageSlotType Type => StorageSlotType.Party;
    public bool CanWriteTo(SaveFile sav) => false;
    public WriteBlockedMessage CanWriteTo(SaveFile sav, PKM pk) => WriteBlockedMessage.InvalidDestination;
    public bool WriteTo(SaveFile sav, PKM pk, EntityImportSettings settings = default) => false;
    public PKM Read(SaveFile sav) => sav.BlankPKM;
    public bool IsSingleFile => Count == 1;
}
