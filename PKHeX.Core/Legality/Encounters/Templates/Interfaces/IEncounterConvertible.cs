namespace PKHeX.Core;

/// <summary>
/// Exposes conversion methods to create a <see cref="PKM"/> from the object's data.
/// </summary>
public interface IEncounterConvertible
{
    /// <summary>
    /// Creates a <see cref="PKM"/> from the template, using the input <see cref="tr"/> as the trainer data.
    /// </summary>
    /// <remarks>This method calls <see cref="ConvertToPKM(ITrainerInfo, EncounterCriteria)"/> with a fixed criteria containing no restrictions on the generated data.</remarks>
    PKM ConvertToPKM(ITrainerInfo tr);

    /// <summary>
    /// Creates a <see cref="PKM"/> from the template, using the input <see cref="tr"/> as the trainer data.
    /// <br>The generation routine will try to yield a result that matches the specifications in the <see cref="criteria"/>.</br>
    /// </summary>
    PKM ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria);
}

/// <summary>
/// Exposes conversion methods to create a <see cref="T"/> from the object's data.
/// </summary>
public interface IEncounterConvertible<out T> where T : PKM
{
    /// <summary>
    /// Creates a <see cref="PKM"/> from the template, using the input <see cref="tr"/> as the trainer data.
    /// </summary>
    /// <remarks>This method calls <see cref="ConvertToPKM(ITrainerInfo, EncounterCriteria)"/> with a fixed criteria containing no restrictions on the generated data.</remarks>
    T ConvertToPKM(ITrainerInfo tr);

    /// <summary>
    /// Creates a <see cref="PKM"/> from the template, using the input <see cref="tr"/> as the trainer data.
    /// <br>The generation routine will try to yield a result that matches the specifications in the <see cref="criteria"/>.</br>
    /// </summary>
    T ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria);
}
