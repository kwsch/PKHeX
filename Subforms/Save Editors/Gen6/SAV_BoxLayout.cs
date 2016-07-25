using System;
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
            MT_BG1.Text = SAV.Data[SAV.PCFlags + 0].ToString();
            CB_Unlocked.SelectedIndex = SAV.Data[SAV.PCFlags + 1] - 1;
            MT_BG2.Text = SAV.Data[SAV.PCFlags + 2].ToString();
            LB_BoxSelect.SelectedIndex = box;
        }
        private readonly SAV6 SAV = new SAV6(Main.SAV.Data);
        private bool editing;

        private void changeBox(object sender, EventArgs e)
        {
            editing = true;

            int bgoff = SAV.PCBackgrounds + LB_BoxSelect.SelectedIndex;
            CB_BG.SelectedIndex = SAV.Data[bgoff];
            TB_BoxName.Text = SAV.getBoxName(LB_BoxSelect.SelectedIndex);

            editing = false; 
        }
        private void changeBoxDetails(object sender, EventArgs e)
        {
            if (!editing)
                SAV.setBoxName(LB_BoxSelect.SelectedIndex, TB_BoxName.Text);
        }
        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void B_Save_Click(object sender, EventArgs e)
        {
            SAV.Data[SAV.PCFlags + 0] = (byte)Util.ToUInt32(MT_BG1.Text);
            SAV.Data[SAV.PCFlags + 1] = (byte)Util.ToUInt32(CB_Unlocked.Text);
            SAV.Data[SAV.PCFlags + 2] = (byte)Util.ToUInt32(MT_BG2.Text);

            Array.Copy(SAV.Data, Main.SAV.Data, SAV.Data.Length);
            Main.SAV.Edited = true;
            Close();
        }

        private void changeBoxBG(object sender, EventArgs e)
        {
            if (!editing)
                SAV.Data[SAV.PCBackgrounds + LB_BoxSelect.SelectedIndex] = (byte)CB_BG.SelectedIndex;

            PAN_BG.BackgroundImage = BoxWallpaper.getWallpaper(SAV, CB_BG.SelectedIndex + 1);
        }
    }
}