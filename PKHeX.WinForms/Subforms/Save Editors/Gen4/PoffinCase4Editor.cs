using System;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class PoffinCase4Editor : UserControl
    {
        public PoffinCase4Editor() => InitializeComponent();
        private PoffinCase4 Case;
        private int CurrentIndex = -1;
        private bool Updating;

        public void Initialize(SAV4Sinnoh sav)
        {
            Case = new PoffinCase4(sav);

            LB_Poffins.Items.Clear();
            for (int i = 0; i < Case.Poffins.Length; i++)
                LB_Poffins.Items.Add(GetPoffinText(i));

            LB_Poffins.SelectedIndex = 0;
        }

        private void RefreshList()
        {
            Updating = true;
            for (int i = 0; i < Case.Poffins.Length; i++)
                LB_Poffins.Items[i] = GetPoffinText(i);
            Updating = false;
        }

        private string GetPoffinText(int i) => $"{i + 1:000} - {Case.Poffins[i].Type.ToString().Replace('_', '-')}";

        public void Save()
        {
            SaveIndex(CurrentIndex);
            Case.Save();
        }

        private void SaveIndex(int i)
        {
            // do nothing, propertygrid handles everything
            if (i < 0)
                return;
            Updating = true;
            LB_Poffins.Items[i] = GetPoffinText(i);
            Updating = false;
        }

        private void LoadIndex(int i)
        {
            if (i < 0)
            {
                LB_Poffins.SelectedIndex = 0;
                return;
            }
            PG_Poffins.SelectedObject = Case.Poffins[i];
        }

        private void LB_Poffins_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            SaveIndex(CurrentIndex);
            LoadIndex(CurrentIndex = LB_Poffins.SelectedIndex);
        }

        private void B_PoffinAll_Click(object sender, EventArgs e)
        {
            Case.FillCase();
            PG_Poffins.Refresh();
            RefreshList();
        }

        private void B_PoffinDel_Click(object sender, EventArgs e)
        {
            Case.DeleteAll();
            PG_Poffins.Refresh();
            RefreshList();
        }

        private void PG_Poffins_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            SaveIndex(LB_Poffins.SelectedIndex);
        }
    }
}
