using Avalonia;
using Avalonia.Controls;
using PKHeX.Presentation.ViewModels;

namespace PKHeX.Avalonia.Views;

public partial class Pokedex7bEditor : UserControl
{
    public Pokedex7bEditor()
    {
        InitializeComponent();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        if (DataContext is Pokedex7bEditorViewModel vm)
        {
            vm.SaveCommand.Execute(null);
        }
    }
}
