using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Drawing;

namespace PKHeX.WinForms
{
    public partial class SAV_HallOfFame : Form
    {
        private readonly SaveFile Origin;
        private readonly SAV6 SAV;

        public SAV_HallOfFame(SaveFile sav)
        {
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
            SAV = (SAV6)(Origin = sav).Clone();

            Array.Copy(SAV.Data, SAV.HoF, data, 0, data.Length); //Copy HoF section of save into Data
            Setup();
            LB_DataEntry.SelectedIndex = 0;
            NUP_PartyIndex_ValueChanged(null, EventArgs.Empty);
            TB_Nickname.Font = TB_OT.Font = FontUtil.GetPKXFont();
            editing = true;
        }

        private bool editing;

        private readonly IReadOnlyList<string> gendersymbols = Main.GenderSymbols;
        private readonly byte[] data = new byte[0x1B40];

        private void Setup()
        {
            CB_Species.Items.Clear();
            CB_HeldItem.Items.Clear();
            CB_Move1.Items.Clear();
            CB_Move2.Items.Clear();
            CB_Move3.Items.Clear();
            CB_Move4.Items.Clear();

            CB_Species.InitializeBinding();
            CB_Species.DataSource = new BindingSource(GameInfo.SpeciesDataSource.Skip(1).Where(s => s.Value <= SAV.MaxSpeciesID).ToList(), null);

            CB_Move1.InitializeBinding();
            CB_Move2.InitializeBinding();
            CB_Move3.InitializeBinding();
            CB_Move4.InitializeBinding();

            var MoveList = GameInfo.MoveDataSource;
            CB_Move1.DataSource = new BindingSource(MoveList, null);
            CB_Move2.DataSource = new BindingSource(MoveList, null);
            CB_Move3.DataSource = new BindingSource(MoveList, null);
            CB_Move4.DataSource = new BindingSource(MoveList, null);

            CB_HeldItem.InitializeBinding();
            CB_HeldItem.DataSource = new BindingSource(GameInfo.ItemDataSource, null);
        }

        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

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

            uint vnd = BitConverter.ToUInt32(data, offset + 0x1B0);
            uint vn = vnd & 0xFF;
            TB_VN.Text = vn.ToString("000");
            var s = new List<string> {$"Entry #{vn}"};
            uint date = vnd >> 14 & 0x1FFFF;
            uint year = (date & 0xFF) + 2000;
            uint month = date >> 8 & 0xF;
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

                if (sender != null)
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
                int species = BitConverter.ToUInt16(data, offset + 0x00);
                if (species == 0)
                    continue;

                moncount++;
                AddEntryDescription(offset, s, species);

                offset += 0x48;
            }

