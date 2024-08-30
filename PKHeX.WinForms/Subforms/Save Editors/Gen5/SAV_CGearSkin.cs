using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public partial class SAV_CGearSkin : Form
{
    private readonly SAV5 SAV;
    private readonly CGearBackground bg;
    private const string FilterBW = $"PokeStock C-Gear Skin Background|*.{CGearBackgroundBW.Extension}";
    private const string FilterB2W2 = $"C-Gear Background|*.{CGearBackgroundB2W2.Extension}";

    public SAV_CGearSkin(SAV5 sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = sav;

        var data = SAV.CGearSkinData.ToArray();
        bg = sav is SAV5BW ? new CGearBackgroundBW(data) : new CGearBackgroundB2W2(data);

        PB_Background.Image = CGearImage.GetBitmap(bg);
    }

    private void B_ImportPNG_Click(object sender, EventArgs e)
    {
        using var ofd = new OpenFileDialog();
        ofd.Filter = "PNG File|*.png";
        ofd.FileName = "Background.png";
        if (ofd.ShowDialog() != DialogResult.OK)
            return;

        try
        {
            using var img = (Bitmap)Image.FromFile(ofd.FileName);
            if (!IsInputCorrect<CGearBackground>(img, out var msg))
            {
                WinFormsUtil.Alert(msg);
                return;
            }
            var result = CGearImage.GetCGearBackground(img, bg);
            PB_Background.Image = CGearImage.GetBitmap(bg); // regenerate rather than reuse input
            if (CheckResult<CGearBackground>(result, out msg))
                System.Media.SystemSounds.Asterisk.Play();
            else
                WinFormsUtil.Alert(msg);
        }
        catch (Exception ex)
        {
            WinFormsUtil.Error(ex.Message);
        }
    }

    private static bool IsInputCorrect<T>(Bitmap img, [NotNullWhen(false)] out string? msg) where T : ITiledImage
    {
        if (img.Width != T.Width || img.Height != T.Height)
        {
            msg = $"Incorrect image dimensions. Expected {T.Width}x{T.Height}";
            return false;
        }
        if (img.PixelFormat != PixelFormat.Format32bppArgb)
        {
            msg = $"Incorrect image format. Expected {PixelFormat.Format32bppArgb}";
            return false;
        }
        msg = null;
        return true;
    }

    private static bool CheckResult<T>(TiledImageStat result, [NotNullWhen(false)] out string? msg)
        where T : ITiledImage
    {
        bool tooManyColors = result.ColorCount > T.ColorCount;
        bool tooManyTiles = result.TileCount > T.TilePoolCount;
        if (!tooManyColors && !tooManyTiles)
        {
            msg = null;
            return true; // Success
        }

        msg = "";
        if (tooManyColors)
            msg += $"Too many colors. Expected: {T.ColorCount}, received {result.ColorCount}";
        if (tooManyTiles)
            msg += (msg.Length != 0 ? Environment.NewLine : "") + $"Too many tiles. Expected {T.TilePoolCount}, received {result.TileCount}";
        return false;
    }

    private void B_ExportPNG_Click(object sender, EventArgs e)
    {
        using var sfd = new SaveFileDialog();
        sfd.Filter = "PNG File|*.png";
        sfd.FileName = "Background.png";
        if (sfd.ShowDialog() != DialogResult.OK)
            return;

        PB_Background.Image.Save(sfd.FileName, ImageFormat.Png);
    }

    private void B_ImportCGB_Click(object sender, EventArgs e)
    {
        using var ofd = new OpenFileDialog();
        bool isBW = SAV is SAV5BW;
        ofd.Filter = isBW ? $"{FilterBW}|{FilterB2W2}" : $"{FilterB2W2}|{FilterBW}";

        if (ofd.ShowDialog() != DialogResult.OK)
            return;

        var path = ofd.FileName;
        var len = new FileInfo(path).Length;
        if (len != CGearBackground.SIZE)
        {
            WinFormsUtil.Error($"Incorrect size, got {len} bytes, expected {CGearBackground.SIZE} bytes.");
            return;
        }

        byte[] data = File.ReadAllBytes(path);

        // Load the data and adjust it to the correct game format if not matching.
        CGearBackground temp = isBW ? new CGearBackgroundBW(data) : new CGearBackgroundB2W2(data);
        bool isPSK = PaletteTileSelection.IsPaletteShiftFormat(temp.Arrange);

        try
        {
            if (isBW && !isPSK)
                PaletteTileSelection.ConvertToShiftFormat<CGearBackgroundBW>(temp.Arrange);
            else if (!isBW && isPSK)
                PaletteTileSelection.ConvertFromShiftFormat(temp.Arrange);
        }
        catch (Exception ex)
        {
            WinFormsUtil.Error(ex.Message);
            return;
        }

        temp.Data.CopyTo(bg.Data);
        PB_Background.Image = CGearImage.GetBitmap(bg);
    }

    private void B_ExportCGB_Click(object sender, EventArgs e)
    {
        using var sfd = new SaveFileDialog();
        sfd.Filter = SAV is SAV5BW ? FilterBW : FilterB2W2;

        if (sfd.ShowDialog() != DialogResult.OK)
            return;
        File.WriteAllBytes(sfd.FileName, bg.Data.ToArray());
    }

    private void B_Save_Click(object sender, EventArgs e)
    {
        if (bg.Data.ContainsAnyExcept<byte>(0))
            SAV.SetCGearSkin(bg.Data);
        Close();
    }

    private void B_Cancel_Click(object sender, EventArgs e)
    {
        Close();
    }
}
