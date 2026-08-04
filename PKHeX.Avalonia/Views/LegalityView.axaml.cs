using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace PKHeX.Avalonia.Views;

public partial class LegalityView : UserControl
{
    public LegalityView()
    {
        InitializeComponent();
    }

    private void OnCloseClick(object sender, RoutedEventArgs e)
    {
        var window = this.GetVisualRoot() as Window;
        window?.Close();
    }

    private async void OnCopyClick(object sender, RoutedEventArgs e)
    {
        try
        {
            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel?.Clipboard is not null)
            {
                await topLevel.Clipboard.SetTextAsync(ReportText.Text);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.TraceWarning($"Failed to copy legality report to clipboard: {ex.Message}");
        }
    }
}
