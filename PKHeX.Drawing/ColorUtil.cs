using System;
using System.Drawing;

namespace PKHeX.Drawing;

public static class ColorUtil
{
    public static Color ColorBaseStat(int v)
    {
        const float maxval = 180; // shift the green cap down
        float x = 100f * v / maxval;
        if (x > 100)
            x = 100;
        double red = 255f * (x > 50 ? 1 - (2 * (x - 50) / 100.0) : 1.0);
        double green = 255f * (x > 50 ? 1.0 : 2 * x / 100.0);

        return Blend(Color.FromArgb((int)red, (int)green, 0), Color.White, 0.4);
    }

    public static Color ColorBaseStatTotal(int tot) => ColorBaseStat((int) (Math.Max(0, tot - 175) / 3f));

    public static Color Blend(Color color, Color backColor, double amount)
    {
        byte r = (byte)((color.R * amount) + (backColor.R * (1 - amount)));
        byte g = (byte)((color.G * amount) + (backColor.G * (1 - amount)));
        byte b = (byte)((color.B * amount) + (backColor.B * (1 - amount)));
        return Color.FromArgb(r, g, b);
    }
}
