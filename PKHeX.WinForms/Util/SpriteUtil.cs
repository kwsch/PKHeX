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

        public static Image GetRibbonSprite(string name, int max, int value)
        {
            var resource = GetRibbonSpriteName(name, max, value);
            return (Bitmap)Resources.ResourceManager.GetObject(resource);
        }

        private static string GetRibbonSpriteName(string name, int max, int value)
        {
            if (max != 4) // Memory
                return name.ToLower() + (max == value ? "2" : "");

            // Count ribbons
            string n = name.Replace("Count", "");
            switch (value)
            {
                case 2: return (n + "Super").ToLower();
                case 3: return (n + "Hyper").ToLower();
                case 4: return (n + "Master").ToLower();
                default:
                    return n.ToLower();
            }
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

            var img = GetBaseImage(gift);
            if (gift.GiftUsed)
                img = ImageUtil.ChangeOpacity(img, 0.3);
            return img;
        }

        private static Image GetBaseImage(MysteryGift gift)
        {
            if (gift.IsEgg && gift.Species == 490) // Manaphy Egg
                return Resources._490_e;
            if (gift.IsPokémon)
                return GetSprite(gift.Species, gift.Form, gift.Gender, gift.HeldItem, gift.IsEgg, gift.IsShiny, gift.Format);
            if (gift.IsItem)
            {
                int item = gift.ItemID;
                if (Legal.ZCrystalDictionary.TryGetValue(item, out int value))
                    item = value;
                return (Image)(Resources.ResourceManager.GetObject("item_" + item) ?? Resources.Bag_Key);
            }
            return Resources.unknown;
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
            string s = BoxWallpaper.GetWallpaperResourceName(SAV, box);
            return (Bitmap)(Resources.ResourceManager.GetObject(s) ?? Resources.box_wp16xy);
        }

        private static Image GetSprite(PKM pkm, SaveFile SAV, int box, int slot, bool flagIllegal = false)
        {
            if (!pkm.Valid)
                return null;

            bool inBox = slot >= 0 && slot < 30;
            var sprite = pkm.Species != 0 ? pkm.Sprite(isBoxBGRed: inBox && BoxWallpaper.IsWallpaperRed(SAV, box)) : null;

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

        public static void GetSpriteGlow(PKM pk, byte[] bgr, out byte[] pixels, out Image baseSprite, bool forceHollow = false)
        {
            bool egg = pk.IsEgg;
            baseSprite = GetSprite(pk.Species, pk.AltForm, pk.Gender, 0, egg, false, pk.Format);
            GetSpriteGlow(baseSprite, bgr, out pixels, forceHollow || egg);
        }

        public static void GetSpriteGlow(Image baseSprite, byte[] bgr, out byte[] pixels, bool forceHollow = false)
        {
            pixels = ImageUtil.GetPixelData((Bitmap) baseSprite);
            if (!forceHollow)
            {
                ImageUtil.GlowEdges(pixels, bgr, baseSprite.Width);
                return;
            }

            // If the image has any transparency, any derived background will bleed into it.
            // Need to undo any transparency values if any present.
            // Remove opaque pixels from original image, leaving only the glow effect pixels.
            var original = (byte[]) pixels.Clone();
            ImageUtil.SetAllUsedPixelsOpaque(pixels);
            ImageUtil.GlowEdges(pixels, bgr, baseSprite.Width);
            ImageUtil.RemovePixels(pixels, original);
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
