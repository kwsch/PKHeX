using System;
using System.Collections.Generic;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

public sealed class Record3(SAV3 SAV)
{
    public uint GetRecord(int record) => ReadUInt32LittleEndian(SAV.Large[GetRecordOffset(record)..]) ^ SAV.SecurityKey;
    public void SetRecord(int record, uint value) => WriteUInt32LittleEndian(SAV.Large[GetRecordOffset(record)..], value ^ SAV.SecurityKey);

    private int GetRecordOffset(int record)
    {
        var baseOffset = GetOffset(SAV.Version);
        var offset = baseOffset + (4 * record);
        return offset;
    }

    public static int GetOffset(GameVersion version) => version switch
    {
        GameVersion.RS or GameVersion.R or GameVersion.S => 0x1540,
        GameVersion.E => 0x159C,
        GameVersion.FRLG or GameVersion.FR or GameVersion.LG => 0x1200,
        _ => throw new ArgumentOutOfRangeException(nameof(version), version, null),
    };

    private static Type GetEnumType(GameVersion version) => version switch
    {
        GameVersion.RS or GameVersion.R or GameVersion.S => typeof(RecID3RuSa),
        GameVersion.FRLG or GameVersion.FR or GameVersion.LG => typeof(RecID3FRLG),
        GameVersion.E => typeof(RecID3Emerald),
        _ => throw new ArgumentOutOfRangeException(nameof(version), version, null),
    };

    public static int[] GetEnumValues(GameVersion version) => (int[])Enum.GetValues(GetEnumType(version));
    public static string[] GetEnumNames(GameVersion version) => Enum.GetNames(GetEnumType(version));

    public static IList<ComboItem> GetItems(SAV3 sav)
    {
        var version = sav.Version;
        var names = GetEnumNames(version);
        var values = GetEnumValues(version);

        var result = new ComboItem[values.Length];
        for (int i = 0; i < result.Length; i++)
        {
            var replaced = names[i].Replace('_', ' ');
            var titled = Util.ToTitleCase(replaced);
            result[i] = new ComboItem(titled, values[i]);
        }
        return result;
    }
}

/// <summary>
/// Record IDs for <see cref="GameVersion.R"/> and <see cref="GameVersion.S"/>
/// </summary>
/// <remarks>
/// https://github.com/pret/pokeruby/blob/f839afb24aa2c7b70e9c28a5c069aacc46993099/include/constants/game_stat.h
/// </remarks>
public enum RecID3RuSa
{
    SAVED_GAME = 0,
    FIRST_HOF_PLAY_TIME = 1,
    STARTED_TRENDS = 2,
    PLANTED_BERRIES = 3,
    TRADED_BIKES = 4,
    STEPS = 5,
    GOT_INTERVIEWED = 6,
    TOTAL_BATTLES = 7,
    WILD_BATTLES = 8,
    TRAINER_BATTLES = 9,
    ENTERED_HOF = 10,
    POKEMON_CAPTURES = 11,
    FISHING_CAPTURES = 12,
    HATCHED_EGGS = 13,
    EVOLVED_POKEMON = 14,
    USED_POKECENTER = 15,
    RESTED_AT_HOME = 16,
    ENTERED_SAFARI_ZONE = 17,
    USED_CUT = 18,
    USED_ROCK_SMASH = 19,
    MOVED_SECRET_BASE = 20,
    POKEMON_TRADES = 21,
    UNKNOWN_22 = 22,
    LINK_BATTLE_WINS = 23,
    LINK_BATTLE_LOSSES = 24,
    LINK_BATTLE_DRAWS = 25,
    USED_SPLASH = 26,
    USED_STRUGGLE = 27,
    SLOT_JACKPOTS = 28,
    CONSECUTIVE_ROULETTE_WINS = 29,
    ENTERED_BATTLE_TOWER = 30,
    UNKNOWN_31 = 31,
    BATTLE_TOWER_BEST_STREAK = 32,
    POKEBLOCKS = 33,
    POKEBLOCKS_WITH_FRIENDS = 34,
    WON_LINK_CONTEST = 35,
    ENTERED_CONTEST = 36,
    WON_CONTEST = 37,
    SHOPPED = 38,
    USED_ITEMFINDER = 39,
    GOT_RAINED_ON = 40,
    CHECKED_POKEDEX = 41,
    RECEIVED_RIBBONS = 42,
    JUMPED_DOWN_LEDGES = 43,
    WATCHED_TV = 44,
    CHECKED_CLOCK = 45,
    WON_POKEMON_LOTTERY = 46,
    USED_DAYCARE = 47,
    RODE_CABLE_CAR = 48,
    ENTERED_HOT_SPRINGS = 49,
    // NUM_GAME_STATS = 50
}

