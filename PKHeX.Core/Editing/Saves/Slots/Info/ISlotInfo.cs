namespace PKHeX.Core;

/// <summary>
/// Data representing info for an individual slot.
/// </summary>
public interface ISlotInfo
{
    /// <summary>
    /// Indicates the type of format the slot originates. Useful for legality purposes.
    /// </summary>
    StorageSlotType Type { get; }

    /// <summary>
    /// Differentiating slot number from other infos of the same type.
    /// </summary>
    int Slot { get; }

    /// <summary>
    /// Indicates if this slot can write to the requested <see cref="sav"/>.
    /// </summary>
    /// <param name="sav">Save file to try writing to.</param>
    /// <returns>True if the slot can be written to</returns>
    bool CanWriteTo(SaveFile sav);

    /// <summary>
    /// Checks if the <see cref="pk"/> can be written to the <see cref="sav"/> for this slot.
    /// </summary>
    /// <param name="sav">Save file to try writing to.</param>
    /// <param name="pk">Entity data to try writing.</param>
    /// <returns>True if the slot can be written to</returns>
    WriteBlockedMessage CanWriteTo(SaveFile sav, PKM pk);

    /// <summary>
    /// Tries writing the <see cref="pk"/> to the <see cref="sav"/>.
    /// </summary>
    /// <param name="sav">Save file to try writing to.</param>
    /// <param name="pk">Entity data to try writing.</param>
    /// <param name="setting">Setting to use when importing the <see cref="pk"/> data</param>
    /// <returns>Returns false if it did not succeed.</returns>
    bool WriteTo(SaveFile sav, PKM pk, PKMImportSetting setting = PKMImportSetting.UseDefault);

    /// <summary>
    /// Reads a <see cref="PKM"/> from the <see cref="sav"/>.
    /// </summary>
    /// <param name="sav">Save file to read from.</param>
    PKM Read(SaveFile sav);

    /// <summary>
    /// Indicates if the slot is empty.
    /// </summary>
    bool IsEmpty(SaveFile sav) => Read(sav).Species == 0;
}
