using Avalonia.Controls;
using Avalonia.Input;
using PKHeX.Presentation.ViewModels;

namespace PKHeX.Avalonia.Views;

public partial class LegalityAuditView : UserControl
{
    public LegalityAuditView()
    {
        InitializeComponent();
    }

    private void OnRowDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is LegalityAuditViewModel vm)
            vm.ActivateSelectedRowCommand.Execute(null);
    }
}
