using System;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Drawing.Misc;

namespace PKHeX.WinForms;

public partial class SAV_BoxLayout : Form
{
    private readonly SaveFile Origin;
    private readonly SaveFile SAV;

    public SAV_BoxLayout(SaveFile sav, int box)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (Origin = sav).Clone();
        editing = true;

        bool any = LoadWallpapers(SAV);
        any |= LoadBoxNames(SAV);
        if (!any)
            WinFormsUtil.Error("Box layout is not supported for this game.", "Please close the window.");
        LoadFlags();
        LoadUnlockedCount();

        LB_BoxSelect.SelectedIndex = box;
        TB_BoxName.MaxLength = SAV.Generation switch
        {
            2 when SAV is SAV2 { Japanese: false, Korean: false } => 8 * 2,
            6 or 7 => 14,
            >= 8 => 16,
            _ => 8,
        };
        editing = false;
    }

    private bool LoadWallpapers(SaveFile sav)
    {
        if (sav is not IBoxDetailWallpaper)
        {
            Controls.Remove(CB_BG);
            Controls.Remove(PAN_BG);
            return false;
        }

        CB_BG.Items.Clear();

        static void AddRange(ComboBox cb, ReadOnlySpan<string> names)
        {
            foreach (var name in names)
                cb.Items.Add(name);
        }

        static void AddPlaceholder(ComboBox cb, int count)
        {
            for (int i = 1; i <= count; i++)
                cb.Items.Add($"Wallpaper {i}");
        }

        var names = GameInfo.Strings.wallpapernames;
        switch (SAV.Generation)
        {
            case 3 when SAV is SAV3:
                AddRange(CB_BG, names.AsSpan(0, 16));
                return true;
            case 4 or 5 or 6:
                AddRange(CB_BG, names.AsSpan(0, 24));
                return true;
            case 7:
                AddRange(CB_BG, names.AsSpan(0, 16));
                return true;
            case 8 when SAV is SAV8BS:
                AddRange(CB_BG, names.AsSpan(0, 32));
                return true;
            case 8:
                AddPlaceholder(CB_BG, 19);
                return true;
            case 9:
                AddPlaceholder(CB_BG, 20);
                return true;
            default:
                return false;
        }
    }

    private bool LoadBoxNames(SaveFile sav)
    {
        if (sav is not IBoxDetailNameRead r)
        {
            Controls.Remove(TB_BoxName);
            return false;
        }
        if (sav is not IBoxDetailName)
            TB_BoxName.Enabled = false;
        LB_BoxSelect.Items.Clear();
        for (int i = 0; i < SAV.BoxCount; i++)
            LB_BoxSelect.Items.Add(r.GetBoxName(i));
        return true;
    }

    private void LoadUnlockedCount()
    {
        if (SAV.BoxesUnlocked <= 0)
        {
            FLP_Unlocked.Visible = L_Unlocked.Visible = CB_Unlocked.Visible = false;
            return;
        }
        CB_Unlocked.Items.Clear();
        int max = SAV.BoxCount;
        for (int i = 0; i <= max; i++)
            CB_Unlocked.Items.Add(i);
        CB_Unlocked.SelectedIndex = Math.Min(max, SAV.BoxesUnlocked);
    }

    private void LoadFlags()
    {
        byte[] flags = SAV.BoxFlags;
        if (flags.Length == 0)
        {
            FLP_Flags.Visible = false;
            return;
        }

        flagArr = new NumericUpDown[flags.Length];
        for (int i = 0; i < flags.Length; i++)
        {
            flagArr[i] = new NumericUpDown
            {
                Minimum = 0,
                Maximum = 255,
                Width = CB_Unlocked.Width - 5,
                Hexadecimal = true,
                Value = flags[i],
            };
            FLP_Flags.Controls.Add(flagArr[i]);
        }
    }

    private NumericUpDown[] flagArr = [];
    private bool editing;
    private bool renamingBox;

    private void ChangeBox(object sender, EventArgs e)
    {
        if (renamingBox)
            return;
        editing = true;

        var box = LB_BoxSelect.SelectedIndex;
        if (SAV is IBoxDetailWallpaper wp)
        {
            var choice = wp.GetBoxWallpaper(box);
            var maxWallpaper = CB_BG.Items.Count - 1;
            CB_BG.SelectedIndex = Math.Clamp(choice, 0, maxWallpaper);
        }

        if (SAV is IBoxDetailNameRead r)
            TB_BoxName.Text = r.GetBoxName(LB_BoxSelect.SelectedIndex);

        editing = false;
    }

    private void ChangeBoxDetails(object sender, EventArgs e)
    {
        if (editing)
            return;

        renamingBox = true;
        if (SAV is IBoxDetailName name)
        {
            name.SetBoxName(LB_BoxSelect.SelectedIndex, TB_BoxName.Text);
            LB_BoxSelect.Items[LB_BoxSelect.SelectedIndex] = TB_BoxName.Text;
        }
        renamingBox = false;
    }

    private void B_Cancel_Click(object sender, EventArgs e) => Close();

    private void B_Save_Click(object sender, EventArgs e)
    {
        if (flagArr.Length != 0)
            SAV.BoxFlags = Array.ConvertAll(flagArr, i => (byte)i.Value);
        if (CB_Unlocked.Visible)
            SAV.BoxesUnlocked = CB_Unlocked.SelectedIndex;

        Origin.CopyChangesFrom(SAV);
        Close();
    }

    private void ChangeBoxBackground(object sender, EventArgs e)
    {
        if (!editing)
        {
            if (SAV is IBoxDetailWallpaper wp)
                wp.SetBoxWallpaper(LB_BoxSelect.SelectedIndex, CB_BG.SelectedIndex);
        }

        PAN_BG.BackgroundImage = SAV.WallpaperImage(LB_BoxSelect.SelectedIndex);
    }

    private bool MoveItem(int direction)
    {
        // Checking selected item
        if (LB_BoxSelect.SelectedItem == null || LB_BoxSelect.SelectedIndex < 0)
            return false; // No selected item - nothing to do

        // Calculate new index using move direction
        int newIndex = LB_BoxSelect.SelectedIndex + direction;

        // Checking bounds of the range
        if ((uint)newIndex >= LB_BoxSelect.Items.Count)
            return false; // Index out of range - nothing to do

        object selected = LB_BoxSelect.SelectedItem;

        // Removing removable element
        LB_BoxSelect.Items.Remove(selected);
        // Insert it in new position
        LB_BoxSelect.Items.Insert(newIndex, selected);
        // Restore selection
        LB_BoxSelect.SetSelected(newIndex, true);
        editing = renamingBox = false;

        return true;
    }

    private void MoveBox(object sender, EventArgs e)
    {
        int index = LB_BoxSelect.SelectedIndex;
        int dir = sender == B_Up ? -1 : +1;
        editing = renamingBox = true;
        if (!MoveItem(dir))
        {
            System.Media.SystemSounds.Asterisk.Play();
        }
        else if (!SAV.SwapBox(index, index + dir)) // valid but locked
        {
            MoveItem(-dir); // undo
            WinFormsUtil.Alert("Locked/Team slots prevent movement of box(es).");
        }
        else
        {
            ChangeBox(sender, EventArgs.Empty);
        }

        editing = renamingBox = false;
    }
}
