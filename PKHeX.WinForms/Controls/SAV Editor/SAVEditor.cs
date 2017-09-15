using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.WinForms.Properties;

namespace PKHeX.WinForms.Controls
{
    public partial class SAVEditor : UserControl
    {
        public SaveFile SAV;
        public readonly PictureBox[] SlotPictureBoxes;
        public readonly SlotChangeManager M;
        public readonly Stack<SlotChange> UndoStack = new Stack<SlotChange>();
        public readonly Stack<SlotChange> RedoStack = new Stack<SlotChange>();
        public readonly ContextMenuSAV menu = new ContextMenuSAV();

        public bool HaX;
        public bool ModifyPKM;
        public ToolStripMenuItem Menu_Redo;
        public ToolStripMenuItem Menu_Undo;
        private bool FieldsLoaded;
        public PKMEditor PKME_Tabs;

        public bool FlagIllegal
        {
            get => Box.FlagIllegal;
            set
            {
                Box.FlagIllegal = value && !HaX;
                UpdateBoxViewers(all: true);
                ResetNonBoxSlots();
            }
        }

        public SAVEditor(SaveFile sav = null)
        {
            var z = Task.Run(() => sav ?? SaveUtil.GetBlankSAV(GameVersion.SN, "PKHeX"));
            InitializeComponent();
            var SupplementarySlots = new[]
            {
                ppkx1, ppkx2, ppkx3, ppkx4, ppkx5, ppkx6,
                bbpkx1, bbpkx2, bbpkx3, bbpkx4, bbpkx5, bbpkx6,

                dcpkx1, dcpkx2, gtspkx, fusedpkx, subepkx1, subepkx2, subepkx3,
            };
            GiveFeedback += (sender, e) => e.UseDefaultCursors = false;
            SAV = z.Result;
            Box.Setup(M = new SlotChangeManager(this));
            foreach (PictureBox pb in SupplementarySlots)
            {
                pb.MouseEnter += M.MouseEnter;
                pb.MouseLeave += M.MouseLeave;
                pb.MouseClick += M.MouseClick;
                pb.MouseMove += BoxSlot_MouseMove;
                pb.MouseDown += M.MouseDown;
                pb.MouseUp += M.MouseUp;

                pb.DragEnter += M.DragEnter;
                pb.DragDrop += BoxSlot_DragDrop;
                pb.QueryContinueDrag += M.QueryContinueDrag;
                pb.GiveFeedback += (sender, e) => e.UseDefaultCursors = false;
                pb.AllowDrop = true;
            }
            foreach (TabPage tab in tabBoxMulti.TabPages)
                tab.AllowDrop = true;

            Box.SlotPictureBoxes.AddRange(SupplementarySlots);
            SlotPictureBoxes = Box.SlotPictureBoxes.ToArray();
            foreach (PictureBox pb in SlotPictureBoxes)
                pb.ContextMenuStrip = menu.mnuVSD;

            GB_Daycare.Click += SwitchDaycare;
            FLP_SAVtools.Scroll += WinFormsUtil.PanelScroll;
        }

        /// <summary>Occurs when the Control Collection requests a cloning operation to the current box.</summary>
        public event EventHandler RequestCloneData;
        /// <summary>Occurs when the Control Collection requests a save to be reloaded.</summary>
        public event EventHandler RequestReloadSave;

        public Cursor GetDefaultCursor => DefaultCursor;
        private Image GetSprite(PKM p, int slot) => p.Sprite(SAV, Box.CurrentBox, slot, Box.FlagIllegal);

        public void EnableDragDrop(DragEventHandler enter, DragEventHandler drop)
        {
            AllowDrop = true;
            DragDrop += drop;
            foreach (TabPage tab in tabBoxMulti.TabPages)
            {
                tab.AllowDrop = true;
                tab.DragEnter += enter;
                tab.DragDrop += drop;
            }
            M.RequestExternalDragDrop += drop;
        }
        
        // Generic Subfunctions //
        public int GetPKMOffset(int slot, int box = -1)
        {
            if (slot < 30) // Box Slot
                return Box.GetOffset(slot, box);
            slot -= 30;
            if (slot < 6) // Party Slot
                return SAV.GetPartyOffset(slot);
            slot -= 6;
            if (slot < 6) // Battle Box Slot
                return SAV.BattleBox + slot * SAV.SIZE_STORED;
            slot -= 6;
            if (slot < 2) // Daycare
                return SAV.GetDaycareSlotOffset(SAV.DaycareIndex, slot);
            slot -= 2;
            if (slot == 0) // GTS
                return SAV.GTS;
            slot -= 1;
            if (slot == 0) // Fused
                return SAV.Fused;
            slot -= 1;
            if (slot < 3) // SUBE
                return SAV.SUBE + slot * (SAV.SIZE_STORED + 4);
            return -1;
        }
        public int GetSlot(object sender) => Array.IndexOf(SlotPictureBoxes, WinFormsUtil.GetUnderlyingControl(sender));
        public int SwapBoxesViewer(int viewBox)
        {
            int mainBox = Box.CurrentBox;
            Box.CurrentBox = viewBox;
            return mainBox;
        }
        public void UpdateBoxViewers(bool all = false)
        {
            foreach (var v in M.Boxes.Where(v => v.CurrentBox == Box.CurrentBox || all))
            {
                v.FlagIllegal = Box.FlagIllegal;
                v.ResetSlots();
            }
        }
        public void SetPKMBoxes()
        {
            if (SAV.HasBox)
                Box.ResetSlots();

            ResetNonBoxSlots();

            // Recoloring of a storage box slot (to not show for other storage boxes)
            if (M?.colorizedslot >= 30)
                SlotPictureBoxes[M.colorizedslot].BackgroundImage = M.colorizedcolor;
        }
        private void ResetNonBoxSlots()
        {
            ResetParty();
            ResetBattleBox();
            ResetDaycare();
            ResetMiscSlots();
        }
        private void ResetMiscSlots()
        {
            if (SAV.HasGTS) // GTS
                GetSlotFiller(SAV.GTS, SlotPictureBoxes[44]);
            
            if (SAV.HasFused) // Fused
                GetSlotFiller(SAV.Fused, SlotPictureBoxes[45]);

            if (SAV.HasSUBE) // SUBE
                for (int i = 0; i < 3; i++)
                {
                    int offset = SAV.SUBE + i * (SAV.SIZE_STORED + 4);
                    if (BitConverter.ToUInt64(SAV.Data, offset) != 0)
                        GetSlotFiller(offset, SlotPictureBoxes[46 + i]);
                    else SlotPictureBoxes[46 + i].Image = null;
                }
        }

