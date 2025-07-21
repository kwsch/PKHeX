using System;
using PKHeX.Core;

namespace PKHeX.WinForms.Controls;

public partial class PKMEditor
{
    private void PopulateFieldsPK7()
    {
        if (Entity is not PK7 pk7)
            throw new FormatException(nameof(Entity));

        LoadMisc1(pk7);
        LoadMisc2(pk7);
        LoadMisc3(pk7);
        LoadMisc4(pk7);
        LoadMisc6(pk7);

        LoadPartyStats(pk7);
        UpdateStats();
    }

    private PK7 PreparePK7()
    {
        if (Entity is not PK7 pk7)
            throw new FormatException(nameof(Entity));

        SaveMisc1(pk7);
        SaveMisc2(pk7);
        SaveMisc3(pk7);
        SaveMisc4(pk7);
        SaveMisc6(pk7);

        // Toss in Party Stats
        SavePartyStats(pk7);

        // Ensure party stats are essentially clean.
        pk7.Data[0xFE..].Clear();
        // Status Condition is allowed to be mutated to pre-set conditions like Burn for Guts.

        pk7.FixMoves();
        pk7.FixRelearn();
        if (ModifyPKM)
            pk7.FixMemories();
        pk7.RefreshChecksum();
        return pk7;
    }

    private void PopulateFieldsPB7()
    {
        if (Entity is not PB7 pk7)
            throw new FormatException(nameof(Entity));

        LoadMisc1(pk7);
        LoadMisc2(pk7);
        LoadMisc3(pk7);
        LoadMisc4(pk7);
        LoadMisc6(pk7);
        LoadAVs(pk7);
        SizeCP.LoadPKM(pk7);

        try
        {
            CAL_ReceivedDateTime.Value = new DateTime(
                pk7.ReceivedYear + 2000, pk7.ReceivedMonth, pk7.ReceivedDay,
                pk7.ReceivedHour, pk7.ReceivedMinute, pk7.ReceivedSecond);
        }
        catch { /* */ }

        LoadPartyStats(pk7);
        UpdateStats();
    }

    private PB7 PreparePB7()
    {
        if (Entity is not PB7 pk7)
            throw new FormatException(nameof(Entity));

        SaveMisc1(pk7);
        SaveMisc2(pk7);
        SaveMisc3(pk7);
        SaveMisc4(pk7);
        SaveMisc6(pk7);

        // Toss in Party Stats
        SavePartyStats(pk7);

        if (pk7.Stat_CP == 0)
            pk7.ResetCP();

        // heal values to original
        pk7.FieldEventFatigue1 = pk7.FieldEventFatigue2 = 100;

        var date = CAL_ReceivedDateTime.Value;
        pk7.ReceivedYear = (byte)(date.Year - 2000);
        pk7.ReceivedMonth = (byte)date.Month;
        pk7.ReceivedDay = (byte)date.Day;
        pk7.ReceivedHour = (byte)date.Hour;
        pk7.ReceivedMinute = (byte)date.Minute;
        pk7.ReceivedSecond = (byte)date.Second;

        pk7.FixMoves();
        pk7.FixRelearn();
        if (ModifyPKM)
            pk7.FixMemories();
        pk7.RefreshChecksum();
        return pk7;
    }
}
