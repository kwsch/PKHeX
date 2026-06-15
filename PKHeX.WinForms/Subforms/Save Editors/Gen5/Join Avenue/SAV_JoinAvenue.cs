using System;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public sealed partial class SAV_JoinAvenue : Form
{
    private readonly SAV5B2W2 Origin;
    private readonly SAV5B2W2 SAV;
    private readonly JoinAvenue5 Avenue;

    private readonly JoinAvenueListEditor<JoinAvenueVisitor5, JoinAvenueVisitorSpecificEditor> VisitorsEditor;
    private readonly JoinAvenueListEditor<JoinAvenueFan5, JoinAvenueFanSpecificEditor> FansEditor;
    private readonly JoinAvenueListEditor<JoinAvenueVisitor5, JoinAvenueVisitorSpecificEditor> OccupantsEditor;
    private readonly JoinAvenueListEditor<JoinAvenueAssistant5, JoinAvenueAssistantSpecificEditor> AssistantsEditor;

    public SAV_JoinAvenue(SAV5B2W2 sav)
    {
        InitializeComponent();

#if DEBUG // Translation bruteforce passes in null, need it to init the form controls for translation to work.
        // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
        sav ??= new SAV5B2W2();
#endif

        Origin = sav;
        SAV = (SAV5B2W2)sav.Clone();
        Avenue = SAV.JoinAvenue;

        VisitorsEditor = new(JoinAvenue5.VisitorCount, Avenue.GetVisitor, new JoinAvenueVisitorSpecificEditor());
        FansEditor = new(JoinAvenue5.FanCount, Avenue.GetFan, new JoinAvenueFanSpecificEditor());
        OccupantsEditor = new(JoinAvenue5.OccupantCount, Avenue.GetOccupant, new JoinAvenueVisitorSpecificEditor());
        AssistantsEditor = new(JoinAvenue5.AssistantCount, Avenue.GetAssistant, new JoinAvenueAssistantSpecificEditor());

        AddDockedControl(P_Visitors, VisitorsEditor);
        AddDockedControl(P_Fans, FansEditor);
        AddDockedControl(P_Occupants, OccupantsEditor);
        AddDockedControl(P_Assistants, AssistantsEditor);

        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);

        LoadData();
    }

    private void LoadData()
    {
        CHK_ScriptFlag.Checked = Avenue.ScriptFlag;
        NUD_VisitorCount.Value = Math.Clamp(Avenue.CountVisitor, 0, (uint)NUD_VisitorCount.Maximum);
        NUD_FanCount.Value = Math.Clamp(Avenue.CountFan, 0, (uint)NUD_FanCount.Maximum);
        UC_Settings.LoadObject(Avenue.Settings);
        VisitorsEditor.LoadAll();
        FansEditor.LoadAll();
        OccupantsEditor.LoadAll();
        AssistantsEditor.LoadAll();
        UC_SelfGeneral.LoadObject(Avenue.Self);
        UC_SelfSpecific.LoadObject(Avenue.Self);
    }

    private void B_Save_Click(object sender, EventArgs e)
    {
        Avenue.ScriptFlag = CHK_ScriptFlag.Checked;
        Avenue.CountVisitor = (uint)NUD_VisitorCount.Value;
        Avenue.CountFan = (uint)NUD_FanCount.Value;
        UC_Settings.SaveObject(Avenue.Settings);
        VisitorsEditor.SaveAll();
        FansEditor.SaveAll();
        OccupantsEditor.SaveAll();
        AssistantsEditor.SaveAll();
        UC_SelfGeneral.SaveObject(Avenue.Self);
        UC_SelfSpecific.SaveObject(Avenue.Self);
        Origin.CopyChangesFrom(SAV);
        Close();
    }

    private static void AddDockedControl(Control parent, Control child)
    {
        child.Dock = DockStyle.Fill;
        parent.Controls.Add(child);
    }

    private void B_Cancel_Click(object sender, EventArgs e) => Close();
}
