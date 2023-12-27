using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public partial class SAV_PokedexSV : Form
{
    private readonly SAV9SV Origin;
    private readonly SAV9SV SAV;
    private readonly Zukan9 Dex;

    private int lastIndex;
    private readonly bool CanSave;
    private readonly bool Loading;

    public SAV_PokedexSV(SAV9SV sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (SAV9SV)(Origin = sav).Clone();
        Dex = SAV.Blocks.Zukan;

        Loading = true;
        // Clear Listbox and ComboBox
        LB_Species.Items.Clear();
        CB_Species.Items.Clear();

        // Fill List
        const int maxSpecies = (int)Species.IronLeaves; // 1010 -- no DLC species
        CB_Species.InitializeBinding();
        var species = GameInfo.SpeciesDataSource.Where(z => SAV.Personal.IsSpeciesInGame((ushort)z.Value) && z.Value <= maxSpecies).ToArray();
        CB_Species.DataSource = new BindingSource(species, null);

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
        public bool IsInAnyDex => Dex != default;
        public (int Group, int Index) Dex { get; }
        public string Name { get; }
        public int ListIndex { get; set; }

        public DexMap(ComboItem c)
        {
            Species = (ushort)c.Value;
            Name = c.Text;
            Dex = GetDexIndex(Species);
        }

        private static (int Group, int Index) GetDexIndex(ushort species)
        {
            var entry = PersonalTable.SV.GetFormEntry(species, 0);
            for (byte i = 0; i < entry.FormCount; i++)
            {
                entry = PersonalTable.SV.GetFormEntry(species, i);
                if (entry.DexPaldea != 0)
                    return (1, entry.DexPaldea);
                if (entry.DexKitakami != 0)
                    return (2, entry.DexKitakami);
                if (entry.DexBlueberry != 0)
                    return (3, entry.DexBlueberry);
            }
            return default;
        }

        public string GetDexString()
        {
            if (!IsInAnyDex)
                return "***";
            var prefix = Dex.Group switch
            {
                1 => "P",
                2 => "K",
                3 => "B",
                _ => "?",
            };
            return $"{prefix}-{Dex.Index:000}";
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
        var entry = SAV.Zukan.DexPaldea.Get(species);
        var forms = GetFormList(species);
        if (forms[0].Length == 0)
            forms[0] = GameInfo.Strings.Types[0];

        CB_State.SelectedIndex = (int)entry.GetState();
        CHK_IsNew.Checked = entry.GetDisplayIsNew();

        CHK_SeenMale.Checked = entry.GetIsGenderSeen(0);
        CHK_SeenFemale.Checked = entry.GetIsGenderSeen(1);
        CHK_SeenGenderless.Checked = entry.GetIsGenderSeen(2);
        CHK_SeenShiny.Checked = entry.GetSeenIsShiny();

        CLB_FormSeen.Items.Clear();
        for (byte i = 0; i < forms.Length; i++)
            CLB_FormSeen.Items.Add(forms[i], entry.GetIsFormSeen(i));

        CB_DisplayForm.Items.Clear();
        CB_DisplayForm.Items.AddRange(forms);
        CB_Gender.SelectedIndex = (int)entry.GetDisplayGender();
        CHK_DisplayShiny.Checked = entry.GetDisplayIsShiny();
        CHK_G.Checked = entry.GetDisplayGenderIsDifferent();
        CB_DisplayForm.SelectedIndex = (int)entry.GetDisplayForm();

        CHK_LangJPN.Checked = entry.GetLanguageFlag((int)LanguageID.Japanese);
        CHK_LangENG.Checked = entry.GetLanguageFlag((int)LanguageID.English);
        CHK_LangFRE.Checked = entry.GetLanguageFlag((int)LanguageID.French);
        CHK_LangITA.Checked = entry.GetLanguageFlag((int)LanguageID.Italian);
        CHK_LangGER.Checked = entry.GetLanguageFlag((int)LanguageID.German);
        CHK_LangSPA.Checked = entry.GetLanguageFlag((int)LanguageID.Spanish);
        CHK_LangKOR.Checked = entry.GetLanguageFlag((int)LanguageID.Korean);
        CHK_LangCHS.Checked = entry.GetLanguageFlag((int)LanguageID.ChineseS);
        CHK_LangCHT.Checked = entry.GetLanguageFlag((int)LanguageID.ChineseT);
    }

    private static string[] GetFormList(in ushort species)
    {
        var s = GameInfo.Strings;
        if (species == (int)Species.Alcremie)
            return FormConverter.GetAlcremieFormList(s.forms);
        return FormConverter.GetFormList(species, s.Types, s.forms, GameInfo.GenderSymbolASCII, EntityContext.Gen9);
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
        var entry = SAV.Zukan.DexPaldea.Get(species);
        entry.SetState((uint)CB_State.SelectedIndex);
        entry.SetDisplayIsNew(CHK_IsNew.Checked);

        entry.SetIsGenderSeen(0, CHK_SeenMale.Checked);
        entry.SetIsGenderSeen(1, CHK_SeenFemale.Checked);
        entry.SetIsGenderSeen(2, CHK_SeenGenderless.Checked);
        entry.SetSeenIsShiny(CHK_SeenShiny.Checked);

        for (byte i = 0; i < CLB_FormSeen.Items.Count; i++)
            entry.SetIsFormSeen(i, CLB_FormSeen.GetItemChecked(i));

        entry.SetDisplayGender(CB_Gender.SelectedIndex);
        entry.SetDisplayIsShiny(CHK_DisplayShiny.Checked);
        entry.SetDisplayGenderIsDifferent(CHK_G.Checked);
        entry.SetDisplayForm((uint)CB_DisplayForm.SelectedIndex);

        entry.SetLanguageFlag((int)LanguageID.Japanese, CHK_LangJPN.Checked);
        entry.SetLanguageFlag((int)LanguageID.English, CHK_LangENG.Checked);
        entry.SetLanguageFlag((int)LanguageID.French, CHK_LangFRE.Checked);
        entry.SetLanguageFlag((int)LanguageID.Italian, CHK_LangITA.Checked);
        entry.SetLanguageFlag((int)LanguageID.German, CHK_LangGER.Checked);
        entry.SetLanguageFlag((int)LanguageID.Spanish, CHK_LangSPA.Checked);
        entry.SetLanguageFlag((int)LanguageID.Korean, CHK_LangKOR.Checked);
        entry.SetLanguageFlag((int)LanguageID.ChineseS, CHK_LangCHS.Checked);
        entry.SetLanguageFlag((int)LanguageID.ChineseT, CHK_LangCHT.Checked);
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
