using System;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SAV_RTC3 : Form
    {
        private readonly SaveFile Origin;
        private readonly IGen3Hoenn SAV;

        public SAV_RTC3(SaveFile sav)
        {
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            SAV = (IGen3Hoenn)(Origin = sav).Clone();

            ClockInitial = SAV.ClockInitial;
            ClockElapsed = SAV.ClockElapsed;
            LoadData();
        }

        private readonly RTC3 ClockInitial;
        private readonly RTC3 ClockElapsed;

        private void LoadData()
        {
            NUD_IDay.Value = ClockInitial.Day;
            NUD_IHour.Value = Math.Min(NUD_IHour.Maximum, ClockInitial.Hour);
            NUD_IMinute.Value = Math.Min(NUD_IMinute.Maximum, ClockInitial.Minute);
            NUD_ISecond.Value = Math.Min(NUD_ISecond.Maximum, ClockInitial.Second);

            NUD_EDay.Value = ClockElapsed.Day;
            NUD_EHour.Value = Math.Min(NUD_EHour.Maximum, ClockElapsed.Hour);
            NUD_EMinute.Value = Math.Min(NUD_EMinute.Maximum, ClockElapsed.Minute);
            NUD_ESecond.Value = Math.Min(NUD_ESecond.Maximum, ClockElapsed.Second);
        }

        private void SaveData()
        {
            ClockInitial.Day = (ushort)NUD_IDay.Value;
            ClockInitial.Hour = (byte)NUD_IHour.Value;
            ClockInitial.Minute = (byte)NUD_IMinute.Value;
            ClockInitial.Second = (byte)NUD_ISecond.Value;

            ClockElapsed.Day = (ushort)NUD_EDay.Value;
            ClockElapsed.Hour = (byte)NUD_EHour.Value;
            ClockElapsed.Minute = (byte)NUD_EMinute.Value;
            ClockElapsed.Second = (byte)NUD_ESecond.Value;
        }

        private void B_Save_Click(object sender, EventArgs e)
        {
            SaveData();

            SAV.ClockInitial = ClockInitial;
            SAV.ClockElapsed = ClockElapsed;

            Origin.CopyChangesFrom((SaveFile)SAV);
            Close();
        }

        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void B_Reset_Click(object sender, EventArgs e)
        {
            NUD_IDay.Value = NUD_IHour.Value = NUD_IMinute.Value = NUD_ISecond.Value = 0;
            NUD_EDay.Value = NUD_EHour.Value = NUD_EMinute.Value = NUD_ESecond.Value = 0;
            System.Media.SystemSounds.Asterisk.Play();
        }

        private void B_BerryFix_Click(object sender, EventArgs e)
        {
            NUD_EDay.Value = Math.Max((2 * 366) + 2, NUD_EDay.Value); // advance
            System.Media.SystemSounds.Asterisk.Play();
        }
    }
}
