using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Application.Abstractions;
using PKHeX.Application.Services;
using PKHeX.Application.UseCases;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

/// <summary>
/// Tool-window ViewModel for the Living Dex generator (Auto-Legality Mod Phase 2, issue #123): fills
/// boxes starting at a user-chosen box with one legal specimen of every species obtainable in the loaded
/// save's game. Generation runs off the UI thread with progress and cancellation; placement refuses
/// cleanly (no partial writes) if there is not enough contiguous empty space, and is recorded as a single
/// undoable operation via <see cref="UndoRedoService"/>.
/// </summary>
public partial class LivingDexGeneratorViewModel : ViewModelBase
{
    private readonly SaveFile _sav;
    private readonly ILivingDexService _service;
    private readonly UndoRedoService _undoRedo;
    private readonly LivingDexPlacementUseCase _placement = new();

    private CancellationTokenSource? _cts;

    /// <summary>Raised once boxes were actually written to, so the host can refresh the box/party viewers.</summary>
    public event Action? BoxesUpdated;

    [ObservableProperty] private bool _includeForms;
    [ObservableProperty] private bool _setShiny;

    [ObservableProperty] private ObservableCollection<string> _boxNames = [];
    [ObservableProperty] private int _selectedBoxIndex;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(GenerateCommand))]
    [NotifyCanExecuteChangedFor(nameof(CancelCommand))]
    private bool _isRunning;

    [ObservableProperty] private double _progress;

    [ObservableProperty] private string _statusMessage = "Choose options and a starting box, then Generate.";

    [ObservableProperty] private string _skippedSpeciesReport = string.Empty;

    public LivingDexGeneratorViewModel(SaveFile sav, ILivingDexService service, UndoRedoService undoRedo)
    {
        _sav = sav;
        _service = service;
        _undoRedo = undoRedo;

        BoxNames = new ObservableCollection<string>(Enumerable.Range(0, sav.BoxCount)
            .Select(b => sav is IBoxDetailNameRead r ? r.GetBoxName(b) : BoxDetailNameExtensions.GetDefaultBoxName(b)));
    }

    private bool CanGenerate => !IsRunning;

    [RelayCommand(CanExecute = nameof(CanGenerate))]
    private async Task GenerateAsync()
    {
        SkippedSpeciesReport = string.Empty;
        IsRunning = true;
        Progress = 0;
        StatusMessage = "Generating a legal Pokémon for every species in this game…";
        _cts = new CancellationTokenSource();

        try
        {
            var options = new LivingDexOptions(IncludeForms, SetShiny);
            var progress = new Progress<LivingDexGenerationProgress>(p =>
                Progress = p.Total == 0 ? 0 : 100.0 * p.Completed / p.Total);
            var token = _cts.Token;
            var startBox = SelectedBoxIndex;

            var result = await Task.Run(() => _service.Generate(_sav, options, progress, token), token);

            if (result.Cancelled)
            {
                StatusMessage = "Generation cancelled. No changes were made.";
                return;
            }

            if (result.Pokemon.Count == 0)
            {
                StatusMessage = "The engine could not generate any legal Pokémon for this save.";
                ReportSkipped(result);
                return;
            }

            var placement = await Task.Run(() => _placement.TryPlace(_sav, result.Pokemon, startBox, _undoRedo), token);

            if (placement.Status == LivingDexPlacementStatus.InsufficientSpace)
            {
                StatusMessage = $"Refused: need {placement.RequiredSlots} contiguous empty slots starting at "
                    + $"\"{BoxNames.ElementAtOrDefault(startBox) ?? $"Box {startBox + 1}"}\", but only "
                    + $"{placement.AvailableSlots} are available there. No changes were made.";
                ReportSkipped(result);
                return;
            }

            StatusMessage = $"Placed {placement.PlacedCount} legal Pokémon starting at "
                + $"\"{BoxNames.ElementAtOrDefault(startBox) ?? $"Box {startBox + 1}"}\".";
            ReportSkipped(result);
            BoxesUpdated?.Invoke();
        }
        catch (OperationCanceledException)
        {
            StatusMessage = "Generation cancelled. No changes were made.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Unexpected error: {ex.Message}";
        }
        finally
        {
            IsRunning = false;
            Progress = 0;
            _cts = null;
        }
    }

    private void ReportSkipped(LivingDexGenerationResult result)
    {
        if (result.SkippedSpeciesNames.Count == 0)
            return;

        SkippedSpeciesReport = $"{result.SkippedSpeciesNames.Count} species/forms could not be legalized and were skipped:\n"
            + string.Join(", ", result.SkippedSpeciesNames);
    }

    [RelayCommand(CanExecute = nameof(CanCancel))]
    private void Cancel() => _cts?.Cancel();

    private bool CanCancel() => IsRunning;
}
