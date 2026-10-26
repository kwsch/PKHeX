using PKHeX.Core;
using System;
using System.Windows.Forms;

namespace PKHeX.WinForms;

public sealed partial class SAV_Safari4 : Form
{
    private readonly SAV4HGSS Origin;
    private readonly SAV4HGSS SAV;
    private readonly SafariZone4 Safari;
    private readonly SafariZoneAreaSet4 Self;
    private readonly SafariZoneAreaSet4 Link;

    private readonly SafariArea4Editor[] SelfAreaEditors = new SafariArea4Editor[SafariZoneAreaSet4.MapCountConfiguration];
    private readonly SafariArea4Editor[] LinkAreaEditors = new SafariArea4Editor[SafariZoneAreaSet4.MapCountConfiguration];

    private const int SafariZoneTypeCount = (int)SafariZoneType4.Desert + 1;

    public bool IsLoading;

    public SAV_Safari4(SAV4HGSS sav)
    {
        InitializeComponent();

#if DEBUG // Translation Rip
        // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
        // ReSharper disable once ArrangeObjectCreationWhenTypeNotEvident
        sav ??= new();
#endif

        Origin = sav;
        SAV = (SAV4HGSS)sav.Clone();
        Safari = SAV.Safari;
        Self = Safari.Self;
        Link = Safari.Link;

        InitializeAreaLevelTabs();
        InitializeAreaEditors();

        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);

        PG_LinkLeader.SelectedObject = Safari.LinkLeader;

        LoadData();
    }

    private void B_Save_Click(object sender, EventArgs e)
    {
        SaveCurrentViews();
        Origin.CopyChangesFrom(SAV);
        Close();
    }

    private void B_Cancel_Click(object sender, EventArgs e) => Close();

    private void InitializeAreaEditors()
    {
        var selfTabs = new[] { Tab_SelfArea1, Tab_SelfArea2, Tab_SelfArea3, Tab_SelfArea4, Tab_SelfArea5, Tab_SelfArea6 };
        var linkTabs = new[] { Tab_LinkArea1, Tab_LinkArea2, Tab_LinkArea3, Tab_LinkArea4, Tab_LinkArea5, Tab_LinkArea6 };

        for (int i = 0; i < selfTabs.Length; i++)
        {
            var editor = new SafariArea4Editor(Self.Configurations[i]) { Dock = DockStyle.Fill };
            SelfAreaEditors[i] = editor;
            selfTabs[i].Controls.Add(editor);
        }

        for (int i = 0; i < linkTabs.Length; i++)
        {
            var editor = new SafariArea4Editor(Link.Configurations[i]) { Dock = DockStyle.Fill };
            LinkAreaEditors[i] = editor;
            linkTabs[i].Controls.Add(editor);
        }
    }

    private void InitializeAreaLevelTabs()
    {
        DGV_SelfAges.CellValidating += AgesesGridCellValidating;
        DGV_LinkAges.CellValidating += AgesesGridCellValidating;

        PopulateAgeGrid(DGV_SelfAges, Self);
        PopulateAgeGrid(DGV_LinkAges, Link);
    }

    private void LoadData()
    {
        IsLoading = true;

        foreach (var editor in SelfAreaEditors)
            editor.LoadData();
        foreach (var editor in LinkAreaEditors)
            editor.LoadData();

        NUD_DayDelay.Value = Safari.DayDelay;
        NUD_ElapsedMinutes.Value = Safari.ElapsedPlayMinutes;
        NUD_ObjectUnlockLevel.Value = Safari.ObjectUnlockLevel;
        NUD_AreaSwitcherState.Value = Safari.AreaSwitcherState;

        LoadAgeGrid(DGV_SelfAges, Self);
        LoadAgeGrid(DGV_LinkAges, Link);

        IsLoading = false;
    }

    private void SaveCurrentViews()
    {
        for (var i = 0; i < SelfAreaEditors.Length; i++)
        {
            var editor = SelfAreaEditors[i];
            editor.SaveData();
            Self.FlushConfiguration(i, editor.IsAutoCount);
        }

        for (var i = 0; i < LinkAreaEditors.Length; i++)
        {
            var editor = LinkAreaEditors[i];
            editor.SaveData();
            Link.FlushConfiguration(i, editor.IsAutoCount);
        }

        Self.FlushMapAges();
        Link.FlushMapAges();
        SaveGeneral();
    }

    private void SaveGeneral()
    {
        Safari.DayDelay = (byte)NUD_DayDelay.Value;
        Safari.ElapsedPlayMinutes = (ushort)NUD_ElapsedMinutes.Value;
        Safari.ObjectUnlockLevel = (byte)NUD_ObjectUnlockLevel.Value;
        Safari.AreaSwitcherState = (byte)NUD_AreaSwitcherState.Value;

        for (int i = 0; i < SafariZoneTypeCount; i++)
            Self.SetMapAge((SafariZoneType4)i, ParseAgeCell(DGV_SelfAges.Rows[i].Cells[1].Value));

        for (int i = 0; i < SafariZoneTypeCount; i++)
            Link.SetMapAge((SafariZoneType4)i, ParseAgeCell(DGV_LinkAges.Rows[i].Cells[1].Value));
    }

    private static void PopulateAgeGrid(DataGridView grid, SafariZoneAreaSet4 set)
    {
        var maps = WinFormsTranslator.GetEnumTranslation<SafariZoneType4>(Main.CurrentLanguage);
        grid.RowCount = maps.Length;
        for (var i = 0; i < maps.Length; i++)
        {
            var row = grid.Rows[i];
            row.Cells[0].Value = maps[i];
            row.Cells[1].Value = set.GetMapAge((SafariZoneType4)i).ToString();
        }
    }

    private static void LoadAgeGrid(DataGridView grid, SafariZoneAreaSet4 set) => PopulateAgeGrid(grid, set);

    private static byte ParseAgeCell(object? value)
    {
        if (value is null)
            return 0;

        if (value is string s && byte.TryParse(s, out var result))
            return result;

        if (value is int i)
            return (byte)Math.Clamp(i, 0, byte.MaxValue);

        if (value is long l)
            return (byte)Math.Clamp(l, 0, byte.MaxValue);

        return 0;
    }

    private void AgesesGridCellValidating(object? sender, DataGridViewCellValidatingEventArgs e)
    {
        if (IsLoading || e.ColumnIndex != 1)
            return;

        var text = e.FormattedValue?.ToString();
        if (string.IsNullOrWhiteSpace(text) || byte.TryParse(text, out _))
            return;

        e.Cancel = true;
    }

    private void B_MaxAllSelf_Click(object sender, EventArgs e) => SetAllAge(DGV_SelfAges, ModifierKeys == Keys.Alt ? byte.MinValue : byte.MaxValue);
    private void B_MaxAllLink_Click(object sender, EventArgs e) => SetAllAge(DGV_LinkAges, ModifierKeys == Keys.Alt ? byte.MinValue : byte.MaxValue);

    private static void SetAllAge(DataGridView dataGridView, byte value = byte.MaxValue)
    {
        foreach (DataGridViewRow row in dataGridView.Rows)
            row.Cells[1].Value = value.ToString();
        WinFormsUtil.Asterisk();
    }
}
