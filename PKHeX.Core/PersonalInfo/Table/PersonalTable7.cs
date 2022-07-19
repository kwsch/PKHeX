using System;

namespace PKHeX.Core;

public sealed class PersonalTable7 : IPersonalTable, IPersonalTable<PersonalInfo7>
{
    internal readonly PersonalInfo7[] Table;
    private const int SIZE = PersonalInfo7.SIZE;
    private readonly int MaxSpecies;
    public int MaxSpeciesID => MaxSpecies;
    public int Count => Table.Length;

    public PersonalTable7(ReadOnlySpan<byte> data, int maxSpecies)
    {
        MaxSpecies = maxSpecies;
        Table = new PersonalInfo7[data.Length / SIZE];
        var count = data.Length / SIZE;
        for (int i = 0, ofs = 0; i < count; i++, ofs += SIZE)
        {
            var slice = data.Slice(ofs, SIZE).ToArray();
            Table[i] = new PersonalInfo7(slice);
        }
    }

    public PersonalInfo7 this[int index] => Table[index];
    public PersonalInfo7 this[int species, int form] => Table[species];
    public PersonalInfo7 GetFormEntry(int species, int form) => Table[GetFormIndex(species, form)];

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
            if (GetFormEntry(species, i).IsPresentInGame)
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
