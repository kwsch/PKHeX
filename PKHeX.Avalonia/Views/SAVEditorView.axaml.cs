using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using PKHeX.Avalonia.ViewModels;

namespace PKHeX.Avalonia.Views;

public partial class SAVEditorView : UserControl
{
    public SAVEditorView()
    {
        InitializeComponent();

        // Tunnel handler so we intercept wheel events on the box area for box navigation
        AddHandler(PointerWheelChangedEvent, OnBoxWheel, RoutingStrategies.Tunnel);
    }

    private void OnBoxWheel(object? sender, PointerWheelEventArgs e)
    {
        if (DataContext is SAVEditorViewModel vm && vm.IsLoaded)
        {
            if (e.Delta.Y > 0)
                vm.PreviousBoxCommand.Execute(null);
            else if (e.Delta.Y < 0)
                vm.NextBoxCommand.Execute(null);
            e.Handled = true;
        }
    }
}
