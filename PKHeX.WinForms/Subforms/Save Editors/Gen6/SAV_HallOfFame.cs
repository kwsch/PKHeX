using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Drawing.PokeSprite;

namespace PKHeX.WinForms;

public partial class SAV_HallOfFame : Form
{
    private readonly SAV6 Origin;
    private readonly SAV6 SAV;
    private readonly HallOfFame6 Fame;

    private bool editing;

    private readonly IReadOnlyList<string> gendersymbols = Main.GenderSymbols;

    public SAV_HallOfFame(SAV6 sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (SAV6)(Origin = sav).Clone();
        Fame = ((ISaveBlock6Main)SAV).HallOfFame;

        Setup();
        LB_DataEntry.SelectedIndex = 0;
        NUP_PartyIndex_ValueChanged(this, EventArgs.Empty);
        TB_Nickname.Font = TB_OT.Font = FontUtil.GetPKXFont();
        editing = true;
    }

    private void Setup()
    {
        CB_Species.Items.Clear();
        CB_HeldItem.Items.Clear();
        CB_Move1.Items.Clear();
        CB_Move2.Items.Clear();
        CB_Move3.Items.Clear();
        CB_Move4.Items.Clear();

        var filtered = GameInfo.FilteredSources;
        CB_Species.InitializeBinding();
        CB_Species.DataSource = new BindingSource(filtered.Species, null);

        CB_Move1.InitializeBinding();
        CB_Move2.InitializeBinding();
        CB_Move3.InitializeBinding();
        CB_Move4.InitializeBinding();

        var MoveList = filtered.Moves;
        CB_Move1.DataSource = new BindingSource(MoveList, null);
        CB_Move2.DataSource = new BindingSource(MoveList, null);
        CB_Move3.DataSource = new BindingSource(MoveList, null);
        CB_Move4.DataSource = new BindingSource(MoveList, null);

        CB_HeldItem.InitializeBinding();
        CB_HeldItem.DataSource = new BindingSource(filtered.Items, null);
    }

    private void B_Cancel_Click(object sender, EventArgs e) => Close();

    private void B_Close_Click(object sender, EventArgs e)
    {
        Origin.CopyChangesFrom(SAV);
        Close();
    }

    private void DisplayEntry(object sender, EventArgs e)
    {
        editing = false;
        RTB.Font = new Font("Courier New", 8);
        RTB.LanguageOption = RichTextBoxLanguageOptions.DualFont;
        int index = LB_DataEntry.SelectedIndex;

        var span = Fame.GetEntry(index);
        var vnd = new HallFame6Index(span[^4..]);
        TB_VN.Text = vnd.ClearIndex.ToString("000");
        var s = new List<string> { $"Entry #{vnd.ClearIndex}" };
        if (!vnd.HasData)
        {
            s.Add("No records in this slot.");
            groupBox1.Enabled = false;

            editing = false;
            NUP_PartyIndex_ValueChanged(sender, e);
        }
        else
        {
            groupBox1.Enabled = true;
            var moncount = AddEntries(span, s, vnd);

            if (sender != this)
            {
                NUP_PartyIndex.Maximum = moncount == 0 ? 1 : moncount;
                NUP_PartyIndex.Value = 1;
                NUP_PartyIndex_ValueChanged(sender, e);
            }
            else
            {
                editing = true;
            }
        }

        RTB.Lines = [.. s];
        RTB.Font = new Font("Courier New", 8);
    }

    private int AddEntries(Span<byte> data, List<string> s, HallFame6Index vnd)
    {
        var year = vnd.Year + 2000;
        var month = vnd.Month;
        var day = vnd.Day;

        s.Add($"Date: {year}/{month:00}/{day:00}");
        s.Add(string.Empty);
        CAL_MetDate.Value = new DateTime((int)year, (int)month, (int)day);
        int moncount = 0;
        for (int i = 0; i < 6; i++)
        {
            var slice = data[(i * HallFame6Entity.SIZE)..];
            var entry = new HallFame6Entity(slice, SAV.Language);
            if (entry.Species == 0)
                continue;
            moncount++;
            AddEntryDescription(s, entry);
        }
        return moncount;
    }

    private void AddEntryDescription(List<string> s, HallFame6Entity entry)
    {
        string genderstr = gendersymbols[(int)entry.Gender];
        string shinystr = entry.IsShiny ? "Yes" : "No";

        var str = GameInfo.Strings;
        s.Add($"Name: {entry.Nickname}");
        s.Add($" ({str.Species[entry.Species]} - {genderstr})");
        s.Add($"Level: {entry.Level}");
        s.Add($"Shiny: {shinystr}");
        s.Add($"Held Item: {str.Item[entry.HeldItem]}");
        s.Add($"Move 1: {str.Move[entry.Move1]}");
        s.Add($"Move 2: {str.Move[entry.Move2]}");
        s.Add($"Move 3: {str.Move[entry.Move3]}");
        s.Add($"Move 4: {str.Move[entry.Move4]}");

        string OTGender = gendersymbols[(int)entry.OriginalTrainerGender];
        s.Add($"OT: {entry.OriginalTrainerName} ({OTGender}) ({entry.TID16}/{entry.SID16})");
        s.Add(string.Empty);
    }

