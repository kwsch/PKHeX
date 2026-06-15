using System.Globalization;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public sealed partial class PokeathlonParticipant4Editor : UserControl
{
    private bool IsLoading;

    public PokeathlonParticipant4Editor()
    {
        InitializeComponent();
        UC_SpeciesForm.ValueChanged += (_, _) => SpeciesFormChanged();
        GT_Gender.Click += (_, _) => WriteBack();
        CHK_IsShiny.CheckedChanged += (_, _) => ShinyChanged();
        TB_PID.TextChanged += (_, _) => WriteBack();
        TB_TID16.TextChanged += (_, _) => WriteBack();
        TB_SID16.TextChanged += (_, _) => WriteBack();
    }

    public void LoadObject(PokeathlonParticipant4 entity)
    {
        IsLoading = true;
        UC_SpeciesForm.DisplayGender = entity.Gender;
        UC_SpeciesForm.DisplayShiny = entity.IsShiny;
        UC_SpeciesForm.LoadValues(entity.Species, entity.Form);
        GT_Gender.Gender = entity.Gender;
        CHK_IsShiny.Checked = entity.IsShiny;
        TB_PID.Text = entity.EncryptionConstant.ToString("X8");
        TB_TID16.Text = entity.TID16.ToString("00000");
        TB_SID16.Text = entity.SID16.ToString("00000");
        IsLoading = false;
    }

    public void SaveObject(PokeathlonParticipant4 entity)
    {
        entity.Species = UC_SpeciesForm.Species;
        entity.Form = UC_SpeciesForm.Form;
        entity.Gender = GT_Gender.Gender;
        entity.IsShiny = CHK_IsShiny.Checked;
        entity.EncryptionConstant = ParseHex(TB_PID.Text);
        entity.TID16 = ParseU16(TB_TID16.Text);
        entity.SID16 = ParseU16(TB_SID16.Text);
    }

    private void SpeciesFormChanged()
    {
        if (IsLoading)
            return;
        WriteBack();
    }

    private void ShinyChanged()
    {
        if (IsLoading)
            return;
        UC_SpeciesForm.DisplayShiny = CHK_IsShiny.Checked;
        WriteBack();
    }

    private void WriteBack()
    {
        if (IsLoading)
            return;
        UC_SpeciesForm.DisplayGender = GT_Gender.Gender;
        UC_SpeciesForm.DisplayShiny = CHK_IsShiny.Checked;
    }

    private static ushort ParseU16(string text)
    {
        if (!ushort.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value))
            return 0;
        return value;
    }

    private static uint ParseHex(string text)
    {
        text = Util.GetOnlyHex(text);
        if (!uint.TryParse(text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var value))
            return 0;
        return value;
    }
}
