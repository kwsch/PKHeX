using System;
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
            "Main.label1=", // idk why this pops up but it does
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
            Clipboard.SetText(string.Join(Environment.NewLine, Util.GetLocalization(typeof(LegalityCheckStrings))));
        }
    }
    #endif
}
