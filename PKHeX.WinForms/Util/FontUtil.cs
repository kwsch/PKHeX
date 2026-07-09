using PKHeX.Core;
using PKHeX.WinForms.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;

namespace PKHeX.WinForms;

/// <summary>
/// Font cache for in-game character rendering.
/// The font is only loaded once per context/size combination.
/// </summary>
public static class FontUtil
{
    private static readonly PrivateFontCollection CustomFonts = new();
    private static readonly Dictionary<(string FamilyName, float Size), Font> GeneratedFonts = [];
    private static readonly Dictionary<string, FontFamily> RegisteredFamilies = [];

    public const EntityContext DefaultContext = EntityContext.Gen6;

    public static Font GetFont(EntityContext context = DefaultContext)
    {
        var (familyName, resourceName, size) = GetFontFileName(context);

        // Check if we've already instantiated this font already.
        var fetch = (FontName: familyName, size);
        if (GeneratedFonts.TryGetValue(fetch, out var f))
            return f;

        var family = RegisterFamily(familyName, resourceName);
        var font = new Font(family, size, FontStyle.Regular, GraphicsUnit.Point);
        GeneratedFonts.Add((family.Name, size), font);
        return font;
    }

    private static FontFamily RegisterFamily(string familyName, string resourceName)
    {
        // If already populated for this context, return that rather than fetching again.
        if (RegisteredFamilies.TryGetValue(familyName, out var family))
            return family;

        // Might not have a font for every context; may be mapped to the same as another context.
        var resource = Resources.ResourceManager.GetObject(resourceName);
        if (resource is not byte[] fontBytes)
            throw new InvalidOperationException($"Font resource '{resourceName}' not found.");

        var path = Path.Combine(Path.GetTempPath(), $"{resourceName}.bin");
        if (!File.Exists(path))
            File.WriteAllBytes(path, fontBytes);
        AddFont(path);

        // doesn't return the family reference; grab it now
        var loaded = Array.Find(CustomFonts.Families, z => z.Name == familyName)
         ?? throw new InvalidOperationException($"Font family '{familyName}' not found in custom fonts.");

        return RegisteredFamilies[familyName] = loaded;
    }

    // declare fonts here; any not present will fall back to Gen6.
    private static (string FamilyName, string ResourceName, float Size) GetFontFileName(EntityContext context) => context switch
    {
        // Internal name, program resource name, scaling to make it look OK in GUI.
        _ => ("PGLDings", "pgldings_normalregular", 13f), // Gen6
    };

    private static void AddFont(string path)
    {
        try
        {
            if (!File.Exists(path))
                File.WriteAllBytes(path, Resources.pgldings_normalregular);
            CustomFonts.AddFontFile(path);
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
}
