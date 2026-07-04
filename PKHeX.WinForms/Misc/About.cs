using System.Windows.Forms;

namespace PKHeX.WinForms;

public partial class About : Form
{
    public About(AboutPage index = AboutPage.Changelog)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        RTB_Changelog.Text = Properties.Resources.changelog;
        RTB_Shortcuts.Text = GetShortcutsText(Main.CurrentLanguage);
        TC_About.SelectedIndex = (int)index;
    }

    private static string GetShortcutsText(string lang)
    {
        var localized = Properties.Resources.ResourceManager.GetObject($"shortcuts_{lang}") as string;
        return localized ?? Properties.Resources.shortcuts;
    }
}

public enum AboutPage
{
    Shortcuts,
    Changelog,
}