        private void ResetParty()
        {
            if (!SAV.HasParty)
                return;

            for (int i = 0; i < 6; i++)
                GetSlotFiller(SAV.GetPartyOffset(i), SlotPictureBoxes[i + 30]);
        }
        private void ResetBattleBox()
        {
            if (!SAV.HasBattleBox)
                return;

            for (int i = 0; i < 6; i++)
                GetSlotFiller(SAV.BattleBox + SAV.SIZE_STORED * i, SlotPictureBoxes[i + 36]);
        }
        private void ResetDaycare()
        {
            if (!SAV.HasDaycare)
                return;

            Label[] L_SlotOccupied = {L_DC1, L_DC2};
            TextBox[] TB_SlotEXP = {TB_Daycare1XP, TB_Daycare2XP};
            Label[] L_SlotEXP = {L_XP1, L_XP2};

            for (int i = 0; i < 2; i++)
            {
                GetSlotFiller(SAV.GetDaycareSlotOffset(SAV.DaycareIndex, i), SlotPictureBoxes[i + 42]);
                uint? exp = SAV.GetDaycareEXP(SAV.DaycareIndex, i);
                TB_SlotEXP[i].Visible = L_SlotEXP[i].Visible = exp != null;
                TB_SlotEXP[i].Text = exp.ToString();
                bool? occ = SAV.IsDaycareOccupied(SAV.DaycareIndex, i);
                L_SlotOccupied[i].Visible = occ != null;
                if (occ == true) // If Occupied
                    L_SlotOccupied[i].Text = $"{i + 1}: ✓";
                else
                {
                    L_SlotOccupied[i].Text = $"{i + 1}: ✘";
                    SlotPictureBoxes[i + 42].Image = ImageUtil.ChangeOpacity(SlotPictureBoxes[i + 42].Image, 0.6);
                }
            }

            bool? egg = SAV.IsDaycareHasEgg(SAV.DaycareIndex);
            DayCare_HasEgg.Visible = egg != null;
            DayCare_HasEgg.Checked = egg == true;

            var seed = SAV.GetDaycareRNGSeed(SAV.DaycareIndex);
            if (seed != null)
            {
                TB_RNGSeed.MaxLength = SAV.DaycareSeedSize;
                TB_RNGSeed.Text = seed;
            }
            L_DaycareSeed.Visible = TB_RNGSeed.Visible = seed != null;
        }
        public void SetParty()
        {
            // Refresh slots
            if (SAV.HasParty)
            {
                PKM[] party = SAV.PartyData;
                for (int i = 0; i < party.Length; i++)
                    SlotPictureBoxes[i + 30].Image = GetSprite(party[i], i + 30);
                for (int i = party.Length; i < 6; i++)
                    SlotPictureBoxes[i + 30].Image = null;
            }
            if (SAV.HasBattleBox)
            {
                PKM[] battle = SAV.BattleBoxData;
                for (int i = 0; i < battle.Length; i++)
                    SlotPictureBoxes[i + 36].Image = GetSprite(battle[i], i + 36);
                for (int i = battle.Length; i < 6; i++)
                    SlotPictureBoxes[i + 36].Image = null;
            }
        }
        public void ClickUndo()
        {
            if (!UndoStack.Any())
                return;

            SlotChange change = UndoStack.Pop();
            if (change.Slot >= 30)
                return;

            RedoStack.Push(new SlotChange
            {
                Slot = change.Slot,
                Box = change.Box,
                Offset = change.Offset,
                PKM = SAV.GetStoredSlot(change.Offset)
            });
            UndoSlotChange(change);
            M.SetColor(change.Box, change.Slot, Resources.slotSet);
        }
        public void ClickRedo()
        {
            if (!RedoStack.Any())
                return;

            SlotChange change = RedoStack.Pop();
            if (change.Slot >= 30)
                return;

            UndoStack.Push(new SlotChange
            {
                Slot = change.Slot,
                Box = change.Box,
                Offset = change.Offset,
                PKM = SAV.GetStoredSlot(change.Offset)
            });
            UndoSlotChange(change);
            M.SetColor(change.Box, change.Slot, Resources.slotSet);
        }
        public void SetClonesToBox(PKM pk)
        {
            if (WinFormsUtil.Prompt(MessageBoxButtons.YesNo, $"Clone Pokemon from Editing Tabs to all slots in {Box.CurrentBoxName}?") != DialogResult.Yes)
                return;

            int slotSkipped = 0;
            for (int i = 0; i < SAV.BoxSlotCount; i++) // set to every slot in box
            {
                if (SAV.IsSlotLocked(Box.CurrentBox, i))
                { slotSkipped++; continue; }
                SAV.SetStoredSlot(pk, GetPKMOffset(i));
                Box.SetSlotFiller(pk, Box.CurrentBox, i);
            }

            if (slotSkipped > 0)
                WinFormsUtil.Alert($"Skipped {slotSkipped} locked slot{(slotSkipped > 1 ? "s" : "")}.");

            UpdateBoxViewers();
        }
        public void ClickSlot(object sender, EventArgs e)
        {
            switch (ModifierKeys)
            {
                case Keys.Control | Keys.Alt: ClickClone(sender, e); break;
                default:
                    menu.OmniClick(sender, e, ModifierKeys);
                    break;
            }
        }

