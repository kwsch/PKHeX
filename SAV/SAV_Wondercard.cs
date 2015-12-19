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

            populateWClist();
            populateReceived();

            LB_WCs.SelectedIndex = 0;

            if (LB_Received.Items.Count > 0)
                LB_Received.SelectedIndex = 0;

            DragEnter += tabMain_DragEnter;
            DragDrop += tabMain_DragDrop;

            if (wcdata == null || wcdata.Length != 0x108) return; // No data to load
            Array.Copy(wcdata, wondercard_data, wcdata.Length);
            loadwcdata();
        }
        public byte[] sav;
        public byte[] wondercard_data = new byte[0x108];
        private const uint EonTicketConst = 0x225D73C2;
        private PictureBox[] pba;

        // Repopulation Functions
        private void setBackground(int index, Image bg)
        {
            for (int i = 0; i < 24; i++)
                pba[i].BackgroundImage = index == i ? bg : null;
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
            LB_WCs.Items.Clear();
            for (int i = 0; i < 24; i++)
            {
                int offset = Main.SAV.WondercardData + i * 0x108;
                int cardID = BitConverter.ToUInt16(sav, offset);
                if (cardID == 0)
                {
                    LB_WCs.Items.Add((i + 1).ToString("00") + " - Empty");
                    pba[i].Image = null;
                }
                else
                {
                    LB_WCs.Items.Add((i + 1).ToString("00") + " - " + cardID.ToString("0000"));
                    pba[i].Image = getWCPreviewImage(sav.Skip(Main.SAV.WondercardData + WC6.Size * i).Take(WC6.Size).ToArray());
                }
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
                Array.Copy(new byte[0x108], wondercard_data, 0x108);
                RTB.Clear();
            }
        }
        private void populateReceived()
        {
            LB_Received.Items.Clear();
            for (int i = 1; i < 2048; i++)
                if ((((sav[Main.SAV.WondercardFlags + i / 8]) >> (i % 8)) & 0x1) == 1)
                    LB_Received.Items.Add(i.ToString("0000"));
        }

        // Wondercard IO (.wc6<->window)
        private void B_Import_Click(object sender, EventArgs e)
        {
            OpenFileDialog importwc6 = new OpenFileDialog {Filter = "Wonder Card|*.wc6"};
            if (importwc6.ShowDialog() != DialogResult.OK) return;

            string path = importwc6.FileName;
            if (new FileInfo(path).Length != 0x108)
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
            outputwc6.FileName = cardID + " - " + cardname + ".wc6";
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

        // Wondercard RW (window<->sav)
        private void cardView(object sender, EventArgs e)
        {
            // Load Wondercard from Save File
            int index = LB_WCs.SelectedIndex;
            setBackground(index, Properties.Resources.slotView);
            int offset = Main.SAV.WondercardData + index * 0x108;
            Array.Copy(sav, offset, wondercard_data, 0, 0x108);
            loadwcdata();
        }
        private void cardSet(object sender, EventArgs e)
        {
            // Write Wondercard to Save File
            int index = LB_WCs.SelectedIndex;
            setBackground(index, Properties.Resources.slotSet);
            int offset = Main.SAV.WondercardData + index * 0x108;
            if (Main.SAV.ORAS) // ORAS Only
                if (BitConverter.ToUInt16(wondercard_data, 0) == 0x800) // Eon Ticket #
                    if (BitConverter.ToUInt16(wondercard_data, 0x68) == 0x2D6) // Eon Ticket
                        Array.Copy(BitConverter.GetBytes(EonTicketConst), 0, sav, Main.SAV.EonTicket, 4);
            Array.Copy(wondercard_data, 0, sav, offset, 0x108);
            populateWClist();
            int cardID = BitConverter.ToUInt16(wondercard_data, 0);

            if (cardID <= 0 || cardID >= 0x100*8) return;

            if (!LB_Received.Items.Contains(cardID.ToString("0000")))
                LB_Received.Items.Add(cardID.ToString("0000"));
        }
        private void cardDelete(object sender, EventArgs e)
        {
            int index = LB_WCs.SelectedIndex;
            setBackground(index, Properties.Resources.slotDel);
            int offset = Main.SAV.WondercardData + index * 0x108;
            byte[] zeros = new byte[0x108];
            Array.Copy(zeros, 0, sav, offset, 0x108);
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
                    if (BitConverter.ToUInt16(sav, offset + i * 0x108) == 0)
                        for (int j = i + 1; j < 24 - i; j++) // Shift everything down
                            Array.Copy(sav, offset + j * 0x108, sav, offset + (j - 1) * 0x108, 0x108);
            }

            Array.Copy(sav, Main.SAV.Data, sav.Length);
            Main.SAV.Edited = true;
            Close();
        }

        // Delete WC Flag
        private void B_DeleteReceived_Click(object sender, EventArgs e)
        {
            if (LB_Received.SelectedIndex <= -1) return;

            if (LB_Received.Items.Count > 0)
                LB_Received.Items.Remove(LB_Received.Items[LB_Received.SelectedIndex]);
        }

        // Drag & Drop Wondercards
        private void tabMain_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }
        private void tabMain_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            // Check for multiple wondercards
            if (LB_WCs.SelectedIndex < 0) LB_WCs.SelectedIndex = 0;
            int ctr = LB_WCs.SelectedIndex;

            if (files.Length == 1)
            {
                string path = files[0]; // open first D&D
                if (new FileInfo(path).Length != 0x108)
                {
                    Util.Error("File is not a Wondercard:", path);
                    return;
                }
                byte[] newwc6 = File.ReadAllBytes(path);
                Array.Copy(newwc6, wondercard_data, newwc6.Length);
                loadwcdata();
            }
            else if (DialogResult.Yes == Util.Prompt(MessageBoxButtons.YesNo, String.Format("Try to load {0} Wondercards starting at Card {1}?", files.Length, ctr + 1)))
            {
                int i = 0;
                while (i < files.Length && ctr < 24)
                {
                    string path = files[i++]; // open first D&D
                    if (new FileInfo(path).Length != 0x108)
                    { Util.Error("File is not a Wondercard:", path); continue; }

                    ctr++; // Load in WC
                    byte[] newwc6 = File.ReadAllBytes(path);
                    Array.Copy(newwc6, wondercard_data, newwc6.Length);
                    loadwcdata();

                    // Set in WC
                    B_DisplaytoWCSlot.PerformClick();

                    // Advance to next WC
                    if (ctr != 24) LB_WCs.SelectedIndex = ctr;
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
                s += "Unknown Wondercard Type!";

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

                if (wc.Length != 0x108) { Util.Alert("Decoded data not 0x108 bytes.", String.Format("QR Data Size: 0x{0}", wc.Length.ToString("X"))); }
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
                { Util.Alert("Eon Ticket Wondercards will not function properly", "Inject to the save file instead."); return; }
                // Prep data
                byte[] wcdata = wondercard_data;
                // Ensure size
                Array.Resize(ref wcdata, 0x108);
                // Setup QR
                const string server = "http://lunarcookies.github.io/wc.html#";
                Image qr = Util.getQRImage(wcdata, server);
                if (qr == null) return;

                string desc = getWCDescriptionString(wondercard_data);

                new QR(qr, PB_Preview.Image, desc, "", "", "PKHeX Wondercard @ ProjectPokemon.org").ShowDialog();
            }
        }

        private void pbBoxSlot_MouseDown(object sender, MouseEventArgs e)
        {
            switch (ModifierKeys)
            {
                case Keys.Control: clickView(sender, e); break;
                case Keys.Shift: clickSet(sender, e); break;
                case Keys.Alt: clickDelete(sender, e); break;
            }
        }
        private void clickView(object sender, EventArgs e)
        {
            string name = (sender is ToolStripItem)
                ? ((sender as ToolStripItem).Owner as ContextMenuStrip).SourceControl.Name
                : (sender as PictureBox).Name;
            LB_WCs.SelectedIndex = Array.FindIndex(pba, p => p.Name == name);
            B_WCSlottoDisplay.PerformClick();
        }
        private void clickSet(object sender, EventArgs e)
        {
            string name = (sender is ToolStripItem)
                ? ((sender as ToolStripItem).Owner as ContextMenuStrip).SourceControl.Name
                : (sender as PictureBox).Name;
            LB_WCs.SelectedIndex = Array.FindIndex(pba, p => p.Name == name);
            B_DisplaytoWCSlot.PerformClick();
        }
        private void clickDelete(object sender, EventArgs e)
        {
            string name = (sender is ToolStripItem)
                ? ((sender as ToolStripItem).Owner as ContextMenuStrip).SourceControl.Name
                : (sender as PictureBox).Name;
            LB_WCs.SelectedIndex = Array.FindIndex(pba, p => p.Name == name);
            B_DeleteWC.PerformClick();
        }
    }
}