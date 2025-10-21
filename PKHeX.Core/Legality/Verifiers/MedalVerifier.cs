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
            var max = b.GetMax();
            data.AddLine(GetInvalid(G6SuperTrainBagHitsInvalid_012, 0, max));
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
    private static ReadOnlySpan<byte> Max =>
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

    // Min is always 0 -- can immediately deposit before finishing a bag.

    /// <summary>
    /// Retrieves the maximum value associated with the specified <see cref="SuperTrainBag"/>.
    /// </summary>
    /// <returns>The maximum value as a <see cref="byte"/> if the <paramref name="bag"/> is valid; otherwise, 0.</returns>
    public static byte GetMax(this SuperTrainBag bag)
    {
        if (!bag.IsValid())
            return 0;
        return Max[(byte)bag];
    }

    /// <summary>
    /// Checks if the bag value is within the valid enum range and if the hits are within the valid range for that bag.
    /// </summary>
    public static bool IsValid(this SuperTrainBag bag, byte hits)
    {
        if (bag > SuperTrainBag.SoothingBag)
            return false;
        return hits <= bag.GetMax();
    }

    /// <summary>
    /// Checks if the bag value is within the valid enum range.
    /// </summary>
    public static bool IsValid(this SuperTrainBag bag) => bag <= SuperTrainBag.SoothingBag;
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

