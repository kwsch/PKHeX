using System;
using System.Drawing;

namespace PKHeX.Drawing
{
    public static class QRImageUtil
    {
        public static Bitmap GetQRImage(Image qr, Image preview)
        {
            // create a small area with the pkm sprite, with a white background
            var foreground = new Bitmap(preview.Width + 4, preview.Height + 4);
            using (Graphics gfx = Graphics.FromImage(foreground))
            {
                gfx.FillRectangle(new SolidBrush(Color.White), 0, 0, foreground.Width, foreground.Height);
                int x = (foreground.Width / 2) - (preview.Width / 2);
                int y = (foreground.Height / 2) - (preview.Height / 2);
                gfx.DrawImage(preview, x, y);
            }

            // Layer on Preview Image
            {
                int x = (qr.Width / 2) - (foreground.Width / 2);
                int y = (qr.Height / 2) - (foreground.Height / 2);
                return ImageUtil.LayerImage(qr, foreground, x, y);
            }
        }

        public static Bitmap GetQRImageExtended(Font font, Image qr, Image pkm, int width, int height, string[] lines, string extraText)
        {
            var pic = GetQRImage(qr, pkm);
            return ExtendImage(font, qr, width, height, pic, lines, extraText);
        }

        private static Bitmap ExtendImage(Font font, Image qr, int width, int height, Image pic, string[] lines, string extraText)
        {
            var newpic = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(newpic))
            {
                g.FillRectangle(new SolidBrush(Color.White), 0, 0, newpic.Width, newpic.Height);
                g.DrawImage(pic, 0, 0);

                g.DrawString(GetLine(lines, 0), font, Brushes.Black, new PointF(18, qr.Height - 5));
                g.DrawString(GetLine(lines, 1), font, Brushes.Black, new PointF(18, qr.Height + 8));
                g.DrawString(GetLine(lines, 2).Replace(Environment.NewLine, "/").Replace("//", "   ").Replace(":/", ": "), font,
                    Brushes.Black, new PointF(18, qr.Height + 20));
                g.DrawString(GetLine(lines, 3) + extraText, font, Brushes.Black, new PointF(18, qr.Height + 32));
            }
            return newpic;
        }

        private static string GetLine(string[] lines, int line) => lines.Length <= line ? string.Empty : lines[line];
    }
}
