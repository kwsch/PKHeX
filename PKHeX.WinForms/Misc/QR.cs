using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows.Forms;
using PKHeX.Core;
using QRCoder;

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

        public QR(Image qr, Image icon, PKM pk, params string[] lines)
        {
            InitializeComponent();
            pkm = pk;

            // Layer on Text
            const int stretch = 50;
            Height += stretch;

            if (pkm?.Format == 7)
                Height += 40;

            this.qr = qr;
            this.icon = icon;
            Lines = lines;

            if (pkm?.Format == 7 && pkm is PK7)
                UpdateBoxSlotCopies(null, EventArgs.Empty);
            else
                RefreshImage();
        }

        private void RefreshImage()
        {
            Font font = !Main.Unicode ? Font : FontUtil.GetPKXFont((float)8.25);
            Image preview = new Bitmap(45, 45);
            using (Graphics gfx = Graphics.FromImage(preview))
            {
                gfx.FillRectangle(new SolidBrush(Color.White), 0, 0, preview.Width, preview.Height);
                int x = (preview.Width / 2) - (icon.Width / 2);
                int y = (preview.Height / 2) - (icon.Height / 2);
                gfx.DrawImage(icon, x, y);
            }
            // Layer on Preview Image
            Image pic;
            {
                int x = (qr.Width / 2) - (preview.Width / 2);
                int y = (qr.Height / 2) - (preview.Height / 2);
                pic = ImageUtil.LayerImage(qr, preview, x, y);
            }

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
            if (DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, MsgQRClipboardImage)) return;
            try { Clipboard.SetImage(PB_QR.BackgroundImage); }
            catch { WinFormsUtil.Alert(MsgQRClipboardFail); }
        }

        // QR Utility
        private const string QR6PathBad = "null/#"; // prefix to prevent URL from loading
        private const string QR6Path = "http://lunarcookies.github.io/b1s1.html#";
        private const string DecodeAPI = "http://api.qrserver.com/v1/read-qr-code/?fileurl=";
        private const int QRSize = 365;
        private static readonly string EncodeAPI = $"http://chart.apis.google.com/chart?chs={QRSize}x{QRSize}&cht=qr&chl=";

        internal static byte[] GetQRData(string address)
        {
            // Fetch data from QR code...
            try { if (address.Length < 4 || !address.StartsWith("http")) { WinFormsUtil.Alert(MsgQRUrlFailPath, address); return Array.Empty<byte>(); } }
            catch { WinFormsUtil.Alert(MsgQRUrlFailPath, address); return Array.Empty<byte>(); }
            string webURL = DecodeAPI + HttpUtility.UrlEncode(address);
            string data;
            try
            {
                data = NetUtil.GetStringFromURL(webURL);
                if (data.Contains("could not find")) { WinFormsUtil.Alert(MsgQRUrlFailImage); return Array.Empty<byte>(); }
                if (data.Contains("filetype not supported")) { WinFormsUtil.Alert(MsgQRUrlFailType, address); return Array.Empty<byte>(); }
            }
            catch { WinFormsUtil.Alert(MsgQRUrlFailConnection); return Array.Empty<byte>(); }

            // Quickly convert the json response to a data string
            try { return DecodeQRJson(data); }
            catch (Exception e) { WinFormsUtil.Alert(MsgQRUrlFailConvert, e.Message); return Array.Empty<byte>(); }
        }

        private static byte[] DecodeQRJson(string data)
        {
            const string cap = "\",\"error\":null}]}]";
            const string intro = "[{\"type\":\"qrcode\",\"symbol\":[{\"seq\":0,\"data\":\"";
            const string qrcode = "nQR-Code:";
            if (!data.StartsWith(intro))
                throw new FormatException();

            string pkstr = data.Substring(intro.Length);
            if (pkstr.Contains(qrcode)) // Remove multiple QR codes in same image
                pkstr = pkstr.Substring(0, pkstr.IndexOf(qrcode, StringComparison.Ordinal));
            pkstr = pkstr.Substring(0, pkstr.IndexOf(cap, StringComparison.Ordinal)); // Trim outro

            if (!pkstr.StartsWith("http") && !pkstr.StartsWith(QR6PathBad)) // G7
            {
                string fstr = Regex.Unescape(pkstr);
                byte[] raw = Encoding.Unicode.GetBytes(fstr);
                // Remove 00 interstitials and retrieve from offset 0x30, take PK7 Stored Size (always)
                return raw.Where((_, i) => i % 2 == 0).Skip(0x30).Take(0xE8).ToArray();
            }
            // All except G7
            pkstr = pkstr.Substring(pkstr.IndexOf('#') + 1); // Trim URL
            pkstr = pkstr.Replace("\\", string.Empty); // Rectify response

            return Convert.FromBase64String(pkstr);
        }

        internal static Image GetQRImage(byte[] data, string server)
        {
            string qrdata = Convert.ToBase64String(data);
            string message = server + qrdata;
            string webURL = EncodeAPI + HttpUtility.UrlEncode(message);

            try
            {
                return NetUtil.GetImageFromURL(webURL);
            }
            catch
            {
                if (DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, MsgQRUrlFailConnection, MsgQRClipboardUrl))
                    return null;
                try { Clipboard.SetText(webURL); }
                catch { WinFormsUtil.Alert(MsgClipboardFailWrite); }
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
            return GenerateQRCode(data, ppm: 4);
        }

        private static Image GenerateQRCode(byte[] data, int ppm = 4)
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
                case 6: return QR6Path;
                default: return QR6PathBad;
            }
        }
    }
}
