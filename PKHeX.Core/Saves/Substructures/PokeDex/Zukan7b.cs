using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Pokédex structure used for <see cref="GameVersion.GG"/> games, slightly modified from <see cref="Zukan7"/>.
/// </summary>>
public sealed class Zukan7b(SAV7b sav, Memory<byte> dex, int langflag) : Zukan7(sav, dex, langflag)
{
    private const int UNSET = 0x007F00FE;
    private const int EntryStart = 0xF78; // 0x3978 - 0x2A00
    private const int EntryCount = 186;
    private const int EntrySize = 6;

    public const byte DefaultEntryValueH = 0xFE;
    public const byte DefaultEntryValueW = 0x7F;

    public override void SetDex(PKM pk)
    {
        if (!TryGetSizeEntryIndex(pk.Species, pk.Form, out _))
            return;
        SetSizeData((PB7)pk);
        base.SetDex(pk);
    }

    protected override void SetDex(ushort species, int bit, byte form, byte gender, bool shiny, int lang)
    {
        if (IsBuddy(species, form))
            form = 0;
        base.SetDex(species, bit, form, gender, shiny, lang);
    }

    private static bool IsBuddy(ushort species, byte form) => (species == (int)Species.Pikachu && form == 8) || (species == (int)Species.Eevee && form == 1);

    public bool GetSizeData(DexSizeType group, ushort species, byte form, out byte height, out byte weight, out bool isFlagged)
    {
        height = DefaultEntryValueH; weight = DefaultEntryValueW;
        isFlagged = false;
        if (TryGetSizeEntryIndex(species, form, out var index))
            return GetSizeData(group, index, out height, out weight, out isFlagged);
        return false;
    }

    public bool GetSizeData(DexSizeType group, int index, out byte height, out byte weight, out bool isFlagged)
    {
        var ofs = GetDexSizeOffset(group, index);
        var entry = Data.Slice(ofs, EntrySize);
        height = entry[0];
        isFlagged = entry[1] == 1;
        weight = entry[2];
        return !IsEntryUnset(height, weight);
    }

    private static bool IsEntryUnset(byte height, byte weight) => height == DefaultEntryValueH && weight == DefaultEntryValueW;

    private void SetSizeData(PB7 pk)
    {
        var species = pk.Species;
        var form = pk.Form;
        if (!TryGetSizeEntryIndex(species, form, out int index))
            return;

        var pi = PersonalTable.GG[species, form];
        if (pk.HeightAbsolute < pi.Height) // possible minimum height
        {
            int ofs = GetDexSizeOffset(DexSizeType.MinHeight, index);
            var entry = Data.Slice(ofs, EntrySize);
            var minHeight = entry[0];
            if (pk.HeightScalar < minHeight || IsUnset(entry))
                SetSizeData(pk, DexSizeType.MinHeight);
        }
        else if (pk.HeightAbsolute > pi.Height) // possible maximum height
        {
            int ofs = GetDexSizeOffset(DexSizeType.MaxHeight, index);
            var entry = Data.Slice(ofs, EntrySize);
            var maxHeight = entry[0];
            if (pk.HeightScalar > maxHeight || IsUnset(entry))
                SetSizeData(pk, DexSizeType.MaxHeight);
        }

        if (pk.WeightAbsolute < pi.Weight) // possible minimum weight
        {
            int ofs = GetDexSizeOffset(DexSizeType.MinWeight, index);
            var entry = Data.Slice(ofs, EntrySize);
            var minHeight = entry[0];
            var minWeight = entry[2];
            var calcWeight = PB7.GetWeightAbsolute(pi, minHeight, minWeight);
            if (pk.WeightAbsolute < calcWeight || IsUnset(entry))
                SetSizeData(pk, DexSizeType.MinWeight);
        }
        else if (pk.WeightAbsolute > pi.Weight) // possible maximum weight
        {
            int ofs = GetDexSizeOffset(DexSizeType.MaxWeight, index);
            var entry = Data.Slice(ofs, EntrySize);
            var maxHeight = entry[0];
            var maxWeight = entry[2];
            var calcWeight = PB7.GetWeightAbsolute(pi, maxHeight, maxWeight);
            if (pk.WeightAbsolute > calcWeight || IsUnset(entry))
                SetSizeData(pk, DexSizeType.MaxWeight);
        }
    }

