using System;
using System.Drawing;
using PKHeX.Core;
using PKHeX.Drawing.PokeSprite.Properties;

namespace PKHeX.Drawing.PokeSprite;

/// <summary>
/// Singleton that builds sprite images.
/// </summary>
public static class SpriteUtil
{
    /// <summary>Square sprite builder instance</summary>
    public static readonly SpriteBuilder5668s SB8s = new();
    /// <summary>Circle sprite builder instance (used in Legends: Arceus)</summary>
    public static readonly SpriteBuilder5668c SB8c = new();
    /// <summary>Circle sprite builder instance (used in Brilliant Diamond, Shining Pearl, Scarlet, and Violet)</summary>
    public static readonly SpriteBuilder5668a SB8a = new();

    /// <summary>Current sprite builder reference used to build sprites.</summary>
    public static SpriteBuilder Spriter { get; private set; } = SB8s;

    /// <summary>
    /// Changes the builder mode to the requested mode.
    /// </summary>
    /// <param name="mode">Requested sprite builder mode</param>
    /// <remarks>If an out of bounds value is provided, will not change.</remarks>
    public static void ChangeMode(SpriteBuilderMode mode) => Spriter = mode switch
    {
        SpriteBuilderMode.SpritesArtwork5668 => SB8a,
        SpriteBuilderMode.CircleMugshot5668 => SB8c,
        SpriteBuilderMode.SpritesClassic5668 => SB8s,
        _ => Spriter,
    };

    private const int MaxSlotCount = 30; // slots in a box
    private static int SpriteWidth => Spriter.Width;
    private static int SpriteHeight => Spriter.Height;
    private static int PartyMarkShiftX => SpriteWidth - 16;
    private static int SlotLockShiftX => SpriteWidth - 14;
    private static int SlotTeamShiftX => SpriteWidth - 19;
    private static int FlagIllegalShiftY => SpriteHeight - 16;

    /// <summary>
    /// Sets up the sprite builder to behave with the input <see cref="sav"/>.
    /// </summary>
    /// <param name="sav">Save File to be generating sprites for.</param>
    public static void Initialize(SaveFile sav)
    {
        ChangeMode(SpriteBuilderUtil.GetSuggestedMode(sav));
        Spriter.Initialize(sav);
    }

    public static Image GetBallSprite(int ball)
    {
        string resource = SpriteName.GetResourceStringBall(ball);
        return (Bitmap?)Resources.ResourceManager.GetObject(resource) ?? Resources._ball4; // Poké Ball (default)
    }

    public static Image? GetItemSprite(int item) => Resources.ResourceManager.GetObject($"item_{item}") as Image;

    public static Image GetSprite(ushort species, byte form, int gender, uint formarg, int item, bool isegg, Shiny shiny, int generation = -1, SpriteBuilderTweak tweak = SpriteBuilderTweak.None)
    {
        return Spriter.GetSprite(species, form, gender, formarg, item, isegg, shiny, generation, tweak);
    }

