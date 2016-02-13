using System;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PKHeX
{
    public partial class SAV_PokedexORAS : Form
    {
        public SAV_PokedexORAS()
        {
            InitializeComponent();
            CP = new[] { CHK_P1, CHK_P2, CHK_P3, CHK_P4, CHK_P5, CHK_P6, CHK_P7, CHK_P8, CHK_P9, };
            CL = new[] { CHK_L1, CHK_L2, CHK_L3, CHK_L4, CHK_L5, CHK_L6, CHK_L7, };
            Util.TranslateInterface(this, Main.curlanguage);

            editing = true;
            // Clear Listbox and ComboBox
            LB_Species.Items.Clear();
            CB_Species.Items.Clear();

            // Fill List
            var species_list = Util.getCBList(Main.specieslist, null);
            species_list.RemoveAt(0); // Remove 0th Entry
            CB_Species.DisplayMember = "Text";
            CB_Species.ValueMember = "Value";
            CB_Species.DataSource = species_list;

            for (int i = 1; i < Main.specieslist.Length; i++)
                LB_Species.Items.Add(i.ToString("000") + " - " + Main.specieslist[i]);

            getBools();
            editing = false;
            LB_Species.SelectedIndex = 0;
            TB_Spinda.Text = BitConverter.ToUInt32(sav, Main.SAV.Spinda).ToString("X8");
        }

        private readonly CheckBox[] CP;
        private readonly CheckBox[] CL;
        private readonly byte[] sav = (byte[])Main.SAV.Data.Clone();
        private readonly bool[,] specbools = new bool[9, 0x60 * 8];
        private readonly bool[,] langbools = new bool[7, 0x60 * 8];
        private bool editing;
        private int species = -1;

        private void changeCBSpecies(object sender, EventArgs e)
        {
            if (editing) return;
            setBools();

            editing = true;
            species = (int)CB_Species.SelectedValue;
            LB_Species.SelectedIndex = species - 1; // Since we don't allow index0 in combobox, everything is shifted by 1
            LB_Species.TopIndex = LB_Species.SelectedIndex;
            loadchks();
            editing = false;
        }
        private void changeLBSpecies(object sender, EventArgs e)
        {
            if (editing) return;
            setBools();

            editing = true;
            species = LB_Species.SelectedIndex + 1;
            CB_Species.SelectedValue = species;
            loadchks();
            editing = false;
        }
        private void loadchks()
        {
            // Load Bools for the data
            int pk = species;

            L_Spinda.Visible = TB_Spinda.Visible = pk == 327;

            // Load Partitions
            for (int i = 0; i < 9; i++)
                CP[i].Checked = specbools[i, pk-1];
            for (int i = 0; i < 7; i++)
                CL[i].Checked = langbools[i, pk-1];
            
            CHK_P1.Enabled = true;

            int index = LB_Species.SelectedIndex + 1;
            int gt = PKX.Personal[index].Gender;

            CHK_P2.Enabled = CHK_P4.Enabled = CHK_P6.Enabled = CHK_P8.Enabled = gt != 254; // Not Female-Only
            CHK_P3.Enabled = CHK_P5.Enabled = CHK_P7.Enabled = CHK_P9.Enabled = gt != 0 && gt != 255; // Not Male-Only and Not Genderless

            for (int i = 0; i < 7; i++)
                CL[i].Enabled = true;

            // Load Encountered Count
            editing = true;
            MT_Count.Text = BitConverter.ToUInt16(sav, Main.SAV.EncounterCount + (pk - 1) * 2).ToString();
            editing = false;
        }
        private void removedropCB(object sender, KeyEventArgs e)
        {
            ((ComboBox)sender).DroppedDown = false;
        }
        private void changeDisplayed(object sender, EventArgs e)
        {
            if (!(sender as CheckBox).Checked) 
                return;

            CHK_P6.Checked = sender == CHK_P6;
            CHK_P7.Checked = sender == CHK_P7;
            CHK_P8.Checked = sender == CHK_P8;
            CHK_P9.Checked = sender == CHK_P9;

            CHK_P2.Checked |= CHK_P6.Checked;
            CHK_P3.Checked |= CHK_P7.Checked;
            CHK_P4.Checked |= CHK_P8.Checked;
            CHK_P5.Checked |= CHK_P9.Checked;
        }
        private void changeEncountered(object sender, EventArgs e)
        {
            if (!(CHK_P2.Checked || CHK_P3.Checked || CHK_P4.Checked || CHK_P5.Checked))
                CHK_P6.Checked = CHK_P7.Checked = CHK_P8.Checked = CHK_P9.Checked = false;
            else if (!(CHK_P6.Checked || CHK_P7.Checked || CHK_P8.Checked || CHK_P9.Checked))
            {
                if (sender == CHK_P2 && CHK_P2.Checked)
                    CHK_P6.Checked = true;
                else if (sender == CHK_P3 && CHK_P3.Checked)
                    CHK_P7.Checked = true;
                else if (sender == CHK_P4 && CHK_P4.Checked)
                    CHK_P8.Checked = true;
                else if (sender == CHK_P5 && CHK_P5.Checked)
                    CHK_P9.Checked = true;
            }
        }

        private void setBools()
        {
            if (species < 0)
                return;

            specbools[0, species - 1] = CHK_P1.Checked;
            specbools[1, species - 1] = CHK_P2.Checked;
            specbools[2, species - 1] = CHK_P3.Checked;
            specbools[3, species - 1] = CHK_P4.Checked;
            specbools[4, species - 1] = CHK_P5.Checked;
            specbools[5, species - 1] = CHK_P6.Checked;
            specbools[6, species - 1] = CHK_P7.Checked;
            specbools[7, species - 1] = CHK_P8.Checked;
            specbools[8, species - 1] = CHK_P9.Checked;

            langbools[0, species - 1] = CHK_L1.Checked;
            langbools[1, species - 1] = CHK_L2.Checked;
            langbools[2, species - 1] = CHK_L3.Checked;
            langbools[3, species - 1] = CHK_L4.Checked;
            langbools[4, species - 1] = CHK_L5.Checked;
            langbools[5, species - 1] = CHK_L6.Checked;
            langbools[6, species - 1] = CHK_L7.Checked;
        }

        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void B_Save_Click(object sender, EventArgs e)
        {
            setBools();
            saveChanges();

            // Return back to the parent savefile
            Array.Copy(sav, Main.SAV.Data, sav.Length);
            Main.SAV.Edited = true;
            Close();
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

                sdata.CopyTo(sav, Main.SAV.PokeDex + 8 + 0x60 * p);
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

                ldata.CopyTo(sav, Main.SAV.PokeDexLanguageFlags);
            }
            
            // Store Spinda Spot
            uint PID = Util.getHEXval(TB_Spinda.Text);
            BitConverter.GetBytes(PID).CopyTo(sav, Main.SAV.Spinda);
        }

        private void getBools()
        {
            // Fill Bit arrays
            for (int i = 0; i < 9; i++)
            {
                byte[] data = new byte[0x60];
                int offset = Main.SAV.PokeDex + 0x8 + 0x60 * i;
                Array.Copy(sav, offset, data, 0, 0x60);
                BitArray BitRegion = new BitArray(data);
                for (int b = 0; b < 0x60 * 8; b++)
                    specbools[i, b] = BitRegion[b];
            }

            // Fill Language arrays
            byte[] langdata = new byte[0x280];
            Array.Copy(sav, Main.SAV.PokeDexLanguageFlags, langdata, 0, 0x280);
            BitArray LangRegion = new BitArray(langdata);
            for (int b = 0; b < 721; b++) // 721 Species
                for (int i = 0; i < 7; i++) // 7 Languages
                    langbools[i, b] = LangRegion[7 * b + i];
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
                CHK_L7.Checked = ModifierKeys != Keys.Control;
            }
            if (CHK_P1.Enabled)
            {
                CHK_P1.Checked = ModifierKeys != Keys.Control;
            }
            int index = LB_Species.SelectedIndex+1;
            int gt = PKX.Personal[index].Gender;

            CHK_P2.Checked = CHK_P4.Checked = gt != 254 && ModifierKeys != Keys.Control;
            CHK_P3.Checked = CHK_P5.Checked = gt != 0 && gt != 255 && ModifierKeys != Keys.Control;

            if (ModifierKeys == Keys.Control)
                foreach (var chk in new[]{CHK_P6, CHK_P7, CHK_P8, CHK_P9})
                    chk.Checked = false;
            else if (!(CHK_P6.Checked || CHK_P7.Checked || CHK_P8.Checked || CHK_P9.Checked))
                (gt != 254 ? CHK_P6 : CHK_P7).Checked = true;

            int dexNav = Util.ToInt32(MT_Count.Text);
            if (dexNav == 0)
                MT_Count.Text = 1.ToString();
        }
        private void B_Modify_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            modifyMenu.Show(btn.PointToScreen(new Point(0, btn.Height)));
        }
        private void modifyAll(object sender, EventArgs e)
        {
            int lang = Main.SAV.Language;
            if (lang > 5) lang -= 1;
            lang -= 1;

            if (new[] { mnuSeenNone, mnuSeenAll, mnuComplete }.Contains(sender))
                    for (int i = 0; i < CB_Species.Items.Count; i++)
                {
                    int gt = PKX.Personal[i].Gender;
                    CB_Species.SelectedIndex = i;
                    foreach (CheckBox t in new[] {CHK_P2, CHK_P3, CHK_P4, CHK_P5})
                        t.Checked = mnuSeenNone != sender && t.Enabled;

                    if (mnuSeenNone != sender)
                    {
                        // if seen ensure at least one Displayed
                        if (!new[] {CHK_P6.Checked, CHK_P7.Checked, CHK_P8.Checked, CHK_P8.Checked}.Any(b => b))
                            (gt != 254 ? CHK_P6 : CHK_P7).Checked = true;
                    }
                    else
                    {
                        for (int f = 0; f < 0x98; f++)
                            sav[Main.SAV.PokeDex + 0x368 + f] = 0;
                        foreach (CheckBox t in CP)
                            t.Checked = false;
                    }

                    if (!new[] { CHK_P6.Checked, CHK_P7.Checked, CHK_P8.Checked, CHK_P8.Checked }.Any(b=> b))
                        foreach (CheckBox t in CL)
                            t.Checked = false;
                }

            if (new[] {mnuCaughtNone, mnuCaughtAll, mnuComplete}.Contains(sender))
                for (int i = 0; i < CB_Species.Items.Count; i++)
                {
                    int gt = PKX.Personal[i].Gender;
                    CB_Species.SelectedIndex = i;
                    foreach (CheckBox t in new[] {CHK_P1})
                        t.Checked = mnuCaughtNone != sender;
                    for (int j = 0; j < CL.Length; j++)
                        CL[j].Checked = sender == mnuComplete || (mnuCaughtNone != sender && j == lang);

                    if (mnuCaughtNone == sender)
                    {
                        if (new[] { CHK_P1.Checked }.Any(b=> b)) // if seen, ensure at least one displayed
                            if (!new[] { CHK_P6.Checked, CHK_P7.Checked, CHK_P8.Checked, CHK_P8.Checked }.Any(b=> b))
                                (gt != 254 ? CHK_P6 : CHK_P7).Checked = true;
                    }
                    if (mnuCaughtNone != sender)
                    {
                        // ensure at least one SEEN
                        if (!new[] { CHK_P2.Checked, CHK_P3.Checked, CHK_P4.Checked, CHK_P5.Checked }.Any(b=> b))
                            (gt != 254 ? CHK_P2 : CHK_P3).Checked = true;
                        // ensure at least one Displayed
                        if (!new[] { CHK_P6.Checked, CHK_P7.Checked, CHK_P8.Checked, CHK_P8.Checked }.Any(b=> b))
                            (gt != 254 ? CHK_P6 : CHK_P7).Checked = true;
                    }
                    else
                        foreach (CheckBox t in CL)
                            t.Checked = false;
                }


            setBools();
            saveChanges();
            if (mnuComplete == sender)
            {
                // Forms Bool Writing
                for (int i = 0; i < 0x98; i++)
                    sav[Main.SAV.PokeDex + 0x368 + i] = 0xFF;

                // Turn off Italian Petlil
                sav[Main.SAV.PokeDexLanguageFlags + 0x1DF] &= 0xFE;
            }
            if (mnuDexNav == sender)
            {
                for (int i = 0; i < 0x2D1; i++)
                    BitConverter.GetBytes((ushort)999).CopyTo(sav, Main.SAV.EncounterCount + i * 2);
            }
            if (mnuResetNav == sender)
            {
                for (int i = 0; i < 0x2D1; i++)
                    BitConverter.GetBytes((ushort)0).CopyTo(sav, Main.SAV.EncounterCount + i * 2);
            }
            
            getBools();
            loadchks();
        }

        private void changeEncounteredCount(object sender, EventArgs e)
        {
            if (!editing)
                BitConverter.GetBytes((ushort)Math.Min(0xFFFF, Util.ToUInt32(MT_Count.Text))).CopyTo(sav, Main.SAV.EncounterCount + (Util.getIndex(CB_Species) - 1) * 2);
        }
    }
}
