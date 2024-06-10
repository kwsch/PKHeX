using System;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public partial class SuperTrainingEditor : Form
{
    private readonly CheckBox[] Regular;
    private readonly CheckBox[] Distribution;

    public SuperTrainingEditor(ISuperTrainRegimen pk)
    {
        InitializeComponent();
        TLP_SuperTrain.SuspendLayout();
        TLP_DistSuperTrain.SuspendLayout();
        Regular = CreateRegular();
        Distribution = CreateDistribution();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        int vertScrollWidth = SystemInformation.VerticalScrollBarWidth;
        TLP_SuperTrain.Padding = TLP_DistSuperTrain.Padding = new Padding(0, 0, vertScrollWidth, 0);

        // Updating a Control display with auto-sized elements on every row addition is cpu intensive. Disable layout updates while populating.
        TLP_SuperTrain.Scroll += WinFormsUtil.PanelScroll;
        TLP_DistSuperTrain.Scroll += WinFormsUtil.PanelScroll;

        Entity = pk;
        LoadRegimens();

        ResizePanel(TLP_SuperTrain);
        ResizePanel(TLP_DistSuperTrain);
        TLP_SuperTrain.ResumeLayout();
        TLP_DistSuperTrain.ResumeLayout();

        CHK_SecretUnlocked.Checked = Entity.SecretSuperTrainingUnlocked;
        CHK_SecretComplete.Checked = Entity.SecretSuperTrainingComplete;

        if (pk is PK6 pk6)
        {
            CB_Bag.Items.Clear();
            CB_Bag.Items.Add("---");
            for (int i = 1; i < GameInfo.Strings.trainingbags.Length - 1; i++)
                CB_Bag.Items.Add(GameInfo.Strings.trainingbags[i]);

            CB_Bag.SelectedIndex = pk6.TrainingBag;
            NUD_BagHits.Value = pk6.TrainingBagHits;

            if (!CHK_SecretUnlocked.Checked) // force update to disable checkboxes
                CHK_Secret_CheckedChanged(this, EventArgs.Empty);
        }
        else
        {
            L_Bag.Visible = CB_Bag.Visible = L_Hits.Visible = NUD_BagHits.Visible = false;
            CHK_SecretUnlocked.Visible = CHK_SecretComplete.Visible = false;
        }
    }

    private CheckBox[] CreateRegular()
    {
        var result = new CheckBox[SuperTrainRegimenExtensions.CountRegimen];
        TLP_SuperTrain.RowCount = result.Length;
        TLP_SuperTrain.ColumnCount = 1;

        for (int i = 0; i < result.Length; i++)
        {
            var name = SuperTrainRegimenExtensions.GetRegimenName(i);
            var chk = GetCheckbox(name);
            TLP_SuperTrain.Controls.Add(chk, 0, i);
            result[i] = chk;
        }
        return result;
    }

    private CheckBox[] CreateDistribution()
    {
        var result = new CheckBox[SuperTrainRegimenExtensions.CountRegimenDistribution];
        TLP_DistSuperTrain.RowCount = result.Length;
        TLP_DistSuperTrain.ColumnCount = 1;

        for (int i = 0; i < result.Length; i++)
        {
            var name = SuperTrainRegimenExtensions.GetRegimenName(i);
            var chk = GetCheckbox(name);
            TLP_DistSuperTrain.Controls.Add(chk, 0, i);
            result[i] = chk;
        }
        return result;
    }

    private static CheckBox GetCheckbox(string name) => new()
    {
        Name = PrefixCHK + name,
        Text = name,
        AutoSize = true,
        Margin = new Padding(2),
        Padding = Padding.Empty,
    };

    private readonly ISuperTrainRegimen Entity;
    private const string PrefixCHK = "CHK_";

    private void B_Cancel_Click(object sender, EventArgs e) => Close();

    private void B_Save_Click(object sender, EventArgs e)
    {
        Save();
        Close();
    }

    private static void ResizePanel(TableLayoutPanel tlp)
    {
        // Force auto-size
        foreach (var style in tlp.RowStyles.OfType<RowStyle>())
            style.SizeType = SizeType.AutoSize;
        foreach (var style in tlp.ColumnStyles.OfType<ColumnStyle>())
            style.SizeType = SizeType.AutoSize;
    }

    private void LoadRegimens()
    {
        for (int i = 0; i < Regular.Length; i++)
            Regular[i].Checked = Entity.GetRegimenState(i);
        for (int i = 0; i < Distribution.Length; i++)
            Distribution[i].Checked = Entity.GetRegimenStateDistribution(i);
    }

    private void Save()
    {
        for (int i = 0; i < Regular.Length; i++)
            Entity.SetRegimenState(i, Regular[i].Checked);
        for (int i = 0; i < Distribution.Length; i++)
            Entity.SetRegimenStateDistribution(i, Distribution[i].Checked);

        if (Entity is PK6 pk6)
        {
            pk6.SecretSuperTrainingUnlocked = CHK_SecretUnlocked.Checked;
            pk6.SecretSuperTrainingComplete = CHK_SecretComplete.Checked;
            pk6.TrainingBag = CB_Bag.SelectedIndex;
            pk6.TrainingBagHits = (int)NUD_BagHits.Value;
        }
        else // clear flags if manually cleared
        {
            Entity.SecretSuperTrainingUnlocked &= CHK_SecretUnlocked.Checked;
            Entity.SecretSuperTrainingComplete &= CHK_SecretComplete.Checked;
        }
    }

    private void B_All_Click(object sender, EventArgs e)
    {
        if (CHK_SecretUnlocked.Checked) // only give dist if Secret is Unlocked (None -> All -> All*)
        {
            foreach (var c in TLP_DistSuperTrain.Controls.OfType<CheckBox>())
                c.Checked = true;
        }

        if (Entity is PK6)
        {
            CHK_SecretUnlocked.Checked = true;
            CHK_SecretComplete.Checked = true;
        }
        foreach (var c in TLP_SuperTrain.Controls.OfType<CheckBox>())
            c.Checked = true;
    }

    private void B_None_Click(object sender, EventArgs e)
    {
        CHK_SecretUnlocked.Checked = false;
        CHK_SecretComplete.Checked = false;
        foreach (var c in TLP_SuperTrain.Controls.OfType<CheckBox>())
            c.Checked = false;
        foreach (var c in TLP_DistSuperTrain.Controls.OfType<CheckBox>())
            c.Checked = false;
    }

    private void CHK_Secret_CheckedChanged(object sender, EventArgs e)
    {
        if (Entity is not PK6)
            return;
        CHK_SecretComplete.Checked &= CHK_SecretUnlocked.Checked;
        CHK_SecretComplete.Enabled = CHK_SecretUnlocked.Checked;
        foreach (var c in TLP_SuperTrain.Controls.OfType<CheckBox>().Where(chk => Convert.ToInt16(chk.Name[14] + "") >= 4))
        {
            c.Enabled = CHK_SecretUnlocked.Checked;
            if (!CHK_SecretUnlocked.Checked)
                c.Checked = false;
        }
    }
}
