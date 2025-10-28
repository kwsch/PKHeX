using System;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public partial class SAV_Misc2 : Form
{
    private readonly SaveFile Origin;
    private readonly SAV2 SAV;

    public SAV_Misc2(SAV2 sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (SAV2)(Origin = sav).Clone();

        B_VirtualConsoleGSBall.Visible = SAV.Version is GameVersion.C;
        B_VirtualConsoleGSBall.Enabled = !SAV.IsEnabledGSBallMobileEvent;
    }

    private void B_Save_Click(object sender, EventArgs e)
    {
        Origin.CopyChangesFrom(SAV);
        Close();
    }

    private void B_Cancel_Click(object sender, EventArgs e) => Close();

    private void B_VirtualConsoleGSBall_Click(object sender, EventArgs e)
    {
        // Don't bother checking if the save is from Virtual Console.
        // Can be moved between VC and GB era, and can be a quick way to enable the event on either.
        SAV.EnableGSBallMobileEvent();
        B_VirtualConsoleGSBall.Enabled = false;
        System.Media.SystemSounds.Asterisk.Play();
    }
}
