using System;
using static PKHeX.Core.DexBlockMode9;

namespace PKHeX.Core;

/// <summary>
/// Pokédex structure used for <see cref="GameVersion.SV"/>.
/// </summary>>
public sealed class Zukan9(SAV9SV sav, SCBlock paldea, SCBlock kitakami) : ZukanBase<SAV9SV>(sav, default)
{
    public readonly Zukan9Paldea DexPaldea = new(sav, paldea);
    public readonly Zukan9Kitakami DexKitakami = new(sav, kitakami);
    private readonly DexBlockMode9 Mode = kitakami.Data.Length != 0 ? Kitakami : Paldea;

    // Starting in 2.0.1, the developers have dummied out the old Paldea Pokédex block and exclusively use the new Kitakami block.

    /// <summary>
    /// Checks how much DLC patches have been installed by detecting if DLC blocks are present.
    /// </summary>
    public int GetRevision() => (int)Mode;

    public override bool GetSeen(ushort species) => Mode switch
    {
        Paldea => DexPaldea.GetSeen(species),
        Kitakami => DexKitakami.GetSeen(species),
        _ => false,
    };

    public override bool GetCaught(ushort species) => Mode switch
    {
        Paldea => DexPaldea.GetCaught(species),
        Kitakami => DexKitakami.GetCaught(species),
        _ => false,
    };

    #region Inherited
    public override void SetDex(PKM pk)
    {
        if (pk.IsEgg) // do not add
            return;
        var species = pk.Species;
        var form = pk.Form;
        var pt = SAV.Personal;
        if (!pt.IsPresentInGame(species, form))
            return;

        // Don't register bad species-form data for DLC-less saves.
        var pi = pt.GetFormEntry(species, form);
        if (SAV.SaveRevision == 0 && pi.DexGroup > 1)
            return;
        if (SAV.SaveRevision == 1 && pi.DexGroup > 2)
            return;

        Register(pk, species, form, Mode);
    }

    private void Register(PKM pk, ushort species, byte form, DexBlockMode9 dex)
    {
        if (dex == Paldea)
            DexPaldea.Register(pk, species, form);
        else if (dex == Kitakami)
            DexKitakami.Register(pk, species, form);
        else
            throw new ArgumentOutOfRangeException(nameof(dex), dex, null);
    }

    public (byte Group, ushort Index) GetDexIndex(ushort species)
    {
        var pt = SAV.Personal;
        // For each form including form 0, check the dex index.
        var pi = pt.GetFormEntry(species, 0);
        for (byte f = 0; f <= pi.FormCount; f++)
        {
            pi = pt.GetFormEntry(species, f);
            if (pi.DexPaldea != 0)
                return (1, pi.DexPaldea);
            if (pi.DexKitakami != 0)
                return (2, pi.DexKitakami);
            if (pi.DexBlueberry != 0)
                return (3, pi.DexBlueberry);
        }
        return default;
    }

    public override void SeenNone()
    {
        DexPaldea.SeenNone();
        DexKitakami.SeenNone();
    }

    public override void CaughtNone()
    {
        DexPaldea.CaughtNone();
        DexKitakami.CaughtNone();
    }

    public override void SeenAll(bool shinyToo = false)
    {
        SetAllSeen(true, shinyToo);
    }

    private void SeenAll(ushort species, byte formCount, bool value = true, bool shinyToo = false)
    {
        var dex = Mode;
        if (dex == Paldea)
            DexPaldea.SeenAll(species, formCount, value, shinyToo);
        else if (dex == Kitakami)
            DexKitakami.SeenAll(species, formCount, value, shinyToo);
        else
            throw new ArgumentOutOfRangeException(nameof(dex), dex, null);
    }

    public override void CompleteDex(bool shinyToo = false)
    {
        for (ushort species = 0; species <= SAV.MaxSpeciesID; species++)
        {
            if (!SAV.Personal.IsSpeciesInGame(species))
                continue;
            if (GetDexIndex(species).Index == 0)
                continue;
            SetDexEntryAll(species, shinyToo);
        }
    }

    public override void CaughtAll(bool shinyToo = false)
    {
        SeenAll(shinyToo);
        for (ushort species = 0; species <= SAV.MaxSpeciesID; species++)
        {
            if (!SAV.Personal.IsSpeciesInGame(species))
                continue;
            if (GetDexIndex(species).Index == 0)
                continue;
            SetAllCaught(species, true, shinyToo);
        }
    }

    private void SetAllCaught(ushort species, bool value = true, bool shinyToo = false)
    {
        var dex = Mode;
        if (dex == Paldea)
            DexPaldea.SetAllCaught(species, value, shinyToo);
        else if (dex == Kitakami)
            DexKitakami.SetAllCaught(species, value, shinyToo);
        else
            throw new ArgumentOutOfRangeException(nameof(dex), dex, null);
    }

    public override void SetAllSeen(bool value = true, bool shinyToo = false)
    {
        var pt = SAV.Personal;
        for (ushort species = 0; species < SAV.MaxSpeciesID; species++)
        {
            if (value && GetDexIndex(species).Index == 0)
                continue;
            var pi = pt[species];
            SeenAll(species, pi.FormCount, value, shinyToo);
        }
    }

    private void SetAllSeen(ushort species, bool value = true, bool shinyToo = false)
    {
        var pi = SAV.Personal[species];
        var fc = pi.FormCount;
        SeenAll(species, fc, value, shinyToo);
    }

    public override void SetDexEntryAll(ushort species, bool shinyToo = false)
    {
        SetAllSeen(species, true, shinyToo);
        SetAllCaught(species, true, shinyToo);
    }

    public override void ClearDexEntryAll(ushort species)
    {
        var dex = Mode;
        if (dex == Paldea)
            DexPaldea.ClearDexEntryAll(species);
        else if (dex == Kitakami)
            DexKitakami.ClearDexEntryAll(species);
        else
            throw new ArgumentOutOfRangeException(nameof(dex), dex, null);
    }

    #endregion
}
