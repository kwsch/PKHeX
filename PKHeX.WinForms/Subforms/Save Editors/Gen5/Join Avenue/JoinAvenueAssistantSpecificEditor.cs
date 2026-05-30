using System;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public sealed partial class JoinAvenueAssistantSpecificEditor : UserControl, IJoinAvenueSpecificEditor<JoinAvenueAssistant5>
{
    public JoinAvenueAssistantSpecificEditor() => InitializeComponent();

    public void LoadObject(JoinAvenueAssistant5 entity)
    {
        NUD_Position0.Value = Math.Clamp(entity.Position0, (byte)0, (byte)NUD_Position0.Maximum);
        NUD_Position1.Value = Math.Clamp(entity.Position1, (byte)0, (byte)NUD_Position1.Maximum);
        NUD_Position2.Value = Math.Clamp(entity.Position2, (byte)0, (byte)NUD_Position2.Maximum);
        NUD_PositionUnused.Value = Math.Clamp(entity.PositionUnused, (byte)0, (byte)NUD_PositionUnused.Maximum);
        CHK_InteractedToday.Checked = entity.IsInteractedToday;
    }

    public void SaveObject(JoinAvenueAssistant5 entity)
    {
        entity.Position0 = (byte)NUD_Position0.Value;
        entity.Position1 = (byte)NUD_Position1.Value;
        entity.Position2 = (byte)NUD_Position2.Value;
        entity.PositionUnused = (byte)NUD_PositionUnused.Value;
        entity.IsInteractedToday = CHK_InteractedToday.Checked;
    }
}
