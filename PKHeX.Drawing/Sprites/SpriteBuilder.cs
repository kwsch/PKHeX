using System.Drawing;
using PKHeX.Core;
using PKHeX.Drawing.Properties;

namespace PKHeX.Drawing
{
    public abstract class SpriteBuilder : ISpriteBuilder<Image>
    {
        public static bool ShowEggSpriteAsItem { get; set; } = true;

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

        protected abstract string GetSpriteStringSpeciesOnly(int species);

        protected abstract string GetSpriteAll(int species, int form, int gender, uint formarg, bool shiny, int generation);
        protected abstract string GetItemResourceName(int item);
        protected abstract Bitmap Unknown { get; }
        protected abstract Bitmap GetEggSprite(int species);
        public abstract Bitmap ShadowLugia { get; }

        public void Initialize(SaveFile sav)
        {
            if (sav.Generation != 3)
                return;

            Game = sav.Version;
            if (Game == GameVersion.FRLG)
                Game = sav.Personal == PersonalTable.FR ? GameVersion.FR : GameVersion.LG;
        }

        private GameVersion Game;

        private static int GetDeoxysForm(GameVersion game) => game switch
        {
            GameVersion.FR => 1, // Attack
            GameVersion.LG => 2, // Defense
            GameVersion.E => 3, // Speed
            _ => 0
        };

        public Image GetSprite(int species, int form, int gender, uint formarg, int heldItem, bool isEgg, bool isShiny, int generation = -1, bool isBoxBGRed = false, bool isAltShiny = false)
        {
            if (species == 0)
                return None;

            if (generation == 3 && species == (int)Species.Deoxys) // Deoxys, special consideration for Gen3 save files
                form = GetDeoxysForm(Game);

            var baseImage = GetBaseImage(species, form, gender, formarg, isShiny, generation);
            return GetSprite(baseImage, species, heldItem, isEgg, isShiny, generation, isBoxBGRed, isAltShiny);
        }

        public Image GetSprite(Image baseSprite, int species, int heldItem, bool isEgg, bool isShiny, int generation = -1, bool isBoxBGRed = false, bool isAltShiny = false)
        {
            if (isEgg)
                baseSprite = LayerOverImageEgg(baseSprite, species, heldItem != 0);
            if (heldItem > 0)
                baseSprite = LayerOverImageItem(baseSprite, heldItem, generation);
            if (isShiny)
                baseSprite = LayerOverImageShiny(baseSprite, isBoxBGRed, generation >= 8 && isAltShiny);
            return baseSprite;
        }

        private Image GetBaseImage(int species, int form, int gender, uint formarg, bool shiny, int generation)
        {
            var img = FormInfo.IsTotemForm(species, form, generation)
                        ? GetBaseImageTotem(species, form, gender, formarg, shiny, generation)
                        : GetBaseImageDefault(species, form, gender, formarg, shiny, generation);
            return img ?? GetBaseImageFallback(species, form, gender, formarg, shiny, generation);
        }

        private Image? GetBaseImageTotem(int species, int form, int gender, uint formarg, bool shiny, int generation)
        {
            var baseform = FormInfo.GetTotemBaseForm(species, form);
            var baseImage = GetBaseImageDefault(species, baseform, gender, formarg, shiny, generation);
            if (baseImage == null)
                return null;
            return ImageUtil.ToGrayscale(baseImage);
        }

        private Image? GetBaseImageDefault(int species, int form, int gender, uint formarg, bool shiny, int generation)
        {
            var file = GetSpriteAll(species, form, gender, formarg, shiny, generation);
            return (Image?)Resources.ResourceManager.GetObject(file);
        }

        private Image GetBaseImageFallback(int species, int form, int gender, uint formarg, bool shiny, int generation)
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
            Image itemimg = generation switch
            {
                <= 4 when item is >=  328 and <=  419 => ItemTM, // gen2/3/4 TM
                >= 8 when item is >= 1130 and <= 1229 => ItemTR, // Gen8 TR
                _ => (Image?)Resources.ResourceManager.GetObject(GetItemResourceName(item)) ?? UnknownItem,
            };

            // Redraw item in bottom right corner; since images are cropped, try to not have them at the edge
            int x = baseImage.Width - itemimg.Width - ((ItemMaxSize - itemimg.Width) / 4) - ItemShiftX;
            int y = baseImage.Height - itemimg.Height - ItemShiftY;
            return ImageUtil.LayerImage(baseImage, itemimg, x, y);
        }

        private static Image LayerOverImageShiny(Image baseImage, bool isBoxBGRed, bool altShiny)
        {
            // Add shiny star to top left of image.
            var rare = isBoxBGRed ? Resources.rare_icon_alt : Resources.rare_icon;
            if (altShiny)
                rare = Resources.rare_icon_2;
            return ImageUtil.LayerImage(baseImage, rare, 0, 0, ShinyTransparency);
        }

        private Image LayerOverImageEgg(Image baseImage, int species, bool hasItem)
        {
            if (ShowEggSpriteAsItem && !hasItem)
                return LayerOverImageEggAsItem(baseImage, species);
            return LayerOverImageEggTransparentSpecies(baseImage, species);
        }

        private Image LayerOverImageEggTransparentSpecies(Image baseImage, int species)
        {
            // Partially transparent species.
            baseImage = ImageUtil.ChangeOpacity(baseImage, EggUnderLayerTransparency);
            // Add the egg layer over-top with full opacity.
            var egg = GetEggSprite(species);
            return ImageUtil.LayerImage(baseImage, egg, 0, 0);
        }

        private Image LayerOverImageEggAsItem(Image baseImage, int species)
        {
            var egg = GetEggSprite(species);
            return ImageUtil.LayerImage(baseImage, egg, EggItemShiftX, EggItemShiftY); // similar to held item, since they can't have any
        }
    }

    /// <summary>
    /// 56 high, 68 wide sprite builder
    /// </summary>
    public sealed class SpriteBuilder5668 : SpriteBuilder
    {
        public override int Height => 56;
        public override int Width => 68;

        protected override int ItemShiftX => 2;
        protected override int ItemShiftY => 2;
        protected override int ItemMaxSize => 32;
        protected override int EggItemShiftX => 18;
        protected override int EggItemShiftY => 1;

        protected override string GetSpriteStringSpeciesOnly(int species) => 'b' + $"_{species}";
        protected override string GetSpriteAll(int species, int form, int gender, uint formarg, bool shiny, int generation) => 'b' + SpriteName.GetResourceStringSprite(species, form, gender, formarg, generation, shiny);
        protected override string GetItemResourceName(int item) => 'b' + $"item_{item}";
        protected override Bitmap Unknown => Resources.b_unknown;
        protected override Bitmap GetEggSprite(int species) => species == (int)Species.Manaphy ? Resources.b_490_e : Resources.b_egg;

        public override Bitmap Hover => Resources.slotHover68;
        public override Bitmap View => Resources.slotView68;
        public override Bitmap Set => Resources.slotSet68;
        public override Bitmap Delete => Resources.slotDel68;
        public override Bitmap Transparent => Resources.slotTrans68;
        public override Bitmap Drag => Resources.slotDrag68;
        public override Bitmap UnknownItem => Resources.bitem_unk;
        public override Bitmap None => Resources.b_0;
        public override Bitmap ItemTM => Resources.bitem_tm;
        public override Bitmap ItemTR => Resources.bitem_tr;
        public override Bitmap ShadowLugia => Resources.b_249x;
    }
}
