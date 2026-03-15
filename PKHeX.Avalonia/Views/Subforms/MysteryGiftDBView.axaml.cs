using Avalonia.Interactivity;

namespace PKHeX.Avalonia.Views.Subforms;

public partial class MysteryGiftDBView : SubformWindow
{
    public MysteryGiftDBView()
    {
        InitializeComponent();
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e)
    {
        CloseWithResult(false);
    }
}
