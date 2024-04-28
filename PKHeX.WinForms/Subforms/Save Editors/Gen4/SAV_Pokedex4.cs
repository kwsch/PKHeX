using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;
using static PKHeX.Core.Zukan4;

namespace PKHeX.WinForms;

public partial class SAV_Pokedex4 : Form
{
    private readonly SaveFile Origin;
    private readonly SAV4 SAV;

    public SAV_Pokedex4(SAV4 sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        CL = [CHK_L1, CHK_L2, CHK_L3, CHK_L5, CHK_L4, CHK_L6]; // JPN,ENG,FRA,GER,ITA,SPA
        SAV = (SAV4)(Origin = sav).Clone();

        editing = true;
        // Clear Listbox and ComboBox
        LB_Species.Items.Clear();
        CB_Species.Items.Clear();

        // Fill List
        CB_Species.InitializeBinding();
        var filtered = GameInfo.FilteredSources;
        CB_Species.DataSource = new BindingSource(filtered.Species.Skip(1).ToList(), null);

        for (int i = 1; i < SAV.MaxSpeciesID + 1; i++)
            LB_Species.Items.Add($"{i:000} - {GameInfo.Strings.specieslist[i]}");

        editing = false;
        LB_Species.SelectedIndex = 0;

        string[] dexMode = ["not given", "simple mode", "detect forms", "national dex", "other languages"];
        if (SAV is SAV4HGSS) dexMode = dexMode.Where((_, i) => i != 2).ToArray();
        foreach (string mode in dexMode)
            CB_DexUpgraded.Items.Add(mode);
        if (SAV.DexUpgraded < CB_DexUpgraded.Items.Count)
            CB_DexUpgraded.SelectedIndex = SAV.DexUpgraded;

        CB_Species.KeyDown += WinFormsUtil.RemoveDropCB;
    }

    private readonly CheckBox[] CL;
    private bool editing;
    private ushort species = ushort.MaxValue;
    private const int LangCount = 6; // No Korean

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

    private void GetEntry()
    {
        var dex = SAV.Dex;

        CHK_Caught.Checked = dex.GetCaught(species);
        CHK_Seen.Checked = dex.GetSeen(species);

        // Genders
        LoadGenders(CHK_Seen.Checked);

        // Forms
        LoadForms();

        // Language
        LoadLanguage();
    }

    private void LoadLanguage()
    {
        var dex = SAV.Dex;
        if (dex.HasLanguage(species))
        {
            for (int i = 0; i < LangCount; i++)
                CL[i].Checked = dex.GetLanguageBitIndex(species, i);
        }
        else
        {
            GB_Language.Enabled = false;
            for (int i = 0; i < LangCount; i++)
                CL[i].Checked = false;
        }
    }

    private void LoadForms()
    {
        LB_Form.Items.Clear();
        LB_NForm.Items.Clear();

        var forms = SAV.Dex.GetForms(species);
        if (forms.Length == 0)
            return;

        string[] formNames = GetFormNames4Dex(species);

        var seen = forms.Where(z => z != FORM_NONE && z < forms.Length).Distinct().Select((_, i) => formNames[forms[i]]).ToArray();
        var not = formNames.Except(seen).ToArray();

        LB_Form.Items.AddRange(seen);
        LB_NForm.Items.AddRange(not);
    }

    private void LoadGenders(bool seen)
    {
        var dex = SAV.Dex;
        var first = seen ? LB_Gender : LB_NGender;
        var second = dex.GetSeenSingleGender(species) ? LB_NGender : first;

        LB_Gender.Items.Clear();
        LB_NGender.Items.Clear();
        var pi = SAV.Personal[species];
        var gr = pi.Gender;
        switch (gr)
        {
            case PersonalInfo.RatioMagicGenderless:
                first.Items.Add(GENDERLESS);
                break;
            case PersonalInfo.RatioMagicMale:
                first.Items.Add(MALE);
                break;
            case PersonalInfo.RatioMagicFemale:
                first.Items.Add(FEMALE);
                break;
            default:
                var firstFem = dex.GetSeenGenderFirst(species) == 1;
                first.Items.Add(firstFem ? FEMALE : MALE);
                second.Items.Add(firstFem ? MALE : FEMALE);
                break;
        }
    }

