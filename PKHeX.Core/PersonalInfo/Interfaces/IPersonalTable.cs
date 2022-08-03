namespace PKHeX.Core;

public interface IPersonalTable
{
    /// <summary>
    /// Max Species ID (National Dex) that is stored in the table.
    /// </summary>
    int MaxSpeciesID { get; }

    /// <summary>
    /// Count of entries in the table.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Gets an index from the inner array.
    /// </summary>
    /// <remarks>Has built in length checks; returns empty (0) entry if out of range.</remarks>
    /// <param name="index">Index to retrieve</param>
    /// <returns>Requested index entry</returns>
    PersonalInfo this[int index] { get; }

    /// <summary>
    /// Alternate way of fetching <see cref="GetFormEntry"/>.
    /// </summary>
    PersonalInfo this[int species, int form] { get; }

    /// <summary>
    /// Gets the <see cref="PersonalInfo"/> entry index for a given <see cref="PKM.Species"/> and <see cref="PKM.Form"/>.
    /// </summary>
    /// <param name="species"><see cref="PKM.Species"/></param>
    /// <param name="form"><see cref="PKM.Form"/></param>
    /// <returns>Entry index for the input criteria</returns>
    int GetFormIndex(int species, int form);

    /// <summary>
    /// Gets the <see cref="PersonalInfo"/> entry for a given <see cref="PKM.Species"/> and <see cref="PKM.Form"/>.
    /// </summary>
    /// <param name="species"><see cref="PKM.Species"/></param>
    /// <param name="form"><see cref="PKM.Form"/></param>
    /// <returns>Entry for the input criteria</returns>
    PersonalInfo GetFormEntry(int species, int form);

    bool IsSpeciesInGame(int species);
    bool IsPresentInGame(int species, int form);
}

public interface IPersonalTable<out T> where T : IPersonalInfo
{
    T this[int index] { get; }
    T this[int species, int form] { get; }
    T GetFormEntry(int species, int form);
}

public static class PersonalTableExtensions
{
    /// <summary>
    /// Gets form names for every species.
    /// </summary>
    /// <param name="table">Table to use</param>
    /// <param name="species">Raw string resource (Species) for the corresponding table.</param>
    /// <param name="MaxSpecies">Max Species ID (<see cref="PKM.Species"/>)</param>
    /// <returns>Array of species containing an array of form names for that species.</returns>
    public static string[][] GetFormList(this IPersonalTable table, string[] species, int MaxSpecies)
    {
        string[][] FormList = new string[MaxSpecies + 1][];
        for (int i = 0; i < FormList.Length; i++)
        {
            int FormCount = table[i].FormCount;
            FormList[i] = new string[FormCount];
            if (FormCount <= 0)
                continue;

            FormList[i][0] = species[i];
            for (int j = 1; j < FormCount; j++)
                FormList[i][j] = $"{species[i]} {j}";
        }

        return FormList;
    }

    /// <summary>
    /// Gets an arranged list of Form names and indexes for use with the individual <see cref="PersonalInfo"/> <see cref="PKM.Form"/> values.
    /// </summary>
    /// <param name="table">Table to use</param>
    /// <param name="forms">Raw string resource (Forms) for the corresponding table.</param>
    /// <param name="species">Raw string resource (Species) for the corresponding table.</param>
    /// <param name="MaxSpecies">Max Species ID (<see cref="PKM.Species"/>)</param>
    /// <param name="baseForm">Pointers for base form IDs</param>
    /// <param name="formVal">Pointers for table indexes for each form</param>
    /// <returns>Sanitized list of species names, and outputs indexes for various lookup purposes.</returns>
    public static string[] GetPersonalEntryList(this IPersonalTable table, string[][] forms, string[] species, int MaxSpecies, out int[] baseForm, out int[] formVal)
    {
        string[] result = new string[table.Count];
        baseForm = new int[result.Length];
        formVal = new int[result.Length];
        for (int i = 0; i <= MaxSpecies; i++)
        {
            result[i] = species[i];
            if (forms[i].Length == 0)
                continue;
            int basePtr = table[i].FormStatsIndex;
            if (basePtr <= 0)
                continue;
            for (int j = 1; j < forms[i].Length; j++)
            {
                int ptr = basePtr + j - 1;
                baseForm[ptr] = i;
                formVal[ptr] = j;
                result[ptr] = forms[i][j];
            }
        }
        return result;
    }
}
