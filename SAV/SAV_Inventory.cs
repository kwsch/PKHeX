using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PKHeX
{
    public partial class SAV_Inventory : Form
    {
        public SAV_Inventory(Form1 frm1)
        {
            InitializeComponent();
            m_parent = frm1;
            Array.Copy(m_parent.savefile, sav, 0x100000);
            savindex = m_parent.savindex;
            shiftval = savindex * 0x7F000;

            if (m_parent.savegame_oras)
            {
                bagoffsets = new int[] 
                {
                    0x05800,
                    0x05E40,
                    0x05FC0,
                    0x06170,
                    0x06270, 
                };
            }

            getListItems();
            getListKeyItems();
            getListTMHM();
            getListMedicine();
            getListBerries();

            B_DisplayItems.ForeColor = Color.Red;

            popItems();

            B_DisplayItems.Text = Form1.itempouch[0];
            B_DisplayMedicine.Text = Form1.itempouch[1];
            B_DisplayTMHM.Text = Form1.itempouch[2];
            B_DisplayBerries.Text = Form1.itempouch[3];
            B_DisplayKeyItems.Text = Form1.itempouch[4];
        }
        Form1 m_parent;
        public byte[] sav = new Byte[0x100000];
        public int savindex; int shiftval;
        public bool editing = false;

        public string[] item_val;
        public string[] keyitem_val;
        public string[] tmhm_val;
        public string[] medicine_val;
        public string[] berries_val;

        public int[] bagoffsets = new int[] {
                                    0x05800,
                                    0x05E40,
                                    0x05FC0,
                                    0x06168,
                                    0x06268,
                                };

        // Initialize String Tables
        private void getListItems()
        {
            int[] itemlist = new int[] {
                000,001,002,003,004,005,006,007,008,009,010,011,012,013,014,015,016,055,056,
                057,058,059,060,061,062,063,064,065,066,067,068,069,070,071,072,073,074,075,
                076,077,078,079,080,081,082,083,084,085,086,087,088,089,090,091,092,093,094,
                099,100,101,102,103,104,105,106,107,108,109,110,112,116,117,118,119,135,136,
                213,214,215,217,218,219,220,221,222,223,224,225,226,227,228,229,230,231,232,
                233,234,235,236,237,238,239,240,241,242,243,244,245,246,247,248,249,250,251,
                252,253,254,255,256,257,258,259,260,261,262,263,264,265,266,267,268,269,270,
                271,272,273,274,275,276,277,278,279,280,281,282,283,284,285,286,287,288,289,
                290,291,292,293,294,295,296,297,298,299,300,301,302,303,304,305,306,307,308,
                309,310,311,312,313,314,315,316,317,318,319,320,321,322,323,324,325,326,327,
                492,493,494,495,496,497,498,499,500,537,538,539,540,541,542,543,544,545,546,
                547,548,549,550,551,552,553,554,555,556,557,558,559,560,561,562,563,564,571,
                572,573,576,577,580,581,582,583,584,585,586,587,588,589,590,639,640,644,646,
                647,648,649,650,652,653,654,655,656,657,658,659,660,661,662,663,664,665,666,
                667,668,669,670,671,672,673,674,675,676,677,678,679,680,681,682,683,684,685,
                699,704,710,711,715,
            };
            if (m_parent.savegame_oras)
            {
                itemlist = new int[] {
                000,001,002,003,004,005,006,007,008,009,010,011,012,013,014,015,016,055,056,
                057,058,059,060,061,062,063,064,065,066,067,068,069,070,071,072,073,074,075,
                076,077,078,079,080,081,082,083,084,085,086,087,088,089,090,091,092,093,094,
                099,100,101,102,103,104,105,106,107,108,109,110,112,116,117,118,119,135,136,
                213,214,215,217,218,219,220,221,222,223,224,225,226,227,228,229,230,231,232,
                233,234,235,236,237,238,239,240,241,242,243,244,245,246,247,248,249,250,251,
                252,253,254,255,256,257,258,259,260,261,262,263,264,265,266,267,268,269,270,
                271,272,273,274,275,276,277,278,279,280,281,282,283,284,285,286,287,288,289,
                290,291,292,293,294,295,296,297,298,299,300,301,302,303,304,305,306,307,308,
                309,310,311,312,313,314,315,316,317,318,319,320,321,322,323,324,325,326,327,
                492,493,494,495,496,497,498,499,500,537,538,539,540,541,542,543,544,545,546,
                547,548,549,550,551,552,553,554,555,556,557,558,559,560,561,562,563,564,571,
                572,573,576,577,580,581,582,583,584,585,586,587,588,589,590,639,640,644,646,
                647,648,649,650,652,653,654,655,656,657,658,659,660,661,662,663,664,665,666,
                667,668,669,670,671,672,673,674,675,676,677,678,679,680,681,682,683,684,685,
                699,704,710,711,715,

                // ORAS
                534,535,
                752,753,754,755,756,757,758,759,760,761,762,763,764,767,768,769,770,
                };
            }
            item_val = new string[itemlist.Length];
            for (int i = 0; i < itemlist.Length; i++)
                item_val[i] = Form1.itemlist[itemlist[i]];
            Array.Sort(item_val);
        }
        private void getListKeyItems()
        {
            int[] itemlist = new int[] {
                000,216,431,442,445,446,447,450,465,466,471,628,
                629,631,632,638,641,642,643,689,695,696,697,698,
                700,701,702,703,705,706,707,712,713,714,
                
                // Illegal
                716,717, // For the cheaters who want useless items...
            };
            if (m_parent.savegame_oras)
            {
                itemlist = new int[] {
                000,216,        445,446,447,    465,466,471,628,
                629,631,632,638,                        697,

                // Illegal
                716,717,745,746,747,748,749,750, // For the cheaters who want useless items...

                // ORAS
                457,474,503,

                718,719,
                720,721,722,723,724,725,726,727,728,729,
                730,731,732,733,734,735,736,    738,739,
                740,741,742,743,744,
                751,765,766,771,772,774,775,
            };
            }
            keyitem_val = new string[itemlist.Length];
            for (int i = 0; i < itemlist.Length; i++)
                keyitem_val[i] = Form1.itemlist[itemlist[i]];
            Array.Sort(keyitem_val);
        }
        private void getListTMHM()
        {
            int[] itemlist = new int[] {
                0,
                328,329,330,331,332,333,334,335,336,337,338,339,340,341,342,343,344,345,
                346,347,348,349,350,351,352,353,354,355,356,357,358,359,360,361,362,363,
                364,365,366,367,368,369,370,371,372,373,374,375,376,377,378,379,380,381,
                382,383,384,385,386,387,388,389,390,391,392,393,394,395,396,397,398,399,
                400,401,402,403,404,405,406,407,408,409,410,411,412,413,414,415,416,417,
                418,419,618,619,620,690,691,692,693,694,
                
                420,421,422,423,424,
            };
            if (m_parent.savegame_oras)
            {
                itemlist = new int[] {
                0,
                328,329,330,331,332,333,334,335,336,337,338,339,340,341,342,343,344,345,
                346,347,348,349,350,351,352,353,354,355,356,357,358,359,360,361,362,363,
                364,365,366,367,368,369,370,371,372,373,374,375,376,377,378,379,380,381,
                382,383,384,385,386,387,388,389,390,391,392,393,394,395,396,397,398,399,
                400,401,402,403,404,405,406,407,408,409,410,411,412,413,414,415,416,417,
                418,419,618,619,620,690,691,692,693,694,
                
                420,421,422,423,424,
                
                // ORAS
                425,737,
            };
            }
            tmhm_val = new string[itemlist.Length];
            for (int i = 0; i < itemlist.Length; i++)
                tmhm_val[i] = Form1.itemlist[itemlist[i]];
            // Array.Sort(tmhm_val); Already sorted, keep HMs last.
        }
        private void getListMedicine()
        {
            int[] itemlist = new int[] {
                000,017,018,019,020,021,022,023,024,025,026,027,028,029,030,031,032,033,
                034,035,036,037,038,039,040,041,042,043,044,045,046,047,048,049,050,051,
                052,053,054,134,504,565,566,567,568,569,570,571,591,645,708,709,
            };
            medicine_val = new string[itemlist.Length];
            for (int i = 0; i < itemlist.Length; i++)
                medicine_val[i] = Form1.itemlist[itemlist[i]];
            Array.Sort(medicine_val); 
        }
        private void getListBerries()
        {
            int[] itemlist = new int[] {
                0,149,150,151,152,153,154,155,156,157,158,159,160,161,162,
                163,164,165,166,167,168,169,170,171,172,173,174,175,176,177,
                178,179,180,181,182,183,184,185,186,187,188,189,190,191,192,
                193,194,195,196,197,198,199,200,201,202,203,204,205,206,207,
                208,209,210,211,212,686,687,688,
            };
            berries_val = new string[itemlist.Length];
            for (int i = 0; i < itemlist.Length; i++)
                berries_val[i] = Form1.itemlist[itemlist[i]];
            Array.Sort(berries_val); 
        }

        // Populate DataGrid
        private void popItems()
        {
            int offset = bagoffsets[0] + shiftval;
            populateList(item_val, offset, 308); // max 400
        }
        private void popKeyItems()
        {
            int offset = bagoffsets[1] + shiftval;
            populateList(keyitem_val, offset, 60); // max 96
        }
        private void popTMHM()
        {
            int offset = bagoffsets[2] + shiftval;
            populateList(tmhm_val, offset, 105 + 3*Convert.ToInt16(m_parent.savegame_oras)); // 106 total tho
        }
        private void popMedicine()
        {
            int offset = bagoffsets[3] + shiftval;
            populateList(medicine_val, offset, 60); // 64 total slots 
        }
        private void popBerries()
        {
            int offset = bagoffsets[4] + shiftval;
            populateList(berries_val, offset, 70); // 102 slots
        }

        private void populateList(string[] itemarr, int offset, int itemcount)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();

            DataGridViewColumn dgvIndex = new DataGridViewTextBoxColumn();
            {
                dgvIndex.HeaderText = "CNT";
                dgvIndex.DisplayIndex = 1;
                dgvIndex.Width = 45;
                dgvIndex.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            DataGridViewComboBoxColumn dgvItemVal = new DataGridViewComboBoxColumn();
            dgvItemVal.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            {
                for (int i = 0; i < itemarr.Length; i++)
                    dgvItemVal.Items.Add(itemarr[i]); // add only the Item Names

                dgvItemVal.DisplayIndex = 0;
                dgvItemVal.Width = 135;
                dgvItemVal.FlatStyle = FlatStyle.Flat;
            }
            dataGridView1.Columns.Add(dgvItemVal);
            dataGridView1.Columns.Add(dgvIndex);

            dataGridView1.Rows.Add(itemcount);

            string itemname = "";
            for (int i = 0; i < itemcount; i++)
            {
                int itemvalue = BitConverter.ToUInt16(sav, offset + i*4);
                try { itemname = Form1.itemlist[itemvalue]; }
                catch
                {
                    Util.Error("Unknown item detected.", "Item ID: " + itemvalue, "Item is after: " + itemname);
                    continue;
                }
                int itemarrayval = Array.IndexOf(itemarr,itemname);
                if (itemarrayval == -1)
                {
                    dataGridView1.Rows[i].Cells[0].Value = itemarr[0];
                    dataGridView1.Rows[i].Cells[1].Value = 0;
                    Util.Alert(itemname + " removed from item pouch.", "If you exit the Item Editor by saving changes, the item will no longer be in the pouch.");
                }
                else
                {
                    dataGridView1.Rows[i].Cells[0].Value = itemarr[itemarrayval];
                    dataGridView1.Rows[i].Cells[1].Value = BitConverter.ToUInt16(sav, offset + i * 4 + 2);
                }
            }
        }
        private void dropclick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                ComboBox comboBox = (ComboBox)dataGridView1.EditingControl;
                comboBox.DroppedDown = true;
            }
        }
        private void saveBag(object sender)
        {
            string[] itemstrarr = {};
            int offset = 0;
            if (B_DisplayItems.ForeColor == Color.Red)
            {
                itemstrarr = item_val;
                offset = bagoffsets[0] + shiftval;
            }
            else if (B_DisplayKeyItems.ForeColor == Color.Red)
            {
                itemstrarr = keyitem_val;
                offset = bagoffsets[1] + shiftval;
            }
            else if (B_DisplayTMHM.ForeColor == Color.Red)
            {
                itemstrarr = tmhm_val;
                offset = bagoffsets[2] + shiftval;
            }
            else if (B_DisplayMedicine.ForeColor == Color.Red)
            {
                itemstrarr = medicine_val;
                offset = bagoffsets[3] + shiftval;
            }
            else if (B_DisplayBerries.ForeColor == Color.Red)
            {
                itemstrarr = berries_val;
                offset = bagoffsets[4] + shiftval;
            }

            // Fetch Data
            int itemcount = dataGridView1.Rows.Count;
            int emptyslots = 0;
            for (int i = 0; i < itemcount; i++)
            {
                string item = dataGridView1.Rows[i].Cells[0].Value.ToString();
                int itemindex = Array.IndexOf(Form1.itemlist, item);
                int itemcnt = 0;
                try 
                { itemcnt = Convert.ToUInt16(dataGridView1.Rows[i].Cells[1].Value.ToString()); }
                catch { itemcnt = 0; }

                if (itemindex == 0) // Compression of Empty Slots
                {
                    emptyslots++;
                    continue;
                }
                else if (itemcnt == 0)
                    itemcnt++; // No 0 count of items
                else if (itemcnt > 995)
                    itemcnt = 995;

                // Write Data into Save File
                Array.Copy(BitConverter.GetBytes(itemindex), 0, sav, offset + 4 * (i - emptyslots), 2); // item #
                Array.Copy(BitConverter.GetBytes(itemcnt), 0, sav, offset + 4 * (i - emptyslots) + 2, 2); // count
            }

            // Delete Empty Trash
            for (int i = itemcount - emptyslots; i < itemcount; i++)
            {
                Array.Copy(BitConverter.GetBytes(0), 0, sav, offset + 4 * (i) + 0, 2); // item #
                Array.Copy(BitConverter.GetBytes(0), 0, sav, offset + 4 * (i) + 2, 2); // count
            }

            // Load New Button Color, after finished we'll load the new data.
            Button btn = sender as Button;
            B_DisplayItems.ForeColor = Color.Black;
            B_DisplayKeyItems.ForeColor = Color.Black;
            B_DisplayTMHM.ForeColor = Color.Black;
            B_DisplayMedicine.ForeColor = Color.Black;
            B_DisplayBerries.ForeColor = Color.Black;

            btn.ForeColor = Color.Red;
        }
        private void giveAll(string[] inarray, int count)
        {
            for (int i = 0; i < inarray.Length - 1; i++)
            {
                string itemname = inarray[i+1];
                int itemarrayval = Array.IndexOf(inarray, itemname);
                dataGridView1.Rows[i].Cells[0].Value = inarray[itemarrayval];
                dataGridView1.Rows[i].Cells[1].Value = count;
            }
        }
        private void B_DisplayItems_Click(object sender, EventArgs e)
        {
            // Store Current Items back to the save file
            saveBag(sender);
            popItems();
            if (ModifierKeys == Keys.Alt)
                giveAll(item_val,995);
        }
        private void B_DisplayKeyItems_Click(object sender, EventArgs e)
        {
            // Store Current Items back to the save file
            saveBag(sender);
            popKeyItems();
            if (ModifierKeys == Keys.Alt && m_parent.HaX)
                giveAll(keyitem_val,1);
        }
        private void B_DisplayTMHM_Click(object sender, EventArgs e)
        {
            // Store Current Items back to the save file
            saveBag(sender);
            popTMHM();

            if (ModifierKeys == Keys.Alt && m_parent.HaX)
                giveAll(tmhm_val,1);
        }
        private void B_DisplayMedicine_Click(object sender, EventArgs e)
        {
            // Store Current Items back to the save file
            saveBag(sender);
            popMedicine();
            if (ModifierKeys == Keys.Alt)
                giveAll(medicine_val,995);
        }
        private void B_DisplayBerries_Click(object sender, EventArgs e)
        {
            // Store Current Items back to the save file
            saveBag(sender);
            popBerries();
            if (ModifierKeys == Keys.Alt)
                giveAll(berries_val,995);
        }
        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void B_Save_Click(object sender, EventArgs e)
        {
            saveBag(sender);
            Array.Copy(sav, m_parent.savefile, 0x100000);
            m_parent.savedited = true;
            Close();
        }
    }
}
