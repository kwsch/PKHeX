using System;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SAV_Roamer3 : Form
    {
        private readonly Roamer3 Reader;

        public SAV_Roamer3(SaveFile sav)
        {
            Reader = new Roamer3((SAV3)sav);
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);

            CB_Species.DisplayMember = "Text";
            CB_Species.ValueMember = "Value";
            CB_Species.DataSource = new BindingSource(GameInfo.SpeciesDataSource.Where(id => id.Value <= sav.MaxSpeciesID).ToList(), null);

            LoadData();
        }

        private void LoadData()
        {
            TB_PID.Text = $"{Reader.PID:X8}";
            CHK_Shiny.Checked = Reader.IsShiny(Reader.PID);

            CB_Species.SelectedValue = Reader.Species;
            var IVs = Reader.IVs;

            var iv = new[] {TB_HPIV, TB_ATKIV, TB_DEFIV, TB_SPEIV, TB_SPAIV, TB_SPDIV};
            for (int i = 0; i < iv.Length; i++)
                iv[i].Text = IVs[i].ToString();
        }
        private void SaveData()
        {
            int[] IVs = new int[6];
            var iv = new[] { TB_HPIV, TB_ATKIV, TB_DEFIV, TB_SPEIV, TB_SPAIV, TB_SPDIV };
            for (int i = 0; i < iv.Length; i++)
                IVs[i] = Util.ToInt32(iv[i].Text);

            Reader.PID = Util.GetHexValue(TB_PID.Text);
            Reader.Species = WinFormsUtil.GetIndex(CB_Species);
        }
        private void B_Save_Click(object sender, EventArgs e)
        {
            SaveData();
            Close();
        }
        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void TB_PID_TextChanged(object sender, EventArgs e)
        {
            var pid = Util.GetHexValue(TB_PID.Text);
            CHK_Shiny.Checked = Reader.IsShiny(pid);
        }
    }
}
