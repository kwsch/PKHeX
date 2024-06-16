using System;
using static PKHeX.Core.Species;

namespace PKHeX.Core;

/// <summary>
/// Personal Table storing <see cref="PersonalInfo7"/> used in Generation 7 games.
/// </summary>
public sealed class PersonalTable7 : IPersonalTable, IPersonalTable<PersonalInfo7>
{
    private readonly PersonalInfo7[] Table;
    private const int SIZE = PersonalInfo7.SIZE;
    public ushort MaxSpeciesID { get; }

    public PersonalTable7(Memory<byte> data, ushort maxSpecies)
    {
        MaxSpeciesID = maxSpecies;
        Table = new PersonalInfo7[data.Length / SIZE];
        var count = data.Length / SIZE;
        for (int i = 0, ofs = 0; i < count; i++, ofs += SIZE)
        {
            var slice = data.Slice(ofs, SIZE);
            Table[i] = new PersonalInfo7(slice);
        }
    }

    public PersonalInfo7 this[int index] => Table[(uint)index < Table.Length ? index : 0];
    public PersonalInfo7 this[ushort species, byte form] => Table[GetFormIndex(species, form)];
    public PersonalInfo7 GetFormEntry(ushort species, byte form) => Table[GetFormIndex(species, form)];

    public int GetFormIndex(ushort species, byte form)
    {
        if (species <= MaxSpeciesID)
            return Table[species].FormIndex(species, form);
        return 0;
    }

    public bool IsSpeciesInGame(ushort species) => species <= MaxSpeciesID;
    public bool IsPresentInGame(ushort species, byte form)
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
    PersonalInfo IPersonalTable.this[ushort species, byte form] => this[species, form];
    PersonalInfo IPersonalTable.GetFormEntry(ushort species, byte form) => GetFormEntry(species, form);
}
