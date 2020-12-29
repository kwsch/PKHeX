using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SuperTrainingEditor : Form
    {
        public SuperTrainingEditor(PKM pk)
        {
            pkm = (ISuperTrain)pk;
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            int vertScrollWidth = SystemInformation.VerticalScrollBarWidth;
            TLP_SuperTrain.Padding = TLP_DistSuperTrain.Padding = new Padding(0, 0, vertScrollWidth, 0);

            // Updating a Control display with autosized elements on every row addition is cpu intensive. Disable layout updates while populating.
            TLP_SuperTrain.SuspendLayout();
            TLP_DistSuperTrain.SuspendLayout();
            TLP_SuperTrain.Scroll += WinFormsUtil.PanelScroll;
            TLP_DistSuperTrain.Scroll += WinFormsUtil.PanelScroll;
            PopulateRegimens("SuperTrain", TLP_SuperTrain, reglist);
            PopulateRegimens("DistSuperTrain", TLP_DistSuperTrain, distlist);
            TLP_SuperTrain.ResumeLayout();
            TLP_DistSuperTrain.ResumeLayout();

            CHK_SecretUnlocked.Checked = pkm.SecretSuperTrainingUnlocked;
            CHK_SecretComplete.Checked = pkm.SecretSuperTrainingComplete;

            if (pkm is PK6 pk6)
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

        private readonly List<RegimenInfo> reglist = new();
        private readonly List<RegimenInfo> distlist = new();
        private readonly ISuperTrain pkm;
        private const string PrefixCHK = "CHK_";

        private void B_Cancel_Click(object sender, EventArgs e) => Close();

        private void B_Save_Click(object sender, EventArgs e)
        {
            Save();
            Close();
        }

        private void PopulateRegimens(string propertyPrefix, TableLayoutPanel TLP, List<RegimenInfo> list)
        {
            // Get a list of all Regimen Attregutes in the PKM
            list.AddRange(GetBooleanRegimenNames(pkm, propertyPrefix));
            TLP.ColumnCount = 1;
            TLP.RowCount = 0;

            // Add Regimens
            foreach (var reg in list)
                AddRegimenChoice(reg, TLP);

            // Force auto-size
            foreach (var style in TLP.RowStyles.OfType<RowStyle>())
                style.SizeType = SizeType.AutoSize;
            foreach (var style in TLP.ColumnStyles.OfType<ColumnStyle>())
                style.SizeType = SizeType.AutoSize;
        }

        private static IEnumerable<RegimenInfo> GetBooleanRegimenNames(ISuperTrain pkm, string propertyPrefix)
        {
            var names = ReflectUtil.GetPropertiesStartWithPrefix(pkm.GetType(), propertyPrefix);
            foreach (var name in names)
            {
                var value = ReflectUtil.GetValue(pkm, name);
                if (value is bool state)
                    yield return new RegimenInfo(name, state);
            }
        }

        private static void AddRegimenChoice(RegimenInfo reg, TableLayoutPanel TLP)
        {
            // Get row we add to
            int row = TLP.RowCount;
            TLP.RowCount++;

            var chk = new CheckBox
            {
                Anchor = AnchorStyles.Left,
                Name = PrefixCHK + reg.Name,
                Margin = new Padding(2),
                Text = reg.Name,
                AutoSize = true,
                Padding = Padding.Empty,
            };
            chk.CheckedChanged += (sender, e) => reg.CompletedRegimen = chk.Checked;
            chk.Checked = reg.CompletedRegimen;
            TLP.Controls.Add(chk, 0, row);
        }

        private void Save()
        {
            foreach (var reg in reglist)
                ReflectUtil.SetValue(pkm, reg.Name, reg.CompletedRegimen);
            foreach (var reg in distlist)
                ReflectUtil.SetValue(pkm, reg.Name, reg.CompletedRegimen);

            if (pkm is PK6 pk6)
            {
                pk6.SecretSuperTrainingUnlocked = CHK_SecretUnlocked.Checked;
                pk6.SecretSuperTrainingComplete = CHK_SecretComplete.Checked;
                pk6.TrainingBag = CB_Bag.SelectedIndex;
                pk6.TrainingBagHits = (int)NUD_BagHits.Value;
            }
            else // clear flags if manually cleared
            {
                pkm.SecretSuperTrainingUnlocked &= CHK_SecretUnlocked.Checked;
                pkm.SecretSuperTrainingComplete &= CHK_SecretComplete.Checked;
            }
        }

        private sealed class RegimenInfo
        {
            public readonly string Name;
            public bool CompletedRegimen;

            internal RegimenInfo(string name, bool completedRegimen)
            {
                Name = name;
                CompletedRegimen = completedRegimen;
            }
        }

        private void B_All_Click(object sender, EventArgs e)
        {
            if (CHK_SecretUnlocked.Checked) // only give dist if Secret is Unlocked (None -> All -> All*)
            {
                foreach (var c in TLP_DistSuperTrain.Controls.OfType<CheckBox>())
                    c.Checked = true;
            }

            if (pkm is PK6)
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
            if (pkm is not PK6)
                return;
            CHK_SecretComplete.Checked &= CHK_SecretUnlocked.Checked;
            CHK_SecretComplete.Enabled = CHK_SecretUnlocked.Checked;
            foreach (var c in TLP_SuperTrain.Controls.OfType<CheckBox>().Where(chk => Convert.ToInt16(chk.Name[14]+"") >= 4))
            {
                c.Enabled = CHK_SecretUnlocked.Checked;
                if (!CHK_SecretUnlocked.Checked)
                    c.Checked = false;
            }
        }
    }
}
