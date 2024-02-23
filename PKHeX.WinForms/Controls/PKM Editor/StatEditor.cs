using System;
using System.Drawing;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Drawing;
using PKHeX.Drawing.Misc;
using PKHeX.Drawing.PokeSprite;

namespace PKHeX.WinForms.Controls;

public partial class StatEditor : UserControl
{
    public StatEditor()
    {
        InitializeComponent();
        MT_IVs = [TB_IVHP, TB_IVATK, TB_IVDEF, TB_IVSPE, TB_IVSPA, TB_IVSPD];
        MT_EVs = [TB_EVHP, TB_EVATK, TB_EVDEF, TB_EVSPE, TB_EVSPA, TB_EVSPD];
        MT_AVs = [TB_AVHP, TB_AVATK, TB_AVDEF, TB_AVSPE, TB_AVSPA, TB_AVSPD];
        MT_GVs = [TB_GVHP, TB_GVATK, TB_GVDEF, TB_GVSPE, TB_GVSPA, TB_GVSPD];
        MT_Stats = [Stat_HP, Stat_ATK, Stat_DEF, Stat_SPE, Stat_SPA, Stat_SPD];
        L_Stats = [Label_HP, Label_ATK, Label_DEF, Label_SPE, Label_SPA, Label_SPD];
        MT_Base = [TB_BaseHP, TB_BaseATK, TB_BaseDEF, TB_BaseSPE, TB_BaseSPA, TB_BaseSPD];

        TB_BST.ResetForeColor();
        TB_IVTotal.ForeColor = TB_EVTotal.ForeColor = MT_EVs[0].ForeColor;
    }

    public Color EVsInvalid { get; set; } = Color.Red;
    public Color EVsMaxed { get; set; } = Color.Honeydew;
    public Color EVsFishy { get; set; } = Color.LightYellow;
    public Color StatIncreased { get; set; } = Color.Red;
    public Color StatDecreased { get; set; } = Color.Blue;
    public Color StatHyperTrained { get; set; } = Color.LightGreen;

    public PKMEditor MainEditor { private get; set; } = null!;
    public bool HaX { get => CHK_HackedStats.Enabled; set => CHK_HackedStats.Enabled = CHK_HackedStats.Visible = value; }

