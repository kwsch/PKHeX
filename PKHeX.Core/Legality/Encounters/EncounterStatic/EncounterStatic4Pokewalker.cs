using System;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Generation 4 Pokéwalker  Encounter
/// </summary>
/// <inheritdoc cref="EncounterStatic"/>
public sealed record EncounterStatic4Pokewalker : EncounterStatic
{
    public override int Generation => 4;
    public override EntityContext Context => EntityContext.Gen4;
    public PokewalkerCourse4 Course { get; }

    public EncounterStatic4Pokewalker(ReadOnlySpan<byte> data, PokewalkerCourse4 course) : base(GameVersion.HGSS)
    {
        Species = ReadUInt16LittleEndian(data);
        Level = data[2];
        Gender = (sbyte)data[3];
        Course = course;
        var move1 = ReadUInt16LittleEndian(data[0x4..]);
        var move2 = ReadUInt16LittleEndian(data[0x6..]);
        var move3 = ReadUInt16LittleEndian(data[0x8..]);
        var move4 = ReadUInt16LittleEndian(data[0xA..]);
        Moves = new(move1, move2, move3, move4);

        // All obtained entities are in Poke Ball and have a met location of "PokeWalker"
        Gift = true;
        Location = Locations.PokeWalker4;
    }

    protected override bool IsMatchLocation(PKM pk)
    {
        if (pk.Format == 4)
            return Location == pk.Met_Location;
        return true; // transfer location verified later
    }

    protected override bool IsMatchLevel(PKM pk, EvoCriteria evo)
    {
        if (pk.Format != 4) // Met Level lost on PK4=>PK5
            return Level <= evo.LevelMax;

        return pk.Met_Level == Level;
    }

    protected override bool IsMatchPartial(PKM pk)
    {
        if (pk.Ball != 4)
            return true;
        if (!IsCourseAvailable(pk.Language))
            return true;
        return base.IsMatchPartial(pk);
    }

    public bool IsCourseAvailable(int language) => Course switch
    {
        PokewalkerCourse4.Rally       => language is (int)LanguageID.Japanese,
        PokewalkerCourse4.Sightseeing => language is (int)LanguageID.Japanese or (int)LanguageID.Korean,
        PokewalkerCourse4.AmityMeadow => language is (int)LanguageID.Japanese,
        _ => true,
    };

    protected override void SetPINGA(PKM pk, EncounterCriteria criteria)
    {
        var pi = pk.PersonalInfo;
        int gender = criteria.GetGender(Gender, pi);
        int nature = (int)criteria.GetNature(Nature.Random);

        // Cannot force an ability; nature-gender-trainerID only yield fixed PIDs.
        // int ability = criteria.GetAbilityFromNumber(Ability, pi);

        PIDGenerator.SetRandomPIDPokewalker(pk, nature, gender);
        criteria.SetRandomIVs(pk);
    }

    public static EncounterStatic4Pokewalker[] GetAll(ReadOnlySpan<byte> data)
    {
        const int size = 0xC;
        var count = data.Length / size;
        System.Diagnostics.Debug.Assert(count == 6 * (int)PokewalkerCourse4.MAX_COUNT);
        var result = new EncounterStatic4Pokewalker[count];
        for (int i = 0; i < result.Length; i++)
        {
            var offset = i * size;
            var slice = data[offset..];
            var course = (PokewalkerCourse4)(i / 6);
            result[i] = new(slice, course);
        }
        return result;
    }
}

public enum PokewalkerCourse4 : byte
{
    RefreshingField = 0,
    NoisyForest = 1,
    RuggedRoad = 2,
    BeautifulBeach = 3,
    SuburbanArea = 4,
    DimCave = 5,
    BlueLake = 6,
    TownOutskirts = 7,
    HoennField = 8,
    WarmBeach = 9,
    VolcanoPath = 10,
    Treehouse = 11,
    ScaryCave = 12,
    SinnohField = 13,
    IcyMountainRoad = 14,
    BigForest = 15,
    WhiteLake = 16,
    StormyBeach = 17,
    Resort = 18,
    QuietCave = 19,
    BeyondTheSea = 20,
    NightSkysEdge = 21,
    YellowForest = 22,
    Rally = 23, // JPN Exclusive
    Sightseeing = 24, // JPN/KOR Exclusive
    WinnersPath = 25,
    AmityMeadow = 26, // JPN Exclusive
    MAX_COUNT = 27,
}
