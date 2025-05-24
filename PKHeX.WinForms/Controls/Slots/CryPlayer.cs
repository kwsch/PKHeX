using System.Diagnostics;
using System.IO;
using System.Media;
using PKHeX.Core;
using PKHeX.Drawing.PokeSprite;

namespace PKHeX.WinForms.Controls;

/// <summary>
/// Provides functionality to play Pokémon cries using sound files.
/// </summary>
public sealed class CryPlayer
{
    /// <summary>
    /// The <see cref="SoundPlayer"/> instance used to play sound files.
    /// </summary>
    private readonly SoundPlayer Sounds = new();

    /// <summary>
    /// Plays the cry for the specified Pokémon species and form.
    /// </summary>
    /// <param name="pk">The Pokémon species and form information.</param>
    /// <param name="context">The entity context (game generation).</param>
    public void PlayCry(ISpeciesForm pk, EntityContext context)
    {
        if (pk.Species == 0)
            return;

        string path = GetCryPath(pk, Main.CryPath, context);
        if (!File.Exists(path))
            return;

        Sounds.SoundLocation = path;
        try { Sounds.Play(); }
        catch { Debug.WriteLine("Failed to play sound."); }
    }

    /// <summary>
    /// Stops any currently playing cry.
    /// </summary>
    public void Stop()
    {
        if (string.IsNullOrWhiteSpace(Sounds.SoundLocation))
            return;

        try { Sounds.Stop(); }
        catch { Debug.WriteLine("Failed to stop sound."); }
    }

    /// <summary>
    /// Gets the file path for the cry sound file for the specified Pokémon.
    /// </summary>
    /// <param name="pk">The Pokémon species and form information.</param>
    /// <param name="cryFolder">The folder containing cry sound files.</param>
    /// <param name="context">The entity context (game generation).</param>
    /// <returns>The file path to the cry sound file.</returns>
    private static string GetCryPath(ISpeciesForm pk, string cryFolder, EntityContext context)
    {
        var name = GetCryFileName(pk, context);
        var path = Path.Combine(cryFolder, $"{name}.wav");
        if (!File.Exists(path))
            path = Path.Combine(cryFolder, $"{pk.Species}.wav");
        return path;
    }

    /// <summary>
    /// Gets the file name for the cry sound file for the specified Pokémon.
    /// </summary>
    /// <param name="pk">The Pokémon species and form information.</param>
    /// <param name="context">The entity context (game generation).</param>
    /// <returns>The file name for the cry sound file.</returns>
    private static string GetCryFileName(ISpeciesForm pk, EntityContext context)
    {
        if (pk is { Species: (int)Species.Urshifu, Form: 1 }) // same sprite for both forms, but different cries
            return "892-1";

        // don't grab sprite of pk, no gender-specific cries
        var res = SpriteName.GetResourceStringSprite(pk.Species, pk.Form, 0, 0, context);

        // people like - instead of _ file names ;)
        return res.Replace('_', '-')[1..]; // skip leading underscore
    }
}
