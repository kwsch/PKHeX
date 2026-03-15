using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a single plus record entry.
/// </summary>
public partial class PlusRecordEntryModel : ObservableObject
{
    public int Index { get; }
    public ushort MoveId { get; }
    public string MoveName { get; }
    public string DisplayIndex { get; }

    [ObservableProperty]
    private bool _hasFlag;

    public PlusRecordEntryModel(int index, ushort moveId, string moveName, bool hasFlag)
    {
        Index = index;
        MoveId = moveId;
        MoveName = moveName;
        DisplayIndex = index.ToString("000");
        _hasFlag = hasFlag;
    }
}

/// <summary>
/// ViewModel for the Plus Record Editor subform.
/// Edits plus record flags for Pokemon implementing <see cref="IPlusRecord"/>.
/// </summary>
public partial class PlusRecordEditorViewModel : ObservableObject
{
    private readonly IPlusRecord _plus;
    private readonly IPermitPlus _permit;
    private readonly PKM _entity;

    [ObservableProperty]
    private bool _modified;

    /// <summary>All plus record entries.</summary>
    public ObservableCollection<PlusRecordEntryModel> Records { get; } = [];

    public PlusRecordEditorViewModel(IPlusRecord plus, IPermitPlus permit, PKM pk)
    {
        _plus = plus;
        _permit = permit;
        _entity = pk;
        PopulateRecords();
    }

    private void PopulateRecords()
    {
        var names = GameInfo.Strings.Move;
        var indexes = _permit.PlusMoveIndexes;

        for (int i = 0; i < indexes.Length; i++)
        {
            var move = indexes[i];
            var moveName = move < names.Count ? names[move] : $"Move {move}";
            bool hasFlag = _plus.GetMovePlusFlag(i);

            Records.Add(new PlusRecordEntryModel(i, move, moveName, hasFlag));
        }
    }

    /// <summary>
    /// Saves all flags back to the entity.
    /// </summary>
    [RelayCommand]
    private void Save()
    {
        foreach (var rec in Records)
            _plus.SetMovePlusFlag(rec.Index, rec.HasFlag);
        Modified = true;
    }

    /// <summary>
    /// Sets all plus record flags using legal logic.
    /// </summary>
    [RelayCommand]
    private void SetAll()
    {
        Save();
        _plus.SetPlusFlags(_entity, _permit, PlusRecordApplicatorOption.LegalCurrent);
        ReloadFromEntity();
        Modified = true;
    }

    /// <summary>
    /// Clears all plus record flags.
    /// </summary>
    [RelayCommand]
    private void ClearAll()
    {
        _plus.ClearPlusFlags(_permit.PlusCountTotal);
        ReloadFromEntity();
        Modified = true;
    }

    private void ReloadFromEntity()
    {
        for (int i = 0; i < Records.Count; i++)
            Records[i].HasFlag = _plus.GetMovePlusFlag(i);
    }
}
