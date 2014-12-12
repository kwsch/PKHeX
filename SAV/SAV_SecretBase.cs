using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace PKHeX
{
    public partial class SAV_SecretBase : Form
    {
        public SAV_SecretBase(Form1 frm1)
        {
            InitializeComponent();
            m_parent = frm1;
            Array.Copy(m_parent.savefile, sav, 0x100000);
            savindex = m_parent.savindex;

            specieslist = Form1.specieslist;
            movelist = Form1.movelist;
            itemlist = Form1.itemlist;
            abilitylist = Form1.abilitylist;
            natures = Form1.natures;

            setupComboBoxes();
            popFavorite();
            popFavorite();

            LB_Favorite.SelectedIndex = 0;
            MT_Flags.Text = BitConverter.ToUInt16(sav, 0x24800 + 0x140).ToString();
            // MT_Flags.Text = BitConverter.ToUInt32(sav, 0x2942C).ToString();
            B_SAV2FAV(null, null);
        }
        Form1 m_parent;
        public byte[] sav = new byte[0x100000];
        public byte[] wondercard_data = new byte[0x108];
        public bool editing = false;
        public int savindex; int sv = 0;
        private int fav_offset = 0x23A00;
        private bool loading = true;

        public static string[] specieslist = { };
        public static string[] movelist = { };
        public static string[] itemlist = { };
        public static string[] abilitylist = { };
        public static string[] natures = { };

        private void setupComboBoxes()
        {
            #region Balls
            {
                // Allowed Balls
                int[] ball_nums = { 7, 576, 13, 492, 497, 14, 495, 493, 496, 494, 11, 498, 8, 6, 12, 15, 9, 5, 499, 10, 1, 16 };
                int[] ball_vals = { 7, 25, 13, 17, 22, 14, 20, 18, 21, 19, 11, 23, 8, 6, 12, 15, 9, 5, 24, 10, 1, 16 };

                // Set up
                List<cbItem> ball_list = new List<cbItem>();

                for (int i = 4; i > 1; i--) // add 4,3,2
                {
                    // First 3 Balls are always first
                    cbItem ncbi = new cbItem();
                    ncbi.Text = itemlist[i];
                    ncbi.Value = i;
                    ball_list.Add(ncbi);
                }

                // Sort the Rest based on String Name
                string[] ballnames = new string[ball_nums.Length];
                for (int i = 0; i < ball_nums.Length; i++)
                    ballnames[i] = itemlist[ball_nums[i]];

                string[] sortedballs = new string[ball_nums.Length];
                Array.Copy(ballnames, sortedballs, ballnames.Length);
                Array.Sort(sortedballs);

                // Add the rest of the balls
                for (int i = 0; i < sortedballs.Length; i++)
                {
                    cbItem ncbi = new cbItem();
                    ncbi.Text = sortedballs[i];
                    ncbi.Value = ball_vals[Array.IndexOf(ballnames, sortedballs[i])];
                    ball_list.Add(ncbi);
                }
                CB_Ball.DisplayMember = "Text";
                CB_Ball.ValueMember = "Value";
                CB_Ball.DataSource = ball_list;
            }
            #endregion
            #region Held Items
            {
                // List of valid items to hold
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
            #region Species
            {
                List<cbItem> species_list = new List<cbItem>();
                // Sort the Rest based on String Name
                string[] sortedspecies = new string[specieslist.Length];
                Array.Copy(specieslist, sortedspecies, specieslist.Length);
                Array.Sort(sortedspecies);

                // Add the rest of the items
                for (int i = 0; i < sortedspecies.Length; i++)
                {
                    cbItem ncbi = new cbItem();
                    ncbi.Text = sortedspecies[i];
                    ncbi.Value = Array.IndexOf(specieslist, sortedspecies[i]);
                    species_list.Add(ncbi);
                }
                CB_Species.DisplayMember = "Text";
                CB_Species.ValueMember = "Value";
                CB_Species.DataSource = species_list;
            }
            #endregion
            #region Natures
            {
                List<cbItem> natures_list = new List<cbItem>();
                // Sort the Rest based on String Name
                string[] sortednatures = new string[natures.Length];
                Array.Copy(natures, sortednatures, natures.Length);
                Array.Sort(sortednatures);

                // Add the rest of the items
                for (int i = 0; i < sortednatures.Length; i++)
                {
                    cbItem ncbi = new cbItem();
                    ncbi.Text = sortednatures[i];
                    ncbi.Value = Array.IndexOf(natures, sortednatures[i]);
                    natures_list.Add(ncbi);
                }
                CB_Nature.DisplayMember = "Text";
                CB_Nature.ValueMember = "Value";
                CB_Nature.DataSource = natures_list;
            }
            #endregion
            #region Moves
            {
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
        }

        // Repopulation Functions
        private void popFavorite()
        {
            LB_Favorite.Items.Clear();

            int playeroff = fav_offset + 0x5400 + 0x326;
            int favoff = fav_offset + 0x5400 + 0x63A;
            string OT = Util.TrimFromZero(Encoding.Unicode.GetString(sav, sv + playeroff + 0x218, 0x1A));
            LB_Favorite.Items.Add("* " + OT);
            for (int i = 0; i < 30; i++)
            {
                string BaseTrainer = Util.TrimFromZero(Encoding.Unicode.GetString(sav, sv + favoff + i * 0x3E0 + 0x218, 0x1A));
                if (BaseTrainer.Length < 1 || BaseTrainer[0] == '\0')
                    BaseTrainer = "Empty";
                LB_Favorite.Items.Add(i.ToString() + " " + BaseTrainer);
            }
        }
        private void B_SAV2FAV(object sender, EventArgs e)
        {
            loading = true;
            int index = LB_Favorite.SelectedIndex;
            if (index < 0) return;
            int offset = fav_offset + 0x5400 + 0x25A;

            // Base Offset Changing
            if (index == 0)
                offset = fav_offset + 0x5400 + 0x326;
            else offset += 0x3E0 * index;

            string TrainerName = Util.TrimFromZero(Encoding.Unicode.GetString(sav, sv + offset + 0x218, 0x1A));
            TB_FOT.Text = TrainerName;

            TB_FT1.Text = Util.TrimFromZero(Encoding.Unicode.GetString(sav, sv + offset + 0x232 + 0x22 * 0, 0x22));
            TB_FT2.Text = Util.TrimFromZero(Encoding.Unicode.GetString(sav, sv + offset + 0x232 + 0x22 * 1, 0x22));

            string saying1 = Util.TrimFromZero(Encoding.Unicode.GetString(sav, sv + offset + 0x276 + 0x22 * 0, 0x22));
            string saying2 = Util.TrimFromZero(Encoding.Unicode.GetString(sav, sv + offset + 0x276 + 0x22 * 1, 0x22));
            string saying3 = Util.TrimFromZero(Encoding.Unicode.GetString(sav, sv + offset + 0x276 + 0x22 * 2, 0x22));
            string saying4 = Util.TrimFromZero(Encoding.Unicode.GetString(sav, sv + offset + 0x276 + 0x22 * 3, 0x22));

            int baseloc = BitConverter.ToInt16(sav, sv + offset);
            NUD_FBaseLocation.Value = baseloc;

            TB_FSay1.Text = saying1; TB_FSay2.Text = saying2; TB_FSay3.Text = saying3; TB_FSay4.Text = saying4;

            // Gather data for Object Array
            objdata = new byte[25, 12];
            for (int i = 0; i < 25; i++)
                for (int z = 0; z < 12; z++)
                    objdata[i, z] = sav[sv + offset + 2 + 12 * i + z];
            NUD_FObject.Value = 1; // Trigger Update
            changeObjectIndex(null, null);

            GB_PKM.Enabled = (index > 0);

            // Trainer Pokemon
            pkmdata = new byte[3, 0x34];
            if (index > 0) 
                for (int i = 0; i < 3; i++)
                    for (int z = 0; z < 0x34; z++)
                        pkmdata[i, z] = sav[sv + offset + 0x32E + 0x34 * i + z];

            NUD_FPKM.Value = 1;
            changeFavPKM(null, null); // Trigger Update

            loading = false;
        }
        private byte[,] objdata;
        private byte[,] pkmdata;
        private void B_FAV2SAV(object sender, EventArgs e)
        {
            // Write data back to save
            int index = LB_Favorite.SelectedIndex; // store for restoring
            if (!GB_PKM.Enabled && index > 0)
            { Util.Error("Sorry, no overwriting someone else's base with your own data."); return; }
            if (GB_PKM.Enabled && index == 0)
            { Util.Error("Sorry, no overwriting of your own base with someone else's."); return; }
            if (LB_Favorite.Items[index].ToString().Substring(LB_Favorite.Items[index].ToString().Length - 5, 5) == "Empty")
            { Util.Error("Sorry, no overwriting an empty base with someone else's."); return; }
            if (index < 0) return;
            int offset = fav_offset + 0x5400 + 0x25A;

            // Base Offset Changing
            if (index == 0)
                offset = fav_offset + 0x5400 + 0x326;
            else offset += 0x3E0 * index;

            string TrainerName = TB_FOT.Text;
            byte[] tr = Encoding.Unicode.GetBytes(TrainerName);
            Array.Resize(ref tr, 0x22); Array.Copy(tr, 0, sav, sv + offset + 0x218, 0x1A);

            string team1 = TB_FT1.Text;
            string team2 = TB_FT2.Text;
            byte[] t1 = Encoding.Unicode.GetBytes(team1);
            Array.Resize(ref t1, 0x22); Array.Copy(t1, 0, sav, sv + offset + 0x232 + 0x22 * 0, 0x22);
            byte[] t2 = Encoding.Unicode.GetBytes(team2);
            Array.Resize(ref t2, 0x22); Array.Copy(t2, 0, sav, sv + offset + 0x232 + 0x22 * 1, 0x22);

            string saying1 = TB_FSay1.Text;
            string saying2 = TB_FSay2.Text;
            string saying3 = TB_FSay3.Text;
            string saying4 = TB_FSay4.Text;
            byte[] s1 = Encoding.Unicode.GetBytes(saying1);
            Array.Resize(ref s1, 0x22); Array.Copy(s1, 0, sav, sv + offset + 0x276 + 0x22 * 0, 0x22);
            byte[] s2 = Encoding.Unicode.GetBytes(saying2);
            Array.Resize(ref s2, 0x22); Array.Copy(s2, 0, sav, sv + offset + 0x276 + 0x22 * 1, 0x22);
            byte[] s3 = Encoding.Unicode.GetBytes(saying3);
            Array.Resize(ref s3, 0x22); Array.Copy(s3, 0, sav, sv + offset + 0x276 + 0x22 * 2, 0x22);
            byte[] s4 = Encoding.Unicode.GetBytes(saying4);
            Array.Resize(ref s4, 0x22); Array.Copy(s4, 0, sav, sv + offset + 0x276 + 0x22 * 3, 0x22);

            int baseloc = (int)NUD_FBaseLocation.Value;
            if (baseloc < 3) baseloc = 0; // skip 1/2 baselocs as they are dummied out ingame.
            Array.Copy(BitConverter.GetBytes(baseloc), 0, sav, sv + offset, 2);

            TB_FOT.Text = TrainerName; TB_FSay1.Text = saying1; TB_FSay2.Text = saying2; TB_FSay3.Text = saying3; TB_FSay4.Text = saying4;

            // Copy back Objects
            for (int i = 0; i < 25; i++)
                for (int z = 0; z < 12; z++)
                    sav[sv + offset + 2 + 12 * i + z] = objdata[i, z];

            if (GB_PKM.Enabled) // Copy pkm data back in
                for (int i = 0; i < 3; i++)
                    for (int z = 0; z < 0x34; z++)
                        sav[sv + offset + 0x32E + 0x34 * i + z] = pkmdata[i, z];

            popFavorite();
            LB_Favorite.SelectedIndex = index;
        }

        // Button Specific
        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void B_Save_Click(object sender, EventArgs e)
        {
            uint flags = Util.ToUInt32(MT_Flags);
            Array.Copy(BitConverter.GetBytes(flags), 0, sav, 0x24800 + 0x140, 4);
            Array.Copy(sav, m_parent.savefile, 0x100000);
            m_parent.savedited = true;
            Close();
        }
        private void B_GiveDecor_Click(object sender, EventArgs e)
        {
            int offset = sv + 0x23A00 + 0x5400;
            for (int i = 0; i < 173; i++)
            {
                // int qty = BitConverter.ToUInt16(sav, offset + i * 4);
                // int has = BitConverter.ToUInt16(sav, offset + i * 4 + 2);

                sav[offset + i * 4] = (byte)25;
                sav[offset + i * 4 + 2] = 1;
            }
        }

        private void changeObjectIndex(object sender, EventArgs e)
        {
            int objindex = (int)(NUD_FObject.Value) - 1;
            byte[] objinfo = new byte[12];
            for (int i = 0; i < 12; i++)
                objinfo[i] = objdata[objindex, i];

            // Array with object data acquired. Fill data.
            int val = objinfo[0]; if (val == 0xFF) val = -1;
            byte x = objinfo[2];
            byte y = objinfo[4];
            byte rot = objinfo[6];
            byte unk1 = objinfo[7];
            ushort unk2 = BitConverter.ToUInt16(objinfo, 0x8);

            // Set values to display
            editing = true;

            NUD_FObjType.Value = val;
            NUD_FX.Value = x;
            NUD_FY.Value = y;
            NUD_FRot.Value = rot;

            editing = false;
        }
        private void changeObjectQuality(object sender, EventArgs e)
        {
            if (editing) return;

            int objindex = (int)(NUD_FObject.Value) - 1;

            byte val = (byte)(NUD_FObjType.Value);
            byte x = (byte)(NUD_FX.Value);
            byte y = (byte)(NUD_FY.Value);
            byte rot = (byte)(NUD_FRot.Value);

            objdata[objindex, 0] = val;
            objdata[objindex, 2] = x;
            objdata[objindex, 4] = y;
            objdata[objindex, 6] = rot;
        }

        private int currentpkm;
        private void changeFavPKM(object sender, EventArgs e)
        {
            int index = (int)(NUD_FPKM.Value);
            saveFavPKM(); // Save existing PKM
            currentpkm = index;
            loadFavPKM();
        }
        private void saveFavPKM()
        {
            if (loading || !GB_PKM.Enabled) return;
            int index = currentpkm;
            byte[] pkm = new byte[0x34];

            Array.Copy(BitConverter.GetBytes(Util.getHEXval(TB_EC)), 0, pkm, 0, 4);  // EC
            Array.Copy(BitConverter.GetBytes(Util.getIndex(CB_Species)), 0, pkm, 8, 2);
            Array.Copy(BitConverter.GetBytes(Util.getIndex(CB_HeldItem)), 0, pkm, 0xA, 2);
            pkm[0xC] = (byte)Array.IndexOf(abilitylist, (CB_Ability.Text).Remove((CB_Ability.Text).Length - 4)); // Ability
            pkm[0xD] = (byte)(CB_Ability.SelectedIndex << 1);   // Number
            pkm[0x14] = (byte)Util.getIndex(CB_Nature);

            int fegform = 0;
            fegform += PKX.getGender(Label_Gender.Text) << 1;                         // Gender
            fegform += ((Util.getIndex(CB_Form)) * 8);
            pkm[0x15] = (byte)fegform;

            pkm[0x16] = (byte)(Convert.ToByte( TB_HPEV.Text) & 0x1F);
            pkm[0x17] = (byte)(Convert.ToByte(TB_ATKEV.Text) & 0x1F);
            pkm[0x18] = (byte)(Convert.ToByte(TB_DEFEV.Text) & 0x1F);
            pkm[0x19] = (byte)(Convert.ToByte(TB_SPAEV.Text) & 0x1F);
            pkm[0x1A] = (byte)(Convert.ToByte(TB_SPDEV.Text) & 0x1F);
            pkm[0x1B] = (byte)(Convert.ToByte(TB_SPEEV.Text) & 0x1F);

            Array.Copy(BitConverter.GetBytes(Util.getIndex(CB_Move1)), 0, pkm, 0x1C, 2);
            Array.Copy(BitConverter.GetBytes(Util.getIndex(CB_Move2)), 0, pkm, 0x1E, 2);
            Array.Copy(BitConverter.GetBytes(Util.getIndex(CB_Move3)), 0, pkm, 0x20, 2);
            Array.Copy(BitConverter.GetBytes(Util.getIndex(CB_Move4)), 0, pkm, 0x22, 2);
            
            pkm[0x24] = (byte)CB_PPu1.SelectedIndex;
            pkm[0x25] = (byte)CB_PPu2.SelectedIndex;
            pkm[0x26] = (byte)CB_PPu3.SelectedIndex;
            pkm[0x27] = (byte)CB_PPu4.SelectedIndex;

            pkm[0x28] = (byte)(Convert.ToByte(TB_HPIV.Text) & 0x1F);
            pkm[0x29] = (byte)(Convert.ToByte(TB_ATKIV.Text) & 0x1F);
            pkm[0x2A] = (byte)(Convert.ToByte(TB_DEFIV.Text) & 0x1F);
            pkm[0x2B] = (byte)(Convert.ToByte(TB_SPAIV.Text) & 0x1F);
            pkm[0x2C] = (byte)(Convert.ToByte(TB_SPDIV.Text) & 0x1F);
            pkm[0x2D] = (byte)(Convert.ToByte(TB_SPEIV.Text) & 0x1F);
            int shiny = (CHK_Shiny.Checked? 1 : 0) << 6;
            pkm[0x2D] |= (byte)shiny;

            pkm[0x2E] = Convert.ToByte(TB_Friendship.Text);
            pkm[0x2F] = (byte)Util.getIndex(CB_Ball);
            pkm[0x30] = Convert.ToByte(TB_Level.Text);

            for (int i = 0; i < 0x34; i++) // Copy data back to storage.
                pkmdata[index - 1, i] = pkm[i];
        }
        private void loadFavPKM()
        {
            int index = currentpkm - 1;
            byte[] fpkm = new byte[0x34];
            for (int i = 0; i < 0x34; i++)
                fpkm[i] = pkmdata[index, i];

            uint ec = BitConverter.ToUInt32(fpkm, 0);
            uint unk = BitConverter.ToUInt32(fpkm, 4);
            int spec = BitConverter.ToInt16(fpkm, 8);
            int item = BitConverter.ToInt16(fpkm, 0xA);
            int abil = fpkm[0xC];
            int abil_no = fpkm[0xD];
            MT_AbilNo.Text = abil_no.ToString();
            // 6 unknown bytes, contest?

            int nature = fpkm[0x14];
            byte genform = fpkm[0x15];
            genderflag = (genform >> 1) & 0x3;
            setGenderLabel();

            byte HP_EV = fpkm[0x16];
            byte AT_EV = fpkm[0x17];
            byte DE_EV = fpkm[0x18];
            byte SA_EV = fpkm[0x19];
            byte SD_EV = fpkm[0x1A];
            byte SP_EV = fpkm[0x1B];

            int move1 = BitConverter.ToInt16(fpkm, 0x1C);
            int move2 = BitConverter.ToInt16(fpkm, 0x1E);
            int move3 = BitConverter.ToInt16(fpkm, 0x20);
            int move4 = BitConverter.ToInt16(fpkm, 0x22);

            byte ppu1 = fpkm[0x24];
            byte ppu2 = fpkm[0x25];
            byte ppu3 = fpkm[0x26];
            byte ppu4 = fpkm[0x27];

            byte HP_IV = fpkm[0x28];
            byte AT_IV = fpkm[0x29];
            byte DE_IV = fpkm[0x2A];
            byte SA_IV = fpkm[0x2B];
            byte SD_IV = fpkm[0x2C];
            byte SP_IV = fpkm[0x2D];

            bool isshiny = ((SP_IV & 0x40) > 0);
            SP_IV &= 0x1F;

            byte friendship = fpkm[0x2E];
            int ball = fpkm[0x2F];
            byte level = fpkm[0x30];
            
            // Put data into fields.
            TB_EC.Text = ec.ToString("X8");
            CB_Species.SelectedValue = spec;
            CB_HeldItem.SelectedValue = item;

            CB_Nature.SelectedValue = nature;
            CB_Ball.SelectedValue = ball;

            TB_HPIV.Text = HP_IV.ToString();
            TB_ATKIV.Text = AT_IV.ToString();
            TB_DEFIV.Text = DE_IV.ToString();
            TB_SPAIV.Text = SA_IV.ToString();
            TB_SPDIV.Text = SD_IV.ToString();
            TB_SPEIV.Text = SP_IV.ToString();

            TB_HPEV.Text = HP_EV.ToString();
            TB_ATKEV.Text = AT_EV.ToString();
            TB_DEFEV.Text = DE_EV.ToString();
            TB_SPAEV.Text = SA_EV.ToString();
            TB_SPDEV.Text = SD_EV.ToString();
            TB_SPEEV.Text = SP_EV.ToString();

            TB_Friendship.Text = friendship.ToString();
            TB_Level.Text = level.ToString();

            CB_Move1.SelectedValue = move1;
            CB_Move2.SelectedValue = move2;
            CB_Move3.SelectedValue = move3;
            CB_Move4.SelectedValue = move4;
            CB_PPu1.SelectedIndex = ppu1;
            CB_PPu2.SelectedIndex = ppu2;
            CB_PPu3.SelectedIndex = ppu3;
            CB_PPu4.SelectedIndex = ppu4;

            CHK_Shiny.Checked = isshiny;

            // Set Form
            m_parent.setForms(spec, CB_Form);
            int form = genform >> 3;
            CB_Form.SelectedIndex = form;

            // Set Ability
            m_parent.updateAbilityList(MT_AbilNo, spec, CB_Ability, CB_Form);
        }

        private void updateSpecies(object sender, EventArgs e)
        {
            int species = Util.getIndex(CB_Species);
            
            // Get Forms for Given Species
            m_parent.setForms(species, CB_Form);

            // Check for Gender Changes
            // Get Gender Threshold
            species = Util.getIndex(CB_Species);
            DataTable spectable = PKX.SpeciesTable();
            gt = (int)spectable.Rows[species][8];

            if (gt == 258)      // Genderless
                genderflag = 2;
            else if (gt == 257) // Female Only
                genderflag = 1;
            else if (gt == 256) // Male Only
                genderflag = 0;

            setGenderLabel();
            m_parent.updateAbilityList(MT_AbilNo, Util.getIndex(CB_Species), CB_Ability, CB_Form);
        }

        private void updateForm(object sender, EventArgs e)
        {
            m_parent.updateAbilityList(MT_AbilNo, Util.getIndex(CB_Species), CB_Ability, CB_Form);

            // If form has a single gender, account for it.
            if (PKX.getGender(CB_Form.Text) == 0) // ♂
                Label_Gender.Text = Form1.gendersymbols[0]; // ♂
            else if (PKX.getGender(CB_Form.Text) == 1) // ♀
                Label_Gender.Text = Form1.gendersymbols[1]; // ♀
        }
        private int species; private int gt; private int genderflag;
        private void Label_Gender_Click(object sender, EventArgs e)
        {
            // Get Gender Threshold
            species = Util.getIndex(CB_Species);
            DataTable spectable = PKX.SpeciesTable();
            gt = (int)spectable.Rows[species][8];

            if (gt > 255) // Single gender/genderless
                return;

            if (gt < 256) // If not a single gender(less) species:
            {
                if (PKX.getGender(Label_Gender.Text) == 0)
                    Label_Gender.Text = Form1.gendersymbols[1];
                else
                    Label_Gender.Text = Form1.gendersymbols[0];
            }
        }
        private void setGenderLabel()
        {
            if (genderflag == 0) // Gender = Male
                Label_Gender.Text = Form1.gendersymbols[0];
            
            else if (genderflag == 1) // Gender = Female
                Label_Gender.Text = Form1.gendersymbols[1];

            else Label_Gender.Text = Form1.gendersymbols[2];
        }
    }
}
