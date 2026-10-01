using System.Xml.Linq;

namespace PKHeX.Web.SpriteAtlas;

/// <summary>
/// The images PKHeX.Drawing.PokeSprite embeds, by resource name, as its <c>Properties/Resources.resx</c> lists them.
/// </summary>
/// <remarks>
/// The desktop looks sprites up by resource name, and the resx is what maps each name to its file (<c>b_100_1</c> is
/// <c>Big Pokemon Sprites/b_100-1.png</c>), so it is read rather than file names being guessed.
/// </remarks>
public sealed class ResxSpriteIndex
{
    private readonly Dictionary<string, string> files;

    private ResxSpriteIndex(Dictionary<string, string> files) => this.files = files;

    /// <summary>Resource names, in ordinal order.</summary>
    public IEnumerable<string> Names => files.Keys.Order(StringComparer.Ordinal);

    /// <summary>Whether a resource of this name exists.</summary>
    public bool Contains(string name) => files.ContainsKey(name);

    /// <summary>The full path of the image file behind resource <paramref name="name"/>.</summary>
    /// <exception cref="KeyNotFoundException">There is no such resource.</exception>
    public string PathOf(string name) => files[name];

    /// <summary>
    /// Reads the bitmap file references of the resx at <paramref name="resxPath"/>. Paths in it are relative to its own directory.
    /// </summary>
    /// <exception cref="InvalidDataException">The resx has no bitmap references, or one is malformed or listed twice.</exception>
    public static ResxSpriteIndex Load(string resxPath)
    {
        var directory = Path.GetDirectoryName(Path.GetFullPath(resxPath))!;
        var result = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var data in XDocument.Load(resxPath).Root!.Elements("data"))
        {
            // Only file references to bitmaps are sprites; strings and other resources are skipped.
            var type = (string?)data.Attribute("type") ?? "";
            var value = (string?)data.Element("value") ?? "";
            if (!type.StartsWith("System.Resources.ResXFileRef", StringComparison.Ordinal) || !value.Contains(";System.Drawing.Bitmap", StringComparison.Ordinal))
            {
                continue;
            }
            var name = (string?)data.Attribute("name");
            var relative = value[..value.IndexOf(';')];
            if (string.IsNullOrEmpty(name) || relative.Length == 0)
            {
                throw new InvalidDataException("A bitmap reference in the resx has no name or no file.");
            }
            // The resx is written on Windows; its separators are backslashes.
            var path = Path.GetFullPath(Path.Combine(directory, relative.Replace('\\', Path.DirectorySeparatorChar)));
            if (!result.TryAdd(name, path))
            {
                throw new InvalidDataException($"The resx lists {name} twice.");
            }
        }
        if (result.Count == 0)
        {
            throw new InvalidDataException("The resx lists no bitmap files.");
        }
        return new ResxSpriteIndex(result);
    }
}
