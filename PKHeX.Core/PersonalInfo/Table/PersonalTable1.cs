using System;

namespace PKHeX.Core;

/// <summary>
/// Personal Table storing <see cref="PersonalInfo1"/> used in Generation 1 games.
/// </summary>
public sealed class PersonalTable1 : IPersonalTable, IPersonalTable<PersonalInfo1>
{
    private readonly PersonalInfo1[] Table;
    private const int SIZE = PersonalInfo1.SIZE;
    private const ushort MaxSpecies = Legal.MaxSpeciesID_1;
    public ushort MaxSpeciesID => MaxSpecies;

    public PersonalTable1(Memory<byte> data)
    {
        Table = new PersonalInfo1[data.Length / SIZE];
        var count = data.Length / SIZE;
        for (int i = 0, ofs = 0; i < count; i++, ofs += SIZE)
        {
            var slice = data.Slice(ofs, SIZE);
            Table[i] = new PersonalInfo1(slice);
        }
    }

    public PersonalInfo1 this[int index] => Table[(uint)index < Table.Length ? index : 0];
    public PersonalInfo1 this[ushort species, byte form] => Table[GetFormIndex(species, form)];
    public PersonalInfo1 GetFormEntry(ushort species, byte form) => Table[GetFormIndex(species, form)];

    public int GetFormIndex(ushort species, byte form) => IsSpeciesInGame(species) ? species : 0;
    public bool IsSpeciesInGame(ushort species) => species <= MaxSpecies;
    public bool IsPresentInGame(ushort species, byte form) => form == 0 && IsSpeciesInGame(species);

    PersonalInfo IPersonalTable.this[int index] => this[index];
    PersonalInfo IPersonalTable.this[ushort species, byte form] => this[species, form];
    PersonalInfo IPersonalTable.GetFormEntry(ushort species, byte form) => GetFormEntry(species, form);

    /// <summary>
    /// Checks to see if either of the input type combinations exist in the table.
    /// </summary>
    /// <remarks>Only useful for checking Generation 1 <see cref="PK1.Type1"/> and <see cref="PK1.Type2"/> properties.</remarks>
    /// <param name="type1">First type</param>
    /// <param name="type2">Second type</param>
    /// <returns>Indication that the combination exists in the table.</returns>
    public int IsValidTypeCombination(byte type1, byte type2)
    {
        for (int i = 1; i <= MaxSpecies; i++)
        {
            var pi = Table[i];
            if (pi.IsValidTypeCombination(type1, type2))
                return i;
        }
        return -1;
    }

    /// <summary>
    /// <inheritdoc cref="IsValidTypeCombination(byte, byte)"/>
    /// </summary>
    /// <param name="other">Type tuple to search for.</param>
    public int IsValidTypeCombination(IPersonalType other) => IsValidTypeCombination(other.Type1, other.Type2);

    /// <summary>
    /// Checks if the type matches any of the type IDs extracted from the Personal Table used for R/G/B/Y games.
    /// </summary>
    /// <remarks>Valid values: 0, 1, 2, 3, 4, 5, 7, 8, 20, 21, 22, 23, 24, 25, 26</remarks>
    public static bool TypeIDExists(byte type) => type < 32 && (0b111111100000000000110111111 & (1 << type)) != 0;
}
