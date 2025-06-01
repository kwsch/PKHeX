using System;
using System.Drawing;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms.Controls;

public partial class PokePreview : Form
{
    /// <summary> Minimum width to display the form. </summary>
    private readonly int InitialWidth;

    private readonly int InitialNameWidth;

    private const int interiorMargin = 4; // 2x pixel border on each side

    public PokePreview()
    {
        InitializeComponent();
        InitialWidth = Width;
        InitialNameWidth = L_Name.Width;
    }

    private static readonly Image[] GenderImages =
    [
        Properties.Resources.gender_0,
        Properties.Resources.gender_1,
        Properties.Resources.gender_2,
    ];

    public void Populate(PKM pk, in BattleTemplateExportSettings settings)
    {
        var la = new LegalityAnalysis(pk);
        int width = PopulateHeader(pk, settings);
        PopulateMoves(pk, la, settings, ref width);
        PopulateText(pk, la, settings, width);
    }

    private int PopulateHeader(PKM pk, in BattleTemplateExportSettings settings)
    {
        var name = GetNameTitle(pk, settings);
        var size = MeasureSize(name, L_Name.Font);
        L_Name.Width = Math.Max(InitialNameWidth, size.Width);
        L_Name.Text = name;

        PopulateBall(pk);
        PopulateGender(pk);

        var width = L_Name.Width + PB_Ball.Width + PB_Ball.Margin.Horizontal + PB_Gender.Width + PB_Gender.Margin.Horizontal + interiorMargin;
        return Math.Max(InitialWidth, width);
    }

    private static string GetNameTitle(PKM pk, in BattleTemplateExportSettings settings)
    {
        // Don't care about form; the user will be able to see the sprite next to the preview.
        var nick = pk.Nickname;
        var strings = settings.Localization.Strings;
        var all = strings.Species;
        var species = pk.Species;
        if (species >= all.Count)
            return nick;
        var expect = all[species];
        if (settings.IsTokenInExport(BattleTemplateToken.Nickname))
            return expect; // Nickname will be on another line.

        if (nick.Equals(expect, StringComparison.OrdinalIgnoreCase))
            return nick;
        return $"{nick} ({expect})";
    }

    private void PopulateBall(PKM pk)
    {
        var ball = (byte)Ball.Poke;
        if (pk.Format >= 3)
            ball = pk.Ball;
        PB_Ball.Image = Drawing.PokeSprite.SpriteUtil.GetBallSprite(ball);
    }

    private void PopulateGender(PKM pk)
    {
        if (pk.Format == 1)
        {
            PB_Gender.Image = null;
            return;
        }

        var gender = pk.Gender;
        if (gender >= GenderImages.Length)
            gender = 2;
        PB_Gender.Image = GenderImages[gender];
    }

    private void PopulateMoves(PKM pk, LegalityAnalysis la, in BattleTemplateExportSettings settings, ref int width)
    {
        var context = pk.Context;
        var strings = settings.Localization.Strings;
        var names = strings.movelist;
        var check = la.Info.Moves;
        var w1 = Move1.Populate(pk, strings, pk.Move1, context, names, check[0].Valid);
        var w2 = Move2.Populate(pk, strings, pk.Move2, context, names, check[1].Valid);
        var w3 = Move3.Populate(pk, strings, pk.Move3, context, names, check[2].Valid);
        var w4 = Move4.Populate(pk, strings, pk.Move4, context, names, check[3].Valid);

        var maxWidth = Math.Max(w1, Math.Max(w2, Math.Max(w3, w4)));
        width = Math.Max(width, maxWidth + Move1.Margin.Horizontal + interiorMargin);
    }

    private void PopulateText(PKM pk, LegalityAnalysis la, in BattleTemplateExportSettings settings, int width)
    {
        var (before, after) = GetBeforeAndAfter(pk, la, settings);
        var hover = Main.Settings.Hover;

        bool hasMoves = pk.MoveCount != 0;
        FLP_Moves.Visible = hasMoves;
        var height = FLP_List.Top + interiorMargin;
        if (hasMoves)
            height += FLP_Moves.Height + FLP_Moves.Margin.Vertical;
        ToggleLabel(L_LinesBeforeMoves, before, hover.PreviewShowPaste, ref width, ref height);
        ToggleLabel(L_LinesAfterMoves, after, hover.HoverSlotShowEncounter, ref width, ref height);
        Size = new Size(width, height);
    }

