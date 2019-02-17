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
            var sprite = name.Replace("CountG3", "G3").ToLower();
            return Resources.ResourceManager.GetObject(sprite) as Image;
        }

        public static Image GetRibbonSprite(string name, int max, int value)
        {
            var resource = GetRibbonSpriteName(name, max, value);
            return (Bitmap)Resources.ResourceManager.GetObject(resource);
        }

        private static string GetRibbonSpriteName(string name, int max, int value)
        {
            if (max != 4) // Memory
            {
                var sprite = name.ToLower();
                if (max == value)
                    return sprite + "2";
                return sprite;
            }

            // Count ribbons
            string n = name.Replace("Count", string.Empty).ToLower();
            switch (value)
            {
                case 2: return n + "super";
                case 3: return n + "hyper";
                case 4: return n + "master";
                default:
                    return n;
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

        private static Image GetSprite(PKM pk, bool isBoxBGRed = false)
        {
            var img = GetSprite(pk.Species, pk.AltForm, pk.Gender, pk.SpriteItem, pk.IsEgg, pk.IsShiny, pk.Format, isBoxBGRed);
            if (pk is IShadowPKM s && s.Purification > 0)
            {
                if (pk.Species == 249) // Lugia
                    img = Spriter.GetSprite(Resources._249x, 249, pk.HeldItem, pk.IsEgg, pk.IsShiny, pk.Format, isBoxBGRed);
                GetSpriteGlow(pk, new byte[] { 75, 0, 130 }, out var pixels, out var baseSprite, true);
                var glowImg = ImageUtil.GetBitmap(pixels, baseSprite.Width, baseSprite.Height, baseSprite.PixelFormat);
                img = ImageUtil.LayerImage(glowImg, img, 0, 0);
            }
            return img;
        }

        private static Image GetSprite(SaveFile sav)
        {
            string file = "tr_00";
            if (sav.Generation == 6 && (sav.ORAS || sav.ORASDEMO))
                file = $"tr_{sav.MultiplayerSpriteID:00}";
            return Resources.ResourceManager.GetObject(file) as Image;
        }

        private static Image GetWallpaper(SaveFile sav, int box)
        {
            string s = BoxWallpaper.GetWallpaperResourceName(sav.Version, sav.GetBoxWallpaper(box));
            return (Bitmap)(Resources.ResourceManager.GetObject(s) ?? Resources.box_wp16xy);
        }

        private static Image GetSprite(PKM pk, SaveFile sav, int box, int slot, bool flagIllegal = false)
        {
            if (!pk.Valid)
                return null;

            bool inBox = slot >= 0 && slot < 30;
            var sprite = pk.Species == 0 ? null : pk.Sprite(isBoxBGRed: inBox && BoxWallpaper.IsWallpaperRed(sav.Version, sav.GetBoxWallpaper(box)));

            if (flagIllegal)
            {
                if (box >= 0)
                    pk.Box = box;
                var la = new LegalityAnalysis(pk, sav.Personal);
                if (!la.Valid && pk.Species != 0)
                    sprite = ImageUtil.LayerImage(sprite, Resources.warn, 0, 14);
            }
            if (inBox) // in box
            {
                var flags = sav.GetSlotFlags(box, slot);
                if (flags.HasFlagFast(StorageSlotFlag.Locked))
                    sprite = ImageUtil.LayerImage(sprite, Resources.locked, 26, 0);
                int team = flags.IsBattleTeam();
                if (team >= 0)
                    sprite = ImageUtil.LayerImage(sprite, Resources.team, 21, 0);
                int party = flags.IsParty();
                if (party >= 0)
                    sprite = ImageUtil.LayerImage(sprite, PartyMarks[party], 24, 0);
                if (flags.HasFlagFast(StorageSlotFlag.Starter))
                    sprite = ImageUtil.LayerImage(sprite, Resources.starter, 0, 0);
            }

            return sprite;
        }

        private static readonly Image[] PartyMarks =
        {
            Resources.party1, Resources.party2, Resources.party3, Resources.party4, Resources.party5, Resources.party6,
        };

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
        public static Image WallpaperImage(this SaveFile sav, int box) => GetWallpaper(sav, box);
        public static Image Sprite(this MysteryGift gift) => GetSprite(gift);
        public static Image Sprite(this SaveFile sav) => GetSprite(sav);
        public static Image Sprite(this PKM pk, bool isBoxBGRed = false) => GetSprite(pk, isBoxBGRed);

        public static Image Sprite(this PKM pk, SaveFile sav, int box, int slot, bool flagIllegal = false)
            => GetSprite(pk, sav, box, slot, flagIllegal);
    }
}
