using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PKHeX.Core;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.WinForms;

public partial class SAV_Misc3 : Form
{
    private readonly SaveFile Origin;
    private readonly SAV3 SAV;

    public SAV_Misc3(SAV3 sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (SAV3)(Origin = sav).Clone();

        LoadRecords();

        if (SAV is IGen3Hoenn h)
        {
            pokeblock3CaseEditor1.Initialize(h);
            ReadDecorations(h);
        }
        else
        {
            TC_Misc.Controls.Remove(Tab_Pokeblocks);
            TC_Misc.Controls.Remove(Tab_Decorations);
        }

        if (SAV is IGen3Joyful j)
            ReadJoyful(j);
        else
            TC_Misc.Controls.Remove(TAB_Joyful);

        if (SAV is SAV3E)
        {
            ReadFerry();
            ReadBattleFrontier();
        }
        else
        {
            TC_Misc.Controls.Remove(TAB_Ferry);
            TC_Misc.Controls.Remove(TAB_BF);
        }

        if (SAV is SAV3FRLG frlg)
        {
            TB_RivalName.Text = frlg.RivalName;

            // Trainer Card Species
            ComboBox[] cba = [CB_TCM1, CB_TCM2, CB_TCM3, CB_TCM4, CB_TCM5, CB_TCM6];
            var legal = GameInfo.FilteredSources.Species.ToList();
            for (int i = 0; i < cba.Length; i++)
            {
                cba[i].Items.Clear();
                cba[i].InitializeBinding();
                cba[i].DataSource = new BindingSource(legal, null);
                var g3Species = SAV.GetWork(0x43 + i);
                var species = SpeciesConverter.GetNational3(g3Species);
                cba[i].SelectedValue = (int)species;
            }
        }
        else
        { TB_RivalName.Visible = L_TrainerName.Visible = GB_TCM.Visible = false; }

        NUD_Coins.Value = Math.Min(NUD_Coins.Maximum, SAV.Coin);
    }

    private void B_Save_Click(object sender, EventArgs e)
    {
        if (SAV is IGen3Hoenn h)
        {
            pokeblock3CaseEditor1.Save(h);
            SaveDecorations(h);
        }
        if (TC_Misc.Controls.Contains(TAB_Joyful) && SAV is IGen3Joyful j)
            SaveJoyful(j);
        if (TC_Misc.Controls.Contains(TAB_Ferry))
            SaveFerry();
        if (TC_Misc.Controls.Contains(TAB_BF))
            SaveBattleFrontier();
        if (SAV is SAV3FRLG frlg)
        {
            frlg.RivalName = TB_RivalName.Text;
            ComboBox[] cba = [CB_TCM1, CB_TCM2, CB_TCM3, CB_TCM4, CB_TCM5, CB_TCM6];
            for (int i = 0; i < cba.Length; i++)
            {
                var species = (ushort)WinFormsUtil.GetIndex(cba[i]);
                var g3Species = SpeciesConverter.GetInternal3(species);
                SAV.SetWork(0x43 + i, g3Species);
            }
        }

        if (SAV is SAV3E se)
            se.BP = (ushort)NUD_BP.Value;
        SAV.Coin = (ushort)NUD_Coins.Value;

        Origin.CopyChangesFrom(SAV);
        Close();
    }

    private void B_Cancel_Click(object sender, EventArgs e) => Close();

    #region Joyful
    private void ReadJoyful(IGen3Joyful j)
    {
        TB_J1.Text = Math.Min((ushort)9999, j.JoyfulJumpInRow).ToString();
        TB_J2.Text = Math.Min(99990, j.JoyfulJumpScore).ToString();
        TB_J3.Text = Math.Min((ushort)9999, j.JoyfulJump5InRow).ToString();
        TB_J4.Text = Math.Min((ushort)9999, j.JoyfulJumpGamesMaxPlayers).ToString();
        TB_B1.Text = Math.Min((ushort)9999, j.JoyfulBerriesInRow).ToString();
        TB_B2.Text = Math.Min(99990, j.JoyfulBerriesScore).ToString();
        TB_B3.Text = Math.Min((ushort)9999, j.JoyfulBerries5InRow).ToString();
        TB_BerryPowder.Text = Math.Min(99999u, j.BerryPowder).ToString();
    }

