using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.WinForms.Properties;
using static PKHeX.Core.MessageStrings;

namespace PKHeX.WinForms.Controls
{
    public partial class SAVEditor : UserControl, ISlotViewer<PictureBox>, ISaveFileProvider
    {
        public SaveFile SAV { get; set; }
        public int CurrentBox => Box.CurrentBox;
        public SlotChangeManager M { get; }
        public readonly Stack<SlotChange> UndoStack = new Stack<SlotChange>();
        public readonly Stack<SlotChange> RedoStack = new Stack<SlotChange>();
        public readonly ContextMenuSAV menu = new ContextMenuSAV();
        public readonly BoxMenuStrip SortMenu;

        public bool  HaX;
        public bool ModifyPKM { private get; set; }
        private bool _hideSecret;
        public bool HideSecretDetails { private get => _hideSecret; set { if (SAV != null) ToggleSecrets(SAV, _hideSecret = value); } }
        public ToolStripMenuItem Menu_Redo;
        public ToolStripMenuItem Menu_Undo;
        private bool FieldsLoaded;
        public PKMEditor PKME_Tabs;

        public SlotChange GetSlotData(PictureBox view)
        {
            int slot = GetSlot(view);
            var type = Extensions.GetMiscSlotType(slot);
            return new SlotChange
            {
                Slot = GetSlot(view),
                Box = ViewIndex,
                Offset = GetSlotOffset(slot),
                Parent = FindForm(),
                Type = type.GetMiscSlotType(),
                Editable = type.IsEditable(),
                IsPartyFormat = type.IsParty(SAV.Generation)
            };
        }

        public IList<PictureBox> SlotPictureBoxes { get; }
        public int GetSlot(PictureBox sender) => SlotPictureBoxes.IndexOf(WinFormsUtil.GetUnderlyingControl(sender) as PictureBox);
        public int ViewIndex { get; set; } = -1;

        public bool FlagIllegal
        {
            get => Box.FlagIllegal;
            set
            {
                Box.FlagIllegal = value && !HaX;
                if (SAV != null)
                    ReloadSlots();
            }
        }

        public void ReloadSlots()
        {
            UpdateBoxViewers(all: true);
            ResetNonBoxSlots();
        }

        public SAVEditor()
        {
            InitializeComponent();

            L_SlotOccupied = new[] { L_DC1, L_DC2 };
            TB_SlotEXP = new[] { TB_Daycare1XP, TB_Daycare2XP };
            L_SlotEXP = new[] { L_XP1, L_XP2 };
            SlotPictureBoxes = new[]
            {
                ppkx1, ppkx2, ppkx3, ppkx4, ppkx5, ppkx6,
                bbpkx1, bbpkx2, bbpkx3, bbpkx4, bbpkx5, bbpkx6,

                dcpkx1, dcpkx2
            };
            Tab_Box.ContextMenuStrip = SortMenu = new BoxMenuStrip(this);
            Box.Setup(M = new SlotChangeManager(this));
            SL_Extra.M = M;

            M.OtherSlots.Add(this);
            SL_Extra.ViewIndex = -2;
            M.OtherSlots.Add(SL_Extra);
            InitializeEvents();
        }

        private void InitializeEvents()
        {
            foreach (PictureBox pb in Box.SlotPictureBoxes)
                pb.ContextMenuStrip = menu.mnuVSD;
            foreach (PictureBox pb in SlotPictureBoxes)
            {
                InitializeDragDrop(pb);
                pb.ContextMenuStrip = menu.mnuVSD;
            }

            GiveFeedback += (sender, e) => e.UseDefaultCursors = false;
            Tab_Box.MouseWheel += (s, e) =>
            {
                if (menu.mnuVSD.Visible)
                    return;
                if (e.Delta > 1)
                    Box.MoveLeft();
                else
                    Box.MoveRight();
            };

            GB_Daycare.Click += SwitchDaycare;
            FLP_SAVtools.Scroll += WinFormsUtil.PanelScroll;
            SortMenu.Opening += (s, x) => x.Cancel = !tabBoxMulti.GetTabRect(tabBoxMulti.SelectedIndex).Contains(PointToClient(MousePosition));
        }

