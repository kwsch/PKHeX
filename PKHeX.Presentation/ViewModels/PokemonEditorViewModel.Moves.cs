
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;
using PKHeX.Presentation.Localization;

namespace PKHeX.Presentation.ViewModels;

public partial class PokemonEditorViewModel
{
    // Moves
    [ObservableProperty]
    private int _move1;

    [ObservableProperty]
    private int _move2;

    [ObservableProperty]
    private int _move3;

    [ObservableProperty]
    private int _move4;

    // Relearn Moves
    [ObservableProperty]
    private int _relearnMove1;

    [ObservableProperty]
    private int _relearnMove2;

    [ObservableProperty]
    private int _relearnMove3;

    [ObservableProperty]
    private int _relearnMove4;

    // PP
    [ObservableProperty]
    private int _pp1;

    [ObservableProperty]
    private int _pp2;

    [ObservableProperty]
    private int _pp3;

    [ObservableProperty]
    private int _pp4;

    [ObservableProperty]
    private int _ppUps1;

    [ObservableProperty]
    private int _ppUps2;

    [ObservableProperty]
    private int _ppUps3;

    [ObservableProperty]
    private int _ppUps4;

    // Tech Records (Gen 8+)
    public bool HasTechRecords => _pk is ITechRecord;

    [RelayCommand]
    private async Task ImportShowdown()
    {
        var text = await _dialogService.GetClipboardTextAsync();
        if (string.IsNullOrWhiteSpace(text)) return;

        if (ShowdownParsing.TryParseAnyLanguage(text, out var set))
        {
            _pk.ApplySetDetails(set);
            LoadFromPKM();
        }
        else
        {
            await _dialogService.ShowErrorAsync(LocalizedStrings.Instance["PokemonEditor_ImportFailedTitle"], LocalizedStrings.Instance["PokemonEditor_CouldNotParseShowdown"]);
        }
    }

    [RelayCommand]
    private async Task ExportShowdown()
    {
        var pk = PreparePKM();
        var set = new ShowdownSet(pk);
        await _dialogService.SetClipboardTextAsync(set.Text);
    }

    [RelayCommand]
    private void SuggestRelearnMoves()
    {
        var pk = PreparePKM();
        var la = new LegalityAnalysis(pk, _sav.Personal);
        pk.SetRelearnMoves(la);
        
        _isLoading = true;
        RelearnMove1 = pk.RelearnMove1;
        RelearnMove2 = pk.RelearnMove2;
        RelearnMove3 = pk.RelearnMove3;
        RelearnMove4 = pk.RelearnMove4;
        _isLoading = false;
        
        Validate();
    }

    [RelayCommand]
    private void SuggestCurrentMoves()
    {
        var pk = PreparePKM();
        pk.SetMoveset(false);
        
        _isLoading = true;
        Move1 = pk.Move1;
        Move2 = pk.Move2;
        Move3 = pk.Move3;
        Move4 = pk.Move4;
        Pp1 = pk.Move1_PP;
        Pp2 = pk.Move2_PP;
        Pp3 = pk.Move3_PP;
        Pp4 = pk.Move4_PP;
        PpUps1 = pk.Move1_PPUps;
        PpUps2 = pk.Move2_PPUps;
        PpUps3 = pk.Move3_PPUps;
        PpUps4 = pk.Move4_PPUps;
        _isLoading = false;
        
        Validate();
    }

    [RelayCommand]
    private void SetAllTechRecords()
    {
        if (_pk is not ITechRecord tr) return;
        
        var pk = PreparePKM();
        tr.SetRecordFlags(pk, TechnicalRecordApplicatorOption.LegalAll);
        LoadFromPKM();
    }

    [RelayCommand]
    private void ClearTechRecords()
    {
        if (_pk is not ITechRecord tr) return;
        
        tr.ClearRecordFlags();
        LoadFromPKM();
    }

    partial void OnMove1Changed(int value) { if (!_isLoading) Validate(); }
    partial void OnMove2Changed(int value) { if (!_isLoading) Validate(); }
    partial void OnMove3Changed(int value) { if (!_isLoading) Validate(); }
    partial void OnMove4Changed(int value) { if (!_isLoading) Validate(); }
    partial void OnRelearnMove1Changed(int value) { if (!_isLoading) Validate(); }
    partial void OnRelearnMove2Changed(int value) { if (!_isLoading) Validate(); }
    partial void OnRelearnMove3Changed(int value) { if (!_isLoading) Validate(); }
    partial void OnRelearnMove4Changed(int value) { if (!_isLoading) Validate(); }
}
