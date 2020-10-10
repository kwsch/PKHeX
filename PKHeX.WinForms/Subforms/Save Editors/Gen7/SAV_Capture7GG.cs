using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SAV_Capture7GG : Form
    {
        private readonly SaveFile Origin;
        private readonly SAV7b SAV;

        private readonly Zukan7b Dex;
        private readonly CaptureRecords Captured;
        private int Index = -1;
        private bool Loading = true;

        public SAV_Capture7GG(SaveFile sav)
        {
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            SAV = (SAV7b)(Origin = sav).Clone();
            Dex = SAV.Blocks.Zukan;
            Captured = SAV.Blocks.Captured;

            // Clear Listbox and ComboBox
            LB_Species.Items.Clear();
            CB_Species.Items.Clear();

            // Fill List
            var list = GetLegalSpecies().ToArray();
            var species = GameInfo.SpeciesDataSource.Where(z => list.Contains(z.Value)).ToList();
            CB_Species.InitializeBinding();
            CB_Species.DataSource = new BindingSource(species, null);
            foreach (var entry in species.OrderBy(z => z.Value))
                LB_Species.Items.Add($"{entry.Value:000}: {entry.Text}");

            GetTotals();
            CB_Species.KeyDown += WinFormsUtil.RemoveDropCB;
            LB_Species.SelectedIndex = Index = 0;
            GetEntry();
            Loading = false;
        }

        private static IEnumerable<int> GetLegalSpecies()
        {
            foreach (var z in Enumerable.Range(1, 151))
                yield return z;
            yield return 808;
            yield return 809;
        }

        private void ChangeCBSpecies(object sender, EventArgs e)
        {
            if (Loading)
                return;
            SetEntry();

            Index = CaptureRecords.GetSpeciesIndex((int)CB_Species.SelectedValue);
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

            Index = LB_Species.SelectedIndex;
            Loading = true;
            CB_Species.SelectedValue = CaptureRecords.GetIndexSpecies(Index);
            GetEntry();
            Loading = false;
        }

        private void GetEntry()
        {
            var index = Index;
            if (index < 0)
                return;
            NUD_SpeciesCaptured.Value = Captured.GetCapturedCountIndex(index);
            NUD_SpeciesTransferred.Value = Captured.GetTransferredCountIndex(index);
        }

        private void SetEntry()
        {
            var index = Index;
            if (index < 0)
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
}
