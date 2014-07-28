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
    public partial class SAV_Pokedex : Form
    {
        public SAV_Pokedex(Form1 frm1)
        {
            m_parent = frm1;
            InitializeComponent();
            Array.Copy(m_parent.savefile, sav, sav.Length); 
            savshift = m_parent.savindex*0x7F000;
            Setup();
            editing = false;
        }
        public byte[] sav = new Byte[0x100000];
        public int savshift = 0;
        public bool[,] specbools = new bool[10, 0x60 * 8];
        public bool[,] langbools = new bool[7, 0x60 * 8];
        Form1 m_parent;
        private void Setup()
        {
            // Clear Listbox and ComboBox
            LB_Species.Items.Clear();
            CB_Species.Items.Clear();

            // Fill List

            List<cbItem> species_list = (List<cbItem>)m_parent.CB_Species.DataSource;
            species_list.RemoveAt(0); // Remove 0th Entry
            CB_Species.DataSource = species_list;
            CB_Species.DisplayMember = "Text";
            CB_Species.ValueMember = "Value";

            for (int i = 1; i < m_parent.specieslist.Length; i++)
            {
                LB_Species.Items.Add(i.ToString("000") + " - " + m_parent.specieslist[i]);
            }

            // Fill Bit arrays
            for (int i = 0; i < 0xA; i++)
            {
                byte[] data = new Byte[0x60];
                Array.Copy(sav, savshift + 0x1A408 + 0x60 * i, data, 0, 0x60);
                BitArray BitRegion = new BitArray(data);
                for (int b = 0; b < (0x60 * 8); b++)
                    specbools[i, b] = BitRegion[b];
            }

            // Fill Language arrays
            byte[] langdata = new Byte[0x280]; 
                Array.Copy(sav, savshift + 0x1A7C8, langdata, 0, 0x280);
                BitArray LangRegion = new BitArray(langdata);
                for (int b = 0; b < (0x60 * 8); b++)
                    langbools[b % 7, b / 7] = LangRegion[b];
        }
        private void B_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void changeCBSpecies(object sender, EventArgs e)
        {
            if (!editing)
            {
                editing = true;
                int index = (int)CB_Species.SelectedValue;
                LB_Species.SelectedIndex = index - 1; // Since we don't allow index0 in combobox, everything is shifted by 1
                LB_Species.TopIndex = (int)(LB_Species.SelectedIndex);
                editing = false;
            }
            loadchks();
        }
        bool editing = true;
        private void changeLBSpecies(object sender, EventArgs e)
        {
            if (!editing)
            {
                editing = true;
                int index = LB_Species.SelectedIndex + 1;
                CB_Species.SelectedValue = index;
                editing = false;
            }
            loadchks();
        }
        private void loadchks()
        {
            // Load Bools for the data
            int pk = (int)((PKHeX.cbItem)(CB_Species.SelectedItem)).Value;
            CheckBox[] CP = new CheckBox[] {
                CHK_P1,CHK_P2,CHK_P3,CHK_P4,CHK_P5,CHK_P6,CHK_P7,CHK_P8,CHK_P9,CHK_P10,
            };
            CheckBox[] CL = new CheckBox[] {
                CHK_L1,CHK_L2,CHK_L3,CHK_L4,CHK_L5,CHK_L6,CHK_L7,
            };
            // Load Partitions
            for (int i = 0; i < 10; i++)
                CP[i].Checked = specbools[i, pk-1];
            for (int i = 0; i < 7; i++)
                CL[i].Checked = langbools[i, pk-1];
        }
        private void removedropCB(object sender, KeyEventArgs e)
        {
            ((ComboBox)sender).DroppedDown = false;
        }
    }
}
