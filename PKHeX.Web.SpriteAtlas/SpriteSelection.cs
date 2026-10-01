using PKHeX.Core;
using PKHeX.Web.Services.Sprites;

namespace PKHeX.Web.SpriteAtlas;

/// <summary>
/// Chooses which images go into the atlas: every image <see cref="SpriteKeys"/> can pick for an X/Y or Omega Ruby/Alpha Sapphire entity.
/// </summary>
/// <remarks>
/// Every species of Generation 6 is combined with every form its Omega Ruby/Alpha Sapphire personal data lists (a superset of X/Y's),
/// every gender and both shininesses, and resolved against all the desktop's images. What is resolved is exactly what the app can show
/// for a valid entity, so the atlas holds what the desktop would draw rather than everything whose file name looks relevant.
/// An entity with a form outside that range still gets a sprite: the species' default with the unknown mark, as on the desktop.
/// </remarks>
public static class SpriteSelection
{
    /// <summary>Highest species a Generation 6 save can hold.</summary>
    public static ushort MaxSpecies => PersonalTable.AO.MaxSpeciesID;

    /// <summary>The resource names to pack, in ordinal order.</summary>
    /// <exception cref="InvalidDataException">An image every sprite may need is missing from <paramref name="index"/>.</exception>
    public static IReadOnlyList<string> Select(ResxSpriteIndex index)
    {
        var missing = SpriteKeys.Fixed.Where(k => !index.Contains(k)).ToArray();
        if (missing.Length != 0)
        {
            throw new InvalidDataException($"The desktop images lack {string.Join(", ", missing)}.");
        }

        var keys = new SortedSet<string>(SpriteKeys.Fixed, StringComparer.Ordinal);
        foreach (var layers in Enumerate(index.Contains))
        {
            keys.Add(layers.Base);
        }
        // A stored form outside the listed range falls back to the species' default sprite, so each species needs its own.
        for (ushort species = 1; species <= MaxSpecies; species++)
        {
            if (index.Contains($"b_{species}"))
            {
                keys.Add($"b_{species}");
            }
        }
        return [.. keys];
    }

    /// <summary>The layers of every species, form, gender and shininess combination, resolved with <paramref name="exists"/>.</summary>
    public static IEnumerable<SpriteLayers> Enumerate(Func<string, bool> exists)
    {
        var table = PersonalTable.AO;
        for (ushort species = 1; species <= MaxSpecies; species++)
        {
            var forms = table.GetFormEntry(species, 0).FormCount;
            for (byte form = 0; form < Math.Max((byte)1, forms); form++)
            {
                for (byte gender = 0; gender <= 2; gender++)
                {
                    yield return SpriteKeys.Resolve(species, form, gender, false, false, false, exists)!;
                    yield return SpriteKeys.Resolve(species, form, gender, true, false, false, exists)!;
                }
            }
        }
    }
}
