using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public static class CGearExtensions
    {
        public static Bitmap GetBitmap(CGearBackground bg)
        {
            const int Width = CGearBackground.Width;
            const int Height = CGearBackground.Height;
            Bitmap img = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);

            // Fill Data
            using (Graphics g = Graphics.FromImage(img))
            for (int i = 0; i < bg.Map.TileChoices.Length; i++)
            {
                int x = (i * 8) % Width;
                int y = 8 * ((i * 8) / Width);
                var tile = bg.Tiles[bg.Map.TileChoices[i] % bg.Tiles.Length];
                var tileData = tile.Rotate(bg.Map.Rotations[i]);
                Bitmap b = ImageUtil.GetBitmap(tileData, 8, 8);
                g.DrawImage(b, new Point(x, y));
            }
            return img;
        }
        public static CGearBackground GetCGearBackground(Bitmap img)
        {
            const int Width = CGearBackground.Width;
            const int Height = CGearBackground.Height;
            if (img.Width != Width)
                throw new ArgumentException($"Invalid image width. Expected {Width} pixels wide.");
            if (img.Height != Height)
                throw new ArgumentException($"Invalid image height. Expected {Height} pixels high.");

            // get raw bytes of image
            byte[] data = ImageUtil.GetPixelData(img);
            const int bpp = 4;
            Debug.Assert(data.Length == Width * Height * bpp);

            return CGearBackground.GetBackground(data, bpp);
        }
    }
}