using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SAV_Wondercard : Form
    {
        public SAV_Wondercard(MysteryGift g = null)
        {
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.curlanguage);
            mga = Main.SAV.GiftAlbum;
            
            switch (SAV.Generation)
            {
                case 4:
                    pba = popG4Gifts().ToArray();
                    break;
                case 5:
                case 6:
                case 7:
                    pba = popG567Gifts().ToArray();
                    break;
                default:
                    throw new ArgumentException("Game not supported.");
            }

            foreach (PictureBox pb in pba)
            {
                pb.AllowDrop = true;
                pb.DragDrop += pbBoxSlot_DragDrop;
                pb.DragEnter += pbBoxSlot_DragEnter;
                pb.MouseDown += pbBoxSlot_MouseDown;
                pb.ContextMenuStrip = mnuVSD;
            }

            setGiftBoxes();
            getReceivedFlags();
            
            if (LB_Received.Items.Count > 0)
                LB_Received.SelectedIndex = 0;

            DragEnter += tabMain_DragEnter;
            DragDrop += tabMain_DragDrop;

            if (g == null)
                clickView(pba[0], null);
            else
                viewGiftData(g);
        }
        
        private readonly SaveFile SAV = Main.SAV.Clone();
        private MysteryGiftAlbum mga;
        private MysteryGift mg;
        private readonly PictureBox[] pba;

        // Repopulation Functions
        private void setBackground(int index, Image bg)
        {
            for (int i = 0; i < mga.Gifts.Length; i++)
                pba[i].BackgroundImage = index == i ? bg : null;
        }
        private void setGiftBoxes()
        {
            for (int i = 0; i < mga.Gifts.Length; i++)
            {
                MysteryGift m = mga.Gifts[i];
                pba[i].Image = m.Sprite();
            }
        }
        private void viewGiftData(MysteryGift g)
        {
            try
            {
                // only check if the form is visible (not opening)
                if (Visible && g.GiftUsed && DialogResult.Yes ==
                        WinFormsUtil.Prompt(MessageBoxButtons.YesNo,
                            "Wonder Card is marked as USED and will not be able to be picked up in-game.",
                            "Do you want to remove the USED flag so that it is UNUSED?"))
                    g.GiftUsed = false;

                RTB.Text = getDescription(g);
                PB_Preview.Image = g.Sprite();
                mg = g;
            }
            catch (Exception e)
            {
                WinFormsUtil.Error("Loading of data failed... is this really a Wonder Card?", e);
                RTB.Clear();
            }
        }
        private void getReceivedFlags()
        {
            LB_Received.Items.Clear();
            for (int i = 1; i < mga.Flags.Length; i++)
                if (mga.Flags[i])
                    LB_Received.Items.Add(i.ToString("0000"));

            if (LB_Received.Items.Count > 0)
                LB_Received.SelectedIndex = 0;
        }
        private void setCardID(int cardID)
        {
            if (cardID <= 0 || cardID >= 0x100 * 8) return;

            string card = cardID.ToString("0000");
            if (!LB_Received.Items.Contains(card))
                LB_Received.Items.Add(card);
            LB_Received.SelectedIndex = LB_Received.Items.IndexOf(card);
        }

        // Mystery Gift IO (.file<->window)
        private void B_Import_Click(object sender, EventArgs e)
        {
            OpenFileDialog import = new OpenFileDialog {Filter = WinFormsUtil.getMysterGiftFilter(SAV.Generation)};
            if (import.ShowDialog() != DialogResult.OK) return;

            string path = import.FileName;
            MysteryGift g = MysteryGift.getMysteryGift(File.ReadAllBytes(path), Path.GetExtension(path));
            if (g == null)
            {
                WinFormsUtil.Error("File is not a Mystery Gift:", path);
                return;
            }
            viewGiftData(g);
        }
        private void B_Output_Click(object sender, EventArgs e)
        {
            WinFormsUtil.SaveMGDialog(mg);
        }

        private int getLastUnfilledByType(MysteryGift Gift, MysteryGiftAlbum Album)
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
        private void clickView(object sender, EventArgs e)
        {
            sender = ((sender as ToolStripItem)?.Owner as ContextMenuStrip)?.SourceControl ?? sender as PictureBox;
            int index = Array.IndexOf(pba, sender);

            setBackground(index, Properties.Resources.slotView);
            viewGiftData(mga.Gifts[index]);
        }
        private void clickSet(object sender, EventArgs e)
        {
            if (!checkSpecialWonderCard(mg))
                return;

            sender = ((sender as ToolStripItem)?.Owner as ContextMenuStrip)?.SourceControl ?? sender as PictureBox;
            int index = Array.IndexOf(pba, sender);

            // Hijack to the latest unfilled slot if index creates interstitial empty slots.
            int lastUnfilled = getLastUnfilledByType(mg, mga);
            if (lastUnfilled > -1 && lastUnfilled < index)
                index = lastUnfilled;

            if (mg is PCD && mga.Gifts[index] is PGT)
                mg = (mg as PCD).Gift;
            else if (mg.Type != mga.Gifts[index].Type)
            {
                WinFormsUtil.Alert("Can't set slot here.", $"{mg.Type} != {mga.Gifts[index].Type}");
                return;
            }
            setBackground(index, Properties.Resources.slotSet);
            mga.Gifts[index] = mg.Clone();
            setGiftBoxes();
            setCardID(mg.CardID);
        }
        private void clickDelete(object sender, EventArgs e)
        {
            sender = ((sender as ToolStripItem)?.Owner as ContextMenuStrip)?.SourceControl ?? sender as PictureBox;
            int index = Array.IndexOf(pba, sender);

            mga.Gifts[index].Data = new byte[mga.Gifts[index].Data.Length];

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
            setBackground(i, Properties.Resources.slotDel);
            setGiftBoxes();
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
                flags[Util.ToUInt32(o.ToString())] = true;

            mga.Flags = flags;
            SAV.GiftAlbum = mga;

            Main.SAV.Data = SAV.Data;
            Main.SAV.Edited = true;
            Close();
        }

        // Delete Received Flag
        private void clearRecievedFlag(object sender, EventArgs e)
        {
            if (LB_Received.SelectedIndex < 0) return;

            if (LB_Received.Items.Count > 0)
                LB_Received.Items.Remove(LB_Received.Items[LB_Received.SelectedIndex]);
            if (LB_Received.Items.Count > 0)
                LB_Received.SelectedIndex = 0;
        }

        // Drag & Drop Wonder Cards
        private void tabMain_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }
        private void tabMain_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            // Check for multiple wondercards
            if (Directory.Exists(files[0]))
                files = Directory.GetFiles(files[0], "*", SearchOption.AllDirectories);
            if (files.Length == 1 && !Directory.Exists(files[0]))
            {
                string path = files[0]; // open first D&D
                long len = new FileInfo(path).Length;
                if (len > 0x1000) // arbitrary
                {
                    WinFormsUtil.Alert("File is not a Mystery Gift.", path);
                    return;
                }
                MysteryGift g = MysteryGift.getMysteryGift(File.ReadAllBytes(path), Path.GetExtension(path));
                if (g == null)
                {
                    WinFormsUtil.Error("File is not a Mystery Gift:", path);
                    return;
                }
                viewGiftData(g);
                return;
            }
            setGiftBoxes();
        }

        private bool checkSpecialWonderCard(MysteryGift g)
        {
            if (SAV.Generation != 6)
                return true;

            if (g is WC6)
            {
                if (g.CardID == 2048 && g.Item == 726) // Eon Ticket (OR/AS)
                {
                    if (!Main.SAV.ORAS || ((SAV6)SAV).EonTicket < 0)
                        goto reject;
                    BitConverter.GetBytes(WC6.EonTicketConst).CopyTo(SAV.Data, ((SAV6)SAV).EonTicket);
                }
            }

            return true;
            reject: WinFormsUtil.Alert("Unable to insert the Mystery Gift.", "Does this Mystery Gift really belong to this game?");
            return false;
        }
        
        private void L_QR_Click(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Alt)
            {
                byte[] data = QR.getQRData();
                if (data == null) return;

                string[] types = mga.Gifts.Select(g => g.Type).Distinct().ToArray();
                MysteryGift gift = MysteryGift.getMysteryGift(data);
                string giftType = gift.Type;

                if (mga.Gifts.All(card => card.Data.Length != data.Length))
                    WinFormsUtil.Alert("Decoded data not valid for loaded save file.", $"QR Data Size: 0x{data.Length:X}");
                else if (types.All(type => type != giftType))
                    WinFormsUtil.Alert("Gift type is not compatible with the save file.", $"QR Gift Type: {gift.Type}" + Environment.NewLine + $"Expected Types: {string.Join(", ", types)}");
                else if (gift.Species > SAV.MaxSpeciesID || gift.Moves.Any(move => move > SAV.MaxMoveID) || gift.HeldItem > SAV.MaxItemID)
                    WinFormsUtil.Alert("Gift Details are not compatible with the save file.");
                else
                    try { viewGiftData(gift); }
                    catch { WinFormsUtil.Alert("Error loading Mystery Gift data."); }
            }
            else
            {
                if (mg.Data.SequenceEqual(new byte[mg.Data.Length]))
                { WinFormsUtil.Alert("No wondercard data found in loaded slot!"); return; }
                if (SAV.Generation == 6 && mg.Item == 726 && mg.IsItem)
                { WinFormsUtil.Alert("Eon Ticket Wonder Cards will not function properly", "Inject to the save file instead."); return; }

                const string server = "http://lunarcookies.github.io/wc.html#";
                Image qr = QR.getQRImage(mg.Data, server);
                if (qr == null) return;

                string desc = $"({mg.Type}) {getDescription(mg)}";

                new QR(qr, PB_Preview.Image, desc + "PKHeX Wonder Card @ ProjectPokemon.org", "", "", "").ShowDialog();
            }
        }

        private void pbBoxSlot_MouseDown(object sender, MouseEventArgs e)
        {
            switch (ModifierKeys)
            {
                case Keys.Control: clickView(sender, e); return;
                case Keys.Shift: clickSet(sender, e); return;
                case Keys.Alt: clickDelete(sender, e); return;
            }
            PictureBox pb = sender as PictureBox;
            if (pb?.Image == null)
                return;

            if (e.Button != MouseButtons.Left || e.Clicks != 1) return;

            int index = Array.IndexOf(pba, sender);
            wc_slot = index;
            // Create Temp File to Drag
            Cursor.Current = Cursors.Hand;

            // Prepare Data
            MysteryGift card = mga.Gifts[index];
            string filename = Util.CleanFileName($"{card.CardID:0000} - {card.CardTitle}.{card.Extension}");

            // Make File
            string newfile = Path.Combine(Path.GetTempPath(), Util.CleanFileName(filename));
            try
            {
                File.WriteAllBytes(newfile, card.Data);
                DoDragDrop(new DataObject(DataFormats.FileDrop, new[] { newfile }), DragDropEffects.Move);
            }
            catch (Exception x)
            { WinFormsUtil.Error("Drag & Drop Error", x); }
            File.Delete(newfile);
            wc_slot = -1;
        }
        private void pbBoxSlot_DragDrop(object sender, DragEventArgs e)
        {
            int index = Array.IndexOf(pba, sender);

            // Hijack to the latest unfilled slot if index creates interstitial empty slots.
            int lastUnfilled = getLastUnfilledByType(mg, mga);
            if (lastUnfilled > -1 && lastUnfilled < index && mga.Gifts[lastUnfilled].Type == mga.Gifts[index].Type)
                index = lastUnfilled;
            
            if (wc_slot == -1) // dropped
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files.Length < 1)
                    return;
                if (PCD.Size < (int)new FileInfo(files[0]).Length)
                { WinFormsUtil.Alert("Data size invalid.", files[0]); return; }
                
                byte[] data = File.ReadAllBytes(files[0]);
                MysteryGift gift = MysteryGift.getMysteryGift(data, new FileInfo(files[0]).Extension);

                if (gift is PCD && mga.Gifts[index] is PGT)
                    gift = (gift as PCD).Gift;
                else if (gift.Type != mga.Gifts[index].Type)
                {
                    WinFormsUtil.Alert("Can't set slot here.", $"{gift.Type} != {mga.Gifts[index].Type}");
                    return;
                }
                setBackground(index, Properties.Resources.slotSet);
                mga.Gifts[index] = gift.Clone();
                
                setCardID(mga.Gifts[index].CardID);
                viewGiftData(mga.Gifts[index]);
            }
            else // Swap Data
            {
                MysteryGift s1 = mga.Gifts[index];
                MysteryGift s2 = mga.Gifts[wc_slot];

                if (s2 is PCD && s1 is PGT)
                {
                    // set the PGT to the PGT slot instead
                    viewGiftData(s2);
                    clickSet(pba[index], null);
                    { WinFormsUtil.Alert($"Set {s2.Type} gift to {s1.Type} slot."); return; }
                }
                if (s1.Type != s2.Type)
                { WinFormsUtil.Alert($"Can't swap {s2.Type} with {s1.Type}."); return; }
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
            setBackground(index, Properties.Resources.slotView);
            setGiftBoxes();
        }
        private void pbBoxSlot_DragEnter(object sender, DragEventArgs e)
        {
            if (e.AllowedEffect == (DragDropEffects.Copy | DragDropEffects.Link)) // external file
                e.Effect = DragDropEffects.Copy;
            else if (e.Data != null) // within
                e.Effect = DragDropEffects.Move;
        }
        private int wc_slot = -1;
        private static string getDescription(MysteryGift gift)
        {
            if (gift.Empty)
                return "Empty Slot. No data!";

            string s = gift.getCardHeader() + Environment.NewLine;
            if (gift.IsItem)
            {
                s += $"Item: {GameInfo.Strings.itemlist[gift.Item]} (Quantity: {gift.Quantity})" + Environment.NewLine;
                if (gift is WC7)
                {
                    var ind = 1;
                    var wc7 = (WC7) gift;
                    while (wc7.GetItem(ind) != 0)
                    {
                        s += $"Item: {GameInfo.Strings.itemlist[wc7.GetItem(ind)]} (Quantity: {wc7.GetQuantity(ind)})" + Environment.NewLine;
                        ind++;
                    }
                }
            }
            else if (gift.IsPokémon)
            {
                PKM pk = gift.convertToPKM(Main.SAV);

                try
                {
                    s += $"{GameInfo.Strings.specieslist[pk.Species]} @ {GameInfo.Strings.itemlist[pk.HeldItem]}  --- ";
                    s += (pk.IsEgg ? GameInfo.Strings.eggname : $"{pk.OT_Name} - {pk.TID:00000}/{pk.SID:00000}") + Environment.NewLine;
                    s += $"{GameInfo.Strings.movelist[pk.Move1]} / {GameInfo.Strings.movelist[pk.Move2]} / {GameInfo.Strings.movelist[pk.Move3]} / {GameInfo.Strings.movelist[pk.Move4]}" + Environment.NewLine;
                    if (gift is WC7)
                    {
                        var addItem = ((WC7) gift).AdditionalItem;
                        if (addItem != 0)
                            s += $"+ {GameInfo.Strings.itemlist[addItem]}" + Environment.NewLine;
                    }
                }
                catch { s += "Unable to create gift description." + Environment.NewLine; }
            }
            else if (gift.IsBP)
            {
                s += "BP: " + gift.BP + Environment.NewLine;
            }
            else if (gift.IsBean)
            {
                s += "Bean ID: " + gift.Bean + Environment.NewLine + "Quantity: " + gift.Quantity + Environment.NewLine;
            }
            else { s += "Unknown Wonder Card Type!" + Environment.NewLine; }
            if (gift is WC7)
            {
                var wc7 = (WC7) gift;
                s += $"Repeatable: {wc7.GiftRepeatable}" + Environment.NewLine;
                s += $"Collected: {wc7.GiftUsed}" + Environment.NewLine;
                s += $"Once Per Day: {wc7.GiftOncePerDay}" + Environment.NewLine;
            }
            return s;
        }

        // UI Generation
        private List<PictureBox> popG4Gifts()
        {
            List<PictureBox> pb = new List<PictureBox>();

            // Row 1
            var f1 = getFlowLayoutPanel();
            f1.Controls.Add(getLabel("PGT 1-6"));
            for (int i = 0; i < 6; i++)
            {
                var p = getPictureBox();
                f1.Controls.Add(p);
                pb.Add(p);
            }
            // Row 2
            var f2 = getFlowLayoutPanel();
            f2.Controls.Add(getLabel("PGT 7-8"));
            for (int i = 6; i < 8; i++)
            {
                var p = getPictureBox();
                f2.Controls.Add(p);
                pb.Add(p);
            }
            // Row 3
            var f3 = getFlowLayoutPanel();
            f3.Margin = new Padding(0, f3.Height, 0, 0);
            f3.Controls.Add(getLabel("PCD 1-3"));
            for (int i = 8; i < 11; i++)
            {
                var p = getPictureBox();
                f3.Controls.Add(p);
                pb.Add(p);
            }

            FLP_Gifts.Controls.Add(f1);
            FLP_Gifts.Controls.Add(f2);
            FLP_Gifts.Controls.Add(f3);
            return pb;
        }
        private List<PictureBox> popG567Gifts()
        {
            List<PictureBox> pb = new List<PictureBox>();

            for (int i = 0; i < mga.Gifts.Length / 6; i++)
            {
                var flp = getFlowLayoutPanel();
                flp.Controls.Add(getLabel($"{i * 6 + 1}-{i * 6 + 6}"));
                for (int j = 0; j < 6; j++)
                {
                    var p = getPictureBox();
                    flp.Controls.Add(p);
                    pb.Add(p);
                }
                FLP_Gifts.Controls.Add(flp);
            }
            return pb;
        }
        private static FlowLayoutPanel getFlowLayoutPanel()
        {
            return new FlowLayoutPanel
            {
                Width = 305,
                Height = 34,
                Padding = new Padding(0),
                Margin = new Padding(0),
            };
        }
        private static Label getLabel(string text)
        {
            return new Label
            {
                Size = new Size(40, 34),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleRight,
                Text = text,
                Padding = new Padding(0),
                Margin = new Padding(0),
            };
        }
        private static PictureBox getPictureBox()
        {
            return new PictureBox
            {
                Size = new Size(42, 32),
                SizeMode = PictureBoxSizeMode.CenterImage,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.Transparent,
                Padding = new Padding(0),
                Margin = new Padding(1),
            };
        }
    }
}