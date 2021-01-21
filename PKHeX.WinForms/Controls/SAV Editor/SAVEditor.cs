using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Drawing;
using static PKHeX.Core.MessageStrings;

namespace PKHeX.WinForms.Controls
{
    public partial class SAVEditor : UserControl, ISlotViewer<PictureBox>, ISaveFileProvider
    {
        public SaveDataEditor<PictureBox> EditEnv = null!;

        public void SetEditEnvironment(SaveDataEditor<PictureBox> value)
        {
            EditEnv = value;
            M.Env = value;
            menu.Editor = value;
            SAV = value.SAV;
            value.Slots.Publisher.Subscribers.Add(this);
            value.Slots.Publisher.Subscribers.Add(SL_Party);
            value.Slots.Publisher.Subscribers.Add(Box);
            value.Slots.Publisher.Subscribers.Add(SL_Extra);
        }

        public SaveFile SAV { get; private set; } = new FakeSaveFile();
        public int CurrentBox => Box.CurrentBox;
        public SlotChangeManager M { get; }
        public readonly ContextMenuSAV menu;
        public readonly BoxMenuStrip SortMenu;

        public bool  HaX;
        public bool ModifyPKM { private get; set; }
        private bool _hideSecret;
        public bool HideSecretDetails { private get => _hideSecret; set => ToggleSecrets(SAV, _hideSecret = value); }
        public ToolStripMenuItem Menu_Redo { get; set; } = null!;
        public ToolStripMenuItem Menu_Undo { get; set; } = null!;
        private bool FieldsLoaded;

        public IList<PictureBox> SlotPictureBoxes { get; }

        public int ViewIndex { get; set; } = -1;

        public bool FlagIllegal
        {
            get => Box.FlagIllegal;
            set
            {
                SL_Extra.FlagIllegal = SL_Party.FlagIllegal = Box.FlagIllegal = value && !HaX;
                if (SAV is FakeSaveFile)
                    return;
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
            SlotPictureBoxes = new[] { dcpkx1, dcpkx2 };

            Tab_Box.ContextMenuStrip = SortMenu = new BoxMenuStrip(this);
            M = new SlotChangeManager(this) {Env = EditEnv};
            Box.Setup(M);
            SL_Party.Setup(M);

            SL_Extra.ViewIndex = -2;
            menu = new ContextMenuSAV { Manager = M };
            InitializeEvents();
        }

        private void InitializeEvents()
        {
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
                Box.CurrentBox = e.Delta > 1 ? Box.Editor.MoveLeft() : Box.Editor.MoveRight();
            };

            GB_Daycare.Click += (o, args) => SwitchDaycare(GB_Daycare, args);
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
        public event EventHandler? RequestCloneData;

        /// <summary>Occurs when the Control Collection requests a save to be reloaded.</summary>
        public event EventHandler? RequestReloadSave;

        public void EnableDragDrop(DragEventHandler enter, DragEventHandler drop)
        {
            AllowDrop = true;
            DragDrop += drop;
            foreach (var tab in tabBoxMulti.TabPages.OfType<TabPage>())
            {
                tab.AllowDrop = true;
                tab.DragEnter += enter;
                tab.DragDrop += drop;
            }
            M.Drag.RequestExternalDragDrop += drop;
        }

        // Generic Subfunctions //
        public int GetSlotOffset(int slot)
        {
            return SAV.GetDaycareSlotOffset(SAV.DaycareIndex, slot);
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

        public void NotifySlotOld(ISlotInfo previous)
        {
            var index = GetViewIndex(previous);
            if (index < 0)
                return;

            var pb = SlotPictureBoxes[index];
            pb.BackgroundImage = null;
        }

        public int GetViewIndex(ISlotInfo slot)
        {
            for (int i = 0; i < SlotPictureBoxes.Count; i++)
            {
                var data = GetSlotData(i);
                if (data.Equals(slot))
                    return i;
            }
            return -1;
        }

        public void NotifySlotChanged(ISlotInfo slot, SlotTouchType type, PKM pkm)
        {
            var index = GetViewIndex(slot);
            if (index < 0)
                return;

            if (type.IsContentChange() && slot is SlotInfoParty)
                ResetParty(); // lots of slots change, just update

            var pb = SlotPictureBoxes[index];
            SlotUtil.UpdateSlot(pb, slot, pkm, SAV, Box.FlagIllegal, type);
        }

        public ISlotInfo GetSlotData(PictureBox view)
        {
            var index = SlotPictureBoxes.IndexOf(view);
            return GetSlotData(index);
        }

        public ISlotInfo GetSlotData(int index)
        {
            var ofs = GetSlotOffset(index);
            return new SlotInfoMisc(SAV, index, ofs);
        }

        public void SetPKMBoxes()
        {
            if (SAV.HasBox)
                Box.ResetSlots();

            ResetNonBoxSlots();
        }

        private void ResetNonBoxSlots()
        {
            ResetParty();
            ResetDaycare();
            ResetMiscSlots();
        }

        private void ResetMiscSlots()
        {
            var slots = SL_Extra.SlotPictureBoxes;
            for (int i = 0; i < SL_Extra.SlotCount; i++)
            {
                var info = SL_Extra.GetSlotData(i);
                var pb = slots[i];
                SlotUtil.UpdateSlot(pb, info, info.Read(SAV), SAV, Box.FlagIllegal);
            }
        }

        private void ResetParty()
        {
            if (SAV.HasParty)
                SL_Party.ResetSlots();
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
                var relIndex = i;
                var pb = UpdateSlot(relIndex);

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
                    var current = pb.Image;
                    if (current != null)
                        pb.Image = ImageUtil.ChangeOpacity(current, 0.6);
                }
            }

            bool? egg = SAV.IsDaycareHasEgg(SAV.DaycareIndex);
            DayCare_HasEgg.Visible = egg != null;
            DayCare_HasEgg.Checked = egg == true;

            var seed = SAV.GetDaycareRNGSeed(SAV.DaycareIndex);
            bool hasSeed = !string.IsNullOrEmpty(seed);
            if (hasSeed)
            {
                TB_RNGSeed.MaxLength = SAV.DaycareSeedSize;
                TB_RNGSeed.Text = seed;
            }
            L_DaycareSeed.Visible = TB_RNGSeed.Visible = hasSeed;
        }

