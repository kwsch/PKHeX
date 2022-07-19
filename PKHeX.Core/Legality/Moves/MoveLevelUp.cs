using System;

using static PKHeX.Core.GameVersion;

namespace PKHeX.Core;

public static class MoveLevelUp
{
    public static int[] GetEncounterMoves(PKM pk, int level, GameVersion version)
    {
        if (version <= 0)
            version = (GameVersion)pk.Version;
        return GetEncounterMoves(pk.Species, pk.Form, level, version);
    }

    private static int[] GetEncounterMoves1(int species, int level, GameVersion version)
    {
        var learn = GameData.GetLearnsets(version);
        var table = GameData.GetPersonal(version);
        var index = table.GetFormIndex(species, 0);

        Span<int> lvl0 = stackalloc int[4];
        ((PersonalInfo1) table[index]).GetMoves(lvl0);
        int start = Math.Max(0, lvl0.IndexOf(0));

        learn[index].SetEncounterMoves(level, lvl0, start);
        return lvl0.ToArray();
    }

    private static int[] GetEncounterMoves2(int species, int level, GameVersion version)
    {
        var learn = GameData.GetLearnsets(version);
        var table = GameData.GetPersonal(version);
        var index = table.GetFormIndex(species, 0);
        var lvl0 = learn[species].GetEncounterMoves(1);
        int start = Math.Max(0, Array.IndexOf(lvl0, 0));

        learn[index].SetEncounterMoves(level, lvl0, start);
        return lvl0;
    }

    public static int[] GetEncounterMoves(int species, int form, int level, GameVersion version)
    {
        if (RBY.Contains(version))
            return GetEncounterMoves1(species, level, version);
        if (GSC.Contains(version))
            return GetEncounterMoves2(species, level, version);
        var learn = GameData.GetLearnsets(version);
        var table = GameData.GetPersonal(version);
        var index = table.GetFormIndex(species, form);
        return learn[index].GetEncounterMoves(level);
    }
}
