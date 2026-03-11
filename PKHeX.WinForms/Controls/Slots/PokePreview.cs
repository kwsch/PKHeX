using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Drawing.Misc;
using PKHeX.Drawing.PokeSprite;

namespace PKHeX.WinForms.Controls;

public sealed partial class PokePreview : Form
{
    private static readonly Image[] GenderImages =
    [
        Properties.Resources.gender_0,
        Properties.Resources.gender_1,
        Properties.Resources.gender_2,
    ];

    private readonly List<RenderTextLine> TextLinesPre = [];
    private readonly List<RenderMoveLine> MoveLines = [];
    private readonly List<RenderTextLine> TextLinesHint = [];
    private readonly List<RenderTextLine> TextLinesEncounter = [];

    private string HeaderName = string.Empty;
    private Image? HeaderBall;
    private Image? HeaderGender;

    private const int Border = 1;
    private const int HeaderTopPadding = 4;
    private const int HeaderBottomPadding = 4;
    private const int HeaderLeftPadding = 4;
    private const int HeaderRightPadding = 4;
    private const int HeaderIconGap = 2;

    private const int BodyTopPadding = 2;
    private const int BodyBottomPadding = 2;
    private const int BodyLeftPadding = 4;
    private const int BodyRightPadding = 4;

    private const int TextSectionTopPadding = 2;
    private const int TextSectionBottomPadding = 2;
    private const int TextLineSpacing = 1;

    private const int MoveIconTextGap = 2;
    private const int MoveSectionTopPadding = 4;
    private const int MoveSectionBottomPadding = 8;

    private const int IconSize = 24;

    private static Color IllegalTextColor => WinFormsUtil.ColorWarn;

    public PokePreview()
    {
        InitializeComponent();

        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        UpdateStyles();
    }

    public void Populate(PKM pk, in BattleTemplateExportSettings settings, in LegalityLocalizationContext ctx)
    {
        var main = Main.Settings;
        HeaderName = GetNameTitle(pk, settings);

        if (pk.Format > 2)
            HeaderBall = GetBallImage(pk);
        else
            HeaderBall = null;

        if (pk.Format != 1 || main.EntityEditor.ShowGenderGen1)
            HeaderGender = GetGenderImage(pk);
        else
            HeaderGender = null;

        TextLinesPre.Clear();
        MoveLines.Clear();
        TextLinesHint.Clear();
        TextLinesEncounter.Clear();

        var hover = Main.Settings.Hover;
        var (before, mid, after) = GetBeforeAndAfter(pk, ctx, settings);

        AppendTextSection(TextLinesPre, before, hover.PreviewShowPaste, ForeColor);
        BuildMoves(pk, ctx.Analysis, settings);
        AppendTextSection(TextLinesHint, mid, hover.HoverSlotShowLegalityHint, IllegalTextColor);
        AppendTextSection(TextLinesEncounter, after, hover.HoverSlotShowEncounter, ForeColor);

        ApplySize();
        Invalidate();
    }

    private void BuildMoves(PKM pk, LegalityAnalysis la, in BattleTemplateExportSettings settings)
    {
        if (pk.MoveCount == 0)
            return;

        var context = pk.Context;
        var strings = settings.Localization.Strings;
        var names = strings.movelist;
        var checks = la.Info.Moves;

        AppendMoveLine(pk, strings, names, context, pk.Move1, checks[0].Valid);
        AppendMoveLine(pk, strings, names, context, pk.Move2, checks[1].Valid);
        AppendMoveLine(pk, strings, names, context, pk.Move3, checks[2].Valid);
        AppendMoveLine(pk, strings, names, context, pk.Move4, checks[3].Valid);
    }

    private void AppendMoveLine(PKM pk, GameStrings strings, ReadOnlySpan<string> names, EntityContext context, ushort move, bool valid)
    {
        if (move == 0 || move >= names.Length)
            return;

        byte type = MoveInfo.GetType(move, context);
        var name = names[move];
        if (move == (int)PKHeX.Core.Move.HiddenPower && pk.Context is not EntityContext.Gen8a)
        {
            if (HiddenPower.TryGetTypeIndex(pk.HPType, out type))
                name = $"{name} ({strings.types[type]}) [{pk.HPPower}]";
        }

        var image = TypeSpriteUtil.GetTypeSpriteIconSmall(type);
        var color = valid ? ForeColor : IllegalTextColor;
        MoveLines.Add(new RenderMoveLine(name, image, color));
    }

