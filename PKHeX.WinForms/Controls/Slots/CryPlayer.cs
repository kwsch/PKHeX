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

        public void PlayCry(PKM pk)
        {
            if (pk.Species == 0)
                return;

            string path = GetCryPath(pk, Main.CryPath);
            if (!File.Exists(path))
                return;

            Sounds.SoundLocation = path;
            try { Sounds.Play(); }
#pragma warning disable CA1031 // Do not catch general exception types
            catch { Debug.WriteLine("Failed to play sound."); }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        public void Stop() => Sounds.Stop();

        private static string GetCryPath(PKM pk, string cryFolder)
        {
            var name = GetCryFileName(pk);
            var path = Path.Combine(cryFolder, $"{name}.wav");
            if (!File.Exists(path))
                path = Path.Combine(cryFolder, $"{pk.Species}.wav");
            return path;
        }

        private static string GetCryFileName(PKM pk)
        {
            if (pk.Species == (int)Species.Urshifu && pk.Form == 1) // same sprite for both forms, but different cries
                return "892-1";

            // don't grab sprite of pkm, no gender specific cries
            var res = SpriteName.GetResourceStringSprite(pk.Species, pk.Form, 0, 0, pk.Format);
            return res.Replace('_', '-') // people like - instead of _ file names ;)
                .Substring(1); // skip leading underscore
        }
    }
}