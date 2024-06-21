using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Drawing.Misc;
using PKHeX.WinForms.Controls;

namespace PKHeX.WinForms;

public partial class RibbonEditor : Form
{
    private readonly PKM Entity;
    private readonly IReadOnlyList<RibbonInfo> riblist;

    private const string PrefixNUD = "NUD_";
    private const string PrefixLabel = "L_";
    private const string PrefixCHK = "CHK_";
    private const string PrefixPB = "PB_";

    private const int AffixedNone = -1;

    private bool EnableBackgroundChange;
    private Control? LastToggledOn;

    public RibbonEditor(PKM pk)
    {
        Entity = pk;
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        riblist = RibbonInfo.GetRibbonInfo(pk);
        int vertScrollWidth = SystemInformation.VerticalScrollBarWidth;
        TLP_Ribbons.Padding = FLP_Ribbons.Padding = new Padding(0, 0, vertScrollWidth, 0);

        // Updating a Control display with auto-sized elements on every row addition is cpu intensive. Disable layout updates while populating.
        TLP_Ribbons.SuspendLayout();
        FLP_Ribbons.Scroll += WinFormsUtil.PanelScroll;
        TLP_Ribbons.Scroll += WinFormsUtil.PanelScroll;
        PopulateRibbons();
        TLP_Ribbons.ResumeLayout();

        InitializeAffixed(pk);
        EnableBackgroundChange = true;
    }

    private void InitializeAffixed(PKM pk)
    {
        if (pk is not IRibbonSetAffixed affixed)
        {
            CB_Affixed.Visible = false;
            return;
        }

        const int count = (int)RibbonIndex.MAX_COUNT;
        static string GetRibbonPropertyName(int z) => RibbonStrings.GetName($"Ribbon{(RibbonIndex)z}");
        static ComboItem GetComboItem(int ribbonIndex) => new(GetRibbonPropertyName(ribbonIndex), ribbonIndex);

        var none = GameInfo.GetStrings(Main.CurrentLanguage).Move[0];
        var ds = new List<ComboItem>(1 + count) { new(none, AffixedNone) };
        var list = Enumerable.Range(0, count).Select(GetComboItem).OrderBy(z => z.Text);
        ds.AddRange(list);

        CB_Affixed.InitializeBinding();
        CB_Affixed.DataSource = ds;
        CB_Affixed.SelectedValue = (int)affixed.AffixedRibbon;
    }

    private void B_Cancel_Click(object sender, EventArgs e) => Close();

    private void B_Save_Click(object sender, EventArgs e)
    {
        Save();
        Close();
    }

    private void PopulateRibbons()
    {
        TLP_Ribbons.ColumnCount = 2;
        TLP_Ribbons.RowCount = 0;

        // Add Ribbons
        foreach (var rib in riblist)
            AddRibbonSprite(rib);

        var pk = Entity;
        var la = new LegalityAnalysis(pk);
        Span<RibbonResult> ribbons = stackalloc RibbonResult[riblist.Count];
        var args = new RibbonVerifierArguments(pk, la.EncounterOriginal, la.Info.EvoChainsAllGens);
        var count = RibbonVerifier.GetRibbonResults(args, ribbons);
        var slice = ribbons[..count];

        var dict = new Dictionary<string, RibbonResult>(slice.Length);
        foreach (var r in slice)
            dict.Add(r.PropertyName, r);

        var clone = pk.Clone();
        RibbonApplicator.SetAllValidRibbons(clone);
        var otherList = RibbonInfo.GetRibbonInfo(clone);
        var preferred = riblist
            .OrderBy(z => GetSortOrder(z.Name, dict, otherList))
            .ThenBy(z => RibbonStrings.GetName(z.Name));

        foreach (var rib in preferred)
        {
            var name = rib.Name;
            Color color = dict.TryGetValue(name, out var r)
                ? r.IsMissing ? Color.LightYellow : Color.Pink
                : GetColor(otherList, name);
            AddRibbonChoice(rib, color);
        }

        // Force auto-size
        foreach (var style in TLP_Ribbons.RowStyles.OfType<RowStyle>())
            style.SizeType = SizeType.AutoSize;
        foreach (var style in TLP_Ribbons.ColumnStyles.OfType<ColumnStyle>())
            style.SizeType = SizeType.AutoSize;
    }

    private static int GetSortOrder(string name, Dictionary<string, RibbonResult> dict, List<RibbonInfo> otherList)
    {
        if (name.StartsWith("RibbonMark"))
            return 99;
        var other = otherList.Find(z => z.Name == name);
        if (other is { HasRibbon: true })
            return 0;
        if (dict.TryGetValue(name, out var r))
            return r.IsMissing ? 1 : 2;
        return 3; // last
    }

    private static Color GetColor(List<RibbonInfo> otherList, string ribName)
    {
        if (ribName.StartsWith("RibbonMark"))
            return Color.SeaShell;
        var other = otherList.Find(z => z.Name == ribName);
        if (other is null)
            return Color.Transparent;
        return other.HasRibbon ? Color.PaleGreen : Color.Transparent;
    }

