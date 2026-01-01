using System;
using System.Drawing;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Drawing.PokeSprite;

namespace PKHeX.WinForms.Controls;

/// <summary>
/// Utility logic for drawing individual Slot views that represent underlying <see cref="PKM"/> data.
/// </summary>
public static class SlotUtil
{
    /// <summary>
    /// Gets the background image for a slot based on the provided <see cref="type"/>.
    /// </summary>
    public static Bitmap GetTouchTypeBackground(SlotTouchType type) => type switch
    {
        SlotTouchType.None => SpriteUtil.Spriter.Transparent,
        SlotTouchType.Get => SpriteUtil.Spriter.View,
        SlotTouchType.Set => SpriteUtil.Spriter.Set,
        SlotTouchType.Delete => SpriteUtil.Spriter.Delete,
        SlotTouchType.Swap => SpriteUtil.Spriter.Set,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
    };

    /// <summary>
    /// Gets the type of action that should be performed for a Drag &amp; Drop request.
    /// </summary>
    public static DropModifier GetDropModifier() => Control.ModifierKeys switch
    {
        Keys.Shift => DropModifier.Clone,
        Keys.Alt => DropModifier.Overwrite,
        _ => DropModifier.None,
    };

    public static readonly Color GoodDataColor = Color.Transparent;
    public static readonly Color BadDataColor = Color.Red;

    /// <summary>
    /// Refreshes a <see cref="PictureBox"/> with the appropriate display content.
    /// </summary>
    public static void UpdateSlot(PictureBox pb, ISlotInfo info, PKM pk, SaveFile sav, bool flagIllegal, SlotTouchType t = SlotTouchType.None)
    {
        pb.BackgroundImage = GetTouchTypeBackground(t);
        if (pk.Species == 0) // Nothing in slot
        {
            pb.Image = null;
            pb.BackColor = GoodDataColor;
            pb.AccessibleDescription = null;
            return;
        }
        if (!pk.Valid) // Invalid
        {
            // Bad Egg present in slot.
            pb.Image = null;
            pb.BackColor = BadDataColor;
            pb.AccessibleDescription = null;
            return;
        }

        pb.Image = GetImage(info, pk, sav, flagIllegal);
        pb.BackColor = GoodDataColor;

        // Get an accessible description for the slot (for screen readers)
        var x = Main.Settings;
        var programLanguage = Language.GetLanguageValue(x.Startup.Language);
        var cfg = x.BattleTemplate;
        var settings = cfg.Hover.GetSettings(programLanguage, pk.Context);
        pb.AccessibleDescription = ShowdownParsing.GetLocalizedPreviewText(pk, settings);
    }

    private static Bitmap GetImage(ISlotInfo info, PKM pk, SaveFile sav, bool flagIllegal) => info switch
    {
        SlotInfoBox b => pk.Sprite(sav, b.Box, b.Slot, flagIllegal, b.Type),
        SlotInfoParty ps => pk.Sprite(sav, -1, ps.Slot, flagIllegal, ps.Type),
        _ => pk.Sprite(sav, -1, -1, flagIllegal, info.Type),
    };
}
