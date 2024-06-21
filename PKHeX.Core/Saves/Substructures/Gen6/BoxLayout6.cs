using System;

namespace PKHeX.Core;

public sealed class BoxLayout6 : SaveBlock<SAV6>, IBoxDetailName, IBoxDetailWallpaper
{
    // gfstr5[31] boxNames;
    // byte[31] wallpapers;
    // byte Flags:7;
    // byte FinalBoxUnlocked:1;
    // byte UnlockedCount;
    // byte CurrentBox;

    private const int StringMaxByteCount = SAV6.LongStringLength; // same for both games
    private const int StringMaxLength = StringMaxByteCount / 2;
    private const int BoxCount = 31;
    private const int PCBackgrounds = BoxCount * StringMaxByteCount; // 0x41E;
    private const int PCFlags = PCBackgrounds + BoxCount;      // 0x43D;
    private const int Unlocked = PCFlags + 1;                  // 0x43E;
    private const int LastViewedBoxOffset = Unlocked + 1;      // 0x43F;

    public BoxLayout6(SAV6XY sav, Memory<byte> raw) : base(sav, raw) { }
    public BoxLayout6(SAV6AO sav, Memory<byte> raw) : base(sav, raw) { }

    private static int GetBoxWallpaperOffset(int box) => PCBackgrounds + box;

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

    private static int GetBoxNameOffset(int box) => (StringMaxByteCount * box);

    public string GetBoxName(int box) => SAV.GetString(Data.Slice(GetBoxNameOffset(box), StringMaxByteCount));

    public void SetBoxName(int box, ReadOnlySpan<char> value)
    {
        var span = Data.Slice(GetBoxNameOffset(box) + (StringMaxByteCount * box), StringMaxByteCount);
        SAV.SetString(span, value, StringMaxLength, StringConverterOption.ClearZero);
    }

    public byte[] BoxFlags
    {
        get => [ Data[PCFlags] ]; // 7 bits for wallpaper unlocks, top bit to unlock final box (delta episode)
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
            if (value == BoxCount)
                Data[PCFlags] |= 0x80; // set final box unlocked flag
            else
                Data[PCFlags] &= 0x7F; // clear final box unlocked flag
            Data[Unlocked] = (byte)value;
        }
    }

    public int CurrentBox { get => Data[LastViewedBoxOffset]; set => Data[LastViewedBoxOffset] = (byte)value; }
}
