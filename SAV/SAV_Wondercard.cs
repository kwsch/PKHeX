using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace PKHeX
{
    public partial class SAV_Wondercard : Form
    {
        public SAV_Wondercard(byte[] wcdata = null)
        {
            InitializeComponent();
            Util.TranslateInterface(this, Main.curlanguage);
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
                return;
            
            Array.Copy(wcdata, wc6.Data, wcdata.Length);
            loadwcdata();
        }
        private readonly SAV6 SAV = new SAV6((byte[])Main.SAV.Data.Clone());
        private WC6 wc6 = new WC6();
        private readonly PictureBox[] pba;

        // Repopulation Functions
        private int currentSlot;
        private void setBackground(int index, Image bg)
        {
            for (int i = 0; i < 24; i++)
                pba[i].BackgroundImage = index == i ? bg : null;
            currentSlot = index;
        }
        private void populateWClist()
        {
            for (int i = 0; i < 24; i++)
            {
                WC6 wc = SAV.getWC6(i);
                pba[i].Image = wc.CardID == 0 ? null : wc.Preview;
            }
        }
        private void loadwcdata()
        {
            if (wc6 == null)
                return;
            try
            {
                if (wc6.GiftUsed && DialogResult.Yes ==
                        Util.Prompt(MessageBoxButtons.YesNo,
                            "Wonder Card is marked as USED and will not be able to be picked up in-game.",
                            "Do you want to remove the USED flag so that it is UNUSED?"))
                    wc6.GiftUsed = false;

                RTB.Text = wc6.Description;
                PB_Preview.Image = wc6.Preview;
            }
            catch (Exception e)
            {
                Util.Error("Loading of data failed... is this really a Wonder Card?", e.ToString());
                wc6 = new WC6();
                RTB.Clear();
            }
        }
        private void populateReceived()
        {
            LB_Received.Items.Clear();
            bool[] flags = SAV.WC6Flags;
            for (int i = 1; i < flags.Length; i++)
                if (flags[i])
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
            OpenFileDialog importwc6 = new OpenFileDialog {Filter = "Wonder Card|*.wc6;*.wc6full"};
            if (importwc6.ShowDialog() != DialogResult.OK) return;

            string path = importwc6.FileName;
            long len = new FileInfo(path).Length;
            if (len != WC6.Size && len != WC6.SizeFull)
            {
                Util.Error("File is not a Wonder Card:", path);
                return;
            }
            wc6 = new WC6(File.ReadAllBytes(path));
            loadwcdata();
        }
        private void B_Output_Click(object sender, EventArgs e)
        {
            SaveFileDialog outputwc6 = new SaveFileDialog();
            int cardID = wc6.CardID;
            string cardname = wc6.CardTitle;
            outputwc6.FileName = Util.CleanFileName($"{cardID} - {cardname}.wc6");
            outputwc6.Filter = "Wonder Card|*.wc6";
            if (outputwc6.ShowDialog() != DialogResult.OK) return;

            string path = outputwc6.FileName;

            if (File.Exists(path)) // File already exists, save a .bak
                File.WriteAllBytes(path + ".bak", File.ReadAllBytes(path));

            File.WriteAllBytes(path, wc6.Data);
        }

        // Wonder Card RW (window<->sav)
        private void clickView(object sender, EventArgs e)
        {
            sender = ((sender as ToolStripItem)?.Owner as ContextMenuStrip)?.SourceControl ?? sender as PictureBox;
            int index = Array.IndexOf(pba, sender);

            setBackground(index, Properties.Resources.slotView);
            wc6 = SAV.getWC6(index);
            loadwcdata();
        }
        private void clickSet(object sender, EventArgs e)
        {
            if (wc6 == null)
                return;
            if (!checkSpecialWonderCard(wc6))
                return;

            sender = ((sender as ToolStripItem)?.Owner as ContextMenuStrip)?.SourceControl ?? sender as PictureBox;
            int index = Array.IndexOf(pba, sender);

            // Hijack to the latest unfilled slot if index creates interstitial empty slots.
            int lastUnfilled = Array.FindIndex(pba, p => p.Image == null);
            if (lastUnfilled > -1 && lastUnfilled < index)
                index = lastUnfilled;

            setBackground(index, Properties.Resources.slotSet);
            SAV.setWC6(wc6, index);
            populateWClist();
            setCardID(wc6.CardID);
        }
        private void clickDelete(object sender, EventArgs e)
        {
            sender = ((sender as ToolStripItem)?.Owner as ContextMenuStrip)?.SourceControl ?? sender as PictureBox;
            int index = Array.IndexOf(pba, sender);

            setBackground(index, Properties.Resources.slotDel);
            SAV.setWC6(new WC6(), index);
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
            bool[] flags = new bool[(SAV.WondercardData - SAV.WondercardFlags)*8];
            foreach (var o in LB_Received.Items)
                flags[Util.ToUInt32(o.ToString())] = true;

            SAV.WC6Flags = flags;
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
            int ctr = currentSlot;
            if (Directory.Exists(files[0]))
                files = Directory.GetFiles(files[0], "*", SearchOption.AllDirectories);
            if (files.Length == 1 && !Directory.Exists(files[0]))
            {
                string path = files[0]; // open first D&D
                long len = new FileInfo(path).Length;
                if (len != WC6.Size && len != WC6.SizeFull)
                {
                    Util.Error("File is not a Wonder Card:", path);
                    return;
                }
                byte[] newwc6 = File.ReadAllBytes(path);
                if (newwc6.Length == WC6.SizeFull)
                    newwc6 = newwc6.Skip(WC6.SizeFull - WC6.Size).ToArray();
                Array.Copy(newwc6, wc6.Data, newwc6.Length);
                loadwcdata();
                return;
            }

            if (DialogResult.Yes != Util.Prompt(MessageBoxButtons.YesNo, $"Try to load {files.Length} Wonder Cards starting at Card {ctr + 1}?"))
                return;

            foreach (string file in files)
            {
                long len = new FileInfo(file).Length;
                if (len != WC6.Size && len != WC6.SizeFull)
                { Util.Error("File is not a Wonder Card:", file); continue; }

                // Load in WC
                byte[] newwc6 = File.ReadAllBytes(file);

                if (newwc6.Length == WC6.SizeFull)
                    newwc6 = newwc6.Skip(WC6.SizeFull - WC6.Size).ToArray();
                if (checkSpecialWonderCard(new WC6(newwc6)))
                {
                    WC6 wc = new WC6(newwc6);
                    SAV.setWC6(wc, ctr++);
                    setCardID(wc.CardID);
                }
                if (ctr >= 24)
                    break;
            }
            populateWClist();
        }

        private bool checkSpecialWonderCard(WC6 wc)
        {
            if (wc6.CardID == 2048 && wc.Item == 726) // Eon Ticket (OR/AS)
            {
                if (!Main.SAV.ORAS || Main.SAV.EonTicket < 0)
                    goto reject;
                BitConverter.GetBytes(WC6.EonTicketConst).CopyTo(SAV.Data, Main.SAV.EonTicket);
            }

            return true;
            reject: Util.Alert("Unable to insert the Wonder Card.", "Does this Wonder Card really belong to this game?");
            return false;
        }
        
        private void L_QR_Click(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Alt)
            {
                byte[] data = Util.getQRData();
                if (data == null) return;
                if (data.Length != WC6.Size) { Util.Alert("Decoded data not 0x108 bytes.",
                    $"QR Data Size: 0x{data.Length.ToString("X")}"); }
                else try
                    {
                        wc6 = new WC6(data);
                        loadwcdata();
                    }
                    catch { Util.Alert("Error loading wondercard data."); }
            }
            else
            {
                if (wc6.Data.SequenceEqual(new byte[wc6.Data.Length]))
                { Util.Alert("No wondercard data found in loaded slot!"); return; }
                if (wc6.Item == 726 && wc6.IsItem)
                { Util.Alert("Eon Ticket Wonder Cards will not function properly", "Inject to the save file instead."); return; }
                // Prep data
                byte[] wcdata = wc6.Data;
                // Ensure size
                Array.Resize(ref wcdata, WC6.Size);
                // Setup QR
                const string server = "http://lunarcookies.github.io/wc.html#";
                Image qr = Util.getQRImage(wcdata, server);
                if (qr == null) return;

                string desc = wc6.Description;

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

            int index = Array.IndexOf(pba, sender);
            wc_slot = index;
            // Create Temp File to Drag
            Cursor.Current = Cursors.Hand;

            // Prepare Data
            WC6 card = SAV.getWC6(index);
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

            // Check for In-Dropped files (PKX,SAV,ETC)

            if (wc_slot == -1)
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files.Length < 1 || new FileInfo(files[0]).Length != WC6.Size)
                    return;
                
                WC6 wc = new WC6(File.ReadAllBytes(files[0]));
                SAV.setWC6(wc, index);
                setCardID(wc.CardID);
                wc6 = wc;
                loadwcdata();
            }
            else // Swap Data
            {
                // Check to see if they copied beyond blank slots.
                if (index > Math.Max(wc_slot, lastUnfilled - 1))
                    index = Math.Max(wc_slot, lastUnfilled - 1);
                WC6 s1 = SAV.getWC6(index);
                WC6 s2 = SAV.getWC6(wc_slot);
                SAV.setWC6(s1, wc_slot);
                SAV.setWC6(s2, index);
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

    // Extension Properties
    public partial class WC6
    {
        public Image Preview
        {
            get
            {
                Image img;
                if (IsPokémon)
                    img = PKX.getSprite(Species, Form, Gender, HeldItem, IsEgg, PIDType == 2);
                else if (IsItem)
                    img = (Image)(Properties.Resources.ResourceManager.GetObject("item_" + Item) ?? Properties.Resources.unknown);
                else
                    img = Properties.Resources.unknown;

                if (GiftUsed)
                    img = Util.LayerImage(new Bitmap(img.Width, img.Height), img, 0, 0, 0.3);
                return img;
            }
        }
        public string Description
        {
            get
            {
                if (CardID == 0)
                    return "Empty Slot. No data!";

                string s = $"Card #: {CardID.ToString("0000")} - {CardTitle.Trim()}" + Environment.NewLine;

                switch (CardType)
                {
                    case 1:
                        s += "Item: " + Main.itemlist[Item] + Environment.NewLine + "Quantity: " + Quantity;
                        return s;
                    case 0:
                        s +=
                            $"{Main.specieslist[Species]} @ {Main.itemlist[HeldItem]} --- {OT} - {TID.ToString("00000")}/{SID.ToString("00000")}" + Environment.NewLine +
                            $"{Main.movelist[Move1]} / {Main.movelist[Move2]} / {Main.movelist[Move3]} / {Main.movelist[Move4]}" + Environment.NewLine;
                        return s;
                    default:
                        s += "Unknown Wonder Card Type!";
                        return s;
                }
            }
        }
    }
}