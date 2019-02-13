using System;
using System.ComponentModel;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.WinForms.Properties;

using static PKHeX.Core.MessageStrings;

namespace PKHeX.WinForms.Controls
{
    public partial class ContextMenuSAV : UserControl
    {
        public ContextMenuSAV() => InitializeComponent();

        public event LegalityRequest RequestEditorLegality;
        public delegate void LegalityRequest(object sender, EventArgs e, PKM pkm);

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
            var m = GetSenderInfo(ref sender, out SlotChange info);
            if (m == null)
                return;
            if ((sender as PictureBox)?.Image == null)
            { System.Media.SystemSounds.Asterisk.Play(); return; }

            m.HoverCancel();

            m.SE.PKME_Tabs.PopulateFields(m.GetPKM(info), false, true);
            m.SetColor(info.Box, info.Slot, Resources.slotView);
        }

        private void ClickSet(object sender, EventArgs e)
        {
            var m = GetSenderInfo(ref sender, out SlotChange info);
            if (m == null)
                return;

            var editor = m.SE.PKME_Tabs;
            var sav = m.SE.SAV;
            if (info.IsParty && editor.IsEmptyOrEgg && sav.IsPartyAllEggs(info.Slot) && !m.SE.HaX)
            { WinFormsUtil.Alert(MsgSaveSlotEmpty); return; }
            if (m.SE.SAV.IsSlotLocked(info.Box, info.Slot))
            { WinFormsUtil.Alert(MsgSaveSlotLocked); return; }

            if (!editor.EditsComplete)
                return;

            PKM pk = editor.PreparePKM();

            var errata = sav.IsPKMCompatible(pk);
            if (errata.Count > 0 && DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, string.Join(Environment.NewLine, errata), MsgContinue))
                return;

            m.HoverCancel();

            if (info.Type == StorageSlotType.Party) // Party
            {
                // If info.Slot isn't overwriting existing PKM, make it write to the lowest empty PKM info.Slot
                if (sav.PartyCount < info.Slot + 1)
                {
                    var pb = (PictureBox)WinFormsUtil.GetUnderlyingControl(sender);
                    var view = WinFormsUtil.FindFirstControlOfType<ISlotViewer<PictureBox>>(pb);
                    info = view.GetSlotData(view.SlotPictureBoxes[sav.PartyCount]);
                }
                m.SetPKM(pk, info, true, Resources.slotSet);
            }
            else if (info.Type == StorageSlotType.Box || m.SE.HaX)
            {
                if (info.Type == StorageSlotType.Box)
                {
                    m.SE.UndoStack.Push(new SlotChange(info, sav));
                    m.SE.Menu_Undo.Enabled = true;
                }

                m.SetPKM(pk, info, true, Resources.slotSet);
            }
            else
            {
                return;
            }

            editor.LastData = pk.Data;
            m.SE.RedoStack.Clear(); m.SE.Menu_Redo.Enabled = false;
        }

        private void ClickDelete(object sender, EventArgs e)
        {
            var m = GetSenderInfo(ref sender, out SlotChange info);
            if (m == null)
                return;

            if ((sender as PictureBox)?.Image == null)
            { System.Media.SystemSounds.Asterisk.Play(); return; }

            var sav = m.SE.SAV;
            if (info.IsParty && sav.IsPartyAllEggs(info.Slot) && !m.SE.HaX)
            { WinFormsUtil.Alert(MsgSaveSlotEmpty); return; }
            if (sav.IsSlotLocked(info.Box, info.Slot))
            { WinFormsUtil.Alert(MsgSaveSlotLocked); return; }

            m.HoverCancel();

            if (info.Type == StorageSlotType.Party) // Party
            {
                m.SetPKM(sav.BlankPKM, info, true, Resources.slotDel);
                return;
            }
            if (info.Type == StorageSlotType.Box || m.SE.HaX)
            {
                if (info.Type == StorageSlotType.Box)
                {
                    m.SE.UndoStack.Push(new SlotChange(info, sav));
                    m.SE.Menu_Undo.Enabled = true;
                }
                m.SetPKM(sav.BlankPKM, info, true, Resources.slotDel);
            }
            else
            {
                return;
            }

            m.SE.RedoStack.Clear(); m.SE.Menu_Redo.Enabled = false;
        }

        private void ClickShowLegality(object sender, EventArgs e)
        {
            var m = GetSenderInfo(ref sender, out SlotChange info);
            if (m == null)
                return;

            var pk = m.GetPKM(info);
            RequestEditorLegality?.Invoke(sender, e, pk);
        }

        private void MenuOpening(object sender, CancelEventArgs e)
        {
            var items = ((ContextMenuStrip)sender).Items;

            object ctrl = ((ContextMenuStrip)sender).SourceControl;
            GetSenderInfo(ref ctrl, out SlotChange info);
            bool SlotFull = (ctrl as PictureBox)?.Image != null;
            bool Editable = info.Editable;
            bool legality = ModifierKeys == Keys.Control;
            ToggleItem(items, mnuSet, Editable);
            ToggleItem(items, mnuDelete, Editable && SlotFull);
            ToggleItem(items, mnuLegality, legality && SlotFull && RequestEditorLegality != null);
            ToggleItem(items, mnuView, SlotFull || !Editable, true);

            if (items.Count == 0)
                e.Cancel = true;
        }

        private static SlotChangeManager GetSenderInfo(ref object sender, out SlotChange loc)
        {
            sender = WinFormsUtil.GetUnderlyingControl(sender);
            var pb = (PictureBox)sender;
            var view = WinFormsUtil.FindFirstControlOfType<ISlotViewer<PictureBox>>(pb);
            loc = view.GetSlotData(pb);
            return view.M;
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
            {
                items.Remove(item);
            }
        }
    }
}
