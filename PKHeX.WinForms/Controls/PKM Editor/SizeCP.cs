using System;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms.Controls
{
    public partial class SizeCP : UserControl
    {
        private PB7 pkm;
        private bool Loading;

        public SizeCP()
        {
            InitializeComponent();
            Initialized = true;
        }

        private readonly bool Initialized;
        private static readonly string[] SizeClass = {"XS", "S", "", "L", "XL"};

        public void LoadPKM(PKM pk)
        {
            pkm = pk as PB7;
            if (pkm == null)
                return;
            TryResetStats();
        }

        public void TryResetStats()
        {
            if (!Initialized || pkm == null)
                return;
            if (CHK_Auto.Checked)
                pkm.ResetCalculatedValues();
            LoadStoredValues();
        }

        private void LoadStoredValues()
        {
            Loading = true;
            MT_CP.Text = Math.Min(65535, pkm.Stat_CP).ToString();

            NUD_HeightScalar.Value = pkm.HeightScalar;
            TB_HeightAbs.Text = pkm.HeightAbsolute.ToString();

            NUD_WeightScalar.Value = pkm.WeightScalar;
            TB_WeightAbs.Text = pkm.WeightAbsolute.ToString();
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
            pkm.HeightScalar = (byte) NUD_HeightScalar.Value;
            L_SizeH.Text = SizeClass[PB7.GetSizeRating(pkm.HeightScalar)];

            if (!CHK_Auto.Checked || Loading)
                return;
            pkm.ResetHeight();
            TB_HeightAbs.Text = pkm.HeightAbsolute.ToString();
        }

        private void NUD_WeightScalar_ValueChanged(object sender, EventArgs e)
        {
            pkm.WeightScalar = (byte) NUD_WeightScalar.Value;
            L_SizeW.Text = SizeClass[PB7.GetSizeRating(pkm.WeightScalar)];

            if (!CHK_Auto.Checked || Loading)
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
    }
}
