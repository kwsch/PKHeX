using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace PKHeX
{
    public partial class SAV_Link6 : Form
    {
        public SAV_Link6()
        {
            InitializeComponent();
            foreach (var cb in TAB_Items.Controls.OfType<ComboBox>())
            {
                cb.DisplayMember = "Text";
                cb.ValueMember = "Value";
                cb.DataSource = new BindingSource(GameInfo.ItemDataSource.Where(item => item.Value <= SAV.MaxItemID).ToArray(), null);
            }
            Util.TranslateInterface(this, Main.curlanguage);
            byte[] data = SAV.LinkBlock;
            if (data == null)
            {
                Util.Alert("Invalid save file / Link Information");
                Close();
            }
            data = data.Skip(0x1FF).Take(PL6.Size).ToArray();
            loadLinkData(data);
        }

        private readonly SAV6 SAV = Main.SAV.Clone() as SAV6;
        private PL6 LinkInfo;

        private void B_Save_Click(object sender, EventArgs e)
        {
            byte[] data = new byte[SAV.LinkBlock.Length];
            Array.Copy(LinkInfo.Data, 0, data, 0x1FF, LinkInfo.Data.Length);

            // Fix Checksum just in case.
            ushort ccitt = SaveUtil.ccitt16(data.Skip(0x200).Take(data.Length - 4 - 0x200).ToArray()); // [app,chk)
            BitConverter.GetBytes(ccitt).CopyTo(data, data.Length - 4);

            SAV.LinkBlock = data;
            Array.Copy(SAV.Data, Main.SAV.Data, SAV.Data.Length);
            Close();
        }
        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void B_Import_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog {Filter = PL6.Filter};
            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            if (new FileInfo(ofd.FileName).Length != PL6.Size)
            { Util.Alert("Invalid file length"); return; }

            byte[] data = File.ReadAllBytes(ofd.FileName);
            
            loadLinkData(data);
            B_Export.Enabled = true;
        }
        private void B_Export_Click(object sender, EventArgs e)
        {
            if (LinkInfo.Data == null)
                return;

            SaveFileDialog sfd = new SaveFileDialog {Filter = PL6.Filter};
            if (sfd.ShowDialog() != DialogResult.OK)
                return;

            File.WriteAllBytes(sfd.FileName, LinkInfo.Data);
            Util.Alert("Pokémon Link data saved to:\r" + sfd.FileName + ".");
        }
        
        private void loadLinkData(byte[] data)
        {
            LinkInfo = new PL6(data);

            RTB_LinkSource.Text = LinkInfo.Origin_app;
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
            TB_PKM1.Text = Main.GameStrings.specieslist[LinkInfo.Pokes[0].Species];
            TB_PKM2.Text = Main.GameStrings.specieslist[LinkInfo.Pokes[1].Species];
            TB_PKM3.Text = Main.GameStrings.specieslist[LinkInfo.Pokes[2].Species];
            TB_PKM4.Text = Main.GameStrings.specieslist[LinkInfo.Pokes[3].Species];
            TB_PKM5.Text = Main.GameStrings.specieslist[LinkInfo.Pokes[4].Species];
            TB_PKM6.Text = Main.GameStrings.specieslist[LinkInfo.Pokes[5].Species];
        }
    }
}
