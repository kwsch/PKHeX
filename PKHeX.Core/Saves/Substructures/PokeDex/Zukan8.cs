using System;
using System.Collections.Generic;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Pokédex structure used for <see cref="GameVersion.SWSH"/>.
/// </summary>>
public sealed class Zukan8 : ZukanBase<SAV8SWSH>
{
    private readonly SCBlock Galar;
    private readonly SCBlock Rigel1;
    private readonly SCBlock Rigel2;

    private const int MaxSpeciesID = Legal.MaxSpeciesID_8_R2;

    /// <summary>
    /// Reverses a Species into the component <see cref="Zukan8Index"/> information.
    /// </summary>
    public readonly IReadOnlyDictionary<ushort, Zukan8Index> DexLookup;

    public Zukan8(SAV8SWSH sav, SCBlock galar, SCBlock rigel1, SCBlock rigel2) : base(sav, default)
    {
        Galar = galar;
        Rigel1 = rigel1;
        Rigel2 = rigel2;
        var revision = GetRevision();
        DexLookup = GetDexLookup(sav.Personal, revision, Zukan8Index.TotalCount);
    }

    /// <summary>
    /// Checks how much DLC patches have been installed by detecting if DLC blocks are present.
    /// </summary>
    public int GetRevision()
    {
        if (Rigel1.Data.Length == 0)
            return 0; // No DLC1 data allocated
        if (Rigel2.Data.Length == 0)
            return 1; // No DLC2 data allocated
        return 2;
    }

    private byte[] GetDexBlock(Zukan8Type infoDexType) => infoDexType switch
    {
        Zukan8Type.Galar => Galar.Data,
        Zukan8Type.Armor => Rigel1.Data,
        Zukan8Type.Crown => Rigel2.Data,
        _ => throw new ArgumentOutOfRangeException(nameof(infoDexType), infoDexType, null),
    };

    private static bool GetFlag(byte[] data, int baseOffset, int bitIndex) => FlagUtil.GetFlag(data, baseOffset + (bitIndex >> 3), bitIndex);
    private static void SetFlag(byte[] data, int baseOffset, int bitIndex, bool value = true) => FlagUtil.SetFlag(data, baseOffset + (bitIndex >> 3), bitIndex, value);

    private static Dictionary<ushort, Zukan8Index> GetDexLookup(PersonalTable8SWSH pt, int dexRevision, int count)
    {
        var lookup = new Dictionary<ushort, Zukan8Index>(count);
        for (ushort i = 1; i <= MaxSpeciesID; i++)
        {
            var p = pt[i];
            var index = p.PokeDexIndex;
            if (index != 0)
            {
                lookup.Add(i, new Zukan8Index(Zukan8Type.Galar, index));
                continue;
            }

            if (dexRevision == 0)
                continue;

            var armor = p.ArmorDexIndex;
            if (armor != 0)
            {
                lookup.Add(i, new Zukan8Index(Zukan8Type.Armor, armor));
                continue;
            }

            if (dexRevision == 1)
                continue;

            var crown = p.CrownDexIndex;
            if (crown != 0)
            {
                lookup.Add(i, new Zukan8Index(Zukan8Type.Crown, crown));
                // continue;
            }
        }
        return lookup;
    }

    public static List<Zukan8EntryInfo> GetRawIndexes(PersonalTable8SWSH pt, int dexRevision, int count)
    {
        var result = new List<Zukan8EntryInfo>(count);
        for (ushort i = 1; i <= MaxSpeciesID; i++)
        {
            var p = pt[i];
            var index = p.PokeDexIndex;
            if (index != 0)
                result.Add(new Zukan8EntryInfo(i, new Zukan8Index(Zukan8Type.Galar, index)));
        }
        if (dexRevision == 0)
            return result;

        for (ushort i = 1; i <= MaxSpeciesID; i++)
        {
            var p = pt[i];
            var index = p.ArmorDexIndex;
            if (index != 0)
                result.Add(new Zukan8EntryInfo(i, new Zukan8Index(Zukan8Type.Armor, index)));
        }
        if (dexRevision == 1)
            return result;

        for (ushort i = 1; i <= MaxSpeciesID; i++)
        {
            var p = pt[i];
            var index = p.CrownDexIndex;
            if (index != 0)
                result.Add(new Zukan8EntryInfo(i, new Zukan8Index(Zukan8Type.Crown, index)));
        }
        return result;
    }

