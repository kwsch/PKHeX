using System.Drawing;
using PKHeX.Core;
using PKHeX.WinForms.Properties;

namespace PKHeX.WinForms
{
    public class SpriteBuilder : ISpriteBuilder<Image>
    {
        public static bool ShowEggSpriteAsItem { get; set; } = true;

        private const int ItemShiftX = 22;
        private const int ItemShiftY = 15;
        private const int ItemMaxSize = 15;
        private const int EggItemShiftX = 9;
        private const int EggItemShiftY = 2;
        private const double UnknownFormTransparency = 0.5;
        private const double ShinyTransparency = 0.7;
        private const double EggUnderLayerTransparency = 0.33;

        public void Initialize(SaveFile sav)
        {
            if (sav.Generation != 3)
                return;

            Game = sav.Version;
            if (Game == GameVersion.FRLG)
                Game = sav.Personal == PersonalTable.FR ? GameVersion.FR : GameVersion.LG;
        }

        private GameVersion Game;

        private static int GetDeoxysForm(GameVersion game)
        {
            switch (game)
            {
                default: return 0;
                case GameVersion.FR: return 1; // Attack
                case GameVersion.LG: return 2; // Defense
                case GameVersion.E: return 3; // Speed
            }
        }

        public Image GetSprite(int species, int form, int gender, int heldItem, bool isEgg, bool isShiny, int generation = -1, bool isBoxBGRed = false)
        {
            if (species == 0)
                return Resources._0;

            if (generation == 3 && species == 386) // Deoxys, special consideration for Gen3 save files
                form = GetDeoxysForm(Game);

            var baseImage = GetBaseImage(species, form, gender, isShiny, generation);

            return GetSprite(baseImage, species, heldItem, isEgg, isShiny, generation, isBoxBGRed);
        }

        public Image GetSprite(Image baseSprite, int species, int heldItem, bool isEgg, bool isShiny, int generation = -1, bool isBoxBGRed = false)
        {
            if (isEgg)
                baseSprite = LayerOverImageEgg(baseSprite, species, heldItem != 0);
            if (isShiny)
                baseSprite = LayerOverImageShiny(baseSprite, isBoxBGRed);
            if (heldItem > 0)
                baseSprite = LayerOverImageItem(baseSprite, heldItem, generation);
            return baseSprite;
        }

        private static Image GetBaseImage(int species, int form, int gender, bool shiny, int generation)
        {
            var img = FormConverter.IsTotemForm(species, form)
                        ? GetBaseImageTotem(species, form, gender, shiny, generation)
                        : GetBaseImageDefault(species, form, gender, shiny, generation);
            return img ?? GetBaseImageFallback(species, form, gender, shiny, generation);
        }

        private static Image GetBaseImageTotem(int species, int form, int gender, bool shiny, int generation)
        {
            var baseform = FormConverter.GetTotemBaseForm(species, form);
            var baseImage = GetBaseImageDefault(species, baseform, gender, shiny, generation);
            return ImageUtil.ToGrayscale(baseImage);
        }

        private static Image GetBaseImageDefault(int species, int form, int gender, bool shiny, int generation)
        {
            var file = PKX.GetResourceStringSprite(species, form, gender, generation, shiny);
            return (Image)Resources.ResourceManager.GetObject(file);
        }

        private static Image GetBaseImageFallback(int species, int form, int gender, bool shiny, int generation)
        {
            if (shiny) // try again without shiny
            {
                var img = GetBaseImageDefault(species, form, gender, false, generation);
                if (img != null)
                    return img;
            }

            // try again without form
            var baseImage = (Image)Resources.ResourceManager.GetObject($"_{species}");
            if (baseImage == null) // failed again
                return Resources.unknown;
            return ImageUtil.LayerImage(baseImage, Resources.unknown, 0, 0, UnknownFormTransparency);
        }

        private static Image LayerOverImageItem(Image baseImage, int item, int generation)
        {
            Image itemimg = (Image)Resources.ResourceManager.GetObject($"item_{item}") ?? Resources.helditem;
            if (generation >= 2 && generation <= 4 && 328 <= item && item <= 419) // gen2/3/4 TM
                itemimg = Resources.item_tm;

            // Redraw item in bottom right corner; since images are cropped, try to not have them at the edge
            int x = ItemShiftX + ((ItemMaxSize - itemimg.Width) / 2);
            if (x + itemimg.Width > baseImage.Width)
                x = baseImage.Width - itemimg.Width;
            int y = ItemShiftY + (ItemMaxSize - itemimg.Height);
            return ImageUtil.LayerImage(baseImage, itemimg, x, y);
        }

        private static Image LayerOverImageShiny(Image baseImage, bool isBoxBGRed)
        {
            // Add shiny star to top left of image.
            var rare = isBoxBGRed ? Resources.rare_icon_alt : Resources.rare_icon;
            return ImageUtil.LayerImage(baseImage, rare, 0, 0, ShinyTransparency);
        }

        private static Image LayerOverImageEgg(Image baseImage, int species, bool hasItem)
        {
            if (ShowEggSpriteAsItem && !hasItem)
                return LayerOverImageEggAsItem(baseImage, species);
            return LayerOverImageEggTransparentSpecies(baseImage, species);
        }

        private static Image GetEggSprite(int species) => species == 490 ? Resources._490_e : Resources.egg;

        private static Image LayerOverImageEggTransparentSpecies(Image baseImage, int species)
        {
            // Partially transparent species.
            baseImage = ImageUtil.ChangeOpacity(baseImage, EggUnderLayerTransparency);
            // Add the egg layer over-top with full opacity.
            var egg = GetEggSprite(species);
            return ImageUtil.LayerImage(baseImage, egg, 0, 0);
        }

        private static Image LayerOverImageEggAsItem(Image baseImage, int species)
        {
            var egg = GetEggSprite(species);
            return ImageUtil.LayerImage(baseImage, egg, EggItemShiftX, EggItemShiftY); // similar to held item, since they can't have any
        }
    }
}
