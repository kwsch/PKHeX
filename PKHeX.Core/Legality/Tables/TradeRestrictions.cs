using System;

namespace PKHeX.Core;

public static class TradeRestrictions
{
    /// <summary>
    /// Indicates if the entity should be prevented from being traded away.
    /// </summary>
    /// <param name="species">Entity species</param>
    /// <param name="form">Entity form</param>
    /// <param name="formArg">Entity form argument</param>
    /// <param name="format">Current generation format</param>
    /// <returns>True if trading should be disallowed.</returns>
    public static bool IsUntradable(ushort species, byte form, uint formArg, byte format) => species switch
    {
        (ushort)Species.Pichu => form == 1, // Spiky-eared Pichu
        (ushort)Species.Koraidon or (int)Species.Miraidon => formArg == 1, // Ride-able Box Legend
        (ushort)Species.Pikachu => format == 7 && form == 8, // Let's Go Pikachu Starter
        (ushort)Species.Eevee => format == 7 && form == 1, // Let's Go Eevee Starter
        _ => FormInfo.IsFusedForm(species, form, format) || FormInfo.IsBattleOnlyForm(species, form, format),
    };

    public static bool IsUntradableHeld(EntityContext context, int item) => context switch
    {
        EntityContext.Gen7 => ItemStorage7USUM.ZCrystalHeld.Contains((ushort)item),
        EntityContext.Gen9a => ItemStorage9ZA.IsUniqueHeldItem((ushort)item),
        _ => false,
    };

    public static bool IsUntradableEncounter(IEncounterTemplate enc) => enc switch
    {
        EncounterStatic4 { Species: (ushort)Species.Pichu, Form: 1 } => true, // Spiky-eared Pichu
        EncounterStatic7b { Location: 28 } => true, // LGP/E Starter
        EncounterStatic9 { StarterBoxLegend: true } => true, // S/V Ride legend
        _ => false,
    };
}
