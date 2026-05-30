using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public sealed partial class SAV_Medals5 : Form
{
    private const string MedalListFilter = "Medal List 5|*.ml5";
    private const string DateFormat = "yyyy-MM-dd";

    private readonly SAV5B2W2 Origin;
    private readonly SAV5B2W2 SAV;
    private readonly MedalList5 Medals;
    private readonly HabitatList5 Habitat;

    private readonly string[] MedalNames = Util.GetStringList("medals", Main.CurrentLanguage);
    private readonly string[] MedalTypeNames = Util.GetStringList("medal_types", Main.CurrentLanguage);
    private readonly string[] MedalStateNames = WinFormsTranslator.GetEnumTranslation<MedalState5>(Main.CurrentLanguage);
    private readonly string[] MedalRankNames = WinFormsTranslator.GetEnumTranslation<MedalRank5>(Main.CurrentLanguage);
    private readonly string[] HabitatCompletionNames = WinFormsTranslator.GetEnumTranslation<HabitatCompletion5>(Main.CurrentLanguage);
    private readonly string[] HabitatEncounterTypeNames = WinFormsTranslator.GetEnumTranslation<HabitatEncounterType5>(Main.CurrentLanguage);

    private static readonly DateOnly MinimumDate = new(2000, 1, 1);
    private static readonly DateOnly MaximumDate = new(2099, 12, 31);

    public SAV_Medals5(SAV5B2W2 sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);

        Origin = sav;
        SAV = (SAV5B2W2)sav.Clone();
        Medals = SAV.Medals;
        Habitat = Medals.HabitatList;

        InitializeMedalGrid();
        InitializeHabitatGrid();
        LoadData();
    }

    private void B_Save_Click(object sender, EventArgs e)
    {
        CommitPendingGridEdits();
        if (!ValidateChildren())
            return;

        Medals.PinnedMedal = (byte)WinFormsUtil.GetIndex(CB_PinnedMedal);
        Medals.Rank = (MedalRank5)WinFormsUtil.GetIndex(CB_Rank);
        Medals.IsTutorialComplete = CHK_TutorialComplete.Checked;

        SaveHabitatSettings();
        Origin.CopyChangesFrom(SAV);
        Close();
    }

    private void B_Cancel_Click(object sender, EventArgs e) => Close();

    private void B_GiveAll_Click(object sender, EventArgs e)
    {
        var now = EncounterDate.GetDateNDS();
        Medals.GiveAll(now, unread: true);
        LoadMedalData();
        WinFormsUtil.Asterisk();
    }

    private void B_ImportAll_Click(object sender, EventArgs e)
    {
        using var ofd = new OpenFileDialog();
        ofd.Filter = MedalListFilter;
        ofd.FileName = GetDefaultFileName();
        if (ofd.ShowDialog() != DialogResult.OK)
            return;

        var fi = new FileInfo(ofd.FileName);
        if (fi.Length != MedalList5.LengthAllMedals)
        {
            WinFormsUtil.Alert(string.Format(MessageStrings.MsgFileSizeIncorrect, fi.Length, MedalList5.LengthAllMedals));
            return;
        }

        var data = File.ReadAllBytes(ofd.FileName);
        data.AsSpan().CopyTo(Medals.AllMedals);
        LoadMedalData();
        WinFormsUtil.Asterisk();
    }

    private void B_ExportAll_Click(object sender, EventArgs e)
    {
        using var sfd = new SaveFileDialog();
        sfd.Filter = MedalListFilter;
        sfd.FileName = GetDefaultFileName();
        if (sfd.ShowDialog() != DialogResult.OK)
            return;

        File.WriteAllBytes(sfd.FileName, Medals.AllMedals.ToArray());
    }

    private void B_HabitatSetComplete_Click(object sender, EventArgs e)
    {
        if (DGV_Habitat.SelectedRows.Count == 0)
        {
            Habitat.CompleteAll();
        }
        else
        {
            foreach (var row in GetSelectedHabitatRows())
                Habitat.GetHabitat(row.Index).SetComplete();
        }

        LoadHabitatData();
        WinFormsUtil.Asterisk();
    }

    private void B_HabitatClear_Click(object sender, EventArgs e)
    {
        foreach (var row in GetSelectedHabitatRows())
            Habitat.GetHabitat(row.Index).Clear();

        LoadHabitatData();
        WinFormsUtil.Asterisk();
    }

    private void L_Rank_Click(object? sender, EventArgs e)
    {
        CommitPendingGridEdits();
        CB_Rank.SelectedValue = (int)Medals.CalculateRank();
        WinFormsUtil.Asterisk();
    }

    private void DGV_Medals_CellBeginEdit(object? sender, DataGridViewCellCancelEventArgs e)
    {
        if (e.RowIndex < 0 || e.ColumnIndex != MedalDateColumn.Index)
            return;

        if (!CanEditMedalDate(e.RowIndex))
            e.Cancel = true;
    }

    private void DGV_Medals_CellParsing(object? sender, DataGridViewCellParsingEventArgs e)
    {
        if (e.RowIndex < 0 || e.ColumnIndex != MedalDateColumn.Index)
            return;

        if (e.Value is not string text)
            return;

        if (!TryParseDate(text, out var date))
            return;

        e.Value = date.ToString(DateFormat, CultureInfo.InvariantCulture);
        e.ParsingApplied = true;
    }

    private void DGV_Medals_CellValidating(object? sender, DataGridViewCellValidatingEventArgs e)
    {
        if (e.RowIndex < 0)
            return;

        if (e.ColumnIndex == MedalDateColumn.Index)
        {
            if (!CanEditMedalDate(e.RowIndex))
                return;

            if (e.FormattedValue is not string text || !TryParseDate(text, out _))
            {
                DGV_Medals.Rows[e.RowIndex].ErrorText = $"Date must be between {MinimumDate:yyyy-MM-dd} and {MaximumDate:yyyy-MM-dd}.";
                e.Cancel = true;
            }
            else
            {
                DGV_Medals.Rows[e.RowIndex].ErrorText = string.Empty;
            }

            return;
        }

        DGV_Medals.Rows[e.RowIndex].ErrorText = string.Empty;
    }

    private void DGV_Medals_CurrentCellDirtyStateChanged(object? sender, EventArgs e)
    {
        if (DGV_Medals.IsCurrentCellDirty)
            DGV_Medals.CommitEdit(DataGridViewDataErrorContexts.Commit);
    }

    private void DGV_Medals_DataError(object? sender, DataGridViewDataErrorEventArgs e)
    {
        e.Cancel = false;
        e.ThrowException = false;
    }

    private void DGV_Medals_EditingControlShowing(object? sender, DataGridViewEditingControlShowingEventArgs e)
    {
        if (e.Control is ComboBox combo && DGV_Medals.CurrentCell?.OwningColumn is DataGridViewComboBoxColumn)
        {
            DGV_Medals.BeginInvoke((MethodInvoker)(() => combo.DroppedDown = true));
            return;
        }

        if (DGV_Medals.CurrentCell?.ColumnIndex != MedalDateColumn.Index)
            return;

        if (e.Control is TextBox tb)
            tb.SelectAll();
    }

    private void DGV_Habitat_CurrentCellDirtyStateChanged(object? sender, EventArgs e)
    {
        if (DGV_Habitat.IsCurrentCellDirty)
            DGV_Habitat.CommitEdit(DataGridViewDataErrorContexts.Commit);
    }

    private void DGV_Habitat_DataError(object? sender, DataGridViewDataErrorEventArgs e)
    {
        e.Cancel = false;
        e.ThrowException = false;
    }

    private void DGV_Habitat_EditingControlShowing(object? sender, DataGridViewEditingControlShowingEventArgs e)
    {
        if (e.Control is ComboBox combo && DGV_Habitat.CurrentCell?.OwningColumn is DataGridViewComboBoxColumn)
            DGV_Habitat.BeginInvoke((MethodInvoker)(() => combo.DroppedDown = true));
    }

    private void InitializeMedalGrid()
    {
        MedalStateColumn.Items.AddRange(MedalStateNames);

        var medalItems = MedalNames.Select((z, i) => new ComboItem(z, i)).ToList();
        medalItems.Insert(0, new ComboItem(GameInfo.Strings.specieslist[0], MedalList5.PinnedMedalNone));
        CB_PinnedMedal.InitializeBinding();
        CB_PinnedMedal.DataSource = new BindingSource(medalItems, string.Empty);

        var rankValues = Enum.GetValues<MedalRank5>();
        var rankItems = new ComboItem[rankValues.Length];
        for (int i = 0; i < rankItems.Length; i++)
            rankItems[i] = new ComboItem(MedalRankNames[i], (int)rankValues[i]);
        CB_Rank.InitializeBinding();
        CB_Rank.DataSource = new BindingSource(rankItems, string.Empty);
    }

    private void InitializeHabitatGrid()
    {
        HabitatGrassColumn.Items.AddRange(HabitatCompletionNames);
        HabitatSurfColumn.Items.AddRange(HabitatCompletionNames);
        HabitatFishColumn.Items.AddRange(HabitatCompletionNames);

        CB_LastEncounterType.Items.AddRange(HabitatEncounterTypeNames);
    }

    private void LoadData()
    {
        LoadMedalData();
        LoadHabitatData();
        LoadHabitatSettings();
    }

    private void LoadMedalData()
    {
        DGV_Medals.Rows.Clear();
        DGV_Medals.Rows.Add(MedalNames.Length);
        for (int i = 0; i < MedalNames.Length; i++)
            LoadMedalRow(i);

        CB_PinnedMedal.SelectedValue = (int)Medals.PinnedMedal;
        CB_Rank.SelectedValue = (int)Medals.Rank;
        CHK_TutorialComplete.Checked = Medals.IsTutorialComplete;
    }

    private void LoadMedalRow(int index)
    {
        var medal = Medals[index];
        var row = DGV_Medals.Rows[index];
        var cells = row.Cells;
        cells[MedalIndexColumn.Index].Value = index;
        cells[MedalNameColumn.Index].Value = MedalNames[index];
        cells[MedalTypeColumn.Index].Value = MedalTypeNames[(int)MedalList5.GetMedalType(index)];
        cells[MedalStateColumn.Index].Value = MedalStateNames[(int)medal.State];
        cells[MedalUnreadColumn.Index].Value = medal.IsUnread;
        cells[MedalDateColumn.Index].Value = GetDisplayedDate(medal);
        row.ErrorText = string.Empty;
        SetMedalDateCellState(row, medal.CanHaveDate);
    }

    private void LoadHabitatData()
    {
        DGV_Habitat.Rows.Clear();
        DGV_Habitat.Rows.Add(HabitatList5.HabitatCount);
        for (int i = 0; i < HabitatList5.HabitatCount; i++)
            LoadHabitatRow(i);
    }

    private void LoadHabitatRow(int index)
    {
        var habitat = Habitat.GetHabitat(index);
        var row = DGV_Habitat.Rows[index];
        var cells = row.Cells;
        cells[HabitatIndexColumn.Index].Value = index;
        cells[HabitatCompleteColumn.Index].Value = habitat.IsComplete;
        cells[HabitatGrassColumn.Index].Value = HabitatCompletionNames[(int)habitat.GetStatus(HabitatEncounterType5.Grass)];
        cells[HabitatSurfColumn.Index].Value = HabitatCompletionNames[(int)habitat.GetStatus(HabitatEncounterType5.Surf)];
        cells[HabitatFishColumn.Index].Value = HabitatCompletionNames[(int)habitat.GetStatus(HabitatEncounterType5.Fish)];
        row.ErrorText = string.Empty;
    }

    private void LoadHabitatSettings()
    {
        CHK_HabitatTutorialViewed.Checked = Habitat.IsTutorialViewed;
        CHK_HabitatTutorialCompleteCapture.Checked = Habitat.IsTutorialCompleteCapture;
        NUD_Unknown90.Value = Habitat.Unknown90;
        NUD_Unknown92.Value = Habitat.Unknown92;
        CB_LastEncounterType.SelectedIndex = (int)Habitat.LastEncounterType;
    }

    private void SaveHabitatSettings()
    {
        Habitat.IsTutorialViewed = CHK_HabitatTutorialViewed.Checked;
        Habitat.IsTutorialCompleteCapture = CHK_HabitatTutorialCompleteCapture.Checked;
        Habitat.Unknown90 = (ushort)NUD_Unknown90.Value;
        Habitat.Unknown92 = (byte)NUD_Unknown92.Value;
        if (CB_LastEncounterType.SelectedIndex >= 0)
            Habitat.LastEncounterType = (HabitatEncounterType5)CB_LastEncounterType.SelectedIndex;
    }

    private void SetMedalDateCellState(DataGridViewRow row, bool enabled)
    {
        var cell = row.Cells[MedalDateColumn.Index];
        cell.ReadOnly = !enabled;
        cell.Style.BackColor = enabled ? DGV_Medals.DefaultCellStyle.BackColor : SystemColors.Control;
        cell.Style.ForeColor = enabled ? DGV_Medals.DefaultCellStyle.ForeColor : SystemColors.GrayText;
        cell.Style.SelectionBackColor = enabled ? DGV_Medals.DefaultCellStyle.SelectionBackColor : SystemColors.Control;
        cell.Style.SelectionForeColor = enabled ? DGV_Medals.DefaultCellStyle.SelectionForeColor : SystemColors.GrayText;
    }

    private bool CanEditMedalDate(int rowIndex)
    {
        var medal = Medals[rowIndex];
        return medal.CanHaveDate;
    }

    private static string GetDisplayedDate(Medal5 medal) => medal is { CanHaveDate: true, HasDate: true }
        ? medal.Date.ToString(DateFormat, CultureInfo.InvariantCulture)
        : string.Empty;

    private static bool TryParseDate(string text, out DateOnly date)
    {
        text = text.Trim();
        if (string.IsNullOrWhiteSpace(text))
        {
            date = default;
            return false;
        }

        if (!DateOnly.TryParse(text, CultureInfo.CurrentCulture, DateTimeStyles.None, out date) &&
            !DateOnly.TryParseExact(text, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
        {
            return false;
        }

        return EncounterDate.IsValidDateNDS(date);
    }

    private string GetDefaultFileName() => PathUtil.CleanFileName($"{SAV.OT} {SAV.Version}.ml5");

    private void CommitPendingGridEdits()
    {
        if (DGV_Medals.IsCurrentCellDirty)
            DGV_Medals.CommitEdit(DataGridViewDataErrorContexts.Commit);
        if (DGV_Habitat.IsCurrentCellDirty)
            DGV_Habitat.CommitEdit(DataGridViewDataErrorContexts.Commit);
        DGV_Medals.EndEdit();
        DGV_Habitat.EndEdit();
    }

    private void DGV_Medals_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0)
            return;

        var medal = Medals[e.RowIndex];
        var row = DGV_Medals.Rows[e.RowIndex];
        if (e.ColumnIndex == MedalStateColumn.Index)
        {
            if (row.Cells[MedalStateColumn.Index].Value is string state)
            {
                int index = Array.IndexOf(MedalStateNames, state);
                if (index >= 0)
                    medal.State = (MedalState5)index;
            }

            if (medal is { CanHaveDate: true, HasDate: false })
                medal.Date = EncounterDate.GetDateNDS();

            LoadMedalRow(e.RowIndex);
            return;
        }

        if (e.ColumnIndex == MedalUnreadColumn.Index)
        {
            medal.IsUnread = row.Cells[MedalUnreadColumn.Index].Value is true;
            return;
        }

        if (e.ColumnIndex == MedalDateColumn.Index)
        {
            var value = row.Cells[MedalDateColumn.Index].Value?.ToString();
            if (value is null || !TryParseDate(value, out var date))
                return;

            medal.Date = date;
            row.Cells[MedalDateColumn.Index].Value = date.ToString(DateFormat, CultureInfo.InvariantCulture);
            row.ErrorText = string.Empty;
        }
    }

    private void DGV_Habitat_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0)
            return;

        var row = DGV_Habitat.Rows[e.RowIndex];
        var habitat = Habitat.GetHabitat(e.RowIndex);

        if (e.ColumnIndex == HabitatCompleteColumn.Index)
        {
            habitat.IsComplete = row.Cells[HabitatCompleteColumn.Index].Value is true;
            return;
        }

        if (e.ColumnIndex == HabitatGrassColumn.Index)
        {
            TrySetHabitatCompletion(row.Cells[HabitatGrassColumn.Index].Value, value => habitat.SetStatus(HabitatEncounterType5.Grass, value));
            return;
        }

        if (e.ColumnIndex == HabitatSurfColumn.Index)
        {
            TrySetHabitatCompletion(row.Cells[HabitatSurfColumn.Index].Value, value => habitat.SetStatus(HabitatEncounterType5.Surf, value));
            return;
        }

        if (e.ColumnIndex == HabitatFishColumn.Index)
            TrySetHabitatCompletion(row.Cells[HabitatFishColumn.Index].Value, value => habitat.SetStatus(HabitatEncounterType5.Fish, value));
    }

    private void TrySetHabitatCompletion(object? cellValue, Action<HabitatCompletion5> setter)
    {
        if (cellValue is not string text)
            return;

        int index = Array.IndexOf(HabitatCompletionNames, text);
        if (index >= 0)
            setter((HabitatCompletion5)index);
    }

    private IEnumerable<DataGridViewRow> GetSelectedHabitatRows()
    {
        if (DGV_Habitat.SelectedRows.Count != 0)
            return DGV_Habitat.SelectedRows.Cast<DataGridViewRow>().OrderBy(z => z.Index);

        return DGV_Habitat.SelectedCells.Cast<DataGridViewCell>()
            .Select(z => z.OwningRow).OfType<DataGridViewRow>()
            .Distinct()
            .OrderBy(z => z.Index);
    }
}
