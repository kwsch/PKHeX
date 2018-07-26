using System.Drawing;
using PKHeX.Core;
using PKHeX.WinForms.Properties;

namespace PKHeX.WinForms
{
    public static class SpriteUtil
    {
        public static ISpriteBuilder<Image> Spriter { get; set; } = new SpriteBuilder();

        public static Image GetBallSprite(int ball)
        {
            string str = PKX.GetResourceStringBall(ball);
            return (Image)Resources.ResourceManager.GetObject(str) ?? Resources._ball4; // Poké Ball (default)
        }
        public static Image GetSprite(int species, int form, int gender, int item, bool isegg, bool shiny, int generation = -1, bool isBoxBGRed = false)
        {
            return Spriter.GetSprite(species, form, gender, item, isegg, shiny, generation, isBoxBGRed);
        }
        public static Image GetRibbonSprite(string name)
        {
            return Resources.ResourceManager.GetObject(name.Replace("CountG3", "G3").ToLower()) as Image;
        }
        public static Image GetTypeSprite(int type, int generation = PKX.Generation)
        {
            if (generation <= 2)
                type = (int)((MoveType)type).GetMoveTypeGeneration(generation);
            return Resources.ResourceManager.GetObject($"type_icon_{type:00}") as Image;
        }

        private static Image GetSprite(MysteryGift gift)
        {
            if (gift.Empty)
                return null;

            Image img;
            if (gift.IsEgg && gift.Species == 490) // Manaphy Egg
                img = (Image)(Resources.ResourceManager.GetObject("_490_e") ?? Resources.unknown);
            else if (gift.IsPokémon)
                img = GetSprite(gift.Species, gift.Form, gift.Gender, gift.HeldItem, gift.IsEgg, gift.IsShiny, gift.Format);
            else if (gift.IsItem)
            {
                int item = gift.ItemID;
                if (Legal.ZCrystalDictionary.TryGetValue(item, out int value))
                    item = value;
                img = (Image)(Resources.ResourceManager.GetObject("item_" + item) ?? Resources.unknown);
            }
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
                file = $"tr_{SAV.MultiplayerSpriteID:00}";
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
                if (box >= 0)
                    pkm.Box = box;
                var la = new LegalityAnalysis(pkm, SAV.Personal);
                if (!la.Valid && pkm.Species != 0)
                    sprite = ImageUtil.LayerImage(sprite, Resources.warn, 0, 14);
            }
            if (inBox) // in box
            {
                if (SAV.IsSlotLocked(box, slot))
                    sprite = ImageUtil.LayerImage(sprite, Resources.locked, 26, 0);
                else if (SAV.IsSlotInBattleTeam(box, slot))
                    sprite = ImageUtil.LayerImage(sprite, Resources.team, 21, 0);
            }

            return sprite;
        }

        // Extension Methods
        public static Image WallpaperImage(this SaveFile SAV, int box) => GetWallpaper(SAV, box);
        public static Image Sprite(this MysteryGift gift) => GetSprite(gift);
        public static Image Sprite(this SaveFile SAV) => GetSprite(SAV);
        public static Image Sprite(this PKM pkm, bool isBoxBGRed = false) => GetSprite(pkm, isBoxBGRed);
        public static Image Sprite(this PKM pkm, SaveFile SAV, int box, int slot, bool flagIllegal = false)
            => GetSprite(pkm, SAV, box, slot, flagIllegal);
    }
}
