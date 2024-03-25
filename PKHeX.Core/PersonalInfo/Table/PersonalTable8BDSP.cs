using System;

namespace PKHeX.Core;

/// <summary>
/// Personal Table storing <see cref="PersonalInfo8BDSP"/> used in <see cref="GameVersion.BDSP"/>.
/// </summary>
public sealed class PersonalTable8BDSP : IPersonalTable, IPersonalTable<PersonalInfo8BDSP>
{
    private readonly PersonalInfo8BDSP[] Table;
    private const int SIZE = PersonalInfo8BDSP.SIZE;
    private const ushort MaxSpecies = Legal.MaxSpeciesID_8b;
    public ushort MaxSpeciesID => MaxSpecies;

    public PersonalTable8BDSP(Memory<byte> data)
    {
        Table = new PersonalInfo8BDSP[data.Length / SIZE];
        var count = data.Length / SIZE;
        for (int i = 0, ofs = 0; i < count; i++, ofs += SIZE)
        {
            var slice = data.Slice(ofs, SIZE);
            Table[i] = new PersonalInfo8BDSP(slice);
        }
    }

    public PersonalInfo8BDSP this[int index] => Table[(uint)index < Table.Length ? index : 0];
    public PersonalInfo8BDSP this[ushort species, byte form] => Table[GetFormIndex(species, form)];
    public PersonalInfo8BDSP GetFormEntry(ushort species, byte form) => Table[GetFormIndex(species, form)];

    public int GetFormIndex(ushort species, byte form)
    {
        if (species <= MaxSpecies)
            return Table[species].FormIndex(species, form);
        return 0;
    }

    public bool IsSpeciesInGame(ushort species) => species <= MaxSpecies;
    public bool IsPresentInGame(ushort species, byte form)
    {
        if (!IsSpeciesInGame(species))
            return false;
        if (form == 0)
            return true;
        if (Table[species].HasForm(form))
            return true;
        return false;
    }

    PersonalInfo IPersonalTable.this[int index] => this[index];
    PersonalInfo IPersonalTable.this[ushort species, byte form] => this[species, form];
    PersonalInfo IPersonalTable.GetFormEntry(ushort species, byte form) => GetFormEntry(species, form);
}
