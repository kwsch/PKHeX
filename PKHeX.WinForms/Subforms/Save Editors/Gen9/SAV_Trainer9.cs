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

    public static class DXT1Decompressor
    {
        public static Bitmap DecompressDXT1(byte[] dxt1Data, int width, int height)
        {
            Bitmap result = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData bmpData = result.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            IntPtr ptr = bmpData.Scan0;

            int bytes = Math.Abs(bmpData.Stride) * height;
            byte[] rgbValues = new byte[bytes];

            int blockCountX = width / 4;
            int blockCountY = height / 4;

            for (int y = 0; y < blockCountY; y++)
            {
                for (int x = 0; x < blockCountX; x++)
                {
                    int blockOffset = (y * blockCountX + x) * 8;
                    ushort color0 = BitConverter.ToUInt16(dxt1Data, blockOffset);
                    ushort color1 = BitConverter.ToUInt16(dxt1Data, blockOffset + 2);
                    uint indices = BitConverter.ToUInt32(dxt1Data, blockOffset + 4);

                    Color[] blockColors = new Color[4];
                    blockColors[0] = RGB565ToColor(color0);
                    blockColors[1] = RGB565ToColor(color1);

                    if (color0 > color1)
                    {
                        blockColors[2] = ColorExtensions.Lerp(blockColors[0], blockColors[1], 1f / 3f);
                        blockColors[3] = ColorExtensions.Lerp(blockColors[0], blockColors[1], 2f / 3f);
                    }
                    else
                    {
                        blockColors[2] = ColorExtensions.Lerp(blockColors[0], blockColors[1], 0.5f);
                        blockColors[3] = Color.FromArgb(0, 0, 0, 0);
                    }

                    for (int pixelY = 0; pixelY < 4; pixelY++)
                    {
                        for (int pixelX = 0; pixelX < 4; pixelX++)
                        {
                            int pixelIndex = (y * 4 + pixelY) * width + x * 4 + pixelX;
                            int index = (int)((indices >> (2 * (4 * pixelY + pixelX))) & 0x3);

                            int byteArrayIndex = pixelIndex * 4;
                            rgbValues[byteArrayIndex] = blockColors[index].B;
                            rgbValues[byteArrayIndex + 1] = blockColors[index].G;
                            rgbValues[byteArrayIndex + 2] = blockColors[index].R;
                            rgbValues[byteArrayIndex + 3] = blockColors[index].A;
                        }
                    }
                }
            }

            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);
            result.UnlockBits(bmpData);

            return result;
        }

        private static Color RGB565ToColor(ushort rgb565)
        {
            byte r = (byte)((rgb565 >> 11) & 0x1F);
            byte g = (byte)((rgb565 >> 5) & 0x3F);
            byte b = (byte)(rgb565 & 0x1F);

            return Color.FromArgb(
                255, (byte)(r << 3 | r >> 2),
            (byte)(g << 2 | g >> 4),
            (byte)(b << 3 | b >> 2));
        }
    }

    public static class ColorExtensions
    {
        public static Color Lerp(Color c1, Color c2, float t)
        {
            int r = (int)(c1.R + (c2.R - c1.R) * t);
            int g = (int)(c1.G + (c2.G - c1.G) * t);
            int b = (int)(c1.B + (c2.B - c1.B) * t);
            int aVal = (int)(c1.A + (c2.A - c1.A) * t);

            return Color.FromArgb(aVal, r, g, b);
        }
    }

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

        P_CurrPhoto.Image = DXT1Decompressor.DecompressDXT1(SAV.Accessor.GetBlock(SaveBlockAccessor9SV.KPictureProfileCurrent).Data,
            (int)SAV.Blocks.GetBlockValue<uint>(SaveBlockAccessor9SV.KPictureProfileCurrentWidth),
            (int)SAV.Blocks.GetBlockValue<uint>(SaveBlockAccessor9SV.KPictureProfileCurrentHeight));
        P_CurrIcon.Image = DXT1Decompressor.DecompressDXT1(SAV.Accessor.GetBlock(SaveBlockAccessor9SV.KPictureIconCurrent).Data,
            (int)SAV.Blocks.GetBlockValue<uint>(SaveBlockAccessor9SV.KPictureIconCurrentWidth),
            (int)SAV.Blocks.GetBlockValue<uint>(SaveBlockAccessor9SV.KPictureIconCurrentHeight));
        P_InitialIcon.Image = DXT1Decompressor.DecompressDXT1(SAV.Accessor.GetBlock(SaveBlockAccessor9SV.KPictureIconInitial).Data,
            (int)SAV.Blocks.GetBlockValue<uint>(SaveBlockAccessor9SV.KPictureIconInitialWidth),
            (int)SAV.Blocks.GetBlockValue<uint>(SaveBlockAccessor9SV.KPictureIconInitialHeight));
        P_CurrPhoto.Height = P_CurrPhoto.Image.Height / 4;
        P_CurrPhoto.Width = P_CurrPhoto.Image.Width / 4;
        P_CurrIcon.Height = P_CurrIcon.Image.Height / 4;
        P_CurrIcon.Width = P_CurrIcon.Image.Width / 4;
        P_InitialIcon.Height = P_InitialIcon.Image.Height / 4;
        P_InitialIcon.Width = P_InitialIcon.Image.Width / 4;
        P_CurrIcon.Location = new Point(P_CurrPhoto.Location.X + P_CurrPhoto.Width + 8, P_CurrPhoto.Location.Y);
        P_InitialIcon.Location = new Point(P_CurrIcon.Location.X, P_CurrIcon.Location.Y + P_CurrIcon.Height + 8);

        GetComboBoxes();
        GetTextBoxes();
        LoadMap();

        Loading = false;
    }

    private readonly bool Loading;
    private bool MapUpdated;

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
