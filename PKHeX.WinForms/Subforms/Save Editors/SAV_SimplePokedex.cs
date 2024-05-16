using System;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public partial class SAV_SimplePokedex : Form
{
    private readonly SaveFile SAV;
    private readonly int MaxSpeciesID;

    public SAV_SimplePokedex(SaveFile sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = sav;
        var count = MaxSpeciesID = SAV.MaxSpeciesID;

        var speciesNames = GameInfo.Strings.specieslist.AsSpan(1, count);
        AddAllSpecies(speciesNames);
        LoadAllFlags(SAV, count);
    }

    private void AddAllSpecies(ReadOnlySpan<string> speciesNames)
    {
        for (int i = 0; i < speciesNames.Length; i++)
        {
            CLB_Seen.Items.Add(speciesNames[i]);
            CLB_Caught.Items.Add(speciesNames[i]);
        }
    }

    private void LoadAllFlags(SaveFile sav, int count)
    {
        for (int i = 0; i < count; i++)
        {
            ushort species = (ushort)(i + 1);
            CLB_Seen.SetItemChecked(i, sav.GetSeen(species));
            CLB_Caught.SetItemChecked(i, sav.GetCaught(species));
        }
    }

    private void SaveAllFlags(SaveFile sav, int count)
    {
        for (int i = 0; i < count; i++)
        {
            ushort species = (ushort)(i + 1);
            sav.SetSeen(species, CLB_Seen.GetItemChecked(i));
            sav.SetCaught(species, CLB_Caught.GetItemChecked(i));
        }
    }

    private void B_Save_Click(object sender, EventArgs e)
    {
        SaveAllFlags(SAV, MaxSpeciesID);
        SAV.State.Edited = true;
        Close();
    }

    private void B_Cancel_Click(object sender, EventArgs e) => Close();

    private void B_SeenAll_Click(object sender, EventArgs e)
    {
        for (int i = 0; i < MaxSpeciesID; i++)
            CLB_Seen.SetItemChecked(i, true);
    }

    private void B_SeenNone_Click(object sender, EventArgs e)
    {
        for (int i = 0; i < MaxSpeciesID; i++)
            CLB_Seen.SetItemChecked(i, false);
    }

    private void B_CaughtAll_Click(object sender, EventArgs e)
    {
        for (int i = 0; i < MaxSpeciesID; i++)
            CLB_Caught.SetItemChecked(i, true);
    }

    private void B_CaughtNone_Click(object sender, EventArgs e)
    {
        for (int i = 0; i < MaxSpeciesID; i++)
            CLB_Caught.SetItemChecked(i, false);
    }
}