    private static void AppendTextSection(List<RenderTextLine> list, ReadOnlySpan<char> text, bool visible, Color color)
    {
        if (!visible || text.Length == 0)
            return;

        int i = 0;
        var lines = text.EnumerateLines();
        foreach (var line in lines)
        {
            var topPadding = i == 0 ? TextSectionTopPadding : TextLineSpacing;
            const int bottomPadding = 0;
            list.Add(new RenderTextLine(line.ToString(), color, topPadding, bottomPadding));
            i++;
        }

        var last = list[^1];
        list[^1] = last with { BottomPadding = TextSectionBottomPadding };
    }

    private void ApplySize()
    {
        var width = GetPreferredWidth();
        var height = GetPreferredHeight();
        var size = new Size(width, height);
        if (Size != size)
            Size = size;
    }

    private int GetPreferredWidth()
    {
        var width = 0;

        var nameSize = MeasureSize(HeaderName, Font);
        var headerWidth = Border + HeaderLeftPadding + nameSize.Width + HeaderRightPadding + Border;
        if (HeaderBall != null)
            headerWidth += IconSize + HeaderIconGap;
        if (HeaderGender != null)
            headerWidth += IconSize + HeaderIconGap;

        width = Math.Max(width, headerWidth);

        foreach (var move in MoveLines)
        {
            var size = MeasureSize(move.Text, Font);
            var lineWidth = Border + BodyLeftPadding + IconSize + MoveIconTextGap + size.Width + BodyRightPadding + Border;
            width = Math.Max(width, lineWidth);
        }
        foreach (var line in TextLinesPre)
        {
            var size = MeasureSize(line.Text, Font);
            var lineWidth = Border + BodyLeftPadding + size.Width + BodyRightPadding + Border;
            width = Math.Max(width, lineWidth);
        }
        foreach (var line in TextLinesHint)
        {
            var size = MeasureSize(line.Text, Font);
            var lineWidth = Border + BodyLeftPadding + size.Width + BodyRightPadding + Border;
            width = Math.Max(width, lineWidth);
        }
        foreach (var line in TextLinesEncounter)
        {
            var size = MeasureSize(line.Text, Font);
            var lineWidth = Border + BodyLeftPadding + size.Width + BodyRightPadding + Border;
            width = Math.Max(width, lineWidth);
        }

        return width;
    }

    private int GetPreferredHeight()
    {
        var height = Border;
        height += HeaderTopPadding + IconSize + HeaderBottomPadding;
        height += Border;

        height += BodyTopPadding;

        if (MoveLines.Count != 0)
        {
            height += MoveSectionTopPadding;
            height += MoveLines.Count * IconSize;
            height += MoveSectionBottomPadding;
        }

        foreach (var line in TextLinesPre)
        {
            height += line.TopPadding;
            var textHeight = Math.Max(Font.Height, MeasureSize(line.Text, Font).Height);
            height += textHeight;
            height += line.BottomPadding;
        }
        foreach (var line in TextLinesHint)
        {
            height += line.TopPadding;
            var textHeight = Math.Max(Font.Height, MeasureSize(line.Text, Font).Height);
            height += textHeight;
            height += line.BottomPadding;
        }
        foreach (var line in TextLinesEncounter)
        {
            height += line.TopPadding;
            var textHeight = Math.Max(Font.Height, MeasureSize(line.Text, Font).Height);
            height += textHeight;
            height += line.BottomPadding;
        }

        height += BodyBottomPadding;
        height += Border;

        return height;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        var g = e.Graphics;
        g.Clear(BackColor);

        var outer = new Rectangle(0, 0, Width - 1, Height - 1);
        g.DrawRectangle(SystemPens.ControlDark, outer);

        var headerTop = Border;
        var headerHeight = HeaderTopPadding + IconSize + HeaderBottomPadding;
        var headerBottom = headerTop + headerHeight;
        g.DrawLine(SystemPens.ControlDark, Border, headerBottom, Width - Border - 1, headerBottom);

        DrawHeader(g, headerTop);

        var y = headerBottom + BodyTopPadding;
        y = DrawTextLines(TextLinesPre, g, y);
        y = DrawMoves(g, y);
        y = DrawTextLines(TextLinesHint, g, y);

        if (TextLinesEncounter.Count != 0)
        {
            g.DrawLine(SystemPens.ControlDark, Border, y, Width - Border - 1, y);
            y += Border;
            _ = DrawTextLines(TextLinesEncounter, g, y);
        }
    }

