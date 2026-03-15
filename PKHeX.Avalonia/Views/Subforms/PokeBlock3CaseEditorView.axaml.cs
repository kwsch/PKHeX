using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using PKHeX.Avalonia.ViewModels.Subforms;
using PKHeX.Core;

namespace PKHeX.Avalonia.Views.Subforms;

public partial class PokeBlock3CaseEditorView : SubformWindow
{
    public PokeBlock3CaseEditorView()
    {
        InitializeComponent();
    }

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        if (DataContext is PokeBlock3CaseEditorViewModel vm)
        {
            var cb = this.FindControl<ComboBox>("CB_Color");
            if (cb != null)
                cb.ItemsSource = vm.ColorValues;
        }
    }

    private void OnOkClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is PokeBlock3CaseEditorViewModel vm)
            vm.SaveCommand.Execute(null);
        CloseWithResult(true);
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        CloseWithResult(false);
    }
}
