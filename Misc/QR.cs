using System;
using System.Drawing;
using System.Windows.Forms;

namespace PKHeX
{
    public partial class QR : Form
    {
        public QR(Image qr, Image pkm, string top, string bottom, string left, string right)
        {
            InitializeComponent();
            Font font = (Form1.unicode) ? FontLabel.Font : PKX.getPKXFont((float)8.25);
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
            Graphics g = Graphics.FromImage(newpic);
            g.FillRectangle(new SolidBrush(Color.White), 0, 0, newpic.Width, newpic.Height);
            g.DrawImage(pic, 0, 0);

            g.DrawString(top, font, Brushes.Black, new PointF(18, qr.Height - 5));
            g.DrawString(bottom, font, Brushes.Black, new PointF(18, qr.Height + 8));
            g.DrawString(left.Replace(Environment.NewLine, "/").Replace("//", "   ").Replace(":/", ": "), font, Brushes.Black, new PointF(18, qr.Height + 20));
            g.DrawString(right, font, Brushes.Black, new PointF(18, qr.Height + 32));

            PB_QR.BackgroundImage = newpic;
        }
        private void PB_QR_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes != Util.Prompt(MessageBoxButtons.YesNo, "Copy QR Image to Clipboard?")) return;
            try { Clipboard.SetImage(PB_QR.BackgroundImage); }
            catch { Util.Alert("Failed to set Image to Clipboard"); }
        }
    }
}