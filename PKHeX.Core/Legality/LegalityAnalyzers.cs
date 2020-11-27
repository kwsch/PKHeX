using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    internal static class LegalityAnalyzers
    {
        public static readonly Verifier LanguageIndex = new LanguageVerifier();
        public static readonly Verifier Nickname = new NicknameVerifier();
        public static readonly Verifier EffortValues = new EffortValueVerifier();
        public static readonly Verifier IndividualValues = new IndividualValueVerifier();
        public static readonly Verifier BallIndex = new BallVerifier();
        public static readonly Verifier FormValues = new FormVerifier();
        public static readonly Verifier ConsoleRegion = new ConsoleRegionVerifier();
        public static readonly Verifier AbilityValues = new AbilityVerifier();
        public static readonly Verifier Medal = new MedalVerifier();
        public static readonly Verifier Ribbon = new RibbonVerifier();
        public static readonly Verifier Item = new ItemVerifier();
        public static readonly Verifier Gen4EncounterType = new EncounterTypeVerifier();
        public static readonly Verifier HyperTraining = new HyperTrainingVerifier();
        public static readonly Verifier GenderValues = new GenderVerifier();
        public static readonly Verifier PIDEC = new PIDVerifier();
        public static readonly Verifier NHarmonia = new NHarmoniaVerifier();
        public static readonly Verifier CXD = new CXDVerifier();
        public static readonly Verifier Memory = new MemoryVerifier();
        public static readonly Verifier History = new HistoryVerifier();
        public static readonly Verifier Contest = new ContestStatVerifier();

        public static readonly TrainerNameVerifier Trainer = new TrainerNameVerifier();
        public static readonly LevelVerifier Level = new LevelVerifier();
        public static readonly MiscVerifier MiscValues = new MiscVerifier();
        public static readonly TransferVerifier Transfer = new TransferVerifier();
        public static readonly MarkVerifier Mark = new MarkVerifier();

        public static IReadOnlyList<string> MoveStrings = Util.GetMovesList(GameLanguage.DefaultLanguage);
        public static IReadOnlyList<string> SpeciesStrings = Util.GetSpeciesList(GameLanguage.DefaultLanguage);
        public static IEnumerable<string> GetMoveNames(IEnumerable<int> moves) => moves.Select(m => (uint)m >= MoveStrings.Count ? L_AError : MoveStrings[m]);

        public static void ChangeLocalizationStrings(IReadOnlyList<string> moves, IReadOnlyList<string> species)
        {
            SpeciesStrings = species;
            MoveStrings = moves;
        }
    }
}
