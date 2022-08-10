using System;
using static PKHeX.Core.Species;

namespace PKHeX.Core;

/// <summary>
/// Personal Table storing <see cref="PersonalInfo7"/> used in Generation 7 games.
/// </summary>
public sealed class PersonalTable7 : IPersonalTable, IPersonalTable<PersonalInfo7>
{
    internal readonly PersonalInfo7[] Table;
    private const int SIZE = PersonalInfo7.SIZE;
    public int MaxSpeciesID { get; }

    public PersonalTable7(ReadOnlySpan<byte> data, int maxSpecies)
    {
        MaxSpeciesID = maxSpecies;
        Table = new PersonalInfo7[data.Length / SIZE];
        var count = data.Length / SIZE;
        for (int i = 0, ofs = 0; i < count; i++, ofs += SIZE)
        {
            var slice = data.Slice(ofs, SIZE).ToArray();
            Table[i] = new PersonalInfo7(slice);
        }
    }

    public PersonalInfo7 this[int index] => Table[(uint)index < Table.Length ? index : 0];
    public PersonalInfo7 this[int species, int form] => Table[GetFormIndex(species, form)];
    public PersonalInfo7 GetFormEntry(int species, int form) => Table[GetFormIndex(species, form)];

    public int GetFormIndex(int species, int form)
    {
        if ((uint)species <= MaxSpeciesID)
            return Table[species].FormIndex(species, form);
        return 0;
    }

    public bool IsSpeciesInGame(int species) => (uint)species <= MaxSpeciesID;
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
            (int)Arceus => form < 18,
            (int)Genesect => form <= 4,
            (int)Scatterbug or (int)Spewpa => form < 18,
            (int)Vivillon => form < 20,
            (int)Deerling or (int)Sawsbuck => form < 4,
            (int)Flabébé or (int)Florges => form < 5,
            (int)Floette => form < 6,
            (int)Xerneas => form == 1,
            (int)Silvally => form < 18,
            _ => false,
        };
    }

    PersonalInfo IPersonalTable.this[int index] => this[index];
    PersonalInfo IPersonalTable.this[int species, int form] => this[species, form];
    PersonalInfo IPersonalTable.GetFormEntry(int species, int form) => GetFormEntry(species, form);
}
