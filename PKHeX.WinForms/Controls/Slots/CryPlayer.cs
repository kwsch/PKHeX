using System.IO;
using System.Media;
using PKHeX.Core;
using PKHeX.Drawing;

namespace PKHeX.WinForms.Controls
{
    public class CryPlayer
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
            var name = SpriteName.GetResourceStringSprite(pk.Species, pk.AltForm, pk.Gender, pk.Format).Replace('_', '-').Substring(1);
            var path = Path.Combine(cryFolder, $"{name}.wav");
            if (!File.Exists(path))
                path = Path.Combine(cryFolder, $"{pk.Species}.wav");
            return path;
        }
    }
}