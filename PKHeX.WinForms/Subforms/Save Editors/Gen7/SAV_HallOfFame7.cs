using System;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SAV_HallOfFame7 : Form
    {
        private readonly SAV7 SAV;
        private readonly ComboBox[] entries;

        public SAV_HallOfFame7(SAV7 sav)
        {
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            SAV = sav;
            entries = new[]
            {
                CB_F1, CB_F2, CB_F3, CB_F4, CB_F5, CB_F6,
                CB_C1, CB_C2, CB_C3, CB_C4, CB_C5, CB_C6,
            };

            var block = SAV.Fame;
            var specList = GameInfo.SpeciesDataSource.Where(s => s.Value <= SAV.MaxSpeciesID).ToList();
            for (int i = 0; i < entries.Length; i++)
            {
                var cb = entries[i];
                cb.Items.Clear();
                cb.InitializeBinding();
                cb.DataSource = new BindingSource(specList, null);
                cb.SelectedValue = block.GetEntry(i);
            }

            if (SAV is SAV7USUM uu)
                TB_EC.Text = uu.Misc.StarterEncryptionConstant.ToString("X8");
            else
                TB_EC.Visible = L_EC.Visible = false;
        }

        private void B_Cancel_Click(object sender, EventArgs e) => Close();

        private void B_Close_Click(object sender, EventArgs e)
        {
            var block = SAV.Fame;
            for (int i = 0; i < entries.Length; i++)
                block.SetEntry(i, (ushort)WinFormsUtil.GetIndex(entries[i]));

            if (SAV is SAV7USUM uu)
                uu.Misc.StarterEncryptionConstant = Util.GetHexValue(TB_EC.Text);

            Close();
        }
    }
}
