using System;
using System.Windows.Forms;

namespace PKHeX.WinForms.Controls;

public class SelectablePictureBox : PictureBox
{
    public SelectablePictureBox() => SetStyle(ControlStyles.Selectable, TabStop = true);

    protected override void OnMouseDown(MouseEventArgs e)
    {
        Focus();
        base.OnMouseDown(e);
    }
    protected override void OnEnter(EventArgs e)
    {
        Invalidate();
        base.OnEnter(e);
    }
    protected override void OnLeave(EventArgs e)
    {
        Invalidate();
        base.OnLeave(e);
    }
    protected override void OnPaint(PaintEventArgs pe)
    {
        base.OnPaint(pe);
        if (!Focused)
            return;
        var rc = ClientRectangle;
        rc.Inflate(-2, -2);
        ControlPaint.DrawFocusRectangle(pe.Graphics, rc);
    }
}
