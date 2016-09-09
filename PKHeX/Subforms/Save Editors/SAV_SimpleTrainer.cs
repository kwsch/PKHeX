using System;
using System.Linq;
using System.Windows.Forms;

namespace PKHeX
{
    public partial class SAV_SimpleTrainer : Form
    {
        public SAV_SimpleTrainer()
        {
            InitializeComponent();
            Util.TranslateInterface(this, Main.curlanguage);

            cba = new[] {CHK_1, CHK_2, CHK_3, CHK_4, CHK_5, CHK_6, CHK_7, CHK_8};
            TB_OTName.MaxLength = SAV.OTLength;
            B_MaxCash.Click += (sender, e) => MT_Money.Text = SAV.MaxMoney.ToString();
            MT_Money.Mask = "00000000000".Substring(0, SAV.MaxMoney.ToString().Length);
            B_MaxCoins.Click += (sender, e) => MT_Coins.Text = "9999";

            CB_Gender.Items.Clear();
            CB_Gender.Items.AddRange(Main.gendersymbols.Take(2).ToArray()); // m/f depending on unicode selection

            L_SID.Visible = MT_SID.Visible = SAV.Generation > 2;
            L_Coins.Visible = B_MaxCoins.Visible = MT_Coins.Visible = SAV.Generation < 3;
            CB_Gender.Visible = SAV.Generation > 1;

            TB_OTName.Text = SAV.OT;
            CB_Gender.SelectedIndex = SAV.Gender;
            MT_TID.Text = SAV.TID.ToString("00000");
            MT_SID.Text = SAV.SID.ToString("00000");
            MT_Money.Text = SAV.Money.ToString();
            MT_Hours.Text = SAV.PlayedHours.ToString();
            MT_Minutes.Text = SAV.PlayedMinutes.ToString();
            MT_Seconds.Text = SAV.PlayedSeconds.ToString();


            int badgeval = 0;
            if (SAV is SAV1)
            {
                SAV1 sav1 = (SAV1)SAV;
                MT_Coins.Text = sav1.Coin.ToString();
                badgeval = sav1.Badges;

                L_Started.Visible = L_Fame.Visible = false;
                CAL_AdventureStartDate.Visible = CAL_HoFDate.Visible = false;
                CAL_AdventureStartTime.Visible = CAL_HoFTime.Visible = false;
                GB_Map.Visible = false;
                GB_Options.Visible = true;
                CB_BattleStyle.Items.AddRange(new[] { "Switch", "Set" });
                CB_SoundType.Items.AddRange(new[] { "Mono", "Stereo", "Earphone2", "Earphone3" });
                CB_TextSpeed.Items.AddRange(new[] { "0 (Instant)", "1 (Fast)", "2", "3 (Normal)", "4", "5 (Slow)", "6", "7" });

                CHK_BattleEffects.Checked = sav1.BattleEffects;
                CB_BattleStyle.SelectedIndex = sav1.BattleStyleSwitch ? 0 : 1;
                CB_SoundType.SelectedIndex = sav1.Sound;
                CB_TextSpeed.SelectedIndex = sav1.TextSpeed;
            }

            if (SAV is SAV2)
            {
                SAV2 sav2 = (SAV2)SAV;
                MT_Coins.Text = sav2.Coin.ToString();

                L_Started.Visible = L_Fame.Visible = false;
                CAL_AdventureStartDate.Visible = CAL_HoFDate.Visible = false;
                CAL_AdventureStartTime.Visible = CAL_HoFTime.Visible = false;
                GB_Map.Visible = false;
                GB_Options.Visible = true;
                CB_BattleStyle.Items.AddRange(new[] { "Switch", "Set" });
                CB_SoundType.Items.AddRange(new[] { "Mono", "Stereo" });
                CB_TextSpeed.Items.AddRange(new[] { "0 (Instant)", "1 (Fast)", "2", "3 (Normal)", "4", "5 (Slow)", "6", "7" });

                CHK_BattleEffects.Checked = sav2.BattleEffects;
                CB_BattleStyle.SelectedIndex = sav2.BattleStyleSwitch ? 0 : 1;
                CB_SoundType.SelectedIndex = sav2.Sound > 0 ? 1 : 0;
                CB_TextSpeed.SelectedIndex = sav2.TextSpeed;
                badgeval = sav2.Badges;
                cba = new[] { CHK_1, CHK_2, CHK_3, CHK_4, CHK_6, CHK_5, CHK_7, CHK_8, CHK_H1, CHK_H2, CHK_H3, CHK_H4, CHK_H5, CHK_H6, CHK_H7, CHK_H8 };
            }

            if (SAV is SAV3)
            {
                GB_Map.Visible = false;
                GB_Badges.Visible = false; // todo

                L_Started.Visible = L_Fame.Visible = false;
                CAL_AdventureStartDate.Visible = CAL_HoFDate.Visible = false;
                CAL_AdventureStartTime.Visible = CAL_HoFTime.Visible = false;
                return;
            }

            if (SAV is SAV4)
            {
                SAV4 s = (SAV4)SAV;
                NUD_M.Value = s.M;
                NUD_X.Value = s.X;
                NUD_Z.Value = s.Z;
                NUD_Y.Value = s.Y;

                badgeval = s.Badges;
                if (s.Version == GameVersion.HGSS)
                {
                    badgeval |= s.Badges16 << 8;
                    cba = cba.Concat(new[] {CHK_H1, CHK_H2, CHK_H3, CHK_H4, CHK_H5, CHK_H6, CHK_H7, CHK_H8}).ToArray();
                }
            }
            else if (SAV is SAV5)
            {
                SAV5 s = (SAV5)SAV;
                NUD_M.Value = s.M;
                NUD_X.Value = s.X;
                NUD_Z.Value = s.Z;
                NUD_Y.Value = s.Y;

                badgeval = s.Badges;
            }

            for (int i = 0; i < cba.Length; i++)
            {
                cba[i].Visible = true;
                cba[i].Checked = (badgeval & 1 << i) != 0;
            }

            CAL_HoFDate.Value = new DateTime(2000, 1, 1).AddSeconds(SAV.SecondsToFame);
            CAL_HoFTime.Value = new DateTime(2000, 1, 1).AddSeconds(SAV.SecondsToFame % 86400);
            CAL_AdventureStartDate.Value = new DateTime(2000, 1, 1).AddSeconds(SAV.SecondsToStart);
            CAL_AdventureStartTime.Value = new DateTime(2000, 1, 1).AddSeconds(SAV.SecondsToStart % 86400);
        }
        private readonly CheckBox[] cba;
        private readonly SaveFile SAV = Main.SAV.Clone();
        
