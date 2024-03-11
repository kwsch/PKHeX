using System;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.LearnMethod;
using static PKHeX.Core.LearnEnvironment;

namespace PKHeX.Core;

/// <summary>
/// Exposes information about how moves are learned in <see cref="ORAS"/>.
/// </summary>
public sealed class LearnSource6AO : ILearnSource<PersonalInfo6AO>, IEggSource
{
    public static readonly LearnSource6AO Instance = new();
    private static readonly PersonalTable6AO Personal = PersonalTable.AO;
    private static readonly Learnset[] Learnsets = LearnsetReader.GetArray(BinLinkerAccessor.Get(Util.GetBinaryResource("lvlmove_ao.pkl"), "ao"u8));
    private static readonly EggMoves6[] EggMoves = EggMoves6.GetArray(BinLinkerAccessor.Get(Util.GetBinaryResource("eggmove_ao.pkl"), "ao"u8));
    private const int MaxSpecies = Legal.MaxSpeciesID_6;
    private const LearnEnvironment Game = ORAS;

    public LearnEnvironment Environment => Game;

    public Learnset GetLearnset(ushort species, byte form) => Learnsets[Personal.GetFormIndex(species, form)];

    public bool TryGetPersonal(ushort species, byte form, [NotNullWhen(true)] out PersonalInfo6AO? pi)
    {
        pi = null;
        if (species > MaxSpecies)
            return false;
        pi = Personal[species, form];
        return true;
    }

    public bool GetIsEggMove(ushort species, byte form, ushort move)
    {
        if (species > MaxSpecies)
            return false;
        var moves = EggMoves[species];
        return moves.GetHasEggMove(move);
    }

    public ReadOnlySpan<ushort> GetEggMoves(ushort species, byte form)
    {
        if (species > MaxSpecies)
            return [];
        return EggMoves[species].Moves;
    }

    public MoveLearnInfo GetCanLearn(PKM pk, PersonalInfo6AO pi, EvoCriteria evo, ushort move, MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
        if (types.HasFlag(MoveSourceType.LevelUp))
        {
            var learn = GetLearnset(evo.Species, evo.Form);
            var level = learn.GetLevelLearnMove(move);
            if (level != -1 && level <= evo.LevelMax)
                return new(LevelUp, Game, (byte)level);
        }

        if (types.HasFlag(MoveSourceType.Machine) && GetIsTM(pi, move))
            return new(TMHM, Game);

        if (types.HasFlag(MoveSourceType.TypeTutor) && pi.GetIsLearnTutorType(LearnSource5.TypeTutor567.IndexOf(move)))
            return new(Tutor, Game);

        if (types.HasFlag(MoveSourceType.SpecialTutor) && pi.GetIsTutorSpecial(move))
            return new(Tutor, Game);

        if (types.HasFlag(MoveSourceType.EnhancedTutor) && GetIsEnhancedTutor(evo, pk, move, option))
            return new(Tutor, Game);

        return default;
    }

    private static bool GetIsEnhancedTutor(EvoCriteria evo, ISpeciesForm current, ushort move, LearnOption option) => evo.Species switch
    {
        (int)Species.Keldeo => move is (int)Move.SecretSword,
        (int)Species.Meloetta => move is (int)Move.RelicSong,
        (int)Species.Rotom => move switch
        {
            (int)Move.Overheat  => option.IsPast() || current.Form == 1,
            (int)Move.HydroPump => option.IsPast() || current.Form == 2,
            (int)Move.Blizzard  => option.IsPast() || current.Form == 3,
            (int)Move.AirSlash  => option.IsPast() || current.Form == 4,
            (int)Move.LeafStorm => option.IsPast() || current.Form == 5,
            _ => false,
        },
        _ => false,
    };

    private static bool GetIsTM(PersonalInfo6AO info, ushort move)
    {
        var index = TMHM_AO.IndexOf(move);
        return info.GetIsLearnTM(index);
    }

    public void GetAllMoves(Span<bool> result, PKM pk, EvoCriteria evo, MoveSourceType types = MoveSourceType.All)
    {
        if (!TryGetPersonal(evo.Species, evo.Form, out var pi))
            return;

        if (types.HasFlag(MoveSourceType.LevelUp))
        {
            var learn = GetLearnset(evo.Species, evo.Form);
            var span = learn.GetMoveRange(evo.LevelMax);
            foreach (var move in span)
                result[move] = true;
        }

        if (types.HasFlag(MoveSourceType.Machine))
            pi.SetAllLearnTM(result, TMHM_AO);
        if (types.HasFlag(MoveSourceType.TypeTutor))
            pi.SetAllLearnTutorType(result, LearnSource5.TypeTutor567);
        if (types.HasFlag(MoveSourceType.SpecialTutor))
            pi.SetAllLearnTutorSpecial(result);

        if (types.HasFlag(MoveSourceType.EnhancedTutor))
        {
            var species = evo.Species;
            if (species is (int)Species.Rotom && evo.Form is not 0)
                result[MoveTutor.GetRotomFormMove(evo.Form)] = true;
            else if (species is (int)Species.Keldeo)
                result[(int)Move.SecretSword] = true;
            else if (species is (int)Species.Meloetta)
                result[(int)Move.RelicSong] = true;
        }
    }

    internal static ReadOnlySpan<ushort> TMHM_AO =>
    [
        468, 337, 473, 347, 046, 092, 258, 339, 474, 237,
        241, 269, 058, 059, 063, 113, 182, 240, 355, 219,
        218, 076, 479, 085, 087, 089, 216, 091, 094, 247,
        280, 104, 115, 482, 053, 188, 201, 126, 317, 332,
        259, 263, 488, 156, 213, 168, 490, 496, 497, 315,
        211, 411, 412, 206, 503, 374, 451, 507, 510, 511,
        261, 512, 373, 153, 421, 371, 514, 416, 397, 148,
        444, 521, 086, 360, 014, 522, 244, 523, 524, 157,
        404, 525, 611, 398, 138, 447, 207, 214, 369, 164,
        430, 433, 528, 290, 555, 267, 399, 612, 605, 590,

        15, 19, 57, 70, 127, 249, 291,
    ];
}
