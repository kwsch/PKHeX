using System;

namespace PKHeX.Core;

public sealed class PersonalTable8LA : IPersonalTable, IPersonalTable<PersonalInfo8LA>
{
    internal readonly PersonalInfo8LA[] Table;
    private const int SIZE = PersonalInfo8LA.SIZE;
    private const int MaxSpecies = Legal.MaxSpeciesID_8a;
    public int MaxSpeciesID => MaxSpecies;
    public int Count => Table.Length;

    public PersonalTable8LA(ReadOnlySpan<byte> data)
    {
        Table = new PersonalInfo8LA[data.Length / SIZE];
        var count = data.Length / SIZE;
        for (int i = 0, ofs = 0; i < count; i++, ofs += SIZE)
        {
            var slice = data.Slice(ofs, SIZE).ToArray();
            Table[i] = new PersonalInfo8LA(slice);
        }
    }

    public PersonalInfo8LA this[int index] => Table[(uint)index < Table.Length ? index : 0];
    public PersonalInfo8LA this[int species, int form] => Table[GetFormIndex(species, form)];
    public PersonalInfo8LA GetFormEntry(int species, int form) => Table[GetFormIndex(species, form)];

    public int GetFormIndex(int species, int form)
    {
        if ((uint)species <= MaxSpecies)
            return Table[species].FormIndex(species, form);
        return 0;
    }

    public bool IsSpeciesInGame(int species)
    {
        if ((uint)species > MaxSpecies)
            return false;

        var form0 = Table[species];
        if (form0.IsPresentInGame)
            return true;

        var fc = form0.FormCount;
        for (int i = 1; i < fc; i++)
        {
            var entry = GetFormEntry(species, i);
            if (entry.IsPresentInGame)
                return true;
        }
        return false;
    }

    public bool IsPresentInGame(int species, int form)
    {
        if ((uint)species > MaxSpecies)
            return false;

        var form0 = Table[species];
        if (form == 0)
            return form0.IsPresentInGame;
        if (!form0.HasForm(form))
            return false;

        var entry = GetFormEntry(species, form);
        return entry.IsPresentInGame;
    }

    PersonalInfo IPersonalTable.this[int index] => this[index];
    PersonalInfo IPersonalTable.this[int species, int form] => this[species, form];
    PersonalInfo IPersonalTable.GetFormEntry(int species, int form) => GetFormEntry(species, form);
}
