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
        public byte[] h = new byte[260]; // Always Visible
        private string disabled = "Disabled";
        private string notleft = "Never left";
        private string ot = "OT";
        private string past = "Past Gen";
        private string withOT = "Memories with"; // these get replaced when the form is translated.
        private string[] vartypes = new string[5];
        public MemoryAmie(Form1 frm1)
        {
            InitializeComponent();
            Util.TranslateInterface(this, Form1.curlanguage);
            m_parent = frm1;
            string[] arguments = Regex.Split(L_Arguments.Text, " ; ");

            for (int i = 5; i < Math.Min(arguments.Length,vartypes.Length+5); i++)
            {
                if (arguments[i] == null) continue;
                vartypes[i - 5] = arguments[i] + ":";
            }
            try
            {
                disabled = arguments[0];
                notleft = arguments[1];
                ot = arguments[2];
                past = arguments[3];
                withOT = arguments[4];
            }
            catch { };
            h = m_parent.buff;

            getCountries();
            getLangStrings();
            loadFields();
        }
        public string[] feeling;
        public string[] quality;

        // Load/Save Actions
        private void loadFields()
        {
            // Load the region/country values.
            v1cb(CB_Country0, 0x95); v1cb(Region0, 0x94);
            v1cb(CB_Country1, 0x97); v1cb(Region1, 0x96);
            v1cb(CB_Country2, 0x99); v1cb(Region2, 0x98);
            v1cb(CB_Country3, 0x9B); v1cb(Region3, 0x9A);
            v1cb(CB_Country4, 0x9D); v1cb(Region4, 0x9C);

            // Load the Fullness, and Enjoyment
            v1s(M_Fullness, 0xAE); v1s(M_Enjoyment, 0xAF);

            // Load the CT Memories
            v1s(M_CT_Friendship, 0xA2); v1s(M_CT_Affection, 0xA3);
            CB_CTQual.SelectedIndex = Math.Max(0,h[0xA4]-1);
            v1cb(CB_CTMemory, 0xA5); v2cb(CB_CTVar, 0xA8); i1cb(CB_CTFeel, 0xA6);

            // Load the OT Memories
            v1s(M_OT_Friendship, 0xCA); v1s(M_OT_Affection, 0xCB);
            CB_OTQual.SelectedIndex = Math.Max(0,h[0xCC]-1);
            v1cb(CB_OTMemory, 0xCD); v2cb(CB_OTVar, 0xCE); i1cb(CB_OTFeel, 0xD0); 

            CB_Handler.Items.Clear();
            CB_Handler.Items.AddRange(new object[] { m_parent.TB_OT.Text + " ("+ot+")"});

            if (Util.TrimFromZero(m_parent.TB_OTt2.Text) != "")
                CB_Handler.Items.AddRange(new object[] { m_parent.TB_OTt2.Text });
            else
                m_parent.buff[0x93] = 0;

            tabControl1.SelectedIndex = CB_Handler.SelectedIndex = m_parent.buff[0x93];

            if (m_parent.CHK_IsEgg.Checked)
            {
                L_OT_Friendship.Text = m_parent.Label_HatchCounter.Text;
                GB_M_OT.Enabled = GB_M_CT.Enabled = GB_Residence.Enabled = false;

                BTN_Save.Enabled = M_Fullness.Enabled = M_Enjoyment.Enabled = false;
                L_Fullness.Enabled = L_Enjoyment.Enabled = false;

                GB_M_OT.Text = "N/A: " + Form1.eggname;
                GB_M_CT.Text = "N/A: " + Form1.eggname;
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

            GB_M_CT.Enabled = true;
            GB_Residence.Enabled = true;

            if ((!m_parent.CHK_IsEgg.Checked))
            {
                bool enable = false;
                int game = (int)(Util.getIndex(m_parent.CB_GameOrigin));
                if ((game < 24) && (game != 0))
                {
                    // Banked Mon
                    GB_M_CT.Text = withOT + " " + m_parent.TB_OTt2.Text;
                    GB_M_OT.Text = past + " " + ot + ": " + m_parent.TB_OT.Text;
                    enable = false;
                }
                else
                {
                    enable = true;
                    GB_M_OT.Text = withOT + " " + m_parent.TB_OT.Text + " (" + ot + ")";
                    GB_M_CT.Text = withOT + m_parent.TB_OTt2.Text;
                    if ((m_parent.TB_OTt2.Text == ""))
                    {
                        GB_M_CT.Enabled = GB_Residence.Enabled = false;
                        GB_M_CT.Text = notleft + " " + ot + " - " + disabled;
                    }
                    else
                        GB_M_CT.Text = withOT + " " + m_parent.TB_OTt2.Text;
                }
                RTB_OT.Visible = CB_OTQual.Enabled = CB_OTMemory.Enabled = CB_OTFeel.Enabled = CB_OTVar.Enabled = M_OT_Affection.Enabled = enable;
            }
        }
        private void saveFields()
        {
            // Save Region & Country Data
            cb1v(Region0, 0x94); cb1v(CB_Country0, 0x95);
            cb1v(Region1, 0x96); cb1v(CB_Country1, 0x97);
            cb1v(Region2, 0x98); cb1v(CB_Country2, 0x99);
            cb1v(Region3, 0x9A); cb1v(CB_Country3, 0x9B);
            cb1v(Region4, 0x9C); cb1v(CB_Country4, 0x9D);

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
                cb2v(CB_CTVar, 0xA8);

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
                cb2v(CB_OTVar, 0xCE);

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
        }

        // Singular Actions
        private void v1s(MaskedTextBox t, int o)
        {
            t.Text = h[o].ToString();
        }  // Value (from ByteArray) to String (textBox): 1 byte
        private void s1v(MaskedTextBox t, int o)
        {
            h[o] = (byte)Util.ToUInt32(t.Text);
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

        private void getCountries()
        {
            ComboBox[] cba = new ComboBox[] { CB_Country0, CB_Country1, CB_Country2, CB_Country3, CB_Country4, };
            for (int i = 0; i < cba.Length; i++)
            {
                m_parent.setCountrySubRegion(cba[i], "countries");
                cba[i].DisplayMember = "Text";
                cba[i].ValueMember = "Value";
            }
        }
        private void getLangStrings()
        {
            // Memory Chooser
            int memorycount = Form1.memories.Length - 38;
            string[] memories = new string[memorycount];
            int[] allowed = new int[memorycount];
            for (int i = 0; i < memorycount; i++)
            {
                memories[i] = Form1.memories[38 + i];
                allowed[i] = i + 1;
            }
            Array.Resize(ref allowed, allowed.Length - 1);
            var memory_list1 = Util.getCBList(new string[] { memories[0] }, null);
            var memory_list = Util.getOffsetCBList(memory_list1, memories, 0, allowed);            

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
                CB_CTQual.Items.Add(Form1.memories[2 + i]);
                CB_OTQual.Items.Add(Form1.memories[2 + i]);
            }

            // Feeling Chooser
            CB_CTFeel.Items.Clear();
            CB_OTFeel.Items.Clear();
            for (int i = 0; i < 24; i++)
            {
                CB_CTFeel.Items.Add(Form1.memories[10 + i]);
                CB_OTFeel.Items.Add(Form1.memories[10 + i]);
            }
        }
        private void getMemoryArguments(string ARG, ComboBox sender)
        {
            var argvals = Util.getCBList(new string[] { "" }, null);
            #region General Locations
            int[] allowed = { };

            var genloc = Util.getCBList(Form1.genloc, null);
            #endregion
            #region Items
            int[] items_allowed = new int[697] 
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
            var item_list = Util.getCBList(Form1.itemlist, items_allowed);
            #endregion
            var species_list = new BindingSource(m_parent.CB_Species.DataSource, null);

            // Met Locations for Pokecenters healing
            var locationXY = Util.getCBList(Form1.metXY_00000, Legal.Met_XY_0);            
            var move_list = new BindingSource(m_parent.CB_Move1.DataSource, null);
            
            string vs = "";
            bool enabled = true;
            if (ARG == "NONE")
            {
                enabled = false;
                vs = "";
            }
            else if (ARG == "PKM")
            {
                argvals = Util.getCBList(Form1.specieslist, null);
                vs = vartypes[0];
            }
            else if (ARG == "GENLOC")
            {
                argvals = Util.getCBList(Form1.genloc, null);
                vs = vartypes[1];
            }
            else if (ARG == "ITEM")
            {
                argvals = item_list;
                vs = vartypes[2];
            }
            else if (ARG == "MOVE")
            {
                argvals = Util.getCBList(Form1.movelist, null);
                vs = vartypes[3];
            }
            else if (ARG == "LOCATION")
            {
                argvals = Util.getCBList(Form1.metXY_00000, allowed);
                vs = vartypes[4];
            }

            if (sender == CB_CTMemory)
            {
                CB_CTVar.DataSource = argvals;
                CB_CTVar.DisplayMember = "Text";
                CB_CTVar.ValueMember = "Value";
                LCTV.Text = vs;
                LCTV.Visible = CB_CTVar.Visible = CB_CTVar.Enabled = enabled;
            }
            else
            {
                CB_OTVar.DataSource = argvals;
                CB_OTVar.DisplayMember = "Text";
                CB_OTVar.ValueMember = "Value";
                LOTV.Text = vs;
                LOTV.Visible = CB_OTVar.Visible = CB_OTVar.Enabled = enabled;
            }
        }
        private string getMemoryString(ComboBox m, ComboBox arg, ComboBox q, ComboBox f, string tr)
        {
            string result = "";
            string nn = m_parent.TB_Nickname.Text;
            string a = arg.Text;
            int mem = Util.getIndex(m);

            bool enabled = false;
            if (mem == 0)
                result = Form1.memories[38];
            else
            {
                result = String.Format(Form1.memories[mem + 38], nn, tr, a, f.Text, q.Text);
                enabled = true;
            }

            // Show labels if the memory allows for them.
            if (q == CB_CTQual)
                L_CT_Quality.Visible = L_CT_Feeling.Visible = enabled;
            else
                L_OT_Quality.Visible = L_OT_Feeling.Visible = enabled;

            // Show Quality and Feeling.
            q.Visible = q.Enabled = f.Visible = f.Enabled = enabled;

            return result;
        }

        private void changeMemory(object sender, EventArgs e)
        {
            ComboBox m = (ComboBox)sender;
            if (m == CB_CTMemory || m == CB_OTMemory)
            {
                int memory = Util.getIndex(m);
                switch (memory) // Memory Case Switchtable
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
                    default: getMemoryArguments("NONE", m); break;
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
            ComboBox[] mta = new ComboBox[]
            {
                Region0, Region1, Region2, Region3, Region4,
            };
            int index = Array.IndexOf(cba, sender as ComboBox);
            if (Util.getIndex(sender as ComboBox) > 0)
                m_parent.setCountrySubRegion(mta[index], "sr_" + Util.getIndex(sender as ComboBox).ToString("000"));
        }

        private void update255_MTB(object sender, EventArgs e)
        {
            MaskedTextBox mtb = sender as MaskedTextBox;
            try
            {
                int val = Util.ToInt32(mtb.Text);
                if (val > 255) mtb.Text = "255";
            }
            catch { mtb.Text = "0"; }
        }

        private void clickResetLocation(object sender, EventArgs e)
        {
            Label[] senderarr = new Label[] 
            {
                L_Geo0, L_Geo1, L_Geo2, L_Geo3, L_Geo4,
            };
            ComboBox[] cba = new ComboBox[]
            {
                CB_Country0, CB_Country1, CB_Country2, CB_Country3, CB_Country4,
            };
            ComboBox[] mta = new ComboBox[]
            {
                Region0, Region1, Region2, Region3, Region4,
            };
            int index = Array.IndexOf(senderarr, sender as Label);
            cba[index].SelectedValue = 0;
            // doesn't work
            // mta[index].SelectedValue = 0;

            mta[index].DataSource = new[] { new { Text = "", Value = 0 } };
            mta[index].DisplayMember = "Text";
            mta[index].ValueMember = "Value";
        }
    }
}
