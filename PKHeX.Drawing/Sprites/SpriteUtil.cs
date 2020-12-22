using System.Drawing;
using System.Linq;
using PKHeX.Core;
using PKHeX.Drawing.Properties;

namespace PKHeX.Drawing
{
    public static class SpriteUtil
    {
        public static readonly SpriteBuilder3040 SB17 = new();
        public static readonly SpriteBuilder5668 SB8 = new();
        public static SpriteBuilder Spriter { get; set; } = SB8;

        public static Image GetBallSprite(int ball)
        {
            string resource = SpriteName.GetResourceStringBall(ball);
            return (Bitmap?)Resources.ResourceManager.GetObject(resource) ?? Resources._ball4; // Poké Ball (default)
        }

        public static Image GetSprite(int species, int form, int gender, uint formarg, int item, bool isegg, bool isShiny, int generation = -1, bool isBoxBGRed = false, bool isAltShiny = false)
        {
            return Spriter.GetSprite(species, form, gender, formarg, item, isegg, isShiny, generation, isBoxBGRed, isAltShiny);
        }

        public static Image? GetRibbonSprite(string name)
        {
            var resource = name.Replace("CountG3", "G3").ToLower();
            return (Bitmap?)Resources.ResourceManager.GetObject(resource);
        }

        public static Image? GetRibbonSprite(string name, int max, int value)
        {
            var resource = GetRibbonSpriteName(name, max, value);
            return (Bitmap?)Resources.ResourceManager.GetObject(resource);
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
            return value switch
            {
                2 => n + "super",
                3 => n + "hyper",
                4 => n + "master",
                _ => n
            };
        }

        public static Image? GetTypeSprite(int type, int generation = PKX.Generation)
        {
            if (generation <= 2)
                type = (int)((MoveType)type).GetMoveTypeGeneration(generation);
            return (Bitmap?)Resources.ResourceManager.GetObject($"type_icon_{type:00}");
        }

        private static Image GetSprite(MysteryGift gift)
        {
            if (gift.Empty)
                return Resources._0;

            var img = GetBaseImage(gift);
            if (gift.GiftUsed)
                img = ImageUtil.ChangeOpacity(img, 0.3);
            return img;
        }

        private static Image GetBaseImage(MysteryGift gift)
        {
            if (gift.IsEgg && gift.Species == (int)Species.Manaphy) // Manaphy Egg
                return Resources._490_e;
            if (gift.IsPokémon)
                return GetSprite(gift.Species, gift.Form, gift.Gender, 0, gift.HeldItem, gift.IsEgg, gift.IsShiny, gift.Generation);
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
            var formarg = pk is IFormArgument f ? f.FormArgument : 0;
            bool alt = pk.Format >= 8 && (pk.ShinyXor == 0 || pk.FatefulEncounter || pk.Version == (int)GameVersion.GO);
            var img = GetSprite(pk.Species, pk.Form, pk.Gender, formarg, pk.SpriteItem, pk.IsEgg, pk.IsShiny, pk.Format, isBoxBGRed, alt);
            if (pk is IShadowPKM {IsShadow: true})
            {
                const int Lugia = 249;
                if (pk.Species == Lugia) // show XD shadow sprite
                    img = Spriter.GetSprite(Spriter.ShadowLugia, Lugia, pk.HeldItem, pk.IsEgg, pk.IsShiny, pk.Format, isBoxBGRed);
                GetSpriteGlow(pk, 75, 0, 130, out var pixels, out var baseSprite, true);
                var glowImg = ImageUtil.GetBitmap(pixels, baseSprite.Width, baseSprite.Height, baseSprite.PixelFormat);
                return ImageUtil.LayerImage(glowImg, img, 0, 0);
            }
            if (pk is IGigantamax {CanGigantamax: true})
            {
                var gm = Resources.dyna;
                return ImageUtil.LayerImage(img, gm, (img.Width - gm.Width) / 2, 0);
            }
            return img;
        }

        private static Image? GetSprite(SaveFile sav)
        {
            string file = "tr_00";
            if (sav is SAV6AO)
                file = $"tr_{sav.MultiplayerSpriteID:00}";
            return Resources.ResourceManager.GetObject(file) as Image;
        }

