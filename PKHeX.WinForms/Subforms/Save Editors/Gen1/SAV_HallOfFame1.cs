using System;
using System.Drawing;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Drawing.PokeSprite;

namespace PKHeX.WinForms;

public partial class SAV_HallOfFame1 : Form
{
    private readonly SAV1 Origin;
    private readonly SAV1 SAV;
    private readonly HallOfFameReader1 Fame;

    private int Team = -1;
    private int Slot = -1;

    public SAV_HallOfFame1(SAV1 sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (SAV1)(Origin = sav).Clone();
        Fame = SAV.HallOfFame;

        Setup();
        LB_DataEntry.SelectedIndex = 0;
        UpdateTeamPreview(0);

        NUD_Clears.Value = sav.HallOfFameCount;
    }

    private void Setup()
    {
        TB_Nickname.MaxLength = SAV.Japanese ? 5 : 10;

        CB_Species.Items.Clear();

        var filtered = GameInfo.FilteredSources;
        CB_Species.InitializeBinding();
        CB_Species.DataSource = new BindingSource(filtered.Species, string.Empty);

        for (int i = 0; i < HallOfFameReader1.TeamCount; i++)
            LB_DataEntry.Items.Add(GetTeamIndication(i));
    }

    private void ResetListBox()
    {
        for (int i = 0; i < HallOfFameReader1.TeamCount; i++)
            ResetListBox(i);
    }

    private void ResetListBox(int team) => LB_DataEntry.Items[team] = GetTeamIndication(team);

    private string GetTeamIndication(int team)
    {
        var state = GetTeamState(team);
        return $"{team + 1:00} ({state})";
    }

    private string GetTeamState(int i)
    {
        var count = Fame.GetTeamMemberCount(i);
        return count switch
        {
            0 => "✕",
            6 => "✓",
            _ => $"{count}/6",
        };
    }

    private void B_Cancel_Click(object sender, EventArgs e) => Close();

    private void B_Close_Click(object sender, EventArgs e) => SaveAndClose();

    private void SaveAndClose(bool entity = true)
    {
        if (entity)
            SaveEntity();
        SAV.HallOfFameCount = (byte)NUD_Clears.Value;
        Origin.CopyChangesFrom(SAV);
        Close();
    }

    private void UpdateTeamPreview() => UpdateTeamPreview(Team);

    private void UpdateTeamPreview(int team)
    {
        if (team < 0)
            return;

        bool loading = LoadingFields;
        LoadingFields = true;
        ResetListBox(team);
        RTB_Team.Text = Fame.GetTeamSummary(team, GameInfo.Strings.specieslist);
        LoadingFields = loading;
    }

    private void NUP_PartyIndex_ValueChanged(object sender, EventArgs e) => DisplayEntry(sender, e);

    private void DisplayEntry(object sender, EventArgs e)
    {
        if (LoadingFields)
            return;

        int team = LB_DataEntry.SelectedIndex;
        if (team < 0)
            return;

        var slot = (int)NUP_PartyIndex.Value - 1;

        SaveEntity();
        LoadEntity(team, slot);

        B_Delete.Enabled = team > 0;
        B_ClearSlot.Enabled = slot > 0;
        Team = team;
        Slot = slot;
        UpdateTeamPreview();
    }

    private void LoadEntity(int team, int slot)
    {
        var pk = Fame.GetEntity(team, slot);
        LoadEntity(pk);
    }

    private void SaveEntity()
    {
        if (Team < 0 || Slot < 0)
            return;
        var pk = Fame.GetEntity(Team, Slot);
        SaveEntity(pk);
        UpdateTeamPreview(Team);
    }

    private bool LoadingFields;

    private void LoadEntity(HallOfFameEntity1 pk)
    {
        LoadingFields = true;

        CB_Species.SelectedValue = (int)pk.Species;
        NUD_Level.Value = pk.Level;
        var nick = pk.Nickname;
        TB_Nickname.Text = nick;

        CHK_Nicknamed.Checked = IsNicknamed(pk.Species, nick);
        TB_Nickname.ReadOnly = !CHK_Nicknamed.Checked;
        PB_Sprite.Image = GetSprite(pk.Species);

        LoadingFields = false;
    }

