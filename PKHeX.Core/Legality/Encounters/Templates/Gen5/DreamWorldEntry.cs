using System;

namespace PKHeX.Core;

/// <summary>
/// Intermediary Representation of Dream World Data
/// </summary>
public readonly record struct DreamWorldEntry(ushort Species, byte Level, ushort Move1 = 0, ushort Move2 = 0, ushort Move3 = 0, byte Form = 0, byte Gender = FixedGenderUtil.GenderRandom)
{
    private int EntryCount => Move1 == 0 ? 1 : Move2 == 0 ? 1 : Move3 == 0 ? 2 : 3;

    private void AddTo(GameVersion game, Span<EncounterStatic5Entree> result, ref int ctr)
    {
        var p = PersonalTable.B2W2[Species];
        var a = p.HasHiddenAbility ? AbilityPermission.OnlyHidden : AbilityPermission.OnlyFirst;
        if (Move1 == 0)
        {
            result[ctr++] = new EncounterStatic5Entree(game, Species, Level, Form, Gender, a);
            return;
        }

        result[ctr++] = new EncounterStatic5Entree(game, Species, Level, Form, Gender, a, Move1);
        if (Move2 == 0)
            return;
        result[ctr++] = new EncounterStatic5Entree(game, Species, Level, Form, Gender, a, Move2);
        if (Move3 == 0)
            return;
        result[ctr++] = new EncounterStatic5Entree(game, Species, Level, Form, Gender, a, Move3);
    }

    public static EncounterStatic5Entree[] GetArray(GameVersion game, ReadOnlySpan<DreamWorldEntry> t)
    {
        // Split encounters with multiple permitted special moves -- a pk can only be obtained with 1 of the special moves!
        int count = 0;
        foreach (var e in t)
            count += e.EntryCount;
        var result = new EncounterStatic5Entree[count];

        int ctr = 0;
        var tmp = result.AsSpan();
        foreach (var s in t)
            s.AddTo(game, tmp, ref ctr);
        return result;
    }
}
