using System;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public partial class TechRecordEditor : Form
{
    private readonly ITechRecord8 Record;
    private readonly PKM Entity;

    public TechRecordEditor(ITechRecord8 techRecord8, PKM pk)
    {
        Record = techRecord8;
        Entity = pk;
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);

        PopulateRecords();
        LoadRecords();
    }

    private void PopulateRecords()
    {
        var names = GameInfo.Strings.Move;
        var indexes = Record.TechRecordPermitIndexes;
        var lines = new string[indexes.Length];
        for (int i = 0; i < lines.Length; i++)
            lines[i] = $"{i:00} - {names[indexes[i]]}";
        CLB_Flags.Items.AddRange(lines);
    }

    private void B_Cancel_Click(object sender, EventArgs e) => Close();

    private void B_Save_Click(object sender, EventArgs e)
    {
        Save();
        Close();
    }

    private void LoadRecords()
    {
        for (int i = 0; i < PersonalInfo8SWSH.CountTR; i++)
            CLB_Flags.SetItemChecked(i, Record.GetMoveRecordFlag(i));
    }

    private void Save()
    {
        for (int i = 0; i < PersonalInfo8SWSH.CountTR; i++)
            Record.SetMoveRecordFlag(i, CLB_Flags.GetItemChecked(i));
    }

    private void B_All_Click(object sender, EventArgs e)
    {
        Save();
        if (ModifierKeys == Keys.Shift)
        {
            Record.SetRecordFlags(true);
        }
        else if (ModifierKeys == Keys.Control)
        {
            Span<ushort> moves = stackalloc ushort[4];
            Entity.GetMoves(moves);
            Record.SetRecordFlags(moves);
        }
        else
        {
            Record.SetRecordFlags();
        }
        LoadRecords();
        Close();
    }

    private void B_None_Click(object sender, EventArgs e)
    {
        Save();
        Record.ClearRecordFlags();
        LoadRecords();
        Close();
    }
}