    private static int GetDexLangFlag(int lang) => lang switch
    {
        > 10 or 6 or <= 0 => -1, // invalid language
        // skip over langID 0 (unused) => [0-8]
        // skip over langID 6 (unused)
        >= 7 => lang - 2,
        _ => lang - 1,
    };

#if DEBUG
    public IList<string> GetEntryNames(IReadOnlyList<string> speciesNames)
    {
        var dex = new List<string>();
        foreach (var (species, entry) in DexLookup)
        {
            var name = entry.GetEntryName(speciesNames, species);
            dex.Add(name);
        }
        dex.Sort();
        return dex;
    }
#endif

    #region Structure

    internal const int EntrySize = 0x30;

    // First 0x20 bytes are for seen flags, allocated as 4 QWORD values.
    private const int SeenRegionCount = 4;
    private const int SeenRegionSize = sizeof(ulong);
    // not_shiny_gender_0,
    // not_shiny_gender_1,
    // shiny_gender_0,
    // shiny_gender_1
    // Each QWORD stores the following bits:
    // - FormsSeen[63], default form is index 0.
    // - Gigantamax:1 -- for Urshifu, they store a bit prior for the second Gigantamax form...

    // Next 4 bytes are for obtained info (u32)
    private const int OFS_CAUGHT = 0x20;
    // Owned:1 (posessed by player)
    // OwnedGigantamax:1 (Gigantamaxed by player in battle)
    // LanguagesObtained:2-14 (flags)
    // DisplayFormID:15-27 (value)
    // DisplayGigantamaxInstead:28 (flag)
    // DisplayGender:29/30 (m/f)
    // DisplayShiny:31 (flag)

    // Next 4 bytes are Battled Count (u32)
    private const int OFS_BATTLED = 0x24;

    // Next 4 bytes are unused/reserved for future updates.
    private const int OFS_UNK1 = 0x28;
    // Seen Gigantamax-1 Form:1 (Urshifu)

    // Next 4 bytes are Unused(?)
    private const int OFS_UNK2 = 0x2C;

    public bool GetEntry(ushort species, out Zukan8Index entry) => DexLookup.TryGetValue(species, out entry);

    public override bool GetSeen(ushort species)
    {
        if (!GetEntry(species, out var entry))
            return false;

        return GetSeen(entry);
    }

    public bool GetSeen(Zukan8Index entry)
    {
        byte[] data = GetDexBlock(entry.DexType);
        int offset = entry.Offset;
        for (int i = 0; i < SeenRegionCount; i++)
        {
            var ofs = offset + (SeenRegionSize * i);
            if (ReadUInt64LittleEndian(data.AsSpan(ofs)) != 0)
                return true;
        }

        return false;
    }

    public bool GetSeenRegion(ushort species, byte form, int region)
    {
        if (!GetEntry(species, out var entry))
            return false;

        return GetSeenRegion(entry, form, region);
    }

    public bool GetSeenRegion(Zukan8Index entry, byte form, int region)
    {
        if ((uint)region >= SeenRegionCount)
            throw new ArgumentOutOfRangeException(nameof(region));
        if (form > 63)
            return false;

        var dex = entry.DexType;
        var offset = entry.Offset;
        var data = GetDexBlock(dex);
        var ofs = SeenRegionSize * region;
        return GetFlag(data, offset + ofs, form);
    }

    public void SetSeenRegion(ushort species, byte form, int region, bool value = true)
    {
        if (!GetEntry(species, out var entry))
            return;

        SetSeenRegion(entry, form, region, value);
    }

