using Avalonia;
using Avalonia.Controls;
using PKHeX.Presentation.ViewModels;

namespace PKHeX.Avalonia.Views;

public partial class Pokedex8Editor : UserControl
{
    public Pokedex8Editor()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(System.EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is Pokedex8EditorViewModel vm)
        {
            // Optional: Hook up events if needed, but ViewModel handles most logic
        }
    }
    
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        if (DataContext is Pokedex8EditorViewModel vm)
        {
            vm.SaveCurrent();
        }
    }
}