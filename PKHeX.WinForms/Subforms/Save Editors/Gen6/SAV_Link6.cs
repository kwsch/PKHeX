using System;
using System.IO;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public partial class SAV_Link6 : Form
{
    private readonly SaveFile Origin;
    private readonly ISaveBlock6Main SAV;

    private PL6 Gifts;

    public SAV_Link6(SaveFile sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (ISaveBlock6Main)(Origin = sav).Clone();
        var filtered = GameInfo.FilteredSources;

        foreach (var cb in (ComboBox[])[CB_Item1, CB_Item2, CB_Item3, CB_Item4, CB_Item5, CB_Item6])
        {
            cb.InitializeBinding();
            cb.DataSource = new BindingSource(filtered.Items, null);
        }
        Gifts = SAV.Link.Gifts;
        LoadLinkData();
    }

    private void B_Save_Click(object sender, EventArgs e)
    {
        SaveLinkData();
        SAV.Link.RefreshChecksum();
        Origin.CopyChangesFrom((SaveFile)SAV);
        Close();
    }

    private void B_Cancel_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void B_Import_Click(object sender, EventArgs e)
    {
        using var ofd = new OpenFileDialog();
        ofd.Filter = PL6.Filter;
        if (ofd.ShowDialog() != DialogResult.OK)
            return;

        if (new FileInfo(ofd.FileName).Length != PL6.Size)
        { WinFormsUtil.Alert("Invalid file length"); return; }

        byte[] data = File.ReadAllBytes(ofd.FileName);
        Gifts = new PL6(data);

        LoadLinkData();
        B_Export.Enabled = true;
    }

    private void B_Export_Click(object sender, EventArgs e)
    {
        using var sfd = new SaveFileDialog();
        sfd.Filter = PL6.Filter;
        if (sfd.ShowDialog() != DialogResult.OK)
            return;

        File.WriteAllBytes(sfd.FileName, Gifts.Data.ToArray());
        WinFormsUtil.Alert("Pokémon Link data saved to:" + Environment.NewLine + sfd.FileName);
    }

    private void LoadLinkData()
    {
        RTB_LinkSource.Text = Gifts.Origin;
        CHK_LinkAvailable.Checked = Gifts.Enabled;

        NUD_BP.Value = Gifts.BattlePoints;
        NUD_Pokemiles.Value = Gifts.Pokemiles;

        CB_Item1.SelectedValue = (int)Gifts.Item1;
        CB_Item2.SelectedValue = (int)Gifts.Item2;
        CB_Item3.SelectedValue = (int)Gifts.Item3;
        CB_Item4.SelectedValue = (int)Gifts.Item4;
        CB_Item5.SelectedValue = (int)Gifts.Item5;
        CB_Item6.SelectedValue = (int)Gifts.Item6;

        NUD_Item1.Value = Gifts.Quantity1;
        NUD_Item2.Value = Gifts.Quantity2;
        NUD_Item3.Value = Gifts.Quantity3;
        NUD_Item4.Value = Gifts.Quantity4;
        NUD_Item5.Value = Gifts.Quantity5;
        NUD_Item6.Value = Gifts.Quantity6;

        // Pokemon slots
        TB_PKM1.Text = GetSpecies(Gifts.Entity1.Species);
        TB_PKM2.Text = GetSpecies(Gifts.Entity2.Species);
        TB_PKM3.Text = GetSpecies(Gifts.Entity3.Species);
        TB_PKM4.Text = GetSpecies(Gifts.Entity4.Species);
        TB_PKM5.Text = GetSpecies(Gifts.Entity5.Species);
        TB_PKM6.Text = GetSpecies(Gifts.Entity6.Species);
    }

    private static string GetSpecies(ushort species)
    {
        var arr = GameInfo.Strings.Species;
        if (species < arr.Count)
            return arr[species];
        return species.ToString();
    }

    private void SaveLinkData()
    {
        Gifts.Origin = RTB_LinkSource.Text;
        Gifts.Enabled = CHK_LinkAvailable.Checked;

        Gifts.BattlePoints = (ushort)NUD_BP.Value;
        Gifts.Pokemiles = (ushort)NUD_Pokemiles.Value;

        Gifts.Item1 = (ushort)WinFormsUtil.GetIndex(CB_Item1);
        Gifts.Item2 = (ushort)WinFormsUtil.GetIndex(CB_Item2);
        Gifts.Item3 = (ushort)WinFormsUtil.GetIndex(CB_Item3);
        Gifts.Item4 = (ushort)WinFormsUtil.GetIndex(CB_Item4);
        Gifts.Item5 = (ushort)WinFormsUtil.GetIndex(CB_Item5);
        Gifts.Item6 = (ushort)WinFormsUtil.GetIndex(CB_Item6);

        Gifts.Quantity1 = (byte)NUD_Item1.Value;
        Gifts.Quantity2 = (byte)NUD_Item2.Value;
        Gifts.Quantity3 = (byte)NUD_Item3.Value;
        Gifts.Quantity4 = (byte)NUD_Item4.Value;
        Gifts.Quantity5 = (byte)NUD_Item5.Value;
        Gifts.Quantity6 = (byte)NUD_Item6.Value;
    }
}
