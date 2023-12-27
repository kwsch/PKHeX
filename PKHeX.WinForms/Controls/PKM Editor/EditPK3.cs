using System;
using PKHeX.Core;

namespace PKHeX.WinForms.Controls;

public partial class PKMEditor
{
    private void PopulateFieldsPK3()
    {
        if (Entity is not G3PKM pk3)
            throw new FormatException(nameof(Entity));

        LoadMisc1(pk3);
        LoadMisc2(pk3);
        LoadMisc3(pk3);

        CB_Ability.SelectedIndex = pk3.AbilityBit && CB_Ability.Items.Count > 1 ? 1 : 0;
        if (pk3 is IShadowCapture s)
            LoadShadow3(s);

        LoadPartyStats(pk3);
        UpdateStats();
    }

    private G3PKM PreparePK3()
    {
        if (Entity is not G3PKM pk3)
            throw new FormatException(nameof(Entity));

        SaveMisc1(pk3);
        SaveMisc2(pk3);
        SaveMisc3(pk3);

        pk3.AbilityBit = CB_Ability.SelectedIndex != 0;
        if (Entity is IShadowCapture ck3)
            SaveShadow3(ck3);

        SavePartyStats(pk3);
        pk3.FixMoves();
        pk3.RefreshChecksum();
        return pk3;
    }
}
