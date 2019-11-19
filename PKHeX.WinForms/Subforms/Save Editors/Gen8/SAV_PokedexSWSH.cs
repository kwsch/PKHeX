using System;
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

        private int entry = -1;
        private int CurrentSpecies => Zukan8.DexLookup.FirstOrDefault(z => z.Value == entry).Key;
        private readonly bool CanSave;
        private readonly bool Loading = true;

        public SAV_PokedexSWSH(SAV8SWSH sav)
        {
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            SAV = (SAV8SWSH)(Origin = sav).Clone();
            Dex = SAV.Blocks.Zukan;
            CL = new[] {CHK_L1, CHK_L2, CHK_L3, CHK_L4, CHK_L5, CHK_L6, CHK_L7, CHK_L8, CHK_L9};
            CHK = new[] {CLB_1, CLB_2, CLB_3, CLB_4};

            // Clear Listbox and ComboBox
            LB_Species.Items.Clear();
            CB_Species.Items.Clear();
            foreach (var c in CHK)
            {
                c.Items.Clear();
                for (int j = 0; j < 63; j++)
                    c.Items.Add($"{j:00} - N/A");
                c.Items.Add("Gigantamax");
            }

            // Fill List
            CB_Species.InitializeBinding();
            var species = GameInfo.FilteredSources.Species.Where(z => Zukan8.DexLookup.ContainsKey(z.Value)).ToArray();
            CB_Species.DataSource = new BindingSource(species, null);

            var Species = GameInfo.Strings.Species;
            var names = Zukan8.GetEntryNames(Species);
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

            var spec = WinFormsUtil.GetIndex(CB_Species);
            if (!Zukan8.DexLookup.TryGetValue(spec, out var index))
                throw new ArgumentException(nameof(spec));

            --index;

            if (LB_Species.SelectedIndex != index)
                LB_Species.SelectedIndex = index; // trigger event
        }

        private void ChangeLBSpecies(object sender, EventArgs e)
        {
            if (Loading)
                return;

            SetEntry();
            entry = LB_Species.SelectedIndex + 1;
            GetEntry();
        }

        private void GetEntry()
        {
            var s = CurrentSpecies;
            if (s <= 0)
                return;

            var forms = GetFormList(s);
            if (forms[0].Length == 0)
                forms[0] = GameInfo.Strings.Types[0];

            for (int i = 0; i < CHK.Length; i++)
            {
                var c = CHK[i];
                for (int j = 0; j < 64; j++)
                {
                    if (j < 63)
                        c.Items[j] = $"{j:00} - {(j < forms.Length ? forms[j] : "N/A")}";
                    var val = Dex.GetSeenRegion(s, j, i);
                    c.SetItemChecked(j, val);
                }
            }

            for (int i = 0; i < CL.Length; i++)
                CL[i].Checked = Dex.GetIsLanguageIndexObtained(s, i);

            NUD_Form.Value = Dex.GetAltFormDisplayed(s);

            CHK_Caught.Checked = Dex.GetCaught(s);
            CHK_Gigantamaxed.Checked = Dex.GetCaughtGigantamaxed(s);
            CHK_G.Checked = Dex.GetDisplayDynamaxInstead(s);
            CHK_S.Checked = Dex.GetDisplayShiny(s);
            CB_Gender.SelectedIndex = (int)Dex.GetGenderDisplayed(s);

            NUD_Battled.Value = Dex.GetBattledCount(s);
        }

        private static string[] GetFormList(in int species)
        {
            var s = GameInfo.Strings;
            if (species == (int)Species.Alcremie)
                return FormConverter.GetAlcremieFormList(s.forms);
            return FormConverter.GetFormList(species, s.Types, s.forms, GameInfo.GenderSymbolASCII, 8).ToArray();
        }

        private void SetEntry()
        {
            if (!CanSave || Loading)
                return;

            var s = CurrentSpecies;
            if (s <= 0)
                return;

            for (int i = 0; i < CHK.Length; i++)
            {
                var c = CHK[i];
                for (int j = 0; j < 64; j++)
                {
                    var val = c.GetItemChecked(j);
                    Dex.SetSeenRegion(s, j, i, val);
                }
            }

            for (int i = 0; i < CL.Length; i++)
                Dex.SetIsLanguageIndexObtained(s, i, CL[i].Checked);

            Dex.SetAltFormDisplayed(s, (uint)NUD_Form.Value);

            Dex.SetCaught(s, CHK_Caught.Checked);
            Dex.SetCaughtGigantamax(s, CHK_Gigantamaxed.Checked);
            Dex.SetGenderDisplayed(s, (uint)CB_Gender.SelectedIndex);
            Dex.SetDisplayDynamaxInstead(s, CHK_G.Checked);
            Dex.SetDisplayShiny(s, CHK_S.Checked);

            Dex.SetBattledCount(s, (uint)NUD_Battled.Value);
        }

        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void B_Save_Click(object sender, EventArgs e)
        {
            SetEntry();
            Origin.CopyChangesFrom(SAV);
            Close();
        }

        private void B_GiveAll_Click(object sender, EventArgs e)
        {
            SetEntry();
            bool shiny = ModifierKeys == Keys.Shift;
            Dex.SetDexEntryAll(CurrentSpecies, shiny);
            System.Media.SystemSounds.Asterisk.Play();
            GetEntry();
        }

        private void B_Modify_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            modifyMenu.Show(btn.PointToScreen(new Point(0, btn.Height)));
        }

        private void SeenNone(object sender, EventArgs e)
        {
            SetEntry();
            Dex.SeenNone();
            System.Media.SystemSounds.Asterisk.Play();
            GetEntry();
        }

        private void SeenAll(object sender, EventArgs e)
        {
            SetEntry();
            bool shiny = ModifierKeys == Keys.Shift;
            Dex.SeenAll(shiny);
            System.Media.SystemSounds.Asterisk.Play();
            GetEntry();
        }

        private void CaughtNone(object sender, EventArgs e)
        {
            SetEntry();
            Dex.CaughtNone();
            System.Media.SystemSounds.Asterisk.Play();
            GetEntry();
        }

        private void CaughtAll(object sender, EventArgs e)
        {
            SetEntry();
            bool shiny = ModifierKeys == Keys.Shift;
            Dex.CaughtAll(shiny);
            System.Media.SystemSounds.Asterisk.Play();
            GetEntry();
        }

        private void CompleteDex(object sender, EventArgs e)
        {
            SetEntry();
            bool shiny = ModifierKeys == Keys.Shift;
            Dex.CompleteDex(shiny);
            System.Media.SystemSounds.Asterisk.Play();
            GetEntry();
        }

        private void ChangeAllCounts(object sender, EventArgs e)
        {
            SetEntry();
            Dex.SetAllBattledCount((uint)NUD_Battled.Value);
            System.Media.SystemSounds.Asterisk.Play();
            GetEntry();
        }
    }
}
