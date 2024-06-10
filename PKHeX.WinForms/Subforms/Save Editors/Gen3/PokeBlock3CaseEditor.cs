using System;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public partial class Pokeblock3CaseEditor : UserControl
{
    public Pokeblock3CaseEditor() => InitializeComponent();
    private PokeBlock3Case Case = null!; // initialized on load
    private int CurrentIndex = -1;
    private bool Updating;

    private readonly string[] ItemNames = Util.GetStringList("pokeblock3", Main.CurrentLanguage);

    public void Initialize(IGen3Hoenn sav)
    {
        Case = sav.PokeBlocks;

        LB_Pokeblocks.Items.Clear();
        for (int i = 0; i < Case.Blocks.Length; i++)
            LB_Pokeblocks.Items.Add(GetPokeblockText(i));

        LB_Pokeblocks.SelectedIndex = 0;
    }

    private void RefreshList()
    {
        Updating = true;
        for (int i = 0; i < Case.Blocks.Length; i++)
            LB_Pokeblocks.Items[i] = GetPokeblockText(i);
        Updating = false;
    }

    private string GetPokeblockName(PokeBlock3Color color)
    {
        var index = (uint)color;
        if (index >= ItemNames.Length)
            index = 0;
        return ItemNames[index];
    }

    private string GetPokeblockText(int index) => $"{index + 1:00} - {GetPokeblockName(Case.Blocks[index].Color)}";

    public void Save(IGen3Hoenn sav)
    {
        SaveIndex(CurrentIndex);
        sav.PokeBlocks = Case;
    }

    private void SaveIndex(int index)
    {
        // do nothing, PropertyGrid handles everything
        if (index < 0)
            return;
        Updating = true;
        LB_Pokeblocks.Items[index] = GetPokeblockText(index);
        Updating = false;
    }

    private void LoadIndex(int index)
    {
        if (index < 0)
        {
            LB_Pokeblocks.SelectedIndex = 0;
            return;
        }
        PG_Pokeblocks.SelectedObject = Case.Blocks[index];
    }

    private void LB_Pokeblocks_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (Updating)
            return;
        SaveIndex(CurrentIndex);
        LoadIndex(CurrentIndex = LB_Pokeblocks.SelectedIndex);
    }

    private void B_PokeblockAll_Click(object sender, EventArgs e)
    {
        Case.MaximizeAll(true);
        PG_Pokeblocks.Refresh();
        RefreshList();
    }

    private void B_PokeblockDel_Click(object sender, EventArgs e)
    {
        Case.DeleteAll();
        PG_Pokeblocks.Refresh();
        RefreshList();
    }

    private void PG_Pokeblocks_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
    {
        SaveIndex(LB_Pokeblocks.SelectedIndex);
    }
}
