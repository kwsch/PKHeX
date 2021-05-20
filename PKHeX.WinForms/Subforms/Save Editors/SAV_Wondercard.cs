using PKHeX.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Drawing;
using PKHeX.WinForms.Controls;
using static PKHeX.Core.MessageStrings;

namespace PKHeX.WinForms
{
    public partial class SAV_Wondercard : Form
    {
        private readonly SaveFile Origin;
        private readonly SaveFile SAV;
        private readonly SummaryPreviewer Summary = new();

        public SAV_Wondercard(SaveFile sav, DataMysteryGift? g = null)
        {
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            SAV = (Origin = sav).Clone();
            mga = SAV.GiftAlbum;

            pba = SAV.Generation switch
            {
                4 => PopulateViewGiftsG4().ToArray(),
                5 or 6 or 7 => PopulateViewGiftsG567().ToArray(),
                _ => throw new ArgumentException("Game not supported."),
            };
            foreach (var pb in pba)
            {
                pb.AllowDrop = true;
                pb.DragDrop += BoxSlot_DragDrop;
                pb.DragEnter += BoxSlot_DragEnter;
                pb.MouseDown += BoxSlot_MouseDown;
                pb.ContextMenuStrip = mnuVSD;
                pb.MouseHover += (_, _) => Summary.Show(pb, mga.Gifts[pba.IndexOf(pb)]);
            }

            SetGiftBoxes();
            GetReceivedFlags();

            if (LB_Received.Items.Count > 0)
                LB_Received.SelectedIndex = 0;

            DragEnter += Main_DragEnter;
            DragDrop += Main_DragDrop;

            if (g == null)
                ClickView(pba[0], EventArgs.Empty);
            else
                ViewGiftData(g);
        }

        private readonly MysteryGiftAlbum mga;
        private DataMysteryGift? mg;
        private readonly IList<PictureBox> pba;

        // Repopulation Functions
        private void SetBackground(int index, Image bg)
        {
            for (int i = 0; i < mga.Gifts.Length; i++)
                pba[i].BackgroundImage = index == i ? bg : null;
        }

        private void SetGiftBoxes()
        {
            for (int i = 0; i < mga.Gifts.Length; i++)
            {
                MysteryGift m = mga.Gifts[i];
                pba[i].Image = m.Sprite();
            }
        }

        private void ViewGiftData(DataMysteryGift g)
        {
            try
            {
                // only check if the form is visible (not opening)
                if (Visible && g.GiftUsed && DialogResult.Yes == WinFormsUtil.Prompt(MessageBoxButtons.YesNo, MsgMsyteryGiftUsedAlert, MsgMysteryGiftUsedFix))
                    g.GiftUsed = false;

                RTB.Lines = g.GetDescription().ToArray();
                PB_Preview.Image = g.Sprite();
                mg = g;
            }
            // Some user input mystery gifts can have out-of-bounds values. Just swallow any exception.
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                WinFormsUtil.Error(MsgMysteryGiftParseTypeUnknown, e);
                RTB.Clear();
            }
        }

        private void GetReceivedFlags()
        {
            LB_Received.Items.Clear();
            for (int i = 1; i < mga.Flags.Length; i++)
            {
                if (mga.Flags[i])
                    LB_Received.Items.Add(i.ToString("0000"));
            }

            if (LB_Received.Items.Count > 0)
                LB_Received.SelectedIndex = 0;
        }

        private void SetCardID(int cardID)
        {
            if (cardID is <= 0 or >= 0x100 * 8)
                return;

            string card = cardID.ToString("0000");
            if (!LB_Received.Items.Contains(card))
                LB_Received.Items.Add(card);
            LB_Received.SelectedIndex = LB_Received.Items.IndexOf(card);
        }

        // Mystery Gift IO (.file<->window)
        private void B_Import_Click(object sender, EventArgs e)
        {
            using var import = new OpenFileDialog {Filter = WinFormsUtil.GetMysterGiftFilter(SAV.Generation, SAV.Version) };
            if (import.ShowDialog() != DialogResult.OK) return;

            string path = import.FileName;
            var g = MysteryGift.GetMysteryGift(File.ReadAllBytes(path), Path.GetExtension(path));
            if (g == null)
            {
                WinFormsUtil.Error(MsgMysteryGiftInvalid, path);
                return;
            }
            ViewGiftData(g);
        }

