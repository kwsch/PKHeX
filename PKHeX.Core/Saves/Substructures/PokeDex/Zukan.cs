using System;

namespace PKHeX.Core;

/// <summary>
/// Base class for Pokédex logic operations.
/// </summary>
public abstract class ZukanBase<T>(T sav, Memory<byte> dex) : SaveBlock<T>(sav, dex)
    where T : SaveFile
{
    #region Overall Info
    /// <summary> Count of unique Species Seen </summary>
    public int SeenCount
    {
        get
        {
            int ctr = 0;
            for (ushort i = 1; i <= SAV.MaxSpeciesID; i++)
            {
                if (GetSeen(i))
                    ctr++;
            }
            return ctr;
        }
    }

    /// <summary> Count of unique Species Caught (Owned) </summary>
    public int CaughtCount
    {
        get
        {
            int ctr = 0;
            for (ushort i = 1; i <= SAV.MaxSpeciesID; i++)
            {
                if (GetCaught(i))
                    ctr++;
            }
            return ctr;
        }
    }

    public decimal PercentSeen => (decimal)SeenCount / SAV.MaxSpeciesID;
    public decimal PercentCaught => (decimal)CaughtCount / SAV.MaxSpeciesID;
    #endregion

    /// <summary> Gets if the Species has been Seen by the player. </summary>
    public abstract bool GetSeen(ushort species);
    /// <summary> Gets if the Species has been Caught (Owned) by the player. </summary>
    public abstract bool GetCaught(ushort species);

    /// <summary> Adds the Pokémon's information to the Pokédex. </summary>
    public abstract void SetDex(PKM pk);

    #region Overall Manipulation
    public abstract void SeenNone();
    public abstract void CaughtNone();

    public abstract void SeenAll(bool shinyToo = false);
    public abstract void CompleteDex(bool shinyToo = false);
    public abstract void CaughtAll(bool shinyToo = false);
    public abstract void SetAllSeen(bool value = true, bool shinyToo = false);

    public abstract void SetDexEntryAll(ushort species, bool shinyToo = false);
    public abstract void ClearDexEntryAll(ushort species);
    #endregion
}

