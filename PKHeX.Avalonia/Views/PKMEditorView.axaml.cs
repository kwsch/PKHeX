using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace PKHeX.Avalonia.Views;

public partial class PKMEditorView : UserControl
{
    public PKMEditorView()
    {
        InitializeComponent();

        // Tunnel handler so we intercept wheel events before the NumericUpDown handles them
        this.AddHandler(PointerWheelChangedEvent, OnPointerWheelChanged, RoutingStrategies.Tunnel);
    }

    private void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (e.Source is NumericUpDown nud && nud.IsFocused)
        {
            var delta = e.Delta.Y > 0 ? 1 : -1;
            nud.Value = Math.Clamp((nud.Value ?? 0) + delta, nud.Minimum, nud.Maximum);
            e.Handled = true;
        }
    }
}
