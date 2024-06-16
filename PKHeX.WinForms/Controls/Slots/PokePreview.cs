using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Text;
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

    public void Populate(PKM pk)
    {
        var la = new LegalityAnalysis(pk);
        int width = PopulateHeader(pk);
        PopulateMoves(pk, la, ref width);
        PopulateText(pk, la, width);
    }

    private int PopulateHeader(PKM pk)
    {
        var name = GetNameTitle(pk);
        var size = MeasureSize(name, L_Name.Font);
        L_Name.Width = Math.Max(InitialNameWidth, size.Width);
        L_Name.Text = name;

        PopulateBall(pk);
        PopulateGender(pk);

        var width = L_Name.Width + PB_Ball.Width + PB_Ball.Margin.Horizontal + PB_Gender.Width + PB_Gender.Margin.Horizontal + interiorMargin;
        return Math.Max(InitialWidth, width);
    }

    private static string GetNameTitle(PKM pk)
    {
        var nick = pk.Nickname;
        var all = GameInfo.Strings.Species;
        var species = pk.Species;
        if (species >= all.Count)
            return nick;
        var expect = all[species];
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

    private void PopulateMoves(PKM pk, LegalityAnalysis la, ref int width)
    {
        var context = pk.Context;
        var names = GameInfo.Strings.movelist;
        var check = la.Info.Moves;
        var w1 = Move1.Populate(pk, pk.Move1, context, names, check[0].Valid);
        var w2 = Move2.Populate(pk, pk.Move2, context, names, check[1].Valid);
        var w3 = Move3.Populate(pk, pk.Move3, context, names, check[2].Valid);
        var w4 = Move4.Populate(pk, pk.Move4, context, names, check[3].Valid);

        var maxWidth = Math.Max(w1, Math.Max(w2, Math.Max(w3, w4)));
        width = Math.Max(width, maxWidth + Move1.Margin.Horizontal + interiorMargin);
    }

    private void PopulateText(PKM pk, LegalityAnalysis la, int width)
    {
        var (stats, enc) = GetStatsString(pk, la);
        var settings = Main.Settings.Hover;

        bool hasMoves = pk.MoveCount != 0;
        FLP_Moves.Visible = hasMoves;
        var height = FLP_List.Top + interiorMargin;
        if (hasMoves)
            height += FLP_Moves.Height + FLP_Moves.Margin.Vertical;
        ToggleLabel(L_Stats, stats, settings.PreviewShowPaste, ref width, ref height);
        ToggleLabel(L_Etc, enc, settings.HoverSlotShowEncounter, ref width, ref height);
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

    private static (string Detail, string Encounter) GetStatsString(PKM pk, LegalityAnalysis la)
    {
        var setText = SummaryPreviewer.GetPreviewText(pk, la);
        var sb = new StringBuilder();
        var lines = setText.AsSpan().EnumerateLines();
        if (!lines.MoveNext())
            throw new ArgumentException("Invalid text format", nameof(pk));

        var first = lines.Current;
        var itemIndex = first.IndexOf('@');
        if (itemIndex != -1) // Held Item
        {
            var remaining = first[(itemIndex + 2)..];
            if (remaining[^1] == ')')
                remaining = remaining[..^3]; // lop off gender
            var item = remaining.Trim();
            if (item.Length != 0)
                sb.AppendLine($"Held Item: {item}");
        }

        if (pk is IGanbaru g)
            AddGanbaru(g, sb);
        if (pk is IAwakened a)
            AddAwakening(a, sb);

        while (lines.MoveNext())
        {
            var line = lines.Current;
            if (IsMoveLine(line))
            {
                while (lines.MoveNext())
                {
                    if (!IsMoveLine(lines.Current))
                        break;
                }
                break;
            }
            sb.AppendLine(line.ToString());
        }

        var detail = sb.ToString();
        sb.Clear();
        while (lines.MoveNext())
        {
            var line = lines.Current;
            sb.AppendLine(line.ToString());
        }
        var enc = sb.ToString();
        return (detail.TrimEnd(), enc.TrimEnd());

        static bool IsMoveLine(ReadOnlySpan<char> line) => line.Length != 0 && line[0] == '-';
    }

    private static void AddGanbaru(IGanbaru g, StringBuilder sb)
    {
        Span<byte> gvs = stackalloc byte[6];
        g.GetGVs(gvs);
        TryAdd<byte>(sb, "GVs", gvs);
    }

    private static void AddAwakening(IAwakened a, StringBuilder sb)
    {
        Span<byte> avs = stackalloc byte[6];
        a.GetAVs(avs);
        TryAdd<byte>(sb, "AVs", avs);
    }

    private static void TryAdd<T>(StringBuilder sb, [ConstantExpected] string type, ReadOnlySpan<T> stats, T ignore = default) where T : unmanaged, IEquatable<T>
    {
        var chunks = ShowdownSet.GetStringStats(stats, ignore);
        if (chunks.Length != 0)
            sb.AppendLine($"{type}: {string.Join(" / ", chunks)}");
    }

    /// <summary> Prevent stealing focus from the form that shows this. </summary>
    protected override bool ShowWithoutActivation => true;

    private const int WS_EX_TOPMOST = 0x00000008;
    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams createParams = base.CreateParams;
            createParams.ExStyle |= WS_EX_TOPMOST;
            return createParams;
        }
    }
}
