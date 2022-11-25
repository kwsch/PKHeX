using System.Drawing;
using PKHeX.Core;
using PKHeX.Drawing.PokeSprite.Properties;

namespace PKHeX.Drawing.PokeSprite;

public abstract class SpriteBuilder : ISpriteBuilder<Image>
{
    public static bool ShowEggSpriteAsItem { get; set; } = true;
    public static bool ShowEncounterBall { get; set; } = true;
    public static SpriteBackgroundType ShowEncounterColor { get; set; } = SpriteBackgroundType.FullBackground;
    public static SpriteBackgroundType ShowEncounterColorPKM { get; set; }
    public static SpriteBackgroundType ShowTeraType { get; set; } = SpriteBackgroundType.TopStripe;
    public static bool ShowExperiencePercent { get; set; }
    public static byte ShowTeraOpacityStripe { get; set; }
    public static int ShowTeraThicknessStripe { get; set; }
    public static byte ShowTeraOpacityBackground { get; set; }
    public static byte ShowEncounterOpacityStripe { get; set; }
    public static byte ShowEncounterOpacityBackground { get; set; }
    public static int ShowEncounterThicknessStripe { get; set; }

    /// <summary> Width of the generated Sprite image. </summary>
    public abstract int Width { get; }
    /// <summary> Height of the generated Sprite image. </summary>
    public abstract int Height { get; }

    /// <summary> Minimum amount of padding on the right side of the image when layering an item sprite. </summary>
    protected abstract int ItemShiftX { get; }
    /// <summary> Minimum amount of padding on the bottom side of the image when layering an item sprite. </summary>
    protected abstract int ItemShiftY { get; }
    /// <summary> Max width / height of an item image. </summary>
    protected abstract int ItemMaxSize { get; }

    protected abstract int EggItemShiftX { get; }
    protected abstract int EggItemShiftY { get; }

    public abstract bool HasFallbackMethod { get; }

    public abstract Bitmap Hover { get; }
    public abstract Bitmap View { get; }
    public abstract Bitmap Set { get; }
    public abstract Bitmap Delete { get; }
    public abstract Bitmap Transparent { get; }
    public abstract Bitmap Drag { get; }
    public abstract Bitmap UnknownItem { get; }
    public abstract Bitmap None { get; }
    public abstract Bitmap ItemTM { get; }
    public abstract Bitmap ItemTR { get; }

    private const double UnknownFormTransparency = 0.5;
    private const double ShinyTransparency = 0.7;
    private const double EggUnderLayerTransparency = 0.33;

    protected abstract string GetSpriteStringSpeciesOnly(ushort species);

    protected abstract string GetSpriteAll(ushort species, byte form, int gender, uint formarg, bool shiny, int generation);
    protected abstract string GetSpriteAllSecondary(ushort species, byte form, int gender, uint formarg, bool shiny, int generation);
    protected abstract string GetItemResourceName(int item);
    protected abstract Bitmap Unknown { get; }
    protected abstract Bitmap GetEggSprite(ushort species);
    public abstract Bitmap ShadowLugia { get; }

    /// <summary>
    /// Ensures all data is set up to generate sprites for the save file.
    /// </summary>
    /// <param name="sav"></param>
    public void Initialize(SaveFile sav)
    {
        if (sav.Generation != 3)
            return;

        // If the game is indeterminate, we might have different form sprites.
        // Currently, this only applies to Gen3's FireRed / LeafGreen
        Game = sav.Version;
        if (Game == GameVersion.FRLG)
            Game = ReferenceEquals(sav.Personal, PersonalTable.FR) ? GameVersion.FR : GameVersion.LG;
    }

    private GameVersion Game;

    private static byte GetDeoxysForm(GameVersion game) => game switch
    {
        GameVersion.FR => 1, // Attack
        GameVersion.LG => 2, // Defense
        GameVersion.E => 3, // Speed
        _ => 0,
    };

    private static byte GetArceusForm4(byte form) => form switch
    {
        > 9 => --form, // Realign to Gen5+ type indexes
        9 => byte.MaxValue, // Curse, make it show as unrecognized form since we don't have a sprite.
        _ => form,
    };

    /// <summary>
    /// Builds a new sprite image with the requested parameters.
    /// </summary>
    /// <param name="species">Entity Species ID</param>
    /// <param name="form">Entity Form index</param>
    /// <param name="gender">Entity gender</param>
    /// <param name="formarg">Entity <see cref="IFormArgument.FormArgument"/> raw value</param>
    /// <param name="heldItem">Entity held item ID</param>
    /// <param name="isEgg">Is currently in an egg</param>
    /// <param name="shiny">Is it shiny</param>
    /// <param name="generation"></param>
    /// <param name="tweak"></param>
    public Image GetSprite(ushort species, byte form, int gender, uint formarg, int heldItem, bool isEgg, Shiny shiny = Shiny.Never, int generation = -1, SpriteBuilderTweak tweak = SpriteBuilderTweak.None)
    {
        if (species == 0)
            return None;

        if (generation == 3 && species == (int)Species.Deoxys) // Depends on Gen3 save file version
            form = GetDeoxysForm(Game);
        else if (generation == 4 && species == (int)Species.Arceus) // Curse type's existence in Gen4
            form = GetArceusForm4(form);

        var baseImage = GetBaseImage(species, form, gender, formarg, shiny.IsShiny(), generation);
        return GetSprite(baseImage, species, heldItem, isEgg, shiny, generation, tweak);
    }

