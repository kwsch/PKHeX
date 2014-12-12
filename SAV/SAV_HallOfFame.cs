using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PKHeX
{
    public partial class SAV_HallOfFame : Form
    {
        public SAV_HallOfFame(Form1 frm1)
        {
            InitializeComponent();
            m_parent = frm1;
            Array.Copy(m_parent.savefile, sav, 0x100000);
            savindex = m_parent.savindex;
            shiftval = savindex * 0x7F000;
            if (m_parent.savegame_oras) data_offset = 0x1F200;
            Array.Copy(sav, shiftval + data_offset, data, 0, data.Length); //Copy HoF section of save into Data
            Setup();
            editor_spec = new object[]{
                GB_OT,
                GB_CurrentMoves,
                CB_Species,
                CB_HeldItem,
                TB_EC,
                TB_VN,
                CAL_MetDate,
                CHK_Nicknamed,
                CHK_Shiny,
                L_PartyNum,
                L_Victory,
                L_Shiny,
                L_Level,
                Label_TID,
                Label_Form,
                Label_Gender,
                Label_HeldItem,
                Label_OT,
                Label_TID,
                Label_SID,
                Label_Species,
                TB_Level,
                NUP_PartyIndex,
                Label_EncryptionConstant,
                Label_MetDate,
            };
            listBox1.SelectedIndex = 0;           
            NUP_PartyIndex_ValueChanged(null, null);
            editing = true;
        }
        Form1 m_parent;
        public byte[] sav = new Byte[0x100000];
        public int savindex; int shiftval;
        public bool editing = false;
        private int data_offset = 0x1E800;

        private string[] types = Form1.types;
        private string[] forms = Form1.forms;
        private string[] gendersymbols = Form1.gendersymbols;
        private byte[] data = new byte[0x1B40];

        private object[] editor_spec;

        private void Setup()
        {
            try
            {
                CB_Species.Items.Clear();
                CB_HeldItem.Items.Clear();
                CB_Move1.Items.Clear();
                CB_Move2.Items.Clear();
                CB_Move3.Items.Clear();
                CB_Move4.Items.Clear();
            }
            catch { }

            #region Species
            {
                List<cbItem> species_list = new List<cbItem>();
                // Sort the Rest based on String Name
                string[] sortedspecies = new string[Form1.specieslist.Length];
                Array.Copy(Form1.specieslist, sortedspecies, Form1.specieslist.Length);
                Array.Sort(sortedspecies);

                // Add the rest of the items
                for (int i = 0; i < sortedspecies.Length; i++)
                {
                    cbItem ncbi = new cbItem();
                    ncbi.Text = sortedspecies[i];
                    ncbi.Value = Array.IndexOf(Form1.specieslist, sortedspecies[i]);
                    species_list.Add(ncbi);
                }
                species_list.RemoveAt(0); // Remove 0th Entry
                CB_Species.DisplayMember = "Text";
                CB_Species.ValueMember = "Value";
                CB_Species.DataSource = species_list;
            }
            #endregion
            #region Moves
            {
                string[] movelist = Form1.movelist;
                List<cbItem> move_list = new List<cbItem>();
                // Sort the Rest based on String Name
                string[] sortedmoves = new string[movelist.Length];
                Array.Copy(movelist, sortedmoves, movelist.Length);
                Array.Sort(sortedmoves);

                // Add the rest of the items
                for (int i = 0; i < sortedmoves.Length; i++)
                {
                    cbItem ncbi = new cbItem();
                    ncbi.Text = sortedmoves[i];
                    ncbi.Value = Array.IndexOf(movelist, sortedmoves[i]);
                    move_list.Add(ncbi);
                }

                CB_Move1.DisplayMember = CB_Move2.DisplayMember = CB_Move3.DisplayMember = CB_Move4.DisplayMember = "Text";
                CB_Move1.ValueMember = CB_Move2.ValueMember = CB_Move3.ValueMember = CB_Move4.ValueMember = "Value";

                var move1_list = new BindingSource(move_list, null);
                CB_Move1.DataSource = move1_list;

                var move2_list = new BindingSource(move_list, null);
                CB_Move2.DataSource = move2_list;

                var move3_list = new BindingSource(move_list, null);
                CB_Move3.DataSource = move3_list;

                var move4_list = new BindingSource(move_list, null);
                CB_Move4.DataSource = move4_list;
            }
            #endregion
            #region Items
            {
                string[] itemlist = Form1.itemlist;
                int[] item_nums = { 
                                            000,001,002,003,004,005,006,007,008,009,010,011,012,013,014,015,017,018,019,020,021,022,023,024,025,026,027,028,029,030,031,032,033,034,035,
                                            036,037,038,039,040,041,042,043,044,045,046,047,048,049,050,051,052,053,054,055,056,057,058,059,060,061,062,063,064,065,066,067,068,069,070,
                                            071,072,073,074,075,076,077,078,079,080,081,082,083,084,085,086,087,088,089,090,091,092,093,094,099,100,101,102,103,104,105,106,107,108,109,
                                            110,112,116,117,118,119,134,135,136,149,150,151,152,153,154,155,156,157,158,159,160,161,162,163,164,165,166,167,168,169,170,171,172,173,174,
                                            175,176,177,178,179,180,181,182,183,184,185,186,187,188,189,190,191,192,193,194,195,196,197,198,199,200,201,202,203,204,205,206,207,208,209,
                                            210,211,212,213,214,215,217,218,219,220,221,222,223,224,225,226,227,228,229,230,231,232,233,234,235,236,237,238,239,240,241,242,243,244,
                                            245,246,247,248,249,250,251,252,253,254,255,256,257,258,259,260,261,262,263,264,265,266,267,268,269,270,271,272,273,274,275,276,277,278,279,
                                            280,281,282,283,284,285,286,287,288,289,290,291,292,293,294,295,296,297,298,299,300,301,302,303,304,305,306,307,308,309,310,311,312,313,314,
                                            315,316,317,318,319,320,321,322,323,324,325,326,327,504,537,538,539,540,541,542,543,544,545,546,547,548,549,550,551,552,553,554,555,556,557,
                                            558,559,560,561,562,563,564,565,566,567,568,569,570,571,572,573,577,580,581,582,583,584,585,586,587,588,589,590,591,639,640,644,645,646,647,
                                            648,649,650,652,653,654,655,656,657,658,659,660,661,662,663,664,665,666,667,668,669,670,671,672,673,674,675,676,677,678,679,680,681,682,683,
                                            684,685,686,687,688,699,704,708,709,710,711,715,

                                            // Appended ORAS Items (Orbs & Mega Stones)
                                            534,535,
                                            752,753,754,755,756,757,758,759,760,761,762,763,764,767,768,769,770,
                                    };

                string filename = Path.GetFileNameWithoutExtension(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);


                if (filename.IndexOf("HaX") >= 0)    // allow all items to be selected
                {
                    item_nums = new int[itemlist.Length];
                    for (int i = 0; i < itemlist.Length; i++)
                        item_nums[i] = i;
                }

                List<cbItem> item_list = new List<cbItem>();
                // Sort the Rest based on String Name
                string[] itemnames = new string[item_nums.Length];
                for (int i = 0; i < item_nums.Length; i++)
                    itemnames[i] = itemlist[item_nums[i]];

                string[] sorteditems = new string[item_nums.Length];
                Array.Copy(itemnames, sorteditems, itemnames.Length);
                Array.Sort(sorteditems);

                // Add the rest of the items
                for (int i = 0; i < sorteditems.Length; i++)
                {
                    cbItem ncbi = new cbItem();
                    ncbi.Text = sorteditems[i];
                    ncbi.Value = item_nums[Array.IndexOf(itemnames, sorteditems[i])];
                    item_list.Add(ncbi);
                }
                CB_HeldItem.DisplayMember = "Text";
                CB_HeldItem.ValueMember = "Value";
                CB_HeldItem.DataSource = item_list;
            }
            #endregion
        }
        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void B_Close_Click(object sender, EventArgs e)
        {
            Array.Copy(data, 0, m_parent.savefile, shiftval + data_offset, data.Length);
            Close();
        }
        private void displayEntry(object sender, EventArgs e)
        {
            editing = false;
            RTB.Font = new Font("Courier New", 8);
            string s = "";
            RTB.LanguageOption = RichTextBoxLanguageOptions.DualFont;
            int index = listBox1.SelectedIndex;
            int offset = index * 0x1B4;

            uint vnd = BitConverter.ToUInt32(data, offset + 0x1B0);
            uint vn = vnd & 0xFF;
            TB_VN.Text = vn.ToString("000");
            s = "Entry #" + vn + Environment.NewLine;
            uint date = (vnd >> 14) & 0x1FFFF;
            uint year = (date & 0xFF) + 2000;
            uint month = (date >> 8) & 0xF;
            uint day = (date >> 12);
            if (day == 0)
            {
                s += "No records in this slot.";
                for (int i = 0; i < editor_spec.Length; i++)
                    ((Control)editor_spec[i]).Enabled = false;

                editing = false;
                NUP_PartyIndex_ValueChanged(sender, e);
                goto end;
            }
            else 
            {
                for (int i = 0; i < editor_spec.Length; i++)
                    ((Control)editor_spec[i]).Enabled = true;
            }

            s += "Date: " + year.ToString() + "/" + month.ToString() + "/" + day.ToString() + "" + Environment.NewLine + Environment.NewLine;
            CAL_MetDate.Value = new DateTime((int)year, (int)month, (int)day);
            int moncount = 0;
            for (int i = 0; i < 6; i++)
            {
                int species = BitConverter.ToUInt16(data, offset + 0x00);
                int helditem = BitConverter.ToUInt16(data, offset + 0x02);
                int move1 = BitConverter.ToUInt16(data, offset + 0x04);
                int move2 = BitConverter.ToUInt16(data, offset + 0x06);
                int move3 = BitConverter.ToUInt16(data, offset + 0x08);
                int move4 = BitConverter.ToUInt16(data, offset + 0x0A);

                int TID = BitConverter.ToUInt16(data, offset + 0x10);
                int SID = BitConverter.ToUInt16(data, offset + 0x12);

                uint slgf = BitConverter.ToUInt32(data, offset + 0x14);
                uint form = slgf & 0x1F;
                uint gender = (slgf >> 5) & 3; // 0 M; 1 F; 2 G
                uint level = (slgf >> 7) & 0x7F;
                uint shiny = (slgf >> 14) & 0x1;
                uint unkn = slgf >> 15;

                string nickname = Util.TrimFromZero(Encoding.Unicode.GetString(data, offset + 0x18, 22));
                string OTname = Util.TrimFromZero(Encoding.Unicode.GetString(data, offset + 0x30, 22));

                if (species == 0) 
                    continue; 

                moncount++;
                string genderstr=gendersymbols[gender];
                string shinystr = (shiny == 1) ? "Yes" : "No";

                s += "Name: " + nickname;
                s += " (" + Form1.specieslist[species] + " - " + genderstr + ")" + Environment.NewLine;
                s += "Level: " + level.ToString() + Environment.NewLine;
                s += "Shiny: " + shinystr + Environment.NewLine;
                s += "Held Item: " + Form1.itemlist[helditem] + Environment.NewLine;
                s += "Move 1: " + Form1.movelist[move1] + Environment.NewLine;
                s += "Move 2: " + Form1.movelist[move2] + Environment.NewLine;
                s += "Move 3: " + Form1.movelist[move3] + Environment.NewLine;
                s += "Move 4: " + Form1.movelist[move4] + Environment.NewLine;
                s += "OT: " + OTname + " (" + TID.ToString() + "/" + SID.ToString() + ")" + Environment.NewLine;
                s += Environment.NewLine;

                offset += 0x48;
            }

            if (sender != null)
            {
                NUP_PartyIndex.Maximum = moncount;
                NUP_PartyIndex.Value = 1;
                NUP_PartyIndex_ValueChanged(sender, e);
            }
            else editing = true;
        end:
            RTB.Text = s;
            RTB.Font = new Font("Courier New", 8);
        }
        private void NUP_PartyIndex_ValueChanged(object sender, EventArgs e)
        {
            editing = false;
            int index = listBox1.SelectedIndex;
            int offset = index * 0x1B4 + (Convert.ToInt32(NUP_PartyIndex.Value)-1) * 0x48;

            int species = BitConverter.ToUInt16(data, offset + 0x00);
            CB_Species.SelectedValue = species;
            int item = BitConverter.ToUInt16(data, offset + 0x02);
            CB_HeldItem.SelectedValue = item;
            int move1 = BitConverter.ToUInt16(data, offset + 0x04);
            int move2 = BitConverter.ToUInt16(data, offset + 0x06);
            int move3 = BitConverter.ToUInt16(data, offset + 0x08);
            int move4 = BitConverter.ToUInt16(data, offset + 0x0A);
            CB_Move1.SelectedValue = move1;
            CB_Move2.SelectedValue = move2;
            CB_Move3.SelectedValue = move3;
            CB_Move4.SelectedValue = move4;

            uint EC = BitConverter.ToUInt32(data, offset + 0xC);
            TB_EC.Text = EC.ToString("X8");

            TB_TID.Text = BitConverter.ToUInt16(data, offset + 0x10).ToString("00000");
            TB_SID.Text = BitConverter.ToUInt16(data, offset + 0x12).ToString("00000");

            TB_Nickname.Text = Util.TrimFromZero(Encoding.Unicode.GetString(data, offset + 0x18, 22));
            TB_OT.Text = Util.TrimFromZero(Encoding.Unicode.GetString(data, offset + 0x30, 22));

            uint slgf = BitConverter.ToUInt32(data, offset + 0x14);
            uint form = slgf & 0x1F;
            uint gender = (slgf >> 5) & 3; // 0 M; 1 F; 2 G
            uint level = (slgf >> 7) & 0x7F;
            uint shiny = (slgf >> 14) & 0x1;
            uint nick = BitConverter.ToUInt16(data,offset+0x16);

            CHK_Shiny.Checked = shiny == 1;

            TB_Level.Text = level.ToString("000");

            CHK_Nicknamed.Checked = nick == 1;

            m_parent.setForms(species, CB_Form);
            CB_Form.SelectedIndex = (int)form;
            setGenderLabel((int)gender);
            updateNickname(sender, e);
            UpdateImage(species, (CB_Form.SelectedIndex & 0x1F), Util.getIndex(CB_HeldItem), PKX.getGender(Label_Gender.Text), CHK_Shiny.Checked);
        }
        private void Write_Entry(object sender, EventArgs e)
        {           
            if (!editing)
                return; //Don't do writing until loaded

            Validate_TextBoxes();
            byte[] StringBuffer = new byte[22]; //Mimic in-game behavior of not clearing strings. It's awful, but accuracy > good.
            string[] text_writes = new string[6 * 2]; //2 strings per mon, 6 mons

            int index = listBox1.SelectedIndex;

            int offset = index * 0x1B4;
            for (int i = 0; i < text_writes.Length; i++)
            {
                string nickname = Util.TrimFromZero(Encoding.Unicode.GetString(data, offset + 0x18, 22));
                string OTname = Util.TrimFromZero(Encoding.Unicode.GetString(data, offset + 0x30, 22));
                text_writes[i] = nickname;
                i++;
                text_writes[i] = OTname;
                offset += 0x48;
            }

            int partymember = (Convert.ToInt32(NUP_PartyIndex.Value) - 1);

            text_writes[partymember * 2] = TB_Nickname.Text;
            text_writes[partymember * 2 + 1] = TB_OT.Text;
           
            offset = index * 0x1B4 + partymember * 0x48;

            Array.Copy(BitConverter.GetBytes(Convert.ToUInt16(CB_Species.SelectedValue)), 0, data, offset + 0x00, 2);
            Array.Copy(BitConverter.GetBytes(Convert.ToUInt16(CB_HeldItem.SelectedValue)), 0, data, offset + 0x02, 2);
            Array.Copy(BitConverter.GetBytes(Convert.ToUInt16(CB_Move1.SelectedValue)), 0, data, offset + 0x04, 2);
            Array.Copy(BitConverter.GetBytes(Convert.ToUInt16(CB_Move2.SelectedValue)), 0, data, offset + 0x06, 2);
            Array.Copy(BitConverter.GetBytes(Convert.ToUInt16(CB_Move3.SelectedValue)), 0, data, offset + 0x08, 2);
            Array.Copy(BitConverter.GetBytes(Convert.ToUInt16(CB_Move4.SelectedValue)), 0, data, offset + 0x0A, 2);
            Array.Copy(BitConverter.GetBytes(Convert.ToUInt32(TB_EC.Text,16)), 0, data, offset + 0x0C, 4);

            Array.Copy(BitConverter.GetBytes(Convert.ToUInt16(TB_TID.Text)), 0, data, offset + 0x010, 2);
            Array.Copy(BitConverter.GetBytes(Convert.ToUInt16(TB_SID.Text)), 0, data, offset + 0x012, 2);

            #region Write Strings
            {
                int ofs = index * 0x1B4;
                for (int i = 0; i < 6; i++)
                {
                    Array.Copy(Encoding.Unicode.GetBytes(text_writes[i * 2]), StringBuffer, text_writes[i*2].Length*2);
                    int pos = text_writes[i * 2].Length * 2;
                    if (pos<StringBuffer.Length)
                        StringBuffer[pos] = 0;

                    Array.Copy(StringBuffer, 0, data, ofs + 0x18, 22);
                    Array.Copy(Encoding.Unicode.GetBytes(text_writes[i * 2 + 1]), StringBuffer, text_writes[i * 2 + 1].Length * 2);
                    pos = text_writes[i * 2 + 1].Length * 2;
                    if (pos < StringBuffer.Length)
                        StringBuffer[pos] = 0;

                    Array.Copy(StringBuffer, 0, data, ofs + 0x30, 22);
                    ofs += 0x48;
                }
            }
            #endregion

            uint rawslgf = BitConverter.ToUInt32(data, offset + 0x14);
            uint slgf = 0;
            slgf |= (uint)(CB_Form.SelectedIndex & 0x1F);
            slgf |= (uint)((PKX.getGender(Label_Gender.Text) & 0x3) << 5);
            slgf |= (uint)((Convert.ToUInt16(TB_Level.Text) & 0x7F) << 7);
            if (CHK_Shiny.Checked)
                slgf |= 1 << 14;

            slgf |= (rawslgf & 0x8000);
            Array.Copy(BitConverter.GetBytes(slgf), 0, data, offset + 0x014, 2);

            uint nick = 0;
            if (CHK_Nicknamed.Checked)
                nick = 1;
            Array.Copy(BitConverter.GetBytes(nick), 0, data, offset + 0x016, 2);

            offset = index * 0x1B4;

            uint vnd = 0;
            uint date = 0;
            vnd |= (Convert.ToUInt32(TB_VN.Text) & 0xFF);
            date |= (uint)((CAL_MetDate.Value.Year - 2000) & 0xFF);
            date |= (uint)((CAL_MetDate.Value.Month & 0xF) << 8);
            date |= (uint)((CAL_MetDate.Value.Day & 0x1F) << 12);
            vnd |= ((date & 0x1FFFF) << 14);
            //Fix for top bit
            uint rawvnd = BitConverter.ToUInt32(data, offset + 0x1B0);
            vnd |= (rawvnd & 0x80000000);
            Array.Copy(BitConverter.GetBytes(vnd), 0, data, offset + 0x1B0, 4);

            UpdateImage(Util.getIndex(CB_Species), (CB_Form.SelectedIndex & 0x1F), Util.getIndex(CB_HeldItem), PKX.getGender(Label_Gender.Text), CHK_Shiny.Checked);
            displayEntry(null, null); // refresh text view
        }
        private void Validate_TextBoxes()
        {
            TB_Level.Text = Math.Min(Util.ToInt32(TB_Level.Text), 100).ToString();
            TB_VN.Text = Math.Min(Util.ToInt32(TB_VN.Text), 255).ToString();
            TB_TID.Text = Math.Min(Util.ToInt32(TB_TID.Text), 65535).ToString();
            TB_SID.Text = Math.Min(Util.ToInt32(TB_SID.Text), 65535).ToString();
        }
        private void updateNickname(object sender, EventArgs e)
        {
            if (!CHK_Nicknamed.Checked)
            {
                // Fetch Current Species and set it as Nickname Text
                int species = Util.getIndex(CB_Species);
                if (species == 0 || species > 721)
                    TB_Nickname.Text = "";
                else
                {
                    // get language
                    string l = Form1.curlanguage;
                    TB_Nickname.Text = Util.getStringList("Species", l)[species];
                }
            }
            TB_Nickname.ReadOnly = !CHK_Nicknamed.Checked;

            Write_Entry(null, null);
        }
        private void updateSpecies(object sender, EventArgs e)
        {
            int species = Util.getIndex(CB_Species);
            m_parent.setForms(species, CB_Form);
            updateNickname(null, null);
        }
        private void updateShiny(object sender, EventArgs e)
        {
            if (!editing)
                return; //Don't do writing until loaded
            UpdateImage(Util.getIndex(CB_Species), (CB_Form.SelectedIndex & 0x1F), Util.getIndex(CB_HeldItem), PKX.getGender(Label_Gender.Text), CHK_Shiny.Checked);
            Write_Entry(null, null);
        }
        private void updateGender(object sender, EventArgs e)
        {
            // Get Gender Threshold
            int species = Util.getIndex(CB_Species);
            PersonalParser.Personal MonData = PKX.PersonalGetter.GetPersonal(species);
            int gt = MonData.GenderRatio;

            if (gt == 255)
                Label_Gender.Text = gendersymbols[2];
            else if (gt == 254)
                Label_Gender.Text = gendersymbols[1];
            else if (gt == 0)
                Label_Gender.Text = gendersymbols[0];

            if (gt == 255 || gt == 0 || gt==254) // Single gender/genderless
                return;

            if (gt < 256) // If not a single gender(less) species:
            {
                if (PKX.getGender(Label_Gender.Text) == 0) // ♂
                    Label_Gender.Text = gendersymbols[1]; // ♀
                else
                    Label_Gender.Text = gendersymbols[0]; // ♂

                if (PKX.getGender(CB_Form.Text) == 0 && Label_Gender.Text != gendersymbols[0])
                    CB_Form.SelectedIndex = 1;
                else if (PKX.getGender(CB_Form.Text) == 1 && Label_Gender.Text != gendersymbols[1])
                    CB_Form.SelectedIndex = 0;
            }

            if (species == 668)
                CB_Form.SelectedIndex = PKX.getGender(Label_Gender.Text);

            Write_Entry(null, null);
        }
        private void setGenderLabel(int gender)
        {
            if (gender == 0)
                Label_Gender.Text = gendersymbols[0];    // Male
            else if (gender == 1)
                Label_Gender.Text = gendersymbols[1];    // Female
            else
                Label_Gender.Text = gendersymbols[2];    // Genderless

            Write_Entry(null, null);
        }
        private void UpdateImage(int species, int form, int item, int gender, bool shiny)
        {
            string file = "";

            if (species == 0)
            { bpkx.Image = (Image)Properties.Resources.ResourceManager.GetObject("_0"); }
            else
            {
                file = "_" + species.ToString();
                if (form > 0) // Alt Form Handling
                    file = file + "_" + form.ToString();
                else if ((gender == 1) && (species == 521 || species == 668))   // Unfezant & Pyroar
                    file = file = "_" + species.ToString() + "f";
            }

            Image baseImage = (Image)Properties.Resources.ResourceManager.GetObject(file);
            if (shiny)
            {   // Is Shiny
                // Redraw our image
                baseImage = PKHeX.Util.LayerImage(baseImage, Properties.Resources.rare_icon, 0, 0, 0.7);
            }
            if (item > 0)
            {
                // Has Item
                Image itemimg = (Image)Properties.Resources.ResourceManager.GetObject("item_" + item.ToString());
                if (itemimg == null) itemimg = Properties.Resources.helditem;
                // Redraw
                baseImage = PKHeX.Util.LayerImage(baseImage, itemimg, 22 + (15 - itemimg.Width) / 2, 15 + (15 - itemimg.Height), 1);
            }
            bpkx.Image = baseImage;
            editing = true;
        }

        private void B_CopyText_Click(object sender, EventArgs e)
        {
            try { Clipboard.SetText(RTB.Text); }
            catch { };
        }
    }
}
