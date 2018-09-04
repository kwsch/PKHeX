using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace PKHeX.WinForms
{
    /// <summary>
    /// Image Layering/Blending Utility
    /// </summary>
    public static class ImageUtil
    {
        public static Bitmap LayerImage(Image baseLayer, Image overLayer, int x, int y, double transparency)
        {
            if (baseLayer == null)
                return overLayer as Bitmap;
            overLayer = ChangeOpacity(overLayer, transparency);
            return LayerImage(baseLayer, overLayer, x, y);
        }

        public static Bitmap LayerImage(Image baseLayer, Image overLayer, int x, int y)
        {
            if (baseLayer == null)
                return overLayer as Bitmap;
            Bitmap img = new Bitmap(baseLayer);
            using (Graphics gr = Graphics.FromImage(img))
                gr.DrawImage(overLayer, x, y, overLayer.Width, overLayer.Height);
            return img;
        }

        public static Bitmap ChangeOpacity(Image img, double trans)
        {
            if (img == null)
                return null;
            if (img.PixelFormat.HasFlag(PixelFormat.Indexed))
                return (Bitmap)img;

            var bmp = (Bitmap)img.Clone();
            GetBitmapData(bmp, out BitmapData bmpData, out IntPtr ptr, out byte[] data);

            Marshal.Copy(ptr, data, 0, data.Length);
            SetAllTransparencyTo(data, trans);
            Marshal.Copy(data, 0, ptr, data.Length);
            bmp.UnlockBits(bmpData);

            return bmp;
        }

        public static Bitmap ChangeAllColorTo(Image img, Color c)
        {
            if (img == null)
                return null;
            if (img.PixelFormat.HasFlag(PixelFormat.Indexed))
                return (Bitmap)img;

            var bmp = (Bitmap)img.Clone();
            GetBitmapData(bmp, out BitmapData bmpData, out IntPtr ptr, out byte[] data);

            Marshal.Copy(ptr, data, 0, data.Length);
            ChangeAllColorTo(data, c);
            Marshal.Copy(data, 0, ptr, data.Length);
            bmp.UnlockBits(bmpData);

            return bmp;
        }

        public static Bitmap ToGrayscale(Image img)
        {
            if (img == null)
                return null;
            if (img.PixelFormat.HasFlag(PixelFormat.Indexed))
                return (Bitmap)img;

            var bmp = (Bitmap)img.Clone();
            GetBitmapData(bmp, out BitmapData bmpData, out IntPtr ptr, out byte[] data);

            Marshal.Copy(ptr, data, 0, data.Length);
            SetAllColorToGrayScale(data);
            Marshal.Copy(data, 0, ptr, data.Length);
            bmp.UnlockBits(bmpData);

            return bmp;
        }

        private static void GetBitmapData(Bitmap bmp, out BitmapData bmpData, out IntPtr ptr, out byte[] data)
        {
            bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            ptr = bmpData.Scan0;
            data = new byte[bmp.Width * bmp.Height * 4];
        }

        public static Bitmap GetBitmap(byte[] data, int width, int height, int stride = -1, PixelFormat format = PixelFormat.Format32bppArgb)
        {
            if (stride == -1 && format == PixelFormat.Format32bppArgb)
                stride = 4 * width; // defaults
            return new Bitmap(width, height, stride, format, Marshal.UnsafeAddrOfPinnedArrayElement(data, 0));
        }

        public static byte[] GetPixelData(Bitmap bitmap)
        {
            var argbData = new byte[bitmap.Width * bitmap.Height * 4];
            var bd = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
            Marshal.Copy(bd.Scan0, argbData, 0, bitmap.Width * bitmap.Height * 4);
            bitmap.UnlockBits(bd);
            return argbData;
        }

        public static void SetAllUsedPixelsOpaque(byte[] data)
        {
            for (int i = 0; i < data.Length; i += 4)
            {
                if (data[i + 3] != 0)
                    data[i + 3] = 0xFF;
            }
        }

        public static void RemovePixels(byte[] pixels, byte[] original)
        {
            for (int i = 0; i < original.Length; i += 4)
            {
                if (original[i + 3] == 0)
                    continue;
                pixels[i + 0] = 0;
                pixels[i + 1] = 0;
                pixels[i + 2] = 0;
                pixels[i + 3] = 0;
            }
        }

        private static void SetAllTransparencyTo(byte[] data, double trans)
        {
            for (int i = 0; i < data.Length; i += 4)
                data[i + 3] = (byte)(data[i + 3] * trans);
        }

        public static void ChangeAllColorTo(byte[] data, Color c)
        {
            byte R = c.R;
            byte G = c.G;
            byte B = c.B;
            for (int i = 0; i < data.Length; i += 4)
            {
                if (data[i + 3] == 0)
                    continue;
                data[i + 0] = B;
                data[i + 1] = G;
                data[i + 2] = R;
            }
        }

        private static void SetAllColorToGrayScale(byte[] data)
        {
            for (int i = 0; i < data.Length; i += 4)
            {
                if (data[i + 3] == 0)
                    continue;
                byte greyS = (byte)(((0.3 * data[i + 2]) + (0.59 * data[i + 1]) + (0.11 * data[i + 0])) / 3);
                data[i + 0] = greyS;
                data[i + 1] = greyS;
                data[i + 2] = greyS;
            }
        }

        public static void GlowEdges(byte[] data, byte[] colors, int width, int reach = 3, double amount = 0.0777)
        {
            // dual pass (pollute, de-transparent)
            int stride = width * 4;
            int height = data.Length / stride;
            for (int i = 0; i < data.Length; i += 4)
            {
                if (data[i + 3] == 0)
                    continue;

                int x = (i % stride) / 4;
                int y = (i / stride);
                Pollute(x, y);
            }

            void Pollute(int x, int y)
            {
                int left = Math.Max(0, x - reach);
                int right = Math.Min(width - 1, x + reach);
                int top = Math.Max(0, y - reach);
                int bottom = Math.Min(height - 1, y + reach);
                for (int i = left; i <= right; i++)
                {
                    for (int j = top; j <= bottom; j++)
                    {
                        var c = 4 * (i + (j * width));
                        data[c + 0] += (byte)(amount * (0xFF - data[c + 0]));
                    }
                }
            }
            for (int i = 0; i < data.Length; i += 4)
            {
                if (data[i + 3] != 0)
                    continue;
                var flair = data[i + 0];
                if (flair == 0)
                    continue;

                data[i + 3] = flair;
                data[i + 0] = colors[0];
                data[i + 1] = colors[1];
                data[i + 2] = colors[2];
            }
        }

        public static Color ColorBaseStat(int v)
        {
            const float maxval = 180; // shift the green cap down
            float x = 100f * v / maxval;
            if (x > 100)
                x = 100;
            double red = 255f * (x > 50 ? 1 - (2 * (x - 50) / 100.0) : 1.0);
            double green = 255f * (x > 50 ? 1.0 : 2 * x / 100.0);

            return Blend(Color.FromArgb((int)red, (int)green, 0), Color.White, 0.4);
        }

        public static Color Blend(Color color, Color backColor, double amount)
        {
            byte r = (byte)((color.R * amount) + (backColor.R * (1 - amount)));
            byte g = (byte)((color.G * amount) + (backColor.G * (1 - amount)));
            byte b = (byte)((color.B * amount) + (backColor.B * (1 - amount)));
            return Color.FromArgb(r, g, b);
        }
    }
}
