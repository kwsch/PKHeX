using Avalonia;
using Avalonia.Controls;
using PKHeX.Presentation.ViewModels;

namespace PKHeX.Avalonia.Views;

public partial class Pokedex8bEditor : UserControl
{
    public Pokedex8bEditor()
    {
        InitializeComponent();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        if (DataContext is Pokedex8bEditorViewModel vm)
        {
            vm.SaveCurrent();
        }
    }
}
