using System;
using PKHeX.Core;

namespace PKHeX.WinForms.Controls;

public partial class PKMEditor
{
    private void PopulateFieldsPK8()
    {
        if (Entity is not PK8 pk8)
            throw new FormatException(nameof(Entity));

        LoadMisc1(pk8);
        LoadMisc2(pk8);
        LoadMisc3(pk8);
        LoadMisc4(pk8);
        LoadMisc6(pk8);
        SizeCP.LoadPKM(pk8);
        LoadMisc8(pk8);

        LoadPartyStats(pk8);
        UpdateStats();
    }

    private PK8 PreparePK8()
    {
        if (Entity is not PK8 pk8)
            throw new FormatException(nameof(Entity));

        SaveMisc1(pk8);
        SaveMisc2(pk8);
        SaveMisc3(pk8);
        SaveMisc4(pk8);
        SaveMisc6(pk8);
        SaveMisc8(pk8);

        // Toss in Party Stats
        SavePartyStats(pk8);

        pk8.FixMoves();
        pk8.FixRelearn();
        if (ModifyPKM)
            pk8.FixMemories();
        pk8.RefreshChecksum();
        return pk8;
    }

    private PB8 PreparePB8()
    {
        if (Entity is not PB8 pk8)
            throw new FormatException(nameof(Entity));

        SaveMisc1(pk8);
        SaveMisc2(pk8);
        SaveMisc3(pk8);
        SaveMisc4(pk8);
        SaveMisc6(pk8);
        SaveMisc8(pk8);

        // Toss in Party Stats
        SavePartyStats(pk8);

        pk8.FixMoves();
        pk8.FixRelearn();
        if (ModifyPKM)
            pk8.FixMemories();
        pk8.RefreshChecksum();
        return pk8;
    }

    private void PopulateFieldsPB8()
    {
        if (Entity is not PB8 pk8)
            throw new FormatException(nameof(Entity));

        LoadMisc1(pk8);
        LoadMisc2(pk8);
        LoadMisc3(pk8);
        LoadMisc4(pk8);
        LoadMisc6(pk8);
        SizeCP.LoadPKM(pk8);
        LoadMisc8(pk8);

        LoadPartyStats(pk8);
        UpdateStats();
    }

    private PA8 PreparePA8()
    {
        if (Entity is not PA8 pk8)
            throw new FormatException(nameof(Entity));

        SaveMisc1(pk8);
        SaveMisc2(pk8);
        SaveMisc3(pk8);
        SaveMisc4(pk8);
        SaveMisc6(pk8);
        SaveMisc8(pk8);

        // Toss in Party Stats
        SavePartyStats(pk8);

        pk8.FixMoves();
        pk8.FixRelearn();
        if (ModifyPKM)
            pk8.FixMemories();
        pk8.RefreshChecksum();
        return pk8;
    }

    private void PopulateFieldsPA8()
    {
        if (Entity is not PA8 pk8)
            throw new FormatException(nameof(Entity));

        LoadMisc1(pk8);
        LoadMisc2(pk8);
        LoadMisc3(pk8);
        LoadMisc4(pk8);
        LoadMisc6(pk8);
        LoadGVs(pk8);
        SizeCP.LoadPKM(pk8);
        LoadMisc8(pk8);

        LoadPartyStats(pk8);
        UpdateStats();
    }
}