        private static Image GetWallpaper(SaveFile sav, int box)
        {
            string s = BoxWallpaper.GetWallpaperResourceName(sav.Version, sav.GetBoxWallpaper(box));
            return (Bitmap?)Resources.ResourceManager.GetObject(s) ?? Resources.box_wp16xy;
        }

        private static Image GetSprite(PKM pk, SaveFile sav, int box, int slot, bool flagIllegal = false)
        {
            if (!pk.Valid)
                return Resources._0;

            bool inBox = (uint)slot < MaxSlotCount;
            bool empty = pk.Species == 0;
            var sprite = empty ? Resources._0 : pk.Sprite(isBoxBGRed: inBox && BoxWallpaper.IsWallpaperRed(sav.Version, sav.GetBoxWallpaper(box)));

            if (!empty && flagIllegal)
            {
                if (box >= 0)
                    pk.Box = box;
                var la = new LegalityAnalysis(pk, sav.Personal);
                if (!la.Valid)
                    sprite = ImageUtil.LayerImage(sprite, Resources.warn, 0, FlagIllegalShiftY);
                else if (pk.Format >= 8 && pk.Moves.Any(Legal.DummiedMoves_SWSH.Contains))
                    sprite = ImageUtil.LayerImage(sprite, Resources.hint, 0, FlagIllegalShiftY);
            }
            if (inBox) // in box
            {
                var flags = sav.GetSlotFlags(box, slot);
                int team = flags.IsBattleTeam();
                if (team >= 0)
                    sprite = ImageUtil.LayerImage(sprite, Resources.team, SlotTeamShiftX, 0);
                if (flags.HasFlagFast(StorageSlotFlag.Locked))
                    sprite = ImageUtil.LayerImage(sprite, Resources.locked, SlotLockShiftX, 0);
                int party = flags.IsParty();
                if (party >= 0)
                    sprite = ImageUtil.LayerImage(sprite, PartyMarks[party], PartyMarkShiftX, 0);
                if (flags.HasFlagFast(StorageSlotFlag.Starter))
                    sprite = ImageUtil.LayerImage(sprite, Resources.starter, 0, 0);
            }

            return sprite;
        }

        private const int MaxSlotCount = 30; // slots in a box
        private static int SpriteWidth => Spriter.Width;
        private static int SpriteHeight => Spriter.Height;
        private static int PartyMarkShiftX => SpriteWidth - 16;
        private static int SlotLockShiftX => SpriteWidth - 14;
        private static int SlotTeamShiftX => SpriteWidth - 19;
        private static int FlagIllegalShiftY => SpriteHeight - 16;

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
            var formarg = pk is IFormArgument f ? f.FormArgument : 0;
            baseSprite = GetSprite(pk.Species, pk.Form, pk.Gender, formarg, 0, egg, false, pk.Format);
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

        public static Image GetLegalIndicator(bool valid) => valid ? Resources.valid : Resources.warn;

        // Extension Methods
        public static Image WallpaperImage(this SaveFile sav, int box) => GetWallpaper(sav, box);
        public static Image Sprite(this MysteryGift gift) => GetSprite(gift);
        public static Image? Sprite(this SaveFile sav) => GetSprite(sav);
        public static Image Sprite(this PKM pk, bool isBoxBGRed = false) => GetSprite(pk, isBoxBGRed);

        public static Image Sprite(this PKM pk, SaveFile sav, int box, int slot, bool flagIllegal = false)
            => GetSprite(pk, sav, box, slot, flagIllegal);

        public static bool UseLargeAlways { get; set; } = true;

        public static void Initialize(SaveFile sav)
        {
            var s = GetSpriter(sav);

            // gen3 specific sprites
            s.Initialize(sav);

            Spriter = s;
        }

        private static SpriteBuilder GetSpriter(SaveFile sav)
        {
            if (UseLargeAlways)
                return SB8;

            var big = GameVersion.GG.Contains(sav.Version) || sav.Generation >= 8;
            return big ? (SpriteBuilder) SB8 : SB17;
        }
    }
}
