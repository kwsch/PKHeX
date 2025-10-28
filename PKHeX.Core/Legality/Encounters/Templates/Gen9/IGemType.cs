namespace PKHeX.Core;

/// <summary>
/// Interface for encounter-able entities that can be obtained with a specified (or unspecified) <see cref="ITeraType.TeraTypeOriginal"/>.
/// </summary>
public interface IGemType
{
    GemType TeraType { get; } // 0 = Default, 1 = Random, 2++ => [0..types] = specific type
}

/// <summary>
/// Possible values the encounter may have.
/// </summary>
public enum GemType : byte
{
    /// <summary> Default value, first type when encountered. </summary>
    Default = 0,
    /// <summary> Random type [0,17] </summary>
    Random = 1,

    Normal = 2,
    Fighting = 3,
    Flying = 4,
    Poison = 5,
    Ground = 6,
    Rock = 7,
    Bug = 8,
    Ghost = 9,
    Steel = 10,
    Fire = 11,
    Water = 12,
    Grass = 13,
    Electric = 14,
    Psychic = 15,
    Ice = 16,
    Dragon = 17,
    Dark = 18,
    Fairy = 19,
    Stellar = 101,
}

/// <summary>
/// Extension methods for <see cref="GemType"/>.
/// </summary>
public static class GemTypeExtensions
{
    /// <summary>
    /// Gets the <see cref="expect"/> type that corresponds to the input <see cref="type"/>.
    /// </summary>
    /// <returns>False if no specific value is expected.</returns>
    public static bool IsSpecified(this GemType type, out byte expect)
    {
        if (type is GemType.Default or GemType.Random)
        {
            expect = 0;
            return false;
        }
        expect = (byte)(type - 2);
        return true;
    }
}
