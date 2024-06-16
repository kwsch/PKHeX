using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;
using static System.Buffers.Binary.BinaryPrimitives;
using static PKHeX.WinForms.PoketchDotMatrix;

namespace PKHeX.WinForms;

public partial class SAV_Misc4 : Form
{
    private readonly SAV4 Origin;
    private readonly SAV4 SAV;
    private readonly Hall4? Hall;
    private readonly Record4 Record;

    private readonly string[] seals, accessories, backdrops, poketchapps;
    private readonly string[] backdropsSorted;

    public SAV_Misc4(SAV4 sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (SAV4)(Origin = sav).Clone();

        seals = GameInfo.Strings.seals;
        accessories = GameInfo.Strings.accessories;
        backdrops = GameInfo.Strings.backdrops;
        poketchapps = GameInfo.Strings.poketchapps;
        backdropsSorted = [.. backdrops.OrderBy(z => z)]; // sorted copy

        StatNUDA = [NUD_Stat0, NUD_Stat1, NUD_Stat2, NUD_Stat3];
        StatLabelA = [L_Stat0, L_Stat1, L_Stat2, L_Stat3]; // Current, Trade, Record, Trade
        StatRBA = [RB_Stats3_01, RB_Stats3_02];
        HallNUDA =
        [
            NUD_HallType01, NUD_HallType02, NUD_HallType03, NUD_HallType04, NUD_HallType05, NUD_HallType06,
            NUD_HallType07, NUD_HallType08, NUD_HallType09, NUD_HallType10, NUD_HallType11, NUD_HallType12,
            NUD_HallType13, NUD_HallType14, NUD_HallType15, NUD_HallType16, NUD_HallType17,
        ];
        PrintButtonA = [BTN_PrintTower, BTN_PrintFactory, BTN_PrintHall, BTN_PrintCastle, BTN_PrintArcade];

        Record = SAV.Records;
        switch (sav)
        {
            case SAV4DP:
                L_CurrentMap.Visible = CB_UpgradeMap.Visible = false;
                GB_Prints.Visible = GB_Prints.Enabled = GB_Hall.Visible = GB_Hall.Enabled = GB_Castle.Visible = GB_Castle.Enabled = false;
                BFF = [
                    [0, 1, 0x5FCA, 0x04, 0x6601],
                ];
                break;
            case SAV4Pt:
                L_CurrentMap.Visible = CB_UpgradeMap.Visible = false;
                PrintIndexStart = 79;
                BFF = [
                    [0, 1, 0x68E0, 0x04, 0x723D],
                    [1, 0, 0x68F4, 0x10, 0x7EF8],
                    [0, 0, 0x6924, 0x18, 0x7EFC],
                    [2, 0, 0x696C, 0x10, 0x7F00],
                    [0, 0, 0x699C, 0x04, 0x7F04],
                ];
                Hall = SAV.GetHall();
                break;
            case SAV4HGSS:
                GB_Poketch.Visible = false;
                PrintIndexStart = 77;
                BFF = [
                    // { BFV, BFT, addr, 1BFTlen, checkBit
                    [0, 1, 0x5264, 0x04, 0x5BC1],
                    [1, 0, 0x5278, 0x10, 0x687C],
                    [0, 0, 0x52A8, 0x18, 0x6880],
                    [2, 0, 0x52F0, 0x10, 0x6884],
                    [0, 0, 0x5320, 0x04, 0x6888],
                ];
                Hall = SAV.GetHall();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(sav), sav, null);
        }
        ReadMain();
        ReadBattleFrontier();
        if (SAV is SAV4Sinnoh s)
        {
            TC_Misc.Controls.Remove(TAB_Walker);
            poffinCase4Editor1.Initialize(s);
            TC_Misc.Controls.Remove(Tab_PokeGear);
        }
        else if (SAV is SAV4HGSS hgss)
        {
            pokeGear4Editor1.Initialize(hgss);
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

    private void B_Cancel_Click(object sender, EventArgs e) => Close();

    private const int FlyFlagStart = 2480;
    private static ReadOnlySpan<byte> FlyWorkFlagSinnoh => [000, 001, 002, 003, 004, 005, 006, 007, 008, 009, 010, 011, 012, 013, 014, 015, 016, 017,   067, 068];
    private static ReadOnlySpan<byte> LocationIDsSinnoh => [001, 002, 003, 004, 005, 082, 083, 006, 007, 008, 009, 010, 011, 012, 013, 014, 054, 081,   055, 015];
    private static ReadOnlySpan<byte> FlyWorkFlagHGSS   => [000, 001, 002, 003, 004, 005, 006, 007, 008, 009, 010, 011, 012, 013, 014, 015, 016, 017, 018, 019, 020, 021, 022,   027, 030, 033, 035];
    private static ReadOnlySpan<byte> LocationIDsHGSS   => [138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137,   229, 227, 221, 225];

    private void ReadMain()
    {
        NUD_Coin.Maximum = SAV.MaxCoins;
        NUD_Coin.Value = Math.Clamp(SAV.Coin, 0, SAV.MaxCoins);
        NUD_BP.Value = Math.Clamp(SAV.BP, 0, 9999);

        var locations = SAV is SAV4Sinnoh ? LocationIDsSinnoh : LocationIDsHGSS;
        var flags = SAV is SAV4Sinnoh ? FlyWorkFlagSinnoh : FlyWorkFlagHGSS;

        CLB_FlyDest.Items.Clear();
        for (int i = 0; i < locations.Length; i++)
        {
            var flagIndex = FlyFlagStart + flags[i];
            var state = SAV.GetEventFlag(flagIndex);

            var locationID = locations[i];
            var name = GameInfo.Strings.Gen4.Met0[locationID];
            CLB_FlyDest.Items.Add(name, state);
        }

        if (SAV is SAV4Sinnoh sinnoh)
        {
            ReadPoketch(sinnoh);
            NUD_UGFlags.Value = Math.Clamp(sinnoh.UG_Flags, 0, 999_999);
            L_PokeathlonPoints.Visible = NUD_PokeathlonPoints.Visible = false;
        }
        else if (SAV is SAV4HGSS hgss)
        {
            ReadWalker(hgss);
            ReadPokeathlon(hgss);
            L_UGFlags.Visible = NUD_UGFlags.Visible = false;
            ReadOnlySpan<string> items = ["Map Johto", "Map Johto+", "Map Johto & Kanto"];
            var index = hgss.MapUnlockState;
            if (index >= MapUnlockState4.Invalid)
                index = MapUnlockState4.JohtoKanto;
            foreach (var item in items)
                CB_UpgradeMap.Items.Add(item);
            CB_UpgradeMap.SelectedIndex = (int)index;
        }

        ReadSeals();
        ReadAccessories();
        ReadBackdrops();
        ReadRecord();
    }

    private void SaveMain()
    {
        SAV.Coin = (uint)NUD_Coin.Value;
        SAV.BP = (ushort)NUD_BP.Value;

        var flags = SAV is SAV4Sinnoh ? FlyWorkFlagSinnoh : FlyWorkFlagHGSS;
        for (int i = 0; i < CLB_FlyDest.Items.Count; i++)
        {
            var index = FlyFlagStart + flags[i];
            SAV.SetEventFlag(index, CLB_FlyDest.GetItemChecked(i));
        }

        if (SAV is SAV4Sinnoh sinnoh)
        {
            SavePoketch(sinnoh);
            sinnoh.UG_Flags = (uint)NUD_UGFlags.Value;
        }
        else if (SAV is SAV4HGSS hgss)
        {
            SaveWalker(hgss);
            SavePokeathlon(hgss);
            hgss.MapUnlockState = (MapUnlockState4)CB_UpgradeMap.SelectedIndex;
        }

        SaveSeals();
        SaveAccessories();
        SaveBackdrops();
        SaveRecord();
    }

    private void B_AllFlyDest_Click(object sender, EventArgs e)
    {
        for (int i = 0; i < CLB_FlyDest.Items.Count; i++)
            CLB_FlyDest.SetItemChecked(i, true);
    }

    #region Poketch
    private byte[] DotArtistByte = [];

    private void ReadPoketch(SAV4Sinnoh s)
    {
        CB_CurrentApp.Items.Clear();
        CLB_Poketch.Items.Clear();
        for (PoketchApp i = 0; i <= PoketchApp.Alarm_Clock; i++)
        {
            var name = poketchapps[(int)i];
            var title = $"{(int)i:00} - {name}";
            CB_CurrentApp.Items.Add(name);
            var value = s.GetPoketchAppUnlocked(i);
            CLB_Poketch.Items.Add(title, value);
        }
        CB_CurrentApp.SelectedIndex = s.CurrentPoketchApp;

        DotArtistByte = s.GetPoketchDotArtistData();
        SetPictureBoxFromFlags(DotArtistByte);
        const string tip = """
                           Guide about D&D ImageFile Format
                            width = 24px
                            height = 20px
                            used color count <= 4
                            file size < 2058byte
                           """;
        tip1.SetToolTip(PB_DotArtist, tip);
        TAB_Main.AllowDrop = true;
    }

    private void SavePoketch(SAV4Sinnoh s)
    {
        int unlockedCount = 0;
        s.CurrentPoketchApp = (sbyte)CB_CurrentApp.SelectedIndex;
        for (int i = 0; i < CLB_Poketch.Items.Count; i++)
        {
            var b = CLB_Poketch.GetItemChecked(i);
            s.SetPoketchAppUnlocked((PoketchApp)i, b);
            if (b) unlockedCount++;
        }
        s.SetPoketchDotArtistData(DotArtistByte);
        s.PoketchUnlockedCount = (byte)unlockedCount;
    }

    private void SetPictureBoxFromFlags(ReadOnlySpan<byte> inp)
    {
        if (inp.Length != DotMatrixPixelCount / 4)
            return;
        PB_DotArtist.Image = GetDotArt(inp);
    }

    private void SetFlagsFromFileName(string fileName)
    {
        var dest = DotArtistByte;
        TryBuild(fileName, dest);
    }

    private void SetFlagsFromClickPoint(int inpX, int inpY)
    {
        inpX = Math.Clamp(inpX, 0, (DotMatrixWidth * DotMatrixUpscaleFactor) - 1);
        inpY = Math.Clamp(inpY, 0, (DotMatrixHeight * DotMatrixUpscaleFactor) - 1);
        int i = (inpX >> 2) + (DotMatrixWidth * (inpY >> 2));
        Span<byte> ndab = stackalloc byte[DotMatrixPixelCount / 4];
        DotArtistByte.AsSpan().CopyTo(ndab);

        byte c = (byte)((ndab[i >> 2] >> ((i % 4) << 1)) & 3);
        if (++c >= 4)
            c = 0;

        ndab[i >> 2] &= (byte)~(3 << ((i % 4) << 1));
        ndab[i >> 2] |= (byte)((c & 3) << ((i % 4) << 1));

        ndab.CopyTo(DotArtistByte);
    }

    private void B_GiveAll_Click(object sender, EventArgs e)
    {
        // foreach (CheckBox c in Apps) c.Checked = true;
        for (int i = 0; i < CLB_Poketch.Items.Count; i++)
            CLB_Poketch.SetItemChecked(i, true);
        System.Media.SystemSounds.Asterisk.Play();
    }

    private void TAB_Poketch_DragEnter(object? sender, DragEventArgs? e)
    {
        if (e?.Data is null)
            return;
        if (TAB_Main.AllowDrop && e.Data.GetDataPresent(DataFormats.FileDrop))
            e.Effect = DragDropEffects.Copy;
        else
            e.Effect = DragDropEffects.None;
    }

    private void TAB_Poketch_DragDrop(object? sender, DragEventArgs? e)
    {
        if (!TAB_Main.AllowDrop)
            return;
        if (e?.Data?.GetData(DataFormats.FileDrop) is not string[] { Length: not 0 } files)
            return;
        SetFlagsFromFileName(files[0]);
        SetPictureBoxFromFlags(DotArtistByte);
    }

    private void PB_DotArtist_MouseClick(object sender, MouseEventArgs e)
    {
        SetFlagsFromClickPoint(e.X, e.Y);
        SetPictureBoxFromFlags(DotArtistByte);
    }
    #endregion

    #region BattleFrontier
    private readonly RadioButton[] StatRBA;
    private readonly NumericUpDown[] StatNUDA;
    private readonly Label[] StatLabelA;
    private readonly NumericUpDown[] HallNUDA;
    private readonly Button[] PrintButtonA;
    private readonly int[][] BFF;
    private readonly int PrintIndexStart;

    private bool editing;
    private string[][] BFT = null!;
    private int[][] BFV = null!;
    private bool HallStatUpdated;

    private void ReadBattleFrontier()
    {
        BFV = [
            [2, 0], // Max, Current
            [2, 0, 3, 1], // Max, Current, Max(Trade), Current(Trade)
            [2, 0, 1, -1, 3], // Max, Current, Current(CP), (UsedCP), Max(CP)
        ];
        BFT = [
            ["Singles", "Doubles", "Multi"],
            ["Singles", "Doubles", "Multi (Trainer)", "Multi (Friend)", "Wi-Fi"],
        ];

        if (SAV is not SAV4DP)
        {
            SetPrintColors(PrintButtonA);

            ReadOnlySpan<string> typeNames = GameInfo.Strings.types;
            ReadOnlySpan<byte> typenameIndex = [0, 9, 10, 12, 11, 14, 1, 3, 4, 2, 13, 6, 5, 7, 15, 16, 8];
            for (int i = 0; i < HallNUDA.Length; i++)
                tip2.SetToolTip(HallNUDA[i], typeNames[typenameIndex[i]]);
        }
        if (Hall is null)
            NUD_HallStreaks.Visible = NUD_HallStreaks.Enabled = false;

        editing = true;
        CB_Stats1.Items.Clear();
        for (BattleFrontierFacility4 i = 0; i <= SAV.MaxFacility; i++)
            CB_Stats1.Items.Add(i.ToString());
        StatRBA[0].Checked = true;

        // Clear Listbox and ComboBox
        CB_Species.Items.Clear();

        // Fill List
        CB_Species.InitializeBinding();

        var speciesList = GameInfo.FilteredSources.Species.Skip(1).ToList();
        CB_Species.DataSource = new BindingSource(speciesList, null);

        editing = false;
        CB_Stats1.SelectedIndex = 0;
    }

    private void SaveBattleFrontier()
    {
        if (HallStatUpdated)
            Hall?.RefreshChecksum();
    }

    private void SetPrintColors(ReadOnlySpan<Control> controls)
    {
        for (int i = 0; i < controls.Length; i++)
        {
            var pb = controls[i];
            var workIndex = PrintIndexStart + i;
            var value = SAV.GetWork(workIndex);
            SetPrintColor(pb, (BattleFrontierPrintStatus4)value);
        }
    }

    private static void SetPrintColor(Control pb, BattleFrontierPrintStatus4 value)
    {
        bool ready = value is BattleFrontierPrintStatus4.FirstReady or BattleFrontierPrintStatus4.SecondReady;
        if (ready)
            pb.ForeColor = Color.Red;
        else if (value != 0)
            pb.ForeColor = Color.Green;
        else
            pb.ResetForeColor();

        if (value is BattleFrontierPrintStatus4.FirstReady or BattleFrontierPrintStatus4.FirstReceived)
            pb.BackColor = Color.Silver;
        else if (value is BattleFrontierPrintStatus4.SecondReady or BattleFrontierPrintStatus4.SecondReceived)
            pb.BackColor = Color.Gold;
        else
            pb.ResetBackColor();
    }

    private void BTN_Print_Click(object sender, EventArgs e)
    {
        if (sender is not Button b)
            return;
        int index = Array.IndexOf(PrintButtonA, b);
        if (index < 0)
            return;
        index += PrintIndexStart;
        var current = SAV.GetWork(index);
        current++;
        if (current > (int)BattleFrontierPrintStatus4.SecondReceived)
            current = 0;
        SAV.SetWork(index, current);

        SetPrintColor(b, (BattleFrontierPrintStatus4)current);
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
            if (StatNUDA[1].Value > 9999)
                StatNUDA[1].Value = 9999;
            StatNUDA[1].Maximum = 9999;
        }

        if (facility == 1)
            StatLabelA[1].Text = StatLabelA[3].Text = "Trade";
        else if (facility == 3)
            StatLabelA[1].Text = StatLabelA[3].Text = "CP";

        GB_Hall.Visible = facility == 2;
        GB_Castle.Visible = facility == 3;

        editing = false;
        CB_Stats2.SelectedIndex = 0;
    }

