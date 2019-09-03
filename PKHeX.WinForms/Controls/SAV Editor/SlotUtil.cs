using System;
using System.Drawing;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.WinForms.Properties;

namespace PKHeX.WinForms.Controls
{
    /// <summary>
    /// Utility logic for drawing individual Slot views that represent underlying <see cref="PKM"/> data.
    /// </summary>
    public static class SlotUtil
    {
        /// <summary>
        /// Gets the background image for a slot based on the provided <see cref="type"/>.
        /// </summary>
        public static Image GetTouchTypeBackground(SlotTouchType type)
        {
            switch (type)
            {
                case SlotTouchType.None: return Resources.slotTrans;
                case SlotTouchType.Get: return Resources.slotView;
                case SlotTouchType.Set: return Resources.slotSet;
                case SlotTouchType.Delete: return Resources.slotDel;
                case SlotTouchType.Swap: return Resources.slotSet;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        /// <summary>
        /// Gets the type of action that should be performed for a Drag &amp; Drop request.
        /// </summary>
        public static DropModifier GetDropModifier()
        {
            switch (Control.ModifierKeys)
            {
                case Keys.Shift: return DropModifier.Clone;
                case Keys.Alt: return DropModifier.Overwrite;
                default: return DropModifier.None;
            }
        }

        /// <summary>
        /// Refreshes a <see cref="PictureBox"/> with the appropriate display content.
        /// </summary>
        public static void UpdateSlot(PictureBox pb, ISlotInfo c, PKM p, SaveFile s, bool flagIllegal, SlotTouchType t = SlotTouchType.None)
        {
            pb.BackgroundImage = GetTouchTypeBackground(t);
            if (p.Species == 0) // Nothing in slot
            {
                pb.Image = null;
                pb.BackColor = Color.Transparent;
                return;
            }
            if (!p.Valid) // Invalid
            {
                // Bad Egg present in slot.
                pb.Image = null;
                pb.BackColor = Color.Red;
                return;
            }

            var img = c is SlotInfoBox b
                ? p.Sprite(s, b.Box, b.Slot, flagIllegal)
                : c is SlotInfoParty ps
                    ? p.Sprite(s, -1, ps.Slot, flagIllegal)
                    : p.Sprite(s, -1, -1, flagIllegal);

            pb.BackColor = Color.Transparent;
            pb.Image = img;
        }
    }
}