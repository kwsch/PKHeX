using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public partial class SAV_DonutGenerator9a : Form
{
    private readonly SAV_Donut9a SAV;

    public SAV_DonutGenerator9a(SAV_Donut9a sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = sav;

        NUD_Start.Maximum = DonutPocket9a.MaxCount - 1;
        NUD_End.Maximum = DonutPocket9a.MaxCount;
        NUD_End.Value = DonutPocket9a.MaxCount;

        InitializeFlavorChoices();
    }

    private void InitializeFlavorChoices()
    {
        var localized = GameInfo.Strings.donutFlavor;
        List<FlavorChoice> types = [];

        for (int i = 0; i < DonutInfo.Flavors.Count; i++)
        {
            var (hash, name) = DonutInfo.Flavors[i];
            if (!TryGetTypeIndex(name, out var typeIndex))
                continue;
            types.Add(new(localized[i], typeIndex, hash));
        }

        CLB_Flavors.Items.AddRange([.. types]);
        for (int i = 0; i < CLB_Flavors.Items.Count; i++)
            CLB_Flavors.SetItemChecked(i, true);
    }

    private static bool TryGetTypeIndex(string name, out int typeIndex)
    {
        typeIndex = -1;
        if (name.Length < 8 || !int.TryParse(name.AsSpan(6, 2), out var value) || value is < 3 or > 21)
            return false;

        typeIndex = value - 3;
        return true;
    }

    private void B_Generate_Click(object sender, EventArgs e)
    {
        var start = (int)NUD_Start.Value;
        var end = (int)NUD_End.Value;
        if (start >= end)
        {
            WinFormsUtil.Alert("Choose a valid donut range.");
            return;
        }

        var flavors = GetCheckedValues(CLB_Flavors, (FlavorChoice z) => z.Hash);
        if (flavors.Length == 0)
        {
            WinFormsUtil.Alert("Select at least one flavor type.");
            return;
        }

        SAV.GenerateRandomDonuts(flavors, start, end);
        DialogResult = DialogResult.OK;
        Close();
    }

    private static TOut[] GetCheckedValues<TItem, TOut>(CheckedListBox list, Func<TItem, TOut> selector) where TItem : class
    {
        List<TOut> result = [];
        foreach (var item in list.CheckedItems)
        {
            if (item is TItem typed)
                result.Add(selector(typed));
        }
        return [.. result];
    }

    private void B_Cancel_Click(object sender, EventArgs e) => Close();

    private sealed record FlavorChoice(string Text, int TypeIndex, ulong Hash)
    {
        public override string ToString() => Text;
    }
}
