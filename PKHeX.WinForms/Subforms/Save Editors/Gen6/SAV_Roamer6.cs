using PKHeX.Core;
using System;
using System.Windows.Forms;

namespace PKHeX.WinForms;

public partial class SAV_Roamer6 : Form
{
    private const int SpeciesOffset = 144;
    private const int StarterChoiceIndex = 48;
    private readonly SAV6XY Origin;
    private readonly SAV6XY SAV;
    private readonly Roamer6 roamer;

    public SAV_Roamer6(SAV6XY sav)
    {
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        SAV = (SAV6XY)(Origin = sav).Clone();
        roamer = SAV.Encount.Roamer;

        var species = GameInfo.Strings.specieslist;
        CB_Species.Items.AddRange([species[(int)Species.Articuno], species[(int)Species.Zapdos], species[(int)Species.Moltres]]);
        CB_RoamState.Items.AddRange(["Inactive", "Roaming", "Stationary", "Defeated", "Captured"]);

        CB_Species.SelectedIndex = GetInitialIndex(sav);
        NUD_TimesEncountered.Value = roamer.TimesEncountered;
        CB_RoamState.SelectedIndex = (int)roamer.RoamStatus;
    }

    private int GetInitialIndex(SAV6XY sav)
    {
        if (roamer.Species != 0)
            return roamer.Species - SpeciesOffset;
        // Roamer Species is not set if the player hasn't beaten the league so derive the species from the starter choice
        return sav.EventWork.GetWork(StarterChoiceIndex);
    }

    private void CB_Species_SelectedIndexChanged(object sender, EventArgs e) => roamer.Species = (ushort)(SpeciesOffset + CB_Species.SelectedIndex);

    private void NUD_TimesEncountered_ValueChanged(object sender, EventArgs e) => roamer.TimesEncountered = (uint)NUD_TimesEncountered.Value;

    private void CB_RoamState_SelectedIndexChanged(object sender, EventArgs e) => roamer.RoamStatus = (Roamer6State)CB_RoamState.SelectedIndex;

    private void B_Cancel_Click(object sender, EventArgs e) => Close();

    private void B_Save_Click(object sender, EventArgs e)
    {
        Origin.CopyChangesFrom(SAV);
        Close();
    }
}
