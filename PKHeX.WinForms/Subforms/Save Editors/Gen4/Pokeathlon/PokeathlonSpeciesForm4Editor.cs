using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Drawing.PokeSprite;

namespace PKHeX.WinForms;

public sealed partial class PokeathlonSpeciesForm4Editor : UserControl
{
    private bool IsLoading;
    private byte SpriteGender;
    private bool SpriteShiny;

    public event EventHandler? ValueChanged;

    public ushort Species => (ushort)WinFormsUtil.GetIndex(CB_Species);
    public byte Form => (byte)Math.Max(CB_Form.SelectedIndex, 0);

    public byte DisplayGender
    {
        get => SpriteGender;
        set
        {
            SpriteGender = value > 2 ? (byte)2 : value;
            RefreshSprite();
        }
    }

    public bool DisplayShiny
    {
        get => SpriteShiny;
        set
        {
            SpriteShiny = value;
            RefreshSprite();
        }
    }

    public PokeathlonSpeciesForm4Editor()
    {
        InitializeComponent();
        InitializeCombo(CB_Species, GameInfo.FilteredSources.Species.ToList());
        CB_Species.SelectedValueChanged += CB_Species_SelectedValueChanged;
        CB_Form.SelectedValueChanged += CB_Form_SelectedValueChanged;
        LoadValues(0, 0);
    }

    public void LoadValues(ushort species, byte form)
    {
        IsLoading = true;
        CB_Species.SelectedValue = (int)species;
        LoadForms(form);
        RefreshSprite();
        IsLoading = false;
    }

    private void CB_Species_SelectedValueChanged(object? sender, EventArgs e)
    {
        LoadForms();
        OnValueChanged();
    }

    private void CB_Form_SelectedValueChanged(object? sender, EventArgs e)
    {
        RefreshSprite();
        OnValueChanged();
    }

    private void LoadForms(byte selectedForm = 0)
    {
        var species = Species;
        var forms = FormConverter.GetFormList(species, GameInfo.Strings.types, GameInfo.Strings.forms, Main.GenderSymbols, EntityContext.Gen4);
        var source = GetFormSource(forms);
        CB_Form.InitializeBinding();
        CB_Form.DataSource = new BindingSource(source, string.Empty);
        CB_Form.SelectedValue = Math.Min(selectedForm, source.Count - 1);
        RefreshSprite();
    }

    private void RefreshSprite()
    {
        var image = SpriteUtil.GetSprite(Species, Form, SpriteGender, 0, 0, false, SpriteShiny ? Shiny.Always : Shiny.Never, EntityContext.Gen4);
        PB_Sprite.Image = image;
    }

    private void OnValueChanged()
    {
        if (!IsLoading)
            ValueChanged?.Invoke(this, EventArgs.Empty);
    }

    private static List<ComboItem> GetFormSource(IReadOnlyList<string> forms)
    {
        var result = new List<ComboItem>(forms.Count);
        for (int i = 0; i < forms.Count; i++)
            result.Add(new ComboItem(forms[i], i));
        return result;
    }

    private static void InitializeCombo(ComboBox cb, IReadOnlyList<ComboItem> source)
    {
        cb.InitializeBinding();
        cb.DataSource = new BindingSource(source, string.Empty);
    }
}
