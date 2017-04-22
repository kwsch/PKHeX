using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SAV_Misc4 : Form
    {
        private readonly SAV4 SAV = (SAV4)Main.SAV.Clone();
        public SAV_Misc4()
        {
            InitializeComponent();

            switch (SAV.Version)
            {
                case GameVersion.DP:
                    readMain(isPoketch: true, isFly: false, isBP: false, isTC: false, isMap: false);
                    TC_Misc.Controls.Remove(TAB_BF);
                    break;
                case GameVersion.HGSS:
                    readMain(isPoketch: false, isFly: true, isBP: true, isTC: true, isMap: true);
                    readBF();
                    break;
                default:
                    readMain();
                    TC_Misc.Controls.Remove(TAB_BF);
                    break;
            }
        }

        private void B_Save_Click(object sender, EventArgs e)
        {
            saveMain();
            if (TC_Misc.Controls.Contains(TAB_BF))
                saveBF();

            SAV.Data.CopyTo(Main.SAV.Data, 0);
            Main.SAV.Edited = true;
            Close();
        }

        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private int ofsFly;
        private int ofsBP;
        private int[] FlyDestC;
        private void readMain(bool isPoketch = false, bool isFly = false, bool isBP = false, bool isTC = false, bool isMap = false)
        {
            int GBO = SAV.getGBO;
            if (isPoketch)
                readPoketch();
            else GB_Poketch.Visible = false;

            if (isFly)
            {
                string[] FlyDestA = null;
                switch (SAV.Version)
                {
                    case GameVersion.HGSS:
                        ofsFly = GBO + 0x11FA;
                        FlyDestA = new[] {
                            "NewBark Town", "Cherrygrove City", "Violet City", "Azalea Town",
                            "Goldenrod City", "Ecruteak City", "Olivine City", "Cianwood City",
                            "Mahogany Town", "Lake of Rage", "Blackthorn City",
                            "Safari Zone Gate", "Frontier Access", "Mt.Silver",
                            "Victory Road", "Indigo Plateau",
                            "Pallet Town", "Viridian City", "Pewter City", "Cerulean City", "Vermilion City",
                            "Lavender Town", "Celadon City", "Saffron City", "Fuchsia City", "Cinnabar Island"
                        };
                        FlyDestC = new[] {
                            11, 12, 13, 14,
                            16, 18, 17, 15,
                            19, 20, 21,
                            30, 27, 22,
                            33, 9,
                            0, 1, 2, 3, 5,
                            4, 6, 10, 7, 8
                        };
                        break;
                        // case GameVersion.DP: break;
                }
                uint val = BitConverter.ToUInt32(SAV.Data, ofsFly);
                CLB_FlyDest.Items.Clear();
                CLB_FlyDest.Items.AddRange(FlyDestA);
                for (int i = 0; i < CLB_FlyDest.Items.Count; i++)
                {
                    if (FlyDestC[i] < 32)
                        CLB_FlyDest.SetItemChecked(i, (val & (uint)1 << FlyDestC[i]) != 0);
                    else if (FlyDestC[i] < 40)
                        CLB_FlyDest.SetItemChecked(i, (SAV.Data[ofsFly + 4] & 1 << (FlyDestC[i] - 32)) != 0);
                }
            }
            else GB_FlyDest.Visible = false;

            if (isBP)
            {
                switch (SAV.Version)
                {
                    case GameVersion.HGSS: ofsBP = GBO + 0x5BB8; break;
                        // case GameVersion.DP: break;
                }
                uint val = BitConverter.ToUInt16(SAV.Data, ofsBP);
                if (val > 9999) val = 9999;
                NUD_BP.Value = val;
            }
            else L_BP.Visible = NUD_BP.Visible = false;

            if (isTC)
                CHK_UpgradeTC.Checked = (SAV.Data[GBO + 0x11F0] & 0x10) != 0;
            else CHK_UpgradeTC.Visible = false;

            if (isMap)
            {
                string[] items = new[] { "Map Johto", "Map Johto+", "Map Johto & Kanto" };
                int index = SAV.Data[SAV.getGBO + 0xBAE7] >> 3 & 3;
                if (index > 2) index = 2;
                CB_UpgradeMap.Items.AddRange(items);
                CB_UpgradeMap.SelectedIndex = index;
            }
            else CB_UpgradeMap.Visible = false;
        }
        private void saveMain()
        {
            int GBO = SAV.getGBO;
            if (GB_Poketch.Visible)
            {
                savePoketch();
            }
            if (GB_FlyDest.Visible)
            {
                uint val = BitConverter.ToUInt32(SAV.Data, ofsFly);
                for (int i = 0; i < CLB_FlyDest.Items.Count; i++)
                {
                    if (FlyDestC[i] < 32)
                    {
                        if (CLB_FlyDest.GetItemChecked(i))
                            val |= (uint)1 << FlyDestC[i];
                        else
                            val &= ~((uint)1 << FlyDestC[i]);
                    }
                    else if (FlyDestC[i] < 40)
                        SAV.Data[ofsFly + 4] = (byte)(SAV.Data[ofsFly + 4] & ~(1 << (FlyDestC[i] - 32)) | ((CLB_FlyDest.GetItemChecked(i) ? 1 : 0) << (FlyDestC[i] - 32)));
                }
                BitConverter.GetBytes(val).CopyTo(SAV.Data, ofsFly);

            }
            if (NUD_BP.Visible)
                BitConverter.GetBytes((ushort)NUD_BP.Value).CopyTo(SAV.Data, ofsBP);

            if (CHK_UpgradeTC.Visible)
                SAV.Data[GBO + 0x11F0] = (byte)(SAV.Data[GBO + 0x11F0] & 0xEF | (CHK_UpgradeTC.Checked ? 0x10 : 0));

            if (CB_UpgradeMap.Visible)
            {
                int val = CB_UpgradeMap.SelectedIndex;
                if (val >= 0)
                    SAV.Data[GBO + 0xBAE7] = (byte)(SAV.Data[GBO + 0xBAE7] & 0xE7 | val << 3);
            }

        }
        private void B_AllFlyDest_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < CLB_FlyDest.Items.Count; i++)
                CLB_FlyDest.SetItemChecked(i, true);
        }

        #region Poketch
        private byte[] DotArtistByte;
        private byte[] ColorTable;
        private readonly ToolTip tip1 = new ToolTip();
        private void readPoketch()
        {
            string[] PoketchTitle = new[] {
                "01 Digital Watch", "02 Calculator", "03 Memo Pad", "04 Pedometer", "05 Pokemon List",
                "06 Friendship Checker", "07 Dowsing Machine", "08 Berry Searcher", "09 Day Care Checker", "10 Pokemon History",
                "11 Counter", "12 Analog Watch", "13 Marking Map", "14 Link Searcher", "15 Coin Toss",
                "16 Move Tester", "17 Calendar", "18 Dot Artist", "19 Roulette", "20 Trainer Counter",
                "21 Kitchen Timer", "22 Color Changer", "23 Matchup Checker", "24 Stopwatch", "25 Alarm Clock"
            };
            CLB_Poketch.Items.Clear();
            CLB_Poketch.Items.AddRange(PoketchTitle);

            int ret = SAV.PoketchApps;
            for (int i = 0; i < CLB_Poketch.Items.Count; i++)
                CLB_Poketch.SetItemChecked(i, (ret & 1 << i) != 0);

            DotArtistByte = SAV.PoketchDotArtist;
            ColorTable = new byte[] { 248, 168, 88, 8 };
            setPictureBoxFromFlags(DotArtistByte);
            string tip = "Guide about D&D ImageFile Format";
            tip += Environment.NewLine + " width = 24px";
            tip += Environment.NewLine + " height = 20px";
            tip += Environment.NewLine + " used color count <= 4";
            tip += Environment.NewLine + " file size < 2058byte";
            tip1.SetToolTip(PB_DotArtist, tip);
            TAB_Main.AllowDrop = true;
        }
        private void savePoketch()
        {
            int ret = 0;
            for(int i = 0; i < CLB_Poketch.Items.Count; i++)
                if (CLB_Poketch.GetItemChecked(i)) ret |= 1 << i;

            SAV.PoketchApps = ret;
            SAV.PoketchDotArtist = DotArtistByte;
        }

        private void setPictureBoxFromFlags(byte[] inp)
        {
            if (inp.Length != 120) return;
            byte[] dupbyte = new byte[23040];
            for (int iy = 0; iy < 20; iy++)
                for (int ix = 0; ix < 24; ix++)
                {
                    var ib = ix + 24 * iy;
                    var ict = ColorTable[inp[ib >> 2] >> (ib % 4 << 1) & 3];
                    var iz = 12 * ix + 0x480 * iy;
                    for (int izy = 0; izy < 4; izy++)
                        for (int izx = 0; izx < 4; izx++)
                            for (int ic = 0; ic < 3; ic++)
                                dupbyte[ic + 3 * izx + 0x120 * izy + iz] = ict;
                }
            Bitmap dabmp = new Bitmap(96, 80);
            BitmapData dabdata = dabmp.LockBits(new Rectangle(0, 0, dabmp.Width, dabmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            System.Runtime.InteropServices.Marshal.Copy(dupbyte, 0, dabdata.Scan0, dupbyte.Length);
            dabmp.UnlockBits(dabdata);
            PB_DotArtist.Image = dabmp;
        }
        private void setFlagsFromFileName(string inpFileName)
        {
            if (new FileInfo(inpFileName).Length > 2058) return; // 24*20*4(ARGB)=1920
            Bitmap bmp;
            FileStream fs = new FileStream(inpFileName, FileMode.Open, FileAccess.Read);
            try
            {
                bmp = (Bitmap)Image.FromStream(fs);
            }
            catch
            {
                bmp = null;
            }
            fs.Close();
            if (bmp == null || bmp.Width != 24 || bmp.Height != 20) return;

            byte[] BrightMap = new byte[480];
            byte[] BrightCount = new byte[0x100];
            byte[] iBrightCount = new byte[0x100];
            for (int iy = 0; iy < 20; iy++)
                for (int ix = 0; ix < 24; ix++)
                {
                    var ig = (byte)(0xFF * bmp.GetPixel(ix, iy).GetBrightness());
                    BrightMap[ix + 24 * iy] = ig;
                    BrightCount[ig]++;
                }

            int ColorCount = BrightCount.Count(v => v > 0);
            if (ColorCount > 4 || ColorCount == 0) return;
            int errmin = int.MaxValue;
            byte[] LCT = new byte[4];
            byte[] mLCT = new byte[4];
            for (int i = 0; i < 4; i++)
                LCT[i] = (byte)(ColorCount < i + 1 ? 4 : ColorCount - i - 1);
            int ee = 0;
            while (++ee < 1000)
            {
                BrightCount.CopyTo(iBrightCount, 0);
                for (int i = 0, j = 0; i < 0x100; i++)
                    if (iBrightCount[i] > 0)
                        iBrightCount[i] = LCT[j++];
                var errtot = 0;
                for (int i = 0; i < 480; i++)
                    errtot += Math.Abs(BrightMap[i] - ColorTable[iBrightCount[BrightMap[i]]]);
                if (errmin > errtot)
                {
                    errmin = errtot;
                    LCT.CopyTo(mLCT, 0);
                }
                LCT = getNextLCT(LCT);
                if (LCT[0] >= 4) break;
            }
            for (int i = 0, j = 0; i < 0x100; i++)
                if (BrightCount[i] > 0)
                    BrightCount[i] = mLCT[j++];
            for (int i = 0; i < 480; i++)
                BrightMap[i] = BrightCount[BrightMap[i]];

            byte[] ndab = new byte[120];
            for (int i = 0; i < 480; i++)
                ndab[i >> 2] |= (byte)((BrightMap[i] & 3) << (i % 4 << 1));

            ndab.CopyTo(DotArtistByte, 0);
        }

        private byte[] getNextLCT(byte[] inp)
        {
            while (true)
            {
                if (++inp[0] < 4)
                    continue;

                inp[0] = 0;
                if (++inp[1] < 4)
                    continue;

                inp[1] = 0;
                if (++inp[2] < 4)
                    continue;

                inp[2] = 0;
                if (++inp[3] < 4)
                    continue;

                inp[0] = 4;
                return inp;
            }
        }

        private void setFlagsFromClickPoint(int inpX, int inpY)
        {
            if (inpX < 0) inpX = 0;
            else if (inpX > 95) inpX = 95;
            if (inpY < 0) inpY = 0;
            else if (inpY > 79) inpY = 79;
            int i = (inpX >> 2) + 24 * (inpY >> 2);
            byte[] ndab = new byte[120];
            DotArtistByte.CopyTo(ndab, 0);
            byte c = (byte)(ndab[i >> 2] >> (i % 4 << 1) & 3);
            if (++c >= 4) c = 0;
            ndab[i >> 2] &= (byte)~(3 << (i % 4 << 1));
            ndab[i >> 2] |= (byte)((c & 3) << (i % 4 << 1));
            ndab.CopyTo(DotArtistByte, 0);
        }

        private void B_GiveAll_Click(object sender, EventArgs e)
        {
            // foreach (CheckBox c in Apps) c.Checked = true;
            for (int i = 0; i < CLB_Poketch.Items.Count; i++)
                CLB_Poketch.SetItemChecked(i, true);
        }

        private void TAB_Poketch_DragEnter(object sender, DragEventArgs e)
        {
            if (TAB_Main.AllowDrop && e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void TAB_Poketch_DragDrop(object sender, DragEventArgs e)
        {
            if (!TAB_Main.AllowDrop) return;
            string[] t = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            if (t.Length != 1) return;
            setFlagsFromFileName(t[0]);
            setPictureBoxFromFlags(DotArtistByte);
        }
        private void PB_DotArtist_MouseClick(object sender, MouseEventArgs e)
        {
            setFlagsFromClickPoint(e.X, e.Y);
            setPictureBoxFromFlags(DotArtistByte);
        }
        #endregion

        #region BattleFrontier
        private int[] Prints;
        private int ofsPrints;
        private Color[] PrintColorA;
        private Button[] PrintButtonA;
        // private bool editingcont;
        // private bool editingval;
        private bool editing;
        private RadioButton[] StatRBA;
        private NumericUpDown[] StatNUDA;
        private Label[] StatLabelA;
        private int[][] BFF;
        private string[][] BFT;
        private int[][] BFV;
        private string[] BFN;
        private readonly ToolTip tip2 = new ToolTip();
        private NumericUpDown[] HallNUDA;
        private bool XBupdated = false;
        private void readBF()
        {
            BFF = new[] {
                // { BFV, BFT, addr, 1BFTlen, checkBit
                new[] { 0, 1, 0x5264, 0x04, 0x5BC1 },
                new[] { 1, 0, 0x5278, 0x10, 0x687C },
                new[] { 0, 0, 0x52A8, 0x18, 0x6880 },
                new[] { 2, 0, 0x52F0, 0x10, 0x6884 },
                new[] { 0, 0, 0x5320, 0x04, 0x6888 },
            };
            BFV = new[]
            {
                new[] { 2, 0 }, // Max, Current
                new[] { 2, 0, 3, 1 }, // Max, Current, Max(Trade), Current(Trade)
                new[] { 2, 0, 1, -1, 3 }, // Max, Current, Current(CP), (UsedCP), Max(CP)
            };
            BFT = new[] {
                new[] { "Singles", "Doubles", "Multi" },
                new[] { "Singles", "Doubles", "Multi(NPC)", "Multi(2P)", "WiFi" },
            };
            BFN = new[]
            {
                "Tower","Factory","Hall","Castle","Arcade"
            };
            StatNUDA = new[] { NUD_Stat0, NUD_Stat1, NUD_Stat2, NUD_Stat3 };
            StatLabelA = new[] { L_Stat0, L_Stat1, L_Stat2, L_Stat3 };
            StatRBA = new[] { RB_Stats3_01, RB_Stats3_02 };
            PrintColorA = new[] { Color.Transparent, Color.Silver, Color.Gold };
            PrintButtonA = new[] { BTN_PrintTower, BTN_PrintFactory, BTN_PrintHall, BTN_PrintCastle, BTN_PrintArcade };
            HallNUDA = new[] {
                NUD_HallType01, NUD_HallType02, NUD_HallType03, NUD_HallType04, NUD_HallType05, NUD_HallType06,
                NUD_HallType07, NUD_HallType08, NUD_HallType09, NUD_HallType10, NUD_HallType11, NUD_HallType12,
                NUD_HallType13, NUD_HallType14, NUD_HallType15, NUD_HallType16, NUD_HallType17
            };
            string[] TypeName = Util.getTypesList("en");
            int[] typenameIndex = new[] { 0, 9, 10, 12, 11, 14, 1, 3, 4, 2, 13, 6, 5, 7, 15, 16, 8 };
            for (int i = 0; i < HallNUDA.Length; i++)
                tip2.SetToolTip(HallNUDA[i], TypeName[typenameIndex[i]]);

            ofsPrints = SAV.getGBO + 0xE7E;
            Prints = new int[PrintButtonA.Length];
            for (int i = 0; i < Prints.Length; i++)
                Prints[i] = 1 + Math.Sign((BitConverter.ToUInt16(SAV.Data, ofsPrints + i * 2) >> 1) - 1);
            setPrints();

            editing = true;
            CB_Stats1.Items.Clear();
            foreach (string t in BFN)
                CB_Stats1.Items.Add(t);
            StatRBA[0].Checked = true;

            // Clear Listbox and ComboBox
            CB_Species.Items.Clear();

            // Fill List
            CB_Species.DisplayMember = "Text";
            CB_Species.ValueMember = "Value";
            CB_Species.DataSource = new BindingSource(GameInfo.SpeciesDataSource.Skip(1).Where(id => id.Value <= SAV.MaxSpeciesID).ToList(), null);

            editing = false;
            CB_Stats1.SelectedIndex = 0;

        }
        private void saveBF()
        {
            for(int i = 0; i < Prints.Length; i++)
            {
                if (Prints[i] == 1 + Math.Sign((BitConverter.ToUInt16(SAV.Data, ofsPrints + i * 2) >> 1) - 1)) continue;
                BitConverter.GetBytes(Prints[i] << 1).CopyTo(SAV.Data, ofsPrints + i * 2);
            }
            if (XBupdated) setXBChksum();
        }

        private void setPrints()
        {
            for (int i = 0; i < PrintButtonA.Length; i++)
                PrintButtonA[i].BackColor = PrintColorA[Prints[i]];
        }
        private void BTN_Print_Click(object sender, EventArgs e)
        {
            int index = Array.IndexOf(PrintButtonA, sender);
            if (index < 0) return;
            Prints[index] = (Prints[index] + 1) % 3;
            setPrints();
        }

        private void ChangeStat1(object sender, EventArgs e)
        {
            if (editing) return;
            int facility = CB_Stats1.SelectedIndex;
            if (facility < 0) return;

            editing = true;
            CB_Stats2.Items.Clear();
            CB_Stats2.Items.AddRange(BFT[BFF[facility][1]]);

            foreach (RadioButton r in StatRBA)
                r.Visible = facility == 1;

            for (int i = 0; i < StatLabelA.Length; i++)
                StatLabelA[i].Visible = StatLabelA[i].Enabled = StatNUDA[i].Visible = StatNUDA[i].Enabled = Array.IndexOf(BFV[BFF[facility][0]], i) >= 0;
            if (facility == 0)
            {
                StatLabelA[1].Visible = StatLabelA[1].Enabled = StatNUDA[1].Visible = StatNUDA[1].Enabled = true;
                StatLabelA[1].Text = "Continue";
                StatNUDA[1].Maximum = 65535;
            }
            else
            {
                if (StatNUDA[1].Value > 9999) StatNUDA[1].Value = 9999;
                StatNUDA[1].Maximum = 9999;
            }
            if (facility == 1) StatLabelA[1].Text = StatLabelA[3].Text = "Trade";
            if (facility == 3) StatLabelA[1].Text = StatLabelA[3].Text = "CP";
            GB_Hall.Visible = facility == 2;
            GB_Castle.Visible = facility == 3;

            editing = false;
            CB_Stats2.SelectedIndex = 0;
        }
        private void ChangeStat(object sender, EventArgs e)
        {
            if (editing) return;
            StatAddrControl(SetValToSav: -2, SetSavToVal: true);
            if (CB_Stats1.SelectedIndex == 2)
            {
                GB_Hall.Text = "Battle Hall (" + (string)CB_Stats2.SelectedItem + ")";
                editing = true;
                getHallStat();
                editing = false;
            }
            else if (CB_Stats1.SelectedIndex == 3)
            {
                GB_Castle.Text = "Battle Castle (" + (string)CB_Stats2.SelectedItem + ")";
                editing = true;
                getCastleStat();
                editing = false;
            }
        }
        private void StatAddrControl(int SetValToSav = -2, bool SetSavToVal = false)
        {
            int Facility = CB_Stats1.SelectedIndex;
            int BattleType = CB_Stats2.SelectedIndex;
            int RBi = StatRBA.First().Checked ? 0 : 1;
            int addrVal = SAV.getGBO + BFF[Facility][2] + BFF[Facility][3] * BattleType + (RBi << 3);
            int addrFlag = SAV.getGBO + BFF[Facility][4];
            byte maskFlag = (byte)(1 << BattleType + (RBi << 2));

            if (SetSavToVal)
            {
                editing = true;
                for (int i = 0; i < BFV[BFF[Facility][0]].Length; i++)
                {
                    if (BFV[BFF[Facility][0]][i] < 0) continue;
                    int vali = BitConverter.ToUInt16(SAV.Data, addrVal + (i << 1));
                    if (vali > 9999) vali = 9999;
                    StatNUDA[BFV[BFF[Facility][0]][i]].Value = vali;
                }
                CHK_Continue.Checked = (SAV.Data[addrFlag] & maskFlag) != 0;

                if (Facility == 0)
                    StatNUDA[1].Value = BitConverter.ToUInt16(SAV.Data, addrFlag + 1 + (BattleType << 1));

                editing = false;
                return;
            }
            else if (SetValToSav >= 0)
            {
                ushort val = (ushort)StatNUDA[SetValToSav].Value;

                if (Facility == 0 && SetValToSav == 1)
                    BitConverter.GetBytes(val).CopyTo(SAV.Data, addrFlag + 1 + (BattleType << 1));

                SetValToSav = Array.IndexOf(BFV[BFF[Facility][0]], SetValToSav);
                if (SetValToSav < 0) return;
                if (val > 9999) val = 9999;
                BitConverter.GetBytes(val).CopyTo(SAV.Data, addrVal + (SetValToSav << 1));
                return;
            }
            else if (SetValToSav == -1)
            {
                if (CHK_Continue.Checked)
                {
                    SAV.Data[addrFlag] |= maskFlag;
                    if (Facility == 3) SAV.Data[addrFlag + 1] |= 0x01; // not found what this flag means
                }
                else
                    SAV.Data[addrFlag] &= (byte)~maskFlag;
                return;
            }
        }
        private void ChangeStatVal(object sender, EventArgs e)
        {
            if (editing) return;
            int n = Array.IndexOf(StatNUDA, sender);
            if (n < 0) return;
            StatAddrControl(SetValToSav: n, SetSavToVal: false);

            if (CB_Stats1.SelectedIndex == 0 && Math.Floor(StatNUDA[0].Value / 7) != StatNUDA[1].Value)
            {
                if (n == 0)
                    StatNUDA[1].Value = Math.Floor(StatNUDA[0].Value / 7);
                else if (n == 1)
                {
                    if (StatNUDA[0].Maximum > StatNUDA[1].Value * 7)
                        StatNUDA[0].Value = StatNUDA[1].Value * 7;
                    else if (StatNUDA[0].Value < StatNUDA[0].Maximum)
                        StatNUDA[0].Value = StatNUDA[0].Maximum;
                }
            }
        }

        private void CHK_Continue_CheckedChanged(object sender, EventArgs e)
        {
            if (editing) return;
            StatAddrControl(SetValToSav: -1, SetSavToVal: false);
        }

        // memo BattleStage  !:write
        //
        // 0x0230C randC     randC   !!randD      randD     !randD         randD   !!randE
        // 0x02320 randB     randB   !!randC      randC     !randC         randC   !!randD
        // 0x0F618 1235      1235    !!1237       1237     !!1239          1239    !!1241
        // 0x26000 randC     randC     randC      randC      randC         randC   !!randE
        //               -->StartBS-->EndBS   -->MenuSave-->MenuSave   -->StartBS-->EndBS
        // 0x4230C randB   !!randC     randC    !!randD      randD        !randD     randD
        // 0x42320 randA   !!randB     randB    !!randC      randC        !randC     randC
        // 0x4F618 1234    !!1236      1236     !!1238       1238        !!1240      1240
        // 0x66000 randB     randB   !!randD      randD      randD         randD     randD

        private int species = -1;
        private void changeCBSpecies(object sender, EventArgs e)
        {
            species = (int)CB_Species.SelectedValue;
            if (editing) return;
            editing = true;
            getHallStat();
            editing = false;
        }
        private void getCastleStat()
        {
            int ofs = SAV.getGBO + BFF[3][2] + BFF[3][3] * CB_Stats2.SelectedIndex + 0x0A;
            NumericUpDown[] na = new[] { NUD_CastleRankRcv, NUD_CastleRankItem, NUD_CastleRankInfo };
            for (int i = 0, val; i < na.Length; i++)
            {
                val = BitConverter.ToInt16(SAV.Data, ofs + (i << 1));
                if (val > na[i].Maximum) val = (int)na[i].Maximum;
                if (val < na[i].Minimum) val = (int)na[i].Minimum;
                na[i].Value = val;
            }
        }
        private void NUD_CastleRank_ValueChanged(object sender, EventArgs e)
        {
            if (editing) return;
            NumericUpDown[] na = new[] { NUD_CastleRankRcv, NUD_CastleRankItem, NUD_CastleRankInfo };
            int i = Array.IndexOf(na, sender);
            if (i < 0) return;
            BitConverter.GetBytes((int)na[i].Value).CopyTo(SAV.Data, SAV.getGBO + BFF[3][2] + BFF[3][3] * CB_Stats2.SelectedIndex + 0x0A + (i << 1));
        }
        private void getHallStat()
        {
            int ofscur = SAV.getGBO + BFF[2][2] + BFF[2][3] * CB_Stats2.SelectedIndex;
            int curspe = BitConverter.ToInt16(SAV.Data, ofscur + 4);
            bool c = curspe == species;
            CHK_HallCurrent.Checked = c;
            if (curspe > 0 && curspe <= SAV.MaxSpeciesID)
                CHK_HallCurrent.Text = "Current: " + CB_Species.Items.Cast<ComboItem>().Where(x => x.Value == curspe).FirstOrDefault()?.Text ?? "(none)";
            else
                CHK_HallCurrent.Text = "Current: (none)";

            int s = 0;
            for (int i = 0, d; i < HallNUDA.Length; i++)
            {
                if (c)
                {
                    d = SAV.Data[ofscur + 6 + (i >> 1 << 1)] >> ((i & 1) << 2) & 0x0F;
                    if (d > 10) d = 10;
                }
                else d = 0;
                HallNUDA[i].Value = d;
                HallNUDA[i].Enabled = c;
                s += d;
            }
            L_SumHall.Text = s.ToString();

            int XBO = getXBO();
            NUD_HallStreaks.Visible = XBO > 0;
            if (XBO <= 0) return;
            ushort v = BitConverter.ToUInt16(SAV.Data, XBO + 4 + 0x3DE * CB_Stats2.SelectedIndex + (species << 1));
            if (v > 9999) v = 9999;
            NUD_HallStreaks.Value = v;
        }

        private void CHK_HallCurrent_CheckedChanged(object sender, EventArgs e)
        {
            if (editing) return;
            BitConverter.GetBytes((ushort)(CHK_HallCurrent.Checked ? species : 0)).CopyTo(SAV.Data, SAV.getGBO + BFF[2][2] + BFF[2][3] * CB_Stats2.SelectedIndex + 4);
            editing = true;
            getHallStat();
            editing = false;
        }

        private void NUD_HallType_ValueChanged(object sender, EventArgs e)
        {
            if (editing) return;
            int i = Array.IndexOf(HallNUDA, sender);
            if (i < 0) return;
            int ofs = SAV.getGBO + BFF[2][2] + BFF[2][3] * CB_Stats2.SelectedIndex + 6 + (i >> 1 << 1);
            SAV.Data[ofs] = (byte)(SAV.Data[ofs] & ~(0xF << ((i & 1) << 2)) | (int)HallNUDA[i].Value << ((i & 1) << 2));
            L_SumHall.Text = HallNUDA.Sum(x => x.Value).ToString();
        }

        private void NUD_HallStreaks_ValueChanged(object sender, EventArgs e)
        {
            if (editing) return;
            int XBO = getXBO();
            if (XBO < 0) return;
            BitConverter.GetBytes((ushort)NUD_HallStreaks.Value).CopyTo(SAV.Data, XBO + 4 + 0x3DE * CB_Stats2.SelectedIndex + (species << 1));
            XBupdated = true;
        }
        private int getXBO()
        {
            int[] XBC = new int[0x1E * 2];
            for (int i = 0; i < 0x1E; i++)
            {
                XBC[i] = BitConverter.ToInt32(SAV.Data, 0x22 + i << 12);
                XBC[i + 0x1E] = BitConverter.ToInt32(SAV.Data, 0x62 + i << 12);
            }
            for (int i = 0, h, j, a; i < 10; i++)
            {
                a = SAV.getGBO + 0x230C + (i << 2);
                h = BitConverter.ToInt32(SAV.Data, a);
                if (h == -1) continue;
                j = Array.IndexOf(XBC, h);
                if (j < 0) continue;
                if (j < 0x1E)
                    a = 0x22 + j;
                else
                    a = 0x44 + j;
                a <<= 12;
                if (BitConverter.ToUInt16(SAV.Data, a + 0xBA8) == 0xBA0)
                    return a;
            }
            return -1;
        }
        private void setXBChksum()
        {
            int XBO = getXBO();
            if (XBO < 0) return;
            BitConverter.GetBytes(SaveUtil.ccitt16(SAV.Data.Skip(XBO).Take(0xBAE).ToArray())).CopyTo(SAV.Data, XBO + 0xBAE);
        }
        #endregion

    }
}
