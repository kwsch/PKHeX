using System;
using PKHeX.Core;

namespace PKHeX.WinForms.Controls;

public partial class PKMEditor
{
    private void PopulateFieldsPK6()
    {
        if (Entity is not PK6 pk6)
            throw new FormatException(nameof(Entity));

        LoadMisc1(pk6);
        LoadMisc2(pk6);
        LoadMisc3(pk6);
        LoadMisc4(pk6);
        LoadMisc6(pk6);

        CB_GroundTile.SelectedValue = pk6.Gen4 ? (int)pk6.GroundTile : 0;
        CB_GroundTile.Visible = Label_GroundTile.Visible = pk6.Gen4;

        LoadPartyStats(pk6);
        UpdateStats();
    }

    private PK6 PreparePK6()
    {
        if (Entity is not PK6 pk6)
            throw new FormatException(nameof(Entity));

        SaveMisc1(pk6);
        SaveMisc2(pk6);
        SaveMisc3(pk6);
        SaveMisc4(pk6);
        SaveMisc6(pk6);

        pk6.GroundTile = (GroundTileType)WinFormsUtil.GetIndex(CB_GroundTile);

        // Toss in Party Stats
        SavePartyStats(pk6);

        // Ensure party stats are essentially clean.
        pk6.Data.AsSpan(0xFE).Clear();
        // Status Condition is allowed to be mutated to pre-set conditions like Burn for Guts.

        pk6.FixMoves();
        pk6.FixRelearn();
        if (ModifyPKM)
            pk6.FixMemories();
        pk6.RefreshChecksum();
        return pk6;
    }
}
