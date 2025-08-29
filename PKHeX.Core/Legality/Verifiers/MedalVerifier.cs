using System;
using static PKHeX.Core.LegalityCheckResultCode;

namespace PKHeX.Core;

/// <summary>
/// Verifies the <see cref="ISuperTrain.SuperTrainBitFlags"/> and associated values.
/// </summary>
public sealed class MedalVerifier : Verifier
{
    protected override CheckIdentifier Identifier => CheckIdentifier.Training;

    public override void Verify(LegalityAnalysis data)
    {
        if (data.Entity is not ISuperTrain s)
            return;
        VerifyMedalsRegular(data, s);
        VerifyMedalsEvent(data, s);

        if (data.Entity is PK6 pk6)
            VerifyTrainingBag(data, pk6.TrainingBag, pk6.TrainingBagHits);
    }

    private void VerifyTrainingBag(LegalityAnalysis data, byte bag, byte hits)
    {
        var pk = data.Entity;
        if (pk.IsEgg)
        {
            if (bag != 0)
                data.AddLine(GetInvalid(G6SuperTrainEggBag));
            if (hits != 0)
                data.AddLine(GetInvalid(G6SuperTrainEggHits));
            return;
        }

        var b = (SuperTrainBag)bag;
        if (!b.IsValid())
        {
            data.AddLine(GetInvalid(G6SuperTrainBagInvalid_0));
            return;
        }
        if (!b.IsValid(hits))
        {
            var range = b.GetRange();
            data.AddLine(GetInvalid(G6SuperTrainBagHitsInvalid_012, range.Min, range.Max));
        }
    }

    private void VerifyMedalsRegular(LegalityAnalysis data, ISuperTrain train)
    {
        var pk = data.Entity;
        var Info = data.Info;
        uint value = train.SuperTrainBitFlags;
        if ((value & 3) != 0) // 2 unused flags
            data.AddLine(GetInvalid(SuperUnused));

        if (pk.IsEgg)
        {
            VerifySuperEgg(data, train, value);
            return;
        }

        if (Info.Generation is >= 7 or <= 2)
        {
            VerifyTrainNoVisit6(data, train, value);
            return;
        }

        // To acquire the Secret Super Training unlocked flag, it must finish a regimen and have max EVs.
        // Since EVs can be reduced, there is no need to verify the lower bits.
        // Finishing a regimen doesn't require great performance (to acquire a medal), just completion (+EVs, hit 510 cap).

        if (pk.Format >= 7)
        {
            // Gen6->Gen7 transfer wipes the two Secret flags.
            if (train.SecretSuperTrainingUnlocked)
                data.AddLine(GetInvalid(SuperNoUnlocked));
            if (train.SuperTrainSupremelyTrained)
                data.AddLine(GetInvalid(SuperNoComplete));
            return;
        }

        // Only reach here if Format==6.
        // The Secret Unlocked flag doesn't require any medals, as mentioned above.
        // The Secret Complete flag does require all Secret medals.
        var actuallyComplete = RibbonRules.IsSuperTrainSupremelyTrained(value);
        if (train.SuperTrainSupremelyTrained != actuallyComplete)
            data.AddLine(GetInvalid(SuperComplete));
    }

    private void VerifySuperEgg(LegalityAnalysis data, ISuperTrain train, uint bits)
    {
        // Can't have any super training data as an egg.
        if (bits != 0)
            data.AddLine(GetInvalid(SuperEgg));
        if (train.SecretSuperTrainingUnlocked)
            data.AddLine(GetInvalid(SuperNoUnlocked));
        if (train.SuperTrainSupremelyTrained)
            data.AddLine(GetInvalid(SuperNoComplete));
    }

    private void VerifyTrainNoVisit6(LegalityAnalysis data, ISuperTrain train, uint bits)
    {
        // Can't have any super training data if it never visited Gen6.
        if (bits != 0)
            data.AddLine(GetInvalid(SuperUnavailable));
        if (train.SecretSuperTrainingUnlocked)
            data.AddLine(GetInvalid(SuperNoUnlocked));
        if (train.SuperTrainSupremelyTrained)
            data.AddLine(GetInvalid(SuperNoComplete));
    }

    private void VerifyMedalsEvent(LegalityAnalysis data, ISuperTrain train)
    {
        // Event regimens were never distributed.
        if (train.DistTrainBitFlags != 0)
            data.AddLine(GetInvalid(SuperDistro));
    }
}

/// <summary>
/// Utility extensions for <see cref="SuperTrainBag"/>.
/// </summary>
public static class SuperTrainBagExtensions
{
    public static ReadOnlySpan<byte> Min =>
    [
        0,
        1,1,1, // HP
        1,1,1, // ATK
        1,1,1, // DEF
        1,1,1, // SpA
        1,1,1, // SpD
        1,1,1, // SPE
        0, // Strength
        0, // Toughen Up
        0, // Swiftness
        0, // Big Shot
        0, // Double Up
        1, // Team Flare (bonus stored elsewhere when activated)
        1, // Reset
        1, // Soothing Bag
    ];

    public static ReadOnlySpan<byte> Max =>
    [
        0,
        2,5,10, // HP
        2,5,10, // ATK
        2,5,10, // DEF
        2,5,10, // SpA
        2,5,10, // SpD
        2,5,10, // SPE
        2, // Strength
        2, // Toughen Up
        2, // Swiftness
        2, // Big Shot
        2, // Double Up
        2, // Team Flare
        20, // Reset
        50,  // Soothing Bag
    ];

    /// <summary>
    /// Gets the valid range of hits for the (valid) specified bag.
    /// </summary>
    public static (byte Min, byte Max) GetRange(this SuperTrainBag bag)
    {
        if (bag > SuperTrainBag.SoothingBag)
            throw new ArgumentOutOfRangeException(nameof(bag), bag, null);
        return (Min[(int)bag], Max[(int)bag]);
    }

    /// <summary>
    /// Checks if the bag value is within the valid enum range and if the hits are within the valid range for that bag.
    /// </summary>
    public static bool IsValid(this SuperTrainBag bag, byte hits)
    {
        if (bag > SuperTrainBag.SoothingBag)
            return false;
        var (min, max) = bag.GetRange();
        return hits >= min && hits <= max;
    }

    /// <summary>
    /// Checks if the bag value is within the valid enum range.
    /// </summary>
    public static bool IsValid(this SuperTrainBag bag) => bag <= SuperTrainBag.SoothingBag;

    /// <summary>
    /// The bag value can stick around with a hit count of 0 if it is a lingering effect bag.
    /// </summary>
    /// <returns><c>true</c> if the bag can linger with 0 hits, <c>false</c> otherwise.</returns>
    public static bool CanLinger(this SuperTrainBag bag) => bag is
        SuperTrainBag.Strength or
        SuperTrainBag.ToughenUp or
        SuperTrainBag.Swiftness or
        SuperTrainBag.BigShot or
        SuperTrainBag.DoubleUp;
}

public enum SuperTrainBag : byte
{
    None = 0,
    HPBagS,
    HPBagM,
    HPBagL,
    AttackBagS,
    AttackBagM,
    AttackBagL,
    DefenseBagS,
    DefenseBagM,
    DefenseBagL,
    SpAtkBagS,
    SpAtkBagM,
    SpAtkBagL,
    SpDefBagS,
    SpDefBagM,
    SpDefBagL,
    SpeedBagS,
    SpeedBagM,
    SpeedBagL,
    Strength,
    ToughenUp,
    Swiftness,
    BigShot,
    DoubleUp,
    TeamFlare,
    Reset,
    SoothingBag,
}

