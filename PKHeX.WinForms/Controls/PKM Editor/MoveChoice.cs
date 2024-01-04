using System;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Drawing.Misc;

namespace PKHeX.WinForms.Controls;

public partial class MoveChoice : UserControl
{
    private EntityContext Context;

    public MoveChoice()
    {
        InitializeComponent();
        CB_Move.InitializeBinding();
    }

    public ushort SelectedMove { get => (ushort)WinFormsUtil.GetIndex(CB_Move); set => CB_Move.SelectedValue = (int)value; }
    public int PP { get => SelectedMove == 0 ? 0 : Util.ToInt32(TB_PP.Text); set => TB_PP.Text = value.ToString(); }
    public int PPUps { get => SelectedMove == 0 ? 0 : CB_PPUps.SelectedIndex; set => LoadClamp(CB_PPUps, value); }
    public bool HideLegality { private get; set; }
    public void SetContext(EntityContext context) => Context = context;

    private void UpdateTypeSprite(int value)
    {
        if (value <= 0)
        {
            PB_Type.Image = null;
            return;
        }

        var type = MoveInfo.GetType((ushort)value, Context);
        PB_Type.Image = TypeSpriteUtil.GetTypeSpriteIconSmall(type);
    }

    private static void LoadClamp(ComboBox cb, int value)
    {
        var max = cb.Items.Count - 1;
        if (value > max)
            value = max;
        else if (value < -1)
            value = 0;
        cb.SelectedIndex = value;
    }

    public void UpdateLegality(MoveResult move, PKM entity, int i)
    {
        if (HideLegality)
        {
            PB_Triangle.Visible = false;
            return;
        }
        PB_Triangle.Visible = true;
        PB_Triangle.Image = MoveDisplayState.GetMoveImage(!move.Valid, entity, i);
    }

    public void HealPP(PKM pk)
    {
        var move = SelectedMove;
        PP = move <= 0 ? (PPUps = 0) : pk.GetMovePP(move, PPUps);
    }

    private void CB_Move_SelectedIndexChanged(object sender, EventArgs e)
    {
        var value = WinFormsUtil.GetIndex(CB_Move);
        UpdateTypeSprite(value);
    }
}