    private void ChangeStat(object sender, EventArgs e)
    {
        if (editing)
            return;
        if (sender is RadioButton { Checked: false })
            return;
        StatAddrControl(SetValToSav: -2, SetSavToVal: true);
        if (GB_Hall.Visible && CB_Stats2.SelectedItem is string sH)
        {
            GB_Hall.Text = $"Battle Hall ({sH})";
            editing = true;
            GetHallStat();
            editing = false;
        }
        else if (GB_Castle.Visible && CB_Stats2.SelectedItem is string sC)
        {
            GB_Castle.Text = $"Battle Castle ({sC})";
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
        byte maskFlag = (byte)(1 << (BattleType + (RBi << 2)));
        int TowerContinueCountOfs = SAV is SAV4DP ? 3 : 1;

        var general = SAV.General;
        if (SetSavToVal)
        {
            editing = true;
            for (int i = 0; i < BFV[BFF[Facility][0]].Length; i++)
            {
                if (BFV[BFF[Facility][0]][i] < 0)
                    continue;
                int vali = ReadUInt16LittleEndian(general[(addrVal + (i << 1))..]);
                StatNUDA[BFV[BFF[Facility][0]][i]].Value = vali > 9999 ? 9999 : vali;
            }
            CHK_Continue.Checked = (SAV.General[addrFlag] & maskFlag) != 0;

            if (Facility == 0) // tower continue count
                StatNUDA[1].Value = ReadUInt16LittleEndian(general[(addrFlag + TowerContinueCountOfs + (BattleType << 1))..]);

            editing = false;
            return;
        }
        if (SetValToSav >= 0)
        {
            ushort val = (ushort)StatNUDA[SetValToSav].Value;

            if (Facility == 0 && SetValToSav == 1) // tower continue count
            {
                var offset = addrFlag + TowerContinueCountOfs + (BattleType << 1);
                WriteUInt16LittleEndian(general[offset..], val);
            }

            SetValToSav = Array.IndexOf(BFV[BFF[Facility][0]], SetValToSav);
            if (SetValToSav < 0)
                return;
            var clamp = Math.Min((ushort)9999, val);
            WriteUInt16LittleEndian(general[(addrVal + (SetValToSav << 1))..], clamp);
            return;
        }
        if (SetValToSav == -1)
        {
            if (CHK_Continue.Checked)
            {
                general[addrFlag] |= maskFlag;
                if (Facility == 3)
                    general[addrFlag + 1] |= 0x01; // not found what this flag means
            }
            else
            {
                general[addrFlag] &= (byte)~maskFlag;
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

        if (CB_Stats1.SelectedIndex != 0)
            return;

        const int bias = 7;
        var n0 = StatNUDA[0];
        var n1 = StatNUDA[1];
        if (Math.Floor(n0.Value / bias) == n1.Value)
            return;

        if (n == 0)
        {
            n1.Value = Math.Floor(n0.Value / bias);
        }
        else if (n == 1)
        {
            if (n0.Maximum > n1.Value * bias)
                n0.Value = n1.Value * bias;
            else if (n0.Value < n0.Maximum)
                n0.Value = n0.Maximum;
        }
    }

    private void CHK_Continue_CheckedChanged(object sender, EventArgs e)
    {
        if (editing)
            return;
        StatAddrControl(SetValToSav: -1, SetSavToVal: false);
    }

    private ushort species = ushort.MaxValue;

    private void ChangeSpecies(object sender, EventArgs e)
    {
        species = (ushort)WinFormsUtil.GetIndex(CB_Species);
        if (editing)
            return;

        editing = true;
        GetHallStat();
        editing = false;
    }

    private void GetCastleStat()
    {
        int ofs = BFF[3][2] + (BFF[3][3] * CB_Stats2.SelectedIndex) + 0x0A;
        NumericUpDown[] na = [NUD_CastleRankRcv, NUD_CastleRankItem, NUD_CastleRankInfo];
        for (int i = 0; i < na.Length; i++)
        {
            int val = ReadInt16LittleEndian(SAV.General[(ofs + (i << 1))..]);
            na[i].Value = val > na[i].Maximum ? na[i].Maximum : val < na[i].Minimum ? na[i].Minimum : val;
        }
    }

    private void NUD_CastleRank_ValueChanged(object sender, EventArgs e)
    {
        if (editing)
            return;
        NumericUpDown[] na = [NUD_CastleRankRcv, NUD_CastleRankItem, NUD_CastleRankInfo];
        int i = Array.IndexOf(na, sender);
        if (i < 0)
            return;
        var offset = BFF[3][2] + (BFF[3][3] * CB_Stats2.SelectedIndex) + 0x0A + (i << 1);
        WriteInt32LittleEndian(SAV.General[offset..], (int)na[i].Value);
    }

    private void GetHallStat()
    {
        int ofscur = BFF[2][2] + (BFF[2][3] * CB_Stats2.SelectedIndex);
        var curspe = ReadUInt16LittleEndian(SAV.General[(ofscur + 4)..]);
        bool c = curspe == species;
        CHK_HallCurrent.Checked = c;
        CHK_HallCurrent.Text = curspe > 0 && curspe <= SAV.MaxSpeciesID
            ? $"Current: {SpeciesName.GetSpeciesName(curspe, GameLanguage.GetLanguageIndex(Main.CurrentLanguage))}"
            : "Current: (None)";

        int s = 0;
        for (int i = 0; i < HallNUDA.Length; i++)
        {
            var d = c ? Math.Min(10, (SAV.General[ofscur + 6 + ((i >> 1) << 1)] >> ((i & 1) << 2)) & 0x0F) : 0;
            HallNUDA[i].Value = d;
            HallNUDA[i].Enabled = c;
            s += d;
        }
        L_SumHall.Text = s.ToString();

        if (Hall is not null)
        {
            ushort v = Hall.GetCount(CB_Stats2.SelectedIndex, species);
            NUD_HallStreaks.Value = Math.Min((ushort)9999, v);
        }
    }

    private void CHK_HallCurrent_CheckedChanged(object sender, EventArgs e)
    {
        if (editing)
            return;
        var offset = BFF[2][2] + (BFF[2][3] * CB_Stats2.SelectedIndex) + 4;
        ushort value = (ushort)(CHK_HallCurrent.Checked ? species : 0);
        WriteUInt16LittleEndian(SAV.General[offset..], value);
        editing = true;
        GetHallStat();
        editing = false;
    }

    private void NUD_HallType_ValueChanged(object sender, EventArgs e)
    {
        if (editing)
            return;
        int i = Array.IndexOf(HallNUDA, sender);
        if (i < 0)
            return;
        int ofs = BFF[2][2] + (BFF[2][3] * CB_Stats2.SelectedIndex) + 6 + ((i >> 1) << 1);
        SAV.General[ofs] = (byte)((SAV.General[ofs] & ~(0xF << ((i & 1) << 2))) | ((int)HallNUDA[i].Value << ((i & 1) << 2)));
        L_SumHall.Text = HallNUDA.Sum(x => x.Value).ToString(CultureInfo.InvariantCulture);
    }

    private void NUD_HallStreaks_ValueChanged(object sender, EventArgs e)
    {
        if (editing || Hall is null)
            return;
        Hall.SetCount(CB_Stats2.SelectedIndex, species, (ushort)NUD_HallStreaks.Value);
        HallStatUpdated = true;
    }
    #endregion

    #region Walker
    private void ReadWalker(SAV4HGSS s)
    {
        ReadOnlySpan<string> walkercourses = GameInfo.Sources.Strings.walkercourses;
        CLB_WalkerCourses.Items.Clear();
        foreach (var name in walkercourses)
            CLB_WalkerCourses.Items.Add(name);

        ReadWalkerCourseUnlockFlags(s);

        NUD_Watts.Value = s.PokewalkerWatts;
        NUD_Steps.Value = s.PokewalkerSteps;
    }

    private void ReadWalkerCourseUnlockFlags(SAV4HGSS s)
    {
        Span<bool> courses = stackalloc bool[SAV4HGSS.PokewalkerCourseFlagCount];
        s.GetPokewalkerCoursesUnlocked(courses);
        for (int i = 0; i < CLB_WalkerCourses.Items.Count; i++)
            CLB_WalkerCourses.SetItemChecked(i, courses[i]);
    }

    private void SaveWalker(SAV4HGSS s)
    {
        Span<bool> courses = stackalloc bool[SAV4HGSS.PokewalkerCourseFlagCount];
        for (int i = 0; i < CLB_WalkerCourses.Items.Count; i++)
            courses[i] = CLB_WalkerCourses.GetItemChecked(i);
        s.SetPokewalkerCoursesUnlocked(courses);

        s.PokewalkerWatts = (uint)NUD_Watts.Value;
        s.PokewalkerSteps = (uint)NUD_Steps.Value;
    }

    private void B_AllWalkerCourses_Click(object sender, EventArgs e)
    {
        if (SAV is not SAV4HGSS s)
            throw new Exception("Invalid SAV type");
        s.PokewalkerCoursesUnlockAll();
        ReadWalkerCourseUnlockFlags(s);
    }
    #endregion

    private void ReadPokeathlon(SAV4HGSS s)
    {
        NUD_PokeathlonPoints.Value = s.PokeathlonPoints;
    }

    private void SavePokeathlon(SAV4HGSS s)
    {
        s.PokeathlonPoints = (uint)NUD_PokeathlonPoints.Value;
    }

    #region Seals
    private void ReadSeals()
    {
        DGV_Seals.Rows.Clear();
        DGV_Seals.Columns.Clear();

        DataGridViewTextBoxColumn dgvSlot = new()
        {
            HeaderText = "Slot",
            DisplayIndex = 0,
            Width = 145,
            ReadOnly = true,
        };
        DataGridViewTextBoxColumn dgvCount = new()
        {
            DisplayIndex = 1,
            Width = 45,
            DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter },
            MaxInputLength = 2, // 0-99
        };
        DGV_Seals.Columns.Add(dgvSlot);
        DGV_Seals.Columns.Add(dgvCount);

        const int count = (int)Seal4.MAX;
        DGV_Seals.Rows.Add(count);
        for (int i = 0; i < count; i++)
            DGV_Seals.Rows[i].Cells[0].Value = seals[i];
        LoadSealsCount();
    }

    private void LoadSealsCount()
    {
        for (int i = 0; i < (int)Seal4.MAX; i++)
            DGV_Seals.Rows[i].Cells[1].Value = SAV.GetSealCount((Seal4)i).ToString();
    }

    public void ClearSeals()
    {
        for (int i = 0; i < (int)Seal4.MAX; i++)
            DGV_Seals.Rows[i].Cells[1].Value = "0";
    }

    public void SetAllSeals(bool unreleased = false)
    {
        var sealIndexCount = (int)(unreleased ? Seal4.MAX : Seal4.MAXLEGAL);
        for (int i = 0; i < sealIndexCount; i++)
            DGV_Seals.Rows[i].Cells[1].Value = SAV4.SealMaxCount.ToString();
    }

    private void SaveSeals()
    {
        for (int i = 0; i < (int)Seal4.MAX; i++)
        {
            var cells = DGV_Seals.Rows[i].Cells;
            var count = int.TryParse(cells[1].Value?.ToString() ?? "0", out var val) ? val : 0;
            SAV.SetSealCount((Seal4)i, (byte)Math.Clamp(count, 0, byte.MaxValue));
        }
    }

    private void B_ClearSeals_Click(object sender, EventArgs e) => ClearSeals();

    private void OnBAllSealsLegalOnClick(object sender, EventArgs e)
    {
        bool setUnreleasedIndexes = sender == B_AllSealsIllegal;
        SetAllSeals(setUnreleasedIndexes);
        System.Media.SystemSounds.Asterisk.Play();
    }
    #endregion

    #region Accessories
    private void ReadAccessories()
    {
        DGV_Accessories.Rows.Clear();
        DGV_Accessories.Columns.Clear();

        DataGridViewTextBoxColumn dgvSlot = new()
        {
            HeaderText = "Slot",
            DisplayIndex = 0,
            Width = 140,
            ReadOnly = true,
        };
        DataGridViewTextBoxColumn dgvCount = new()
        {
            DisplayIndex = 1,
            Width = 50,
            DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter },
            MaxInputLength = 1, // 0-9
        };
        DGV_Accessories.Columns.Add(dgvSlot);
        DGV_Accessories.Columns.Add(dgvCount);

        const int count = AccessoryInfo.Count;
        DGV_Accessories.Rows.Add(count);
        for (int i = 0; i < count; i++)
            DGV_Accessories.Rows[i].Cells[0].Value = accessories[i];
        LoadAccessoriesCount();
    }

    private void LoadAccessoriesCount()
    {
        for (int i = 0; i < AccessoryInfo.Count; i++)
            DGV_Accessories.Rows[i].Cells[1].Value = SAV.GetAccessoryOwnedCount((Accessory4)i).ToString();
    }

    public void ClearAccessories()
    {
        for (int i = 0; i < AccessoryInfo.Count; i++)
            DGV_Accessories.Rows[i].Cells[1].Value = "0";
    }

    public void SetAllAccessories(bool unreleased = false)
    {
        for (int i = 0; i <= AccessoryInfo.MaxMulti; i++)
            DGV_Accessories.Rows[i].Cells[1].Value = AccessoryInfo.AccessoryMaxCount.ToString();

        var count = unreleased ? AccessoryInfo.Count : (AccessoryInfo.MaxLegal + 1);
        for (int i = AccessoryInfo.MaxMulti + 1; i < count; i++)
            DGV_Accessories.Rows[i].Cells[1].Value = "1";
    }

    private void SaveAccessories()
    {
        for (int i = 0; i < AccessoryInfo.Count; i++)
        {
            var cells = DGV_Accessories.Rows[i].Cells;
            var count = int.TryParse(cells[1].Value?.ToString() ?? "0", out var val) ? val : 0;
            SAV.SetAccessoryOwnedCount((Accessory4)i, (byte)Math.Clamp(count, 0, byte.MaxValue));
        }
    }

    private void B_ClearAccessories_Click(object sender, EventArgs e) => ClearAccessories();

    private void OnBAllAccessoriesLegalOnClick(object sender, EventArgs e)
    {
        bool setUnreleasedIndexes = sender == B_AllAccessoriesIllegal;
        SetAllAccessories(setUnreleasedIndexes);
        System.Media.SystemSounds.Asterisk.Play();
    }
    #endregion

    #region Backdrops
    private void ReadBackdrops()
    {
        DGV_Backdrops.Rows.Clear();
        DGV_Backdrops.Columns.Clear();

        DataGridViewComboBoxColumn dgv = new()
        {
            HeaderText = "Slot",
            DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
            DisplayIndex = 0,
            Width = 190,
            FlatStyle = FlatStyle.Flat,
            DataSource = new BindingSource(backdropsSorted, null),
        };
        DGV_Backdrops.Columns.Add(dgv);

        LoadBackdropPositions();
    }

    private void LoadBackdropPositions()
    {
        const int count = BackdropInfo.Count;
        DGV_Backdrops.Rows.Add(count);
        ClearBackdrops();

        for (int i = 0; i < count; i++)
        {
            var pos = SAV.GetBackdropPosition((Backdrop4)i);
            if (pos < BackdropInfo.Count)
                DGV_Backdrops.Rows[pos].Cells[0].Value = backdrops[i];
        }
    }

    private void ClearBackdrops()
    {
        for (int i = 0; i < BackdropInfo.Count; i++)
            DGV_Backdrops.Rows[i].Cells[0].Value = backdrops[(int)Backdrop4.Unset];
    }

    public void SetAllBackdrops(bool unreleased = false)
    {
        var count = unreleased ? BackdropInfo.Count : ((int)BackdropInfo.MaxLegal + 1);
        for (int i = 0; i < count; i++)
            DGV_Backdrops.Rows[i].Cells[0].Value = backdrops[i];
    }

    private void SaveBackdrops()
    {
        for (int i = 0; i < BackdropInfo.Count; i++)
            SAV.RemoveBackdrop((Backdrop4)i); // clear all slots

        byte ctr = 0;
        for (int i = 0; i < BackdropInfo.Count; i++)
        {
            Backdrop4 bd = (Backdrop4)Array.IndexOf(backdrops, DGV_Backdrops.Rows[i].Cells[0].Value);
            if (bd.IsUnset()) // skip empty slots
                continue;

            SAV.SetBackdropPosition(bd, ctr);
            ctr++;
        }
    }

    private void B_ClearBackdrops_Click(object sender, EventArgs e) => ClearBackdrops();

    private void OnBAllBackdropsLegalOnClick(object sender, EventArgs e)
    {
        bool setUnreleasedIndexes = sender == B_AllBackdropsIllegal;
        SetAllBackdrops(setUnreleasedIndexes);
        System.Media.SystemSounds.Asterisk.Play();
    }
    #endregion

    #region Records
    private void ReadRecord()
    {
        NUD_Record16.Maximum = Record4.Record16 - 1;
        NUD_Record32.Maximum = Record.Record32 - 1;
        NUD_Record16V.Value = Record.GetRecord16(0);
        NUD_Record32V.Value = Record.GetRecord32(0);
        NUD_Record16V.ValueChanged += (_, _) => Record.SetRecord16((int)NUD_Record16.Value, (ushort)NUD_Record16V.Value);
        NUD_Record32V.ValueChanged += (_, _) => Record.SetRecord32((int)NUD_Record32.Value, (uint)NUD_Record32V.Value);
        NUD_Record16.ValueChanged += (_, _) => NUD_Record16V.Value = Record.GetRecord16((int)NUD_Record16.Value);
        NUD_Record32.ValueChanged += (_, _) => NUD_Record32V.Value = Record.GetRecord32((int)NUD_Record32.Value);
    }

    private void SaveRecord() => Record.EndAccess();
    #endregion
}

public static class PoketchDotMatrix
{
    public const int DotMatrixHeight = 20;
    public const int DotMatrixWidth = 24;
    public const int DotMatrixPixelCount = DotMatrixHeight * DotMatrixWidth;

