using System;

namespace PKHeX.Core;

public static class EntityCharacteristic
{
    public static int GetCharacteristic(uint ec, uint iv32)
    {
        int index = (int)(ec % 6);

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

        return (maxStatIndex * 5) + ((int)maxStatValue % 5);
    }

    public static int GetCharacteristic(uint ec, Span<int> ivs)
    {
        int index = (int)(ec % 6);

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

        return (maxStatIndex * 5) + (maxStatValue % 5);
    }

    public static int GetCharacteristicInvertFields(uint ec, uint iv32)
    {
        int index = (int)(ec % 6);

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

        return (maxStatIndex * 5) + ((int)maxStatValue % 5);
    }
}
