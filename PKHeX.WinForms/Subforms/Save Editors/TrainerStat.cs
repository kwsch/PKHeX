using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms.Subforms.Save_Editors
{
    public partial class TrainerStat : UserControl
    {
        public TrainerStat()
        {
            InitializeComponent();
            CB_Stats.MouseWheel += (s, e) => ((HandledMouseEventArgs)e).Handled = true; // disallowed
        }

        private bool Editing;
        private ITrainerStatRecord SAV = null!;
        private Dictionary<int, string> RecordList = null!; // index, description
        public Func<int, string?>? GetToolTipText { private get; set; }

        public void LoadRecords(ITrainerStatRecord sav, Dictionary<int, string> records)
        {
            SAV = sav;
            RecordList = records;
            CB_Stats.Items.Clear();
            for (int i = 0; i < sav.RecordCount; i++)
            {
                if (!RecordList.TryGetValue(i, out var name))
                    name = $"{i:D3}";

                CB_Stats.Items.Add(name);
            }
            CB_Stats.SelectedIndex = RecordList.First().Key;
        }

        private void ChangeStat(object sender, EventArgs e)
        {
            Editing = true;
            int index = CB_Stats.SelectedIndex;
            int val = SAV.GetRecord(index);
            NUD_Stat.Maximum = Math.Max(val, SAV.GetRecordMax(index));
            NUD_Stat.Value = val;

            int offset = SAV.GetRecordOffset(index);
            L_Offset.Text = $"Offset: 0x{offset:X3}";
            UpdateTip(index, true);
            Editing = false;
        }

        private void ChangeStatVal(object sender, EventArgs e)
        {
            if (Editing)
                return;
            int index = CB_Stats.SelectedIndex;
            SAV.SetRecord(index, (int)NUD_Stat.Value);
            UpdateTip(index, false);
        }

        private void UpdateTip(int index, bool updateStats)
        {
            if (GetToolTipText != null)
                UpdateToolTipSpecial(index, updateStats);
            else
                UpdateToolTipDefault(index, updateStats);
        }

        private void UpdateToolTipSpecial(int index, bool updateStats)
        {
            var str = GetToolTipText?.Invoke(index);
            if (str != null)
            {
                Tip.SetToolTip(NUD_Stat, str);
                return;
            }
            UpdateToolTipDefault(index, updateStats); // fallback
        }

        private void UpdateToolTipDefault(int index, bool updateStats)
        {
            if (!updateStats || !RecordList.TryGetValue(index, out var tip))
            {
                Tip.RemoveAll();
                return;
            }
            Tip.SetToolTip(CB_Stats, tip);
        }
    }
}
