using PKHeX.Core;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace PKHeX.WinForms.Controls;

public class RenderedString : TextBox
{
    public bool DisableInGameFont
    {
        get;
        set
        {
            if (value == field)
                return;
            field = value;
            if (value)
                ResetFont();
            else
                ChangeContext(DisplayContext);
        }
    }

    /// <summary>
    /// Context the string is rendered in.
    /// </summary>
    public EntityContext DisplayContext
    {
        get;
        set
        {
            if (value == field)
                return;
            field = value;
            if (!DisableInGameFont)
                ChangeContext(value);
        }
    }

    /// <summary>
    /// Fetches the necessary font.
    /// </summary>
    private void ChangeContext(EntityContext context)
    {
        if (DesignMode)
            return;

        Font = FontUtil.GetFont(context);
    }
}
