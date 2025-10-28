namespace PKHeX.Core;

/// <summary>
/// Interface for Accessing named blocks within a Generation 6 save file.
/// </summary>
/// <remarks>Blocks specific for <see cref="SAV6XY"/> in addition to the <see cref="ISaveBlock6Main"/> blocks.</remarks>
public interface ISaveBlock6XY : ISaveBlock6Main
{
    Misc6XY Misc { get; }
    Zukan6XY Zukan { get; }
    Fashion6XY Fashion { get; }
    BerryField6XY BerryField { get; }
}
