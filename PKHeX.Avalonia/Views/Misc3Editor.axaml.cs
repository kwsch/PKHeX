using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace PKHeX.Avalonia.Views;

public partial class Misc3Editor : UserControl
{
    public Misc3Editor()
    {
        InitializeComponent();
    }
}

public static class Misc3EditorConverters
{
    /// <summary>
    /// Converts symbol status (0=None, 1=Silver, 2=Gold) to a color.
    /// </summary>
    public static FuncValueConverter<int, IBrush> SymbolColorConverter { get; } =
        new(status => status switch
        {
            1 => Brushes.Silver,
            2 => Brushes.Gold,
            _ => Brushes.Gray
        });
}
