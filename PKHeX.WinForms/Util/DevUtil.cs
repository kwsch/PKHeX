#if DEBUG
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.WinForms.Controls;

namespace PKHeX.WinForms;

public static class DevUtil
{
    public static void AddControl(ToolStripDropDownItem t)
    {
        t.DropDownItems.Add(GetTranslationUpdater());
    }

    private static readonly string[] Languages = ["ja", "fr", "it", "de", "es", "ko", "zh", "zh2"];
    private static string DefaultLanguage => Main.CurrentLanguage;

    public static bool IsUpdatingTranslations { get; private set; }

    /// <summary>
    /// Call this to update all translatable resources (Program Messages, Legality Text, Program GUI)
    /// </summary>
    private static void UpdateAll()
    {
        if (DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "Update translation files with current values?"))
            return;
        IsUpdatingTranslations = true;
        DumpStringsLegality();
        DumpStringsMessage();
        UpdateTranslations();
        IsUpdatingTranslations = false;
    }

    private static ToolStripMenuItem GetTranslationUpdater()
    {
        var ti = new ToolStripMenuItem
        {
            ShortcutKeys = Keys.Control | Keys.Alt | Keys.D,
            Visible = false,
        };
        ti.Click += (_, _) => UpdateAll();
        return ti;
    }

    private static void UpdateTranslations()
    {
        var assembly = System.Reflection.Assembly.GetExecutingAssembly();
        var types = assembly.GetTypes();

        // Trigger a translation then dump all.
        foreach (var lang in Languages) // get all languages ready to go
            _ = WinFormsTranslator.GetDictionary(lang);
        WinFormsTranslator.SetUpdateMode();
        WinFormsTranslator.LoadSettings<PKHeXSettings>(DefaultLanguage);
        WinFormsTranslator.LoadEnums(EnumTypesToTranslate, DefaultLanguage);
        WinFormsTranslator.LoadAllForms(types, LoadBanlist); // populate with every possible control
        WinFormsTranslator.TranslateControls(GetExtraControls(), DefaultLanguage);
        var dir = GetResourcePath("PKHeX.WinForms", "Resources", "text");
        WinFormsTranslator.DumpAll(DefaultLanguage, Banlist, dir); // dump current to file
        WinFormsTranslator.SetUpdateMode(false);

        // Move translated files from the debug exe loc to their project location
        var files = Directory.GetFiles(Application.StartupPath);
        foreach (var f in files)
        {
            var fn = Path.GetFileName(f);
            if (!fn.EndsWith(".txt"))
                continue;
            if (!fn.StartsWith("lang_"))
                continue;

            var loc = Path.Combine(dir, fn);
            if (File.Exists(loc))
                File.Delete(loc);
            File.Move(f, loc, true);
        }

        Application.Exit();
    }

    private static readonly Type[] EnumTypesToTranslate =
    [
        typeof(StatusCondition),
        typeof(PokeSize),
        typeof(PokeSizeDetailed),

        typeof(PassPower5),
        typeof(Funfest5Mission),
        typeof(OPower6Index),
        typeof(OPower6FieldType),
        typeof(OPower6BattleType),
        typeof(PlayerBattleStyle7),
        typeof(PlayerSkinColor7),
        typeof(Stamp7),
        typeof(FestivalPlazaFacilityColor),
    ];

    private static IEnumerable<Control> GetExtraControls()
    {
        foreach (var name in SlotList.GetEnumNames().Distinct())
            yield return new Label { Name = $"{nameof(Main)}.L_{name}", Text = name };
    }

    private static readonly string[] LoadBanlist =
    [
        nameof(SplashScreen),
        nameof(PokePreview),
    ];

    private static readonly string[] Banlist =
    [
        "Gender=", // editor gender labels
        "BTN_Shinytize", // ☆
        "Hidden_", // Hidden controls
        "CAL_", // calendar controls now expose Text, don't care.
        ".Count", // enum count
        $"{nameof(Main)}.L_SizeH", // height rating
        $"{nameof(Main)}.L_SizeW", // weight rating
        $"{nameof(Main)}.L_SizeS", // scale rating
        $"{nameof(Main)}.B_Box", // << and >> arrows
        $"{nameof(Main)}.L_Characteristic=", // Characterstic (dynamic)
        $"{nameof(Main)}.L_Potential", // ★☆☆☆ IV judge evaluation
        $"{nameof(SAV_HoneyTree)}.L_Tree0", // dynamic, don't bother
        $"{nameof(SAV_Misc3)}.BTN_Symbol", // symbols should stay as their current character
        $"{nameof(SAV_GameSelect)}.L_Prompt", // prompt text (dynamic)
        $"{nameof(SAV_BlockDump8)}.L_BlockName", // Block name (dynamic)
        $"{nameof(SAV_PokedexResearchEditorLA)}.L_", // Dynamic label
        $"{nameof(SAV_OPower)}.L_", // Dynamic label
    ];

    private static void DumpStringsMessage() => DumpStrings(typeof(MessageStrings), false, "PKHeX.Core", "Resources", "text", "program");
    private static void DumpStringsLegality() => DumpStrings(typeof(LegalityCheckStrings), true, "PKHeX.Core", "Resources", "legality", "checks");

    private static void DumpStrings(Type t, bool sorted, params string[] rel)
    {
        var dir = GetResourcePath(rel);
        DumpStrings(t, sorted, DefaultLanguage, dir);
        foreach (var lang in Languages)
            DumpStrings(t, sorted, lang, dir);
    }

    private static void DumpStrings(Type t, bool sorted, string lang, string dir)
    {
        LocalizationUtil.SetLocalization(t, lang);
        var entries = LocalizationUtil.GetLocalization(t);
        IEnumerable<string> export = entries.OrderBy(GetName); // sorted lines

        if (!sorted)
            export = entries;

        var location = GetFileLocationInText(t.Name, dir, lang);
        File.WriteAllLines(location, export);
        LocalizationUtil.SetLocalization(t, DefaultLanguage);

        static string GetName(string line)
        {
            var index = line.IndexOf('=');
            if (index == -1)
                return line;
            return line[..index];
        }
    }

    private static string GetFileLocationInText(string fileType, string dir, string lang)
    {
        var fn = $"{fileType}_{lang}.txt";
        return Path.Combine(dir, fn);
    }

    private static string GetResourcePath(params string[] subdir)
    {
        // Starting from the executable path, crawl upwards until we get to the repository/sln root
        const string repo = "PKHeX";
        var path = Application.StartupPath;
        while (true)
        {
            var parent = Directory.GetParent(path) ?? throw new DirectoryNotFoundException(path);
            path = parent.FullName;
            if (path.EndsWith(repo))
                return Path.Combine(path, Path.Combine(subdir));
        }
    }
}
#endif
