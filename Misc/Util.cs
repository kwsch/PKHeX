using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace PKHeX
{
    public partial class Util
    {
        public static Image layerImage(Image baseLayer, Image overLayer, int x, int y, double trans)
        {
            Bitmap overlayImage = (Bitmap)overLayer;
            Bitmap newImage = (Bitmap)baseLayer;
            for (int i = 0; i < (overlayImage.Width * overlayImage.Height); i++)
            {
                Color newColor = overlayImage.GetPixel(i % (overlayImage.Width), i / (overlayImage.Width));
                newColor = Color.FromArgb((int)((double)(newColor.A) * trans), newColor.R, newColor.G, newColor.B); // Apply transparency change
                if (newColor.A != 0) // If Pixel isn't transparent, we'll overwrite the color.
                {
                    newImage.SetPixel(
                        i % (overlayImage.Width) + x,
                        i / (overlayImage.Width) + y,
                        newColor);
                }
            }
            return newImage;
        }
        public static string GetTempFolder() // From 3DSSE's decompiled source.
        {
            string tempPath = Path.GetTempPath();
            string str2 = "SE3DS";
            str2 = "3DSSE";
            tempPath = Path.Combine(tempPath, str2);
            // Directory.CreateDirectory(tempPath);
            return (tempPath + "/");
        }
    }
}
