namespace PKHeX.Core;

public interface IPersonalFormInfo
{
    /// <summary>
    /// Count of <see cref="PKM.Form"/> values the entry can have.
    /// </summary>
    int FormCount { get; set; }

    /// <summary>
    /// Pointer to the first <see cref="PKM.Form"/> <see cref="PersonalInfo"/> index
    /// </summary>
    int FormStatsIndex { get; set; }

    /// <summary>
    /// Indicates if the entry has forms or not.
    ///  </summary>
    bool HasForms { get; }

    /// <summary>
    /// Gets the <see cref="PersonalTable"/> <see cref="PKM.Form"/> entry index for the input criteria, with fallback for the original species entry.
    /// </summary>
    /// <param name="species"><see cref="PKM.Species"/> to retrieve for</param>
    /// <param name="form"><see cref="PKM.Form"/> to retrieve for</param>
    /// <returns>Index the <see cref="PKM.Form"/> exists as in the <see cref="PersonalTable"/>.</returns>
    int FormIndex(int species, int form);

    /// <summary>
    /// Checks if the <see cref="PersonalInfo"/> has the requested <see cref="PKM.Form"/> entry index available.
    /// </summary>
    /// <param name="form"><see cref="PKM.Form"/> to retrieve for</param>
    bool HasForm(int form);

    /// <summary>
    /// Checks to see if the <see cref="PKM.Form"/> is valid within the <see cref="PersonalInfo.FormCount"/>
    /// </summary>
    /// <param name="form"></param>
    bool IsFormWithinRange(int form);
}
