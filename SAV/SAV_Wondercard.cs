using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace PKHeX
{
    public partial class SAV_Wondercard : Form
    {
        public SAV_Wondercard(MysteryGift g = null)
        {
            InitializeComponent();
            Util.TranslateInterface(this, Main.curlanguage);
            mga = Main.SAV.GiftAlbum;
            pba = new[]
            {
                PB_Card01, PB_Card02, PB_Card03, PB_Card04, PB_Card05, PB_Card06,
                PB_Card07, PB_Card08, PB_Card09, PB_Card10, PB_Card11, PB_Card12,
                PB_Card13, PB_Card14, PB_Card15, PB_Card16, PB_Card17, PB_Card18,
                PB_Card19, PB_Card20, PB_Card21, PB_Card22, PB_Card23, PB_Card24,
            };

            foreach (PictureBox pb in pba)
            {
                pb.AllowDrop = true;
                // The PictureBoxes have their own drag&drop event handlers.
            }

            // Hide slots not present on game
            for (int i = mga.Gifts.Length; i < pba.Length; i++)
                pba[i].Visible = false;
            if (mga.Gifts.Length < 7)
                L_r2.Visible = false;
            if (mga.Gifts.Length < 13)
                L_r3.Visible = false;
            if (mga.Gifts.Length < 19)
                L_r4.Visible = false;

            setGiftBoxes();
            getReceivedFlags();
            
            if (LB_Received.Items.Count > 0)
                LB_Received.SelectedIndex = 0;

            DragEnter += tabMain_DragEnter;
            DragDrop += tabMain_DragDrop;

            if (g == null)
                clickView(PB_Card01, null);
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
                pba[i].Image = m.Empty ? null : getSprite(m);
            }
        }
        private void viewGiftData(MysteryGift g)
        {
            try
            {
                if (g.GiftUsed && DialogResult.Yes ==
                        Util.Prompt(MessageBoxButtons.YesNo,
                            "Wonder Card is marked as USED and will not be able to be picked up in-game.",
                            "Do you want to remove the USED flag so that it is UNUSED?"))
                    g.GiftUsed = false;

                RTB.Text = getDescription(g);
                PB_Preview.Image = getSprite(g);
                mg = g;
            }
            catch (Exception e)
            {
                Util.Error("Loading of data failed... is this really a Wonder Card?", e.ToString());
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

        // Wonder Card IO (.wc6<->window)
        private string getFilter()
        {
            switch (SAV.Generation)
            {
                case 4:
                    return "Gen4 Mystery Gift|*.pgt;*.pcd|All Files|*.*";
                case 5:
                    return "Gen5 Mystery Gift|*.pgf|All Files|*.*";
                case 6:
                    return "Gen6 Mystery Gift|*.wc6;*.wc6full|All Files|*.*";
                default:
                    return "";
            }
        }
        private void B_Import_Click(object sender, EventArgs e)
        {
            OpenFileDialog import = new OpenFileDialog {Filter = getFilter()};
            if (import.ShowDialog() != DialogResult.OK) return;

            string path = import.FileName;
            MysteryGift g = MysteryGift.getMysteryGift(File.ReadAllBytes(path), Path.GetExtension(path));
            if (g == null)
            {
                Util.Error("File is not a Mystery Gift:", path);
                return;
            }
            viewGiftData(g);
        }
        private void B_Output_Click(object sender, EventArgs e)
        {
            SaveFileDialog outputwc6 = new SaveFileDialog
            {
                Filter = getFilter(),
                FileName = Util.CleanFileName($"{mg.CardID} - {mg.CardTitle}{mg.Extension}")
            };
            if (outputwc6.ShowDialog() != DialogResult.OK) return;

            string path = outputwc6.FileName;

            if (File.Exists(path)) // File already exists, save a .bak
                File.WriteAllBytes(path + ".bak", File.ReadAllBytes(path));

            File.WriteAllBytes(path, mg.Data);
        }

        // Wonder Card RW (window<->sav)
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
            int lastUnfilled = Array.FindIndex(pba, p => p.Image == null);
            if (lastUnfilled > -1 && lastUnfilled < index)
                index = lastUnfilled;
            if (mg.Data.Length != mga.Gifts[index].Data.Length)
            {
                Util.Alert("Can't set slot here.", $"{mg.GetType()} != {mga.Gifts[index].GetType()}");
                return;
            }
            setBackground(index, Properties.Resources.slotSet);
            mga.Gifts[index] = mg;
            setGiftBoxes();
            setCardID(mg.CardID);
        }
        private void clickDelete(object sender, EventArgs e)
        {
            sender = ((sender as ToolStripItem)?.Owner as ContextMenuStrip)?.SourceControl ?? sender as PictureBox;
            int index = Array.IndexOf(pba, sender);

            setBackground(index, Properties.Resources.slotDel);
            mga.Gifts[index].Data = new byte[mga.Gifts[index].Data.Length];
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

        // Delete WC Flag
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
                    Util.Alert("File is not a Mystery Gift.", path);
                    return;
                }
                MysteryGift g = MysteryGift.getMysteryGift(File.ReadAllBytes(path), Path.GetExtension(path));
                if (g == null)
                {
                    Util.Error("File is not a Mystery Gift:", path);
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
            reject: Util.Alert("Unable to insert the Mystery Gift.", "Does this Mystery Gift really belong to this game?");
            return false;
        }
        
        private void L_QR_Click(object sender, EventArgs e)
        {
            if (SAV.Generation != 6)
            {
                Util.Alert("Feature not available for non Gen6 games.");
                return;
            }
            if (ModifierKeys == Keys.Alt)
            {
                byte[] data = QR.getQRData();
                if (data == null) return;
                if (data.Length != WC6.Size) { Util.Alert($"Decoded data not 0x{WC6.Size.ToString("X")} bytes.",
                    $"QR Data Size: 0x{data.Length.ToString("X")}"); }
                else try { viewGiftData(new WC6(data)); }
                catch { Util.Alert("Error loading wondercard data."); }
            }
            else
            {
                if (mg.Data.SequenceEqual(new byte[mg.Data.Length]))
                { Util.Alert("No wondercard data found in loaded slot!"); return; }
                if (mg.Item == 726 && mg.IsItem)
                { Util.Alert("Eon Ticket Wonder Cards will not function properly", "Inject to the save file instead."); return; }

                const string server = "http://lunarcookies.github.io/wc.html#";
                Image qr = QR.getQRImage(mg.Data, server);
                if (qr == null) return;

                string desc = getDescription(mg);

                new QR(qr, PB_Preview.Image, desc, "", "", "PKHeX Wonder Card @ ProjectPokemon.org").ShowDialog();
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
            string filename = Util.CleanFileName($"{card.CardID.ToString("0000")} - {card.CardTitle}.wc6");

            // Make File
            string newfile = Path.Combine(Path.GetTempPath(), Util.CleanFileName(filename));
            try
            {
                File.WriteAllBytes(newfile, card.Data);
                DoDragDrop(new DataObject(DataFormats.FileDrop, new[] { newfile }), DragDropEffects.Move);
            }
            catch (ArgumentException x)
            { Util.Error("Drag & Drop Error:", x.ToString()); }
            File.Delete(newfile);
            wc_slot = -1;
        }
        private void pbBoxSlot_DragDrop(object sender, DragEventArgs e)
        {
            int index = Array.IndexOf(pba, sender);

            // Hijack to the latest unfilled slot if index creates interstitial empty slots.
            int lastUnfilled = Array.FindIndex(pba, p => p.Image == null);
            if (lastUnfilled < index)
                index = lastUnfilled;
            
            if (wc_slot == -1) // dropped
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files.Length < 1)
                    return;
                if (PCD.Size < (int)new FileInfo(files[0]).Length)
                { Util.Alert("Data size invalid.", files[0]); return; }
                
                byte[] data = File.ReadAllBytes(files[0]);
                chk:
                if (data.Length != mga.Gifts[index].Data.Length)
                {
                    if (index < 8)
                    {
                        index = 8;
                        goto chk;
                    }
                    { Util.Alert("Can't set slot here.", $"{data.Length} != {mga.Gifts[index].Data.Length}, {mga.Gifts[index].GetType()}", files[0]); return; }
                }

                mga.Gifts[index].Data = data;
                setCardID(mga.Gifts[index].CardID);
                viewGiftData(mga.Gifts[index]);
            }
            else // Swap Data
            {
                // Check to see if they copied beyond blank slots.
                if (index > Math.Max(wc_slot, lastUnfilled - 1))
                    index = Math.Max(wc_slot, lastUnfilled - 1);

                MysteryGift s1 = mga.Gifts[index];
                MysteryGift s2 = mga.Gifts[wc_slot];

                if (s1.Data.Length != s2.Data.Length)
                { Util.Alert("Can't swap these two slots."); return; }
                mga.Gifts[wc_slot] = s1;
                mga.Gifts[index] = s2;
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

        private static Image getSprite(MysteryGift gift)
        {
            Image img;
            if (gift.IsPokémon)
                img = PKX.getSprite(gift.convertToPKM(Main.SAV));
            else if (gift.IsItem)
                img = (Image)(Properties.Resources.ResourceManager.GetObject("item_" + gift.Item) ?? Properties.Resources.unknown);
            else
                img = Properties.Resources.unknown;

            if (gift.GiftUsed)
                img = Util.LayerImage(new Bitmap(img.Width, img.Height), img, 0, 0, 0.3);
            return img;
        }
        private static string getDescription(MysteryGift gift)
        {
            if (gift.Empty)
                return "Empty Slot. No data!";

            string s = gift.getCardHeader();
            if (gift.IsItem)
            {
                s += "Item: " + Main.itemlist[gift.Item] + Environment.NewLine + "Quantity: " + gift.Quantity;
                return s;
            }
            if (gift.IsPokémon)
            {
                PKM pk = gift.convertToPKM(Main.SAV);

                try
                {
                    s += $"{Main.specieslist[pk.Species]} @ {Main.itemlist[pk.HeldItem]}  --- ";
                    s += (pk.IsEgg ? Main.eggname : $"{pk.OT_Name} - {pk.TID.ToString("00000")}/{pk.SID.ToString("00000")}") + Environment.NewLine;
                    s += $"{Main.movelist[pk.Move1]} / {Main.movelist[pk.Move2]} / {Main.movelist[pk.Move3]} / {Main.movelist[pk.Move4]}" + Environment.NewLine;
                }
                catch { s += "Unable to create gift description."; }
                return s;
            }
            s += "Unknown Wonder Card Type!";
            return s;
        }
    }
}