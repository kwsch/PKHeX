using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed record EncounterDist9 : EncounterStatic, ITeraRaid9
{
    public override int Generation => 9;
    public override int Location => Locations.TeraCavern9;
    public override EntityContext Context => EntityContext.Gen9;
    public bool IsDistribution => Index != 0;
    public GemType TeraType { get; private init; }
    public byte Index { get; private init; }
    public byte Stars { get; private init; }
    public byte RandRate { get; private init; } // weight chance of this encounter

    public ushort RandRate0MinScarlet { get; private init; }
    public ushort RandRate0MinViolet { get; private init; }
    public ushort RandRate0TotalScarlet { get; private init; }
    public ushort RandRate0TotalViolet { get; private init; }

    public ushort RandRate1MinScarlet { get; private init; }
    public ushort RandRate1MinViolet { get; private init; }
    public ushort RandRate1TotalScarlet { get; private init; }
    public ushort RandRate1TotalViolet { get; private init; }

    public ushort RandRate2MinScarlet { get; private init; }
    public ushort RandRate2MinViolet { get; private init; }
    public ushort RandRate2TotalScarlet { get; private init; }
    public ushort RandRate2TotalViolet { get; private init; }

    public ushort RandRate3MinScarlet { get; private init; }
    public ushort RandRate3MinViolet { get; private init; }
    public ushort RandRate3TotalScarlet { get; private init; }
    public ushort RandRate3TotalViolet { get; private init; }

    public ushort GetRandRateTotalScarlet(int stage) => stage switch
    {
        0 => RandRate0TotalScarlet,
        1 => RandRate1TotalScarlet,
        2 => RandRate2TotalScarlet,
        3 => RandRate3TotalScarlet,
        _ => throw new ArgumentOutOfRangeException(nameof(stage)),
    };

    public ushort GetRandRateTotalViolet(int stage) => stage switch
    {
        0 => RandRate0TotalViolet,
        1 => RandRate1TotalViolet,
        2 => RandRate2TotalViolet,
        3 => RandRate3TotalViolet,
        _ => throw new ArgumentOutOfRangeException(nameof(stage)),
    };

    public ushort GetRandRateMinScarlet(int stage) => stage switch
    {
        0 => RandRate0MinScarlet,
        1 => RandRate1MinScarlet,
        2 => RandRate2MinScarlet,
        3 => RandRate3MinScarlet,
        _ => throw new ArgumentOutOfRangeException(nameof(stage)),
    };

    public ushort GetRandRateMinViolet(int stage) => stage switch
    {
        0 => RandRate0MinViolet,
        1 => RandRate1MinViolet,
        2 => RandRate2MinViolet,
        3 => RandRate3MinViolet,
        _ => throw new ArgumentOutOfRangeException(nameof(stage)),
    };

    private const int StageCount = 4;
    private const int StageNone = -1;

    public bool CanBeEncountered(uint seed) => GetProgressMaximum(seed) != StageNone;

    public int ProgressStageMin
    {
        get
        {
            for (int stage = 0; stage < StageCount; stage++)
            {
                if (GetRandRateTotalScarlet(stage) != 0 || GetRandRateTotalViolet(stage) != 0)
                    return stage;
            }
            return StageNone;
        }
    }

    public int ProgressStageMax
    {
        get
        {
            for (int stage = StageCount - 1; stage >= 0; stage--)
            {
                if (GetRandRateTotalScarlet(stage) != 0 || GetRandRateTotalViolet(stage) != 0)
                    return stage;
            }
            return StageNone;
        }
    }

    public int GetProgressMaximum(uint seed)
    {
        // We loop from the highest progress, since that is where the majority of samples will be from.
        for (int i = StageCount - 1; i >= 0; i--)
        {
            if (GetIsPossibleSlot(seed, i))
                return i;
        }
        return StageNone;
    }

    private bool GetIsPossibleSlot(uint seed, int stage)
    {
        var totalScarlet = GetRandRateTotalScarlet(stage);
        if (totalScarlet != 0)
        {
            var rand = new Xoroshiro128Plus(seed);
            _ = rand.NextInt(100);
            var val = rand.NextInt(totalScarlet);
            var min = GetRandRateMinScarlet(stage);
            if ((uint)((int)val - min) < RandRate)
                return true;
        }

        var totalViolet = GetRandRateTotalViolet(stage);
        if (totalViolet != 0)
        {
            var rand = new Xoroshiro128Plus(seed);
            _ = rand.NextInt(100);
            var val = rand.NextInt(totalViolet);
            var min = GetRandRateMinViolet(stage);
            if ((uint)((int)val - min) < RandRate)
                return true;
        }
        return false;
    }

    public static EncounterDist9[] GetArray(ReadOnlySpan<byte> data)
    {
        var count = data.Length / SerializedSize;
        var result = new EncounterDist9[count];
        for (int i = 0; i < result.Length; i++)
            result[i] = ReadEncounter(data.Slice(i * SerializedSize, SerializedSize));
        return result;
    }

    private EncounterDist9() : base(GameVersion.SV) { }

    private const int SerializedSize = WeightStart + (sizeof(ushort) * 2 * 2 * 4);
    private const int WeightStart = 0x14;
    private static EncounterDist9 ReadEncounter(ReadOnlySpan<byte> data) => new()
    {
        Species = ReadUInt16LittleEndian(data),
        Form = data[0x02],
        Gender = (sbyte)(data[0x03] - 1),
        Ability = GetAbility(data[0x04]),
        FlawlessIVCount = data[5],
        Shiny = data[0x06] switch { 0 => Shiny.Random, 1 => Shiny.Never, 2 => Shiny.Always, _ => throw new ArgumentOutOfRangeException(nameof(data)) },
        Level = data[0x07],
        Moves = new Moveset(
            ReadUInt16LittleEndian(data[0x08..]),
            ReadUInt16LittleEndian(data[0x0A..]),
            ReadUInt16LittleEndian(data[0x0C..]),
            ReadUInt16LittleEndian(data[0x0E..])),
        TeraType = (GemType)data[0x10],
        Index = data[0x11],
        Stars = data[0x12],
        RandRate = data[0x13],

        RandRate0MinScarlet   = ReadUInt16LittleEndian(data[WeightStart..]),
        RandRate0MinViolet    = ReadUInt16LittleEndian(data[(WeightStart + sizeof(ushort))..]),
        RandRate0TotalScarlet = ReadUInt16LittleEndian(data[(WeightStart + (sizeof(ushort) * 2))..]),
        RandRate0TotalViolet  = ReadUInt16LittleEndian(data[(WeightStart + (sizeof(ushort) * 3))..]),

        RandRate1MinScarlet   = ReadUInt16LittleEndian(data[(WeightStart + (sizeof(ushort) * 4))..]),
        RandRate1MinViolet    = ReadUInt16LittleEndian(data[(WeightStart + (sizeof(ushort) * 5))..]),
        RandRate1TotalScarlet = ReadUInt16LittleEndian(data[(WeightStart + (sizeof(ushort) * 6))..]),
        RandRate1TotalViolet  = ReadUInt16LittleEndian(data[(WeightStart + (sizeof(ushort) * 7))..]),

        RandRate2MinScarlet   = ReadUInt16LittleEndian(data[(WeightStart + (sizeof(ushort) * 8))..]),
        RandRate2MinViolet    = ReadUInt16LittleEndian(data[(WeightStart + (sizeof(ushort) * 9))..]),
        RandRate2TotalScarlet = ReadUInt16LittleEndian(data[(WeightStart + (sizeof(ushort) * 10))..]),
        RandRate2TotalViolet  = ReadUInt16LittleEndian(data[(WeightStart + (sizeof(ushort) * 11))..]),

        RandRate3MinScarlet   = ReadUInt16LittleEndian(data[(WeightStart + (sizeof(ushort) * 12))..]),
        RandRate3MinViolet    = ReadUInt16LittleEndian(data[(WeightStart + (sizeof(ushort) * 13))..]),
        RandRate3TotalScarlet = ReadUInt16LittleEndian(data[(WeightStart + (sizeof(ushort) * 14))..]),
        RandRate3TotalViolet  = ReadUInt16LittleEndian(data[(WeightStart + (sizeof(ushort) * 15))..]),
    };

    private static AbilityPermission GetAbility(byte b) => b switch
    {
        0 => AbilityPermission.Any12,
        1 => AbilityPermission.Any12H,
        2 => AbilityPermission.OnlyFirst,
        3 => AbilityPermission.OnlySecond,
        4 => AbilityPermission.OnlyHidden,
        _ => throw new ArgumentOutOfRangeException(nameof(b), b, null),
    };

    protected override EncounterMatchRating IsMatchDeferred(PKM pk)
    {
        if (Ability != AbilityPermission.Any12H)
        {
            // HA-Only is a strict match. Ability Capsule and Patch can potentially change these.
            var num = pk.AbilityNumber;
            if (num == 4)
            {
                if (Ability is not AbilityPermission.OnlyHidden && !AbilityVerifier.CanAbilityPatch(9, PersonalTable.SV.GetFormEntry(Species, Form), pk.Species))
                    return EncounterMatchRating.DeferredErrors;
            }
            else if (Ability.IsSingleValue(out int index) && 1 << index != num) // Fixed regular ability
            {
                if (Ability is AbilityPermission.OnlyFirst or AbilityPermission.OnlySecond && !AbilityVerifier.CanAbilityCapsule(9, PersonalTable.SV.GetFormEntry(Species, Form)))
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
        if (!CanBeEncountered(seed))
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