    public static ReadOnlySpan<byte> ColorTable => [248, 168, 88, 8];

    public static bool TryBuild(string fileName, Span<byte> result)
    {
        if (FileUtil.GetFileSize(fileName) > 0x80A)
            return false;

        using var bmp = (Bitmap)Image.FromFile(fileName);
        if (bmp.Width != DotMatrixWidth || bmp.Height != DotMatrixHeight)
            return false;

        Span<byte> brightMap = stackalloc byte[DotMatrixPixelCount];
        Span<byte> brightCount = stackalloc byte[0x100];
        if (!TryBuildBrightMap(bmp, brightMap, brightCount, out int colorCount))
            return false;

        Build(colorCount, brightCount, brightMap, result);
        return true;
    }

    private static bool TryBuildBrightMap(Bitmap bmp, Span<byte> brightMap, Span<byte> brightCount, out int colorCount)
    {
        for (int iy = 0; iy < DotMatrixHeight; iy++) // Height
        {
            for (int ix = 0; ix < DotMatrixWidth; ix++) // Width
            {
                var ig = (byte)(0xFF * bmp.GetPixel(ix, iy).GetBrightness());
                brightMap[ix + (DotMatrixWidth * iy)] = ig;
                brightCount[ig]++;
            }
        }
        colorCount = brightCount.Length - brightCount.Count<byte>(0);
        return (colorCount - 1) <= 3; // 1-4
    }

