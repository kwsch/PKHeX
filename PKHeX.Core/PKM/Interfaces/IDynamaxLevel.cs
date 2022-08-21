using static PKHeX.Core.Species;

namespace PKHeX.Core;

/// <summary>
/// Exposes info about Dynamax potential
/// </summary>
public interface IDynamaxLevel
{
    /// <summary>
    /// Dynamax Level used by <see cref="GameVersion.SWSH"/> format entity data.
    /// </summary>
    byte DynamaxLevel { get; set; }
}

public static class DynamaxLevelExtensions
{
    /// <summary>
    /// Checks if the species is allowed to have a non-zero value for <see cref="IDynamaxLevel.DynamaxLevel"/>.
    /// </summary>
    public static bool CanHaveDynamaxLevel(this IDynamaxLevel _, PKM pk)
    {
        if (pk.IsEgg)
            return false;
        return pk is PK8 && CanHaveDynamaxLevel(pk.Species);
    }

    public static byte GetSuggestedDynamaxLevel(this IDynamaxLevel _, PKM pk, byte requested = 10) => _.CanHaveDynamaxLevel(pk) ? requested : (byte)0;

    /// <summary>
    /// Checks if the species is prevented from gaining any <see cref="IDynamaxLevel.DynamaxLevel"/> via candy in <see cref="GameVersion.SWSH"/>.
    /// </summary>
    private static bool CanHaveDynamaxLevel(int species)
    {
        return species is not ((int)Zacian or (int)Zamazenta or (int)Eternatus);
    }
}
