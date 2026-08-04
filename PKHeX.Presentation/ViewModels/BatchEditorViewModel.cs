using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class BatchEditorViewModel : ViewModelBase
{
    private readonly SaveFile _sav;
    private readonly IDialogService _dialogService;

    public event Action? BatchEditCompleted;

    public BatchEditorViewModel(SaveFile sav, IDialogService dialogService)
    {
        _sav = sav;
        _dialogService = dialogService;

        PropertySuggestions = GetCommonPkmProperties();
    }

    private static List<string> GetCommonPkmProperties()
    {
        var props = typeof(PKM)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && p.CanWrite && p.GetIndexParameters().Length == 0)
            .Select(p => p.Name)
            .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var priority = new[]
        {
            "Species", "Nickname", "CurrentLevel", "IsShiny", "Nature", "Ability",
            "Gender", "HeldItem", "Ball", "OriginalTrainerFriendship", "IsEgg",
            "IV_HP", "IV_ATK", "IV_DEF", "IV_SPA", "IV_SPD", "IV_SPE",
            "EV_HP", "EV_ATK", "EV_DEF", "EV_SPA", "EV_SPD", "EV_SPE",
            "Move1", "Move2", "Move3", "Move4",
            "OriginalTrainerName", "Language", "Version",
        };

        var result = priority.Where(props.Contains).ToList();
        result.AddRange(props.Except(priority));
        return result;
    }

    public IReadOnlyList<string> PropertySuggestions { get; }

    [ObservableProperty]
    private string _instructions = string.Empty;

    [ObservableProperty]
    private string _results = string.Empty;

    [ObservableProperty]
    private bool _editBoxes = true;

    [ObservableProperty]
    private bool _editParty;

    [ObservableProperty]
    private string _selectedProperty = string.Empty;

    [ObservableProperty]
    private string _selectedOperator = "=";

    [ObservableProperty]
    private string _selectedValue = string.Empty;

    public IReadOnlyList<string> Operators { get; } = ["=", "!", ".", "=RNG", "=POKEMON"];

    [RelayCommand]
    private void AddInstruction()
    {
        if (string.IsNullOrWhiteSpace(SelectedProperty))
            return;

        var instruction = $".{SelectedProperty}{SelectedOperator}{SelectedValue}";

        if (!string.IsNullOrEmpty(Instructions))
            Instructions += Environment.NewLine;
        Instructions += instruction;
    }

    [RelayCommand]
    private void AddFilter()
    {
        if (string.IsNullOrWhiteSpace(SelectedProperty))
            return;

        var filter = $"={SelectedProperty}{SelectedOperator}{SelectedValue}";

        if (!string.IsNullOrEmpty(Instructions))
            Instructions += Environment.NewLine;
        Instructions += filter;
    }

    [RelayCommand]
    private void ClearInstructions()
    {
        Instructions = string.Empty;
        Results = string.Empty;
    }

    [RelayCommand]
    private async Task RunBatchAsync()
    {
        if (string.IsNullOrWhiteSpace(Instructions))
        {
            Results = "No instructions provided.";
            return;
        }

        var lines = Instructions.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length == 0)
        {
            Results = "No valid instructions found.";
            return;
        }

        try
        {
            // Snapshot box/party data first so we can batch-modify in-memory
            // then write the same objects back (not a fresh re-read).
            var boxData  = EditBoxes  ? Enumerable.Range(0, _sav.BoxCount).Select(b => _sav.GetBoxData(b)).ToList()  : null;
            var partyData = EditParty ? Enumerable.Range(0, _sav.PartyCount).Select(i => _sav.GetPartySlotAtIndex(i)).ToList() : null;

            var pkms = new List<PKM>();
            if (boxData is not null)
                foreach (var box in boxData)
                    pkms.AddRange(box.Where(pk => pk.Species != 0));
            if (partyData is not null)
                pkms.AddRange(partyData.Where(pk => pk.Species != 0));

            if (pkms.Count == 0)
            {
                Results = "No Pokémon to process.";
                return;
            }

            var editor = EntityBatchProcessor.Execute(lines, pkms);
            var sets = StringInstructionSet.GetBatchSets(lines);
            Results = editor.GetEditorResults(sets);

            // Write back the modified snapshots (not a fresh re-read from save).
            if (boxData is not null)
                for (int box = 0; box < boxData.Count; box++)
                    _sav.SetBoxData(boxData[box], box);

            if (partyData is not null)
                for (int i = 0; i < partyData.Count; i++)
                    _sav.SetPartySlotAtIndex(partyData[i], i);

            BatchEditCompleted?.Invoke();
        }
        catch (Exception ex)
        {
            Results = $"Error: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task SetMaxIVs()
    {
        Instructions = ".IVs=$suggestPokemon MaxIVs($0)";
        await RunBatchAsync();
    }

    [RelayCommand]
    private async Task SetMaxEVs()
    {
        Instructions = ".EVs=$suggestPokemon MaxEVs($0)";
        await RunBatchAsync();
    }

    [RelayCommand]
    private async Task SetShiny()
    {
        Instructions = ".Shiny=Star";
        await RunBatchAsync();
    }

    [RelayCommand]
    private async Task HealAll()
    {
        Instructions = ".Heal";
        await RunBatchAsync();
    }
}
