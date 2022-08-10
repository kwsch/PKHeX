using System;
using static PKHeX.Core.Species;

namespace PKHeX.Core;

/// <summary>
/// Personal Table storing <see cref="PersonalInfo5BW"/> used in Generation 5 games.
/// </summary>
public sealed class PersonalTable5BW : IPersonalTable, IPersonalTable<PersonalInfo5BW>
{
    private readonly PersonalInfo5BW[] Table;
    private const int SIZE = PersonalInfo5BW.SIZE;
    private const int MaxSpecies = Legal.MaxSpeciesID_5;
    public int MaxSpeciesID => MaxSpecies;

    public PersonalTable5BW(ReadOnlySpan<byte> data)
    {
        Table = new PersonalInfo5BW[data.Length / SIZE];
        var count = data.Length / SIZE;
        for (int i = 0, ofs = 0; i < count; i++, ofs += SIZE)
        {
            var slice = data.Slice(ofs, SIZE).ToArray();
            Table[i] = new PersonalInfo5BW(slice);
        }
    }

    public PersonalInfo5BW this[int index] => Table[(uint)index < Table.Length ? index : 0];
    public PersonalInfo5BW this[int species, int form] => Table[GetFormIndex(species, form)];
    public PersonalInfo5BW GetFormEntry(int species, int form) => Table[GetFormIndex(species, form)];

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
        return species switch
        {
            (int)Unown => form < 28,
            (int)Burmy => form < 3,
            (int)Mothim => form < 3,
            (int)Cherrim => form == 1,
            (int)Shellos or (int)Gastrodon => form == 1,
            (int)Arceus => form < 17,
            (int)Deerling or (int)Sawsbuck => form < 4,
            (int)Genesect => form <= 4,
            _ => false,
        };
    }

    PersonalInfo IPersonalTable.this[int index] => this[index];
    PersonalInfo IPersonalTable.this[int species, int form] => this[species, form];
    PersonalInfo IPersonalTable.GetFormEntry(int species, int form) => GetFormEntry(species, form);
}
