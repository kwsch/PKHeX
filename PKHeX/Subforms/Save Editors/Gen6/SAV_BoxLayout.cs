using System;
using System.Linq;
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

            switch (SAV.Generation)
            {
                case 4:
                case 5:
                case 6:
                    CB_BG.Items.AddRange(Main.GameStrings.wallpapernames);
                    break;
                case 7:
                    CB_BG.Items.AddRange(Main.GameStrings.wallpapernames.Take(16).ToArray());
                    break;
                default:
                    Util.Error("Box layout is not supported for this game.");
                    return;
            }

            // Go
            LB_BoxSelect.Items.Clear();
            for (int i = 0; i < SAV.BoxCount; i++)
                LB_BoxSelect.Items.Add(SAV.getBoxName(i));

            // Flags
            byte[] flags = SAV.BoxFlags;
            if (flags != null)
            {
                flagArr = new NumericUpDown[flags.Length];
                for (int i = 0; i < flags.Length; i++)
                {
                    flagArr[i] = new NumericUpDown
                    {
                        Minimum = 0,
                        Maximum = 255,
                        Width = CB_Unlocked.Width - 5,
                        Hexadecimal = true,
                        Value = flags[i]
                    };
                    FLP_Flags.Controls.Add(flagArr[i]);
                }
            }
            else
            {
                FLP_Flags.Visible = false;
            }

            // Unlocked
            if (SAV.BoxesUnlocked > 0)
            {
                CB_Unlocked.Items.Clear();
                for (int i = 0; i <= SAV.BoxCount; i++)
                    CB_Unlocked.Items.Add(i);
                CB_Unlocked.SelectedIndex = Math.Min(SAV.BoxCount, SAV.BoxesUnlocked);
            }
            else
            {
                FLP_Unlocked.Visible = L_Unlocked.Visible = CB_Unlocked.Visible = false;
            }
            LB_BoxSelect.SelectedIndex = box;
        }

        private readonly NumericUpDown[] flagArr = new NumericUpDown[0];
        private readonly SaveFile SAV = Main.SAV.Clone();
        private bool editing;
        private bool renameBox;
        private void changeBox(object sender, EventArgs e)
        {
            if (renameBox)
                return;
            editing = true;
            
            CB_BG.SelectedIndex = SAV.getBoxWallpaper(LB_BoxSelect.SelectedIndex);
            TB_BoxName.Text = SAV.getBoxName(LB_BoxSelect.SelectedIndex);

            editing = false; 
        }
        private void changeBoxDetails(object sender, EventArgs e)
        {
            if (editing)
                return;

            renameBox = true;
            SAV.setBoxName(LB_BoxSelect.SelectedIndex, TB_BoxName.Text);
            LB_BoxSelect.Items[LB_BoxSelect.SelectedIndex] = TB_BoxName.Text;
            renameBox = false;
        }
        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void B_Save_Click(object sender, EventArgs e)
        {
            if (flagArr.Length > 0)
                SAV.BoxFlags = flagArr.Select(i => (byte) i.Value).ToArray();
            if (CB_Unlocked.Visible)
                SAV.BoxesUnlocked = CB_Unlocked.SelectedIndex;

            Main.SAV = SAV;
            Main.SAV.Edited = true;
            Close();
        }

        private void changeBoxBG(object sender, EventArgs e)
        {
            if (!editing)
                SAV.setBoxWallpaper(LB_BoxSelect.SelectedIndex, CB_BG.SelectedIndex);

            PAN_BG.BackgroundImage = BoxWallpaper.getWallpaper(SAV, CB_BG.SelectedIndex);
        }
    }
}