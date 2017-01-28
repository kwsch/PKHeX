using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SuperTrainingEditor : Form
    {
        public SuperTrainingEditor()
        {
            InitializeComponent();
            int vertScrollWidth = SystemInformation.VerticalScrollBarWidth;
            TLP_SuperTrain.Padding = TLP_DistSuperTrain.Padding = new Padding(0, 0, vertScrollWidth, 0);

            // Updating a Control display with autosized elements on every row addition is cpu intensive. Disable layout updates while populating.
            TLP_SuperTrain.SuspendLayout();
            TLP_DistSuperTrain.SuspendLayout();
            TLP_SuperTrain.Scroll += WinFormsUtil.PanelScroll;
            TLP_DistSuperTrain.Scroll += WinFormsUtil.PanelScroll;
            populateRegimens("SuperTrain", TLP_SuperTrain, reglist);
            populateRegimens("DistSuperTrain", TLP_DistSuperTrain, distlist);
            WinFormsUtil.TranslateInterface(this, Main.curlanguage);
            TLP_SuperTrain.ResumeLayout();
            TLP_DistSuperTrain.ResumeLayout();
            
            CHK_SecretUnlocked.Checked = pkm.SecretSuperTrainingUnlocked;
            CHK_SecretComplete.Checked = pkm.SecretSuperTrainingComplete;

            if (pkm is PK6)
            {
                CB_Bag.Items.Clear();
                CB_Bag.Items.Add("---");
                for (int i = 1; i < GameInfo.Strings.trainingbags.Length - 1; i++)
                    CB_Bag.Items.Add(GameInfo.Strings.trainingbags[i]);

                PK6 pk6 = (PK6) pkm;
                CB_Bag.SelectedIndex = pk6.TrainingBag;
                NUD_BagHits.Value = pk6.TrainingBagHits;

                if (!CHK_SecretUnlocked.Checked) // force update to disable checkboxes
                    CHK_Secret_CheckedChanged(null, null);
            }
            else
            {
                L_Bag.Visible = CB_Bag.Visible = L_Hits.Visible = NUD_BagHits.Visible = false;
                CHK_SecretUnlocked.Visible = CHK_SecretComplete.Visible = false;
            }
        }

        private readonly List<RegimenInfo> reglist = new List<RegimenInfo>();
        private readonly List<RegimenInfo> distlist = new List<RegimenInfo>();
        private readonly PKM pkm = Main.pkm.Clone();
        private const string PrefixLabel = "L_";
        private const string PrefixCHK = "CHK_";

        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void B_Save_Click(object sender, EventArgs e)
        {
            save();
            Close();
        }

        private void populateRegimens(string Type, TableLayoutPanel TLP, List<RegimenInfo> list)
        {
            // Get a list of all Regimen Attregutes in the PKM
            var RegimenNames = ReflectUtil.getPropertiesStartWithPrefix(pkm.GetType(), Type);
            list.AddRange(from RegimenName in RegimenNames
                          let RegimenValue = ReflectUtil.GetValue(pkm, RegimenName)
                          where RegimenValue is bool
                          select new RegimenInfo(RegimenName, (bool) RegimenValue));
            TLP.ColumnCount = 1;
            TLP.RowCount = 0;
            
            // Add Regimens
            foreach (var reg in list)
                addRegimenChoice(reg, TLP);

            // Force auto-size
            foreach (RowStyle style in TLP.RowStyles)
                style.SizeType = SizeType.AutoSize;
            foreach (ColumnStyle style in TLP.ColumnStyles)
                style.SizeType = SizeType.AutoSize;
        }
        private void addRegimenChoice(RegimenInfo reg, TableLayoutPanel TLP)
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
            chk.CheckedChanged += (sender, e) => { reg.CompletedRegimen = chk.Checked; };
            chk.Checked = reg.CompletedRegimen;
            TLP.Controls.Add(chk, 0, row);
        }

        private void save()
        {
            foreach (var reg in reglist)
                ReflectUtil.SetValue(pkm, reg.Name, reg.CompletedRegimen);
            foreach (var reg in distlist)
                ReflectUtil.SetValue(pkm, reg.Name, reg.CompletedRegimen);

            if (pkm is PK6)
            {
                PK6 pk6 = (PK6) pkm;
                pk6.SecretSuperTrainingUnlocked = CHK_SecretUnlocked.Checked;
                pk6.SecretSuperTrainingComplete = CHK_SecretComplete.Checked;
                pk6.TrainingBag = CB_Bag.SelectedIndex;
                pk6.TrainingBagHits = (int)NUD_BagHits.Value;
            }

            Main.pkm = pkm;
        }
        
        private class RegimenInfo
        {
            public readonly string Name;
            public bool CompletedRegimen;
            public RegimenInfo(string name, bool completedRegimen)
            {
                Name = name;
                CompletedRegimen = completedRegimen;
            }
        }

        private void B_All_Click(object sender, EventArgs e)
        {
            if (CHK_SecretUnlocked.Checked) // only give dist if Secret is Unlocked (None -> All -> All*)
                foreach (var c in TLP_DistSuperTrain.Controls.OfType<CheckBox>())
                    c.Checked = true;

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
            if (pkm is PK6)
            {
                CHK_SecretUnlocked.Checked = false;
                CHK_SecretComplete.Checked = false;
            }
            foreach (var c in TLP_SuperTrain.Controls.OfType<CheckBox>())
                c.Checked = false;
            foreach (var c in TLP_DistSuperTrain.Controls.OfType<CheckBox>())
                c.Checked = false;
        }
        private void CHK_Secret_CheckedChanged(object sender, EventArgs e)
        {
            if (!(pkm is PK6))
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
