using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public partial class SAV_PokedexBDSP : Form
{
    private readonly SaveFile Origin;
    private readonly SAV8BS SAV;
    private readonly Zukan8b Zukan;

    public SAV_PokedexBDSP(SAV8BS sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (SAV8BS)(Origin = sav).Clone();
        Zukan = SAV.Zukan;

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
        CB_Species.KeyDown += WinFormsUtil.RemoveDropCB;
        CHK_National.Checked = Zukan.HasNationalDex;
    }

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

    private void GetEntry()
    {
        // Load Bools for the data
        CB_State.SelectedIndex = (int)Zukan.GetState(species);
        Zukan.GetGenderFlags(species, out var m, out var f, out var ms, out var fs);
        CHK_M.Checked = m;
        CHK_F.Checked = f;
        CHK_MS.Checked = ms;
        CHK_FS.Checked = fs;

        CHK_LangJPN.Checked = Zukan.GetLanguageFlag(species, (int)LanguageID.Japanese);
        CHK_LangENG.Checked = Zukan.GetLanguageFlag(species, (int)LanguageID.English);
        CHK_LangFRE.Checked = Zukan.GetLanguageFlag(species, (int)LanguageID.French);
        CHK_LangITA.Checked = Zukan.GetLanguageFlag(species, (int)LanguageID.Italian);
        CHK_LangGER.Checked = Zukan.GetLanguageFlag(species, (int)LanguageID.German);
        CHK_LangSPA.Checked = Zukan.GetLanguageFlag(species, (int)LanguageID.Spanish);
        CHK_LangKOR.Checked = Zukan.GetLanguageFlag(species, (int)LanguageID.Korean);
        CHK_LangCHS.Checked = Zukan.GetLanguageFlag(species, (int)LanguageID.ChineseS);
        CHK_LangCHT.Checked = Zukan.GetLanguageFlag(species, (int)LanguageID.ChineseT);

        var f1 = CLB_FormRegular;
        var f2 = CLB_FormShiny;
        f1.Items.Clear();
        f2.Items.Clear();
        var fc = Zukan8b.GetFormCount(species);
        if (fc <= 0)
            return;

        var forms = FormConverter.GetFormList(species, GameInfo.Strings.types, GameInfo.Strings.forms, Main.GenderSymbols, SAV.Context).Take(fc).ToArray();
        f1.Items.AddRange(forms);
        f2.Items.AddRange(forms);
        for (byte i = 0; i < f1.Items.Count; i++)
        {
            f1.SetItemChecked(i, Zukan.GetHasFormFlag(species, i, false));
            f2.SetItemChecked(i, Zukan.GetHasFormFlag(species, i, true));
        }
    }

    private void SetEntry()
    {
        if (species > 493)
            return;

        Zukan.SetState(species, (ZukanState8b)CB_State.SelectedIndex);
        Zukan.SetGenderFlags(species, CHK_M.Checked, CHK_F.Checked, CHK_MS.Checked, CHK_FS.Checked);

        Zukan.SetLanguageFlag(species, (int)LanguageID.Japanese, CHK_LangJPN.Checked);
        Zukan.SetLanguageFlag(species, (int)LanguageID.English, CHK_LangENG.Checked);
        Zukan.SetLanguageFlag(species, (int)LanguageID.French, CHK_LangFRE.Checked);
        Zukan.SetLanguageFlag(species, (int)LanguageID.Italian, CHK_LangITA.Checked);
        Zukan.SetLanguageFlag(species, (int)LanguageID.German, CHK_LangGER.Checked);
        Zukan.SetLanguageFlag(species, (int)LanguageID.Spanish, CHK_LangSPA.Checked);
        Zukan.SetLanguageFlag(species, (int)LanguageID.Korean, CHK_LangKOR.Checked);
        Zukan.SetLanguageFlag(species, (int)LanguageID.ChineseS, CHK_LangCHS.Checked);
        Zukan.SetLanguageFlag(species, (int)LanguageID.ChineseT, CHK_LangCHT.Checked);

        var f1 = CLB_FormRegular;
        var f2 = CLB_FormShiny;
        for (byte i = 0; i < f1.Items.Count; i++)
        {
            Zukan.SetHasFormFlag(species, i, false, f1.GetItemChecked(i));
            Zukan.SetHasFormFlag(species, i, true, f2.GetItemChecked(i));
        }
    }

    private void B_Cancel_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void B_Save_Click(object sender, EventArgs e)
    {
        SetEntry();
        Zukan.HasNationalDex = CHK_National.Checked;
        Origin.CopyChangesFrom(SAV);
        Close();
    }

    private void B_GiveAll_Click(object sender, EventArgs e)
    {
        bool all = ModifierKeys != Keys.Control;
        CB_State.SelectedIndex = all ? (int)ZukanState8b.Caught : 0;
        CHK_M.Checked = CHK_F.Checked = CHK_MS.Checked = CHK_FS.Checked = all;
        CHK_LangJPN.Checked = CHK_LangENG.Checked = CHK_LangFRE.Checked = CHK_LangGER.Checked = CHK_LangITA.Checked = all;
        CHK_LangSPA.Checked = CHK_LangKOR.Checked = CHK_LangCHS.Checked = CHK_LangCHT.Checked = all;
    }

    private void B_Modify_Click(object sender, EventArgs e)
    {
        Button btn = (Button)sender;
        modifyMenu.Show(btn.PointToScreen(new Point(0, btn.Height)));
    }

    private void B_ModifyForms_Click(object sender, EventArgs e)
    {
        Button btn = (Button)sender;
        modifyMenuForms.Show(btn.PointToScreen(new Point(0, btn.Height)));
    }

    private void ModifyAll(object sender, EventArgs e)
    {
        SetEntry();

        if (sender == mnuSeenNone)
            Zukan.SetAllSeen(false);
        if (sender == mnuSeenAll)
            Zukan.SetAllSeen(shinyToo: ModifierKeys == Keys.Control);
        else if (sender == mnuCaughtAll)
            Zukan.CaughtAll();
        else if (sender == mnuCaughtNone)
            Zukan.CaughtNone();
        else if (sender == mnuComplete)
            Zukan.CompleteDex(ModifierKeys == Keys.Control);

        GetEntry();
    }

    private void ModifyAllForms(object sender, EventArgs e)
    {
        var f1 = CLB_FormRegular;
        var f2 = CLB_FormShiny;
        if (sender == mnuFormAllRegular)
        {
            for (int i = 0; i < f1.Items.Count; i++)
            {
                f1.SetItemChecked(i, true);
                f2.SetItemChecked(i, sender == mnuFormAllShinies);
            }
        }
        else if (sender == mnuFormAllShinies)
        {
            for (int i = 0; i < f1.Items.Count; i++)
            {
                f2.SetItemChecked(i, true);
            }
        }
        else // None
        {
            for (int i = 0; i < f1.Items.Count; i++)
            {
                f1.SetItemChecked(i, false);
                f2.SetItemChecked(i, false);
            }
        }
    }
}