    private static void ToggleLabel(Control display, string text, bool visible, ref int width, ref int height)
    {
        if (!visible)
        {
            display.Visible = false;
            return;
        }

        var size = MeasureSize(text, display.Font);
        width = Math.Max(width, display.Margin.Horizontal + size.Width);
        height += size.Height + display.Margin.Vertical;
        display.Text = text;
        display.Visible = true;
    }

    public static Size MeasureSize(ReadOnlySpan<char> text, Font font)
    {
        const TextFormatFlags flags = TextFormatFlags.LeftAndRightPadding | TextFormatFlags.VerticalCenter;
        return TextRenderer.MeasureText(text, font, new Size(), flags);
    }

    private static (string Before, string After) GetBeforeAndAfter(PKM pk, LegalityAnalysis la, in BattleTemplateExportSettings settings)
    {
        var order = settings.Order;
        // Bifurcate the order into two sections; split via Moves.
        var moveIndex = settings.GetTokenIndex(BattleTemplateToken.Moves);
        var before = moveIndex == -1 ? order : order[..moveIndex];
        var after = moveIndex == -1 ? default : order[(moveIndex + 1)..];
        if (before.Length > 0 && before[0] == BattleTemplateToken.FirstLine)
            before = before[1..]; // remove first line token; trust that users don't randomly move it lower in the list.

        var start = SummaryPreviewer.GetPreviewText(pk, settings with { Order = before });
        var end = SummaryPreviewer.GetPreviewText(pk, settings with { Order = after });
        if (settings.IsTokenInExport(BattleTemplateToken.IVs, before))
            TryAppendOtherStats(pk, ref start, settings);
        else if (settings.IsTokenInExport(BattleTemplateToken.IVs, after))
            TryAppendOtherStats(pk, ref end, settings);

        if (Main.Settings.Hover.HoverSlotShowEncounter)
            end = SummaryPreviewer.AppendEncounterInfo(la, end);

        return (start, end);
    }

    private static void TryAppendOtherStats(PKM pk, ref string line, in BattleTemplateExportSettings settings)
    {
        if (pk is IGanbaru g)
            AppendGanbaru(g, ref line, settings);
        if (pk is IAwakened a)
            AppendAwakening(a, ref line, settings);
    }

    private static void AppendGanbaru(IGanbaru g, ref string line, in BattleTemplateExportSettings settings)
    {
        Span<byte> stats = stackalloc byte[6];
        g.GetGVs(stats);
        var token = BattleTemplateToken.GVs;
        TryAppend<byte>(stats, ref line, settings, token);
    }

    private static void AppendAwakening(IAwakened a, ref string line, in BattleTemplateExportSettings settings)
    {
        Span<byte> stats = stackalloc byte[6];
        a.GetAVs(stats);
        var token = BattleTemplateToken.AVs;
        TryAppend<byte>(stats, ref line, settings, token);
    }

    private static void TryAppend<T>(ReadOnlySpan<T> stats, ref string line, BattleTemplateExportSettings settings, BattleTemplateToken token) where T : unmanaged, IEquatable<T>
    {
        var localization = settings.Localization;
        var statNames = localization.Config.GetStatDisplay(settings.StatsOther);
        var value = ShowdownSet.GetStringStats(stats, default, statNames);
        if (value.Length == 0)
            return;
        var result = localization.Config.Push(token, value);
        line += Environment.NewLine + result;
    }

    /// <summary> Prevent stealing focus from the form that shows this. </summary>
    protected override bool ShowWithoutActivation => true;

    private const int WS_EX_TOPMOST = 0x00000008;
    private const int WS_EX_TOOLWINDOW = 0x00000080;
    private const int WS_EX_NOACTIVATE = 0x08000000;

    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams createParams = base.CreateParams;
            createParams.ExStyle |= WS_EX_TOPMOST | WS_EX_NOACTIVATE | WS_EX_TOOLWINDOW;
            return createParams;
        }
    }
}
