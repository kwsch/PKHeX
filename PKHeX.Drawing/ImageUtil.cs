using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace PKHeX.Drawing
{
    /// <summary>
    /// Image Layering/Blending Utility
    /// </summary>
    public static class ImageUtil
    {
        public static Bitmap LayerImage(Image baseLayer, Image overLayer, int x, int y, double transparency)
        {
            overLayer = ChangeOpacity(overLayer, transparency);
            return LayerImage(baseLayer, overLayer, x, y);
        }

        public static Bitmap LayerImage(Image baseLayer, Image overLayer, int x, int y)
        {
            Bitmap img = new(baseLayer);
            using Graphics gr = Graphics.FromImage(img);
            gr.DrawImage(overLayer, x, y, overLayer.Width, overLayer.Height);
            return img;
        }

        public static Bitmap ChangeOpacity(Image img, double trans)
        {
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

        public static Bitmap GetBitmap(byte[] data, int width, int height, PixelFormat format = PixelFormat.Format32bppArgb)
        {
            var bmp = new Bitmap(width, height, format);
            var bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, format);
            var ptr = bmpData.Scan0;
            Marshal.Copy(data, 0, ptr, data.Length);
            bmp.UnlockBits(bmpData);
            return bmp;
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

        public static void GlowEdges(byte[] data, byte blue, byte green, byte red, int width, int reach = 3, double amount = 0.0777)
        {
            PollutePixels(data, width, reach, amount);
            CleanPollutedPixels(data, blue, green, red);
        }

        private static void PollutePixels(byte[] data, int width, int reach, double amount)
        {
            int stride = width * 4;
            int height = data.Length / stride;
            for (int i = 0; i < data.Length; i += 4)
            {
                // only pollute outwards if the current pixel isn't transparent
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
                        // update one of the color bits
                        // it is expected that a transparent pixel RGBA value is 0.
                        var c = 4 * (i + (j * width));
                        data[c + 0] += (byte)(amount * (0xFF - data[c + 0]));
                    }
                }
            }
        }

        private static void CleanPollutedPixels(byte[] data, byte blue, byte green, byte red)
        {
            for (int i = 0; i < data.Length; i += 4)
            {
                // only clean if the current pixel isn't transparent
                if (data[i + 3] != 0)
                    continue;

                // grab the transparency from the donor byte
                var transparency = data[i + 0];
                if (transparency == 0)
                    continue;

                data[i + 0] = blue;
                data[i + 1] = green;
                data[i + 2] = red;
                data[i + 3] = transparency;
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

        public static Color ColorBaseStatTotal(int tot) => ColorBaseStat((int) (Math.Max(0, tot - 175) / 3f));

        public static Color Blend(Color color, Color backColor, double amount)
        {
            byte r = (byte)((color.R * amount) + (backColor.R * (1 - amount)));
            byte g = (byte)((color.G * amount) + (backColor.G * (1 - amount)));
            byte b = (byte)((color.B * amount) + (backColor.B * (1 - amount)));
            return Color.FromArgb(r, g, b);
        }
    }
}
