using System.Windows.Forms;

namespace PKHeX.WinForms;

public partial class SplashScreen : Form
{
    public SplashScreen() => InitializeComponent();

    private bool CanClose;

    private void SplashScreen_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (!CanClose)
            e.Cancel = true;
    }

    public void ForceClose()
    {
        CanClose = true;
        Close();
    }
}