    private void NUP_PartyIndex_ValueChanged(object sender, EventArgs e)
    {
        editing = false;
        int index = LB_DataEntry.SelectedIndex;
        var member = (Convert.ToInt32(NUP_PartyIndex.Value) - 1);
        var slice = Fame.GetEntity(index, member);
        var entry = new HallFame6Entity(slice, SAV.Language);
        CB_Species.SelectedValue = (int)entry.Species;
        CB_HeldItem.SelectedValue = (int)entry.HeldItem;
        CB_Move1.SelectedValue = (int)entry.Move1;
        CB_Move2.SelectedValue = (int)entry.Move2;
        CB_Move3.SelectedValue = (int)entry.Move3;
        CB_Move4.SelectedValue = (int)entry.Move4;

        TB_EC.Text = entry.EncryptionConstant.ToString("X8");

        TB_TID.Text = entry.TID16.ToString("00000");
        TB_SID.Text = entry.SID16.ToString("00000");

        TB_Nickname.Text = entry.Nickname;
        TB_OT.Text = entry.OriginalTrainerName;
        CHK_Shiny.Checked = entry.IsShiny;
        TB_Level.Text = entry.Level.ToString("000");
        CHK_Nicknamed.Checked = entry.IsNicknamed;

        SetForms();
        CB_Form.SelectedIndex = entry.Form;
        SetGenderLabel((byte)entry.Gender);
        Label_OTGender.Text = gendersymbols[(int)entry.OriginalTrainerGender];
        UpdateNickname(sender, e);
        var shiny = entry.IsShiny ? Shiny.Always : Shiny.Never;
        bpkx.Image = SpriteUtil.GetSprite(entry.Species, entry.Form, (byte)entry.Gender, 0, entry.HeldItem, false, shiny, EntityContext.Gen6);
        editing = true;
    }

    private void Write_Entry(object sender, EventArgs e)
    {
        if (!editing)
            return; //Don't do writing until loaded

        Validate_TextBoxes();

        int index = LB_DataEntry.SelectedIndex;
        int member = Convert.ToInt32(NUP_PartyIndex.Value) - 1;

        var slice = Fame.GetEntity(index, member);
        var entry = new HallFame6Entity(slice, SAV.Language)
        {
            Species = Convert.ToUInt16(CB_Species.SelectedValue),
            HeldItem = Convert.ToUInt16(CB_HeldItem.SelectedValue),
            Move1 = Convert.ToUInt16(CB_Move1.SelectedValue),
            Move2 = Convert.ToUInt16(CB_Move2.SelectedValue),
            Move3 = Convert.ToUInt16(CB_Move3.SelectedValue),
            Move4 = Convert.ToUInt16(CB_Move4.SelectedValue),
            EncryptionConstant = Util.GetHexValue(TB_EC.Text),
            TID16 = Convert.ToUInt16(TB_TID.Text),
            SID16 = Convert.ToUInt16(TB_SID.Text),
            Form = (byte)CB_Form.SelectedIndex,
            Gender = (uint)EntityGender.GetFromString(Label_Gender.Text) & 0x3,
            Level = Convert.ToUInt16(TB_Level.Text),
            IsShiny = CHK_Shiny.Checked,
            IsNicknamed = CHK_Nicknamed.Checked,
            Nickname = TB_Nickname.Text,
            OriginalTrainerName = TB_OT.Text,
            OriginalTrainerGender = (uint)EntityGender.GetFromString(Label_OTGender.Text) & 1,
        };

        var span = Fame.GetEntry(index);
        _ = new HallFame6Index(span[^4..])
        {
            ClearIndex = Convert.ToUInt16(TB_VN.Text),
            Year = (uint)CAL_MetDate.Value.Year - 2000,
            Month = (uint)CAL_MetDate.Value.Month,
            Day = (uint)CAL_MetDate.Value.Day,
            HasData = true,
        };

        var shiny = entry.IsShiny ? Shiny.Always : Shiny.Never;
        bpkx.Image = SpriteUtil.GetSprite(entry.Species, entry.Form, (byte)entry.Gender, 0, entry.HeldItem, false, shiny, EntityContext.Gen6);
        DisplayEntry(this, EventArgs.Empty); // refresh text view
    }

    private void Validate_TextBoxes()
    {
        TB_Level.Text = Math.Min(Util.ToInt32(TB_Level.Text), 100).ToString("000");
        TB_VN.Text = Math.Min(Util.ToInt32(TB_VN.Text), byte.MaxValue).ToString("000");
        TB_TID.Text = Math.Min(Util.ToInt32(TB_TID.Text), ushort.MaxValue).ToString("00000");
        TB_SID.Text = Math.Min(Util.ToInt32(TB_SID.Text), ushort.MaxValue).ToString("00000");
    }

