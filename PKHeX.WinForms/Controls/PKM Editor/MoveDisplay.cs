using System;
using System.Drawing;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Drawing.Misc;

namespace PKHeX.WinForms.Controls;

public partial class MoveDisplay : UserControl
{
    public MoveDisplay() => InitializeComponent();

    public int Populate(PKM pk, ushort move, EntityContext context, ReadOnlySpan<string> moves, bool valid = true)
    {
        if (move == 0 || move >= moves.Length)
        {
            Visible = false;
            return 0;
        }
        Visible = true;

        byte type = MoveInfo.GetType(move, context);
        var name = moves[move];
        if (move == (int)Core.Move.HiddenPower && pk.Context is not EntityContext.Gen8a)
        {
            if (HiddenPower.TryGetTypeIndex(pk.HPType, out type))
                name = $"{name} ({GameInfo.Strings.types[type]}) [{pk.HPPower}]";
        }

        var size = PokePreview.MeasureSize(name, L_Move.Font);
        var ctrlWidth = PB_Type.Width + PB_Type.Margin.Horizontal + size.Width + L_Move.Margin.Horizontal;

        PB_Type.Image = TypeSpriteUtil.GetTypeSpriteIconSmall(type);
        L_Move.Text = name;
        if (valid)
            L_Move.ResetForeColor();
        else
            L_Move.ForeColor = Color.Red;
        L_Move.Width = size.Width;
        Width = ctrlWidth;

        return ctrlWidth;
    }
}
