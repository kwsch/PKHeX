using System;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SAV_SimpleTrainer : Form
    {
        private readonly SaveFile Origin;
        private readonly SaveFile SAV;

        public SAV_SimpleTrainer(SaveFile sav)
        {
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            SAV = (Origin = sav).Clone();
            Loading = true;

            cba = new[] {CHK_1, CHK_2, CHK_3, CHK_4, CHK_5, CHK_6, CHK_7, CHK_8};
            TB_OTName.MaxLength = SAV.OTLength;
            B_MaxCash.Click += (sender, e) => MT_Money.Text = SAV.MaxMoney.ToString();
            B_MaxCoins.Click += (sender, e) => MT_Coins.Text = SAV.MaxCoins.ToString();
            MT_Money.Mask = "00000000000".Substring(0, SAV.MaxMoney.ToString().Length);
            MT_Coins.Mask = "00000000000".Substring(0, SAV.MaxCoins.ToString().Length);

            CB_Gender.Items.Clear();
            CB_Gender.Items.AddRange(Main.GenderSymbols.Take(2).ToArray()); // m/f depending on unicode selection

            L_SID.Visible = MT_SID.Visible = SAV.Generation > 2;
            L_Coins.Visible = B_MaxCoins.Visible = MT_Coins.Visible = SAV.Generation < 3;
            CB_Gender.Visible = SAV.Generation > 1;

            L_PikaFriend.Visible = MT_PikaFriend.Visible = L_PikaBeach.Visible = MT_PikaBeach.Visible = SAV.Generation == 1;

            TB_OTName.Text = SAV.OT;
            CB_Gender.SelectedIndex = SAV.Gender;
            MT_TID.Text = SAV.TID.ToString("00000");
            MT_SID.Text = SAV.SID.ToString("00000");
            MT_Money.Text = SAV.Money.ToString();
            MT_Hours.Text = SAV.PlayedHours.ToString();
            MT_Minutes.Text = SAV.PlayedMinutes.ToString();
            MT_Seconds.Text = SAV.PlayedSeconds.ToString();

            int badgeval = 0;
            if (SAV is SAV1 sav1)
            {
                MT_Coins.Text = sav1.Coin.ToString();
                badgeval = sav1.Badges;

                L_Started.Visible = L_Fame.Visible = false;
                CAL_AdventureStartDate.Visible = CAL_HoFDate.Visible = false;
                CAL_AdventureStartTime.Visible = CAL_HoFTime.Visible = false;
                GB_Map.Visible = false;
                GB_Options.Visible = true;
                CB_BattleStyle.Items.AddRange(new[] { "Switch", "Set" });
                CB_SoundType.Items.AddRange(new[] { "Mono", "Stereo", "Left", "Right" });
                CB_TextSpeed.Items.AddRange(new[] { "0 (Instant)", "1 (Fast)", "2", "3 (Normal)", "4", "5 (Slow)", "6", "7" });

                CHK_BattleEffects.Checked = sav1.BattleEffects;
                CB_BattleStyle.SelectedIndex = sav1.BattleStyleSwitch ? 0 : 1;
                CB_SoundType.SelectedIndex = sav1.Sound;
                CB_TextSpeed.SelectedIndex = sav1.TextSpeed;

                MT_PikaFriend.Text = sav1.PikaFriendship.ToString();
                MT_PikaBeach.Text = sav1.PikaBeachScore.ToString();
                if (!sav1.Version.Contains(GameVersion.YW))
                {
                    L_PikaFriend.Visible = MT_PikaFriend.Visible = false;
                    L_PikaBeach.Visible = MT_PikaBeach.Visible = false;
                    CB_SoundType.Visible = LBL_SoundType.Visible = false;
                }
            }

            if (SAV is SAV2 sav2)
            {
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

            if (SAV is SAV3 sav3)
            {
                GB_Map.Visible = false;
                badgeval = sav3.Badges;

                L_Started.Visible = L_Fame.Visible = false;
                CAL_AdventureStartDate.Visible = CAL_HoFDate.Visible = false;
                CAL_AdventureStartTime.Visible = CAL_HoFTime.Visible = false;
            }
            if (SAV is SAV3Colosseum or SAV3XD)
            {
                GB_Map.Visible = false;
                GB_Badges.Visible = false;
                L_Started.Visible = L_Fame.Visible = false;
                CAL_AdventureStartDate.Visible = CAL_HoFDate.Visible = false;
                CAL_AdventureStartTime.Visible = CAL_HoFTime.Visible = false;
                GB_Adventure.Visible = false;
                return;
            }

            if (SAV is SAV4 sav4)
            {
                NUD_M.Value = sav4.M;
                NUD_X.Value = sav4.X;
                NUD_Z.Value = sav4.Z;
                NUD_Y.Value = sav4.Y;

                badgeval = sav4.Badges;
                if (sav4 is SAV4HGSS hgss)
                {
                    badgeval |= hgss.Badges16 << 8;
                    cba = cba.Concat(new[] { CHK_H1, CHK_H2, CHK_H3, CHK_H4, CHK_H5, CHK_H6, CHK_H7, CHK_H8 }).ToArray();
                }
            }
            else if (SAV is SAV5 s)
            {
                foreach (var control in new Control[] {L_Coins, B_MaxCoins, MT_Coins})
                {
                    var pos = control.Location;
                    GB_Badges.Controls.Add(control);
                    control.Location = pos;
                    control.Visible = true;
                }
                L_Coins.Text = "BP"; // no translation boo
                MT_Coins.Text = s.BattleSubway.BP.ToString();

                var pd = s.PlayerData;
                NUD_M.Value = pd.M;
                NUD_X.Value = pd.X;
                NUD_Z.Value = pd.Z;
                NUD_Y.Value = pd.Y;
                badgeval = s.Misc.Badges;
            }

            for (int i = 0; i < cba.Length; i++)
            {
                cba[i].Visible = true;
                cba[i].Checked = (badgeval & 1 << i) != 0;
            }

            Util.GetDateTime2000(SAV.SecondsToStart, out var date, out var time);
            CAL_AdventureStartDate.Value = date;
            CAL_AdventureStartTime.Value = time;

            Util.GetDateTime2000(SAV.SecondsToFame, out date, out time);
            CAL_HoFDate.Value = date;
            CAL_HoFTime.Value = time;

            Loading = false;
        }

        private readonly CheckBox[] cba;
        private readonly bool Loading;
        private bool MapUpdated;

        private void ChangeFFFF(object sender, EventArgs e)
        {
            MaskedTextBox box = (MaskedTextBox)sender;
            if (box.Text.Length == 0) box.Text = "0";
            if (Util.ToInt32(box.Text) > 65535) box.Text = "65535";
        }

        private void Change255(object sender, EventArgs e)
        {
            MaskedTextBox box = (MaskedTextBox)sender;
            if (box.Text.Length == 0) box.Text = "0";
            if (Util.ToInt32(box.Text) > byte.MaxValue) box.Text = "255";
        }

        private void B_Save_Click(object sender, EventArgs e)
        {
            if (SAV.OT != TB_OTName.Text) // only modify if changed (preserve trash bytes?)
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

            if (SAV is SAV1 sav1)
            {
                sav1.Coin = (ushort)Math.Min(Util.ToUInt32(MT_Coins.Text), SAV.MaxCoins);
                sav1.Badges = badgeval & 0xFF;
                sav1.PikaFriendship = (byte)Math.Min(255, Util.ToUInt32(MT_PikaFriend.Text));
                sav1.PikaBeachScore = (ushort)Math.Min(9999, Util.ToUInt32(MT_PikaBeach.Text));
                sav1.BattleEffects = CHK_BattleEffects.Checked;
                sav1.BattleStyleSwitch = CB_BattleStyle.SelectedIndex == 0;
                sav1.Sound = CB_SoundType.SelectedIndex;
                sav1.TextSpeed = CB_TextSpeed.SelectedIndex;
            }

            if (SAV is SAV2 sav2)
            {
                sav2.Coin = (ushort)Math.Min(Util.ToUInt32(MT_Coins.Text), SAV.MaxCoins);
                sav2.Badges = badgeval & 0xFFFF;

                sav2.BattleEffects = CHK_BattleEffects.Checked;
                sav2.BattleStyleSwitch = CB_BattleStyle.SelectedIndex == 0;
                sav2.Sound = CB_SoundType.SelectedIndex > 0 ? 2 : 0;
                sav2.TextSpeed = CB_TextSpeed.SelectedIndex;
            }

            if (SAV is SAV3 sav3)
            {
                sav3.Badges = badgeval & 0xFF;
            }

            if (SAV is SAV4 sav4)
            {
                if (MapUpdated)
                {
                    sav4.M = (int)NUD_M.Value;
                    sav4.X = (int)NUD_X.Value;
                    sav4.Z = (int)NUD_Z.Value;
                    sav4.Y = (int)NUD_Y.Value;
                }
                sav4.Badges = badgeval & 0xFF;
                if (sav4 is SAV4HGSS hgss)
                {
                    hgss.Badges16 = badgeval >> 8;
                }
            }
            else if (SAV is SAV5 s)
            {
                if (MapUpdated)
                {
                    var pd = s.PlayerData;
                    pd.M = (int)NUD_M.Value;
                    pd.X = (int)NUD_X.Value;
                    pd.Z = (int)NUD_Z.Value;
                    pd.Y = (int)NUD_Y.Value;
                }
                s.Misc.Badges = badgeval & 0xFF;
                s.BattleSubway.BP = (ushort)Math.Min(Util.ToUInt32(MT_Coins.Text), SAV.MaxCoins);
            }

            SAV.SecondsToStart = (uint)Util.GetSecondsFrom2000(CAL_AdventureStartDate.Value, CAL_AdventureStartTime.Value);
            SAV.SecondsToFame = (uint)Util.GetSecondsFrom2000(CAL_HoFDate.Value, CAL_HoFTime.Value);

            Origin.CopyChangesFrom(SAV);
            Close();
        }

        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ChangeMapValue(object sender, EventArgs e)
        {
            if (!Loading)
                MapUpdated = true;
        }
    }
}
