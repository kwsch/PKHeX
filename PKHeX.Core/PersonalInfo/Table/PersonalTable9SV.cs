using System;

namespace PKHeX.Core;

/// <summary>
/// Personal Table storing <see cref="PersonalTable9SV"/> used in <see cref="GameVersion.SV"/>.
/// </summary>
public sealed class PersonalTable9SV : IPersonalTable, IPersonalTable<PersonalInfo9SV>
{
    private readonly PersonalInfo9SV[] Table;
    private const int SIZE = PersonalInfo9SV.SIZE;
    private const ushort MaxSpecies = Legal.MaxSpeciesID_9;
    public ushort MaxSpeciesID => MaxSpecies;

    public PersonalTable9SV(Memory<byte> data)
    {
        Table = new PersonalInfo9SV[data.Length / SIZE];
        var count = data.Length / SIZE;
        for (int i = 0, ofs = 0; i < count; i++, ofs += SIZE)
        {
            var slice = data.Slice(ofs, SIZE);
            Table[i] = new PersonalInfo9SV(slice);
        }
    }

    public PersonalInfo9SV this[int index] => Table[(uint)index < Table.Length ? index : 0];
    public PersonalInfo9SV this[ushort species, byte form] => Table[GetFormIndex(species, form)];
    public PersonalInfo9SV GetFormEntry(ushort species, byte form) => Table[GetFormIndex(species, form)];

    public int GetFormIndex(ushort species, byte form)
    {
        if (species <= MaxSpeciesID)
            return Table[species].FormIndex(species, form);
        return 0;
    }

    public bool IsSpeciesInGame(ushort species)
    {
        if (species > MaxSpeciesID)
            return false;

        var form0 = Table[species];
        if (form0.IsPresentInGame)
            return true;

        var fc = form0.FormCount;
        for (byte i = 1; i < fc; i++)
        {
            var entry = GetFormEntry(species, i);
            if (entry.IsPresentInGame)
                return true;
        }
        return false;
    }

    public bool IsPresentInGame(ushort species, byte form)
    {
        if (species > MaxSpeciesID)
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
    PersonalInfo IPersonalTable.this[ushort species, byte form] => this[species, form];
    PersonalInfo IPersonalTable.GetFormEntry(ushort species, byte form) => GetFormEntry(species, form);
}
