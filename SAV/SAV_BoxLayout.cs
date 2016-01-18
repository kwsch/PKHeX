using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PKHeX
{
    public partial class SAV_BoxLayout : Form
    {
        public SAV_BoxLayout(int box)
        {
            InitializeComponent();
            Util.TranslateInterface(this, Main.curlanguage);
            sav = (byte[])Main.SAV.Data.Clone();
            editing = true;
            
            // Repopulate Wallpaper names
            CB_BG.Items.Clear();
            foreach (string wallpaper in Main.wallpapernames)
                CB_BG.Items.Add(wallpaper);
            
            // Go
            MT_BG1.Text = sav[Main.SAV.PCFlags + 0].ToString();
            CB_Unlocked.SelectedIndex = sav[Main.SAV.PCFlags + 1] - 1;
            MT_BG2.Text = sav[Main.SAV.PCFlags + 2].ToString();
            LB_BoxSelect.SelectedIndex = box;
        }
        public byte[] sav;
        public bool editing;

        private void changeBox(object sender, EventArgs e)
        {
            editing = true;

            int bgoff = Main.SAV.PCBackgrounds + LB_BoxSelect.SelectedIndex;
            CB_BG.SelectedIndex = sav[bgoff];
            TB_BoxName.Text = Encoding.Unicode.GetString(sav, Main.SAV.PCLayout + 0x22 * LB_BoxSelect.SelectedIndex, 0x22).Trim();

            editing = false; 
        }
        private void changeBoxDetails(object sender, EventArgs e)
        {
            if (!editing)
                Encoding.Unicode.GetBytes(TB_BoxName.Text.PadRight(17))
                    .CopyTo(sav, Main.SAV.PCLayout + 0x22 * LB_BoxSelect.SelectedIndex);
        }
        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void B_Save_Click(object sender, EventArgs e)
        {
            sav[Main.SAV.PCFlags + 0] = (byte)Util.ToUInt32(MT_BG1.Text);
            sav[Main.SAV.PCFlags + 1] = (byte)Util.ToUInt32(CB_Unlocked.Text);
            sav[Main.SAV.PCFlags + 2] = (byte)Util.ToUInt32(MT_BG2.Text);

            Array.Copy(sav, Main.SAV.Data, sav.Length);
            Main.SAV.Edited = true;
            Close();
        }

        private void changeBoxBG(object sender, EventArgs e)
        {
            if (!editing)
                sav[Main.SAV.PCBackgrounds + LB_BoxSelect.SelectedIndex] = (byte)CB_BG.SelectedIndex;

            string imagename = "box_wp" + (CB_BG.SelectedIndex + 1).ToString("00"); if (Main.SAV.ORAS && CB_BG.SelectedIndex + 1 > 16) imagename += "o";
            PAN_BG.BackgroundImage = (Image)Properties.Resources.ResourceManager.GetObject(imagename);
        }
    }
}