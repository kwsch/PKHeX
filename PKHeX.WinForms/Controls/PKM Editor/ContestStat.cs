using System;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms.Controls
{
    public partial class ContestStat : UserControl, IContestStats
    {
        public ContestStat()
        {
            InitializeComponent();
        }

        public int CNT_Sheen
        {
            get => Util.ToInt32(TB_Sheen.Text);
            set => TB_Sheen.Text = value.ToString();
        }

        public int CNT_Cool
        {
            get => Util.ToInt32(TB_Cool.Text);
            set => TB_Cool.Text = value.ToString();
        }

        public int CNT_Beauty
        {
            get => Util.ToInt32(TB_Beauty.Text);
            set => TB_Beauty.Text = value.ToString();
        }

        public int CNT_Cute
        {
            get => Util.ToInt32(TB_Cute.Text);
            set => TB_Cute.Text = value.ToString();
        }

        public int CNT_Smart
        {
            get => Util.ToInt32(TB_Smart.Text);
            set => TB_Smart.Text = value.ToString();
        }

        public int CNT_Tough
        {
            get => Util.ToInt32(TB_Tough.Text);
            set => TB_Tough.Text = value.ToString();
        }

        private void Update255_MTB(object sender, EventArgs e)
        {
            if (!(sender is MaskedTextBox tb)) return;
            if (Util.ToInt32(tb.Text) > byte.MaxValue)
                tb.Text = "255";
        }

        public void ToggleInterface(object o, int gen = PKX.Generation)
        {
            if (!(o is IContestStats))
            {
                Visible = false;
                return;
            }

            Visible = true;
            bool smart = gen < 6;
            Label_Smart.Visible = smart; // show smart gen3-5
            Label_Clever.Visible = !smart; // show clever gen6+
        }
    }
}
