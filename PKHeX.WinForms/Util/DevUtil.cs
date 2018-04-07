using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    #if DEBUG
    public static class DevUtil
    {
        private static readonly string[] Languages = {"ja", "fr", "it", "de", "es", "ko", "zh", "pt"};
        private const string DefaultLanguage = "en";
        public static void UpdateTranslations()
        {
            WinFormsTranslator.LoadAllForms(LoadBanlist); // populate with every possible control
            WinFormsTranslator.UpdateAll(DefaultLanguage, Languages); // propagate to others
            WinFormsTranslator.DumpAll(Banlist); // dump current to file
            WinFormsTranslator.SetRemovalMode(); // remove used keys, don't add any
            WinFormsTranslator.LoadAllForms(LoadBanlist); // de-populate
            WinFormsTranslator.RemoveAll(DefaultLanguage, PurgeBanlist); // remove all lines from above generated files that still remain

            Application.Exit();
        }

        private static readonly string[] LoadBanlist =
        {
            nameof(SplashScreen),
        };

        private static readonly string[] Banlist =
        {
            nameof(SplashScreen),
            "Gender=", // editor gender labels
            "BTN_Shinytize", // ☆
            "Main.B_Box", // << and >> arrows
            "Main.L_Characteristic=", // Characterstic (dynamic)
            "Main.L_Potential", // ★☆☆☆ IV judge evaluation
            "SAV_FolderList.", // don't translate that form's buttons, only title
            "SAV_HoneyTree.L_Tree0", // dynamic, don't bother
            "SAV_Misc3.BTN_Symbol", // symbols should stay as their current character
        };

        private static readonly string[] PurgeBanlist =
        {
            nameof(SuperTrainingEditor),
            nameof(ErrorWindow),
            nameof(SettingsEditor),
        };

        public static void DumpLegalityStrings()
        {
            var langs = new[] {DefaultLanguage}.Concat(Languages);
            foreach (var lang in langs)
            {
                Util.SetLocalization(typeof(LegalityCheckStrings), lang);
                var entries = Util.GetLocalization(typeof(LegalityCheckStrings));
                var export = entries.Select(z => new {Variable = z.Split('=')[0], Line = z})
                    .GroupBy(z => z.Variable.Length) // fancy sort!
                    .OrderBy(z => z.Key) // sort by length (V1 = 2, V100 = 4)
                    .SelectMany(z => z.OrderBy(n => n.Variable)) // select sets from ordered Names
                    .Select(z => z.Line); // sorted lines

                var fn = $"{nameof(LegalityCheckStrings)}_{lang}.txt";
                File.WriteAllLines(fn, export);
                Util.SetLocalization(typeof(LegalityCheckStrings), DefaultLanguage);
            }
        }
    }
    #endif
}
