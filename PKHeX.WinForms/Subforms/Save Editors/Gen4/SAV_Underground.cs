using System;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SAV_Underground : Form
    {
        private readonly SaveFile Origin;
        private readonly SAV4Sinnoh SAV;

        private const int MAX_SIZE = SAV4Sinnoh.UG_POUCH_SIZE;
        private readonly string[] ugGoods, ugSpheres, ugTraps, ugTreasures;

        private readonly string[] ugGoodsSorted;
        private readonly string[] ugTrapsSorted;
        private readonly string[] ugTreasuresSorted;

        public SAV_Underground(SaveFile sav)
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

            Item_Goods.DataSource = new BindingSource(ugGoodsSorted, null);
            Item_Goods.DisplayIndex = 0;
            DGV_UGGoods.CancelEdit();

            // Spheres
            DGV_UGSpheres.Rows.Add(MAX_SIZE);

            Item_Spheres.DataSource = new BindingSource(ugSpheres, null);
            Item_Spheres.DisplayIndex = 0;
            DGV_UGSpheres.CancelEdit();

            // Traps
            DGV_UGTraps.Rows.Add(MAX_SIZE);

            Item_Traps.DataSource = new BindingSource(ugTrapsSorted, null);
            Item_Traps.DisplayIndex = 0;
            DGV_UGTraps.CancelEdit();

            // Treasures
            DGV_UGTreasures.Rows.Add(MAX_SIZE);

            Item_Treasures.DataSource = new BindingSource(ugTreasuresSorted, null);
            Item_Treasures.DisplayIndex = 0;
            DGV_UGTreasures.CancelEdit();
        }

        private void ReadUGData()
        {
            byte[] goodsList = SAV.GetUGI_Goods();
            byte[] spheresList = SAV.GetUGI_Spheres();
            byte[] trapsList = SAV.GetUGI_Traps();
            byte[] treasuresList = SAV.GetUGI_Treasures();

            // Goods
            for (int i = 0; i < goodsList.Length; i++)
            {
                DGV_UGGoods.Rows[i].Cells[0].Value = ugGoods[goodsList[i]];
            }

            // Spheres (split in two, first 40 positions are the sphere type, last 40 are their size)
            for (int i = 0; i < (spheresList.Length / 2); i++)
            {
                var row = DGV_UGSpheres.Rows[i];
                row.Cells[0].Value = ugSpheres[spheresList[i]];
                row.Cells[1].Value = spheresList[i + MAX_SIZE].ToString();
            }

            // Traps
            for (int i = 0; i < trapsList.Length; i++)
            {
                DGV_UGTraps.Rows[i].Cells[0].Value = ugTraps[trapsList[i]];
            }

            // Treasures
            for (int i = 0; i < treasuresList.Length; i++)
            {
                DGV_UGTreasures.Rows[i].Cells[0].Value = ugTreasures[treasuresList[i]];
            }
        }

        private void SaveUGData()
        {
            byte[] goodsList = new byte[SAV.GetUGI_Goods().Length];
            byte[] spheresList = new byte[SAV.GetUGI_Spheres().Length];
            byte[] trapsList = new byte[SAV.GetUGI_Traps().Length];
            byte[] treasuresList = new byte[SAV.GetUGI_Treasures().Length];

            // Goods
            int ctr = 0;
            for (int i = 0; i < DGV_UGGoods.Rows.Count; i++)
            {
                var str = DGV_UGGoods.Rows[i].Cells[0].Value.ToString();
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
                var str = row.Cells[0].Value.ToString();
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
                var str = DGV_UGTraps.Rows[i].Cells[0].Value.ToString();
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
                var str = DGV_UGTreasures.Rows[i].Cells[0].Value.ToString();
                var itemindex = Array.IndexOf(ugTreasures, str);

                if (itemindex <= 0)
                    continue; // ignore empty slot

                treasuresList[ctr] = (byte)itemindex;
                ctr++;
            }

            SAV.SetUGI_Goods(goodsList);
            SAV.SetUGI_Spheres(spheresList);
            SAV.SetUGI_Traps(trapsList);
            SAV.SetUGI_Treasures(treasuresList);
        }

        private void GetUGScores()
        {
            U_PlayersMet.Value = SAV.UG_PlayersMet;
            U_Gifts.Value = SAV.UG_Gifts;
            U_Spheres.Value = SAV.UG_Spheres;
            U_Fossils.Value = SAV.UG_Fossils;
            U_TrapsA.Value = SAV.UG_TrapsAvoided;
            U_TrapsT.Value = SAV.UG_TrapsTriggered;
            U_Flags.Value = SAV.UG_Flags;
        }

        private void SetUGScores()
        {
            SAV.UG_PlayersMet = (uint)U_PlayersMet.Value;
            SAV.UG_Gifts = (uint)U_Gifts.Value;
            SAV.UG_Spheres = (uint)U_Spheres.Value;
            SAV.UG_Fossils = (uint)U_Fossils.Value;
            SAV.UG_TrapsAvoided = (uint)U_TrapsA.Value;
            SAV.UG_TrapsTriggered = (uint)U_TrapsT.Value;
            SAV.UG_Flags = (uint)U_Flags.Value;
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
}