    private static Image GetSprite(PKM pk, SpriteBuilderTweak tweak = SpriteBuilderTweak.None)
    {
        var formarg = pk is IFormArgument f ? f.FormArgument : 0;
        var shiny = !pk.IsShiny ? Shiny.Never : (ShinyExtensions.IsSquareShinyExist(pk) ? Shiny.AlwaysSquare : Shiny.AlwaysStar);

        var img = GetSprite(pk.Species, pk.Form, pk.Gender, formarg, pk.SpriteItem, pk.IsEgg, shiny, pk.Format, tweak);
        if (pk is IShadowCapture {IsShadow: true})
        {
            const int Lugia = (int)Species.Lugia;
            if (pk.Species == Lugia) // show XD shadow sprite
                img = Spriter.GetSprite(Spriter.ShadowLugia, Lugia, pk.SpriteItem, pk.IsEgg, shiny, pk.Format, tweak);

            GetSpriteGlow(pk, 75, 0, 130, out var pixels, out var baseSprite, true);
            var glowImg = ImageUtil.GetBitmap(pixels, baseSprite.Width, baseSprite.Height, baseSprite.PixelFormat);
            return ImageUtil.LayerImage(glowImg, img, 0, 0);
        }
        if (pk is IGigantamaxReadOnly { CanGigantamax: true})
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
        bool inBox = (uint)slot < MaxSlotCount;
        bool empty = pk.Species == 0;
        var tweak = inBox && sav.IsWallpaperRed(box)
            ? SpriteBuilderTweak.BoxBackgroundRed
            : SpriteBuilderTweak.None;
        var sprite = empty ? Spriter.None : pk.Sprite(tweak: tweak);

        if (!empty)
        {
            if (SpriteBuilder.ShowTeraType != SpriteBackgroundType.None && pk is ITeraType t)
            {
                var type = t.TeraType;
                sprite = ApplyTeraColor((byte)type, sprite, SpriteBuilder.ShowTeraType);
            }
            if (flagIllegal)
            {
                var la = new LegalityAnalysis(pk, sav.Personal, box != -1 ? SlotOrigin.Box : SlotOrigin.Party);
                if (!la.Valid)
                    sprite = ImageUtil.LayerImage(sprite, Resources.warn, 0, FlagIllegalShiftY);
                else if (pk.Format >= 8 && MoveInfo.IsDummiedMoveAny(pk))
                    sprite = ImageUtil.LayerImage(sprite, Resources.hint, 0, FlagIllegalShiftY);

                if (SpriteBuilder.ShowEncounterColorPKM != SpriteBackgroundType.None)
                    sprite = ApplyEncounterColor(la.EncounterOriginal, sprite, SpriteBuilder.ShowEncounterColorPKM);

                if (SpriteBuilder.ShowExperiencePercent)
                    sprite = ApplyExperience(pk, sprite, la.EncounterMatch);
            }
        }
        if (inBox) // in box
        {
            var flags = sav.GetSlotFlags(box, slot);

            // Indicate any battle box teams & according locked state.
            int team = flags.IsBattleTeam();
            if (team >= 0)
                sprite = ImageUtil.LayerImage(sprite, Resources.team, SlotTeamShiftX, 0);
            if (flags.HasFlagFast(StorageSlotSource.Locked))
                sprite = ImageUtil.LayerImage(sprite, Resources.locked, SlotLockShiftX, 0);

            // Some games store Party directly in the list of pokemon data (LGP/E). Indicate accordingly.
            int party = flags.IsParty();
            if (party >= 0)
                sprite = ImageUtil.LayerImage(sprite, PartyMarks[party], PartyMarkShiftX, 0);
            if (flags.HasFlagFast(StorageSlotSource.Starter))
                sprite = ImageUtil.LayerImage(sprite, Resources.starter, 0, 0);
        }

        if (SpriteBuilder.ShowExperiencePercent && !flagIllegal)
            sprite = ApplyExperience(pk, sprite);

        return sprite;
    }

    private static Image ApplyTeraColor(byte elementalType, Image img, SpriteBackgroundType type)
    {
        var color = TypeColor.GetTypeSpriteColor(elementalType);
        var thk = SpriteBuilder.ShowTeraThicknessStripe;
        var op  = SpriteBuilder.ShowTeraOpacityStripe;
        var bg  = SpriteBuilder.ShowTeraOpacityBackground;
        return ApplyColor(img, type, color, thk, op, bg);
    }

    public static Image ApplyEncounterColor(IEncounterTemplate enc, Image img, SpriteBackgroundType type)
    {
        var index = (enc.GetType().Name.GetHashCode() * 0x43FD43FD);
        var color = Color.FromArgb(index);
        var thk = SpriteBuilder.ShowEncounterThicknessStripe;
        var op = SpriteBuilder.ShowEncounterOpacityStripe;
        var bg = SpriteBuilder.ShowEncounterOpacityBackground;
        return ApplyColor(img, type, color, thk, op, bg);
    }

    private static Image ApplyColor(Image img, SpriteBackgroundType type, Color color, int thick, byte opacStripe, byte opacBack)
    {
        if (type == SpriteBackgroundType.BottomStripe)
        {
            int stripeHeight = thick; // from bottom
            if ((uint)stripeHeight > img.Height) // clamp negative & too-high values back to height.
                stripeHeight = img.Height;

            return ImageUtil.BlendTransparentTo(img, color, opacStripe, img.Width * 4 * (img.Height - stripeHeight));
        }
        if (type == SpriteBackgroundType.TopStripe)
        {
            int stripeHeight = thick; // from top
            if ((uint)stripeHeight > img.Height) // clamp negative & too-high values back to height.
                stripeHeight = img.Height;

            return ImageUtil.BlendTransparentTo(img, color, opacStripe, 0, (img.Width * 4 * stripeHeight) - 4);
        }
        if (type == SpriteBackgroundType.FullBackground) // full background
        {
            return ImageUtil.ChangeTransparentTo(img, color, opacBack);
        }
        return img;
    }

