using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public partial class SAV_Accessor<T> : Form where T : class, ISaveBlockAccessor<BlockInfo>
{
    private readonly SaveBlockMetadata<BlockInfo> Metadata;

    public SAV_Accessor(SaveFile sav, T accessor)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);

        Metadata = new SaveBlockMetadata<BlockInfo>(accessor);

        CB_Key.Items.AddRange(Metadata.GetSortedBlockList().ToArray());
        CB_Key.SelectedIndex = 0;

        propertyGrid1.SelectedObject = sav;
    }

    private void CB_Key_SelectedIndexChanged(object sender, EventArgs e)
    {
        var name = CB_Key.Text;
        var block = Metadata.GetBlock(name);
        UpdateBlockSummaryControls(block);
    }

    private void UpdateBlockSummaryControls(IDataIndirect obj)
    {
        PG_BlockView.SelectedObject = obj;
    }

    private void PG_BlockView_PropertyValueChanged(object? s, PropertyValueChangedEventArgs? e)
    {
        if (e?.ChangedItem is null)
            return;
        Debug.WriteLine($"ChangedItem = {e.ChangedItem.Label}, OldValue = {e.OldValue}, NewValue = {e.ChangedItem.Value}");
    }
}