    private readonly ToolTip EVTip = new();
    private StatEditorStatOrder StatOrder;

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
            return Convert.ToUInt32(TB_EVTotal.Text) <= EffortValues.Max510;
        }
    }

    private readonly Label[] L_Stats;
    private readonly MaskedTextBox[] MT_EVs, MT_IVs, MT_AVs, MT_GVs, MT_Stats, MT_Base;
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
            var max = AwakeningUtil.AwakeningMax.ToString();
            t.Text = t.Text == max ? 0.ToString() : max;
        }
        else if ((ModifierKeys & Keys.Alt) != 0) // Min
        {
            t.Text = 0.ToString();
        }
    }
    private void ClickGV(object sender, EventArgs e)
    {
        if (sender is not MaskedTextBox t || Entity is not IGanbaru g)
            return;

        if ((ModifierKeys & Keys.Control) != 0) // Max
        {
            int index = Array.IndexOf(MT_GVs, t);
            var max = g.GetMax(Entity, index).ToString();
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
            if (Entity is IGanbaru g)
                RefreshGanbaru(Entity, g, index);
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
            var value = (byte)Math.Min(byte.MaxValue, Util.ToInt32(m.Text));
            if (value > AwakeningUtil.AwakeningMax)
            {
                m.Text = AwakeningUtil.AwakeningMax.ToString();
                return; // recursive on text set
            }

            int index = Array.IndexOf(MT_AVs, m);
            a.SetAV(index, value);
        }

        UpdateAVTotals();
        UpdateStats();
    }

    private void UpdateGVs(object sender, EventArgs e)
    {
        if (Entity is not IGanbaru g)
            return;
        if (sender is MaskedTextBox m)
        {
            int value = Util.ToInt32(m.Text);
            if (value > GanbaruExtensions.TrueMax)
            {
                m.Text = GanbaruExtensions.TrueMax.ToString();
                return; // recursive on text set
            }

            int index = Array.IndexOf(MT_GVs, m);
            g.SetGV(index, (byte)value);
            RefreshGanbaru(Entity, g, index);
        }

        UpdateStats();
    }

    private void UpdateRandomEVs(object sender, EventArgs e)
    {
        Span<int> values = stackalloc int[6];
        switch (ModifierKeys)
        {
            case Keys.Control:
                EffortValues.SetMax(values, Entity);
                break;
            case Keys.Alt:
                EffortValues.Clear(values);
                break;
            default:
                EffortValues.SetRandom(values, Entity.Format);
                break;
        }
        LoadEVs(values);
        UpdateEVs(sender, EventArgs.Empty);
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
        Span<int> ivs = stackalloc int[6];
        Entity.GetIVs(ivs);
        int hpower = WinFormsUtil.GetIndex(CB_HPType);
        if (Main.Settings.EntityEditor.HiddenPowerOnChangeMaxPower)
            ivs.Fill(Entity.MaxIV);
        HiddenPower.SetIVs(hpower, ivs, Entity.Context);
        LoadIVs(ivs);
    }

    private void ClickStatLabel(object sender, MouseEventArgs e)
    {
        if (Entity.Format < 3)
            return;

        if (ModifierKeys == Keys.None)
            return;

        int index = Array.IndexOf(L_Stats, sender as Label) - 1;
        if (index < 0)
            return;

        var request = ModifierKeys switch
        {
            Keys.Control => NatureAmpRequest.Neutral,
            Keys.Alt => NatureAmpRequest.Decrease,
            _ => NatureAmpRequest.Increase,
        };

        var newNature = request.GetNewNature(index, Entity.StatNature);
        if (newNature == Nature.Random)
            return;

        MainEditor.ChangeNature(newNature);
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

    private Color GetEVTotalColor(int evtotal, Color defaultColor) => EffortValues.GetGrade(evtotal) switch
    {
        EffortValueGrade.Illegal => EVsInvalid, // Background turns Red
        EffortValueGrade.MaxLegal => EVsMaxed, // Maximum EVs
        EffortValueGrade.MaxEffective => EVsFishy, // Fishy EVs
        _ => defaultColor,
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
            Span<ushort> stats = stackalloc ushort[6];
            Entity.LoadStats(pi, stats);
            Entity.SetStats(stats);
            LoadBST(pi);
            LoadPartyStats(Entity);
        }
        if (Entity is ITeraType)
        {
            var pi = Entity.PersonalInfo;
            PB_TeraType1.SetType(pi.Type1, false); // Personal Info are just regular move types.
            PB_TeraType2.SetType(pi.Type2, false); // Personal Info are just regular move types.
        }
    }

    private void LoadBST(IBaseStat pi)
    {
        int bst = 0;
        for (int index = 0; index < 6; index++)
        {
            var value = pi.GetBaseStatValue(index);
            var s = MT_Base[index];
            s.Text = value.ToString("000");
            s.BackColor = ColorUtil.ColorBaseStat(value);
            bst += value;
        }

        TB_BST.Text = bst.ToString("000");
        TB_BST.BackColor = ColorUtil.ColorBaseStatTotal(bst);
    }

    public void UpdateRandomIVs(object sender, EventArgs e)
    {
        Span<int> ivs = stackalloc int[6];
        if (ModifierKeys == Keys.Control)
        {
            ivs.Fill(Entity.MaxIV);
        }
        else if (ModifierKeys == Keys.Alt)
        {
            ivs.Clear();
        }
        else
        {
            var pk = Entity;
            var la = new LegalityAnalysis(pk);
            var enc = la.EncounterMatch;
            if (enc is IFlawlessIVCount { FlawlessIVCount: not 0 } fc)
                pk.SetRandomIVs(ivs, fc.FlawlessIVCount);
            else if (enc is IFixedIVSet { IVs: { IsSpecified: true } iv })
                pk.SetRandomIVs(ivs, iv);
            else if (enc is IFlawlessIVCountConditional c && c.GetFlawlessIVCount(pk) is { Max: not 0 } x)
                pk.SetRandomIVs(ivs, Util.Rand.Next(x.Min, x.Max + 1));
            else
                pk.SetRandomIVs(ivs);
        }

        LoadIVs(ivs);
        if (Entity is IGanbaru g)
        {
            Entity.SetIVs(ivs);
            if (ModifierKeys == Keys.Control)
                g.SetSuggestedGanbaruValues(Entity);
            else if (ModifierKeys == Keys.Alt)
                g.ClearGanbaruValues();
            LoadGVs(g);
        }
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
                a.AwakeningMinimum(); // will still set AVs by level gain
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

    public string UpdateNatureModification(Nature nature)
    {
        // Reset Label Colors
        foreach (var l in L_Stats)
            l.ResetForeColor();

        // Set Colored StatLabels only if Nature isn't Neutral
        var (up, dn) = NatureAmp.GetNatureModification(nature);
        if (NatureAmp.IsNeutralOrInvalid(nature, up, dn))
            return "-/-";

        var incr = L_Stats[up + 1];
        var decr = L_Stats[dn + 1];
        incr.ForeColor = StatIncreased;
        decr.ForeColor = StatDecreased;
        return $"+{incr.Text} / -{decr.Text}".Replace(":", "");
    }

    public void SetATKIVGender(byte gender)
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
        if (!HaX)
            pk.Stat_Level = pk.CurrentLevel;
    }

    public void LoadEVs(ReadOnlySpan<int> EVs)
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

    public void LoadIVs(ReadOnlySpan<int> IVs)
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

    public void LoadGVs(IGanbaru a)
    {
        ChangingFields = true;
        TB_GVHP.Text = a.GV_HP.ToString();
        TB_GVATK.Text = a.GV_ATK.ToString();
        TB_GVDEF.Text = a.GV_DEF.ToString();
        TB_GVSPE.Text = a.GV_SPE.ToString();
        TB_GVSPA.Text = a.GV_SPA.ToString();
        TB_GVSPD.Text = a.GV_SPD.ToString();
        ChangingFields = false;
        for (int i = 0; i < 6; i++)
            RefreshGanbaru(Entity, a, i);
        UpdateStats();
    }

    private void L_DynamaxLevel_Click(object sender, EventArgs e)
    {
        var cb = CB_DynamaxLevel;
        bool isMin = cb.SelectedIndex == 0;
        cb.SelectedIndex = isMin ? cb.Items.Count - 1 : 0;
    }

    private void RefreshGanbaru(PKM entity, IGanbaru ganbaru, int i)
    {
        int current = ganbaru.GetGV(i);
        var max = ganbaru.GetMax(entity, i);
        var tb = MT_GVs[i];
        if (current > max)
            tb.BackColor = EVsInvalid;
        else if (current == max)
            tb.BackColor = StatHyperTrained;
        else
            tb.ResetBackColor();
    }

    private void SetStatOrder(StatEditorStatOrder order)
    {
        if (order == StatOrder)
            return;

        // https://stackoverflow.com/a/30219698
        // WinForms hack to create the handles and avoid Z-order changing on visibility toggle.
        // Otherwise, our stat ordering may be incorrect if we change it more than once.
        foreach (Control ctrl in FLP_Stats.Controls)
            _ = ctrl.Handle;

        // In Generation 1, Special Defense and Special Attack are combined.
        // Additionally, Speed is shown before Special.
        const int baseIndex = 1;
        if (order == StatEditorStatOrder.Gen1Special)
        {
            FLP_SpD.Visible = Label_SPA.Visible = false;
            Label_SPC.Visible = true;
            FLP_Stats.Controls.SetChildIndex(FLP_Spe, baseIndex + 3); // Speed
        }
        else if (order == StatEditorStatOrder.Current)
        {
            FLP_SpD.Visible = Label_SPA.Visible = true;
            Label_SPC.Visible = false;
            FLP_Stats.Controls.SetChildIndex(FLP_Spe, baseIndex + 5); // Speed
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(order), order, null);
        }

        StatOrder = order;
    }

    public void ToggleInterface(PKM pk, byte format)
    {
        FLP_StatsTotal.Visible = format >= 3;
        FLP_Characteristic.Visible = format >= 3;
        FLP_HPType.Visible = format <= 7 || pk is PB8;
        FLP_TeraType.Visible = FLP_TeraInner.Visible = pk is ITeraType;
        Label_HiddenPowerPower.Visible = format <= 5;
        FLP_DynamaxLevel.Visible = format == 8;
        FLP_AlphaNoble.Visible = pk is PA8;

        SetStatOrder(format == 1 ? StatEditorStatOrder.Gen1Special : StatEditorStatOrder.Current);

        switch (format)
        {
            case 1:
                TB_IVHP.Enabled = false;
                SetEVMaskSize(Stat_HP.Size, "00000", MT_EVs);
                break;
            case 2:
                TB_IVHP.Enabled = false;
                SetEVMaskSize(Stat_HP.Size, "00000", MT_EVs);
                break;
            default:
                TB_IVHP.Enabled = true;
                SetEVMaskSize(TB_EVTotal.Size, "000", MT_EVs);
                break;
        }

        var showAV = pk is IAwakened;
        Label_AVs.Visible = TB_AVTotal.Visible = BTN_RandomAVs.Visible = showAV;
        foreach (var mtb in MT_AVs)
            mtb.Visible = showAV;
        Label_EVs.Visible = TB_EVTotal.Visible = BTN_RandomEVs.Visible = !showAV;
        foreach (var mtb in MT_EVs)
            mtb.Visible = !showAV;

        FLP_PKMEditors.PerformLayout();

        var showGV = pk is IGanbaru;
        Label_GVs.Visible = showGV;
        foreach (var mtb in MT_GVs)
            mtb.Visible = showGV;

        static void SetEVMaskSize(Size s, string Mask, MaskedTextBox[] arr)
        {
            foreach (var ctrl in arr)
            {
                ctrl.Size = s;
                ctrl.Mask = Mask;
            }
        }
    }

    private const string TeraOverrideNone = "---";
    private const byte TeraOverrideNoneValue = TeraTypeUtil.OverrideNone;
    private const byte TeraStellarValue = TeraTypeUtil.Stellar;
    private const byte TeraDisplayIndex = TeraTypeUtil.StellarTypeDisplayStringIndex;

    private void L_TeraTypeOriginal_Click(object sender, EventArgs e)
    {
        var pi = Entity.PersonalInfo;
        if (!Entity.SV)
        {
            var expect = TeraTypeUtil.GetTeraTypeImport(pi.Type1, pi.Type2);
            SetOriginalTeraType((byte)expect);
            return;
        }
        var current = WinFormsUtil.GetIndex(CB_TeraTypeOriginal);
        var update = pi.Type1 == current ? pi.Type2 : pi.Type1;
        SetOriginalTeraType(update);
    }

    private void SetOriginalTeraType(byte value)
    {
        CB_TeraTypeOriginal.SelectedValue = (int)value;
        CB_TeraTypeOverride.SelectedValue = (int)TeraOverrideNoneValue;
    }

    private void PB_TeraType1_Click(object sender, EventArgs e) => SetOriginalTeraType(Entity.PersonalInfo.Type1);
    private void PB_TeraType2_Click(object sender, EventArgs e) => SetOriginalTeraType(Entity.PersonalInfo.Type2);

    public void InitializeDataSources()
    {
        ChangingFields = true;

        CB_HPType.InitializeBinding();
        CB_TeraTypeOriginal.InitializeBinding();
        CB_TeraTypeOverride.InitializeBinding();

        var types = GameInfo.Strings.types.AsSpan();
        CB_HPType.DataSource = Util.GetCBList(types.Slice(1, HiddenPower.TypeCount));

        var tera = Util.GetCBList(types[..TeraDisplayIndex]);
        tera.Insert(0, new(TeraOverrideNone, TeraOverrideNoneValue));
        tera.Add(new(types[TeraDisplayIndex], TeraStellarValue));
        CB_TeraTypeOriginal.DataSource = new BindingSource(tera, null);
        CB_TeraTypeOverride.DataSource = new BindingSource(tera, null);

        ChangingFields = false;
    }

    private void CHK_Gigantamax_CheckedChanged(object sender, EventArgs e)
    {
        if (!ChangingFields)
            MainEditor.UpdateSprite();
    }

    private void CHK_IsAlpha_CheckedChanged(object sender, EventArgs e)
    {
        if (!ChangingFields)
            MainEditor.UpdateSprite();
    }

    private void L_TeraTypeOverride_Click(object sender, EventArgs e) => CB_TeraTypeOverride.SelectedValue = Entity.SV ? (int)TeraOverrideNoneValue : CB_TeraTypeOriginal.SelectedValue;

    private void ChangeTeraType(object sender, EventArgs e)
    {
        if (ChangingFields && sender == CB_TeraTypeOriginal)
            return;

        var original = (byte)WinFormsUtil.GetIndex(CB_TeraTypeOriginal);
        var update = (byte)WinFormsUtil.GetIndex(CB_TeraTypeOverride);
        if (!ChangingFields && Entity is ITeraType t) // Store back
        {
            if (sender == CB_TeraTypeOriginal)
                t.TeraTypeOriginal = (MoveType)original;
            else if (sender == CB_TeraTypeOverride)
                t.TeraTypeOverride = (MoveType)update;
        }

        var type = update;
        if (type == TeraOverrideNoneValue)
            type = original;
        PB_TeraType.Image = TypeSpriteUtil.GetTypeSpriteGem(type);
        if (!ChangingFields)
            MainEditor.UpdateSprite();
    }

    public void CenterSubEditors()
    {
        FLP_PKMEditors.HorizontallyCenter(this);
    }
}

public sealed class TypePictureBox : PictureBox
{
    private byte Type;

    public void SetType(byte type, bool tera) => BackColor = tera
        ? TypeColor.GetTeraSpriteColor(Type = type)
        : TypeColor.GetTypeSpriteColor(Type = type);

    private readonly ToolTip Tip = new() { InitialDelay = 500, ReshowDelay = 500, ShowAlways = true };

    // Show a tooltip when hovered.
    protected override void OnMouseHover(EventArgs e)
    {
        base.OnMouseHover(e);
        var index = Type;
        if (index == TeraTypeUtil.Stellar)
            index = TeraTypeUtil.StellarTypeDisplayStringIndex;
        var name = GameInfo.Strings.types[index];
        Tip.SetToolTip(this, name);
    }
}

/// <summary>
/// Stat display order for a stat editor.
/// </summary>
public enum StatEditorStatOrder
{
    /// <summary>
    /// Stat order for everything after Generation 1.
    /// </summary>
    /// <remarks>
    /// Default load state for a GUI.
    /// </remarks>
    Current = 0,

    /// <summary>
    /// Stat order for Generation 1; Speed before Special.
    /// </summary>
    Gen1Special,
}
