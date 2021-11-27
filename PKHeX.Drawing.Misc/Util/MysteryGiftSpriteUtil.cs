using System.Drawing;
using PKHeX.Core;
using PKHeX.Drawing.Misc.Properties;
using PKHeX.Drawing.PokeSprite;

namespace PKHeX.Drawing.Misc
{
    public static class MysteryGiftSpriteUtil
    {
        public static Image Sprite(this MysteryGift gift) => GetSprite(gift);

        private static Image GetSprite(MysteryGift gift)
        {
            if (gift.Empty)
                return SpriteUtil.Spriter.None;

            var img = GetBaseImage(gift);
            if (SpriteBuilder.ShowEncounterColor != SpriteBackgroundType.None)
                img = SpriteUtil.ApplyEncounterColor(gift, img, SpriteBuilder.ShowEncounterColor);
            if (gift.GiftUsed)
                img = ImageUtil.ChangeOpacity(img, 0.3);
            return img;
        }

        private static Image GetBaseImage(MysteryGift gift)
        {
            if (gift.IsEgg && gift.Species == (int)Species.Manaphy) // Manaphy Egg
                return SpriteUtil.GetSprite((int)Species.Manaphy, 0, 2, 0, 0, true, false, gift.Generation);
            if (gift.IsPokémon)
            {
                return SpriteUtil.GetMysteryGiftPreviewPoke(gift);
            }
            if (gift.IsItem)
            {
                int item = gift.ItemID;
                if (Legal.ZCrystalDictionary.TryGetValue(item, out int value))
                    item = value;
                return SpriteUtil.GetItemSprite(item) ?? Resources.Bag_Key;
            }
            return PokeSprite.Properties.Resources.b_unknown;
        }
    }
}
