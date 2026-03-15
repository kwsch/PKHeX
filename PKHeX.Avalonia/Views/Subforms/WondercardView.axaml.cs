using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using PKHeX.Avalonia.ViewModels.Subforms;

namespace PKHeX.Avalonia.Views.Subforms;

public partial class WondercardView : SubformWindow
{
    public WondercardView()
    {
        InitializeComponent();
    }

    private void OnOkClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is WondercardViewModel vm)
            vm.SaveCommand.Execute(null);
        CloseWithResult(true);
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        CloseWithResult(false);
    }

    private void OnSlotClick(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Border { Tag: int index })
            return;
        if (DataContext is not WondercardViewModel vm)
            return;
        if (index >= 0 && index < vm.Slots.Count)
            vm.SelectedSlot = vm.Slots[index];
    }
}