        private PictureBox UpdateSlot(int relIndex)
        {
            var info = GetSlotData(relIndex);
            var pb = SlotPictureBoxes[relIndex];
            SlotUtil.UpdateSlot(pb, info, info.Read(SAV), SAV, Box.FlagIllegal);
            return pb;
        }

        public void SetParty() => ResetParty();

        public void ClickUndo()
        {
            EditEnv.Slots.Undo();
            UpdateUndoRedo();
        }

        public void ClickRedo()
        {
            EditEnv.Slots.Redo();
            UpdateUndoRedo();
        }

        public void UpdateUndoRedo()
        {
            Menu_Undo.Enabled = EditEnv.Slots.Changelog.CanUndo;
            Menu_Redo.Enabled = EditEnv.Slots.Changelog.CanRedo;
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
            var arr = new PKM[SAV.BoxSlotCount];
            for (int i = 0; i < SAV.BoxSlotCount; i++) // set to every slot in box
                arr[i] = pk;

            int slotSkipped = SAV.SetBoxData(arr, box);
            Box.ResetSlots();
            return slotSkipped;
        }

        public void ClickSlot(object sender, EventArgs e)
        {
            switch (ModifierKeys)
            {
                case Keys.Control | Keys.Alt:
                    ClickClone(sender, e);
                    break;
                default: // forward to contextmenu for default behavior
                    menu.OmniClick(sender, e, ModifierKeys);
                    break;
            }
        }

