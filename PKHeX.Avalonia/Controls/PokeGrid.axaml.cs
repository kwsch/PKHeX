using System.Collections;
using Avalonia;
using Avalonia.Controls;

namespace PKHeX.Avalonia.Controls;

/// <summary>
/// A sprite grid control for displaying Pokemon slots in a uniform grid.
/// </summary>
public partial class PokeGrid : UserControl
{
    public static readonly StyledProperty<IEnumerable?> SlotsProperty =
        AvaloniaProperty.Register<PokeGrid, IEnumerable?>(nameof(Slots));

    public IEnumerable? Slots
    {
        get => GetValue(SlotsProperty);
        set => SetValue(SlotsProperty, value);
    }

    public PokeGrid()
    {
        InitializeComponent();
    }
}
