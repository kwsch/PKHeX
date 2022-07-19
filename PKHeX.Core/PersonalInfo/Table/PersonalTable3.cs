using System;

namespace PKHeX.Core;

public sealed class PersonalTable3 : IPersonalTable, IPersonalTable<PersonalInfo3>
{
    internal readonly PersonalInfo3[] Table; // internal to share with Gen1 tables
    private const int SIZE = PersonalInfo3.SIZE;
    private const int MaxSpecies = Legal.MaxSpeciesID_3;
    public int MaxSpeciesID => MaxSpecies;
    public int Count => Table.Length;

    public PersonalTable3(ReadOnlySpan<byte> data)
    {
        Table = new PersonalInfo3[data.Length / SIZE];
        var count = data.Length / SIZE;
        for (int i = 0, ofs = 0; i < count; i++, ofs += SIZE)
        {
            var slice = data.Slice(ofs, SIZE).ToArray();
            Table[i] = new PersonalInfo3(slice);
        }
    }

    public PersonalInfo3 this[int index] => Table[index];
    public PersonalInfo3 this[int species, int form] => Table[species];
    public PersonalInfo3 GetFormEntry(int species, int form) => Table[GetFormIndex(species, form)];

    public int GetFormIndex(int species, int form) => species;
    public bool IsSpeciesInGame(int species) => (uint)species <= MaxSpecies;
    public bool IsPresentInGame(int species, int form)
    {
        if (!IsSpeciesInGame(species))
            return false;
        if (form == 0)
            return true;
        return species switch
        {
            (int)Species.Unown => form < 26,
            (int)Species.Deoxys => form < 3,
            _ => false,
        };
    }

    PersonalInfo IPersonalTable.this[int index] => this[index];
    PersonalInfo IPersonalTable.this[int species, int form] => this[species, form];
    PersonalInfo IPersonalTable.GetFormEntry(int species, int form) => GetFormEntry(species, form);
}
