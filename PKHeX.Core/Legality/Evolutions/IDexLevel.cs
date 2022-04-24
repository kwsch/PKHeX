namespace PKHeX.Core;

/// <summary>
/// Small general purpose value passing object with misc data pertaining to an encountered Species.
/// </summary>
public interface IDexLevel
{
    ushort Species { get; }
    byte Form { get; }
    byte LevelMax { get; }
}
