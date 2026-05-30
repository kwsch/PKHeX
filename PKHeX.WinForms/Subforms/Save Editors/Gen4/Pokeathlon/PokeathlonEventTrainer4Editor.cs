using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public sealed partial class PokeathlonEventTrainer4Editor : UserControl
{
    public PokeathlonEventTrainer4Editor()
    {
        InitializeComponent();
        var available = GameInfo.LanguageDataSource(4, EntityContext.Gen4);
        var languages = new List<ComboItem>(available.Count + 1) { new(GameInfo.Strings.specieslist[0], 0) };
        languages.AddRange(available);
        CB_Language.InitializeBinding();
        CB_Language.DataSource = new BindingSource(languages, string.Empty);
    }

    public void LoadObject(PokeathlonEventTrainer4 trainer)
    {
        TB_OT.Text = trainer.OriginalTrainerName;
        TB_TID16.Text = trainer.TID16.ToString("00000");
        TB_SID16.Text = trainer.SID16.ToString("00000");
        CB_Language.SelectedValue = (int)trainer.Language;
    }

    public void SaveObject(PokeathlonEventTrainer4 trainer)
    {
        trainer.OriginalTrainerName = TB_OT.Text;
        trainer.TID16 = ParseU16(TB_TID16.Text);
        trainer.SID16 = ParseU16(TB_SID16.Text);
        trainer.Language = (byte)WinFormsUtil.GetIndex(CB_Language);
    }

    private static ushort ParseU16(ReadOnlySpan<char> text)
    {
        if (!ushort.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value))
            return 0;
        return value;
    }
}