    private void SaveJoyful(IGen3Joyful j)
    {
        j.JoyfulJumpInRow = (ushort)Util.ToUInt32(TB_J1.Text);
        j.JoyfulJumpScore = (ushort)Util.ToUInt32(TB_J2.Text);
        j.JoyfulJump5InRow = (ushort)Util.ToUInt32(TB_J3.Text);
        j.JoyfulJumpGamesMaxPlayers = (ushort)Util.ToUInt32(TB_J4.Text);
        j.JoyfulBerriesInRow = (ushort)Util.ToUInt32(TB_B1.Text);
        j.JoyfulBerriesScore = (ushort)Util.ToUInt32(TB_B2.Text);
        j.JoyfulBerries5InRow = (ushort)Util.ToUInt32(TB_B3.Text);
        j.BerryPowder = Util.ToUInt32(TB_BerryPowder.Text);
    }
    #endregion

    private const ushort ItemIDOldSeaMap = 0x178;
    private static ReadOnlySpan<ushort> TicketItemIDs => [ 0x109, 0x113, 0x172, 0x173, ItemIDOldSeaMap ]; // item IDs

    #region Ferry
    private void B_GetTickets_Click(object sender, EventArgs e)
    {
        var Pouches = SAV.Inventory;
        var itemlist = GameInfo.Strings.GetItemStrings(SAV.Context, SAV.Version);

        var tickets = TicketItemIDs;
        var p = Pouches.First(z => z.Type == InventoryType.KeyItems);
        bool hasOldSea = Array.Exists(p.Items, static z => z.Index == ItemIDOldSeaMap);
        if (!hasOldSea && !SAV.Japanese && DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, $"Non Japanese save file. Add {itemlist[ItemIDOldSeaMap]} (unreleased)?"))
            tickets = tickets[..^1]; // remove old sea map

        // check for missing tickets
        Span<ushort> have = stackalloc ushort[tickets.Length]; int h = 0;
        Span<ushort> missing = stackalloc ushort[tickets.Length]; int m = 0;
        foreach (var item in tickets)
        {
            bool has = Array.Exists(p.Items, z => z.Index == item);
            if (has)
                have[h++] = item;
            else
                missing[m++] = item;
        }
        have = have[..h];
        missing = missing[..m];

        if (missing.Length == 0)
        {
            WinFormsUtil.Alert("Already have all tickets.");
            B_GetTickets.Enabled = false;
            return;
        }

        // check for space
        int end = Array.FindIndex(p.Items, static z => z.Index == 0);
        if (end == -1 || end + missing.Length >= p.Items.Length)
        {
            WinFormsUtil.Alert("Not enough space in pouch.", "Please use the InventoryEditor.");
            B_GetTickets.Enabled = false;
            return;
        }

