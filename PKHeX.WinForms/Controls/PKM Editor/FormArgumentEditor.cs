using System;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms.Controls;

public partial class FormArgumentEditor : UserControl
{
    private FormArgumentType Mode { get; set; }
    private ushort Species { get; set; }
    private byte Form { get; set; }
    private EntityContext Context { get; set; }
    public bool IsControlVisible { get; private set; }

    private bool FieldsLoaded;
    public FormArgumentEditor()
    {
        InitializeComponent();
        toolTip1.SetToolTip(NUD_FormArgMax, nameof(NUD_FormArgMax));
        toolTip1.SetToolTip(NUD_FormArgElapsed, nameof(NUD_FormArgElapsed));
        toolTip1.SetToolTip(NUD_FormArgRemain, nameof(NUD_FormArgRemain));
    }

    public bool LoadArgument(IFormArgument f, ushort species, byte form, EntityContext context)
    {
        FieldsLoaded = false;
        Initialize(f, species, form, context);
        FieldsLoaded = true;
        return IsControlVisible = Mode != FormArgumentType.None;
    }

    private void Initialize(IFormArgument f, ushort species, byte form, EntityContext context)
    {
        var mode = FormArgumentUtil.GetType(species, form, context);
        var same = Mode == mode && Species == species && Form == form && Context == context;
        Context = context;
        Species = species;
        Form = form;
        Mode = mode;

        CB_FormArg.Visible = mode == FormArgumentType.Named;
        NUD_FormArg.Visible = mode == FormArgumentType.Raw;
        NUD_FormArgMax.Visible = mode == FormArgumentType.Triple;
        NUD_FormArgRemain.Visible = mode is FormArgumentType.Triple or FormArgumentType.TripleParty;
        NUD_FormArgElapsed.Visible = mode is FormArgumentType.Triple or FormArgumentType.TripleParty;

        LoadValue(f, species, form, context, mode, same);
    }

    private void LoadValue(IFormArgument f, ushort species, byte form, EntityContext context, FormArgumentType mode, bool isSame)
    {
        if (mode == FormArgumentType.Named)
        {
            if (!isSame) // Already initialized.
            {
                var args = FormConverter.GetFormArgumentStrings(species);
                CB_FormArg.Items.Clear();
                CB_FormArg.Items.AddRange(args);
            }

            CB_FormArg.SelectedIndex = Math.Clamp((int)f.FormArgument, 0, CB_FormArg.Items.Count - 1);
            return;
        }

        var max = FormArgumentUtil.GetFormArgumentMaxEdge(species, form, context);
        if (mode == FormArgumentType.Raw)
        {
            NUD_FormArg.Maximum = max;
            NUD_FormArg.Value = Math.Clamp(f.FormArgument, 0, max);
        }
        else if (mode == FormArgumentType.Triple)
        {
            NUD_FormArgMax.Value = f.FormArgumentMaximum;
            NUD_FormArgRemain.Value = f.FormArgumentRemain;
            NUD_FormArgElapsed.Value = f.FormArgumentElapsed;
        }
        else if (mode == FormArgumentType.TripleParty)
        {
            NUD_FormArgRemain.Value = f.FormArgumentRemain;
            NUD_FormArgElapsed.Value = f.FormArgumentElapsed;
        }

        // Should be processed by one of the above conditions.
    }

    public void SaveArgument(IFormArgument f)
    {
        if (Mode == FormArgumentType.TripleParty)
        {
            var streak = Species != (ushort)Core.Species.Furfrou ? (byte)0 : (byte)NUD_FormArgElapsed.Value;
            f.FormArgumentMaximum = streak;
            f.FormArgumentElapsed = streak;
            f.FormArgumentRemain = (byte)NUD_FormArgRemain.Value;
        }
        else if (Mode == FormArgumentType.Triple)
        {
            f.FormArgumentMaximum = (byte)NUD_FormArgMax.Value;
            f.FormArgumentElapsed = (byte)NUD_FormArgElapsed.Value;
            f.FormArgumentRemain = (byte)NUD_FormArgRemain.Value;
        }
        else if (Mode == FormArgumentType.Named)
        {
            f.FormArgument = (uint)CB_FormArg.SelectedIndex;
        }
        else if (Mode == FormArgumentType.Raw)
        {
            f.FormArgument = (uint)NUD_FormArg.Value;
        }
        else
        {
            f.FormArgument = 0;
        }
    }

    public event EventHandler? ValueChanged;

    private void SelectionChanged(object sender, EventArgs e)
    {
        if (FieldsLoaded)
            ValueChanged?.Invoke(sender, e);
    }
}
