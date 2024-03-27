using System;

namespace PKHeX.Core;

/// <summary>
/// Personal Table storing <see cref="PersonalInfo3"/> used in Generation 3 games.
/// </summary>
public sealed class PersonalTable3 : IPersonalTable, IPersonalTable<PersonalInfo3>
{
    private readonly PersonalInfo3[] Table;
    private const int SIZE = PersonalInfo3.SIZE;
    private const ushort MaxSpecies = Legal.MaxSpeciesID_3;
    public ushort MaxSpeciesID => MaxSpecies;

    public PersonalTable3(Memory<byte> data)
    {
        Table = new PersonalInfo3[data.Length / SIZE];
        var count = data.Length / SIZE;
        for (int i = 0, ofs = 0; i < count; i++, ofs += SIZE)
        {
            var slice = data.Slice(ofs, SIZE);
            Table[i] = new PersonalInfo3(slice);
        }
    }

    public PersonalInfo3 this[int index] => Table[(uint)index < Table.Length ? index : 0];
    public PersonalInfo3 this[ushort species, byte form] => Table[GetFormIndex(species, form)];
    public PersonalInfo3 GetFormEntry(ushort species, byte form) => Table[GetFormIndex(species, form)];

    public int GetFormIndex(ushort species, byte form) => IsSpeciesInGame(species) ? species : 0;
    public bool IsSpeciesInGame(ushort species) => species <= MaxSpecies;
    public bool IsPresentInGame(ushort species, byte form)
    {
        if (!IsSpeciesInGame(species))
            return false;
        return form == 0 || species switch
        {
            (int)Species.Unown => form < 28,
            (int)Species.Castform => form < 4,
            (int)Species.Deoxys => form < 4,
            _ => false,
        };
    }

    PersonalInfo IPersonalTable.this[int index] => this[index];
    PersonalInfo IPersonalTable.this[ushort species, byte form] => this[species, form];
    PersonalInfo IPersonalTable.GetFormEntry(ushort species, byte form) => GetFormEntry(species, form);

    internal void LoadTables(BinLinkerAccessor machine, BinLinkerAccessor tutors)
    {
        var table = Table;
        for (int i = Legal.MaxSpeciesID_3; i >= 1; i--)
        {
            var entry = table[i];
            entry.AddTMHM(machine[i]);
            entry.AddTypeTutors(tutors[i]);
        }
    }

    internal void CopyTables(PersonalTable3 pt)
    {
        // Copy to other tables
        var other = pt.Table;
        var table = Table;
        for (int i = Legal.MaxSpeciesID_3; i >= 1; i--)
            table[i].CopyFrom(other[i]);
    }
}
