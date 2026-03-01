using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public partial class DonutFlavorProfile9a : UserControl
{
    private const int MaxStatValue = 760;
    private const int PentagonWidth = 110;
    private const int PentagonHeight = 110;

    private int ScaleValue;

    private readonly int[] FlavorProfileStats = new int[5];

    public DonutFlavorProfile9a() => InitializeComponent();

    /// <summary>
    /// Load stats from a Donut9a object
    /// </summary>
    public void LoadFromDonut(Donut9a donut)
    {
        Span<int> flavorStats = stackalloc int[5];
        donut.RecalculateDonutFlavors(flavorStats);

        flavorStats.CopyTo(FlavorProfileStats);
        FlavorProfileStats[0] = flavorStats[0]; // Spicy - top
        FlavorProfileStats[1] = flavorStats[4]; // Sour - top-right
        FlavorProfileStats[2] = flavorStats[1]; // Fresh - bottom-right
        FlavorProfileStats[3] = flavorStats[3]; // Bitter - bottom-left
        FlavorProfileStats[4] = flavorStats[2]; // Sweet - top-left

        int maxStat = 0;
        foreach (var stat in FlavorProfileStats)
            if (stat > maxStat) maxStat = stat;

        ScaleValue = maxStat < 100 ? 500 : Math.Min((((maxStat + 99) / 100) * 100) + 100, MaxStatValue);

        UpdateStatLabels(flavorStats);
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        // Calculate pentagon area - centered in the control
        var center = Size / 2;

        // Scale pentagon size relative to control size
        float scaleX = Width / 276f;
        float scaleY = Height / 240f;
        float radiusX = (PentagonWidth / 2f) * scaleX;
        float radiusY = (PentagonHeight / 2f) * scaleY;

        // Draw filled pentagon based on stats
        DrawStatsPentagon(g, center, radiusX, radiusY);
    }

    private void DrawStatsPentagon(Graphics g, Size center, float radiusX, float radiusY)
    {
        // Calculate scaled points based on stat values with individual max per stat
        Span<PointF> calculatedPoints = stackalloc PointF[5];
        for (int i = 0; i < 5; i++)
        {
            // Calculate individual max for this stat
            var point = GetStatCoordinate(radiusX, radiusY, FlavorProfileStats[i], i);
            point += center;
            calculatedPoints[i] = point;
        }

        // Reorder points for counter-clockwise drawing
        Span<PointF> statPoints = stackalloc PointF[5];
        ReorderCounterClockwise(statPoints, calculatedPoints);

        // Fill the pentagon with semi-transparent yellow
        using Brush fillBrush = new SolidBrush(Color.Yellow);
        g.FillPolygon(fillBrush, statPoints);
    }

    private static PointF GetStatCoordinate(float radiusX, float radiusY, int statValue, int i)
    {
        int statMax = statValue switch
        {
            <= 350 => statValue + 200,
            <= 700 => ((statValue + 99) / 100) * 100,
            _ => MaxStatValue,
        };

        float scale = statMax > 0 ? Math.Min((float)statValue / statMax, 1.0f) : 0f;

        // Use baseline scale (10%) if stat is 0, otherwise use calculated scale
        const float baselineScale = 0.10f;
        if (scale == 0f)
            scale = baselineScale;

        const double angleStep = 2 * Math.PI / 5;
        double angle = (-Math.PI / 2) + (i * angleStep);

        float scaledRadiusX = radiusX * scale;
        float scaledRadiusY = radiusY * scale;
        return new PointF((float)(scaledRadiusX * Math.Cos(angle)), (float)(scaledRadiusY * Math.Sin(angle)));
    }

    private static void ReorderCounterClockwise(Span<PointF> statPoints, ReadOnlySpan<PointF> calculatedPoints)
    {
        ReadOnlySpan<int> counterClockwiseOrder = [0, 4, 3, 2, 1];
        for (int i = 0; i < 5; i++)
            statPoints[i] = calculatedPoints[counterClockwiseOrder[i]];
    }

    private void UpdateStatLabels(ReadOnlySpan<int> flavorStats)
    {
        L_StatSpicy.Text  = flavorStats[0].ToString();
        L_StatFresh.Text  = flavorStats[1].ToString();
        L_StatSweet.Text  = flavorStats[2].ToString();
        L_StatBitter.Text = flavorStats[3].ToString();
        L_StatSour.Text   = flavorStats[4].ToString();
    }
}
