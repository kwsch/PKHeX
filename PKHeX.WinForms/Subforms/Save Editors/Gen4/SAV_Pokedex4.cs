using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SAV_Pokedex4 : Form
    {
        private readonly SaveFile Origin;
        private readonly SAV4 SAV;
        public SAV_Pokedex4(SaveFile sav)
        {
            SAV = (SAV4)(Origin = sav).Clone();
            InitializeComponent();
            CL = new[] { CHK_L1, CHK_L2, CHK_L3, CHK_L5, CHK_L4, CHK_L6, }; // JPN,ENG,FRA,GER,ITA,SPA
            WinFormsUtil.TranslateInterface(this, Main.curlanguage);

            editing = true;
            // Clear Listbox and ComboBox
            LB_Species.Items.Clear();
            CB_Species.Items.Clear();

            // Fill List
            CB_Species.DisplayMember = "Text";
            CB_Species.ValueMember = "Value";
            CB_Species.DataSource = new BindingSource(GameInfo.SpeciesDataSource.Skip(1).Where(id => id.Value <= SAV.MaxSpeciesID).ToList(), null);

            for (int i = 1; i < SAV.MaxSpeciesID + 1; i++)
                LB_Species.Items.Add(i.ToString("000") + " - " + GameInfo.Strings.specieslist[i]);

            editing = false;
            LB_Species.SelectedIndex = 0;

            string[] dexMode = { "not given", "simple mode", "detect forms", "national dex", "other languages" };
            if (SAV.HGSS) dexMode = dexMode.Where((t, i) => i != 2).ToArray();
            foreach (string mode in dexMode)
                CB_DexUpgraded.Items.Add(mode);
            if (SAV.DexUpgraded < CB_DexUpgraded.Items.Count)
                CB_DexUpgraded.SelectedIndex = SAV.DexUpgraded;
        }

        private readonly CheckBox[] CL;
        private bool editing;
        private int species = -1;
        private const int brSize = 0x40;
        private const int LangCount = 6; // No Korean

        private void changeCBSpecies(object sender, EventArgs e)
        {
            if (editing) return;
            setEntry();

            editing = true;
            species = (int)CB_Species.SelectedValue;
            LB_Species.SelectedIndex = species - 1; // Since we don't allow index0 in combobox, everything is shifted by 1
            LB_Species.TopIndex = LB_Species.SelectedIndex;
            getEntry();
            editing = false;
        }
        private void changeLBSpecies(object sender, EventArgs e)
        {
            if (editing) return;
            setEntry();

            editing = true;
            species = LB_Species.SelectedIndex + 1;
            CB_Species.SelectedValue = species;
            getEntry();
            editing = false;
        }
        private void removedropCB(object sender, KeyEventArgs e)
        {
            ((ComboBox)sender).DroppedDown = false;
        }

        private const string GENDERLESS = "Genderless";
        private const string MALE = "Male";
        private const string FEMALE = "Female";
        private static readonly int[] DPLangSpecies = { 23, 25, 54, 77, 120, 129, 202, 214, 215, 216, 228, 278, 287, 315 };
        private void getEntry()
        {
            // Load Bools for the data
            int bit = species - 1;
            byte mask = (byte)(1 << (bit & 7));
            int ofs = SAV.PokeDex + (bit >> 3) + 0x4;
            int FormOffset1 = SAV.PokeDex + 4 + brSize * 4 + 4;
            int PokeDexLanguageFlags = FormOffset1 + (SAV.HGSS ? 0x3C : 0x20);
            int l_ofs = !SAV.DP ? species : 1 + Array.IndexOf(DPLangSpecies, species);
            if (l_ofs > 0)
            {
                l_ofs += PokeDexLanguageFlags;
                for (int i = 0; i < LangCount; i++)
                {
                    CL[i].Enabled = true;
                    CL[i].Checked = (SAV.Data[l_ofs] & (byte)(1 << i)) != 0;
                }
            }
            else
                for (int i = 0; i < LangCount; i++) CL[i].Enabled = CL[i].Checked = false;

            bool bit2 = (SAV.Data[ofs + brSize * 2] & mask) != 0;
            bool bit3 = (SAV.Data[ofs + brSize * 3] & mask) != 0;

            CHK_Seen.Checked = (SAV.Data[ofs + brSize * 1] & mask) != 0;
            CHK_Caught.Checked = (SAV.Data[ofs + brSize * 0] & mask) != 0;

            // Genders
            LB_Gender.Items.Clear();
            LB_NGender.Items.Clear();

            var active = CHK_Seen.Checked ? LB_Gender : LB_NGender;
            var inactive = LB_NGender;
            var other = bit2 ^ bit3 ? active : inactive;

            switch (SAV.Personal[species].Gender)
            {
                case 255: // Genderless
                    active.Items.Add(GENDERLESS);
                    break;
                case 0:
                    active.Items.Add(MALE);
                    break;
                case 254:
                    active.Items.Add(FEMALE);
                    break;
                default:
                    active.Items.Add(bit2 ? FEMALE : MALE);
                    other.Items.Add(bit2 ? MALE : FEMALE);
                    break;
            }

            // Forms
            LB_Form.Items.Clear();
            LB_NForm.Items.Clear();

            var forms = SAV.getForms(species);
            if (forms == null)
                return;

            string[] formNames = PKX.getFormList(species, GameInfo.Strings.types, GameInfo.Strings.forms,
                Main.gendersymbols, 4);
            var seen = forms.Where(z => (byte)z != 0xFF).Select((v, i) => formNames[forms[i]]).ToArray();
            var not = formNames.Where(z => !seen.Contains(z)).ToArray();

            LB_Form.Items.AddRange(seen);
            LB_NForm.Items.AddRange(not);
        }

        private void setEntry()
        {
            if (species < 0)
                return;

            int bit = species - 1;
            byte mask = (byte)(1 << (bit & 7));
            int ofs = SAV.PokeDex + (bit >> 3) + 0x4;

            // Check if already Seen
            if (!CHK_Seen.Checked || LB_Gender.Items.Count == 0)
            {
                SAV.Data[ofs + brSize * 0] &= (byte)~mask;
                SAV.Data[ofs + brSize * 1] &= (byte)~mask;
                SAV.Data[ofs + brSize * 2] &= (byte)~mask;
                SAV.Data[ofs + brSize * 3] &= (byte)~mask;
            }
            else // Is Seen
            {
                // Set the Species Owned Flag
                if (CHK_Caught.Checked)
                    SAV.Data[ofs + brSize * 0] |= mask;
                else
                    SAV.Data[ofs + brSize * 0] &= (byte)~mask;

                SAV.Data[ofs + brSize * 1] |= mask;
                switch ((string)LB_Gender.Items[0])
                {
                    case GENDERLESS:
                        SAV.Data[ofs + brSize * 2] &= (byte)~mask;
                        SAV.Data[ofs + brSize * 3] &= (byte)~mask;
                        break;
                    case FEMALE:
                        SAV.Data[ofs + brSize * 2] |= mask; // set
                        if (LB_Gender.Items.Count != 1) // Male present
                            SAV.Data[ofs + brSize * 3] &= (byte)~mask; // unset
                        else
                            SAV.Data[ofs + brSize * 3] |= mask; // set
                        break;
                    case MALE:
                        SAV.Data[ofs + brSize * 2] &= (byte)~mask; // unset
                        if (LB_Gender.Items.Count != 1) // Female present
                            SAV.Data[ofs + brSize * 3] |= mask; // set
                        else
                            SAV.Data[ofs + brSize * 3] &= (byte)~mask; // unset
                        break;
                    default:
                        throw new ArgumentException("Invalid Gender???");
                }
            }

            int FormOffset1 = SAV.PokeDex + 4 + 4 * brSize + 4;
            int PokeDexLanguageFlags = FormOffset1 + (SAV.HGSS ? 0x3C : 0x20);
            int l_ofs = !SAV.DP ? species : (1 + Array.IndexOf(DPLangSpecies, species));
            if (l_ofs > 0)
            {
                l_ofs += PokeDexLanguageFlags;
                for (int i = 0; i < LangCount; i++)
                {
                    if (CL[i].Checked)
                        SAV.Data[l_ofs] |= (byte)(1 << i);
                    else
                        SAV.Data[l_ofs] &= (byte)~(1 << i);
                }
            }

            var forms = SAV.getForms(species);
            if (forms != null)
            {
                int[] arr = new int[LB_Form.Items.Count];
                string[] formNames = PKX.getFormList(species, GameInfo.Strings.types, GameInfo.Strings.forms, Main.gendersymbols, 4);
                for (int i = 0; i < LB_Form.Items.Count; i++)
                    arr[i] = Array.IndexOf(formNames, (string)LB_Form.Items[i]);
                SAV.setForms(species, arr);
            }

            editing = false;
        }

        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void B_Save_Click(object sender, EventArgs e)
        {
            setEntry();
            int s = CB_DexUpgraded.SelectedIndex;
            if (s >= 0) SAV.DexUpgraded = s;

            Origin.setData(SAV.Data, 0);
            Close();
        }

        private void B_GiveAll_Click(object sender, EventArgs e)
        {
            if (GB_Language.Enabled)
            {
                CHK_L1.Checked =
                CHK_L2.Checked =
                CHK_L3.Checked =
                CHK_L4.Checked =
                CHK_L5.Checked =
                CHK_L6.Checked = ModifierKeys != Keys.Control;
            }
            CHK_Caught.Checked = CHK_Seen.Checked = ModifierKeys != Keys.Control;

            if (ModifierKeys == Keys.Control)
                seenNone();
            else
                seenAll();
        }
        private void B_Modify_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            modifyMenu.Show(btn.PointToScreen(new Point(0, btn.Height)));
        }

        private void seenNone()
        {
            LB_NGender.Items.AddRange(LB_Gender.Items);
            LB_Gender.Items.Clear();

            LB_NForm.Items.AddRange(LB_Form.Items);
            LB_Form.Items.Clear();
            CHK_Seen.Checked = false;
            foreach (var c in CL)
                c.Checked = false;
        }
        private void seenAll()
        {
            LB_Gender.Items.AddRange(LB_NGender.Items);
            LB_NGender.Items.Clear();

            LB_Form.Items.AddRange(LB_NForm.Items);
            LB_NForm.Items.Clear();
            CHK_Seen.Checked = true;
        }
        private void modifyAll(object sender, EventArgs e)
        {
            int lang = SAV.Language;
            if (lang > 5 || lang < 0) // KOR or Invalid
                lang = 0;

            bool seenA = sender == mnuSeenAll || sender == mnuCaughtAll || sender == mnuComplete;
            bool seenN = sender == mnuSeenNone;
            bool caughtA = sender == mnuCaughtAll || sender == mnuComplete;
            bool caughtN = sender == mnuCaughtNone || sender == mnuSeenNone;

            for (int i = 0; i < LB_Species.Items.Count; i++)
            {
                LB_Species.SelectedIndex = i;

                if (seenN) // move all to none
                    seenNone();
                else if (seenA) // move all to seen
                    seenAll();

                if (caughtA)
                {
                    CHK_Caught.Checked = true;
                    for (int j = 0; j < CL.Length; j++) // set SAV language (and others if Complete)
                        CL[j].Checked = sender == mnuComplete || mnuCaughtNone != sender && j == lang;
                }
                else if (caughtN)
                    CHK_Caught.Checked = false;
                else if (!CHK_Seen.Checked)
                    foreach (CheckBox t in CL)
                        t.Checked = false;
            }

            setEntry();
            getEntry();
        }

        private void CHK_Seen_CheckedChanged(object sender, EventArgs e)
        {
            if (!editing)
            {
                if (!CHK_Seen.Checked) // move all to none
                {
                    CHK_Caught.Checked = false;
                    seenNone();
                }
                else if (LB_NGender.Items.Count > 0)
                {
                    int count = LB_NGender.Items.Count;
                    for (int i = 0; i < count; i++)
                    {
                        LB_NGender.SelectedIndex = 0;
                        toggleSeen(B_GLeft, e);
                    }
                    int count2 = LB_NForm.Items.Count;
                    for (int i = 0; i < count2; i++)
                    {
                        LB_NForm.SelectedIndex = 0;
                        toggleForm(B_FLeft, e);
                    }
                }
            }
            LB_Gender.Enabled = LB_NGender.Enabled = LB_Form.Enabled = LB_NForm.Enabled = CHK_Seen.Checked;
            CHK_Caught.Enabled = CHK_Seen.Checked;
        }

        private void toggleSeen(object sender, EventArgs e)
        {
            if (editing)
                return;
            var lb = sender == B_GLeft ? LB_NGender : LB_Gender;
            if (lb == null || lb.SelectedIndex < 0)
            {
                WinFormsUtil.Alert("No Gender selected.");
                return;
            }

            var item = lb.SelectedItem;
            lb.Items.RemoveAt(lb.SelectedIndex);
            var dest = lb == LB_Gender ? LB_NGender : LB_Gender;
            dest.Items.Add(item);
            dest.SelectedIndex = dest.Items.Count - 1;
        }
        private void moveGender(object sender, EventArgs e)
        {
            if (editing)
                return;
            var lb = LB_Gender;
            if (lb == null || lb.SelectedIndex < 0)
            {
                WinFormsUtil.Alert("No Gender selected.");
                return;
            }

            int index = lb.SelectedIndex;
            int delta = sender == B_GUp ? -1 : 1;

            if (index == 0 && lb.Items.Count == 1)
                return;

            int newIndex = index + delta;
            if (newIndex < 0)
                return;
            if (newIndex >= lb.Items.Count)
                return;

            var item = lb.SelectedItem;
            lb.Items.Remove(item);
            lb.Items.Insert(newIndex, item);
            lb.SelectedIndex = newIndex;
        }

        private void toggleForm(object sender, EventArgs e)
        {
            if (editing)
                return;
            var lb = sender == B_FLeft ? LB_NForm : LB_Form;
            if (lb == null || lb.SelectedIndex < 0)
            {
                WinFormsUtil.Alert("No Form selected.");
                return;
            }

            var item = lb.SelectedItem;
            lb.Items.RemoveAt(lb.SelectedIndex);
            var dest = lb == LB_Form ? LB_NForm : LB_Form;
            dest.Items.Add(item);
            dest.SelectedIndex = dest.Items.Count - 1;
        }

        private void moveForm(object sender, EventArgs e)
        {
            if (editing)
                return;
            var lb = LB_Form;
            if (lb == null || lb.SelectedIndex < 0)
            {
                WinFormsUtil.Alert("No Form selected.");
                return;
            }

            int index = lb.SelectedIndex;
            int delta = sender == B_FUp ? -1 : 1;

            if (index == 0 && lb.Items.Count == 1)
                return;

            int newIndex = index + delta;
            if (newIndex < 0)
                return;
            if (newIndex >= lb.Items.Count)
                return;

            var item = lb.SelectedItem;
            lb.Items.Remove(item);
            lb.Items.Insert(newIndex, item);
            lb.SelectedIndex = newIndex;
        }
    }
}
