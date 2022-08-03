using System;

namespace PKHeX.Core;

/// <summary>
/// Encounter Slot found in <see cref="GameVersion.ORAS"/>.
/// </summary>
/// <inheritdoc cref="EncounterSlot"/>
public sealed record EncounterSlot6AO : EncounterSlot
{
    public override int Generation => 6;
    public override EntityContext Context => EntityContext.Gen6;
    public bool CanDexNav => Area.Type != SlotType.Rock_Smash;
    public bool IsHorde => Area.Type == SlotType.Horde;

    public bool Pressure { get; init; }
    public bool DexNav { get; init; }
    public bool WhiteFlute { get; init; }
    public bool BlackFlute { get; init; }

    public EncounterSlot6AO(EncounterArea6AO area, ushort species, byte form, byte min, byte max) : base(area, species, form, min, max)
    {
    }

    protected override void SetFormatSpecificData(PKM pk)
    {
        var pk6 = (PK6)pk;
        if (CanDexNav)
        {
            var eggMoves = GetDexNavMoves();
            if (eggMoves.Length > 0)
                pk6.RelearnMove1 = eggMoves[Util.Rand.Next(eggMoves.Length)];
        }
        pk6.SetRandomMemory6();
        pk6.SetRandomEC();
    }

    public override string GetConditionString(out bool valid)
    {
        valid = true;
        if (WhiteFlute) // Decreased Level Encounters
            return Pressure ? LegalityCheckStrings.LEncConditionWhiteLead : LegalityCheckStrings.LEncConditionWhite;
        if (BlackFlute) // Increased Level Encounters
            return Pressure ? LegalityCheckStrings.LEncConditionBlackLead : LegalityCheckStrings.LEncConditionBlack;
        if (DexNav)
            return LegalityCheckStrings.LEncConditionDexNav;

        return Pressure ? LegalityCheckStrings.LEncConditionLead : LegalityCheckStrings.LEncCondition;
    }

    protected override HiddenAbilityPermission IsHiddenAbilitySlot() => CanDexNav || IsHorde ? HiddenAbilityPermission.Possible : HiddenAbilityPermission.Never;

    private ReadOnlySpan<int> GetDexNavMoves()
    {
        var et = EvolutionTree.Evolves6;
        var baby = et.GetBaseSpeciesForm((ushort)Species, (byte)Form);
        return MoveEgg.GetEggMoves(6, baby.Species, baby.Form, Version);
    }

    public bool CanBeDexNavMove(int move)
    {
        var baseEgg = GetDexNavMoves();
        return baseEgg.Contains(move);
    }
}
