using System.Windows.Forms;

namespace PKHeX.WinForms
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            RTB_Changelog.Text = Properties.Resources.changelog;
            RTB_Shortcuts.Text = Properties.Resources.shortcuts;
        }
    }
}
