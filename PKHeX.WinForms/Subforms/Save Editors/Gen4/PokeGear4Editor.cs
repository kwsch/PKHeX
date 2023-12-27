using System;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms;

public partial class PokeGear4Editor : UserControl
{
    public PokeGear4Editor() => InitializeComponent();
    private PokegearNumber[] Rolodex = null!;
    private SAV4HGSS SAV = null!;

    public void Initialize(SAV4HGSS sav)
    {
        SAV = sav;
        RefreshList();
    }

    public void Save() => SAV.SetPokeGearRoloDex(Rolodex);

    private void RefreshList()
    {
        PG_Rolodex.SelectedObject = Rolodex = SAV.GetPokeGearRoloDex().ToArray();
        PG_Rolodex.Refresh();
    }

    private void B_GiveAll_Click(object sender, EventArgs e)
    {
        SAV.PokeGearUnlockAllCallers();
        RefreshList();
        System.Media.SystemSounds.Asterisk.Play();
    }

    private void B_GiveAllNoTrainers_Click(object sender, EventArgs e)
    {
        SAV.PokeGearUnlockAllCallersNoTrainers();
        RefreshList();
        System.Media.SystemSounds.Asterisk.Play();
    }

    private void B_DeleteAll_Click(object sender, EventArgs e)
    {
        SAV.PokeGearClearAllCallers();
        RefreshList();
        System.Media.SystemSounds.Asterisk.Play();
    }

    private void PG_Rolodex_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
    {
    }
}
