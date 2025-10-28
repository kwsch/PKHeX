using System.Drawing;
using PKHeX.Core;
using PKHeX.Drawing.PokeSprite.Properties;

namespace PKHeX.Drawing.PokeSprite;

/// <summary>
/// 56 high, 68 wide sprite builder using Circular Sprites
/// </summary>
public sealed class SpriteBuilder5668c : SpriteBuilder
{
    public override int Height => 56;
    public override int Width => 68;

    protected override int ItemShiftX => 2;
    protected override int ItemShiftY => 2;
    protected override int ItemMaxSize => 32;
    protected override int EggItemShiftX => 18;
    protected override int EggItemShiftY => 1;
    public override bool HasFallbackMethod => true;

    protected override string GetSpriteStringSpeciesOnly(ushort species) => 'c' + $"_{species}";
    protected override string GetSpriteAll(ushort species, byte form, byte gender, uint formarg, bool shiny, EntityContext context) => 'c' + SpriteName.GetResourceStringSprite(species, form, gender, formarg, context, shiny);
    protected override string GetSpriteAllSecondary(ushort species, byte form, byte gender, uint formarg, bool shiny, EntityContext context) => 'b' + SpriteName.GetResourceStringSprite(species, form, gender, formarg, context, shiny);
    protected override string GetItemResourceName(int item) => 'b' + $"item_{item}";
    protected override Bitmap Unknown => Resources.b_unknown;
    protected override Bitmap GetEggSprite(ushort species) => species == (int)Species.Manaphy ? Resources.b_490_e : Resources.b_egg;

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
