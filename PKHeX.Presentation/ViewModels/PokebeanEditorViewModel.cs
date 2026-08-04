using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class PokebeanEditorViewModel : ViewModelBase
{
    private readonly SaveFile _sav;
    private readonly ResortSave7? _resortSave;

    public PokebeanEditorViewModel(SaveFile sav)
    {
        _sav = sav;

        if (sav is SAV7 sav7)
        {
            _resortSave = sav7.ResortSave;
            IsSupported = true;
            LoadBeans();
        }
    }

    public bool IsSupported { get; }

    [ObservableProperty]
    private ObservableCollection<BeanSlotViewModel> _beans = [];

    private void LoadBeans()
    {
        Beans.Clear();
        if (_resortSave is null) return;

        var names = ResortSave7.GetBeanIndexNames();
        var beanValues = _resortSave.GetBeans();

        for (int i = 0; i < beanValues.Length; i++)
        {
            Beans.Add(new BeanSlotViewModel(i, names[i], beanValues[i], SaveBeanValue));
        }
    }

    private void SaveBeanValue(int index, byte value)
    {
        if (_resortSave is null) return;
        var beans = _resortSave.GetBeans();
        beans[index] = value;
    }

    [RelayCommand]
    private void FillAll()
    {
        _resortSave?.FillBeans();
        LoadBeans();
    }

    [RelayCommand]
    private void ClearAll()
    {
        _resortSave?.ClearBeans();
        LoadBeans();
    }

    [RelayCommand]
    private void Refresh()
    {
        LoadBeans();
    }
}

public partial class BeanSlotViewModel : ViewModelBase
{
    private readonly System.Action<int, byte> _onValueChanged;

    public BeanSlotViewModel(int index, string name, byte count, System.Action<int, byte> onValueChanged)
    {
        Index = index;
        Name = name;
        _count = count;
        _onValueChanged = onValueChanged;
    }

    public int Index { get; }
    public string Name { get; }

    [ObservableProperty]
    private byte _count;

    partial void OnCountChanged(byte value)
    {
        _onValueChanged(Index, value);
    }
}