        private void InitializeDragDrop(Control pb)
        {
            pb.MouseEnter += M.MouseEnter;
            pb.MouseLeave += M.MouseLeave;
            pb.MouseClick += M.MouseClick;
            pb.MouseMove += M.MouseMove;
            pb.MouseDown += M.MouseDown;
            pb.MouseUp += M.MouseUp;

            pb.DragEnter += M.DragEnter;
            pb.DragDrop += M.DragDrop;
            pb.QueryContinueDrag += M.QueryContinueDrag;
            pb.GiveFeedback += (sender, e) => e.UseDefaultCursors = false;
            pb.AllowDrop = true;
            pb.ContextMenuStrip = menu.mnuVSD;
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
        public int GetSlotOffset(int slot)
        {
            if (slot < (int)SlotIndex.BattleBox) // Party Slot
                return SAV.GetPartyOffset(slot - (int)SlotIndex.Party);
            if (slot < (int)SlotIndex.Daycare) // Battle Box Slot
                return SAV.BattleBox + ((slot - (int)SlotIndex.BattleBox) * SAV.SIZE_STORED);
            return SAV.GetDaycareSlotOffset(SAV.DaycareIndex, slot - (int)SlotIndex.Daycare);
        }

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
            if (M?.ColorizedSlot >= (int)SlotIndex.Party && M.ColorizedSlot < SlotPictureBoxes.Count)
                SlotPictureBoxes[M.ColorizedSlot].BackgroundImage = M.ColorizedColor;
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
            var slots = SL_Extra.SlotPictureBoxes;
            for (int i = 0; i < SL_Extra.SlotCount; i++)
                GetSlotFiller(SL_Extra.GetSlotOffset(i), slots[i]);
        }

        private void ResetParty()
        {
            if (!SAV.HasParty)
                return;

            for (int i = 0; i < 6; i++)
                GetSlotFiller(SAV.GetPartyOffset(i), SlotPictureBoxes[i + (int)SlotIndex.Party]);
        }

        private void ResetBattleBox()
        {
            if (!SAV.HasBattleBox)
                return;

            for (int i = 0; i < 6; i++)
                GetSlotFiller(SAV.BattleBox + (SAV.SIZE_STORED * i), SlotPictureBoxes[i + (int)SlotIndex.BattleBox]);
        }

        private readonly Label[] L_SlotOccupied;
        private readonly TextBox[] TB_SlotEXP;
        private readonly Label[] L_SlotEXP;

