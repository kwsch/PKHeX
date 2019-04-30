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
            string resource = PKX.GetResourceStringBall(ball);
            return (Image)Resources.ResourceManager.GetObject(resource) ?? Resources._ball4; // Poké Ball (default)
        }

        public static Image GetSprite(int species, int form, int gender, int item, bool isegg, bool shiny, int generation = -1, bool isBoxBGRed = false)
        {
            return Spriter.GetSprite(species, form, gender, item, isegg, shiny, generation, isBoxBGRed);
        }

        public static Image GetRibbonSprite(string name)
        {
            var resource = name.Replace("CountG3", "G3").ToLower();
            return (Bitmap)Resources.ResourceManager.GetObject(resource);
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
            return (Bitmap)Resources.ResourceManager.GetObject($"type_icon_{type:00}");
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
                return (Image)(Resources.ResourceManager.GetObject($"item_{item}") ?? Resources.Bag_Key);
            }
            return Resources.unknown;
        }

        private static Image GetSprite(PKM pk, bool isBoxBGRed = false)
        {
            var img = GetSprite(pk.Species, pk.AltForm, pk.Gender, pk.SpriteItem, pk.IsEgg, pk.IsShiny, pk.Format, isBoxBGRed);
            if (pk is IShadowPKM s && s.Purification > 0)
            {
                const int Lugia = 249;
                if (pk.Species == Lugia) // show XD shadow sprite
                    img = Spriter.GetSprite(Resources._249x, Lugia, pk.HeldItem, pk.IsEgg, pk.IsShiny, pk.Format, isBoxBGRed);
                GetSpriteGlow(pk, 75, 0, 130, out var pixels, out var baseSprite, true);
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
            return (Bitmap)Resources.ResourceManager.GetObject(s) ?? Resources.box_wp16xy;
        }

        private static Image GetSprite(PKM pk, SaveFile sav, int box, int slot, bool flagIllegal = false)
        {
            if (!pk.Valid)
                return null;

            bool inBox = (uint)slot < MaxSlotCount;
            bool empty = pk.Species == 0;
            var sprite = empty ? null : pk.Sprite(isBoxBGRed: inBox && BoxWallpaper.IsWallpaperRed(sav.Version, sav.GetBoxWallpaper(box)));

            if (!empty && flagIllegal)
            {
                if (box >= 0)
                    pk.Box = box;
                var la = new LegalityAnalysis(pk, sav.Personal);
                if (!la.Valid)
                    sprite = ImageUtil.LayerImage(sprite, Resources.warn, 0, FlagIllegalShiftY);
            }
            if (inBox) // in box
            {
                var flags = sav.GetSlotFlags(box, slot);
                if (flags.HasFlagFast(StorageSlotFlag.Locked))
                    sprite = ImageUtil.LayerImage(sprite, Resources.locked, SlotLockShiftX, 0);
                int team = flags.IsBattleTeam();
                if (team >= 0)
                    sprite = ImageUtil.LayerImage(sprite, Resources.team, SlotTeamShiftX, 0);
                int party = flags.IsParty();
                if (party >= 0)
                    sprite = ImageUtil.LayerImage(sprite, PartyMarks[party], PartyMarkShiftX, 0);
                if (flags.HasFlagFast(StorageSlotFlag.Starter))
                    sprite = ImageUtil.LayerImage(sprite, Resources.starter, 0, 0);
            }

            return sprite;
        }

        private const int MaxSlotCount = 30; // slots in a box
        private const int SpriteWidth = 40;
        private const int SpriteHeight = 30;
        private const int PartyMarkShiftX = SpriteWidth - 16;
        private const int SlotLockShiftX = SpriteWidth - 14;
        private const int SlotTeamShiftX = SpriteWidth - 19;
        private const int FlagIllegalShiftY = SpriteHeight - 16;

        private static readonly Bitmap[] PartyMarks =
        {
            Resources.party1, Resources.party2, Resources.party3, Resources.party4, Resources.party5, Resources.party6,
        };

        public static void GetSpriteGlow(PKM pk, byte[] bgr, out byte[] pixels, out Image baseSprite, bool forceHollow = false)
        {
            GetSpriteGlow(pk, bgr[0], bgr[1], bgr[2], out pixels, out baseSprite, forceHollow);
        }

        public static void GetSpriteGlow(PKM pk, byte blue, byte green, byte red, out byte[] pixels, out Image baseSprite, bool forceHollow = false)
        {
            bool egg = pk.IsEgg;
            baseSprite = GetSprite(pk.Species, pk.AltForm, pk.Gender, 0, egg, false, pk.Format);
            GetSpriteGlow(baseSprite, blue, green, red, out pixels, forceHollow || egg);
        }

        public static void GetSpriteGlow(Image baseSprite, byte blue, byte green, byte red, out byte[] pixels, bool forceHollow = false)
        {
            pixels = ImageUtil.GetPixelData((Bitmap)baseSprite);
            if (!forceHollow)
            {
                ImageUtil.GlowEdges(pixels, blue, green, red, baseSprite.Width);
                return;
            }

            // If the image has any transparency, any derived background will bleed into it.
            // Need to undo any transparency values if any present.
            // Remove opaque pixels from original image, leaving only the glow effect pixels.
            var original = (byte[])pixels.Clone();
            ImageUtil.SetAllUsedPixelsOpaque(pixels);
            ImageUtil.GlowEdges(pixels, blue, green, red, baseSprite.Width);
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
