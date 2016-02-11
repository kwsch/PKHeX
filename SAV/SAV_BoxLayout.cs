using System;
using System.Drawing;
using System.Windows.Forms;

namespace PKHeX
{
    public partial class SAV_BoxLayout : Form
    {
        public SAV_BoxLayout(int box)
        {
            InitializeComponent();
            Util.TranslateInterface(this, Main.curlanguage);
            editing = true;
            
            // Repopulate Wallpaper names
            CB_BG.Items.Clear();
            foreach (string wallpaper in Main.wallpapernames)
                CB_BG.Items.Add(wallpaper);
            
            // Go
            MT_BG1.Text = sav.Data[sav.PCFlags + 0].ToString();
            CB_Unlocked.SelectedIndex = sav.Data[sav.PCFlags + 1] - 1;
            MT_BG2.Text = sav.Data[sav.PCFlags + 2].ToString();
            LB_BoxSelect.SelectedIndex = box;
        }
        private readonly SAV6 sav = new SAV6((byte[])Main.SAV.Data.Clone());
        private bool editing;

        private void changeBox(object sender, EventArgs e)
        {
            editing = true;

            int bgoff = sav.PCBackgrounds + LB_BoxSelect.SelectedIndex;
            CB_BG.SelectedIndex = sav.Data[bgoff];
            TB_BoxName.Text = sav.getBoxName(LB_BoxSelect.SelectedIndex);

            editing = false; 
        }
        private void changeBoxDetails(object sender, EventArgs e)
        {
            if (!editing)
                sav.setBoxName(LB_BoxSelect.SelectedIndex, TB_BoxName.Text);
        }
        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void B_Save_Click(object sender, EventArgs e)
        {
            sav.Data[sav.PCFlags + 0] = (byte)Util.ToUInt32(MT_BG1.Text);
            sav.Data[sav.PCFlags + 1] = (byte)Util.ToUInt32(CB_Unlocked.Text);
            sav.Data[sav.PCFlags + 2] = (byte)Util.ToUInt32(MT_BG2.Text);

            Array.Copy(sav.Data, Main.SAV.Data, sav.Data.Length);
            Main.SAV.Edited = true;
            Close();
        }

        private void changeBoxBG(object sender, EventArgs e)
        {
            if (!editing)
                sav.Data[Main.SAV.PCBackgrounds + LB_BoxSelect.SelectedIndex] = (byte)CB_BG.SelectedIndex;

            string imagename = "box_wp" + (CB_BG.SelectedIndex + 1).ToString("00"); if (sav.ORAS && CB_BG.SelectedIndex + 1 > 16) imagename += "o";
            PAN_BG.BackgroundImage = (Image)Properties.Resources.ResourceManager.GetObject(imagename);
        }
    }
}