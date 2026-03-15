using PKHeX.Core;
using SkiaSharp;

namespace PKHeX.Drawing.PokeSprite.Avalonia;

/// <summary>
/// 56 high, 68 wide sprite builder using Artwork Sprites
/// </summary>
public sealed class SpriteBuilder5668a : SpriteBuilder
{
    public override int Height => 56;
    public override int Width => 68;

    protected override int ItemShiftX => 2;
    protected override int ItemShiftY => 2;
    protected override int ItemMaxSize => 32;
    protected override int EggItemShiftX => 18;
    protected override int EggItemShiftY => 1;
    public override bool HasFallbackMethod => true;

    protected override string GetSpriteStringSpeciesOnly(ushort species) => 'a' + $"_{species}";
    protected override string GetSpriteAll(ushort species, byte form, byte gender, uint formarg, bool shiny, EntityContext context) => 'a' + SpriteName.GetResourceStringSprite(species, form, gender, formarg, context, shiny);
    protected override string GetSpriteAllSecondary(ushort species, byte form, byte gender, uint formarg, bool shiny, EntityContext context) => 'b' + SpriteName.GetResourceStringSprite(species, form, gender, formarg, context, shiny);
    protected override string GetItemResourceName(int item) => 'a' + $"item_{item}";
    protected override SKBitmap Unknown => ResourceLoader.Get("b_unknown");
    protected override SKBitmap GetEggSprite(ushort species) => species == (int)Species.Manaphy ? ResourceLoader.Get("a_490_e") : ResourceLoader.Get("a_egg");

    public override SKBitmap Hover => ResourceLoader.Get("slotHover68");
    public override SKBitmap View => ResourceLoader.Get("slotView68");
    public override SKBitmap Set => ResourceLoader.Get("slotSet68");
    public override SKBitmap Delete => ResourceLoader.Get("slotDel68");
    public override SKBitmap Transparent => ResourceLoader.Get("slotTrans68");
    public override SKBitmap Drag => ResourceLoader.Get("slotDrag68");
    public override SKBitmap UnknownItem => ResourceLoader.Get("bitem_unk");
    public override SKBitmap None => ResourceLoader.Get("b_0");
    public override SKBitmap ItemTM => ResourceLoader.Get("aitem_tm");
    public override SKBitmap ItemTR => ResourceLoader.Get("bitem_tr");
    public override SKBitmap ShadowLugia => ResourceLoader.Get("b_249x");
}
