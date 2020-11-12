using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SAV_Link6 : Form
    {
        private readonly SaveFile Origin;
        private readonly ISaveBlock6Main SAV;

        private PL6 LinkInfo;

        public SAV_Link6(SaveFile sav)
        {
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            SAV = (ISaveBlock6Main)(Origin = sav).Clone();
            foreach (var cb in TAB_Items.Controls.OfType<ComboBox>())
            {
                cb.InitializeBinding();
                cb.DataSource = new BindingSource(GameInfo.ItemDataSource.Where(item => item.Value <= sav.MaxItemID).ToArray(), null);
            }
            LinkInfo = SAV.Link.GetLinkInfo();
            LoadLinkData();
        }

        private void B_Save_Click(object sender, EventArgs e)
        {
            SAV.Link.SetLinkInfo(LinkInfo);
            Origin.CopyChangesFrom((SaveFile)SAV);
            Close();
        }

        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void B_Import_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog {Filter = PL6.Filter};
            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            if (new FileInfo(ofd.FileName).Length != PL6.Size)
            { WinFormsUtil.Alert("Invalid file length"); return; }

            byte[] data = File.ReadAllBytes(ofd.FileName);
            LinkInfo = new PL6(data);

            LoadLinkData();
            B_Export.Enabled = true;
        }

        private void B_Export_Click(object sender, EventArgs e)
        {
            using var sfd = new SaveFileDialog {Filter = PL6.Filter};
            if (sfd.ShowDialog() != DialogResult.OK)
                return;

            File.WriteAllBytes(sfd.FileName, LinkInfo.Data);
            WinFormsUtil.Alert("Pokémon Link data saved to:" + Environment.NewLine + sfd.FileName);
        }

        private void LoadLinkData()
        {
            RTB_LinkSource.Text = LinkInfo.Origin;
            CHK_LinkAvailable.Checked = LinkInfo.PL_enabled;

            NUD_BP.Value = LinkInfo.BattlePoints;
            NUD_Pokemiles.Value = LinkInfo.Pokemiles;

            CB_Item1.SelectedIndex = LinkInfo.Item_1;
            CB_Item2.SelectedIndex = LinkInfo.Item_2;
            CB_Item3.SelectedIndex = LinkInfo.Item_3;
            CB_Item4.SelectedIndex = LinkInfo.Item_4;
            CB_Item5.SelectedIndex = LinkInfo.Item_5;
            CB_Item6.SelectedIndex = LinkInfo.Item_6;

            NUD_Item1.Value = LinkInfo.Quantity_1;
            NUD_Item2.Value = LinkInfo.Quantity_2;
            NUD_Item3.Value = LinkInfo.Quantity_3;
            NUD_Item4.Value = LinkInfo.Quantity_4;
            NUD_Item5.Value = LinkInfo.Quantity_5;
            NUD_Item6.Value = LinkInfo.Quantity_6;

            // Pokemon slots
            TB_PKM1.Text = GameInfo.Strings.specieslist[LinkInfo.Poke_1.Species];
            TB_PKM2.Text = GameInfo.Strings.specieslist[LinkInfo.Poke_2.Species];
            TB_PKM3.Text = GameInfo.Strings.specieslist[LinkInfo.Poke_3.Species];
            TB_PKM4.Text = GameInfo.Strings.specieslist[LinkInfo.Poke_4.Species];
            TB_PKM5.Text = GameInfo.Strings.specieslist[LinkInfo.Poke_5.Species];
            TB_PKM6.Text = GameInfo.Strings.specieslist[LinkInfo.Poke_6.Species];
        }
    }
}
