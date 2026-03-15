using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// ViewModel for the Gen 6 Hall of Fame viewer/editor.
/// Shows 16 entries with 6 team members each.
/// </summary>
public partial class SAVHallOfFame6ViewModel : SaveEditorViewModelBase
{
    private readonly SAV6 _origin;
    private readonly SAV6 _sav;
    private readonly HallOfFame6 _fame;
    private bool _loading;

    public List<string> EntryNames { get; } = [];
    public List<ComboItem> SpeciesList { get; }
    public List<ComboItem> ItemsList { get; }
    public List<ComboItem> MovesList { get; }

    [ObservableProperty]
    private int _selectedEntryIndex = -1;

    [ObservableProperty]
    private int _memberIndex = 1;

    [ObservableProperty]
    private int _maxMembers = 6;

    [ObservableProperty]
    private int _selectedSpecies;

    [ObservableProperty]
    private int _selectedItem;

    [ObservableProperty]
    private int _move1;

    [ObservableProperty]
    private int _move2;

    [ObservableProperty]
    private int _move3;

    [ObservableProperty]
    private int _move4;

    [ObservableProperty]
    private string _ec = "00000000";

    [ObservableProperty]
    private string _tid = "00000";

    [ObservableProperty]
    private string _sid = "00000";

    [ObservableProperty]
    private string _nickname = string.Empty;

    [ObservableProperty]
    private string _otName = string.Empty;

    [ObservableProperty]
    private bool _isShiny;

    [ObservableProperty]
    private bool _isNicknamed;

    [ObservableProperty]
    private int _level;

    [ObservableProperty]
    private string _entryText = string.Empty;

    [ObservableProperty]
    private string _clearIndex = "000";

    public SAVHallOfFame6ViewModel(SAV6 sav) : base(sav)
    {
        _sav = (SAV6)(_origin = sav).Clone();
        _fame = ((ISaveBlock6Main)_sav).HallOfFame;

        var filtered = GameInfo.FilteredSources;
        SpeciesList = filtered.Species.ToList();
        ItemsList = filtered.Items.ToList();
        MovesList = filtered.Moves.ToList();

        for (int i = 0; i < HallOfFame6.Entries; i++)
            EntryNames.Add($"Entry {i}");

        SelectedEntryIndex = 0;
    }

    partial void OnSelectedEntryIndexChanged(int value)
    {
        if (value < 0)
            return;
        LoadEntry(value);
    }

    partial void OnMemberIndexChanged(int value)
    {
        if (_loading || value < 1 || SelectedEntryIndex < 0)
            return;
        LoadMember(SelectedEntryIndex, value - 1);
    }

    private void LoadEntry(int index)
    {
        _loading = true;
        var span = _fame.GetEntry(index);
        var vnd = new HallFame6Index(span[^4..]);
        ClearIndex = vnd.ClearIndex.ToString("000");

        var sb = new StringBuilder();
        sb.AppendLine($"Entry #{vnd.ClearIndex}");

        if (!vnd.HasData)
        {
            sb.AppendLine("No records in this slot.");
            EntryText = sb.ToString();
            MemberIndex = 1;
            MaxMembers = 1;
            _loading = false;
            return;
        }

        var year = vnd.Year + 2000;
        var month = vnd.Month;
        var day = vnd.Day;
        sb.AppendLine($"Date: {year}/{month:00}/{day:00}");

        int monCount = 0;
        for (int i = 0; i < 6; i++)
        {
            var slice = span[(i * HallFame6Entity.SIZE)..];
            var entity = new HallFame6Entity(slice, _sav.Language);
            if (entity.Species == 0)
                continue;
            monCount++;
            var str = GameInfo.Strings;
            sb.AppendLine($"  {str.Species[entity.Species]} Lv.{entity.Level} - {str.Item[entity.HeldItem]}");
        }

        EntryText = sb.ToString();
        MaxMembers = monCount == 0 ? 1 : monCount;
        MemberIndex = 1;
        LoadMember(index, 0);
        _loading = false;
    }

    private void LoadMember(int entryIndex, int memberIdx)
    {
        _loading = true;
        var slice = _fame.GetEntity(entryIndex, memberIdx);
        var entity = new HallFame6Entity(slice, _sav.Language);

        SelectedSpecies = entity.Species;
        SelectedItem = entity.HeldItem;
        Move1 = entity.Move1;
        Move2 = entity.Move2;
        Move3 = entity.Move3;
        Move4 = entity.Move4;
        Ec = entity.EncryptionConstant.ToString("X8");
        Tid = entity.TID16.ToString("00000");
        Sid = entity.SID16.ToString("00000");
        Nickname = entity.Nickname;
        OtName = entity.OriginalTrainerName;
        IsShiny = entity.IsShiny;
        IsNicknamed = entity.IsNicknamed;
        Level = (int)entity.Level;
        _loading = false;
    }

    private void SaveMember()
    {
        if (SelectedEntryIndex < 0 || MemberIndex < 1)
            return;
        var slice = _fame.GetEntity(SelectedEntryIndex, MemberIndex - 1);
        var entity = new HallFame6Entity(slice, _sav.Language)
        {
            Species = (ushort)SelectedSpecies,
            HeldItem = (ushort)SelectedItem,
            Move1 = (ushort)Move1,
            Move2 = (ushort)Move2,
            Move3 = (ushort)Move3,
            Move4 = (ushort)Move4,
            EncryptionConstant = Convert.ToUInt32(Ec, 16),
            TID16 = ushort.TryParse(Tid, out var tid) ? tid : (ushort)0,
            SID16 = ushort.TryParse(Sid, out var sid) ? sid : (ushort)0,
            Nickname = Nickname,
            OriginalTrainerName = OtName,
            IsShiny = IsShiny,
            IsNicknamed = IsNicknamed,
            Level = (uint)Math.Clamp(Level, 0, 100),
        };

        var span = _fame.GetEntry(SelectedEntryIndex);
        _ = new HallFame6Index(span[^4..])
        {
            ClearIndex = ushort.TryParse(ClearIndex, out var ci) ? ci : (ushort)0,
            HasData = true,
        };
    }

    [RelayCommand]
    private void DeleteEntry()
    {
        if (SelectedEntryIndex < 1)
            return;
        _fame.ClearEntry(SelectedEntryIndex);
        LoadEntry(SelectedEntryIndex);
    }

    [RelayCommand]
    private void Save()
    {
        SaveMember();
        _origin.CopyChangesFrom(_sav);
        Modified = true;
    }
}
