using Avalonia.Controls;

namespace PKHeX.Android;

/// <summary>
/// The full desktop layout hosted as a single Android view. Android cannot create a second
/// Avalonia Window, so the shared layout is compiled against UserControl instead.
/// </summary>
public partial class AndroidMainView : UserControl
{
    public AndroidMainView()
    {
        InitializeComponent();
    }
}
