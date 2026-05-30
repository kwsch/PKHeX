using System;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public sealed partial class PokeathlonConnection4Editor : UserControl
{
    public PokeathlonConnection4Editor()
    {
        InitializeComponent();
        EventRecords = [UC_Record0, UC_Record1, UC_Record2, UC_Record3, UC_Record4];
        Trainers = [UC_Trainer0, UC_Trainer1, UC_Trainer2, UC_Trainer3, UC_Trainer4];
    }

    private PokeathlonEventRecord4Editor[] EventRecords { get; }
    private PokeathlonEventTrainer4Editor[] Trainers { get; }

    public void LoadObject(PokeathlonConnection4 entity)
    {
        SuspendLayout();
        TLP_Main.SuspendLayout();

        var inner = entity.Inner;
        NUD_Attempts.Value = Math.Clamp(inner.Attempts, 0, (uint)NUD_Attempts.Maximum);
        for (int i = 0; i < EventRecords.Length; i++)
        {
            EventRecords[i].LoadObject(inner.GetRecord(i));
            Trainers[i].LoadObject(entity.GetTrainer(i));
        }

        TLP_Main.ResumeLayout();
        ResumeLayout();
    }

    public void SaveObject(PokeathlonConnection4 entity)
    {
        var inner = entity.Inner;
        inner.Attempts = (uint)NUD_Attempts.Value;
        for (int i = 0; i < EventRecords.Length; i++)
        {
            EventRecords[i].SaveObject(inner.GetRecord(i));
            Trainers[i].SaveObject(entity.GetTrainer(i));
        }
    }
}