    public void SetSeenRegion(Zukan8Index entry, byte form, int region, bool value = true)
    {
        if ((uint) region >= SeenRegionCount)
            throw new ArgumentOutOfRangeException(nameof(region));
        if ((uint) form > 63)
            return;

        var data = GetDexBlock(entry.DexType);
        int index = entry.Offset;
        var ofs = SeenRegionSize * region;
        SetFlag(data, index + ofs, form, value);
    }

    public override bool GetCaught(ushort species) => GetCaughtFlagID(species, 0);
    public void SetCaught(ushort species, bool value = true) => SetCaughtFlagID(species, 0, value);
    public bool GetCaughtGigantamaxed(ushort species) => GetCaughtFlagID(species, 1);
    public void SetCaughtGigantamax(ushort species, bool value = true) => SetCaughtFlagID(species, 1, value);
    public bool GetIsLanguageIndexObtained(ushort species, int langIndex) => GetCaughtFlagID(species, 2 + langIndex);
    public void SetIsLanguageIndexObtained(ushort species, int langIndex, bool value = true) => SetCaughtFlagID(species, 2 + langIndex, value);

    public bool GetCaught(Zukan8Index entry) => GetCaughtFlagID(entry, 0);
    public void SetCaught(Zukan8Index entry, bool value = true) => SetCaughtFlagID(entry, 0, value);
    public bool GetCaughtGigantamaxed(Zukan8Index entry) => GetCaughtFlagID(entry, 1);
    public void SetCaughtGigantamax(Zukan8Index entry, bool value = true) => SetCaughtFlagID(entry, 1, value);
    public bool GetIsLanguageIndexObtained(Zukan8Index entry, int langIndex) => GetCaughtFlagID(entry, 2 + langIndex);
    public void SetIsLanguageIndexObtained(Zukan8Index entry, int langIndex, bool value = true) => SetCaughtFlagID(entry, 2 + langIndex, value);

    private bool GetCaughtFlagID(ushort species, int bit)
    {
        if (!GetEntry(species, out var entry))
            return false;

        return GetCaughtFlagID(entry, bit);
    }

    private bool GetCaughtFlagID(Zukan8Index entry, int bit)
    {
        var data = GetDexBlock(entry.DexType);
        return GetFlag(data, entry.Offset + OFS_CAUGHT, bit);
    }

    public void SetCaughtFlagID(ushort species, int bit, bool value = true)
    {
        if (!GetEntry(species, out var entry))
            return;

        SetCaughtFlagID(entry, bit, value);
    }

    public void SetCaughtFlagID(Zukan8Index entry, int bit, bool value = true)
    {
        var data = GetDexBlock(entry.DexType);
        SetFlag(data, entry.Offset + OFS_CAUGHT, bit, value);
    }

    public bool GetIsLanguageObtained(ushort species, int language)
    {
        int langIndex = GetDexLangFlag(language);
        if (langIndex < 0)
            return false;

        return GetIsLanguageIndexObtained(species, langIndex);
    }

    public void SetIsLanguageObtained(ushort species, int language, bool value = true)
    {
        int langIndex = GetDexLangFlag(language);
        if (langIndex < 0)
            return;

        SetIsLanguageIndexObtained(species, langIndex, value);
    }

    public uint GetFormDisplayed(ushort species)
    {
        if (!GetEntry(species, out var entry))
            return 0;

        return GetFormDisplayed(entry);
    }

    public uint GetFormDisplayed(Zukan8Index entry)
    {
        var data = GetDexBlock(entry.DexType);
        var index = entry.Offset;
        var value = ReadUInt32LittleEndian(data.AsSpan(index + OFS_CAUGHT));
        return (value >> 15) & 0x1FFF; // (0x1FFF is really overkill, GameFreak)
    }

