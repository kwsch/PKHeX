using CommunityToolkit.Mvvm.Input;

namespace PKHeX.Presentation.ViewModels;

public partial class MainWindowViewModel
{
    // Cached so re-invoking the menu item focuses the existing tool window instead of opening a
    // duplicate (ShowTool keys off the ViewModel instance). Reset to null on save change.
    private LivingDexGeneratorViewModel? _livingDexGenerator;

    [RelayCommand(CanExecute = nameof(HasSave))]
    private void OpenLivingDexGenerator()
    {
        if (CurrentSave is null) return;

        if (_livingDexGenerator is null)
        {
            _livingDexGenerator = new LivingDexGeneratorViewModel(CurrentSave, _livingDexService, _undoRedo);
            _livingDexGenerator.BoxesUpdated += OnLivingDexBoxesUpdated;
        }

        _windowService.ShowTool(_livingDexGenerator, "Living Dex Generator");
    }

    private void OnLivingDexBoxesUpdated()
    {
        BoxViewer?.RefreshCurrentBox();
        PartyViewer?.RefreshParty();
    }
}
