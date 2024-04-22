using System;
using System.Runtime.CompilerServices;

namespace PKHeX.Core;

/// <summary>
/// Calculations for <see cref="PKM.EXP"/> and <see cref="PKM.CurrentLevel"/>.
/// </summary>
public static class Experience
{
    /// <summary>
    /// Gets the current level of a species.
    /// </summary>
    /// <param name="exp">Experience points</param>
    /// <param name="growth">Experience growth rate</param>
    /// <returns>Current level of the species.</returns>
    public static byte GetLevel(uint exp, byte growth)
    {
        var table = GetTable(growth);
        return GetLevel(exp, table);
    }

    /// <summary>
    /// Gets the current level of a species.
    /// </summary>
    /// <param name="exp">Experience points</param>
    /// <param name="table">Experience growth table</param>
    /// <returns>Current level of the species.</returns>
    public static byte GetLevel(uint exp, ReadOnlySpan<uint> table)
    {
        // Eagerly return 100 if the exp is at max
        // Also avoids overflow issues with the table in the event EXP is out of bounds
        if (exp >= table[^1])
            return 100;

        // Most will be below level 50, so start from the bottom
        // Don't bother with binary search, as the table is small
        byte tl = 1; // Initial Level. Iterate upwards to find the level
        while (exp >= table[tl])
            ++tl;
        return tl;
    }

    /// <summary>
    /// Gets the minimum Experience points for the specified level.
    /// </summary>
    /// <param name="level">Current level</param>
    /// <param name="growth">Growth Rate type</param>
    /// <returns>Experience points needed to have specified level.</returns>
    public static uint GetEXP(byte level, byte growth)
    {
        if (level <= 1)
            return 0;
        if (level > 100)
            level = 100;

        var table = GetTable(growth);
        return GetEXP(level, table);
    }

    /// <summary>
    /// Gets the minimum Experience points for the specified level.
    /// </summary>
    /// <param name="level">Current level</param>
    /// <param name="table">Experience growth table</param>
    /// <returns>Experience points needed to have specified level.</returns>
    /// <remarks>No bounds checking is performed.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint GetEXP(byte level, ReadOnlySpan<uint> table) => table[level - 1];

    /// <summary>
    /// Gets the minimum Experience points for all levels possible.
    /// </summary>
    /// <param name="growth">Growth Rate type</param>
    /// <returns>Experience points needed to have an indexed level.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static ReadOnlySpan<uint> GetTable(byte growth) => growth switch
    {
        0 => Growth0,
        1 => Growth1,
        2 => Growth2,
        3 => Growth3,
        4 => Growth4,
        5 => Growth5,
        _ => throw new ArgumentOutOfRangeException(nameof(growth)),
    };

    /// <summary>
    /// Gets the <see cref="PKM.Nature"/> value for <see cref="PK1"/> / <see cref="PK2"/> entries based on the <see cref="PKM.EXP"/>
    /// </summary>
    /// <param name="experience">Current Experience</param>
    /// <returns>Nature ID (<see cref="Nature"/>)</returns>
    public static Nature GetNatureVC(uint experience) => (Nature)(experience % 25);

    /// <summary>
    /// Gets the amount of EXP to be earned until the next level-up occurs.
    /// </summary>
    /// <param name="level">Current Level</param>
    /// <param name="growth">Growth Rate type</param>
    /// <returns>EXP to level up</returns>
    public static uint GetEXPToLevelUp(byte level, byte growth)
    {
        if (level >= 100)
            return 0;
        var table = GetTable(growth);
        var current = GetEXP(level, table);
        var next = GetEXP(++level, table);
        return next - current;
    }

    /// <summary>
    /// Gets a percentage for Experience Bar progress indication.
    /// </summary>
    /// <param name="level">Current Level</param>
    /// <param name="exp">Current Experience</param>
    /// <param name="growth">Growth Rate type</param>
    /// <returns>Percentage [0,1.00)</returns>
    public static double GetEXPToLevelUpPercentage(byte level, uint exp, byte growth)
    {
        if (level >= 100)
            return 0;

        var table = GetTable(growth);
        var current = GetEXP(level, table);
        var next = GetEXP(++level, table);
        var amount = next - current;
        double progress = exp - current;
        return progress / amount;
    }

