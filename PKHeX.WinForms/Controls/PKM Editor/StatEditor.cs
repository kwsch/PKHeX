using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Drawing;

namespace PKHeX.WinForms.Controls
{
    public partial class StatEditor : UserControl
    {
        public StatEditor()
        {
            InitializeComponent();
            MT_IVs   = new[] {TB_IVHP, TB_IVATK, TB_IVDEF, TB_IVSPE, TB_IVSPA, TB_IVSPD};
            MT_EVs   = new[] {TB_EVHP, TB_EVATK, TB_EVDEF, TB_EVSPE, TB_EVSPA, TB_EVSPD};
            MT_AVs   = new[] {TB_AVHP, TB_AVATK, TB_AVDEF, TB_AVSPE, TB_AVSPA, TB_AVSPD};
            MT_Stats = new[] {Stat_HP, Stat_ATK, Stat_DEF, Stat_SPE, Stat_SPA, Stat_SPD};
            L_Stats  = new[] {Label_HP, Label_ATK, Label_DEF, Label_SPE, Label_SPA, Label_SPD};
            MT_Base  = new[] {TB_BaseHP, TB_BaseATK, TB_BaseDEF, TB_BaseSPE, TB_BaseSPA, TB_BaseSPD};

            TB_BST.ResetForeColor();
            TB_IVTotal.ForeColor = MT_EVs[0].ForeColor;
            TB_EVTotal.ForeColor = MT_EVs[0].ForeColor;
        }

        public Color EVsInvalid { get; set; } = Color.Red;
        public Color EVsMaxed { get; set; } = Color.Honeydew;
        public Color EVsFishy { get; set; } = Color.LightYellow;
        public Color StatIncreased { get; set; } = Color.Red;
        public Color StatDecreased { get; set; } = Color.Blue;
        public Color StatHyperTrained { get; set; } = Color.LightGreen;

        public IMainEditor MainEditor { private get; set; } = null!;
        public bool HaX { set => CHK_HackedStats.Enabled = CHK_HackedStats.Visible = value; }

        public bool Valid
        {
            get
            {
                if (Entity.Format < 3)
                    return true;
                if (CHK_HackedStats.Checked)
                    return true;
                if (Entity is IAwakened a)
                    return a.AwakeningAllValid();
                return Convert.ToUInt32(TB_EVTotal.Text) <= 510;
            }
        }

        private readonly Label[] L_Stats;
        private readonly MaskedTextBox[] MT_EVs, MT_IVs, MT_AVs, MT_Stats, MT_Base;
        private PKM Entity => MainEditor.Entity;

        private bool ChangingFields
        {
            get => MainEditor.ChangingFields;
            set => MainEditor.ChangingFields = value;
        }

        private void ClickIV(object sender, EventArgs e)
        {
            if (sender is not MaskedTextBox t)
                return;

            switch (ModifierKeys)
            {
                case Keys.Alt: // Min
                    t.Text = 0.ToString();
                    break;

                case Keys.Control: // Max
                {
                    var index = Array.IndexOf(MT_IVs, t);
                    t.Text = Entity.GetMaximumIV(index, true).ToString();
                    break;
                }

                case Keys.Shift when Entity is IHyperTrain h: // HT
                {
                    var index = Array.IndexOf(MT_IVs, t);
                    bool flag = h.HyperTrainInvert(index);
                    UpdateHyperTrainingFlag(index, flag);
                    UpdateStats();
                    break;
                }
            }
        }

        private void ClickEV(object sender, EventArgs e)
        {
            if (sender is not MaskedTextBox t)
                return;

            if ((ModifierKeys & Keys.Control) != 0) // Max
            {
                int index = Array.IndexOf(MT_EVs, t);
                int newEV = Entity.GetMaximumEV(index);
                t.Text = newEV.ToString();
            }
            else if ((ModifierKeys & Keys.Alt) != 0) // Min
            {
                t.Text = 0.ToString();
            }
        }

        private void ClickAV(object sender, EventArgs e)
        {
            if (sender is not MaskedTextBox t)
                return;

            if ((ModifierKeys & Keys.Control) != 0) // Max
            {
                var max = Legal.AwakeningMax.ToString();
                t.Text = t.Text == max ? 0.ToString() : max;
            }
            else if ((ModifierKeys & Keys.Alt) != 0) // Min
            {
                t.Text = 0.ToString();
            }
        }

        public void UpdateIVs(object sender, EventArgs e)
        {
            if (sender is MaskedTextBox m)
            {
                int value = Util.ToInt32(m.Text);
                if (value > Entity.MaxIV)
                {
                    m.Text = Entity.MaxIV.ToString();
                    return; // recursive on text set
                }

                int index = Array.IndexOf(MT_IVs, m);
                Entity.SetIV(index, value);
            }
            RefreshDerivedValues(e);
            UpdateStats();
        }

