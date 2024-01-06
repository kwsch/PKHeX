using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Drawing.Misc;

namespace PKHeX.WinForms;

public partial class TechRecordEditor : Form
{
    private readonly ITechRecord Record;
    private readonly PKM Entity;
    private readonly LegalityAnalysis Legality;

    private const int ColumnHasFlag = 0;
    private const int ColumnIndex = 1;
    private const int ColumnTypeIcon = 2;
    private const int ColumnType = 3;
    private const int ColumnName = 4;

    public TechRecordEditor(ITechRecord techRecord, PKM pk)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);

        Record = techRecord;
        Entity = pk;
        Legality = new LegalityAnalysis(pk);

        Span<ushort> currentMoves = stackalloc ushort[4];
        pk.GetMoves(currentMoves);
        PopulateRecords(pk.Context, currentMoves);
        LoadRecords();
    }

    private void PopulateRecords(EntityContext context, ReadOnlySpan<ushort> currentMoves)
    {
        var names = GameInfo.Strings.Move;
        var indexes = Record.Permit.RecordPermitIndexes;
        // Add the records to the datagrid.
        dgv.Rows.Add(indexes.Length);
        var evos = Legality.Info.EvoChainsAllGens.Get(context);
        for (int i = 0; i < indexes.Length; i++)
        {
            var move = indexes[i];
            var type = MoveInfo.GetType(move, context);
            var isValid = Record.Permit.IsRecordPermitted(i);
            var row = dgv.Rows[i];
            var cell = row.Cells[ColumnHasFlag];
            if (isValid)
                SetStyleColor(cell, Color.LightGreen);
            else if (Record.IsRecordPermitted(evos, i))
                SetStyleColor(cell, Color.Yellow);
            else
                cell.Style.SelectionBackColor = Color.Red;

            if (currentMoves.Contains(move))
                row.Cells[ColumnName].Style.BackColor = Color.LightBlue;

            row.Cells[ColumnIndex].Value = i.ToString("000");
            row.Cells[ColumnTypeIcon].Value = TypeSpriteUtil.GetTypeSpriteIconSmall(type);
            row.Cells[ColumnType].Value = type.ToString("00") + (isValid ? 0 : 1) + names[move]; // type -> valid -> name sorting
            row.Cells[ColumnName].Value = names[move];
        }

        static void SetStyleColor(DataGridViewCell cell, Color color) => cell.Style.BackColor = cell.Style.SelectionBackColor = color;
    }

    private void B_Cancel_Click(object sender, EventArgs e) => Close();

    private void B_Save_Click(object sender, EventArgs e)
    {
        Save();
        Close();
    }

    private void LoadRecords()
    {
        var range = Record.Permit.RecordPermitIndexes;
        for (int i = 0; i < range.Length; i++)
        {
            var row = dgv.Rows[i];
            var index = int.Parse(row.Cells[ColumnIndex].Value?.ToString() ?? "");
            row.Cells[ColumnHasFlag].Value = Record.GetMoveRecordFlag(index);
        }
    }

    private void Save()
    {
        var range = Record.Permit.RecordPermitIndexes;
        for (int i = 0; i < range.Length; i++)
        {
            var row = dgv.Rows[i];
            var index = int.Parse(row.Cells[ColumnIndex].Value?.ToString() ?? "");
            Record.SetMoveRecordFlag(index, (bool)row.Cells[ColumnHasFlag].Value);
        }
    }

    private void B_All_Click(object sender, EventArgs e)
    {
        if (ModifierKeys == Keys.Shift)
        {
            Record.ClearRecordFlags();
            Record.SetRecordFlagsAll(true, Record.Permit.RecordCountUsed);
        }
        else if (ModifierKeys == Keys.Control)
        {
            Save();
            Span<ushort> moves = stackalloc ushort[4];
            Entity.GetMoves(moves);
            var la = new LegalityAnalysis(Entity);
            Record.SetRecordFlags(moves, la.Info.EvoChainsAllGens.Get(Entity.Context));
        }
        else
        {
            Record.ClearRecordFlags();
            Record.SetRecordFlagsAll();
            var la = new LegalityAnalysis(Entity);
            Record.SetRecordFlagsAll(la.Info.EvoChainsAllGens.Get(Entity.Context));
        }
        Close();
    }

    private void B_None_Click(object sender, EventArgs e)
    {
        Record.ClearRecordFlags();
        Close();
    }

    private void ClickCell(object sender, DataGridViewCellEventArgs e)
    {
        var rowInd = e.RowIndex;
        if (rowInd < 0)
            return;
        var row = dgv.Rows[rowInd];

        // Toggle the checkbox of cell 0
        var cell = row.Cells[ColumnHasFlag];
        cell.Value = !(bool)cell.Value;
    }

    private void PressKeyCell(object sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.Space)
            return;

        var row = dgv.CurrentRow;
        if (row == null)
            return;

        // Toggle the checkbox of cell 0
        var cell = row.Cells[ColumnHasFlag];
        cell.Value = !(bool)cell.Value;
    }

    private void SortColumn(object sender, DataGridViewCellMouseEventArgs e)
    {
        if (e.ColumnIndex == ColumnTypeIcon)
            dgv.Sort(TypeInt, ListSortDirection.Ascending);
    }
}
