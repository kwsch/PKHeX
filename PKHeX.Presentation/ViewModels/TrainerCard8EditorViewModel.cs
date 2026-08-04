using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class TrainerCard8EditorViewModel : ViewModelBase
{
    private readonly SaveFile _sav;
    private readonly TrainerCard8? _card;

    public TrainerCard8EditorViewModel(SaveFile sav)
    {
        _sav = sav;

        if (sav is SAV8SWSH swsh)
        {
            _card = swsh.TrainerCard;
            IsSupported = true;
            LoadData();
        }
    }

    public bool IsSupported { get; }
    public string GameInfo => $"{_sav.Version} - Trainer Card";

    [ObservableProperty]
    private string _trainerName = string.Empty;

    [ObservableProperty]
    private int _pokeDexOwned;

    [ObservableProperty]
    private int _shinyPokemonFound;

    [ObservableProperty]
    private int _caughtPokemon;

    [ObservableProperty]
    private int _curryTypesOwned;

    [ObservableProperty]
    private int _rotoRallyScore;

    [ObservableProperty]
    private int _starter;

    [ObservableProperty]
    private bool _pokeDexComplete;

    [ObservableProperty]
    private bool _armorDexComplete;

    [ObservableProperty]
    private bool _crownDexComplete;

    public string StarterName => Starter switch
    {
        0 => "Grookey",
        1 => "Scorbunny",
        2 => "Sobble",
        _ => "Unknown"
    };

    private void LoadData()
    {
        if (_card is null) return;

        TrainerName = _card.OT;
        PokeDexOwned = _card.PokeDexOwned;
        ShinyPokemonFound = _card.ShinyPokemonFound;
        CaughtPokemon = _card.CaughtPokemon;
        CurryTypesOwned = _card.CurryTypesOwned;
        RotoRallyScore = _card.RotoRallyScore;
        Starter = _card.Starter;
        PokeDexComplete = _card.PokeDexComplete;
        ArmorDexComplete = _card.ArmorDexComplete;
        CrownDexComplete = _card.CrownDexComplete;
    }

    [RelayCommand]
    private void Save()
    {
        if (_card is null) return;

        _card.OT = TrainerName;
        _card.PokeDexOwned = (ushort)PokeDexOwned;
        _card.ShinyPokemonFound = (ushort)ShinyPokemonFound;
        _card.CaughtPokemon = CaughtPokemon;
        _card.CurryTypesOwned = (ushort)CurryTypesOwned;
        _card.RotoRallyScore = RotoRallyScore;
        _card.Starter = (byte)Starter;
        _card.PokeDexComplete = PokeDexComplete;
        _card.ArmorDexComplete = ArmorDexComplete;
        _card.CrownDexComplete = CrownDexComplete;
    }

    [RelayCommand]
    private void SetPartyToCard()
    {
        _card?.SetPartyData();
    }

    [RelayCommand]
    private void Refresh() => LoadData();
}
