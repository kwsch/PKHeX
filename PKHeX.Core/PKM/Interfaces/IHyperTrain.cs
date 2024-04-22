using System;

namespace PKHeX.Core;

/// <summary>
/// Exposes flags to indicate if a stat index has been hyper trained.
/// </summary>
public interface IHyperTrain
{
    byte HyperTrainFlags { get; set; }
    bool HT_HP { get; set; }
    bool HT_ATK { get; set; }
    bool HT_DEF { get; set; }
    bool HT_SPA { get; set; }
    bool HT_SPD { get; set; }
    bool HT_SPE { get; set; }
}

public static partial class Extensions
{
    /// <summary>
    /// Toggles the Hyper Training flag for a given stat.
    /// </summary>
    /// <param name="t">Hyper Trainable object</param>
    /// <param name="index">Battle Stat (H/A/B/S/C/D)</param>
    /// <returns>Final Hyper Training Flag value</returns>
    public static bool HyperTrainInvert(this IHyperTrain t, int index) => index switch
    {
        0 => t.HT_HP ^= true,
        1 => t.HT_ATK ^= true,
        2 => t.HT_DEF ^= true,
        3 => t.HT_SPE ^= true,
        4 => t.HT_SPA ^= true,
        5 => t.HT_SPD ^= true,
        _ => false,
    };

    public static bool IsHyperTrainedAll(this IHyperTrain t) => t.HyperTrainFlags == 0x3F;
    public static void HyperTrainClear(this IHyperTrain t) => t.HyperTrainFlags = 0;
    public static bool IsHyperTrained(this IHyperTrain t) => t.HyperTrainFlags != 0;

    /// <summary>
    /// Gets one of the <see cref="IHyperTrain"/> values based on its index within the array.
    /// </summary>
    /// <param name="t">Entity to check.</param>
    /// <param name="index">Index to get</param>
    public static bool IsHyperTrained(this IHyperTrain t, int index) => index switch
    {
        0 => t.HT_HP,
        1 => t.HT_ATK,
        2 => t.HT_DEF,
        3 => t.HT_SPE,
        4 => t.HT_SPA,
        5 => t.HT_SPD,
        _ => throw new ArgumentOutOfRangeException(nameof(index)),
    };

    /// <summary>
    /// Sets <see cref="IHyperTrain.HyperTrainFlags"/> to valid values which may best enhance the <see cref="PKM"/> stats.
    /// </summary>
    /// <param name="pk"></param>
    /// <param name="h">History of evolutions present as</param>
    /// <param name="IVs"><see cref="PKM.IVs"/> to use (if already known). Will fetch the current <see cref="PKM.IVs"/> if not provided.</param>
    public static void SetSuggestedHyperTrainingData(this PKM pk, EvolutionHistory h, ReadOnlySpan<int> IVs)
    {
        if (pk is not IHyperTrain t)
            return;

        if (!pk.IsHyperTrainingAvailable(h))
            t.HyperTrainFlags = 0;
        else
            t.SetSuggestedHyperTrainingData(pk, IVs);
    }

    /// <inheritdoc cref="SetSuggestedHyperTrainingData(PKM,EvolutionHistory,ReadOnlySpan{int})"/>
    public static void SetSuggestedHyperTrainingData(this PKM pk, EntityContext c, ReadOnlySpan<int> IVs)
    {
        if (pk is not IHyperTrain t)
            return;

        if (!c.IsHyperTrainingAvailable(pk.CurrentLevel))
            t.HyperTrainFlags = 0;
        else
            t.SetSuggestedHyperTrainingData(pk, IVs);
    }

    private static void SetSuggestedHyperTrainingData(this IHyperTrain t, PKM pk, ReadOnlySpan<int> IVs)
    {
        t.HT_HP = IVs[0] != 31;
        t.HT_ATK = IVs[1] != 31 && IVs[1] > 2;
        t.HT_DEF = IVs[2] != 31;
        t.HT_SPA = IVs[4] != 31;
        t.HT_SPD = IVs[5] != 31;
        t.HT_SPE = IsSpeedHyperTrainSuggested(t, IVs);

        if (pk is ICombatPower pb)
            pb.ResetCP();
    }

