using System;

namespace PKHeX.Core;

public sealed class PersonalTable8BDSP : IPersonalTable, IPersonalTable<PersonalInfo8BDSP>
{
    private readonly PersonalInfo8BDSP[] Table;
    private const int SIZE = PersonalInfo8BDSP.SIZE;
    private const int MaxSpecies = Legal.MaxSpeciesID_8b;
    public int MaxSpeciesID => MaxSpecies;
    public int Count => Table.Length;

    public PersonalTable8BDSP(ReadOnlySpan<byte> data)
    {
        Table = new PersonalInfo8BDSP[data.Length / SIZE];
        var count = data.Length / SIZE;
        for (int i = 0, ofs = 0; i < count; i++, ofs += SIZE)
        {
            var slice = data.Slice(ofs, SIZE).ToArray();
            Table[i] = new PersonalInfo8BDSP(slice);
        }
    }

    public PersonalInfo8BDSP this[int index] => Table[(uint)index < Table.Length ? index : 0];
    public PersonalInfo8BDSP this[int species, int form] => Table[GetFormIndex(species, form)];
    public PersonalInfo8BDSP GetFormEntry(int species, int form) => Table[GetFormIndex(species, form)];

    public int GetFormIndex(int species, int form)
    {
        if ((uint)species <= MaxSpecies)
            return Table[species].FormIndex(species, form);
        return 0;
    }

    public bool IsSpeciesInGame(int species) => (uint)species <= MaxSpecies;
    public bool IsPresentInGame(int species, int form)
    {
        if (!IsSpeciesInGame(species))
            return false;
        if (form == 0)
            return true;
        if (Table[species].HasForm(form))
            return true;
        return false;
    }

    PersonalInfo IPersonalTable.this[int index] => this[index];
    PersonalInfo IPersonalTable.this[int species, int form] => this[species, form];
    PersonalInfo IPersonalTable.GetFormEntry(int species, int form) => GetFormEntry(species, form);
}
