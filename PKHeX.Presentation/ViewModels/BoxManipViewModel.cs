using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;
using PKHeX.Presentation.Localization;

namespace PKHeX.Presentation.ViewModels;

public partial class BoxManipViewModel : ViewModelBase
{
    private readonly SaveFile _sav;
    private readonly IDialogService _dialogService;
    private readonly Action _onComplete;

    public BoxManipViewModel(SaveFile sav, IDialogService dialogService, Action onComplete)
    {
        _sav = sav;
        _dialogService = dialogService;
        _onComplete = onComplete;

        LoadManipulations();
    }

    [ObservableProperty]
    private ObservableCollection<BoxManipItemViewModel> _sortActions = [];

    [ObservableProperty]
    private ObservableCollection<BoxManipItemViewModel> _clearActions = [];

    [ObservableProperty]
    private ObservableCollection<BoxManipItemViewModel> _modifyActions = [];

    [ObservableProperty]
    private bool _applyToAllBoxes = true;

    [ObservableProperty]
    private int _currentBox;

    public int BoxCount => _sav.BoxCount;

    private void LoadManipulations()
    {
        foreach (var m in BoxManipDefaults.SortCommon.Concat(BoxManipDefaults.SortAdvanced))
        {
            if (m.Usable(_sav))
                SortActions.Add(new BoxManipItemViewModel(m));
        }

        foreach (var m in BoxManipDefaults.ClearCommon)
        {
            if (m.Usable(_sav))
                ClearActions.Add(new BoxManipItemViewModel(m));
        }

        foreach (var m in BoxManipDefaults.ModifyCommon)
        {
            if (m.Usable(_sav))
                ModifyActions.Add(new BoxManipItemViewModel(m));
        }
    }

    [RelayCommand]
    private async Task ExecuteAsync(BoxManipItemViewModel? item)
    {
        if (item?.Manip is not { } manip)
            return;

        var start = ApplyToAllBoxes ? 0 : CurrentBox;
        var stop = ApplyToAllBoxes ? _sav.BoxCount - 1 : CurrentBox;
        var param = new BoxManipParam(start, stop, false);

        try
        {
            var count = manip.Execute(_sav, param);
            var message = count > 0 
                ? manip.GetSuccess(ApplyToAllBoxes) + $" ({count} slots affected)" 
                : manip.GetFail(ApplyToAllBoxes);
            
            await _dialogService.ShowInformationAsync(LocalizedStrings.Instance["BoxManip_Title"], message);

            if (count > 0)
                _onComplete?.Invoke();
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync(LocalizedStrings.Instance["BoxManip_ErrorTitle"], ex.Message);
        }
    }
}

public class BoxManipItemViewModel(BoxManipBase manip)
{
    public BoxManipBase Manip { get; } = manip;
    public string Name => Manip.Type.ToString().Replace("Sort", "").Replace("Delete", "").Replace("Modify", "");
    public BoxManipType Type => Manip.Type;
}
