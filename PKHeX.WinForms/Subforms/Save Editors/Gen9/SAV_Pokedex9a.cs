using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public partial class SAV_Pokedex9a : Form
{
    private readonly SAV9ZA Origin;
    private readonly SAV9ZA SAV;
    private readonly Zukan9a Dex;

    private int lastIndex;
    private readonly bool CanSave;
    private readonly bool Loading;
    private readonly MegaFormNames MegaNames;

    public SAV_Pokedex9a(SAV9ZA sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (SAV9ZA)(Origin = sav).Clone();
        Dex = SAV.Blocks.Zukan;

        Loading = true;
        // Clear Listbox and ComboBox
        LB_Species.Items.Clear();
        CB_Species.Items.Clear();

        CLB_FormCaught.Items.Clear();
        CLB_FormSeen.Items.Clear();
        CLB_FormShiny.Items.Clear();
        var empty = new string[32];
        empty.AsSpan().Fill("");
        CLB_FormCaught.Items.AddRange(empty);
        CLB_FormSeen.Items.AddRange(empty);
        CLB_FormShiny.Items.AddRange(empty);

        // Fill List
        var filtered = GameInfo.FilteredSources;
        var strings = filtered.Source.Strings;
        MegaNames = FormConverter.GetMegaFormNames(strings.forms, Main.GenderSymbols, strings.Types);
        int maxSpecies = sav.MaxSpeciesID; // no DLC species

        CB_Species.InitializeBinding();
        var species = filtered.Species.Where(z => z.Value <= maxSpecies).ToArray();
        CB_Species.DataSource = new BindingSource(species, string.Empty);

        var list = species
            .Select(z => new DexMap(z))
            .OrderByDescending(z => z.IsInAnyDex)
            .ThenBy(z => z.Dex).ToArray();

        var lbi = LB_Species.Items;
        for (var i = 0; i < list.Length; i++)
        {
            var n = list[i];
            var display = n.GetDexString();
            lbi.Add($"{display} - {n.Name}");
            n.ListIndex = i;
        }

        ListBoxToSpecies = list;
        LB_Species.SelectedIndex = 0;
        CB_Species.KeyDown += WinFormsUtil.RemoveDropCB;
        Loading = false;
        CanSave = true;
        lastIndex = 0;
        GetEntry(0);
    }

    private record DexMap
    {
        public ushort Species { get; }
        public bool IsInAnyDex => Dex != 0;
        public ushort Dex { get; }
        public string Name { get; }
        public int ListIndex { get; set; }

        public DexMap(ComboItem c)
        {
            Species = (ushort)c.Value;
            Name = c.Text;
            Dex = GetDexIndex(Species);
        }

        private static ushort GetDexIndex(ushort species)
        {
            var entry = PersonalTable.ZA.GetFormEntry(species, 0);
            for (byte i = 0; i < entry.FormCount; i++)
            {
                entry = PersonalTable.ZA.GetFormEntry(species, i);
                if (entry.DexIndex != 0)
                    return entry.DexIndex;
            }
            return 0;
        }

        public string GetDexString()
        {
            if (!IsInAnyDex)
                return "***";
            return $"{Dex:000}";
        }
    }

    private DexMap[] ListBoxToSpecies { get; }

    private ushort GetSpecies(int listBoxIndex) => Array.Find(ListBoxToSpecies, z => z.ListIndex == listBoxIndex)?.Species ?? 0;
    private int GetIndex(ushort species) => Array.Find(ListBoxToSpecies, z => z.Species == species)?.ListIndex ?? 0;

    private void ChangeCBSpecies(object sender, EventArgs e)
    {
        if (Loading)
            return;

        var species = (ushort)WinFormsUtil.GetIndex(CB_Species);
        var index = GetIndex(species);
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
        if (!CanSave || Loading || index < 0)
            return;
        var species = GetSpecies(index);
        GetEntry(species);
    }

    private void GetEntry(ushort species)
    {
        var entry = SAV.Zukan.GetEntry(species);
        var forms = GetFormList(species);
        if (forms[0].Length == 0)
            forms[0] = GameInfo.Strings.Types[0];

        CHK_IsNew.Checked = entry.GetDisplayIsNew();
        CHK_SeenMale.Checked = entry.GetIsGenderSeen(0);
        CHK_SeenFemale.Checked = entry.GetIsGenderSeen(1);
        CHK_SeenGenderless.Checked = entry.GetIsGenderSeen(2);
        CHK_SeenAlpha.Checked = entry.GetIsSeenAlpha();

        CHK_SeenMega0.Checked = entry.GetIsSeenMega(0);
        CHK_SeenMega1.Checked = entry.GetIsSeenMega(1);
        CHK_SeenMega2.Checked = entry.GetIsSeenMega(2);
        if (Zukan9a.IsMegaFormXY(species, SAV.SaveRevision))
        {
            CHK_SeenMega2.Visible = false;

            CHK_SeenMega0.Text = MegaNames.X;
            CHK_SeenMega1.Text = MegaNames.Y;
            CHK_SeenMega0.Visible = true;
            CHK_SeenMega1.Visible = true;
        }
        else if (Zukan9a.IsMegaFormZA(species, SAV.SaveRevision))
        {
            CHK_SeenMega2.Visible = false;

            CHK_SeenMega0.Text = MegaNames.Regular;
            CHK_SeenMega1.Text = MegaNames.Z;
            CHK_SeenMega0.Visible = true;
            CHK_SeenMega1.Visible = true;
        }
        else if (species is (int)Species.Meowstic)
        {
            CHK_SeenMega0.Text = MegaNames.MeowsticM;
            CHK_SeenMega1.Text = MegaNames.MeowsticF;
            CHK_SeenMega0.Visible = true;
            CHK_SeenMega1.Visible = true;
            CHK_SeenMega2.Visible = false;
        }
        else if (species is (int)Species.Magearna)
        {
            CHK_SeenMega0.Text = MegaNames.Magearna0;
            CHK_SeenMega1.Text = MegaNames.Magearna1;
            CHK_SeenMega0.Visible = true;
            CHK_SeenMega1.Visible = true;
            CHK_SeenMega2.Visible = false;
        }
        else if (species is (int)Species.Tatsugiri)
        {
            CHK_SeenMega0.Text = MegaNames.Tatsu0;
            CHK_SeenMega1.Text = MegaNames.Tatsu1;
            CHK_SeenMega2.Text = MegaNames.Tatsu2;
            CHK_SeenMega0.Visible = true;
            CHK_SeenMega1.Visible = true;
            CHK_SeenMega2.Visible = true;
        }
        else
        {
            CHK_SeenMega0.Text = MegaNames.Regular;
            CHK_SeenMega0.Visible = true;
            CHK_SeenMega1.Visible = false;
            CHK_SeenMega2.Visible = false;
        }

        for (byte i = 0; i < forms.Length; i++)
        {
            CLB_FormCaught.Items[i] = CLB_FormSeen.Items[i] = CLB_FormShiny.Items[i] = forms[i];
            CLB_FormCaught.SetItemCheckState(i, entry.GetIsFormCaught(i) ? CheckState.Checked : CheckState.Unchecked);
            CLB_FormSeen.SetItemCheckState(i, entry.GetIsFormSeen(i) ? CheckState.Checked : CheckState.Unchecked);
            CLB_FormShiny.SetItemCheckState(i, entry.GetIsShinySeen(i) ? CheckState.Checked : CheckState.Unchecked);
        }

        CB_DisplayForm.Items.Clear();
        CB_DisplayForm.Items.AddRange(forms);
        CB_DisplayForm.SelectedIndex = Math.Clamp(entry.DisplayForm, 0, CB_DisplayForm.Items.Count - 1);
        CB_Gender.SelectedIndex = Math.Clamp((byte)entry.DisplayGender, 0, CB_Gender.Items.Count - 1);
        CHK_DisplayShiny.Checked = entry.GetDisplayIsShiny();

        CHK_LangJPN.Checked = entry.GetLanguageFlag((int)LanguageID.Japanese);
        CHK_LangENG.Checked = entry.GetLanguageFlag((int)LanguageID.English);
        CHK_LangFRE.Checked = entry.GetLanguageFlag((int)LanguageID.French);
        CHK_LangITA.Checked = entry.GetLanguageFlag((int)LanguageID.Italian);
        CHK_LangGER.Checked = entry.GetLanguageFlag((int)LanguageID.German);
        CHK_LangSPA.Checked = entry.GetLanguageFlag((int)LanguageID.Spanish);
        CHK_LangKOR.Checked = entry.GetLanguageFlag((int)LanguageID.Korean);
        CHK_LangCHS.Checked = entry.GetLanguageFlag((int)LanguageID.ChineseS);
        CHK_LangCHT.Checked = entry.GetLanguageFlag((int)LanguageID.ChineseT);
        CHK_LangLATAM.Checked = entry.GetLanguageFlag((int)LanguageID.SpanishL);
    }

    private static string[] GetFormList(in ushort species)
    {
        var s = GameInfo.Strings;
        string[] result = new string[8 * sizeof(uint)];
        var regular = FormConverter.GetFormList(species, s.Types, s.forms, GameInfo.GenderSymbolASCII, EntityContext.Gen9a);
        for (int i = 0; i < regular.Length; i++)
            result[i] = regular[i];
        for (int i = regular.Length; i < result.Length; i++)
            result[i] = $"{i:00} - N/A";
        return result;
    }

    private void SetEntry(int index)
    {
        if (!CanSave || Loading || index < 0)
            return;
        var species = GetSpecies(index);
        SetEntry(species);
    }

    private void SetEntry(ushort species)
    {
        var entry = SAV.Zukan.GetEntry(species);
        entry.SetDisplayIsNew(CHK_IsNew.Checked);
        entry.SetIsGenderSeen(0, CHK_SeenMale.Checked);
        entry.SetIsGenderSeen(1, CHK_SeenFemale.Checked);
        entry.SetIsGenderSeen(2, CHK_SeenGenderless.Checked);
        entry.SetIsSeenAlpha(CHK_SeenAlpha.Checked);

        if (Zukan9a.IsMegaFormXY(species, SAV.SaveRevision) || Zukan9a.IsMegaFormZA(species, SAV.SaveRevision) || species is (int)Species.Magearna or (int)Species.Meowstic)
        {
            entry.SetIsSeenMega(0, CHK_SeenMega0.Checked);
            entry.SetIsSeenMega(1, CHK_SeenMega1.Checked);
        }
        else if (species == (int)Species.Tatsugiri)
        {
            entry.SetIsSeenMega(0, CHK_SeenMega0.Checked);
            entry.SetIsSeenMega(1, CHK_SeenMega1.Checked);
            entry.SetIsSeenMega(2, CHK_SeenMega2.Checked);
        }
        else
        {
            entry.SetIsSeenMega(0, CHK_SeenMega0.Checked);
        }

        for (byte i = 0; i < CLB_FormSeen.Items.Count; i++)
        {
            entry.SetIsFormSeen(i, CLB_FormSeen.GetItemChecked(i));
            entry.SetIsFormCaught(i, CLB_FormCaught.GetItemChecked(i));
            entry.SetIsShinySeen(i, CLB_FormShiny.GetItemChecked(i));
        }

        entry.DisplayForm = (byte)CB_DisplayForm.SelectedIndex;
        entry.DisplayGender = (DisplayGender9a)CB_Gender.SelectedIndex;
        entry.SetDisplayIsShiny(CHK_DisplayShiny.Checked);

        entry.SetLanguageFlag((int)LanguageID.Japanese, CHK_LangJPN.Checked);
        entry.SetLanguageFlag((int)LanguageID.English, CHK_LangENG.Checked);
        entry.SetLanguageFlag((int)LanguageID.French, CHK_LangFRE.Checked);
        entry.SetLanguageFlag((int)LanguageID.Italian, CHK_LangITA.Checked);
        entry.SetLanguageFlag((int)LanguageID.German, CHK_LangGER.Checked);
        entry.SetLanguageFlag((int)LanguageID.Spanish, CHK_LangSPA.Checked);
        entry.SetLanguageFlag((int)LanguageID.Korean, CHK_LangKOR.Checked);
        entry.SetLanguageFlag((int)LanguageID.ChineseS, CHK_LangCHS.Checked);
        entry.SetLanguageFlag((int)LanguageID.ChineseT, CHK_LangCHT.Checked);
        entry.SetLanguageFlag((int)LanguageID.SpanishL, CHK_LangLATAM.Checked);
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
        var species = GetSpecies(lastIndex);
        Dex.SetDexEntryAll(species, shiny);
        System.Media.SystemSounds.Asterisk.Play();
        GetEntry(species);
    }

    private void B_Modify_Click(object sender, EventArgs e)
    {
        Button btn = (Button)sender;
        modifyMenu.Show(btn.PointToScreen(new Point(0, btn.Height)));
    }

    private void SeenNone(object sender, EventArgs e)
    {
        var species = GetSpecies(lastIndex);
        SetEntry(species);
        Dex.SeenNone();
        System.Media.SystemSounds.Asterisk.Play();
        GetEntry(species);
    }

    private void SeenAll(object sender, EventArgs e)
    {
        var species = GetSpecies(lastIndex);
        SetEntry(species);
        bool shiny = ModifierKeys == Keys.Shift;
        Dex.SeenAll(shiny);
        System.Media.SystemSounds.Asterisk.Play();
        GetEntry(species);
    }

    private void CaughtNone(object sender, EventArgs e)
    {
        var species = GetSpecies(lastIndex);
        SetEntry(species);
        Dex.CaughtNone();
        System.Media.SystemSounds.Asterisk.Play();
        GetEntry(species);
    }

    private void CaughtAll(object sender, EventArgs e)
    {
        var species = GetSpecies(lastIndex);
        SetEntry(species);
        bool shiny = ModifierKeys == Keys.Shift;
        Dex.CaughtAll(shiny);
        System.Media.SystemSounds.Asterisk.Play();
        GetEntry(species);
    }

    private void CompleteDex(object sender, EventArgs e)
    {
        var species = GetSpecies(lastIndex);
        SetEntry(species);
        bool shiny = ModifierKeys == Keys.Shift;
        Dex.CompleteDex(shiny);
        System.Media.SystemSounds.Asterisk.Play();
        GetEntry(species);
    }
}
