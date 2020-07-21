using System;
using System.Collections.Generic;
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
        private readonly SAV4 Origin;
        private readonly SAV4 SAV;

        public SAV_Misc4(SAV4 sav)
        {
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            int ofsFlag;
            SAV = (SAV4)(Origin = sav).Clone();

            switch (SAV.Version)
            {
                case GameVersion.D:
                case GameVersion.P:
                case GameVersion.DP:
                    ofsFlag = 0xFDC;
                    ofsBP = 0x65F8;
                    ofsUGFlagCount = 0x3A60;
                    L_CurrentMap.Visible = CB_UpgradeMap.Visible = false;
                    GB_Prints.Visible = GB_Prints.Enabled = GB_Hall.Visible = GB_Hall.Enabled = GB_Castle.Visible = GB_Castle.Enabled = false;
                    BFF = new[] { new[] { 0, 1, 0x5FCA, 0x04, 0x6601 }, };
                    break;
                case GameVersion.Pt:
                    ofsFlag = 0xFEC;
                    ofsBP = 0x7234;
                    ofsUGFlagCount = 0x3CE8;
                    L_CurrentMap.Visible = CB_UpgradeMap.Visible = false;
                    ofsPrints = 0xE4A;
                    BFF = new[] {
                        new[] { 0, 1, 0x68E0, 0x04, 0x723D },
                        new[] { 1, 0, 0x68F4, 0x10, 0x7EF8 },
                        new[] { 0, 0, 0x6924, 0x18, 0x7EFC },
                        new[] { 2, 0, 0x696C, 0x10, 0x7F00 },
                        new[] { 0, 0, 0x699C, 0x04, 0x7F04 },
                    };
                    ofsHallStat = 0x2820;
                    break;
                case GameVersion.HG:
                case GameVersion.SS:
                case GameVersion.HGSS:
                    ofsFlag = 0x10C4;
                    ofsBP = 0x5BB8;
                    L_UGFlags.Visible = NUD_UGFlags.Visible = false;
                    GB_Poketch.Visible = false;
                    ofsMap = 0xBAE7;
                    ofsPrints = 0xE7E;
                    BFF = new[] {
                        // { BFV, BFT, addr, 1BFTlen, checkBit
                        new[] { 0, 1, 0x5264, 0x04, 0x5BC1 },
                        new[] { 1, 0, 0x5278, 0x10, 0x687C },
                        new[] { 0, 0, 0x52A8, 0x18, 0x6880 },
                        new[] { 2, 0, 0x52F0, 0x10, 0x6884 },
                        new[] { 0, 0, 0x5320, 0x04, 0x6888 },
                    };
                    ofsHallStat = 0x230C;
                    break;
                default: return;
            }
            ofsFly = ofsFlag + 0x136;
            ReadMain();
            ReadBattleFrontier();
            if (SAV is SAV4Sinnoh s)
            {
                TC_Misc.Controls.Remove(TAB_Walker);
                poffinCase4Editor1.Initialize(s);
                TC_Misc.Controls.Remove(Tab_PokeGear);
            }
            else
            {
                pokeGear4Editor1.Initialize((SAV4HGSS)SAV);
                TC_Misc.Controls.Remove(Tab_Poffins);
            }
        }

        private void B_Save_Click(object sender, EventArgs e)
        {
            SaveMain();
            SaveBattleFrontier();
            if (SAV is SAV4HGSS)
                pokeGear4Editor1.Save();
            else if (SAV is SAV4Sinnoh)
                poffinCase4Editor1.Save();

            Origin.CopyChangesFrom(SAV);
            Close();
        }

        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private readonly int ofsFly;
        private readonly int ofsBP;
        private readonly int ofsMap = -1;
        private readonly int ofsUGFlagCount = -1;
        private int[] FlyDestC;

        private void ReadMain()
        {
            NUD_Coin.Value = SAV.Coin;
            NUD_Coin.Maximum = SAV.MaxCoins;
            int[] FlyDestD;
            IReadOnlyList<ComboItem> metLocationList;
            switch (SAV)
            {
                case SAV4Sinnoh _:
                    metLocationList = GameInfo.GetLocationList(GameVersion.Pt, 4, false);
                    FlyDestD = new[] { 1, 2, 6, 8, 3, 9, 10, 4, 12, 11, 5, 7, 14, 13, 54, 15, 81, 82, 83, 55, };
                    FlyDestC = new[] { 0, 1, 7, 9, 2, 10, 11, 3, 13, 12, 4, 8, 15, 14, 16, 68, 17, 5, 6, 67, };
                    break;
                case SAV4HGSS _:
                    metLocationList = GameInfo.GetLocationList(GameVersion.HG, 4, false);
                    FlyDestD = new[] { 126, 127, 128, 129, 131, 133, 132, 130, 134, 135, 136, 227, 229, 137, 221, 147, 138, 139, 140, 141, 143, 142, 144, 148, 145, 146, 225, };
                    FlyDestC = new[] { 11, 12, 13, 14, 16, 18, 17, 15, 19, 20, 21, 30, 27, 22, 33, 9, 0, 1, 2, 3, 5, 4, 6, 10, 7, 8, 35, };
                    break;
                default: return;
            }
            uint valFly = BitConverter.ToUInt32(SAV.General, ofsFly);
            CLB_FlyDest.Items.Clear();
            for (int i = 0; i < FlyDestD.Length; i++)
                CLB_FlyDest.Items.Add(metLocationList.First(v => v.Value == FlyDestD[i]).Text, FlyDestC[i] < 32 ? (valFly & 1u << FlyDestC[i]) != 0 : (SAV.General[ofsFly + (FlyDestC[i] >> 3)] & 1 << (FlyDestC[i] & 7)) != 0);
            uint valBP = BitConverter.ToUInt16(SAV.General, ofsBP);
            NUD_BP.Value = valBP > 9999 ? 9999 : valBP;

            if (SAV is SAV4Sinnoh)
                ReadPoketch();

            if (ofsUGFlagCount > 0)
            {
                uint fc = BitConverter.ToUInt32(SAV.General, ofsUGFlagCount) & 0xFFFFF;
                NUD_UGFlags.Value = fc > 999999 ? 999999 : fc;
            }
            if (ofsMap > 0)
            {
                string[] items = { "Map Johto", "Map Johto+", "Map Johto & Kanto" };
                int index = SAV.General[ofsMap] >> 3 & 3;
                if (index > 2) index = 2;
                CB_UpgradeMap.Items.AddRange(items);
                CB_UpgradeMap.SelectedIndex = index;
            }
        }

        private void SaveMain()
        {
            SAV.Coin = (uint)NUD_Coin.Value;
            uint valFly = BitConverter.ToUInt32(SAV.General, ofsFly);
            for (int i = 0; i < CLB_FlyDest.Items.Count; i++)
            {
                if (FlyDestC[i] < 32)
                {
                    if (CLB_FlyDest.GetItemChecked(i))
                        valFly |= (uint) 1 << FlyDestC[i];
                    else
                        valFly &= ~((uint) 1 << FlyDestC[i]);
                }
                else
                {
                    var o = ofsFly + (FlyDestC[i] >> 3);
                    SAV.General[o] = (byte)((SAV.General[o] & ~(1 << (FlyDestC[i] & 7))) | (CLB_FlyDest.GetItemChecked(i) ? 1 << (FlyDestC[i] & 7) : 0));
                }
            }
            BitConverter.GetBytes(valFly).CopyTo(SAV.General, ofsFly);
            BitConverter.GetBytes((ushort)NUD_BP.Value).CopyTo(SAV.General, ofsBP);

            if (SAV is SAV4Sinnoh)
                SavePoketch();

            if (ofsUGFlagCount > 0)
                BitConverter.GetBytes((BitConverter.ToUInt32(SAV.General, ofsUGFlagCount) & ~0xFFFFFu) | (uint)NUD_UGFlags.Value).CopyTo(SAV.General, ofsUGFlagCount);
            if (ofsMap > 0)
            {
                int valMap = CB_UpgradeMap.SelectedIndex;
                if (valMap >= 0)
                    SAV.General[ofsMap] = (byte)((SAV.General[ofsMap] & 0xE7) | valMap << 3);
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

        private void ReadPoketch()
        {
            string[] PoketchTitle = Enum.GetNames(typeof(PoketchApp));
            var s = (SAV4Sinnoh) SAV;

            CB_CurrentApp.Items.AddRange(PoketchTitle);
            CB_CurrentApp.SelectedIndex = s.CurrentPoketchApp;
            CLB_Poketch.Items.Clear();
            for (int i = 0; i < PoketchTitle.Length; i++)
            {
                var title = $"{i:00} - {PoketchTitle[i]}";
                var val = s.GetPoketchAppUnlocked((PoketchApp)i);
                CLB_Poketch.Items.Add(title, val);
            }

            DotArtistByte = s.PoketchDotArtistData;
            ColorTable = new byte[] { 248, 168, 88, 8 };
            SetPictureBoxFromFlags(DotArtistByte);
            string tip = "Guide about D&D ImageFile Format";
            tip += Environment.NewLine + " width = 24px";
            tip += Environment.NewLine + " height = 20px";
            tip += Environment.NewLine + " used color count <= 4";
            tip += Environment.NewLine + " file size < 2058byte";
            tip1.SetToolTip(PB_DotArtist, tip);
            TAB_Main.AllowDrop = true;
        }

        private void SavePoketch()
        {
            var s = (SAV4Sinnoh)SAV;
            s.CurrentPoketchApp = (sbyte)CB_CurrentApp.SelectedIndex;
            for (int i = 0; i < CLB_Poketch.Items.Count; i++)
            {
                var b = CLB_Poketch.GetItemChecked(i);
                s.SetPoketchAppUnlocked((PoketchApp)i, b);
            }
            s.PoketchDotArtistData = DotArtistByte;
        }

        private void SetPictureBoxFromFlags(byte[] inp)
        {
            if (inp.Length != 120) return;
            byte[] dupbyte = new byte[23040];
            for (int iy = 0; iy < 20; iy++)
            {
                for (int ix = 0; ix < 24; ix++)
                {
                    var ib = ix + (24 * iy);
                    var ict = ColorTable[inp[ib >> 2] >> (ib % 4 << 1) & 3];
                    var iz = (12 * ix) + (0x480 * iy);
                    for (int izy = 0; izy < 4; izy++)
                    {
                        for (int izx = 0; izx < 4; izx++)
                        {
                            for (int ic = 0; ic < 3; ic++)
                                dupbyte[ic + (3 * izx) + (0x120 * izy) + iz] = ict;
                        }
                    }
                }
            }

            Bitmap dabmp = new Bitmap(96, 80);
            BitmapData dabdata = dabmp.LockBits(new Rectangle(0, 0, dabmp.Width, dabmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            System.Runtime.InteropServices.Marshal.Copy(dupbyte, 0, dabdata.Scan0, dupbyte.Length);
            dabmp.UnlockBits(dabdata);
            PB_DotArtist.Image = dabmp;
        }

        private void SetFlagsFromFileName(string inpFileName)
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
            {
                for (int ix = 0; ix < 24; ix++)
                {
                    var ig = (byte)(0xFF * bmp.GetPixel(ix, iy).GetBrightness());
                    BrightMap[ix + (24 * iy)] = ig;
                    BrightCount[ig]++;
                }
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
                {
                    if (iBrightCount[i] > 0)
                        iBrightCount[i] = LCT[j++];
                }

                var errtot = 0;
                for (int i = 0; i < 480; i++)
                    errtot += Math.Abs(BrightMap[i] - ColorTable[iBrightCount[BrightMap[i]]]);
                if (errmin > errtot)
                {
                    errmin = errtot;
                    LCT.CopyTo(mLCT, 0);
                }
                LCT = GetNextLCT(LCT);
                if (LCT[0] >= 4) break;
            }
            for (int i = 0, j = 0; i < 0x100; i++)
            {
                if (BrightCount[i] > 0)
                    BrightCount[i] = mLCT[j++];
            }

            for (int i = 0; i < 480; i++)
                BrightMap[i] = BrightCount[BrightMap[i]];

            byte[] ndab = new byte[120];
            for (int i = 0; i < 480; i++)
                ndab[i >> 2] |= (byte)((BrightMap[i] & 3) << (i % 4 << 1));

            ndab.CopyTo(DotArtistByte, 0);
        }

        private static byte[] GetNextLCT(byte[] inp)
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

        private void SetFlagsFromClickPoint(int inpX, int inpY)
        {
            if (inpX < 0) inpX = 0;
            else if (inpX > 95) inpX = 95;
            if (inpY < 0) inpY = 0;
            else if (inpY > 79) inpY = 79;
            int i = (inpX >> 2) + (24 * (inpY >> 2));
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
            System.Media.SystemSounds.Asterisk.Play();
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
            SetFlagsFromFileName(t[0]);
            SetPictureBoxFromFlags(DotArtistByte);
        }

        private void PB_DotArtist_MouseClick(object sender, MouseEventArgs e)
        {
            SetFlagsFromClickPoint(e.X, e.Y);
            SetPictureBoxFromFlags(DotArtistByte);
        }
        #endregion

        #region BattleFrontier
        private int[] Prints;
        private readonly int ofsPrints = -1;
        private Color[] PrintColorA;
        private Button[] PrintButtonA;
        private bool editing;
        private RadioButton[] StatRBA;
        private NumericUpDown[] StatNUDA;
        private Label[] StatLabelA;
        private readonly int[][] BFF;
        private string[][] BFT;
        private int[][] BFV;
        private string[] BFN;
        private NumericUpDown[] HallNUDA;
        private bool HallStatUpdated;
        private int ofsHallStat = -1;

        private void ReadBattleFrontier()
        {
            BFV = new[] {
                new[] { 2, 0 }, // Max, Current
                new[] { 2, 0, 3, 1 }, // Max, Current, Max(Trade), Current(Trade)
                new[] { 2, 0, 1, -1, 3 }, // Max, Current, Current(CP), (UsedCP), Max(CP)
            };
            BFT = new[] {
                new[] { "Singles", "Doubles", "Multi" },
                new[] { "Singles", "Doubles", "Multi (Trainer)", "Multi (Friend)", "Wi-Fi" },
            };
            BFN = new[] { "Tower", "Factory", "Hall", "Castle", "Arcade" };
            if (SAV.DP) BFN = BFN.Take(1).ToArray();
            StatNUDA = new[] { NUD_Stat0, NUD_Stat1, NUD_Stat2, NUD_Stat3 };
            StatLabelA = new[] { L_Stat0, L_Stat1, L_Stat2, L_Stat3 };
            StatRBA = new[] { RB_Stats3_01, RB_Stats3_02 };

            if (ofsPrints > 0)
            {
                PrintColorA = new[] { Color.Transparent, Color.Silver, Color.Gold };
                PrintButtonA = new[] { BTN_PrintTower, BTN_PrintFactory, BTN_PrintHall, BTN_PrintCastle, BTN_PrintArcade };
                Prints = new int[PrintButtonA.Length];
                for (int i = 0; i < Prints.Length; i++)
                    Prints[i] = 1 + Math.Sign((BitConverter.ToUInt16(SAV.General, ofsPrints + (i << 1)) >> 1) - 1);
                SetPrints();

                HallNUDA = new[] {
                        NUD_HallType01, NUD_HallType02, NUD_HallType03, NUD_HallType04, NUD_HallType05, NUD_HallType06,
                        NUD_HallType07, NUD_HallType08, NUD_HallType09, NUD_HallType10, NUD_HallType11, NUD_HallType12,
                        NUD_HallType13, NUD_HallType14, NUD_HallType15, NUD_HallType16, NUD_HallType17
                    };
                string[] TypeName = GameInfo.Strings.types;
                int[] typenameIndex = new[] { 0, 9, 10, 12, 11, 14, 1, 3, 4, 2, 13, 6, 5, 7, 15, 16, 8 };
                for (int i = 0; i < HallNUDA.Length; i++)
                    tip2.SetToolTip(HallNUDA[i], TypeName[typenameIndex[i]]);
            }
            if (ofsHallStat > 0)
            {
                bool f = false;
                for (int i = 0; i < 2; i++, ofsHallStat += 0x14)
                {
                    var h = BitConverter.ToInt32(SAV.Data, ofsHallStat);
                    if (h == -1) continue;
                    for (int j = 0; j < 0x20; j++)
                    {
                        for (int k = 0, a = j + 0x20 << 12; k < 2; k++, a += 0x40000)
                        {
                            if (h != BitConverter.ToInt32(SAV.Data, a) || BitConverter.ToInt16(SAV.Data, a + 0xBA8) != 0xBA0)
                                continue;

                            f = true;
                            ofsHallStat = a;
                            break;
                        }
                        if (f) break;
                    }
                    if (f) break;
                }
                if (!f)
                {
                    ofsHallStat = -1;
                    NUD_HallStreaks.Visible = NUD_HallStreaks.Enabled = false;
                }
            }

            editing = true;
            CB_Stats1.Items.Clear();
            foreach (string t in BFN)
                CB_Stats1.Items.Add(t);
            StatRBA[0].Checked = true;

            // Clear Listbox and ComboBox
            CB_Species.Items.Clear();

            // Fill List
            CB_Species.InitializeBinding();
            CB_Species.DataSource = new BindingSource(GameInfo.SpeciesDataSource.Skip(1).Where(id => id.Value <= SAV.MaxSpeciesID).ToList(), null);

            editing = false;
            CB_Stats1.SelectedIndex = 0;
        }

        private void SaveBattleFrontier()
        {
            if (ofsPrints > 0)
            {
                for (int i = 0; i < Prints.Length; i++)
                {
                    if (Prints[i] == 1 + Math.Sign((BitConverter.ToUInt16(SAV.General, ofsPrints + (i << 1)) >> 1) - 1)) continue;
                    BitConverter.GetBytes(Prints[i] << 1).CopyTo(SAV.General, ofsPrints + (i << 1));
                }
            }

            if (HallStatUpdated)
                BitConverter.GetBytes(Checksums.CRC16_CCITT(SAV.Data, ofsHallStat, 0xBAE)).CopyTo(SAV.Data, ofsHallStat + 0xBAE);
        }

        private void SetPrints()
        {
            for (int i = 0; i < PrintButtonA.Length; i++)
                PrintButtonA[i].BackColor = PrintColorA[Prints[i]];
        }

        private void BTN_Print_Click(object sender, EventArgs e)
        {
            int index = Array.IndexOf(PrintButtonA, sender);
            if (index < 0)
                return;
            Prints[index] = (Prints[index] + 1) % 3;
            SetPrints();
        }

        private void ChangeStat1(object sender, EventArgs e)
        {
            if (editing)
                return;
            int facility = CB_Stats1.SelectedIndex;
            if (facility < 0)
                return;

            editing = true;
            CB_Stats2.Items.Clear();
            CB_Stats2.Items.AddRange(BFT[BFF[facility][1]]);

            StatRBA[0].Checked = true;
            foreach (RadioButton rb in StatRBA)
                rb.Visible = rb.Enabled = facility == 1;

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
            if (editing)
                return;
            if (sender is RadioButton radioButton && !radioButton.Checked) return;
            StatAddrControl(SetValToSav: -2, SetSavToVal: true);
            if (GB_Hall.Visible)
            {
                GB_Hall.Text = $"Battle Hall ({(string) CB_Stats2.SelectedItem})";
                editing = true;
                GetHallStat();
                editing = false;
            }
            else if (GB_Castle.Visible)
            {
                GB_Castle.Text = $"Battle Castle ({(string) CB_Stats2.SelectedItem})";
                editing = true;
                GetCastleStat();
                editing = false;
            }
        }

        private void StatAddrControl(int SetValToSav = -2, bool SetSavToVal = false)
        {
            int Facility = CB_Stats1.SelectedIndex;
            int BattleType = CB_Stats2.SelectedIndex;
            int RBi = StatRBA[1].Checked ? 1 : 0;
            int addrVal = BFF[Facility][2] + (BFF[Facility][3] * BattleType) + (RBi << 3);
            int addrFlag = BFF[Facility][4];
            byte maskFlag = (byte)(1 << BattleType + (RBi << 2));
            int TowerContinueCountOfs = SAV.DP ? 3 : 1;

            if (SetSavToVal)
            {
                editing = true;
                for (int i = 0; i < BFV[BFF[Facility][0]].Length; i++)
                {
                    if (BFV[BFF[Facility][0]][i] < 0) continue;
                    int vali = BitConverter.ToUInt16(SAV.General, addrVal + (i << 1));
                    StatNUDA[BFV[BFF[Facility][0]][i]].Value = vali > 9999 ? 9999 : vali;
                }
                CHK_Continue.Checked = (SAV.General[addrFlag] & maskFlag) != 0;

                if (Facility == 0) // tower continue count
                    StatNUDA[1].Value = BitConverter.ToUInt16(SAV.General, addrFlag + TowerContinueCountOfs + (BattleType << 1));

                editing = false;
                return;
            }
            if (SetValToSav >= 0)
            {
                ushort val = (ushort)StatNUDA[SetValToSav].Value;

                if (Facility == 0 && SetValToSav == 1) // tower continue count
                    BitConverter.GetBytes(val).CopyTo(SAV.General, addrFlag + TowerContinueCountOfs + (BattleType << 1));

                SetValToSav = Array.IndexOf(BFV[BFF[Facility][0]], SetValToSav);
                if (SetValToSav < 0)
                    return;
                BitConverter.GetBytes((ushort)(val > 9999 ? 9999 : val)).CopyTo(SAV.General, addrVal + (SetValToSav << 1));
                return;
            }
            if (SetValToSav == -1)
            {
                if (CHK_Continue.Checked)
                {
                    SAV.General[addrFlag] |= maskFlag;
                    if (Facility == 3) SAV.General[addrFlag + 1] |= 0x01; // not found what this flag means
                }
                else
                {
                    SAV.General[addrFlag] &= (byte)~maskFlag;
                }
            }
        }

        private void ChangeStatVal(object sender, EventArgs e)
        {
            if (editing)
                return;
            int n = Array.IndexOf(StatNUDA, sender);
            if (n < 0)
                return;
            StatAddrControl(SetValToSav: n, SetSavToVal: false);

            if (CB_Stats1.SelectedIndex == 0 && Math.Floor(StatNUDA[0].Value / 7) != StatNUDA[1].Value)
            {
                if (n == 0)
                {
                    StatNUDA[1].Value = Math.Floor(StatNUDA[0].Value / 7);
                }
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
            if (editing)
                return;
            StatAddrControl(SetValToSav: -1, SetSavToVal: false);
        }

        private int species = -1;

        private void ChangeSpecies(object sender, EventArgs e)
        {
            species = (int)CB_Species.SelectedValue;
            if (editing)
                return;
            editing = true;
            GetHallStat();
            editing = false;
        }

        private void GetCastleStat()
        {
            int ofs = BFF[3][2] + (BFF[3][3] * CB_Stats2.SelectedIndex) + 0x0A;
            NumericUpDown[] na = { NUD_CastleRankRcv, NUD_CastleRankItem, NUD_CastleRankInfo };
            for (int i = 0; i < na.Length; i++)
            {
                int val = BitConverter.ToInt16(SAV.General, ofs + (i << 1));
                na[i].Value = val > na[i].Maximum ? na[i].Maximum : val < na[i].Minimum ? na[i].Minimum : val;
            }
        }

        private void NUD_CastleRank_ValueChanged(object sender, EventArgs e)
        {
            if (editing)
                return;
            NumericUpDown[] na = new[] { NUD_CastleRankRcv, NUD_CastleRankItem, NUD_CastleRankInfo };
            int i = Array.IndexOf(na, sender);
            if (i < 0)
                return;
            BitConverter.GetBytes((int)na[i].Value).CopyTo(SAV.General, BFF[3][2] + (BFF[3][3] * CB_Stats2.SelectedIndex) + 0x0A + (i << 1));
        }

        private void GetHallStat()
        {
            int ofscur = BFF[2][2] + (BFF[2][3] * CB_Stats2.SelectedIndex);
            int curspe = BitConverter.ToInt16(SAV.General, ofscur + 4);
            bool c = curspe == species;
            CHK_HallCurrent.Checked = c;
            CHK_HallCurrent.Text = curspe > 0 && curspe <= SAV.MaxSpeciesID
                ? "Current: " + CB_Species.Items.OfType<ComboItem>().FirstOrDefault(x => x.Value == curspe).Text
                : "Current: (none)";

            int s = 0;
            for (int i = 0; i < HallNUDA.Length; i++)
            {
                var d = c ? Math.Min(10, SAV.General[ofscur + 6 + (i >> 1 << 1)] >> ((i & 1) << 2) & 0x0F) : 0;
                HallNUDA[i].Value = d;
                HallNUDA[i].Enabled = c;
                s += d;
            }
            L_SumHall.Text = s.ToString();

            if (ofsHallStat > 0)
            {
                ushort v = BitConverter.ToUInt16(SAV.Data, ofsHallStat + 4 + (0x3DE * CB_Stats2.SelectedIndex) + (species << 1));
                NUD_HallStreaks.Value = v > 9999 ? 9999 : v;
            }
        }

        private void CHK_HallCurrent_CheckedChanged(object sender, EventArgs e)
        {
            if (editing) return;
            BitConverter.GetBytes((ushort)(CHK_HallCurrent.Checked ? species : 0)).CopyTo(SAV.General, BFF[2][2] + (BFF[2][3] * CB_Stats2.SelectedIndex) + 4);
            editing = true;
            GetHallStat();
            editing = false;
        }

        private void NUD_HallType_ValueChanged(object sender, EventArgs e)
        {
            if (editing) return;
            int i = Array.IndexOf(HallNUDA, sender);
            if (i < 0) return;
            int ofs = BFF[2][2] + (BFF[2][3] * CB_Stats2.SelectedIndex) + 6 + (i >> 1 << 1);
            SAV.General[ofs] = (byte)((SAV.General[ofs] & ~(0xF << ((i & 1) << 2))) | (int)HallNUDA[i].Value << ((i & 1) << 2));
            L_SumHall.Text = HallNUDA.Sum(x => x.Value).ToString();
        }

        private void NUD_HallStreaks_ValueChanged(object sender, EventArgs e)
        {
            if (editing || ofsHallStat < 0)
                return;
            BitConverter.GetBytes((ushort)NUD_HallStreaks.Value).CopyTo(SAV.Data, ofsHallStat + 4 + (0x3DE * CB_Stats2.SelectedIndex) + (species << 1));
            HallStatUpdated = true;
        }
        #endregion

        private void B_UnlockCourses_Click(object sender, EventArgs e)
        {
            ((SAV4HGSS)SAV).PokewalkerCoursesUnlockAll();
        }

        private void OnBAllSealsLegalOnClick(object sender, EventArgs e)
        {
            SAV.SetAllSeals(99, sender == B_AllSealsIllegal);
            System.Media.SystemSounds.Asterisk.Play();
        }
    }
}
