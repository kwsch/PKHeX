using System;
using PKHeX.Core;

namespace PKHeX.WinForms.Controls;

public partial class PKMEditor
{
    private void PopulateFieldsPK5()
    {
        if (Entity is not PK5 pk5)
            throw new FormatException(nameof(Entity));

        LoadMisc1(pk5);
        LoadMisc2(pk5);
        LoadMisc3(pk5);
        LoadMisc4(pk5);
        CB_GroundTile.SelectedValue = pk5.Gen4 ? (int)pk5.GroundTile : 0;
        CB_GroundTile.Visible = Label_GroundTile.Visible = pk5.Gen4;
        CHK_NSparkle.Checked = pk5.NSparkle;

        if (HaX)
            DEV_Ability.SelectedValue = pk5.Ability;
        else if (pk5.HiddenAbility)
            CB_Ability.SelectedIndex = CB_Ability.Items.Count - 1;
        else
            LoadAbility4(pk5);

        LoadPartyStats(pk5);
        UpdateStats();
    }

    private PK5 PreparePK5()
    {
        if (Entity is not PK5 pk5)
            throw new FormatException(nameof(Entity));

        SaveMisc1(pk5);
        SaveMisc2(pk5);
        SaveMisc3(pk5);
        SaveMisc4(pk5);

        pk5.GroundTile = (GroundTileType)WinFormsUtil.GetIndex(CB_GroundTile);
        pk5.NSparkle = CHK_NSparkle.Checked;
        if (!HaX)
        {
            pk5.HiddenAbility = CB_Ability.SelectedIndex is not (0 or 1);
        }
        else
        {
            var pi = pk5.PersonalInfo;
            pk5.HiddenAbility = pi.HasHiddenAbility && pk5.Ability == pi.AbilityH;
        }

        SavePartyStats(pk5);
        pk5.FixMoves();
        pk5.RefreshChecksum();
        return pk5;
    }
}
