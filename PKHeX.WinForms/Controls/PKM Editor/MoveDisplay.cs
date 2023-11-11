using System;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Drawing.Misc;

namespace PKHeX.WinForms.Controls;

public partial class MoveDisplay : UserControl
{
    public MoveDisplay() => InitializeComponent();

    public void Populate(ushort move, EntityContext context, ReadOnlySpan<string> moves)
    {
        if (move == 0 || move >= moves.Length)
        {
            Visible = false;
            return;
        }
        Visible = true;

        L_Move.Text = moves[move];
        var type = MoveInfo.GetType(move, context);
        PB_Type.Image = TypeSpriteUtil.GetTypeSpriteIcon(type);
    }
}
