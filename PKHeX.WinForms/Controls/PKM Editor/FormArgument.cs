using System;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms.Controls;

public partial class FormArgument : UserControl
{
    private bool IsRawMode;
    private ushort CurrentSpecies;
    private byte CurrentForm;
    private EntityContext CurrentContext;
    private bool FieldsLoaded;
    public bool IsControlVisible { get; private set; }

    public FormArgument() => InitializeComponent();

    public bool LoadArgument(IFormArgument f, ushort species, byte form, EntityContext context)
    {
        FieldsLoaded = false;
        var max = FormArgumentUtil.GetFormArgumentMax(species, form, context);
        if (max == 0)
        {
            CurrentSpecies = species;
            CurrentForm = form;
            CurrentContext = context;
            NUD_FormArg.Value = CB_FormArg.SelectedIndex = 0;
            CB_FormArg.Visible = false;
            NUD_FormArg.Visible = false;
            FieldsLoaded = true;
            IsControlVisible = false;
            return IsControlVisible;
        }

        bool named = FormConverter.GetFormArgumentIsNamedIndex(species);
        if (named)
        {
            if (CurrentSpecies == species && CurrentForm == form && CurrentContext == context)
            {
                CurrentValue = f.FormArgument;
                FieldsLoaded = true;
                return IsControlVisible;
            }
            IsRawMode = false;

            NUD_FormArg.Visible = false;
            CB_FormArg.Items.Clear();
            var args = FormConverter.GetFormArgumentStrings(species);
            CB_FormArg.Items.AddRange(args);
            CB_FormArg.SelectedIndex = 0;
            CB_FormArg.Visible = true;
            IsControlVisible = true;
        }
        else
        {
            IsRawMode = true;

            CB_FormArg.Visible = false;
            NUD_FormArg.Maximum = max;
            NUD_FormArg.Visible = true;
            IsControlVisible = true;
        }
        CurrentSpecies = species;
        CurrentForm = form;
        CurrentContext = context;

        bool isPair = FormArgumentUtil.IsFormArgumentTypeDatePair(species, form);
        CurrentValue = isPair ? f.FormArgumentRemain : f.FormArgument;

        FieldsLoaded = true;
        return IsControlVisible;
    }

    public void SaveArgument(IFormArgument f)
    {
        f.ChangeFormArgument(CurrentSpecies, CurrentForm, CurrentContext, CurrentValue);
    }

    private uint CurrentValue
    {
        get => IsRawMode ? (uint)NUD_FormArg.Value : (uint)CB_FormArg.SelectedIndex;
        set
        {
            if (IsRawMode)
                NUD_FormArg.Value = Math.Min(NUD_FormArg.Maximum, value);
            else
                CB_FormArg.SelectedIndex = Math.Min(CB_FormArg.Items.Count - 1, (int)value);
        }
    }

    public event EventHandler? ValueChanged;

    private void CB_FormArg_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (FieldsLoaded)
            ValueChanged?.Invoke(sender, e);
    }

    private void NUD_FormArg_ValueChanged(object sender, EventArgs e)
    {
        if (FieldsLoaded)
            ValueChanged?.Invoke(sender, e);
    }
}
