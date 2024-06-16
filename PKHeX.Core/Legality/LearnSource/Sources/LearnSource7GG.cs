using System;
using System.Diagnostics.CodeAnalysis;
using static PKHeX.Core.LearnMethod;
using static PKHeX.Core.LearnEnvironment;

namespace PKHeX.Core;

/// <summary>
/// Exposes information about how moves are learned in <see cref="USUM"/>.
/// </summary>
public sealed class LearnSource7GG : ILearnSource<PersonalInfo7GG>
{
    public static readonly LearnSource7GG Instance = new();
    private static readonly PersonalTable7GG Personal = PersonalTable.GG;
    private static readonly Learnset[] Learnsets = LearnsetReader.GetArray(BinLinkerAccessor.Get(Util.GetBinaryResource("lvlmove_gg.pkl"), "gg"u8));
    private const int MaxSpecies = Legal.MaxSpeciesID_7b;
    private const LearnEnvironment Game = GG;
    private const int ReminderBonus = 100; // Move reminder allows re-learning ALL level up moves regardless of level.

    public LearnEnvironment Environment => Game;

    public Learnset GetLearnset(ushort species, byte form) => Learnsets[Personal.GetFormIndex(species, form)];

    public bool TryGetPersonal(ushort species, byte form, [NotNullWhen(true)] out PersonalInfo7GG? pi)
    {
        pi = null;
        if (species > MaxSpecies)
            return false;
        pi = Personal[species, form];
        return true;
    }

    public MoveLearnInfo GetCanLearn(PKM pk, PersonalInfo7GG pi, EvoCriteria evo, ushort move, MoveSourceType types = MoveSourceType.All, LearnOption option = LearnOption.Current)
    {
        if (types.HasFlag(MoveSourceType.LevelUp))
        {
            var learn = GetLearnset(evo.Species, evo.Form);
            var level = learn.GetLevelLearnMove(move);
            if (level != -1) // Can relearn at any level!
                return new(LevelUp, Game, (byte)level);
        }

        if (types.HasFlag(MoveSourceType.Machine) && pi.GetIsLearnTM(TMHM_GG.IndexOf(move)))
            return new(TMHM, Game);

        if (types.HasFlag(MoveSourceType.EnhancedTutor) && GetIsEnhancedTutor(evo.Species, evo.Form, move))
            return new(Tutor, Game);

        return default;
    }

    private static bool GetIsEnhancedTutor(ushort species, byte form, ushort move)
    {
        if (species == (int)Species.Pikachu && form == 8) // Partner
            return Tutor_StarterPikachu.Contains(move);
        if (species == (int)Species.Eevee && form == 1) // Partner
            return Tutor_StarterEevee.Contains(move);
        return false;
    }

    public void GetAllMoves(Span<bool> result, PKM pk, EvoCriteria evo, MoveSourceType types = MoveSourceType.All)
    {
        if (!TryGetPersonal(evo.Species, evo.Form, out var pi))
            return;

        if (types.HasFlag(MoveSourceType.LevelUp))
        {
            var learn = GetLearnset(evo.Species, evo.Form);
            var span = learn.GetMoveRange(ReminderBonus);
            foreach (var move in span)
                result[move] = true;
        }

        if (types.HasFlag(MoveSourceType.Machine))
            pi.SetAllLearnTM(result, TMHM_GG);

        if (types.HasFlag(MoveSourceType.SpecialTutor))
        {
            if (evo is { Species: (int)Species.Pikachu, Form: 8 }) // Partner
            {
                foreach (var move in Tutor_StarterPikachu)
                    result[move] = true;
            }
            else if (evo is { Species: (int)Species.Eevee, Form: 1 }) // Partner
            {
                foreach (var move in Tutor_StarterEevee)
                    result[move] = true;
            }
        }
    }

    private static ReadOnlySpan<ushort> TMHM_GG =>
    [
        029, 269, 270, 100, 156, 113, 182, 164, 115, 091,
        261, 263, 280, 019, 069, 086, 525, 369, 231, 399,
        492, 157, 009, 404, 127, 398, 092, 161, 503, 339,
        007, 605, 347, 406, 008, 085, 053, 087, 200, 094,
        089, 120, 247, 583, 076, 126, 057, 063, 276, 355,
        059, 188, 072, 430, 058, 446, 006, 529, 138, 224,
        // rest are same as SM, unused

        // No HMs
    ];

    private static ReadOnlySpan<ushort> Tutor_StarterPikachu =>
    [
        (int)Move.ZippyZap,
        (int)Move.SplishySplash,
        (int)Move.FloatyFall,
        //(int)Move.PikaPapow, // Joycon Shake
    ];

    private static ReadOnlySpan<ushort> Tutor_StarterEevee =>
    [
        (int)Move.BouncyBubble,
        (int)Move.BuzzyBuzz,
        (int)Move.SizzlySlide,
        (int)Move.GlitzyGlow,
        (int)Move.BaddyBad,
        (int)Move.SappySeed,
        (int)Move.FreezyFrost,
        (int)Move.SparklySwirl,
        //(int)Move.VeeveeVolley, // Joycon Shake
    ];
}
