using System.Drawing;
using PKHeX.Core;
using PKHeX.WinForms.Properties;

namespace PKHeX.WinForms
{
    public class SpriteBuilder : ISpriteBuilder<Image>
    {
        public static bool ShowEggSpriteAsItem { get; set; } = true;

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

            if (isEgg)
                baseImage = LayerOverImageEgg(baseImage, species);
            if (isShiny)
                baseImage = LayerOverImageShiny(baseImage, isBoxBGRed);
            if (heldItem > 0)
                baseImage = LayerOverImageItem(baseImage, heldItem, generation);
            return baseImage;
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
            var file = PKX.GetResourceStringSprite(species, baseform, gender, generation, shiny);
            var baseImage = (Image)Resources.ResourceManager.GetObject(file);
            return ImageUtil.ToGrayscale(baseImage);
        }

        private static Image GetBaseImageDefault(int species, int form, int gender, bool shiny, int generation)
        {
            var file = PKX.GetResourceStringSprite(species, form, gender, generation, shiny);
            return (Image)Resources.ResourceManager.GetObject(file);
        }

        private static Image GetBaseImageFallback(int species, int form, int gender, bool shiny, int generation)
        {
            Image baseImage;
            if (shiny) // try again without shiny
            {
                var file = PKX.GetResourceStringSprite(species, form, gender, generation);
                baseImage = (Image)Resources.ResourceManager.GetObject(file);
                if (baseImage != null)
                    return baseImage;
            }

            // try again without form
            baseImage = (Image)Resources.ResourceManager.GetObject($"_{species}");
            if (baseImage == null) // failed again
                return Resources.unknown;
            return ImageUtil.LayerImage(baseImage, Resources.unknown, 0, 0, .5);
        }

        private static Image LayerOverImageItem(Image baseImage, int item, int generation)
        {
            Image itemimg = (Image)Resources.ResourceManager.GetObject($"item_{item}") ?? Resources.helditem;
            if (generation >= 2 && generation <= 4 && 328 <= item && item <= 419) // gen2/3/4 TM
                itemimg = Resources.item_tm;

            // Redraw
            int x = 22 + ((15 - itemimg.Width) / 2);
            if (x + itemimg.Width > baseImage.Width)
                x = baseImage.Width - itemimg.Width;
            int y = 15 + (15 - itemimg.Height);
            return ImageUtil.LayerImage(baseImage, itemimg, x, y);
        }

        private static Image LayerOverImageShiny(Image baseImage, bool isBoxBGRed)
        {
            // Add shiny star to top left of image.
            var rare = isBoxBGRed ? Resources.rare_icon_alt : Resources.rare_icon;
            return ImageUtil.LayerImage(baseImage, rare, 0, 0, 0.7);
        }

        private static Image LayerOverImageEgg(Image baseImage, int species)
        {
            if (ShowEggSpriteAsItem)
                return LayerOverImageEggAsItem(baseImage, species);
            return LayerOverImageEggTransparentSpecies(baseImage, species);
        }

        private static Image GetEggSprite(int species) => species == 490 ? Resources._490_e : Resources.egg;

        private const double EggUnderLayerTransparency = 0.33;
        private const int EggOverLayerAsItemShiftX = 9;
        private const int EggOverLayerAsItemShiftY = 2;

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
            return ImageUtil.LayerImage(baseImage, egg, EggOverLayerAsItemShiftX, EggOverLayerAsItemShiftY); // similar to held item, since they can't have any
        }
    }
}
