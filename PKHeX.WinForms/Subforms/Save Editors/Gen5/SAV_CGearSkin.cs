using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public partial class SAV_CGearSkin : Form
{
    private readonly SaveFile Origin;
    private readonly SAV5 SAV;

    public SAV_CGearSkin(SAV5 sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (SAV5)(Origin = sav).Clone();

        byte[] data = SAV.CGearSkinData;
        bg = new CGearBackground(data);

        PB_Background.Image = CGearImage.GetBitmap(bg);
    }

    private CGearBackground bg;

    private void B_ImportPNG_Click(object sender, EventArgs e)
    {
        using var ofd = new OpenFileDialog();
        ofd.Filter = "PNG File|*.png";
        ofd.FileName = "Background.png";
        if (ofd.ShowDialog() != DialogResult.OK)
            return;

        using var img = (Bitmap)Image.FromFile(ofd.FileName);
        try
        {
            bg = CGearImage.GetCGearBackground(img);
            PB_Background.Image = CGearImage.GetBitmap(bg);
        }
        catch (Exception ex)
        {
            WinFormsUtil.Error(ex.Message);
        }
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
        ofd.Filter = CGearBackground.Filter + "|PokeStock C-Gear Skin|*.psk";

        if (ofd.ShowDialog() != DialogResult.OK)
            return;

        var path = ofd.FileName;
        var len = new FileInfo(path).Length;
        if (len != CGearBackground.SIZE_CGB)
        {
            WinFormsUtil.Error($"Incorrect size, got {len} bytes, expected {CGearBackground.SIZE_CGB} bytes.");
            return;
        }

        byte[] data = File.ReadAllBytes(path);
        bg = new CGearBackground(data);
        PB_Background.Image = CGearImage.GetBitmap(bg);
    }

    private void B_ExportCGB_Click(object sender, EventArgs e)
    {
        using var sfd = new SaveFileDialog();
        sfd.Filter = CGearBackground.Filter;

        if (sfd.ShowDialog() != DialogResult.OK)
            return;

        var data = new byte[CGearBackground.SIZE_CGB];
        bg.Write(data, true);
        File.WriteAllBytes(sfd.FileName, data);
    }

    private void B_Save_Click(object sender, EventArgs e)
    {
        var data = new byte[CGearBackground.SIZE_CGB];
        bool cgb = SAV is SAV5B2W2;
        bg.Write(data, cgb);
        if (data.AsSpan().ContainsAnyExcept<byte>(0))
        {
            SAV.CGearSkinData = data;
            Origin.CopyChangesFrom(SAV);
        }
        Close();
    }

    private void B_Cancel_Click(object sender, EventArgs e)
    {
        Close();
    }
}