    #region ExpTable

    private static ReadOnlySpan<uint> Growth0 =>
    [
        0000000, 0000008, 0000027, 0000064, 0000125, 0000216, 0000343, 0000512, 0000729, 0001000,
        0001331, 0001728, 0002197, 0002744, 0003375, 0004096, 0004913, 0005832, 0006859, 0008000,
        0009261, 0010648, 0012167, 0013824, 0015625, 0017576, 0019683, 0021952, 0024389, 0027000,
        0029791, 0032768, 0035937, 0039304, 0042875, 0046656, 0050653, 0054872, 0059319, 0064000,
        0068921, 0074088, 0079507, 0085184, 0091125, 0097336, 0103823, 0110592, 0117649, 0125000,
        0132651, 0140608, 0148877, 0157464, 0166375, 0175616, 0185193, 0195112, 0205379, 0216000,
        0226981, 0238328, 0250047, 0262144, 0274625, 0287496, 0300763, 0314432, 0328509, 0343000,
        0357911, 0373248, 0389017, 0405224, 0421875, 0438976, 0456533, 0474552, 0493039, 0512000,
        0531441, 0551368, 0571787, 0592704, 0614125, 0636056, 0658503, 0681472, 0704969, 0729000,
        0753571, 0778688, 0804357, 0830584, 0857375, 0884736, 0912673, 0941192, 0970299, 1000000,
    ];

    private static ReadOnlySpan<uint> Growth1 =>
    [
        0000000, 0000015, 0000052, 0000122, 0000237, 0000406, 0000637, 0000942, 0001326, 0001800,
        0002369, 0003041, 0003822, 0004719, 0005737, 0006881, 0008155, 0009564, 0011111, 0012800,
        0014632, 0016610, 0018737, 0021012, 0023437, 0026012, 0028737, 0031610, 0034632, 0037800,
        0041111, 0044564, 0048155, 0051881, 0055737, 0059719, 0063822, 0068041, 0072369, 0076800,
        0081326, 0085942, 0090637, 0095406, 0100237, 0105122, 0110052, 0115015, 0120001, 0125000,
        0131324, 0137795, 0144410, 0151165, 0158056, 0165079, 0172229, 0179503, 0186894, 0194400,
        0202013, 0209728, 0217540, 0225443, 0233431, 0241496, 0249633, 0257834, 0267406, 0276458,
        0286328, 0296358, 0305767, 0316074, 0326531, 0336255, 0346965, 0357812, 0367807, 0378880,
        0390077, 0400293, 0411686, 0423190, 0433572, 0445239, 0457001, 0467489, 0479378, 0491346,
        0501878, 0513934, 0526049, 0536557, 0548720, 0560922, 0571333, 0583539, 0591882, 0600000,
    ];

    private static ReadOnlySpan<uint> Growth2 =>
    [
        0000000, 0000004, 0000013, 0000032, 0000065, 0000112, 0000178, 0000276, 0000393, 0000540,
        0000745, 0000967, 0001230, 0001591, 0001957, 0002457, 0003046, 0003732, 0004526, 0005440,
        0006482, 0007666, 0009003, 0010506, 0012187, 0014060, 0016140, 0018439, 0020974, 0023760,
        0026811, 0030146, 0033780, 0037731, 0042017, 0046656, 0050653, 0055969, 0060505, 0066560,
        0071677, 0078533, 0084277, 0091998, 0098415, 0107069, 0114205, 0123863, 0131766, 0142500,
        0151222, 0163105, 0172697, 0185807, 0196322, 0210739, 0222231, 0238036, 0250562, 0267840,
        0281456, 0300293, 0315059, 0335544, 0351520, 0373744, 0390991, 0415050, 0433631, 0459620,
        0479600, 0507617, 0529063, 0559209, 0582187, 0614566, 0639146, 0673863, 0700115, 0737280,
        0765275, 0804997, 0834809, 0877201, 0908905, 0954084, 0987754, 1035837, 1071552, 1122660,
        1160499, 1214753, 1254796, 1312322, 1354652, 1415577, 1460276, 1524731, 1571884, 1640000,
    ];

