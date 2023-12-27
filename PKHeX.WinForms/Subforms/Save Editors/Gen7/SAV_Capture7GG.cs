using System;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public partial class SAV_Capture7GG : Form
{
    private readonly SaveFile Origin;
    private readonly SAV7b SAV;

    private readonly Zukan7b Dex;
    private readonly CaptureRecords Captured;
    private ushort Index;
    private bool Loading;

    public SAV_Capture7GG(SAV7b sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (SAV7b)(Origin = sav).Clone();
        Dex = SAV.Blocks.Zukan;
        Captured = SAV.Blocks.Captured;

        Loading = true;
        // Clear Listbox and ComboBox
        LB_Species.Items.Clear();
        CB_Species.Items.Clear();

        // Fill List
        var species = GameInfo.FilteredSources.Species.Where(z => IsLegalSpecies(z.Value)).ToList();
        CB_Species.InitializeBinding();
        CB_Species.DataSource = new BindingSource(species, null);
        foreach (var (text, value) in species.OrderBy(z => z.Value))
            LB_Species.Items.Add($"{value:000}: {text}");

        GetTotals();
        CB_Species.KeyDown += WinFormsUtil.RemoveDropCB;
        LB_Species.SelectedIndex = Index = 0;
        GetEntry();
        Loading = false;
    }

    private static bool IsLegalSpecies(int species) => species is >= 1 and (<= 151 or 808 or 809);

    private void ChangeCBSpecies(object sender, EventArgs e)
    {
        if (Loading)
            return;
        SetEntry();

        var species = (ushort)WinFormsUtil.GetIndex(CB_Species);
        Index = CaptureRecords.GetSpeciesIndex(species);
        Loading = true;
        LB_Species.SelectedIndex = Index;
        LB_Species.TopIndex = LB_Species.SelectedIndex;
        GetEntry();
        Loading = false;
    }

    private void ChangeLBSpecies(object sender, EventArgs e)
    {
        if (Loading)
            return;
        SetEntry();

        Index = (ushort)LB_Species.SelectedIndex;
        Loading = true;
        CB_Species.SelectedValue = (int)CaptureRecords.GetIndexSpecies(Index);
        GetEntry();
        Loading = false;
    }

    private void GetEntry()
    {
        var index = Index;
        if (index > CaptureRecords.MaxIndex)
            return;
        NUD_SpeciesCaptured.Value = Captured.GetCapturedCountIndex(index);
        NUD_SpeciesTransferred.Value = Captured.GetTransferredCountIndex(index);
    }

    private void SetEntry()
    {
        var index = Index;
        if (index > CaptureRecords.MaxIndex)
            return;
        Captured.SetCapturedCountIndex(index, (uint)NUD_SpeciesCaptured.Value);
        Captured.SetTransferredCountIndex(index, (uint)NUD_SpeciesTransferred.Value);
    }

    private void B_Cancel_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void B_Save_Click(object sender, EventArgs e)
    {
        SetEntry();
        SetTotals();

        Origin.CopyChangesFrom(SAV);
        Close();
    }

    private void GetTotals()
    {
        NUD_TotalCaptured.Value = Captured.TotalCaptured;
        NUD_TotalTransferred.Value = Captured.TotalTransferred;
    }

    private void SetTotals()
    {
        Captured.TotalCaptured = (uint)NUD_TotalCaptured.Value;
        Captured.TotalTransferred = (uint)NUD_TotalTransferred.Value;
    }

    private void B_Modify_Click(object sender, EventArgs e)
    {
        SetTotals();
        Captured.SetAllCaptured((uint)NUD_SpeciesCaptured.Value, Dex);
        Captured.SetAllTransferred((uint)NUD_SpeciesTransferred.Value, Dex);

        GetEntry();
        System.Media.SystemSounds.Asterisk.Play();
    }

    private void B_SumTotal_Click(object sender, EventArgs e)
    {
        SetEntry();
        NUD_TotalCaptured.Value = Captured.CalculateTotalCaptured();
        NUD_TotalTransferred.Value = Captured.CalculateTotalTransferred();
        System.Media.SystemSounds.Asterisk.Play();
    }

    private static void ToggleMax(NumericUpDown nud) => nud.Value = (nud.Value != nud.Maximum) ? nud.Maximum : 0;
    private void ClickSpeciesLabel(object sender, EventArgs e) => ToggleMax(sender == L_SpeciesCaptured ? NUD_SpeciesCaptured : NUD_SpeciesTransferred);
    private void ClickTotalLabel(object sender, EventArgs e) => ToggleMax(sender == L_TotalCaptured ? NUD_TotalCaptured : NUD_TotalTransferred);
}
