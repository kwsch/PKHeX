using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

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
                if (newColor != Color.FromArgb(0, 0, 0, 0))
                {
                    newImage.SetPixel(
                        i % (overlayImage.Width) + x,
                        i / (overlayImage.Width) + y,
                        newColor);
                }
            }
            return newImage;
        }
    }
}
