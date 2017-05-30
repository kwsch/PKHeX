using System;
using System.ComponentModel;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.WinForms.Properties;

namespace PKHeX.WinForms.Controls
{
    public partial class ContextMenuSAV : UserControl
    {
        public ContextMenuSAV()
        {
            InitializeComponent();
        }

        public void OmniClick(object sender, EventArgs e, Keys z)
        {
            switch (z)
            {
                case Keys.Control: ClickView(sender, e); break;
                case Keys.Shift: ClickSet(sender, e); break;
                case Keys.Alt: ClickDelete(sender, e); break;
            }
        }
        private void ClickView(object sender, EventArgs e)
        {
            SlotChangeManager m = GetSenderInfo(ref sender, out SlotChange info);
            if (m == null)
                return;
            if ((sender as PictureBox)?.Image == null)
            { System.Media.SystemSounds.Asterisk.Play(); return; }

            m.SE.PKME_Tabs.populateFields(m.GetPKM(info), false);
            m.SetColor(info.Box, info.Slot, Resources.slotView);
        }
        private void ClickSet(object sender, EventArgs e)
        {
            SlotChangeManager m = GetSenderInfo(ref sender, out SlotChange info);
            if (m == null)
                return;

            var editor = m.SE.PKME_Tabs;
            var sav = m.SE.SAV;
            if (info.Slot == 30 && editor.IsEmptyOrEgg)
            { WinFormsUtil.Alert("Can't have empty/egg first info.Slot."); return; }
            if (m.SE.SAV.getIsSlotLocked(info.Box, info.Slot))
            { WinFormsUtil.Alert("Can't set to locked info.Slot."); return; }

            PKM pk = editor.preparePKM();

            string[] errata = sav.IsPKMCompatible(pk);
            if (errata.Length > 0 && DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, string.Join(Environment.NewLine, errata), "Continue?"))
                return;

            if (info.Slot >= 30)
                info.Box = -1;
            if (info.Slot >= 30 && info.Slot < 36) // Party
            {
                // If info.Slot isn't overwriting existing PKM, make it write to the lowest empty PKM info.Slot
                if (sav.PartyCount < info.Slot + 1 - 30)
                {
                    info.Slot = sav.PartyCount + 30;
                    info.Offset = m.SE.getPKXOffset(info.Slot);
                }
                m.SetPKM(pk, info, true, Resources.slotSet);
            }
            else if (info.Slot < 30 || m.SE.HaX)
            {
                if (info.Slot < 30)
                {
                    m.SE.UndoStack.Push(new SlotChange
                    {
                        Box = info.Box,
                        Slot = info.Slot,
                        Offset = info.Offset,
                        PKM = sav.getStoredSlot(info.Offset)
                    });
                    m.SE.Menu_Undo.Enabled = true;
                }

                m.SetPKM(pk, info, true, Resources.slotSet);
            }

            editor.lastData = pk.Data;
            m.SE.RedoStack.Clear(); m.SE.Menu_Redo.Enabled = false;
        }
        private void ClickDelete(object sender, EventArgs e)
        {
            SlotChangeManager m = GetSenderInfo(ref sender, out SlotChange info);
            if (m == null)
                return;

            if ((sender as PictureBox)?.Image == null)
            { System.Media.SystemSounds.Asterisk.Play(); return; }

            var sav = m.SE.SAV;
            if (info.Slot == 30 && sav.PartyCount == 1 && !m.SE.HaX)
            { WinFormsUtil.Alert("Can't delete first slot."); return; }
            if (sav.getIsSlotLocked(info.Box, info.Slot))
            { WinFormsUtil.Alert("Can't delete locked slot."); return; }

            if (info.Slot >= 30 && info.Slot < 36) // Party
            {
                m.SetPKM(sav.BlankPKM, info, true, Resources.slotDel);
                return;
            }
            if (info.Slot < 30 || m.SE.HaX)
            {
                if (info.Slot < 30)
                {
                    m.SE.UndoStack.Push(new SlotChange
                    {
                        Box = info.Box,
                        Slot = info.Slot,
                        Offset = info.Offset,
                        PKM = sav.getStoredSlot(info.Offset)
                    });
                    m.SE.Menu_Undo.Enabled = true;
                }
                m.SetPKM(sav.BlankPKM, info, true, Resources.slotDel);
            }
            else return;

            m.SE.RedoStack.Clear(); m.SE.Menu_Redo.Enabled = false;
        }
        private void ClickShowLegality(object sender, EventArgs e)
        {
            SlotChangeManager m = GetSenderInfo(ref sender, out SlotChange info);
            if (m == null)
                return;

            bool verbose = ModifierKeys == Keys.Control;
            var pk = m.GetPKM(info);
            LegalityAnalysis la = new LegalityAnalysis(pk);
            var report = la.Report(verbose);
            if (verbose)
            {
                var dr = WinFormsUtil.Prompt(MessageBoxButtons.YesNo, report, "Copy report to Clipboard?");
                if (dr == DialogResult.Yes)
                    Clipboard.SetText(report);
            }
            else
                WinFormsUtil.Alert(report);
        }
        private void MenuOpening(object sender, CancelEventArgs e)
        {
            var items = ((ContextMenuStrip)sender).Items;

            object ctrl = ((ContextMenuStrip)sender).SourceControl;
            GetSenderInfo(ref ctrl, out SlotChange info);
            bool SlotFull = (ctrl as PictureBox)?.Image != null;
            bool Editable = info.Slot < 36;
            bool legality = ModifierKeys == Keys.Control;
            ToggleItem(items, mnuSet, Editable);
            ToggleItem(items, mnuDelete, Editable && SlotFull);
            ToggleItem(items, mnuLegality, legality && SlotFull);
            ToggleItem(items, mnuView, SlotFull || !Editable, true);

            if (items.Count == 0)
                e.Cancel = true;
        }

        private static SlotChangeManager GetSenderInfo(ref object sender, out SlotChange loc)
        {
            loc = new SlotChange();
            var ctrl = WinFormsUtil.GetUnderlyingControl(sender);
            var obj = ctrl.Parent.Parent;
            if (obj is BoxEditor b)
            {
                loc.Box = b.CurrentBox;
                loc.Slot = b.getSlot(sender);
                loc.Offset = b.getOffset(loc.Slot, loc.Box);
                loc.Parent = b.FindForm();
                sender = ctrl;
                return b.M;
            }
            obj = obj.Parent.Parent;
            if (obj is SAVEditor z)
            {
                loc.Box = z.Box.CurrentBox;
                loc.Slot = z.getSlot(sender);
                loc.Offset = z.getPKXOffset(loc.Slot, loc.Box);
                loc.Parent = z.FindForm();
                sender = ctrl;
                return z.M;
            }
            return null;
        }
        private static void ToggleItem(ToolStripItemCollection items, ToolStripItem item, bool visible, bool first = false)
        {
            if (visible)
            {
                if (first)
                    items.Insert(0, item);
                else
                    items.Add(item);
            }
            else if (items.Contains(item))
                items.Remove(item);
        }
    }
}
