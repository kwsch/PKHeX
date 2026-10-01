using PKHeX.Web.SpriteAtlas;

// Usage: PKHeX.Web.SpriteAtlas --resx <PKHeX.Drawing.PokeSprite/Properties/Resources.resx> --out <directory>
// Writes the atlas, its stylesheet, the manifest and the provenance list. Any failure is reported and exits with a non-zero code,
// so the publish that runs this fails rather than shipping a partial catalog.
string? resx = null;
string? output = null;
for (var i = 0; i < args.Length; i++)
{
    switch (args[i])
    {
        case "--resx" when i + 1 < args.Length:
            resx = args[++i];
            break;
        case "--out" when i + 1 < args.Length:
            output = args[++i];
            break;
        default:
            Console.Error.WriteLine($"Unknown or incomplete argument: {args[i]}");
            return 2;
    }
}
if (resx is null || output is null)
{
    Console.Error.WriteLine("Usage: PKHeX.Web.SpriteAtlas --resx <Resources.resx> --out <directory>");
    return 2;
}

try
{
    var index = ResxSpriteIndex.Load(resx);
    var names = SpriteSelection.Select(index);
    // Provenance paths are relative to the PokeSprite project, which holds Properties/Resources.resx.
    var projectRoot = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Path.GetFullPath(resx))!, ".."));
    var files = AtlasWriter.Build(index, names, projectRoot);
    AtlasWriter.Write(files, output);
    Console.WriteLine($"Packed {files.Entries.Count} sprites into {files.AtlasFileName} ({files.Atlas.Length} bytes) in {output}.");
    return 0;
}
catch (Exception e) when (e is InvalidDataException or IOException or UnauthorizedAccessException or System.Xml.XmlException)
{
    Console.Error.WriteLine($"Sprite atlas generation failed: {e.Message}");
    return 1;
}
