using System;
using PKHeX.Core;

namespace PKHeX.WinForms.Controls;

public partial class PKMEditor
{
    private void PopulateFieldsPK4()
    {
        if (Entity is not G4PKM pk4)
            throw new FormatException(nameof(Entity));

        LoadMisc1(pk4);
        LoadMisc2(pk4);
        LoadMisc3(pk4);
        LoadMisc4(pk4);

        CB_GroundTile.SelectedValue = pk4.Gen4 ? (int)pk4.GroundTile : 0;
        CB_GroundTile.Visible = Label_GroundTile.Visible = Entity.Gen4;

        if (HaX)
            DEV_Ability.SelectedValue = pk4.Ability;
        else
            LoadAbility4(pk4);

        // Minor properties
        ShinyLeaf.SetValue(pk4.ShinyLeaf);

        LoadPartyStats(pk4);
        UpdateStats();
    }

    private G4PKM PreparePK4()
    {
        if (Entity is not G4PKM pk4)
            throw new FormatException(nameof(Entity));

        SaveMisc1(pk4);
        SaveMisc2(pk4);
        SaveMisc3(pk4);
        SaveMisc4(pk4);

        pk4.GroundTile = (GroundTileType)WinFormsUtil.GetIndex(CB_GroundTile);

        // Minor properties
        pk4.ShinyLeaf = ShinyLeaf.GetValue();

        SavePartyStats(pk4);
        pk4.FixMoves();
        pk4.RefreshChecksum();
        return pk4;
    }
}
