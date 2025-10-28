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

    private void PopulateFieldsPA9()
    {
        if (Entity is not PA9 pa9)
            throw new FormatException(nameof(Entity));

        LoadMisc1(pa9);
        LoadMisc2(pa9);
        LoadMisc3(pa9);
        LoadMisc4(pa9);
        LoadMisc6(pa9);
        SizeCP.LoadPKM(pa9);
        LoadMisc9(pa9);

        LoadPartyStats(pa9);
        UpdateStats();
    }

    private PA9 PreparePA9()
    {
        if (Entity is not PA9 pa9)
            throw new FormatException(nameof(Entity));

        SaveMisc1(pa9);
        SaveMisc2(pa9);
        SaveMisc3(pa9);
        SaveMisc4(pa9);
        SaveMisc6(pa9);
        SaveMisc9(pa9);

        // Toss in Party Stats
        SavePartyStats(pa9);

        pa9.FixMoves();
        pa9.FixRelearn();
        if (ModifyPKM)
            pa9.FixMemories();
        pa9.RefreshChecksum();
        return pa9;
    }
}