        private void UndoSlotChange(SlotChange change)
        {
            int box = change.Box;
            int slot = change.Slot;
            int offset = change.Offset;
            PKM pk = change.PKM;

            if (Box.CurrentBox != change.Box)
                Box.CurrentBox = change.Box;
            SAV.SetStoredSlot(pk, offset);
            Box.SetSlotFiller(pk, box, slot);
            M?.SetColor(box, slot, Resources.slotSet);

            if (Menu_Undo != null)
                Menu_Undo.Enabled = UndoStack.Any();
            if (Menu_Redo != null)
                Menu_Redo.Enabled = RedoStack.Any();

            SystemSounds.Asterisk.Play();
        }
        private void GetSlotFiller(int offset, PictureBox pb)
        {
            if (SAV.GetData(offset, SAV.SIZE_STORED).SequenceEqual(new byte[SAV.SIZE_STORED]))
            {
                // 00s present in slot.
                pb.Image = null;
                pb.BackColor = Color.Transparent;
                pb.Visible = true;
                return;
            }
            PKM p = SAV.GetStoredSlot(offset);
            if (!p.Valid) // Invalid
            {
                // Bad Egg present in slot.
                pb.Image = null;
                pb.BackColor = Color.Red;
                pb.Visible = true;
                return;
            }

            int slot = GetSlot(pb);
            pb.Image = GetSprite(p, slot);
            pb.BackColor = Color.Transparent;
            pb.Visible = true;
        }

