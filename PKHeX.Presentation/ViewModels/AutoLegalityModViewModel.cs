using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Application.Abstractions;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

/// <summary>
/// Tool-window ViewModel for the Auto Legality Mod: the user pastes a Pokémon Showdown set, the
/// vendored ALM engine generates a legal Pokémon for the loaded save, and the result (plus its legality
/// report) is shown. The generated Pokémon can be loaded into the main editor via <see cref="PokemonGenerated"/>.
/// </summary>
public partial class AutoLegalityModViewModel : ViewModelBase
{
    private readonly SaveFile _sav;
    private readonly IAutoLegalityService _service;
    private readonly IClipboardService _clipboard;

    private PKM? _generated;

    /// <summary>Raised when the user chooses to load the generated Pokémon into the main editor.</summary>
    public event Action<PKM>? PokemonGenerated;

    [ObservableProperty] private string _showdownText = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LegalizeCommand))]
    [NotifyCanExecuteChangedFor(nameof(PasteFromClipboardCommand))]
    private bool _isBusy;

    [ObservableProperty] private string _statusMessage = "Paste a Showdown set and press Legalize.";

    [ObservableProperty] private string _legalityReport = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoadIntoEditorCommand))]
    private bool _hasResult;

    public AutoLegalityModViewModel(SaveFile sav, IAutoLegalityService service, IClipboardService clipboard)
    {
        _sav = sav;
        _service = service;
        _clipboard = clipboard;
    }

    private bool CanInteract => !IsBusy;

    [RelayCommand(CanExecute = nameof(CanInteract))]
    private async Task PasteFromClipboardAsync()
    {
        var text = await _clipboard.GetTextAsync();
        if (!string.IsNullOrWhiteSpace(text))
            ShowdownText = text;
    }

    [RelayCommand(CanExecute = nameof(CanInteract))]
    private async Task LegalizeAsync()
    {
        _generated = null;
        HasResult = false;
        LegalityReport = string.Empty;

        if (string.IsNullOrWhiteSpace(ShowdownText))
        {
            StatusMessage = "Enter a Showdown set first.";
            return;
        }

        IsBusy = true;
        StatusMessage = "Generating a legal Pokémon…";
        try
        {
            // The engine is CPU-bound (seconds); keep the UI responsive.
            var text = ShowdownText;
            var result = await Task.Run(() => _service.TryLegalizeShowdownSet(_sav, text));

            if (result.Success && result.Pokemon is not null)
            {
                _generated = result.Pokemon;
                HasResult = true;
                LegalityReport = result.LegalityReport ?? string.Empty;
                StatusMessage = $"Legal {GetDisplayName(result.Pokemon)} generated. Load it into the editor to keep it.";
            }
            else
            {
                LegalityReport = result.LegalityReport ?? string.Empty;
                StatusMessage = result.Messages.Count > 0
                    ? result.MessageText
                    : "Legalization failed.";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Unexpected error: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand(CanExecute = nameof(HasResult))]
    private void LoadIntoEditor()
    {
        if (_generated is not null)
            PokemonGenerated?.Invoke(_generated);
    }

    private static string GetDisplayName(PKM pk)
    {
        var species = GameInfo.Strings.Species;
        return pk.Species < species.Count ? species[pk.Species] : $"species {pk.Species}";
    }
}
