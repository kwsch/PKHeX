using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace PKHeX.WinForms
{
    public static class ImageUtil
    {
        // Image Layering/Blending Utility
        public static Bitmap LayerImage(Image baseLayer, Image overLayer, int x, int y, double trans)
        {
            if (baseLayer == null)
                return overLayer as Bitmap;
            Bitmap img = new Bitmap(baseLayer.Width, baseLayer.Height);
            using (Graphics gr = Graphics.FromImage(img))
            {
                gr.DrawImage(baseLayer, new Point(0, 0));
                Bitmap o = ChangeOpacity(overLayer, trans);
                gr.DrawImage(o, new Rectangle(x, y, overLayer.Width, overLayer.Height));
            }
            return img;
        }
        public static Bitmap ChangeOpacity(Image img, double trans)
        {
            if (img == null)
                return null;
            if (img.PixelFormat.HasFlag(PixelFormat.Indexed))
                return (Bitmap)img;

            Bitmap bmp = (Bitmap)img.Clone();
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            IntPtr ptr = bmpData.Scan0;

            int len = bmp.Width * bmp.Height * 4;
            byte[] data = new byte[len];

            Marshal.Copy(ptr, data, 0, len);

            for (int i = 0; i < data.Length; i += 4)
                data[i + 3] = (byte)(data[i + 3] * trans);

            Marshal.Copy(data, 0, ptr, len);
            bmp.UnlockBits(bmpData);

            return bmp;
        }
        public static Bitmap ChangeAllColorTo(Image img, Color c)
        {
            if (img == null)
                return null;
            if (img.PixelFormat.HasFlag(PixelFormat.Indexed))
                return (Bitmap)img;

            Bitmap bmp = (Bitmap)img.Clone();
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            IntPtr ptr = bmpData.Scan0;

            int len = bmp.Width * bmp.Height * 4;
            byte[] data = new byte[len];

            Marshal.Copy(ptr, data, 0, len);

            byte R = c.R;
            byte G = c.G;
            byte B = c.B;
            for (int i = 0; i < data.Length; i += 4)
                if (data[i + 3] != 0)
                {
                    data[i + 0] = B;
                    data[i + 1] = G;
                    data[i + 2] = R;
                }

            Marshal.Copy(data, 0, ptr, len);
            bmp.UnlockBits(bmpData);

            return bmp;
        }
        public static Bitmap ToGrayscale(Image img)
        {
            if (img == null)
                return null;
            if (img.PixelFormat.HasFlag(PixelFormat.Indexed))
                return (Bitmap)img;

            Bitmap bmp = (Bitmap)img.Clone();
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            IntPtr ptr = bmpData.Scan0;

            int len = bmp.Width * bmp.Height * 4;
            byte[] data = new byte[len];

            Marshal.Copy(ptr, data, 0, len);

            for (int i = 0; i < data.Length; i += 4)
                if (data[i + 3] != 0)
                {
                    byte greyS = (byte)((0.3 * data[i + 2] + 0.59 * data[i + 1] + 0.11 * data[i + 0]) / 3);
                    data[i + 0] = greyS;
                    data[i + 1] = greyS;
                    data[i + 2] = greyS;
                }

            Marshal.Copy(data, 0, ptr, len);
            bmp.UnlockBits(bmpData);

            return bmp;
        }
    }
}
