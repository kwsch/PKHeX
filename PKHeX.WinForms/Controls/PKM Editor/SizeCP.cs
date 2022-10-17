using System;
using System.Globalization;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms.Controls;

public partial class SizeCP : UserControl
{
    private IScaledSize? ss;
    private IScaledSizeValue? sv;
    private ICombatPower? pk;
    private bool Loading;

    public SizeCP()
    {
        InitializeComponent();
        Initialized = true;
    }

    private readonly bool Initialized;
    private static readonly string[] SizeClass = Enum.GetNames(typeof(PokeSize));

    public void LoadPKM(PKM entity)
    {
        pk = entity as ICombatPower;
        ss = entity as IScaledSize;
        sv = entity as IScaledSizeValue;
        if (ss == null)
            return;
        TryResetStats();
    }

    public void TryResetStats()
    {
        if (!Initialized)
            return;

        if (CHK_Auto.Checked)
            ResetCalculatedStats();
        LoadStoredValues();
    }

    private void ResetCalculatedStats()
    {
        sv?.ResetHeight();
        sv?.ResetWeight();
        pk?.ResetCP();
    }

    private static string GetString(float value) => value.ToString("R", CultureInfo.InvariantCulture);

    private void LoadStoredValues()
    {
        Loading = true;
        if (ss != null)
        {
            if (NUD_HeightScalar.Focused || NUD_WeightScalar.Focused)
                CHK_Auto.Focus();
            NUD_HeightScalar.Value = ss.HeightScalar;
            NUD_WeightScalar.Value = ss.WeightScalar;
        }
        if (sv != null)
        {
            TB_HeightAbs.Text = GetString(sv.HeightAbsolute);
            TB_WeightAbs.Text = GetString(sv.WeightAbsolute);
        }
        if (pk != null)
        {
            MT_CP.Text = Math.Min(65535, pk.Stat_CP).ToString();
        }
        Loading = false;
    }

    private void UpdateFlagState(object sender, EventArgs e)
    {
        if (!CHK_Auto.Checked)
            return;

        ResetCalculatedStats();
        LoadStoredValues();
    }

    private void MT_CP_TextChanged(object sender, EventArgs e)
    {
        if (pk != null && int.TryParse(MT_CP.Text, out var cp))
            pk.Stat_CP = Math.Min(65535, cp);
    }

    private void NUD_HeightScalar_ValueChanged(object sender, EventArgs e)
    {
        if (ss != null)
        {
            if (!Loading)
                ss.HeightScalar = (byte) NUD_HeightScalar.Value;
            L_SizeH.Text = SizeClass[(int)PokeSizeUtil.GetSizeRating(ss.HeightScalar)];
        }

        if (!CHK_Auto.Checked || Loading || sv == null)
            return;
        sv.ResetHeight();
        sv.ResetWeight();
        TB_HeightAbs.Text = GetString(sv.HeightAbsolute);
        TB_WeightAbs.Text = GetString(sv.WeightAbsolute);
        if (sv is PA8 a)
            a.HeightScalarCopy = a.HeightScalar;
    }

    private void NUD_WeightScalar_ValueChanged(object sender, EventArgs e)
    {
        if (ss != null)
        {
            if (!Loading)
                ss.WeightScalar = (byte) NUD_WeightScalar.Value;
            L_SizeW.Text = SizeClass[(int)PokeSizeUtil.GetSizeRating(ss.WeightScalar)];
        }

        if (!CHK_Auto.Checked || Loading || sv == null)
            return;
        sv.ResetWeight();
        TB_WeightAbs.Text = GetString(sv.WeightAbsolute);
    }

    private void TB_HeightAbs_TextChanged(object sender, EventArgs e)
    {
        if (sv == null || Loading)
            return;
        if (CHK_Auto.Checked)
            sv.ResetHeight();
        else if (float.TryParse(TB_HeightAbs.Text, out var result))
            sv.HeightAbsolute = result;
    }

    private void TB_WeightAbs_TextChanged(object sender, EventArgs e)
    {
        if (sv == null || Loading)
            return;
        if (CHK_Auto.Checked)
            sv.ResetWeight();
        else if (float.TryParse(TB_WeightAbs.Text, out var result))
            sv.WeightAbsolute = result;
    }

    public void ToggleVisibility(PKM entity)
    {
        bool isCP = entity is ICombatPower;
        bool isAbsolute = entity is IScaledSizeValue;
        MT_CP.Visible = L_CP.Visible = isCP;
        TB_HeightAbs.Visible = TB_WeightAbs.Visible = isAbsolute;
        FLP_CP.Visible = isCP || isAbsolute; // Auto checkbox
    }

    private void ClickScalarEntry(object sender, EventArgs e)
    {
        if (sender is not NumericUpDown nud || ModifierKeys != Keys.Control)
            return;
        nud.Value = PokeSizeUtil.GetRandomScalar();
    }
}