    public void SetFormDisplayed(ushort species, uint value = 0)
    {
        if (!GetEntry(species, out var entry))
            return;

        SetFormDisplayed(entry, value);
    }

    public void SetFormDisplayed(Zukan8Index entry, uint value = 0)
    {
        var data = GetDexBlock(entry.DexType);
        var index = entry.Offset;
        var span = data.AsSpan(index + OFS_CAUGHT);
        var current = ReadUInt32LittleEndian(span);
        uint update = (current & ~(0x1FFFu << 15)) | ((value & 0x1FFF) << 15);
        WriteUInt32LittleEndian(span, update);
    }

    public uint GetGenderDisplayed(ushort species)
    {
        if (!GetEntry(species, out var entry))
            return 0;

        return GetGenderDisplayed(entry);
    }

    public uint GetGenderDisplayed(Zukan8Index entry)
    {
        var data = GetDexBlock(entry.DexType);
        var index = entry.Offset;
        var value = ReadUInt32LittleEndian(data.AsSpan(index + OFS_CAUGHT));
        return (value >> 29) & 3;
    }

    public void SetGenderDisplayed(ushort species, uint value = 0)
    {
        if (!GetEntry(species, out var entry))
            return;

        SetGenderDisplayed(entry, value);
    }

    public void SetGenderDisplayed(Zukan8Index entry, uint value = 0)
    {
        var data = GetDexBlock(entry.DexType);
        var index = entry.Offset;
        var span = data.AsSpan(index + OFS_CAUGHT);
        var current = ReadUInt32LittleEndian(span);
        uint update = (current & ~(3u << 29)) | ((value & 3) << 29);
        WriteUInt32LittleEndian(span, update);
    }

    public bool GetDisplayDynamaxInstead(ushort species) => GetCaughtFlagID(species, 28);
    public void SetDisplayDynamaxInstead(ushort species, bool value = true) => SetCaughtFlagID(species, 28, value);
    public bool GetDisplayShiny(ushort species) => GetCaughtFlagID(species, 31);
    public void SetDisplayShiny(ushort species, bool value = true) => SetCaughtFlagID(species, 31, value);

    public void SetCaughtFlags32(ushort species, uint value) => SetU32(species, value, OFS_CAUGHT);
    public uint GetBattledCount(ushort species) => GetU32(species, OFS_BATTLED);
    public void SetBattledCount(ushort species, uint value) => SetU32(species, value, OFS_BATTLED);

    public uint GetUnk1Count(ushort species) => GetU32(species, OFS_UNK1);
    public void SetUnk1Count(ushort species, uint value) => SetU32(species, value, OFS_UNK1);
    public uint GetUnk2Count(ushort species) => GetU32(species, OFS_UNK2);
    public void SetUnk2Count(ushort species, uint value) => SetU32(species, value, OFS_UNK2);
    public bool GetCaughtGigantamax1(ushort species) => GetFlag28(species, 0);
    public void SetCaughtGigantamax1(ushort species, bool value = true) => SetFlag28(species, 0, value);
    public bool GetCaughtGigantamax1(Zukan8Index entry) => GetFlag28(entry, 0);
    public void SetCaughtGigantamax1(Zukan8Index entry, bool value = true) => SetFlag28(entry, 0, value);

    private bool GetFlag28(ushort species, int bit)
    {
        if (!GetEntry(species, out var entry))
            return false;

        return GetFlag28(entry, bit);
    }

    public void SetFlag28(ushort species, int bit, bool value = true)
    {
        if (!GetEntry(species, out var entry))
            return;

        SetFlag28(entry, bit, value);
    }

    private bool GetFlag28(Zukan8Index entry, int bit)
    {
        var data = GetDexBlock(entry.DexType);
        return GetFlag(data, entry.Offset + OFS_UNK1, bit);
    }

    public void SetFlag28(Zukan8Index entry, int bit, bool value = true)
    {
        var data = GetDexBlock(entry.DexType);
        SetFlag(data, entry.Offset + OFS_UNK1, bit, value);
    }

