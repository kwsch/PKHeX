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

        public FormArgument() => InitializeComponent();

        public void LoadArgument(IFormArgument f, int species, int form, int generation)
        {
            var max = FormConverter.GetFormArgumentMax(species, form, generation);
            if (max == 0)
            {
                CB_FormArg.SelectedIndex = 0;
                CB_FormArg.Visible = false;
                NUD_FormArg.Visible = false;
                return;
            }

            bool named = FormConverter.GetFormArgumentIsNamedIndex(species);
            if (named)
            {
                if (CurrentSpecies == species && CurrentForm == form)
                {
                    CurrentValue = f.FormArgument;
                    return;
                }
                IsRawMode = false;

                NUD_FormArg.Visible = false;
                CB_FormArg.Items.Clear();
                var args = FormConverter.GetFormArgumentStrings(species);
                CB_FormArg.Items.AddRange(args);
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
        }

        public uint CurrentValue
        {
            get => IsRawMode ? (uint) NUD_FormArg.Value : (uint) CB_FormArg.SelectedIndex;
            set
            {
                if (IsRawMode)
                    NUD_FormArg.Value = Math.Min(NUD_FormArg.Maximum, value);
                else
                    CB_FormArg.SelectedIndex = Math.Min(CB_FormArg.SelectedIndex, (int)value);
            }
        }

        public void SaveArgument(IFormArgument f) => f.FormArgument = CurrentValue;
    }
}
