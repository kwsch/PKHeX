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
        var baseRecordIndex = context == EntityContext.Gen9a ? 1 : 0; // TM001 in Legends: Z-A but is 0-index bits.
        // Add the records to the datagrid.
        dgv.Rows.Add(indexes.Length);
        var evos = Legality.Info.EvoChainsAllGens.Get(context);
        for (int i = 0; i < indexes.Length; i++)
        {
            var move = indexes[i];
            var type = MoveInfo.GetType(move, context);
            var cells = dgv.Rows[i].Cells;
            var cell = cells[ColumnHasFlag];

            bool isValid = Record.Permit.IsRecordPermitted(i);
            if (isValid)
                SetStyleColor(cell, Color.LightGreen);
            else if (Record.IsRecordPermitted(evos, i))
                SetStyleColor(cell, Color.Yellow);
            else
                SetStyleColor(cell, Color.LightCoral);
            if (currentMoves.Contains(move))
            {
                var style = cells[ColumnName].Style;
                style.BackColor = Color.LightBlue;
                style.ForeColor = Color.Black;
            }

            cells[ColumnIndex].Value = (i+ baseRecordIndex).ToString("000");
            cells[ColumnTypeIcon].Value = TypeSpriteUtil.GetTypeSpriteIconSmall(type);
            cells[ColumnType].Value = type.ToString("00") + (isValid ? 0 : 1) + names[move]; // type -> valid -> name sorting
            cells[ColumnName].Value = names[move];
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
            Record.SetMoveRecordFlag(index, (bool)row.Cells[ColumnHasFlag].Value!);
        }
    }

    private void B_All_Click(object sender, EventArgs e)
    {
        Save();
        var option = ModifierKeys switch
        {
            Keys.Alt => TechnicalRecordApplicatorOption.None,
            Keys.Shift => TechnicalRecordApplicatorOption.ForceAll,
            Keys.Control => TechnicalRecordApplicatorOption.LegalCurrent,
            _ => TechnicalRecordApplicatorOption.LegalAll,
        };
        Record.SetRecordFlags(Entity, option);
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
        cell.Value = !(bool)cell.Value!;
    }

    private void PressKeyCell(object sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.Space)
            return;

        var row = dgv.CurrentRow;
        if (row is null)
            return;

        // Toggle the checkbox of cell 0
        var cell = row.Cells[ColumnHasFlag];
        cell.Value = !(bool)cell.Value!;
    }

    private void SortColumn(object sender, DataGridViewCellMouseEventArgs e)
    {
        if (e.ColumnIndex == ColumnTypeIcon)
            dgv.Sort(TypeInt, ListSortDirection.Ascending);
    }
}
