using System;
using System.Buffers.Binary;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using PKHeX.Core;
using static System.Net.Mime.MediaTypeNames;
using static PKHeX.Core.SaveBlockAccessor9SV;

namespace PKHeX.WinForms;

public partial class SAV_Trainer9 : Form
{
    private readonly SaveFile Origin;
    private readonly SAV9SV SAV;

    public SAV_Trainer9(SaveFile sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (SAV9SV)(Origin = sav).Clone();

        Loading = true;
        if (Main.Unicode)
        {
            TB_OTName.Font = FontUtil.GetPKXFont();
        }

        B_MaxCash.Click += (sender, e) => MT_Money.Text = SAV.MaxMoney.ToString();
        B_MaxLP.Click += (sender, e) => MT_LP.Text = SAV.MaxMoney.ToString();

        var games = GameInfo.Strings.gamelist;
        CB_Game.Items.Clear();
        CB_Game.Items.Add(games[(int)GameVersion.SL]);
        CB_Game.Items.Add(games[(int)GameVersion.VL]);

        CB_Gender.Items.Clear();
        CB_Gender.Items.AddRange(Main.GenderSymbols.Take(2).ToArray()); // m/f depending on unicode selection

        P_CurrPhoto.Image = GetImage(SAV, SaveBlockAccessor9SV.KPictureProfileCurrent, SaveBlockAccessor9SV.KPictureProfileCurrentHeight, SaveBlockAccessor9SV.KPictureProfileCurrentWidth);
        if (P_CurrPhoto.Image.Width >= 360)
            this.Size = new Size(this.Width + 30, this.Height);
        P_CurrIcon.Image = GetImage(SAV, SaveBlockAccessor9SV.KPictureIconCurrent, SaveBlockAccessor9SV.KPictureIconCurrentHeight, SaveBlockAccessor9SV.KPictureIconCurrentWidth);
        P_InitialIcon.Image = GetImage(SAV, SaveBlockAccessor9SV.KPictureIconInitial, SaveBlockAccessor9SV.KPictureIconInitialHeight, SaveBlockAccessor9SV.KPictureIconInitialWidth);
        P_CurrIcon.Location = new Point(P_CurrPhoto.Location.X + P_CurrPhoto.Image.Width + 8, P_CurrPhoto.Location.Y);
        P_InitialIcon.Location = new Point(P_CurrIcon.Location.X, P_CurrIcon.Location.Y + P_CurrIcon.Image.Height + 8);

        GetComboBoxes();
        GetTextBoxes();
        LoadMap();

        Loading = false;
    }

    private readonly bool Loading;
    private bool MapUpdated;

    private Bitmap GetImage(SAV9SV sav, uint imageBlock, uint heightBlock, uint widthBlock)
    {
        SCBlock image = sav.Blocks.GetBlock(imageBlock);
        int height = (int)sav.Blocks.GetBlockValue<uint>(heightBlock);
        int width = (int)sav.Blocks.GetBlockValue<uint>(widthBlock);

        //get sequences of 2 bytes each and convert to RGB565
        Bitmap GetSection(int offset)
        {
            Bitmap bitmap = new Bitmap(width / 4, height / 4);
            for (int y = 0, byteIndex = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++, byteIndex += 8)
                {
                    byte[] colourBytes = image.Data[(offset + byteIndex)..(offset + byteIndex + 2)];
                    int colour = BinaryPrimitives.ReadUInt16LittleEndian(colourBytes);
                    int b = (colour & 0x001F) << 3;
                    int g = ((colour & 0x07E0) >> 5) << 2;
                    int r = ((colour & 0xF800) >> 11) << 3;
                    Color c = Color.FromArgb(255, r, g, b);
                    bitmap.SetPixel(x, y, c);
                }
            }
            return bitmap;
        }

