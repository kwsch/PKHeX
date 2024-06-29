using System;
using System.Buffers;
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
    /// <remarks>If an out-of-bounds value is provided, will not change.</remarks>
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

    public static Bitmap GetBallSprite(byte ball)
    {
        string resource = SpriteName.GetResourceStringBall(ball);
        return (Bitmap?)Resources.ResourceManager.GetObject(resource) ?? Resources._ball4; // Poké Ball (default)
    }

    public static Bitmap? GetItemSprite(int item) => Resources.ResourceManager.GetObject($"item_{item}") as Bitmap;

    public static Bitmap GetSprite(ushort species, byte form, byte gender, uint formarg, int item, bool isegg, Shiny shiny, EntityContext context = EntityContext.None)
    {
        return Spriter.GetSprite(species, form, gender, formarg, item, isegg, shiny, context);
    }

    private static Bitmap GetSprite(PKM pk)
    {
        var formarg = pk is IFormArgument f ? f.FormArgument : 0;
        var shiny = ShinyExtensions.GetType(pk);

        var img = GetSprite(pk.Species, pk.Form, pk.Gender, formarg, pk.SpriteItem, pk.IsEgg, shiny, pk.Context);
        if (pk is IShadowCapture {IsShadow: true})
        {
            const int Lugia = (int)Species.Lugia;
            if (pk.Species == Lugia) // show XD shadow sprite
                img = Spriter.GetSprite(Spriter.ShadowLugia, Lugia, pk.SpriteItem, pk.IsEgg, shiny, pk.Context);

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
            var alpha = Resources.alpha_alt;
            return ImageUtil.LayerImage(img, alpha, SlotTeamShiftX, 0);
        }
        return img;
    }

    private static Bitmap GetSprite(PKM pk, SaveFile sav, int box, int slot, bool flagIllegal = false, StorageSlotType storage = StorageSlotType.None)
    {
        bool inBox = (uint)slot < MaxSlotCount;
        bool empty = pk.Species == 0;
        var sprite = empty ? Spriter.None : pk.Sprite();

        if (!empty)
        {
            if (SpriteBuilder.ShowTeraType != SpriteBackgroundType.None && pk is ITeraType t)
            {
                var type = t.TeraType;
                if (TeraTypeUtil.IsOverrideValid((byte)type))
                    sprite = ApplyTeraColor((byte)type, sprite, SpriteBuilder.ShowTeraType);
            }
            if (flagIllegal)
            {
                var la = new LegalityAnalysis(pk, sav.Personal, storage);
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
            var flags = sav.GetBoxSlotFlags(box, slot);

            // Indicate any battle box teams & according locked state.
            int team = flags.IsBattleTeam();
            if (team >= 0)
                sprite = ImageUtil.LayerImage(sprite, Resources.team, SlotTeamShiftX, 0);
            if (flags.HasFlag(StorageSlotSource.Locked))
                sprite = ImageUtil.LayerImage(sprite, Resources.locked, SlotLockShiftX, 0);

            // Some games store Party directly in the list of Pokémon data (LGP/E). Indicate accordingly.
            int party = flags.IsParty();
            if (party >= 0)
                sprite = ImageUtil.LayerImage(sprite, PartyMarks[party], PartyMarkShiftX, 0);
            if (flags.HasFlag(StorageSlotSource.Starter))
                sprite = ImageUtil.LayerImage(sprite, Resources.starter, 0, 0);
        }

        if (SpriteBuilder.ShowExperiencePercent && !flagIllegal)
            sprite = ApplyExperience(pk, sprite);

        return sprite;
    }

    private static Bitmap ApplyTeraColor(byte elementalType, Bitmap img, SpriteBackgroundType type)
    {
        var color = TypeColor.GetTeraSpriteColor(elementalType);
        var thk = SpriteBuilder.ShowTeraThicknessStripe;
        var op  = SpriteBuilder.ShowTeraOpacityStripe;
        var bg  = SpriteBuilder.ShowTeraOpacityBackground;
        return ApplyColor(img, type, color, thk, op, bg);
    }

    public static Bitmap ApplyEncounterColor(IEncounterTemplate enc, Bitmap img, SpriteBackgroundType type)
    {
        var index = (enc.GetType().Name.GetHashCode() * 0x43FD43FD);
        var color = Color.FromArgb(index);
        var thk = SpriteBuilder.ShowEncounterThicknessStripe;
        var op = SpriteBuilder.ShowEncounterOpacityStripe;
        var bg = SpriteBuilder.ShowEncounterOpacityBackground;
        return ApplyColor(img, type, color, thk, op, bg);
    }

    private static Bitmap ApplyColor(Bitmap img, SpriteBackgroundType type, Color color, int thick, byte opacStripe, byte opacBack)
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

    private static Bitmap ApplyExperience(PKM pk, Bitmap img, IEncounterTemplate? enc = null)
    {
        const int bpp = 4;
        int start = bpp * SpriteWidth * (SpriteHeight - 1);
        var level = pk.CurrentLevel;
        if (level == 100)
            return ImageUtil.WritePixels(img, Color.Lime, start, start + (SpriteWidth * bpp));

        var pct = Experience.GetEXPToLevelUpPercentage(level, pk.EXP, pk.PersonalInfo.EXPGrowth);
        if (pct is not 0)
            return ImageUtil.WritePixels(img, Color.DodgerBlue, start, start + (int)(SpriteWidth * pct * bpp));

        var encLevel = enc is { IsEgg: true } ? enc.LevelMin : pk.MetLevel;
        var color = level != encLevel && pk.HasOriginalMetLocation ? Color.DarkOrange : Color.Yellow;
        return ImageUtil.WritePixels(img, color, start, start + (SpriteWidth * bpp));
    }

    private static readonly Bitmap[] PartyMarks =
    [
        Resources.party1, Resources.party2, Resources.party3, Resources.party4, Resources.party5, Resources.party6,
    ];

    public static void GetSpriteGlow(PKM pk, byte blue, byte green, byte red, out byte[] pixels, out Image baseSprite, bool forceHollow = false)
    {
        bool egg = pk.IsEgg;
        var formarg = pk is IFormArgument f ? f.FormArgument : 0;
        var shiny = pk.IsShiny ? Shiny.Always : Shiny.Never;
        baseSprite = GetSprite(pk.Species, pk.Form, pk.Gender, formarg, 0, egg, shiny, pk.Context);
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
        var temp = ArrayPool<byte>.Shared.Rent(pixels.Length);
        var original = temp.AsSpan(0, pixels.Length);
        pixels.CopyTo(original);

        ImageUtil.SetAllUsedPixelsOpaque(pixels);
        ImageUtil.GlowEdges(pixels, blue, green, red, baseSprite.Width);
        ImageUtil.RemovePixels(pixels, original);

        original.Clear();
        ArrayPool<byte>.Shared.Return(temp);
    }

    public static Bitmap GetLegalIndicator(bool valid) => valid ? Resources.valid : Resources.warn;

    // Extension Methods
    public static Bitmap Sprite(this PKM pk) => GetSprite(pk);

    public static Bitmap Sprite(this IEncounterTemplate enc)
    {
        if (enc is MysteryGift g)
            return GetMysteryGiftPreviewPoke(g);
        var gender = GetDisplayGender(enc);
        var shiny = enc.IsShiny ? Shiny.Always : Shiny.Never;
        var img = GetSprite(enc.Species, enc.Form, gender, 0, 0, enc.IsEgg, shiny, enc.Context);
        if (SpriteBuilder.ShowEncounterBall && enc is {FixedBall: not Ball.None})
        {
            var ballSprite = GetBallSprite((byte)enc.FixedBall);
            img = ImageUtil.LayerImage(img, ballSprite, 0, img.Height - ballSprite.Height);
        }
        if (enc is IGigantamaxReadOnly {CanGigantamax: true})
        {
            var gm = Resources.dyna;
            img = ImageUtil.LayerImage(img, gm, (img.Width - gm.Width) / 2, 0);
        }
        if (enc is IAlphaReadOnly { IsAlpha: true })
        {
            var alpha = Resources.alpha_alt;
            img = ImageUtil.LayerImage(img, alpha, SlotTeamShiftX, 0);
        }
        if (SpriteBuilder.ShowEncounterColor != SpriteBackgroundType.None)
            img = ApplyEncounterColor(enc, img, SpriteBuilder.ShowEncounterColor);
        return img;
    }

    public static byte GetDisplayGender(IEncounterTemplate enc) => enc switch
    {
        IFixedGender { IsFixedGender: true } s => Math.Max((byte)0, s.Gender),
        IPogoSlot g => (byte)((int)g.Gender & 1),
        _ => 0,
    };

    public static Bitmap Sprite(this PKM pk, SaveFile sav, int box = -1, int slot = -1,
        bool flagIllegal = false, StorageSlotType storage = StorageSlotType.None)
        => GetSprite(pk, sav, box, slot, flagIllegal, storage);

    public static Bitmap GetMysteryGiftPreviewPoke(MysteryGift gift)
    {
        if (gift is { IsEgg: true, Species: (int)Species.Manaphy }) // Manaphy Egg
            return GetSprite((int)Species.Manaphy, 0, 2, 0, 0, true, Shiny.Never, gift.Context);

        var gender = Math.Max((byte)0, gift.Gender);
        var img = GetSprite(gift.Species, gift.Form, gender, 0, gift.HeldItem, gift.IsEgg, gift.IsShiny ? Shiny.Always : Shiny.Never, gift.Context);

        if (SpriteBuilder.ShowEncounterBall && gift is { FixedBall: not Ball.None })
        {
            var ballSprite = GetBallSprite((byte)gift.FixedBall);
            img = ImageUtil.LayerImage(img, ballSprite, 0, img.Height - ballSprite.Height);
        }

        if (gift is IGigantamaxReadOnly { CanGigantamax: true })
        {
            var gm = Resources.dyna;
            img = ImageUtil.LayerImage(img, gm, (img.Width - gm.Width) / 2, 0);
        }
        return img;
    }

    public static Image? GetStatusSprite(this StatusCondition value)
    {
        if (value == 0)
            return null;
        if (value < StatusCondition.Poison)
            return Resources.sicksleep;
        if (value.HasFlag(StatusCondition.PoisonBad))
            return Resources.sicktoxic;
        if (value.HasFlag(StatusCondition.Poison))
            return Resources.sickpoison;
        if (value.HasFlag(StatusCondition.Burn))
            return Resources.sickburn;
        if (value.HasFlag(StatusCondition.Paralysis))
            return Resources.sickparalyze;
        if (value.HasFlag(StatusCondition.Freeze))
            return Resources.sickfrostbite;
        return null;
    }
}
