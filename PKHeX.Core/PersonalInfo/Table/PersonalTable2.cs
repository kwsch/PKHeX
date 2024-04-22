using System;

namespace PKHeX.Core;

/// <summary>
/// Personal Table storing <see cref="PersonalInfo2"/> used in Generation 2 games.
/// </summary>
public sealed class PersonalTable2 : IPersonalTable, IPersonalTable<PersonalInfo2>
{
    private readonly PersonalInfo2[] Table;
    private const int SIZE = PersonalInfo2.SIZE;
    private const ushort MaxSpecies = Legal.MaxSpeciesID_2;
    public ushort MaxSpeciesID => MaxSpecies;

    public PersonalTable2(Memory<byte> data)
    {
        Table = new PersonalInfo2[data.Length / SIZE];
        var count = data.Length / SIZE;
        for (int i = 0, ofs = 0; i < count; i++, ofs += SIZE)
        {
            var slice = data.Slice(ofs, SIZE);
            Table[i] = new PersonalInfo2(slice);
        }
    }

    public PersonalInfo2 this[int index] => Table[(uint)index < Table.Length ? index : 0];
    public PersonalInfo2 this[ushort species, byte form] => Table[GetFormIndex(species, form)];
    public PersonalInfo2 GetFormEntry(ushort species, byte form) => Table[GetFormIndex(species, form)];

    public int GetFormIndex(ushort species, byte form) => IsSpeciesInGame(species) ? species : 0;
    public bool IsSpeciesInGame(ushort species) => species <= MaxSpecies;
    public bool IsPresentInGame(ushort species, byte form)
    {
        if (!IsSpeciesInGame(species))
            return false;
        if (form == 0)
            return true;
        return species == (int)Species.Unown && form < 26;
    }

    PersonalInfo IPersonalTable.this[int index] => this[index];
    PersonalInfo IPersonalTable.this[ushort species, byte form] => this[species, form];
    PersonalInfo IPersonalTable.GetFormEntry(ushort species, byte form) => GetFormEntry(species, form);
}