    private void AddRibbonSprite(RibbonInfo rib)
    {
        var name = rib.Name;
        var pb = new SelectablePictureBox
        {
            AutoSize = false,
            Size = new Size(40, 40),
            BackgroundImageLayout = ImageLayout.Center,
            Visible = false,
            Name = PrefixPB + name,
            AccessibleName = name,
            AccessibleDescription = name,
            AccessibleRole = AccessibleRole.Graphic,
        };
        var img = RibbonSpriteUtil.GetRibbonSprite(name);
        pb.BackgroundImage = img;

        var display = RibbonStrings.GetName(name);
        pb.MouseEnter += (_, _) => tipName.SetToolTip(pb, display);
        if (Entity is IRibbonSetAffixed)
            pb.Click += (_, _) => CB_Affixed.Text = RibbonStrings.GetName(name);
        FLP_Ribbons.Controls.Add(pb);
    }

    private void AddRibbonChoice(RibbonInfo rib, Color color)
    {
        // Get row we add to
        int row = TLP_Ribbons.RowCount;
        TLP_Ribbons.RowCount++;

        var label = new Label
        {
            Anchor = AnchorStyles.Left,
            Name = PrefixLabel + rib.Name,
            Text = RibbonStrings.GetName(rib.Name),
            Padding = Padding.Empty,
            Margin = Padding.Empty,
            BackColor = color,
            AutoSize = true,
        };
        TLP_Ribbons.Controls.Add(label, 1, row);

        if (rib.Type is RibbonValueType.Byte) // numeric count ribbon
            AddRibbonNumericUpDown(rib, row, label);
        else // boolean ribbon
            AddRibbonCheckBox(rib, row, label);
    }

    private void AddRibbonNumericUpDown(RibbonInfo rib, int row, Control label)
    {
        var nud = new NumericUpDown
        {
            Anchor = AnchorStyles.Right,
            Name = PrefixNUD + rib.Name,
            Minimum = 0,
            Width = 35,
            Increment = 1,
            Padding = Padding.Empty,
            Margin = Padding.Empty,
            Maximum = rib.MaxCount,
        };

        nud.ValueChanged += (_, _) =>
        {
            var controlName = PrefixPB + rib.Name;
            var pb = FLP_Ribbons.Controls[controlName] ?? throw new ArgumentException($"{controlName} not found in {FLP_Ribbons.Name}.");
            pb.Visible = (rib.RibbonCount = (byte)nud.Value) != 0;
            pb.BackgroundImage = RibbonSpriteUtil.GetRibbonSprite(rib.Name, (int)nud.Maximum, (int)nud.Value);

            ToggleNewRibbon(rib, pb);
        };

        // Setting value will trigger above event
        nud.Value = Math.Min(rib.MaxCount, rib.RibbonCount);
        TLP_Ribbons.Controls.Add(nud, 0, row);

        label.Click += (_, _) => nud.Value = (nud.Value == 0) ? nud.Maximum : 0;
    }

    private void AddRibbonCheckBox(RibbonInfo rib, int row, Control label)
    {
        var chk = new CheckBox
        {
            Anchor = AnchorStyles.Right,
            Name = PrefixCHK + rib.Name,
            AutoSize = true,
            Padding = Padding.Empty,
            Margin = Padding.Empty,
        };
        chk.CheckedChanged += (_, _) =>
        {
            rib.HasRibbon = chk.Checked;
            var controlName = PrefixPB + rib.Name;
            var control = FLP_Ribbons.Controls[controlName];
            ArgumentNullException.ThrowIfNull(control);
            control.Visible = rib.HasRibbon;

            ToggleNewRibbon(rib, control);
        };

        // Setting value will trigger above event
        chk.Checked = rib.HasRibbon;
        TLP_Ribbons.Controls.Add(chk, 0, row);

        label.Click += (_, _) => chk.Checked ^= true;
    }

    private void ToggleNewRibbon(RibbonInfo rib, Control pb)
    {
        if (!EnableBackgroundChange)
            return;
        if (LastToggledOn is not null)
            LastToggledOn.BackColor = Color.Transparent;
        pb.BackColor = rib.HasRibbon ? Color.LightBlue : Color.Transparent;
        LastToggledOn = pb;
    }

    private void Save()
    {
        foreach (var rib in riblist)
            ReflectUtil.SetValue(Entity, rib.Name, rib.Type is RibbonValueType.Boolean ? rib.HasRibbon : rib.RibbonCount);

        if (Entity is IRibbonSetAffixed affixed)
            affixed.AffixedRibbon = (sbyte)WinFormsUtil.GetIndex(CB_Affixed);
    }

    private void B_All_Click(object sender, EventArgs e)
    {
        if (ModifierKeys == Keys.Shift)
        {
            RibbonApplicator.RemoveAllValidRibbons(Entity);
            RibbonApplicator.SetAllValidRibbons(Entity);
            Close();
            return;
        }

        EnableBackgroundChange = false;
        foreach (var c in TLP_Ribbons.Controls)
        {
            if (c is CheckBox chk)
                chk.Checked = true;
            else if (c is NumericUpDown nud)
                nud.Value = nud.Maximum;
        }
        EnableBackgroundChange = true;
    }

    private void B_None_Click(object sender, EventArgs e)
    {
        if (ModifierKeys == Keys.Shift)
        {
            RibbonApplicator.RemoveAllValidRibbons(Entity);
            if (Entity is IRibbonSetAffixed affixed)
                affixed.AffixedRibbon = AffixedNone;
            Close();
            return;
        }

        CB_Affixed.SelectedValue = AffixedNone;
        foreach (var c in TLP_Ribbons.Controls)
        {
            if (c is CheckBox chk)
                chk.Checked = false;
            else if (c is NumericUpDown nud)
                nud.Value = 0;
        }
    }
}
