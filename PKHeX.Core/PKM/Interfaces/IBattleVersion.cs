using System;

namespace PKHeX.Core;

/// <summary>
/// Interface that exposes a <see cref="BattleVersion"/> for allowing a Pokémon into ranked battles if it originated from a prior game.
/// </summary>
public interface IBattleVersion
{
    /// <summary>
    /// Indicates which <see cref="GameVersion"/> the Pokémon's moves were reset on.
    /// </summary>
    GameVersion BattleVersion { get; set; }
}

public static class BattleVersionExtensions
{
    /// <summary>
    /// Checks if the applied Battle Version value is valid based on visitation.
    /// </summary>
    public static bool IsBattleVersionValid<T>(this T pk, EvolutionHistory h) where T : PKM, IBattleVersion => pk.BattleVersion switch
    {
        0 => true,
        GameVersion.SW or GameVersion.SH => h.HasVisitedSWSH && LocationsHOME.GetVersionSWSH(pk.Version) is not (GameVersion.SW or GameVersion.SH),
        _ => false,
    };

    /// <summary>
    /// Resets the <see cref="PKM"/>'s moves and sets the requested version.
    /// </summary>
    /// <param name="v">Reference to the object to set the <see cref="version"/></param>
    /// <param name="pk">Reference to the same object that gets moves reset</param>
    /// <param name="version">Version to apply</param>
    public static void AdaptToBattleVersion(this IBattleVersion v, PKM pk, GameVersion version)
    {
        var empty = new Moveset();
        pk.SetMoves(empty);
        pk.SetRelearnMoves(empty);

        Span<ushort> moves = stackalloc ushort[4];
        var source = GameData.GetLearnSource(version);
        source.SetEncounterMoves(pk.Species, pk.Form, pk.CurrentLevel, moves);
        pk.SetMoves(moves);
        pk.FixMoves();
        v.BattleVersion = version;
    }

    /// <summary>
    /// Gets the minimum Generation ID that it was last reset in.
    /// </summary>
    public static int GetMinGeneration(this IBattleVersion v)
    {
        var version = v.BattleVersion;
        if (version == 0)
            return 1;
        if (!version.IsValidSavedVersion())
            return -1;
        var gen = version.GetGeneration();
        if (gen >= 8)
            return gen;
        return -1;
    }
}