        private void ClickBoxSort(object sender, MouseEventArgs e)
        {
            if (tabBoxMulti.SelectedTab != Tab_Box)
                return;
            if (!tabBoxMulti.GetTabRect(tabBoxMulti.SelectedIndex).Contains(PointToClient(MousePosition)))
                return;
            if ((e.Button & MouseButtons.Right) == 0)
            {
                if ((ModifierKeys & Keys.Alt) != 0)
                    SortMenu.Clear();
                else if ((ModifierKeys & Keys.Control) != 0)
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
            if (!string.IsNullOrWhiteSpace(message))
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
            {
                var z = M.Boxes[1].ParentForm;
                if (z == null)
                    return;
                z.CenterToForm(ParentForm);
                z.BringToFront();
                return;
            }
            new SAV_BoxViewer(this, M).Show();
        }

        private void ClickClone(object sender, EventArgs e)
        {
            var detail = Box.GetSlotData((PictureBox) sender);
            if (detail is SlotInfoBox)
                RequestCloneData?.Invoke(sender, e);
        }

        private void UpdateSaveSlot(object sender, EventArgs e)
        {
            if (SAV is not SAV4BR br)
                return;
            br.CurrentSlot = WinFormsUtil.GetIndex(CB_SaveSlot);
            Box.ResetBoxNames(); // fix box names
            SetPKMBoxes();
            UpdateBoxViewers(true);
        }

        private void UpdateStringSeed(object sender, EventArgs e)
        {
            if (!FieldsLoaded)
                return;

            if (sender is not TextBox tb)
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
                SAV.State.Edited = true;
            }
            else if (tb == TB_GameSync && SAV is IGameSync sync)
            {
                var value = filterText.PadLeft(sync.GameSyncIDSize, '0');
                sync.GameSyncID = value;
                SAV.State.Edited = true;
            }
            else if (SAV is ISecureValueStorage s)
            {
                var value = Convert.ToUInt64(filterText, 16);
                if (tb == TB_Secure1)
                    s.TimeStampCurrent = value;
                else if (tb == TB_Secure2)
                    s.TimeStampPrevious = value;
                SAV.State.Edited = true;
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
        private static void OpenDialog(Form f)
        {
            f.ShowDialog();
            f.Dispose();
        }

        private void B_OpenWondercards_Click(object sender, EventArgs e) => OpenDialog(new SAV_Wondercard(SAV, sender as DataMysteryGift));
        private void B_OpenPokepuffs_Click(object sender, EventArgs e) => OpenDialog(new SAV_Pokepuff(SAV));
        private void B_OpenPokeBeans_Click(object sender, EventArgs e) => OpenDialog(new SAV_Pokebean(SAV));
        private void B_OpenItemPouch_Click(object sender, EventArgs e) => OpenDialog(new SAV_Inventory(SAV));
        private void B_OpenBerryField_Click(object sender, EventArgs e) => OpenDialog(new SAV_BerryFieldXY((SAV6XY)SAV));
        private void B_OpenPokeblocks_Click(object sender, EventArgs e) => OpenDialog(new SAV_PokeBlockORAS(SAV));
        private void B_OpenSuperTraining_Click(object sender, EventArgs e) => OpenDialog(new SAV_SuperTrain(SAV));
        private void B_OpenSecretBase_Click(object sender, EventArgs e) => OpenDialog(new SAV_SecretBase(SAV));
        private void B_CellsStickers_Click(object sender, EventArgs e) => OpenDialog(new SAV_ZygardeCell(SAV));
        private void B_LinkInfo_Click(object sender, EventArgs e) => OpenDialog(new SAV_Link6(SAV));
        private void B_Roamer_Click(object sender, EventArgs e) => OpenDialog(new SAV_Roamer3(SAV));
        private void B_OpenApricorn_Click(object sender, EventArgs e) => OpenDialog(new SAV_Apricorn((SAV4HGSS)SAV));
        private void B_CGearSkin_Click(object sender, EventArgs e) => OpenDialog(new SAV_CGearSkin(SAV));
        private void B_OpenTrainerInfo_Click(object sender, EventArgs e) => OpenDialog(GetTrainerEditor(SAV));
        private void B_OpenOPowers_Click(object sender, EventArgs e) => OpenDialog(new SAV_OPower((ISaveBlock6Main)SAV));
        private void B_OpenHoneyTreeEditor_Click(object sender, EventArgs e) => OpenDialog(new SAV_HoneyTree((SAV4Sinnoh)SAV));

        private void B_OpenEventFlags_Click(object sender, EventArgs e)
        {
            using var form = SAV switch
            {
                SAV1 s => (Form) new SAV_EventReset1(s),
                SAV7b s => new SAV_EventWork(s),
                _ => new SAV_EventFlags(SAV)
            };
            form.ShowDialog();
        }

        private void B_OpenBoxLayout_Click(object sender, EventArgs e)
        {
            OpenDialog(new SAV_BoxLayout(SAV, Box.CurrentBox));
            Box.ResetBoxNames(); // fix box names
            Box.ResetSlots(); // refresh box background
            UpdateBoxViewers(all: true); // update subviewers
        }

        private static Form GetTrainerEditor(SaveFile sav) => sav switch
        {
            SAV6 s6 => new SAV_Trainer(s6),
            SAV7 s7 => new SAV_Trainer7(s7),
            SAV7b b7 => new SAV_Trainer7GG(b7),
            SAV8SWSH swsh => new SAV_Trainer8(swsh),
            _ => new SAV_SimpleTrainer(sav)
        };

        private void B_OpenRaids_Click(object sender, EventArgs e)
        {
            if (SAV is not SAV8SWSH swsh)
                return;
            if (sender == B_Raids)
                OpenDialog(new SAV_Raid8(swsh, swsh.Raid));
            else if (sender == B_RaidArmor)
                OpenDialog(new SAV_Raid8(swsh, swsh.RaidArmor));
            else
                OpenDialog(new SAV_Raid8(swsh, swsh.RaidCrown));
        }

        private void B_OtherSlots_Click(object sender, EventArgs e)
        {
            void TryOpen(SaveFile sav, IReadOnlyList<SlotGroup> g)
            {
                var form = WinFormsUtil.FirstFormOfType<SAV_GroupViewer>();
                if (form != null)
                    form.CenterToForm(ParentForm);
                else
                    form = new SAV_GroupViewer(sav, M.Env.PKMEditor, g);
                form.BringToFront();
                form.Show();
            }

            if (SAV is SAV_STADIUM s0)
                TryOpen(s0, s0.GetRegisteredTeams());
        }

        private void B_Blocks_Click(object sender, EventArgs e)
        {
            var form = GetAccessorForm(SAV);
            form.ShowDialog();
            form.Dispose();
        }

        private static Form GetAccessorForm(SaveFile sav) => sav switch
        {
            SAV5BW s => new SAV_Accessor<SaveBlockAccessor5BW>(s.Blocks),
            SAV5B2W2 s => new SAV_Accessor<SaveBlockAccessor5B2W2>(s.Blocks),
            SAV6XY s => new SAV_Accessor<SaveBlockAccessor6XY>(s.Blocks),
            SAV6AO s => new SAV_Accessor<SaveBlockAccessor6AO>(s.Blocks),
            SAV6AODemo s => new SAV_Accessor<SaveBlockAccessor6AODemo>(s.Blocks),
            SAV7SM s => new SAV_Accessor<SaveBlockAccessor7SM>(s.Blocks),
            SAV7USUM s => new SAV_Accessor<SaveBlockAccessor7USUM>(s.Blocks),
            SAV8SWSH s => new SAV_BlockDump8(s),
            _ => GetPropertyForm(sav),
        };

        private static Form GetPropertyForm(object sav)
        {
            var form = new Form
            {
                Text = "Simple Editor",
                StartPosition = FormStartPosition.CenterParent,
                MinimumSize = new Size(350, 380),
                MinimizeBox = false,
                MaximizeBox = false
            };
            var pg = new PropertyGrid {SelectedObject = sav, Dock = DockStyle.Fill};
            form.Controls.Add(pg);
            return form;
        }

        private void B_OpenFriendSafari_Click(object sender, EventArgs e)
        {
            if (SAV is not SAV6XY xy)
                return;

            var dr = WinFormsUtil.Prompt(MessageBoxButtons.YesNo, MsgSaveGen6FriendSafari, MsgSaveGen6FriendSafariCheatDesc);
            if (dr == DialogResult.Yes)
                xy.UnlockAllFriendSafariSlots();
        }

        private void B_OpenPokedex_Click(object sender, EventArgs e)
        {
            using var form = SAV switch
            {
                SAV1 s1 => new SAV_SimplePokedex(s1),
                SAV2 s2 => new SAV_SimplePokedex(s2),
                SAV3 s3 => new SAV_SimplePokedex(s3),
                SAV4 s4 => new SAV_Pokedex4(s4),
                SAV5 s5 => new SAV_Pokedex5(s5),
                SAV6XY xy => new SAV_PokedexXY(xy),
                SAV6AO ao => new SAV_PokedexORAS(ao),
                SAV7 s7 => new SAV_PokedexSM(s7),
                SAV7b b7 => new SAV_PokedexGG(b7),
                SAV8SWSH swsh => new SAV_PokedexSWSH(swsh),
                _ => (Form?)null
            };
            form?.ShowDialog();
        }

        private void B_OpenMiscEditor_Click(object sender, EventArgs e)
        {
            using var form = SAV.Generation switch
            {
                3 => new SAV_Misc3(SAV),
                4 => new SAV_Misc4((SAV4) SAV),
                5 => new SAV_Misc5(SAV),
                _ => (Form?)null,
            };
            form?.ShowDialog();
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
                    OpenDialog(new SAV_RTC3(SAV));
                    break;
            }
        }

        private void B_OUTPasserby_Click(object sender, EventArgs e)
        {
            if (SAV.Generation != 6)
                return;
            if (DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, MsgSaveGen6Passerby))
                return;
            var result = PSS6.GetPSSParse((SAV6)SAV);
            WinFormsUtil.SetClipboardText(string.Join(Environment.NewLine, result));
        }