        private void ResetDaycare()
        {
            if (!SAV.HasDaycare)
                return;

            for (int i = 0; i < 2; i++)
            {
                var pb = SlotPictureBoxes[i + (int)SlotIndex.Daycare];
                GetSlotFiller(SAV.GetDaycareSlotOffset(SAV.DaycareIndex, i), pb);
                uint? exp = SAV.GetDaycareEXP(SAV.DaycareIndex, i);
                TB_SlotEXP[i].Visible = L_SlotEXP[i].Visible = exp != null;
                TB_SlotEXP[i].Text = exp.ToString();
                bool? occ = SAV.IsDaycareOccupied(SAV.DaycareIndex, i);
                L_SlotOccupied[i].Visible = occ != null;
                if (occ == true) // If Occupied
                {
                    L_SlotOccupied[i].Text = $"{i + 1}: ✓";
                }
                else
                {
                    L_SlotOccupied[i].Text = $"{i + 1}: ✘";
                    pb.Image = ImageUtil.ChangeOpacity(pb.Image, 0.6);
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

        public void SetParty() => ResetParty();

        public void ClickUndo()
        {
            if (UndoStack.Count == 0)
                return;

            SlotChange change = UndoStack.Pop();
            if (change.Box < 0)
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
            if (RedoStack.Count == 0)
                return;

            SlotChange change = RedoStack.Pop();
            if (change.Box < 0)
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
            if (WinFormsUtil.Prompt(MessageBoxButtons.YesNo, string.Format(MsgSaveBoxCloneFromTabs, Box.CurrentBoxName)) != DialogResult.Yes)
                return;

            int slotSkipped = SetClonesToCurrentBox(pk, Box.CurrentBox);
            if (slotSkipped > 0)
                WinFormsUtil.Alert(string.Format(MsgSaveBoxImportSkippedLocked, slotSkipped));

            UpdateBoxViewers();
        }

        private int SetClonesToCurrentBox(PKM pk, int box)
        {
            int slotSkipped = 0;
            for (int i = 0; i < SAV.BoxSlotCount; i++) // set to every slot in box
            {
                if (SAV.IsSlotOverwriteProtected(box, i))
                { slotSkipped++; continue; }
                SAV.SetStoredSlot(pk, Box.GetSlotOffset(box, i));
                Box.SetSlotFiller(pk, box, i);
            }

            return slotSkipped;
        }

        public void ClickSlot(object sender, EventArgs e)
        {
            switch (ModifierKeys)
            {
                case Keys.Control | Keys.Alt: ClickClone(sender, e); break;
                default: // forward to contextmenu for default behavior
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
                Menu_Undo.Enabled = UndoStack.Count > 0;
            if (Menu_Redo != null)
                Menu_Redo.Enabled = RedoStack.Count > 0;

            SystemSounds.Asterisk.Play();
        }

        private void GetSlotFiller(int offset, PictureBox pb)
        {
            if (!SAV.IsPKMPresent(offset))
            {
                pb.Image = null;
                pb.BackColor = Color.Transparent;
                return;
            }
            PKM p = SAV.GetStoredSlot(offset);
            if (!p.Valid) // Invalid
            {
                // Bad Egg present in slot.
                pb.Image = null;
                pb.BackColor = Color.Red;
                return;
            }

            int slot = GetSlot(pb);
            pb.Image = GetSprite(p, slot < 6 ? -1 : slot);
            pb.BackColor = Color.Transparent;
        }

        private void ClickBoxSort(object sender, MouseEventArgs e)
        {
            if (tabBoxMulti.SelectedTab != Tab_Box)
                return;
            if (!tabBoxMulti.GetTabRect(tabBoxMulti.SelectedIndex).Contains(PointToClient(MousePosition)))
                return;
            if (!e.Button.HasFlag(MouseButtons.Right))
            {
                if (ModifierKeys.HasFlag(Keys.Alt))
                    SortMenu.Clear();
                else if (ModifierKeys.HasFlag(Keys.Control))
                    SortMenu.Sort();
                return;
            }
            var pt = Tab_Box.PointToScreen(new Point(0, 0));
            SortMenu.Show(pt);
        }

        public void FinishBoxManipulation(string message, bool all, int count)
        {
            SetPKMBoxes();
            UpdateBoxViewers(all);
            if (message != null)
                WinFormsUtil.Alert(message + $" ({count})");
            else
                SystemSounds.Asterisk.Play();
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
            if (ModifierKeys == Keys.Shift)
            {
                if (M.Boxes.Count > 1) // subview open
                {
                    // close all subviews
                    for (int i = 1; i < M.Boxes.Count; i++)
                        M.Boxes[i].ParentForm?.Close();
                }
                new SAV_BoxList(this, M).Show();
                return;
            }
            if (M.Boxes.Count > 1) // subview open
            { var z = M.Boxes[1].ParentForm; z.CenterToForm(ParentForm); z.BringToFront(); return; }
            new SAV_BoxViewer(this, M).Show();
        }

        private void ClickClone(object sender, EventArgs e)
        {
            if (GetSlot((PictureBox)sender) >= 0)
                return; // only perform action if cloning to boxes
            RequestCloneData?.Invoke(sender, e);
        }

        private void UpdateSaveSlot(object sender, EventArgs e)
        {
            if (SAV.Version != GameVersion.BATREV)
                return;
            ((SAV4BR)SAV).CurrentSlot = WinFormsUtil.GetIndex(CB_SaveSlot);
            Box.ResetBoxNames(); // fix box names
            SetPKMBoxes();
            UpdateBoxViewers(true);
        }

        private void UpdateStringSeed(object sender, EventArgs e)
        {
            if (!FieldsLoaded)
                return;

            if (!(sender is TextBox tb))
                return;

            if (string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Undo();
                return;
            }

            string filterText = Util.GetOnlyHex(tb.Text);
            if (string.IsNullOrWhiteSpace(filterText) || filterText.Length != tb.Text.Length)
            {
                WinFormsUtil.Alert(MsgProgramErrorExpectedHex, tb.Text);
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
            else if (SAV is ISecureValueStorage s)
            {
                var value = Convert.ToUInt64(filterText, 16);
                if (tb == TB_Secure1)
                    s.TimeStampCurrent = value;
                else if (tb == TB_Secure2)
                    s.TimeStampPrevious = value;
                SAV.Edited = true;
            }
        }

        private void SwitchDaycare(object sender, EventArgs e)
        {
            if (!SAV.HasTwoDaycares)
                return;
            var current = string.Format(MsgSaveSwitchDaycareCurrent, SAV.DaycareIndex + 1);
            if (DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, MsgSaveSwitchDaycareView, current))
                return;
            SAV.DaycareIndex ^= 1;
            ResetDaycare();
        }

        private void B_SaveBoxBin_Click(object sender, EventArgs e)
        {
            if (!SAV.HasBox)
            { WinFormsUtil.Alert(MsgSaveBoxFailNone); return; }
            Box.SaveBoxBinary();
        }

        // Subfunction Save Buttons //
        private void B_OpenWondercards_Click(object sender, EventArgs e) => new SAV_Wondercard(SAV, sender as MysteryGift).ShowDialog();
        private void B_OpenPokepuffs_Click(object sender, EventArgs e) => new SAV_Pokepuff(SAV).ShowDialog();
        private void B_OpenPokeBeans_Click(object sender, EventArgs e) => new SAV_Pokebean(SAV).ShowDialog();
        private void B_OpenItemPouch_Click(object sender, EventArgs e) => new SAV_Inventory(SAV).ShowDialog();
        private void B_OpenBerryField_Click(object sender, EventArgs e) => new SAV_BerryFieldXY(SAV).ShowDialog();
        private void B_OpenPokeblocks_Click(object sender, EventArgs e) => new SAV_PokeBlockORAS(SAV).ShowDialog();
        private void B_OpenSuperTraining_Click(object sender, EventArgs e) => new SAV_SuperTrain(SAV).ShowDialog();
        private void B_OpenSecretBase_Click(object sender, EventArgs e) => new SAV_SecretBase(SAV).ShowDialog();
        private void B_CellsStickers_Click(object sender, EventArgs e) => new SAV_ZygardeCell(SAV).ShowDialog();
        private void B_LinkInfo_Click(object sender, EventArgs e) => new SAV_Link6(SAV).ShowDialog();
        private void B_Roamer_Click(object sender, EventArgs e) => new SAV_Roamer3(SAV).ShowDialog();
        private void B_OpenApricorn_Click(object sender, EventArgs e) => new SAV_Apricorn(SAV).ShowDialog();
        private void B_CGearSkin_Click(object sender, EventArgs e) => new SAV_CGearSkin(SAV).ShowDialog();

        private void B_OpenEventFlags_Click(object sender, EventArgs e)
        {
            Form form;
            switch (SAV)
            {
                case SAV1 s:
                    form = new SAV_EventReset1(s);
                    break;
                case SAV7b s:
                    form = new SAV_EventWork(s);
                    break;
                default:
                    form = new SAV_EventFlags(SAV);
                    break;
            }
            form.ShowDialog();
        }

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
            else if (SAV is SAV7)
                new SAV_Trainer7(SAV).ShowDialog();
            else if (SAV is SAV7b b)
                new SAV_Trainer7GG(b).ShowDialog();
        }

        private void B_OpenOPowers_Click(object sender, EventArgs e)
        {
            if (SAV.Generation != 6)
                return;
            new SAV_OPower((SAV6)SAV).ShowDialog();
        }

        private void B_OpenFriendSafari_Click(object sender, EventArgs e)
        {
            if (!SAV.XY)
                return;

            var dr = WinFormsUtil.Prompt(MessageBoxButtons.YesNo, MsgSaveGen6FriendSafari, MsgSaveGen6FriendSafariCheatDesc);
            if (dr == DialogResult.Yes)
                ((SAV6)SAV).UnlockAllFriendSafariSlots();
        }

        private static Form GetPokeDexEditor(SaveFile sav)
        {
            switch (sav)
            {
                case SAV1 s1: return new SAV_SimplePokedex(s1);
                case SAV2 s2: return new SAV_SimplePokedex(s2);
                case SAV3 s3: return new SAV_SimplePokedex(s3);
                case SAV4 s4: return new SAV_Pokedex4(s4);
                case SAV5 s5: return new SAV_Pokedex5(s5);
                case SAV6 s6 when s6.XY: return new SAV_PokedexXY(s6);
                case SAV6 s6 when s6.ORAS: return new SAV_PokedexORAS(s6);
                case SAV7 s7 when s7.SM || s7.USUM: return new SAV_PokedexSM(s7);
                case SAV7b b7: return new SAV_PokedexGG(b7);

                default: return null;
            }
        }

        private void B_OpenPokedex_Click(object sender, EventArgs e)
        {
            var editor = GetPokeDexEditor(SAV);
            editor?.ShowDialog();
        }

        private void B_OpenMiscEditor_Click(object sender, EventArgs e)
        {
            switch (SAV.Generation)
            {
                case 3: new SAV_Misc3(SAV).ShowDialog(); break;
                case 4: new SAV_Misc4(SAV).ShowDialog(); break;
                case 5: new SAV_Misc5(SAV).ShowDialog(); break;
            }
        }

        private void B_OpenRTCEditor_Click(object sender, EventArgs e)
        {
            switch (SAV.Generation)
            {
                case 2:
                    var sav2 = ((SAV2) SAV);
                    var msg = MsgSaveGen2RTCResetBitflag;
                    if (!sav2.Japanese) // show Reset Key for non-Japanese saves
                        msg = string.Format(MsgSaveGen2RTCResetPassword, sav2.ResetKey) + Environment.NewLine + Environment.NewLine + msg;
                    var dr = WinFormsUtil.Prompt(MessageBoxButtons.YesNo, msg);
                    if (dr == DialogResult.Yes)
                        sav2.ResetRTC();
                    break;
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
            if (DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, MsgSaveGen6Passerby))
                return;
            var result = PSS6.GetPSSParse((SAV6)SAV);
            Clipboard.SetText(string.Join(Environment.NewLine, result));
        }

        private void B_OUTHallofFame_Click(object sender, EventArgs e)
        {
            if (SAV.Generation == 6)
                new SAV_HallOfFame(SAV).ShowDialog();
            else if (SAV is SAV7)
                new SAV_HallOfFame7(SAV).ShowDialog();
        }

        private void B_JPEG_Click(object sender, EventArgs e)
        {
            byte[] jpeg = SAV.JPEGData;
            if (SAV.JPEGData.Length == 0)
            {
                WinFormsUtil.Alert(MsgSaveJPEGExportFail);
                return;
            }
            string filename = $"{SAV.JPEGTitle}'s picture";
            var sfd = new SaveFileDialog { FileName = filename, Filter = "JPEG|*.jpeg" };
            if (sfd.ShowDialog() != DialogResult.OK)
                return;
            File.WriteAllBytes(sfd.FileName, jpeg);
        }

        private void ClickVerifyCHK(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                var bulk = new BulkAnalysis(SAV);
                if (bulk.Parse.Count == 0)
                {
                    WinFormsUtil.Alert("Clean!");
                    return;
                }
                var lines = bulk.Parse.Select(z => $"{z.Judgement}: {z.Comment}");
                var msg = string.Join(Environment.NewLine, lines);
                Clipboard.SetText(msg);
                SystemSounds.Asterisk.Play();
                return;
            }

            if (SAV.Edited)
            {
                WinFormsUtil.Alert(MsgSaveChecksumFailEdited);
                return;
            }
            if (SAV.ChecksumsValid)
            {
                WinFormsUtil.Alert(MsgSaveChecksumValid);
                return;
            }

            if (DialogResult.Yes == WinFormsUtil.Prompt(MessageBoxButtons.YesNo, MsgSaveChecksumFailExport))
                Clipboard.SetText(SAV.ChecksumInfo);
        }

        // File I/O
        public bool GetBulkImportSettings(out bool clearAll, out bool overwrite, out PKMImportSetting noSetb)
        {
            clearAll = false; noSetb = PKMImportSetting.UseDefault; overwrite = false;
            var dr = WinFormsUtil.Prompt(MessageBoxButtons.YesNoCancel, MsgSaveBoxImportClear, MsgSaveBoxImportClearNo);
            if (dr == DialogResult.Cancel)
                return false;

            clearAll = dr == DialogResult.Yes;
            noSetb = GetPKMSetOverride(ModifyPKM);
            return true;
        }

        private static bool IsFolderPath(out string path)
        {
            var fbd = new FolderBrowserDialog();
            var result = fbd.ShowDialog() == DialogResult.OK;
            path = fbd.SelectedPath;
            return result;
        }

        public bool ExportSaveFile()
        {
            ValidateChildren();
            bool reload = SAV is SAV7b b && b.FixPreWrite();
            if (reload)
                ReloadSlots();
            return WinFormsUtil.SaveSAVDialog(SAV, SAV.CurrentBox);
        }

        public bool ExportBackup()
        {
            if (!SAV.Exportable)
                return false;
            var sfd = new SaveFileDialog {FileName = Util.CleanFileName(SAV.BAKName)};
            if (sfd.ShowDialog() != DialogResult.OK)
                return false;

            string path = sfd.FileName;
            File.WriteAllBytes(path, SAV.BAK);
            WinFormsUtil.Alert(MsgSaveBackup, path);

            return true;
        }

        public bool OpenPCBoxBin(byte[] input, out string c)
        {
            if (SAV.PCBinary.Length == input.Length)
            {
                if (SAV.IsAnySlotLockedInBox(0, SAV.BoxCount - 1))
                { c = MsgSaveBoxImportPCFailBattle; return false; }
                if (!SAV.SetPCBinary(input))
                { c = string.Format(MsgSaveCurrentGeneration, SAV.Generation); return false; }

                c = MsgSaveBoxImportPCBinary;
            }
            else if (SAV.GetBoxBinary(Box.CurrentBox).Length == input.Length)
            {
                if (SAV.IsAnySlotLockedInBox(Box.CurrentBox, Box.CurrentBox))
                { c = MsgSaveBoxImportBoxFailBattle; return false; }
                if (!SAV.SetBoxBinary(input, Box.CurrentBox))
                { c = string.Format(MsgSaveCurrentGeneration, SAV.Generation); return false; }

                c = MsgSaveBoxImportBoxBinary;
            }
            else
            {
                c = string.Format(MsgSaveCurrentGeneration, SAV.Generation);
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
                c = MsgSaveBoxImportVideoFailGeneration;
                return false;
            }

            var prompt = WinFormsUtil.Prompt(MessageBoxButtons.YesNo,
                string.Format(MsgSaveBoxImportVideo, Box.CurrentBoxName), MsgSaveBoxImportOverwrite);
            if (prompt != DialogResult.Yes)
            {
                c = string.Empty;
                return false;
            }

            var noSetb = GetPKMSetOverride(ModifyPKM);
            PKM[] data = b.BattlePKMs;
            int offset = SAV.GetBoxOffset(Box.CurrentBox);
            int slotSkipped = 0;
            for (int i = 0; i < 24; i++)
            {
                if (SAV.IsSlotOverwriteProtected(Box.CurrentBox, i))
                { slotSkipped++; continue; }
                SAV.SetStoredSlot(data[i], offset + (i * SAV.SIZE_STORED), noSetb);
            }

            SetPKMBoxes();
            UpdateBoxViewers();

            c = slotSkipped > 0 ? string.Format(MsgSaveBoxImportSkippedLocked, slotSkipped) : MsgSaveBoxImportVideoSuccess;

            return true;
        }

        public bool DumpBoxes(out string result, string path = null, bool separate = false)
        {
            if (path == null && !IsFolderPath(out path))
            {
                result = path;
                return false;
            }

            Directory.CreateDirectory(path);

            var count = SAV.DumpBoxes(path, separate);
            if (count < 0)
                result = MsgSaveBoxExportInvalid;
            else
                result = string.Format(MsgSaveBoxExportPathCount, count) + Environment.NewLine + path;
            return true;
        }

        public bool DumpBox(out string result, string path = null)
        {
            if (path == null && !IsFolderPath(out path))
            {
                result = path;
                return false;
            }

            Directory.CreateDirectory(path);

            var count = SAV.DumpBox(path, Box.CurrentBox);
            if (count < 0)
                result = MsgSaveBoxExportInvalid;
            else
                result = string.Format(MsgSaveBoxExportPathCount, count) + Environment.NewLine + path;
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

            if (!GetBulkImportSettings(out bool clearAll, out var overwrite, out var noSetb))
                return false;

            SAV.LoadBoxes(path, out result, Box.CurrentBox, clearAll, overwrite, noSetb);
            SetPKMBoxes();
            UpdateBoxViewers();
            return true;
        }

        public bool ToggleInterface()
        {
            FieldsLoaded = false;

            ToggleViewReset();
            ToggleViewSubEditors(SAV);

            bool WindowTranslationRequired = false;
            WindowTranslationRequired |= ToggleViewBox(SAV);
            int BoxTab = tabBoxMulti.TabPages.IndexOf(Tab_Box);
            WindowTranslationRequired |= ToggleViewParty(SAV, BoxTab);
            int PartyTab = tabBoxMulti.TabPages.IndexOf(Tab_PartyBattle);
            WindowTranslationRequired |= ToggleViewDaycare(SAV, BoxTab, PartyTab);
            SetPKMBoxes();   // Reload all of the PKX Windows

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
            SortMenu.ToggleVisibility();
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

            if (tabBoxMulti.TabPages.Contains(Tab_Box))
                return false;
            tabBoxMulti.TabPages.Insert(0, Tab_Box);
            return true;
        }

        private bool ToggleViewParty(SaveFile sav, int BoxTab)
        {
            if (!sav.HasParty || !sav.Exportable)
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
            if (!sav.HasDaycare || !sav.Exportable)
            {
                if (tabBoxMulti.TabPages.Contains(Tab_Other))
                    tabBoxMulti.TabPages.Remove(Tab_Other);
                return false;
            }

            SlotPictureBoxes[(int)SlotIndex.Daycare + 1].Visible = sav.Generation >= 2; // Second daycare slot
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
            if (!sav.Exportable || sav is BulkStorage)
            {
                GB_SAVtools.Visible = false;
                B_JPEG.Visible = false;
                SL_Extra.HideAllSlots();
                return;
            }

            {
                PAN_BattleBox.Visible = L_BattleBox.Visible = L_ReadOnlyPBB.Visible = sav.HasBattleBox;
                GB_Daycare.Visible = sav.HasDaycare;
                B_OpenSecretBase.Enabled = sav.HasSecretBase;
                B_OpenPokepuffs.Enabled = sav is IPokePuff p && p.HasPuffData;
                B_OUTPasserby.Enabled = sav.HasPSS;
                B_OpenBoxLayout.Enabled = sav.HasNamableBoxes;
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
                B_OpenPokeBeans.Enabled = B_CellsStickers.Enabled = B_FestivalPlaza.Enabled = sav is SAV7;

                B_OpenTrainerInfo.Enabled = B_OpenItemPouch.Enabled = (sav.HasParty && !(SAV is SAV4BR)) || SAV is SAV7b; // Box RS & Battle Revolution
                B_OpenMiscEditor.Enabled = sav is SAV3 || sav is SAV4 || sav is SAV5;
                B_Roamer.Enabled = sav is SAV3;

                B_OpenHoneyTreeEditor.Enabled = sav.DP || sav.Pt;
                B_OpenApricorn.Enabled = sav.HGSS;
                B_OpenRTCEditor.Enabled = sav.RS || sav.E || sav.Generation == 2;
                B_OpenUGSEditor.Enabled = sav.DP || sav.Pt;
                B_MailBox.Enabled = sav is SAV2 || sav is SAV3 || sav is SAV4 || sav is SAV5;

                SL_Extra.Initialize(sav.GetExtraSlots(HaX), InitializeDragDrop);
            }
            GB_SAVtools.Visible = sav.Exportable && FLP_SAVtools.Controls.OfType<Control>().Any(c => c.Enabled);
            foreach (Control c in FLP_SAVtools.Controls.OfType<Control>())
                c.Visible = c.Enabled;
        }

        private void ToggleViewMisc(SaveFile sav)
        {
            // Generational Interface
            ToggleSecrets(sav, HideSecretDetails);
            B_VerifyCHK.Enabled = SAV.Exportable;

            if (sav is SAV4BR br)
            {
                L_SaveSlot.Visible = CB_SaveSlot.Visible = true;
                var list = br.SaveNames.Select((z, i) => new ComboItem {Text = z, Value = i}).ToList();
                CB_SaveSlot.InitializeBinding();
                CB_SaveSlot.DataSource = new BindingSource(list, null);
                CB_SaveSlot.SelectedValue = br.CurrentSlot;
            }
            else
            {
                L_SaveSlot.Visible = CB_SaveSlot.Visible = false;
            }

            if (sav is ISecureValueStorage s)
            {
                TB_Secure1.Text = s.TimeStampCurrent.ToString("X16");
                TB_Secure2.Text = s.TimeStampPrevious.ToString("X16");
            }

            switch (sav.Generation)
            {
                case 6:
                case 7:
                    TB_GameSync.Enabled = sav.GameSyncID != null;
                    TB_GameSync.MaxLength = sav.GameSyncIDSize;
                    TB_GameSync.Text = (sav.GameSyncID ?? 0.ToString()).PadLeft(sav.GameSyncIDSize, '0');
                    break;
            }
        }

        private void ToggleSecrets(SaveFile sav, bool hide)
        {
            TB_Secure1.Visible = TB_Secure2.Visible = L_Secure1.Visible = L_Secure2.Visible = sav.Exportable && sav.Generation >= 6 && !hide;
            TB_GameSync.Visible = L_GameSync.Visible = sav.Exportable && sav.Generation >= 6 && !hide;
        }

        // DragDrop
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

        public void ClickShowdownExportParty(object sender, EventArgs e) => ExportShowdownText(SAV, MsgSimulatorExportParty, sav => sav.PartyData);
        public void ClickShowdownExportBattleBox(object sender, EventArgs e) => ExportShowdownText(SAV, MsgSimulatorExportBattleBox, sav => sav.BattleBoxData);

        public void ClickShowdownExportCurrentBox(object sender, EventArgs e)
        {
            if (!SAV.HasBox)
                return;
            ExportShowdownText(SAV, MsgSimulatorExportList,
                sav => (ModifierKeys & Keys.Control) != 0 ? sav.BoxData : sav.GetBoxData(CurrentBox));
        }

        private static void ExportShowdownText(SaveFile SAV, string success, Func<SaveFile, IEnumerable<PKM>> func)
        {
            try
            {
                var pkms = func(SAV);
                var str = ShowdownSet.GetShowdownSets(pkms, Environment.NewLine + Environment.NewLine);
                if (string.IsNullOrWhiteSpace(str)) return;
                Clipboard.SetText(str);
                WinFormsUtil.Alert(success);
            }
            catch
            {
                WinFormsUtil.Error(MsgClipboardFailWrite);
            }
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

        private void B_FestivalPlaza_Click(object sender, EventArgs e)
        {
            if (SAV is SAV7)
                new SAV_FestivalPlaza(SAV).ShowDialog();
        }

        private void B_MailBox_Click(object sender, EventArgs e)
        {
            new SAV_MailBox(SAV).ShowDialog();
            ResetParty();
        }

        private void GenerateLivingDex()
        {
            SAV.BoxData = GetLivingDex(SAV);
            ReloadSlots();
        }

        private static PKMImportSetting GetPKMSetOverride(bool currentSetting)
        {
            var yn = currentSetting ? MsgYes : MsgNo;
            DialogResult noSet = WinFormsUtil.Prompt(MessageBoxButtons.YesNoCancel,
                MsgSaveBoxImportModifyIntro,
                MsgSaveBoxImportModifyYes + Environment.NewLine +
                MsgSaveBoxImportModifyNo + Environment.NewLine +
                string.Format(MsgSaveBoxImportModifyCurrent, yn));
            switch (noSet)
            {
                case DialogResult.Yes: return PKMImportSetting.Update;
                case DialogResult.No: return PKMImportSetting.Skip;
                default: return PKMImportSetting.UseDefault;
            }
        }

        private static IList<PKM> GetLivingDex(SaveFile SAV)
        {
            var bd = SAV.BoxData;
            var tr = SAV;
            for (int i = 1; i <= 807; i++)
            {
                var pk = SAV.BlankPKM;
                pk.Species = i;
                pk.Gender = pk.GetSaneGender();
                if (i == 678)
                    pk.AltForm = pk.Gender;
                var f = EncounterMovesetGenerator.GeneratePKMs(pk, tr).FirstOrDefault();
                if (f != null)
                    bd[i] = PKMConverter.ConvertToType(f, SAV.PKMType, out _);
            }
            return bd;
        }
    }
}
