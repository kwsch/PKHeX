using System;

namespace PKHeX.Core;

/// <summary>
/// Personal Table storing <see cref="PersonalInfo7GG"/> used in <see cref="GameVersion.GG"/>.
/// </summary>
public sealed class PersonalTable7GG : IPersonalTable, IPersonalTable<PersonalInfo7GG>
{
    private readonly PersonalInfo7GG[] Table;
    private const int SIZE = PersonalInfo7GG.SIZE;
    private const ushort MaxSpecies = Legal.MaxSpeciesID_7b;
    public ushort MaxSpeciesID => MaxSpecies;

    public PersonalTable7GG(Memory<byte> data)
    {
        Table = new PersonalInfo7GG[data.Length / SIZE];
        var count = data.Length / SIZE;
        for (int i = 0, ofs = 0; i < count; i++, ofs += SIZE)
        {
            var slice = data.Slice(ofs, SIZE);
            Table[i] = new PersonalInfo7GG(slice);
        }
    }

    public PersonalInfo7GG this[int index] => Table[(uint)index < Table.Length ? index : 0];
    public PersonalInfo7GG this[ushort species, byte form] => Table[GetFormIndex(species, form)];
    public PersonalInfo7GG GetFormEntry(ushort species, byte form) => Table[GetFormIndex(species, form)];

    public int GetFormIndex(ushort species, byte form)
    {
        if (species <= MaxSpecies)
            return Table[species].FormIndex(species, form);
        return 0;
    }

    public bool IsSpeciesInGame(ushort species) => species is <= Legal.MaxSpeciesID_1 or (int)Species.Meltan or (int)Species.Melmetal;
    public bool IsPresentInGame(ushort species, byte form)
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
    PersonalInfo IPersonalTable.this[ushort species, byte form] => this[species, form];
    PersonalInfo IPersonalTable.GetFormEntry(ushort species, byte form) => GetFormEntry(species, form);
}
