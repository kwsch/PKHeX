using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public partial class TrashEditor : Form
{
    private readonly IStringConverter Converter;
    private readonly ToolTip Tip = new() { InitialDelay = 200, IsBalloon = false, AutoPopDelay = 32_767 };
    private readonly List<NumericUpDown> Bytes = [];
    public string FinalString { get; private set; }
    public byte[] FinalBytes { get; private set; }

    private readonly byte[] Raw;
    private bool editing;

    public TrashEditor(TextBoxBase TB_NN, IStringConverter sav, byte generation) : this(TB_NN, [], sav, generation) { }

    public TrashEditor(TextBoxBase TB_NN, Span<byte> raw, IStringConverter converter, byte generation)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        Converter = converter;

        FinalString = TB_NN.Text;

        editing = true;
        if (raw.Length != 0)
        {
            Raw = FinalBytes = raw.ToArray();
            AddTrashEditing(raw.Length, generation);
        }
        else
        {
            Raw = FinalBytes = [];
        }

        var f = FontUtil.GetPKXFont();
        AddCharEditing(f, generation);
        TB_Text.MaxLength = TB_NN.MaxLength;
        TB_Text.Text = TB_NN.Text;
        TB_Text.Font = f;

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
        B_ApplyTrash.MouseHover += (_, _) =>
        {
            var text = GetTrashString();
            var data = SetString(text);
            var display = $"{text} = {Environment.NewLine}{string.Join(' ', data.Select(z => $"{z:X2}"))}";
            Tip.Show(display, B_ApplyTrash);
        };
    }

    private void B_Cancel_Click(object sender, EventArgs e) => Close();

    private void B_Save_Click(object sender, EventArgs e)
    {
        FinalString = TB_Text.Text;
        if (FinalBytes.Length == 0)
            FinalBytes = Raw;
        Close();
    }

    private void AddCharEditing(Font f, byte generation)
    {
        var chars = GetChars(generation);
        if (chars.Length == 0)
            return;

        FLP_Characters.Visible = true;
        foreach (ushort c in chars)
        {
            var l = GetLabel(((char)c).ToString());
            l.Font = f;
            l.AutoSize = false;
            l.Size = new Size(20, 20);
            l.Click += (_, _) => { if (TB_Text.Text.Length < TB_Text.MaxLength) TB_Text.AppendText(l.Text); };
            FLP_Characters.Controls.Add(l);
            var tt = new ToolTip();
            tt.SetToolTip(l, $"Insert {l.Text} (0x{c:X4})");
        }
    }

    private void AddTrashEditing(int count, byte generation)
    {
        FLP_Hex.Visible = true;
        GB_Trash.Visible = true;
        NUD_Generation.Value = generation;
        for (int i = 0; i < count; i++)
        {
            var l = GetLabel($"${i:X2}");
            l.Font = NUD_Generation.Font;
            var n = GetNUD(min: 0, max: 255, hex: true);
            n.Click += (_, _) =>
            {
                switch (ModifierKeys)
                {
                    case Keys.Shift: n.Value = n.Maximum; break;
                    case Keys.Alt: n.Value = n.Minimum; break;
                }
            };
            n.Value = Raw[i];
            n.ValueChanged += (_, _) => UpdateNUD(n);

            FLP_Hex.Controls.Add(l);
            FLP_Hex.Controls.Add(n);
            Bytes.Add(n);
            if (i % 4 == 3)
                FLP_Hex.SetFlowBreak(n, true);
        }
        TB_Text.TextChanged += (_, _) => UpdateString(TB_Text);

        CB_Species.InitializeBinding();
        CB_Species.DataSource = new BindingSource(GameInfo.SpeciesDataSource, null);

        CB_Language.InitializeBinding();
        CB_Language.DataSource = GameInfo.LanguageDataSource(generation);
    }

    private void UpdateNUD(NumericUpDown nud)
    {
        if (editing)
            return;

        // build bytes
        editing = true;
        int index = Bytes.IndexOf(nud);
        Raw[index] = (byte)nud.Value;

        TB_Text.Text = GetString();
        editing = false;
    }

    private void UpdateString(TextBox tb)
    {
        if (editing)
            return;
        editing = true;
        // build bytes
        ReadOnlySpan<byte> data = SetString(tb.Text);
        if (data.Length > Raw.Length)
            data = data[..Raw.Length];
        data.CopyTo(Raw);
        for (int i = 0; i < Raw.Length; i++)
            Bytes[i].Value = Raw[i];
        editing = false;
    }

    private void B_ApplyTrash_Click(object sender, EventArgs e)
    {
        string text = GetTrashString();
        ReadOnlySpan<byte> data = SetString(text);
        ReadOnlySpan<byte> current = SetString(TB_Text.Text);
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

    private string GetTrashString()
    {
        var species = (ushort)WinFormsUtil.GetIndex(CB_Species);
        var language = WinFormsUtil.GetIndex(CB_Language);
        var gen = (byte)NUD_Generation.Value;
        string text = SpeciesName.GetSpeciesNameGeneration(species, language, gen);

        if (string.IsNullOrEmpty(text)) // no result
            text = CB_Species.Text;
        return text;
    }

    private void B_ClearTrash_Click(object sender, EventArgs e)
    {
        byte[] current = SetString(TB_Text.Text);
        for (int i = current.Length; i < Bytes.Count; i++)
            Bytes[i].Value = 0;
    }

    private byte[] SetString(ReadOnlySpan<char> text)
    {
        Span<byte> temp = stackalloc byte[Raw.Length];
        var written = Converter.SetString(temp, text, text.Length, StringConverterOption.None);
        return temp[..written].ToArray();
    }

    private string GetString() => Converter.GetString(Raw);

    // Helpers
    private static Label GetLabel(string str) => new() { Text = str, AutoSize = false, Size = new Size(40, 24), TextAlign = ContentAlignment.MiddleRight };

    private static NumericUpDown GetNUD(byte min, byte max, bool hex) => new()
    {
        Maximum = max,
        Minimum = min,
        Hexadecimal = hex,
        Width = 40,
        Padding = new Padding(0),
        Margin = new Padding(0),
    };

    private static ReadOnlySpan<ushort> GetChars(byte generation) => generation switch
    {
        5 => SpecialCharsGen5,
        6 => SpecialCharsGen67,
        7 => SpecialCharsGen67,
        >= 8 => SpecialCharsGen8,
        _ => [], // Undocumented
    };

    // Unicode codepoints for special characters, incorrectly starting at 0x2460 instead of 0xE0xx.
    private static ReadOnlySpan<ushort> SpecialCharsGen5 =>
    [
        0x2460, // Full Neutral (Happy in Gen7)
        0x2461, // Full Happy (Angry in Gen7)
        0x2462, // Full Sad
        0x2463, // Full Angry (Neutral in Gen7)
        0x2464, // Full Right-up arrow
        0x2465, // Full Right-down arrow
        0x2466, // Full Zz
        0x2467, // ×
        0x2468, // ÷
        // Skip 69-6B, can't be entered.
        0x246C, // …
        0x246D, // ♂
        0x246E, // ♀
        0x246F, // ♠
        0x2470, // ♣
        0x2471, // ♥
        0x2472, // ♦
        0x2473, // ★
        0x2474, // ◎
        0x2475, // ○
        0x2476, // □
        0x2477, // △
        0x2478, // ◇
        0x2479, // ♪
        0x247A, // ☀
        0x247B, // ☁
        0x247C, // ☂
        0x247D, // ☃
        0x247E, // Half Neutral
        0x247F, // Half Happy
        0x2480, // Half Sad
        0x2481, // Half Angry
        0x2482, // Half Right-up arrow
        0x2483, // Half Right-down arrow 
        0x2484, // Half Zz
    ];

    private static ReadOnlySpan<ushort> SpecialCharsGen67 =>
    [
        0xE081, // Full Neutral (Happy in Gen7)
        0xE082, // Full Happy (Angry in Gen7)
        0xE083, // Full Sad
        0xE084, // Full Angry (Neutral in Gen7)
        0xE085, // Full Right-up arrow
        0xE086, // Full Right-down arrow
        0xE087, // Full Zz
        0xE088, // ×
        0xE089, // ÷
        // Skip 8A-8C, can't be entered.
        0xE08D, // …
        0xE08E, // ♂
        0xE08F, // ♀
        0xE090, // ♠
        0xE091, // ♣
        0xE092, // ♥
        0xE093, // ♦
        0xE094, // ★
        0xE095, // ◎
        0xE096, // ○
        0xE097, // □
        0xE098, // △
        0xE099, // ◇
        0xE09A, // ♪
        0xE09B, // ☀
        0xE09C, // ☁
        0xE09D, // ☂
        0xE09E, // ☃
        0xE09F, // Half Neutral
        0xE0A0, // Half Happy
        0xE0A1, // Half Sad
        0xE0A2, // Half Angry
        0xE0A3, // Half Right-up arrow
        0xE0A4, // Half Right-down arrow 
        0xE0A5, // Half Zz
    ];

    private static ReadOnlySpan<ushort> SpecialCharsGen8 =>
    [
        '…', // '\uE08D' -> '\u2026'
        '♂', // '\uE08E' -> '\u2642'
        '♀', // '\uE08F' -> '\u2640'
        '♠', // '\uE090' -> '\u2660'
        '♣', // '\uE091' -> '\u2663'
        '♥', // '\uE092' -> '\u2665'
        '♦', // '\uE093' -> '\u2666'
        '★', // '\uE094' -> '\u2605'
        '◎', // '\uE095' -> '\u25CE'
        '○', // '\uE096' -> '\u25CB'
        '□', // '\uE097' -> '\u25A1'
        '△', // '\uE098' -> '\u25B3'
        '◇', // '\uE099' -> '\u25C7'
        '♪', // '\uE09A' -> '\u266A'
        '☀', // '\uE09B' -> '\u2600'
        '☁', // '\uE09C' -> '\u2601'
        '☂', // '\uE09D' -> '\u2602'
        '☃', // '\uE09E' -> '\u2603'
    ];
}
