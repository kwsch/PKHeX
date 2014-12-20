using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PKHeX
{
    public partial class SAV_PokedexORAS : Form
    {
        public SAV_PokedexORAS(Form1 frm1)
        {
            InitializeComponent();
            Util.TranslateInterface(this, Form1.curlanguage);
            m_parent = frm1;
            Array.Copy(m_parent.savefile, sav, sav.Length);
            sv = m_parent.savindex * 0x7F000;
            dexoffset += sv;
            Setup();
            editing = false;
            LB_Species.SelectedIndex = 0;
            TB_Spinda.Text = BitConverter.ToUInt32(sav, dexoffset + 0x680).ToString("X8");
        }
        private int dexoffset = 0x15000 + 0x5400;
        Form1 m_parent;
        public byte[] sav = new byte[0x100000];
        public int sv = 0;
        public bool[,] specbools = new bool[9, 0x60 * 8];
        public bool[,] langbools = new bool[7, 0x60 * 8];
        bool editing = true;
        private void Setup()
        {
            // Clear Listbox and ComboBox
            try
            {
                LB_Species.Items.Clear();
                CB_Species.Items.Clear();
            }
            catch { }

            // Fill List
            #region Species
            {
                var species_list = Util.getCBList(Form1.specieslist, null);
                species_list.RemoveAt(0); // Remove 0th Entry
                CB_Species.DisplayMember = "Text";
                CB_Species.ValueMember = "Value";
                CB_Species.DataSource = species_list;
            }
            #endregion

            for (int i = 1; i < Form1.specieslist.Length; i++)
                LB_Species.Items.Add(i.ToString("000") + " - " + Form1.specieslist[i]);

            // Fill Bit arrays
            for (int i = 0; i < 9; i++)
            {
                byte[] data = new byte[0x60];
                int offset = dexoffset + 0x8 + 0x60 * i;
                Array.Copy(sav, offset, data, 0, 0x60);
                BitArray BitRegion = new BitArray(data);
                bool fuck = BitRegion[767];
                for (int b = 0; b < (0x60 * 8); b++)
                    specbools[i, b] = BitRegion[b];
            }

            // Fill Language arrays
            byte[] langdata = new byte[0x280];
            Array.Copy(sav, dexoffset + 0x400, langdata, 0, 0x280);
            BitArray LangRegion = new BitArray(langdata);
            for (int b = 0; b < (721); b++) // 721 Species
                for (int i = 0; i < 7; i++) // 7 Languages
                    langbools[i, b] = LangRegion[7 * b + i];
        }
        private void changeCBSpecies(object sender, EventArgs e)
        {
            if (!editing)
            {
                editing = true;
                int index = (int)CB_Species.SelectedValue;
                LB_Species.SelectedIndex = index - 1; // Since we don't allow index0 in combobox, everything is shifted by 1
                LB_Species.TopIndex = (int)(LB_Species.SelectedIndex);
                loadchks();
                editing = false;
            }
        }
        private void changeLBSpecies(object sender, EventArgs e)
        {
            if (!editing)
            {
                editing = true;
                try
                {
                    int index = LB_Species.SelectedIndex + 1;
                    CB_Species.SelectedValue = index;
                }
                catch { };
                loadchks();
                editing = false;
            }
        }
        private void loadchks()
        {
            // Load Bools for the data
            int pk = 0;
            try
            { pk = Util.getIndex(CB_Species); }
            catch { pk = (int)LB_Species.SelectedIndex + 1; }

            CheckBox[] CP = new CheckBox[] {
                CHK_P1,CHK_P2,CHK_P3,CHK_P4,CHK_P5,CHK_P6,CHK_P7,CHK_P8,CHK_P9,
            };
            CheckBox[] CL = new CheckBox[] {
                CHK_L1,CHK_L2,CHK_L3,CHK_L4,CHK_L5,CHK_L6,CHK_L7,
            };
            // Load Partitions
            for (int i = 0; i < 9; i++)
                CP[i].Checked = specbools[i, pk-1];
            for (int i = 0; i < 7; i++)
                CL[i].Checked = langbools[i, pk-1];
            {
                CHK_P1.Enabled = true;

                int index = LB_Species.SelectedIndex + 1;
                PKX.PersonalParser.Personal MonData = PKX.PersonalGetter.GetPersonal(index);
                int gt = MonData.GenderRatio;

                CHK_P2.Enabled = CHK_P4.Enabled = CHK_P6.Enabled = CHK_P8.Enabled = (gt != 254); // Not Female-Only
                CHK_P3.Enabled = CHK_P5.Enabled = CHK_P7.Enabled = CHK_P9.Enabled = (gt != 0) && (gt != 255); // Not Male-Only and Not Genderless

                for (int i = 0; i < 7; i++)
                    CL[i].Enabled = true;
            }

            // Load Encountered Count
            editing = true;
            MT_Count.Text = BitConverter.ToUInt16(sav, dexoffset + 0x686 + (pk - 1) * 2).ToString();
            editing = false;
        }
        private void removedropCB(object sender, KeyEventArgs e)
        {
            ((ComboBox)sender).DroppedDown = false;
        }
        private void changeLanguageBool(object sender, EventArgs e)
        {
            int species = LB_Species.SelectedIndex + 1;
            langbools[0, (species - 1)] = CHK_L1.Checked;
            langbools[1, (species - 1)] = CHK_L2.Checked;
            langbools[2, (species - 1)] = CHK_L3.Checked;
            langbools[3, (species - 1)] = CHK_L4.Checked;
            langbools[4, (species - 1)] = CHK_L5.Checked;
            langbools[5, (species - 1)] = CHK_L6.Checked;
            langbools[6, (species - 1)] = CHK_L7.Checked;
        }
        private void changePartitionBool(object sender, EventArgs e)
        {
            int species = LB_Species.SelectedIndex + 1;
            specbools[0, (species - 1)] = CHK_P1.Checked;
            specbools[1, (species - 1)] = CHK_P2.Checked;
            specbools[2, (species - 1)] = CHK_P3.Checked;
            specbools[3, (species - 1)] = CHK_P4.Checked;
            specbools[4, (species - 1)] = CHK_P5.Checked;
            specbools[5, (species - 1)] = CHK_P6.Checked;
            specbools[6, (species - 1)] = CHK_P7.Checked;
            specbools[7, (species - 1)] = CHK_P8.Checked;
            specbools[8, (species - 1)] = CHK_P9.Checked;
        }

        private void B_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void B_Save_Click(object sender, EventArgs e)
        {
            saveChanges();

            // Return back to the parent savefile
            Array.Copy(sav, m_parent.savefile, sav.Length);
            this.Close();
        }
        private void saveChanges()
        {
            // Save back the Species Bools 
            // Return to Byte Array        
            for (int p = 0; p < 9; p++)
            {
                byte[] sdata = new byte[0x60];

                for (int i = 0; i < 0x60 * 8; i++)
                    if (specbools[p, i])
                        sdata[i / 8] |= (byte)(1 << i % 8);

                Array.Copy(sdata, 0, sav, sv + dexoffset + 0x8 + 0x60 * p, 0x60);
            }

            // Build new bool array for the Languages
            {
                bool[] languagedata = new bool[0x27C * 8];
                for (int i = 0; i < 722; i++)
                    for (int l = 0; l < 7; l++)
                        languagedata[i * 7 + l] = langbools[l, i];

                // Return to Byte Array
                byte[] ldata = new byte[languagedata.Length / 8];

                for (int i = 0; i < languagedata.Length; i++)
                    if (languagedata[i])
                        ldata[i / 8] |= (byte)(1 << i % 8);

                Array.Copy(ldata, 0, sav, sv + dexoffset + 0x400, 0x27C);
            }
            
            // Store Spinda Spot
            try
            {
                uint PID = Util.getHEXval(TB_Spinda);
                Array.Copy(BitConverter.GetBytes(PID), 0, sav, dexoffset + 0x680, 4);
            }
            catch { };
        }

        private void B_GiveAll_Click(object sender, EventArgs e)
        {
            if (LB_Species.SelectedIndex > 0x2D0) return;
            if (CHK_L1.Enabled)
            {
                CHK_L1.Checked =
                CHK_L2.Checked =
                CHK_L3.Checked =
                CHK_L4.Checked =
                CHK_L5.Checked =
                CHK_L6.Checked =
                CHK_L7.Checked = !(ModifierKeys == Keys.Control);
            }
            if (CHK_P1.Enabled)
            {
                CHK_P1.Checked = !(ModifierKeys == Keys.Control);
            }
            int index = LB_Species.SelectedIndex+1;

            PKX.PersonalParser.Personal MonData = PKX.PersonalGetter.GetPersonal(index);
            int gt = MonData.GenderRatio;

            CHK_P2.Checked = CHK_P4.Checked = CHK_P6.Checked = CHK_P8.Checked = ((gt != 254)) && !(ModifierKeys == Keys.Control);
            CHK_P3.Checked = CHK_P5.Checked = CHK_P7.Checked = CHK_P9.Checked = (gt != 0) && (gt != 255) && !(ModifierKeys == Keys.Control);
 
            changePartitionBool(null, null);
            changeLanguageBool(null, null);
            LB_Species.SelectedIndex++;
        }
        private void B_FillDex_Click(object sender, EventArgs e)
        {
            saveChanges();
            int index = LB_Species.SelectedIndex;
            // Copy Full Dex Byte Array
            // Native Byte Writing
            for (int i = 0; i < 0x5A; i++)
                sav[sv + 0x1A400 + 8 + i] = 0xFF;
            sav[sv + 8 + 0x1A45A] = 0x01;

            // Midsection Writing
            {
                // starting at 0x15068;
                #region Payload
                uint[] payload = new uint[] { 
                        0x8FFFFFFF, // 20015068  
                        0xFFFFFFFF, // 2001506C 
                        0xFFFFFFFF, // 20015070 
                        0xF7FAFFFF, // 20015074 
                        0xFFFFFFFF, // 20015078 
                        0xFFFFFFFF, // 2001507C 
                        0xFFFFFFFF, // 20015080 
                        0xFFFCDFFF, // 20015084 
                        0xFFFFFFFF, // 20015088 
                        0xFDFFFFFF, // 2001508C 
                        0xFFFFFFFF, // 20015090 
                        0xF7FFFFFF, // 20015094 
                        0x6FFFFFFF, // 20015098 
                        0xFF7FFFFF, // 2001509C 
                        0xDFFFFFFF, // 200150A0 
                        0xFFFFFF7F, // 200150A4 
                        0xFFFFFFFF, // 200150A8 
                        0xFFFFFFE7, // 200150AC 
                        0xFFFFFFFF, // 200150B0 
                        0xFFCFFFFF, // 200150B4 
                        0x8FFFFFFF, // 200150B8 
                        0xFFFFFFFF, // 200150BC 
                        0x0001FFFF, // 200150C0 
                        0x00000000, // 
 
                        0x7FFFFFFF, // 200150C8 
                        0xFFFFFFFC, // 200150CC 
                        0xFFFCFFFF, // 200150D0 
                        0x7E7FF9E7, // 200150D4 
                        0xFF9C7EF7, // 200150D8 
                        0xFFFFFFFF, // 200150DC 
                        0xFFFFFEFF, // 200150E0 
                        0xF8E3E6FF, // 200150E4 
                        0xFFFFFFFF, // 200150E8 
                        0xFEFFFFF7, // 200150EC 
                        0xFF3CFFFF, // 200150F0 
                        0x081FFFFF, // 200150F4 
                        0xDFFFFFFC, // 200150F8 
                        0xFFE7FFFF, // 200150FC 
                        0x39FFDFFF, // 20015100 
                        0xFFFFC090, // 20015104 
                        0xF9FFFFFF, // 20015108 
                        0xFFFFFFFF, // 2001510C 
                        0xFE3FFFFF, // 20015110 
                        0x1FF39FBF, // 20015114 
                        0xFFFFFE00, // 20015118 
                        0xBFFFFFFF, // 2001511C 
                        0x000007FF, // 10015120 
                        0x00000000, // 

                        0x8FFFFFFF, // 20015128 
                        0xFFFFFFFF, // 2001512C 
                        0xFFFFFFFF, // 20015130 
                        0xF7FAFFFF, // 20015134 
                        0xFFFFFFFF, // 20015138 
                        0xFFFFFFFF, // 2001513C 
                        0xFFFFFFFF, // 20015140 
                        0xFFFCDFFF, // 20015144 
                        0xFFFFFFFF, // 20015148 
                        0xFDFFFFFF, // 2001514C 
                        0xFFFFFFFF, // 20015150 
                        0xF7FFFFFF, // 20015154 
                        0x6FFFFFFF, // 20015158 
                        0xFF7FFFFF, // 2001515C 
                        0xDFFFFFFF, // 20015160 
                        0xFFFFFF7F, // 20015164 
                        0xFFFFFFFF, // 20015168 
                        0xFFFFFFE7, // 2001516C 
                        0xFFFFFFFF, // 20015170 
                        0xFFCFFFFF, // 20015174 
                        0x8FFFFFFF, // 20015178 
                        0xFFFFFFFF, // 2001517C 
                        0x0001FFFF, // 20015180 
                        0x00000000, //  

                        0x7FFFFFFF, // 20015188 
                        0xFFFFFFFC, // 2001518C 
                        0xFFFCFFFF, // 20015190 
                        0x7E7FF9E7, // 20015194 
                        0xFF9C7EF7, // 20015198 
                        0xFFFFFFFF, // 2001519C 
                        0xFFFFFEFF, // 200151A0 
                        0xF8E3E6FF, // 200151A4 
                        0xFFFFFFFF, // 200151A8 
                        0xFEFFFFF7, // 200151AC 
                        0xFF3CFFFF, // 200151B0 
                        0x081FFFFF, // 200151B4 
                        0xDFFFFFFC, // 200151B8 
                        0xFFE7FFFF, // 200151BC 
                        0x39FFDFFF, // 200151C0 
                        0xFFFFC090, // 200151C4 
                        0xF9FFFFFF, // 200151C8 
                        0xFFFFFFFF, // 200151CC 
                        0xFE3FFFFF, // 200151D0 
                        0x1FF39FBF, // 200151D4 
                        0xFFFFFE00, // 200151D8 
                        0xBFFFFFFF, // 200151DC 
                        0x000007FF, // 100151E0 
                        0x00000000, // 

                        0x8FFFFFFF, // 200151E8 
                        0xFFFFFFFF, // 200151EC 
                        0xFFFFFFFF, // 200151F0 
                        0xF7FAFFFF, // 200151F4 
                        0xFFFFFFFF, // 200151F8 
                        0xFFFFFFFF, // 200151FC 
                        0xFFFFFFFF, // 20015200 
                        0xFFFCDFFF, // 20015204 
                        0xFFFFFFFF, // 20015208 
                        0xFDFFFFFF, // 2001520C 
                        0xFFFFFFFF, // 20015210 
                        0xF7FFFFFF, // 20015214 
                        0x6FFFFFFF, // 20015218 
                        0xFF7FFFFF, // 2001521C 
                        0xDFFFFFFF, // 20015220 
                        0xFFFFFF7F, // 20015224 
                        0xFFFFFFFF, // 20015228 
                        0xFFFFFFE7, // 2001522C 
                        0xFFFFFFFF, // 20015230 
                        0xFFCFFFFF, // 20015234 
                        0x8FFFFFFF, // 20015238 
                        0xFFFFFFFF, // 2001523C 
                        0x0001FFFF, // 20015240 
                        0x00000000, // 

                        0x7FFFFFFF, // 20015248 
                        0xFFFFFFFC, // 2001524C 
                        0xFFFCFFFF, // 20015250 
                        0x7E7FF9E7, // 20015254 
                        0xFF9C7EF7, // 20015258 
                        0xFFFFFFFF, // 2001525C 
                        0xFFFFFEFF, // 20015260 
                        0xF8E3E6FF, // 20015264 
                        0xFFFFFFFF, // 20015268 
                        0xFEFFFFF7, // 2001526C 
                        0xFF3CFFFF, // 20015270 
                        0x081FFFFF, // 20015274 
                        0xDFFFFFFC, // 20015278 
                        0xFFE7FFFF, // 2001527C 
                        0x39FFDFFF, // 20015280 
                        0xFFFFC090, // 20015284 
                        0xF9FFFFFF, // 20015288 
                        0xFFFFFFFF, // 2001528C 
                        0xFE3FFFFF, // 20015290 
                        0x1FF39FBF, // 20015294 
                        0xFFFFFE00, // 20015298 
                        0xBFFFFFFF, // 2001529C 
                        0x000007FF, // 100152A0 
                        0x00000000, // 

                        0x8FFFFFFF, // 200152A8 
                        0xFFFFFFFF, // 200152AC 
                        0xFFFFFFFF, // 200152B0 
                        0xF7FAFFFF, // 200152B4 
                        0xFFFFFFFF, // 200152B8 
                        0xFFFFFFFF, // 200152BC 
                        0xFFFFFFFF, // 200152C0 
                        0xFFFCDFFF, // 200152C4 
                        0xFFFFFFFF, // 200152C8 
                        0xFDFFFFFF, // 200152CC 
                        0xFFFFFFFF, // 200152D0 
                        0xF7FFFFFF, // 200152D4 
                        0x6FFFFFFF, // 200152D8 
                        0xFF7FFFFF, // 200152DC 
                        0xDFFFFFFF, // 200152E0 
                        0xFFFFFF7F, // 200152E4 
                        0xFFFFFFFF, // 200152E8 
                        0xFFFFFFE7, // 200152EC 
                        0xFFFFFFFF, // 200152F0 
                        0xFFCFFFFF, // 200152F4 
                        0x8FFFFFFF, // 200152F8 
                        0xFFFFFFFF, // 200152FC 
                        0x0001FFFF, // 20015300 
                        0x00000000, // 

                        0x7FFFFFFF, // 20015308 
                        0xFFFFFFFC, // 2001530C 
                        0xFFFCFFFF, // 20015310 
                        0x7E7FF9E7, // 20015314 
                        0xFF9C7EF7, // 20015318 
                        0xFFFFFFFF, // 2001531C 
                        0xFFFFFEFF, // 20015320 
                        0xF8E3E6FF, // 20015324 
                        0xFFFFFFFF, // 20015328 
                        0xFEFFFFF7, // 2001532C 
                        0xFF3CFFFF, // 20015330 
                        0x081FFFFF, // 20015334 
                        0xDFFFFFFC, // 20015338 
                        0xFFE7FFFF, // 2001533C 
                        0x39FFDFFF, // 20015340 
                        0xFFFFC090, // 20015344 
                        0xF9FFFFFF, // 20015348 
                        0xFFFFFFFF, // 2001534C 
                        0xFE3FFFFF, // 20015350 
                        0x1FF39FBF, // 20015354 
                        0xFFFFFE00, // 20015358 
                        0xBFFFFFFF, // 2001535C 
                        0x000007FF, // 10015360 
                        0x00000000, // 
                    };
                #endregion
                for (int i = 0; i < payload.Length; i++)
                    Array.Copy(BitConverter.GetBytes(payload[i]), 0, sav, sv + 0x5400 + 0x15068 + 4 * i, 4);
            }

            // Language Writing
            for (int i = 0; i < 0x9D; i++)
                Array.Copy(BitConverter.GetBytes(0xFFFFFFFF), 0, sav ,sv + 0x5400 + 0x15400 + 4 * i, 4);
            Array.Copy(BitConverter.GetBytes(0x007FFFFF), 0, sav ,sv + 0x5400 + 0x15674, 4);

            // Encounter Count Writing (999*all species)
            for (int i = 0; i < 0x2D1; i++)
                Array.Copy(BitConverter.GetBytes(999), 0, sav, sv + 0x5400 + 0x15686 + i * 2, 2);
            // Turn off Italian Petlil
            sav[0x1A9DF + sv] &= 0xFE;

            // Forms Bool Writing
            for (int i = 0; i < 0x9C; i++)
                sav[sv + 0x5400 + 0x15000 + 0x364 + i] = 0xFF;
            sav[sv + 0x5400 + 0x15000 + 0x3FF] = 0x8F; // make sure we don't have FF because CGSE may screw up.

            editing = true;
            Setup();
            editing = false;
            LB_Species.SelectedIndex = index; // restore selection
        }

        private void changeEncounteredCount(object sender, EventArgs e)
        {
            if (!editing)
                Array.Copy(BitConverter.GetBytes(Math.Min(0xFFFF, Util.ToUInt32(MT_Count))), 0, sav, dexoffset + 0x686 + (Util.getIndex(CB_Species) - 1) * 2, 2);
        }
    }
}
