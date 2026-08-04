using CommunityToolkit.Mvvm.Input;

namespace PKHeX.Presentation.ViewModels;

public partial class MainWindowViewModel
{
    // Cached so re-invoking the menu item focuses the existing tool window instead of opening a
    // duplicate (ShowTool keys off the ViewModel instance). Reset to null on save change.
    private AutoLegalityModViewModel? _autoLegalityMod;

    [RelayCommand(CanExecute = nameof(HasSave))]
    private void OpenAutoLegalityMod()
    {
        if (CurrentSave is null) return;

        if (_autoLegalityMod is null)
        {
            _autoLegalityMod = new AutoLegalityModViewModel(CurrentSave, _autoLegalityService, _clipboardService);
            _autoLegalityMod.PokemonGenerated += LoadEntity;
        }

        _windowService.ShowTool(_autoLegalityMod, "Auto Legality Mod");
    }
}
