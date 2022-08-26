using System.Drawing;
using PKHeX.Core;
using PKHeX.Drawing.Misc.Properties;
using PKHeX.Drawing.PokeSprite;

namespace PKHeX.Drawing.Misc;

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
            return SpriteUtil.GetMysteryGiftPreviewPoke(gift);
        if (gift.IsEntity)
            return SpriteUtil.GetMysteryGiftPreviewPoke(gift);

        if (gift.IsItem)
        {
            var item = (ushort)gift.ItemID;
            if (Legal.ZCrystalDictionary.TryGetValue(item, out var value))
                item = value;
            return SpriteUtil.GetItemSprite(item) ?? Resources.Bag_Key;
        }
        return PokeSprite.Properties.Resources.b_unknown;
    }
}
