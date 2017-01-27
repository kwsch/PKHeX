using System;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class MemoryAmie : Form
    {
        private readonly string[] args = new string[5];
        private readonly string[] vartypes = new string[5];
        public MemoryAmie() // Keeping the form reference as a lot of control elements are required to operate.
        {
            InitializeComponent();
            cba = new[] { CB_Country0, CB_Country1, CB_Country2, CB_Country3, CB_Country4 };
            mta = new[] { CB_Region0, CB_Region1, CB_Region2, CB_Region3, CB_Region4, };
            CB_Country0.DisplayMember = CB_Country1.DisplayMember = CB_Country2.DisplayMember = CB_Country3.DisplayMember = CB_Country4.DisplayMember = "Text";
            CB_Country0.ValueMember = CB_Country1.ValueMember = CB_Country2.ValueMember = CB_Country3.ValueMember = CB_Country4.ValueMember = "Value";
            CB_Region0.DisplayMember = CB_Region1.DisplayMember = CB_Region2.DisplayMember = CB_Region3.DisplayMember = CB_Region4.DisplayMember = "Text";
            CB_Region0.ValueMember = CB_Region1.ValueMember = CB_Region2.ValueMember = CB_Region3.ValueMember = CB_Region4.ValueMember = "Value";
            WinFormsUtil.TranslateInterface(this, Main.curlanguage);
            string[] arguments = L_Arguments.Text.Split(new[] {" ; "}, StringSplitOptions.None);

            for (int i = 5; i < Math.Min(arguments.Length, vartypes.Length + 5); i++)
            {
                if (arguments[i] == null) continue;
                vartypes[i - 5] = arguments[i] + ":";
            }
            args[0] = arguments.Length > 0 ? arguments[0] ?? "Disabled" : "Disabled";
            args[1] = arguments.Length > 1 ? arguments[1] ?? "Never left" : "Never left";
            args[2] = arguments.Length > 2 ? arguments[2] ?? "OT" : "OT";
            args[3] = arguments.Length > 3 ? arguments[3] ?? "Past Gen": "Past Gen";
            args[4] = arguments.Length > 4 ? arguments[4] ?? "Memories with" : "Memories with";
            foreach (ComboBox comboBox in cba)
            {
                comboBox.DisplayMember = "Text";
                comboBox.ValueMember = "Value";
                Main.setCountrySubRegion(comboBox, "countries");
            }
            getLangStrings();
            loadFields();
        }

        private bool init;
        private readonly ComboBox[] cba;
        private readonly ComboBox[] mta;
        private readonly PKM pkm = Main.pkm;

        // Load/Save Actions
        private void loadFields()
        {
            // Load the region/country values.
            CB_Country0.SelectedValue = pkm.Geo1_Country;
            CB_Country1.SelectedValue = pkm.Geo2_Country;
            CB_Country2.SelectedValue = pkm.Geo3_Country;
            CB_Country3.SelectedValue = pkm.Geo4_Country;
            CB_Country4.SelectedValue = pkm.Geo5_Country;
            CB_Region0.SelectedValue = pkm.Geo1_Region;
            CB_Region1.SelectedValue = pkm.Geo2_Region;
            CB_Region2.SelectedValue = pkm.Geo3_Region;
            CB_Region3.SelectedValue = pkm.Geo4_Region;
            CB_Region4.SelectedValue = pkm.Geo5_Region;

            // Load the Fullness, and Enjoyment
            M_Fullness.Text = pkm.Fullness.ToString();
            M_Enjoyment.Text = pkm.Enjoyment.ToString();

            // Load the CT Memories
            M_CT_Friendship.Text = pkm.HT_Friendship.ToString();
            M_CT_Affection.Text = pkm.HT_Affection.ToString();
            CB_CTQual.SelectedIndex = Math.Max(0, pkm.HT_Intensity - 1);
            CB_CTMemory.SelectedValue = pkm.HT_Memory;
            CB_CTVar.SelectedValue = pkm.HT_TextVar;
            CB_CTFeel.SelectedIndex = pkm.HT_Feeling;

            // Load the OT Memories
            M_OT_Friendship.Text = pkm.OT_Friendship.ToString();
            M_OT_Affection.Text = pkm.OT_Affection.ToString();
            CB_OTQual.SelectedIndex = Math.Max(0, pkm.OT_Intensity - 1);
            CB_OTMemory.SelectedValue = pkm.OT_Memory;
            CB_OTVar.SelectedValue = pkm.OT_TextVar;
            CB_OTFeel.SelectedIndex = pkm.OT_Feeling;
            
            CB_Handler.Items.Clear();
            CB_Handler.Items.AddRange(new object[] {$"{pkm.OT_Name} ({args[2]})"}); // OTNAME : OT

            if (Util.TrimFromZero(pkm.HT_Name) != "")
                CB_Handler.Items.AddRange(new object[] { pkm.HT_Name });
            else
                pkm.CurrentHandler = 0;

            tabControl1.SelectedIndex = CB_Handler.SelectedIndex = pkm.CurrentHandler;

            GB_M_OT.Enabled = GB_M_CT.Enabled = GB_Residence.Enabled = 
            BTN_Save.Enabled = M_Fullness.Enabled = M_Enjoyment.Enabled = 
            L_Fullness.Enabled = L_Enjoyment.Enabled = !pkm.IsEgg;
            
            if (!pkm.IsEgg)
            {
                bool enable;
                if (pkm.GenNumber < 6)
                {
                    // Previous Generation Mon
                    GB_M_OT.Text = $"{args[3]} {pkm.OT_Name}: {args[2]}"; // Past Gen OT : OTNAME
                    GB_M_CT.Text = $"{args[4]} {pkm.HT_Name}"; // Memories with : HTNAME
                    enable = false;
                    // Reset to no memory
                    M_OT_Affection.Text = "0";
                    CB_OTQual.SelectedIndex = CB_OTFeel.SelectedIndex = 0;
                    CB_OTVar.SelectedValue = CB_OTMemory.SelectedValue = 0;
                }
                else
                {
                    enable = true;
                    GB_M_OT.Text = $"{args[4]} {pkm.OT_Name} ({args[2]})"; // Memories with : OTNAME
                    GB_M_CT.Text = $"{args[4]} {pkm.HT_Name}"; // Memories with : HTNAME
                    if (pkm.HT_Name == "")
                    {
                        CB_Country1.Enabled = CB_Country2.Enabled = CB_Country3.Enabled = CB_Country4.Enabled = 
                        CB_Region1.Enabled = CB_Region2.Enabled = CB_Region3.Enabled = CB_Region4.Enabled = 
                        GB_M_CT.Enabled = false;
                        GB_M_CT.Text = $"{args[1]} {args[2]} - {args[0]}"; // Never Left : OT : Disabled
                    }
                    else
                        GB_M_CT.Text = args[4] + " " + pkm.HT_Name;
                }
                RTB_OT.Visible = CB_OTQual.Enabled = CB_OTMemory.Enabled = CB_OTFeel.Enabled = CB_OTVar.Enabled = M_OT_Affection.Enabled = enable;
            }
            else
                GB_M_OT.Text = GB_M_CT.Text = $"N/A: {GameInfo.Strings.eggname}";

            init = true;

            // Manually load the Memory Parse
            RTB_CT.Text = getMemoryString(CB_CTMemory, CB_CTVar, CB_CTQual, CB_CTFeel, pkm.HT_Name);
            RTB_OT.Text = getMemoryString(CB_OTMemory, CB_OTVar, CB_OTQual, CB_OTFeel, pkm.OT_Name);
        }
        private void saveFields()
        {
            // Save Region & Country Data
            pkm.Geo1_Region = WinFormsUtil.getIndex(CB_Region0);
            pkm.Geo2_Region = WinFormsUtil.getIndex(CB_Region1);
            pkm.Geo3_Region = WinFormsUtil.getIndex(CB_Region2);
            pkm.Geo4_Region = WinFormsUtil.getIndex(CB_Region3);
            pkm.Geo5_Region = WinFormsUtil.getIndex(CB_Region4);
            pkm.Geo1_Country = WinFormsUtil.getIndex(CB_Country0);
            pkm.Geo2_Country = WinFormsUtil.getIndex(CB_Country1);
            pkm.Geo3_Country = WinFormsUtil.getIndex(CB_Country2);
            pkm.Geo4_Country = WinFormsUtil.getIndex(CB_Country3);
            pkm.Geo5_Country = WinFormsUtil.getIndex(CB_Country4);

            // Save 0-255 stats
            pkm.HT_Friendship = Util.ToInt32(M_CT_Friendship.Text);
            pkm.HT_Affection = Util.ToInt32(M_CT_Affection.Text);
            pkm.OT_Friendship = Util.ToInt32(M_OT_Friendship.Text);
            pkm.OT_Affection = Util.ToInt32(M_OT_Affection.Text);
            pkm.Fullness = (byte)Util.ToInt32(M_Fullness.Text);
            pkm.Enjoyment = (byte)Util.ToInt32(M_Enjoyment.Text);

            // Save Memories
            pkm.HT_Memory = WinFormsUtil.getIndex(CB_CTMemory);
            pkm.HT_TextVar = CB_CTVar.Enabled ? WinFormsUtil.getIndex(CB_CTVar) : 0;
            pkm.HT_Intensity = CB_CTFeel.Enabled ? CB_CTQual.SelectedIndex + 1 : 0;
            pkm.HT_Feeling = CB_CTFeel.Enabled ? CB_CTFeel.SelectedIndex : 0;

            pkm.OT_Memory = WinFormsUtil.getIndex(CB_OTMemory);
            pkm.OT_TextVar = CB_OTVar.Enabled ? WinFormsUtil.getIndex(CB_OTVar) : 0;
            pkm.OT_Intensity = CB_OTFeel.Enabled ? CB_OTQual.SelectedIndex + 1 : 0;
            pkm.OT_Feeling = CB_OTFeel.Enabled ? CB_OTFeel.SelectedIndex : 0;

            Main.pkm = pkm;
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


        private void getLangStrings()
        {
            // Memory Chooser
            int memorycount = GameInfo.Strings.memories.Length - 38;
            string[] memories = new string[memorycount];
            int[] allowed = new int[memorycount];
            for (int i = 0; i < memorycount; i++)
            {
                memories[i] = GameInfo.Strings.memories[38 + i];
                allowed[i] = i + 1;
            }
            Array.Resize(ref allowed, allowed.Length - 1);
            var memory_list1 = Util.getCBList(new[] { memories[0] }, null);
            var memory_list = Util.getOffsetCBList(memory_list1, memories, 0, allowed);

            CB_OTMemory.DisplayMember = "Text";
            CB_OTMemory.ValueMember = "Value";
            CB_OTMemory.DataSource = memory_list;

            CB_CTMemory.DisplayMember = "Text";
            CB_CTMemory.ValueMember = "Value";
            CB_CTMemory.DataSource = new BindingSource(memory_list, null);

            // Quality Chooser
            CB_CTQual.Items.Clear();
            CB_OTQual.Items.Clear();
            for (int i = 0; i < 7; i++)
            {
                CB_CTQual.Items.Add(GameInfo.Strings.memories[2 + i]);
                CB_OTQual.Items.Add(GameInfo.Strings.memories[2 + i]);
            }

            // Feeling Chooser
            CB_CTFeel.Items.Clear();
            CB_OTFeel.Items.Clear();
            for (int i = 0; i < 24; i++)
            {
                CB_CTFeel.Items.Add(GameInfo.Strings.memories[10 + i]);
                CB_OTFeel.Items.Add(GameInfo.Strings.memories[10 + i]);
            }
        }
        private void getMemoryArguments(string ARG, ComboBox sender)
        {
            var argvals = Util.getCBList(new[] { "" }, null);

            string vs = "";
            bool enabled = true;
            switch (ARG)
            {
                case "NONE":
                    enabled = false;
                    vs = "";
                    break;
                case "PKM":
                    argvals = Util.getCBList(GameInfo.Strings.specieslist.Take(Main.SAV.MaxSpeciesID+1).ToArray(), null);
                    vs = vartypes[0];
                    break;
                case "GENLOC":
                    argvals = Util.getCBList(GameInfo.Strings.genloc, null);
                    vs = vartypes[1];
                    break;
                case "ITEM":
                {
                    #region Items
                    int[] items_allowed =
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
                        700,701,702,703,704,705,706,707,708,709,710,711,712,713,714,715,716,717,
                        /* ORAS */
                        718,719,720,737,738,739,740,741,742,752,753,754,755,756,757,758,759,760,761,762,763,764,765,767,768, 769,770,775
                    };
                    var item_list = Util.getCBList(GameInfo.Strings.itemlist, items_allowed);
                    #endregion
                    argvals = item_list;
                    vs = vartypes[2];
                }
                    break;
                case "MOVE":
                    argvals = Util.getCBList(GameInfo.Strings.movelist.Take(622).ToArray(), null); // Hyperspace Fury
                    vs = vartypes[3];
                    break;
                case "LOCATION":
                    argvals = Util.getCBList(GameInfo.Strings.metXY_00000, Legal.Met_XY_0);
                    vs = vartypes[4];
                    break;
            }

            if (sender == CB_CTMemory)
            {
                CB_CTVar.DisplayMember = "Text";
                CB_CTVar.ValueMember = "Value";
                CB_CTVar.DataSource = argvals;
                LCTV.Text = vs;
                LCTV.Visible = CB_CTVar.Visible = CB_CTVar.Enabled = enabled;
            }
            else
            {
                CB_OTVar.DisplayMember = "Text";
                CB_OTVar.ValueMember = "Value";
                CB_OTVar.DataSource = argvals;
                LOTV.Text = vs;
                LOTV.Visible = CB_OTVar.Visible = CB_OTVar.Enabled = enabled;
            }
        }
        private string getMemoryString(ComboBox m, ComboBox arg, ComboBox q, ComboBox f, string tr)
        {
            string result;
            string nn = pkm.Nickname;
            string a = (ComboItem)arg.SelectedItem == null ? arg.Text ?? "ERROR" : ((ComboItem)arg.SelectedItem).Text;
            int mem = WinFormsUtil.getIndex(m);

            bool enabled = false;
            if (mem == 0)
                result = GameInfo.Strings.memories[38];
            else
            {
                result = string.Format(GameInfo.Strings.memories[mem + 38], nn, tr, a, f.Text, q.Text);
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
                int memory = WinFormsUtil.getIndex(m);
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
                }
            }

            if (!init) return;
            RTB_OT.Text = getMemoryString(CB_OTMemory, CB_OTVar, CB_OTQual, CB_OTFeel, pkm.OT_Name);
            RTB_CT.Text = getMemoryString(CB_CTMemory, CB_CTVar, CB_CTQual, CB_CTFeel, pkm.HT_Name);
        }
        private void changeCountryIndex(object sender, EventArgs e)
        {
            int index = Array.IndexOf(cba, sender);
            if (WinFormsUtil.getIndex(sender as ComboBox) > 0)
            {
                Main.setCountrySubRegion(mta[index], "sr_" + WinFormsUtil.getIndex(sender as ComboBox).ToString("000"));
                mta[index].Enabled = true;
            }
            else
            {
                mta[index].DataSource = new[] { new { Text = "", Value = 0 } };
                mta[index].Enabled = false;
                mta[index].SelectedValue = 0;
            }
        }
        private void changeCountryText(object sender, EventArgs e)
        {
            if (((ComboBox) sender).Text == "")
            {
                ((ComboBox) sender).SelectedValue = 0;
                changeCountryIndex(sender, e);
            }
        }

        private void update255_MTB(object sender, EventArgs e)
        {
            MaskedTextBox mtb = sender as MaskedTextBox;
            try
            {
                int val = Util.ToInt32(mtb?.Text);
                if (val > 255) mtb.Text = "255";
            }
            catch { mtb.Text = "0"; }
        }

        private void clickResetLocation(object sender, EventArgs e)
        {
            Label[] senderarr = { L_Geo0, L_Geo1, L_Geo2, L_Geo3, L_Geo4, };
            int index = Array.IndexOf(senderarr, sender);
            cba[index].SelectedValue = 0;

            mta[index].DisplayMember = "Text";
            mta[index].ValueMember = "Value";
            mta[index].DataSource = new[] { new { Text = "", Value = 0 } };
            mta[index].SelectedValue = 0;
        }

        private void B_ClearAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 5; i++)
                cba[i].SelectedValue = 0;
        }
    }
}