    public Image GetSprite(Image baseSprite, ushort species, int heldItem, bool isEgg, Shiny shiny, int generation = -1, SpriteBuilderTweak tweak = SpriteBuilderTweak.None)
    {
        if (isEgg)
            baseSprite = LayerOverImageEgg(baseSprite, species, heldItem != 0);
        if (heldItem > 0)
            baseSprite = LayerOverImageItem(baseSprite, heldItem, generation);
        if (shiny.IsShiny())
            baseSprite = LayerOverImageShiny(baseSprite, tweak, generation >= 8 && shiny == Shiny.AlwaysSquare ? Shiny.AlwaysSquare : Shiny.Always);
        return baseSprite;
    }

    private Image GetBaseImage(ushort species, byte form, int gender, uint formarg, bool shiny, int generation)
    {
        var img = FormInfo.IsTotemForm(species, form, generation)
            ? GetBaseImageTotem(species, form, gender, formarg, shiny, generation)
            : GetBaseImageDefault(species, form, gender, formarg, shiny, generation);
        return img ?? GetBaseImageFallback(species, form, gender, formarg, shiny, generation);
    }

    private Image? GetBaseImageTotem(ushort species, byte form, int gender, uint formarg, bool shiny, int generation)
    {
        var baseform = FormInfo.GetTotemBaseForm(species, form);
        var baseImage = GetBaseImageDefault(species, baseform, gender, formarg, shiny, generation);
        if (baseImage == null)
            return null;
        return ImageUtil.ToGrayscale(baseImage);
    }

    private Image? GetBaseImageDefault(ushort species, byte form, int gender, uint formarg, bool shiny, int generation)
    {
        var file = GetSpriteAll(species, form, gender, formarg, shiny, generation);
        var resource = (Image?)Resources.ResourceManager.GetObject(file);
        if (resource is null && HasFallbackMethod)
        {
            file = GetSpriteAllSecondary(species, form, gender, formarg, shiny, generation);
            resource = (Image?)Resources.ResourceManager.GetObject(file);
        }
        return resource;
    }

    private Image GetBaseImageFallback(ushort species, byte form, int gender, uint formarg, bool shiny, int generation)
    {
        if (shiny) // try again without shiny
        {
            var img = GetBaseImageDefault(species, form, gender, formarg, false, generation);
            if (img != null)
                return img;
        }

        // try again without form
        var baseImage = (Image?)Resources.ResourceManager.GetObject(GetSpriteStringSpeciesOnly(species));
        if (baseImage == null) // failed again
            return Unknown;
        return ImageUtil.LayerImage(baseImage, Unknown, 0, 0, UnknownFormTransparency);
    }

    private Image LayerOverImageItem(Image baseImage, int item, int generation)
    {
        var lump = HeldItemLumpUtil.GetIsLump(item, generation);
        var itemimg = lump switch
        {
            HeldItemLumpImage.TechnicalMachine => ItemTM,
            HeldItemLumpImage.TechnicalRecord => ItemTR,
            _ => (Image?)Resources.ResourceManager.GetObject(GetItemResourceName(item)) ?? UnknownItem,
        };

        // Redraw item in bottom right corner; since images are cropped, try to not have them at the edge
        int x = baseImage.Width - itemimg.Width - ((ItemMaxSize - itemimg.Width) / 4) - ItemShiftX;
        int y = baseImage.Height - itemimg.Height - ItemShiftY;
        return ImageUtil.LayerImage(baseImage, itemimg, x, y);
    }

    private static Image LayerOverImageShiny(Image baseImage, SpriteBuilderTweak tweak, Shiny shiny)
    {
        // Add shiny star to top left of image.
        Bitmap rare;
        if (shiny is Shiny.AlwaysSquare)
            rare = Resources.rare_icon_2;
        else if (tweak.HasFlagFast(SpriteBuilderTweak.BoxBackgroundRed))
            rare = Resources.rare_icon_alt;
        else
            rare = Resources.rare_icon;
        return ImageUtil.LayerImage(baseImage, rare, 0, 0, ShinyTransparency);
    }

    private Image LayerOverImageEgg(Image baseImage, ushort species, bool hasItem)
    {
        if (ShowEggSpriteAsItem && !hasItem)
            return LayerOverImageEggAsItem(baseImage, species);
        return LayerOverImageEggTransparentSpecies(baseImage, species);
    }

    private Image LayerOverImageEggTransparentSpecies(Image baseImage, ushort species)
    {
        // Partially transparent species.
        baseImage = ImageUtil.ChangeOpacity(baseImage, EggUnderLayerTransparency);
        // Add the egg layer over-top with full opacity.
        var egg = GetEggSprite(species);
        return ImageUtil.LayerImage(baseImage, egg, 0, 0);
    }

    private Image LayerOverImageEggAsItem(Image baseImage, ushort species)
    {
        var egg = GetEggSprite(species);
        return ImageUtil.LayerImage(baseImage, egg, EggItemShiftX, EggItemShiftY); // similar to held item, since they can't have any
    }

    public static void LoadSettings(ISpriteSettings sprite)
    {
        ShowEggSpriteAsItem = sprite.ShowEggSpriteAsHeldItem;
        ShowEncounterBall = sprite.ShowEncounterBall;

        ShowEncounterColor = sprite.ShowEncounterColor;
        ShowEncounterColorPKM = sprite.ShowEncounterColorPKM;
        ShowEncounterThicknessStripe = sprite.ShowEncounterThicknessStripe;
        ShowEncounterOpacityBackground = sprite.ShowEncounterOpacityBackground;
        ShowEncounterOpacityStripe = sprite.ShowEncounterOpacityStripe;
        ShowExperiencePercent = sprite.ShowExperiencePercent;

        ShowTeraType = sprite.ShowTeraType;
        ShowTeraThicknessStripe   = sprite.ShowTeraThicknessStripe;
        ShowTeraOpacityBackground = sprite.ShowTeraOpacityBackground;
        ShowTeraOpacityStripe     = sprite.ShowTeraOpacityStripe;
    }
}
