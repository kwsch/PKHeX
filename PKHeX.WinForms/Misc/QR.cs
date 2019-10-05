using System;
using System.Drawing;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Drawing;

using static PKHeX.Core.MessageStrings;

namespace PKHeX.WinForms
{
    public partial class QR : Form
    {
        private readonly PKM pkm;
        private readonly Image icon;
        private Image qr;

        private readonly string[] Lines;
        private string extraText;

        public QR(Image qr, Image icon, params string[] lines)
        {
            InitializeComponent();
            this.qr = qr;
            this.icon = icon;
            Lines = lines;

            const int stretch = 50;
            Height += stretch;
            RefreshImage();
        }

        public QR(Image qr, Image icon, PKM pk, params string[] lines)
        {
            InitializeComponent();
            this.qr = qr;
            this.icon = icon;
            Lines = lines;

            const int stretch = 50;
            Height += stretch;

            pkm = pk;
            // Layer on Text
            if (pkm.Format == 7 && pkm is PK7 pk7)
            {
                Height += 40;
                ReloadQRData(pk7);
            }
            RefreshImage();
        }

        private void ReloadQRData(PK7 pk7)
        {
            var box = (int)NUD_Box.Value - 1;
            var slot = (int)NUD_Slot.Value - 1;
            var copies = (int)NUD_Copies.Value;
            extraText = $" (Box {box + 1}, Slot {slot + 1}, {copies} cop{(copies > 1 ? "ies" : "y")})";
            qr = QREncode.GenerateQRCode7(pk7, box, slot, copies);
        }

        private void RefreshImage()
        {
            SuspendLayout();
            ResumeLayout();
            Font font = !Main.Unicode ? Font : FontUtil.GetPKXFont((float)8.25);
            PB_QR.BackgroundImage = QRImageUtil.GetQRImageExtended(font, qr, icon, PB_QR.Width, PB_QR.Height, Lines, extraText);
        }

        private void PB_QR_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, MsgQRClipboardImage))
                return;
            try { Clipboard.SetImage(PB_QR.BackgroundImage); }
            catch { WinFormsUtil.Alert(MsgQRClipboardFail); }
        }

        private void UpdateBoxSlotCopies(object sender, EventArgs e)
        {
            if (!(pkm is PK7 pk7))
                throw new ArgumentException("Can't update QR7 if pkm isn't a PK7!");
            ReloadQRData(pk7);
            RefreshImage();
        }
    }
}
