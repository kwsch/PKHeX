using System;
using System.Windows.Forms;

namespace PKHeX.WinForms.Controls;

public partial class ContextMenuPKM : UserControl
{
    public ContextMenuPKM() => InitializeComponent();

    public event EventHandler? RequestEditorLegality;
    public event EventHandler? RequestEditorQR;
    public event EventHandler? RequestEditorSaveAs;
    private void ClickShowLegality(object sender, EventArgs e) => RequestEditorLegality?.Invoke(sender, e);
    private void ClickShowQR(object sender, EventArgs e) => RequestEditorQR?.Invoke(sender, e);
    private void ClickSaveAs(object sender, EventArgs e) => RequestEditorSaveAs?.Invoke(sender, e);
}