        static string Format(ReadOnlySpan<ushort> items, ReadOnlySpan<string> names)
        {
            var sbAdd = new StringBuilder();
            foreach (var item in items)
            {
                if (sbAdd.Length != 0)
                    sbAdd.Append(", ");
                sbAdd.Append(names[item]);
            }
            return sbAdd.ToString();
        }
        var added = Format(missing, itemlist);
        var addmsg = $"Add the following items?{Environment.NewLine}{added}";
        if (have.Length != 0)
        {
            string had = Format(have, itemlist);
            var havemsg = $"Already have:{Environment.NewLine}{had}";
            addmsg += Environment.NewLine + Environment.NewLine + havemsg;
        }
        if (DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, addmsg))
            return;

        // insert items at the end
        for (int i = 0; i < missing.Length; i++)
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
        CHK_Catchable.Checked = SAV.GetEventFlag(0x864);
        CHK_ReachSouthern.Checked = SAV.GetEventFlag(0x8B3);
        CHK_ReachBirth.Checked = SAV.GetEventFlag(0x8D5);
        CHK_ReachFaraway.Checked = SAV.GetEventFlag(0x8D6);
        CHK_ReachNavel.Checked = SAV.GetEventFlag(0x8E0);
        CHK_ReachBF.Checked = SAV.GetEventFlag(0x1D0);
        CHK_InitialSouthern.Checked = SAV.GetEventFlag(0x1AE);
        CHK_InitialBirth.Checked = SAV.GetEventFlag(0x1AF);
        CHK_InitialFaraway.Checked = SAV.GetEventFlag(0x1B0);
        CHK_InitialNavel.Checked = SAV.GetEventFlag(0x1DB);
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
    private Button[] SymbolButtonA = null!;
    private bool editingcont;
    private bool editingval;
    private RadioButton[] StatRBA = null!;
    private NumericUpDown[] StatNUDA = null!;
    private Label[] StatLabelA = null!;
    private bool loading;
    private int[][] BFF = null!;
    private string[]?[] BFT = null!;
    private int[][] BFV = null!;
    private string[] BFN = null!;

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

        var bft = BFT[BFF[facility][1]];
        if (bft == null)
        {
            CB_Stats2.Visible = false;
        }
        else
        {
            CB_Stats2.Visible = true;
            CB_Stats2.Items.AddRange(bft);
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
        var bft = BFT[BFF[Facility][1]];
        if (bft == null)
            BattleType = 0;
        else if (BattleType < 0)
            return;
        else if (BattleType >= bft.Length)
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
            if (val > 9999)
                val = 9999;
            var offset = BFF[Facility][2 + SetValToSav] + (4 * BattleType) + (2 * RBi);
            WriteUInt32LittleEndian(SAV.Small.AsSpan(offset), val);
            return;
        }
        if (SetValToSav == -1)
        {
            int p = BFF[Facility][2 + BFV[BFF[Facility][0]].Length + BattleType] + RBi;
            const int offset = 0xCDC;
            var current = ReadUInt32LittleEndian(SAV.Small.AsSpan(offset));
            var update = (current & ~(1u << p)) | (CHK_Continue.Checked ? 1u : 0) << p;
            WriteUInt32LittleEndian(SAV.Small.AsSpan(offset), update);
            return;
        }
        if (!SetSavToVal)
            return;

        editingval = true;
        for (int i = 0; i < BFV[BFF[Facility][0]].Length; i++)
        {
            var offset = BFF[Facility][2 + i] + (4 * BattleType) + (2 * RBi);
            int vali = ReadUInt16LittleEndian(SAV.Small.AsSpan(offset));
            if (vali > 9999)
                vali = 9999;
            StatNUDA[BFV[BFF[Facility][0]][i]].Value = vali;
        }

        var shift = (BFF[Facility][2 + BFV[BFF[Facility][0]].Length + BattleType] + RBi);
        CHK_Continue.Checked = (ReadUInt32LittleEndian(SAV.Small.AsSpan(0xCDC)) & (1 << shift)) != 0;
        editingval = false;
    }

    private void ChangeStatVal(object sender, EventArgs e)
    {
        if (editingval || sender is not NumericUpDown nud)
            return;
        int n = Array.IndexOf(StatNUDA, nud);
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
        BFF = [
            // { BFV, BFT, addr(BFV.len), checkBitShift(BFT.len)
            [0, 2, 0xCE0, 0xCF0, 0x00, 0x0E, 0x10, 0x12],
            [1, 1, 0xD0C, 0xD14, 0xD1C, 0x02, 0x14],
            [0, 1, 0xDC8, 0xDD0, 0x04, 0x16],
            [0, 0, 0xDDA, 0xDDE, 0x06],
            [2, 1, 0xDE2, 0xDF2, 0xDEA, 0xDFA, 0x08, 0x18],
            [1, 0, 0xE04, 0xE08, 0xE0C, 0x0A],
            [0, 0, 0xE1A, 0xE1E, 0x0C],
        ];
        BFV =
        [
            [0, 2], // Current, Max
            [0, 2, 3], // Current, Max, Total
            [0, 1, 2, 3], // Current, Trade, Max, Trade
        ];
        BFT = [
            null,
            ["Singles", "Doubles"],
            ["Singles", "Doubles", "Multi", "Linked"],
        ];
        BFN =
        [
            "Tower","Dome","Palace","Arena","Factory","Pike","Pyramid",
        ];
        StatNUDA = [NUD_Stat0, NUD_Stat1, NUD_Stat2, NUD_Stat3];
        StatLabelA = [L_Stat0, L_Stat1, L_Stat2, L_Stat3];
        StatRBA = [RB_Stats3_01, RB_Stats3_02];
        SymbolButtonA = [BTN_SymbolA, BTN_SymbolT, BTN_SymbolS, BTN_SymbolG, BTN_SymbolK, BTN_SymbolL, BTN_SymbolB];
        CHK_ActivatePass.Checked = SAV.GetEventFlag(0x860 + 0x72);
        SetFrontierSymbols();

        CB_Stats1.Items.Clear();
        CB_Stats1.Items.AddRange(BFN);

        loading = false;
        CB_Stats1.SelectedIndex = 0;
    }

    private void SetFrontierSymbols()
    {
        for (int i = 0; i < SymbolButtonA.Length; i++)
        {
            var flagIndex = 0x860 + 0x64 + (i * 2);
            var silver = SAV.GetEventFlag(flagIndex);
            var gold = SAV.GetEventFlag(flagIndex + 1);
            var value = silver ? gold ? Color.Gold : Color.Silver : Color.Transparent;
            SymbolButtonA[i].BackColor = value;
        }
    }

    private void SaveBattleFrontier()
    {
        for (int i = 0; i < 7; i++)
        {
            var color = SymbolButtonA[i].BackColor;
            bool silver = color != Color.Transparent;
            bool gold = color == Color.Gold;

            var flagIndex = 0x860 + 0x64 + (i * 2);
            SAV.SetEventFlag(flagIndex, silver);
            SAV.SetEventFlag(flagIndex + 1, gold);
        }
        SAV.SetEventFlag(0x860 + 0x72, CHK_ActivatePass.Checked);
    }

    private void BTN_Symbol_Click(object sender, EventArgs e)
    {
        var match = Array.Find(SymbolButtonA, z => z == sender);
        if (match == null)
            return;

        var color = match.BackColor;
        color = color == Color.Transparent ? Color.Silver : color == Color.Silver ? Color.Gold : Color.Transparent;
        match.BackColor = color;
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

        CB_Record.SelectedIndexChanged += (_, _) =>
        {
            if (CB_Record.SelectedValue == null)
                return;

            var index = WinFormsUtil.GetIndex(CB_Record);
            LoadRecordID(index);
            NUD_FameH.Visible = NUD_FameS.Visible = NUD_FameM.Visible = index == 1;
        };
        CB_Record.MouseWheel += (_, e) => ((HandledMouseEventArgs)e).Handled = true; // disallowed
        CB_Record.SelectedIndex = 0;
        LoadRecordID(0);
        NUD_RecordValue.ValueChanged += (_, _) =>
        {
            if (CB_Record.SelectedValue == null)
                return;

            var index = WinFormsUtil.GetIndex(CB_Record);
            var value = (uint)NUD_RecordValue.Value;
            records.SetRecord(index, value);
            if (index == 1)
                LoadFame(value);
        };

        if (SAV is SAV3E em)
        {
            NUD_BP.Value = Math.Min(NUD_BP.Maximum, em.BP);
            NUD_BPEarned.Value = em.BPEarned;
            NUD_BPEarned.ValueChanged += (_, _) => em.BPEarned = (uint)NUD_BPEarned.Value;
        }
        else
        {
            NUD_BP.Visible = L_BP.Visible = false;
            NUD_BPEarned.Visible = L_BPEarned.Visible = false;
        }

        NUD_FameH.ValueChanged += (_, _) => ChangeFame(records);
        NUD_FameM.ValueChanged += (_, _) => ChangeFame(records);
        NUD_FameS.ValueChanged += (_, _) => ChangeFame(records);

        void ChangeFame(Record3 r3) => r3.SetRecord(1, (uint)(NUD_RecordValue.Value = GetFameTime()));
        void LoadRecordID(int index) => NUD_RecordValue.Value = records.GetRecord(index);
        void LoadFame(uint val) => SetFameTime(val);
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
        NUD_FameM.Value = Math.Min(NUD_FameM.Maximum, (byte)(time >> 8));
        NUD_FameS.Value = Math.Min(NUD_FameS.Maximum, (byte)time);
    }

    #region Decorations
    private void ReadDecorations(IGen3Hoenn h)
    {
        DataGridViewComboBoxColumn[] columns =
        [
            Item_Desk,
            Item_Chair,
            Item_Plant,
            Item_Ornament,
            Item_Mat,
            Item_Poster,
            Item_Doll,
            Item_Cushion,
        ];

        var decorations = Util.GetStringList("decoration3", Main.CurrentLanguage);
        var list = Util.GetCBList(decorations);

        foreach (var col in columns)
        {
            col.Items.Clear();
            col.InitializeBinding();
        }
        foreach (var cb in list)
        {
            var cat = ((Decoration3)cb.Value).GetCategory();
            if (cb.Value == (int)Decoration3.NONE)
            {
                foreach (var col in columns) // all categories can have empty slots
                    col.Items.Add(cb);
                continue;
            }
            columns[(int)cat].Items.Add(cb);
        }

        ReadDecorationCategory(h.Decorations.Desk, DGV_Desk);
        ReadDecorationCategory(h.Decorations.Chair, DGV_Chair);
        ReadDecorationCategory(h.Decorations.Plant, DGV_Plant);
        ReadDecorationCategory(h.Decorations.Ornament, DGV_Ornament);
        ReadDecorationCategory(h.Decorations.Mat, DGV_Mat);
        ReadDecorationCategory(h.Decorations.Poster, DGV_Poster);
        ReadDecorationCategory(h.Decorations.Doll, DGV_Doll);
        ReadDecorationCategory(h.Decorations.Cushion, DGV_Cushion);
    }

    private static void ReadDecorationCategory(ReadOnlySpan<Decoration3> data, DataGridView dgv)
    {
        dgv.Rows.Clear();
        dgv.Rows.Add(data.Length);
        for (int i = 0; i < data.Length; i++)
            dgv.Rows[i].Cells[0].Value = (int)data[i];
    }

    private void SaveDecorations(IGen3Hoenn h)
    {
        SaveDecorationCategory(h.Decorations.Desk, DGV_Desk);
        SaveDecorationCategory(h.Decorations.Chair, DGV_Chair);
        SaveDecorationCategory(h.Decorations.Plant, DGV_Plant);
        SaveDecorationCategory(h.Decorations.Ornament, DGV_Ornament);
        SaveDecorationCategory(h.Decorations.Mat, DGV_Mat);
        SaveDecorationCategory(h.Decorations.Poster, DGV_Poster);
        SaveDecorationCategory(h.Decorations.Doll, DGV_Doll);
        SaveDecorationCategory(h.Decorations.Cushion, DGV_Cushion);
    }

    private static void SaveDecorationCategory(Span<Decoration3> data, DataGridView dgv)
    {
        int ctr = 0;
        for (int i = 0; i < data.Length; i++)
        {
            var deco = (Decoration3)(int)dgv.Rows[i].Cells[0].Value;
            if (deco == Decoration3.NONE) // Compression of Empty Slots
                continue;

            data[ctr] = deco;
            ctr++;
        }
        for (int i = ctr; i < data.Length; i++)
            data[i] = Decoration3.NONE; // Empty Slots at the end
    }
    #endregion
}
