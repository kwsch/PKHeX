using System;
using System.Collections.Generic;
using System.Diagnostics;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Pokédex structure used for Generation 7 games.
/// </summary>>
public class Zukan7 : Zukan<SaveFile>
{
    private const int MAGIC = 0x2F120F17;
    private const int SIZE_MAGIC = 4;
    private const int SIZE_FLAGS = 4;
    private const int SIZE_MISC = 0x80; // Misc Data (1024 bits)
    private const int SIZE_CAUGHT = 0x68; // 832 bits

    protected sealed override int OFS_CAUGHT => SIZE_MAGIC + SIZE_FLAGS + SIZE_MISC;
    protected sealed override int OFS_SEEN => OFS_CAUGHT + SIZE_CAUGHT;

    protected sealed override int BitSeenSize => 0x8C; // 1120 bits
    protected sealed override int DexLangFlagByteCount => 920; // 0x398 = 817*9, top off the savedata block.
    protected sealed override int DexLangIDCount => 9; // CHT, skipping langID 6 (unused)

    private readonly IList<ushort> FormBaseSpecies;

    public Zukan7(SAV7SM sav, Memory<byte> dex, int langflag) : this(sav, dex, langflag, DexFormUtil.GetDexFormIndexSM) { }
    public Zukan7(SAV7USUM sav, Memory<byte> dex, int langflag) : this(sav, dex, langflag, DexFormUtil.GetDexFormIndexUSUM) { }
    protected Zukan7(SAV7b sav, Memory<byte> dex, int langflag) : this(sav, dex, langflag, DexFormUtil.GetDexFormIndexGG) { }

    private Zukan7(SaveFile sav, Memory<byte> dex, int langflag, Func<ushort, byte, int> form) : base(sav, dex, langflag)
    {
        GetCountFormsPriorTo = form;
        FormBaseSpecies = GetFormIndexBaseSpeciesList();
        Debug.Assert(!SAV.State.Exportable || ReadUInt32LittleEndian(Data) == MAGIC);
    }

    public Func<ushort, byte, int> GetCountFormsPriorTo { get; }

    protected sealed override void SetAllDexSeenFlags(int baseBit, byte form, byte gender, bool isShiny, bool value = true)
    {
        var species = (ushort)(baseBit + 1);
        if (species == (int)Species.Castform)
            isShiny = false;

        // Starting with Gen7, form bits are stored in the same region as the species flags.
        int formStart = form;
        int formEnd = form;
        bool reset = GetSaneFormsToIterate(species, out var fs, out var fe, form);
        if (reset)
            (formStart, formEnd) = (fs, fe);

        int shiny = isShiny ? 1 : 0;
        for (int f = formStart; f <= formEnd; f++)
        {
            int formBit = baseBit;
            if (f > 0) // Override the bit to overwrite
            {
                var fc = SAV.Personal[species].FormCount;
                if (fc > 1) // actually has forms
                {
                    int index = GetCountFormsPriorTo(species, fc);
                    if (index >= 0) // bit index valid
                        formBit = SAV.MaxSpeciesID + index + (f - 1);
                }
            }
            SetDexFlags(baseBit, formBit, gender, shiny, value);
        }
    }

    protected override bool GetSaneFormsToIterate(ushort species, out int formStart, out int formEnd, int formIn)
    {
        return SanitizeFormsToIterate(species, out formStart, out formEnd, formIn, SAV is SAV7USUM);
    }

    public static bool SanitizeFormsToIterate(ushort species, out int formStart, out int formEnd, int formIn, bool USUM)
    {
        // 004AA370 in Moon
        // Simplified in terms of usage -- only overrides to give all the battle forms for a pk
        switch (species)
        {
            case 351: // Castform
                formStart = 0;
                formEnd = 3;
                return true;

            case 421: // Cherrim
            case 555: // Darmanitan
            case 648: // Meloetta
            case 746: // Wishiwashi
            case 778: // Mimikyu
            // Alolans
            case 020: // Raticate
            case 105: // Marowak
                formStart = 0;
                formEnd = 1;
                return true;

            case 735: // Gumshoos
            case 758: // Salazzle
            case 754: // Lurantis
            case 738: // Vikavolt
            case 784: // Kommo-o
            case 752: // Araquanid
            case 777: // Togedemaru
            case 743: // Ribombee
            case 744: // Rockruff
                break;

            case 774 when formIn <= 6: // Minior
                break; // don't give meteor forms except the first

            case 718 when formIn > 1:
                break;
            default:
                int count = USUM ? DexFormUtil.GetDexFormCountUSUM(species) : DexFormUtil.GetDexFormCountSM(species);
                formStart = formEnd = 0;
                return count < formIn;
        }
        formStart = 0;
        formEnd = 0;
        return true;
    }

    protected sealed override int GetDexLangFlag(int lang) => lang switch
    {
        > 10 or 6 or <= 0 => -1, // invalid language
        // skip over langID 0 (unused) => [0-8]
        // skip over langID 6 (unused)
        >= 7 => lang - 2,
        _ => lang - 1,
    };

    protected sealed override void SetSpindaDexData(PKM pk, bool alreadySeen)
    {
        int shift = (pk.Gender & 1) | (pk.IsShiny ? 2 : 0);
        if (alreadySeen) // update?
        {
            var flag1 = (1 << (shift + 4));
            if ((Data[0x84] & flag1) != 0) // Already showing this one
                return;

            var span = Data[(0x8E8 + (shift * 4))..];
            WriteUInt32LittleEndian(span, pk.EncryptionConstant);
            Data[0x84] |= (byte)(flag1 | (1 << shift));
        }
        else if ((Data[0x84] & (1 << shift)) == 0)
        {
            var span = Data[(0x8E8 + (shift * 4))..];
            WriteUInt32LittleEndian(span, pk.EncryptionConstant);
            Data[0x84] |= (byte)(1 << shift);
        }
    }

