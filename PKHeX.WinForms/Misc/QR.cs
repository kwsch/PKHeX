using System;
using System.Drawing;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Drawing.Misc;
using static PKHeX.Core.MessageStrings;

namespace PKHeX.WinForms;

public partial class QR : Form
{
    private readonly PKM? Entity;
    private readonly Image icon;
    private Image qr;

    private readonly string[] Lines;
    private string extraText = string.Empty;

    public QR(Image qr, Image icon, params string[] lines)
    {
        InitializeComponent();
        this.qr = qr;
        this.icon = icon;
        Lines = lines;
        splitContainer1.Panel1Collapsed = true;
        RefreshImage();
        ResizeWindow();
    }

    public QR(Image qr, Image icon, PKM pk, params string[] lines)
    {
        InitializeComponent();
        this.qr = qr;
        this.icon = icon;
        Lines = lines;

        Entity = pk;
        // Layer on Text
        if (pk is PK7 pk7)
            this.qr = ReloadQRData(pk7);
        else
            splitContainer1.Panel1Collapsed = true;

        RefreshImage();
        ResizeWindow();
        splitContainer1.SplitterDistance = 34;
    }

    private void ResizeWindow()
    {
        var img = PB_QR.Image;
        splitContainer1.Height = splitContainer1.Panel1.Height + img.Height;
        splitContainer1.Width = img.Width;
    }

    private Bitmap ReloadQRData(PK7 pk7)
    {
        var box = (int)NUD_Box.Value - 1;
        var slot = (int)NUD_Slot.Value - 1;
        var copies = (int)NUD_Copies.Value;
        extraText = $" (Box {box + 1}, Slot {slot + 1}, {copies} cop{(copies > 1 ? "ies" : "y")})";
        return QREncode.GenerateQRCode7(pk7, box, slot, copies);
    }

    private void RefreshImage()
    {
        SuspendLayout();
        ResumeLayout();
        var font = !Main.Unicode ? Font : FontUtil.GetPKXFont(8.25f);

        var width = Math.Max(qr.Width, 370);
        var height = qr.Height + 50;
        var img = QRImageUtil.GetQRImageExtended(font, qr, icon, width, height, Lines, extraText);
        PB_QR.Image = img;
    }

    private void PB_QR_Click(object sender, EventArgs e)
    {
        if (DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, MsgQRClipboardImage))
            return;
        try { Clipboard.SetImage(PB_QR.Image); }
        // Clipboard can be locked periodically, just notify on failure.
        catch { WinFormsUtil.Alert(MsgQRClipboardFail); }
    }

    private void UpdateBoxSlotCopies(object sender, EventArgs e)
    {
        if (Entity is not PK7 pk7)
            throw new ArgumentException("Can't update QR7 if pk isn't a PK7!");
        qr = ReloadQRData(pk7);
        RefreshImage();
    }
}