        private void RefreshDerivedValues(object _)
        {
            if (Entity.Format < 3)
            {
                TB_IVHP.Text = Entity.IV_HP.ToString();
                TB_IVSPD.Text = Entity.IV_SPD.ToString();

                MainEditor.UpdateIVsGB(false);
            }

            if (!ChangingFields)
            {
                ChangingFields = true;
                CB_HPType.SelectedValue = Entity.HPType;
                Label_HiddenPowerPower.Text = Entity.HPPower.ToString();
                ChangingFields = false;
            }

            // Potential Reading
            L_Potential.Text = Entity.GetPotentialString(MainEditor.Unicode);

            TB_IVTotal.Text = Entity.IVTotal.ToString();
            UpdateCharacteristic(Entity.Characteristic);
        }

        private void UpdateEVs(object sender, EventArgs e)
        {
            if (sender is MaskedTextBox m)
            {
                int value = Util.ToInt32(m.Text);
                if (value > Entity.MaxEV)
                {
                    m.Text = Entity.MaxEV.ToString();
                    return; // recursive on text set
                }

                int index = Array.IndexOf(MT_EVs, m);
                Entity.SetEV(index, value);
            }

            UpdateEVTotals();

            if (Entity.Format < 3)
            {
                ChangingFields = true;
                TB_EVSPD.Text = TB_EVSPA.Text;
                ChangingFields = false;
            }

            UpdateStats();
        }

        private void UpdateAVs(object sender, EventArgs e)
        {
            if (Entity is not IAwakened a)
                return;
            if (sender is MaskedTextBox m)
            {
                int value = Util.ToInt32(m.Text);
                if (value > Legal.AwakeningMax)
                {
                    m.Text = Legal.AwakeningMax.ToString();
                    return; // recursive on text set
                }

                int index = Array.IndexOf(MT_AVs, m);
                a.SetAV(index, value);
            }

            UpdateAVTotals();
            UpdateStats();
        }

        private void UpdateRandomEVs(object sender, EventArgs e)
        {
            var evs = ModifierKeys switch
            {
                Keys.Control => SetMaxEVs(Entity),
                Keys.Alt => new int[6],
                _ => PKX.GetRandomEVs(Entity.Format),
            };
            LoadEVs(evs);
            UpdateEVs(sender, EventArgs.Empty);

            static int[] SetMaxEVs(PKM entity)
            {
                if (entity.Format < 3)
                    return Enumerable.Repeat((int) ushort.MaxValue, 6).ToArray();

                var stats = entity.PersonalInfo.Stats;
                var ordered = stats.Select((z, i) => new {Stat = z, Index = i}).OrderByDescending(z => z.Stat).ToArray();

                var result = new int[6];
                result[ordered[0].Index] = 252;
                result[ordered[1].Index] = 252;
                result[ordered[2].Index] = 6;

                return result;
            }
        }

        private void UpdateHackedStats(object sender, EventArgs e)
        {
            foreach (var s in MT_Stats)
                s.Enabled = CHK_HackedStats.Checked;
            if (!CHK_HackedStats.Checked)
                UpdateStats();
        }

        private void UpdateHackedStatText(object sender, EventArgs e)
        {
            if (!CHK_HackedStats.Checked || sender is not TextBox tb)
                return;

            string text = tb.Text;
            if (string.IsNullOrWhiteSpace(text))
                tb.Text = "0";
            else if (Convert.ToUInt32(text) > ushort.MaxValue)
                tb.Text = "65535";
        }

        private void UpdateHyperTrainingFlag(int index, bool value)
        {
            var tb = MT_IVs[index];
            if (value)
                tb.BackColor = StatHyperTrained;
            else
                tb.ResetBackColor();
        }

        private void UpdateHPType(object sender, EventArgs e)
        {
            if (ChangingFields)
                return;

            // Change IVs to match the new Hidden Power
            int hpower = WinFormsUtil.GetIndex(CB_HPType);
            int[] newIVs = HiddenPower.SetIVs(hpower, Entity.IVs, Entity.Format);
            LoadIVs(newIVs);
        }

        private void ClickStatLabel(object sender, MouseEventArgs e)
        {
            if (sender == Label_SPC)
                sender = Label_SPA;

            int index = Array.IndexOf(L_Stats, sender as Label);
            if ((ModifierKeys & Keys.Alt) != 0) // EV
            {
                bool min = e.Button != MouseButtons.Left;
                if (Entity is IAwakened)
                {
                    var value = min ? 0 : 200;
                    MT_AVs[index].Text = value.ToString();
                }
                else
                {
                    var value = min ? 0 : Entity.GetMaximumEV(index);
                    MT_EVs[index].Text = value.ToString();
                }
            }
            else if ((ModifierKeys & Keys.Control) != 0) // IV
            {
                var value = e.Button != MouseButtons.Left ? 0 : Entity.GetMaximumIV(index, true);
                MT_IVs[index].Text = value.ToString();
            }
        }

