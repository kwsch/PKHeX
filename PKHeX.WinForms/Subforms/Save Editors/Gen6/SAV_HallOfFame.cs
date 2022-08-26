using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Drawing.PokeSprite;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.WinForms;

public partial class SAV_HallOfFame : Form
{
    private readonly SAV6 Origin;
    private readonly SAV6 SAV;

    private bool editing;

    private readonly IReadOnlyList<string> gendersymbols = Main.GenderSymbols;
    private readonly byte[] data;

    public SAV_HallOfFame(SAV6 sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (SAV6)(Origin = sav).Clone();

        data = SAV.Data.Slice(SAV.HoF, 0x1B40); // Copy HoF section of save into Data
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
        Origin.SetData(data, SAV.HoF);
        Close();
    }

    private void DisplayEntry(object sender, EventArgs e)
    {
        editing = false;
        RTB.Font = new Font("Courier New", 8);
        RTB.LanguageOption = RichTextBoxLanguageOptions.DualFont;
        int index = LB_DataEntry.SelectedIndex;
        int offset = index * 0x1B4;

        uint vnd = ReadUInt32LittleEndian(data.AsSpan(offset + 0x1B0));
        uint vn = vnd & 0xFF;
        TB_VN.Text = vn.ToString("000");
        var s = new List<string> {$"Entry #{vn}"};
        uint date = (vnd >> 14) & 0x1FFFF;
        uint month = (date >> 8) & 0xF;
        uint year = (date & 0xFF) + 2000;
        uint day = date >> 12;
        if (day == 0)
        {
            s.Add("No records in this slot.");
            groupBox1.Enabled = false;

            editing = false;
            NUP_PartyIndex_ValueChanged(sender, e);
        }
        else
        {
            groupBox1.Enabled = true;
            var moncount = AddEntries(offset, s, year, month, day);

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

        RTB.Lines = s.ToArray();
        RTB.Font = new Font("Courier New", 8);
    }

    private int AddEntries(int offset, List<string> s, uint year, uint month, uint day)
    {
        s.Add($"Date: {year}/{month:00}/{day:00}");
        s.Add(string.Empty);
        CAL_MetDate.Value = new DateTime((int)year, (int)month, (int)day);
        int moncount = 0;
        for (int i = 0; i < 6; i++)
        {
            var entry = new HallFame6Entity(data.AsSpan(offset, HallFame6Entity.SIZE));
            if (entry.Species == 0)
                continue;

            moncount++;
            AddEntryDescription(s, entry);

            offset += HallFame6Entity.SIZE;
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

        string OTGender = gendersymbols[(int)entry.OT_Gender];
        s.Add($"OT: {entry.OT_Name} ({OTGender}) ({entry.TID}/{entry.SID})");
        s.Add(string.Empty);
    }

    private void NUP_PartyIndex_ValueChanged(object sender, EventArgs e)
    {
        editing = false;
        int index = LB_DataEntry.SelectedIndex;
        int offset = (index * 0x1B4) + ((Convert.ToInt32(NUP_PartyIndex.Value)-1) * HallFame6Entity.SIZE);

        if (offset < 0)
            return;

        var entry = new HallFame6Entity(data.AsSpan(offset, HallFame6Entity.SIZE));
        CB_Species.SelectedValue = (int)entry.Species;
        CB_HeldItem.SelectedValue = (int)entry.HeldItem;
        CB_Move1.SelectedValue = (int)entry.Move1;
        CB_Move2.SelectedValue = (int)entry.Move2;
        CB_Move3.SelectedValue = (int)entry.Move3;
        CB_Move4.SelectedValue = (int)entry.Move4;

        TB_EC.Text = entry.EncryptionConstant.ToString("X8");

        TB_TID.Text = entry.TID.ToString("00000");
        TB_SID.Text = entry.SID.ToString("00000");

        TB_Nickname.Text = entry.Nickname;
        TB_OT.Text = entry.OT_Name;
        CHK_Shiny.Checked = entry.IsShiny;
        TB_Level.Text = entry.Level.ToString("000");
        CHK_Nicknamed.Checked = entry.IsNicknamed;

        SetForms();
        CB_Form.SelectedIndex = entry.Form;
        SetGenderLabel((int)entry.Gender);
        Label_OTGender.Text = gendersymbols[(int)entry.OT_Gender];
        UpdateNickname(sender, e);
        var shiny = entry.IsShiny ? Shiny.Always : Shiny.Never;
        bpkx.Image = SpriteUtil.GetSprite(entry.Species, entry.Form, (int)entry.Gender, 0, entry.HeldItem, false, shiny, 6);
        editing = true;
    }

    private void Write_Entry(object sender, EventArgs e)
    {
        if (!editing)
            return; //Don't do writing until loaded

        Validate_TextBoxes();

        int index = LB_DataEntry.SelectedIndex;
        int partymember = Convert.ToInt32(NUP_PartyIndex.Value) - 1;
        int offset = (index * 0x1B4) + (partymember * HallFame6Entity.SIZE);
        var span = data.AsSpan(offset, HallFame6Entity.SIZE);
        var entry = new HallFame6Entity(span)
        {
            Species = Convert.ToUInt16(CB_Species.SelectedValue),
            HeldItem = Convert.ToUInt16(CB_HeldItem.SelectedValue),
            Move1 = Convert.ToUInt16(CB_Move1.SelectedValue),
            Move2 = Convert.ToUInt16(CB_Move2.SelectedValue),
            Move3 = Convert.ToUInt16(CB_Move3.SelectedValue),
            Move4 = Convert.ToUInt16(CB_Move4.SelectedValue),
            EncryptionConstant = Util.GetHexValue(TB_EC.Text),
            TID = Convert.ToUInt16(TB_TID.Text),
            SID = Convert.ToUInt16(TB_SID.Text),
            Form = (byte)CB_Form.SelectedIndex,
            Gender = (uint)EntityGender.GetFromString(Label_Gender.Text) & 0x3,
            Level = Convert.ToUInt16(TB_Level.Text),
            IsShiny = CHK_Shiny.Checked,
            IsNicknamed = CHK_Nicknamed.Checked,
            Nickname = TB_Nickname.Text,
            OT_Name = TB_OT.Text,
            OT_Gender = (uint)EntityGender.GetFromString(Label_OTGender.Text) & 1,
        };

        offset = index * 0x1B4;

        uint vnd = 0;
        uint date = 0;
        vnd |= Convert.ToUInt32(TB_VN.Text) & 0xFF;
        date |= (uint)((CAL_MetDate.Value.Year - 2000) & 0xFF);
        date |= (uint)((CAL_MetDate.Value.Month & 0xF) << 8);
        date |= (uint)((CAL_MetDate.Value.Day & 0x1F) << 12);
        vnd |= (date & 0x1FFFF) << 14;
        //Fix for top bit
        uint rawvnd = ReadUInt32LittleEndian(data.AsSpan(offset + 0x1B0));
        vnd |= rawvnd & 0x80000000;
        WriteUInt32LittleEndian(data.AsSpan(offset + 0x1B0), vnd);

        var shiny = entry.IsShiny ? Shiny.Always : Shiny.Never;
        bpkx.Image = SpriteUtil.GetSprite(entry.Species, entry.Form, (int)entry.Gender, 0, entry.HeldItem, false, shiny, 6);
        DisplayEntry(this, EventArgs.Empty); // refresh text view
    }

    private void Validate_TextBoxes()
    {
        TB_Level.Text = Math.Min(Util.ToInt32(TB_Level.Text), 100).ToString();
        TB_VN.Text = Math.Min(Util.ToInt32(TB_VN.Text), byte.MaxValue).ToString();
        TB_TID.Text = Math.Min(Util.ToInt32(TB_TID.Text), ushort.MaxValue).ToString();
        TB_SID.Text = Math.Min(Util.ToInt32(TB_SID.Text), ushort.MaxValue).ToString();
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
        bpkx.Image = SpriteUtil.GetSprite(species, form, gender, 0, item, false, shiny, 6);

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
            fg = (fg ^ 1) & 1;
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

    private void SetGenderLabel(int gender)
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
        if (LB_DataEntry.SelectedIndex < 1) { WinFormsUtil.Alert("Cannot delete your first Hall of Fame Clear entry."); return; }
        int index = LB_DataEntry.SelectedIndex;
        if (WinFormsUtil.Prompt(MessageBoxButtons.YesNo, $"Delete Entry {index} from your records?") != DialogResult.Yes)
            return;

        int offset = index * 0x1B4;
        if (index != 15) Array.Copy(data, offset + 0x1B4, data, offset, 0x1B4 * (15 - index));
        // Ensure Last Entry is Cleared
        Array.Copy(new byte[0x1B4], 0, data, 0x1B4 * 15, 0x1B4);
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
        int offset = (team * (4 + (6 * HallFame6Entity.SIZE))) + (member * HallFame6Entity.SIZE);
        var nicktrash = data.AsSpan(offset + 0x18, 26);
        var text = tb.Text;
        SAV.SetString(nicktrash, text.AsSpan(), 12, StringConverterOption.ClearZero);
        var d = new TrashEditor(tb, nicktrash, SAV);
        d.ShowDialog();
        tb.Text = d.FinalString;
        d.FinalBytes.CopyTo(nicktrash);

        TB_Nickname.Text = StringConverter6.GetString(nicktrash);
    }
}
