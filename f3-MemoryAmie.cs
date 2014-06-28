using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace PKHeX
{
    public partial class MemoryAmie : Form
    {
        Form1 m_parent;
        public byte[] h = new Byte[260]; // Always Visible
        private string disabled = "Disabled";
        private string notleft = "Never left";
        private string ot = "OT";
        private string past = "Past Gen";
        private string withOT = "Memories with"; // these get replaced when the form is translated.
        private string[] vartypes = new string[5];
        public MemoryAmie(Form1 frm1)
        {
            InitializeComponent();
            m_parent = frm1;
            TranslateInterface("MemoryAmie");
            string[] arguments = Regex.Split(L_Arguments.Text, " ; ");

            for (int i = 5; i < Math.Min(arguments.Length,vartypes.Length+5); i++)
            {
                if (arguments[i] == null) continue;
                vartypes[i - 5] = arguments[i] + ":";
            }
            disabled = arguments[0];
            notleft = arguments[1];
            ot = arguments[2];
            past = arguments[3];
            withOT = arguments[4];
            
            h = m_parent.buff;

            // Set the current friendship from main window
            if (m_parent.buff[0x93] == 0)
            {
                m_parent.buff[0xCA] = (byte)Convert.ToUInt16(m_parent.TB_Friendship.Text);
            }
            else
            {
                m_parent.buff[0xA2] = (byte)Convert.ToUInt16(m_parent.TB_Friendship.Text);
            }

            getCountries();
            getLangStrings();
            loadFields();

        }
        public string[] feeling;
        public string[] quality;

        // Conversion
        public void TranslateInterface(string FORM_NAME)
        {
            string curlanguage = m_parent.curlanguage;
            // Fetch a File
            // Check to see if a the translation file exists in the same folder as the executable
            string externalLangPath = System.Windows.Forms.Application.StartupPath + "\\lang_" + curlanguage + ".txt";
            string[] rawlist;
            if (File.Exists(externalLangPath))
            {
                rawlist = File.ReadAllLines(externalLangPath);
            }
            else
            {
                object txt;
                txt = Properties.Resources.ResourceManager.GetObject("lang_" + curlanguage); // Fetch File, \n to list.
                if (txt == null) return; // Translation file does not exist as a resource; abort this function and don't translate UI.
                string[] stringSeparators = new string[] { "\r\n" };
                rawlist = ((string)txt).Split(stringSeparators, StringSplitOptions.None);
            }

            string[] stringdata = new string[rawlist.Length];
            int itemsToRename = 0;
            for (int i = 0; i < rawlist.Length; i++)
            {
                // Find our starting point
                if (rawlist[i] == "! " + FORM_NAME) // Start our data
                {
                    // Copy our Control Names and Text to a new array for later processing.
                    for (int j = i + 1; j < rawlist.Length; j++)
                    {
                        if (rawlist[j].Length == 0)
                            continue; // Skip Over Empty Lines, errhandled
                        if (rawlist[j][0].ToString() != "-") // If line is not a comment line...
                        {
                            if (rawlist[j][0].ToString() == "!") // Stop if we have reached the end of translation
                                break; // exit inner loop
                            stringdata[itemsToRename] = rawlist[j]; // Add the entry to process later.
                            itemsToRename++;
                        }
                    }
                    break; // exit outer loop
                }
            }

            // Now that we have our items to rename in: Control = Text format, let's execute the changes!

            for (int i = 0; i < itemsToRename; i++)
            {
                string[] SplitString = Regex.Split(stringdata[i], " = ");
                if (SplitString.Length < 2)
                    continue; // Error in Input, errhandled
                string ctrl = SplitString[0]; // Control to change the text of...
                string text = SplitString[1]; // Text to set Control.Text to...
                Control[] controllist = Controls.Find(ctrl, true);
                if (controllist.Length == 0) // If Control isn't found...
                {
                    // If not found, it is not something to rename and is thus skipped.
                }
                else // Set the input control's text.
                {
                    controllist[0].Text = text;
                }
            }
        }
        public static uint ToUInt32(String value)
        {
            if (String.IsNullOrEmpty(value))
                return 0;
            return UInt32.Parse(value);
        }
        private static int ToInt32(String value)
        {
            if (String.IsNullOrEmpty(value))
                return 0;
            return Int32.Parse(value);
        }
        public int getIndex(ComboBox cb)
        {
            int val = 0;
            try { val = ToInt32(cb.SelectedValue.ToString()); }
            catch { };
            return val;
        }

        // Load/Save Actions
        private void loadFields()
        {
            v1s(Region0, 0x94); v1cb(CB_Country0, 0x95);
            v1s(Region1, 0x96); v1cb(CB_Country1, 0x97);
            v1s(Region2, 0x98); v1cb(CB_Country2, 0x99);
            v1s(Region3, 0x9A); v1cb(CB_Country3, 0x9B);
            v1s(Region4, 0x9C); v1cb(CB_Country4, 0x9D);

            v1s(M_CT_Friendship, 0xA2); v1s(M_CT_Affection, 0xA3);
            CB_CTQual.SelectedIndex = Math.Max(0,h[0xA4]-1);
            v1cb(CB_CTMemory, 0xA5); v2cb(CB_CTVar, 0xA8); i1cb(CB_CTFeel, 0xA6); 
            v1s(M_Fullness, 0xAE); v1s(M_Enjoyment, 0xAF); 

            v1s(M_OT_Friendship, 0xCA); v1s(M_OT_Affection, 0xCB);
            CB_OTQual.SelectedIndex = Math.Max(0,h[0xCC]-1);
            v1cb(CB_OTMemory, 0xCD); v2cb(CB_OTVar, 0xCE); i1cb(CB_OTFeel, 0xD0); 

            CB_Handler.Items.Clear();
            CB_Handler.Items.AddRange(new object[] { m_parent.TB_OT.Text + " ("+ot+")"});
            if ((m_parent.TB_OTt2.Text != "") && (m_parent.TB_OTt2.Text != "\0\0\0\0\0\0\0\0\0\0\0\0"))
            {
                CB_Handler.Items.AddRange(new object[] { m_parent.TB_OTt2.Text });
                CB_Handler.Enabled = true;
            }
            else
            {
                m_parent.buff[0x93] = 0;
                CB_Handler.Enabled = false;
            }
            CB_Handler.SelectedIndex = m_parent.buff[0x93];

            if (m_parent.CHK_IsEgg.Checked)
            {
                L_OT_Friendship.Text = m_parent.Label_HatchCounter.Text;
                GB_M_OT.Enabled = GB_M_CT.Enabled = GB_Residence.Enabled = false;

                BTN_Save.Enabled = M_Fullness.Enabled = M_Enjoyment.Enabled = false;
                L_Fullness.Enabled = L_Enjoyment.Enabled = false;

                GB_M_OT.Text = "N/A: " + m_parent.eggname;
                GB_M_CT.Text = "N/A: " + m_parent.eggname;
            }
            else
            {
                L_OT_Friendship.Text = m_parent.Label_Friendship.Text;
                GB_M_OT.Enabled = GB_M_CT.Enabled = true;
                GB_Residence.Enabled = true;

                BTN_Save.Enabled = M_Fullness.Enabled = M_Enjoyment.Enabled = true;
                L_Fullness.Enabled = L_Enjoyment.Enabled = true;

                GB_M_OT.Text = String.Format(withOT + " (" + ot + ")", m_parent.TB_OT.Text);
                GB_M_CT.Text = String.Format(withOT,m_parent.TB_OTt2.Text);
            }
            bool ggnore;
            GB_M_CT.Enabled = true;
            GB_Residence.Enabled = true;

            if ((!m_parent.CHK_IsEgg.Checked))
            {
                int game = (int)(m_parent.getIndex(m_parent.CB_GameOrigin));
                if ((game < 24) && (game != 0))
                {
                    // Banked Mon
                    GB_M_CT.Text = withOT + " " + m_parent.TB_OTt2.Text;
                    GB_M_OT.Text = past + " " + ot + ": " + m_parent.TB_OT.Text;
                    ggnore = false;
                }
                else
                {
                    ggnore = true;
                    GB_M_OT.Text = withOT + " " + m_parent.TB_OT.Text + " (" + ot + ")";
                    GB_M_CT.Text = withOT + m_parent.TB_OTt2.Text;
                    if ((m_parent.TB_OTt2.Text == ""))
                    {
                        GB_M_CT.Enabled = false;
                        GB_Residence.Enabled = false;
                        GB_M_CT.Text = notleft + " " + ot + " - " + disabled;
                    }
                    else
                    {
                        GB_M_CT.Text = withOT + " " + m_parent.TB_OTt2.Text;
                    }
                }
                RTB_OT.Visible = CB_OTQual.Enabled = CB_OTMemory.Enabled = CB_OTFeel.Enabled = CB_OTVar.Enabled = M_OT_Affection.Enabled = ggnore;
            }
        }
        private void saveFields()
        {
            // Save Region & Country Data
            s1v(Region0, 0x94); cb1v(CB_Country0, 0x95);
            s1v(Region1, 0x96); cb1v(CB_Country1, 0x97);
            s1v(Region2, 0x98); cb1v(CB_Country2, 0x99);
            s1v(Region3, 0x9A); cb1v(CB_Country3, 0x9B);
            s1v(Region4, 0x9C); cb1v(CB_Country4, 0x9D);

            // Save 0-255 stats
            s1v(M_CT_Friendship, 0xA2); s1v(M_CT_Affection, 0xA3);
            s1v(M_OT_Friendship, 0xCA); s1v(M_OT_Affection, 0xCB);
            s1v(M_Fullness, 0xAE); s1v(M_Enjoyment, 0xAF);

            // Save Memories
            #region // CT MEMORIES
            cb1v(CB_CTMemory, 0xA5);
            if (!CB_CTVar.Enabled)
            {
                m_parent.buff[0xA8] = 0;
                m_parent.buff[0xA9] = 0;
            }
            else
            {
                cb2v(CB_CTVar, 0xA8);
            }

            // If memory doesn't contain a feeling/quality
            if (!CB_CTFeel.Enabled)
            {
                m_parent.buff[0xA4] = 0;
                m_parent.buff[0xA6] = 0;
            }
            else 
            {
                cb1i(CB_CTFeel, 0xA6);
                m_parent.buff[0xA4] = (byte)(CB_CTQual.SelectedIndex+1);
            }
            #endregion
            #region // OT MEMORIES
            cb1v(CB_OTMemory, 0xCD);
            if (!CB_OTVar.Enabled)
            {
                m_parent.buff[0xCE] = 0;
                m_parent.buff[0xCF] = 0;
            }
            else
            {
                cb2v(CB_OTVar, 0xCE);
            }

            // If memory doesn't contain a feeling/quality
            if (!CB_OTFeel.Enabled)
            {
                m_parent.buff[0xCC] = 0;
                m_parent.buff[0xD0] = 0;
            }
            else
            {
                cb1i(CB_OTFeel, 0xD0);
                m_parent.buff[0xCC] = (byte)(CB_OTQual.SelectedIndex+1);
            }
            #endregion

            m_parent.buff[0x93] = (byte)CB_Handler.SelectedIndex;

            // Update Current Friendship
            if (CB_Handler.SelectedIndex == 1)
            {
                m_parent.TB_Friendship.Text = M_CT_Friendship.Text;
                m_parent.GB_nOT.BackColor = System.Drawing.Color.FromArgb(232, 255, 255);
                m_parent.GB_OT.BackColor = Color.Transparent;
            }
            else
            {
                m_parent.TB_Friendship.Text = M_OT_Friendship.Text;
                m_parent.GB_OT.BackColor = System.Drawing.Color.FromArgb(232, 255, 255);
                m_parent.GB_nOT.BackColor = Color.Transparent;
            }
        }

        // Singular Actions
        private void v1s(MaskedTextBox t, int o)
        {
            t.Text = h[o].ToString();
        }  // Value (from ByteArray) to String (textBox): 1 byte
        private void s1v(MaskedTextBox t, int o)
        {
            h[o] = (byte)ToUInt32(t.Text);
        }  // String (textBox) to Value (in ByteArray): 1 byte

        private void v1cb(ComboBox cb, int o)
        {
            cb.SelectedValue = (int)h[o];
        }
        private void i1cb(ComboBox cb, int o)
        {
            cb.SelectedIndex = (int)h[o];
        }
        private void v2cb(ComboBox cb, int o)
        {
            cb.SelectedValue = (int)(h[o] + h[o+1]*0x100);
        }
        private void cb1v(ComboBox cb, int o)
        {
            h[o] = (byte)Convert.ToUInt32(cb.SelectedValue);
        }
        private void cb1i(ComboBox cb, int o)
        {
            h[o] = (byte)Convert.ToUInt32(cb.SelectedIndex);
        }
        private void cb2v(ComboBox cb, int o)
        {
            h[o] = (byte)(Convert.ToUInt32(cb.SelectedValue) & 0xFF);
            h[o+1] = (byte)(Convert.ToUInt32(cb.SelectedValue) >> 8);
        }

        // Event Actions
        private void B_Save_Click(object sender, EventArgs e)
        {
            saveFields();
            Close();
        }      // Button: Save pressed.
        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }    // Button: Cancel pressed.
        private void changeTextBox(object sender, EventArgs e)
        {

            // Loop through all textboxes we want to check (<0x100)
            MaskedTextBox[] tb = { 
                                     Region0, //Country0,     // GR 0
                                     Region1, //Country1,     // GR 1
                                     Region2, //Country2,     // GR 2
                                     Region3, //Country3,     // GR 3
                                     Region4, //Country4,     // GR 4

                                     M_Fullness, M_Enjoyment,
                                 };
            for (int i = 0; i < tb.Length; i++)
            {
                if (ToUInt32(tb[i].Text) < 0x100)
                {
                    tb[i].BackColor = Color.White;
                }
                else tb[i].BackColor = Color.Red;
            }

        }   // Change the contents of a text box

        private void getCountries()
        {
            #region country table
            var country_list = new[] {
                            new { Text = "---", Value = 0 },
                            new { Text = "Albania", Value = 64 },
                            new { Text = "Andorra", Value = 122 },
                            new { Text = "Anguilla", Value = 8 },
                            new { Text = "Antigua and Barbuda", Value = 9 },
                            new { Text = "Argentina", Value = 10 },
                            new { Text = "Aruba", Value = 11 },
                            new { Text = "Australia", Value = 65 },
                            new { Text = "Austria", Value = 66 },
                            new { Text = "Azerbaijan", Value = 113 },
                            new { Text = "Bahamas", Value = 12 },
                            new { Text = "Barbados", Value = 13 },
                            new { Text = "Belgium", Value = 67 },
                            new { Text = "Belize", Value = 14 },
                            new { Text = "Bermuda", Value = 186 },
                            new { Text = "Bolivia", Value = 15 },
                            new { Text = "Bosnia and Herzegovina", Value = 68 },
                            new { Text = "Botswana", Value = 69 },
                            new { Text = "Brazil", Value = 16 },
                            new { Text = "British Virgin Islands", Value = 17 },
                            new { Text = "Bulgaria", Value = 70 },
                            new { Text = "Canada", Value = 18 },
                            new { Text = "Cayman Islands", Value = 19 },
                            new { Text = "Chad", Value = 117 },
                            new { Text = "Chile", Value = 20 },
                            new { Text = "China", Value = 160 },
                            new { Text = "Colombia", Value = 21 },
                            new { Text = "Costa Rica", Value = 22 },
                            new { Text = "Croatia", Value = 71 },
                            new { Text = "Cyprus", Value = 72 },
                            new { Text = "Czech Republic", Value = 73 },
                            new { Text = "Denmark (Kingdom of)", Value = 74 },
                            new { Text = "Djibouti", Value = 120 },
                            new { Text = "Dominica", Value = 23 },
                            new { Text = "Dominican Republic", Value = 24 },
                            new { Text = "Ecuador", Value = 25 },
                            new { Text = "El Salvador", Value = 26 },
                            new { Text = "Eritrea", Value = 119 },
                            new { Text = "Estonia", Value = 75 },
                            new { Text = "Finland", Value = 76 },
                            new { Text = "France", Value = 77 },
                            new { Text = "French Guiana", Value = 27 },
                            new { Text = "Germany", Value = 78 },
                            new { Text = "Gibraltar", Value = 123 },
                            new { Text = "Greece", Value = 79 },
                            new { Text = "Grenada", Value = 28 },
                            new { Text = "Guadeloupe", Value = 29 },
                            new { Text = "Guatemala", Value = 30 },
                            new { Text = "Guernsey", Value = 124 },
                            new { Text = "Guyana", Value = 31 },
                            new { Text = "Haiti", Value = 32 },
                            new { Text = "Honduras", Value = 33 },
                            new { Text = "Hong Kong", Value = 144 },
                            new { Text = "Hungary", Value = 80 },
                            new { Text = "Iceland", Value = 81 },
                            new { Text = "India", Value = 169 },
                            new { Text = "Ireland", Value = 82 },
                            new { Text = "Isle of Man", Value = 125 },
                            new { Text = "Italy", Value = 83 },
                            new { Text = "Jamaica", Value = 34 },
                            new { Text = "Japan", Value = 1 },
                            new { Text = "Jersey", Value = 126 },
                            new { Text = "Latvia", Value = 84 },
                            new { Text = "Lesotho", Value = 85 },
                            new { Text = "Liechtenstein", Value = 86 },
                            new { Text = "Lithuania", Value = 87 },
                            new { Text = "Luxembourg", Value = 88 },
                            new { Text = "Macedonia (Republic of)", Value = 89 },
                            new { Text = "Malaysia", Value = 156 },
                            new { Text = "Mali", Value = 115 },
                            new { Text = "Malta", Value = 90 },
                            new { Text = "Martinique", Value = 35 },
                            new { Text = "Mauritania", Value = 114 },
                            new { Text = "Mexico", Value = 36 },
                            new { Text = "Monaco", Value = 127 },
                            new { Text = "Montenegro", Value = 91 },
                            new { Text = "Montserrat", Value = 37 },
                            new { Text = "Mozambique", Value = 92 },
                            new { Text = "Namibia", Value = 93 },
                            new { Text = "Netherlands", Value = 94 },
                            new { Text = "Netherlands Antilles", Value = 38 },
                            new { Text = "New Zealand", Value = 95 },
                            new { Text = "Nicaragua", Value = 39 },
                            new { Text = "Niger", Value = 116 },
                            new { Text = "Norway", Value = 96 },
                            new { Text = "Panama", Value = 40 },
                            new { Text = "Paraguay", Value = 41 },
                            new { Text = "Peru", Value = 42 },
                            new { Text = "Poland", Value = 97 },
                            new { Text = "Portugal", Value = 98 },
                            new { Text = "Romania", Value = 99 },
                            new { Text = "Russia", Value = 100 },
                            new { Text = "San Marino", Value = 184 },
                            new { Text = "Saudi Arabia", Value = 174 },
                            new { Text = "Serbia and Kosovo", Value = 101 },
                            new { Text = "Singapore", Value = 153 },
                            new { Text = "Slovakia", Value = 102 },
                            new { Text = "Slovenia", Value = 103 },
                            new { Text = "Somalia", Value = 121 },
                            new { Text = "South Africa", Value = 104 },
                            new { Text = "South Korea", Value = 136 },
                            new { Text = "Spain", Value = 105 },
                            new { Text = "St. Kitts and Nevis", Value = 43 },
                            new { Text = "St. Lucia", Value = 44 },
                            new { Text = "St. Vincent and the Grenadines", Value = 45 },
                            new { Text = "Sudan", Value = 118 },
                            new { Text = "Suriname", Value = 46 },
                            new { Text = "Swaziland", Value = 106 },
                            new { Text = "Sweden", Value = 107 },
                            new { Text = "Switzerland", Value = 108 },
                            new { Text = "Taiwan", Value = 128 },
                            new { Text = "Trinidad and Tobago", Value = 47 },
                            new { Text = "Turkey", Value = 109 },
                            new { Text = "Turks and Caicos Islands", Value = 48 },
                            new { Text = "U.A.E.", Value = 168 },
                            new { Text = "United Kingdom", Value = 110 },
                            new { Text = "United States", Value = 49 },
                            new { Text = "Uruguay", Value = 50 },
                            new { Text = "US Virgin Islands", Value = 51 },
                            new { Text = "Vatican City", Value = 185 },
                            new { Text = "Venezuela", Value = 52 },
                            new { Text = "Zambia", Value = 111 },
                            new { Text = "Zimbabwe", Value = 112 },
            };
            #endregion

            var c0_list = new BindingSource(country_list, null);
            var c1_list = new BindingSource(country_list, null);
            var c2_list = new BindingSource(country_list, null);
            var c3_list = new BindingSource(country_list, null);
            var c4_list = new BindingSource(country_list, null);


            CB_Country0.DataSource = c0_list;
            CB_Country0.DisplayMember = "Text";
            CB_Country0.ValueMember = "Value";
            CB_Country1.DataSource = c1_list;
            CB_Country1.DisplayMember = "Text";
            CB_Country1.ValueMember = "Value";
            CB_Country2.DataSource = c2_list;
            CB_Country2.DisplayMember = "Text";
            CB_Country2.ValueMember = "Value";
            CB_Country3.DataSource = c3_list;
            CB_Country3.DisplayMember = "Text";
            CB_Country3.ValueMember = "Value";
            CB_Country4.DataSource = c4_list;
            CB_Country4.DisplayMember = "Text";
            CB_Country4.ValueMember = "Value";
        }
        private void getLangStrings()
        {
            // Memory Chooser
            string[] memories = new string[64];
            int[] allowed = new int[64];
            for (int i = 0; i < 64; i++)
            {
                memories[i] = m_parent.memories[39 + i];
                allowed[i] = i+1;
            }
            List<cbItem> memory_list = getComboBoxItems2(memories, allowed);
            cbItem def = new cbItem();
            def.Text = m_parent.memories[38+0];
            def.Value = 0;
            memory_list.Insert(0, def);

            CB_OTMemory.DataSource = memory_list;
            CB_OTMemory.DisplayMember = "Text";
            CB_OTMemory.ValueMember = "Value";

            var mem1_list = new BindingSource(memory_list, null);
            CB_CTMemory.DataSource = mem1_list;
            CB_CTMemory.DisplayMember = "Text";
            CB_CTMemory.ValueMember = "Value";

            // Quality Chooser
            CB_CTQual.Items.Clear();
            CB_OTQual.Items.Clear();
            for (int i = 0; i < 7; i++)
            {
                CB_CTQual.Items.Add(m_parent.memories[2 + i]);
                CB_OTQual.Items.Add(m_parent.memories[2 + i]);
            }

            // Feeling Chooser
            CB_CTFeel.Items.Clear();
            CB_OTFeel.Items.Clear();
            for (int i = 0; i < 24; i++)
            {
                CB_CTFeel.Items.Add(m_parent.memories[10 + i]);
                CB_OTFeel.Items.Add(m_parent.memories[10 + i]);
            }
        }
        private void getMemoryArguments(string ARG, ComboBox sender)
        {
            List<cbItem> argvals = new List<cbItem>();
            #region General Locations
            // add general locations
            int[] allowed = { };
            allowed = new int[62];
            for (int i = 0; i < 62; i++)
            {
                allowed[i] = i;
            }
            List<cbItem> genloc = getComboBoxItems(m_parent.genloc, allowed);
            #endregion
            #region Items
            allowed = new int[697] 
            { 
            1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,
            50,51,52,53,54,55,56,57,58,59,60,61,62,63,64,65,66,67,68,69,70,71,72,73,74,75,76,77,78,79,80,81,82,83,84,85,86,87,88,89,90,91,92,93,94,95,96,97,98,99,
            100,101,102,103,104,105,106,107,108,109,110,111,112,116,117,118,119,134,135,136,137,138,139,140,141,142,143,144,145,146,147,148,149,
            150,151,152,153,154,155,156,157,158,159,160,161,162,163,164,165,166,167,168,169,170,171,172,173,174,175,176,177,178,179,180,181,182,183,184,185,186,187,188,189,190,191,192,193,194,195,196,197,198,199,
            200,201,202,203,204,205,206,207,208,209,210,211,212,213,214,215,216,217,218,219,220,221,222,223,224,225,226,227,228,229,230,231,232,233,234,235,236,237,238,239,240,241,242,243,244,245,246,247,248,249,
            250,251,252,253,254,255,256,257,258,259,260,261,262,263,264,265,266,267,268,269,270,271,272,273,274,275,276,277,278,279,280,281,282,283,284,285,286,287,288,289,290,291,292,293,294,295,296,297,298,299,
            300,301,302,303,304,305,306,307,308,309,310,311,312,313,314,315,316,317,318,319,320,321,322,323,324,325,326,327,328,329,330,331,332,333,334,335,336,337,338,339,340,341,342,343,344,345,346,347,348,349,
            350,351,352,353,354,355,356,357,358,359,360,361,362,363,364,365,366,367,368,369,370,371,372,373,374,375,376,377,378,379,380,381,382,383,384,385,386,387,388,389,390,391,392,393,394,395,396,397,398,399,
            400,401,402,403,404,405,406,407,408,409,410,411,412,413,414,415,416,417,418,419,420,421,422,423,424,425,428,429,430,431,432,433,434,435,436,437,438,439,440,441,442,443,444,445,446,447,448,449,
            450,451,452,453,454,455,456,457,458,459,460,461,462,463,464,465,466,467,468,469,470,471,472,473,474,475,476,477,478,479,480,481,482,483,484,485,486,487,488,489,490,491,492,493,494,495,496,497,498,499,
            500,501,502,503,504,505,506,507,508,509,510,511,512,513,514,515,516,517,518,519,520,521,522,523,524,525,526,527,528,529,530,531,532,533,534,535,536,537,538,539,540,541,542,543,544,545,546,547,548,549,
            550,551,552,553,554,555,556,557,558,559,560,561,562,563,564,565,566,567,568,569,570,571,572,573,574,575,576,577,578,579,580,581,582,583,584,585,586,587,588,589,590,591,592,593,594,595,596,597,598,599,
            600,601,602,603,604,605,606,607,608,609,610,611,612,613,614,615,616,617,618,619,620,621,623,624,625,626,627,628,629,630,631,632,633,634,635,636,637,638,639,640,641,642,643,644,645,646,647,648,649,
            650,651,652,653,654,655,656,657,658,659,660,661,662,663,664,665,666,667,668,669,670,671,672,673,674,675,676,677,678,679,680,681,682,683,684,685,686,687,688,689,690,691,692,693,694,695,696,697,698,699,
            700,701,702,703,704,705,706,707,708,709,710,711,712,713,714,715,716,717
            };
            List<cbItem> item_list = getComboBoxItems(m_parent.itemlist, allowed);
            #endregion
            List<cbItem> species_list = (List<cbItem>)m_parent.CB_Species.DataSource;

            // Met Locations for Pokecenters healing
            allowed = new int[] {
                2,6,8,10,12,14,16,17,18,20,22,24,26,28,30,32,34,36,38,40,42,44,46,48,50,52,54,56,58,60,62,64,66,68,70,72,74,76,78,82,84,86,88,90,92,94,96,98,100,102,104,106,108,110,112,114,116,118,120,122,124,126,128,130,132,134,136,138,140,142,144,146,148,150,152,154,156,158,160,162,164,166,168,
            };
            List<cbItem> locationXY = getComboBoxItems(m_parent.metXY_00000, allowed);
            #region Moves
            
                List<cbItem> move_list = new List<cbItem>();
                // Sort the Rest based on String Name
                string[] sortedmoves = new string[m_parent.movelist.Length];
                Array.Copy(m_parent.movelist, sortedmoves, m_parent.movelist.Length);
                Array.Sort(sortedmoves);

                // Add the rest of the items
                for (int i = 0; i < sortedmoves.Length; i++)
                {
                    cbItem ncbi = new cbItem();
                    ncbi.Text = sortedmoves[i];
                    ncbi.Value = Array.IndexOf(m_parent.movelist, sortedmoves[i]);
                    move_list.Add(ncbi);
                }
            
            #endregion
            string vs = "";
            bool enabled = true;
            if (ARG == "NONE")
            {
                enabled = false;
                vs = "";
            }
            else if (ARG == "PKM")
            {
                argvals = species_list;
                vs = vartypes[0];
            }
            else if (ARG == "GENLOC")
            {
                argvals = genloc;
                vs = vartypes[1];
            }
            else if (ARG == "ITEM")
            {
                argvals = item_list;
                vs = vartypes[2];
            }
            else if (ARG == "MOVE")
            {
                argvals = move_list;
                vs = vartypes[3];
            }
            else if (ARG == "LOCATION")
            {
                argvals = locationXY;
                vs = vartypes[4];
            }

            if (sender == CB_CTMemory)
            {
                List<cbItem> CTmemargs = new List<cbItem>();
                CTmemargs = argvals;
                CB_CTVar.DataSource = CTmemargs;
                CB_CTVar.DisplayMember = "Text";
                CB_CTVar.ValueMember = "Value";
                LCTV.Text = vs;
                LCTV.Visible = CB_CTVar.Visible = CB_CTVar.Enabled = enabled;
            }
            else
            {
                List<cbItem> OTmemargs = new List<cbItem>();
                OTmemargs = argvals;
                CB_OTVar.DataSource = OTmemargs;
                CB_OTVar.DisplayMember = "Text";
                CB_OTVar.ValueMember = "Value";
                LOTV.Text = vs;
                LOTV.Visible = CB_OTVar.Visible = CB_OTVar.Enabled = enabled;
            }

        }
        private List<cbItem> getComboBoxItems(string[] list, int[] allowed)
        {

            List<cbItem> combolist = new List<cbItem>();
            // Sort the Rest based on String Name
            string[] sorter = new string[list.Length];
            Array.Copy(list, sorter, list.Length);
            Array.Sort(sorter);

            for (int i = 0; i < sorter.Length; i++)
            {
                int locnum = Array.IndexOf(allowed, Array.IndexOf(list, sorter[i]));
                if (locnum >= 0)	// If the given text is allowed (if found, >0)
                {
                    cbItem ncbi = new cbItem();
                    ncbi.Text = sorter[i];
                    ncbi.Value = allowed[locnum];
                    combolist.Add(ncbi);
                }
            }
            return combolist;
        }
        private List<cbItem> getComboBoxItems2(string[] list, int[] allowed)
        {

            List<cbItem> combolist = new List<cbItem>();
            // Sort the Rest based on String Name
            string[] sorter = new string[list.Length];
            Array.Copy(list, sorter, list.Length);
            Array.Sort(sorter);

            for (int i = 0; i < sorter.Length; i++)
            {
                int locnum = Array.IndexOf(list, sorter[i]);
                if (locnum >= 0)	// If the given text is allowed (if found, >0)
                {
                    cbItem ncbi = new cbItem();
                    ncbi.Text = sorter[i];
                    ncbi.Value = allowed[locnum];
                    combolist.Add(ncbi);
                }
            }
            return combolist;
        }
        private string getMemoryString(ComboBox m, ComboBox arg, ComboBox q, ComboBox f, string tr)
        {
            string result = "";
            string nn = m_parent.TB_Nickname.Text;
            string a = arg.Text;
            int mem = getIndex(m);

            bool enabled = false;
            if (mem == 0)
            {
                result = m_parent.memories[38];
            }
            else
            {
                result = String.Format(m_parent.memories[mem + 38], nn, tr, a, f.Text, q.Text);
                enabled = true;
            }

            // ugh, labels
            if (q == CB_CTQual)
            {
                L_CT_Quality.Visible = L_CT_Feeling.Visible = enabled;
            }
            else
                L_OT_Quality.Visible = L_OT_Feeling.Visible = enabled;

            q.Visible = q.Enabled = f.Visible = f.Enabled = enabled;

            return result;
        }

        private void changeMemory(object sender, EventArgs e)
        {
            ComboBox m = (ComboBox)sender;
            if (m == CB_CTMemory || m == CB_OTMemory)
            {
                int memory = getIndex(m);
                switch (memory) // dis switchtable...
                {
                    case 0: getMemoryArguments("NONE", m); break;
                    case 1: getMemoryArguments("GENLOC", m); break;
                    case 2: getMemoryArguments("GENLOC", m); break;
                    case 3: getMemoryArguments("GENLOC", m); break;
                    case 4: getMemoryArguments("GENLOC", m); break;
                    case 5: getMemoryArguments("ITEM", m); break;
                    case 6: getMemoryArguments("LOCATION", m); break;
                    case 7: getMemoryArguments("PKM", m); break;
                    case 8: getMemoryArguments("NONE", m); break;
                    case 9: getMemoryArguments("PKM", m); break;
                    case 10: getMemoryArguments("NONE", m); break;
                    case 11: getMemoryArguments("NONE", m); break;
                    case 12: getMemoryArguments("MOVE", m); break;
                    case 13: getMemoryArguments("PKM", m); break;
                    case 14: getMemoryArguments("PKM", m); break;
                    case 15: getMemoryArguments("ITEM", m); break;
                    case 16: getMemoryArguments("MOVE", m); break;
                    case 17: getMemoryArguments("PKM", m); break;
                    case 18: getMemoryArguments("PKM", m); break;
                    case 19: getMemoryArguments("GENLOC", m); break;
                    case 20: getMemoryArguments("NONE", m); break;
                    case 21: getMemoryArguments("PKM", m); break;
                    case 22: getMemoryArguments("NONE", m); break;
                    case 23: getMemoryArguments("NONE", m); break;
                    case 24: getMemoryArguments("GENLOC", m); break;
                    case 25: getMemoryArguments("PKM", m); break;
                    case 26: getMemoryArguments("ITEM", m); break;
                    case 27: getMemoryArguments("NONE", m); break;
                    case 28: getMemoryArguments("NONE", m); break;
                    case 29: getMemoryArguments("PKM", m); break;
                    case 30: getMemoryArguments("NONE", m); break;
                    case 31: getMemoryArguments("GENLOC", m); break;
                    case 32: getMemoryArguments("GENLOC", m); break;
                    case 33: getMemoryArguments("GENLOC", m); break;
                    case 34: getMemoryArguments("ITEM", m); break;
                    case 35: getMemoryArguments("GENLOC", m); break;
                    case 36: getMemoryArguments("GENLOC", m); break;
                    case 37: getMemoryArguments("GENLOC", m); break;
                    case 38: getMemoryArguments("GENLOC", m); break;
                    case 39: getMemoryArguments("GENLOC", m); break;
                    case 40: getMemoryArguments("ITEM", m); break;
                    case 41: getMemoryArguments("NONE", m); break;
                    case 42: getMemoryArguments("GENLOC", m); break;
                    case 43: getMemoryArguments("NONE", m); break;
                    case 44: getMemoryArguments("PKM", m); break;
                    case 45: getMemoryArguments("PKM", m); break;
                    case 46: getMemoryArguments("NONE", m); break;
                    case 47: getMemoryArguments("NONE", m); break;
                    case 48: getMemoryArguments("MOVE", m); break;
                    case 49: getMemoryArguments("MOVE", m); break;
                    case 50: getMemoryArguments("PKM", m); break;
                    case 51: getMemoryArguments("ITEM", m); break;
                    case 52: getMemoryArguments("GENLOC", m); break;
                    case 53: getMemoryArguments("NONE", m); break;
                    case 54: getMemoryArguments("NONE", m); break;
                    case 55: getMemoryArguments("NONE", m); break;
                    case 56: getMemoryArguments("NONE", m); break;
                    case 57: getMemoryArguments("NONE", m); break;
                    case 58: getMemoryArguments("NONE", m); break;
                    case 59: getMemoryArguments("GENLOC", m); break;
                    case 60: getMemoryArguments("PKM", m); break;
                    case 61: getMemoryArguments("NONE", m); break;
                    case 62: getMemoryArguments("NONE", m); break;
                    case 63: getMemoryArguments("NONE", m); break;
                    case 64: getMemoryArguments("NONE", m); break;
                    default: m.SelectedIndex = 0; break;
                };
            }

            RTB_OT.Text = getMemoryString(CB_OTMemory, CB_OTVar, CB_OTQual, CB_OTFeel, m_parent.TB_OT.Text);
            RTB_CT.Text = getMemoryString(CB_CTMemory, CB_CTVar, CB_CTQual, CB_CTFeel, m_parent.TB_OTt2.Text);
        }
        private void changeCountry(object sender, EventArgs e)
        {
            ComboBox[] cba = new ComboBox[]
            {
                CB_Country0, CB_Country1, CB_Country2, CB_Country3, CB_Country4
            };
            MaskedTextBox[] mta = new MaskedTextBox[]
            {
                Region0, Region1, Region2, Region3, Region4,
            };
            for (int i = 0; i < cba.Length; i++)
            {
                if (cba[i].SelectedIndex == 0)
                {
                    mta[i].Text = "0";
                    mta[i].Enabled = false;
                }
                else
                    mta[i].Enabled = true;
            }
        }
        private void changeHandler(object sender, EventArgs e)
        {
            if (CB_Handler.SelectedIndex == 1)
            {
                m_parent.TB_Friendship.Text = M_CT_Friendship.Text;
                m_parent.GB_nOT.BackColor = System.Drawing.Color.FromArgb(232, 255, 255);
                m_parent.GB_OT.BackColor = Color.Transparent;

                GB_M_CT.BackColor = System.Drawing.Color.FromArgb(232, 255, 255);
                GB_M_OT.BackColor = Color.Transparent;

            }
            else
            {
                m_parent.TB_Friendship.Text = M_OT_Friendship.Text;
                m_parent.GB_OT.BackColor = System.Drawing.Color.FromArgb(232, 255, 255);
                m_parent.GB_nOT.BackColor = Color.Transparent;

                GB_M_OT.BackColor = System.Drawing.Color.FromArgb(232, 255, 255);
                GB_M_CT.BackColor = Color.Transparent;
            }
        }

    }
}
