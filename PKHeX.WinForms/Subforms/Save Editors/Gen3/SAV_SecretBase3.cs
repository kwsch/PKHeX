using PKHeX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PKHeX.WinForms;

public partial class SAV_SecretBase3 : Form
{
    private readonly SaveFile Origin;
    private readonly SAV3 SAV;
    private readonly SecretBaseManager3 Manager;

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
        CB_Move1.DataSource = new BindingSource(moves, string.Empty);
        CB_Move2.DataSource = new BindingSource(moves, string.Empty);
        CB_Move3.DataSource = new BindingSource(moves, string.Empty);
        CB_Move4.DataSource = new BindingSource(moves, string.Empty);
        CB_Item.DataSource = new BindingSource(filtered.Items, string.Empty);
        CB_Species.DataSource = new BindingSource(filtered.Species, string.Empty);
        CB_Form.DataSource = new List<char> { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '!', '?' };

        TB_Entered.KeyPress += (_, e) => e.Handled = !(char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar));
        TB_TID.KeyPress += (_, e) => e.Handled = !(char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar));
        TB_SID.KeyPress += (_, e) => e.Handled = !(char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar));
        TB_TID.TextChanged += (_, _) =>
        {
            if (TB_TID.Text.Length == 0)
                TB_TID.Text = "0";
            if (uint.Parse(TB_TID.Text) > ushort.MaxValue)
                TB_TID.Text = ushort.MaxValue.ToString();
        };
        TB_SID.TextChanged += (_, _) =>
        {
            if (TB_SID.Text.Length == 0)
                TB_SID.Text = "0";
            if (uint.Parse(TB_SID.Text) > ushort.MaxValue)
                TB_SID.Text = ushort.MaxValue.ToString();
        };
        TB_Entered.TextChanged += (_, _) =>
        {
            if (TB_Entered.Text.Length == 0)
                TB_Entered.Text = "0";
            if (uint.Parse(TB_Entered.Text) > byte.MaxValue)
                TB_Entered.Text = byte.MaxValue.ToString();
        };
        TB_PID.TextChanged += (_, _) =>
        {
            if (TB_PID.Text.Length == 0)
                TB_PID.Text = "0";
            if (!TB_PID.Text.All(c => "0123456789abcdefABCDEF\n".Contains(c)))
                TB_PID.Text = uint.MaxValue.ToString("X8");
        };

        Manager = ((IGen3Hoenn)SAV).SecretBases;
        LB_Bases.InitializeBinding();
        LB_Bases.DataSource = Manager.Bases;
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
            var secret = (SecretBase3)LB_Bases.SelectedItem!;
            ShowPKM(secret.Team.Team[(int)NUD_TeamMember.Value - 1]);
        };
        LB_Bases.SelectedIndexChanged += (_, _) =>
        {
            ShowTrainer();
            //NUD_TeamMember.Maximum = ((SecretBase3)LB_Bases.SelectedItem).Team.Team.Length;
        };

        if (Manager.Count > 0)
        {
            LB_Bases.SelectedIndex = 0;
            ShowTrainer();
        }
        else
        {
            B_Save.Enabled = false;
        }
    }

    private void ShowTrainer()
    {
        var secret = (SecretBase3)LB_Bases.SelectedItem!;
        ShowTrainer(secret);
    }

    private void ShowTrainer(SecretBase3 trainer)
    {
        TB_Name.Text = trainer.OriginalTrainerName;
        T_TrainerGender.Gender = trainer.OriginalTrainerGender;
        T_TrainerGender.Show();
        TB_TID.Text = trainer.TID16.ToString();
        TB_SID.Text = trainer.SID16.ToString();
        TB_Entered.Text = trainer.TimesEntered.ToString();
        TB_Class.Text = trainer.OriginalTrainerClassName;
        CHK_Battled.Checked = trainer.BattledToday;
        CHK_Registered.Checked = trainer.RegistryStatus == 1;
        NUD_TeamMember.Value = 1;
        ShowPKM(trainer.Team.Team[(int)NUD_TeamMember.Value - 1]);
    }

    private void ShowPKM(SecretBase3PKM pk)
    {
        CB_Species.SelectedValue = (int)pk.Species;
        if (pk.Species == (int)Species.Unown)
        {
            CB_Form.SelectedIndex = pk.Form - 1;
            CB_Form.Visible = true;
        }
        else
        {
            CB_Form.Visible = false;
        }
        TB_PID.Text = pk.PID.ToString("X8");
        CB_Item.SelectedValue = (int)pk.HeldItem;
        CB_Move1.SelectedValue = (int)pk.Move1;
        CB_Move2.SelectedValue = (int)pk.Move2;
        CB_Move3.SelectedValue = (int)pk.Move3;
        CB_Move4.SelectedValue = (int)pk.Move4;
        NUD_Level.Value = pk.Level;
        NUD_EVs.Value = pk.EVAll;
    }

    private void B_UpdateTrainer_Click(object sender, EventArgs e)
    {
        var secret = (SecretBase3)LB_Bases.SelectedItem!;
        secret.OriginalTrainerName = TB_Name.Text;
        secret.OriginalTrainerGender = T_TrainerGender.Gender;
        secret.TID16 = ushort.Parse(TB_TID.Text);
        secret.SID16 = ushort.Parse(TB_SID.Text);
        secret.TimesEntered = byte.Parse(TB_Entered.Text);
        secret.BattledToday = CHK_Battled.Checked;
        secret.RegistryStatus = CHK_Registered.Checked ? 1 : 0;
        LB_Bases.DisplayMember = null!;
        LB_Bases.DisplayMember = "OriginalTrainerName";
        System.Media.SystemSounds.Asterisk.Play();
    }

    private void B_UpdatePKM_Click(object sender, EventArgs e)
    {
        var secret = (SecretBase3)LB_Bases.SelectedItem!;
        var pkmteam = secret.Team;
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
        secret.Team = pkmteam; // save changes
        System.Media.SystemSounds.Asterisk.Play();
    }

    private void B_Cancel_Click(object sender, EventArgs e) => Close();

    private void B_Save_Click(object sender, EventArgs e)
    {
        var bases = LB_Bases.Items.Cast<SecretBase3>().ToList();
        Manager.Bases = bases;
        Manager.Save();
        Origin.CopyChangesFrom(SAV);
        Close();
    }
}