    private static void Build(int colorCount, Span<byte> brightCount, Span<byte> brightMap, Span<byte> result)
    {
        int errorsMin = int.MaxValue;
        Span<byte> LCT = stackalloc byte[4];
        for (int i = 0; i < LCT.Length; i++)
            LCT[i] = (byte)(colorCount < i + 1 ? 4 : colorCount - i - 1);

        Span<byte> mLCT = stackalloc byte[4];
        Span<byte> iBrightCount = stackalloc byte[0x100];
        int ee = 0;
        while (++ee < 1000)
        {
            brightCount.CopyTo(iBrightCount);
            for (int i = 0, j = 0; i < iBrightCount.Length; i++)
            {
                if (iBrightCount[i] > 0)
                    iBrightCount[i] = LCT[j++];
            }

            var errorsTotal = 0;
            for (int i = 0; i < DotMatrixPixelCount; i++)
                errorsTotal += Math.Abs(brightMap[i] - ColorTable[iBrightCount[brightMap[i]]]);
            if (errorsMin > errorsTotal)
            {
                errorsMin = errorsTotal;
                LCT.CopyTo(mLCT);
            }
            GetNextLCT(LCT);
            if (LCT[0] >= 4)
                break;
        }
        for (int i = 0, j = 0; i < brightCount.Length; i++)
        {
            if (brightCount[i] > 0)
                brightCount[i] = mLCT[j++];
        }

        for (int i = 0; i < brightMap.Length; i++)
            brightMap[i] = brightCount[brightMap[i]];

        for (int i = 0; i < brightMap.Length; i++)
            result[i >> 2] |= (byte)((brightMap[i] & 3) << ((i % 4) << 1));
    }

    private static void GetNextLCT(Span<byte> inp)
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
            return;
        }
    }

    public const int DotMatrixUpscaleFactor = 4;

    public static Bitmap GetDotArt(ReadOnlySpan<byte> inp)
    {
        byte[] dupbyte = new byte[23040];
        for (int iy = 0; iy < DotMatrixHeight; iy++)
        {
            for (int ix = 0; ix < DotMatrixWidth; ix++)
            {
                var ib = ix + (DotMatrixWidth * iy);
                var ict = ColorTable[(inp[ib >> 2] >> ((ib % 4) << 1)) & 3];
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

        var dabmp = new Bitmap(DotMatrixWidth * DotMatrixUpscaleFactor, DotMatrixHeight * DotMatrixUpscaleFactor);
        var dabdata = dabmp.LockBits(new Rectangle(0, 0, dabmp.Width, dabmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
        System.Runtime.InteropServices.Marshal.Copy(dupbyte, 0, dabdata.Scan0, dupbyte.Length);
        dabmp.UnlockBits(dabdata);
        return dabmp;
    }
}
