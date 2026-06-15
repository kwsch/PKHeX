namespace PKHeX.Core;

public record struct SpeciesForm10(ushort Value) : ISpeciesForm
{
    // 10 bits species
    public ushort Species { get => (ushort)(Value & 0x3FF); set => Value = (ushort)((Value & ~0x3FF) | (value & 0x3FF)); }
    public byte Form { get => (byte)((Value >> 10) & 0x3F); set => Value = (ushort)((Value & ~(0x3Fu << 10)) | ((((uint)value & 0x3F) << 10))); }

    /// <summary>
    /// Useful sanity check.
    /// </summary>
    public bool IsValid => Value is 0 || (Species != 0 && PersonalTable.HGSS.IsPresentInGame(Species, Form));

    public static implicit operator SpeciesForm10(ushort value) => new(value);
    public static implicit operator ushort(SpeciesForm10 value) => value.Value;
}
