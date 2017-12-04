using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SAV_PokedexSM : Form
    {
        private readonly SaveFile Origin;
        private readonly SAV7 SAV;
        public SAV_PokedexSM(SaveFile sav)
        {
            SAV = (SAV7)(Origin = sav).Clone();
            InitializeComponent();
            CP = new[] { CHK_P1, CHK_P2, CHK_P3, CHK_P4, CHK_P5, CHK_P6, CHK_P7, CHK_P8, CHK_P9, };
            CL = new[] { CHK_L1, CHK_L2, CHK_L3, CHK_L4, CHK_L5, CHK_L6, CHK_L7, CHK_L8, CHK_L9, };
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);

            editing = true;
            // Clear Listbox and ComboBox
            LB_Species.Items.Clear();
            CB_Species.Items.Clear();
            LB_Forms.Items.Clear();

            // Fill List
            CB_Species.DisplayMember = "Text";
            CB_Species.ValueMember = "Value";
            CB_Species.DataSource = new BindingSource(GameInfo.SpeciesDataSource.Skip(1).ToList(), null);

            for (int i = 1; i < SAV.MaxSpeciesID + 1; i++)
                LB_Species.Items.Add($"{i:000} - {GameInfo.Strings.specieslist[i]}");

            // Add Formes
            int ctr = SAV.MaxSpeciesID;
            baseSpecies = new List<int>();
            for (int i = 1; i < SAV.MaxSpeciesID + 1; i++)
            {
                int c = SAV.Personal[i].FormeCount;
                for (int j = 0; j < c; j++)
                {
                    int x = SAV.USUM ? SaveUtil.GetDexFormIndexUSUM(i, c, j) : SaveUtil.GetDexFormIndexSM(i, c, j);
                    if (x == -1 || j == 0)
                        continue;
                    baseSpecies.Add(i);
                    ctr++;
                    LB_Species.Items.Add($"{ctr:000} - {GameInfo.Strings.specieslist[i]}-{j}");
                }
            }

            Dex = new PokeDex7(SAV);
            editing = false;
            LB_Species.SelectedIndex = 0;
            CB_Species.KeyDown += WinFormsUtil.RemoveDropCB;
        }

        private readonly PokeDex7 Dex;
        private bool editing;
        private bool allModifying;
        private int species = -1;
        private readonly CheckBox[] CP, CL;

        private readonly List<int> baseSpecies;
        private int GetBaseSpeciesGender(int index)
        {
            // meowstic special handling
            const int meow = 678;
            if (index == meow - 1 || index >= SAV.MaxSpeciesID && baseSpecies[index - SAV.MaxSpeciesID] == meow)
                return index < SAV.MaxSpeciesID ? 0 : 254; // M : F

            if (index < SAV.MaxSpeciesID)
                return SAV.Personal[index + 1].Gender;

            index -= SAV.MaxSpeciesID;
            int spec = baseSpecies[index];
            return SAV.Personal[spec].Gender;
        }

        private void ChangeCBSpecies(object sender, EventArgs e)
        {
            if (editing) return;
            SetEntry();

            editing = true;
            species = (int)CB_Species.SelectedValue;
            LB_Species.SelectedIndex = species - 1; // Since we don't allow index0 in combobox, everything is shifted by 1
            LB_Species.TopIndex = LB_Species.SelectedIndex;
            if (!allModifying) FillLBForms();
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
            if (!allModifying) FillLBForms();
            GetEntry();
            editing = false;
        }
        private void ChangeLBForms(object sender, EventArgs e)
        {
            if (allModifying) return;
            if (editing) return;
            SetEntry();

            editing = true;
            int fspecies = LB_Species.SelectedIndex + 1;
            var bspecies = fspecies <= SAV.MaxSpeciesID ? fspecies : baseSpecies[fspecies - SAV.MaxSpeciesID - 1];
            int form = LB_Forms.SelectedIndex;
            if (form > 0)
            {
                int fc = SAV.Personal[bspecies].FormeCount;
                if (fc > 1) // actually has forms
                {
                    int f = SAV.USUM ? SaveUtil.GetDexFormIndexUSUM(bspecies, fc, SAV.MaxSpeciesID - 1) : SaveUtil.GetDexFormIndexSM(bspecies, fc, SAV.MaxSpeciesID - 1);
                    if (f >= 0) // bit index valid
                        species = f + form + 1;
                    else
                        species = bspecies;
                }
                else
                    species = bspecies;
            }
            else species = bspecies;
            CB_Species.SelectedValue = species;
            LB_Species.SelectedIndex = species - 1;
            LB_Species.TopIndex = LB_Species.SelectedIndex;
            GetEntry();
            editing = false;
        }
        private bool FillLBForms()
        {
            if (allModifying) return false;
            LB_Forms.DataSource = null;
            LB_Forms.Items.Clear();

            int fspecies = LB_Species.SelectedIndex + 1;
            var bspecies = fspecies <= SAV.MaxSpeciesID ? fspecies : baseSpecies[fspecies - SAV.MaxSpeciesID - 1];
            bool hasForms = SAV.Personal[bspecies].HasFormes || new[] { 201, 664, 665, 414 }.Contains(bspecies);
            LB_Forms.Enabled = hasForms;
            if (!hasForms) return false;
            var ds = PKX.GetFormList(bspecies, GameInfo.Strings.types, GameInfo.Strings.forms, Main.GenderSymbols, SAV.Generation).ToList();
            if (ds.Count == 1 && string.IsNullOrEmpty(ds[0]))
            { 
                // empty
                LB_Forms.Enabled = false;
                return false;
            }

            LB_Forms.DataSource = ds;
            if (fspecies <= SAV.MaxSpeciesID)
                LB_Forms.SelectedIndex = 0;
            else
            {
                int fc = SAV.Personal[bspecies].FormeCount;
                if (fc <= 1)
                    return true;

                int f = SAV.USUM ? SaveUtil.GetDexFormIndexUSUM(bspecies, fc, SAV.MaxSpeciesID - 1) : SaveUtil.GetDexFormIndexSM(bspecies, fc, SAV.MaxSpeciesID - 1);
                if (f < 0)
                    return true; // bit index valid

                if (f > fspecies - LB_Forms.Items.Count - 1)
                    LB_Forms.SelectedIndex = fspecies - f - 1;
                else
                    LB_Forms.SelectedIndex = -1;
            }
            return true;
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

        private void GetEntry()
        {
            int pk = species - 1;
            editing = true;
            CHK_P1.Enabled = species <= SAV.MaxSpeciesID;
            CHK_P1.Checked = CHK_P1.Enabled && Dex.Owned[pk];

            int gt = GetBaseSpeciesGender(LB_Species.SelectedIndex);

            CHK_P2.Enabled = CHK_P4.Enabled = CHK_P6.Enabled = CHK_P8.Enabled = gt != 254; // Not Female-Only
            CHK_P3.Enabled = CHK_P5.Enabled = CHK_P7.Enabled = CHK_P9.Enabled = gt != 0 && gt != 255; // Not Male-Only and Not Genderless

            for (int i = 0; i < 4; i++)
                CP[i + 1].Checked = Dex.Seen[i][pk];

            for (int i = 0; i < 4; i++)
                CP[i + 5].Checked = Dex.Displayed[i][pk];

            for (int i = 0; i < 9; i++)
            {
                CL[i].Enabled = species <= SAV.MaxSpeciesID;
                CL[i].Checked = CL[i].Enabled && Dex.LanguageFlags[pk*9 + i];
            }
            editing = false;
        }
        private void SetEntry()
        {
            if (species <= 0) 
                return;

            int pk = species - 1;

            for (int i = 0; i < 4; i++)
                Dex.Seen[i][pk] = CP[i + 1].Checked;

            for (int i = 0; i < 4; i++)
                Dex.Displayed[i][pk] = CP[i + 5].Checked;

            if (species > SAV.MaxSpeciesID)
                return;

            Dex.Owned[pk] = CHK_P1.Checked;

            for (int i = 0; i < 9; i++)
                Dex.LanguageFlags[pk*9 + i] = CL[i].Checked;
        }

        private sealed class PokeDex7
        {
            public readonly bool[] Owned;
            public readonly bool[][] Seen = new bool[4][];
            public readonly bool[][] Displayed = new bool[4][];
            public readonly bool[] LanguageFlags;

            private const int MiscLen = 0x80;
            private const int OwnedLen = 0x68;
            private const int SeenDispLen = 0x8C;
            private const int LanguageLen = 0x398;
            internal PokeDex7(SAV7 SAV)
            {
                if (SAV.Generation != 7)
                    return;

                int ofs = SAV.PokeDex + 0x8 + MiscLen;
                Owned = SetBits(SAV.Data, ofs, OwnedLen);

                ofs += OwnedLen;
                for (int i = 0; i < 4; i++)
                {
                    Seen[i] = SetBits(SAV.Data, ofs, SeenDispLen);
                    ofs += SeenDispLen;
                }
                for (int i = 0; i < 4; i++)
                {
                    Displayed[i] = SetBits(SAV.Data, ofs, SeenDispLen);
                    ofs += SeenDispLen;
                }
                LanguageFlags = SetBits(SAV.Data, SAV.PokeDexLanguageFlags, LanguageLen);
            }
            internal void WriteToSAV(SAV7 SAV)
            {
                if (SAV.Generation != 7)
                    return;

                int ofs = SAV.PokeDex + 0x8 + MiscLen;
                SetBits(Owned).CopyTo(SAV.Data, ofs);

                ofs += OwnedLen;
                for (int i = 0; i < 4; i++)
                {
                    SetBits(Seen[i]).CopyTo(SAV.Data, ofs);
                    ofs += SeenDispLen;
                }
                for (int i = 0; i < 4; i++)
                {
                    SetBits(Displayed[i]).CopyTo(SAV.Data, ofs);
                    ofs += SeenDispLen;
                }
                SetBits(LanguageFlags).CopyTo(SAV.Data, SAV.PokeDexLanguageFlags);
            }

            private static bool[] SetBits(byte[] data, int offset, int length)
            {
                byte[] d = new byte[length];
                Array.Copy(data, offset, d, 0, length);
                bool[] b = new bool[8*d.Length];
                for (int i = 0; i < b.Length; i++)
                    b[i] = (d[i/8] & 1 << (i&7)) != 0;
                return b;
            }
            private static byte[] SetBits(bool[] b)
            {
                byte[] data = new byte[b.Length/8];
                for (int i = 0; i < b.Length; i++)
                    data[i/8] |= (byte)(b[i] ? 1 << (i&7) : 0);
                return data;
            }
        }

        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void B_Save_Click(object sender, EventArgs e)
        {
            SetEntry();
            Dex.WriteToSAV(SAV);

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
                CHK_L7.Checked = 
                CHK_L8.Checked = 
                CHK_L9.Checked = ModifierKeys != Keys.Control;
            }
            if (CHK_P1.Enabled)
            {
                CHK_P1.Checked = ModifierKeys != Keys.Control;
            }
            int gt = GetBaseSpeciesGender(LB_Species.SelectedIndex);

            CHK_P2.Checked = CHK_P4.Checked = gt != 254 && ModifierKeys != Keys.Control;
            CHK_P3.Checked = CHK_P5.Checked = gt != 0 && gt != 255 && ModifierKeys != Keys.Control;

            if (ModifierKeys == Keys.Control)
                foreach (var chk in new[] { CHK_P6, CHK_P7, CHK_P8, CHK_P9 })
                    chk.Checked = false;
            else if (!(CHK_P6.Checked || CHK_P7.Checked || CHK_P8.Checked || CHK_P9.Checked))
                (gt != 254 ? CHK_P6 : CHK_P7).Checked = true;
        }
        private void B_Modify_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            modifyMenu.Show(btn.PointToScreen(new Point(0, btn.Height)));
        }
        private void ModifyAll(object sender, EventArgs e)
        {
            allModifying = true;
            LB_Forms.Enabled = LB_Forms.Visible = false;
            bool USUM = SAV.USUM;
            int lang = SAV.Language;
            if (lang > 5) lang -= 1;
            lang -= 1;

            if (sender == mnuSeenAll || sender == mnuCaughtAll || sender == mnuComplete)
                SetAll(sender, USUM, lang);
            else
                ClearAll(sender);

            SetEntry();
            // Turn off zh2 Petilil
            Dex.LanguageFlags[548 * 9 + 8] = false;
            GetEntry();
            allModifying = false;
            LB_Forms.Enabled = LB_Forms.Visible = true;
            LB_Species.SelectedIndex = 0;
            System.Media.SystemSounds.Asterisk.Play();
        }
        private void ClearAll(object sender)
        {
            for (int i = 0; i < LB_Species.Items.Count; i++)
            {
                LB_Species.SelectedIndex = i;
                foreach (CheckBox chk in CL)
                    chk.Checked = false;
                CHK_P1.Checked = false; // not caught
                if (sender == mnuCaughtNone)
                    continue;
                // remove seen/displayed
                CHK_P2.Checked = CHK_P4.Checked = CHK_P3.Checked = CHK_P5.Checked = false;
                CHK_P6.Checked = CHK_P7.Checked = CHK_P8.Checked = CHK_P9.Checked = false;
            }
        }
        private void SetAll(object sender, bool USUM, int lang)
        {
            for (int i = 0; i < SAV.MaxSpeciesID; i++)
            {
                int spec = i + 1;
                var gt = GetBaseSpeciesGender(i);

                // Set base species flags
                LB_Species.SelectedIndex = i;
                SetSeen(sender, gt, false);
                if (sender != mnuSeenAll)
                    SetCaught(sender, gt, lang, false);

                // Set forme flags
                var entries = GetAllFormEntries(spec, USUM).Where(z => z >= SAV.MaxSpeciesID).Distinct();
                foreach (var f in entries)
                {
                    LB_Species.SelectedIndex = f;
                    SetSeen(sender, gt, true);
                    if (sender != mnuSeenAll)
                        SetCaught(sender, gt, lang, true);
                }
            }
        }
        private IEnumerable<int> GetAllFormEntries(int spec, bool USUM)
        {
            var fc = SAV.Personal[spec].FormeCount;
            for (int j = 1; j < fc; j++)
            {
                int start = j;
                int end = j;
                if (SAV7.SanitizeFormsToIterate(spec, out int s, out int n, j, USUM))
                {
                    start = s;
                    end = n;
                }
                start = Math.Max(1, start);
                for (int f = start; f <= end; f++)
                {
                    int x = SAV.USUM ? SaveUtil.GetDexFormIndexUSUM(spec, fc, f) : SaveUtil.GetDexFormIndexSM(spec, fc, f);
                    if (x >= 0)
                        yield return SAV.MaxSpeciesID - 1 + x;
                }
            }
        }

        private void SetCaught(object sender, int gt, int lang, bool isForm)
        {
            CHK_P1.Checked = mnuCaughtNone != sender;
            for (int j = 0; j < CL.Length; j++)
                CL[j].Checked = CL[j].Enabled && (sender == mnuComplete || (mnuCaughtNone != sender && j == lang));

            if (mnuCaughtNone == sender)
            {
                if (isForm)
                    return;
                if (!(CHK_P2.Checked || CHK_P3.Checked || CHK_P4.Checked || CHK_P5.Checked)) // if seen
                    if (!(CHK_P6.Checked || CHK_P7.Checked || CHK_P8.Checked || CHK_P9.Checked)) // not displayed
                        (gt != 254 ? CHK_P6 : CHK_P7).Checked = true; // check one

                return;
            }

            if (mnuComplete == sender)
            {
                // Seen All
                foreach (var chk in new[] {CHK_P2, CHK_P3, CHK_P4, CHK_P5})
                    chk.Checked = chk.Enabled;
            }
            else
            {
                // ensure at least one SEEN
                if (!(CHK_P2.Checked || CHK_P3.Checked || CHK_P4.Checked || CHK_P5.Checked))
                    (gt != 254 ? CHK_P2 : CHK_P3).Checked = true;
            }

            // ensure at least one Displayed except for formes
            if (isForm)
                return;
            if (!(CHK_P6.Checked || CHK_P7.Checked || CHK_P8.Checked || CHK_P9.Checked))
                (gt != 254 ? CHK_P6 : CHK_P7).Checked = CHK_P1.Enabled;
        }

        private void SetSeen(object sender, int gt, bool isForm)
        {
            foreach (CheckBox t in new[] {CHK_P2, CHK_P3, CHK_P4, CHK_P5})
                t.Checked = mnuSeenNone != sender && t.Enabled;

            if (mnuSeenNone != sender)
            {
                // ensure at least one Displayed except for formes
                if (isForm)
                    return;
                if (!(CHK_P6.Checked || CHK_P7.Checked || CHK_P8.Checked || CHK_P9.Checked))
                    (gt != 254 ? CHK_P6 : CHK_P7).Checked = true;
            }
            else
            {
                foreach (CheckBox t in CP)
                    t.Checked = false;
            }

            if (!CHK_P1.Checked)
                foreach (CheckBox t in CL)
                    t.Checked = false;
        }
    }
}
