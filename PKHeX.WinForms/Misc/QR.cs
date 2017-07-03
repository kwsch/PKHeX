using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows.Forms;
using PKHeX.Core;
using QRCoder;

namespace PKHeX.WinForms
{
    public partial class QR : Form
    {
        private readonly PKM pkm;
        private readonly Image icon;
        private Image qr;

        private readonly string[] Lines;
        private string extraText;

        public QR(Image qr, Image icon, PKM pk, params string[] lines)
        {
            InitializeComponent();
            pkm = pk;

            // Layer on Text
            const int stretch = 50;
            Height += stretch;

            if (pkm != null && pkm.Format == 7)
                Height += 40;

            this.qr = qr;
            this.icon = icon;
            Lines = lines;

            if (pkm != null && pkm.Format == 7)
                UpdateBoxSlotCopies(null, null);
            else
                RefreshImage();
        }

        private void RefreshImage()
        {
            Font font = !Main.Unicode ? FontLabel.Font : FontUtil.GetPKXFont((float)8.25);
            Image preview = new Bitmap(45, 45);
            using (Graphics gfx = Graphics.FromImage(preview))
            {
                gfx.FillRectangle(new SolidBrush(Color.White), 0, 0, preview.Width, preview.Height);
                gfx.DrawImage(icon, preview.Width / 2 - icon.Width / 2, preview.Height / 2 - icon.Height / 2);
            }
            // Layer on Preview Image
            Image pic = ImageUtil.LayerImage(qr, preview, qr.Width / 2 - preview.Width / 2, qr.Height / 2 - preview.Height / 2, 1);

            Image newpic = new Bitmap(PB_QR.Width, PB_QR.Height);
            using (Graphics g = Graphics.FromImage(newpic))
            {
                g.FillRectangle(new SolidBrush(Color.White), 0, 0, newpic.Width, newpic.Height);
                g.DrawImage(pic, 0, 0);

                g.DrawString(GetLine(0), font, Brushes.Black, new PointF(18, qr.Height - 5));
                g.DrawString(GetLine(1), font, Brushes.Black, new PointF(18, qr.Height + 8));
                g.DrawString(GetLine(2).Replace(Environment.NewLine, "/").Replace("//", "   ").Replace(":/", ": "), font, Brushes.Black, new PointF(18, qr.Height + 20));
                g.DrawString(GetLine(3) + extraText, font, Brushes.Black, new PointF(18, qr.Height + 32));
            }
            PB_QR.BackgroundImage = newpic;
        }

        private string GetLine(int line) => Lines.Length <= line ? string.Empty : Lines[line];

