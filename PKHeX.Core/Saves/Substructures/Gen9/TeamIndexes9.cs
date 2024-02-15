using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class TeamIndexes9(SAV9SV sav, SCBlock indexes, SCBlock locks) : ITeamIndexSet
{
    private const int TeamCount = 6;
    private const int NONE_SELECTED = -1;
    public readonly int[] TeamSlots = new int[TeamCount * 6];

    public void LoadBattleTeams()
    {
        if (!sav.State.Exportable)
        {
            ClearBattleTeams();
            return;
        }

        for (int i = 0; i < TeamCount * 6; i++)
        {
            short val = ReadInt16LittleEndian(indexes.Data.AsSpan(i * 2));
            if (val < 0)
            {
                TeamSlots[i] = NONE_SELECTED;
                continue;
            }

            int box = val >> 8;
            int slot = val & 0xFF;
            int index = (sav.BoxSlotCount * box) + slot;
            TeamSlots[i] = index & 0xFFFF;
        }
    }

    public void ClearBattleTeams()
    {
        for (int i = 0; i < TeamSlots.Length; i++)
            TeamSlots[i] = NONE_SELECTED;
    }

    public void UnlockAllTeams()
    {
        for (int i = 0; i < TeamCount; i++)
            SetIsTeamLocked(i, false);
    }

    public void SaveBattleTeams()
    {
        var span = indexes.Data.AsSpan();
        for (int i = 0; i < TeamCount * 6; i++)
        {
            int index = TeamSlots[i];
            if (index < 0)
            {
                WriteInt16LittleEndian(span[(i * 2)..], (short)index);
                continue;
            }

            sav.GetBoxSlotFromIndex(index, out var box, out var slot);
            index = (box << 8) | slot;
            WriteInt16LittleEndian(span[(i * 2)..], (short)index);
        }
    }

    public bool GetIsTeamLocked(int team) => FlagUtil.GetFlag(locks.Data, 0, team);
    public void SetIsTeamLocked(int team, bool value) => FlagUtil.SetFlag(locks.Data, 0, team, value);
}
