using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Drawing;
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

        GetImages();
        GetComboBoxes();
        GetTextBoxes();
        LoadMap();

        CB_Fashion.SelectedIndex = 0;

        Loading = false;
    }

    private void GetImages()
    {
        static Image GetImage(SCBlockAccessor blocks, uint kd, uint kw, uint kh)
        {
            var data = blocks.GetBlock(kd).Data;
            var width = blocks.GetBlockValue<uint>(kw);
            var height = blocks.GetBlockValue<uint>(kh);
            var result = DXT1.Decompress(data, (int)width, (int)height);
            return ImageUtil.GetBitmap(result, (int)width, (int)height, PixelFormat.Format32bppArgb);
        }

        var blocks = SAV.Blocks;
        P_CurrPhoto.Image = GetImage(blocks, KPictureProfileCurrent, KPictureProfileCurrentWidth, KPictureProfileCurrentHeight);
        P_CurrIcon.Image = GetImage(blocks, KPictureIconCurrent, KPictureIconCurrentWidth, KPictureIconCurrentHeight);
        P_InitialIcon.Image = GetImage(blocks, KPictureIconInitial, KPictureIconInitialWidth, KPictureIconInitialHeight);
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

            // Sudachi 1
            FSYS_YMAP_SU1MAP_CHANGE, // can change map to Kitakami
            FSYS_YMAP_FLY_SU1_AREA10,
            FSYS_YMAP_FLY_SU1_BUSSTOP,
            FSYS_YMAP_FLY_SU1_CENTER01,
            FSYS_YMAP_FLY_SU1_PLAZA,
            FSYS_YMAP_FLY_SU1_SPOT01,
            FSYS_YMAP_FLY_SU1_SPOT02,
            FSYS_YMAP_FLY_SU1_SPOT03,
            FSYS_YMAP_FLY_SU1_SPOT04,
            FSYS_YMAP_FLY_SU1_SPOT05,
            FSYS_YMAP_FLY_SU1_SPOT06,
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

    private static void IMG_Save(Image image, string name)
    {
        var sfd = new SaveFileDialog
        {
            FileName = name,
            Filter = "Images|*.png;*.bmp;*.jpg",
        };
        if (sfd.ShowDialog() != DialogResult.OK)
            return;

        var path = sfd.FileName;
        var format = Path.GetExtension(path) switch
        {
            ".jpg" or ".jpeg" => ImageFormat.Jpeg,
            ".bmp" => ImageFormat.Bmp,
            _ => ImageFormat.Png,
        };
        image.Save(path, format);
        System.Media.SystemSounds.Asterisk.Play();
    }

    private void P_CurrPhoto_Click(object sender, EventArgs e) => IMG_Save(P_CurrPhoto.Image, "current_photo");
    private void P_CurrIcon_Click(object sender, EventArgs e) => IMG_Save(P_CurrIcon.Image, "current_icon");
    private void P_InitialIcon_Click(object sender, EventArgs e) => IMG_Save(P_InitialIcon.Image, "initial_icon");

    private void B_UnlockClothing_Click(object sender, EventArgs e)
    {
        var accessor = SAV.Accessor;
        var added = CB_Fashion.SelectedIndex switch
        {
            0 => PlayerFashionUnlock9.UnlockBase(accessor, SAV.Gender),
            1 => PlayerFashionUnlock9.UnlockExtras(accessor),
            2 => PlayerFashionUnlock9.UnlockPreorder(accessor, SAV.Gender),
            3 => PlayerFashionUnlock9.UnlockPortal(accessor),
            _ => throw new Exception("Invalid fashion type."),
        };
        WinFormsUtil.Alert(string.Format(MessageStrings.MsgClothingAdded, added));
        System.Media.SystemSounds.Asterisk.Play();
    }
}
