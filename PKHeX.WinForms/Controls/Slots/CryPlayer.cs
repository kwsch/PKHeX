using System.Diagnostics;
using System.IO;
using System.Media;
using PKHeX.Core;
using PKHeX.Drawing;

namespace PKHeX.WinForms.Controls
{
    public sealed class CryPlayer
    {
        private readonly SoundPlayer Sounds = new();

        public void PlayCry(ISpeciesForm pk, int format)
        {
            if (pk.Species == 0)
                return;

            string path = GetCryPath(pk, Main.CryPath, format);
            if (!File.Exists(path))
                return;

            Sounds.SoundLocation = path;
            try { Sounds.Play(); }
#pragma warning disable CA1031 // Do not catch general exception types
            catch { Debug.WriteLine("Failed to play sound."); }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        public void Stop()
        {
            if (string.IsNullOrWhiteSpace(Sounds.SoundLocation))
                return;

            try { Sounds.Stop(); }
#pragma warning disable CA1031 // Do not catch general exception types
            catch { Debug.WriteLine("Failed to stop sound."); }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        private static string GetCryPath(ISpeciesForm pk, string cryFolder, int format)
        {
            var name = GetCryFileName(pk, format);
            var path = Path.Combine(cryFolder, $"{name}.wav");
            if (!File.Exists(path))
                path = Path.Combine(cryFolder, $"{pk.Species}.wav");
            return path;
        }

        private static string GetCryFileName(ISpeciesForm pk, int format)
        {
            if (pk.Species == (int)Species.Urshifu && pk.Form == 1) // same sprite for both forms, but different cries
                return "892-1";

            // don't grab sprite of pkm, no gender specific cries
            var res = SpriteName.GetResourceStringSprite(pk.Species, pk.Form, 0, 0, format);

            // people like - instead of _ file names ;)
            return res.Replace('_', '-')[1..]; // skip leading underscore
        }
    }
}
