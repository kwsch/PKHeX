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
        public SAV_Wondercard(Form1 frm1)
        {
            InitializeComponent();
            Util.TranslateInterface(this, Form1.curlanguage);
            m_parent = frm1;
            Array.Copy(m_parent.savefile, sav, 0x100000);
            savindex = m_parent.savindex;
            if (m_parent.savegame_oras) wcoffset = 0x22100;
            populateWClist();
            populateReceived();

            LB_WCs.SelectedIndex = 0;

            if (LB_Received.Items.Count > 0)
                LB_Received.SelectedIndex = 0;

            DragEnter += tabMain_DragEnter;
            DragDrop += tabMain_DragDrop;
        }
        Form1 m_parent;
        public byte[] sav = new byte[0x100000];
        public byte[] wondercard_data = new byte[0x108];
        public int savindex;
        private int wcoffset = 0x21100;
        private const uint herpesval = 0x225D73C2;

        // Repopulation Functions
        private void populateWClist()
        {
            LB_WCs.Items.Clear();
            for (int i = 0; i < 24; i++)
            {
                int offset = wcoffset + savindex * 0x7F000 + i * 0x108;
                int cardID = BitConverter.ToUInt16(sav, offset);
                if (cardID == 0)
                    LB_WCs.Items.Add((i + 1).ToString("00") + " - Empty");
                else
                    LB_WCs.Items.Add((i + 1).ToString("00") + " - " + cardID.ToString("0000"));
            }
        }
        private void loadwcdata()
        {
            try
            {
                RTB.Text = getWCDescriptionString(wondercard_data);
                PB_Preview.Image = getWCPreviewImage(wondercard_data);
            }
            catch (Exception e)
            {
                Util.Error("Loading of data failed... is this really a Wondercard?", e.ToString());
                Array.Copy(new byte[0x108], wondercard_data, 0x108);
                RTB.Clear();
            }
        }
        private void populateReceived()
        {
            int offset = wcoffset - 0x100 + savindex * 0x7F000;
            LB_Received.Items.Clear();
            for (int i = 1; i < 2048; i++)
                if ((((sav[offset + i / 8]) >> (i % 8)) & 0x1) == 1)
                    LB_Received.Items.Add(i.ToString("0000"));
        }

        // Wondercard IO (.wc6<->window)
        private void B_Import_Click(object sender, EventArgs e)
        {
            OpenFileDialog importwc6 = new OpenFileDialog {Filter = "Wondercard|*.wc6"};
            if (importwc6.ShowDialog() != DialogResult.OK) return;

            string path = importwc6.FileName;
            if (new FileInfo(path).Length > 0x108)
            {
                Util.Error("File is not a Wondercard:", path);
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
            outputwc6.Filter = "Wondercard|*.wc6";
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
        private void B_SAV2WC(object sender, EventArgs e)
        {
            // Load Wondercard from Save File
            int index = LB_WCs.SelectedIndex;
            int offset = wcoffset + savindex * 0x7F000 + index * 0x108;
            Array.Copy(sav, offset, wondercard_data, 0, 0x108);
            loadwcdata();
        }
        private void B_WC2SAV(object sender, EventArgs e)
        {
            // Write Wondercard to Save File
            int index = LB_WCs.SelectedIndex;
            int offset = wcoffset + savindex * 0x7F000 + index * 0x108;
            if (m_parent.savegame_oras) // ORAS Only
                if (BitConverter.ToUInt16(wondercard_data, 0) == 0x800) // Eon Ticket #
                    if (BitConverter.ToUInt16(wondercard_data, 0x68) == 0x2D6) // Eon Ticket
                        Array.Copy(BitConverter.GetBytes(herpesval), 0, sav, 0x319B8 + 0x5400 + savindex * 0x7F000, 4);
            Array.Copy(wondercard_data, 0, sav, offset, 0x108);
            populateWClist();
            int cardID = BitConverter.ToUInt16(wondercard_data, 0);

            if (cardID <= 0 || cardID >= 0x100*8) return;

            if (!LB_Received.Items.Contains(cardID.ToString("0000")))
                LB_Received.Items.Add(cardID.ToString("0000"));
        }
        private void B_DeleteWC_Click(object sender, EventArgs e)
        {
            int index = LB_WCs.SelectedIndex;
            int offset = wcoffset + savindex * 0x7F000 + index * 0x108;
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
            int offset = wcoffset - 0x100 + savindex * 0x7F000;

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

            Array.Copy(sav, m_parent.savefile, 0x100000);
            m_parent.savedited = true;
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
                if (new FileInfo(path).Length > 0x108)
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

                s += "Item: " + Form1.itemlist[item] + Environment.NewLine + "Quantity: " + qty;
            }
            else if (cardtype == 0) // PKM
            {
                int species = BitConverter.ToUInt16(data, 0x82);
                int helditem = BitConverter.ToUInt16(data, 0x78);
                int move1 = BitConverter.ToUInt16(data, 0x7A);
                int move2 = BitConverter.ToUInt16(data, 0x7C);
                int move3 = BitConverter.ToUInt16(data, 0x7E);
                int move4 = BitConverter.ToUInt16(data, 0x80);
                int TID = BitConverter.ToUInt16(data, 0x68);
                int SID = BitConverter.ToUInt16(data, 0x6A);

                string OTname = Util.TrimFromZero(Encoding.Unicode.GetString(data, 0xB6, 22));
                s += String.Format(
                    "{1} @ {2} --- {7} - {8}/{9}{0}" +
                    "{3} / {4} / {5} / {6}{0}",
                    Environment.NewLine,
                    Form1.specieslist[species],
                    Form1.itemlist[helditem],
                    Form1.movelist[move1],
                    Form1.movelist[move2],
                    Form1.movelist[move3],
                    Form1.movelist[move4],
                    OTname, TID.ToString("00000"), SID.ToString("00000"));
            }
            else
                s += "Unknown Wondercard Type!";

            return s;
        }

        private Image getWCPreviewImage(byte[] data)
        {
            Image img;
            switch (data[0x51])
            {
                case 0:
                    ushort species = BitConverter.ToUInt16(data, 0x82);
                    byte altforms = data[0x84];
                    byte gender = data[0xA1];
                    string file = "_" + species;
                    if (altforms > 0) // Alt Form Handling
                        file = file + "_" + altforms;
                    else if (gender == 1 && (species == 592 || species == 593)) // Frillish & Jellicent
                        file = file + "_" + gender;
                    else if (gender == 1 && (species == 521 || species == 668)) // Unfezant & Pyroar
                        file = "_" + species + "f";
                    img = (Image) Properties.Resources.ResourceManager.GetObject(file);

                    // Improve the Preview
                    ushort item = BitConverter.ToUInt16(data, 0x78);
                    if (data[0xD1] > 0) img = Util.LayerImage(img, Properties.Resources.egg, 0, 0, 1);
                    if (data[0xA3] == 2) img = Util.LayerImage(img, Properties.Resources.rare_icon, 0, 0, 0.7);
                    if (item > 0)
                    {
                        Image itemimg = (Image)Properties.Resources.ResourceManager.GetObject("item_" + item) ?? Properties.Resources.helditem;
                        img = Util.LayerImage(img, itemimg, 22 + (15 - itemimg.Width) / 2, 15 + (15 - itemimg.Height), 1);
                    }
                    break;
                case 1:
                    img = (Image)Properties.Resources.ResourceManager.GetObject("item_" + BitConverter.ToUInt16(data, 0x68));
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
                // Fetch data from QR code...
                string address;
                try { address = Clipboard.GetText(); }
                catch { Util.Alert("No text (url) in clipboard."); return; }
                try { if (address.Length < 4 || address.Substring(0, 3) != "htt") { Util.Alert("Clipboard text is not a valid URL:", address); return; } }
                catch { Util.Alert("Clipboard text is not a valid URL:", address); return; }
                string webURL = "http://api.qrserver.com/v1/read-qr-code/?fileurl=" + System.Web.HttpUtility.UrlEncode(address);
                try
                {
                    System.Net.HttpWebRequest httpWebRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(webURL);
                    System.Net.HttpWebResponse httpWebReponse = (System.Net.HttpWebResponse)httpWebRequest.GetResponse();
                    var reader = new StreamReader(httpWebReponse.GetResponseStream());
                    string data = reader.ReadToEnd();
                    if (data.Contains("could not find")) { Util.Alert("Reader could not find QR data in the image."); return; }
                    // Quickly convert the json response to a data string
                    string pkstr = data.Substring(data.IndexOf("#", StringComparison.Ordinal) + 1); // Trim intro
                    pkstr = pkstr.Substring(0, pkstr.IndexOf("\",\"error\":null}]}]", StringComparison.Ordinal)); // Trim outro
                    if (pkstr.Contains("nQR-Code:")) pkstr = pkstr.Substring(0, pkstr.IndexOf("nQR-Code:", StringComparison.Ordinal)); //  Remove multiple QR codes in same image
                    pkstr = pkstr.Replace("\\", ""); // Rectify response

                    byte[] wc;
                    try { wc = Convert.FromBase64String(pkstr); }
                    catch { Util.Alert("QR string to Data failed.", pkstr); return; }

                    if (wc.Length != 0x108) { Util.Alert("Decoded data not 0x108 bytes.", String.Format("QR Data Size: 0x{0}", wc.Length.ToString("X"))); }
                    else try
                    {
                        Array.Copy(wc, wondercard_data, wc.Length);
                        loadwcdata(); 
                    }
                    catch { Util.Alert("Error loading wondercard data."); }
                }
                catch { Util.Alert("Unable to connect to the internet to decode QR code."); }
            }
            else
            {
                if (wondercard_data.SequenceEqual((new byte[wondercard_data.Length])))
                { Util.Alert("No wondercard data found"); return; }
                // Prep data
                byte[] wcdata = wondercard_data;
                // Ensure size
                Array.Resize(ref wcdata, 0x108);
                // Setup QR
                const string server = "http://lunarcookies.github.io/wc.html#";
                string qrdata = Convert.ToBase64String(wcdata);
                string message = server + qrdata;
                string webURL = "http://chart.apis.google.com/chart?chs=365x365&cht=qr&chl=" + System.Web.HttpUtility.UrlEncode(message);

                Image qr = null;
                try
                {
                    System.Net.HttpWebRequest httpWebRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(webURL);
                    System.Net.HttpWebResponse httpWebReponse = (System.Net.HttpWebResponse)httpWebRequest.GetResponse();
                    Stream stream = httpWebReponse.GetResponseStream();
                    if (stream != null) qr = Image.FromStream(stream);
                }
                catch
                {
                    if (DialogResult.Yes == Util.Prompt(MessageBoxButtons.YesNo, "Unable to connect to the internet to receive QR code.", "Copy QR URL to Clipboard?"))
                    {
                        try { Clipboard.SetText(webURL); }
                        catch { Util.Alert("Failed to set text to Clipboard"); }
                        return;
                    }
                }
                string desc = getWCDescriptionString(wondercard_data);
                Image img = PB_Preview.Image;

                new QR(qr, img, desc, "", "", "PKHeX Wondercard @ ProjectPokemon.org").ShowDialog();
            }
        }
    }
}