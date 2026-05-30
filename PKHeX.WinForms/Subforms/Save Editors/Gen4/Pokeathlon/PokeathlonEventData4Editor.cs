using System;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public sealed partial class PokeathlonEventData4Editor : UserControl
{
    public PokeathlonEventData4Editor()
    {
        InitializeComponent();
        EventRecords = [UC_Record0, UC_Record1, UC_Record2, UC_Record3, UC_Record4];
    }

    private PokeathlonEventRecord4Editor[] EventRecords { get; }

    public void LoadObject(PokeathlonEventData4 entity)
    {
        NUD_Attempts.Value = Math.Clamp(entity.Attempts, 0, (uint)NUD_Attempts.Maximum);
        for (int i = 0; i < EventRecords.Length; i++)
            EventRecords[i].LoadObject(entity.GetRecord(i));
    }

    public void SaveObject(PokeathlonEventData4 entity)
    {
        entity.Attempts = (uint)NUD_Attempts.Value;
        for (int i = 0; i < EventRecords.Length; i++)
            EventRecords[i].SaveObject(entity.GetRecord(i));
    }
}
