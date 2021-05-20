using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SAV_PokedexORAS : Form
    {
        private readonly SaveFile Origin;
        private readonly SAV6AO SAV;

        public SAV_PokedexORAS(SaveFile sav)
        {
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            SAV = (SAV6AO)(Origin = sav).Clone();
            Zukan = SAV.Zukan;
            CP = new[] { CHK_P1, CHK_P2, CHK_P3, CHK_P4, CHK_P5, CHK_P6, CHK_P7, CHK_P8, CHK_P9, };
            CL = new[] { CHK_L1, CHK_L2, CHK_L3, CHK_L4, CHK_L5, CHK_L6, CHK_L7, };

            editing = true;
            // Clear Listbox and ComboBox
            LB_Species.Items.Clear();
            CB_Species.Items.Clear();

            // Fill List
            CB_Species.InitializeBinding();
            CB_Species.DataSource = new BindingSource(GameInfo.FilteredSources.Species.Skip(1).ToList(), null);

            for (int i = 1; i < SAV.MaxSpeciesID + 1; i++)
                LB_Species.Items.Add($"{i:000} - {GameInfo.Strings.specieslist[i]}");

            editing = false;
            LB_Species.SelectedIndex = 0;
            TB_Spinda.Text = Zukan.SpindaPID.ToString("X8");
            CB_Species.KeyDown += WinFormsUtil.RemoveDropCB;
        }

        private readonly CheckBox[] CP;
        private readonly CheckBox[] CL;
        private readonly Zukan6AO Zukan;
        private bool editing;
        private int species = -1;

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

            L_Spinda.Visible = TB_Spinda.Visible = pk == (int)Species.Spinda;

            // Load Partitions
            CP[0].Checked = Zukan.GetCaught(species);
            for (int i = 0; i < 4; i++)
                CP[i + 1].Checked = Zukan.GetSeen(species, i);

            for (int i = 0; i < 4; i++)
                CP[i + 5].Checked = Zukan.GetDisplayed(species - 1, i);

            for (int i = 0; i < CL.Length; i++)
                CL[i].Checked = Zukan.GetLanguageFlag(species - 1, i);

            var pi = SAV.Personal[pk];

            CHK_P2.Enabled = CHK_P4.Enabled = CHK_P6.Enabled = CHK_P8.Enabled = !pi.OnlyFemale;
            CHK_P3.Enabled = CHK_P5.Enabled = CHK_P7.Enabled = CHK_P9.Enabled = !(pi.OnlyMale || pi.Genderless);

            MT_Count.Text = Zukan.GetEncounterCount(species - 1).ToString();

            CLB_FormsSeen.Items.Clear();
            CLB_FormDisplayed.Items.Clear();

            int fc = pi.FormCount;
            int f = DexFormUtil.GetDexFormIndexORAS(species, fc);
            if (f < 0)
                return;
            string[] forms = FormConverter.GetFormList(species, GameInfo.Strings.types, GameInfo.Strings.forms, Main.GenderSymbols, SAV.Generation);
            if (forms.Length < 1)
                return;

            for (int i = 0; i < forms.Length; i++) // Seen
                CLB_FormsSeen.Items.Add(forms[i], Zukan.GetFormFlag(f + i, 0));
            for (int i = 0; i < forms.Length; i++) // Seen Shiny
                CLB_FormsSeen.Items.Add($"* {forms[i]}", Zukan.GetFormFlag(f + i, 1));

            for (int i = 0; i < forms.Length; i++) // Displayed
                CLB_FormDisplayed.Items.Add(forms[i], Zukan.GetFormFlag(f + i, 2));
            for (int i = 0; i < forms.Length; i++) // Displayed Shiny
                CLB_FormDisplayed.Items.Add($"* {forms[i]}", Zukan.GetFormFlag(f + i, 3));
        }

        private void SetEntry()
        {
            if (species < 0)
                return;

            Zukan.SetCaught(species, CP[0].Checked);
            for (int i = 0; i < 4; i++)
                Zukan.SetSeen(species, i, CP[i + 1].Checked);
            for (int i = 0; i < 4; i++)
                Zukan.SetDisplayed(species - 1, i, CP[i + 5].Checked);

            for (int i = 0; i < CL.Length; i++)
                Zukan.SetLanguageFlag(species - 1, i, CL[i].Checked);

            ushort count = (ushort) Math.Min(0xFFFF, Util.ToUInt32(MT_Count.Text));
            Zukan.SetEncounterCount(species - 1, count);

            int fc = SAV.Personal[species].FormCount;
            int f = DexFormUtil.GetDexFormIndexORAS(species, fc);
            if (f < 0)
                return;

            var seen = CLB_FormsSeen;
            for (int i = 0; i < seen.Items.Count/2; i++) // Seen
                Zukan.SetFormFlag(f + i, 0, seen.GetItemChecked(i));
            for (int i = 0; i < seen.Items.Count/2; i++)  // Seen Shiny
                Zukan.SetFormFlag(f + i, 1, seen.GetItemChecked(i + (seen.Items.Count/2)));

            var display = CLB_FormDisplayed;
            for (int i = 0; i < display.Items.Count/2; i++) // Displayed
                Zukan.SetFormFlag(f + i, 2, display.GetItemChecked(i));
            for (int i = 0; i < display.Items.Count/2; i++)  // Displayed Shiny
                Zukan.SetFormFlag(f + i, 3, display.GetItemChecked(i + (display.Items.Count/2)));
        }

        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void B_Save_Click(object sender, EventArgs e)
        {
            SetEntry();
            Zukan.SpindaPID = Util.GetHexValue(TB_Spinda.Text);
            Origin.CopyChangesFrom(SAV);
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
            int index = LB_Species.SelectedIndex+1;
            int gt = SAV.Personal[index].Gender;

            CHK_P2.Checked = CHK_P4.Checked = gt != 254 && ModifierKeys != Keys.Control;
            CHK_P3.Checked = CHK_P5.Checked = gt is not (0 or 255) && ModifierKeys != Keys.Control;

            if (ModifierKeys == Keys.Control)
            {
                foreach (var chk in new[] { CHK_P6, CHK_P7, CHK_P8, CHK_P9 })
                    chk.Checked = false;
            }
            else if (!(CHK_P6.Checked || CHK_P7.Checked || CHK_P8.Checked || CHK_P9.Checked))
            {
                (gt != 254 ? CHK_P6 : CHK_P7).Checked = true;
            }

            for (int i = 0; i < CLB_FormsSeen.Items.Count; i++)
                CLB_FormsSeen.SetItemChecked(i, ModifierKeys != Keys.Control);
            if (CLB_FormsSeen.Items.Count > 0 && CLB_FormDisplayed.CheckedItems.Count == 0)
                CLB_FormDisplayed.SetItemChecked(0, ModifierKeys != Keys.Control);

            if (Util.ToInt32(MT_Count.Text) == 0)
                MT_Count.Text = "1";
        }

        private void B_Modify_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            modifyMenu.Show(btn.PointToScreen(new Point(0, btn.Height)));
        }

        private void ModifyAll(object sender, EventArgs e)
        {
            if (mnuDexNav == sender)
            {
                for (int i = 0; i < SAV.MaxSpeciesID; i++)
                    Zukan.SetEncounterCount(i, 999);
                return;
            }
            if (mnuResetNav == sender)
            {
                for (int i = 0; i < SAV.MaxSpeciesID; i++)
                    Zukan.SetEncounterCount(i, 0);
                return;
            }

            int lang = SAV.Language;
            if (lang > 5) lang--;
            lang--;

            if (sender == mnuSeenNone || sender == mnuSeenAll || sender == mnuComplete)
            {
                for (int i = 0; i < CB_Species.Items.Count; i++)
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

                    if (!CHK_P1.Checked)
                    {
                        foreach (CheckBox t in CL)
                            t.Checked = false;
                    }
                }
            }

            if (sender == mnuCaughtNone || sender == mnuCaughtAll || sender == mnuComplete)
            {
                for (int i = 0; i < LB_Species.Items.Count; i++)
                {
                    int gt = SAV.Personal[i + 1].Gender;
                    LB_Species.SelectedIndex = i;
                    foreach (CheckBox t in new[] { CHK_P1 })
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
                            CHK_P3.Checked = CHK_P5.Checked = gt is not (0 or 255); // not male only or genderless
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
            if (mnuComplete == sender) // Turn off Italian Petlil
                Zukan.SetLanguageFlag((int)Species.Petilil - 1, (int)LanguageID.Italian - 1, false);
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
                else // none
                {
                    for (int f = 0; f < CLB_FormsSeen.Items.Count; f++)
                        CLB_FormsSeen.SetItemChecked(f, false);
                    for (int f = 0; f < CLB_FormDisplayed.Items.Count; f++)
                        CLB_FormDisplayed.SetItemChecked(f, false);
                }
            }
        }
    }
}
