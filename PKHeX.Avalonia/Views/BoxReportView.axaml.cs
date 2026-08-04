using Avalonia.Controls;
using Avalonia.Input;
using PKHeX.Presentation.ViewModels;

namespace PKHeX.Avalonia.Views;

public partial class BoxReportView : UserControl
{
    public BoxReportView()
    {
        InitializeComponent();
    }

    private void OnRowDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is BoxReportViewModel vm)
            vm.ActivateSelectedRowCommand.Execute(null);
    }
}
