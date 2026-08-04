using Avalonia.Controls;
using Avalonia.Interactivity;
using PKHeX.Presentation.ViewModels;

namespace PKHeX.Avalonia.Views;

public partial class AboutView : UserControl
{
    public AboutView()
    {
        InitializeComponent();
        DataContext = new AboutViewModel();
    }

    private void OnCloseClick(object sender, RoutedEventArgs e)
    {
        if (VisualRoot is Window window)
            window.Close();
    }
}