/// <summary>
/// Record IDs for <see cref="GameVersion.E"/>
/// </summary>
/// <remarks>
/// https://github.com/pret/pokeemerald/blob/3a40f5203baafb29f94dda8abdce6489d81635ae/include/constants/game_stat.h
/// </remarks>
public enum RecID3Emerald
{
    SAVED_GAME = 0,
    FIRST_HOF_PLAY_TIME = 1,
    STARTED_TRENDS = 2,
    PLANTED_BERRIES = 3,
    TRADED_BIKES = 4,
    STEPS = 5,
    GOT_INTERVIEWED = 6,
    TOTAL_BATTLES = 7,
    WILD_BATTLES = 8,
    TRAINER_BATTLES = 9,
    ENTERED_HOF = 10,
    POKEMON_CAPTURES = 11,
    FISHING_CAPTURES = 12,
    HATCHED_EGGS = 13,
    EVOLVED_POKEMON = 14,
    USED_POKECENTER = 15,
    RESTED_AT_HOME = 16,
    ENTERED_SAFARI_ZONE = 17,
    USED_CUT = 18,
    USED_ROCK_SMASH = 19,
    MOVED_SECRET_BASE = 20,
    POKEMON_TRADES = 21,
    UNKNOWN_22 = 22,
    LINK_BATTLE_WINS = 23,
    LINK_BATTLE_LOSSES = 24,
    LINK_BATTLE_DRAWS = 25,
    USED_SPLASH = 26,
    USED_STRUGGLE = 27,
    SLOT_JACKPOTS = 28,
    CONSECUTIVE_ROULETTE_WINS = 29,
    ENTERED_BATTLE_TOWER = 30,
    UNKNOWN_31 = 31,
    BATTLE_TOWER_BEST_STREAK = 32,
    POKEBLOCKS = 33,
    POKEBLOCKS_WITH_FRIENDS = 34,
    WON_LINK_CONTEST = 35,
    ENTERED_CONTEST = 36,
    WON_CONTEST = 37,
    SHOPPED = 38,
    USED_ITEMFINDER = 39,
    GOT_RAINED_ON = 40,
    CHECKED_POKEDEX = 41,
    RECEIVED_RIBBONS = 42,
    JUMPED_DOWN_LEDGES = 43,
    WATCHED_TV = 44,
    CHECKED_CLOCK = 45,
    WON_POKEMON_LOTTERY = 46,
    USED_DAYCARE = 47,
    RODE_CABLE_CAR = 48,
    ENTERED_HOT_SPRINGS = 49,
    UNION_WITH_FRIENDS = 50,
    BERRY_CRUSH_WITH_FRIENDS = 51,

    // NUM_USED_GAME_STATS = 52,
    // NUM_GAME_STATS = 64
}

/// <summary>
/// Record IDs for <see cref="GameVersion.FR"/> and <see cref="GameVersion.LG"/>
/// </summary>
/// <remarks>
/// https://github.com/pret/pokefirered/blob/8367b0015fbf99070cc5a5244d8213420419d2c8/include/constants/game_stat.h
/// </remarks>
public enum RecID3FRLG
{
    SAVED_GAME = 0,
    FIRST_HOF_PLAY_TIME = 1,
    STARTED_TRENDS = 2,
    PLANTED_BERRIES = 3,
    TRADED_BIKES = 4,
    STEPS = 5,
    GOT_INTERVIEWED = 6,
    TOTAL_BATTLES = 7,
    WILD_BATTLES = 8,
    TRAINER_BATTLES = 9,
    ENTERED_HOF = 10,
    POKEMON_CAPTURES = 11,
    FISHING_CAPTURES = 12,
    HATCHED_EGGS = 13,
    EVOLVED_POKEMON = 14,
    USED_POKECENTER = 15,
    RESTED_AT_HOME = 16,
    ENTERED_SAFARI_ZONE = 17,
    USED_CUT = 18,
    USED_ROCK_SMASH = 19,
    MOVED_SECRET_BASE = 20,
    POKEMON_TRADES = 21,
    UNKNOWN_22 = 22,
    LINK_BATTLE_WINS = 23,
    LINK_BATTLE_LOSSES = 24,
    LINK_BATTLE_DRAWS = 25,
    USED_SPLASH = 26,
    USED_STRUGGLE = 27,
    SLOT_JACKPOTS = 28,
    CONSECUTIVE_ROULETTE_WINS = 29,
    ENTERED_BATTLE_TOWER = 30,
    UNKNOWN_31 = 31,
    BATTLE_TOWER_BEST_STREAK = 32,
    POKEBLOCKS = 33,
    POKEBLOCKS_WITH_FRIENDS = 34,
    WON_LINK_CONTEST = 35,
    ENTERED_CONTEST = 36,
    WON_CONTEST = 37,
    SHOPPED = 38,
    USED_ITEMFINDER = 39,
    GOT_RAINED_ON = 40,
    CHECKED_POKEDEX = 41,
    RECEIVED_RIBBONS = 42,
    JUMPED_DOWN_LEDGES = 43,
    WATCHED_TV = 44,
    CHECKED_CLOCK = 45,
    WON_POKEMON_LOTTERY = 46,
    USED_DAYCARE = 47,
    RODE_CABLE_CAR = 48,
    ENTERED_HOT_SPRINGS = 49,
    UNION_WITH_FRIENDS = 50,
    BERRY_CRUSH_WITH_FRIENDS = 51,

    // NUM_GAME_STATS = 64,
}