    private static bool IsSpeedHyperTrainSuggested(IHyperTrain t, ReadOnlySpan<int> IVs)
    {
        // sometimes unusual speed IVs are desirable for competitive reasons
        var speed = IVs[3];
        if (speed is 31 or <= 2)
            return false;

        // if IV isn't too high and nothing else was HT'd, it was probably intentional to stay low.
        if (speed > 17)
            return true;
        return t.HT_HP || t.HT_ATK || t.HT_DEF || t.HT_SPA || t.HT_SPD;
    }

    /// <inheritdoc cref="SetSuggestedHyperTrainingData(PKM,EvolutionHistory,ReadOnlySpan{int})"/>
    public static void SetSuggestedHyperTrainingData(this PKM pk, ReadOnlySpan<int> IVs)
    {
        pk.SetSuggestedHyperTrainingData(pk.Context, IVs);
    }

    /// <inheritdoc cref="SetSuggestedHyperTrainingData(PKM,EvolutionHistory,ReadOnlySpan{int})"/>
    public static void SetSuggestedHyperTrainingData(this PKM pk)
    {
        Span<int> ivs = stackalloc int[6];
        pk.GetIVs(ivs);
        pk.SetSuggestedHyperTrainingData(ivs);
    }

    /// <summary>
    /// Indicates if Hyper Training is available to be set.
    /// </summary>
    /// <param name="t">Entity to train</param>
    /// <param name="h">History of evolutions present as</param>
    /// <returns>True if available, otherwise false.</returns>
    public static bool IsHyperTrainingAvailable(this IHyperTrain t, EvolutionHistory h) => t switch
    {
        // Check for game formats where training is unavailable:
        PA8 => h.HasVisitedGen7 || h.HasVisitedSWSH || h.HasVisitedBDSP || h.HasVisitedGen9,
        _ => true,
    };

    /// <inheritdoc cref="IsHyperTrainingAvailable(IHyperTrain,EvolutionHistory)"/>
    public static bool IsHyperTrainingAvailable(this EntityContext c, byte currentLevel)
    {
        var min = GetHyperTrainMinLevel(c);
        return currentLevel >= min;
    }

    /// <inheritdoc cref="GetHyperTrainMinLevel(IHyperTrain,EvolutionHistory, EntityContext)"/>
    public static int GetHyperTrainMinLevel(this EntityContext c) => c switch
    {
        EntityContext.Gen7 or EntityContext.Gen8 or EntityContext.Gen8b => 100,
        EntityContext.Gen9 => 50,
        _ => 101,
    };

    /// <summary>
    /// Gets the minimum level required for Hyper Training an IV.
    /// </summary>
    /// <param name="_">Entity to train</param>
    /// <param name="h">History of evolutions present as</param>
    /// <param name="current">Current context</param>
    /// <returns>True if available, otherwise false.</returns>
    public static int GetHyperTrainMinLevel(this IHyperTrain _, EvolutionHistory h, EntityContext current)
    {
        // HOME 3.0.0+ disallows inbound transfers of Hyper Trained Pokémon below level 100.
        // PokeDupeChecker in BD/SP will DprIllegal if < 100, even if it was legitimately trained in S/V+.
        if (current == EntityContext.Gen8b)
            return 100;

        if (h.HasVisitedGen9)
            return 50;
        return 100;
    }

    /// <inheritdoc cref="IsHyperTrainingAvailable(IHyperTrain, EvolutionHistory)"/>
    /// <param name="pk">Entity data</param>
    /// <param name="h">History of evolutions present as</param>
    public static bool IsHyperTrainingAvailable(this PKM pk, EvolutionHistory h)
    {
        if (pk is not IHyperTrain t)
            return false;
        if (!t.IsHyperTrainingAvailable(h))
            return false;

        // Gated behind level.
        var min = t.GetHyperTrainMinLevel(h, pk.Context);
        return pk.CurrentLevel >= min;
    }
}
