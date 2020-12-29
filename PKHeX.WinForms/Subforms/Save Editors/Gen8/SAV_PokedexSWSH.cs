using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SAV_PokedexSWSH : Form
    {
        private readonly SAV8SWSH Origin;
        private readonly SAV8SWSH SAV;
        private readonly Zukan8 Dex;
        private readonly CheckBox[] CL;
        private readonly CheckedListBox[] CHK;
        private readonly IReadOnlyList<Zukan8EntryInfo> Indexes;

        private int lastIndex = -1;
        private readonly bool CanSave;
        private readonly bool Loading;

        public SAV_PokedexSWSH(SAV8SWSH sav)
        {
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            SAV = (SAV8SWSH)(Origin = sav).Clone();
            Dex = SAV.Blocks.Zukan;
            var indexes = Zukan8.GetRawIndexes(PersonalTable.SWSH, Dex.GetRevision());
            var speciesNames = GameInfo.Strings.Species;
            Indexes = indexes.OrderBy(z => z.GetEntryName(speciesNames)).ToArray();
            CL = new[] {CHK_L1, CHK_L2, CHK_L3, CHK_L4, CHK_L5, CHK_L6, CHK_L7, CHK_L8, CHK_L9};
            CHK = new[] {CLB_1, CLB_2, CLB_3, CLB_4};

            Loading = true;
            // Clear Listbox and ComboBox
            LB_Species.Items.Clear();
            CB_Species.Items.Clear();
            foreach (var c in CHK)
            {
                c.Items.Clear();
                for (int j = 0; j < 63; j++)
                    c.Items.Add($"{j:00} - N/A");
                c.Items.Add("Gigantamax (0)");
            }

            // Fill List
            CB_Species.InitializeBinding();
            var species = GameInfo.FilteredSources.Species.Where(z => Dex.DexLookup.ContainsKey(z.Value)).ToArray();
            CB_Species.DataSource = new BindingSource(species, null);

            var names = Indexes.Select(z => z.GetEntryName(speciesNames) + (Dex.DexLookup[z.Species].DexType == z.Entry.DexType ? string.Empty : "***"));
            foreach (var n in names)
                LB_Species.Items.Add(n);

            Loading = false;
            LB_Species.SelectedIndex = 0;
            CB_Species.KeyDown += WinFormsUtil.RemoveDropCB;
            CanSave = true;
        }

        private void ChangeCBSpecies(object sender, EventArgs e)
        {
            if (Loading)
                return;

            var species = WinFormsUtil.GetIndex(CB_Species);
            if (!Dex.DexLookup.TryGetValue(species, out var info))
                throw new ArgumentException(nameof(species));

            var index = info.AbsoluteIndex - 1;
            if (LB_Species.SelectedIndex != index)
                LB_Species.SelectedIndex = index; // trigger event
        }

        private void ChangeLBSpecies(object sender, EventArgs e)
        {
            if (Loading || LB_Species.SelectedIndex < 0)
                return;

            SetEntry(lastIndex);
            lastIndex = LB_Species.SelectedIndex;
            GetEntry(lastIndex);
        }

        private void GetEntry(int index)
        {
            var entry = Indexes[index].Entry;
            if (entry.DexType == Zukan8Type.None)
                return;

            var species = Indexes[index].Species;
            var forms = GetFormList(species);
            if (forms[0].Length == 0)
                forms[0] = GameInfo.Strings.Types[0];

            for (int i = 0; i < CHK.Length; i++)
            {
                var c = CHK[i];
                for (int j = 0; j < 64; j++)
                {
                    if (j < 63)
                        c.Items[j] = $"{j:00} - {(j < forms.Length ? forms[j] : "N/A")}";
                    var val = Dex.GetSeenRegion(entry, j, i);
                    c.SetItemChecked(j, val);
                }

                if (species == (int) Species.Urshifu)
                {
                    c.Items[62] = $"Gmax-{forms[1]}";
                    c.Items[63] = $"Gmax-{forms[0]}";
                }
                else
                {
                    c.Items[63] = "Gigantamax";
                }
            }

            for (int i = 0; i < CL.Length; i++)
                CL[i].Checked = Dex.GetIsLanguageIndexObtained(entry, i);

            NUD_Form.Value = Dex.GetFormDisplayed(entry);

            CHK_Caught.Checked = Dex.GetCaught(entry);
            CHK_Gigantamaxed.Checked = Dex.GetCaughtGigantamaxed(entry);
            CHK_G.Checked = Dex.GetDisplayDynamaxInstead(entry);
            CHK_S.Checked = Dex.GetDisplayShiny(entry);
            CB_Gender.SelectedIndex = (int)Dex.GetGenderDisplayed(entry);

            if (species == (int) Species.Urshifu)
            {
                CHK_Gigantamaxed1.Visible = true;
                CHK_Gigantamaxed1.Checked = Dex.GetCaughtGigantamax1(entry);
            }
            else
            {
                CHK_Gigantamaxed1.Visible = false;
            }

            NUD_Battled.Value = Dex.GetBattledCount(entry);
        }

        private static string[] GetFormList(in int species)
        {
            var s = GameInfo.Strings;
            if (species == (int)Species.Alcremie)
                return FormConverter.GetAlcremieFormList(s.forms);
            return FormConverter.GetFormList(species, s.Types, s.forms, GameInfo.GenderSymbolASCII, 8).ToArray();
        }

        private void SetEntry(int index)
        {
            if (!CanSave || Loading || index < 0)
                return;

            var entry = Indexes[index].Entry;
            if (entry.DexType == Zukan8Type.None)
                return;

            for (int i = 0; i < CHK.Length; i++)
            {
                var c = CHK[i];
                for (int j = 0; j < 64; j++)
                {
                    var val = c.GetItemChecked(j);
                    Dex.SetSeenRegion(entry, j, i, val);
                }
            }

            for (int i = 0; i < CL.Length; i++)
                Dex.SetIsLanguageIndexObtained(entry, i, CL[i].Checked);

            Dex.SetFormDisplayed(entry, (uint)NUD_Form.Value);

            Dex.SetCaught(entry, CHK_Caught.Checked);
            Dex.SetCaughtGigantamax(entry, CHK_Gigantamaxed.Checked);
            Dex.SetGenderDisplayed(entry, (uint)CB_Gender.SelectedIndex);
            Dex.SetDisplayDynamaxInstead(entry, CHK_G.Checked);
            Dex.SetDisplayShiny(entry, CHK_S.Checked);

            Dex.SetBattledCount(entry, (uint)NUD_Battled.Value);

            if (CHK_Gigantamaxed1.Visible)
                Dex.SetCaughtGigantamax1(entry, CHK_Gigantamaxed1.Checked);
        }

        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void B_Save_Click(object sender, EventArgs e)
        {
            SetEntry(lastIndex);
            Origin.CopyChangesFrom(SAV);
            Close();
        }

        private void B_GiveAll_Click(object sender, EventArgs e)
        {
            SetEntry(lastIndex);
            bool shiny = ModifierKeys == Keys.Shift;
            var species = Indexes[lastIndex].Species;
            Dex.SetDexEntryAll(species, shiny);
            System.Media.SystemSounds.Asterisk.Play();
            GetEntry(lastIndex);
        }

        private void B_Modify_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            modifyMenu.Show(btn.PointToScreen(new Point(0, btn.Height)));
        }

        private void SeenNone(object sender, EventArgs e)
        {
            SetEntry(lastIndex);
            Dex.SeenNone();
            System.Media.SystemSounds.Asterisk.Play();
            GetEntry(lastIndex);
        }

        private void SeenAll(object sender, EventArgs e)
        {
            SetEntry(lastIndex);
            bool shiny = ModifierKeys == Keys.Shift;
            Dex.SeenAll(shiny);
            System.Media.SystemSounds.Asterisk.Play();
            GetEntry(lastIndex);
        }

        private void CaughtNone(object sender, EventArgs e)
        {
            SetEntry(lastIndex);
            Dex.CaughtNone();
            System.Media.SystemSounds.Asterisk.Play();
            GetEntry(lastIndex);
        }

        private void CaughtAll(object sender, EventArgs e)
        {
            SetEntry(lastIndex);
            bool shiny = ModifierKeys == Keys.Shift;
            Dex.CaughtAll(shiny);
            System.Media.SystemSounds.Asterisk.Play();
            GetEntry(lastIndex);
        }

        private void CompleteDex(object sender, EventArgs e)
        {
            SetEntry(lastIndex);
            bool shiny = ModifierKeys == Keys.Shift;
            Dex.CompleteDex(shiny);
            System.Media.SystemSounds.Asterisk.Play();
            GetEntry(lastIndex);
        }

        private void ChangeAllCounts(object sender, EventArgs e)
        {
            SetEntry(lastIndex);
            Dex.SetAllBattledCount((uint)NUD_Battled.Value);
            System.Media.SystemSounds.Asterisk.Play();
            GetEntry(lastIndex);
        }
    }
}
