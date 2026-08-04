
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;
using PKHeX.Presentation.Localization;

namespace PKHeX.Presentation.ViewModels;

public partial class PokemonEditorViewModel
{
    [ObservableProperty]
    private bool _isLegal;

    [ObservableProperty]
    private string _legalityReport = string.Empty;

    private void Validate()
    {
        var pk = PreparePKM();
        var la = new LegalityAnalysis(pk, _sav.Personal);
        IsLegal = la.Valid;
        LegalityReport = la.Report();
    }

    [RelayCommand]
    private async Task ShowLegalityAsync()
    {
        Validate();
        await _windowService.ShowDialogAsync(new LegalityViewModel(LegalityReport), LocalizedStrings.Instance["PokemonEditor_LegalityAnalysisTitle"]);
    }
}
