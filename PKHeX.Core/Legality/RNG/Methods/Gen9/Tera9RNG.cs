using System;

namespace PKHeX.Core;

/// <summary>
/// RNG Correlation Logic for <see cref="ITeraRaid9"/> encounters.
/// </summary>
public static class Tera9RNG
{
    /// <summary>
    /// Type max (exclusive) to use when picking a random a Tera Type.
    /// </summary>
    private const uint TeraTypeCount = 18;

    /// <summary>
    /// Checks if the <see cref="ITeraType.TeraTypeOriginal"/> matches the specification of the <see cref="gem"/> value.
    /// </summary>
    /// <param name="seed">Seed used to generate the Tera Type</param>
    /// <param name="gem">Encounter specified Gem Type</param>
    /// <param name="species">Encounter Species</param>
    /// <param name="form">Encounter Form</param>
    /// <param name="original">Original Tera Type from the Entity</param>
    /// <returns>True if the Tera Type matches the RNG and specification</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static bool IsMatchTeraType(in uint seed, in GemType gem, in ushort species, in byte form, in byte original)
    {
        if (gem.IsSpecified(out var type))
            return original == type;

        var rand = new Xoroshiro128Plus(seed);
        if (gem == GemType.Default)
        {
            var pivot = rand.NextInt(2);
            var expect = GetTeraTypeFromPersonal(species, form, pivot);
            return original == expect;
        }
        if (gem == GemType.Random)
        {
            var expect = rand.NextInt(TeraTypeCount);
            return original == expect;
        }
        throw new ArgumentOutOfRangeException(nameof(gem), gem, null);
    }

    /// <summary>
    /// Checks if the original Tera Type matches the specification of the <see cref="gem"/> value.
    /// </summary>
    public static bool IsMatchTeraType(in GemType gem, in ushort species, in byte form, in byte original)
    {
        if (gem == GemType.Random)
            return true;
        if (gem.IsSpecified(out var type))
            return original == type;
        if (gem == GemType.Default)
            return IsMatchTeraTypePersonal(species, form, original);
        throw new ArgumentOutOfRangeException(nameof(gem), gem, null);
    }

    /// <summary>
    /// Gets the expected Tera Type from the Personal Info and RNG seed.
    /// </summary>
    /// <param name="seed">Seed used to generate the Tera Type</param>
    /// <param name="gem">Encounter specified Gem Type</param>
    /// <param name="species">Encounter Species</param>
    /// <param name="form">Encounter Form</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static byte GetTeraType(in ulong seed, GemType gem, in ushort species, in byte form)
    {
        if (gem.IsSpecified(out var type))
            return type;

        var rand = new Xoroshiro128Plus(seed);
        if (gem == GemType.Default)
        {
            var pivot = rand.NextInt(2);
            return GetTeraTypeFromPersonal(species, form, pivot);
        }
        if (gem == GemType.Random)
        {
            return (byte)rand.NextInt(TeraTypeCount);
        }
        throw new ArgumentOutOfRangeException(nameof(gem), gem, null);
    }

    /// <summary>
    /// Checks if the original Tera Type matches either of the Personal Info types.
    /// </summary>
    private static bool IsMatchType(IPersonalType pi, in byte original) => original == pi.Type1 || original == pi.Type2;

    /// <summary>
    /// Checks if the original Tera Type matches the Personal Info type for the specified form.
    /// </summary>
    /// <param name="species">Egg Species</param>
    /// <param name="form">Egg Form</param>
    /// <param name="original">Original Tera Type from the Entity</param>
    /// <returns>True if the Tera Type matches the expected Personal Info type</returns>
    /// <remarks>
    /// Special consideration is required as some eggs can change forms that have different Personal Info types.
    /// </remarks>
    public static bool IsMatchTeraTypePersonalEgg(in ushort species, in byte form, in byte original) =>
        FormInfo.IsFormChangeEggTypes(species)
            ? IsMatchTeraTypePersonalAnyForm(species, original)
            : IsMatchTeraTypePersonal(species, form, original);

    /// <inheritdoc cref="IsMatchType"/>
    public static bool IsMatchTeraTypePersonal(in ushort species, in byte form, in byte original) => IsMatchType(PersonalTable.SV[species, form], original);