    // Dex Flags
    public bool NationalDex
    {
        get => (Data[4] & 1) == 1;
        set => Data[4] = (byte)((Data[4] & 0xFE) | (value ? 1 : 0));
    }

    /// <summary>
    /// Gets the last viewed dex entry in the Pokédex (by National Dex ID), internally called DefaultMons
    /// </summary>
    public uint CurrentViewedDex => (ReadUInt32LittleEndian(Data[4..]) >> 9) & 0x3FF;

    public IEnumerable<int> GetAllFormEntries(ushort species)
    {
        var fc = SAV.Personal[species].FormCount;
        for (int j = 1; j < fc; j++)
        {
            int start = j;
            int end = j;
            if (GetSaneFormsToIterate(species, out int s, out int n, j))
            {
                start = s;
                end = n;
            }
            start = Math.Max(1, start);
            for (int f = start; f <= end; f++)
            {
                int x = GetDexFormIndex(species, fc, f);
                if (x >= 0)
                    yield return x;
            }
        }
    }

    public int GetDexFormIndex(ushort species, byte formCount, int form)
    {
        var index = GetCountFormsPriorTo(species, formCount);
        if (index < 0)
            return index;
        return SAV.MaxSpeciesID + index + (form - 1);
    }

    public IList<string> GetEntryNames(IReadOnlyList<string> speciesNames)
    {
        var names = new List<string>();
        var max = SAV.MaxSpeciesID;
        for (int i = 1; i <= max; i++)
            names.Add($"{i:000} - {speciesNames[i]}");

        // Add forms
        int ctr = max + 1;
        for (ushort species = 1; species <= max; species++)
        {
            var c = SAV.Personal[species].FormCount;
            for (int f = 1; f < c; f++)
            {
                int x = GetDexFormIndex(species, c, f);
                if (x >= 0)
                    names.Add($"{ctr++:000} - {speciesNames[species]}-{f}");
            }
        }
        return names;
    }

    /// <summary>
    /// Gets a list of Species IDs that a given dex-form index corresponds to.
    /// </summary>
    private List<ushort> GetFormIndexBaseSpeciesList()
    {
        var baseSpecies = new List<ushort>();
        for (ushort species = 1; species <= SAV.MaxSpeciesID; species++)
        {
            var c = SAV.Personal[species].FormCount;
            for (int f = 1; f < c; f++)
            {
                int x = GetDexFormIndex(species, c, f);
                if (x >= 0)
                    baseSpecies.Add(species);
            }
        }
        return baseSpecies;
    }

    public byte GetBaseSpeciesGenderValue(int index)
    {
        // meowstic special handling
        const int meow = 678;
        if (index == meow - 1 || (index >= SAV.MaxSpeciesID && FormBaseSpecies[index - SAV.MaxSpeciesID] == meow))
            return index < SAV.MaxSpeciesID ? PersonalInfo.RatioMagicMale : PersonalInfo.RatioMagicFemale; // M : F

        if (index < SAV.MaxSpeciesID)
            return SAV.Personal[index + 1].Gender;

        index -= SAV.MaxSpeciesID;
        ushort species = FormBaseSpecies[index];
        return SAV.Personal[species].Gender;
    }

    public ushort GetBaseSpecies(int index)
    {
        if (index < SAV.MaxSpeciesID)
            return (ushort)(index + 1);

        var species = index - SAV.MaxSpeciesID;
        return FormBaseSpecies[species];
    }

    protected sealed override void SetAllDexFlagsLanguage(int bit, int lang, bool value = true)
    {
        lang = GetDexLangFlag(lang);
        if (lang < 0)
            return;

        int lbit = (bit * DexLangIDCount) + lang;
        if (lbit < DexLangFlagByteCount << 3) // Sanity check for max length of region
            SetFlag(PokeDexLanguageFlags, lbit, value);
    }

    public bool[] GetLanguageBitflags(ushort species)
    {
        var result = new bool[DexLangIDCount];
        int bit = species - 1;
        for (int i = 0; i < DexLangIDCount; i++)
        {
            int lbit = (bit * DexLangIDCount) + i;
            result[i] = GetFlag(PokeDexLanguageFlags, lbit);
        }
        return result;
    }

    public void SetLanguageBitflags(ushort species, ReadOnlySpan<bool> value)
    {
        int bit = species - 1;
        for (int i = 0; i < DexLangIDCount; i++)
        {
            int lbit = (bit * DexLangIDCount) + i;
            SetFlag(PokeDexLanguageFlags, lbit, value[i]);
        }
    }

    public void ToggleLanguageFlagsAll(bool value)
    {
        var arr = GetBlankLanguageBits(value);
        for (ushort i = 1; i <= SAV.MaxSpeciesID; i++)
            SetLanguageBitflags(i, arr);
    }

    public void ToggleLanguageFlagsSingle(ushort species, bool value)
    {
        var arr = GetBlankLanguageBits(value);
        SetLanguageBitflags(species, arr);
    }

    private bool[] GetBlankLanguageBits(bool value)
    {
        var result = new bool[DexLangIDCount];
        for (int i = 0; i < DexLangIDCount; i++)
            result[i] = value;
        return result;
    }

    public int GetEntryIndex(ushort species, byte form)
    {
        if (form <= 0)
            return species - 1;
        var fc = SAV.Personal[species].FormCount;
        if (fc <= 1) // no forms at all
            return species - 1;

        int previous = GetCountFormsPriorTo(species, fc);
        if (previous < 0) // no dex forms
            return species - 1;

        // bit index valid
        return SAV.MaxSpeciesID + previous + (form - 1);
    }
}
