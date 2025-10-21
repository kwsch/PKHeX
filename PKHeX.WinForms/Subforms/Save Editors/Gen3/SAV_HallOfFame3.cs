using System;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Drawing.PokeSprite;

namespace PKHeX.WinForms;

public partial class SAV_HallOfFame3 : Form
{
    private readonly SAV3 Origin;
    private readonly SAV3 SAV;
    private readonly HallFame3Entry[] Fame;
    private int prevEntry;
    private int prevMember;
    private bool Loading;

    public SAV_HallOfFame3(SAV3 sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (SAV3)(Origin = sav).Clone();
        Fame = HallFame3Entry.GetEntries(SAV);

        LB_Entries.DataSource = Enumerable.Range(0, 50).ToList();
        var filtered = GameInfo.FilteredSources;
        CB_Species.InitializeBinding();
        CB_Species.DataSource = new BindingSource(filtered.Species.ToList(), string.Empty);

        LB_Entries.SelectedIndex = 0;
        NUD_Members.Value = 0;
        var pk = Fame[LB_Entries.SelectedIndex].Team[(int)NUD_Members.Value];
        LoadEntry(pk);
        UpdateSprite();

        TB_TID.TextChanged += (_, _) => ValidateIDs();
        TB_SID.TextChanged += (_, _) => ValidateIDs();
        TB_PID.TextChanged += (_, _) => ValidateIDs();
        TB_Nickname.Click += ClickNickname;
        CB_Species.SelectedValueChanged += (_, _) => UpdateSprite();
        NUD_Members.ValueChanged += (_, _) =>
        {
            SaveEntry(Fame[prevEntry].Team[prevMember]);
            var pkm = Fame[LB_Entries.SelectedIndex].Team[(int)NUD_Members.Value];
            LoadEntry(pkm);
            prevMember = (int)NUD_Members.Value;
            prevEntry = LB_Entries.SelectedIndex;
            UpdateSprite();
        };

        LB_Entries.SelectedIndexChanged += (_, _) =>
        {
            SaveEntry(Fame[prevEntry].Team[prevMember]);
            NUD_Members.Value = 0;
            var pkm = Fame[LB_Entries.SelectedIndex].Team[0];
            LoadEntry(pkm);
            prevMember = (int)NUD_Members.Value;
            prevEntry = LB_Entries.SelectedIndex;
            UpdateSprite();
        };
    }

    private void ClearFields()
    {
        TB_TID.Text = TB_SID.Text = "0";
        TB_PID.Text = "0";
        TB_Nickname.Text = string.Empty;
        NUD_Level.Value = 0;
        CB_Species.SelectedIndex = 0;
    }

    private void LoadEntry(HallFame3PKM pk)
    {
        Loading = true;
        TB_TID.Text = pk.TID16.ToString("00000");
        TB_SID.Text = pk.SID16.ToString("00000");
        TB_PID.Text = pk.PID.ToString("X8");
        TB_Nickname.Text = pk.Nickname;
        NUD_Level.Value = pk.Level;
        CB_Species.SelectedValue = (int)pk.Species;
        Loading = false;
    }

    private void SaveEntry(HallFame3PKM pk)
    {
        pk.TID16 = Convert.ToUInt16(TB_TID.Text);
        pk.SID16 = Convert.ToUInt16(TB_SID.Text);
        pk.PID = Util.GetHexValue(TB_PID.Text);
        if (pk.Nickname != TB_Nickname.Text) // preserve trash
            pk.Nickname = TB_Nickname.Text;
        pk.Level = (int)NUD_Level.Value;
        pk.Species = (ushort)WinFormsUtil.GetIndex(CB_Species);
    }

    private void B_Cancel_Click(object sender, EventArgs e) => Close();

    private void B_Save_Click(object sender, EventArgs e)
    {
        var pkm = Fame[LB_Entries.SelectedIndex].Team[(int)NUD_Members.Value];
        SaveEntry(pkm);
        HallFame3Entry.SetEntries(SAV, Fame);
        Origin.CopyChangesFrom(SAV);
        Close();
    }

    private void ValidateIDs()
    {
        var pid = Util.GetHexValue(TB_PID.Text);
        if (pid.ToString("X") != TB_PID.Text && pid.ToString("X8") != TB_PID.Text)
            TB_PID.Text = pid.ToString("X8");

        var tid = Util.ToUInt32(TB_TID.Text);
        if (tid > ushort.MaxValue)
            tid = ushort.MaxValue;
        if (tid.ToString() != TB_TID.Text)
            TB_TID.Text = tid.ToString();

        var sid = Util.ToUInt32(TB_SID.Text);
        if (sid > ushort.MaxValue)
            sid = ushort.MaxValue;
        if (sid.ToString() != TB_SID.Text)
            TB_SID.Text = sid.ToString();

        CHK_Shiny.Checked = ShinyUtil.GetIsShiny3((sid << 16) | tid, pid);
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        if (Loading)
            return;

        var entry = Fame[LB_Entries.SelectedIndex].Team[(int)NUD_Members.Value];
        SaveEntry(entry);
        var shiny = entry.IsShiny ? Shiny.Always : Shiny.Never;
        PB_Sprite.Image = SpriteUtil.GetSprite(entry.Species, entry.DisplayForm(SAV.Version), 0, 0, 0, false, shiny, EntityContext.Gen3);
    }

    private void B_Clear_Click(object sender, EventArgs e) => ClearFields();

    private void ClickNickname(object? sender, EventArgs e)
    {
        if (sender is not TextBox tb)
            return;

        // Special Character Form
        if (ModifierKeys != Keys.Control)
            return;

        var pk = Fame[LB_Entries.SelectedIndex].Team[(int)NUD_Members.Value];
        if (tb.Text != pk.Nickname) // preserve trash
            pk.Nickname = tb.Text;

        var nicktrash = pk.NicknameTrash;
        var d = new TrashEditor(tb, nicktrash, SAV, SAV.Generation, SAV.Context);
        d.ShowDialog();
        tb.Text = d.FinalString;
        d.FinalBytes.CopyTo(nicktrash);

        if (tb.Text != pk.Nickname) // preserve trash
            tb.Text = pk.Nickname;
    }
}
