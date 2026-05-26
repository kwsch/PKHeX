using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public sealed partial class PokeathlonEventTrainer4Editor : UserControl
{
    public PokeathlonEventTrainer4Editor()
    {
        InitializeComponent();
        var languages = new List<ComboItem> { new(GameInfo.Strings.specieslist[0], 0) };
        languages.AddRange(GameInfo.LanguageDataSource(4, EntityContext.Gen4).Where(z => z.Value is not 0));
        CB_Language.InitializeBinding();
        CB_Language.DataSource = new BindingSource(languages, string.Empty);
    }

    public void LoadObject(PokeathlonEventTrainer4 entity)
    {
        TB_OT.Text = entity.OT;
        TB_TID16.Text = entity.TID16.ToString(CultureInfo.InvariantCulture);
        TB_SID16.Text = entity.SID16.ToString(CultureInfo.InvariantCulture);
        CB_Language.SelectedValue = (int)entity.Language;
    }

    public void SaveObject(PokeathlonEventTrainer4 entity)
    {
        entity.OT = TB_OT.Text;
        entity.TID16 = ParseU16(TB_TID16.Text);
        entity.SID16 = ParseU16(TB_SID16.Text);
        entity.Language = (byte)WinFormsUtil.GetIndex(CB_Language);
    }

    private static ushort ParseU16(string text)
    {
        if (!ushort.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value))
            return 0;
        return value;
    }
}