    /// <summary>
    /// Checks if the original Tera Type matches any of the Personal Info types for any form it may change into.
    /// </summary>
    /// <remarks>Only enter this method if it is permitted to change into all forms.</remarks>
    /// <param name="species">Encounter Species</param>
    /// <param name="original">Entity's original Tera Type</param>
    /// <returns>True if the Tera Type matches any of the Personal Info types</returns>
    public static bool IsMatchTeraTypePersonalAnyForm(in ushort species, in byte original)
    {
        var pt = PersonalTable.SV;
        var pi = pt.GetFormEntry(species, 0);
        if (pi.IsPresentInGame && IsMatchType(pi, original))
            return true;
        var fc = pi.FormCount;
        for (byte form = 1; form < fc; form++)
        {
            pi = pt.GetFormEntry(species, form);
            if (pi.IsPresentInGame && IsMatchType(pi, original))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if the original Tera Type matches the Personal Info type for the specified form.
    /// </summary>
    /// <remarks>Used for HOME imports into <see cref="GameVersion.SV"/>.</remarks>
    /// <param name="pi">Arrival Personal Info</param>
    /// <param name="original">Entity's original Tera Type</param>
    /// <returns>True if the Tera Type matches the expected Personal Info type</returns>
    private static bool IsMatchTeraTypeImport(PersonalInfo9SV pi, in byte original)
    {
        var import = TeraTypeUtil.GetTeraTypeImport(pi.Type1, pi.Type2);
        return (MoveType)original == import;
    }

    /// <inheritdoc cref="IsMatchTeraTypeImport"/>
    public static bool IsMatchTeraTypePersonalImport(in ushort species, in byte form, in byte original)
    {
        var pi = PersonalTable.SV[species, form];
        return IsMatchTeraTypeImport(pi, original);
    }

    /// <inheritdoc cref="IsMatchTeraTypeImport"/>
    public static bool IsMatchTeraTypePersonalAnyFormImport(in ushort species, in byte original)
    {
        var pt = PersonalTable.SV;
        var pi = pt.GetFormEntry(species, 0);
        if (pi.IsPresentInGame && IsMatchTeraTypeImport(pi, original))
            return true;
        var fc = pi.FormCount;
        for (byte form = 1; form < fc; form++)
        {
            pi = pt.GetFormEntry(species, form);
            if (pi.IsPresentInGame && IsMatchTeraTypeImport(pi, original))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Gets the expected Tera Type from the Personal Info and pivot.
    /// </summary>
    /// <param name="species">Encounter Species</param>
    /// <param name="form">Encounter Form</param>
    /// <param name="pivot">Random pivot to determine which Personal Info type to use</param>
    /// <returns>Expected Tera Type</returns>
    public static byte GetTeraTypeFromPersonal(in ushort species, in byte form, in ulong pivot)
    {
        var pi = PersonalTable.SV[species, form];
        return pivot == 0 ? pi.Type1 : pi.Type2;
    }

    /// <summary>
    /// Compares the Raid Seed to the possible raid bounds to see if the encounter can originate from that seed.
    /// </summary>
    /// <remarks>A given raid seed might yield a different encounter or star count, hence why we need to check.</remarks>
    /// <param name="seed">Random seed used to generate the raid.</param>
    /// <param name="stars">Difficulty rating.</param>
    /// <param name="raidRate">Random weight of the raid to be used in the comparison with the game specific min rates.</param>
    /// <param name="randRateMinScarlet">Total weight of all Scarlet raids prior to this encounter.</param>
    /// <param name="randRateMinViolet">Total weight of all Violet raids prior to this encounter.</param>
    /// <param name="map">Parent map the Raid can be obtained on.</param>
    /// <returns>True if the raid seed can generate the encounter.</returns>
    public static bool IsMatchStarChoice(in uint seed, in byte stars, in byte raidRate, in short randRateMinScarlet, in short randRateMinViolet, TeraRaidMapParent map)
    {
        // When determining a raid, the game takes the u32 seed and does two rand calls.
        // Rand 1: Star count of the raid (depends on game progress).
        // Rand 2: Raid opponent choice (based on total possible rates).

        // Rand 1: Rand 100 based on story progress.
        // Story progress changes the spread of raid stars available.
        // world\data\encount\setting\raid_difficulty_lottery\raid_difficulty_lottery_array.bin
        var rand = new Xoroshiro128Plus(seed);
        if (stars < 6)
        {
            var starRand = (uint)rand.NextInt(100);
            if (!IsValidStarRand(starRand, stars))
                return false;
        }

        // Rand 2: Rand based on the total of all possible raid opponents.
        // Once the rand is calculated, we iterate down the list until the total is less than the rand.
        // If the rateRand is within the randRateMin range for either game, return true.

        // Some raid encounters are version specific, which impacts the total rate value during iteration.
        // Depending on the Host's game, the rate sum will be different.
        // The inputs to this function have pre-computed the total rate when the encounter is checked.

        // If Scarlet...
        var maxScarlet = EncounterTera9.GetRateTotalSL(stars, map);
        {
            var xoro = rand; // copy
            var rateRand = (int)xoro.NextInt((uint)maxScarlet);
            if (randRateMinScarlet >= 0 && (uint)(rateRand - randRateMinScarlet) < raidRate)
                return true; // Seed can yield this encounter for Scarlet.
        }
        // If Violet..
        var maxViolet = EncounterTera9.GetRateTotalVL(stars, map);
        {
            var xoro = rand; // copy
            var rateRand = (int)xoro.NextInt((uint)maxViolet);
            if (randRateMinViolet >= 0 && (uint)(rateRand - randRateMinViolet) < raidRate)
                return true; // Seed can yield this encounter for Violet.
        }

        // Rate range for the seed does not yield this encounter.
        return false;
    }

    private static bool IsValidStarRand(in uint rand, in byte stars) => stars switch
    {
        // Thanks to a silly <= check, the range check edge is inclusive (unlike the slot determination).
        // Each rate has its low bound used by the prior's rate check.
        // This results in:
        // - the 1st rate check has a total range of one more than the intended range.
        // - the last rate check has a total range of one less than the intended range.
        1 => rand <= 80,                      // [  0, 80], [ 0, 30], or [ 0, 20]
        2 => rand is (<=70 and >20) or (>80), // (80, 100], (30, 70], or (20, 40]
        3 => true, // all ranges overlap!     // (70, 100], (40, 70], or [ 0, 40]
        4 => rand > 30,                       // (70, 100], (40, 75], or (30, 70]
        5 => rand > 70,                       // (70, 100]
        _ => true,                            // forced star count
    };

    /// <summary>
    /// Obtains the original seed for the encounter.
    /// </summary>
    /// <param name="pk">Entity to check for</param>
    /// <returns>Seed</returns>
    /// <remarks>Refer to <see cref="Overworld8RNG.GetOriginalSeed"/> for similar implementation.</remarks>
    public static uint GetOriginalSeed(PKM pk)
    {
        // Same RNG goof as SW/SH, the seed->PKM algorithm generates the EC first. Difference: has FakeTID roll between.
        // The raid seed is always 32 bits, so we know 96 bits of the seed (zeroes and the 64bit const).
        // Since the 32-bit EC is generated first by adding the two 64-bit RNG states, we can calc the other 32bits.
        var enc = pk.EncryptionConstant;
        const uint TwoSeedsEC = 0xF8572EBE;
        if (enc != TwoSeedsEC)
            return enc - unchecked((uint)Xoroshiro128Plus.XOROSHIRO_CONST);

        // Two seeds can result in an EC of 0xF8572EBE, so we need to check the PID as well.
        // Seed => EC => FakeTrainerID (!!!) => PID
        const uint Seed_1 = 0xD5B9C463; // First rand()
        const uint Seed_2 = 0xDD6295A4; // First rand() is uint.MaxValue, rolls again.
        const uint PID_1  = 0xC9E69326; // 0xD68E8499 for PID_2
        return pk.PID == PID_1 ? Seed_1 : Seed_2; // don't care if the PID != second's case. Other validation will check.
    }
}