    private static Bitmap GetSprite(ushort species)
        => SpriteUtil.GetSprite(species, 0, 0, 0, 0, false, 0, EntityContext.Gen1);

    private bool IsNicknamed(ushort species, string nickname)
    {
        var expect = SpeciesName.GetSpeciesNameGeneration(species, SAV.Language, 1);
        return nickname != expect;
    }

    private void SaveEntity(HallOfFameEntity1 pk)
    {
        var species = (ushort)WinFormsUtil.GetIndex(CB_Species);
        if (species is 0 or > 151)
        {
            pk.Clear();
            return;
        }

        pk.Species = species;
        pk.Level = (byte)NUD_Level.Value;

        if (pk.Nickname != TB_Nickname.Text) // preserve trash
            pk.Nickname = TB_Nickname.Text;
    }

    private void UpdateNickname(object sender, EventArgs e)
    {
        if (LoadingFields)
            return;

        if (!CHK_Nicknamed.Checked)
        {
            // Fetch Current Species and set it as Nickname Text
            var species = (ushort)WinFormsUtil.GetIndex(CB_Species);
            bool isNone = species is 0 or > (int)Species.Mew;
            var pk = Fame.GetEntity(Team, Slot);
            var name = isNone ? string.Empty : SpeciesName.GetSpeciesNameGeneration(species, SAV.Language, 1);
            TB_Nickname.Text = name;
            if (pk.Nickname != name) // preserve trash
                pk.Nickname = name;
        }
        TB_Nickname.ReadOnly = !CHK_Nicknamed.Checked;
    }

    private void ClickNickname(object sender, MouseEventArgs e)
    {
        if (sender is not TextBox tb)
            return;

        // Special Character Form
        if (ModifierKeys != Keys.Control)
            return;

        var team = LB_DataEntry.SelectedIndex;
        var member = (int)NUP_PartyIndex.Value - 1;
        var pk = Fame.GetEntity(team, member);
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

    private void B_Delete_Click(object sender, EventArgs e)
    {
        var index = Team;
        LoadingFields = true;
        Fame.Delete(index);
        LoadEntity(Team, Slot);
        ResetListBox();
        UpdateTeamPreview(index);
        LoadingFields = false;
    }

    private void B_ClearSlot_Click(object sender, EventArgs e)
    {
        var entity = Fame.GetEntity(LB_DataEntry.SelectedIndex, (int)NUP_PartyIndex.Value - 1);
        entity.Clear();
        LoadEntity(entity);
        UpdateTeamPreview();
    }

    private void B_SetParty_Click(object sender, EventArgs e)
    {
        LoadingFields = true;
        var count = Fame.RegisterParty(SAV, SAV.HallOfFameCount);
        ResetListBox();
        NUD_Clears.Value = SAV.HallOfFameCount = count;
        Team = -1;
        LoadingFields = false;

        var index = count - 1;
        ResetListBox(index);
        LB_DataEntry.SelectedIndex = index;
    }

    private void NUD_Level_ValueChanged(object sender, EventArgs e)
    {
        if (LoadingFields || Team < 0)
            return;
        SaveEntity();
    }

    private void CB_Species_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (LoadingFields || Team < 0)
            return;
        SaveEntity();
        var pk = Fame.GetEntity(Team, Slot);
        var species = pk.Species;
        if (!CHK_Nicknamed.Checked)
            TB_Nickname.Text = SpeciesName.GetSpeciesNameGeneration(species, SAV.Language, 1);
        PB_Sprite.Image = GetSprite(species);
    }

    private void TB_Nickname_TextChanged(object sender, EventArgs e)
    {
        if (LoadingFields || Team < 0)
            return;
        SaveEntity();
        var pk = Fame.GetEntity(Team, Slot);
        CHK_Nicknamed.Checked = IsNicknamed(pk.Species, TB_Nickname.Text);
        TB_Nickname.ReadOnly = !CHK_Nicknamed.Checked;
    }

    private void B_ClearAll_Click(object sender, EventArgs e)
    {
        Fame.Clear();
        NUD_Clears.Value = 0;
        SaveAndClose(entity: false);
    }
}
