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
        public static Image getSprite(int species, int form, int gender, int item, bool isegg, bool shiny, int generation = -1)
        {
            if (species == 0)
                return Resources._0;

            string file = PKX.getSpriteString(species, form, gender, generation);

            // Redrawing logic
            Image baseImage = (Image)Resources.ResourceManager.GetObject(file);
            if (baseImage == null)
            {
                baseImage = (Image) Resources.ResourceManager.GetObject("_" + species);
                baseImage = baseImage != null ? ImageUtil.LayerImage(baseImage, Resources.unknown, 0, 0, .5) : Resources.unknown;
            }
            if (isegg)
            {
                // Start with a partially transparent species by layering the species with partial opacity onto a blank image.
                baseImage = ImageUtil.LayerImage(Resources._0, baseImage, 0, 0, 0.33);
                // Add the egg layer over-top with full opacity.
                baseImage = ImageUtil.LayerImage(baseImage, Resources.egg, 0, 0, 1);
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
        public static Image getRibbonSprite(string name)
        {
            return Resources.ResourceManager.GetObject(name.Replace("CountG3", "G3").ToLower()) as Image;
        }
        public static Image getTypeSprite(int type)
        {
            return Resources.ResourceManager.GetObject("type_icon_" + type.ToString("00")) as Image;
        }

        private static Image getSprite(MysteryGift gift)
        {
            if (gift.Empty)
                return null;

            Image img;
            if (gift.IsPokémon)
                img = getSprite(gift.convertToPKM(Main.SAV));
            else if (gift.IsItem)
                img = (Image)(Resources.ResourceManager.GetObject("item_" + gift.Item) ?? Resources.unknown);
            else
                img = Resources.unknown;

            if (gift.GiftUsed)
                img = ImageUtil.LayerImage(new Bitmap(img.Width, img.Height), img, 0, 0, 0.3);
            return img;
        }
        private static Image getSprite(PKM pkm)
        {
            return getSprite(pkm.Species, pkm.AltForm, pkm.Gender, pkm.SpriteItem, pkm.IsEgg, pkm.IsShiny, pkm.Format);
        }
        private static Image getSprite(SaveFile SAV)
        {
            string file = "tr_00";
            if (SAV.Generation == 6 && (SAV.ORAS || SAV.ORASDEMO))
                file = "tr_" + SAV.MultiplayerSpriteID.ToString("00");
            return Resources.ResourceManager.GetObject(file) as Image;
        }
        private static Image getWallpaper(SaveFile SAV, int box)
        {
            string s = BoxWallpaper.getWallpaper(SAV, box);
            return (Bitmap)(Resources.ResourceManager.GetObject(s) ?? Resources.box_wp16xy);
        }

        // Extension Methods
        public static Image Sprite(this MysteryGift gift) => getSprite(gift);
        public static Image Sprite(this PKM pkm) => getSprite(pkm);
        public static Image Sprite(this SaveFile SAV) => getSprite(SAV);
        public static Image WallpaperImage(this SaveFile SAV, int box) => getWallpaper(SAV, box);
    }
}