    public bool GetDisplayDynamaxInstead(Zukan8Index entry) => GetCaughtFlagID(entry, 28);
    public void SetDisplayDynamaxInstead(Zukan8Index entry, bool value = true) => SetCaughtFlagID(entry, 28, value);
    public bool GetDisplayShiny(Zukan8Index entry) => GetCaughtFlagID(entry, 31);
    public void SetDisplayShiny(Zukan8Index entry, bool value = true) => SetCaughtFlagID(entry, 31, value);

    public void SetCaughtFlags32(Zukan8Index entry, uint value) => SetU32(entry, value, OFS_CAUGHT);
    public uint GetBattledCount(Zukan8Index entry) => GetU32(entry, OFS_BATTLED);
    public void SetBattledCount(Zukan8Index entry, uint value) => SetU32(entry, value, OFS_BATTLED);
    public uint GetUnk1Count(Zukan8Index entry) => GetU32(entry, OFS_UNK1);
    public void SetUnk1Count(Zukan8Index entry, uint value) => SetU32(entry, value, OFS_UNK1);
    public uint GetUnk2Count(Zukan8Index entry) => GetU32(entry, OFS_UNK2);
    public void SetUnk2Count(Zukan8Index entry, uint value) => SetU32(entry, value, OFS_UNK2);

    private uint GetU32(ushort species, int ofs)
    {
        if (!GetEntry(species, out var entry))
            return 0;

        return GetU32(entry, ofs);
    }

    private uint GetU32(Zukan8Index entry, int ofs)
    {
        var dex = entry.DexType;
        var index = entry.Offset;
        var data = GetDexBlock(dex);
        return ReadUInt32LittleEndian(data.AsSpan(index + ofs));
    }

    private void SetU32(ushort species, uint value, int ofs)
    {
        if (!GetEntry(species, out var entry))
            return;

        SetU32(entry, value, ofs);
    }

    private void SetU32(Zukan8Index entry, uint value, int ofs)
    {
        var dex = entry.DexType;
        var index = entry.Offset;
        var data = GetDexBlock(dex);
        WriteUInt32LittleEndian(data.AsSpan(index + ofs), value);
    }

    #endregion

    #region Inherited
    public override void SetDex(PKM pk)
    {
        ushort species = pk.Species;
        if (!GetEntry(species, out _))
            return;
        if (pk.IsEgg) // do not add
            return;

        bool owned = GetCaught(species);

        var gender = pk.Gender;
        bool shiny = pk.IsShiny;
        var form = pk.Form;
        var language = pk.Language;

        var g = gender & 1;
        var s = shiny ? 2 : 0;
        if (species == (int)Species.Alcremie)
        {
            form *= 7;
            form += (byte)((PK8)pk).FormArgument; // alteration byte
        }
        else if (species == (int) Species.Eternatus && form == 1)
        {
            form = 0;
            SetSeenRegion(species, 63, g | s);
        }

        SetSeenRegion(species, form, g | s);
        SetCaught(species);
        SetIsLanguageObtained(species, language);
        if (!owned)
        {
            SetFormDisplayed(species, form);
            if (shiny)
                SetDisplayShiny(species);
            SetGenderDisplayed(species, (uint)g);
        }

        var count = GetBattledCount(species);
        if (count == 0)
            SetBattledCount(species, 1);
    }

    public override void SeenNone()
    {
        Array.Clear(Galar.Data, 0, Galar.Data.Length);
        Array.Clear(Rigel1.Data, 0, Rigel1.Data.Length);
        Array.Clear(Rigel2.Data, 0, Rigel2.Data.Length);
    }

    public override void CaughtNone()
    {
        foreach (var kvp in DexLookup)
            CaughtNone(kvp.Key);
    }

    private void CaughtNone(ushort species)
    {
        SetCaughtFlags32(species, 0);
        SetUnk1Count(species, 0);
        SetUnk2Count(species, 0);
    }

