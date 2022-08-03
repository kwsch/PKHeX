using System;
using static PKHeX.Core.EvolutionType;

namespace PKHeX.Core;

/// <summary>
/// Describes how an evolution can be triggered.
/// </summary>
public enum EvolutionType : byte
{
    None = 0,
    LevelUpFriendship = 1,
    LevelUpFriendshipMorning = 2,
    LevelUpFriendshipNight = 3,
    LevelUp = 4,
    Trade = 5,
    TradeHeldItem = 6,
    TradeShelmetKarrablast = 7,
    UseItem = 8,
    LevelUpATK = 9,
    LevelUpAeqD = 10,
    LevelUpDEF = 11,
    LevelUpECl5 = 12,
    LevelUpECgeq5 = 13,
    LevelUpNinjask = 14,
    LevelUpShedinja = 15,
    LevelUpBeauty = 16,
    UseItemMale = 17,
    UseItemFemale = 18,
    LevelUpHeldItemDay = 19,
    LevelUpHeldItemNight = 20,
    LevelUpKnowMove = 21,
    LevelUpWithTeammate = 22,
    LevelUpMale = 23,
    LevelUpFemale = 24,
    LevelUpElectric = 25,
    LevelUpForest = 26,
    LevelUpCold = 27,
    LevelUpInverted = 28,
    LevelUpAffection50MoveType = 29,
    LevelUpMoveType = 30,
    LevelUpWeather = 31,
    LevelUpMorning = 32,
    LevelUpNight = 33,
    LevelUpFormFemale1 = 34,
    UNUSED = 35,
    LevelUpVersion = 36,
    LevelUpVersionDay = 37,
    LevelUpVersionNight = 38,
    LevelUpSummit = 39,
    LevelUpDusk = 40,
    LevelUpWormhole = 41,
    UseItemWormhole = 42,
    CriticalHitsInBattle = 43, // Sirfetch'd
    HitPointsLostInBattle = 44, // Runerigus
    Spin = 45, // Alcremie
    LevelUpNatureAmped = 46, // Toxtricity
    LevelUpNatureLowKey = 47, // Toxtricity
    TowerOfDarkness = 48, // Urshifu
    TowerOfWaters = 49, // Urshifu
    UseItemFullMoon = 50, // Ursaluna
    UseAgileStyleMoves = 51, // Wyrdeer
    UseStrongStyleMoves = 52, // Overqwil
    RecoilDamageMale = 53, // Basculegion-0
    RecoilDamageFemale = 54, // Basculegion-1
}

public static class EvolutionTypeExtensions
{
    public static bool IsTrade(this EvolutionType t) => t is Trade or TradeHeldItem or TradeShelmetKarrablast;

    public static bool IsLevelUpRequired(this EvolutionType type) => type switch
    {
        None => false,
        LevelUpFriendship => true,
        LevelUpFriendshipMorning => true,
        LevelUpFriendshipNight => true,
        LevelUp => true,
        Trade => false,
        TradeHeldItem => false,
        TradeShelmetKarrablast => false,
        UseItem => false,
        LevelUpATK => true,
        LevelUpAeqD => true,
        LevelUpDEF => true,
        LevelUpECl5 => true,
        LevelUpECgeq5 => true,
        LevelUpNinjask => true,
        LevelUpShedinja => true,
        LevelUpBeauty => true,
        UseItemMale => false,
        UseItemFemale => false,
        LevelUpHeldItemDay => true,
        LevelUpHeldItemNight => true,
        LevelUpKnowMove => true,
        LevelUpWithTeammate => true,
        LevelUpMale => true,
        LevelUpFemale => true,
        LevelUpElectric => true,
        LevelUpForest => true,
        LevelUpCold => true,
        LevelUpInverted => true,
        LevelUpAffection50MoveType => true,
        LevelUpMoveType => true,
        LevelUpWeather => true,
        LevelUpMorning => true,
        LevelUpNight => true,
        LevelUpFormFemale1 => true,
        UNUSED => false,
        LevelUpVersion => true,
        LevelUpVersionDay => true,
        LevelUpVersionNight => true,
        LevelUpSummit => true,
        LevelUpDusk => true,
        LevelUpWormhole => true,
        UseItemWormhole => false,
        CriticalHitsInBattle => false,
        HitPointsLostInBattle => false,
        Spin => false,
        LevelUpNatureAmped => true,
        LevelUpNatureLowKey => true,
        TowerOfDarkness => false,
        TowerOfWaters => false,
        UseItemFullMoon => false,
        UseAgileStyleMoves => false,
        UseStrongStyleMoves => false,
        RecoilDamageMale => false,
        RecoilDamageFemale => false,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
    };
}
