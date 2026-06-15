using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public sealed partial class JoinAvenueFanSpecificEditor : UserControl, IJoinAvenueSpecificEditor<JoinAvenueFan5>
{
    public JoinAvenueFanSpecificEditor()
    {
        InitializeComponent();
        InitializeCombo(CB_Species, GameInfo.FilteredSources.Species.ToList());
    }

    public void LoadObject(JoinAvenueFan5 entity)
    {
        NUD_Unknown4C.Value = Math.Clamp(entity.Unknown4C, (byte)0, (byte)NUD_Unknown4C.Maximum);
        NUD_Unknown4D.Value = Math.Clamp(entity.Unknown4D, (byte)0, (byte)NUD_Unknown4D.Maximum);
        NUD_Unknown4E.Value = Math.Clamp(entity.Unknown4F, (byte)0, (byte)NUD_Unknown4E.Maximum);
        CHK_InteractedToday.Checked = entity.IsInteractedToday;
        SetComboValue(CB_Species, entity.Species);
        NUD_Unknown52.Value = Math.Clamp(entity.Unknown52, (ushort)0, (ushort)NUD_Unknown52.Maximum);
        NUD_Unknown54.Value = Math.Clamp(entity.Unknown54, (byte)0, (byte)NUD_Unknown54.Maximum);
        NUD_BubbleTarget.Value = Math.Clamp(entity.BubbleTarget, (byte)0, (byte)NUD_BubbleTarget.Maximum);
        NUD_Unknown56.Value = Math.Clamp(entity.Unknown56, (byte)0, (byte)NUD_Unknown56.Maximum);
        NUD_Unknown5A.Value = Math.Clamp(entity.Unknown5A, (ushort)0, (ushort)NUD_Unknown5A.Maximum);
    }

    public void SaveObject(JoinAvenueFan5 entity)
    {
        entity.Unknown4C = (byte)NUD_Unknown4C.Value;
        entity.Unknown4D = (byte)NUD_Unknown4D.Value;
        entity.Unknown4F = (byte)NUD_Unknown4E.Value;
        entity.IsInteractedToday = CHK_InteractedToday.Checked;
        entity.Species = (ushort)WinFormsUtil.GetIndex(CB_Species);
        entity.Unknown52 = (ushort)NUD_Unknown52.Value;
        entity.Unknown54 = (byte)NUD_Unknown54.Value;
        entity.BubbleTarget = (byte)NUD_BubbleTarget.Value;
        entity.Unknown56 = (byte)NUD_Unknown56.Value;
        entity.Unknown5A = (ushort)NUD_Unknown5A.Value;
    }

    private static void InitializeCombo(ComboBox cb, IReadOnlyList<ComboItem> source)
    {
        cb.InitializeBinding();
        cb.DataSource = new BindingSource(source, string.Empty);
    }

    private static void SetComboValue(ComboBox cb, int value) => cb.SelectedValue = value;
}
