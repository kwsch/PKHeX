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
        private string[] uggoods, ugspheres, ugtraps, ugtreasures;
        private string[] uggoods_sorted, ugtraps_sort, ugtreasures_sort;

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
            uggoods = GameInfo.Strings.uggoods;
            ugspheres = GameInfo.Strings.ugspheres;
            ugtraps = GameInfo.Strings.ugtraps;
            ugtreasures = GameInfo.Strings.ugtreasures;

            //Goods
            uggoods_sorted = SanitizeList(uggoods);
            DGV_UGGoods.Rows.Add(MAX_SIZE); 
            
            Item_Goods.DataSource = new BindingSource(uggoods_sorted, null);
            Item_Goods.DisplayIndex = 0;
            DGV_UGGoods.CancelEdit();

            //Spheres
            DGV_UGSpheres.Rows.Add(MAX_SIZE);

            Item_Spheres.DataSource = new BindingSource(ugspheres, null);
            Item_Spheres.DisplayIndex = 0;
            DGV_UGSpheres.CancelEdit();

            //Traps
            ugtraps_sort = SanitizeList(ugtraps);
            DGV_UGTraps.Rows.Add(MAX_SIZE);

            Item_Traps.DataSource = new BindingSource(ugtraps_sort, null);
            Item_Traps.DisplayIndex = 0;
            DGV_UGTraps.CancelEdit();

            //Treasures
            ugtreasures_sort = SanitizeList(ugtreasures);
            DGV_UGTreasures.Rows.Add(MAX_SIZE);

            Item_Treasures.DataSource = new BindingSource(ugtreasures_sort, null);
            Item_Treasures.DisplayIndex = 0;
            DGV_UGTreasures.CancelEdit();
        }

        private void ReadUGData()
        {
            byte[] goodslist = SAV.UGI_Goods;
            byte[] sphereslist = SAV.UGI_Spheres;
            byte[] trapslist = SAV.UGI_Traps;
            byte[] treasureslist = SAV.UGI_Treasures;

            //Goods
            for (int i = 0; i < goodslist.Length; i++)
            {   
                DGV_UGGoods.Rows[i].Cells[0].Value = uggoods[goodslist[i]].ToString();
            }

            //Spheres (split in two, first 40 positions are the sphere type, last 40 are their 1=1 size)
            for (int i = 0; i < (sphereslist.Length / 2); i++)
            {
                DGV_UGSpheres.Rows[i].Cells[0].Value = ugspheres[sphereslist[i]].ToString();
                DGV_UGSpheres.Rows[i].Cells[1].Value = sphereslist[i + MAX_SIZE].ToString();
            }

            //Traps
            for (int i = 0; i < trapslist.Length; i++)
            {
                DGV_UGTraps.Rows[i].Cells[0].Value = ugtraps[trapslist[i]].ToString();
            }

            //Treasures
            for (int i = 0; i < treasureslist.Length; i++)
            {
                DGV_UGTreasures.Rows[i].Cells[0].Value = ugtreasures[treasureslist[i]].ToString();
            }
        }

        private void SaveUGData()
        {
            byte[] goodslist = new byte[SAV.UGI_Goods.Length];
            byte[] sphereslist = new byte[SAV.UGI_Spheres.Length];
            byte[] trapslist = new byte[SAV.UGI_Traps.Length];
            byte[] treasureslist = new byte[SAV.UGI_Treasures.Length];

            //Goods
            int ctr = 0;
            for (int i = 0; i < DGV_UGGoods.Rows.Count; i++)
            {
                var str = DGV_UGGoods.Rows[i].Cells[0].Value.ToString();
                var itemindex = Array.IndexOf(uggoods, str);

                if (itemindex <= 0)
                    continue; //ignore empty slot

                goodslist[ctr] = (byte)itemindex;
                ctr++;
            }

            //Spheres
            int itemcnt;
            ctr = 0;
            for (int i = 0; i < DGV_UGSpheres.Rows.Count; i++)
            {
                var str = DGV_UGSpheres.Rows[i].Cells[0].Value.ToString();
                var itemindex = Array.IndexOf(ugspheres, str);
                
                bool success = int.TryParse(DGV_UGSpheres.Rows[i].Cells[1].Value?.ToString(), out itemcnt);
                if (!success || itemindex <= 0)
                    continue;  //ignore empty slot or non-numeric values

                sphereslist[ctr] = (byte)itemindex;
                sphereslist[ctr + MAX_SIZE] = (byte)itemcnt;
                ctr++;
            }

            //Traps
            ctr = 0;
            for (int i = 0; i < DGV_UGTraps.Rows.Count; i++)
            {
                var str = DGV_UGTraps.Rows[i].Cells[0].Value.ToString();
                var itemindex = Array.IndexOf(ugtraps, str);

                if (itemindex <= 0)
                    continue; //ignore empty slot

                trapslist[ctr] = (byte)itemindex;
                ctr++;
            }

            //Treasures
            ctr = 0;
            for (int i = 0; i < DGV_UGTreasures.Rows.Count; i++)
            {
                var str = DGV_UGTreasures.Rows[i].Cells[0].Value.ToString();
                var itemindex = Array.IndexOf(ugtreasures, str);

                if (itemindex <= 0)
                    continue; //ignore empty slot

                treasureslist[ctr] = (byte)itemindex;
                ctr++;
            }

            SAV.UGI_Goods = goodslist;
            SAV.UGI_Spheres = sphereslist;
            SAV.UGI_Traps = trapslist;
            SAV.UGI_Treasures = treasureslist;
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
            string[] list_sort = inputlist.Where(x => !string.IsNullOrEmpty(x)).ToArray();
            Array.Sort(list_sort);

            return list_sort;
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