        private void LoadHyperTraining()
        {
            if (Entity is not IHyperTrain h)
            {
                foreach (var iv in MT_IVs)
                    iv.ResetBackColor();
                return;
            }

            for (int i = 0; i < MT_IVs.Length; i++)
                UpdateHyperTrainingFlag(i, h.IsHyperTrained(i));
        }

        private void UpdateAVTotals()
        {
            if (Entity is not IAwakened a)
                return;
            var total = a.AwakeningSum();
            TB_AVTotal.Text = total.ToString();
        }

        private void UpdateEVTotals()
        {
            var evtotal = Entity.EVTotal;
            TB_EVTotal.BackColor = GetEVTotalColor(evtotal, TB_IVTotal.BackColor);
            TB_EVTotal.Text = evtotal.ToString();
            EVTip.SetToolTip(TB_EVTotal, $"Remaining: {510 - evtotal}");
        }

        private Color GetEVTotalColor(int evtotal, Color defaultColor) => evtotal switch
        {
            > 510 => EVsInvalid, // Background turns Red
            510 => EVsMaxed, // Maximum EVs
            508 => EVsFishy, // Fishy EVs
            _ => defaultColor
        };

        public void UpdateStats()
        {
            // Generate the stats.
            // Some entity formats don't store stat values regardless of Box/Party/Etc format.
            // If its attack stat is zero, we need to generate party stats.
            // PK1 format stores Current HP in the compact format, so we have to use attack stat!
            if (!CHK_HackedStats.Checked || Entity.Stat_ATK == 0)
            {
                var pt = MainEditor.RequestSaveFile.Personal;
                var pi = pt.GetFormEntry(Entity.Species, Entity.Form);
                Entity.SetStats(Entity.GetStats(pi));
                LoadBST(pi);
                LoadPartyStats(Entity);
            }
        }

        private void LoadBST(PersonalInfo pi)
        {
            var stats = pi.Stats;
            for (int i = 0; i < stats.Count; i++)
            {
                MT_Base[i].Text = stats[i].ToString("000");
                MT_Base[i].BackColor = ImageUtil.ColorBaseStat(stats[i]);
            }
            var bst = pi.Stats.Sum();
            TB_BST.Text = bst.ToString("000");
            TB_BST.BackColor = ImageUtil.ColorBaseStatTotal(bst);
        }

        public void UpdateRandomIVs(object sender, EventArgs e)
        {
            var IVs = ModifierKeys switch
            {
                Keys.Control => Entity.SetRandomIVs(6),
                Keys.Alt => new int[6],
                _ => Entity.SetRandomIVs(),
            };
            LoadIVs(IVs);
        }

        private void UpdateRandomAVs(object sender, EventArgs e)
        {
            if (Entity is not IAwakened a)
                return;

            switch (ModifierKeys)
            {
                case Keys.Control:
                    a.SetSuggestedAwakenedValues(Entity);
                    break;
                case Keys.Alt:
                    a.AwakeningClear();
                    break;
                default:
                    a.AwakeningSetRandom();
                    break;
            }
            LoadAVs(a);
        }

        public void UpdateCharacteristic() => UpdateCharacteristic(Entity.Characteristic);

        private void UpdateCharacteristic(int characteristic)
        {
            L_Characteristic.Visible = Label_CharacteristicPrefix.Visible = characteristic > -1;
            if (characteristic > -1)
                L_Characteristic.Text = GameInfo.Strings.characteristics[characteristic];
        }

        public string UpdateNatureModification(int nature)
        {
            // Reset Label Colors
            for (var i = 1; i < L_Stats.Length; i++)
                L_Stats[i].ResetForeColor();

            // Set Colored StatLabels only if Nature isn't Neutral
            if (PKX.GetNatureModification(nature, out int incr, out int decr))
                return "-/-";

            L_Stats[incr].ForeColor = StatIncreased;
            L_Stats[decr].ForeColor = StatDecreased;
            return $"+{L_Stats[incr].Text} / -{L_Stats[decr].Text}".Replace(":", "");
        }

        public void SetATKIVGender(int gender)
        {
            Entity.SetAttackIVFromGender(gender);
            TB_IVATK.Text = Entity.IV_ATK.ToString();
        }

        public void LoadPartyStats(PKM pk)
        {
            Stat_HP.Text = pk.Stat_HPCurrent.ToString();
            Stat_ATK.Text = pk.Stat_ATK.ToString();
            Stat_DEF.Text = pk.Stat_DEF.ToString();
            Stat_SPA.Text = pk.Stat_SPA.ToString();
            Stat_SPD.Text = pk.Stat_SPD.ToString();
            Stat_SPE.Text = pk.Stat_SPE.ToString();
        }

