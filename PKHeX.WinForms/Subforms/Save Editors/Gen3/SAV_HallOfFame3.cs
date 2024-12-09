using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Drawing.PokeSprite;

namespace PKHeX.WinForms;

public partial class SAV_HallOfFame3 : Form
{
    private SAV3 Origin;
    private SAV3 SAV;
    private HallFame3Entry[] Fame;
    private int prevEntry = 0;
    private int prevMember = 0;

    public SAV_HallOfFame3(SAV3 sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (SAV3)(Origin = sav).Clone();
        Fame = HallFame3Entry.GetEntries(SAV);

        TB_PID.MaxLength = 8;
        TB_TID.MaxLength = TB_SID.MaxLength = 5;
        TB_Nickname.MaxLength = 10;
        TB_PID.CharacterCasing = CharacterCasing.Upper;

        TB_TID.KeyPress += (s, e) => { if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true; };
        TB_TID.TextChanged += (s, e) =>
        {
            if (TB_TID.Text == string.Empty) TB_TID.Text = "00000";
            if (Convert.ToInt32(TB_TID.Text) > 65535) TB_TID.Text = "65535";
        };
        TB_SID.KeyPress += (s, e) => { if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true; };
        TB_SID.TextChanged += (s, e) =>
        {
            if (TB_SID.Text == string.Empty) TB_SID.Text = "00000";
            if (Convert.ToInt32(TB_SID.Text) > 65535) TB_SID.Text = "65535";
        };
        TB_PID.KeyPress += (s, e) => { if (!char.IsControl(e.KeyChar) && !Uri.IsHexDigit(e.KeyChar)) e.Handled = true; };

        LB_Entries.InitializeBinding();
        LB_Entries.DataSource = Enumerable.Range(0, 50).ToList();
        var filtered = GameInfo.FilteredSources;
        CB_Species.InitializeBinding();
        CB_Species.DataSource = new BindingSource(filtered.Species.ToList(), string.Empty);

        NUD_Members.ValueChanged += (s, e) =>
        {
            UpdatePKM(Fame[prevEntry].Team[prevMember]);
            var pkm = Fame[LB_Entries.SelectedIndex].Team[(int)NUD_Members.Value];
            SetField(pkm);
            prevMember = (int)NUD_Members.Value;
            prevEntry = LB_Entries.SelectedIndex;
        };

        LB_Entries.SelectedIndexChanged += (s, e) =>
        {
            UpdatePKM(Fame[prevEntry].Team[prevMember]);
            NUD_Members.Value = 0;
            var pkm = Fame[LB_Entries.SelectedIndex].Team[0];
            SetField(pkm);
            prevMember = (int)NUD_Members.Value;
            prevEntry = LB_Entries.SelectedIndex;
        };

        LB_Entries.SelectedIndex = 0;
        NUD_Members.Value = 0;
        var pkm = Fame[LB_Entries.SelectedIndex].Team[(int)NUD_Members.Value];
        SetField(pkm);
    }

    private void ClearFields()
    {
        TB_TID.Text = TB_SID.Text = "00000";
        TB_PID.Text = "00000000";
        TB_Nickname.Text = string.Empty;
        NUD_Level.Value = 0;
        CB_Species.SelectedIndex = 0;
    }

    private void SetField(HallFame3PKM pkm)
    {
        if (pkm.Species == 0)
        {
            ClearFields();
            return;
        }
        TB_TID.Text = pkm.TID16.ToString("00000");
        TB_SID.Text = pkm.SID16.ToString("00000");
        TB_PID.Text = pkm.PID.ToString("X8");
        TB_Nickname.Text = pkm.Nickname;
        NUD_Level.Value = pkm.Level;
        CB_Species.SelectedValue = (int)(pkm.Species);
    }

    private void UpdatePKM(HallFame3PKM pkm)
    {
        pkm.TID16 = Convert.ToInt32(TB_TID.Text);
        pkm.SID16 = Convert.ToInt32(TB_SID.Text);
        pkm.PID = Convert.ToUInt32(TB_PID.Text, 16);
        pkm.Nickname = TB_Nickname.Text;
        pkm.Level = (int)NUD_Level.Value;
        pkm.Species = Convert.ToUInt16(CB_Species.SelectedValue);
    }

    private void B_Cancel_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void B_Save_Click(object sender, EventArgs e)
    {
        var pkm = Fame[LB_Entries.SelectedIndex].Team[(int)NUD_Members.Value];
        UpdatePKM(pkm);
        HallFame3Entry.SetEntries(SAV, Fame);
        Origin.CopyChangesFrom(SAV);
        Close();
    }
}

