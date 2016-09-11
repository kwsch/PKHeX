using System;
using System.Drawing;
using System.Web;
using System.Windows.Forms;

namespace PKHeX
{
    public partial class QR : Form
    {
        public QR(Image qr, Image pkm, string top, string bottom, string left, string right)
        {
            InitializeComponent();
            Font font = Main.unicode ? FontLabel.Font : PKX.getPKXFont((float)8.25);
            Image preview = new Bitmap(45, 45);
            using (Graphics gfx = Graphics.FromImage(preview))
            {
                gfx.FillRectangle(new SolidBrush(Color.White), 0, 0, preview.Width, preview.Height);
                gfx.DrawImage(pkm, preview.Width / 2 - pkm.Width / 2, preview.Height / 2 - pkm.Height / 2);
            }
            // Layer on Preview Image
            Image pic = Util.LayerImage(qr, preview, qr.Width / 2 - preview.Width / 2, qr.Height / 2 - preview.Height / 2, 1);
            
            // Layer on Text
            const int stretch = 50;
            Height += stretch;
            Image newpic = new Bitmap(PB_QR.Width, PB_QR.Height);
            using (Graphics g = Graphics.FromImage(newpic))
            {
                g.FillRectangle(new SolidBrush(Color.White), 0, 0, newpic.Width, newpic.Height);
                g.DrawImage(pic, 0, 0);

                g.DrawString(top, font, Brushes.Black, new PointF(18, qr.Height - 5));
                g.DrawString(bottom, font, Brushes.Black, new PointF(18, qr.Height + 8));
                g.DrawString(left.Replace(Environment.NewLine, "/").Replace("//", "   ").Replace(":/", ": "), font, Brushes.Black, new PointF(18, qr.Height + 20));
                g.DrawString(right, font, Brushes.Black, new PointF(18, qr.Height + 32));
            }
            PB_QR.BackgroundImage = newpic;
        }
        private void PB_QR_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes != Util.Prompt(MessageBoxButtons.YesNo, "Copy QR Image to Clipboard?")) return;
            try { Clipboard.SetImage(PB_QR.BackgroundImage); }
            catch { Util.Alert("Failed to set Image to Clipboard"); }
        }
        
        // QR Utility
        internal static byte[] getQRData()
        {
            // Fetch data from QR code...
            string address;
            try { address = Clipboard.GetText(); }
            catch { Util.Alert("No text (url) in clipboard."); return null; }
            try { if (address.Length < 4 || address.Substring(0, 3) != "htt") { Util.Alert("Clipboard text is not a valid URL:", address); return null; } }
            catch { Util.Alert("Clipboard text is not a valid URL:", address); return null; }
            string webURL = "http://api.qrserver.com/v1/read-qr-code/?fileurl=" + HttpUtility.UrlEncode(address);
            try
            {
                string data = Util.getStringFromURL(webURL);
                if (data.Contains("could not find")) { Util.Alert("Reader could not find QR data in the image."); return null; }
                if (data.Contains("filetype not supported")) { Util.Alert("Input URL is not valid. Double check that it is an image (jpg/png).", address); return null; }
                // Quickly convert the json response to a data string
                string pkstr = data.Substring(data.IndexOf("#", StringComparison.Ordinal) + 1); // Trim intro
                pkstr = pkstr.Substring(0, pkstr.IndexOf("\",\"error\":null}]}]", StringComparison.Ordinal)); // Trim outro
                if (pkstr.Contains("nQR-Code:")) pkstr = pkstr.Substring(0, pkstr.IndexOf("nQR-Code:", StringComparison.Ordinal)); //  Remove multiple QR codes in same image
                pkstr = pkstr.Replace("\\", ""); // Rectify response

                try { return Convert.FromBase64String(pkstr); }
                catch { Util.Alert("QR string to Data failed.", pkstr); return null; }
            }
            catch { Util.Alert("Unable to connect to the internet to decode QR code."); return null; }
        }
        internal static Image getQRImage(byte[] data, string server)
        {
            string qrdata = Convert.ToBase64String(data);
            string message = server + qrdata;
            string webURL = "http://chart.apis.google.com/chart?chs=365x365&cht=qr&chl=" + HttpUtility.UrlEncode(message);

            try
            {
                return Util.getImageFromURL(webURL);
            }
            catch
            {
                if (DialogResult.Yes != Util.Prompt(MessageBoxButtons.YesNo, "Unable to connect to the internet to receive QR code.", "Copy QR URL to Clipboard?"))
                    return null;
                try { Clipboard.SetText(webURL); }
                catch { Util.Alert("Failed to set text to Clipboard"); }
            }
            return null;
        }
    }
}