using System;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public partial class SAV_Underground : Form
{
    private readonly SaveFile Origin;
    private readonly SAV4Sinnoh SAV;

    private const int MAX_SIZE = SAV4Sinnoh.UG_POUCH_SIZE;
    private readonly string[] ugGoods, ugSpheres, ugTraps, ugTreasures;

    private readonly string[] ugGoodsSorted;
    private readonly string[] ugTrapsSorted;
    private readonly string[] ugTreasuresSorted;

    public SAV_Underground(SAV4Sinnoh sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (SAV4Sinnoh)(Origin = sav).Clone();

        ugGoods = GameInfo.Strings.uggoods;
        ugSpheres = GameInfo.Strings.ugspheres;
        ugTraps = GameInfo.Strings.ugtraps;
        ugTreasures = GameInfo.Strings.ugtreasures;

        ugGoodsSorted = SanitizeList(ugGoods);
        ugTrapsSorted = SanitizeList(ugTraps);
        ugTreasuresSorted = SanitizeList(ugTreasures);

        InitializeDGV();
        GetUGScores();
        ReadUGData();
    }

    private void InitializeDGV()
    {
        // Goods
        DGV_UGGoods.Rows.Add(SAV4Sinnoh.UG_POUCH_SIZE);

        Item_Goods.DataSource = new BindingSource(ugGoodsSorted, string.Empty);
        Item_Goods.DisplayIndex = 0;
        DGV_UGGoods.CancelEdit();

        // Spheres
        DGV_UGSpheres.Rows.Add(MAX_SIZE);

        Item_Spheres.DataSource = new BindingSource(ugSpheres, string.Empty);
        Item_Spheres.DisplayIndex = 0;
        DGV_UGSpheres.CancelEdit();

        // Traps
        DGV_UGTraps.Rows.Add(MAX_SIZE);

        Item_Traps.DataSource = new BindingSource(ugTrapsSorted, string.Empty);
        Item_Traps.DisplayIndex = 0;
        DGV_UGTraps.CancelEdit();

        // Treasures
        DGV_UGTreasures.Rows.Add(MAX_SIZE);

        Item_Treasures.DataSource = new BindingSource(ugTreasuresSorted, string.Empty);
        Item_Treasures.DisplayIndex = 0;
        DGV_UGTreasures.CancelEdit();
    }

    private void ReadUGData()
    {
        var goodsList = SAV.GetUGI_Goods();
        var spheresList = SAV.GetUGI_Spheres();
        var trapsList = SAV.GetUGI_Traps();
        var treasuresList = SAV.GetUGI_Treasures();

        var sphereCount = spheresList[MAX_SIZE..];
        spheresList = spheresList[..MAX_SIZE];

        // Goods
        for (int i = 0; i < goodsList.Length; i++)
        {
            var goods = goodsList[i];
            if (goods >= ugGoods.Length)
                goods = 0;
            var cell = DGV_UGGoods.Rows[i].Cells;
            cell[0].Value = ugGoods[goods];
        }

        // Spheres
        for (int i = 0; i < spheresList.Length; i++)
        {
            var sphere = spheresList[i];
            var count = sphereCount[i];
            if (sphere >= ugSpheres.Length)
                sphere = count = 0;

            var cell = DGV_UGSpheres.Rows[i].Cells;
            cell[0].Value = ugSpheres[sphere];
            cell[1].Value = count;
        }

        // Traps
        for (int i = 0; i < trapsList.Length; i++)
        {
            var trap = trapsList[i];
            if (trap >= ugTraps.Length)
                trap = 0;

            var cell = DGV_UGTraps.Rows[i].Cells;
            cell[0].Value = ugTraps[trap];
        }

        // Treasures
        for (int i = 0; i < treasuresList.Length; i++)
        {
            var treasure = treasuresList[i];
            if (treasure >= ugTreasures.Length)
                treasure = 0;
            var cell = DGV_UGTreasures.Rows[i].Cells;
            cell[0].Value = ugTreasures[treasure];
        }
    }

    private void SaveUGData()
    {
        var goodsList = SAV.GetUGI_Goods();
        var spheresList = SAV.GetUGI_Spheres();
        var trapsList = SAV.GetUGI_Traps();
        var treasuresList = SAV.GetUGI_Treasures();

        goodsList.Clear();
        spheresList.Clear();
        trapsList.Clear();
        treasuresList.Clear();

        // Goods
        int ctr = 0;
        for (int i = 0; i < DGV_UGGoods.Rows.Count; i++)
        {
            var str = DGV_UGGoods.Rows[i].Cells[0].Value!.ToString();
            var itemindex = Array.IndexOf(ugGoods, str);

            if (itemindex <= 0)
                continue; // ignore empty slot

            goodsList[ctr] = (byte)itemindex;
            ctr++;
        }

        // Spheres
        ctr = 0;
        for (int i = 0; i < DGV_UGSpheres.Rows.Count; i++)
        {
            var row = DGV_UGSpheres.Rows[i];
            var str = row.Cells[0].Value!.ToString();
            var itemindex = Array.IndexOf(ugSpheres, str);

            bool success = int.TryParse(row.Cells[1].Value?.ToString(), out var itemcnt);
            if (!success || itemindex <= 0)
                continue;  // ignore empty slot or non-numeric values

            spheresList[ctr] = (byte)itemindex;
            spheresList[ctr + MAX_SIZE] = (byte)itemcnt;
            ctr++;
        }

        // Traps
        ctr = 0;
        for (int i = 0; i < DGV_UGTraps.Rows.Count; i++)
        {
            var str = DGV_UGTraps.Rows[i].Cells[0].Value!.ToString();
            var itemindex = Array.IndexOf(ugTraps, str);

            if (itemindex <= 0)
                continue; // ignore empty slot

            trapsList[ctr] = (byte)itemindex;
            ctr++;
        }

        // Treasures
        ctr = 0;
        for (int i = 0; i < DGV_UGTreasures.Rows.Count; i++)
        {
            var str = DGV_UGTreasures.Rows[i].Cells[0].Value!.ToString();
            var itemindex = Array.IndexOf(ugTreasures, str);

            if (itemindex <= 0)
                continue; // ignore empty slot

            treasuresList[ctr] = (byte)itemindex;
            ctr++;
        }
    }

    private void GetUGScores()
    {
        LoadValue(NUD_PlayersMet, SAV.UG_PeopleMet);
        LoadValue(NUD_GiftsGiven, SAV.UG_GiftsGiven);
        LoadValue(NUD_GiftsReceived, SAV.UG_GiftsReceived);
        LoadValue(NUD_Spheres, SAV.UG_Spheres);
        LoadValue(NUD_Fossils, SAV.UG_Fossils);
        LoadValue(NUD_TrapPlayers, SAV.UG_TrapPlayers);
        LoadValue(NUD_TrapSelf, SAV.UG_TrapSelf);
        LoadValue(NUD_MyBaseMoved, SAV.UG_MyBaseMoved);
        LoadValue(NUD_FlagsObtained, SAV.UG_FlagsTaken);
        LoadValue(NUD_MyFlagTaken, SAV.UG_FlagsFromMe);
        LoadValue(NUD_MyFlagRecovered, SAV.UG_FlagsRecovered);
        LoadValue(NUD_FlagsCaptured, SAV.UG_FlagsCaptured);
        LoadValue(NUD_HelpedOthers, SAV.UG_HelpedOthers);

        static void LoadValue(NumericUpDown box, uint value)
            => box.Value = Math.Clamp(value, 0, SAV4Sinnoh.UG_MAX);
    }

    private void SetUGScores()
    {
        SAV.UG_PeopleMet = (uint)NUD_PlayersMet.Value;
        SAV.UG_GiftsGiven = (uint)NUD_GiftsGiven.Value;
        SAV.UG_GiftsReceived = (uint)NUD_GiftsReceived.Value;
        SAV.UG_Spheres = (uint)NUD_Spheres.Value;
        SAV.UG_Fossils = (uint)NUD_Fossils.Value;
        SAV.UG_TrapPlayers = (uint)NUD_TrapPlayers.Value;
        SAV.UG_TrapSelf = (uint)NUD_TrapSelf.Value;
        SAV.UG_MyBaseMoved = (uint)NUD_MyBaseMoved.Value;
        SAV.UG_FlagsTaken = (uint)NUD_FlagsObtained.Value;
        SAV.UG_FlagsFromMe = (uint)NUD_MyFlagTaken.Value;
        SAV.UG_FlagsRecovered = (uint)NUD_MyFlagRecovered.Value;
        SAV.UG_FlagsCaptured = (uint)NUD_FlagsCaptured.Value;
        SAV.UG_HelpedOthers = (uint)NUD_HelpedOthers.Value;
    }

    private static string[] SanitizeList(string[] inputlist)
    {
        string[] listSorted = inputlist.Where(x => !string.IsNullOrEmpty(x)).ToArray();
        Array.Sort(listSorted);

        return listSorted;
    }

    private void B_Save_Click(object sender, EventArgs e)
    {
        SetUGScores();
        SaveUGData();

        Origin.CopyChangesFrom(SAV);
        Close();
    }

    private void B_Cancel_Click(object sender, EventArgs e) => Close();
}