        //overlap masks and apply alpha blending pixel by pixel using pixels
        //from composite mask as 'alpha' and blending the images fro first 2 sections
        Bitmap lmask = GetSection(4);
        Bitmap dmask = GetSection(6);
        Bitmap light = GetSection(0);
        Bitmap dark = GetSection(2);
        Bitmap result = new Bitmap(dark.Width, dark.Height);
        for (int y = 0; y < dark.Height; y++)
        {
            for (int x = 0; x < dark.Width; x++)
            {
                Color lmpx = lmask.GetPixel(x, y);
                Color dmpx = dmask.GetPixel(x, y);
                //luminance conversion on masks for grayscale
                int lgray = (int)(0.299 * lmpx.R + 0.587 * lmpx.G + 0.114 * lmpx.B);
                int dgray = (int)(0.299 * dmpx.R + 0.587 * dmpx.G + 0.114 * dmpx.B);
                int al = (lgray | dgray);
                //save as double in order to not lose precision
                double alpha = al / 255.0;
                Color lpx = light.GetPixel(x, y);
                Color dpx = dark.GetPixel(x, y);
                //alpha blending
                int newR = (int)(lpx.R * (1 - alpha) + dpx.R * alpha);
                int newG = (int)(lpx.G * (1 - alpha) + dpx.G * alpha);
                int newB = (int)(lpx.B * (1 - alpha) + dpx.B * alpha);
                result.SetPixel(x, y, Color.FromArgb(newR, newG, newB));
            }
        }
        return result;
    }

    private void LoadMap()
    {
        try
        {
            NUD_X.Value = (decimal)SAV.X;
            NUD_Y.Value = (decimal)SAV.Y;
            NUD_Z.Value = (decimal)SAV.Z;
        }
        // If we can't accurately represent the coordinates, don't allow them to be changed.
        catch { GB_Map.Enabled = false; }
    }

    private void GetComboBoxes()
    {
        CB_Language.InitializeBinding();
        CB_Language.DataSource = GameInfo.LanguageDataSource(SAV.Generation);
    }

    private void GetTextBoxes()
    {
        // Get Data
        CB_Game.SelectedIndex = SAV.Game - (int)GameVersion.SL;
        CB_Gender.SelectedIndex = SAV.Gender;

        // Display Data
        TB_OTName.Text = SAV.OT;
        trainerID1.LoadIDValues(SAV, SAV.Generation);
        MT_Money.Text = SAV.Money.ToString();
        MT_LP.Text = SAV.LeaguePoints.ToString();
        CB_Language.SelectedValue = SAV.Language;

        // Load Play Time
        MT_Hours.Text = SAV.PlayedHours.ToString();
        MT_Minutes.Text = SAV.PlayedMinutes.ToString();
        MT_Seconds.Text = SAV.PlayedSeconds.ToString();

        CAL_AdventureStartDate.Value = SAV.EnrollmentDate.Timestamp;
    }

    private void Save()
    {
        SaveTrainerInfo();
        SaveMap();
    }

    private void SaveMap()
    {
        if (!MapUpdated)
            return;
        SAV.SetCoordinates((float)NUD_X.Value, (float)NUD_Y.Value, (float)NUD_Z.Value);
    }

    private void SaveTrainerInfo()
    {
        SAV.Game = (byte)(CB_Game.SelectedIndex + (int)GameVersion.SL);
        SAV.Gender = (byte)CB_Gender.SelectedIndex;
        SAV.Money = Util.ToUInt32(MT_Money.Text);
        SAV.LeaguePoints = Util.ToUInt32(MT_LP.Text);
        SAV.Language = WinFormsUtil.GetIndex(CB_Language);
        SAV.OT = TB_OTName.Text;

        // Save PlayTime
        SAV.PlayedHours = ushort.Parse(MT_Hours.Text);
        SAV.PlayedMinutes = ushort.Parse(MT_Minutes.Text) % 60;
        SAV.PlayedSeconds = ushort.Parse(MT_Seconds.Text) % 60;

        SAV.EnrollmentDate.Timestamp = CAL_AdventureStartDate.Value;
    }

    private void ClickOT(object sender, MouseEventArgs e)
    {
        TextBox tb = sender as TextBox ?? TB_OTName;
        // Special Character Form
        if (ModifierKeys != Keys.Control)
            return;

        var d = new TrashEditor(tb, SAV);
        d.ShowDialog();
        tb.Text = d.FinalString;
    }

    private void B_Cancel_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void B_Save_Click(object sender, EventArgs e)
    {
        Save();
        Origin.CopyChangesFrom(SAV);
        Close();
    }

    private void ChangeMapValue(object sender, EventArgs e)
    {
        if (!Loading)
            MapUpdated = true;
    }

    private void Change255(object sender, EventArgs e)
    {
        MaskedTextBox box = (MaskedTextBox)sender;
        if (box.Text.Length == 0) box.Text = "0";
        if (Util.ToInt32(box.Text) > 255) box.Text = "255";
    }

    private void B_UnlockFlyLocations_Click(object sender, EventArgs e) => UnlockFlyLocations();

    private void UnlockFlyLocations()
    {
        var blocks = new[]
        {
            #region Fly Flags
            FSYS_YMAP_FLY_01,
            FSYS_YMAP_FLY_02,
            FSYS_YMAP_FLY_03,
            FSYS_YMAP_FLY_04,
            FSYS_YMAP_FLY_05,
            FSYS_YMAP_FLY_06,
            FSYS_YMAP_FLY_07,
            FSYS_YMAP_FLY_08,
            FSYS_YMAP_FLY_09,
            FSYS_YMAP_FLY_10,
            FSYS_YMAP_FLY_11,
            FSYS_YMAP_FLY_12,
            FSYS_YMAP_FLY_13,
            FSYS_YMAP_FLY_14,
            FSYS_YMAP_FLY_15,
            FSYS_YMAP_FLY_16,
            FSYS_YMAP_FLY_17,
            FSYS_YMAP_FLY_18,
            FSYS_YMAP_FLY_19,
            FSYS_YMAP_FLY_20,
            FSYS_YMAP_FLY_21,
            FSYS_YMAP_FLY_22,
            FSYS_YMAP_FLY_23,
            FSYS_YMAP_FLY_24,
            FSYS_YMAP_FLY_25,
            FSYS_YMAP_FLY_26,
            FSYS_YMAP_FLY_27,
            FSYS_YMAP_FLY_28,
            FSYS_YMAP_FLY_29,
            FSYS_YMAP_FLY_30,
            FSYS_YMAP_FLY_31,
            FSYS_YMAP_FLY_32,
            FSYS_YMAP_FLY_33,
            FSYS_YMAP_FLY_34,
            FSYS_YMAP_FLY_35,
            FSYS_YMAP_FLY_MAGATAMA,
            FSYS_YMAP_FLY_MOKKAN,
            FSYS_YMAP_FLY_TSURUGI,
            FSYS_YMAP_FLY_UTSUWA,
            FSYS_YMAP_POKECEN_02,
            FSYS_YMAP_POKECEN_03,
            FSYS_YMAP_POKECEN_04,
            FSYS_YMAP_POKECEN_05,
            FSYS_YMAP_POKECEN_06,
            FSYS_YMAP_POKECEN_07,
            FSYS_YMAP_POKECEN_08,
            FSYS_YMAP_POKECEN_09,
            FSYS_YMAP_POKECEN_10,
            FSYS_YMAP_POKECEN_11,
            FSYS_YMAP_POKECEN_12,
            FSYS_YMAP_POKECEN_13,
            FSYS_YMAP_POKECEN_14,
            FSYS_YMAP_POKECEN_15,
            FSYS_YMAP_POKECEN_16,
            FSYS_YMAP_POKECEN_17,
            FSYS_YMAP_POKECEN_18,
            FSYS_YMAP_POKECEN_19,
            FSYS_YMAP_POKECEN_20,
            FSYS_YMAP_POKECEN_21,
            FSYS_YMAP_POKECEN_22,
            FSYS_YMAP_POKECEN_23,
            FSYS_YMAP_POKECEN_24,
            FSYS_YMAP_POKECEN_25,
            FSYS_YMAP_POKECEN_26,
            FSYS_YMAP_POKECEN_27,
            FSYS_YMAP_POKECEN_28,
            FSYS_YMAP_POKECEN_29,
            FSYS_YMAP_POKECEN_30,
            FSYS_YMAP_POKECEN_31,
            FSYS_YMAP_POKECEN_32,
            FSYS_YMAP_POKECEN_33,
            FSYS_YMAP_POKECEN_34,
            FSYS_YMAP_POKECEN_35,

            // Treasures of Ruin shrine toggles
            FSYS_YMAP_MAGATAMA,
            FSYS_YMAP_MOKKAN,
            FSYS_YMAP_TSURUGI,
            FSYS_YMAP_UTSUWA,
            #endregion
        };
        var accessor = SAV.Accessor;
        foreach (var block in blocks)
            accessor.GetBlock(block).ChangeBooleanType(SCTypeCode.Bool2);
        System.Media.SystemSounds.Asterisk.Play();
    }

    private void B_CollectAllStakes_Click(object sender, EventArgs e)
    {
        SAV.CollectAllStakes();
        System.Media.SystemSounds.Asterisk.Play();
    }

    private void B_UnlockTMRecipes_Click(object sender, EventArgs e)
    {
        SAV.UnlockAllTMRecipes();
        System.Media.SystemSounds.Asterisk.Play();
    }

    private void B_UnlockBikeUpgrades_Click(object sender, EventArgs e)
    {
        var blocks = new[]
        {
            "FSYS_RIDE_DASH_ENABLE",
            "FSYS_RIDE_SWIM_ENABLE",
            "FSYS_RIDE_HIJUMP_ENABLE",
            "FSYS_RIDE_GLIDE_ENABLE",
            "FSYS_RIDE_CLIMB_ENABLE",
        };

        var accessor = SAV.Accessor;
        foreach (var block in blocks)
            accessor.GetBlock(block).ChangeBooleanType(SCTypeCode.Bool2);
        System.Media.SystemSounds.Asterisk.Play();
    }

    private static void IMG_Save(System.Drawing.Image image, string name)
    {
        SaveFileDialog saveFileDialog = new SaveFileDialog();
        saveFileDialog.FileName = name;
        saveFileDialog.Filter = "Images|*.png;*.bmp;*.jpg";
        if (saveFileDialog.ShowDialog() == DialogResult.OK)
        {
            ImageFormat format = Path.GetExtension(saveFileDialog.FileName) switch
            {
                ".jpg" or ".jpeg" => ImageFormat.Jpeg,
                ".bmp" => ImageFormat.Bmp,
                ".png" or _ => ImageFormat.Png,
            };
            image.Save(saveFileDialog.FileName, format);
            System.Media.SystemSounds.Asterisk.Play();
        }
    }

    private void P_CurrPhoto_Click(object sender, EventArgs e)
    {
        IMG_Save(P_CurrPhoto.Image, "current_photo");
    }

    private void P_CurrIcon_Click(object sender, EventArgs e)
    {
        IMG_Save(P_CurrIcon.Image, "current_icon");
    }

    private void P_InitialIcon_Click(object sender, EventArgs e)
    {
        IMG_Save(P_InitialIcon.Image, "initial_icon");
    }
}