        private void B_OUTHallofFame_Click(object sender, EventArgs e)
        {
            using var form = SAV switch
            {
                SAV6 s6 => new SAV_HallOfFame(s6),
                SAV7 s7 => new SAV_HallOfFame7(s7),
                _ => (Form?)null,
            };
            form?.ShowDialog();
        }

        private void B_JPEG_Click(object sender, EventArgs e)
        {
            var s6 = (SAV6)SAV;
            byte[] jpeg = s6.GetJPEGData();
            if (jpeg.Length == 0)
            {
                WinFormsUtil.Alert(MsgSaveJPEGExportFail);
                return;
            }
            string filename = $"{s6.JPEGTitle}'s picture";
            using var sfd = new SaveFileDialog { FileName = filename, Filter = "JPEG|*.jpeg" };
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
                WinFormsUtil.SetClipboardText(msg);
                SystemSounds.Asterisk.Play();
                return;
            }

            if (SAV.State.Edited)
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
                WinFormsUtil.SetClipboardText(SAV.ChecksumInfo);
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
            using var fbd = new FolderBrowserDialog();
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
            return WinFormsUtil.ExportSAVDialog(SAV, SAV.CurrentBox);
        }

        public bool ExportBackup()
        {
            if (!SAV.State.Exportable)
                return false;
            using var sfd = new SaveFileDialog {FileName = Util.CleanFileName(SAV.Metadata.BAKName)};
            if (sfd.ShowDialog() != DialogResult.OK)
                return false;

            string path = sfd.FileName;
            File.WriteAllBytes(path, SAV.State.BAK);
            WinFormsUtil.Alert(MsgSaveBackup, path);

            return true;
        }

