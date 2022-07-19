using System;

namespace PKHeX.Core;

public sealed class PersonalTable6XY : IPersonalTable, IPersonalTable<PersonalInfo6XY>
{
    private readonly PersonalInfo6XY[] Table;
    private const int SIZE = PersonalInfo6XY.SIZE;
    private const int MaxSpecies = Legal.MaxSpeciesID_6;
    public int MaxSpeciesID => MaxSpecies;
    public int Count => Table.Length;

    public PersonalTable6XY(ReadOnlySpan<byte> data)
    {
        Table = new PersonalInfo6XY[data.Length / SIZE];
        var count = data.Length / SIZE;
        for (int i = 0, ofs = 0; i < count; i++, ofs += SIZE)
        {
            var slice = data.Slice(ofs, SIZE).ToArray();
            Table[i] = new PersonalInfo6XY(slice);
        }
    }

    public PersonalInfo6XY this[int index] => Table[index];
    public PersonalInfo6XY this[int species, int form] => Table[species];
    public PersonalInfo6XY GetFormEntry(int species, int form) => Table[GetFormIndex(species, form)];

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
