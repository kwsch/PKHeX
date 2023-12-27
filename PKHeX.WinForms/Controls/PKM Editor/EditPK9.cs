using System;
using PKHeX.Core;

namespace PKHeX.WinForms.Controls;

public partial class PKMEditor
{
    private void PopulateFieldsPK9()
    {
        if (Entity is not PK9 pk9)
            throw new FormatException(nameof(Entity));

        LoadMisc1(pk9);
        LoadMisc2(pk9);
        LoadMisc3(pk9);
        LoadMisc4(pk9);
        LoadMisc6(pk9);
        SizeCP.LoadPKM(pk9);
        LoadMisc9(pk9);

        LoadPartyStats(pk9);
        UpdateStats();
    }

    private PK9 PreparePK9()
    {
        if (Entity is not PK9 pk9)
            throw new FormatException(nameof(Entity));

        SaveMisc1(pk9);
        SaveMisc2(pk9);
        SaveMisc3(pk9);
        SaveMisc4(pk9);
        SaveMisc6(pk9);
        SaveMisc9(pk9);

        // Toss in Party Stats
        SavePartyStats(pk9);

        pk9.FixMoves();
        pk9.FixRelearn();
        if (ModifyPKM)
            pk9.FixMemories();
        pk9.RefreshChecksum();
        return pk9;
    }
}
