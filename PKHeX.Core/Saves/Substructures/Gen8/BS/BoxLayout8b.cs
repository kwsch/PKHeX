using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Information about the storage boxes.
/// </summary>
/// <remarks>Structure name: SaveBoxData, size: 0x64A</remarks>
public sealed class BoxLayout8b(SAV8BS sav, Memory<byte> raw) : SaveBlock<SAV8BS>(sav, raw), IBoxDetailName
{
    public const int BoxCount = 40;

    private const int StringMaxLength = SAV6.LongStringLength / 2;
    public readonly int[] TeamSlots = new int[TeamCount * TeamSlotCount];
    private const int TeamNameLength = 0x16;

    private static int GetBoxNameOffset(int box) => SAV6.LongStringLength * box;
    private static int GetTeamNameOffset(int box) => GetBoxNameOffset(BoxCount) + (TeamNameLength * box);

    public string GetBoxName(int box)
    {
        var span = Data.Slice(GetBoxNameOffset(box), SAV6.LongStringLength);
        if (ReadUInt16LittleEndian(span) == 0)
            return BoxDetailNameExtensions.GetDefaultBoxName(box);
        return SAV.GetString(span);
    }

    public void SetBoxName(int box, ReadOnlySpan<char> value)
    {
        var span = Data.Slice(GetBoxNameOffset(box), SAV6.LongStringLength);
        SAV.SetString(span, value, StringMaxLength, StringConverterOption.ClearZero);
    }

    public string GetTeamName(int team)
    {
        var offset = GetTeamNameOffset(team);
        var span = Data.Slice(offset, TeamNameLength);
        if (ReadUInt16LittleEndian(span) == 0)
            return $"Team {team + 1}";
        return SAV.GetString(span);
    }

    public void SetTeamName(int team, ReadOnlySpan<char> value)
    {
        var offset = GetTeamNameOffset(team);
        var span = Data.Slice(offset, TeamNameLength);
        SAV.SetString(span, value, TeamNameLength/2, StringConverterOption.ClearZero);
    }

    public string this[int i]
    {
        get => GetBoxName(i);
        set => SetBoxName(i, value);
    }

    public const int TeamPositionOffset = 0x5D4;
    public const int TeamCount = 6;
    public const int TeamSlotCount = 6;
    private const short NONE_SELECTED = -1;

    public void LoadBattleTeams()
    {
        for (int i = 0; i < TeamCount * TeamSlotCount; i++)
        {
            short val = ReadInt16LittleEndian(Data[(TeamPositionOffset + (i * 2))..]);
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
        LockedTeam = 0;
    }

    public void SaveBattleTeams()
    {
        var span = Data[TeamPositionOffset..];
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

    // bitflags
    public byte LockedTeam
    {
        get => Data[0x61C];
        set
        {
            if (value > BoxCount)
                value = BoxCount;
            Data[0x61C] = value;
        }
    }

    public byte BoxesUnlocked
    {
        get => Data[0x61D];
        set
        {
            if (value > BoxCount)
                value = BoxCount;
            Data[0x61D] = value;
        }
    }

    public byte CurrentBox
    {
        get => Data[0x61E];
        set => Data[0x61E] = value;
    }

    public bool GetIsTeamLocked(int team) => (LockedTeam & (1 << team)) != 0;

    public static int GetBoxWallpaperOffset(int box) => 0x620 + box;

    public int GetBoxWallpaper(int box)
    {
        if ((uint)box >= BoxCount)
            return 0;
        return Data[GetBoxWallpaperOffset(box)] - 1;
    }

    public void SetBoxWallpaper(int box, int value)
    {
        if ((uint)box >= BoxCount)
            return;
        Data[GetBoxWallpaperOffset(box)] = (byte)(value + 1);
    }

    public ushort StatusPut
    {
        get => ReadUInt16LittleEndian(Data[0x648..]);
        set => WriteUInt16LittleEndian(Data[0x648..], value);
    }
}
