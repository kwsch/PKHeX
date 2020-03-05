using System.IO;
using System.Media;
using PKHeX.Core;
using PKHeX.Drawing;

namespace PKHeX.WinForms.Controls
{
    public sealed class CryPlayer
    {
        private readonly SoundPlayer Sounds = new SoundPlayer();

        public void PlayCry(PKM pk)
        {
            if (pk.Species == 0)
                return;

            string path = GetCryPath(pk, Main.CryPath);
            if (!File.Exists(path))
                return;

            Sounds.SoundLocation = path;
            try { Sounds.Play(); }
            catch { }
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
            // don't grab sprite of pkm, no gender specific cries
            var res = SpriteName.GetResourceStringSprite(pk.Species, pk.AltForm, 0, 0, pk.Format);
            return res.Replace('_', '-') // people like - instead of _ file names ;)
                .Substring(1); // skip leading underscore
        }
    }
}