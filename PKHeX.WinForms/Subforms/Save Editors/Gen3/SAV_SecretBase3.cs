using PKHeX.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PKHeX.WinForms
{
    public partial class SAV_SecretBase3 : Form
    {

        private readonly SaveFile Origin;
        private readonly SAV3 SAV;
        private SecretBaseManager3 manager;

        public SAV_SecretBase3(SAV3 sav)
        {
            InitializeComponent();
            //WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            SAV = (SAV3)(Origin = sav).Clone();

            TB_Name.MaxLength = 7;
            TB_SID.MaxLength = 5;
            TB_TID.MaxLength = 5;
            TB_Entered.MaxLength = 3;
            TB_PID.CharacterCasing = CharacterCasing.Upper;
            TB_PID.MaxLength = 8;

            CB_Species.InitializeBinding();
            CB_Item.InitializeBinding();
            CB_Move1.InitializeBinding();
            CB_Move2.InitializeBinding();
            CB_Move3.InitializeBinding();
            CB_Move4.InitializeBinding();
            CB_Form.InitializeBinding();
            var filtered = GameInfo.FilteredSources;
            var moves = filtered.Moves;
            CB_Move1.DataSource = new BindingSource(moves, null);
            CB_Move2.DataSource = new BindingSource(moves, null);
            CB_Move3.DataSource = new BindingSource(moves, null);
            CB_Move4.DataSource = new BindingSource(moves, null);
            CB_Item.DataSource = new BindingSource(filtered.Items, null);
            CB_Species.DataSource = new BindingSource(filtered.Species, null);
            CB_Form.DataSource = new List<char> { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '!', '?' };

            TB_Entered.KeyPress += (_, e) =>
            {
                e.Handled = !(char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar));
            };
            TB_TID.KeyPress += (_, e) =>
            {
                e.Handled = !(char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar));
            };
            TB_SID.KeyPress += (_, e) =>
            {
                e.Handled = !(char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar));
            };
            TB_TID.TextChanged += (_, _) =>
            {
                if (TB_TID.Text == string.Empty)
                    TB_TID.Text = "0";
                if (UInt32.Parse(TB_TID.Text) > ushort.MaxValue)
                    TB_TID.Text = ushort.MaxValue.ToString();
            };
            TB_SID.TextChanged += (_, _) =>
            {
                if (TB_SID.Text == string.Empty)
                    TB_SID.Text = "0";
                if (UInt32.Parse(TB_SID.Text) > ushort.MaxValue)
                    TB_SID.Text = ushort.MaxValue.ToString();
            };
            TB_Entered.TextChanged += (_, _) =>
            {
                if (TB_Entered.Text == string.Empty)
                    TB_Entered.Text = "0";
                if (UInt32.Parse(TB_Entered.Text) > byte.MaxValue)
                    TB_Entered.Text = byte.MaxValue.ToString();
            };
            TB_PID.TextChanged += (_, _) =>
            {
                if (TB_PID.Text == string.Empty)
                    TB_PID.Text = "0";
                if (!TB_PID.Text.All(c => "0123456789abcdefABCDEF\n".Contains(c)))
                    TB_PID.Text = "FFFFFFFF";
            };

            manager = ((IGen3Hoenn)SAV).SecretBases;
            LB_Bases.InitializeBinding();
            LB_Bases.DataSource = manager.Bases;
            LB_Bases.DisplayMember = "OriginalTrainerName";

            CB_Species.SelectedIndexChanged += (_, _) =>
            {
                if (CB_Species.SelectedIndex <= 0)
                {
                    NUD_Level.Minimum = 0;
                    TB_PID.Text = "0";
                    TB_PID.Enabled = false;
                    CB_Item.SelectedIndex = 0;
                    CB_Item.Enabled = false;
                    CB_Move1.SelectedIndex = 0;
                    CB_Move1.Enabled = false;
                    CB_Move2.SelectedIndex = 0;
                    CB_Move2.Enabled = false;
                    CB_Move3.SelectedIndex = 0;
                    CB_Move3.Enabled = false;
                    CB_Move4.SelectedIndex = 0;
                    CB_Move4.Enabled = false;
                    NUD_Level.Value = 0;
                    NUD_Level.Enabled = false;
                    NUD_EVs.Value = 0;
                    NUD_EVs.Enabled = false;
                }
                else
                {
                    NUD_Level.Minimum = 2;
                    TB_PID.Enabled = true;
                    CB_Item.Enabled = true;
                    CB_Move1.Enabled = true;
                    CB_Move2.Enabled = true;
                    CB_Move3.Enabled = true;
                    CB_Move4.Enabled = true;
                    NUD_Level.Enabled = true;
                    NUD_EVs.Enabled = true;
                }
            };
            NUD_TeamMember.ValueChanged += (_, _) =>
            {
                ShowPKM(((SecretBase3)LB_Bases.SelectedItem).Team.Team[(int)NUD_TeamMember.Value - 1]);
            };
            LB_Bases.SelectedIndexChanged += (_, _) =>
            {
                ShowTrainer();
                //NUD_TeamMember.Maximum = ((SecretBase3)LB_Bases.SelectedItem).Team.Team.Length;
            };

            if (manager.Count > 0)
            {
                LB_Bases.SelectedIndex = 0;
                ShowTrainer();
            }
            else
            {
                B_Save.Enabled = false;
            }

            void ShowTrainer()
            {
                TB_Name.Text = ((SecretBase3)LB_Bases.SelectedItem).OriginalTrainerName;
                T_TrainerGender.Gender = ((SecretBase3)LB_Bases.SelectedItem).OriginalTrainerGender;
                T_TrainerGender.Show();
                TB_TID.Text = ((SecretBase3)LB_Bases.SelectedItem).TID16.ToString();
                TB_SID.Text = ((SecretBase3)LB_Bases.SelectedItem).SID16.ToString();
                TB_Entered.Text = ((SecretBase3)LB_Bases.SelectedItem).TimesEntered.ToString();
                TB_Class.Text = ((SecretBase3)LB_Bases.SelectedItem).OT_TrainerClass;
                CHK_Battled.Checked = ((SecretBase3)LB_Bases.SelectedItem).BattledToday;
                CHK_Registered.Checked = ((SecretBase3)LB_Bases.SelectedItem).RegistryStatus == 1;
                NUD_TeamMember.Value = 1;
                ShowPKM(((SecretBase3)LB_Bases.SelectedItem).Team.Team[(int)NUD_TeamMember.Value - 1]);
            }
        }

        private void ShowPKM(SecretBase3PKM pkm)
        {
            CB_Species.SelectedValue = (int)pkm.Species;
            if (pkm.Species == (int)Core.Species.Unown)
            {
                CB_Form.SelectedIndex = pkm.Form - 1;
                CB_Form.Visible = true;
            }
            else
            {
                CB_Form.Visible = false;
            }
            TB_PID.Text = pkm.PID.ToString("X8");
            CB_Item.SelectedValue = (int)pkm.HeldItem;
            CB_Move1.SelectedValue = (int)pkm.Move1;
            CB_Move2.SelectedValue = (int)pkm.Move2;
            CB_Move3.SelectedValue = (int)pkm.Move3;
            CB_Move4.SelectedValue = (int)pkm.Move4;
            NUD_Level.Value = pkm.Level;
            NUD_EVs.Value = pkm.EVAll;
        }

        private void B_UpdateTrainer_Click(object sender, EventArgs e)
        {
            var Trainer = (SecretBase3)LB_Bases.SelectedItem;
            Trainer.OriginalTrainerName = TB_Name.Text;
            Trainer.OriginalTrainerGender = T_TrainerGender.Gender;
            Trainer.TID16 = ushort.Parse(TB_TID.Text);
            Trainer.SID16 = ushort.Parse(TB_SID.Text);
            Trainer.TimesEntered = byte.Parse(TB_Entered.Text);
            Trainer.BattledToday = CHK_Battled.Checked;
            Trainer.RegistryStatus = CHK_Registered.Checked ? 1 : 0;
            LB_Bases.DisplayMember = null;
            LB_Bases.DisplayMember = "OriginalTrainerName";
            System.Media.SystemSounds.Asterisk.Play();
        }

        private void B_UpdatePKM_Click(object sender, EventArgs e)
        {
            var pkmteam = ((SecretBase3)LB_Bases.SelectedItem).Team;
            var pkm = pkmteam.Team[(int)NUD_TeamMember.Value - 1];
            pkm.Species = (ushort)WinFormsUtil.GetIndex(CB_Species);
            pkm.PID = Util.GetHexValue(TB_PID.Text);
            pkm.HeldItem = (ushort)WinFormsUtil.GetIndex(CB_Item);
            pkm.Move1 = (ushort)WinFormsUtil.GetIndex(CB_Move1);
            pkm.Move2 = (ushort)WinFormsUtil.GetIndex(CB_Move2);
            pkm.Move3 = (ushort)WinFormsUtil.GetIndex(CB_Move3);
            pkm.Move4 = (ushort)WinFormsUtil.GetIndex(CB_Move4);
            pkm.Level = Convert.ToByte(NUD_Level.Value);
            pkm.EVAll = Convert.ToByte(NUD_EVs.Value);
            ((SecretBase3)LB_Bases.SelectedItem).Team = pkmteam;
            System.Media.SystemSounds.Asterisk.Play();
        }

        private void B_Cancel_Click(object sender, EventArgs e) => Close();

        private void B_Save_Click(object sender, EventArgs e)
        {
            List<SecretBase3> bases = LB_Bases.Items.Cast<SecretBase3>().ToList();
            manager.Bases = bases;
            ((IGen3Hoenn)SAV).SecretBases = manager;
            Origin.CopyChangesFrom(SAV);
            Close();
        }
    }
}
