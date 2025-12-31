using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Drawing.PokeSprite;

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

            CB_Species.InitializeBinding();
            CB_Species.DataSource = new BindingSource(GameInfo.FilteredSources.Species.ToList(), string.Empty);
            LoadPaintings();
        }
        else
        {
            TC_Misc.Controls.Remove(Tab_Pokeblocks);
            TC_Misc.Controls.Remove(Tab_Decorations);
            TC_Misc.Controls.Remove(Tab_Paintings);
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
                cba[i].DataSource = new BindingSource(legal, string.Empty);
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
            SavePaintings();
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
    private static ReadOnlySpan<ushort> TicketItemIDs => [0x109, 0x113, 0x172, 0x173, ItemIDOldSeaMap]; // item IDs

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

    private void ChangeStat1(object sender, EventArgs e)
    {
        if (loading)
            return;
        if (CB_Stats1.SelectedValue is not BattleFrontierFacility3 facility)
            return;

        editingcont = true;
        CB_Stats2.Items.Clear();
        foreach (RadioButton rb in StatRBA)
            rb.Checked = false;

        int modeCount = BattleFrontier3.GetModeCount(facility);
        if (modeCount == 1)
        {
            CB_Stats2.Visible = false;
            L_Mode.Visible = false;
        }
        else
        {
            CB_Stats2.Visible = true;
            L_Mode.Visible = true;
            for (int i = 0; i < modeCount; i++)
                CB_Stats2.Items.Add(((BattleFrontierBattleMode3)i).ToString());
            CB_Stats2.SelectedIndex = 0;
        }

        var validStats = BattleFrontier3.GetValidStats(facility);
        var context = WinFormsTranslator.GetDictionary(Main.CurrentLanguage);
        for (int i = 0; i < StatLabelA.Length; i++)
        {
            bool isValid = i < validStats.Length;

            StatNUDA[i].Visible = StatNUDA[i].Enabled = isValid;

            var label = StatLabelA[i];
            label.Visible = label.Enabled = isValid;

            // Set the label text using translation keys
            if (!isValid)
                continue;

            var key = GetTranslationKey(facility, validStats[i]);
            label.Text = context.TryGetValue(key, out var text) ? text : key.Split('_')[^1];
        }

        editingcont = false;
        StatRBA[0].Checked = true;
    }

    private static string GetTranslationKey(BattleFrontierFacility3 facility, BattleFrontierStatType3 stat) => (facility, stat) switch
    {
        // Factory has "Rentals Swapped" stat
        (BattleFrontierFacility3.Factory, BattleFrontierStatType3.CurrentSwapped) => $"{nameof(SAV_Misc3)}.L_CurrentSwapped",
        (BattleFrontierFacility3.Factory, BattleFrontierStatType3.RecordSwapped) => $"{nameof(SAV_Misc3)}.L_RecordSwapped",
        // Dome has "Championships" stat
        (BattleFrontierFacility3.Dome, BattleFrontierStatType3.Championships) => $"{nameof(SAV_Misc3)}.L_Championships",
        // Pike has "Cleared" stat
        (BattleFrontierFacility3.Pike, BattleFrontierStatType3.RecordCleared) => $"{nameof(SAV_Misc3)}.L_RecordCleared",
        // Standard labels (Current/Record Streak)
        (_, BattleFrontierStatType3.CurrentStreak) => $"{nameof(SAV_Misc3)}.L_CurrentStreak",
        (_, BattleFrontierStatType3.RecordStreak) => $"{nameof(SAV_Misc3)}.L_RecordStreak",
        _ => "",
    };

    private void ChangeStat(object sender, EventArgs e)
    {
        if (editingcont)
            return;
        LoadStatsFromSave();
    }

    private void LoadStatsFromSave()
    {
        if (CB_Stats1.SelectedValue is not BattleFrontierFacility3 facility)
            return;

        int modeIndex = CB_Stats2.Visible ? CB_Stats2.SelectedIndex : 0;
        if (modeIndex < 0)
            return;

        int recordIndex = -1;
        for (int i = 0; i < StatRBA.Length; i++)
        {
            if (StatRBA[i].Checked)
            {
                recordIndex = i;
                break;
            }
        }
        if (recordIndex < 0)
            return;

        var bf = ((SAV3E)SAV).BattleFrontier;
        var mode = (BattleFrontierBattleMode3)modeIndex;
        var record = (BattleFrontierRecordType3)recordIndex;

        editingval = true;

        var validStats = BattleFrontier3.GetValidStats(facility);
        for (int i = 0; i < validStats.Length; i++)
        {
            var stat = validStats[i];
            ushort value = bf.GetStat(facility, mode, record, stat);
            StatNUDA[i].Value = Math.Min((ushort)9999, value);
        }

        CHK_Continue.Checked = bf.GetContinueFlag(facility, mode, record);

        editingval = false;
    }

    private void ChangeStatVal(object sender, EventArgs e)
    {
        if (editingval || sender is not NumericUpDown nud)
            return;

        int statIndex = StatNUDA.IndexOf(nud);
        if (statIndex < 0)
            return;

        SaveStatToSave(statIndex);
    }

    private void SaveStatToSave(int statIndex)
    {
        if (CB_Stats1.SelectedValue is not BattleFrontierFacility3 facility)
            return;

        int modeIndex = CB_Stats2.Visible ? CB_Stats2.SelectedIndex : 0;
        if (modeIndex < 0)
            return;

        int recordIndex = -1;
        for (int i = 0; i < StatRBA.Length; i++)
        {
            if (StatRBA[i].Checked)
            {
                recordIndex = i;
                break;
            }
        }
        if (recordIndex < 0)
            return;

        var bf = ((SAV3E)SAV).BattleFrontier;
        var mode = (BattleFrontierBattleMode3)modeIndex;
        var record = (BattleFrontierRecordType3)recordIndex;

        var validStats = BattleFrontier3.GetValidStats(facility);
        if (statIndex >= validStats.Length)
            return;

        var stat = validStats[statIndex];
        var value = (ushort)Math.Min(9999, StatNUDA[statIndex].Value);
        bf.SetStat(facility, mode, record, stat, value);
    }

    private void CHK_Continue_CheckedChanged(object sender, EventArgs e)
    {
        if (editingval)
            return;
        SaveContinueFlag();
    }

    private void SaveContinueFlag()
    {
        if (CB_Stats1.SelectedValue is not BattleFrontierFacility3 facility)
            return;

        int modeIndex = CB_Stats2.Visible ? CB_Stats2.SelectedIndex : 0;
        if (modeIndex < 0)
            return;

        int recordIndex = -1;
        for (int i = 0; i < StatRBA.Length; i++)
        {
            if (StatRBA[i].Checked)
            {
                recordIndex = i;
                break;
            }
        }
        if (recordIndex < 0)
            return;

        var bf = ((SAV3E)SAV).BattleFrontier;
        var mode = (BattleFrontierBattleMode3)modeIndex;
        var record = (BattleFrontierRecordType3)recordIndex;

        bf.SetContinueFlag(facility, mode, record, CHK_Continue.Checked);
    }

    private void ReadBattleFrontier()
    {
        loading = true;

        StatNUDA = [NUD_Stat0, NUD_Stat1, NUD_Stat2, NUD_Stat3];
        StatLabelA = [L_Stat0, L_Stat1, L_Stat2, L_Stat3];
        StatRBA = [RB_Stats3_01, RB_Stats3_02];
        SymbolButtonA = [BTN_SymbolA, BTN_SymbolT, BTN_SymbolS, BTN_SymbolG, BTN_SymbolK, BTN_SymbolL, BTN_SymbolB];

        CHK_ActivatePass.Checked = SAV.GetEventFlag(BattleFrontier3.FrontierPassFlagIndex);
        SetFrontierSymbols();

        CB_Stats1.Items.Clear();
        CB_Stats1.InitializeBinding();
        CB_Stats1.DataSource = Enum.GetValues<BattleFrontierFacility3>();

        loading = false;
        CB_Stats1.SelectedIndex = 0;
        ChangeStat1(CB_Stats1, EventArgs.Empty);
    }

    private void SetFrontierSymbols()
    {
        for (int i = 0; i < SymbolButtonA.Length; i++)
        {
            var facility = (BattleFrontierFacility3)i;
            var silver = SAV.GetEventFlag(BattleFrontier3.GetSymbolSilverFlagIndex(facility));
            var gold = SAV.GetEventFlag(BattleFrontier3.GetSymbolGoldFlagIndex(facility));
            var value = silver ? gold ? Color.Gold : Color.Silver : Color.Transparent;
            SymbolButtonA[i].BackColor = value;
        }
    }

    private void SaveBattleFrontier()
    {
        for (int i = 0; i < 7; i++)
        {
            var facility = (BattleFrontierFacility3)i;
            var color = SymbolButtonA[i].BackColor;

            SAV.SetEventFlag(BattleFrontier3.GetSymbolSilverFlagIndex(facility), color != Color.Transparent);
            SAV.SetEventFlag(BattleFrontier3.GetSymbolGoldFlagIndex(facility), color == Color.Gold);
        }

        SAV.SetEventFlag(BattleFrontier3.FrontierPassFlagIndex, CHK_ActivatePass.Checked);
    }

    private void BTN_Symbol_Click(object sender, EventArgs e)
    {
        var match = Array.Find(SymbolButtonA, z => z == sender);
        if (match is null)
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
        NUD_RecordValue.Minimum = 0;
        NUD_RecordValue.Maximum = uint.MaxValue;

        CB_Record.SelectedIndexChanged += (_, _) =>
        {
            if (CB_Record.SelectedValue is null)
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
            if (CB_Record.SelectedValue is null)
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
            var deco = (Decoration3)(int)dgv.Rows[i].Cells[0].Value!;
            if (deco == Decoration3.NONE) // Compression of Empty Slots
                continue;

            data[ctr] = deco;
            ctr++;
        }
        for (int i = ctr; i < data.Length; i++)
            data[i] = Decoration3.NONE; // Empty Slots at the end
    }
    #endregion

    #region Paintings

    private int PaintingIndex = -1;

    private void LoadPaintings() => LoadPainting((int)NUD_Painting.Value);
    private void SavePaintings() => SavePainting((int)NUD_Painting.Value);

    private void ChangePainting(object sender, EventArgs e)
    {
        var index = (int)NUD_Painting.Value;
        if (PaintingIndex == index)
            return;
        SavePainting(PaintingIndex);
        LoadPainting(index);
    }

    private void LoadPainting(int index)
    {
        if ((uint)index >= 5)
            return;
        var gallery = (IGen3Hoenn)SAV;
        var painting = gallery.GetPainting(index);

        GB_Painting.Visible = CHK_EnablePaint.Checked = SAV.GetEventFlag(Paintings3.GetFlagIndexContestStat(index));

        CB_Species.SelectedValue = (int)painting.Species;
        NUD_Caption.Value = painting.GetCaptionRelative(index);
        TB_TID.Text = painting.TID.ToString();
        TB_SID.Text = painting.SID.ToString();
        TB_PID.Text = painting.PID.ToString("X8");
        TB_Nickname.Text = painting.Nickname;
        TB_OT.Text = painting.OT;

        PaintingIndex = index;

        NUD_Painting.BackColor = ContestColor.GetColor(index);
    }

    private void SavePainting(int index)
    {
        if ((uint)index >= 5)
            return;
        var gallery = (IGen3Hoenn)SAV;
        var painting = gallery.GetPainting(index);

        var enabled = CHK_EnablePaint.Checked;
        SAV.SetEventFlag(Paintings3.GetFlagIndexContestStat(index), enabled);
        if (!enabled)
        {
            painting.Clear();
            gallery.SetPainting(index, painting);
            return;
        }

        painting.Species = (ushort)WinFormsUtil.GetIndex(CB_Species);
        painting.SetCaptionRelative(index, (byte)NUD_Caption.Value);
        painting.TID = (ushort)Util.ToUInt32(TB_TID.Text);
        painting.SID = (ushort)Util.ToUInt32(TB_SID.Text);
        painting.PID = Util.GetHexValue(TB_PID.Text);
        painting.Nickname = TB_Nickname.Text;
        painting.OT = TB_OT.Text;

        gallery.SetPainting(index, painting);
    }

    private void CHK_EnablePaint_CheckedChanged(object sender, EventArgs e) => GB_Painting.Visible = CHK_EnablePaint.Checked;

    private void TB_PaintingIDChanged(object sender, EventArgs e)
    {
        ValidatePaintingIDs();

        var pid = Util.GetHexValue(TB_PID.Text);
        var tid = Util.ToUInt32(TB_TID.Text);
        var sid = Util.ToUInt32(TB_SID.Text);
        CHK_Shiny.Checked = ShinyUtil.GetIsShiny3((sid << 16) | tid, pid);
    }

    private void ValidatePaintingIDs()
    {
        var pid = Util.GetHexValue(TB_PID.Text);
        if (pid.ToString("X") != TB_PID.Text && pid.ToString("X8") != TB_PID.Text)
            TB_PID.Text = pid.ToString();

        var tid = Util.ToUInt32(TB_TID.Text);
        if (tid > ushort.MaxValue)
            tid = ushort.MaxValue;
        if (tid.ToString() != TB_TID.Text)
            TB_TID.Text = tid.ToString();

        var sid = Util.ToUInt32(TB_SID.Text);
        if (sid > ushort.MaxValue)
            sid = ushort.MaxValue;
        if (sid.ToString() != TB_SID.Text)
            TB_SID.Text = sid.ToString();
    }
    #endregion
}