    public override void SeenAll(bool shinyToo = false)
    {
        SetAllSeen(true, shinyToo);
    }

    private void SeenAll(ushort species, byte formCount, bool shinyToo, bool value = true)
    {
        var pt = SAV.Personal;
        for (byte form = 0; form < formCount; form++)
        {
            var pi = pt.GetFormEntry(species, form);
            SeenAll(species, form, value, pi, shinyToo);
        }

        if (species == (int)Species.Slowbro) // Clear Mega Slowbro always
            SeenAll(species, 1, false, pt[species], shinyToo);

        if (!value)
            ClearGigantamaxFlags(species);
    }

    private void SeenAll(ushort species, byte bitIndex, bool value, IGenderDetail pi, bool shinyToo)
    {
        if (pi.IsDualGender || !value)
        {
            SetSeenRegion(species, bitIndex, 0, value);
            SetSeenRegion(species, bitIndex, 1, value);
            if (!shinyToo && value)
                return;
            SetSeenRegion(species, bitIndex, 2, value);
            SetSeenRegion(species, bitIndex, 3, value);
        }
        else
        {
            var index = pi.OnlyFemale ? 1 : 0;
            SetSeenRegion(species, bitIndex, 0 + index);
            if (!shinyToo)
                return;
            SetSeenRegion(species, bitIndex, 2 + index);
        }
    }

    private void ClearGigantamaxFlags(ushort species)
    {
        SetSeenRegion(species, 63, 0, false);
        SetSeenRegion(species, 63, 1, false);
        SetSeenRegion(species, 63, 2, false);
        SetSeenRegion(species, 63, 3, false);
    }

    public override void CompleteDex(bool shinyToo = false)
    {
        foreach (var kvp in DexLookup)
            SetDexEntryAll(kvp.Key, shinyToo);
    }

    public override void CaughtAll(bool shinyToo = false)
    {
        SeenAll(shinyToo);
        foreach (var kvp in DexLookup)
        {
            var species = kvp.Key;
            SetAllCaught(species, true, shinyToo);
        }
    }

    private void SetAllCaught(ushort species, bool value = true, bool shinyToo = false)
    {
        SetCaught(species);
        for (int i = 0; i < 11; i++)
            SetIsLanguageObtained(species, i, value);

        if (value)
        {
            var pi = SAV.Personal[species];
            if (shinyToo)
                SetDisplayShiny(species);

            SetGenderDisplayed(species, pi.RandomGender());
        }
        else
        {
            SetDisplayShiny(species, false);
            SetDisplayDynamaxInstead(species, false);
            SetGenderDisplayed(species, 0);
        }
    }

    public override void SetAllSeen(bool value = true, bool shinyToo = false)
    {
        foreach (var kvp in DexLookup)
        {
            var species = kvp.Key;
            SetAllSeen(species, value, shinyToo);
        }
    }

    private void SetAllSeen(ushort species, bool value = true, bool shinyToo = false)
    {
        var pi = SAV.Personal[species];
        var fc = pi.FormCount;
        if (species == (int) Species.Eternatus)
            fc = 1; // ignore gigantamax
        SeenAll(species, fc, shinyToo, value);

        if (species == (int) Species.Alcremie)
        {
            // Alcremie forms
            const int deco = 7;
            const int forms = 9;
            for (byte i = 0; i < deco * forms; i++) // 0-62
                SeenAll(species, i, value, pi, shinyToo);
        }

        if (IsGigantamaxFormStored(species))
        {
            SeenAll(species, 63, value, pi, shinyToo);
            if (species == (int)Species.Urshifu)
            {
                SeenAll(species, 62, value, pi, shinyToo);
                SetCaughtGigantamax1(species);
            }
            SetCaughtGigantamax(species);
        }
    }

    public override void SetDexEntryAll(ushort species, bool shinyToo = false)
    {
        SetAllSeen(species, true, shinyToo);
        SetAllCaught(species, true);
    }

