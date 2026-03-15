using PKHeX.Core;
using PKHeX.Drawing.Avalonia;
using PKHeX.Drawing.PokeSprite.Avalonia;
using SkiaSharp;

namespace PKHeX.Drawing.Misc.Avalonia;

public static class MysteryGiftSpriteUtil
{
    public static SKBitmap Sprite(this MysteryGift gift) => GetSprite(gift);

    private static SKBitmap GetSprite(MysteryGift gift)
    {
        if (gift.IsEmpty)
            return SpriteUtil.Spriter.None;

        var img = GetBaseImage(gift);
        if (SpriteBuilder.ShowEncounterColor != SpriteBackgroundType.None)
            SpriteUtil.ApplyEncounterColor(gift, img, SpriteBuilder.ShowEncounterColor);
        if (gift.GiftUsed)
            img.ChangeOpacity(0.3);
        return img;
    }

    private static SKBitmap GetBaseImage(MysteryGift gift)
    {
        if (gift is { IsEgg: true, Species: (int)Species.Manaphy })
            return SpriteUtil.GetMysteryGiftPreviewPoke(gift);
        if (gift.IsEntity)
            return SpriteUtil.GetMysteryGiftPreviewPoke(gift);

        if (gift.IsItem)
        {
            var item = (ushort)gift.ItemID;
            if (ItemStorage7USUM.GetCrystalHeld(item, out var value))
                item = value;
            return SpriteUtil.GetItemSprite(item) ?? ResourceLoader.Get("Bag_Key");
        }
        return PokeSprite.Avalonia.ResourceLoader.Get("b_unknown");
    }
}