        public bool OpenPCBoxBin(byte[] input, out string c)
        {
            if (SAV.GetPCBinary().Length == input.Length)
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

        public bool OpenGroup(IPokeGroup b, out string c)
        {
            var msg = string.Format(MsgSaveBoxImportGroup, Box.CurrentBoxName);
            var prompt = WinFormsUtil.Prompt(MessageBoxButtons.YesNo, msg, MsgSaveBoxImportOverwrite);
            if (prompt != DialogResult.Yes)
            {
                c = string.Empty;
                return false;
            }

            var noSetb = GetPKMSetOverride(ModifyPKM);
            var slotSkipped = ImportGroup(b.Contents, SAV, Box.CurrentBox, noSetb);

            SetPKMBoxes();
            UpdateBoxViewers();

            c = slotSkipped > 0 ? string.Format(MsgSaveBoxImportSkippedLocked, slotSkipped) : MsgSaveBoxImportGroupSuccess;

            return true;
        }

        private static int ImportGroup(IEnumerable<PKM> data, SaveFile sav, int box, PKMImportSetting noSetb)
        {
            var type = sav.PKMType;
            int slotSkipped = 0;
            int index = 0;
            foreach (var x in data)
            {
                var i = index++;
                if (sav.IsSlotOverwriteProtected(box, i))
                {
                    slotSkipped++;
                    continue;
                }

                var convert = PKMConverter.ConvertToType(x, type, out _);
                if (convert?.GetType() != type)
                {
                    slotSkipped++;
                    continue;
                }
                sav.SetBoxSlotAtIndex(x, box, i, noSetb);
            }

            return slotSkipped;
        }

        public bool DumpBoxes(out string result, string? path = null, bool separate = false)
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

        public bool DumpBox(out string result, string? path = null)
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

        public bool LoadBoxes(out string result, string? path = null)
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

            Box.M = M;
            SL_Party.M = M;
            if (SAV.HasBox)
            {
                bool newSlots = Box.InitializeFromSAV(SAV);
                if (newSlots)
                {
                    Box.HorizontallyCenter(Tab_Box);
                    foreach (var pb in Box.SlotPictureBoxes)
                        pb.ContextMenuStrip = menu.mnuVSD;

                    var grid = Box.BoxPokeGrid;
                    var height = grid.Height + grid.Location.Y + Box.Location.Y; // needed height
                    var required = height + 16;
                    var allowed = Tab_Box.Height;
                    if (required > allowed)
                        FindForm().Height += required - allowed;
                }
            }
            if (SAV.HasParty)
            {
                bool newSlots = SL_Party.InitializeFromSAV(SAV);
                if (newSlots)
                {
                    SL_Party.HorizontallyCenter(Tab_PartyBattle);
                    foreach (var pb in SL_Party.SlotPictureBoxes)
                        pb.ContextMenuStrip = menu.mnuVSD;
                }
            }

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
            tabBoxMulti.SelectedIndex = 0;

            var box = sav.CurrentBox;
            Box.CurrentBox = (uint)box >= sav.BoxCount ? 0 : box;

            if (tabBoxMulti.TabPages.Contains(Tab_Box))
                return false;
            tabBoxMulti.TabPages.Insert(0, Tab_Box);
            return true;
        }