    public override void ClearDexEntryAll(ushort species)
    {
        if (!GetEntry(species, out var entry))
            return;

        ClearDexEntryAll(entry);
    }

    private void ClearDexEntryAll(Zukan8Index entry)
    {
        var data = GetDexBlock(entry.DexType);
        Array.Clear(data, entry.Offset, EntrySize);
    }

    public void SetAllBattledCount(uint count = 500)
    {
        foreach (var kvp in DexLookup)
        {
            var species = kvp.Key;
            SetBattledCount(species, count);
        }
    }

    public static bool IsGigantamaxFormStored(ushort species)
    {
        return Gigantamax.CanToggle(species) || species == (int)Species.Eternatus;
    }
    #endregion
}

/// <summary>
/// Indicates which <see cref="Zukan8Type"/> block will store the entry, and at what index.
/// </summary>
/// <param name="DexType">Which block stores the Pokédex entry.</param>
/// <param name="Index">Index that the Pokédex entry is stored at.</param>
public readonly record struct Zukan8Index(Zukan8Type DexType, ushort Index)
{
    public override string ToString() => $"{Index:000} - {DexType}";

    private int GetSavedIndex()
    {
        var index = Index;
        if (index < 1)
            throw new ArgumentOutOfRangeException(nameof(Index));

        return index - 1;
    }

    public int Offset => GetSavedIndex() * Zukan8.EntrySize;

    private const int GalarCount = 400; // Count within Galar dex
    private const int Rigel1Count = 211; // Count within Armor dex
    private const int Rigel2Count = 210; // Count within Crown dex
    public const int TotalCount = GalarCount + Rigel1Count + Rigel2Count;
#if DEBUG
    /// <summary>
    /// Gets the <see cref="Zukan8Index"/> from the absolute (overall) dex index. Don't use this method unless you're analyzing things.
    /// </summary>
    /// <param name="index">Unique Pokédex index (incremental). Should be 0-indexed.</param>
    public static Zukan8Index GetFromAbsoluteIndex(ushort index)
    {
        if (index > TotalCount)
            return new Zukan8Index();

        if (index < GalarCount)
            return new Zukan8Index(Zukan8Type.Galar, ++index);
        index -= GalarCount;

        if (index < Rigel1Count)
            return new Zukan8Index(Zukan8Type.Armor, ++index);
        index -= Rigel1Count;

        if (index < Rigel2Count)
            return new Zukan8Index(Zukan8Type.Crown, ++index);

        throw new ArgumentOutOfRangeException(nameof(index));
    }
#endif

    public string DexPrefix => DexType.GetZukanTypeInternalPrefix();

    public int AbsoluteIndex => GetAbsoluteIndex(DexType);

    private int GetAbsoluteIndex(Zukan8Type type) => type switch
    {
        Zukan8Type.Galar => Index,
        Zukan8Type.Armor => Index + GalarCount,
        Zukan8Type.Crown => Index + GalarCount + Rigel1Count,
        _ => throw new ArgumentOutOfRangeException(nameof(type)),
    };

    public string GetEntryName(IReadOnlyList<string> speciesNames, ushort species)
    {
        return $"{DexPrefix}.{Index:000} - {speciesNames[species]}";
    }
}

public readonly record struct Zukan8EntryInfo(ushort Species, Zukan8Index Entry)
{
    public string GetEntryName(IReadOnlyList<string> speciesNames) => Entry.GetEntryName(speciesNames, Species);
}

public enum Zukan8Type : sbyte
{
    None = 0,
    Galar,
    Armor,
    Crown,
}

public static class Zukan8TypeExtensions
{
    public static string GetZukanTypeInternalPrefix(this Zukan8Type type) => type switch
    {
        Zukan8Type.Galar => "O0",
        Zukan8Type.Armor => "R1",
        Zukan8Type.Crown => "R2",
        _ => throw new ArgumentOutOfRangeException(nameof(type)),
    };
}
