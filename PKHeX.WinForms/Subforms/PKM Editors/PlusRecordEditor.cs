using PKHeX.Core;
using PKHeX.Drawing.Misc;
using System;
using System.Buffers;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace PKHeX.WinForms;

public partial class PlusRecordEditor : Form
{
    private readonly IPlusRecord Plus;
    private readonly IPermitPlus Permit;
    private readonly PKM Entity;
    private readonly LegalityAnalysis Legality;

    private const int ColumnHasFlag = 0;
    private const int ColumnIndex = 1;
    private const int ColumnTypeIcon = 2;
    private const int ColumnType = 3;
    private const int ColumnName = 4;

    public PlusRecordEditor(IPlusRecord plus, IPermitPlus permit, PKM pk)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);

        Plus = plus;
        Permit = permit;
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
        var indexes = Permit.PlusMoveIndexes;
        // Add the records to the datagrid.
        dgv.Rows.Add(indexes.Length);

        var count = Entity.MaxMoveID + 1;
        var rent = ArrayPool<bool>.Shared.Rent(count);
        var span = rent.AsSpan(0, count);
        span.Clear();
        LearnPossible.Get(Entity, Legality.EncounterMatch, Legality.Info.EvoChainsAllGens, span);

        for (int i = 0; i < indexes.Length; i++)
        {
            var move = indexes[i];
            var type = MoveInfo.GetType(move, context);
            bool isValid = true;
            var row = dgv.Rows[i];
            var cell = row.Cells[ColumnHasFlag];
            Color color;
            if (currentMoves.Contains(move))
            {
                color = Color.LightBlue;
            }
            else if (span[move])
            {
                color = Color.LightGreen;
            }
            else
            {
                color = Color.LightCoral;
                isValid = false;
            }
            SetStyleColor(cell, color);

            row.Cells[ColumnIndex].Value = i.ToString("000");
            row.Cells[ColumnTypeIcon].Value = TypeSpriteUtil.GetTypeSpriteIconSmall(type);
            row.Cells[ColumnType].Value = type.ToString("00") + (isValid ? 0 : 1) + names[move]; // type -> valid -> name sorting
            row.Cells[ColumnName].Value = names[move];
        }

        ArrayPool<bool>.Shared.Return(rent);
        return;

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
        var range = Permit.PlusMoveIndexes;
        for (int i = 0; i < range.Length; i++)
        {
            var row = dgv.Rows[i];
            var index = int.Parse(row.Cells[ColumnIndex].Value?.ToString() ?? "");
            row.Cells[ColumnHasFlag].Value = Plus.GetMovePlusFlag(index);
        }
    }

    private void Save()
    {
        var range = Permit.PlusMoveIndexes;
        for (int i = 0; i < range.Length; i++)
        {
            var row = dgv.Rows[i];
            var index = int.Parse(row.Cells[ColumnIndex].Value?.ToString() ?? "");
            Plus.SetMovePlusFlag(index, (bool)row.Cells[ColumnHasFlag].Value!);
        }
    }

    private void B_All_Click(object sender, EventArgs e)
    {
        Save();
        var option = ModifierKeys switch
        {
            Keys.Alt => PlusRecordApplicatorOption.None,
            Keys.Shift => PlusRecordApplicatorOption.LegalSeedTM,
            Keys.Control => PlusRecordApplicatorOption.LegalCurrentTM,
            _ => PlusRecordApplicatorOption.LegalCurrent,
        };
        Plus.SetPlusFlags(Entity, Permit, option);
        Close();
    }

    private void B_None_Click(object sender, EventArgs e)
    {
        Plus.ClearPlusFlags(Permit.PlusCountTotal);
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
