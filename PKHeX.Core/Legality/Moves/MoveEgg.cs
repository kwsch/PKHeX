using System;
using System.Collections.Generic;
using static PKHeX.Core.Legal;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core;

public static class MoveEgg
{
    public static ushort[] GetEggMoves(ushort species, byte form, GameVersion version, int generation)
    {
        if (!Breeding.CanGameGenerateEggs(version))
            return Array.Empty<ushort>();

        return GetEggMoves(generation, species, form, version);
    }

    public static ushort[] GetEggMoves(int generation, ushort species, byte form, GameVersion version) => generation switch
    {
        1 or 2 => GetMovesSafe(version == C ? EggMovesC : EggMovesGS, species),
        3 => GetMovesSafe(EggMovesRS, species),
        4 when version is D or P or Pt => GetMovesSafe(EggMovesDPPt, species),
        4 when version is HG or SS => GetMovesSafe(EggMovesHGSS, species),
        5 => GetMovesSafe(EggMovesBW, species),

        6 when version is X or Y => GetMovesSafe(EggMovesXY, species),
        6 when version is OR or AS => GetMovesSafe(EggMovesAO, species),

        7 when version is SN or MN => GetFormEggMoves(species, form, EggMovesSM),
        7 when version is US or UM => GetFormEggMoves(species, form, EggMovesUSUM),
        8 when version is SW or SH => GetFormEggMoves(species, form, EggMovesSWSH),
        8 when version is BD or SP => GetMovesSafe(EggMovesBDSP, species),
        _ => Array.Empty<ushort>(),
    };

    private static ushort[] GetMovesSafe<T>(IReadOnlyList<T> moves, ushort species) where T : EggMoves
    {
        if (species >= moves.Count)
            return Array.Empty<ushort>();
        return moves[species].Moves;
    }

    public static ushort[] GetFormEggMoves(ushort species, byte form, IReadOnlyList<EggMoves7> table)
    {
        if (species >= table.Count)
            return Array.Empty<ushort>();

        var entry = table[species];
        if (form <= 0 || entry.FormTableIndex <= species)
            return entry.Moves;

        // Sanity check form in the event it is out of range.
        var baseIndex = entry.FormTableIndex;
        var index = baseIndex + form - 1;
        if ((uint)index >= table.Count)
            return Array.Empty<ushort>();
        entry = table[index];
        if (entry.FormTableIndex != baseIndex)
            return Array.Empty<ushort>();

        return entry.Moves;
    }
}
