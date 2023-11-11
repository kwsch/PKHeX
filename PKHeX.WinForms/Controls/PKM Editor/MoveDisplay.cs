using System;
using System.Drawing;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Drawing.Misc;

namespace PKHeX.WinForms.Controls;

public partial class MoveDisplay : UserControl
{
    public MoveDisplay() => InitializeComponent();

    public void Populate(PKM pk, ushort move, EntityContext context, ReadOnlySpan<string> moves, bool valid = true)
    {
        if (move == 0 || move >= moves.Length)
        {
            Visible = false;
            return;
        }
        Visible = true;

        byte type = MoveInfo.GetType(move, context);
        var name = moves[move];
        if (move == (int)Core.Move.HiddenPower && pk.Format < 8)
        {
            type = (byte)pk.HPType;
            name = $"{name} {pk.HPPower}";
        }

        PB_Type.Image = TypeSpriteUtil.GetTypeSpriteIcon(type);
        L_Move.Text = name;
        if (valid)
            L_Move.ResetForeColor();
        else
            L_Move.ForeColor = Color.Red;
    }
}
