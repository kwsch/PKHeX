using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;


namespace PKHeX.Presentation.ViewModels;

public partial class TechRecordItemViewModel : ObservableObject
{
    public int Index { get; init; }
    public string Name { get; init; } = "";
    public string TypeName { get; init; } = "";
    public int TypeId { get; init; }
    public byte[]? TypeIcon { get; init; }
    
    // Status
    [ObservableProperty] private bool _isActive;
    [ObservableProperty] private bool _isValid;
    [ObservableProperty] private bool _isLearned; // If the Pokemon currently knows this move
}

public partial class TechRecordEditorViewModel : ViewModelBase
{
    private readonly ITechRecord _techRecord;
    private readonly PKM _pkm;
    private readonly LegalityAnalysis _legality;
    private readonly Action? _closeRequested;

    [ObservableProperty]
    private ObservableCollection<TechRecordItemViewModel> _records = new();

    public TechRecordEditorViewModel(ITechRecord techRecord, PKM pkm, Action? closeHelper = null)
    {
        _techRecord = techRecord;
        _pkm = pkm;
        _legality = new LegalityAnalysis(pkm);
        _closeRequested = closeHelper;
        
        LoadRecords();
    }

    private void LoadRecords()
    {
        var permit = _techRecord.Permit;
        var indexes = permit.RecordPermitIndexes;
        var context = _pkm.Context;
        var baseRecordIndex = context == EntityContext.Gen9a ? 1 : 0;
        
        var moveNames = GameInfo.Strings.Move;
        Span<ushort> currentMoves = stackalloc ushort[4];
        _pkm.GetMoves(currentMoves);
        
        var evos = _legality.Info.EvoChainsAllGens.Get(context);
        var list = new List<TechRecordItemViewModel>();

        for (int i = 0; i < indexes.Length; i++)
        {
            var move = indexes[i];
            var type = MoveInfo.GetType(move, context);
            
            // Validity
            bool isValid = permit.IsRecordPermitted(i);
            // Note: WinForms also checked permit.IsRecordPermitted(evos, i) for "Hint" color (yellow)
            
            bool isActive = _techRecord.GetMoveRecordFlag(i + baseRecordIndex); // Logic from WinForms: row.Cells[ColumnHasFlag].Value = Record.GetMoveRecordFlag(index); where index is from ColumnIndex.
            // WinForms ColumnIndex value was (i + baseRecordIndex).
            // WinForms GetMoveRecordFlag takes the 'index' which is the TR Index (0..99).
            // Re-verifying WinForms logic:
            // cells[ColumnIndex].Value = (i+ baseRecordIndex).ToString("000");
            // LoadRecords: index = int.Parse(row...Value); Record.GetMoveRecordFlag(index);
            // So yes, GetMoveRecordFlag takes the displayed index.
            
            bool isLearned = currentMoves.Contains(move);
            
            // Icon
            // TypeSpriteUtil logic to get icon.
            // Need to convert GDI+ bitmap if using PKHeX.Drawing.Misc
            // Or use SpriteLoader if it supports types. SpriteLoader has GetItemSprite but not Type sprite.
            // I'll assume null icon for now or use a placeholder/text color.
            // Actually, TypeSpriteUtil.GetTypeSpriteIconSmall(type) returns Bitmap.
            
            list.Add(new TechRecordItemViewModel
            {
                Index = i + baseRecordIndex,
                Name = moveNames[move],
                TypeId = type,
                TypeName = ((MoveType)type).ToString(),
                IsValid = isValid,
                IsActive = isActive,
                IsLearned = isLearned,
            });
        }
        
        Records = new ObservableCollection<TechRecordItemViewModel>(list);
    }
    
    [RelayCommand]
    private void Save()
    {
        foreach (var item in Records)
        {
            _techRecord.SetMoveRecordFlag(item.Index, item.IsActive);
        }
        _closeRequested?.Invoke();
    }
    
    [RelayCommand]
    private void GiveAll()
    {
         _techRecord.SetRecordFlags(_pkm, TechnicalRecordApplicatorOption.LegalAll);
         Reload();
    }
    
    [RelayCommand]
    private void RemoveAll()
    {
        _techRecord.ClearRecordFlags();
        Reload();
    }
    
    private void Reload()
    {
        // Re-read values
        foreach (var item in Records)
        {
            item.IsActive = _techRecord.GetMoveRecordFlag(item.Index);
        }
    }
    [RelayCommand]
    private void Close() => _closeRequested?.Invoke();
}
