using System;
using static PKHeX.Core.Species;

namespace PKHeX.Core;

public sealed class PersonalTable4 : IPersonalTable, IPersonalTable<PersonalInfo4>
{
    internal readonly PersonalInfo4[] Table;
    private const int SIZE = PersonalInfo4.SIZE;
    private const int MaxSpecies = Legal.MaxSpeciesID_4;
    public int MaxSpeciesID => MaxSpecies;
    public int Count => Table.Length;

    public PersonalTable4(ReadOnlySpan<byte> data)
    {
        Table = new PersonalInfo4[data.Length / SIZE];
        var count = data.Length / SIZE;
        for (int i = 0, ofs = 0; i < count; i++, ofs += SIZE)
        {
            var slice = data.Slice(ofs, SIZE).ToArray();
            Table[i] = new PersonalInfo4(slice);
        }
    }

    public PersonalInfo4 this[int index] => Table[(uint)index < Table.Length ? index : 0];
    public PersonalInfo4 this[int species, int form] => Table[GetFormIndex(species, form)];
    public PersonalInfo4 GetFormEntry(int species, int form) => Table[GetFormIndex(species, form)];

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
            (int)Pichu => form == 1,
            (int)Unown => form < 28,
            (int)Castform => form < 4,
            (int)Burmy => form < 3,
            (int)Mothim => form < 3,
            (int)Cherrim => form == 1,
            (int)Shellos or (int)Gastrodon => form == 1,
            (int)Arceus => form is < 18 and not 9, // Curse type does not exist legally.
            _ => false,
        };
    }

    PersonalInfo IPersonalTable.this[int index] => this[index];
    PersonalInfo IPersonalTable.this[int species, int form] => this[species, form];
    PersonalInfo IPersonalTable.GetFormEntry(int species, int form) => GetFormEntry(species, form);
}
