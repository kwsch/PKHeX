using System;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SAV_HallOfFame7 : Form
    {
        private readonly SaveFile Origin;
        private readonly SAV7 SAV;

        public SAV_HallOfFame7(SaveFile sav)
        {
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            SAV = (SAV7)(Origin = sav).Clone();
            entries = new[]
            {
                CB_F1, CB_F2, CB_F3, CB_F4, CB_F5, CB_F6,
                CB_C1, CB_C2, CB_C3, CB_C4, CB_C5, CB_C6,
            };
            Setup();
        }

        private readonly ComboBox[] entries;

        private void Setup()
        {
            int ofs = SAV.HoF;

            CHK_Flag.Checked = (BitConverter.ToUInt16(SAV.Data, ofs) & 1) == 1;
            NUD_Count.Value = BitConverter.ToUInt16(SAV.Data, ofs + 2);

            var specList = GameInfo.SpeciesDataSource.Where(s => s.Value <= SAV.MaxSpeciesID).ToList();
            for (int i = 0; i < entries.Length; i++)
            {
                int o = ofs + 4 + (i * 2);
                var cb = entries[i];
                cb.Items.Clear();

                cb.InitializeBinding();
                cb.DataSource = new BindingSource(specList, null);

                cb.SelectedValue = (int)BitConverter.ToUInt16(SAV.Data, o);
            }

            if (SAV is SAV7USUM)
                TB_EC.Text = SAV.Misc.StarterEncryptionConstant.ToString("X8");
            else
                TB_EC.Visible = L_EC.Visible = false;
        }

        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void B_Close_Click(object sender, EventArgs e)
        {
            int ofs = SAV.HoF;

            SAV.Data[ofs] &= 0xFE;
            SAV.Data[ofs] |= (byte)(CHK_Flag.Checked ? 1 : 0);
            BitConverter.GetBytes((ushort)NUD_Count.Value).CopyTo(SAV.Data, ofs + 2);
            for (int i = 0; i < entries.Length; i++)
            {
                int o = ofs + 4 + (i * 2);
                var cb = entries[i];
                var val = WinFormsUtil.GetIndex(cb);
                BitConverter.GetBytes((ushort)val).CopyTo(SAV.Data, o);
            }

            if (SAV is SAV7USUM)
                SAV.Misc.StarterEncryptionConstant = Util.GetHexValue(TB_EC.Text);

            Origin.CopyChangesFrom(SAV);
            Close();
        }
    }
}
