using System;

namespace PKHeX.Core;

public sealed class BoxLayout5(SAV5 sav, Memory<byte> raw) : SaveBlock<SAV5>(sav, raw)
{
    public int CurrentBox { get => Data[0]; set => Data[0] = (byte)value; }
    private static int GetBoxNameOffset(int box) => (0x28 * box) + 4;
    private static int GetBoxWallpaperOffset(int box) => 0x3C4 + box;

    public int GetBoxWallpaper(int box)
    {
        if ((uint)box > SAV.BoxCount)
            return 0;
        return Data[GetBoxWallpaperOffset(box)];
    }

    public void SetBoxWallpaper(int box, int value)
    {
        if ((uint)box > SAV.BoxCount)
            return;
        Data[GetBoxWallpaperOffset(box)] = (byte)value;
    }

    private Span<byte> GetBoxNameSpan(int box) => Data.Slice(GetBoxNameOffset(box), 0x14);

    public string GetBoxName(int box)
    {
        if (box >= SAV.BoxCount)
            return string.Empty;
        return SAV.GetString(GetBoxNameSpan(box));
    }

    public void SetBoxName(int box, ReadOnlySpan<char> value)
    {
        SAV.SetString(GetBoxNameSpan(box), value, 13, StringConverterOption.ClearZero);
    }

    public byte BoxesUnlocked
    {
        get => Data[0x3DD];
        set
        {
            if (value > SAV.BoxCount)
                value = (byte)SAV.BoxCount;
            Data[0x3DD] = value;
        }
    }

    public string this[int i]
    {
        get => GetBoxName(i);
        set => SetBoxName(i, value);
    }
}