        private void B_Output_Click(object sender, EventArgs e)
        {
            if (mg == null)
                return;
            WinFormsUtil.ExportMGDialog(mg, SAV.Version);
        }

        private static int GetLastUnfilledByType(MysteryGift Gift, MysteryGiftAlbum Album)
        {
            for (int i = 0; i < Album.Gifts.Length; i++)
            {
                if (!Album.Gifts[i].Empty)
                    continue;
                if (Album.Gifts[i].Type != Gift.Type)
                    continue;
                return i;
            }
            return -1;
        }
        // Mystery Gift RW (window<->sav)
        private void ClickView(object sender, EventArgs e)
        {
            var pb = WinFormsUtil.GetUnderlyingControl<PictureBox>(sender);
            if (pb == null)
                return;
            int index = pba.IndexOf(pb);

            SetBackground(index, Drawing.Properties.Resources.slotView);
            ViewGiftData(mga.Gifts[index]);
        }

        private void ClickSet(object sender, EventArgs e)
        {
            if (mg == null)
                return;

            if (!mg.IsCardCompatible(SAV, out var msg))
            {
                WinFormsUtil.Alert(MsgMysteryGiftSlotFail, msg);
                return;
            }

            var pb = WinFormsUtil.GetUnderlyingControl<PictureBox>(sender);
            if (pb == null)
                return;
            int index = pba.IndexOf(pb);

            // Hijack to the latest unfilled slot if index creates interstitial empty slots.
            int lastUnfilled = GetLastUnfilledByType(mg, mga);
            if (lastUnfilled > -1 && lastUnfilled < index)
                index = lastUnfilled;

            if (mg is PCD pcd && mga.Gifts[index] is PGT)
            {
                mg = pcd.Gift;
            }
            else if (mg.Type != mga.Gifts[index].Type)
            {
                WinFormsUtil.Alert(MsgMysteryGiftSlotFail, $"{mg.Type} != {mga.Gifts[index].Type}");
                return;
            }
            SetBackground(index, Drawing.Properties.Resources.slotSet);
            mga.Gifts[index] = (DataMysteryGift)mg.Clone();
            SetGiftBoxes();
            SetCardID(mg.CardID);
        }

        private void ClickDelete(object sender, EventArgs e)
        {
            var pb = WinFormsUtil.GetUnderlyingControl<PictureBox>(sender);
            if (pb == null)
                return;
            int index = pba.IndexOf(pb);

            var arr = mga.Gifts[index].Data;
            Array.Clear(arr, 0, arr.Length);

            // Shuffle blank card down
            int i = index;
            while (i < mga.Gifts.Length - 1)
            {
                if (mga.Gifts[i+1].Empty)
                    break;
                if (mga.Gifts[i+1].Type != mga.Gifts[i].Type)
                    break;

                i++;

                var mg1 = mga.Gifts[i];
                var mg2 = mga.Gifts[i-1];

                mga.Gifts[i-1] = mg1;
                mga.Gifts[i] = mg2;
            }
            SetBackground(i, Drawing.Properties.Resources.slotDel);
            SetGiftBoxes();
        }

        // Close Window
        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void B_Save_Click(object sender, EventArgs e)
        {
            // Make sure all of the Received Flags are flipped!
            bool[] flags = new bool[mga.Flags.Length];
            foreach (var o in LB_Received.Items)
            {
                var value = o?.ToString();
                if (value == null)
                    continue;
                var flag = Util.ToUInt32(value);
                flags[flag] = true;
            }

            flags.CopyTo(mga.Flags, 0);
            SAV.GiftAlbum = mga;

            Origin.CopyChangesFrom(SAV);
            Close();
        }

