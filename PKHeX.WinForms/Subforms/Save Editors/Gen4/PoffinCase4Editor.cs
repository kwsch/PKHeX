using System;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public partial class PoffinCase4Editor : UserControl
{
    public PoffinCase4Editor() => InitializeComponent();
    private PoffinCase4 Case = null!; // initialized on load
    private int CurrentIndex = -1;
    private bool Updating;

    private readonly string[] ItemNames = Util.GetStringList("poffin4", Main.CurrentLanguage);

    public void Initialize(SAV4Sinnoh sav)
    {
        Case = new PoffinCase4(sav);

        LB_Poffins.Items.Clear();
        for (int i = 0; i < Case.Poffins.Length; i++)
            LB_Poffins.Items.Add(GetPoffinText(i));

        LB_Poffins.SelectedIndex = 0;
    }

    private void RefreshList()
    {
        Updating = true;
        for (int i = 0; i < Case.Poffins.Length; i++)
            LB_Poffins.Items[i] = GetPoffinText(i);
        Updating = false;
    }

    private string GetPoffinName(PoffinFlavor4 flavor)
    {
        var index = (uint)flavor;
        if (index >= ItemNames.Length)
            index = 0;
        return ItemNames[index];
    }

    private string GetPoffinText(int index) => $"{index + 1:000} - {GetPoffinName(Case.Poffins[index].Type)}";

    public void Save()
    {
        SaveIndex(CurrentIndex);
        Case.Save();
    }

    private void SaveIndex(int index)
    {
        // do nothing, PropertyGrid handles everything
        if (index < 0)
            return;
        Updating = true;
        LB_Poffins.Items[index] = GetPoffinText(index);
        Updating = false;
    }

    private void LoadIndex(int index)
    {
        if (index < 0)
        {
            LB_Poffins.SelectedIndex = 0;
            return;
        }
        PG_Poffins.SelectedObject = Case.Poffins[index];
    }

    private void LB_Poffins_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (Updating)
            return;
        SaveIndex(CurrentIndex);
        LoadIndex(CurrentIndex = LB_Poffins.SelectedIndex);
    }

    private void B_PoffinAll_Click(object sender, EventArgs e)
    {
        Case.FillCase();
        PG_Poffins.Refresh();
        RefreshList();
    }

    private void B_PoffinDel_Click(object sender, EventArgs e)
    {
        Case.DeleteAll();
        PG_Poffins.Refresh();
        RefreshList();
    }

    private void PG_Poffins_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
    {
        SaveIndex(LB_Poffins.SelectedIndex);
    }
}