        public void SavePartyStats(PKM pk)
        {
            pk.Stat_HPCurrent = Util.ToInt32(Stat_HP.Text);
            pk.Stat_HPMax = Util.ToInt32(Stat_HP.Text);
            pk.Stat_ATK = Util.ToInt32(Stat_ATK.Text);
            pk.Stat_DEF = Util.ToInt32(Stat_DEF.Text);
            pk.Stat_SPE = Util.ToInt32(Stat_SPE.Text);
            pk.Stat_SPA = Util.ToInt32(Stat_SPA.Text);
            pk.Stat_SPD = Util.ToInt32(Stat_SPD.Text);
        }

        public void LoadEVs(int[] EVs)
        {
            ChangingFields = true;
             TB_EVHP.Text = EVs[0].ToString();
            TB_EVATK.Text = EVs[1].ToString();
            TB_EVDEF.Text = EVs[2].ToString();
            TB_EVSPE.Text = EVs[3].ToString();
            TB_EVSPA.Text = EVs[4].ToString();
            TB_EVSPD.Text = EVs[5].ToString();
            ChangingFields = false;
            UpdateStats();
        }

        public void LoadIVs(int[] IVs)
        {
            ChangingFields = true;
             TB_IVHP.Text = IVs[0].ToString();
            TB_IVATK.Text = IVs[1].ToString();
            TB_IVDEF.Text = IVs[2].ToString();
            TB_IVSPE.Text = IVs[3].ToString();
            TB_IVSPA.Text = IVs[4].ToString();
            TB_IVSPD.Text = IVs[5].ToString();
            ChangingFields = false;
            LoadHyperTraining();
            RefreshDerivedValues(TB_IVSPD);
            UpdateStats();
        }

        public void LoadAVs(IAwakened a)
        {
            ChangingFields = true;
             TB_AVHP.Text = a.AV_HP.ToString();
            TB_AVATK.Text = a.AV_ATK.ToString();
            TB_AVDEF.Text = a.AV_DEF.ToString();
            TB_AVSPE.Text = a.AV_SPE.ToString();
            TB_AVSPA.Text = a.AV_SPA.ToString();
            TB_AVSPD.Text = a.AV_SPD.ToString();
            ChangingFields = false;
            UpdateStats();
        }

        public void ToggleInterface(PKM pk, int gen)
        {
            FLP_StatsTotal.Visible = gen >= 3;
            FLP_Characteristic.Visible = gen >= 3;
            FLP_HPType.Visible = gen <= 7;
            FLP_HPPower.Visible = gen <= 5;
            FLP_DynamaxLevel.Visible = gen >= 8;

            switch (gen)
            {
                case 1:
                    FLP_SpD.Visible = false;
                    Label_SPA.Visible = false;
                    Label_SPC.Visible = true;
                    TB_IVHP.Enabled = false;
                    SetEVMaskSize(Stat_HP.Size, "00000");
                    break;
                case 2:
                    FLP_SpD.Visible = true;
                    Label_SPA.Visible = true;
                    Label_SPC.Visible = false;
                    TB_IVHP.Enabled = false;
                    SetEVMaskSize(Stat_HP.Size, "00000");
                    TB_EVSPD.Enabled = TB_IVSPD.Enabled = false;
                    break;
                default:
                    FLP_SpD.Visible = true;
                    Label_SPA.Visible = true;
                    Label_SPC.Visible = false;
                    TB_IVHP.Enabled = true;
                    SetEVMaskSize(TB_EVTotal.Size, "000");
                    TB_EVSPD.Enabled = TB_IVSPD.Enabled = true;
                    break;
            }

            var showAV = pk is IAwakened;
            Label_AVs.Visible = TB_AVTotal.Visible = BTN_RandomAVs.Visible = showAV;
            foreach (var mtb in MT_AVs)
                mtb.Visible = showAV;
            Label_EVs.Visible = TB_EVTotal.Visible = BTN_RandomEVs.Visible = !showAV;
            foreach (var mtb in MT_EVs)
                mtb.Visible = !showAV;

            void SetEVMaskSize(Size s, string Mask)
            {
                foreach (var ctrl in MT_EVs)
                {
                    ctrl.Size = s;
                    ctrl.Mask = Mask;
                }
            }
        }

        public void InitializeDataSources()
        {
            ChangingFields = true;
            CB_HPType.InitializeBinding();
            CB_HPType.DataSource = Util.GetCBList(GameInfo.Strings.types.Skip(1).Take(16).ToArray());
            ChangingFields = false;
        }

        private void CHK_Gigantamax_CheckedChanged(object sender, EventArgs e)
        {
            if (!ChangingFields)
                ((PKMEditor) MainEditor).UpdateSprite();
        }
    }
}
