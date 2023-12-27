namespace PKHeX.Core;

/// <summary>
/// Interface for Accessing named blocks within a Generation 7 save file.
/// </summary>
/// <remarks>Blocks specific for <see cref="SAV7USUM"/> in addition to the <see cref="ISaveBlock7Main"/> blocks.</remarks>
public interface ISaveBlock7USUM : ISaveBlock7Main
{
    BattleAgency7 BattleAgency { get; }
    // FinderStudioSave
}
