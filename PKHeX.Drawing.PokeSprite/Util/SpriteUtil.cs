using System;
using System.Drawing;
using System.Linq;
using PKHeX.Core;
using PKHeX.Drawing.PokeSprite.Properties;

namespace PKHeX.Drawing.PokeSprite;

public static class SpriteUtil
{
    public static readonly SpriteBuilder5668 SB8 = new();
    public static SpriteBuilder Spriter { get; set; } = SB8;

    private const int MaxSlotCount = 30; // slots in a box
    private static int SpriteWidth => Spriter.Width;
    private static int SpriteHeight => Spriter.Height;
    private static int PartyMarkShiftX => SpriteWidth - 16;
    private static int SlotLockShiftX => SpriteWidth - 14;
    private static int SlotTeamShiftX => SpriteWidth - 19;
    private static int FlagIllegalShiftY => SpriteHeight - 16;

    public static void Initialize(SaveFile sav) => Spriter.Initialize(sav);

    public static Image GetBallSprite(int ball)
    {
        string resource = SpriteName.GetResourceStringBall(ball);
        return (Bitmap?)Resources.ResourceManager.GetObject(resource) ?? Resources._ball4; // Poké Ball (default)
    }

    public static Image? GetItemSprite(int item) => Resources.ResourceManager.GetObject($"item_{item}") as Image;

    public static Image GetSprite(int species, int form, int gender, uint formarg, int item, bool isegg, bool isShiny, int generation = -1, bool isBoxBGRed = false, bool isAltShiny = false)
    {
        return Spriter.GetSprite(species, form, gender, formarg, item, isegg, isShiny, generation, isBoxBGRed, isAltShiny);
    }

