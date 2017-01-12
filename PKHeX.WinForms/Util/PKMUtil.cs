using System.Drawing;
using PKHeX.Core;
using PKHeX.Core.Properties;

namespace PKHeX.WinForms
{
    public static class PKMUtil
    {
        public static Image getBallSprite(int ball)
        {
            string str = PKX.getBallString(ball);
            return (Image)Resources.ResourceManager.GetObject(str) ?? Resources._ball4; // Poké Ball (default)
        }
        public static Image Sprite(this PKM pkm)
        {
            return getSprite(pkm);
        }
        public static Image getSprite(int species, int form, int gender, int item, bool isegg, bool shiny, int generation = -1)
        {
            if (species == 0)
                return (Image)Resources.ResourceManager.GetObject("_0");

            string file = PKX.getSpriteString(species, form, gender, generation);

            // Redrawing logic
            Image baseImage = (Image)Resources.ResourceManager.GetObject(file);
            if (baseImage == null)
            {
                if (species < 803)
                {
                    baseImage = ImageUtil.LayerImage(
                        (Image)Resources.ResourceManager.GetObject("_" + species),
                        Resources.unknown,
                        0, 0, .5);
                }
                else
                    baseImage = Resources.unknown;
            }
            if (isegg)
            {
                // Start with a partially transparent species by layering the species with partial opacity onto a blank image.
                baseImage = ImageUtil.LayerImage((Image)Resources.ResourceManager.GetObject("_0"), baseImage, 0, 0, 0.33);
                // Add the egg layer over-top with full opacity.
                baseImage = ImageUtil.LayerImage(baseImage, (Image)Resources.ResourceManager.GetObject("egg"), 0, 0, 1);
            }
            if (shiny)
            {
                // Add shiny star to top left of image.
                baseImage = ImageUtil.LayerImage(baseImage, Resources.rare_icon, 0, 0, 0.7);
            }
            if (item > 0)
            {
                Image itemimg = (Image)Resources.ResourceManager.GetObject("item_" + item) ?? Resources.helditem;
                if (generation >= 2 && generation <= 4 && 328 <= item && item <= 419) // gen2/3/4 TM
                    itemimg = Resources.item_tm;

                // Redraw
                baseImage = ImageUtil.LayerImage(baseImage, itemimg, 22 + (15 - itemimg.Width) / 2, 15 + (15 - itemimg.Height), 1);
            }
            return baseImage;
        }
        public static Image getSprite(PKM pkm)
        {
            return getSprite(pkm.Species, pkm.AltForm, pkm.Gender, pkm.SpriteItem, pkm.IsEgg, pkm.IsShiny, pkm.Format);
        }

        public static Image getWallpaper(this SaveFile SAV, int box)
        {
            string s = BoxWallpaper.getWallpaper(SAV, box);
            return (Bitmap)(Resources.ResourceManager.GetObject(s) ?? Resources.box_wp16xy);
        }
    }
}
