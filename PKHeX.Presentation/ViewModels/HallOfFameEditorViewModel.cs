using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class HallOfFameEditorViewModel : ViewModelBase
{
    private readonly SaveFile _sav;
    private readonly ISpriteRenderer _spriteRenderer;

    public HallOfFameEditorViewModel(SaveFile sav, ISpriteRenderer spriteRenderer)
    {
        _sav = sav;
        _spriteRenderer = spriteRenderer;

        // Determine support based on save type
        IsSupported = sav is SAV6XY or SAV6AO;

        if (IsSupported)
        {
            LoadEntries();
        }
    }

    public bool IsSupported { get; }

    [ObservableProperty]
    private ObservableCollection<HallOfFameEntryViewModel> _entries = [];

    [ObservableProperty]
    private HallOfFameEntryViewModel? _selectedEntry;

    private void LoadEntries()
    {
        Entries.Clear();

        if (_sav is SAV6XY xy)
        {
            LoadGen6Entries(xy.HallOfFame);
        }
        else if (_sav is SAV6AO ao)
        {
            LoadGen6Entries(ao.HallOfFame);
        }
    }

    private void LoadGen6Entries(HallOfFame6 hof)
    {
        for (int i = 0; i < HallOfFame6.Entries; i++)
        {
            var entryData = hof.GetEntry(i);
            var indexData = entryData[^4..];
            var index = new HallFame6Index(indexData.ToArray());

            if (!index.HasData)
                continue;

            var entry = new HallOfFameEntryViewModel(i, index.ClearIndex,
                new DateOnly((int)(2000 + index.Year), (int)index.Month, (int)index.Day));

            // Load team members
            for (int j = 0; j < HallOfFame6.PokeCount; j++)
            {
                var entityData = hof.GetEntity(i, j);
                var entity = new HallFame6Entity(entityData.ToArray(), _sav.Language);

                if (entity.Species == 0)
                    continue;

                entry.Members.Add(new HallOfFameMemberViewModel(
                    entity.Species,
                    entity.Form,
                    entity.Nickname,
                    (int)entity.Level,
                    entity.IsShiny,
                    _spriteRenderer,
                    _sav
                ));
            }

            Entries.Add(entry);
        }

        if (Entries.Count > 0)
            SelectedEntry = Entries[0];
    }

    [RelayCommand]
    private void Refresh()
    {
        LoadEntries();
    }

    [RelayCommand]
    private void ClearAll()
    {
        // Note: This is destructive - would need confirmation in real use
        Entries.Clear();
        SelectedEntry = null;
    }
}

public partial class HallOfFameEntryViewModel : ViewModelBase
{
    public HallOfFameEntryViewModel(int index, uint clearNumber, DateOnly date)
    {
        Index = index;
        ClearNumber = clearNumber;
        Date = date;
    }

    public int Index { get; }
    public uint ClearNumber { get; }
    public DateOnly Date { get; }

    public string Title => ClearNumber == 0 ? "First Clear" : $"Clear #{ClearNumber}";
    public string DateString => Date.ToString("yyyy-MM-dd");

    [ObservableProperty]
    private ObservableCollection<HallOfFameMemberViewModel> _members = [];
}

public partial class HallOfFameMemberViewModel : ViewModelBase
{
    private readonly ISpriteRenderer _spriteRenderer;
    private readonly SaveFile _sav;

    public HallOfFameMemberViewModel(ushort species, byte form, string nickname, int level, bool isShiny,
        ISpriteRenderer spriteRenderer, SaveFile sav)
    {
        Species = species;
        Form = form;
        Nickname = nickname;
        Level = level;
        IsShiny = isShiny;
        _spriteRenderer = spriteRenderer;
        _sav = sav;

        // Get species name
        var names = GameInfo.Strings.Species;
        SpeciesName = species < names.Count ? names[species] : $"Species #{species}";
    }

    public ushort Species { get; }
    public byte Form { get; }
    public string Nickname { get; }
    public int Level { get; }
    public bool IsShiny { get; }
    public string SpeciesName { get; }

    public string DisplayName => string.IsNullOrEmpty(Nickname) || Nickname == SpeciesName
        ? SpeciesName
        : $"{Nickname} ({SpeciesName})";

    public byte[]? Sprite
    {
        get
        {
            // Create a temporary PKM to render sprite
            var pk = _sav.BlankPKM;
            pk.Species = Species;
            pk.Form = Form;
            if (IsShiny)
                pk.SetShiny();
            return _spriteRenderer.GetSprite(pk);
        }
    }
}
