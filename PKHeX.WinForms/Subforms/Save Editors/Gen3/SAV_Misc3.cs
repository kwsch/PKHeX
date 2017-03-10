using System;
using System.Linq;
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

            
            if (SAV.FRLG)
            {
                readJoyful();
                TB_OTName.Text = PKX.getG3Str(SAV.getData(SAV.getBlockOffset(4) + 0xBCC, 8), SAV.Japanese);
            }
            else
            {
                TAB_Joyful.Hide();
                TB_OTName.Hide();
                L_TrainerName.Hide();
            }
            NUD_BP.Value = SAV.BP;
            NUD_Coins.Value = SAV.Coin;
        }
        private void B_Save_Click(object sender, EventArgs e)
        {
            if (SAV.FRLG)
            {
                saveJoyful();
                SAV.setData(PKX.setG3Str(TB_OTName.Text, SAV.Japanese), SAV.getBlockOffset(4) + 0xBCC);
            }
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
    }
}
