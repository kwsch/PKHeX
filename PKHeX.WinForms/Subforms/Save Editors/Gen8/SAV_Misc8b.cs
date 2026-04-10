using System;
using System.Windows.Forms;
using PKHeX.Core;
namespace PKHeX.WinForms;

public partial class SAV_Misc8b : Form
{
    private readonly SAV8BS Origin;
    private readonly SAV8BS SAV;
    private readonly EventUnlocker8b Unlocker;

    public SAV_Misc8b(SAV8BS sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (SAV8BS)(Origin = sav).Clone();
        Unlocker = new EventUnlocker8b(SAV);

        ReadMain();
    }

    private void B_Cancel_Click(object sender, EventArgs e) => Close();

    private void B_Save_Click(object sender, EventArgs e)
    {
        SaveMain();
        Origin.CopyChangesFrom(SAV);
        Close();
    }

    private void ReadMain()
    {
        B_Spiritomb.Enabled = Unlocker.UnlockReadySpiritomb;
        B_Darkrai.Enabled = Unlocker.UnlockReadyDarkrai;
        B_Shaymin.Enabled = Unlocker.UnlockReadyShaymin;
        B_Arceus.Enabled = Unlocker.UnlockReadyArceus;
        B_DialgaPalkia.Enabled = Unlocker.UnlockReadyBoxLegend;
        B_Roamer.Enabled = Unlocker.ResetReadyRoamerMesprit || Unlocker.ResetReadyRoamerCresselia;

        B_RebattleEyecatch.Enabled = SAV.BattleTrainer.AnyDefeated;
        B_DefeatEyecatch.Enabled = SAV.BattleTrainer.AnyUndefeated;
    }

    private void SaveMain()
    {
    }

    private void B_Spiritomb_Click(object sender, EventArgs e)
    {
        Unlocker.UnlockSpiritomb();
        WinFormsUtil.Asterisk();
        B_Spiritomb.Enabled = Unlocker.UnlockReadySpiritomb;
    }

    private void B_Shaymin_Click(object sender, EventArgs e)
    {
        Unlocker.UnlockShaymin();
        WinFormsUtil.Asterisk();
        B_Shaymin.Enabled = Unlocker.UnlockReadyShaymin;
    }

    private void B_Darkrai_Click(object sender, EventArgs e)
    {
        Unlocker.UnlockDarkrai();
        WinFormsUtil.Asterisk();
        B_Darkrai.Enabled = Unlocker.UnlockReadyDarkrai;
    }

    private void B_Arceus_Click(object sender, EventArgs e)
    {
        Unlocker.UnlockArceus();
        WinFormsUtil.Asterisk();
        B_Arceus.Enabled = Unlocker.UnlockReadyArceus;
    }

    private void B_DialgaPalkia_Click(object sender, EventArgs e)
    {
        Unlocker.UnlockBoxLegend();
        WinFormsUtil.Asterisk();
        B_DialgaPalkia.Enabled = Unlocker.UnlockReadyBoxLegend;
    }

    private void B_Roamer_Click(object sender, EventArgs e)
    {
        Unlocker.RespawnRoamer();
        WinFormsUtil.Asterisk();
        B_Roamer.Enabled = Unlocker.ResetReadyRoamerMesprit || Unlocker.ResetReadyRoamerCresselia;
    }

    private void B_Zones_Click(object sender, EventArgs e)
    {
        Unlocker.UnlockZones();
        WinFormsUtil.Asterisk();
        B_Zones.Enabled = false;
    }

    private void B_DefeatEyecatch_Click(object sender, EventArgs e)
    {
        SAV.BattleTrainer.DefeatAll();
        WinFormsUtil.Asterisk();
        B_DefeatEyecatch.Enabled = false;
        B_RebattleEyecatch.Enabled = true;
    }

    private void B_RebattleEyecatch_Click(object sender, EventArgs e)
    {
        SAV.BattleTrainer.RebattleAll();
        WinFormsUtil.Asterisk();
        B_RebattleEyecatch.Enabled = false;
        B_DefeatEyecatch.Enabled = true;
    }

    private void B_Fashion_Click(object sender, EventArgs e)
    {
        Unlocker.UnlockFashion();
        WinFormsUtil.Asterisk();
        B_Fashion.Enabled = false;
    }
}