    private static Image ApplyExperience(PKM pk, Image img, IEncounterTemplate? enc = null)
    {
        const int bpp = 4;
        int start = bpp * SpriteWidth * (SpriteHeight - 1);
        var level = pk.CurrentLevel;
        if (level == 100)
            return ImageUtil.WritePixels(img, Color.Lime, start, start + (SpriteWidth * bpp));

        var pct = Experience.GetEXPToLevelUpPercentage(level, pk.EXP, pk.PersonalInfo.EXPGrowth);
        if (pct is not 0)
            return ImageUtil.WritePixels(img, Color.DodgerBlue, start, start + (int)(SpriteWidth * pct * bpp));

        var encLevel = enc is { EggEncounter: true } x ? x.LevelMin : pk.Met_Level;
        var color = level != encLevel && pk.HasOriginalMetLocation ? Color.DarkOrange : Color.Yellow;
        return ImageUtil.WritePixels(img, color, start, start + (SpriteWidth * bpp));
    }

    private static readonly Bitmap[] PartyMarks =
    {
        Resources.party1, Resources.party2, Resources.party3, Resources.party4, Resources.party5, Resources.party6,
    };

    public static void GetSpriteGlow(PKM pk, byte blue, byte green, byte red, out byte[] pixels, out Image baseSprite, bool forceHollow = false)
    {
        bool egg = pk.IsEgg;
        var formarg = pk is IFormArgument f ? f.FormArgument : 0;
        baseSprite = GetSprite(pk.Species, pk.Form, pk.Gender, formarg, 0, egg, Shiny.Never, pk.Format);
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
    public static Image Sprite(this PKM pk, SpriteBuilderTweak tweak = SpriteBuilderTweak.None) => GetSprite(pk, tweak);

    public static Image Sprite(this IEncounterTemplate enc)
    {
        if (enc is MysteryGift g)
            return GetMysteryGiftPreviewPoke(g);
        var gender = GetDisplayGender(enc);
        var img = GetSprite(enc.Species, enc.Form, gender, 0, 0, enc.EggEncounter, enc.IsShiny ? Shiny.Always : Shiny.Never, enc.Generation);
        if (SpriteBuilder.ShowEncounterBall && enc is IFixedBall {FixedBall: not Ball.None} b)
        {
            var ballSprite = GetBallSprite((int)b.FixedBall);
            img = ImageUtil.LayerImage(img, ballSprite, 0, img.Height - ballSprite.Height);
        }
        if (enc is IGigantamaxReadOnly {CanGigantamax: true})
        {
            var gm = Resources.dyna;
            img = ImageUtil.LayerImage(img, gm, (img.Width - gm.Width) / 2, 0);
        }
        if (enc is IAlphaReadOnly { IsAlpha: true })
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
        EncounterStatic s => Math.Max(0, (int)s.Gender),
        EncounterTrade t => Math.Max(0, (int)t.Gender),
        _ => 0,
    };

    public static Image Sprite(this PKM pk, SaveFile sav, int box, int slot, bool flagIllegal = false)
        => GetSprite(pk, sav, box, slot, flagIllegal);

    public static Image GetMysteryGiftPreviewPoke(MysteryGift gift)
    {
        if (gift.IsEgg && gift.Species == (int)Species.Manaphy) // Manaphy Egg
            return GetSprite((int)Species.Manaphy, 0, 2, 0, 0, true, Shiny.Never, gift.Generation);

        var gender = Math.Max(0, gift.Gender);
        var img = GetSprite(gift.Species, gift.Form, gender, 0, gift.HeldItem, gift.IsEgg, gift.IsShiny ? Shiny.Always : Shiny.Never, gift.Generation);

        if (SpriteBuilder.ShowEncounterBall && gift is IFixedBall { FixedBall: not Ball.None } b)
        {
            var ballSprite = GetBallSprite((int)b.FixedBall);
            img = ImageUtil.LayerImage(img, ballSprite, 0, img.Height - ballSprite.Height);
        }

        if (gift is IGigantamaxReadOnly { CanGigantamax: true })
        {
            var gm = Resources.dyna;
            img = ImageUtil.LayerImage(img, gm, (img.Width - gm.Width) / 2, 0);
        }
        return img;
    }
}
