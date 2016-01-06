using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PKHeX
{
    public partial class SAV_Wondercard : Form
    {
        public SAV_Wondercard(byte[] wcdata = null)
        {
            InitializeComponent();
            Util.TranslateInterface(this, Main.curlanguage);
            sav = (byte[])Main.SAV.Data.Clone();
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
            populateWClist();
            populateReceived();
            
            if (LB_Received.Items.Count > 0)
                LB_Received.SelectedIndex = 0;

            DragEnter += tabMain_DragEnter;
            DragDrop += tabMain_DragDrop;

            if (wcdata == null || wcdata.Length != WC6.Size)
            { 
                // No data to load, load first wc
                clickView(pba[0], null);
                return;
            }
            
            Array.Copy(wcdata, wondercard_data, wcdata.Length);
            loadwcdata();
        }
        public byte[] sav;
        public byte[] wondercard_data = new byte[WC6.Size];
        private const uint EonTicketConst = 0x225D73C2;
        private PictureBox[] pba;

        // Repopulation Functions
        private int currentSlot;
        private void setBackground(int index, Image bg)
        {
            for (int i = 0; i < 24; i++)
                pba[i].BackgroundImage = index == i ? bg : null;
            currentSlot = index;
        }
        private void removeEmptyWC6s()
        {
            byte[][] ca = new byte[24][];
            for (int i = 0; i < ca.Length; i++)
                ca[i] = sav.Skip(Main.SAV.WondercardData + WC6.Size*i).Take(WC6.Size).ToArray();

            ca = ca.Where(c => BitConverter.ToUInt16(c, 0) > 0).ToArray();
            for (int i = 0; i < ca.Length; i++)
                ca[i].CopyTo(sav, Main.SAV.WondercardData + WC6.Size * i);
            for (int i = ca.Length; i < 24; i++)
                new byte[WC6.Size].CopyTo(sav, Main.SAV.WondercardData + WC6.Size * i);
        }
        private void populateWClist()
        {
            removeEmptyWC6s();
            for (int i = 0; i < 24; i++)
            {
                int offset = Main.SAV.WondercardData + i * WC6.Size;
                int cardID = BitConverter.ToUInt16(sav, offset);
                pba[i].Image = cardID == 0 ? null : getWCPreviewImage(sav.Skip(Main.SAV.WondercardData + WC6.Size * i).Take(WC6.Size).ToArray());
            }
        }
        private void loadwcdata()
        {
            try
            {
                if ((wondercard_data[0x52] & 2) != 0) // is used
                {
                    if (DialogResult.Yes !=
                        Util.Prompt(MessageBoxButtons.YesNo,
                            "Wonder Card is marked as USED and will not be able to be picked up in-game.",
                            "Do you want to remove the USED flag so that it is UNUSED?"))
                        return;

                    wondercard_data[0x52] &= 0xFD; // keep all bits but bit1 (11111101)
                }
                RTB.Text = getWCDescriptionString(wondercard_data);
                PB_Preview.Image = getWCPreviewImage(wondercard_data);
            }
            catch (Exception e)
            {
                Util.Error("Loading of data failed... is this really a Wonder Card?", e.ToString());
                Array.Copy(new byte[WC6.Size], wondercard_data, WC6.Size);
                RTB.Clear();
            }
        }
        private void populateReceived()
        {
            LB_Received.Items.Clear();
            for (int i = 1; i < 2048; i++)
                if ((((sav[Main.SAV.WondercardFlags + i / 8]) >> (i % 8)) & 0x1) == 1)
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
        private void B_Import_Click(object sender, EventArgs e)
        {
            OpenFileDialog importwc6 = new OpenFileDialog {Filter = "Wonder Card|*.wc6"};
            if (importwc6.ShowDialog() != DialogResult.OK) return;

            string path = importwc6.FileName;
            if (new FileInfo(path).Length != WC6.Size)
            {
                Util.Error("File is not a Wonder Card:", path);
                return;
            }
            byte[] newwc6 = File.ReadAllBytes(path);
            Array.Copy(newwc6, wondercard_data, newwc6.Length);
            loadwcdata();
        }
        private void B_Output_Click(object sender, EventArgs e)
        {
            SaveFileDialog outputwc6 = new SaveFileDialog();
            int cardID = BitConverter.ToUInt16(wondercard_data, 0);
            string cardname = Encoding.Unicode.GetString(wondercard_data, 0x2, 0x48);
            outputwc6.FileName = Util.CleanFileName(cardID + " - " + cardname + ".wc6");
            outputwc6.Filter = "Wonder Card|*.wc6";
            if (outputwc6.ShowDialog() != DialogResult.OK) return;

            string path = outputwc6.FileName;

            if (File.Exists(path))
            {
                // File already exists, save a .bak
                byte[] backupfile = File.ReadAllBytes(path);
                File.WriteAllBytes(path + ".bak", backupfile);
            }

            File.WriteAllBytes(path, wondercard_data);
        }

        // Wonder Card RW (window<->sav)
        private void clickView(object sender, EventArgs e)
        {
            string name = sender is ToolStripItem
                ? ((sender as ToolStripItem).Owner as ContextMenuStrip).SourceControl.Name
                : (sender as PictureBox).Name;

            int index = Array.FindIndex(pba, p => p.Name == name);

            setBackground(index, Properties.Resources.slotView);
            int offset = Main.SAV.WondercardData + index * WC6.Size;
            Array.Copy(sav, offset, wondercard_data, 0, WC6.Size);
            loadwcdata();
        }
        private void clickSet(object sender, EventArgs e)
        {
            string name = sender is ToolStripItem
                ? ((sender as ToolStripItem).Owner as ContextMenuStrip).SourceControl.Name
                : (sender as PictureBox).Name;

            int index = Array.FindIndex(pba, p => p.Name == name);
            // Hijack to the latest unfilled slot if index creates interstitial empty slots.
            int lastUnfilled = Array.FindIndex(pba, p => p.Image == null);
            if (lastUnfilled < index)
                index = lastUnfilled;

            setBackground(index, Properties.Resources.slotSet);
            int offset = Main.SAV.WondercardData + index * WC6.Size;
            if (Main.SAV.ORAS) // ORAS Only
                if (BitConverter.ToUInt16(wondercard_data, 0) == 0x800) // Eon Ticket #
                    if (BitConverter.ToUInt16(wondercard_data, 0x68) == 0x2D6) // Eon Ticket
                        BitConverter.GetBytes(EonTicketConst).CopyTo(sav, Main.SAV.EonTicket);
            Array.Copy(wondercard_data, 0, sav, offset, WC6.Size);
            populateWClist();
            setCardID(BitConverter.ToUInt16(wondercard_data, 0));

        }
        private void clickDelete(object sender, EventArgs e)
        {
            string name = sender is ToolStripItem
                ? ((sender as ToolStripItem).Owner as ContextMenuStrip).SourceControl.Name
                : (sender as PictureBox).Name;

            int index = Array.FindIndex(pba, p => p.Name == name);

            setBackground(index, Properties.Resources.slotDel);
            int offset = Main.SAV.WondercardData + index * WC6.Size;
            new byte[WC6.Size].CopyTo(sav, offset);
            populateWClist();
        }

        // Close Window
        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void B_Save_Click(object sender, EventArgs e)
        {
            int offset = Main.SAV.WondercardFlags;

            // Make sure all of the Received Flags are flipped!
            byte[] wcflags = new byte[0x100];
            foreach (uint cardnum in from object card in LB_Received.Items 
                                     select card.ToString() into cardID 
                                     select Util.ToUInt32(cardID))
                wcflags[(cardnum / 8) & 0xFF] |= (byte)(1 << ((byte)(cardnum & 0x7)));

            Array.Copy(wcflags, 0, sav, offset, 0x100);

            offset += 0x100;
            // Make sure there's no space between wondercards
            {
                for (int i = 0; i < 24; i++)
                    if (BitConverter.ToUInt16(sav, offset + i * WC6.Size) == 0)
                        for (int j = i + 1; j < 24 - i; j++) // Shift everything down
                            Array.Copy(sav, offset + j * WC6.Size, sav, offset + (j - 1) * WC6.Size, WC6.Size);
            }

            Array.Copy(sav, Main.SAV.Data, sav.Length);
            Main.SAV.Edited = true;
            Close();
        }

        // Delete WC Flag
        private void clearRecievedFlag(object sender, EventArgs e)
        {
            if (LB_Received.SelectedIndex <= -1) return;

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
            int ctr = currentSlot;

            if (files.Length == 1)
            {
                string path = files[0]; // open first D&D
                if (new FileInfo(path).Length != WC6.Size)
                {
                    Util.Error("File is not a Wonder Card:", path);
                    return;
                }
                byte[] newwc6 = File.ReadAllBytes(path);
                Array.Copy(newwc6, wondercard_data, newwc6.Length);
                loadwcdata();
            }
            else if (DialogResult.Yes == Util.Prompt(MessageBoxButtons.YesNo, String.Format("Try to load {0} Wonder Cards starting at Card {1}?", files.Length, ctr + 1)))
            {
                foreach (string file in files)
                {
                    if (new FileInfo(file).Length != WC6.Size)
                    { Util.Error("File is not a Wonder Card:", file); continue; }

                    // Load in WC
                    File.ReadAllBytes(file).CopyTo(sav, Main.SAV.WondercardData + WC6.Size * ctr++);
                    if (ctr >= 24) break;
                }
            }
        }

        // String Creation
        private string getWCDescriptionString(byte[] data)
        {
            // Load up the data according to the wiki!
            int cardID = BitConverter.ToUInt16(data, 0);
            if (cardID == 0) return "Empty Slot. No data!";

            string cardname = Util.TrimFromZero(Encoding.Unicode.GetString(data, 0x2, 0x48));
            int cardtype = data[0x51];
            string s = "";
            s += "Card #: " + cardID.ToString("0000") + " - " + cardname + Environment.NewLine;

            if (cardtype == 1) // Item
            {
                int item = BitConverter.ToUInt16(data, 0x68);
                int qty = BitConverter.ToUInt16(data, 0x70);

                s += "Item: " + Main.itemlist[item] + Environment.NewLine + "Quantity: " + qty;
            }
            else if (cardtype == 0) // PKM
            {
                WC6 card = new WC6(data);
                s += String.Format(
                    "{1} @ {2} --- {7} - {8}/{9}{0}" +
                    "{3} / {4} / {5} / {6}{0}",
                    Environment.NewLine,
                    Main.specieslist[card.Species],
                    Main.itemlist[card.HeldItem],
                    Main.movelist[card.Move1],
                    Main.movelist[card.Move2],
                    Main.movelist[card.Move3],
                    Main.movelist[card.Move4],
                    card.OT, card.TID.ToString("00000"), card.SID.ToString("00000"));
            }
            else
                s += "Unknown Wonder Card Type!";

            return s;
        }

        private Image getWCPreviewImage(byte[] data)
        {
            Image img;
            switch (data[0x51]) // Gift Type
            {
                case 0:
                    ushort species = BitConverter.ToUInt16(data, 0x82);
                    byte form = data[0x84];
                    byte gender = data[0xA1];
                    ushort item = BitConverter.ToUInt16(data, 0x78);
                    bool isEgg = data[0xD1] == 1;
                    bool isShiny = data[0xA3] == 2;
                    img = PKX.getSprite(species, form, gender, item, isEgg, isShiny);
                    break;
                case 1:
                    img = (Image)(Properties.Resources.ResourceManager.GetObject("item_" + BitConverter.ToUInt16(data, 0x68)) ?? Properties.Resources.unknown);
                    break;
                default:
                    img = Properties.Resources.unknown;
                    break;
            }
            return img;
        }
        private void L_QR_Click(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Alt)
            {
                byte[] wc = Util.getQRData();
                if (wc == null) return;
                if (wc.Length != WC6.Size) { Util.Alert("Decoded data not 0x108 bytes.", String.Format("QR Data Size: 0x{0}", wc.Length.ToString("X"))); }
                else try
                    {
                        Array.Copy(wc, wondercard_data, wc.Length);
                        loadwcdata();
                    }
                    catch { Util.Alert("Error loading wondercard data."); }
            }
            else
            {
                if (wondercard_data.SequenceEqual((new byte[wondercard_data.Length])))
                { Util.Alert("No wondercard data found in loaded slot!"); return; }
                if (BitConverter.ToUInt16(wondercard_data, 0x68) == 726 && wondercard_data[0x51] == 1)
                { Util.Alert("Eon Ticket Wonder Cards will not function properly", "Inject to the save file instead."); return; }
                // Prep data
                byte[] wcdata = wondercard_data;
                // Ensure size
                Array.Resize(ref wcdata, WC6.Size);
                // Setup QR
                const string server = "http://lunarcookies.github.io/wc.html#";
                Image qr = Util.getQRImage(wcdata, server);
                if (qr == null) return;

                string desc = getWCDescriptionString(wondercard_data);

                new QR(qr, PB_Preview.Image, desc, "", "", "PKHeX Wonder Card @ ProjectPokemon.org").ShowDialog();
            }
        }

        private void pbBoxSlot_MouseDown(object sender, MouseEventArgs e)
        {
            if (ModifierKeys == Keys.Control || ModifierKeys == Keys.Alt || ModifierKeys == Keys.Shift ||
                ModifierKeys == (Keys.Control | Keys.Alt))
            {
                switch (ModifierKeys)
                {
                    case Keys.Control: clickView(sender, e); break;
                    case Keys.Shift: clickSet(sender, e); break;
                    case Keys.Alt: clickDelete(sender, e); break;
                }
                return;
            }
            PictureBox pb = (PictureBox)sender;
            if (pb.Image == null)
                return;

            if (e.Button != MouseButtons.Left || e.Clicks != 1) return;

            int index = Array.FindIndex(pba, p => p.Name == (sender as PictureBox).Name);
            wc_slot = index;
            // Create Temp File to Drag
            Cursor.Current = Cursors.Hand;

            // Prepare Data
            byte[] dragdata = sav.Skip(Main.SAV.WondercardData + WC6.Size * index).Take(WC6.Size).ToArray();
            WC6 card = new WC6(dragdata);
            string filename = Util.CleanFileName(card.CardID.ToString("0000") + " - " + card.CardTitle + ".wc6");

            // Make File
            string newfile = Path.Combine(Path.GetTempPath(), Util.CleanFileName(filename));
            try
            {
                File.WriteAllBytes(newfile, dragdata);
                DoDragDrop(new DataObject(DataFormats.FileDrop, new[] { newfile }), DragDropEffects.Move);
            }
            catch (ArgumentException x)
            { Util.Error("Drag & Drop Error:", x.ToString()); }
            File.Delete(newfile);
            wc_slot = -1;
        }
        private void pbBoxSlot_DragDrop(object sender, DragEventArgs e)
        {
            int index = Array.FindIndex(pba, p => p.Name == (sender as PictureBox).Name);

            // Hijack to the latest unfilled slot if index creates interstitial empty slots.
            int lastUnfilled = Array.FindIndex(pba, p => p.Image == null);
            if (lastUnfilled < index)
                index = lastUnfilled;

            // Check for In-Dropped files (PKX,SAV,ETC)

            if (wc_slot == -1)
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files.Length < 1 || new FileInfo(files[0]).Length != WC6.Size)
                    return;

                byte[] wcdata = File.ReadAllBytes(files[0]);
                wcdata.CopyTo(sav, Main.SAV.WondercardData + WC6.Size * index);
                wcdata.CopyTo(wondercard_data, 0);
                loadwcdata();
                setCardID(BitConverter.ToUInt16(wcdata, 0));
            }
            else // Swap Data
            {
                // Check to see if they copied beyond blank slots.
                if (index > Math.Max(wc_slot, lastUnfilled - 1))
                    index = Math.Max(wc_slot, lastUnfilled - 1);
                byte[] s1 = sav.Skip(Main.SAV.WondercardData + WC6.Size * index).Take(WC6.Size).ToArray();
                byte[] s2 = sav.Skip(Main.SAV.WondercardData + WC6.Size * wc_slot).Take(WC6.Size).ToArray();
                s1.CopyTo(sav, Main.SAV.WondercardData + WC6.Size * wc_slot);
                s2.CopyTo(sav, Main.SAV.WondercardData + WC6.Size * index);
            }
            setBackground(index, Properties.Resources.slotView);
            populateWClist();
        }
        private void pbBoxSlot_DragEnter(object sender, DragEventArgs e)
        {
            if (e.AllowedEffect == (DragDropEffects.Copy | DragDropEffects.Link)) // external file
                e.Effect = DragDropEffects.Copy;
            else if (e.Data != null) // within
                e.Effect = DragDropEffects.Move;
        }
        private int wc_slot = -1;
    }
}