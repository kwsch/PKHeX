using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace PKHeX
{
    public static partial class Util
    {
        // Image Layering/Blending Utility
        internal static Bitmap LayerImage(Image baseLayer, Image overLayer, int x, int y, double trans)
        {
            Bitmap img = new Bitmap(baseLayer.Width, baseLayer.Height);
            using (Graphics gr = Graphics.FromImage(img))
            {
                gr.DrawImage(baseLayer, new Point(0, 0));
                Bitmap o = ChangeOpacity(overLayer, trans);
                gr.DrawImage(o, new Rectangle(x, y, overLayer.Width, overLayer.Height));
            }
            return img;
        }
        internal static Bitmap ChangeOpacity(Image img, double trans)
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
    }
}
