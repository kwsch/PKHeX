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
            sav = (byte[])Main.savefile.Clone();
            editing = true;

            // Repopulate Wallpaper names
            CB_BG.Items.Clear();
            foreach (string wallpaper in Main.wallpapernames)
                CB_BG.Items.Add(wallpaper);

            // Go
            LB_BoxSelect.SelectedIndex = box;
        }
        public byte[] sav;
        public bool editing;

        private void changeBox(object sender, EventArgs e)
        {
            editing = true;
            int index = LB_BoxSelect.SelectedIndex;
            int bgoff = Main.SaveGame.PCBackgrounds + LB_BoxSelect.SelectedIndex;
            CB_BG.SelectedIndex = sav[bgoff];

            TB_BoxName.Text = Encoding.Unicode.GetString(sav, Main.SaveGame.PCLayout + 0x22 * index, 0x22);
            CB_BG.SelectedIndex = sav[bgoff];

            MT_BG1.Text = sav[Main.SaveGame.PCFlags + 0].ToString();
            CB_Unlocked.SelectedIndex = sav[Main.SaveGame.PCFlags + 1] - 1;
            MT_BG2.Text = sav[Main.SaveGame.PCFlags + 2].ToString();

            editing = false; 
        }
        private void changeBoxDetails(object sender, EventArgs e)
        {
            if (editing) return;

            int index = LB_BoxSelect.SelectedIndex;
            sav[Main.SaveGame.PCBackgrounds + index] = (byte)CB_BG.SelectedIndex;
            
            byte[] boxname = Encoding.Unicode.GetBytes(TB_BoxName.Text);
            Array.Resize(ref boxname, 0x22);
            Array.Copy(boxname, 0, sav, Main.SaveGame.PCLayout + 0x22 * index, boxname.Length);

            sav[Main.SaveGame.PCBackgrounds + index] = (byte)CB_BG.SelectedIndex;
            sav[Main.SaveGame.PCFlags + 0] = (byte)Util.ToUInt32(MT_BG1.Text);
            sav[Main.SaveGame.PCFlags + 1] = (byte)Util.ToUInt32(CB_Unlocked.Text);
            sav[Main.SaveGame.PCFlags + 2] = (byte)Util.ToUInt32(MT_BG2.Text);
        }
        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void B_Save_Click(object sender, EventArgs e)
        {
            Array.Copy(sav, Main.savefile, sav.Length);
            Main.savedited = true;
            Close();
        }

        private void changeBoxBG(object sender, EventArgs e)
        {
            sav[Main.SaveGame.PCBackgrounds + LB_BoxSelect.SelectedIndex] = (byte)CB_BG.SelectedIndex;

            string imagename = "box_wp" + (CB_BG.SelectedIndex + 1).ToString("00"); if (Main.SaveGame.ORAS && (CB_BG.SelectedIndex + 1) > 16) imagename += "o";
            PAN_BG.BackgroundImage = (Image)Properties.Resources.ResourceManager.GetObject(imagename);
        }
    }
}