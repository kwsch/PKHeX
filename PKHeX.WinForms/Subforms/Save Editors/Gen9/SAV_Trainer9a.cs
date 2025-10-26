using PKHeX.Core;
using PKHeX.Drawing;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using static PKHeX.Core.SaveBlockAccessor9ZA;

namespace PKHeX.WinForms;

public sealed partial class SAV_Trainer9a : Form
{
    private readonly SaveFile Origin;
    private readonly SAV9ZA SAV;

    public SAV_Trainer9a(SAV9ZA sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (SAV9ZA)(Origin = sav).Clone();

        Loading = true;
        if (Main.Unicode)
            TB_OTName.Font = FontUtil.GetPKXFont();

        B_MaxCash.Click += (_, _) => MT_Money.Text = SAV.MaxMoney.ToString();
        B_RoyaleRegularMax.Click += (_, _) => MT_RoyaleRegular.Text = 310_000.ToString();
        B_RoyaleInfiniteMax.Click += (_, _) => MT_RoyaleInfinite.Text = 50_000.ToString();

        CB_Gender.Items.Clear();
        CB_Gender.Items.AddRange(Main.GenderSymbols.Take(2).ToArray()); // m/f depending on unicode selection

        GetImages();
        GetComboBoxes();
        GetTextBoxes();
        LoadMap();

        MinimumSize = Size;
        MaximumSize = Size with { Width = Width * 2, Height = Height * 2 };

        Loading = false;
    }

    private void GetImages()
    {
        var blocks = SAV.Blocks;
        var result = false;
        result |= SetImage(P_Picture1, GetImage(blocks, KPictureCurrentData, KPictureCurrentWidth, KPictureCurrentHeight));
        result |= SetImage(P_Picture2, GetImage(blocks, KPictureSBCData, KPictureSBCWidth, KPictureSBCHeight));
        result |= SetImage(P_Picture3, GetImage(blocks, KPictureInitialData, KPictureInitialWidth, KPictureInitialHeight));

        if (!result)
            TC_Editor.TabPages.Remove(Tab_Images);
        return;

        static Bitmap? GetImage(SCBlockAccessor blocks, uint kd, uint kw, uint kh)
        {
            var width = blocks.GetBlockValue<uint>(kw);
            var height = blocks.GetBlockValue<uint>(kh);
            if (width == 0 && height == 0) // No image present
                return null;
            var data = blocks.GetBlock(kd).Data;
            var result = DXT1.Decompress(data, (int)width, (int)height);
            return ImageUtil.GetBitmap(result, (int)width, (int)height, PixelFormat.Format32bppArgb);
        }

        static bool SetImage(PictureBox pb, Image? img)
        {
            if (img is null)
            {
                pb.Visible = false;
                return false;
            }

            pb.SizeMode = PictureBoxSizeMode.AutoSize;
            pb.Image = img;
            return true;
        }
    }

    private readonly bool Loading;
    private bool MapUpdated;

    private void LoadMap()
    {
        try
        {
            NUD_X.Value = (decimal)(double)SAV.Coordinates.X;
            NUD_Y.Value = (decimal)(double)SAV.Coordinates.Y;
            NUD_Z.Value = (decimal)(double)SAV.Coordinates.Z;
            NUD_R.Value = (decimal)SAV.Coordinates.Rotation;
            TB_Map.Text = SAV.Coordinates.Map;
        }
        // If we can't accurately represent the coordinates, don't allow them to be changed.
        catch { GB_Map.Enabled = false; }
    }

    private void GetComboBoxes()
    {
        CB_Language.InitializeBinding();
        CB_Language.DataSource = GameInfo.LanguageDataSource(SAV.Generation, SAV.Context);
    }

    private void GetTextBoxes()
    {
        // Get Data
        CB_Gender.SelectedIndex = SAV.Gender;

        // Display Data
        TB_OTName.Text = SAV.OT;
        trainerID1.LoadIDValues(SAV, SAV.Generation);
        MT_Money.Text = SAV.Money.ToString();
        CB_Language.SelectedValue = SAV.Language;

        // Load Play Time
        MT_Hours.Text = SAV.PlayedHours.ToString();
        MT_Minutes.Text = SAV.PlayedMinutes.ToString();
        MT_Seconds.Text = SAV.PlayedSeconds.ToString();

        //CAL_AdventureStartDate.Value = SAV.StartTime.Timestamp;
        CAL_LastSavedDate.Value = CAL_LastSavedTime.Value = SAV.LastSaved.Timestamp;

        MT_RoyaleRegular.Text = SAV.TicketPointsRoyale.ToString();
        MT_RoyaleInfinite.Text = SAV.TicketPointsRoyaleInfinite.ToString();
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
        SAV.Coordinates.SetCoordinates((float)NUD_X.Value, (float)NUD_Y.Value, (float)NUD_Z.Value);
        SAV.Coordinates.SetPlayerRotation((double)NUD_R.Value);
        SAV.Coordinates.Map = TB_Map.Text;
    }

    private void SaveTrainerInfo()
    {
        SAV.Gender = (byte)CB_Gender.SelectedIndex;

        SAV.Money = Util.ToUInt32(MT_Money.Text);
        SAV.Language = WinFormsUtil.GetIndex(CB_Language);
        SAV.OT = TB_OTName.Text;

        // Save PlayTime
        SAV.PlayedHours = ushort.Parse(MT_Hours.Text);
        SAV.PlayedMinutes = ushort.Parse(MT_Minutes.Text) % 60;
        SAV.PlayedSeconds = ushort.Parse(MT_Seconds.Text) % 60;

        //SAV.StartTime.Timestamp = CAL_AdventureStartDate.Value;
        SAV.LastSaved.Timestamp = CAL_LastSavedDate.Value.Date.AddSeconds(CAL_LastSavedTime.Value.TimeOfDay.TotalSeconds);

        SAV.TicketPointsRoyale = Util.ToUInt32(MT_RoyaleRegular.Text);
        SAV.TicketPointsRoyaleInfinite = Util.ToUInt32(MT_RoyaleInfinite.Text);
    }

    private void ClickOT(object sender, MouseEventArgs e)
    {
        TextBox tb = sender as TextBox ?? TB_OTName;
        // Special Character Form
        if (ModifierKeys != Keys.Control)
            return;

        var d = new TrashEditor(tb, SAV, SAV.Generation, SAV.Context);
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

    private void P_Picture1_Click(object sender, EventArgs e) => SAV_Trainer9.IMG_Save(P_Picture1.Image!, nameof(KPictureSBCData));
    private void P_Picture2_Click(object sender, EventArgs e) => SAV_Trainer9.IMG_Save(P_Picture2.Image!, nameof(KPictureInitialData));
    private void P_Picture3_Click(object sender, EventArgs e) => SAV_Trainer9.IMG_Save(P_Picture3.Image!, nameof(KPictureCurrentData));
}
