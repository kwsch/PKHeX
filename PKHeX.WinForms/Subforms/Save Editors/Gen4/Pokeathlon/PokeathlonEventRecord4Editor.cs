using System;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public sealed partial class PokeathlonEventRecord4Editor : UserControl
{
    public PokeathlonEventRecord4Editor()
    {
        InitializeComponent();
    }

    public void LoadObject(PokeathlonEventRecord4 entity)
    {
        NUD_Record.Value = Math.Clamp(entity.Record, (ushort)0, (ushort)NUD_Record.Maximum);
        UC_Entry0.LoadValues(entity.Entry0.Species, entity.Entry0.Form);
        UC_Entry1.LoadValues(entity.Entry1.Species, entity.Entry1.Form);
        UC_Entry2.LoadValues(entity.Entry2.Species, entity.Entry2.Form);
    }

    public void SaveObject(PokeathlonEventRecord4 entity)
    {
        entity.Record = (ushort)NUD_Record.Value;
        entity.Entry0 = new SpeciesForm10 { Species = UC_Entry0.Species, Form = UC_Entry0.Form };
        entity.Entry1 = new SpeciesForm10 { Species = UC_Entry1.Species, Form = UC_Entry1.Form };
        entity.Entry2 = new SpeciesForm10 { Species = UC_Entry2.Species, Form = UC_Entry2.Form };
    }
}
