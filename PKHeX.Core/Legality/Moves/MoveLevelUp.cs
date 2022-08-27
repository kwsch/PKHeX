using System;

using static PKHeX.Core.GameVersion;

namespace PKHeX.Core;

public static class MoveLevelUp
{
    public static ushort[] GetEncounterMoves(PKM pk, int level, GameVersion version)
    {
        if (version <= 0)
            version = (GameVersion)pk.Version;
        return GetEncounterMoves(pk.Species, pk.Form, level, version);
    }

    private static ushort[] GetEncounterMoves1(ushort species, int level, GameVersion version)
    {
        var learn = GameData.GetLearnsets(version);
        var table = GameData.GetPersonal(version);
        var index = table.GetFormIndex(species, 0);

        Span<ushort> lvl0 = stackalloc ushort[4];
        ((PersonalInfo1) table[index]).GetMoves(lvl0);
        int start = Math.Max(0, lvl0.IndexOf((ushort)0));

        learn[index].SetEncounterMoves(level, lvl0, start);
        return lvl0.ToArray();
    }

    private static ushort[] GetEncounterMoves2(ushort species, int level, GameVersion version)
    {
        var learn = GameData.GetLearnsets(version);
        var table = GameData.GetPersonal(version);
        var index = table.GetFormIndex(species, 0);
        var lvl0 = learn[species].GetEncounterMoves(1);
        int start = Math.Max(0, Array.IndexOf(lvl0, (ushort)0));

        learn[index].SetEncounterMoves(level, lvl0, start);
        return lvl0;
    }

    public static ushort[] GetEncounterMoves(ushort species, byte form, int level, GameVersion version)
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