    private void UpdateNickname(object sender, EventArgs e)
    {
        if (!CHK_Nicknamed.Checked)
        {
            // Fetch Current Species and set it as Nickname Text
            var species = (ushort)WinFormsUtil.GetIndex(CB_Species);
            bool isNone = species is 0 or > (int)Species.Volcanion;
            TB_Nickname.Text = isNone ? string.Empty : SpeciesName.GetSpeciesNameGeneration(species, SAV.Language, 6);
        }
        TB_Nickname.ReadOnly = !CHK_Nicknamed.Checked;

        Write_Entry(this, EventArgs.Empty);
    }

    private void SetForms()
    {
        var species = (ushort)WinFormsUtil.GetIndex(CB_Species);
        var pi = PersonalTable.AO[species];
        bool hasForms = FormInfo.HasFormSelection(pi, species, 6);
        CB_Form.Enabled = CB_Form.Visible = hasForms;

        CB_Form.InitializeBinding();
        CB_Form.DataSource = FormConverter.GetFormList(species, GameInfo.Strings.types, GameInfo.Strings.forms, gendersymbols, SAV.Context);
    }

    private void UpdateSpecies(object sender, EventArgs e)
    {
        SetForms();
        UpdateNickname(this, EventArgs.Empty);
    }

    private void UpdateShiny(object sender, EventArgs e)
    {
        if (!editing)
            return; //Don't do writing until loaded

        var species = (ushort)WinFormsUtil.GetIndex(CB_Species);
        var form = (byte)(CB_Form.SelectedIndex & 0x1F);
        var gender = EntityGender.GetFromString(Label_Gender.Text);
        var item = WinFormsUtil.GetIndex(CB_HeldItem);
        var shiny = CHK_Shiny.Checked ? Shiny.Always : Shiny.Never;
        bpkx.Image = SpriteUtil.GetSprite(species, form, gender, 0, item, false, shiny, EntityContext.Gen6);

        Write_Entry(this, EventArgs.Empty);
    }

    private void UpdateOTGender(object sender, EventArgs e)
    {
        var g = EntityGender.GetFromString(Label_OTGender.Text);
        Label_OTGender.Text = gendersymbols[g ^ 1];

        Write_Entry(this, EventArgs.Empty);
    }

    private void UpdateGender(object sender, EventArgs e)
    {
        // Get Gender Threshold
        var species = (ushort)WinFormsUtil.GetIndex(CB_Species);
        var pi = SAV.Personal[species];
        if (pi.IsDualGender)
        {
            var fg = EntityGender.GetFromString(Label_Gender.Text);
            fg ^= 1;
            fg &= 1;
            Label_Gender.Text = gendersymbols[fg];
        }
        else
        {
            var fg = pi.FixedGender();
            Label_Gender.Text = gendersymbols[fg];
            return;
        }

        var g = EntityGender.GetFromString(CB_Form.Text);
        if (g == 0 && Label_Gender.Text != gendersymbols[0])
            CB_Form.SelectedIndex = 1;
        else if (g == 1 && Label_Gender.Text != gendersymbols[1])
            CB_Form.SelectedIndex = 0;

        if (species == (int)Species.Pyroar)
            CB_Form.SelectedIndex = EntityGender.GetFromString(Label_Gender.Text);

        Write_Entry(this, EventArgs.Empty);
    }

    private void SetGenderLabel(byte gender)
    {
        Label_Gender.Text = gender switch
        {
            0 => gendersymbols[0], // M
            1 => gendersymbols[1], // F
            _ => gendersymbols[2], // -
        };

        Write_Entry(this, EventArgs.Empty);
    }

    private void B_CopyText_Click(object sender, EventArgs e)
    {
        WinFormsUtil.SetClipboardText(RTB.Text);
    }

    private void B_Delete_Click(object sender, EventArgs e)
    {
        if (LB_DataEntry.SelectedIndex < 1)
        {
            WinFormsUtil.Alert("Cannot delete your first Hall of Fame Clear entry.");
            return;
        }

        int index = LB_DataEntry.SelectedIndex;
        var prompt = WinFormsUtil.Prompt(MessageBoxButtons.YesNo, $"Delete Entry {index} from your records?");
        if (prompt != DialogResult.Yes)
            return;

        Fame.ClearEntry(index);
        DisplayEntry(LB_DataEntry, EventArgs.Empty);
    }

    private void ChangeNickname(object sender, MouseEventArgs e)
    {
        TextBox tb = (TextBox)sender;
        // Special Character Form
        if (ModifierKeys != Keys.Control)
            return;

        var team = LB_DataEntry.SelectedIndex;
        var member = (int)NUP_PartyIndex.Value - 1;
        var data = Fame.GetEntity(team, member);
        var nicktrash = data.Slice(0x18, 26);
        var text = tb.Text;
        SAV.SetString(nicktrash, text, 12, StringConverterOption.ClearZero);
        var d = new TrashEditor(tb, nicktrash, SAV, SAV.Generation);
        d.ShowDialog();
        tb.Text = d.FinalString;
        d.FinalBytes.CopyTo(nicktrash);

        TB_Nickname.Text = StringConverter6.GetString(nicktrash);
    }
}