        private void changeFFFF(object sender, EventArgs e)
        {
            MaskedTextBox box = sender as MaskedTextBox;
            if (box.Text == "") box.Text = "0";
            if (Util.ToInt32(box.Text) > 65535) box.Text = "65535";
        }

        private void B_Save_Click(object sender, EventArgs e)
        {
            SAV.OT = TB_OTName.Text;
            SAV.Gender = (byte)CB_Gender.SelectedIndex;

            SAV.TID = (ushort)Util.ToUInt32(MT_TID.Text);
            SAV.SID = (ushort)Util.ToUInt32(MT_SID.Text);
            SAV.Money = Util.ToUInt32(MT_Money.Text);

            SAV.PlayedHours = ushort.Parse(MT_Hours.Text);
            SAV.PlayedMinutes = ushort.Parse(MT_Minutes.Text) % 60;
            SAV.PlayedSeconds = ushort.Parse(MT_Seconds.Text) % 60;

            // Copy Badges
            int badgeval = 0;
            for (int i = 0; i < cba.Length; i++)
                badgeval |= (cba[i].Checked ? 1 : 0) << i;

            if (SAV is SAV1)
            {
                SAV1 sav1 = (SAV1) SAV;
                sav1.Coin = (ushort) Util.ToUInt32(MT_Coins.Text);
                sav1.Badges = badgeval & 0xFF;

                sav1.BattleEffects = CHK_BattleEffects.Checked;
                sav1.BattleStyleSwitch = CB_BattleStyle.SelectedIndex == 0;
                sav1.Sound = CB_SoundType.SelectedIndex;
                sav1.TextSpeed = CB_TextSpeed.SelectedIndex;
            }

            if (SAV is SAV2)
            {
                SAV2 sav2 = (SAV2)SAV;
                sav2.Coin = (ushort)Util.ToUInt32(MT_Coins.Text);
                sav2.Badges = badgeval & 0xFFFF;

                sav2.BattleEffects = CHK_BattleEffects.Checked;
                sav2.BattleStyleSwitch = CB_BattleStyle.SelectedIndex == 0;
                sav2.Sound = CB_SoundType.SelectedIndex > 0 ? 2 : 0;
                sav2.TextSpeed = CB_TextSpeed.SelectedIndex;
            }

            if (SAV is SAV4)
            {
                SAV4 s = (SAV4)SAV;
                s.M = (int)NUD_M.Value;
                s.X = (int)NUD_X.Value;
                s.Z = (int)NUD_Z.Value;
                s.Y = (int)NUD_Y.Value;
                s.Badges = badgeval & 0xFF;
                if (s.Version == GameVersion.HGSS)
                {
                    s.Badges16 = badgeval >> 8;
                }
            }
            else if (SAV is SAV5)
            {
                SAV5 s = (SAV5)SAV;
                s.M = (int)NUD_M.Value;
                s.X = (int)NUD_X.Value;
                s.Z = (int)NUD_Z.Value;
                s.Y = (int)NUD_Y.Value;
                s.Badges = badgeval & 0xFF;
            }
            
            SAV.SecondsToStart = getSeconds(CAL_AdventureStartDate, CAL_AdventureStartTime);
            SAV.SecondsToFame = getSeconds(CAL_HoFDate, CAL_HoFTime);
            
            SAV.Data.CopyTo(Main.SAV.Data, 0);
            Main.SAV.Edited = true;
            Close();
        }
        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        private int getSeconds(DateTimePicker date, DateTimePicker time)
        {
            int val = (int)(date.Value - new DateTime(2000, 1, 1)).TotalSeconds;
            val -= val % 86400;
            val += (int)(time.Value - new DateTime(2000, 1, 1)).TotalSeconds;
            return val;
        }
    }
}
