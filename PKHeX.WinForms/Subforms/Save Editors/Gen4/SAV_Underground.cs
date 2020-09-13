using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SAV_Underground : Form
    {
        private readonly SaveFile Origin;
        private readonly SAV4Sinnoh SAV;

        private const int MAX_SIZE = 0x28; // 0x28 max length for each of the inventory pouches
        private string[] ugGoods, ugSpheres, ugTraps, ugTreasures;
        private string[] ugGoodsSorted, ugTrapsSorted, ugTreasuresSorted;

        public SAV_Underground(SaveFile sav)
        {
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            SAV = (SAV4Sinnoh)(Origin = sav).Clone();

            InitializeDGV();

            GetUGScores();
            ReadUGData();
        }

        private void InitializeDGV()
        {
            ugGoods = GameInfo.Strings.uggoods;
            ugSpheres = GameInfo.Strings.ugspheres;
            ugTraps = GameInfo.Strings.ugtraps;
            ugTreasures = GameInfo.Strings.ugtreasures;

            //Goods
            ugGoodsSorted = SanitizeList(ugGoods);
            DGV_UGGoods.Rows.Add(MAX_SIZE); 
            
            Item_Goods.DataSource = new BindingSource(ugGoodsSorted, null);
            Item_Goods.DisplayIndex = 0;
            DGV_UGGoods.CancelEdit();

            //Spheres
            DGV_UGSpheres.Rows.Add(MAX_SIZE);

            Item_Spheres.DataSource = new BindingSource(ugSpheres, null);
            Item_Spheres.DisplayIndex = 0;
            DGV_UGSpheres.CancelEdit();

            //Traps
            ugTrapsSorted = SanitizeList(ugTraps);
            DGV_UGTraps.Rows.Add(MAX_SIZE);

            Item_Traps.DataSource = new BindingSource(ugTrapsSorted, null);
            Item_Traps.DisplayIndex = 0;
            DGV_UGTraps.CancelEdit();

            //Treasures
            ugTreasuresSorted = SanitizeList(ugTreasures);
            DGV_UGTreasures.Rows.Add(MAX_SIZE);

            Item_Treasures.DataSource = new BindingSource(ugTreasuresSorted, null);
            Item_Treasures.DisplayIndex = 0;
            DGV_UGTreasures.CancelEdit();
        }

        private void ReadUGData()
        {
            byte[] goodsList = SAV.UGI_Goods;
            byte[] spheresList = SAV.UGI_Spheres;
            byte[] trapsList = SAV.UGI_Traps;
            byte[] treasuresList = SAV.UGI_Treasures;

            //Goods
            for (int i = 0; i < goodsList.Length; i++)
            {   
                DGV_UGGoods.Rows[i].Cells[0].Value = ugGoods[goodsList[i]].ToString();
            }

            //Spheres (split in two, first 40 positions are the sphere type, last 40 are their size)
            for (int i = 0; i < (spheresList.Length / 2); i++)
            {
                DGV_UGSpheres.Rows[i].Cells[0].Value = ugSpheres[spheresList[i]].ToString();
                DGV_UGSpheres.Rows[i].Cells[1].Value = spheresList[i + MAX_SIZE].ToString();
            }

            //Traps
            for (int i = 0; i < trapsList.Length; i++)
            {
                DGV_UGTraps.Rows[i].Cells[0].Value = ugTraps[trapsList[i]].ToString();
            }

            //Treasures
            for (int i = 0; i < treasuresList.Length; i++)
            {
                DGV_UGTreasures.Rows[i].Cells[0].Value = ugTreasures[treasuresList[i]].ToString();
            }
        }

        private void SaveUGData()
        {
            byte[] goodsList = new byte[SAV.UGI_Goods.Length];
            byte[] spheresList = new byte[SAV.UGI_Spheres.Length];
            byte[] trapsList = new byte[SAV.UGI_Traps.Length];
            byte[] treasuresList = new byte[SAV.UGI_Treasures.Length];

            //Goods
            int ctr = 0;
            for (int i = 0; i < DGV_UGGoods.Rows.Count; i++)
            {
                var str = DGV_UGGoods.Rows[i].Cells[0].Value.ToString();
                var itemindex = Array.IndexOf(ugGoods, str);

                if (itemindex <= 0)
                    continue; //ignore empty slot

                goodsList[ctr] = (byte)itemindex;
                ctr++;
            }

            //Spheres
            int itemcnt;
            ctr = 0;
            for (int i = 0; i < DGV_UGSpheres.Rows.Count; i++)
            {
                var str = DGV_UGSpheres.Rows[i].Cells[0].Value.ToString();
                var itemindex = Array.IndexOf(ugSpheres, str);
                
                bool success = int.TryParse(DGV_UGSpheres.Rows[i].Cells[1].Value?.ToString(), out itemcnt);
                if (!success || itemindex <= 0)
                    continue;  //ignore empty slot or non-numeric values

                spheresList[ctr] = (byte)itemindex;
                spheresList[ctr + MAX_SIZE] = (byte)itemcnt;
                ctr++;
            }

            //Traps
            ctr = 0;
            for (int i = 0; i < DGV_UGTraps.Rows.Count; i++)
            {
                var str = DGV_UGTraps.Rows[i].Cells[0].Value.ToString();
                var itemindex = Array.IndexOf(ugTraps, str);

                if (itemindex <= 0)
                    continue; //ignore empty slot

                trapsList[ctr] = (byte)itemindex;
                ctr++;
            }

            //Treasures
            ctr = 0;
            for (int i = 0; i < DGV_UGTreasures.Rows.Count; i++)
            {
                var str = DGV_UGTreasures.Rows[i].Cells[0].Value.ToString();
                var itemindex = Array.IndexOf(ugTreasures, str);

                if (itemindex <= 0)
                    continue; //ignore empty slot

                treasuresList[ctr] = (byte)itemindex;
                ctr++;
            }

            SAV.UGI_Goods = goodsList;
            SAV.UGI_Spheres = spheresList;
            SAV.UGI_Traps = trapsList;
            SAV.UGI_Treasures = treasuresList;
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

        private string[] SanitizeList(string[] inputlist)
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
