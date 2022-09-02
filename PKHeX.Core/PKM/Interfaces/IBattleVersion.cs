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
    byte BattleVersion { get; set; }
}

public static class BattleVersionExtensions
{
    public static bool IsBattleVersionValid<T>(this T pk, EvolutionHistory h) where T : PKM, IBattleVersion => pk.BattleVersion switch
    {
        0 => true,
        (int)GameVersion.SW or (int)GameVersion.SH => !(pk.SWSH || pk.BDSP || pk.LA) && h.HasVisitedSWSH,
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
        MoveLevelUp.GetEncounterMoves(moves, pk, pk.CurrentLevel, version);
        pk.SetMoves(moves);
        pk.FixMoves();
        v.BattleVersion = (byte) version;
    }

    public static int GetMinGeneration(this IBattleVersion v)
    {
        var ver = v.BattleVersion;
        if (ver == 0)
            return 1;
        var game = (GameVersion) ver;
        if (!game.IsValidSavedVersion())
            return -1;
        var gen = game.GetGeneration();
        if (gen >= 8)
            return gen;
        return -1;
    }
}