/// <summary>
/// Base class for Pokédex operations, exposing the shared structure features used by Generations 5, 6, and 7.
/// </summary>
public abstract class Zukan<T>(T sav, Memory<byte> raw, int langflag) : ZukanBase<T>(sav, raw)
    where T : SaveFile
{
    protected readonly int PokeDexLanguageFlags = langflag;

    protected abstract int OFS_SEEN { get; }
    protected abstract int OFS_CAUGHT { get; }
    protected abstract int BitSeenSize { get; }
    protected abstract int DexLangFlagByteCount { get; }
    protected abstract int DexLangIDCount { get; }
    protected abstract int GetDexLangFlag(int lang);

    protected abstract bool GetSaneFormsToIterate(ushort species, out int formStart, out int formEnd, int formIn);
    protected virtual void SetSpindaDexData(PKM pk, bool alreadySeen) { }
    protected abstract void SetAllDexFlagsLanguage(int bit, int lang, bool value = true);
    protected abstract void SetAllDexSeenFlags(int baseBit, byte b, byte gender, bool isShiny, bool value = true);

    protected bool GetFlag(int ofs, int bitIndex) => SAV.GetFlag(Data, ofs + (bitIndex >> 3), bitIndex);
    protected void SetFlag(int ofs, int bitIndex, bool value = true) => SAV.SetFlag(Data, ofs + (bitIndex >> 3), bitIndex, value);

    public override bool GetCaught(ushort species) => GetFlag(OFS_CAUGHT, species - 1);
    public virtual void SetCaught(ushort species, bool value = true) => SetFlag(OFS_CAUGHT, species - 1, value);

    public override bool GetSeen(ushort species)
    {
        // check all 4 seen flags (gender/shiny)
        for (int i = 0; i < 4; i++)
        {
            if (GetSeen(species, i))
                return true;
        }
        return false;
    }

    public bool GetSeen(ushort species, int bitRegion) => GetFlag(OFS_SEEN + (bitRegion * BitSeenSize), species - 1);
    public void SetSeen(ushort species, int bitRegion, bool value) => SetFlag(OFS_SEEN + (bitRegion * BitSeenSize), species - 1, value);

    public bool GetDisplayed(int bit, int bitRegion) => GetFlag(OFS_SEEN + ((bitRegion + 4) * BitSeenSize), bit);
    public void SetDisplayed(int bit, int bitRegion, bool value) => SetFlag(OFS_SEEN + ((bitRegion + 4) * BitSeenSize), bit, value);

    public bool GetLanguageFlag(int bit, int lang) => GetFlag(PokeDexLanguageFlags, (bit * DexLangIDCount) + lang);
    public void SetLanguageFlag(int bit, int lang, bool value) => SetFlag(PokeDexLanguageFlags, (bit * DexLangIDCount) + lang, value);

    public virtual void SetSeen(ushort species, bool value = true)
    {
        if (!value)
        {
            ClearSeen(species);
            return;
        }

        // check all 4 seen flags (gender/shiny)
        for (int i = 0; i < 4; i++)
        {
            if (GetFlag(OFS_SEEN + (i * BitSeenSize), species - 1))
                return;
        }
        byte gender = (byte)(SAV.Personal[species].RandomGender() & 1);
        SetAllDexSeenFlags(species - 1, 0, gender, false);
    }

    private void ClearSeen(ushort species)
    {
        SetCaught(species, false);
        for (int i = 0; i < 4; i++)
            SetFlag(OFS_SEEN + (i * BitSeenSize), species - 1, false);
    }

    public override void SetDex(PKM pk)
    {
        if (pk.Species - 1u >= SAV.MaxSpeciesID) // out of range
            return;
        if (pk.IsEgg) // do not add
            return;

        var species = pk.Species;
        if (species == (int)Species.Spinda)
            SetSpindaDexData(pk, GetSeen(species));

        int bit = pk.Species - 1;
        var form = pk.Form;
        byte gender = (byte)(pk.Gender & 1);
        bool shiny = pk.IsShiny;
        int lang = pk.Language;
        SetDex(species, bit, form, gender, shiny, lang);
    }

    protected virtual void SetDex(ushort species, int bit, byte form, byte gender, bool shiny, int lang)
    {
        SetCaught(species); // Set the Owned Flag
        SetAllDexSeenFlags(bit, form, gender, shiny); // genderless -> male
        SetAllDexFlagsLanguage(bit, lang);
    }

    protected void SetDexFlags(int baseBit, int formBit, byte gender, int shiny, bool value = true)
    {
        int shift = (gender & 1) | (shiny << 1);

        // Set the [Species/Gender/Shiny] Seen Flag
        SetFlag(OFS_SEEN + (shift * BitSeenSize), baseBit, value);

        // Set the Display flag if none are set
        SetDisplayedFlag(baseBit, formBit, value, shift);
    }

    protected virtual void SetDisplayedFlag(int baseBit, int formBit, bool value, int shift)
    {
        var bit = formBit >= 0 ? formBit : baseBit;
        if (!value)
        {
            SetDisplayed(bit, shift, false);
            return;
        }

        bool displayed = GetIsSpeciesFormAnyDisplayed(baseBit, formBit);
        if (displayed)
            return; // no need to set another bit

        SetDisplayed(bit, shift, true);
    }

    private bool GetIsSpeciesFormAnyDisplayed(int baseBit, int formBit)
    {
        // Check Displayed Status for base form
        for (int i = 0; i < 4; i++)
        {
            if (GetDisplayed(baseBit, i))
                return true;
        }
        if (baseBit == formBit)
            return false;

        // If form is not base form, check form too
        for (int i = 0; i < 4; i++)
        {
            if (GetDisplayed(formBit, i))
                return true;
        }
        return false;
    }

    // Bulk Manipulation
    public override void SeenNone() => SetDexEntriesAll(false, shinyToo: true);
    public override void CaughtNone() => SetAllCaught(false, true);
    public override void SeenAll(bool shinyToo = false) => SetAllSeen(shinyToo);
    public override void CompleteDex(bool shinyToo = false) => SetDexEntriesAll(shinyToo: shinyToo);

    public override void CaughtAll(bool shinyToo = false)
    {
        SetAllSeen(true, shinyToo);
        SetAllCaught(true, shinyToo);
    }

    public void SetAllCaught(bool value = true, bool shinyToo = false)
    {
        for (ushort i = 1; i <= SAV.MaxSpeciesID; i++)
        {
            SetCaught(i, value); // Set the Owned Flag
            SetSeenSingle(i, value, shinyToo);
        }
    }

    public override void SetAllSeen(bool value = true, bool shinyToo = false)
    {
        for (ushort i = 1; i <= SAV.MaxSpeciesID; i++)
            SetSeenSingle(i, value, shinyToo);
    }

    public override void SetDexEntryAll(ushort species, bool shinyToo = false)
    {
        SetSeenSingle(species, true, shinyToo);
        SetCaughtSingle(species);
    }

    public override void ClearDexEntryAll(ushort species)
    {
        SetSeenSingle(species, false);
        SetCaughtSingle(species, false);
    }

    public void SetDexEntriesAll(bool value = true, int max = -1, bool shinyToo = false)
    {
        if (max <= 0)
            max = SAV.MaxSpeciesID;

        for (ushort i = 1; i <= max; i++)
        {
            SetSeenSingle(i, value, shinyToo);
            SetCaughtSingle(i, value);
        }
    }

    public void SetCaughtSingle(ushort species, bool value = true)
    {
        SetCaught(species, value);
        int baseBit = species - 1;
        SetAllDexFlagsLanguage(baseBit, value);
    }

    public void SetSeenSingle(ushort species, bool seen = true, bool shinyToo = false)
    {
        SetSeen(species, seen);

        var entry = SAV.Personal[species];
        int baseBit = species - 1;
        var fc = entry.FormCount;
        for (byte f = 0; f < fc; f++)
        {
            if (!entry.OnlyFemale)
            {
                SetAllDexSeenFlags(baseBit, f, 0, false, seen);
                if (shinyToo)
                    SetAllDexSeenFlags(baseBit, f, 0, true, seen);
            }
            if (entry is { OnlyMale: false, Genderless: false })
            {
                SetAllDexSeenFlags(baseBit, f, 1, false, seen);
                if (shinyToo)
                    SetAllDexSeenFlags(baseBit, f, 1, true, seen);
            }
        }
    }

    protected void SetAllDexFlagsLanguage(int bit, bool value = true)
    {
        for (int i = 1; i <= DexLangIDCount + 1; i++)
            SetAllDexFlagsLanguage(bit, i, value);
    }
}
