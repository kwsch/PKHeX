using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using PKHeX.WinForms.Properties;

namespace PKHeX.WinForms;

public static class FontUtil
{
    private static readonly PrivateFontCollection CustomFonts = new();
    private static readonly Dictionary<float, Font> GeneratedFonts = [];

    static FontUtil()
    {
        string g6path = Path.Combine(Path.GetTempPath(), "pgldings6.ttf");
        try
        {
            if (!File.Exists(g6path))
                File.WriteAllBytes(g6path, Resources.pgldings_normalregular);
            CustomFonts.AddFontFile(g6path);
        }
        catch (FileNotFoundException ex)
        {
            Debug.WriteLine($"Unable to read font file: {ex.Message}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to add in-game font: {ex.Message}");
        }
    }

    public static Font GetPKXFont(float size = 11f)
    {
        if (GeneratedFonts.TryGetValue(size, out var f))
            return f;
        var family = CustomFonts.Families.Length == 0 ? FontFamily.GenericSansSerif : CustomFonts.Families[0];
        var font = new Font(family, size);
        GeneratedFonts.Add(size, font);
        return font;
    }
}
