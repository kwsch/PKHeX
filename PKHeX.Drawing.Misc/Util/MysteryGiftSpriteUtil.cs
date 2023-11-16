using System.Drawing;
using PKHeX.Core;
using PKHeX.Drawing.Misc.Properties;
using PKHeX.Drawing.PokeSprite;

namespace PKHeX.Drawing.Misc;

public static class MysteryGiftSpriteUtil
{
    public static Bitmap Sprite(this MysteryGift gift) => GetSprite(gift);

    private static Bitmap GetSprite(MysteryGift gift)
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

    private static Bitmap GetBaseImage(MysteryGift gift)
    {
        if (gift is { IsEgg: true, Species: (int)Species.Manaphy }) // Manaphy Egg
            return SpriteUtil.GetMysteryGiftPreviewPoke(gift);
        if (gift.IsEntity)
            return SpriteUtil.GetMysteryGiftPreviewPoke(gift);

        if (gift.IsItem)
        {
            var item = (ushort)gift.ItemID;
            if (ItemStorage7USUM.GetCrystalHeld(item, out var value))
                item = value;
            return SpriteUtil.GetItemSprite(item) ?? Resources.Bag_Key;
        }
        return PokeSprite.Properties.Resources.b_unknown;
    }
}
