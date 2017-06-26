using System.Drawing;
using PKHeX.Core;
using PKHeX.WinForms.Properties;

namespace PKHeX.WinForms
{
    public static class PKMUtil
    {
        public static Image GetBallSprite(int ball)
        {
            string str = PKX.GetResourceStringBall(ball);
            return (Image)Resources.ResourceManager.GetObject(str) ?? Resources._ball4; // Poké Ball (default)
        }
        public static Image GetSprite(int species, int form, int gender, int item, bool isegg, bool shiny, int generation = -1, bool isBoxBGRed = false)
        {
            if (species == 0)
                return Resources._0;

            string file = PKX.GetResourceStringSprite(species, form, gender, generation);

            // Redrawing logic
            Image baseImage = (Image)Resources.ResourceManager.GetObject(file);
            if (baseImage == null)
            {
                baseImage = (Image) Resources.ResourceManager.GetObject("_" + species);
                baseImage = baseImage != null ? ImageUtil.LayerImage(baseImage, Resources.unknown, 0, 0, .5) : Resources.unknown;
            }
            if (isegg)
            {
                // Partially transparent species.
                baseImage = ImageUtil.ChangeOpacity(baseImage, 0.33);
                // Add the egg layer over-top with full opacity.
                var egg = species == 490 ? (Image) Resources.ResourceManager.GetObject("_490_e") : Resources.egg;
                baseImage = ImageUtil.LayerImage(baseImage, egg, 0, 0, 1);
            }
            if (shiny)
            {
                // Add shiny star to top left of image.
                var rare = isBoxBGRed ? Resources.rare_icon_alt : Resources.rare_icon;
                baseImage = ImageUtil.LayerImage(baseImage, rare, 0, 0, 0.7);
            }
            if (item > 0)
            {
                Image itemimg = (Image)Resources.ResourceManager.GetObject("item_" + item) ?? Resources.helditem;
                if (generation >= 2 && generation <= 4 && 328 <= item && item <= 419) // gen2/3/4 TM
                    itemimg = Resources.item_tm;

                // Redraw
                int x = 22 + (15 - itemimg.Width)/2;
                int y = 15 + (15 - itemimg.Height);
                baseImage = ImageUtil.LayerImage(baseImage, itemimg, x, y, 1);
            }
            return baseImage;
        }
        public static Image GetRibbonSprite(string name)
        {
            return Resources.ResourceManager.GetObject(name.Replace("CountG3", "G3").ToLower()) as Image;
        }
        public static Image GetTypeSprite(int type)
        {
            return Resources.ResourceManager.GetObject("type_icon_" + type.ToString("00")) as Image;
        }

        private static Image GetSprite(MysteryGift gift, SaveFile SAV)
        {
            if (gift.Empty)
                return null;

            Image img;
            if (gift.IsEgg && gift.Species == 490) // Manaphy Egg
                img = (Image)(Resources.ResourceManager.GetObject("_490_e") ?? Resources.unknown);
            else if (gift.IsPokémon)
                img = GetSprite(gift.ConvertToPKM(SAV));
            else if (gift.IsItem)
                img = (Image)(Resources.ResourceManager.GetObject("item_" + gift.ItemID) ?? Resources.unknown);
            else
                img = Resources.unknown;

            if (gift.GiftUsed)
                img = ImageUtil.LayerImage(new Bitmap(img.Width, img.Height), img, 0, 0, 0.3);
            return img;
        }
        private static Image GetSprite(PKM pkm, bool isBoxBGRed = false)
        {
            return GetSprite(pkm.Species, pkm.AltForm, pkm.Gender, pkm.SpriteItem, pkm.IsEgg, pkm.IsShiny, pkm.Format, isBoxBGRed);
        }
        private static Image GetSprite(SaveFile SAV)
        {
            string file = "tr_00";
            if (SAV.Generation == 6 && (SAV.ORAS || SAV.ORASDEMO))
                file = "tr_" + SAV.MultiplayerSpriteID.ToString("00");
            return Resources.ResourceManager.GetObject(file) as Image;
        }
        private static Image GetWallpaper(SaveFile SAV, int box)
        {
            string s = BoxWallpaper.GetWallpaper(SAV, box);
            return (Bitmap)(Resources.ResourceManager.GetObject(s) ?? Resources.box_wp16xy);
        }
        private static Image GetSprite(PKM pkm, SaveFile SAV, int box, int slot, bool flagIllegal = false)
        {
            if (!pkm.Valid)
                return null;

            bool inBox = slot >= 0 && slot < 30;
            var sprite = pkm.Species != 0 ? pkm.Sprite(isBoxBGRed: inBox && BoxWallpaper.IsWallpaperRed(SAV, box)) : null;

            if (slot <= -1) // from tabs
                return sprite;

            if (flagIllegal)
            {
                if (slot < 30)
                    pkm.Box = box;
                var la = new LegalityAnalysis(pkm);
                if (la.Parsed && !la.Valid && pkm.Species != 0)
                    sprite = ImageUtil.LayerImage(sprite, Resources.warn, 0, 14, 1);
            }
            if (inBox) // in box
            {
                if (SAV.IsSlotLocked(box, slot))
                    sprite = ImageUtil.LayerImage(sprite, Resources.locked, 26, 0, 1);
                else if (SAV.IsSlotInBattleTeam(box, slot))
                    sprite = ImageUtil.LayerImage(sprite, Resources.team, 21, 0, 1);
            }

            return sprite;
        }

        // Extension Methods
        public static Image WallpaperImage(this SaveFile SAV, int box) => GetWallpaper(SAV, box);
        public static Image Sprite(this MysteryGift gift, SaveFile SAV) => GetSprite(gift, SAV);
        public static Image Sprite(this SaveFile SAV) => GetSprite(SAV);
        public static Image Sprite(this PKM pkm, bool isBoxBGRed = false) => GetSprite(pkm, isBoxBGRed);
        public static Image Sprite(this PKM pkm, SaveFile SAV, int box, int slot, bool flagIllegal = false)
            => GetSprite(pkm, SAV, box, slot, flagIllegal);
    }
}
