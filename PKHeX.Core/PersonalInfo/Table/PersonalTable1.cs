using System;

namespace PKHeX.Core;

/// <summary>
/// Personal Table storing <see cref="PersonalInfo1"/> used in Generation 1 games.
/// </summary>
public sealed class PersonalTable1 : IPersonalTable, IPersonalTable<PersonalInfo1>
{
    private readonly PersonalInfo1[] Table;
    private const int SIZE = PersonalInfo1.SIZE;
    private const int MaxSpecies = Legal.MaxSpeciesID_1;
    public int MaxSpeciesID => MaxSpecies;

    public PersonalTable1(ReadOnlySpan<byte> data)
    {
        Table = new PersonalInfo1[data.Length / SIZE];
        var count = data.Length / SIZE;
        for (int i = 0, ofs = 0; i < count; i++, ofs += SIZE)
        {
            var slice = data.Slice(ofs, SIZE).ToArray();
            Table[i] = new PersonalInfo1(slice);
        }
    }

    public PersonalInfo1 this[int index] => Table[(uint)index < Table.Length ? index : 0];
    public PersonalInfo1 this[int species, int form] => Table[GetFormIndex(species, form)];
    public PersonalInfo1 GetFormEntry(int species, int form) => Table[GetFormIndex(species, form)];

    public int GetFormIndex(int species, int form) => IsSpeciesInGame(species) ? species : 0;
    public bool IsSpeciesInGame(int species) => (uint)species <= MaxSpecies;
    public bool IsPresentInGame(int species, int form) => form == 0 && IsSpeciesInGame(species);

    PersonalInfo IPersonalTable.this[int index] => this[index];
    PersonalInfo IPersonalTable.this[int species, int form] => this[species, form];
    PersonalInfo IPersonalTable.GetFormEntry(int species, int form) => GetFormEntry(species, form);

    public void LoadValuesFrom(PersonalInfo2[] data)
    {
        for (int i = MaxSpecies; i >= 0; i--)
            Table[i].Gender = data[i].Gender;
    }

    /// <summary>
    /// Checks to see if either of the input type combinations exist in the table.
    /// </summary>
    /// <remarks>Only useful for checking Generation 1 <see cref="PK1.Type_A"/> and <see cref="PK1.Type_B"/> properties.</remarks>
    /// <param name="type1">First type</param>
    /// <param name="type2">Second type</param>
    /// <returns>Indication that the combination exists in the table.</returns>
    public bool IsValidTypeCombination(int type1, int type2)
    {
        for (int i = 1; i <= MaxSpecies; i++)
        {
            var pi = Table[i];
            if (pi.IsValidTypeCombination(type1, type2))
                return true;
        }
        return false;
    }
}
