using System;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SAV_PokedexXY : Form
    {
        private readonly SaveFile Origin;
        private readonly SAV6 SAV;

        public SAV_PokedexXY(SaveFile sav)
        {
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            SAV = (SAV6)(Origin = sav).Clone();
            CP = new[] { CHK_P1, CHK_P2, CHK_P3, CHK_P4, CHK_P5, CHK_P6, CHK_P7, CHK_P8, CHK_P9, };
            CL = new[] { CHK_L1, CHK_L2, CHK_L3, CHK_L4, CHK_L5, CHK_L6, CHK_L7, };

            editing = true;
            // Clear Listbox and ComboBox
            LB_Species.Items.Clear();
            CB_Species.Items.Clear();

            // Fill List
            CB_Species.InitializeBinding();
            CB_Species.DataSource = new BindingSource(GameInfo.SpeciesDataSource.Skip(1).Where(id => id.Value <= SAV.MaxSpeciesID).ToList(), null);

            for (int i = 1; i < SAV.MaxSpeciesID + 1; i++)
                LB_Species.Items.Add($"{i:000} - {GameInfo.Strings.specieslist[i]}");

            GetData();
            editing = false;
            LB_Species.SelectedIndex = 0;
            TB_Spinda.Text = BitConverter.ToUInt32(SAV.Data, SAV.Spinda).ToString("X8");
            CB_Species.KeyDown += WinFormsUtil.RemoveDropCB;
        }

        private readonly CheckBox[] CP;
        private readonly CheckBox[] CL;
        private readonly bool[,] specbools = new bool[9, 0x60 * 8];
        private readonly bool[,] langbools = new bool[7, 0x60 * 8];
        private readonly bool[] foreignbools = new bool[0x52 * 8];
        private BitArray formbools;
        private bool editing;
        private int species = -1;
        private const int FormLen = 0x18;

        private void ChangeCBSpecies(object sender, EventArgs e)
        {
            if (editing) return;
            SetEntry();

            editing = true;
            species = (int)CB_Species.SelectedValue;
            LB_Species.SelectedIndex = species - 1; // Since we don't allow index0 in combobox, everything is shifted by 1
            LB_Species.TopIndex = LB_Species.SelectedIndex;
            GetEntry();
            editing = false;
        }

        private void ChangeLBSpecies(object sender, EventArgs e)
        {
            if (editing) return;
            SetEntry();

            editing = true;
            species = LB_Species.SelectedIndex + 1;
            CB_Species.SelectedValue = species;
            GetEntry();
            editing = false;
        }

        private void ChangeDisplayed(object sender, EventArgs e)
        {
            if (!((CheckBox) sender).Checked)
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

        private void ChangeEncountered(object sender, EventArgs e)
        {
            if (!(CHK_P2.Checked || CHK_P3.Checked || CHK_P4.Checked || CHK_P5.Checked))
            {
                CHK_P6.Checked = CHK_P7.Checked = CHK_P8.Checked = CHK_P9.Checked = false;
            }
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

        private void GetEntry()
        {
            // Load Bools for the data
            int pk = species;

            L_Spinda.Visible = TB_Spinda.Visible = pk == 327;

            // Load Partitions
            for (int i = 0; i < 9; i++)
                CP[i].Checked = specbools[i, pk - 1];
            for (int i = 0; i < 7; i++)
                CL[i].Checked = langbools[i, pk - 1];

            if (pk < 650) { CHK_F1.Enabled = true; CHK_F1.Checked = foreignbools[pk - 1]; }
            else { CHK_F1.Enabled = CHK_F1.Checked = false; }

            int gt = SAV.Personal[pk].Gender;

            CHK_P2.Enabled = CHK_P4.Enabled = CHK_P6.Enabled = CHK_P8.Enabled = gt != 254; // Not Female-Only
            CHK_P3.Enabled = CHK_P5.Enabled = CHK_P7.Enabled = CHK_P9.Enabled = !(gt == 0 || (gt == 255)); // Not Male-Only and Not Genderless

            CLB_FormsSeen.Items.Clear();
            CLB_FormDisplayed.Items.Clear();

            int fc = SAV.Personal[species].FormeCount;
            int f = DexFormUtil.GetDexFormIndexXY(species, fc);
            if (f < 0)
                return;
            string[] forms = PKX.GetFormList(species, GameInfo.Strings.types, GameInfo.Strings.forms, Main.GenderSymbols, SAV.Generation);
            if (forms.Length < 1)
                return;

            // 0x26 packs of bools
            for (int i = 0; i < forms.Length; i++) // Seen
                CLB_FormsSeen.Items.Add(forms[i], formbools[f + i + (0 *FormLen*8)]);
            for (int i = 0; i < forms.Length; i++) // Seen Shiny
                CLB_FormsSeen.Items.Add($"* {forms[i]}", formbools[f + i + (1 *FormLen*8)]);

            for (int i = 0; i < forms.Length; i++) // Displayed
                CLB_FormDisplayed.Items.Add(forms[i], formbools[f + i + (2 *FormLen*8)]);
            for (int i = 0; i < forms.Length; i++) // Displayed Shiny
                CLB_FormDisplayed.Items.Add($"* {forms[i]}", formbools[f + i + (3 *FormLen*8)]);
        }

        private void SetEntry()
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
            if (CHK_F1.Enabled) // species < 650 // (1-649)
                foreignbools[species - 1] = CHK_F1.Checked;

            langbools[0, species - 1] = CHK_L1.Checked;
            langbools[1, species - 1] = CHK_L2.Checked;
            langbools[2, species - 1] = CHK_L3.Checked;
            langbools[3, species - 1] = CHK_L4.Checked;
            langbools[4, species - 1] = CHK_L5.Checked;
            langbools[5, species - 1] = CHK_L6.Checked;
            langbools[6, species - 1] = CHK_L7.Checked;

            int fc = SAV.Personal[species].FormeCount;
            int f = DexFormUtil.GetDexFormIndexORAS(species, fc);
            if (f < 0)
                return;

            for (int i = 0; i < CLB_FormsSeen.Items.Count/2; i++) // Seen
                formbools[f + i + (0 *FormLen*8)] = CLB_FormsSeen.GetItemChecked(i);
            for (int i = 0; i < CLB_FormsSeen.Items.Count/2; i++)  // Seen Shiny
                formbools[f + i + (1 *FormLen*8)] = CLB_FormsSeen.GetItemChecked(i + (CLB_FormsSeen.Items.Count/2));

            editing = true;
            for (int i = 0; i < CLB_FormDisplayed.Items.Count/2; i++) // Displayed
                formbools[f + i + (2 *FormLen*8)] = CLB_FormDisplayed.GetItemChecked(i);
            for (int i = 0; i < CLB_FormDisplayed.Items.Count/2; i++)  // Displayed Shiny
                formbools[f + i + (3 *FormLen*8)] = CLB_FormDisplayed.GetItemChecked(i + (CLB_FormDisplayed.Items.Count/2));
            editing = false;
        }

        private void GetData()
        {
            // Fill Bit arrays
            for (int i = 0; i < 9; i++)
            {
                byte[] data = new byte[0x60];
                Array.Copy(SAV.Data, SAV.PokeDex + 8 + (0x60 * i), data, 0, 0x60);
                BitArray BitRegion = new BitArray(data);
                for (int b = 0; b < 0x60 * 8; b++)
                    specbools[i, b] = BitRegion[b];
            }

            // Fill Language arrays
            byte[] langdata = new byte[0x280];
            Array.Copy(SAV.Data, SAV.PokeDexLanguageFlags, langdata, 0, 0x280);
            BitArray LangRegion = new BitArray(langdata);
            for (int b = 0; b < 721; b++) // 721 Species
            {
                for (int i = 0; i < 7; i++) // 7 Languages
                    langbools[i, b] = LangRegion[(7 * b) + i];
            }

            // Fill Foreign array
            byte[] foreigndata = new byte[0x52];
            Array.Copy(SAV.Data, SAV.PokeDex + 0x64C, foreigndata, 0, 0x52);
            BitArray ForeignRegion = new BitArray(foreigndata);
            for (int b = 0; b < 0x52 * 8; b++)
                foreignbools[b] = ForeignRegion[b];

            byte[] formdata = new byte[FormLen*4];
            Array.Copy(SAV.Data, SAV.PokeDex + 0x368, formdata, 0, formdata.Length);
            formbools = new BitArray(formdata);
        }

        private void SetData()
        {
            // Save back the Species Bools
            // Return to Byte Array
            for (int p = 0; p < 9; p++)
            {
                byte[] sdata = new byte[0x60];

                for (int i = 0; i < 0x60 * 8; i++)
                {
                    if (specbools[p, i])
                        sdata[i / 8] |= (byte)(1 << i % 8);
                }

                sdata.CopyTo(SAV.Data, SAV.PokeDex + 8 + (0x60 * p));
            }

            // Build new bool array for the Languages
            {
                bool[] languagedata = new bool[0x280 * 8];
                for (int i = 0; i < 731; i++)
                {
                    for (int l = 0; l < 7; l++)
                        languagedata[(i * 7) + l] = langbools[l, i];
                }

                // Return to Byte Array
                byte[] ldata = new byte[languagedata.Length / 8];

                for (int i = 0; i < languagedata.Length; i++)
                {
                    if (languagedata[i])
                        ldata[i / 8] |= (byte)(1 << i % 8);
                }

                ldata.CopyTo(SAV.Data, SAV.PokeDexLanguageFlags);
            }

            formbools.CopyTo(SAV.Data, SAV.PokeDex + 0x368);

            // Return Foreign Array
            {
                byte[] foreigndata = new byte[0x52];
                for (int i = 0; i < 0x52 * 8; i++)
                {
                    if (foreignbools[i])
                        foreigndata[i / 8] |= (byte)(1 << i % 8);
                }

                foreigndata.CopyTo(SAV.Data, SAV.PokeDex + 0x64C);
            }

            // Store Spinda Spot
            uint PID = Util.GetHexValue(TB_Spinda.Text);
            BitConverter.GetBytes(PID).CopyTo(SAV.Data, SAV.Spinda);
        }

        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void B_Save_Click(object sender, EventArgs e)
        {
            SetEntry();
            SetData();

            // Return back to the parent savefile
            Origin.SetData(SAV.Data, 0);
            Close();
        }

        private void B_GiveAll_Click(object sender, EventArgs e)
        {
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
            if (CHK_F1.Enabled)
            {
                CHK_F1.Checked = ModifierKeys != Keys.Control;
            }
            int index = LB_Species.SelectedIndex+1;
            int gt = SAV.Personal[index].Gender;

            CHK_P2.Checked = CHK_P4.Checked = gt != 254 && ModifierKeys != Keys.Control;
            CHK_P3.Checked = CHK_P5.Checked = gt != 0 && gt != 255 && ModifierKeys != Keys.Control;

            if (ModifierKeys == Keys.Control)
            {
                foreach (var chk in new[] { CHK_P6, CHK_P7, CHK_P8, CHK_P9 })
                    chk.Checked = false;
            }
            else if (!(CHK_P6.Checked || CHK_P7.Checked || CHK_P8.Checked || CHK_P9.Checked))
            {
                (gt != 254 ? CHK_P6 : CHK_P7).Checked = true;
            }
        }

        private void B_Modify_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            modifyMenu.Show(btn.PointToScreen(new Point(0, btn.Height)));
        }

        private void ModifyAll(object sender, EventArgs e)
        {
            int lang = SAV.Language;
            if (lang > 5) lang--;
            lang--;

            if (sender == mnuSeenNone || sender == mnuSeenAll || sender == mnuComplete)
            {
                for (int i = 0; i < LB_Species.Items.Count; i++)
                {
                    int gt = SAV.Personal[i + 1].Gender;
                    LB_Species.SelectedIndex = i;
                    foreach (CheckBox t in new[] { CHK_P2, CHK_P3, CHK_P4, CHK_P5 })
                        t.Checked = mnuSeenNone != sender && t.Enabled;

                    if (mnuSeenNone != sender)
                    {
                        // if seen ensure at least one Displayed
                        if (!(CHK_P6.Checked || CHK_P7.Checked || CHK_P8.Checked || CHK_P9.Checked))
                            (gt != 254 ? CHK_P6 : CHK_P7).Checked = true;
                    }
                    else
                    {
                        foreach (CheckBox t in CP)
                            t.Checked = false;
                    }

                    if (!CHK_P1.Checked && !CHK_F1.Checked)
                    {
                        foreach (CheckBox t in CL)
                            t.Checked = false;
                    }
                }
            }

            if (sender == mnuCaughtNone || sender == mnuCaughtAll || sender == mnuComplete)
            {
                for (int i = 0; i < CB_Species.Items.Count; i++)
                {
                    int gt = SAV.Personal[i + 1].Gender;
                    LB_Species.SelectedIndex = i;
                    foreach (CheckBox t in new[] { CHK_P1, CHK_F1 })
                        t.Checked = mnuCaughtNone != sender;
                    for (int j = 0; j < CL.Length; j++)
                        CL[j].Checked = sender == mnuComplete || (mnuCaughtNone != sender && j == lang);

                    if (mnuCaughtNone == sender)
                    {
                        if (!(CHK_P2.Checked || CHK_P3.Checked || CHK_P4.Checked || CHK_P5.Checked)) // if seen
                        {
                            if (!(CHK_P6.Checked || CHK_P7.Checked || CHK_P8.Checked || CHK_P9.Checked)) // not displayed
                                (gt != 254 ? CHK_P6 : CHK_P7).Checked = true; // check one
                        }
                    }
                    if (mnuCaughtNone != sender)
                    {
                        if (mnuComplete == sender)
                        {
                            CHK_P2.Checked = CHK_P4.Checked = gt != 254; // not female only
                            CHK_P3.Checked = CHK_P5.Checked = gt != 0 && gt != 255; // not male only or genderless
                        }
                        else
                        {
                            // ensure at least one SEEN
                            if (!(CHK_P2.Checked || CHK_P3.Checked || CHK_P4.Checked || CHK_P5.Checked))
                                (gt != 254 ? CHK_P2 : CHK_P3).Checked = true;
                        }

                        // ensure at least one Displayed
                        if (!(CHK_P6.Checked || CHK_P7.Checked || CHK_P8.Checked || CHK_P9.Checked))
                            (gt != 254 ? CHK_P6 : CHK_P7).Checked = true;
                    }
                }
            }

            SetEntry();
            SetData();
            if (mnuComplete == sender) // Turn off Italian Petlil
                SAV.Data[SAV.PokeDexLanguageFlags + 0x1DF] &= 0xFE;

            GetData();
            GetEntry();
        }

        private void UpdateDisplayedForm(object sender, ItemCheckEventArgs e)
        {
            if (editing)
                return;

            // Only allow one form to be displayed if the user sets a new display value
            if (e.NewValue != CheckState.Checked) return;
            for (int i = 0; i < CLB_FormDisplayed.Items.Count; i++)
            {
                if (i != e.Index)
                    CLB_FormDisplayed.SetItemChecked(i, false);
                else if (sender == CLB_FormDisplayed)
                    CLB_FormsSeen.SetItemChecked(e.Index, true); // ensure this form is seen
            }
        }

        private void B_ModifyForms_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            modifyMenuForms.Show(btn.PointToScreen(new Point(0, btn.Height)));
        }

        private void ModifyAllForms(object sender, EventArgs e)
        {
            if (sender == mnuFormNone)
            {
                formbools = new BitArray(new bool[formbools.Length]); // reset false
                GetEntry();
                return;
            }

            for (int i = 0; i < CB_Species.Items.Count; i++)
            {
                LB_Species.SelectedIndex = i;
                if (CLB_FormsSeen.Items.Count == 0)
                    continue;

                if (sender == mnuForm1)
                {
                    if (CLB_FormsSeen.CheckedItems.Count == 0)
                        CLB_FormsSeen.SetItemChecked(0, true);

                    if (CLB_FormDisplayed.CheckedItems.Count == 0)
                        CLB_FormDisplayed.SetItemChecked(0, true);
                }
                else if (sender == mnuFormAll)
                {
                    for (int f = 0; f < CLB_FormsSeen.Items.Count; f++)
                        CLB_FormsSeen.SetItemChecked(f, true);
                    if (CLB_FormDisplayed.CheckedItems.Count == 0)
                        CLB_FormDisplayed.SetItemChecked(0, true);
                }
            }
        }
    }
}
