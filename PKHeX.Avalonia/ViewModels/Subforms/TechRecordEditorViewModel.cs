using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a single TR/TM record entry.
/// </summary>
public partial class TechRecordModel : ObservableObject
{
    public int Index { get; }
    public ushort MoveId { get; }
    public string MoveName { get; }
    public string DisplayIndex { get; }
    public bool IsPermitted { get; }

    [ObservableProperty]
    private bool _isLearned;

    public TechRecordModel(int index, ushort moveId, string moveName, bool isPermitted, bool isLearned, int baseRecordIndex)
    {
        Index = index;
        MoveId = moveId;
        MoveName = moveName;
        IsPermitted = isPermitted;
        DisplayIndex = (index + baseRecordIndex).ToString("000");
        _isLearned = isLearned;
    }
}

/// <summary>
/// ViewModel for the Tech Record Editor subform.
/// Displays and edits TR/TM flags for a Pokemon implementing <see cref="ITechRecord"/>.
/// </summary>
public partial class TechRecordEditorViewModel : ObservableObject
{
    private readonly ITechRecord _record;
    private readonly PKM _entity;

    [ObservableProperty]
    private bool _modified;

    /// <summary>All tech record entries.</summary>
    public ObservableCollection<TechRecordModel> Records { get; } = [];

    public TechRecordEditorViewModel(ITechRecord record, PKM pk)
    {
        _record = record;
        _entity = pk;
        PopulateRecords();
    }

    private void PopulateRecords()
    {
        var names = GameInfo.Strings.Move;
        var indexes = _record.Permit.RecordPermitIndexes;
        var context = _entity.Context;
        var baseRecordIndex = context == EntityContext.Gen9a ? 1 : 0;

        for (int i = 0; i < indexes.Length; i++)
        {
            var move = indexes[i];
            var moveName = move < names.Count ? names[move] : $"Move {move}";
            bool isPermitted = _record.Permit.IsRecordPermitted(i);
            bool isLearned = _record.GetMoveRecordFlag(i);

            Records.Add(new TechRecordModel(i, move, moveName, isPermitted, isLearned, baseRecordIndex));
        }
    }

    /// <summary>
    /// Saves all record flags back to the entity.
    /// </summary>
    [RelayCommand]
    private void Save()
    {
        foreach (var rec in Records)
            _record.SetMoveRecordFlag(rec.Index, rec.IsLearned);

        Modified = true;
    }

    /// <summary>
    /// Sets all record flags to learned.
    /// </summary>
    [RelayCommand]
    private void SetAll()
    {
        foreach (var rec in Records)
            rec.IsLearned = true;
    }

    /// <summary>
    /// Clears all record flags.
    /// </summary>
    [RelayCommand]
    private void ClearAll()
    {
        foreach (var rec in Records)
            rec.IsLearned = false;
    }
}
