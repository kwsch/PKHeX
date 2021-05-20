namespace PKHeX.Core
{
    /// <summary>
    /// Exposes conversion methods to create a <see cref="PKM"/> from the object's data.
    /// </summary>
    public interface IEncounterConvertible
    {
        /// <summary>
        /// Creates a <see cref="PKM"/> from the template, using the input <see cref="sav"/> as the trainer data.
        /// </summary>
        /// <remarks>This method calls <see cref="ConvertToPKM(ITrainerInfo, EncounterCriteria)"/> with a fixed criteria containing no restrictions on the generated data.</remarks>
        PKM ConvertToPKM(ITrainerInfo sav);

        /// <summary>
        /// Creates a <see cref="PKM"/> from the template, using the input <see cref="sav"/> as the trainer data.
        /// <br>The generation routine will try to yield a result that matches the specifications in the <see cref="criteria"/>.</br>
        /// </summary>
        PKM ConvertToPKM(ITrainerInfo sav, EncounterCriteria criteria);
    }
}
