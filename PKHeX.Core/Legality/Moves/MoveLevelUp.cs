using System;

using static PKHeX.Core.GameVersion;

namespace PKHeX.Core;

public static class MoveLevelUp
{
    public static void GetEncounterMoves(Span<ushort> moves, PKM pk, int level, GameVersion version)
    {
        if (version <= 0)
            version = (GameVersion)pk.Version;
        GetEncounterMoves(moves, pk.Species, pk.Form, level, version);
    }

    private static void GetEncounterMoves1(Span<ushort> result, ushort species, int level, GameVersion version)
    {
        var learn = GameData.GetLearnsets(version);
        var table = version is YW or RBY ? PersonalTable.Y : PersonalTable.RB;
        var index = table.GetFormIndex(species, 0);

        // The initial moves are seeded from Personal rather than learn.
        table[index].GetMoves(result);
        int start = Math.Max(0, result.IndexOf((ushort)0));

        learn[index].SetEncounterMoves(level, result, start);
    }

    public static void GetEncounterMoves(Span<ushort> result, ushort species, byte form, int level, GameVersion version)
    {
        if (RBY.Contains(version))
        {
            GetEncounterMoves1(result, species, level, version);
        }
        else
        {
            var learn = GameData.GetLearnsets(version);
            var table = GameData.GetPersonal(version);
            var index = table.GetFormIndex(species, form);
            learn[index].SetEncounterMoves(level, result);
        }
    }

    public static ushort[] GetEncounterMoves(ushort species, byte form, int level, GameVersion version)
    {
        var result = new ushort[4];
        GetEncounterMoves(result, species, form, level, version);
        return result;
    }
}