    private static ReadOnlySpan<uint> Growth3 =>
    [
        0000000, 0000009, 0000057, 0000096, 0000135, 0000179, 0000236, 0000314, 0000419, 0000560,
        0000742, 0000973, 0001261, 0001612, 0002035, 0002535, 0003120, 0003798, 0004575, 0005460,
        0006458, 0007577, 0008825, 0010208, 0011735, 0013411, 0015244, 0017242, 0019411, 0021760,
        0024294, 0027021, 0029949, 0033084, 0036435, 0040007, 0043808, 0047846, 0052127, 0056660,
        0061450, 0066505, 0071833, 0077440, 0083335, 0089523, 0096012, 0102810, 0109923, 0117360,
        0125126, 0133229, 0141677, 0150476, 0159635, 0169159, 0179056, 0189334, 0199999, 0211060,
        0222522, 0234393, 0246681, 0259392, 0272535, 0286115, 0300140, 0314618, 0329555, 0344960,
        0360838, 0377197, 0394045, 0411388, 0429235, 0447591, 0466464, 0485862, 0505791, 0526260,
        0547274, 0568841, 0590969, 0613664, 0636935, 0660787, 0685228, 0710266, 0735907, 0762160,
        0789030, 0816525, 0844653, 0873420, 0902835, 0932903, 0963632, 0995030, 1027103, 1059860,
    ];

    private static ReadOnlySpan<uint> Growth4 =>
    [
        0000000, 0000006, 0000021, 0000051, 0000100, 0000172, 0000274, 0000409, 0000583, 0000800,
        0001064, 0001382, 0001757, 0002195, 0002700, 0003276, 0003930, 0004665, 0005487, 0006400,
        0007408, 0008518, 0009733, 0011059, 0012500, 0014060, 0015746, 0017561, 0019511, 0021600,
        0023832, 0026214, 0028749, 0031443, 0034300, 0037324, 0040522, 0043897, 0047455, 0051200,
        0055136, 0059270, 0063605, 0068147, 0072900, 0077868, 0083058, 0088473, 0094119, 0100000,
        0106120, 0112486, 0119101, 0125971, 0133100, 0140492, 0148154, 0156089, 0164303, 0172800,
        0181584, 0190662, 0200037, 0209715, 0219700, 0229996, 0240610, 0251545, 0262807, 0274400,
        0286328, 0298598, 0311213, 0324179, 0337500, 0351180, 0365226, 0379641, 0394431, 0409600,
        0425152, 0441094, 0457429, 0474163, 0491300, 0508844, 0526802, 0545177, 0563975, 0583200,
        0602856, 0622950, 0643485, 0664467, 0685900, 0707788, 0730138, 0752953, 0776239, 0800000,
    ];

    private static ReadOnlySpan<uint> Growth5 =>
    [
        0000000, 0000010, 0000033, 0000080, 0000156, 0000270, 0000428, 0000640, 0000911, 0001250,
        0001663, 0002160, 0002746, 0003430, 0004218, 0005120, 0006141, 0007290, 0008573, 0010000,
        0011576, 0013310, 0015208, 0017280, 0019531, 0021970, 0024603, 0027440, 0030486, 0033750,
        0037238, 0040960, 0044921, 0049130, 0053593, 0058320, 0063316, 0068590, 0074148, 0080000,
        0086151, 0092610, 0099383, 0106480, 0113906, 0121670, 0129778, 0138240, 0147061, 0156250,
        0165813, 0175760, 0186096, 0196830, 0207968, 0219520, 0231491, 0243890, 0256723, 0270000,
        0283726, 0297910, 0312558, 0327680, 0343281, 0359370, 0375953, 0393040, 0410636, 0428750,
        0447388, 0466560, 0486271, 0506530, 0527343, 0548720, 0570666, 0593190, 0616298, 0640000,
        0664301, 0689210, 0714733, 0740880, 0767656, 0795070, 0823128, 0851840, 0881211, 0911250,
        0941963, 0973360, 1005446, 1038230, 1071718, 1105920, 1140841, 1176490, 1212873, 1250000,
    ];

    #endregion
}