    private void SetEntry()
    {
        if (species > 493)
            return;

        var dex = SAV.Dex;
        dex.SetCaught(species, CHK_Caught.Checked);
        dex.SetSeen(species, CHK_Seen.Checked);
        dex.SetSeenGenderNeither(species);
        if (LB_Gender.Items.Count != 0)
        {
            var femaleFirst = LB_Gender.Items[0].ToString() == FEMALE;
            var firstGender = femaleFirst ? (byte)1: (byte)0;
            dex.SetSeenGenderNewFlag(species, firstGender);
            if (LB_Gender.Items.Count != 1)
                dex.SetSeenGenderSecond(species, firstGender ^ 1);
        }

        if (dex.HasLanguage(species))
        {
            for (int i = 0; i < LangCount; i++)
                dex.SetLanguageBitIndex(species, i, CL[i].Checked);
        }

        var forms = SAV.Dex.GetForms(species);
        if (forms.Length != 0)
        {
            var items = LB_Form.Items;
            Span<byte> arr = stackalloc byte[items.Count];
            string[] formNames = GetFormNames4Dex(species);
            for (int i = 0; i < items.Count; i++)
                arr[i] = (byte)Array.IndexOf(formNames, (string)items[i]); // shouldn't ever fail
            SAV.Dex.SetForms(species, arr);
        }
    }

    private void B_Cancel_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void B_Save_Click(object sender, EventArgs e)
    {
        SetEntry();
        int s = CB_DexUpgraded.SelectedIndex;
        if (s >= 0) SAV.DexUpgraded = s;

        Origin.CopyChangesFrom(SAV);
        Close();
    }

    private void B_GiveAll_Click(object sender, EventArgs e)
    {
        bool all = ModifierKeys != Keys.Control;
        var args = all ? SetDexArgs.Complete : SetDexArgs.None;
        SAV.Dex.ModifyAll(species, args, GetGen4LanguageBitIndex(SAV.Language));
        GetEntry();
    }

    private void B_Modify_Click(object sender, EventArgs e)
    {
        Button btn = (Button)sender;
        modifyMenu.Show(btn.PointToScreen(new Point(0, btn.Height)));
    }

    private void ModifyAll(object sender, EventArgs e)
    {
        SetEntry();

        var lang = GetGen4LanguageBitIndex(SAV.Language);
        SetDexArgs args;
        if (sender == mnuComplete)
            args = SetDexArgs.Complete;
        else if (sender == mnuCaughtAll)
            args = SetDexArgs.CaughtAll;
        else if (sender == mnuCaughtNone)
            args = SetDexArgs.CaughtNone;
        else if (sender == mnuSeenAll)
            args = SetDexArgs.SeenAll;
        else
            args = SetDexArgs.None;

        for (ushort i = 1; i <= 493; i++)
            SAV.Dex.ModifyAll(i, args, lang);

        GetEntry();
        System.Media.SystemSounds.Asterisk.Play();
    }

    private void CHK_Seen_CheckedChanged(object sender, EventArgs e)
    {
        if (!editing)
        {
            if (!CHK_Seen.Checked) // move all to none
            {
                SAV.Dex.ClearSeen(species);
                GetEntry();
            }
            else if (LB_NGender.Items.Count > 0)
            {
                int count = LB_NGender.Items.Count;
                for (int i = 0; i < count; i++)
                {
                    LB_NGender.SelectedIndex = 0;
                    ToggleSeen(B_GLeft, e);
                }
                int count2 = LB_NForm.Items.Count;
                for (int i = 0; i < count2; i++)
                {
                    LB_NForm.SelectedIndex = 0;
                    ToggleForm(B_FLeft, e);
                }
            }
        }
        LB_Gender.Enabled = LB_NGender.Enabled = LB_Form.Enabled = LB_NForm.Enabled = CHK_Seen.Checked;
        CHK_Caught.Enabled = CHK_Seen.Checked;
    }

    private void ToggleSeen(object sender, EventArgs e)
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
        ArgumentNullException.ThrowIfNull(item);
        lb.Items.RemoveAt(lb.SelectedIndex);
        var dest = lb == LB_Gender ? LB_NGender : LB_Gender;
        dest.Items.Add(item);
        dest.SelectedIndex = dest.Items.Count - 1;
    }

    private void MoveGender(object sender, EventArgs e)
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
        ArgumentNullException.ThrowIfNull(item);
        lb.Items.Remove(item);
        lb.Items.Insert(newIndex, item);
        lb.SelectedIndex = newIndex;
    }

    private void ToggleForm(object sender, EventArgs e)
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
        ArgumentNullException.ThrowIfNull(item);
        lb.Items.RemoveAt(lb.SelectedIndex);
        var dest = lb == LB_Form ? LB_NForm : LB_Form;
        dest.Items.Add(item);
        dest.SelectedIndex = dest.Items.Count - 1;
    }

    private void MoveForm(object sender, EventArgs e)
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
        ArgumentNullException.ThrowIfNull(item);
        lb.Items.Remove(item);
        lb.Items.Insert(newIndex, item);
        lb.SelectedIndex = newIndex;
    }
}
