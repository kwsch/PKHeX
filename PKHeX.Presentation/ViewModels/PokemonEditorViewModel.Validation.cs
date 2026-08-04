
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
        // PKHaX intentionally permits combinations the legality checker rejects. Keep the report
        // available for diagnostics, but do not surface the normal illegal-state indicator.
        IsLegal = _haxMode || la.Valid;
        LegalityReport = la.Report();
    }

    [RelayCommand]
    private async Task ShowLegalityAsync()
    {
        Validate();
        await _windowService.ShowDialogAsync(new LegalityViewModel(LegalityReport), LocalizedStrings.Instance["PokemonEditor_LegalityAnalysisTitle"]);
    }
}
