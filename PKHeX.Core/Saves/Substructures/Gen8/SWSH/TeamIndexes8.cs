using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class TeamIndexes8 : ITeamIndexSet
{
    private const int TeamCount = 6;
    private const int NONE_SELECTED = -1;
    private readonly SaveFile SAV;
    private readonly SCBlock Indexes;
    private readonly SCBlock Locks;
    public readonly int[] TeamSlots = new int[TeamCount * 6];

    private TeamIndexes8(SaveFile sav, SCBlock indexes, SCBlock locks)
    {
        SAV = sav;
        Indexes = indexes;
        Locks = locks;
    }

    public TeamIndexes8(SAV8SWSH sav, SCBlock indexes, SCBlock locks) : this((SaveFile)sav, indexes, locks) { }
    public TeamIndexes8(SAV9SV sav, SCBlock indexes, SCBlock locks) : this((SaveFile)sav, indexes, locks) { }

    public void LoadBattleTeams()
    {
        if (!SAV.State.Exportable)
        {
            ClearBattleTeams();
            return;
        }

        for (int i = 0; i < TeamCount * 6; i++)
        {
            short val = ReadInt16LittleEndian(Indexes.Data.AsSpan(i * 2));
            if (val < 0)
            {
                TeamSlots[i] = NONE_SELECTED;
                continue;
            }

            int box = val >> 8;
            int slot = val & 0xFF;
            int index = (SAV.BoxSlotCount * box) + slot;
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
        var span = Indexes.Data.AsSpan();
        for (int i = 0; i < TeamCount * 6; i++)
        {
            int index = TeamSlots[i];
            if (index < 0)
            {
                WriteInt16LittleEndian(span[(i * 2)..], (short)index);
                continue;
            }

            SAV.GetBoxSlotFromIndex(index, out var box, out var slot);
            index = (box << 8) | slot;
            WriteInt16LittleEndian(span[(i * 2)..], (short)index);
        }
    }

    public bool GetIsTeamLocked(int team) => FlagUtil.GetFlag(Locks.Data, 0, team);
    public void SetIsTeamLocked(int team, bool value) => FlagUtil.SetFlag(Locks.Data, 0, team, value);
}
