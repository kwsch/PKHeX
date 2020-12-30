using System;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms.Controls
{
    public partial class FormArgument : UserControl
    {
        private bool IsRawMode;
        private int CurrentSpecies;
        private int CurrentForm;
        private bool FieldsLoaded;

        public FormArgument() => InitializeComponent();

        public void LoadArgument(IFormArgument f, int species, int form, int generation)
        {
            FieldsLoaded = false;
            var max = FormConverter.GetFormArgumentMax(species, form, generation);
            if (max == 0)
            {
                CurrentSpecies = species;
                CurrentForm = form;
                NUD_FormArg.Value = CB_FormArg.SelectedIndex = 0;
                CB_FormArg.Visible = false;
                NUD_FormArg.Visible = false;
                FieldsLoaded = true;
                return;
            }

            bool named = FormConverter.GetFormArgumentIsNamedIndex(species);
            if (named)
            {
                if (CurrentSpecies == species && CurrentForm == form)
                {
                    CurrentValue = f.FormArgument;
                    FieldsLoaded = true;
                    return;
                }
                IsRawMode = false;

                NUD_FormArg.Visible = false;
                CB_FormArg.Items.Clear();
                var args = FormConverter.GetFormArgumentStrings(species);
                CB_FormArg.Items.AddRange(args);
                CB_FormArg.SelectedIndex = 0;
                CB_FormArg.Visible = true;
            }
            else
            {
                IsRawMode = true;

                CB_FormArg.Visible = false;
                NUD_FormArg.Maximum = max;
                NUD_FormArg.Visible = true;
            }
            CurrentSpecies = species;
            CurrentForm = form;
            CurrentValue = f.FormArgument;
            FieldsLoaded = true;
        }

        public uint CurrentValue
        {
            get => IsRawMode ? (uint) NUD_FormArg.Value : (uint) CB_FormArg.SelectedIndex;
            set
            {
                if (IsRawMode)
                    NUD_FormArg.Value = Math.Min(NUD_FormArg.Maximum, value);
                else
                    CB_FormArg.SelectedIndex = Math.Min(CB_FormArg.Items.Count - 1, (int)value);
            }
        }

        public void SaveArgument(IFormArgument f) => f.FormArgument = CurrentValue;
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
}
