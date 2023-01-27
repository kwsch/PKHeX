using System;
using System.Buffers.Binary;
using static PKHeX.Core.AbilityPermission;

namespace PKHeX.Core;

/// <summary>
/// Generation 9 Tera Raid Encounter
/// </summary>
public sealed record EncounterTera9 : EncounterStatic, ITeraRaid9
{
    public override int Generation => 9;
    public override int Location => Locations.TeraCavern9;
    public override EntityContext Context => EntityContext.Gen9;
    public bool IsDistribution => Index != 0;
    public GemType TeraType { get; private init; }
    public byte Index { get; private init; }
    public byte Stars { get; private init; }
    public byte RandRate { get; private init; } // weight chance of this encounter
    public short RandRateMinScarlet { get; private init; } // weight chance total of all lower index encounters, for Scarlet
    public short RandRateMinViolet { get; private init; } // weight chance total of all lower index encounters, for Violet
    public bool IsAvailableHostScarlet => RandRateMinScarlet != -1;
    public bool IsAvailableHostViolet => RandRateMinViolet != -1;

    public bool CanBeEncountered(uint seed) => Tera9RNG.IsMatchStarChoice(seed, Stars, RandRate, RandRateMinScarlet, RandRateMinViolet);

    /// <summary>
    /// Fetches the rate sum for the base ROM raid, depending on star count.
    /// </summary>
    /// <param name="star">Raid Difficulty</param>
    /// <returns>Total rate value the game uses to call rand(x) with.</returns>
    public static short GetRateTotalBaseSL(int star) => star switch
    {
        1 => 5800,
        2 => 5300,
        3 => 7400,
        4 => 8800, // Scarlet has one more encounter.
        5 => 9100,
        6 => 6500,
        _ => 0,
    };

    public static short GetRateTotalBaseVL(int star) => star switch
    {
        1 => 5800,
        2 => 5300,
        3 => 7400,
        4 => 8700, // Violet has one less encounter.
        5 => 9100,
        6 => 6500,
        _ => 0,
    };

    public static EncounterTera9[] GetArray(ReadOnlySpan<byte> data)
    {
        const int size = 0x18;
        var count = data.Length / size;
        var result = new EncounterTera9[count];
        for (int i = 0; i < result.Length; i++)
            result[i] = ReadEncounter(data.Slice(i * size, size));
        return result;
    }

    private EncounterTera9() : base(GameVersion.SV) { }

    private static EncounterTera9 ReadEncounter(ReadOnlySpan<byte> data) => new()
    {
        Species = BinaryPrimitives.ReadUInt16LittleEndian(data),
        Form = data[0x02],
        Gender = (sbyte)(data[0x03] - 1),
        Ability = GetAbility(data[0x04]),
        FlawlessIVCount = data[5],
        Shiny = data[0x06] switch { 0 => Shiny.Random, 1 => Shiny.Never, 2 => Shiny.Always, _ => throw new ArgumentOutOfRangeException(nameof(data)) },
        Level = data[0x07],
        Moves = new Moveset(
            BinaryPrimitives.ReadUInt16LittleEndian(data[0x08..]),
            BinaryPrimitives.ReadUInt16LittleEndian(data[0x0A..]),
            BinaryPrimitives.ReadUInt16LittleEndian(data[0x0C..]),
            BinaryPrimitives.ReadUInt16LittleEndian(data[0x0E..])),
        TeraType = (GemType)data[0x10],
        Index = data[0x11],
        Stars = data[0x12],
        RandRate = data[0x13],
        RandRateMinScarlet = BinaryPrimitives.ReadInt16LittleEndian(data[0x14..]),
        RandRateMinViolet = BinaryPrimitives.ReadInt16LittleEndian(data[0x16..]),
    };

    private static AbilityPermission GetAbility(byte b) => b switch
    {
        0 => Any12,
        1 => Any12H,
        2 => OnlyFirst,
        3 => OnlySecond,
        4 => OnlyHidden,
        _ => throw new ArgumentOutOfRangeException(nameof(b), b, null),
    };

    protected override EncounterMatchRating IsMatchDeferred(PKM pk)
    {
        if (Ability != Any12H)
        {
            // HA-Only is a strict match. Ability Capsule and Patch can potentially change these.
            var num = pk.AbilityNumber;
            if (num == 4)
            {
                if (Ability is not OnlyHidden && !AbilityVerifier.CanAbilityPatch(9, PersonalTable.SV.GetFormEntry(Species, Form), pk.Species))
                    return EncounterMatchRating.DeferredErrors;
            }
            else if (Ability.IsSingleValue(out int index) && 1 << index != num) // Fixed regular ability
            {
                if (Ability is OnlyFirst or OnlySecond && !AbilityVerifier.CanAbilityCapsule(9, PersonalTable.SV.GetFormEntry(Species, Form)))
                    return EncounterMatchRating.DeferredErrors;
            }
        }

        return base.IsMatchDeferred(pk);
    }

    protected override bool IsMatchPartial(PKM pk)
    {
        switch (Shiny)
        {
            case Shiny.Never when pk.IsShiny:
            case Shiny.Always when !pk.IsShiny:
                return true;
        }

        var seed = Tera9RNG.GetOriginalSeed(pk);
        if (pk is ITeraType t && !Tera9RNG.IsMatchTeraType(seed, TeraType, Species, Form, (byte)t.TeraTypeOriginal))
            return true;
        if (!Tera9RNG.IsMatchStarChoice(seed, Stars, RandRate, RandRateMinScarlet, RandRateMinViolet))
            return true;

        var pi = PersonalTable.SV.GetFormEntry(Species, Form);
        var param = new GenerateParam9(Species, pi.Gender, FlawlessIVCount, 1, 0, 0, 0, Ability, Shiny);
        if (!Encounter9RNG.IsMatch(pk, param, seed))
            return true;
        return base.IsMatchPartial(pk);
    }

    protected override void ApplyDetails(ITrainerInfo tr, EncounterCriteria criteria, PKM pk)
    {
        base.ApplyDetails(tr, criteria, pk);
        var pk9 = (PK9)pk;
        pk9.Obedience_Level = (byte)pk9.Met_Level;
    }

    protected override void SetPINGA(PKM pk, EncounterCriteria criteria)
    {
        var pk9 = (PK9)pk;

        const byte rollCount = 1;
        const byte undefinedSize = 0;
        var pi = PersonalTable.SV.GetFormEntry(Species, Form);
        var param = new GenerateParam9(Species, pi.Gender, FlawlessIVCount, rollCount,
            undefinedSize, undefinedSize, undefinedSize,
            Ability, Shiny);

        var init = Util.Rand.Rand64();
        var success = this.TryApply32(pk9, init, param, criteria);
        if (!success)
            this.TryApply32(pk9, init, param, EncounterCriteria.Unrestricted);
    }
}
