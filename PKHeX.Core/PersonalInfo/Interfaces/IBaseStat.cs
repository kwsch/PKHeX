using System;

namespace PKHeX.Core;

/// <summary>
/// Exposes details about base stat values.
/// </summary>
public interface IBaseStat
{
    /// <summary>
    /// Base HP
    /// </summary>
    int HP { get; set; }

    /// <summary>
    /// Base Attack
    /// </summary>
    int ATK { get; set; }

    /// <summary>
    /// Base Defense
    /// </summary>
    int DEF { get; set; }

    /// <summary>
    /// Base Special Attack
    /// </summary>
    int SPA { get; set; }

    /// <summary>
    /// Base Special Defense
    /// </summary>
    int SPD { get; set; }

    /// <summary>
    /// Base Speed
    /// </summary>
    int SPE { get; set; }
}

public static class BaseStatExtensions
{
    extension(IBaseStat stats)
    {
        /// <summary>
        /// Base Stat Total sum of all stats.
        /// </summary>
        public int BST => stats.GetBaseStatTotal();

        /// <summary>
        /// Base Stat Total sum of all stats.
        /// </summary>
        public int GetBaseStatTotal() => stats.HP + stats.ATK + stats.DEF + stats.SPA + stats.SPD + stats.SPE;

        /// <summary>
        /// Gets the requested Base Stat value with the requested <see cref="index"/>.
        /// </summary>
        public int GetBaseStatValue(int index) => index switch
        {
            0 => stats.HP,
            1 => stats.ATK,
            2 => stats.DEF,
            3 => stats.SPE,
            4 => stats.SPA,
            5 => stats.SPD,
            _ => throw new ArgumentOutOfRangeException(nameof(index)),
        };

        /// <summary>
        /// Gathers the base stat values into the <see cref="span"/> then sorts (by value) lowest to highest.
        /// </summary>
        /// <param name="span">Result storage</param>
        public void GetSortedStatIndexes(Span<(int Index, int Stat)> span)
        {
            for (int i = 0; i < span.Length; i++)
                span[i] = (i, stats.GetBaseStatValue(i));

            // Bubble sort based off Stat value
            // Higher stat values go to lower indexes in the span.
            for (int i = 0; i < span.Length - 1; i++)
            {
                for (int j = 0; j < span.Length - 1 - i; j++)
                {
                    if (span[j].Stat < span[j + 1].Stat)
                        (span[j], span[j + 1]) = (span[j + 1], span[j]);
                }
            }
        }
    }
}
