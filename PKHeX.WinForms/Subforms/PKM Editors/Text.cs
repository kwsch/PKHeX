using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class f2_Text : Form
    {
        public f2_Text(TextBoxBase TB_NN, byte[] raw)
        {
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.curlanguage);

            FinalString = TB_NN.Text;
            Raw = FinalBytes = raw;

            editing = true;
            if (raw != null)
                addTrashEditing(TB_NN.MaxLength);

            addCharEditing();
            TB_Text.MaxLength = TB_NN.MaxLength;
            TB_Text.Text = TB_NN.Text;
            TB_Text.Font = pkxFont;

            if (FLP_Characters.Controls.Count == 0)
            {
                FLP_Characters.Visible = false;
                FLP_Hex.Height *= 2;
            }
            else if (FLP_Hex.Controls.Count == 0)
            {
                FLP_Characters.Location = FLP_Hex.Location;
                FLP_Characters.Height *= 2;
            }

            editing = false;
            CenterToParent();
        }
        
        private readonly List<NumericUpDown> Bytes = new List<NumericUpDown>();
        private readonly Font pkxFont = FontUtil.getPKXFont(12F);
        public string FinalString;
        public byte[] FinalBytes { get; private set; }
        private readonly byte[] Raw;
        private bool editing;
        private readonly bool bigendian = new[] { GameVersion.COLO, GameVersion.XD, GameVersion.BATREV, }.Contains(Main.SAV.Version);
        private void B_Cancel_Click(object sender, EventArgs e) => Close();
        private void B_Save_Click(object sender, EventArgs e)
        {
            FinalString = TB_Text.Text;
            if (FinalBytes != null)
                FinalBytes = Raw;
            Close();
        }

        private void addCharEditing()
        {
            ushort[] chars = getChars(Main.SAV.Generation);
            if (chars.Length == 0)
                return;

            FLP_Characters.Visible = true;
            foreach (ushort c in chars)
            {
                var l = getLabel((char)c+"");
                l.Font = pkxFont;
                l.AutoSize = false;
                l.Size = new Size(20, 20);
                l.Click += (s, e) => { if (TB_Text.Text.Length < TB_Text.MaxLength) TB_Text.AppendText(l.Text); };
                FLP_Characters.Controls.Add(l);
            }
        }
        private void addTrashEditing(int length)
        {
            FLP_Hex.Visible = true;
            GB_Trash.Visible = true;
            NUD_Generation.Value = Main.SAV.Generation;
            int charct = length;
            int bytesperchar = bigendian || Main.SAV.Generation > 3 ? 2 : 1;
            Font courier = new Font("Courier New", 8);
            for (int i = 0; i < charct * bytesperchar; i++)
            {
                var l = getLabel($"${i:X2}");
                l.Font = courier;
                var n = getNUD(hex: true, min: 0, max: 255);
                n.Click += (s, e) =>
                {
                    switch (ModifierKeys)
                    {
                        case Keys.Shift: n.Value = n.Maximum; break;
                        case Keys.Alt: n.Value = n.Minimum; break;
                    }
                };
                n.Value = Raw[i];
                n.ValueChanged += updateNUD;
                

                FLP_Hex.Controls.Add(l);
                FLP_Hex.Controls.Add(n);
                Bytes.Add(n);
            }
            TB_Text.TextChanged += updateString;

            CB_Species.DisplayMember = "Text";
            CB_Species.ValueMember = "Value";
            CB_Species.DataSource = new BindingSource(GameInfo.SpeciesDataSource, null);

            CB_Language.DisplayMember = "Text";
            CB_Language.ValueMember = "Value";
            var languages = Util.getUnsortedCBList("languages");
            if (Main.SAV.Generation < 7)
                languages = languages.Where(l => l.Value <= 8).ToList(); // Korean
            CB_Language.DataSource = languages;
        }

        private void updateNUD(object sender, EventArgs e)
        {
            if (editing)
                return;
            editing = true;
            // build bytes
            var nud = sender as NumericUpDown;
            int index = Bytes.IndexOf(nud);
            Raw[index] = (byte)nud.Value;

            string str = PKX.getString(Raw, Main.SAV.Generation, Main.SAV.Japanese, bigendian, Raw.Length);
            TB_Text.Text = str;
            editing = false;
        }
        private void updateString(object sender, EventArgs e)
        {
            if (editing)
                return;
            editing = true;
            // build bytes
            byte[] data = PKX.setString(TB_Text.Text, Main.SAV.Generation, Main.SAV.Japanese, bigendian, Raw.Length, Main.SAV.Language);
            Array.Copy(data, Raw, data.Length);
            for (int i = 0; i < data.Length; i++)
                Bytes[i].Value = Raw[i];
            editing = false;
        }
        private void B_ApplyTrash_Click(object sender, EventArgs e)
        {
            string species = PKX.getSpeciesNameGeneration(WinFormsUtil.getIndex(CB_Species),
                WinFormsUtil.getIndex(CB_Language), (int) NUD_Generation.Value);

            if (species == "") // no result
                species = CB_Species.Text;

            byte[] current = PKX.setString(TB_Text.Text, Main.SAV.Generation, Main.SAV.Japanese, bigendian, Raw.Length, Main.SAV.Language);
            byte[] data = PKX.setString(species, Main.SAV.Generation, Main.SAV.Japanese, bigendian, Raw.Length, Main.SAV.Language);
            if (data.Length <= current.Length)
            {
                WinFormsUtil.Alert("Trash byte layer is hidden by current text.",
                    $"Current Bytes: {current.Length}" + Environment.NewLine + $"Layer Bytes: {data.Length}");
                return;
            }
            if (data.Length > Bytes.Count)
            {
                WinFormsUtil.Alert("Trash byte layer is too long to apply.");
                return;
            }
            for (int i = current.Length; i < data.Length; i++)
                Bytes[i].Value = data[i];
        }
        private void B_ClearTrash_Click(object sender, EventArgs e)
        {
            byte[] current = PKX.setString(TB_Text.Text, Main.SAV.Generation, Main.SAV.Japanese, bigendian, Raw.Length, Main.SAV.Language);
            for (int i = current.Length; i < Bytes.Count; i++)
                Bytes[i].Value = 0;
        }

        // Helpers
        private static Label getLabel(string str) => new Label {Text = str, AutoSize = true};
        private static NumericUpDown getNUD(int min, int max, bool hex) => new NumericUpDown
        {
            Maximum = max,
            Minimum = min,
            Hexadecimal = hex,
            Width = 36,
            Padding = new Padding(0),
            Margin = new Padding(0),
        };

        private static ushort[] getChars(int generation)
        {
            switch (generation)
            {
                case 6:
                case 7:
                    return chars67;
                default: return new ushort[0];
            }
        }
        private static readonly ushort[] chars67 =
        {
            0xE081, 0xE082, 0xE083, 0xE084, 0xE085, 0xE086, 0xE087, 0xE08D,
            0xE08E, 0xE08F, 0xE090, 0xE091, 0xE092, 0xE093, 0xE094, 0xE095,
            0xE096, 0xE097, 0xE098, 0xE099, 0xE09A, 0xE09B, 0xE09C, 0xE09D,
            0xE09E, 0xE09F, 0xE0A0, 0xE0A1, 0xE0A2, 0xE0A3, 0xE0A4, 0xE0A5,
        };
    }
}
