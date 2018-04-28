using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms.Controls
{
    public partial class TrainerID : UserControl
    {
        public TrainerID() => InitializeComponent();
        public event EventHandler UpdatedID;

        private int Format = -1;
        private ITrainerID Trainer;
        private readonly ToolTip TSVTooltip = new ToolTip();

        public void UpdateTSV()
        {
            if (!(Trainer is PKM pkm))
                return;
            string IDstr = $"TSV: {pkm.TSV:d4}";
            TSVTooltip.SetToolTip(TB_TID, IDstr);
            TSVTooltip.SetToolTip(TB_SID, IDstr);
            TSVTooltip.SetToolTip(TB_TID7, IDstr);
            TSVTooltip.SetToolTip(TB_SID7, IDstr);
        }
        public void LoadIDValues(ITrainerID tr)
        {
            Trainer = tr;
            int format = tr is PKM p ? p.GenNumber : tr is SaveFile s ? s.Generation : -1;
            SetFormat(format);
            LoadValues();
        }
        public void UpdateSID() => LoadValues();
        public void LoadInfo(ITrainerInfo info)
        {
            Trainer.TID = info.TID;
            Trainer.SID = info.SID;
            LoadValues();
        }

        private void LoadValues()
        {
            if (Format <= 2)
                TB_TID.Text = Trainer.TID.ToString();
            else if (Format <= 6)
                LoadTID(Trainer.TID, Trainer.SID);
            else
                LoadTID7(Trainer.TID, Trainer.SID);
        }
        private void LoadTID(int tid, int sid)
        {
            TB_TID.Text = tid.ToString("D5");
            TB_SID.Text = sid.ToString("D5");
        }
        private void LoadTID7(int tid, int sid)
        {
            var repack = (uint)((sid << 16) | tid);
            sid = (int)(repack / 1_000_000);
            tid = (int)(repack % 1_000_000);

            TB_TID7.Text = tid.ToString("D6");
            TB_SID7.Text = sid.ToString("D4");
        }
        private void SetFormat(int format)
        {
            if (format == Format)
                return;

            var controls = GetControlsForFormat(format);
            FLP.Controls.Clear(); int i = 0;
            foreach (var c in controls)
            {
                FLP.Controls.Add(c);
                FLP.Controls.SetChildIndex(c, i++); // because you don't listen the first time
            }

            Format = format;
        }
        private IEnumerable<Control> GetControlsForFormat(int format)
        {
            if (format >= 7)
                return new Control[] { Label_SID, TB_SID7, Label_TID, TB_TID7 };
            if (format >= 3)
                return new Control[] { Label_TID, TB_TID, Label_SID, TB_SID };
            return new Control[] { Label_TID, TB_TID };
        }
        private void UpdateTSV(object sender, EventArgs e) => UpdateTSV();
        private void Update_ID(object sender, EventArgs e)
        {
            if (!(sender is MaskedTextBox mt))
                return;

            var val = int.Parse(mt.Text);
            if (mt == TB_TID7)
            {
                if (val > 999_999)
                {
                    mt.Text = "999999";
                    return;
                }
                SanityCheckSID7(val, int.Parse(TB_SID7.Text));
            }
            else if (mt == TB_SID7)
            {
                if (val > 4294) // max 4 digits of 32bit int
                {
                    mt.Text = "4294";
                    return;
                }
                SanityCheckSID7(int.Parse(TB_TID7.Text), val);
            }
            else
            {
                if (val > ushort.MaxValue) // prior to gen7
                    mt.Text = (val = ushort.MaxValue).ToString();

                if (mt == TB_TID)
                    Trainer.TID = val;
                else
                    Trainer.SID = val;
            }

            UpdatedID?.Invoke(sender, e);
        }
        private void SanityCheckSID7(int tid, int sid)
        {
            var repack = (long)sid * 1_000_000 + tid;
            if (repack > uint.MaxValue)
            {
                TB_SID7.Text = (sid - 1).ToString();
                return;
            }

            Trainer.SID = (ushort)(repack >> 16);
            Trainer.TID = (ushort)repack;
        }
    }
}