        private bool ToggleViewParty(SaveFile sav, int BoxTab)
        {
            if (!sav.HasParty || !sav.State.Exportable)
            {
                if (tabBoxMulti.TabPages.Contains(Tab_PartyBattle))
                    tabBoxMulti.TabPages.Remove(Tab_PartyBattle);
                return false;
            }

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
            if ((!sav.HasDaycare && SL_Extra.SlotCount == 0) || !sav.State.Exportable)
            {
                if (tabBoxMulti.TabPages.Contains(Tab_Other))
                    tabBoxMulti.TabPages.Remove(Tab_Other);
                return false;
            }

            SlotPictureBoxes[1].Visible = sav.Generation >= 2; // Second daycare slot
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
            if (!sav.State.Exportable || sav is BulkStorage)
            {
                GB_SAVtools.Visible = false;
                B_JPEG.Visible = false;
                SL_Extra.HideAllSlots();
                return;
            }

            GB_Daycare.Visible = sav.HasDaycare;
            B_OpenPokeblocks.Visible = sav is SAV6AO;
            B_OpenSecretBase.Visible = sav is SAV6AO;
            B_OpenPokepuffs.Visible = sav is ISaveBlock6Main;
            B_JPEG.Visible = B_OpenLinkInfo.Visible = B_OpenSuperTraining.Visible = B_OUTPasserby.Visible = sav is ISaveBlock6Main;
            B_OpenBoxLayout.Visible = sav.HasNamableBoxes;
            B_OpenWondercards.Visible = sav.HasWondercards;
            B_OpenHallofFame.Visible = sav is ISaveBlock6Main or SAV7;
            B_OpenOPowers.Visible = sav is ISaveBlock6Main;
            B_OpenPokedex.Visible = sav.HasPokeDex;
            B_OpenBerryField.Visible = sav is SAV6XY; // oras undocumented
            B_OpenFriendSafari.Visible = sav is SAV6XY;
            B_OpenEventFlags.Visible = sav.HasEvents;
            B_CGearSkin.Visible = sav.Generation == 5;
            B_OpenPokeBeans.Visible = B_CellsStickers.Visible = B_FestivalPlaza.Visible = sav is SAV7;

            B_OtherSlots.Visible = sav is SAV1StadiumJ or SAV1Stadium or SAV2Stadium;
            B_OpenTrainerInfo.Visible = B_OpenItemPouch.Visible = (sav.HasParty && SAV is not SAV4BR) || SAV is SAV7b; // Box RS & Battle Revolution
            B_OpenMiscEditor.Visible = sav is SAV3 or SAV4 or SAV5;
            B_Roamer.Visible = sav is SAV3;

            B_OpenHoneyTreeEditor.Visible = B_OpenUGSEditor.Visible = sav is SAV4Sinnoh;
            B_OpenApricorn.Visible = sav is SAV4HGSS;
            B_OpenRTCEditor.Visible = sav.Generation == 2 || (sav is SAV3 s3 && (s3.RS || s3.E));
            B_MailBox.Visible = sav is SAV2 or SAV3 or SAV4 or SAV5;

            B_Raids.Visible = sav is SAV8SWSH;
            B_RaidArmor.Visible = sav is SAV8SWSH {SaveRevision: >= 1};
            B_RaidCrown.Visible = sav is SAV8SWSH {SaveRevision: >= 2};
            GB_SAVtools.Visible = B_Blocks.Visible = true;

            var list = FLP_SAVtools.Controls.OfType<Control>().OrderBy(z => z.Text).ToArray();
            FLP_SAVtools.Controls.Clear();
            FLP_SAVtools.Controls.AddRange(list);

            SL_Extra.SAV = sav;
            SL_Extra.Initialize(sav.GetExtraSlots(HaX), InitializeDragDrop);
        }

