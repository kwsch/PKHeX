using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public partial class SAV_Pokedex5 : Form
{
    private readonly SaveFile Origin;
    private readonly SAV5 SAV;
    private readonly Zukan5 Dex;

    public SAV_Pokedex5(SAV5 sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (SAV5)(Origin = sav).Clone();
        Dex = SAV.Zukan;
        CP = [CHK_P1, CHK_P2, CHK_P3, CHK_P4, CHK_P5, CHK_P6, CHK_P7, CHK_P8, CHK_P9];
        CL = [CHK_L1, CHK_L2, CHK_L3, CHK_L4, CHK_L5, CHK_L6, CHK_L7];

        editing = true;
        // Clear Listbox and ComboBox
        LB_Species.Items.Clear();
        CB_Species.Items.Clear();

        // Fill List
        CB_Species.InitializeBinding();
        CB_Species.DataSource = new BindingSource(GameInfo.FilteredSources.Species.Skip(1).ToList(), string.Empty);

        for (int i = 1; i < SAV.MaxSpeciesID + 1; i++)
            LB_Species.Items.Add($"{i:000} - {GameInfo.Strings.Species[i]}");

        CHK_NationalDexUnlocked.Checked = Dex.IsNationalDexUnlocked;
        CHK_NationalDexActive.Checked = Dex.IsNationalDexMode;
        CHK_NationalDexUnlocked.CheckedChanged += (_, _) => CHK_NationalDexActive.Checked = CHK_NationalDexUnlocked.Checked;
        TB_PID.Text = Dex.Spinda.ToString("X8");

        editing = false;
        if (Dex.InitialSpecies is not (0 or > 649))
            CB_Species.SelectedValue = (int)Dex.InitialSpecies;
        else
            LB_Species.SelectedIndex = 0;
        CB_Species.KeyDown += WinFormsUtil.RemoveDropCB;
    }

    private readonly CheckBox[] CP;
    private readonly CheckBox[] CL;
    private bool editing;
    private ushort species = ushort.MaxValue;

    private void ChangeCBSpecies(object sender, EventArgs e)
    {
        if (editing)
            return;
        SetEntry();

        editing = true;
        species = (ushort)WinFormsUtil.GetIndex(CB_Species);
        LB_Species.SelectedIndex = species - 1; // Since we don't allow index0 in combobox, everything is shifted by 1
        LB_Species.TopIndex = LB_Species.SelectedIndex;
        GetEntry();
        editing = false;
    }

    private void ChangeLBSpecies(object sender, EventArgs e)
    {
        if (editing)
            return;
        SetEntry();

        editing = true;
        species = (ushort)(LB_Species.SelectedIndex + 1);
        CB_Species.SelectedValue = (int)species;
        GetEntry();
        editing = false;
    }

    private void ChangeDisplayed(object sender, EventArgs e)
    {
        if (!((CheckBox)sender).Checked)
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

    private void GetEntry(bool skipFormRepop = false)
    {
        // Load Bools for the data
        int pk = species;

        // Load Partitions
        CP[0].Checked = Dex.GetCaught(species);
        for (int i = 0; i < 4; i++)
            CP[i + 1].Checked = Dex.GetSeen(species, i);

        for (int i = 0; i < 4; i++)
            CP[i + 5].Checked = Dex.GetDisplayed(species, i);

        if (species > 493)
        {
            for (int i = 0; i < 7; i++)
                CL[i].Checked = false;
            GB_Language.Enabled = false;
        }
        else
        {
            for (int i = 0; i < 7; i++)
                CL[i].Checked = SAV.Zukan.GetLanguageFlag(species, i);
            GB_Language.Enabled = true;
        }

        var pi = SAV.Personal[pk];

        CHK_P2.Enabled = CHK_P4.Enabled = CHK_P6.Enabled = CHK_P8.Enabled = !pi.OnlyFemale;
        CHK_P3.Enabled = CHK_P5.Enabled = CHK_P7.Enabled = CHK_P9.Enabled = !(pi.OnlyMale || pi.Genderless);

        var (index, count) = Dex.GetFormIndex(species);
        if (skipFormRepop)
        {
            // Just re-load without changing the text.
            if (count == 0)
                return;
            for (int i = 0; i < count; i++)
            {
                CLB_FormsSeen.SetItemChecked(i, Dex.GetFormFlag(index + i, 0));
                CLB_FormsSeen.SetItemChecked(i + count, Dex.GetFormFlag(index + i, 1));
                CLB_FormDisplayed.SetItemChecked(i, Dex.GetFormFlag(index + i, 2));
                CLB_FormDisplayed.SetItemChecked(i + count, Dex.GetFormFlag(index + i, 3));
            }
            return;
        }

        CLB_FormsSeen.Items.Clear();
        CLB_FormDisplayed.Items.Clear();

        if (count == 0)
            return; // No forms to set.
        var forms = FormConverter.GetFormList(species, GameInfo.Strings.types, GameInfo.Strings.forms, Main.GenderSymbols, SAV.Context);
        if (forms.Length < 1)
            return;

        for (int i = 0; i < forms.Length; i++) // Seen
            CLB_FormsSeen.Items.Add(forms[i], Dex.GetFormFlag(index + i, 0));
        for (int i = 0; i < forms.Length; i++) // Seen Shiny
            CLB_FormsSeen.Items.Add($"* {forms[i]}", Dex.GetFormFlag(index + i, 1));

        for (int i = 0; i < forms.Length; i++) // Displayed
            CLB_FormDisplayed.Items.Add(forms[i], Dex.GetFormFlag(index + i, 2));
        for (int i = 0; i < forms.Length; i++) // Displayed Shiny
            CLB_FormDisplayed.Items.Add($"* {forms[i]}", Dex.GetFormFlag(index + i, 3));
    }

    private void SetEntry()
    {
        if (species > 649)
            return;

        Dex.SetCaught(species, CP[0].Checked);
        for (int i = 0; i < 4; i++)
            Dex.SetSeen(species, i, CP[i + 1].Checked);

        for (int i = 0; i < 4; i++)
            Dex.SetDisplayed(species, i, CP[i + 5].Checked);

        if (species <= 493)
        {
            for (int i = 0; i < 7; i++)
                Dex.SetLanguageFlag(species, i, CL[i].Checked);
        }

        var (index, count) = Dex.GetFormIndex(species);
        if (count == 0)
            return;

        for (int i = 0; i < CLB_FormsSeen.Items.Count / 2; i++) // Seen
            Dex.SetFormFlag(index + i, 0, CLB_FormsSeen.GetItemChecked(i));
        for (int i = 0; i < CLB_FormsSeen.Items.Count / 2; i++)  // Seen Shiny
            Dex.SetFormFlag(index + i, 1, CLB_FormsSeen.GetItemChecked(i + (CLB_FormsSeen.Items.Count / 2)));

        editing = true;
        for (int i = 0; i < CLB_FormDisplayed.Items.Count / 2; i++) // Displayed
            Dex.SetFormFlag(index + i, 2, CLB_FormDisplayed.GetItemChecked(i));
        for (int i = 0; i < CLB_FormDisplayed.Items.Count / 2; i++)  // Displayed Shiny
            Dex.SetFormFlag(index + i, 3, CLB_FormDisplayed.GetItemChecked(i + (CLB_FormDisplayed.Items.Count / 2)));
        editing = false;
    }

    private void B_Cancel_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void B_Save_Click(object sender, EventArgs e)
    {
        SetEntry();

        if (species is not 0)
            Dex.InitialSpecies = species;
        Dex.IsNationalDexUnlocked = CHK_NationalDexUnlocked.Checked;
        Dex.IsNationalDexMode = CHK_NationalDexActive.Checked;
        Dex.Spinda = Util.GetHexValue(TB_PID.Text);

        Origin.CopyChangesFrom(SAV);
        Close();
    }

    private void B_GiveAll_Click(object sender, EventArgs e)
    {
        SetEntry();
        var language = (LanguageID)SAV.Language;
        Dex.GiveAll(species, ModifierKeys != Keys.Alt, ModifierKeys.HasFlag(Keys.Shift), language, ModifierKeys.HasFlag(Keys.Control));
        GetEntry(skipFormRepop: true);
        System.Media.SystemSounds.Asterisk.Play();
    }

    private void B_Modify_Click(object sender, EventArgs e)
    {
        var btn = (Button)sender;
        modifyMenu.Show(btn.PointToScreen(new Point(0, btn.Height)));
    }

    private void ModifyAll(object sender, EventArgs e)
    {
        SetEntry();
        var language = (LanguageID)SAV.Language;
        if (sender == mnuSeenNone)
            Dex.SeenNone();
        if (sender == mnuSeenAll || sender == mnuComplete)
            Dex.SeenAll(shinyToo: ModifierKeys.HasFlag(Keys.Shift));
        if (sender == mnuCaughtNone)
            Dex.CaughtNone();
        if (sender == mnuCaughtAll || sender == mnuComplete)
            Dex.CaughtAll(language, allLanguages: ModifierKeys.HasFlag(Keys.Control));
        GetEntry(skipFormRepop: true);
        System.Media.SystemSounds.Asterisk.Play();
    }

    private void UpdateDisplayedForm(object sender, ItemCheckEventArgs e)
    {
        if (editing)
            return;

        // Only allow one form to be displayed if the user sets a new display value
        if (e.NewValue != CheckState.Checked)
            return;

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
        var btn = (Button)sender;
        modifyMenuForms.Show(btn.PointToScreen(new Point(0, btn.Height)));
    }

    private void ModifyAllForms(object sender, EventArgs e)
    {
        SetEntry();
        if (sender == mnuFormNone)
            Dex.ClearFormSeen();
        else if (sender == mnuForm1)
            Dex.SetFormsSeen1(shinyToo: ModifierKeys.HasFlag(Keys.Shift));
        else if (sender == mnuFormAll)
            Dex.SetFormsSeen(shinyToo: ModifierKeys.HasFlag(Keys.Shift));
        GetEntry(skipFormRepop: true);
        System.Media.SystemSounds.Asterisk.Play();
    }
}
