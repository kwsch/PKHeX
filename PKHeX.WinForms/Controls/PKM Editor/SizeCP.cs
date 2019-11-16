using System;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms.Controls
{
    public partial class SizeCP : UserControl
    {
        private IScaledSize ss;
        private PB7 pkm;
        private bool Loading;

        public SizeCP()
        {
            InitializeComponent();
            Initialized = true;
        }

        private readonly bool Initialized;
        private static readonly string[] SizeClass = Enum.GetNames(typeof(PokeSize));

        public void LoadPKM(PKM pk)
        {
            pkm = pk as PB7;
            ss = pk as IScaledSize;
            if (ss == null)
                return;
            TryResetStats();
        }

        public void TryResetStats()
        {
            if (!Initialized)
                return;

            if (pkm != null && CHK_Auto.Checked)
                pkm.ResetCalculatedValues();
            LoadStoredValues();
        }

        private void LoadStoredValues()
        {
            Loading = true;
            if (ss != null)
            {
                NUD_HeightScalar.Value = ss.HeightScalar;
                NUD_WeightScalar.Value = ss.WeightScalar;
            }
            if (pkm != null)
            {
                MT_CP.Text = Math.Min(65535, pkm.Stat_CP).ToString();
                TB_HeightAbs.Text = pkm.HeightAbsolute.ToString();
                TB_WeightAbs.Text = pkm.WeightAbsolute.ToString();
            }
            Loading = false;
        }

        private void UpdateFlagState(object sender, EventArgs e)
        {
            if (!CHK_Auto.Checked)
                return;

            pkm.ResetCalculatedValues();
            LoadStoredValues();
        }

        private void MT_CP_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(MT_CP.Text, out var cp))
                pkm.Stat_CP = Math.Min(65535, cp);
        }

        private void NUD_HeightScalar_ValueChanged(object sender, EventArgs e)
        {
            ss.HeightScalar = (byte) NUD_HeightScalar.Value;
            L_SizeH.Text = SizeClass[(int)PokeSizeExtensions.GetSizeRating(ss.HeightScalar)];

            if (!CHK_Auto.Checked || Loading || pkm == null)
                return;
            pkm.ResetHeight();
            TB_HeightAbs.Text = pkm.HeightAbsolute.ToString();
        }

        private void NUD_WeightScalar_ValueChanged(object sender, EventArgs e)
        {
            ss.WeightScalar = (byte) NUD_WeightScalar.Value;
            L_SizeW.Text = SizeClass[(int)PokeSizeExtensions.GetSizeRating(ss.WeightScalar)];

            if (!CHK_Auto.Checked || Loading || pkm == null)
                return;
            pkm.ResetWeight();
            TB_WeightAbs.Text = pkm.WeightAbsolute.ToString("F8");
        }

        private void TB_HeightAbs_TextChanged(object sender, EventArgs e)
        {
            if (CHK_Auto.Checked)
                pkm.ResetHeight();
            else if (float.TryParse(TB_HeightAbs.Text, out var result))
                pkm.HeightAbsolute = result;
        }

        private void TB_WeightAbs_TextChanged(object sender, EventArgs e)
        {
            if (CHK_Auto.Checked)
                pkm.ResetWeight();
            else if (float.TryParse(TB_WeightAbs.Text, out var result))
                pkm.WeightAbsolute = result;
        }

        public void ToggleVisibility(PKM pk)
        {
            var pb7 = pk is PB7;
            FLP_CP.Visible = L_CP.Visible = TB_HeightAbs.Visible = TB_WeightAbs.Visible = pb7;
        }
    }
}