            return moncount;
        }

        private void AddEntryDescription(int offset, List<string> s, int species)
        {
            int helditem = BitConverter.ToUInt16(data, offset + 0x02);
            int move1 = BitConverter.ToUInt16(data, offset + 0x04);
            int move2 = BitConverter.ToUInt16(data, offset + 0x06);
            int move3 = BitConverter.ToUInt16(data, offset + 0x08);
            int move4 = BitConverter.ToUInt16(data, offset + 0x0A);

            int TID = BitConverter.ToUInt16(data, offset + 0x10);
            int SID = BitConverter.ToUInt16(data, offset + 0x12);

            uint slgf = BitConverter.ToUInt32(data, offset + 0x14);
            // uint form = slgf & 0x1F;
            uint gender = slgf >> 5 & 3; // 0 M; 1 F; 2 G
            uint level = slgf >> 7 & 0x7F;
            uint shiny = slgf >> 14 & 0x1;
            // uint unkn = slgf >> 15;

            string nickname = Util.TrimFromZero(Encoding.Unicode.GetString(data, offset + 0x18, 24));
            string OTname = Util.TrimFromZero(Encoding.Unicode.GetString(data, offset + 0x30, 24));

            string genderstr = gendersymbols[(int)gender];
            string shinystr = shiny == 1 ? "Yes" : "No";

            var str = GameInfo.Strings;
            s.Add($"Name: {nickname}");
            s.Add($" ({str.Species[species]} - {genderstr})");
            s.Add($"Level: {level}");
            s.Add($"Shiny: {shinystr}");
            s.Add($"Held Item: {str.Item[helditem]}");
            s.Add($"Move 1: {str.Move[move1]}");
            s.Add($"Move 2: {str.Move[move2]}");
            s.Add($"Move 3: {str.Move[move3]}");
            s.Add($"Move 4: {str.Move[move4]}");
            s.Add($"OT: {OTname} ({TID}/{SID})");
            s.Add(string.Empty);
        }

        private void NUP_PartyIndex_ValueChanged(object sender, EventArgs e)
        {
            editing = false;
            int index = LB_DataEntry.SelectedIndex;
            int offset = (index * 0x1B4) + ((Convert.ToInt32(NUP_PartyIndex.Value)-1) * 0x48);

            if (offset < 0)
                return;

            int species = BitConverter.ToUInt16(data, offset + 0x00);
            CB_Species.SelectedValue = species;
            int item = BitConverter.ToUInt16(data, offset + 0x02);
            CB_HeldItem.SelectedValue = item;
            int move1 = BitConverter.ToUInt16(data, offset + 0x04);
            int move2 = BitConverter.ToUInt16(data, offset + 0x06);
            int move3 = BitConverter.ToUInt16(data, offset + 0x08);
            int move4 = BitConverter.ToUInt16(data, offset + 0x0A);
            CB_Move1.SelectedValue = move1;
            CB_Move2.SelectedValue = move2;
            CB_Move3.SelectedValue = move3;
            CB_Move4.SelectedValue = move4;

            uint EC = BitConverter.ToUInt32(data, offset + 0xC);
            TB_EC.Text = EC.ToString("X8");

            TB_TID.Text = BitConverter.ToUInt16(data, offset + 0x10).ToString("00000");
            TB_SID.Text = BitConverter.ToUInt16(data, offset + 0x12).ToString("00000");

            TB_Nickname.Text = Util.TrimFromZero(Encoding.Unicode.GetString(data, offset + 0x18, 24));
            TB_OT.Text = Util.TrimFromZero(Encoding.Unicode.GetString(data, offset + 0x30, 24));

            uint slgf = BitConverter.ToUInt32(data, offset + 0x14);
            uint form = slgf & 0x1F;
            uint gender = slgf >> 5 & 3; // 0 M; 1 F; 2 G
            uint level = slgf >> 7 & 0x7F;
            uint shiny = slgf >> 14 & 0x1;
            uint nick = BitConverter.ToUInt16(data,offset+0x16);

            CHK_Shiny.Checked = shiny == 1;

            TB_Level.Text = level.ToString("000");

            CHK_Nicknamed.Checked = nick == 1;

            SetForms();
            CB_Form.SelectedIndex = (int)form;
            SetGenderLabel((int)gender);
            UpdateNickname(sender, e);
            bpkx.Image = SpriteUtil.GetSprite(species, (int)form, (int)gender, 0, item, false, shiny == 1);
            editing = true;
        }

        private void Write_Entry(object sender, EventArgs e)
        {
            if (!editing)
                return; //Don't do writing until loaded

            Validate_TextBoxes();

            int index = LB_DataEntry.SelectedIndex;
            int partymember = Convert.ToInt32(NUP_PartyIndex.Value) - 1;
            int offset = (index * 0x1B4) + (partymember * 0x48);

            BitConverter.GetBytes(Convert.ToUInt16(CB_Species.SelectedValue)).CopyTo(data, offset + 0x00);
            BitConverter.GetBytes(Convert.ToUInt16(CB_HeldItem.SelectedValue)).CopyTo(data, offset + 0x02);
            BitConverter.GetBytes(Convert.ToUInt16(CB_Move1.SelectedValue)).CopyTo(data, offset + 0x04);
            BitConverter.GetBytes(Convert.ToUInt16(CB_Move2.SelectedValue)).CopyTo(data, offset + 0x06);
            BitConverter.GetBytes(Convert.ToUInt16(CB_Move3.SelectedValue)).CopyTo(data, offset + 0x08);
            BitConverter.GetBytes(Convert.ToUInt16(CB_Move4.SelectedValue)).CopyTo(data, offset + 0x0A);
            BitConverter.GetBytes(Util.GetHexValue(TB_EC.Text)).CopyTo(data, offset + 0x0C);

            BitConverter.GetBytes(Convert.ToUInt16(TB_TID.Text)).CopyTo(data, offset + 0x10);
            BitConverter.GetBytes(Convert.ToUInt16(TB_SID.Text)).CopyTo(data, offset + 0x12);

            uint rawslgf = BitConverter.ToUInt32(data, offset + 0x14);
            uint slgf = 0;
            slgf |= (uint)(CB_Form.SelectedIndex & 0x1F);
            slgf |= (uint)((PKX.GetGenderFromString(Label_Gender.Text) & 0x3) << 5);
            slgf |= (uint)((Convert.ToUInt16(TB_Level.Text) & 0x7F) << 7);
            if (CHK_Shiny.Checked)
                slgf |= 1 << 14;

            slgf |= rawslgf & 0x8000;
            Array.Copy(BitConverter.GetBytes(slgf), 0, data, offset + 0x14, 2);

            uint nick = 0;
            if (CHK_Nicknamed.Checked)
                nick = 1;
            Array.Copy(BitConverter.GetBytes(nick), 0, data, offset + 0x16, 2);

            //Mimic in-game behavior of not clearing strings. It's awful, but accuracy > good.
            string pk = TB_Nickname.Text; if (pk.Length != 12) pk = pk.PadRight(pk.Length + 1, '\0');
            string ot = TB_OT.Text; if (ot.Length != 12) ot = ot.PadRight(pk.Length + 1, '\0');
            Encoding.Unicode.GetBytes(pk).CopyTo(data, offset + 0x18);
            Encoding.Unicode.GetBytes(ot).CopyTo(data, offset + 0x30);

            offset = index * 0x1B4;

            uint vnd = 0;
            uint date = 0;
            vnd |= Convert.ToUInt32(TB_VN.Text) & 0xFF;
            date |= (uint)((CAL_MetDate.Value.Year - 2000) & 0xFF);
            date |= (uint)((CAL_MetDate.Value.Month & 0xF) << 8);
            date |= (uint)((CAL_MetDate.Value.Day & 0x1F) << 12);
            vnd |= (date & 0x1FFFF) << 14;
            //Fix for top bit
            uint rawvnd = BitConverter.ToUInt32(data, offset + 0x1B0);
            vnd |= rawvnd & 0x80000000;
            Array.Copy(BitConverter.GetBytes(vnd), 0, data, offset + 0x1B0, 4);

            var spec = WinFormsUtil.GetIndex(CB_Species);
            var form = CB_Form.SelectedIndex & 0x1F;
            var gender = PKX.GetGenderFromString(Label_Gender.Text);
            var item = WinFormsUtil.GetIndex(CB_HeldItem);
            bpkx.Image = SpriteUtil.GetSprite(spec, form, gender, 0, item, false, CHK_Shiny.Checked);
            DisplayEntry(null, EventArgs.Empty); // refresh text view
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
                int species = WinFormsUtil.GetIndex(CB_Species);
                if (species == 0 || species > (int)Species.Volcanion)
                {
                    TB_Nickname.Text = string.Empty;
                }
                else
                {
                    // get language
                    TB_Nickname.Text = SpeciesName.GetSpeciesNameGeneration(species, SAV.Language, 6);
                }
            }
            TB_Nickname.ReadOnly = !CHK_Nicknamed.Checked;

            Write_Entry(null, EventArgs.Empty);
        }

        private void SetForms()
        {
            int species = WinFormsUtil.GetIndex(CB_Species);
            bool hasForms = PersonalTable.AO[species].HasFormes || new[] { 664, 665, 414 }.Contains(species);
            CB_Form.Enabled = CB_Form.Visible = hasForms;

            CB_Form.InitializeBinding();
            CB_Form.DataSource = FormConverter.GetFormList(species, GameInfo.Strings.types, GameInfo.Strings.forms, Main.GenderSymbols, SAV.Generation);
        }

        private void UpdateSpecies(object sender, EventArgs e)
        {
            SetForms();
            UpdateNickname(null, EventArgs.Empty);
        }

        private void UpdateShiny(object sender, EventArgs e)
        {
            if (!editing)
                return; //Don't do writing until loaded

            var spec = WinFormsUtil.GetIndex(CB_Species);
            var form = CB_Form.SelectedIndex & 0x1F;
            var gender = PKX.GetGenderFromString(Label_Gender.Text);
            var item = WinFormsUtil.GetIndex(CB_HeldItem);
            bpkx.Image = SpriteUtil.GetSprite(spec, form, gender, 0, item, false, CHK_Shiny.Checked);

            Write_Entry(null, EventArgs.Empty);
        }

        private void UpdateGender(object sender, EventArgs e)
        {
            // Get Gender Threshold
            int species = WinFormsUtil.GetIndex(CB_Species);
            int gt = SAV.Personal[species].Gender;

            if (gt == 255)
                Label_Gender.Text = gendersymbols[2];
            else if (gt == 254)
                Label_Gender.Text = gendersymbols[1];
            else if (gt == 0)
                Label_Gender.Text = gendersymbols[0];

            if (gt == 255 || gt == 0 || gt == 254) // Single gender/genderless abort
                return;

            if (gt < 256) // If not a single gender(less) species:
            {
                Label_Gender.Text = PKX.GetGenderFromString(Label_Gender.Text) == 0 ? gendersymbols[1] : gendersymbols[0];

                if (PKX.GetGenderFromString(CB_Form.Text) == 0 && Label_Gender.Text != gendersymbols[0])
                    CB_Form.SelectedIndex = 1;
                else if (PKX.GetGenderFromString(CB_Form.Text) == 1 && Label_Gender.Text != gendersymbols[1])
                    CB_Form.SelectedIndex = 0;
            }

            if (species == (int)Species.Pyroar)
                CB_Form.SelectedIndex = PKX.GetGenderFromString(Label_Gender.Text);

            Write_Entry(null, EventArgs.Empty);
        }

        private void SetGenderLabel(int gender)
        {
            if (gender == 0)
                Label_Gender.Text = gendersymbols[0];    // Male
            else if (gender == 1)
                Label_Gender.Text = gendersymbols[1];    // Female
            else
                Label_Gender.Text = gendersymbols[2];    // Genderless

            Write_Entry(null, EventArgs.Empty);
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
            DisplayEntry(LB_DataEntry, null);
        }

        private void ChangeNickname(object sender, MouseEventArgs e)
        {
            TextBox tb = !(sender is TextBox) ? TB_Nickname : (TextBox) sender;
            // Special Character Form
            if (ModifierKeys != Keys.Control)
                return;

            int offset = LB_DataEntry.SelectedIndex * 0x1B4;
            var nicktrash = data.Skip(offset + 0x18).Take(24).ToArray();
            SAV.SetString(TB_Nickname.Text, 12).CopyTo(nicktrash, 0);
            var d = new TrashEditor(tb, nicktrash, SAV);
            d.ShowDialog();
            tb.Text = d.FinalString;
            d.FinalBytes.CopyTo(data, offset + 0x18);

            string nickname = Util.TrimFromZero(Encoding.Unicode.GetString(data, offset + 0x18, 24));
            TB_Nickname.Text = nickname;
        }
    }
}