    private static bool IsUnset(Span<byte> entry) => ReadUInt32LittleEndian(entry) == UNSET;

    // blockofs + 0xF78 + ([186*6]*n) + x*6
    private static int GetDexSizeOffset(DexSizeType group, int index) => EntryStart + (EntrySize * (index + ((int)group * EntryCount)));

    private void SetSizeData(PB7 pk, DexSizeType group, bool flag = false)
    {
        var tree = EvolutionTree.Evolves7b;
        ushort species = pk.Species;
        var form = pk.Form;

        byte height = pk.HeightScalar;
        byte weight = pk.WeightScalar;

        // update for all species in potential lineage
        var allspec = tree.GetEvolutionsAndPreEvolutions(species, form);
        foreach (var (s, f) in allspec)
            SetSizeData(group, s, f, height, weight, flag);
    }

    public void SetSizeData(DexSizeType group, ushort species, byte form, byte height, byte weight, bool flag = false)
    {
        if (TryGetSizeEntryIndex(species, form, out var index))
            SetSizeData(group, index, height, weight, flag);
    }

    public void SetSizeData(DexSizeType group, int index, byte height, byte weight, bool flag = false)
    {
        var ofs = GetDexSizeOffset(group, index);
        var span = Data.Slice(ofs, EntrySize);
        span[0] = height;
        span[1] = flag ? (byte)1 : (byte)0;
        span[2] = weight;
        span[3] = 0;
    }

    private const int EntryMeltan = 151; // Melmetal 152
    private const int EntryForms = 153;

    public static bool TryGetSizeEntryIndex(ushort species, byte form, out int index)
    {
        index = -1;
        if (form == 0)
        {
            if (species <= 151) // Mew
            {
                index = species - 1;
                return true;
            }
            if (species is 808 or 809) // Meltan & Melmetal
            {
                index = EntryMeltan + (species - 808);
                return true;
            }
            return false;
        }

        // Forms
        for (int i = 0; i < SizeDexInfoTable.Length; i += 2)
        {
            if (SizeDexInfoTable[i] != species)
                continue;
            if (SizeDexInfoTable[i + 1] != form)
                continue;
            index = EntryForms + (i >> 1);
            return true;
        }
        return false;
    }

    private static ReadOnlySpan<byte> SizeDexInfoTable =>
    [
        // species, form
        003, 1,
        006, 1,
        006, 2,
        009, 1,
        015, 1,
        018, 1,
        019, 1,
        020, 1,
        026, 1,
        027, 1,
        028, 1,
        037, 1,
        038, 1,
        050, 1,
        051, 1,
        052, 1,
        053, 1,
        065, 1,
        074, 1,
        075, 1,
        076, 1,
        080, 1,
        088, 1,
        089, 1,
        094, 1,
        103, 1,
        105, 1,
        115, 1,
        127, 1,
        130, 1,
        142, 1,
        150, 1,
        150, 2,
    ];

    protected override bool GetSaneFormsToIterate(ushort species, out int formStart, out int formEnd, int formIn)
    {
        switch (species)
        {
            // Totems with Alolan Forms
            case 020 or 105: // Raticate or Marowak
                formStart = 0;
                formEnd = 1;
                return true;

            default:
                int count = DexFormUtil.GetDexFormCountGG(species);
                formStart = formEnd = 0;
                return count < formIn;
        }
    }
}

public enum DexSizeType
{
    MinHeight,
    MaxHeight,
    MinWeight,
    MaxWeight,
}
