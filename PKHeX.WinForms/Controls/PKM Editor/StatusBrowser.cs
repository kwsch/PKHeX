using System;
using System.Windows.Forms;

using PKHeX.Core;
using PKHeX.Drawing.PokeSprite;
using PKHeX.WinForms.Controls;

namespace PKHeX.WinForms;

public partial class StatusBrowser : Form
{
    public bool WasChosen { get; private set; }
    public StatusCondition Choice { get; private set; }

    private const int StatusCount = 7;
    private int StatusHeight { get; }
    private int StatusWidth => StatusHeight;
    private int StatusBrowserWidth => StatusWidth * 2;

    public StatusBrowser(int generation)
    {
        InitializeComponent();
        StatusHeight = Drawing.PokeSprite.Properties.Resources.sicksleep.Height;
        NUD_Sleep = new NumericUpDown
        {
            Minimum = 1,
            Maximum = 7,
            Value = 1,
            Width = 40,
            TextAlign = HorizontalAlignment.Center,
            Margin = Padding.Empty,
            Padding = Padding.Empty,
        };

        Add(GetImage(StatusCondition.None, "None"));
        Add(GetImage(StatusCondition.Sleep1, "Sleep"), generation >= 5);
        if (generation <= 4)
            Add(NUD_Sleep);
        Add(GetImage(StatusCondition.Poison, "Poison"));
        Add(GetImage(StatusCondition.Burn, "Burn"));
        Add(GetImage(StatusCondition.Paralysis, "Paralysis"));
        Add(GetImage(StatusCondition.Freeze, "Freeze"));
        if (generation is 3 or 4)
            Add(GetImage(StatusCondition.PoisonBad, "Toxic"));

        Height = StatusCount * StatusHeight;
        Width = StatusBrowserWidth;
    }

    private readonly NumericUpDown NUD_Sleep;

    private void Add(Control c, bool flowBreak = true)
    {
        flp.Controls.Add(c);
        if (flowBreak)
            flp.SetFlowBreak(c, true);
    }

    public void LoadList(PKM pk)
    {
        StatusType type;
        if (pk.Format <= 4)
        {
            var condition = (StatusCondition)pk.Status_Condition;
            NUD_Sleep.Value = Math.Max(1, (int)condition & 7);
            type = condition.GetStatusType();
        }
        else
        {
            type = (StatusType)(pk.Status_Condition & 7);
        }

        Text = WinFormsTranslator.TranslateEnum(type, Main.CurrentLanguage);
    }

    private SelectablePictureBox GetImage(StatusCondition value, string name)
    {
        var img = value == 0
            ? Drawing.PokeSprite.Properties.Resources.sickfaint
            : value.GetStatusSprite();
        var pb = new SelectablePictureBox
        {
            Image = img,
            Name = name,
            AccessibleDescription = name,
            AccessibleName = name,
            AccessibleRole = AccessibleRole.Graphic,
            Margin = Padding.Empty,
            Padding = Padding.Empty,
            Width = StatusWidth,
            Height = StatusHeight,
        };

        pb.MouseEnter += (_, _) => Text = value is StatusCondition.PoisonBad ? "Toxic" : WinFormsTranslator.TranslateEnum(value.GetStatusType(), Main.CurrentLanguage);
        pb.Click += (_, _) =>
        {
            if (value is StatusCondition.Sleep1)
                value = (StatusCondition)NUD_Sleep.Value;
            SelectValue(value);
        };
        pb.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter)
                SelectValue(value);
        };
        return pb;
    }

    private void SelectValue(StatusCondition value)
    {
        Choice = value;
        WasChosen = true;
        Close();
    }
}
