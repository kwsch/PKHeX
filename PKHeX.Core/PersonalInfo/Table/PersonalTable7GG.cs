using System;

namespace PKHeX.Core;

public sealed class PersonalTable7GG : IPersonalTable, IPersonalTable<PersonalInfo7GG>
{
    private readonly PersonalInfo7GG[] Table;
    private const int SIZE = PersonalInfo7GG.SIZE;
    private const int MaxSpecies = Legal.MaxSpeciesID_7b;
    public int MaxSpeciesID => MaxSpecies;
    public int Count => Table.Length;

    public PersonalTable7GG(ReadOnlySpan<byte> data)
    {
        Table = new PersonalInfo7GG[data.Length / SIZE];
        var count = data.Length / SIZE;
        for (int i = 0, ofs = 0; i < count; i++, ofs += SIZE)
        {
            var slice = data.Slice(ofs, SIZE).ToArray();
            Table[i] = new PersonalInfo7GG(slice);
        }
    }

    public PersonalInfo7GG this[int index] => Table[(uint)index < Table.Length ? index : 0];
    public PersonalInfo7GG this[int species, int form] => Table[GetFormIndex(species, form)];
    public PersonalInfo7GG GetFormEntry(int species, int form) => Table[GetFormIndex(species, form)];

    public int GetFormIndex(int species, int form)
    {
        if ((uint)species <= MaxSpecies)
            return Table[species].FormIndex(species, form);
        return 0;
    }

    public bool IsSpeciesInGame(int species) => (uint)species is <= Legal.MaxSpeciesID_1 or (int)Species.Meltan or (int)Species.Melmetal;
    public bool IsPresentInGame(int species, int form)
    {
        if (!IsSpeciesInGame(species))
            return false;
        if (form == 0)
            return true;
        if (species == (int)Species.Pikachu)
            return form == 8;
        if (Table[species].HasForm(form))
            return true;
        return false;
    }

    PersonalInfo IPersonalTable.this[int index] => this[index];
    PersonalInfo IPersonalTable.this[int species, int form] => this[species, form];
    PersonalInfo IPersonalTable.GetFormEntry(int species, int form) => GetFormEntry(species, form);
}