        private void ToggleViewMisc(SaveFile sav)
        {
            // Generational Interface
            ToggleSecrets(sav, HideSecretDetails);
            B_VerifyCHK.Enabled = SAV.State.Exportable;

            if (sav is SAV4BR br)
            {
                L_SaveSlot.Visible = CB_SaveSlot.Visible = true;
                var current = br.CurrentSlot;
                var list = br.SaveNames.Select((z, i) => new ComboItem(z, i)).ToList();
                CB_SaveSlot.InitializeBinding();
                CB_SaveSlot.DataSource = new BindingSource(list, null);
                CB_SaveSlot.SelectedValue = current;
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

            if (sav is IGameSync sync)
            {
                var gsid = sync.GameSyncID;
                TB_GameSync.Enabled = !string.IsNullOrEmpty(gsid);
                TB_GameSync.MaxLength = sync.GameSyncIDSize;
                TB_GameSync.Text = (string.IsNullOrEmpty(gsid) ? 0.ToString() : gsid).PadLeft(sync.GameSyncIDSize, '0');
            }
        }

        private void ToggleSecrets(SaveFile sav, bool hide)
        {
            var shouldShow = sav.State.Exportable && !hide;
            TB_Secure1.Visible = TB_Secure2.Visible = L_Secure1.Visible = L_Secure2.Visible = shouldShow && sav is ISecureValueStorage;
            TB_GameSync.Visible = L_GameSync.Visible = shouldShow && sav is IGameSync;
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

        public void ClickShowdownExportCurrentBox(object sender, EventArgs e)
        {
            if (!SAV.HasBox)
                return;
            ExportShowdownText(SAV, MsgSimulatorExportList,
                sav => (ModifierKeys & Keys.Control) != 0 ? sav.BoxData : sav.GetBoxData(CurrentBox));
        }

        private static void ExportShowdownText(SaveFile sav, string success, Func<SaveFile, IEnumerable<PKM>> fetch)
        {
            var list = fetch(sav);
            var result = ShowdownParsing.GetShowdownSets(list, Environment.NewLine + Environment.NewLine);
            if (string.IsNullOrWhiteSpace(result))
                return;
            if (WinFormsUtil.SetClipboardText(result))
                WinFormsUtil.Alert(success);
        }

        private void B_OpenUGSEditor_Click(object sender, EventArgs e)
        {
            if (SAV is not SAV4Sinnoh s)
                return;
            using var form = new SAV_Underground(s);
            form.ShowDialog();
        }

        private void B_FestivalPlaza_Click(object sender, EventArgs e)
        {
            if (SAV is not SAV7 s)
                return;
            using var form = new SAV_FestivalPlaza(s);
            form.ShowDialog();
        }

        private void B_MailBox_Click(object sender, EventArgs e)
        {
            using var form = new SAV_MailBox(SAV);
            form.ShowDialog();
            ResetParty();
        }

        private static PKMImportSetting GetPKMSetOverride(bool currentSetting)
        {
            var yn = currentSetting ? MsgYes : MsgNo;
            var choice = WinFormsUtil.Prompt(MessageBoxButtons.YesNoCancel,
                MsgSaveBoxImportModifyIntro,
                MsgSaveBoxImportModifyYes + Environment.NewLine +
                MsgSaveBoxImportModifyNo + Environment.NewLine +
                string.Format(MsgSaveBoxImportModifyCurrent, yn));
            return choice switch
            {
                DialogResult.Yes => PKMImportSetting.Update,
                DialogResult.No => PKMImportSetting.Skip,
                _ => PKMImportSetting.UseDefault
            };
        }
    }
}
