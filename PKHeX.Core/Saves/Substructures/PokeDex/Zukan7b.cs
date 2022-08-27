using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Pokédex structure used for <see cref="GameVersion.GG"/> games, slightly modified from <see cref="Zukan7"/>.
/// </summary>>
public sealed class Zukan7b : Zukan7
{
    public Zukan7b(SAV7b sav, int dex, int langflag) : base(sav, dex, langflag)
    {
    }

    public override void SetDex(PKM pk)
    {
        if (!TryGetSizeEntryIndex(pk.Species, pk.Form, out _))
            return;
        SetSizeData((PB7)pk);
        base.SetDex(pk);
    }

    protected override void SetDex(ushort species, int bit, byte form, int gender, bool shiny, int lang)
    {
        if (IsBuddy(species, form))
            form = 0;
        base.SetDex(species, bit, form, gender, shiny, lang);
    }

    private static bool IsBuddy(ushort species, byte form) => (species == (int)Species.Pikachu && form == 8) || (species == (int)Species.Eevee && form == 1);

    public const int DefaultEntryValue = 0x7F;

    public bool GetSizeData(DexSizeType group, ushort species, byte form, out int height, out int weight)
    {
        height = weight = DefaultEntryValue;
        if (TryGetSizeEntryIndex(species, form, out var index))
            return GetSizeData(group, index, out height, out weight);
        return false;
    }

    public bool GetSizeData(DexSizeType group, int index, out int height, out int weight)
    {
        var ofs = GetDexSizeOffset(group, index);
        var entry = SAV.Data.AsSpan(ofs);
        height = ReadUInt16LittleEndian(entry) >> 1;
        weight = ReadUInt16LittleEndian(entry[2..]);
        return !IsEntryUnset(height, weight);
    }

    private static bool IsEntryUnset(int height, int weight) => height == DefaultEntryValue && weight == DefaultEntryValue;

    private void SetSizeData(PB7 pk)
    {
        var species = pk.Species;
        var form = pk.Form;
        if (!TryGetSizeEntryIndex(species, form, out int index))
            return;

        if (Math.Round(pk.HeightAbsolute) < pk.PersonalInfo.Height) // possible minimum height
        {
            int ofs = GetDexSizeOffset(DexSizeType.MinHeight, index);
            var entry = SAV.Data.AsSpan(ofs);
            var minHeight = ReadUInt16LittleEndian(entry) >> 1;
            var calcHeight = PB7.GetHeightAbsolute(pk.PersonalInfo, minHeight);
            if (Math.Round(pk.HeightAbsolute) < Math.Round(calcHeight) || ReadUInt32LittleEndian(entry) == 0x007F00FE) // unset
                SetSizeData(pk, DexSizeType.MinHeight);
        }
        else if (Math.Round(pk.HeightAbsolute) > pk.PersonalInfo.Height) // possible maximum height
        {
            int ofs = GetDexSizeOffset(DexSizeType.MaxHeight, index);
            var entry = SAV.Data.AsSpan(ofs);
            var maxHeight = ReadUInt16LittleEndian(entry) >> 1;
            var calcHeight = PB7.GetHeightAbsolute(pk.PersonalInfo, maxHeight);
            if (Math.Round(pk.HeightAbsolute) > Math.Round(calcHeight) || ReadUInt32LittleEndian(entry) == 0x007F00FE) // unset
                SetSizeData(pk, DexSizeType.MaxHeight);
        }

        if (Math.Round(pk.WeightAbsolute) < pk.PersonalInfo.Weight) // possible minimum weight
        {
            int ofs = GetDexSizeOffset(DexSizeType.MinWeight, index);
            var entry = SAV.Data.AsSpan(ofs);
            var minHeight = ReadUInt16LittleEndian(entry) >> 1;
            var minWeight = ReadUInt16LittleEndian(entry[2..]);
            var calcWeight = PB7.GetWeightAbsolute(pk.PersonalInfo, minHeight, minWeight);
            if (Math.Round(pk.WeightAbsolute) < Math.Round(calcWeight) || ReadUInt32LittleEndian(entry) == 0x007F00FE) // unset
                SetSizeData(pk, DexSizeType.MinWeight);
        }
        else if (Math.Round(pk.WeightAbsolute) > pk.PersonalInfo.Weight) // possible maximum weight
        {
            int ofs = GetDexSizeOffset(DexSizeType.MaxWeight, index);
            var entry = SAV.Data.AsSpan(ofs);
            var maxHeight = ReadUInt16LittleEndian(entry) >> 1;
            var maxWeight = ReadUInt16LittleEndian(entry[2..]);
            var calcWeight = PB7.GetWeightAbsolute(pk.PersonalInfo, maxHeight, maxWeight);
            if (Math.Round(pk.WeightAbsolute) > Math.Round(calcWeight) || ReadUInt32LittleEndian(entry) == 0x007F00FE) // unset
                SetSizeData(pk, DexSizeType.MaxWeight);
        }
    }

    private static int GetDexSizeOffset(DexSizeType group, int index) => 0x3978 + (index * 6) + ((int)group * 0x45C); // blockofs + 0xF78 + ([186*6]*n) + x*6

    private void SetSizeData(PB7 pk, DexSizeType group)
    {
        var tree = EvolutionTree.Evolves7b;
        ushort species = pk.Species;
        var form = pk.Form;

        int height = pk.HeightScalar;
        int weight = pk.WeightScalar;

        // update for all species in potential lineage
        var allspec = tree.GetEvolutionsAndPreEvolutions(species, form);
        foreach (var (s, f) in allspec)
            SetSizeData(group, s, f, height, weight);
    }

    public void SetSizeData(DexSizeType group, ushort species, byte form, int height, int weight)
    {
        if (TryGetSizeEntryIndex(species, form, out var index))
            SetSizeData(group, index, height, weight);
    }

    public void SetSizeData(DexSizeType group, int index, int height, int weight)
    {
        var ofs = GetDexSizeOffset(group, index);
        var span = SAV.Data.AsSpan(ofs);
        WriteUInt16LittleEndian(span, (ushort)(height << 1));
        WriteUInt16LittleEndian(span[2..], (ushort)weight);
    }

    public static bool TryGetSizeEntryIndex(ushort species, byte form, out int index)
    {
        index = -1;
        if (form == 0 && species <= 151)
        {
            index = species - 1;
            return true;
        }
        for (int i = 0; i < SizeDexInfoTable.Length; i += 3)
        {
            if (SizeDexInfoTable[i] != species)
                continue;
            if (SizeDexInfoTable[i + 1] != form)
                continue;
            index = SizeDexInfoTable[i + 2];
            return true;
        }
        return false;
    }

    private static readonly ushort[] SizeDexInfoTable =
    {
        // species, form, index
        808, 0, 151,
        809, 0, 152,

        003, 1, 153,
        006, 1, 154,
        006, 2, 155,
        009, 1, 156,
        015, 1, 157,
        018, 1, 158,
        019, 1, 159,
        020, 1, 160,
        026, 1, 161,
        027, 1, 162,
        028, 1, 163,
        037, 1, 164,
        038, 1, 165,
        050, 1, 166,
        051, 1, 167,
        052, 1, 168,
        053, 1, 169,
        065, 1, 170,
        074, 1, 171,
        075, 1, 172,
        076, 1, 173,
        080, 1, 174,
        088, 1, 175,
        089, 1, 176,
        094, 1, 177,
        103, 1, 178,
        105, 1, 179,
        115, 1, 180,
        127, 1, 181,
        130, 1, 182,
        142, 1, 183,
        150, 1, 184,
        150, 2, 185,
    };

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
