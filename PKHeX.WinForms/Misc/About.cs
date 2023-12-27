using System.Windows.Forms;

namespace PKHeX.WinForms;

public partial class About : Form
{
    public About(AboutPage index = AboutPage.Changelog)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        RTB_Changelog.Text = Properties.Resources.changelog;
        RTB_Shortcuts.Text = Properties.Resources.shortcuts;
        TC_About.SelectedIndex = (int)index;
    }
}

public enum AboutPage
{
    Shortcuts,
    Changelog,
}
