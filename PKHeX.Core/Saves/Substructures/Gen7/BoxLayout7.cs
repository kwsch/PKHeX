using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class BoxLayout7(SAV7 sav, Memory<byte> raw) : SaveBlock<SAV7>(sav, raw),
    IBoxDetailName, IBoxDetailWallpaper, ITeamIndexSet
{
    private const int BoxCount = 32;

    private const int BattleBoxFlags      = 0x4C4;
    private const int PCBackgrounds       = 0x5C0;
    private const int PCFlags             = 0x5E0;
    private const int Unlocked            = 0x5E1;
    private const int LastViewedBoxOffset = 0x5E3;

    private const int StringMaxLength = SAV6.LongStringLength / 2;

    private const int TeamCount = 6;
    private const int NONE_SELECTED = -1;
    public readonly int[] TeamSlots = new int[TeamCount * 6];

    public static int GetBoxWallpaperOffset(int box) => PCBackgrounds + box;

    public int GetBoxWallpaper(int box)
    {
        if ((uint)box >= SAV.BoxCount)
            return 0;
        return Data[GetBoxWallpaperOffset(box)];
    }

    public void SetBoxWallpaper(int box, int value)
    {
        if ((uint)box >= SAV.BoxCount)
            return;
        Data[GetBoxWallpaperOffset(box)] = (byte)value;
    }

    private Span<byte> GetBoxNameSpan(int box) => Data.Slice(GetBoxNameOffset(box), SAV6.LongStringLength);
    private static int GetBoxNameOffset(int box) => (SAV6.LongStringLength * box);
    public string GetBoxName(int box) => SAV.GetString(GetBoxNameSpan(box));
    public void SetBoxName(int box, ReadOnlySpan<char> value) => SAV.SetString(GetBoxNameSpan(box), value, StringMaxLength, StringConverterOption.ClearZero);

    public byte[] BoxFlags
    {
        get => [ Data[PCFlags] ]; // bits for wallpaper unlocks
        set
        {
            if (value.Length != 1)
                return;
            Data[PCFlags] = value[0];
        }
    }

    public int BoxesUnlocked
    {
        get => Data[Unlocked];
        set
        {
            if (value > BoxCount)
                value = BoxCount;
            Data[Unlocked] = (byte)value;
        }
    }

    public int CurrentBox { get => Data[LastViewedBoxOffset]; set => Data[LastViewedBoxOffset] = (byte)value; }

    public void LoadBattleTeams()
    {
        for (int i = 0; i < TeamCount * 6; i++)
        {
            short val = ReadInt16LittleEndian(Data[(BattleBoxFlags + (i * 2))..]);
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
        for (int i = 0; i < TeamCount; i++)
            SetIsTeamLocked(i, false);
    }

    public void SaveBattleTeams()
    {
        var span = Data[BattleBoxFlags..];
        for (int i = 0; i < TeamCount * 6; i++)
        {
            int index = TeamSlots[i];
            if (index < 0)
            {
                WriteInt16LittleEndian(span[(i*2)..], (short)index);
                continue;
            }

            SAV.GetBoxSlotFromIndex(index, out var box, out var slot);
            index = (box << 8) | slot;
            WriteInt16LittleEndian(span[(i * 2)..], (short)index);
        }
    }

    public void UnlockAllTeams()
    {
        for (int i = 0; i < TeamCount; i++)
            SetIsTeamLocked(i, false);
    }

    public bool GetIsTeamLocked(int team) => Data[PCBackgrounds - TeamCount - team] == 1;
    public void SetIsTeamLocked(int team, bool value) => Data[PCBackgrounds - TeamCount - team] = value ? (byte)1 : (byte)0;

    public string this[int i]
    {
        get => GetBoxName(i);
        set => SetBoxName(i, value);
    }
}