        // Delete Received Flag
        private void ClearReceivedFlag(object sender, EventArgs e)
        {
            if (LB_Received.SelectedIndex < 0)
                return;

            if (LB_Received.SelectedIndices.Count > 1)
            {
                for (int i = LB_Received.SelectedIndices.Count - 1; i >= 0; i--)
                    LB_Received.Items.RemoveAt(LB_Received.SelectedIndices[i]);
            }
            else if (LB_Received.SelectedIndices.Count == 1)
            {
                int lastIndex = LB_Received.SelectedIndex;
                LB_Received.Items.RemoveAt(lastIndex);
                if (LB_Received.Items.Count == 0)
                    return;
                if (lastIndex == LB_Received.Items.Count)
                    lastIndex--;
                LB_Received.SelectedIndex = lastIndex;
            }
        }

        // Drag & Drop Wonder Cards
        private static void Main_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void Main_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            // Check for multiple wondercards
            if (Directory.Exists(files[0]))
                files = Directory.GetFiles(files[0], "*", SearchOption.AllDirectories);
            if (files.Length == 1 && !Directory.Exists(files[0]))
            {
                string path = files[0]; // open first D&D
                if (!MysteryGift.IsMysteryGift(new FileInfo(path).Length)) // arbitrary
                {
                    WinFormsUtil.Alert(MsgMysteryGiftInvalid, path);
                    return;
                }
                var gift = MysteryGift.GetMysteryGift(File.ReadAllBytes(path), Path.GetExtension(path));
                if (gift == null)
                {
                    WinFormsUtil.Error(MsgMysteryGiftInvalid, path);
                    return;
                }
                ViewGiftData(gift);
                return;
            }
            SetGiftBoxes();
        }

        private void ClickQR(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Alt)
            {
                string url = Clipboard.GetText();
                if (!string.IsNullOrWhiteSpace(url))
                {
                    ImportQRToView(url);
                    return;
                }
            }
            ExportQRFromView();
        }

        private void ExportQRFromView()
        {
            if (mg == null)
                return;
            if (mg.Empty)
            {
                WinFormsUtil.Alert(MsgMysteryGiftSlotNone);
                return;
            }
            if (SAV.Generation == 6 && mg.ItemID == 726 && mg.IsItem)
            {
                WinFormsUtil.Alert(MsgMysteryGiftQREonTicket, MsgMysteryGiftQREonTicketAdvice);
                return;
            }

            Image qr = QREncode.GenerateQRCode(mg);

            string desc = $"({mg.Type}) {string.Join(Environment.NewLine, mg.GetDescription())}";

            using var form = new QR(qr, PB_Preview.Image, desc + Environment.NewLine + "PKHeX Wonder Card @ ProjectPokemon.org");
            form.ShowDialog();
        }

        private void ImportQRToView(string url)
        {
            var msg = QRDecode.GetQRData(url, out var data);
            if (msg != 0)
            {
                WinFormsUtil.Alert(msg.ConvertMsg());
                return;
            }

            if (data.Length == 0)
                return;

            string[] types = mga.Gifts.Select(g => g.Type).Distinct().ToArray();
            var gift = MysteryGift.GetMysteryGift(data);
            if (gift == null)
                return;

            string giftType = gift.Type;

            if (mga.Gifts.All(card => card.Data.Length != data.Length))
                WinFormsUtil.Alert(MsgMysteryGiftQRTypeLength, string.Format(MsgQRDecodeSize, $"0x{data.Length:X}"));
            else if (types.All(type => type != giftType))
                WinFormsUtil.Alert(MsgMysteryGiftTypeIncompatible, $"{MsgMysteryGiftQRReceived} {gift.Type}{Environment.NewLine}{MsgMysteryGiftTypeUnexpected} {string.Join(", ", types)}");
            else if (!SAV.CanReceiveGift(gift))
                WinFormsUtil.Alert(MsgMysteryGiftTypeDetails);
            else
                ViewGiftData(gift);
        }

        private void BoxSlot_MouseDown(object? sender, MouseEventArgs e)
        {
            if (sender == null)
                return;
            switch (ModifierKeys)
            {
                case Keys.Control: ClickView(sender, e); return;
                case Keys.Shift: ClickSet(sender, e); return;
                case Keys.Alt: ClickDelete(sender, e); return;
            }
            var pb = sender as PictureBox;
            if (pb?.Image == null)
                return;

            if (e.Button != MouseButtons.Left || e.Clicks != 1) return;

            int index = pba.IndexOf(pb);
            wc_slot = index;
            // Create Temp File to Drag
            Cursor.Current = Cursors.Hand;

            // Make File
            var gift = mga.Gifts[index];
            string newfile = Path.Combine(Path.GetTempPath(), Util.CleanFileName(gift.FileName));
            try
            {
                File.WriteAllBytes(newfile, gift.Write());
                DoDragDrop(new DataObject(DataFormats.FileDrop, new[] { newfile }), DragDropEffects.Move);
            }
            // Sometimes the drag-drop is canceled or ends up at a bad location. Don't bother recovering from an exception; just display a safe error message.
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception x)
#pragma warning restore CA1031 // Do not catch general exception types
            { WinFormsUtil.Error("Drag & Drop Error", x); }
            File.Delete(newfile);
            wc_slot = -1;
        }

        private void BoxSlot_DragDrop(object sender, DragEventArgs e)
        {
            if (mg == null || sender is not PictureBox pb)
                return;

            int index = pba.IndexOf(pb);

            // Hijack to the latest unfilled slot if index creates interstitial empty slots.
            int lastUnfilled = GetLastUnfilledByType(mg, mga);
            if (lastUnfilled > -1 && lastUnfilled < index && mga.Gifts[lastUnfilled].Type == mga.Gifts[index].Type)
                index = lastUnfilled;

            if (wc_slot == -1) // dropped
            {
                var files = (string[]?)e.Data.GetData(DataFormats.FileDrop);
                if (files == null || files.Length == 0)
                    return;

                var first = files[0];
                var fi = new FileInfo(first);
                if (!MysteryGift.IsMysteryGift(fi.Length))
                { WinFormsUtil.Alert(MsgFileUnsupported, first); return; }

                byte[] data = File.ReadAllBytes(first);
                var gift = MysteryGift.GetMysteryGift(data, fi.Extension);
                if (gift == null)
                { WinFormsUtil.Alert(MsgFileUnsupported, first); return; }

                if (gift is PCD pcd && mga.Gifts[index] is PGT)
                {
                    gift = pcd.Gift;
                }
                else if (gift.Type != mga.Gifts[index].Type)
                {
                    WinFormsUtil.Alert(MsgMysteryGiftSlotFail, $"{gift.Type} != {mga.Gifts[index].Type}");
                    return;
                }
                SetBackground(index, Drawing.Properties.Resources.slotSet);
                mga.Gifts[index] = (DataMysteryGift)gift.Clone();

                SetCardID(mga.Gifts[index].CardID);
                ViewGiftData(mga.Gifts[index]);
            }
            else // Swap Data
            {
                DataMysteryGift s1 = mga.Gifts[index];
                DataMysteryGift s2 = mga.Gifts[wc_slot];

                if (s2 is PCD && s1 is PGT)
                {
                    // set the PGT to the PGT slot instead
                    ViewGiftData(s2);
                    ClickSet(pba[index], EventArgs.Empty);
                    { WinFormsUtil.Alert(string.Format(MsgMysteryGiftSlotAlternate, s2.Type, s1.Type)); return; }
                }
                if (s1.Type != s2.Type)
                { WinFormsUtil.Alert(string.Format(MsgMysteryGiftSlotFailSwap, s2.Type, s1.Type)); return; }
                mga.Gifts[wc_slot] = s1;
                mga.Gifts[index] = s2;

                if (mga.Gifts[wc_slot].Empty) // empty slot created, slide down
                {
                    int i = wc_slot;
                    while (i < index)
                    {
                        if (mga.Gifts[i + 1].Empty)
                            break;
                        if (mga.Gifts[i + 1].Type != mga.Gifts[i].Type)
                            break;

                        i++;

                        var mg1 = mga.Gifts[i];
                        var mg2 = mga.Gifts[i - 1];

                        mga.Gifts[i - 1] = mg1;
                        mga.Gifts[i] = mg2;
                    }
                    index = i-1;
                }
            }
            SetBackground(index, Drawing.Properties.Resources.slotView);
            SetGiftBoxes();
        }

        private static void BoxSlot_DragEnter(object sender, DragEventArgs e)
        {
            if (e.AllowedEffect == (DragDropEffects.Copy | DragDropEffects.Link)) // external file
                e.Effect = DragDropEffects.Copy;
            else if (e.Data != null) // within
                e.Effect = DragDropEffects.Move;
        }

        private int wc_slot = -1;

        // UI Generation
        private List<PictureBox> PopulateViewGiftsG4()
        {
            List<PictureBox> pb = new();

            // Row 1
            var f1 = GetFlowLayoutPanel();
            f1.Controls.Add(GetLabel($"{nameof(PGT)} 1-6"));
            for (int i = 0; i < 6; i++)
            {
                var p = GetPictureBox();
                f1.Controls.Add(p);
                pb.Add(p);
            }
            // Row 2
            var f2 = GetFlowLayoutPanel();
            f2.Controls.Add(GetLabel($"{nameof(PGT)} 7-8"));
            for (int i = 6; i < 8; i++)
            {
                var p = GetPictureBox();
                f2.Controls.Add(p);
                pb.Add(p);
            }
            // Row 3
            var f3 = GetFlowLayoutPanel();
            f3.Margin = new Padding(0, f3.Height, 0, 0);
            f3.Controls.Add(GetLabel($"{nameof(PCD)} 1-3"));
            for (int i = 8; i < 11; i++)
            {
                var p = GetPictureBox();
                f3.Controls.Add(p);
                pb.Add(p);
            }

            FLP_Gifts.Controls.Add(f1);
            FLP_Gifts.Controls.Add(f2);
            FLP_Gifts.Controls.Add(f3);
            return pb;
        }

        private List<PictureBox> PopulateViewGiftsG567()
        {
            var pb = new List<PictureBox>();

            const int cellsPerRow = 6;
            int rows = (int)Math.Ceiling(mga.Gifts.Length / (decimal)cellsPerRow);
            int countRemaining = mga.Gifts.Length;

            for (int i = 0; i < rows; i++)
            {
                var row = GetFlowLayoutPanel();
                int count = cellsPerRow >= countRemaining ? countRemaining : cellsPerRow;
                countRemaining -= count;
                int start = (i * cellsPerRow) + 1;
                row.Controls.Add(GetLabel($"{start}-{start + count - 1}"));
                for (int j = 0; j < count; j++)
                {
                    var p = GetPictureBox();
                    row.Controls.Add(p);
                    pb.Add(p);
                }
                FLP_Gifts.Controls.Add(row);
            }
            return pb;
        }

        private static FlowLayoutPanel GetFlowLayoutPanel()
        {
            return new()
            {
                Width = 480,
                Height = 60,
                Padding = new Padding(0),
                Margin = new Padding(0),
            };
        }

        private static Label GetLabel(string text)
        {
            return new()
            {
                Size = new Size(40, 60),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleRight,
                Text = text,
                Padding = new Padding(0),
                Margin = new Padding(0),
            };
        }

        private static PictureBox GetPictureBox()
        {
            return new()
            {
                Size = new Size(70, 58),
                SizeMode = PictureBoxSizeMode.CenterImage,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.Transparent,
                Padding = new Padding(0),
                Margin = new Padding(1),
            };
        }

        private void B_ModifyAll_Click(object sender, EventArgs e)
        {
            foreach (var g in mga.Gifts)
                g.GiftUsed = sender == B_UsedAll;
            SetGiftBoxes();
            System.Media.SystemSounds.Asterisk.Play();
        }

        private void LB_Received_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (LB_Received.SelectedIndices.Count > 1)
                {
                    for (int i = LB_Received.SelectedIndices.Count - 1; i >= 0; i--)
                        LB_Received.Items.RemoveAt(LB_Received.SelectedIndices[i]);
                }
                else if (LB_Received.SelectedIndices.Count == 1)
                {
                    int lastIndex = LB_Received.SelectedIndex;
                    LB_Received.Items.RemoveAt(lastIndex);
                    if (LB_Received.Items.Count == 0)
                        return;
                    if (lastIndex == LB_Received.Items.Count)
                        lastIndex--;
                    LB_Received.SelectedIndex = lastIndex;
                }
            }
        }
    }
}