    private static Image GetSprite(PKM pk, bool isBoxBGRed = false)
    {
        var formarg = pk is IFormArgument f ? f.FormArgument : 0;
        bool alt = pk.Format >= 8 && (pk.ShinyXor == 0 || pk.FatefulEncounter || pk.Version == (int)GameVersion.GO);
        var img = GetSprite(pk.Species, pk.Form, pk.Gender, formarg, pk.SpriteItem, pk.IsEgg, pk.IsShiny, pk.Format, isBoxBGRed, alt);
        if (pk is IShadowPKM {IsShadow: true})
        {
            const int Lugia = (int)Species.Lugia;
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
        if (pk is IAlpha {IsAlpha: true})
        {
            var alpha = Resources.alpha;
            return ImageUtil.LayerImage(img, alpha, SlotTeamShiftX, 0);
        }
        return img;
    }

    private static Image GetSprite(PKM pk, SaveFile sav, int box, int slot, bool flagIllegal = false)
    {
        if (!pk.Valid)
            return Spriter.None;

        bool inBox = (uint)slot < MaxSlotCount;
        bool empty = pk.Species == 0;
        var sprite = empty ? Spriter.None : pk.Sprite(isBoxBGRed: inBox && BoxWallpaper.IsWallpaperRed(sav.Version, sav.GetBoxWallpaper(box)));

        if (!empty && flagIllegal)
        {
            var la = new LegalityAnalysis(pk, sav.Personal, box != -1 ? SlotOrigin.Box : SlotOrigin.Party);
            if (!la.Valid)
                sprite = ImageUtil.LayerImage(sprite, Resources.warn, 0, FlagIllegalShiftY);
            else if (pk.Format >= 8 && pk.Moves.Any(Legal.GetDummiedMovesHashSet(pk).Contains))
                sprite = ImageUtil.LayerImage(sprite, Resources.hint, 0, FlagIllegalShiftY);

            if (SpriteBuilder.ShowEncounterColorPKM != SpriteBackgroundType.None)
                sprite = ApplyEncounterColor(la.EncounterOriginal, sprite, SpriteBuilder.ShowEncounterColorPKM);
        }
        if (inBox) // in box
        {
            var flags = sav.GetSlotFlags(box, slot);

            // Indicate any battle box teams & according locked state.
            int team = flags.IsBattleTeam();
            if (team >= 0)
                sprite = ImageUtil.LayerImage(sprite, Resources.team, SlotTeamShiftX, 0);
            if (flags.HasFlagFast(StorageSlotFlag.Locked))
                sprite = ImageUtil.LayerImage(sprite, Resources.locked, SlotLockShiftX, 0);

            // Some games store Party directly in the list of pokemon data (LGP/E). Indicate accordingly.
            int party = flags.IsParty();
            if (party >= 0)
                sprite = ImageUtil.LayerImage(sprite, PartyMarks[party], PartyMarkShiftX, 0);
            if (flags.HasFlagFast(StorageSlotFlag.Starter))
                sprite = ImageUtil.LayerImage(sprite, Resources.starter, 0, 0);
        }

        if (SpriteBuilder.ShowExperiencePercent)
            sprite = ApplyExperience(pk, sprite);

        return sprite;
    }

    public static Image ApplyEncounterColor(IEncounterTemplate enc, Image img, SpriteBackgroundType type)
    {
        var index = (enc.GetType().Name.GetHashCode() * 0x43FD43FD);
        var color = Color.FromArgb(index);
        if (type == SpriteBackgroundType.BottomStripe)
        {
            int stripeHeight = SpriteBuilder.ShowEncounterThicknessStripe; // from bottom
            byte opacity = SpriteBuilder.ShowEncounterOpacityStripe;
            return ImageUtil.ChangeTransparentTo(img, color, opacity, img.Width * 4 * (img.Height - stripeHeight));
        }
        else // full background
        {
            byte opacity = SpriteBuilder.ShowEncounterOpacityBackground;
            return ImageUtil.ChangeTransparentTo(img, color, opacity);
        }
    }

    private static Image ApplyExperience(PKM pk, Image img)
    {
        const int bpp = 4;
        int start = bpp * SpriteWidth * (SpriteHeight - 1);
        var level = pk.CurrentLevel;
        if (level == 100)
            return ImageUtil.WritePixels(img, Color.Lime, start, start + (SpriteWidth * bpp));

        var pct = Experience.GetEXPToLevelUpPercentage(level, pk.EXP, pk.PersonalInfo.EXPGrowth);
        if (pct is 0)
            return ImageUtil.WritePixels(img, Color.Yellow, start, start + (SpriteWidth * bpp));
        return ImageUtil.WritePixels(img, Color.DodgerBlue, start, start + (int)(SpriteWidth * pct * bpp));
    }

    private static readonly Bitmap[] PartyMarks =
    {
        Resources.party1, Resources.party2, Resources.party3, Resources.party4, Resources.party5, Resources.party6,
    };

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
    public static Image Sprite(this PKM pk, bool isBoxBGRed = false) => GetSprite(pk, isBoxBGRed);

    public static Image Sprite(this IEncounterTemplate enc)
    {
        if (enc is MysteryGift g)
            return GetMysteryGiftPreviewPoke(g);
        var gender = GetDisplayGender(enc);
        var img = GetSprite(enc.Species, enc.Form, gender, 0, 0, enc.EggEncounter, enc.IsShiny, enc.Generation);
        if (SpriteBuilder.ShowEncounterBall && enc is IFixedBall {FixedBall: not Ball.None} b)
        {
            var ballSprite = GetBallSprite((int)b.FixedBall);
            img = ImageUtil.LayerImage(img, ballSprite, 0, img.Height - ballSprite.Height);
        }
        if (enc is IGigantamax {CanGigantamax: true})
        {
            var gm = Resources.dyna;
            img = ImageUtil.LayerImage(img, gm, (img.Width - gm.Width) / 2, 0);
        }
        if (enc is IAlpha { IsAlpha: true })
        {
            var alpha = Resources.alpha;
            img = ImageUtil.LayerImage(img, alpha, SlotTeamShiftX, 0);
        }
        if (SpriteBuilder.ShowEncounterColor != SpriteBackgroundType.None)
            img = ApplyEncounterColor(enc, img, SpriteBuilder.ShowEncounterColor);
        return img;
    }

    public static int GetDisplayGender(IEncounterTemplate enc) => enc switch
    {
        EncounterSlotGO g => (int)g.Gender & 1,
        EncounterStatic s => Math.Max(0, s.Gender),
        EncounterTrade t => Math.Max(0, t.Gender),
        _ => 0,
    };

    public static Image Sprite(this PKM pk, SaveFile sav, int box, int slot, bool flagIllegal = false)
        => GetSprite(pk, sav, box, slot, flagIllegal);

    public static Image GetMysteryGiftPreviewPoke(MysteryGift gift)
    {
        if (gift.IsEgg && gift.Species == (int)Species.Manaphy) // Manaphy Egg
            return GetSprite((int)Species.Manaphy, 0, 2, 0, 0, true, false, gift.Generation);

        var gender = Math.Max(0, gift.Gender);
        var img = GetSprite(gift.Species, gift.Form, gender, 0, gift.HeldItem, gift.IsEgg, gift.IsShiny, gift.Generation);

        if (SpriteBuilder.ShowEncounterBall && gift is IFixedBall { FixedBall: not Ball.None } b)
        {
            var ballSprite = GetBallSprite((int)b.FixedBall);
            img = ImageUtil.LayerImage(img, ballSprite, 0, img.Height - ballSprite.Height);
        }

        if (gift is IGigantamax { CanGigantamax: true })
        {
            var gm = Resources.dyna;
            img = ImageUtil.LayerImage(img, gm, (img.Width - gm.Width) / 2, 0);
        }
        return img;
    }
}
