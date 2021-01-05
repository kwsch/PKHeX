using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms.Controls
{
    public partial class TrainerID : UserControl
    {
        public TrainerID() => InitializeComponent();
        public event EventHandler? UpdatedID;

        private int Format = -1;
        private ITrainerID Trainer = null!;

        public void UpdateTSV()
        {
            var tsv = GetTSV();
            if (tsv < 0)
                return;

            string IDstr = $"TSV: {tsv:d4}";
            var repack = (Trainer.SID * 1_000_000) + Trainer.TID;

            string supplement = Format < 7
                ? $"G7ID: ({repack / 1_000_000:D4}){repack % 1_000_000:D6}"
                : $"ID: {Trainer.TID:D5}/{Trainer.SID:D5}";

            IDstr += Environment.NewLine + supplement;
            TSVTooltip.SetToolTip(TB_TID, IDstr);
            TSVTooltip.SetToolTip(TB_SID, IDstr);
            TSVTooltip.SetToolTip(TB_TID7, IDstr);
            TSVTooltip.SetToolTip(TB_SID7, IDstr);
        }

        private int GetTSV()
        {
            if (Format <= 2)
                return -1;
            var xor = Trainer.SID ^ Trainer.TID;
            if (Format <= 5)
                return xor >> 3;
            return xor >> 4;
        }

        public void LoadIDValues(ITrainerID tr)
        {
            Trainer = tr;
            int format = tr.GetTrainerIDFormat();
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

        private IEnumerable<Control> GetControlsForFormat(int format) => format switch
        {
            >= 7 => new Control[] {Label_SID, TB_SID7, Label_TID, TB_TID7},
            >= 3 => new Control[] {Label_TID, TB_TID,  Label_SID, TB_SID },
               _ => new Control[] {Label_TID, TB_TID} // Gen1/2
        };

        private void UpdateTSV(object sender, EventArgs e) => UpdateTSV();

        private void Update_ID(object sender, EventArgs e)
        {
            if (sender is not MaskedTextBox mt)
                return;

            if (!int.TryParse(mt.Text, out var val))
                val = 0;
            if (mt == TB_TID7)
            {
                if (val > 999_999)
                {
                    mt.Text = "999999";
                    return;
                }
                if (!int.TryParse(TB_SID7.Text, out var sid))
                    sid = 0;
                SanityCheckSID7(val, sid);
            }
            else if (mt == TB_SID7)
            {
                if (val > 4294) // max 4 digits of 32bit int
                {
                    mt.Text = "4294";
                    return;
                }
                if (!int.TryParse(TB_TID7.Text, out var tid))
                    tid = 0;
                SanityCheckSID7(tid, val);
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
            var repack = ((long)sid * 1_000_000) + tid;
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
