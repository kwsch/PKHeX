using System;
using System.Collections.Generic;
using static PKHeX.Core.Legal;
using static PKHeX.Core.GameVersion;

namespace PKHeX.Core;

public static class MoveEgg
{
    public static int[] GetEggMoves(int species, int form, GameVersion version, int generation)
    {
        if (species > GetMaxSpeciesOrigin(generation, version))
            return Array.Empty<int>();

        if (!Breeding.CanGameGenerateEggs(version))
            return Array.Empty<int>();

        return GetEggMoves(generation, species, form, version);
    }

    public static int[] GetEggMoves(int generation, int species, int form, GameVersion version) => generation switch
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
        _ => Array.Empty<int>(),
    };

    private static int[] GetMovesSafe<T>(IReadOnlyList<T> moves, int species) where T : EggMoves
    {
        if ((uint)species >= moves.Count)
            return Array.Empty<int>();
        return moves[species].Moves;
    }

    public static int[] GetFormEggMoves(int species, int form, IReadOnlyList<EggMoves7> table)
    {
        if ((uint)species >= table.Count)
            return Array.Empty<int>();

        var entry = table[species];
        if (form <= 0 || entry.FormTableIndex <= species)
            return entry.Moves;

        // Sanity check form in the event it is out of range.
        var baseIndex = entry.FormTableIndex;
        var index = baseIndex + form - 1;
        if ((uint)index >= table.Count)
            return Array.Empty<int>();
        entry = table[index];
        if (entry.FormTableIndex != baseIndex)
            return Array.Empty<int>();

        return entry.Moves;
    }

    internal static int[] GetRelearnLVLMoves(PKM pk, int species, int form, int lvl, GameVersion version = Any)
    {
        if (version == Any)
            version = (GameVersion)pk.Version;
        // A pk can only have levelup relearn moves from the game it originated on
        // eg Plusle/Minun have Charm/Fake Tears (respectively) only in OR/AS, not X/Y
        return version switch
        {
            X or Y => getMoves(LevelUpXY, PersonalTable.XY),
            OR or AS => getMoves(LevelUpAO, PersonalTable.AO),
            SN or MN when species <= MaxSpeciesID_7 => getMoves(LevelUpSM, PersonalTable.SM),
            US or UM => getMoves(LevelUpUSUM, PersonalTable.USUM),
            SW or SH => getMoves(LevelUpSWSH, PersonalTable.SWSH),
            BD or SP => getMoves(LevelUpBDSP, PersonalTable.BDSP),
            PLA => getMoves(LevelUpLA, PersonalTable.LA),
            _ => Array.Empty<int>(),
        };

        int[] getMoves(IReadOnlyList<Learnset> moves, PersonalTable table) => moves[table.GetFormIndex(species, form)].GetMoves(lvl);
    }
}
