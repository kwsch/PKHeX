using System.Windows.Forms;

namespace PKHeX.WinForms
{
    public partial class About : Form
    {
        public About(int index = 0)
        {
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            RTB_Changelog.Text = Properties.Resources.changelog;
            RTB_Shortcuts.Text = Properties.Resources.shortcuts;
            TC_About.SelectedIndex = index;
        }
    }
}
