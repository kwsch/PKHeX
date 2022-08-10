using System;

namespace PKHeX.Core;

/// <summary>
/// Personal Table storing <see cref="PersonalTable8SWSH"/> used in <see cref="GameVersion.SWSH"/>.
/// </summary>
public sealed class PersonalTable8SWSH : IPersonalTable, IPersonalTable<PersonalInfo8SWSH>
{
    internal readonly PersonalInfo8SWSH[] Table;
    private const int SIZE = PersonalInfo8SWSH.SIZE;
    private const int MaxSpecies = Legal.MaxSpeciesID_8_R2;
    public int MaxSpeciesID => MaxSpecies;

    public PersonalTable8SWSH(ReadOnlySpan<byte> data)
    {
        Table = new PersonalInfo8SWSH[data.Length / SIZE];
        var count = data.Length / SIZE;
        for (int i = 0, ofs = 0; i < count; i++, ofs += SIZE)
        {
            var slice = data.Slice(ofs, SIZE).ToArray();
            Table[i] = new PersonalInfo8SWSH(slice);
        }
    }

    public PersonalInfo8SWSH this[int index] => Table[(uint)index < Table.Length ? index : 0];
    public PersonalInfo8SWSH this[int species, int form] => Table[GetFormIndex(species, form)];
    public PersonalInfo8SWSH GetFormEntry(int species, int form) => Table[GetFormIndex(species, form)];

    public int GetFormIndex(int species, int form)
    {
        if ((uint)species <= MaxSpeciesID)
            return Table[species].FormIndex(species, form);
        return 0;
    }

    public bool IsSpeciesInGame(int species)
    {
        if ((uint)species > MaxSpeciesID)
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
        if ((uint)species > MaxSpeciesID)
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
