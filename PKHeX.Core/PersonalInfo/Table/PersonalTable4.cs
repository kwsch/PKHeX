using System;

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

    public PersonalInfo4 this[int index] => Table[index];
    public PersonalInfo4 this[int species, int form] => Table[species];
    public PersonalInfo4 GetFormEntry(int species, int form) => Table[GetFormIndex(species, form)];

    public int GetFormIndex(int species, int form) => species;
    public bool IsSpeciesInGame(int species) => (uint)species <= MaxSpecies;
    public bool IsPresentInGame(int species, int form) => IsSpeciesInGame(species) && form == 0;

    PersonalInfo IPersonalTable.this[int index] => this[index];
    PersonalInfo IPersonalTable.this[int species, int form] => this[species, form];
    PersonalInfo IPersonalTable.GetFormEntry(int species, int form) => GetFormEntry(species, form);
}
