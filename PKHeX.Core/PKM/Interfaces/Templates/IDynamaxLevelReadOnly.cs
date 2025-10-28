using static PKHeX.Core.Species;

namespace PKHeX.Core;

public interface IDynamaxLevelReadOnly
{
    /// <summary>
    /// Dynamax Level used by <see cref="GameVersion.SWSH"/> format entity data.
    /// </summary>
    byte DynamaxLevel { get; }
}

/// <summary>
/// Extension methods for <see cref="IDynamaxLevelReadOnly"/>.
/// </summary>
public static class DynamaxLevelExtensions
{
    /// <summary>
    /// Checks if the species is allowed to have a non-zero value for <see cref="IDynamaxLevel.DynamaxLevel"/>.
    /// </summary>
    public static bool CanHaveDynamaxLevel(this IDynamaxLevelReadOnly _, PKM pk)
    {
        if (pk.IsEgg)
            return false;
        return pk is PK8 && CanHaveDynamaxLevel(pk.Species);
    }

    public static byte GetSuggestedDynamaxLevel(this IDynamaxLevelReadOnly _, PKM pk, byte requested = 10) => _.CanHaveDynamaxLevel(pk) ? requested : (byte)0;

    /// <summary>
    /// Checks if the species is prevented from gaining any <see cref="IDynamaxLevelReadOnly.DynamaxLevel"/> via candy in <see cref="GameVersion.SWSH"/>.
    /// </summary>
    private static bool CanHaveDynamaxLevel(ushort species)
    {
        return species is not ((int)Zacian or (int)Zamazenta or (int)Eternatus);
    }
}