        private void ClickBoxSort(object sender, EventArgs e)
        {
            if (tabBoxMulti.SelectedTab != Tab_Box)
                return;
            if (!SAV.HasBox)
                return;

            string modified;
            bool all = false;
            if (ModifierKeys == (Keys.Alt | Keys.Shift) && DialogResult.Yes == WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "Clear ALL Boxes?!"))
            {
                if (SAV.IsAnySlotLockedInBox(0, SAV.BoxCount - 1))
                { WinFormsUtil.Alert("Battle Box slots prevent the clearing of all boxes."); return; }
                SAV.ClearBoxes();
                modified = "Boxes cleared!";
                all = true;
            }
            else if (ModifierKeys == Keys.Alt && DialogResult.Yes == WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "Clear Current Box?"))
            {
                if (SAV.IsAnySlotLockedInBox(Box.CurrentBox, Box.CurrentBox))
                { WinFormsUtil.Alert("Battle Box slots prevent the clearing of box."); return; }
                SAV.ClearBoxes(Box.CurrentBox, Box.CurrentBox + 1);
                modified = "Current Box cleared!";
            }
            else if (ModifierKeys == (Keys.Control | Keys.Shift) && DialogResult.Yes == WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "Sort ALL Boxes?!"))
            {
                if (SAV.IsAnySlotLockedInBox(0, SAV.BoxCount - 1))
                { WinFormsUtil.Alert("Battle Box slots prevent the sorting of all boxes."); return; }
                SAV.SortBoxes();
                modified = "Boxes sorted!";
                all = true;
            }
            else if (ModifierKeys == Keys.Control && DialogResult.Yes == WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "Sort Current Box?"))
            {
                if (SAV.IsAnySlotLockedInBox(Box.CurrentBox, Box.CurrentBox))
                { WinFormsUtil.Alert("Battle Box slots prevent the sorting of box."); return; }
                SAV.SortBoxes(Box.CurrentBox, Box.CurrentBox + 1);
                modified = "Current Box sorted!";
            }
            else
                return;

            SetPKMBoxes();
            UpdateBoxViewers(all);
            WinFormsUtil.Alert(modified);
        }
        private void ClickBoxDouble(object sender, MouseEventArgs e)
        {
            if (tabBoxMulti.SelectedTab == Tab_SAV)
            {
                RequestReloadSave?.Invoke(sender, e);
                return;
            }
            if (tabBoxMulti.SelectedTab != Tab_Box)
                return;
            if (!SAV.HasBox)
                return;
            if (ModifierKeys != Keys.Shift)
            {
                if (M.Boxes.Count > 1) // subview open
                { var z = M.Boxes[1].ParentForm; z.CenterToForm(ParentForm); z.BringToFront(); return; }
            }
            new SAV_BoxViewer(this, M).Show();
        }
        private void ClickClone(object sender, EventArgs e)
        {
            if (GetSlot(sender) > 30)
                return; // only perform action if cloning to boxes
            RequestCloneData?.Invoke(sender, e);
        }
        private void UpdateSaveSlot(object sender, EventArgs e)
        {
            if (SAV.Version != GameVersion.BATREV)
                return;
            ((SAV4BR)SAV).CurrentSlot = WinFormsUtil.GetIndex(CB_SaveSlot);
            SetPKMBoxes();
        }
        private void UpdateStringSeed(object sender, EventArgs e)
        {
            if (!FieldsLoaded)
                return;

            TextBox tb = sender as TextBox;
            if (tb == null)
                return;

            if (tb.Text.Length == 0)
            {
                tb.Undo();
                return;
            }

            string filterText = Util.GetOnlyHex(tb.Text);
            if (filterText.Length != tb.Text.Length)
            {
                WinFormsUtil.Alert("Expected HEX (0-9, A-F).", "Received: " + Environment.NewLine + tb.Text);
                tb.Undo();
                return;
            }

            // Write final value back to the save
            if (tb == TB_RNGSeed)
            {
                var value = filterText.PadLeft(SAV.DaycareSeedSize, '0');
                SAV.SetDaycareRNGSeed(SAV.DaycareIndex, value);
                SAV.Edited = true;
            }
            else if (tb == TB_GameSync)
            {
                var value = filterText.PadLeft(SAV.GameSyncIDSize, '0');
                SAV.GameSyncID = value;
                SAV.Edited = true;
            }
            else if (SAV.Generation >= 6)
            {
                var value = Convert.ToUInt64(filterText, 16);
                if (tb == TB_Secure1)
                    SAV.Secure1 = value;
                else if (tb == TB_Secure2)
                    SAV.Secure2 = value;
                SAV.Edited = true;
            }
        }
        private void SwitchDaycare(object sender, EventArgs e)
        {
            if (!SAV.HasTwoDaycares) return;
            if (DialogResult.Yes == WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "Would you like to switch the view to the other Daycare?",
                    $"Currently viewing daycare {SAV.DaycareIndex + 1}."))
                // If ORAS, alter the daycare offset via toggle.
                SAV.DaycareIndex ^= 1;

            // Refresh Boxes
            SetPKMBoxes();
        }
        private void B_SaveBoxBin_Click(object sender, EventArgs e)
        {
            if (!SAV.HasBox)
            { WinFormsUtil.Alert("Save file does not have boxes to dump!"); return; }
            Box.SaveBoxBinary();
        }

        // Subfunction Save Buttons //
        private void B_OpenWondercards_Click(object sender, EventArgs e) => new SAV_Wondercard(SAV, sender as MysteryGift).ShowDialog();
        private void B_OpenPokepuffs_Click(object sender, EventArgs e) => new SAV_Pokepuff(SAV).ShowDialog();
        private void B_OpenPokeBeans_Click(object sender, EventArgs e) => new SAV_Pokebean(SAV).ShowDialog();
        private void B_OpenItemPouch_Click(object sender, EventArgs e) => new SAV_Inventory(SAV).ShowDialog();
        private void B_OpenBerryField_Click(object sender, EventArgs e) => new SAV_BerryFieldXY(SAV).ShowDialog();
        private void B_OpenPokeblocks_Click(object sender, EventArgs e) => new SAV_PokeBlockORAS(SAV).ShowDialog();
        private void B_OpenEventFlags_Click(object sender, EventArgs e) => new SAV_EventFlags(SAV).ShowDialog();
        private void B_OpenSuperTraining_Click(object sender, EventArgs e) => new SAV_SuperTrain(SAV).ShowDialog();
        private void B_OpenSecretBase_Click(object sender, EventArgs e) => new SAV_SecretBase(SAV).ShowDialog();
        private void B_OpenZygardeCells_Click(object sender, EventArgs e) => new SAV_ZygardeCell(SAV).ShowDialog();
        private void B_LinkInfo_Click(object sender, EventArgs e) => new SAV_Link6(SAV).ShowDialog();
        private void B_OpenBoxLayout_Click(object sender, EventArgs e)
        {
            new SAV_BoxLayout(SAV, Box.CurrentBox).ShowDialog();
            Box.ResetBoxNames(); // fix box names
            Box.ResetSlots(); // refresh box background
            UpdateBoxViewers(all: true); // update subviewers
        }
        private void B_OpenTrainerInfo_Click(object sender, EventArgs e)
        {
            if (SAV.Generation < 6)
                new SAV_SimpleTrainer(SAV).ShowDialog();
            else if (SAV.Generation == 6)
                new SAV_Trainer(SAV).ShowDialog();
            else if (SAV.Generation == 7)
                new SAV_Trainer7(SAV).ShowDialog();
            // Refresh conversion info
            PKMConverter.UpdateConfig(SAV.SubRegion, SAV.Country, SAV.ConsoleRegion, SAV.OT, SAV.Gender, SAV.Language);
        }
        private void B_OpenOPowers_Click(object sender, EventArgs e)
        {
            if (SAV.Generation != 6)
                return;
            if (SAV.ORAS)
            {
                DialogResult dr = WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "No editing support for ORAS :(", "Max O-Powers with a working code?");
                if (dr != DialogResult.Yes) return;
                new byte[]
                {
                    0x00, 0x01, 0x01, 0x01, 0x01, 0x00, 0x01, 0x01, 0x01, 0x01, 0x00, 0x01, 0x01, 0x01, 0x01, 0x00,
                    0x01, 0x01, 0x01, 0x01, 0x00, 0x01, 0x01, 0x01, 0x01, 0x00, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01,
                    0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x00, 0x01, 0x01, 0x01, 0x01, 0x00, 0x01, 0x01, 0x01, 0x01,
                    0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01,
                    0x01, 0x00, 0x00, 0x00,
                }.CopyTo(SAV.Data, ((SAV6)SAV).OPower);
            }
            else if (SAV.XY)
                new SAV_OPower(SAV).ShowDialog();
        }
        private void B_OpenFriendSafari_Click(object sender, EventArgs e)
        {
            if (!SAV.XY)
                return;

            DialogResult dr = WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "No editing support for Friend Safari :(", "Unlock all 3 slots for each friend?");
            if (dr != DialogResult.Yes) return;

            // Unlock + reveal all safari slots if friend data is present
            for (int i = 1; i < 101; i++)
                if (SAV.Data[0x1E7FF + 0x15 * i] != 0x00) // no friend data == 0x00
                    SAV.Data[0x1E7FF + 0x15 * i] = 0x3D;
            SAV.Edited = true;
        }
        private void B_OpenPokedex_Click(object sender, EventArgs e)
        {
            switch (SAV.Generation)
            {
                case 1:
                case 2:
                    new SAV_SimplePokedex(SAV).ShowDialog(); break;
                case 3:
                    if (SAV.GameCube)
                        return;
                    new SAV_SimplePokedex(SAV).ShowDialog(); break;
                case 4:
                    if (SAV is SAV4BR)
                        return;
                    new SAV_Pokedex4(SAV).ShowDialog(); break;
                case 5:
                    new SAV_Pokedex5(SAV).ShowDialog(); break;
                case 6:
                    if (SAV.ORAS)
                        new SAV_PokedexORAS(SAV).ShowDialog();
                    else if (SAV.XY)
                        new SAV_PokedexXY(SAV).ShowDialog();
                    break;
                case 7:
                    if (SAV.SM)
                        new SAV_PokedexSM(SAV).ShowDialog();
                    break;
            }
        }
        private void B_OpenMiscEditor_Click(object sender, EventArgs e)
        {
            switch (SAV.Generation)
            {
                case 3:
                    new SAV_Misc3(SAV).ShowDialog(); break;
                case 4:
                    new SAV_Misc4(SAV).ShowDialog(); break;
                case 5:
                    new SAV_Misc5(SAV).ShowDialog(); break;
            }
        }
        private void B_OpenRTCEditor_Click(object sender, EventArgs e)
        {
            switch (SAV.Generation)
            {
                case 3:
                    new SAV_RTC3(SAV).ShowDialog(); break;
            }
        }
        private void B_OpenHoneyTreeEditor_Click(object sender, EventArgs e)
        {
            switch (SAV.Version)
            {
                case GameVersion.DP:
                case GameVersion.Pt:
                    new SAV_HoneyTree(SAV).ShowDialog(); break;
            }
        }
        private void B_OUTPasserby_Click(object sender, EventArgs e)
        {
            if (SAV.Generation != 6)
                return;
            if (DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "Export Passerby Info to Clipboard?"))
                return;
            string result = "PSS List" + Environment.NewLine;
            string[] headers = { "PSS Data - Friends", "PSS Data - Acquaintances", "PSS Data - Passerby", };
            int offset = ((SAV6)SAV).PSS;
            for (int g = 0; g < 3; g++)
            {
                result += Environment.NewLine
                          + "----" + Environment.NewLine
                          + headers[g] + Environment.NewLine
                          + "----" + Environment.NewLine;
                // uint count = BitConverter.ToUInt32(savefile, offset + 0x4E20);
                int r_offset = offset;

                for (int i = 0; i < 100; i++)
                {
                    ulong unkn = BitConverter.ToUInt64(SAV.Data, r_offset);
                    if (unkn == 0) break; // No data present here
                    if (i > 0) result += Environment.NewLine + Environment.NewLine;

                    string otname = Util.TrimFromZero(Encoding.Unicode.GetString(SAV.Data, r_offset + 8, 0x1A));
                    string message = Util.TrimFromZero(Encoding.Unicode.GetString(SAV.Data, r_offset + 0x22, 0x22));

                    // Trim terminated

                    // uint unk1 = BitConverter.ToUInt32(savefile, r_offset + 0x44);
                    // ulong unk2 = BitConverter.ToUInt64(savefile, r_offset + 0x48);
                    // uint unk3 = BitConverter.ToUInt32(savefile, r_offset + 0x50);
                    // uint unk4 = BitConverter.ToUInt16(savefile, r_offset + 0x54);
                    byte region = SAV.Data[r_offset + 0x56];
                    byte country = SAV.Data[r_offset + 0x57];
                    byte game = SAV.Data[r_offset + 0x5A];
                    // ulong outfit = BitConverter.ToUInt64(savefile, r_offset + 0x5C);
                    int favpkm = BitConverter.ToUInt16(SAV.Data, r_offset + 0x9C) & 0x7FF;
                    string gamename;
                    try { gamename = GameInfo.Strings.gamelist[game]; }
                    catch { gamename = "UNKNOWN GAME"; }

                    var cr = GameInfo.GetCountryRegionText(country, region, GameInfo.CurrentLanguage);
                    result +=
                        "OT: " + otname + Environment.NewLine +
                        "Message: " + message + Environment.NewLine +
                        "Game: " + gamename + Environment.NewLine +
                        "Country: " + cr.Item1 + Environment.NewLine +
                        "Region: " + cr.Item2 + Environment.NewLine +
                        "Favorite: " + GameInfo.Strings.specieslist[favpkm];

                    r_offset += 0xC8; // Advance to next entry
                }
                offset += 0x5000; // Advance to next block
            }
            Clipboard.SetText(result);
        }
        private void B_OUTHallofFame_Click(object sender, EventArgs e)
        {
            if (SAV.Generation == 6)
                new SAV_HallOfFame(SAV).ShowDialog();
            else if (SAV.SM)
                new SAV_HallOfFame7(SAV).ShowDialog();
        }
        private void B_CGearSkin_Click(object sender, EventArgs e)
        {
            if (SAV.Generation != 5)
                return; // can never be too safe
            new SAV_CGearSkin(SAV).ShowDialog();
        }
        private void B_JPEG_Click(object sender, EventArgs e)
        {
            byte[] jpeg = SAV.JPEGData;
            if (SAV.JPEGData == null)
            { WinFormsUtil.Alert("No PGL picture data found in the save file!"); return; }
            string filename = SAV.JPEGTitle + "'s picture";
            SaveFileDialog sfd = new SaveFileDialog { FileName = filename, Filter = "JPEG|*.jpeg" };
            if (sfd.ShowDialog() != DialogResult.OK) return;
            File.WriteAllBytes(sfd.FileName, jpeg);
        }
        private void ClickVerifyCHK(object sender, EventArgs e)
        {
            if (SAV.Edited) { WinFormsUtil.Alert("Save has been edited. Cannot integrity check."); return; }

            if (SAV.ChecksumsValid) { WinFormsUtil.Alert("Checksums are valid."); return; }
            if (DialogResult.Yes == WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "Export Checksum Info to Clipboard?"))
                Clipboard.SetText(SAV.ChecksumInfo);
        }
        
        // File I/O
        private bool? GetPKMSetOverride()
        {
            var yn = ModifyPKM ? "Yes" : "No";
            DialogResult noSet = WinFormsUtil.Prompt(MessageBoxButtons.YesNoCancel,
                "Loading overrides:",
                "Yes - Modify .pk* when set to SAV" + Environment.NewLine +
                "No - Don't modify .pk*" + Environment.NewLine +
                $"Cancel - Use current settings ({yn})");
            return noSet == DialogResult.Yes ? true : (noSet == DialogResult.No ? (bool?)false : null);
        }
        private static bool IsFolderPath(out string path)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            var result = fbd.ShowDialog() == DialogResult.OK;
            path = fbd.SelectedPath;
            return result;
        }

        public bool ExportSaveFile()
        {
            ValidateChildren();
            return WinFormsUtil.SaveSAVDialog(SAV, SAV.CurrentBox);
        }
        public bool ExportBackup()
        {
            if (!SAV.Exportable)
                return false;
            SaveFileDialog sfd = new SaveFileDialog
                { FileName = Util.CleanFileName(SAV.BAKName) };
            if (sfd.ShowDialog() != DialogResult.OK)
                return false;

            string path = sfd.FileName;
            File.WriteAllBytes(path, SAV.BAK);
            WinFormsUtil.Alert("Saved Backup of current SAV to:", path);

            return true;
        }
        public bool IsPCBoxBin(int length) => PKX.IsPKM(length / SAV.SlotCount) || PKX.IsPKM(length / SAV.BoxSlotCount);
        public bool OpenPCBoxBin(byte[] input, out string c)
        {
            if (SAV.PCBinary.Length == input.Length)
            {
                if (SAV.IsAnySlotLockedInBox(0, SAV.BoxCount - 1))
                { c = "Battle Box slots prevent loading of PC data."; return false; }
                if (!SAV.SetPCBinary(input))
                { c = "Current SAV Generation: " + SAV.Generation; return false; }

                c = "PC Binary loaded.";
            }
            else if (SAV.GetBoxBinary(Box.CurrentBox).Length == input.Length)
            {
                if (SAV.IsAnySlotLockedInBox(Box.CurrentBox, Box.CurrentBox))
                { c = "Battle Box slots in box prevent loading of box data."; return false; }
                if (!SAV.SetBoxBinary(input, Box.CurrentBox))
                { c = "Current SAV Generation: " + SAV.Generation; return false; }

                c = "Box Binary loaded.";
            }
            else
            {
                c = "Current SAV Generation: " + SAV.Generation;
                return false;
            }
            SetPKMBoxes();
            UpdateBoxViewers();
            return true;
        }
        public bool OpenBattleVideo(BattleVideo b, out string c)
        {
            if (b == null || SAV.Generation != b.Generation)
            {
                c = "Cannot load the Battle Video to a different generation save file.";
                return false;
            }

            var prompt = WinFormsUtil.Prompt(MessageBoxButtons.YesNo, $"Load Battle Video Pokémon data to {Box.CurrentBoxName}?", "The box will be overwritten.");
            if (prompt != DialogResult.Yes)
            {
                c = string.Empty;
                return false;
            }

            bool? noSetb = GetPKMSetOverride();
            PKM[] data = b.BattlePKMs;
            int offset = SAV.GetBoxOffset(Box.CurrentBox);
            int slotSkipped = 0;
            for (int i = 0; i < 24; i++)
            {
                if (SAV.IsSlotLocked(Box.CurrentBox, i))
                { slotSkipped++; continue; }
                SAV.SetStoredSlot(data[i], offset + i * SAV.SIZE_STORED, noSetb);
            }

            SetPKMBoxes();
            UpdateBoxViewers();

            c = slotSkipped > 0
                ? $"Skipped {slotSkipped} locked slot{(slotSkipped > 1 ? "s" : "")}."
                : "Battle Video data loaded to box slots.";

            return true;
        }
        public bool DumpBoxes(out string result, string path = null, bool separate = false)
        {
            if (path == null && !IsFolderPath(out path))
            {
                result = path;
                return false;
            }

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            SAV.DumpBoxes(path, out result, separate);
            return true;
        }
        public bool DumpBox(out string result, string path = null)
        {
            if (path == null && !IsFolderPath(out path))
            {
                result = path;
                return false;
            }

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            SAV.DumpBox(path, out result, Box.CurrentBox);
            return true;
        }
        public bool LoadBoxes(out string result, string path = null)
        {
            result = string.Empty;
            if (!SAV.HasBox)
                return false;

            if (path == null && !IsFolderPath(out path))
            {
                result = path;
                return false;
            }

            if (!Directory.Exists(path))
                return false;

            DialogResult dr = WinFormsUtil.Prompt(MessageBoxButtons.YesNoCancel, "Clear subsequent boxes when importing data?", "If you only want to overwrite for new data, press no.");
            if (dr == DialogResult.Cancel)
                return false;

            bool clearAll = dr == DialogResult.Yes;
            bool? noSetb = GetPKMSetOverride();

            SAV.LoadBoxes(path, out result, Box.CurrentBox, clearAll, noSetb);
            SetPKMBoxes();
            UpdateBoxViewers();
            return true;
        }

        public bool ToggleInterface()
        {
            FieldsLoaded = false;

            ToggleViewReset();
            ToggleViewSubEditors(SAV);

            int BoxTab = tabBoxMulti.TabPages.IndexOf(Tab_Box);
            int PartyTab = tabBoxMulti.TabPages.IndexOf(Tab_PartyBattle);

            bool WindowTranslationRequired = false;
            WindowTranslationRequired |= ToggleViewBox(SAV);
            WindowTranslationRequired |= ToggleViewParty(SAV, BoxTab);
            WindowTranslationRequired |= ToggleViewDaycare(SAV, BoxTab, PartyTab);

            ToggleViewMisc(SAV);

            FieldsLoaded = true;
            return WindowTranslationRequired;
        }
        private void ToggleViewReset()
        {
            // Close subforms that are save dependent
            foreach (var z in M.Boxes.Skip(1).ToArray())
                z.FindForm()?.Close();

            UndoStack.Clear();
            RedoStack.Clear();
            Box.M = M;
            Box.ResetBoxNames();   // Display the Box Names
            M.SetColor(-1, -1, null);
        }
        private bool ToggleViewBox(SaveFile sav)
        {
            if (!sav.HasBox)
            {
                if (tabBoxMulti.TabPages.Contains(Tab_Box))
                    tabBoxMulti.TabPages.Remove(Tab_Box);
                B_SaveBoxBin.Enabled = false;
                return false;
            }

            B_SaveBoxBin.Enabled = true;
            int startBox = !sav.Exportable ? 0 : sav.CurrentBox; // FF if BattleBox
            if (startBox > sav.BoxCount - 1) { tabBoxMulti.SelectedIndex = 1; Box.CurrentBox = 0; }
            else { tabBoxMulti.SelectedIndex = 0; Box.CurrentBox = startBox; }
            SetPKMBoxes();   // Reload all of the PKX Windows

            if (tabBoxMulti.TabPages.Contains(Tab_Box))
                return false;
            tabBoxMulti.TabPages.Insert(0, Tab_Box);
            return true;
        }
        private bool ToggleViewParty(SaveFile sav, int BoxTab)
        {
            if (!sav.HasParty)
            {
                if (tabBoxMulti.TabPages.Contains(Tab_PartyBattle))
                    tabBoxMulti.TabPages.Remove(Tab_PartyBattle);
                return false;
            }

            PB_Locked.Visible = sav.HasBattleBox && sav.BattleBoxLocked;
            if (tabBoxMulti.TabPages.Contains(Tab_PartyBattle))
                return false;

            int index = BoxTab;
            if (index < 0)
                index = -1;
            tabBoxMulti.TabPages.Insert(index + 1, Tab_PartyBattle);
            return true;
        }
        private bool ToggleViewDaycare(SaveFile sav, int BoxTab, int PartyTab)
        {

            if (!sav.HasDaycare)
            {
                if (tabBoxMulti.TabPages.Contains(Tab_Other))
                    tabBoxMulti.TabPages.Remove(Tab_Other);
                return false;
            }

            SlotPictureBoxes[43].Visible = sav.Generation >= 2; // Second daycare slot
            if (tabBoxMulti.TabPages.Contains(Tab_Other))
                return false;

            int index = PartyTab;
            if (index < 0)
                index = BoxTab;
            if (index < 0)
                index = -1;
            tabBoxMulti.TabPages.Insert(index + 1, Tab_Other);
            return true;
        }
        private void ToggleViewSubEditors(SaveFile sav)
        {
            if (sav.Exportable) // Actual save file
            {
                PAN_BattleBox.Visible = L_BattleBox.Visible = L_ReadOnlyPBB.Visible = sav.HasBattleBox;
                GB_Daycare.Visible = sav.HasDaycare;
                GB_Fused.Visible = sav.HasFused;
                GB_GTS.Visible = sav.HasGTS;
                B_OpenSecretBase.Enabled = sav.HasSecretBase;
                B_OpenPokepuffs.Enabled = sav.HasPuff;
                B_OpenPokeBeans.Enabled = sav.Generation == 7;
                B_OpenZygardeCells.Enabled = sav.Generation == 7;
                B_OUTPasserby.Enabled = sav.HasPSS;
                B_OpenBoxLayout.Enabled = sav.HasBoxWallpapers;
                B_OpenWondercards.Enabled = sav.HasWondercards;
                B_OpenSuperTraining.Enabled = sav.HasSuperTrain;
                B_OpenHallofFame.Enabled = sav.HasHoF;
                B_OpenOPowers.Enabled = sav.HasOPower;
                B_OpenPokedex.Enabled = sav.HasPokeDex;
                B_OpenBerryField.Enabled = sav.HasBerryField && sav.XY;
                B_OpenFriendSafari.Enabled = sav.XY;
                B_OpenPokeblocks.Enabled = sav.HasPokeBlock;
                B_JPEG.Visible = sav.HasJPEG;
                B_OpenEventFlags.Enabled = sav.HasEvents;
                B_OpenLinkInfo.Enabled = sav.HasLink;
                B_CGearSkin.Enabled = sav.Generation == 5;

                B_OpenTrainerInfo.Enabled = B_OpenItemPouch.Enabled = sav.HasParty; // Box RS
                B_OpenMiscEditor.Enabled = sav is SAV3 || sav is SAV4 || sav is SAV5;

                B_OpenHoneyTreeEditor.Enabled = sav.DP || sav.Pt;
                B_OpenRTCEditor.Enabled = sav.RS || sav.E;
                B_OpenUGSEditor.Enabled = sav.DP || sav.Pt;
            }
            GB_SAVtools.Visible = sav.Exportable && FLP_SAVtools.Controls.Cast<Control>().Any(c => c.Enabled);
            foreach (Control c in FLP_SAVtools.Controls.Cast<Control>())
                c.Visible = c.Enabled;
        }
        private void ToggleViewMisc(SaveFile sav)
        {
            // Generational Interface
            TB_Secure1.Visible = TB_Secure2.Visible = L_Secure1.Visible = L_Secure2.Visible = sav.Exportable && sav.Generation >= 6;
            TB_GameSync.Visible = L_GameSync.Visible = sav.Exportable && sav.Generation >= 6;
            GB_SUBE.Visible = SAV.HasSUBE;
            B_VerifyCHK.Enabled = SAV.Exportable;

            if (sav.Version == GameVersion.BATREV)
            {
                L_SaveSlot.Visible = CB_SaveSlot.Visible = true;
                CB_SaveSlot.DisplayMember = "Text"; CB_SaveSlot.ValueMember = "Value";
                CB_SaveSlot.DataSource = new BindingSource(((SAV4BR)sav).SaveSlots.Select(i => new ComboItem
                {
                    Text = ((SAV4BR)sav).SaveNames[i],
                    Value = i
                }).ToList(), null);
                CB_SaveSlot.SelectedValue = ((SAV4BR)sav).CurrentSlot;
            }
            else
                L_SaveSlot.Visible = CB_SaveSlot.Visible = false;

            switch (sav.Generation)
            {
                case 6:
                    TB_GameSync.Enabled = sav.GameSyncID != null;
                    TB_GameSync.MaxLength = sav.GameSyncIDSize;
                    TB_GameSync.Text = (sav.GameSyncID ?? 0.ToString()).PadLeft(sav.GameSyncIDSize, '0');
                    TB_Secure1.Text = sav.Secure1?.ToString("X16");
                    TB_Secure2.Text = sav.Secure2?.ToString("X16");
                    break;
                case 7:
                    TB_GameSync.Enabled = sav.GameSyncID != null;
                    TB_GameSync.MaxLength = sav.GameSyncIDSize;
                    TB_GameSync.Text = (sav.GameSyncID ?? 0.ToString()).PadLeft(sav.GameSyncIDSize, '0');
                    TB_Secure1.Text = sav.Secure1?.ToString("X16");
                    TB_Secure2.Text = sav.Secure2?.ToString("X16");
                    break;
            }
        }

        // DragDrop
        private void BoxSlot_MouseMove(object sender, MouseEventArgs e)
        {
            if (M == null || M.DragActive)
                return;

            // Abort if there is no Pokemon in the given slot.
            PictureBox pb = (PictureBox)sender;
            if (pb.Image == null)
                return;
            int slot = GetSlot(pb);
            int box = slot >= 30 ? -1 : Box.CurrentBox;
            if (SAV.IsSlotLocked(box, slot))
                return;

            bool encrypt = ModifierKeys == Keys.Control;
            M.HandleMovePKM(pb, slot, box, encrypt);
        }
        private void BoxSlot_DragDrop(object sender, DragEventArgs e)
        {
            if (M == null)
                return;

            PictureBox pb = (PictureBox)sender;
            int slot = GetSlot(pb);
            int box = slot >= 30 ? -1 : Box.CurrentBox;
            if (SAV.IsSlotLocked(box, slot) || slot >= 36)
            {
                SystemSounds.Asterisk.Play();
                e.Effect = DragDropEffects.Copy;
                M.DragInfo.Reset();
                return;
            }

            bool overwrite = ModifierKeys == Keys.Alt;
            bool clone = ModifierKeys == Keys.Control;
            M.DragInfo.Destination.Parent = FindForm();
            M.DragInfo.Destination.Slot = GetSlot(sender);
            M.DragInfo.Destination.Box = M.DragInfo.Destination.IsParty ? -1 : Box.CurrentBox;
            M.HandleDropPKM(sender, e, overwrite, clone);
        }
        private void MultiDragOver(object sender, DragEventArgs e)
        {
            // iterate over all tabs to see if a tab switch should occur when drag/dropping
            Point pt = tabBoxMulti.PointToClient(new Point(e.X, e.Y));
            for (int i = 0; i < tabBoxMulti.TabCount; i++)
            {
                if (tabBoxMulti.SelectedIndex == i || !tabBoxMulti.GetTabRect(i).Contains(pt))
                    continue;
                tabBoxMulti.SelectedIndex = i;
                return;
            }
        }
        private void ClickShowdownExportParty(object sender, EventArgs e)
        {
            try
            {
                var str = string.Join(Environment.NewLine + Environment.NewLine, SAV.PartyData.Select(pk => pk.ShowdownText));
                if (string.IsNullOrWhiteSpace(str)) return;
                Clipboard.SetText(str);
            }
            catch { }
            WinFormsUtil.Alert("Showdown Team (Party) set to Clipboard.");
        }
        private void ClickShowdownExportBattleBox(object sender, EventArgs e)
        {
            try
            {
                var str = string.Join(Environment.NewLine + Environment.NewLine, SAV.BattleBoxData.Select(pk => pk.ShowdownText));
                if (string.IsNullOrWhiteSpace(str)) return;
                Clipboard.SetText(str);
            }
            catch { }
            WinFormsUtil.Alert("Showdown Team (Battle Box) set to Clipboard.");
        }

        private void B_OpenUGSEditor_Click(object sender, EventArgs e)
        {
            switch (SAV.Version)
            {
                case GameVersion.DP:
                case GameVersion.Pt:
                    new SAV_Underground(SAV).ShowDialog(); break;
            }
        }
    }
}
