using System;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SAV_Misc3 : Form
    {
        private readonly SAV3 SAV = (SAV3)Main.SAV.Clone();
        public SAV_Misc3()
        {
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.curlanguage);

            if (SAV.FRLG || SAV.E)
                readJoyful();
            else
                tabControl1.Controls.Remove(TAB_Joyful);

            if (SAV.E)
                readFerry();
            else
                tabControl1.Controls.Remove(TAB_Ferry);

            if (SAV.FRLG)
                TB_OTName.Text = PKX.getString3(SAV.Data, SAV.getBlockOffset(4) + 0xBCC, 8, SAV.Japanese);
            else
                TB_OTName.Visible = L_TrainerName.Visible = false;
            
            NUD_BP.Value = Math.Min(NUD_BP.Maximum, SAV.BP);
            NUD_Coins.Value = Math.Min(NUD_Coins.Maximum, SAV.Coin);
        }
        private void B_Save_Click(object sender, EventArgs e)
        {
            if (tabControl1.Controls.Contains(TAB_Joyful))
                saveJoyful();
            if (tabControl1.Controls.Contains(TAB_Ferry))
            {
                saveFerry();
                if (Pouches != null) SAV.Inventory = Pouches;
            }
            if (SAV.FRLG)
                SAV.setData(SAV.setString(TB_OTName.Text, TB_OTName.MaxLength), SAV.getBlockOffset(4) + 0xBCC);

            SAV.BP = (ushort)NUD_BP.Value;
            SAV.Coin = (ushort)NUD_Coins.Value;

            SAV.Data.CopyTo(Main.SAV.Data, 0);
            Main.SAV.Edited = true;
            Close();
        }
        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        #region Joyful
        private int JUMPS_IN_ROW, JUMPS_SCORE, JUMPS_5_IN_ROW;
        private int BERRIES_IN_ROW, BERRIES_SCORE, BERRIES_5_IN_ROW;
        private void readJoyful()
        {
            switch (SAV.Version)
            {
                case GameVersion.E:
                    JUMPS_IN_ROW = SAV.getBlockOffset(0) + 0x1fc;
                    JUMPS_SCORE = SAV.getBlockOffset(0) + 0x208;
                    JUMPS_5_IN_ROW = SAV.getBlockOffset(0) + 0x200;

                    BERRIES_IN_ROW = SAV.getBlockOffset(0) + 0x210;
                    BERRIES_SCORE = SAV.getBlockOffset(0) + 0x20c;
                    BERRIES_5_IN_ROW = SAV.getBlockOffset(0) + 0x214;
                    break;
                case GameVersion.FRLG:
                    JUMPS_IN_ROW = SAV.getBlockOffset(0) + 0xB00;
                    JUMPS_SCORE = SAV.getBlockOffset(0) + 0xB0C;
                    JUMPS_5_IN_ROW = SAV.getBlockOffset(0) + 0xB04;

                    BERRIES_IN_ROW = SAV.getBlockOffset(0) + 0xB14;
                    BERRIES_SCORE = SAV.getBlockOffset(0) + 0xB10;
                    BERRIES_5_IN_ROW = SAV.getBlockOffset(0) + 0xB18;
                    break;
            }
            TB_J1.Text = Math.Min((ushort)9999, BitConverter.ToUInt16(SAV.Data, JUMPS_IN_ROW)).ToString();
            TB_J2.Text = Math.Min((ushort)9999, BitConverter.ToUInt16(SAV.Data, JUMPS_SCORE)).ToString();
            TB_J3.Text = Math.Min((ushort)9999, BitConverter.ToUInt16(SAV.Data, JUMPS_5_IN_ROW)).ToString();
            TB_B1.Text = Math.Min((ushort)9999, BitConverter.ToUInt16(SAV.Data, BERRIES_IN_ROW)).ToString();
            TB_B2.Text = Math.Min((ushort)9999, BitConverter.ToUInt16(SAV.Data, BERRIES_SCORE)).ToString();
            TB_B3.Text = Math.Min((ushort)9999, BitConverter.ToUInt16(SAV.Data, BERRIES_5_IN_ROW)).ToString();
        }
        private void saveJoyful()
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
        private int ofsFerry;
        private InventoryPouch[] Pouches = null;
        private void B_GetTickets_Click(object sender, EventArgs e)
        {
            Pouches = SAV.Inventory;
            string[] itemlist = GameInfo.Strings.getItemStrings(SAV.Generation, SAV.Version);
            bool b = false;
            int[] targetItems = new int[5] { 0x109, 0x113, 0x172, 0x173, 0x178 };
            int ai;
            string AlertMessage = "";
            foreach (InventoryPouch p in Pouches)
            {
                if (p.Type != InventoryType.KeyItems) continue;
                foreach (InventoryItem i in p.Items)
                {
                    ai = Array.IndexOf(targetItems, i.Index);
                    if (ai < 0) continue;
                    if (i.Count <= 0) i.Count = 1;
                    targetItems[ai] = 0;
                    if (!Array.Exists(targetItems, v => v > 0))
                    {
                        b = true;
                        break;
                    }
                }
                if (b)
                {
                    B_GetTickets.Text = "Already have";
                    B_GetTickets.Enabled = false;
                    return;
                }
                for (int i = 0; i < itemlist.Length; i++)
                    if (itemlist[i] == "")
                        itemlist[i] = $"(Item #{i:000})";
                for (int t = 0, i = -1; t < targetItems.Length; t++)
                {
                    if (targetItems[t] == 0) continue;
                    for (i++; i < p.Items.Length; i++)
                    {
                        if (p.Items[i].Index > 0) continue;
                        p.Items[i].Index = targetItems[t];
                        p.Items[i].Count = 1;
                        p.Items[i].New = true;
                        AlertMessage += Environment.NewLine + " " + itemlist[targetItems[t]];
                        targetItems[t] = 0;
                        break;
                    }
                    if (!Array.Exists(targetItems, v => v > 0))
                        break;
                }
                break;
            }
            B_GetTickets.Enabled = false;
            if (AlertMessage.Length == 0)
            {
                B_GetTickets.Text = "Failed";
                WinFormsUtil.Alert("Failed. Please use the InventoryEditor.");
            }
            else
            {
                if (Array.Exists(targetItems, v => v > 0))
                {
                    AlertMessage += Environment.NewLine;
                    AlertMessage += "Failed to get some items. Please use the InventoryEditor.";
                    foreach (int u in targetItems)
                    {
                        if (u == 0) continue;
                        AlertMessage += Environment.NewLine + " " + itemlist[u];
                    }
                }
                B_GetTickets.Text = "Ready";
                WinFormsUtil.Alert("Ready to get new items." + AlertMessage);
            }
        }
        private void readFerry()
        {
            ofsFerry = SAV.getBlockOffset(2) + 0x2F0;
            CHK_Catchable.Checked = getFerryFlagFromNum(0x864);
            CHK_ReachSouthern.Checked = getFerryFlagFromNum(0x8B3);
            CHK_ReachBirth.Checked = getFerryFlagFromNum(0x8D5);
            CHK_ReachFaraway.Checked = getFerryFlagFromNum(0x8D6);
            CHK_ReachNavel.Checked = getFerryFlagFromNum(0x8E0);
            CHK_ReachBF.Checked = getFerryFlagFromNum(0x1D0);
            CHK_InitialSouthern.Checked = getFerryFlagFromNum(0x1AE);
            CHK_InitialBirth.Checked = getFerryFlagFromNum(0x1AF);
            CHK_InitialFaraway.Checked = getFerryFlagFromNum(0x1B0);
            CHK_InitialNavel.Checked = getFerryFlagFromNum(0x1DB);
        }
        private bool getFerryFlagFromNum(int n)
        {
            return (SAV.Data[ofsFerry + (n >> 3)] >> (n & 7) & 1) != 0;
        }
        private void setFerryFlagFromNum(int n, bool b)
        {
            SAV.Data[ofsFerry + (n >> 3)] = (byte)(SAV.Data[ofsFerry + (n >> 3)] & ~(1 << (n & 7)) | (b ? 1 : 0) << (n & 7));
        }
        private void saveFerry()
        {
            setFerryFlagFromNum(0x864, CHK_Catchable.Checked);
            setFerryFlagFromNum(0x8B3, CHK_ReachSouthern.Checked);
            setFerryFlagFromNum(0x8D5, CHK_ReachBirth.Checked);
            setFerryFlagFromNum(0x8D6, CHK_ReachFaraway.Checked);
            setFerryFlagFromNum(0x8E0, CHK_ReachNavel.Checked);
            setFerryFlagFromNum(0x1D0, CHK_ReachBF.Checked);
            setFerryFlagFromNum(0x1AE, CHK_InitialSouthern.Checked);
            setFerryFlagFromNum(0x1AF, CHK_InitialBirth.Checked);
            setFerryFlagFromNum(0x1B0, CHK_InitialFaraway.Checked);
            setFerryFlagFromNum(0x1DB, CHK_InitialNavel.Checked);
        }
        #endregion
    }
}
