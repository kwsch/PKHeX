using System.Diagnostics;
using System.IO;
using System.Media;
using PKHeX.Core;
using PKHeX.Drawing.PokeSprite;

namespace PKHeX.WinForms.Controls;

public sealed class CryPlayer
{
    private readonly SoundPlayer Sounds = new();

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

    public void Stop()
    {
        if (string.IsNullOrWhiteSpace(Sounds.SoundLocation))
            return;

        try { Sounds.Stop(); }
        catch { Debug.WriteLine("Failed to stop sound."); }
    }

    private static string GetCryPath(ISpeciesForm pk, string cryFolder, EntityContext context)
    {
        var name = GetCryFileName(pk, context);
        var path = Path.Combine(cryFolder, $"{name}.wav");
        if (!File.Exists(path))
            path = Path.Combine(cryFolder, $"{pk.Species}.wav");
        return path;
    }

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
