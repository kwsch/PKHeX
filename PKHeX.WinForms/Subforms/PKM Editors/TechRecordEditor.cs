using System;
using System.Windows.Forms;
using PKHeX.Core;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Linq;

namespace PKHeX.WinForms;

enum SortType
{
    Numerical,
    Alphabetical
}

public partial class TechRecordEditor : Form
{
    private readonly ITechRecord Record;
    private readonly PKM Entity;
    private SortType sort;
    private SortedDictionary<int, string> moves_byID;
    private SortedDictionary<string, int> moves_byName;

    public TechRecordEditor(ITechRecord techRecord, PKM pk)
    {
        Record = techRecord;
        Entity = pk;
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);

        moves_byID = new SortedDictionary<int, string>();
        moves_byName = new SortedDictionary<string, int>();

        PopulateRecords();
        sort = SortType.Numerical;
        LoadRecords();
    }

    private void PopulateRecords()
    {
        var names = GameInfo.Strings.Move;
        var indexes = Record.Permit.RecordPermitIndexes;
        var lines = new string[indexes.Length];
        for (int i = 0; i < lines.Length; i++)
        {
            lines[i] = $"{i:00} - {names[indexes[i]]}";
            moves_byID.Add(i, names[indexes[i]]);
            moves_byName.Add(names[indexes[i]], i);
        }
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
        var range = Record.Permit.RecordPermitIndexes;
        for (int i = 0; i < range.Length; i++)
        {
            if (sort == SortType.Numerical)
            {
                CLB_Flags.SetItemChecked(i, Record.GetMoveRecordFlag(i));
            }
            else if (sort == SortType.Alphabetical)
            {
                int idx = moves_byName.ElementAt(i).Value;
                CLB_Flags.SetItemChecked(i, Record.GetMoveRecordFlag(idx));
            }
        }
    }

    private void Save()
    {
        var range = Record.Permit.RecordPermitIndexes;
        for (int i = 0; i < range.Length; i++)
        {
            if (sort == SortType.Numerical)
            {
                Record.SetMoveRecordFlag(i, CLB_Flags.GetItemChecked(i));
            }
            else if (sort == SortType.Alphabetical)
            {
                int idx = moves_byName.ElementAt(i).Value;
                Record.SetMoveRecordFlag(idx, CLB_Flags.GetItemChecked(i));
            }
        }
    }

    private void B_All_Click(object sender, EventArgs e)
    {
        if (ModifierKeys == Keys.Shift)
        {
            Record.ClearRecordFlags();
            Record.SetRecordFlags(true, Record.Permit.RecordCountUsed);
        }
        else if (ModifierKeys == Keys.Control)
        {
            Save();
            Span<ushort> moves = stackalloc ushort[4];
            Entity.GetMoves(moves);
            Record.SetRecordFlags(moves);
        }
        else
        {
            Record.ClearRecordFlags();
            Record.SetRecordFlags();
        }
        Close();
    }

    private void B_None_Click(object sender, EventArgs e)
    {
        Record.ClearRecordFlags();
        Close();
    }

    private void B_SortID_Click(object sender, EventArgs e)
    {
        var lines = new string[moves_byID.Count];
        int i = 0;
        foreach (KeyValuePair<int, string> pair in moves_byID)
        {
            lines[i] = $"{pair.Key:00} - {pair.Value}";
            ++i;
        }
        CLB_Flags.Items.Clear();
        CLB_Flags.Items.AddRange(lines);
        sort = SortType.Numerical;
        LoadRecords();
    }

    private void B_SortName_Click(object sender, EventArgs e)
    {
        var lines = new string[moves_byName.Count];
        int i = 0;
        foreach (KeyValuePair<string, int> pair in moves_byName)
        {
            lines[i] = $"{pair.Value:00} - {pair.Key}";
            ++i;
        }
        CLB_Flags.Items.Clear();
        CLB_Flags.Items.AddRange(lines);
        sort = SortType.Alphabetical;
        LoadRecords();
    }
}