    private void DrawHeader(Graphics g, int headerTop)
    {
        var y = headerTop + HeaderTopPadding;
        var x = Border + HeaderLeftPadding;

        if (HeaderBall is not null)
        {
            g.DrawImage(HeaderBall, new Rectangle(x, y, IconSize, IconSize));
            x += IconSize + HeaderIconGap;
        }

        var textRect = new Rectangle(x, y, Math.Max(0, Width - x - Border - HeaderRightPadding - IconSize - HeaderIconGap), IconSize);
        TextRenderer.DrawText(g, HeaderName, Font, textRect, ForeColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);

        // Gender displayed at right edge.
        if (HeaderGender is not null)
        {
            var genderX = Width - Border - HeaderRightPadding - IconSize;
            g.DrawImage(HeaderGender, new Rectangle(genderX, y, IconSize, IconSize));
        }
    }

    private int DrawMoves(Graphics g, int y)
    {
        if (MoveLines.Count == 0)
            return y;

        const int x = Border + BodyLeftPadding;
        y += MoveSectionTopPadding;
        foreach (var line in MoveLines)
        {
            if (line.Icon is not null)
                g.DrawImage(line.Icon, new Rectangle(x, y, IconSize, IconSize));

            var textRect = new Rectangle(x + IconSize + MoveIconTextGap, y, Math.Max(0, Width - (x + IconSize + MoveIconTextGap) - Border - BodyRightPadding), IconSize);
            TextRenderer.DrawText(g, line.Text, Font, textRect, line.Color, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
            y += IconSize;
        }
        y += MoveSectionBottomPadding;

        return y;
    }

    private int DrawTextLines(List<RenderTextLine> list, Graphics g, int y)
    {
        const int x = Border + BodyLeftPadding;
        var textWidth = Math.Max(0, Width - x - Border - BodyRightPadding);

        foreach (var line in list)
        {
            y += line.TopPadding;
            var height = Math.Max(Font.Height, MeasureSize(line.Text, Font).Height);
            var rect = new Rectangle(x, y, textWidth, height);
            TextRenderer.DrawText(g, line.Text, Font, rect, line.Color, TextFormatFlags.Left | TextFormatFlags.NoPadding);
            y += height + line.BottomPadding;
        }

        return y;
    }

    public static Size MeasureSize(ReadOnlySpan<char> text, Font font)
    {
        const TextFormatFlags flags = TextFormatFlags.LeftAndRightPadding | TextFormatFlags.VerticalCenter;
        return TextRenderer.MeasureText(text, font, new Size(), flags);
    }

    private static string GetNameTitle(PKM pk, in BattleTemplateExportSettings settings)
    {
        var nick = pk.Nickname;
        var strings = settings.Localization.Strings;
        var all = strings.Species;
        var species = pk.Species;
        if (species >= all.Count)
            return nick;
        var expect = all[species];
        if (settings.IsTokenInExport(BattleTemplateToken.Nickname))
            return expect;

        if (nick.Equals(expect, StringComparison.OrdinalIgnoreCase))
            return nick;
        return $"{nick} ({expect})";
    }

    private static Image GetBallImage(PKM pk)
    {
        var ball = (byte)Ball.Poke;
        if (pk.Format >= 3)
            ball = pk.Ball;
        return SpriteUtil.GetBallSprite(ball);
    }

    private static Image? GetGenderImage(PKM pk)
    {
        if (pk.Format == 1)
            return null;

        var gender = pk.Gender;
        if (gender >= GenderImages.Length)
            gender = 2;
        return GenderImages[gender];
    }

    private static (string Before, string Middle, string After) GetBeforeAndAfter(PKM pk, in LegalityLocalizationContext la, in BattleTemplateExportSettings settings)
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

        var mid = "";
        if (Main.Settings.Hover.HoverSlotShowLegalityHint)
            mid = SummaryPreviewer.AppendLegalityHint(la, mid);

        if (Main.Settings.Hover.HoverSlotShowEncounter)
            end = SummaryPreviewer.AppendEncounterInfo(la, end);

        return (start, mid, end);
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
        TryAppend(stats, ref line, settings, BattleTemplateToken.GVs);
    }

    private static void AppendAwakening(IAwakened a, ref string line, in BattleTemplateExportSettings settings)
    {
        Span<byte> stats = stackalloc byte[6];
        a.GetAVs(stats);
        TryAppend(stats, ref line, settings, BattleTemplateToken.AVs);
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

    private readonly record struct RenderMoveLine(string Text, Image? Icon, Color Color);
    private readonly record struct RenderTextLine(string Text, Color Color, int TopPadding, int BottomPadding);
}
