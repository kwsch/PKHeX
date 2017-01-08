using System;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SAV_BoxLayout : Form
    {
        public SAV_BoxLayout(int box)
        {
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.curlanguage);
            editing = true;
            
            // Repopulate Wallpaper names
            CB_BG.Items.Clear();

            switch (SAV.Generation)
            {
                case 3:
                    if (SAV.GameCube)
                        goto default;
                    CB_BG.Items.AddRange(GameInfo.Strings.wallpapernames.Take(16).ToArray());
                    break;
                case 4:
                case 5:
                case 6:
                    CB_BG.Items.AddRange(GameInfo.Strings.wallpapernames);
                    break;
                case 7:
                    CB_BG.Items.AddRange(GameInfo.Strings.wallpapernames.Take(16).ToArray());
                    break;
                default:
                    WinFormsUtil.Error("Box layout is not supported for this game.", "Please close the window.");
                    break;
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
        
        private bool MoveItem(int direction)
        {
            // Checking selected item
            if (LB_BoxSelect.SelectedItem == null || LB_BoxSelect.SelectedIndex < 0)
                return false; // No selected item - nothing to do

            // Calculate new index using move direction
            int newIndex = LB_BoxSelect.SelectedIndex + direction;

            // Checking bounds of the range
            if (newIndex < 0 || newIndex >= LB_BoxSelect.Items.Count)
                return false; // Index out of range - nothing to do

            object selected = LB_BoxSelect.SelectedItem;

            // Removing removable element
            LB_BoxSelect.Items.Remove(selected);
            // Insert it in new position
            LB_BoxSelect.Items.Insert(newIndex, selected);
            // Restore selection
            LB_BoxSelect.SetSelected(newIndex, true);
            editing = renameBox = false;

            return true;
        }

        private void moveBox(object sender, EventArgs e)
        {
            int index = LB_BoxSelect.SelectedIndex;
            int dir = sender == B_Up ? -1 : +1;
            editing = renameBox = true;
            if (!MoveItem(dir))
            {
                System.Media.SystemSounds.Asterisk.Play();
            }
            else if (!SAV.SwapBox(index, index + dir)) // valid but locked
            {
                MoveItem(-dir); // undo
                WinFormsUtil.Alert("Locked slots prevent movement of box(es).");
            }
            else
                changeBox(null, null);
            editing = renameBox = false;
        }
    }
}