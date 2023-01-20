using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms.Controls;

public partial class MoveChoice : UserControl
{
    public MoveChoice()
    {
        InitializeComponent();
        CB_Move.InitializeBinding();
    }

    public ushort SelectedMove { get => (ushort)WinFormsUtil.GetIndex(CB_Move); set => CB_Move.SelectedValue = (int)value; }
    public int PP { get => SelectedMove == 0 ? 0 : Util.ToInt32(TB_PP.Text); set => TB_PP.Text = value.ToString(); }
    public int PPUps { get => SelectedMove == 0 ? 0 : CB_PPUps.SelectedIndex; set => LoadClamp(CB_PPUps, value); }
    public bool HideLegality { private get; set; }

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
        PB_Triangle.Image = MoveDisplay.GetMoveImage(!move.Valid, entity, i);
    }

    public void HealPP(PKM pk)
    {
        var move = SelectedMove;
        PP = move <= 0 ? (PPUps = 0) : pk.GetMovePP(move, PPUps);
    }
}
