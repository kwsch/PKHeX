using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SAV_Misc3 : Form
    {
        private readonly SaveFile Origin;
        private readonly SAV3 SAV;

        public SAV_Misc3(SaveFile sav)
        {
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            SAV = (SAV3)(Origin = sav).Clone();

            LoadRecords();

            if (SAV.FRLG || SAV.E)
                ReadJoyful();
            else
                tabControl1.Controls.Remove(TAB_Joyful);

            if (SAV.E)
            {
                ReadFerry();
                ReadBattleFrontier();
            }
            else
            {
                tabControl1.Controls.Remove(TAB_Ferry);
                tabControl1.Controls.Remove(TAB_BF);
            }

            if (SAV.FRLG)
            {
                TB_OTName.Text = SAV.GetString(SAV.GetBlockOffset(4) + 0xBCC, 8);
                ComboBox[] cba = { CB_TCM1, CB_TCM2, CB_TCM3, CB_TCM4, CB_TCM5, CB_TCM6 };
                int[] HoennListMixed = {
                     277,278,279,280,281,282,283,284,285,286,287,288,289,290,291,292,293,294,295,296,297,298,299,300,
                 304,305,309,310,392,393,394,311,312,306,307,364,365,366,301,302,303,370,371,372,335,336,350,320,315,316,
                                         322,355,382,383,384,356,357,337,338,353,354,386,387,363,367,368,330,331,313,314,
                                         339,340,321,351,352,308,332,333,334,344,345,358,359,380,379,348,349,323,324,
                                     326,327,318,319,388,389,390,391,328,329,385,317,377,378,361,362,369,411,376,360,
                                     346,347,341,342,343,373,374,375,381,325,395,396,397,398,399,400,
                     401,402,403,407,408,404,405,406,409,410
                };
                var speciesList = GameInfo.SpeciesDataSource.Where(v => v.Value <= SAV.MaxSpeciesID).Select(v =>
                    new ComboItem (v.Text, v.Value < 252 ? v.Value : HoennListMixed[v.Value - 252])
                ).ToList();
                int ofsTCM = SAV.GetBlockOffset(2) + 0x106;
                for (int i = 0; i < cba.Length; i++)
                {
                    cba[i].Items.Clear();
                    cba[i].InitializeBinding();
                    cba[i].DataSource = new BindingSource(speciesList, null);
                    cba[i].SelectedValue = (int)BitConverter.ToUInt16(SAV.Data, ofsTCM + (i << 1));
                }
            }
            else
            { TB_OTName.Visible = L_TrainerName.Visible = GB_TCM.Visible = false; }

            if (SAV.RS)
                NUD_BP.Visible = L_BP.Visible = false;
            else
                NUD_BP.Value = Math.Min(NUD_BP.Maximum, SAV.BP);
            NUD_Coins.Value = Math.Min(NUD_Coins.Maximum, SAV.Coin);
        }

        private void B_Save_Click(object sender, EventArgs e)
        {
            if (tabControl1.Controls.Contains(TAB_Joyful))
                SaveJoyful();
            if (tabControl1.Controls.Contains(TAB_Ferry))
                SaveFerry();
            if (tabControl1.Controls.Contains(TAB_BF))
                SaveBattleFrontier();
            if (SAV.FRLG)
            {
                SAV.SetData(SAV.SetString(TB_OTName.Text, TB_OTName.MaxLength), SAV.GetBlockOffset(4) + 0xBCC);
                ComboBox[] cba = { CB_TCM1, CB_TCM2, CB_TCM3, CB_TCM4, CB_TCM5, CB_TCM6 };
                int ofsTCM = SAV.GetBlockOffset(2) + 0x106;
                for (int i = 0; i < cba.Length; i++)
                    BitConverter.GetBytes((ushort)(int)cba[i].SelectedValue).CopyTo(SAV.Data, ofsTCM + (i << 1));
            }

            if (!SAV.RS)
                SAV.BP = (ushort)NUD_BP.Value;
            SAV.Coin = (ushort)NUD_Coins.Value;

            Origin.CopyChangesFrom(SAV);
            Close();
        }

        private void B_Cancel_Click(object sender, EventArgs e) => Close();

        #region Joyful
        private int JUMPS_IN_ROW, JUMPS_SCORE, JUMPS_5_IN_ROW;
        private int BERRIES_IN_ROW, BERRIES_SCORE, BERRIES_5_IN_ROW;

        private void ReadJoyful()
        {
            switch (SAV.Version)
            {
                case GameVersion.E:
                    JUMPS_IN_ROW = SAV.GetBlockOffset(0) + 0x1fc;
                    JUMPS_SCORE = SAV.GetBlockOffset(0) + 0x208;
                    JUMPS_5_IN_ROW = SAV.GetBlockOffset(0) + 0x200;

                    BERRIES_IN_ROW = SAV.GetBlockOffset(0) + 0x210;
                    BERRIES_SCORE = SAV.GetBlockOffset(0) + 0x20c;
                    BERRIES_5_IN_ROW = SAV.GetBlockOffset(0) + 0x212;
                    break;
                case GameVersion.FRLG:
                    JUMPS_IN_ROW = SAV.GetBlockOffset(0) + 0xB00;
                    JUMPS_SCORE = SAV.GetBlockOffset(0) + 0xB0C;
                    JUMPS_5_IN_ROW = SAV.GetBlockOffset(0) + 0xB04;

                    BERRIES_IN_ROW = SAV.GetBlockOffset(0) + 0xB14;
                    BERRIES_SCORE = SAV.GetBlockOffset(0) + 0xB10;
                    BERRIES_5_IN_ROW = SAV.GetBlockOffset(0) + 0xB16;
                    break;
            }
            TB_J1.Text = Math.Min((ushort)9999, BitConverter.ToUInt16(SAV.Data, JUMPS_IN_ROW)).ToString();
            TB_J2.Text = Math.Min((ushort)9999, BitConverter.ToUInt16(SAV.Data, JUMPS_SCORE)).ToString();
            TB_J3.Text = Math.Min((ushort)9999, BitConverter.ToUInt16(SAV.Data, JUMPS_5_IN_ROW)).ToString();
            TB_B1.Text = Math.Min((ushort)9999, BitConverter.ToUInt16(SAV.Data, BERRIES_IN_ROW)).ToString();
            TB_B2.Text = Math.Min((ushort)9999, BitConverter.ToUInt16(SAV.Data, BERRIES_SCORE)).ToString();
            TB_B3.Text = Math.Min((ushort)9999, BitConverter.ToUInt16(SAV.Data, BERRIES_5_IN_ROW)).ToString();
        }

        private void SaveJoyful()
        {
            BitConverter.GetBytes((ushort)Util.ToUInt32(TB_J1.Text)).CopyTo(SAV.Data, JUMPS_IN_ROW);
            BitConverter.GetBytes((ushort)Util.ToUInt32(TB_J2.Text)).CopyTo(SAV.Data, JUMPS_SCORE);
            BitConverter.GetBytes((ushort)Util.ToUInt32(TB_J3.Text)).CopyTo(SAV.Data, JUMPS_5_IN_ROW);
            BitConverter.GetBytes((ushort)Util.ToUInt32(TB_B1.Text)).CopyTo(SAV.Data, BERRIES_IN_ROW);
            BitConverter.GetBytes((ushort)Util.ToUInt32(TB_B2.Text)).CopyTo(SAV.Data, BERRIES_SCORE);
            BitConverter.GetBytes((ushort)Util.ToUInt32(TB_B3.Text)).CopyTo(SAV.Data, BERRIES_5_IN_ROW);
        }
        #endregion

        #region Ferry
        private void B_GetTickets_Click(object sender, EventArgs e)
        {
            var Pouches = SAV.Inventory;
            var itemlist = GameInfo.Strings.GetItemStrings(SAV.Generation, SAV.Version).ToArray();
            for (int i = 0; i < itemlist.Length; i++)
            {
                if (string.IsNullOrEmpty(itemlist[i]))
                    itemlist[i] = $"(Item #{i:000})";
            }

            const int oldsea = 0x178;
            int[] tickets = {0x109, 0x113, 0x172, 0x173, oldsea }; // item IDs
            if (!SAV.Japanese && DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, $"Non Japanese save file. Add {itemlist[oldsea]} (unreleased)?"))
                tickets = tickets.Take(tickets.Length - 1).ToArray(); // remove old sea map

            var p = Array.Find(Pouches, z => z.Type == InventoryType.KeyItems);

            // check for missing tickets
            var missing = tickets.Where(z => !p.Items.Any(item => item.Index == z && item.Count == 1)).ToList();
            var have = tickets.Except(missing).ToList();
            if (missing.Count == 0)
            {
                WinFormsUtil.Alert("Already have all tickets.");
                B_GetTickets.Enabled = false;
                return;
            }

            // check for space
            int end = Array.FindIndex(p.Items, z => z.Index == 0);
            if (end + missing.Count >= p.Items.Length)
            {
                WinFormsUtil.Alert("Not enough space in pouch.", "Please use the InventoryEditor.");
                B_GetTickets.Enabled = false;
                return;
            }

            var added = string.Join(", ", missing.Select(u => itemlist[u]));
            var addmsg = $"Add the following items?{Environment.NewLine}{added}";
            if (have.Count > 0)
            {
                string had = string.Join(", ", have.Select(u => itemlist[u]));
                var havemsg = $"Already have:{Environment.NewLine}{had}";
                addmsg += Environment.NewLine + Environment.NewLine + havemsg;
            }
            if (DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, addmsg))
                return;

            // insert items at the end
            for (int i = 0; i < missing.Count; i++)
            {
                var item = p.Items[end + i];
                item.Index = missing[i];
                item.Count = 1;
            }

            string alert = $"Inserted the following items to the Key Items Pouch:{Environment.NewLine}{added}";
            WinFormsUtil.Alert(alert);
            SAV.Inventory = Pouches;

            B_GetTickets.Enabled = false;
        }

        private void ReadFerry()
        {
            CHK_Catchable.Checked       = SAV.GetEventFlag(0x864);
            CHK_ReachSouthern.Checked   = SAV.GetEventFlag(0x8B3);
            CHK_ReachBirth.Checked      = SAV.GetEventFlag(0x8D5);
            CHK_ReachFaraway.Checked    = SAV.GetEventFlag(0x8D6);
            CHK_ReachNavel.Checked      = SAV.GetEventFlag(0x8E0);
            CHK_ReachBF.Checked         = SAV.GetEventFlag(0x1D0);
            CHK_InitialSouthern.Checked = SAV.GetEventFlag(0x1AE);
            CHK_InitialBirth.Checked    = SAV.GetEventFlag(0x1AF);
            CHK_InitialFaraway.Checked  = SAV.GetEventFlag(0x1B0);
            CHK_InitialNavel.Checked    = SAV.GetEventFlag(0x1DB);
        }

        private void SaveFerry()
        {
            SAV.SetEventFlag(0x864, CHK_Catchable.Checked);
            SAV.SetEventFlag(0x8B3, CHK_ReachSouthern.Checked);
            SAV.SetEventFlag(0x8D5, CHK_ReachBirth.Checked);
            SAV.SetEventFlag(0x8D6, CHK_ReachFaraway.Checked);
            SAV.SetEventFlag(0x8E0, CHK_ReachNavel.Checked);
            SAV.SetEventFlag(0x1D0, CHK_ReachBF.Checked);
            SAV.SetEventFlag(0x1AE, CHK_InitialSouthern.Checked);
            SAV.SetEventFlag(0x1AF, CHK_InitialBirth.Checked);
            SAV.SetEventFlag(0x1B0, CHK_InitialFaraway.Checked);
            SAV.SetEventFlag(0x1DB, CHK_InitialNavel.Checked);
        }
        #endregion

        #region BattleFrontier
        private int[] Symbols;
        private int ofsSymbols;
        private Color[] SymbolColorA;
        private Button[] SymbolButtonA;
        private bool editingcont;
        private bool editingval;
        private RadioButton[] StatRBA;
        private NumericUpDown[] StatNUDA;
        private Label[] StatLabelA;
        private bool loading;
        private int[][] BFF;
        private string[][] BFT;
        private int[][] BFV;
        private string[] BFN;

        private void ChangeStat1(object sender, EventArgs e)
        {
            if (loading)
                return;
            int facility = CB_Stats1.SelectedIndex;
            if ((uint)facility >= BFN.Length)
                return;
            editingcont = true;
            CB_Stats2.Items.Clear();
            foreach (RadioButton rb in StatRBA)
                rb.Checked = false;

            if (BFT[BFF[facility][1]] == null)
            {
                CB_Stats2.Visible = false;
            }
            else
            {
                CB_Stats2.Visible = true;
                for (int i = 0; i < BFT[BFF[facility][1]].Length; i++)
                    CB_Stats2.Items.Add(BFT[BFF[facility][1]][i]);
                CB_Stats2.SelectedIndex = 0;
            }

            for (int i = 0; i < StatLabelA.Length; i++)
                StatLabelA[i].Visible = StatLabelA[i].Enabled = StatNUDA[i].Visible = StatNUDA[i].Enabled = Array.IndexOf(BFV[BFF[facility][0]], i) >= 0;

            editingcont = false;
            StatRBA[0].Checked = true;
        }

        private void ChangeStat(object sender, EventArgs e)
        {
            if (editingcont)
                return;
            StatAddrControl(SetValToSav: -2, SetSavToVal: true);
        }

        private void StatAddrControl(int SetValToSav = -2, bool SetSavToVal = false)
        {
            int Facility = CB_Stats1.SelectedIndex;
            if (Facility < 0)
                return;

            int BattleType = CB_Stats2.SelectedIndex;
            if (BFT[BFF[Facility][1]] == null)
                BattleType = 0;
            else if (BattleType < 0)
                return;
            else if (BattleType >= BFT[BFF[Facility][1]].Length)
                return;

            int RBi = -1;
            for (int i = 0, j = 0; i < StatRBA.Length; i++)
            {
                if (!StatRBA[i].Checked)
                    continue;
                if (++j > 1)
                    return;
                RBi = i;
            }
            if (RBi < 0)
                return;

            if (SetValToSav >= 0)
            {
                ushort val = (ushort)StatNUDA[SetValToSav].Value;
                SetValToSav = Array.IndexOf(BFV[BFF[Facility][0]], SetValToSav);
                if (SetValToSav < 0)
                    return;
                if (val > 9999) val = 9999;
                BitConverter.GetBytes(val).CopyTo(SAV.Data, SAV.GetBlockOffset(0) + BFF[Facility][2 + SetValToSav] + (4 * BattleType) + (2 * RBi));
                return;
            }
            if (SetValToSav == -1)
            {
                int p = BFF[Facility][2 + BFV[BFF[Facility][0]].Length + BattleType] + RBi;
                int offset = SAV.GetBlockOffset(0) + 0xCDC;
                BitConverter.GetBytes((BitConverter.ToUInt32(SAV.Data, offset) & (uint)~(1 << p)) | (uint)((CHK_Continue.Checked ? 1 : 0) << p)).CopyTo(SAV.Data, offset);
                return;
            }
            if (!SetSavToVal)
                return;

            editingval = true;
            for (int i = 0; i < BFV[BFF[Facility][0]].Length; i++)
            {
                int vali = BitConverter.ToUInt16(SAV.Data, SAV.GetBlockOffset(0) + BFF[Facility][2 + i] + (4 * BattleType) + (2 * RBi));
                if (vali > 9999) vali = 9999;
                StatNUDA[BFV[BFF[Facility][0]][i]].Value = vali;
            }
            CHK_Continue.Checked = (BitConverter.ToUInt32(SAV.Data, SAV.GetBlockOffset(0) + 0xCDC) & 1 << (BFF[Facility][2 + BFV[BFF[Facility][0]].Length + BattleType] + RBi)) != 0;
            editingval = false;
        }

        private void ChangeStatVal(object sender, EventArgs e)
        {
            if (editingval)
                return;
            int n = Array.IndexOf(StatNUDA, sender);
            if (n < 0)
                return;
            StatAddrControl(SetValToSav: n, SetSavToVal: false);
        }

        private void CHK_Continue_CheckedChanged(object sender, EventArgs e)
        {
            if (editingval)
                return;
            StatAddrControl(SetValToSav: -1, SetSavToVal: false);
        }

        private void ReadBattleFrontier()
        {
            loading = true;
            BFF = new[] {
                // { BFV, BFT, addr(BFV.len), checkBitShift(BFT.len)
                new[] { 0, 2, 0xCE0, 0xCF0, 0x00, 0x0E, 0x10, 0x12 },
                new[] { 1, 1, 0xD0C, 0xD14, 0xD1C, 0x02, 0x14 },
                new[] { 0, 1, 0xDC8, 0xDD0, 0x04, 0x16 },
                new[] { 0, 0, 0xDDA, 0xDDE, 0x06 },
                new[] { 2, 1, 0xDE2, 0xDF2, 0xDEA, 0xDFA, 0x08, 0x18 },
                new[] { 1, 0, 0xE04, 0xE08, 0xE0C, 0x0A },
                new[] { 0, 0, 0xE1A, 0xE1E, 0x0C },
            };
            BFV = new[]
            {
                new[] { 0, 2 }, // Current, Max
                new[] { 0, 2, 3 }, // Current, Max, Total
                new[] { 0, 1, 2, 3 }, // Current, Trade, Max, Trade
            };
            BFT = new[] {
                null,
                new[] { "Singles", "Doubles" },
                new[] { "Singles", "Doubles", "Multi", "Linked" },
            };
            BFN = new[]
            {
                "Tower","Dome","Palace","Arena","Factory","Pike","Pyramid"
            };
            StatNUDA = new[] { NUD_Stat0, NUD_Stat1, NUD_Stat2, NUD_Stat3 };
            StatLabelA = new[] { L_Stat0, L_Stat1, L_Stat2, L_Stat3 };
            StatRBA = new[] { RB_Stats3_01, RB_Stats3_02 };
            SymbolColorA = new[] { Color.Transparent, Color.Silver, Color.Silver, Color.Gold };
            SymbolButtonA = new[] { BTN_SymbolA, BTN_SymbolT, BTN_SymbolS, BTN_SymbolG, BTN_SymbolK, BTN_SymbolL, BTN_SymbolB };
            ofsSymbols = SAV.GetBlockOffset(2) + 0x408;
            int iSymbols = BitConverter.ToInt32(SAV.Data, ofsSymbols) >> 4 & 0x7FFF;
            CHK_ActivatePass.Checked = (iSymbols >> 14 & 1) != 0;
            Symbols = new int[7];
            for (int i = 0; i < 7; i++)
                Symbols[i] = iSymbols >> i * 2 & 3;
            SetFrontierSymbols();

            CB_Stats1.Items.Clear();
            foreach (string t in BFN)
                CB_Stats1.Items.Add(t);

            loading = false;
            CB_Stats1.SelectedIndex = 0;
        }

        private void SetFrontierSymbols()
        {
            for (int i = 0; i < SymbolButtonA.Length; i++)
                SymbolButtonA[i].BackColor = SymbolColorA[Symbols[i]];
        }

        private void SaveBattleFrontier()
        {
            uint iSymbols = 0;
            for (int i = 0; i < 7; i++)
                iSymbols |= (uint)((Symbols[i] & 3) << i * 2);
            if (CHK_ActivatePass.Checked)
                iSymbols |= 1 << 14;

            uint val = (uint)((BitConverter.ToUInt32(SAV.Data, ofsSymbols) & ~(0x7FFF << 4)) | (iSymbols & 0x7FFF) << 4);
            BitConverter.GetBytes(val).CopyTo(SAV.Data, ofsSymbols);
        }

        private void BTN_Symbol_Click(object sender, EventArgs e)
        {
            int index = Array.IndexOf(SymbolButtonA, sender);
            if (index < 0)
                return;

            // 0 (none) | 1 (silver) | 2 (silver) | 3 (gold)
            // bit rotation 00 -> 01 -> 11 -> 00
            if (Symbols[index] == 1) Symbols[index] = 3;
            else Symbols[index] = (Symbols[index] + 1) & 3;

            SetFrontierSymbols();
        }
        #endregion

        private void LoadRecords()
        {
            var records = new Record3(SAV);
            var items = Record3.GetItems(SAV);
            CB_Record.InitializeBinding();
            CB_Record.DataSource = items;
            NUD_RecordValue.Minimum = int.MinValue;
            NUD_RecordValue.Maximum = int.MaxValue;

            CB_Record.SelectedIndexChanged += (s, e) =>
            {
                if (CB_Record.SelectedValue == null)
                    return;

                var index = WinFormsUtil.GetIndex(CB_Record);
                LoadRecordID(index);
                NUD_FameH.Visible = NUD_FameS.Visible = NUD_FameM.Visible = index == 1;
            };
            CB_Record.SelectedIndex = 0;
            LoadRecordID(0);
            NUD_RecordValue.ValueChanged += (s, e) =>
            {
                if (CB_Record.SelectedValue == null)
                    return;

                var index = WinFormsUtil.GetIndex(CB_Record);
                var val = (uint) NUD_RecordValue.Value;
                records.SetRecord(index, val);
                if (index == 1)
                    LoadFame(val);
            };
            NUD_FameH.ValueChanged += (s, e) => ChangeFame();
            NUD_FameM.ValueChanged += (s, e) => ChangeFame();
            NUD_FameS.ValueChanged += (s, e) => ChangeFame();

            void ChangeFame() => records.SetRecord(1, (uint)(NUD_RecordValue.Value = GetFameTime()));
            void LoadRecordID(int index) => NUD_RecordValue.Value = records.GetRecord(index);
            void LoadFame(uint val) => SetFameTime(val);

            NUD_BPEarned.Value = SAV.BPEarned;
            NUD_BPEarned.ValueChanged += (s, e) => SAV.BPEarned = (uint)NUD_BPEarned.Value;
        }

        public uint GetFameTime()
        {
            var hrs = Math.Min(9999, (uint)NUD_FameH.Value);
            var min = Math.Min(59, (uint)NUD_FameM.Value);
            var sec = Math.Min(59, (uint)NUD_FameS.Value);

            return (hrs << 16) | (min << 8) | sec;
        }

        public void SetFameTime(uint time)
        {
            NUD_FameH.Value = Math.Min(NUD_FameH.Maximum, time >> 16);
            NUD_FameM.Value = Math.Min(NUD_FameH.Maximum, (byte)(time >> 8));
            NUD_FameS.Value = Math.Min(NUD_FameH.Maximum, (byte)time);
        }
    }
}
