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
            Util.TranslateInterface(this, Form1.curlanguage);
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
        public byte[] sav = new byte[0x100000];
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
                var species_list = Util.getCBList(Form1.specieslist, null);
                species_list.RemoveAt(0); // Remove 0th Entry
                CB_Species.DisplayMember = "Text";
                CB_Species.ValueMember = "Value";
                CB_Species.DataSource = species_list;
            }
            #endregion
            #region Moves
            {
                CB_Move1.DisplayMember = CB_Move2.DisplayMember = CB_Move3.DisplayMember = CB_Move4.DisplayMember = "Text";
                CB_Move1.ValueMember = CB_Move2.ValueMember = CB_Move3.ValueMember = CB_Move4.ValueMember = "Value";

                CB_Move1.DataSource = new BindingSource(m_parent.CB_Move1.DataSource, null);
                CB_Move2.DataSource = new BindingSource(m_parent.CB_Move1.DataSource, null);
                CB_Move3.DataSource = new BindingSource(m_parent.CB_Move1.DataSource, null);
                CB_Move4.DataSource = new BindingSource(m_parent.CB_Move1.DataSource, null);
            }
            #endregion
            #region Items
            {
                CB_HeldItem.DisplayMember = "Text";
                CB_HeldItem.ValueMember = "Value";
                CB_HeldItem.DataSource = new BindingSource(m_parent.CB_HeldItem.DataSource, null);
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
                NUP_PartyIndex.Maximum = (moncount == 0) ? 1 : moncount;
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

            if (offset < 0) return;

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
            PKX.PersonalParser.Personal MonData = PKX.PersonalGetter.GetPersonal(species);
            int gt = MonData.GenderRatio;

            if (gt == 255)
                Label_Gender.Text = gendersymbols[2];
            else if (gt == 254)
                Label_Gender.Text = gendersymbols[1];
            else if (gt == 0)
                Label_Gender.Text = gendersymbols[0];

            if (gt == 255 || gt == 0 || gt == 254) // Single gender/genderless abort
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