        private void PB_QR_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "Copy QR Image to Clipboard?")) return;
            try { Clipboard.SetImage(PB_QR.BackgroundImage); }
            catch { WinFormsUtil.Alert("Failed to set Image to Clipboard"); }
        }
        
        // QR Utility
        private const string QR6PathBad = "null/#"; // prefix to prevent URL from loading
        private const string QR6Path = @"http://lunarcookies.github.io/b1s1.html#";

        internal static byte[] GetQRData(string address)
        {
            // Fetch data from QR code...
            try { if (address.Length < 4 || address.Substring(0, 3) != "htt") { WinFormsUtil.Alert("Clipboard text is not a valid URL:", address); return null; } }
            catch { WinFormsUtil.Alert("Clipboard text is not a valid URL:", address); return null; }
            string webURL = "http://api.qrserver.com/v1/read-qr-code/?fileurl=" + HttpUtility.UrlEncode(address);
            try
            {
                string data = NetUtil.GetStringFromURL(webURL);
                if (data.Contains("could not find")) { WinFormsUtil.Alert("Reader could not find QR data in the image."); return null; }
                if (data.Contains("filetype not supported")) { WinFormsUtil.Alert("Input URL is not valid. Double check that it is an image (jpg/png).", address); return null; }
                // Quickly convert the json response to a data string
                const string cap = "\",\"error\":null}]}]";
                const string intro = "[{\"type\":\"qrcode\",\"symbol\":[{\"seq\":0,\"data\":\"";
                if (!data.StartsWith(intro))
                    throw new FormatException();

                string pkstr = data.Substring(intro.Length);
                if (pkstr.Contains("nQR-Code:")) // Remove multiple QR codes in same image
                    pkstr = pkstr.Substring(0, pkstr.IndexOf("nQR-Code:", StringComparison.Ordinal));
                pkstr = pkstr.Substring(0, pkstr.IndexOf(cap, StringComparison.Ordinal)); // Trim outro
                try
                {
                    if (!pkstr.StartsWith("http") && !pkstr.StartsWith(QR6PathBad)) // G7
                    {
                        string fstr = Regex.Unescape(pkstr);
                        byte[] raw = Encoding.Unicode.GetBytes(fstr);
                        // Remove 00 interstitials and retrieve from offset 0x30, take PK7 Stored Size (always)
                        return raw.ToList().Where((c, i) => i % 2 == 0).Skip(0x30).Take(0xE8).ToArray();
                    } 
                    // All except G7
                    pkstr = pkstr.Substring(pkstr.IndexOf("#", StringComparison.Ordinal) + 1); // Trim URL
                    pkstr = pkstr.Replace("\\", ""); // Rectify response

                    return Convert.FromBase64String(pkstr);
                }
                catch { WinFormsUtil.Alert("QR string to Data failed."); return null; }
            }
            catch { WinFormsUtil.Alert("Unable to connect to the internet to decode QR code."); return null; }
        }
        internal static Image GetQRImage(byte[] data, string server)
        {
            string qrdata = Convert.ToBase64String(data);
            string message = server + qrdata;
            string webURL = "http://chart.apis.google.com/chart?chs=365x365&cht=qr&chl=" + HttpUtility.UrlEncode(message);

            try
            {
                return NetUtil.GetImageFromURL(webURL);
            }
            catch
            {
                if (DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "Unable to connect to the internet to receive QR code.", "Copy QR URL to Clipboard?"))
                    return null;
                try { Clipboard.SetText(webURL); }
                catch { WinFormsUtil.Alert("Failed to set text to Clipboard"); }
            }
            return null;
        }

        private void UpdateBoxSlotCopies(object sender, EventArgs e)
        {
            if (pkm == null || pkm.Format != 7)
                throw new ArgumentException("Can't update QR7 if pkm isn't a PK7!");
            var box = (int) NUD_Box.Value - 1;
            var slot = (int) NUD_Slot.Value - 1;
            var copies = (int) NUD_Copies.Value;
            var new_qr = GenerateQRCode7((PK7)pkm, box, slot, copies);
            qr = new_qr;
            SuspendLayout();
            extraText = $" (Box {box+1}, Slot {slot+1}, {copies} cop{(copies > 1 ? "ies" : "y")})";
            RefreshImage();
            ResumeLayout();
        }

        // QR7 Utility
        public static Image GenerateQRCode7(PK7 pk7, int box = 0, int slot = 0, int num_copies = 1)
        {
            byte[] data = QR7.GenerateQRData(pk7, box, slot, num_copies);
            using (var generator = new QRCodeGenerator())
            using (var qr_data = generator.CreateQRCode(data))
            using (var qr_code = new QRCode(qr_data))
                return qr_code.GetGraphic(4);
        }
        public static Image GenerateQRCode(byte[] data, int ppm = 4)
        {
            using (var generator = new QRCodeGenerator())
            using (var qr_data = generator.CreateQRCode(data))
            using (var qr_code = new QRCode(qr_data))
                return qr_code.GetGraphic(ppm);
        }

        public static string GetQRServer(int format)
        {
            switch (format)
            {
                case 6:
                    return QR6Path;
                default:
                    return QR6PathBad;
            }
        }
    }
}
