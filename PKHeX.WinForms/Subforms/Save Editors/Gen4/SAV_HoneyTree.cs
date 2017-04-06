using System;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SAV_HoneyTree : Form
    {
        private readonly SAV4 SAV = (SAV4)Main.SAV.Clone();
        public SAV_HoneyTree()
        {
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.curlanguage);

            if (SAV.DP)
            {
                SAV.getData(SAV.PokeDex - 0x12DC + DP_OFFSET, 8 * 21).CopyTo(TreeBlock, 0);
            }
            else if (SAV.Pt)
            {
                SAV.getData(SAV.PokeDex - 0x1328 + PT_OFFSET, 8 * 21).CopyTo(TreeBlock, 0);
                //No silcoon/cascoon in Pt
                CB_Species.Items.Remove(CB_Species.Items[(int)TreeSpecies.Silcoon_Wurmple]);
            }

            CB_TreeList.SelectedIndex = 0;

            //Get Munchlax tree for this savegame in screen
            A = (SAV.TID >> 8) % 21;
            B = (SAV.TID & 0x00FF) % 21;
            C = (SAV.SID >> 8) % 21;
            D = (SAV.SID & 0x00FF) % 21;

            if (A == B)
            {
                B += 1;
                if (B == 21)
                    B = 0;
            }
            if (A == C)
            {
                C += 1;
                if (C == 21)
                    C = 0;
            }
            if (B == C)
            {
                C += 1;
                if (C == 21)
                    C = 0;
            }
            if (A == D)
            {
                D += 1;
                if (D == 21)
                    D = 0;
            }
            if (B == D)
            {
                D += 1;
                if (D == 21)
                    D = 0;
            }
            if (C == D)
             {
                D += 1;
                if (D == 21)
                    D = 0;
            }

            L_Tree0.Text = "- " + CB_TreeList.GetItemText(CB_TreeList.Items[A]);
            L_Tree1.Text = "- " + CB_TreeList.GetItemText(CB_TreeList.Items[B]);
            L_Tree2.Text = "- " + CB_TreeList.GetItemText(CB_TreeList.Items[C]);
            L_Tree3.Text = "- " + CB_TreeList.GetItemText(CB_TreeList.Items[D]);
        }
        private void B_Save_Click(object sender, EventArgs e)
        {
            if (SAV.DP)
                SAV.setData(TreeBlock, SAV.PokeDex - 0x12DC + DP_OFFSET);
            else if (SAV.Pt)
                SAV.setData(TreeBlock, SAV.PokeDex - 0x1328 + PT_OFFSET);
            SAV.Data.CopyTo(Main.SAV.Data, 0);
            Main.SAV.Edited = true;
            Close();
        }
        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void CB_TreeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            TreeIndex = CB_TreeList.SelectedIndex;
            readTree();
        }
        #region Tree
        private int TreeIndex;
        private int row;
        private int column;
        private int modifier;
        private byte[] TreeBlock = new byte[8 * 21];
        private int DP_OFFSET = 0x72E4;
        private int PT_OFFSET = 0x7F38;
        private int A, B, C, D;

        private void CB_Species_SelectedIndexChanged(object sender, EventArgs e)
        {
            saveTreeSpecies();
            if (getTreeSpecies() == (int)TreeSpecies.Munchlax && CB_TreeList.SelectedIndex != A && CB_TreeList.SelectedIndex != B && CB_TreeList.SelectedIndex != C && CB_TreeList.SelectedIndex != D )
            {
                MessageBox.Show("Warning!\n\nCatching Munchlax in this tree will make it illegal\nfor this savegame's TID/SID combination.");
            }
        }

        private void NUD_Time_ValueChanged(object sender, EventArgs e)
        {
            saveTreeTime();
        }

        private void NUD_Shake_ValueChanged(object sender, EventArgs e)
        {
            saveTreeShake();
        }

        private void B_Catchable_Click(object sender, EventArgs e)
        {
            NUD_Time.Value = 1080;
        }

        private enum TreeSpecies
        {
            None,
            Aipom,
            Burmy,
            Cherubi,
            Combee,
            Heracross,
            Munchlax,
            Silcoon_Wurmple, //Silcoon/cascoon aren't available in Pt
            Wurmple //Not used for Pt
        };

        private int getTreeSpecies()
        {
            int species = (int)TreeSpecies.None;
            switch (SAV.Version)
            {
                case GameVersion.DP:
                    if (modifier == 0 || modifier > 3)//No Pokémon
                    {
                        species = (int)TreeSpecies.None;
                    }
                    else if (modifier == 1)//Column 0
                    {
                        switch (row)
                        {
                            case 0:
                                species = (int)TreeSpecies.Wurmple;
                                break;
                            case 1:
                                species = (int)TreeSpecies.Silcoon_Wurmple;
                                break;
                            case 2:
                                species = (int)TreeSpecies.Combee;
                                break;
                            case 3:
                                species = (int)TreeSpecies.Burmy;
                                break;
                            case 4:
                                species = (int)TreeSpecies.Cherubi;
                                break;
                            case 5:
                                species = (int)TreeSpecies.Aipom;
                                break;
                        }
                    }
                    else if (modifier == 2)//Column 1
                    {
                        switch (row)
                        {
                            case 0:
                                species = (int)TreeSpecies.Combee;
                                break;
                            case 1:
                                species = (int)TreeSpecies.Burmy;
                                break;
                            case 2:
                                species = (int)TreeSpecies.Cherubi;
                                break;
                            case 3:
                                species = (int)TreeSpecies.Aipom;
                                break;
                            case 4:
                                species = (int)TreeSpecies.Heracross;
                                break;
                            case 5:
                                species = (int)TreeSpecies.Wurmple;
                                break;
                        }
                    }
                    else if (modifier == 3)//Munchlax
                    {
                        species = (int)TreeSpecies.Munchlax;
                    }
                    break;
                case GameVersion.Pt:
                    if (modifier == 0 || modifier > 3)//No Pokémon
                    {
                        species = (int)TreeSpecies.None;
                    }
                    else if (modifier == 1)//Column 0
                    {
                        switch (row)
                        {
                            case 0:
                                species = (int)TreeSpecies.Combee;
                                break;
                            case 1:
                                species = (int)TreeSpecies.Silcoon_Wurmple;
                                break;
                            case 2:
                                species = (int)TreeSpecies.Burmy;
                                break;
                            case 3:
                                species = (int)TreeSpecies.Cherubi;
                                break;
                            case 4:
                                species = (int)TreeSpecies.Aipom;
                                break;
                            case 5:
                                species = (int)TreeSpecies.Aipom;
                                break;
                        }
                    }
                    else if (modifier == 2)//Column 1
                    {
                        switch (row)
                        {
                            case 0:
                                species = (int)TreeSpecies.Burmy;
                                break;
                            case 1:
                                species = (int)TreeSpecies.Cherubi;
                                break;
                            case 2:
                                species = (int)TreeSpecies.Combee;
                                break;
                            case 3:
                                species = (int)TreeSpecies.Aipom;
                                break;
                            case 4:
                                species = (int)TreeSpecies.Aipom;
                                break;
                            case 5:
                                species = (int)TreeSpecies.Heracross;
                                break;
                        }
                    }
                    else if (modifier == 3)//Munchlax
                    {
                        species = (int)TreeSpecies.Munchlax;
                    }
                    break;
            }
            return species;
        }

        private void readTree()
        {
            NUD_Time.Value = BitConverter.ToUInt32(TreeBlock, TreeIndex * 8);
            NUD_Shake.Value = (int)TreeBlock[TreeIndex * 8 + 7];
            row = TreeBlock[TreeIndex * 8 + 4];
            column = TreeBlock[TreeIndex * 8 + 5];
            modifier = TreeBlock[TreeIndex * 8 + 6];

            CB_Species.SelectedIndex = getTreeSpecies();

            
        }
        private void saveTreeTime()
        {
            BitConverter.GetBytes((UInt32)NUD_Time.Value).CopyTo(TreeBlock, TreeIndex * 8);
        }
        private void saveTreeShake()
        {
            TreeBlock[TreeIndex * 8 + 7] = (byte)NUD_Shake.Value;
        }
        private void saveTreeSpecies()
        {
            //Only change species if actually different (prevents needless savedata modification)
            if (CB_Species.SelectedIndex != getTreeSpecies())
            {
                if (SAV.Version == GameVersion.DP)
                {
                    switch (CB_Species.SelectedIndex)
                    {
                        case (int)TreeSpecies.None:
                            TreeBlock[TreeIndex * 8 + 6] = 0;//Modifier
                            TreeBlock[TreeIndex * 8 + 5] = 0;//Column
                            TreeBlock[TreeIndex * 8 + 4] = 0;//Row
                            break;
                        case (int)TreeSpecies.Aipom:
                            TreeBlock[TreeIndex * 8 + 6] = 1;//Modifier
                            TreeBlock[TreeIndex * 8 + 5] = 0;//Column
                            TreeBlock[TreeIndex * 8 + 4] = 5;//Row
                            break;
                        case (int)TreeSpecies.Burmy:
                            TreeBlock[TreeIndex * 8 + 6] = 1;//Modifier
                            TreeBlock[TreeIndex * 8 + 5] = 0;//Column
                            TreeBlock[TreeIndex * 8 + 4] = 3;//Row
                            break;
                        case (int)TreeSpecies.Cherubi:
                            TreeBlock[TreeIndex * 8 + 6] = 2;//Modifier
                            TreeBlock[TreeIndex * 8 + 5] = 1;//Column
                            TreeBlock[TreeIndex * 8 + 4] = 2;//Row
                            break;
                        case (int)TreeSpecies.Combee:
                            TreeBlock[TreeIndex * 8 + 6] = 1;//Modifier
                            TreeBlock[TreeIndex * 8 + 5] = 0;//Column
                            TreeBlock[TreeIndex * 8 + 4] = 2;//Row
                            break;
                        case (int)TreeSpecies.Heracross:
                            TreeBlock[TreeIndex * 8 + 6] = 2;//Modifier
                            TreeBlock[TreeIndex * 8 + 5] = 1;//Column
                            TreeBlock[TreeIndex * 8 + 4] = 4;//Row
                            break;
                        case (int)TreeSpecies.Munchlax:
                            TreeBlock[TreeIndex * 8 + 6] = 3;//Modifier
                            TreeBlock[TreeIndex * 8 + 5] = 2;//Column
                            TreeBlock[TreeIndex * 8 + 4] = 0;//Row
                            break;
                        case (int)TreeSpecies.Silcoon_Wurmple:
                            TreeBlock[TreeIndex * 8 + 6] = 1;//Modifier
                            TreeBlock[TreeIndex * 8 + 5] = 0;//Column
                            TreeBlock[TreeIndex * 8 + 4] = 1;//Row
                            break;
                        case (int)TreeSpecies.Wurmple:
                            TreeBlock[TreeIndex * 8 + 6] = 1;//Modifier
                            TreeBlock[TreeIndex * 8 + 5] = 0;//Column
                            TreeBlock[TreeIndex * 8 + 4] = 0;//Row
                            break;
                    }
                }
                else if (SAV.Version == GameVersion.Pt)
                {
                    switch (CB_Species.SelectedIndex)
                    {
                        case (int)TreeSpecies.None:
                            TreeBlock[TreeIndex * 8 + 6] = 0;//Modifier
                            TreeBlock[TreeIndex * 8 + 5] = 0;//Column
                            TreeBlock[TreeIndex * 8 + 4] = 0;//Row
                            break;
                        case (int)TreeSpecies.Aipom:
                            TreeBlock[TreeIndex * 8 + 6] = 1;//Modifier
                            TreeBlock[TreeIndex * 8 + 5] = 0;//Column
                            TreeBlock[TreeIndex * 8 + 4] = 4;//Row
                            break;
                        case (int)TreeSpecies.Burmy:
                            TreeBlock[TreeIndex * 8 + 6] = 1;//Modifier
                            TreeBlock[TreeIndex * 8 + 5] = 0;//Column
                            TreeBlock[TreeIndex * 8 + 4] = 2;//Row
                            break;
                        case (int)TreeSpecies.Cherubi:
                            TreeBlock[TreeIndex * 8 + 6] = 1;//Modifier
                            TreeBlock[TreeIndex * 8 + 5] = 0;//Column
                            TreeBlock[TreeIndex * 8 + 4] = 3;//Row
                            break;
                        case (int)TreeSpecies.Combee:
                            TreeBlock[TreeIndex * 8 + 6] = 1;//Modifier
                            TreeBlock[TreeIndex * 8 + 5] = 0;//Column
                            TreeBlock[TreeIndex * 8 + 4] = 0;//Row
                            break;
                        case (int)TreeSpecies.Heracross:
                            TreeBlock[TreeIndex * 8 + 6] = 2;//Modifier
                            TreeBlock[TreeIndex * 8 + 5] = 1;//Column
                            TreeBlock[TreeIndex * 8 + 4] = 5;//Row
                            break;
                        case (int)TreeSpecies.Munchlax:
                            TreeBlock[TreeIndex * 8 + 6] = 3;//Modifier
                            TreeBlock[TreeIndex * 8 + 5] = 2;//Column
                            TreeBlock[TreeIndex * 8 + 4] = 0;//Row
                            break;
                        case (int)TreeSpecies.Silcoon_Wurmple://There one less pokémon available in Pt
                            TreeBlock[TreeIndex * 8 + 6] = 1;//Modifier
                            TreeBlock[TreeIndex * 8 + 5] = 0;//Column
                            TreeBlock[TreeIndex * 8 + 4] = 1;//Row
                            break;
                    }
                }
                else
                {
                    TreeBlock[TreeIndex * 8 + 6] = 0;//Modifier
                    TreeBlock[TreeIndex * 8 + 5] = 0;//Column
                    TreeBlock[TreeIndex * 8 + 4] = 0;//Row
                }
                //Update info for getTreeSpecies
                row = TreeBlock[TreeIndex * 8 + 4];
                column = TreeBlock[TreeIndex * 8 + 5];
                modifier = TreeBlock[TreeIndex * 8 + 6];
            }
        }
        #endregion
    }
}
