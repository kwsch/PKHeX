using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Drawing.Misc;

namespace PKHeX.WinForms.Controls;

public partial class PokePreview : Form
{
    public PokePreview() => InitializeComponent();

    private static readonly Image[] GenderImages =
    {
        Properties.Resources.gender_0,
        Properties.Resources.gender_1,
        Properties.Resources.gender_2,
    };

    public void Populate(PKM pk)
    {
        var pi = pk.PersonalInfo;
        PopulateHeader(pk, pi);
        PopulateMoves(pk);
        PopulateStats(pk, pi);
    }

    private void PopulateHeader(PKM pk, IPersonalType pi)
    {
        L_Name.Text = pk.Nickname;
        PopulateBall(pk);
        PopulateGender(pk);
        PopulateExtra(pk);

        var type1 = pi.Type1;
        var type2 = pi.Type2;
        PB_Type1.Image = TypeSpriteUtil.GetTypeSpriteIcon(type1);
        PB_Type2.Image = type1 == type2 ? null : TypeSpriteUtil.GetTypeSpriteIcon(type2);
    }

    private void PopulateBall(PKM pk)
    {
        int ball = (int)Ball.Poke;
        if (pk.Format >= 3)
            ball = pk.Ball;
        PB_Ball.Image = Drawing.PokeSprite.SpriteUtil.GetBallSprite(ball);
    }

    private void PopulateExtra(PKM pk)
    {
        if (pk is IGigantamaxReadOnly { CanGigantamax: true })
            PB_Other.Image = Drawing.PokeSprite.Properties.Resources.dyna;
        else if (pk is ITeraTypeReadOnly tera)
            PB_Other.Image = TypeSpriteUtil.GetTypeSpriteGem((byte)tera.TeraType);
        else
            PB_Other.Image = null;
    }

    private void PopulateGender(PKM pk)
    {
        if (pk.Format == 1)
        {
            PB_Gender.Image = null;
            return;
        }

        var gender = pk.Gender;
        if (gender > GenderImages.Length)
            gender = 2;
        PB_Gender.Image = GenderImages[gender];
    }

    private void PopulateMoves(PKM pk)
    {
        var context = pk.Context;
        var names = GameInfo.Strings.movelist;
        ToggleMove(PB_Move1, L_Move1, pk.Move1, names, context);
        ToggleMove(PB_Move2, L_Move2, pk.Move2, names, context);
        ToggleMove(PB_Move3, L_Move3, pk.Move3, names, context);
        ToggleMove(PB_Move4, L_Move4, pk.Move4, names, context);
    }

    private static void ToggleMove(PictureBox type, Control text, ushort move, ReadOnlySpan<string> moves, EntityContext context)
    {
        if (move == 0 || move >= moves.Length)
        {
            type.Visible = text.Visible = false;
            return;
        }

        var moveType = MoveInfo.GetType(move, context);
        type.Image = TypeSpriteUtil.GetTypeSpriteIcon(moveType);
        type.Visible = text.Visible = true;
        text.Text = moves[move];
    }

    private void PopulateStats(PKM pk, PersonalInfo pi)
    {
        var sb = new StringBuilder(128);
        sb.AppendLine("* Base IV EV");
        sb.AppendLine($"H {pi.HP:000}  {pk.IV_HP:00} {pk.EV_HP,-3}");
        sb.AppendLine($"A {pi.ATK:000}  {pk.IV_ATK:00} {pk.EV_ATK,-3}");
        sb.AppendLine($"B {pi.DEF:000}  {pk.IV_DEF:00} {pk.EV_DEF,-3}");
        sb.AppendLine($"C {pi.SPA:000}  {pk.IV_SPA:00} {pk.EV_SPA,-3}");
        sb.AppendLine($"D {pi.SPD:000}  {pk.IV_SPD:00} {pk.EV_SPD,-3}");
        sb.AppendLine($"S {pi.SPE:000}  {pk.IV_SPE:00} {pk.EV_SPE,-3}");
        L_Stats.Text = sb.ToString();
    }
}
