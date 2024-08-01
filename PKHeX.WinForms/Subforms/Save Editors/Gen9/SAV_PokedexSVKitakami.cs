using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public partial class SAV_PokedexSVKitakami : Form
{
    private readonly SAV9SV Origin;
    private readonly SAV9SV SAV;
    private readonly Zukan9 Dex;

    private int lastIndex;
    private readonly bool CanSave;
    private readonly bool Loading;

    public SAV_PokedexSVKitakami(SAV9SV sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (SAV9SV)(Origin = sav).Clone();
        Dex = SAV.Blocks.Zukan;

        Loading = true;
        // Clear Listbox and ComboBox
        LB_Species.Items.Clear();
        CB_Species.Items.Clear();

        var empty = new string[32];
        empty.AsSpan().Fill(string.Empty);
        CLB_FormSeen.Items.AddRange(empty);
        CLB_FormObtained.Items.AddRange(empty);
        CLB_FormHeard.Items.AddRange(empty);
        CLB_FormViewed.Items.AddRange(empty);

        // Fill List
        CB_Species.InitializeBinding();
        var species = GameInfo.SpeciesDataSource.Where(z => SAV.Personal.IsSpeciesInGame((ushort)z.Value)).ToArray();
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

    private bool IgnoreChangeEvent;

    private void ChangeCBSpecies(object sender, EventArgs e)
    {
        if (Loading || IgnoreChangeEvent)
            return;

        var species = (ushort)WinFormsUtil.GetIndex(CB_Species);
        var index = GetIndex(species);
        if (LB_Species.SelectedIndex != index)
            LB_Species.SelectedIndex = index; // trigger event
    }

    private void ChangeLBSpecies(object sender, EventArgs e)
    {
        if (Loading || LB_Species.SelectedIndex < 0 || IgnoreChangeEvent)
            return;

        SetEntry(lastIndex);
        lastIndex = LB_Species.SelectedIndex;
        GetEntry(lastIndex);

        IgnoreChangeEvent = true;
        CB_Species.SelectedValue = (int)GetSpecies(lastIndex);
        IgnoreChangeEvent = false;
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
        var entry = SAV.Zukan.DexKitakami.Get(species);
        var forms = GetFormList(species);
        if (forms[0].Length == 0)
            forms[0] = GameInfo.Strings.Types[0];

        // Clear all CheckedListBoxes
        var seen = CLB_FormSeen.Items;
        var obtained = CLB_FormObtained.Items;
        var heard = CLB_FormHeard.Items;
        var viewed = CLB_FormViewed.Items;
        var p = CB_PaldeaForm.Items;
        var k = CB_KitakamiForm.Items;
        var b = CB_BlueberryForm.Items;
        p.Clear();
        k.Clear();
        b.Clear();
        p.AddRange(forms);
        k.AddRange(forms);
        b.AddRange(forms);

        // Fill CheckedListBoxes
        for (byte i = 0; i < sizeof(uint) * 8; i++)
        {
            var name = i < forms.Length ? forms[i] : $"--{i:00}--";
            seen[i] = obtained[i] = heard[i] = viewed[i] = name;
            CLB_FormSeen.SetItemChecked(i, entry.GetSeenForm(i));
            CLB_FormObtained.SetItemChecked(i, entry.GetObtainedForm(i));
            CLB_FormHeard.SetItemChecked(i, entry.GetHeardForm(i));
            CLB_FormViewed.SetItemChecked(i, entry.GetCheckedForm(i));
        }

        // Fill Checkboxes
        CHK_SeenMale.Checked = entry.GetIsGenderSeen(0);
        CHK_SeenFemale.Checked = entry.GetIsGenderSeen(1);
        CHK_SeenGenderless.Checked = entry.GetIsGenderSeen(2);
        CHK_SeenShiny.Checked = entry.GetIsModelSeen(true);

        // Fill Languages
        CHK_LangJPN.Checked = entry.GetLanguageFlag((int)LanguageID.Japanese);
        CHK_LangENG.Checked = entry.GetLanguageFlag((int)LanguageID.English);
        CHK_LangFRE.Checked = entry.GetLanguageFlag((int)LanguageID.French);
        CHK_LangITA.Checked = entry.GetLanguageFlag((int)LanguageID.Italian);
        CHK_LangGER.Checked = entry.GetLanguageFlag((int)LanguageID.German);
        CHK_LangSPA.Checked = entry.GetLanguageFlag((int)LanguageID.Spanish);
        CHK_LangKOR.Checked = entry.GetLanguageFlag((int)LanguageID.Korean);
        CHK_LangCHS.Checked = entry.GetLanguageFlag((int)LanguageID.ChineseS);
        CHK_LangCHT.Checked = entry.GetLanguageFlag((int)LanguageID.ChineseT);

        // Fill Paldea
        CB_PaldeaForm.SelectedIndex = entry.DisplayedPaldeaForm;
        CB_PaldeaGender.SelectedIndex = entry.DisplayedPaldeaGender;
        CHK_PaldeaShiny.Checked = entry.DisplayedPaldeaShiny != 0;

        // Fill Kitakami
        CB_KitakamiForm.SelectedIndex = entry.DisplayedKitakamiForm;
        CB_KitakamiGender.SelectedIndex = entry.DisplayedKitakamiGender;
        CHK_KitakamiShiny.Checked = entry.DisplayedKitakamiShiny != 0;

        // Fill Blueberry
        CB_BlueberryForm.SelectedIndex = entry.DisplayedBlueberryForm;
        CB_BlueberryGender.SelectedIndex = entry.DisplayedBlueberryGender;
        CHK_BlueberryShiny.Checked = entry.DisplayedBlueberryShiny != 0;

        var pi = SAV.Personal[species];
        var fc = pi.FormCount;
        bool paldea = false, kitakami = false, blueberry = false;
        for (byte i = 0; i < fc; i++)
        {
            pi = SAV.Personal.GetFormEntry(species, i);
            if (pi.DexPaldea != 0)
                paldea = true;
            if (pi.DexKitakami != 0)
                kitakami = true;
            if (pi.DexBlueberry != 0)
                blueberry = true;
        }
        GB_Paldea.Enabled = paldea;
        GB_Kitakami.Enabled = kitakami;
        GB_Blueberry.Enabled = blueberry;
    }

    private static string[] GetFormList(in ushort species)
    {
        var s = GameInfo.Strings;
        // Alcremie: formarg-forms are not stored in bitflags; shown by default. No need for special handling for Alcremie.
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
        var entry = SAV.Zukan.DexKitakami.Get(species);

        // Set Form Flags
        for (byte i = 0; i < sizeof(uint) * 8; i++)
        {
            entry.SetSeenForm(i, CLB_FormSeen.GetItemChecked(i));
            entry.SetObtainedForm(i, CLB_FormObtained.GetItemChecked(i));
            entry.SetHeardForm(i, CLB_FormHeard.GetItemChecked(i));
            entry.SetCheckedForm(i, CLB_FormViewed.GetItemChecked(i));
        }

        // Set Flags
        entry.SetIsGenderSeen(0, CHK_SeenMale.Checked);
        entry.SetIsGenderSeen(1, CHK_SeenFemale.Checked);
        entry.SetIsGenderSeen(2, CHK_SeenGenderless.Checked);
        entry.SetIsModelSeen(true, CHK_SeenShiny.Checked);

        // Set Languages
        entry.SetLanguageFlag((int)LanguageID.Japanese, CHK_LangJPN.Checked);
        entry.SetLanguageFlag((int)LanguageID.English, CHK_LangENG.Checked);
        entry.SetLanguageFlag((int)LanguageID.French, CHK_LangFRE.Checked);
        entry.SetLanguageFlag((int)LanguageID.Italian, CHK_LangITA.Checked);
        entry.SetLanguageFlag((int)LanguageID.German, CHK_LangGER.Checked);
        entry.SetLanguageFlag((int)LanguageID.Spanish, CHK_LangSPA.Checked);
        entry.SetLanguageFlag((int)LanguageID.Korean, CHK_LangKOR.Checked);
        entry.SetLanguageFlag((int)LanguageID.ChineseS, CHK_LangCHS.Checked);
        entry.SetLanguageFlag((int)LanguageID.ChineseT, CHK_LangCHT.Checked);

        // Set Local Dexes
        entry.SetLocalPaldea((byte)CB_PaldeaForm.SelectedIndex, (byte)CB_PaldeaGender.SelectedIndex, CHK_PaldeaShiny.Checked ? (byte)1 : (byte)0);
        entry.SetLocalKitakami((byte)CB_KitakamiForm.SelectedIndex, (byte)CB_KitakamiGender.SelectedIndex, CHK_KitakamiShiny.Checked ? (byte)1 : (byte)0);
        entry.SetLocalBlueberry((byte)CB_BlueberryForm.SelectedIndex, (byte)CB_BlueberryGender.SelectedIndex, CHK_BlueberryShiny.Checked ? (byte)1 : (byte)0);
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
