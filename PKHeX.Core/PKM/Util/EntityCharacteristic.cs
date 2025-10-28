using System;

namespace PKHeX.Core;

/// <summary>
/// Logic for calculating the characteristic behavior of Pokémon.
/// </summary>
public static class EntityCharacteristic
{
    /// <summary>
    /// Gets the characteristic index from the input parameters.
    /// </summary>
    /// <param name="maxStatIndex">Index of the highest IV.</param>
    /// <param name="maxStatValue">Value of the highest IV.</param>
    /// <returns>Characteristic index.</returns>
    public static int GetCharacteristic(int maxStatIndex, int maxStatValue) => (maxStatIndex * 5) + (maxStatValue % 5);

    /// <summary>
    /// Gets the characteristic index of the given IVs in Little Endian format.
    /// </summary>
    /// <param name="ec">Encryption Constant.</param>
    /// <param name="iv32">Lumped IVs in Little Endian format.</param>
    /// <returns>Characteristic index.</returns>
    public static int GetCharacteristic(uint ec, uint iv32)
    {
        int index = (int)(ec % 6);
        var (maxStatIndex, maxStatValue) = GetMaxStat(iv32, index);
        return GetCharacteristic(maxStatIndex, maxStatValue);
    }

    /// <inheritdoc cref="GetCharacteristic(int,int)"/>
    /// <remarks>
    /// Generations 5-9(+?) initialize the "best" characteristic index to 0 (HP).
    /// In the event that all IVs are 0, the characteristic index is also 0, regardless of EC % 6.
    /// The original implementation in Gen4 has the "correct" behavior, being based on EC % 6; as does HOME on mobile.
    /// Only formats that display the all-zero-IV characteristic as always-"HP" reference this method.
    /// </remarks>
    public static int GetCharacteristicInit0(uint ec, uint iv32) => iv32 == 0 ? 0 : GetCharacteristic(ec, iv32);

    /// <summary>
    /// Gets the characteristic index of the given unpacked IVs.
    /// </summary>
    /// <param name="ec">Encryption Constant.</param>
    /// <param name="ivs">Unpacked IVs.</param>
    /// <returns>Characteristic index.</returns>
    public static int GetCharacteristic(uint ec, Span<int> ivs)
    {
        int index = (int)(ec % 6);
        var (maxStatIndex, maxStatValue) = GetMaxStat(ivs, index);
        return GetCharacteristic(maxStatIndex, maxStatValue);
    }

    /// <summary>
    /// Gets the characteristic index of the given IVs when the IVs are stored in reverse order (HP highest bits).
    /// </summary>
    /// <param name="ec">Encryption Constant.</param>
    /// <param name="iv32">Lumped IVs with HP occupying the highest bits.</param>
    /// <returns>Characteristic index.</returns>
    public static int GetCharacteristicInvertFields(uint ec, uint iv32)
    {
        int index = (int)(ec % 6);
        var (maxStatIndex, maxStatValue) = GetMaxStat32Invert(iv32, index);
        return GetCharacteristic(maxStatIndex, maxStatValue);
    }

    private static (int MaxStatIndex, int MaxStatValue) GetMaxStat(uint iv32, int index)
    {
        // Get individual IVs from the lumped value.
        // The IVs are stored in the following order: HP, Atk, Def, Spe, SpA, SpD
        // Check all IVs, get the highest IV and its index. If there are multiple highest IVs, the first index checked is chosen.
        int maxStatIndex = index;
        var maxStatValue = 0u;
        do
        {
            var value = iv32 >> (index * 5) & 0x1F;
            if (value > maxStatValue)
            {
                maxStatIndex = index;
                maxStatValue = value;
            }
            if (index != 5)
                index++;
            else
                index = 0;
        } while (maxStatIndex != index);

        return (maxStatIndex, (int)maxStatValue);
    }

    private static (int MaxStatIndex, int MaxStatValue) GetMaxStat(Span<int> ivs, int index)
    {
        // Get individual IVs from the lumped value.
        // The IVs are stored in the following order: HP, Atk, Def, Spe, SpA, SpD
        // Check all IVs, get the highest IV and its index. If there are multiple highest IVs, the first index checked is chosen.
        int maxStatIndex = index;
        var maxStatValue = 0;
        do
        {
            var value = ivs[index];
            if (value > maxStatValue)
            {
                maxStatIndex = index;
                maxStatValue = value;
            }
            if (index != 5)
                index++;
            else
                index = 0;
        } while (maxStatIndex != index);

        return (maxStatIndex, maxStatValue);
    }

    private static (int MaxStatIndex, int MaxStatValue) GetMaxStat32Invert(uint iv32, int index)
    {
        // Same as GetMaxStat, but for formats where the IVs are stored in reverse order.
        int maxStatIndex = index;
        var maxStatValue = 0u;
        do
        {
            // IVs are stored in reverse order, get the bits from the end of the IV value
            var value = iv32 >> (27 - (index * 5)) & 0x1F;
            if (value > maxStatValue)
            {
                maxStatIndex = index;
                maxStatValue = value;
            }
            if (index != 5)
                index++;
            else
                index = 0;
        } while (maxStatIndex != index);

        return (maxStatIndex, (int)maxStatValue);
    }
}
