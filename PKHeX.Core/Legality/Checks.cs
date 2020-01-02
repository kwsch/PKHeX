using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    public partial class LegalityAnalysis
    {
        private static readonly Verifier Language = new LanguageVerifier();
        private static readonly Verifier Nickname = new NicknameVerifier();
        private static readonly Verifier EffortValues = new EffortValueVerifier();
        private static readonly Verifier IndividualValues = new IndividualValueVerifier();
        private static readonly Verifier Ball = new BallVerifier();
        private static readonly Verifier Form = new FormVerifier();
        private static readonly Verifier ConsoleRegion = new ConsoleRegionVerifier();
        private static readonly Verifier Ability = new AbilityVerifier();
        private static readonly Verifier Medal = new MedalVerifier();
        public static readonly Verifier Ribbon = new RibbonVerifier();
        private static readonly Verifier Item = new ItemVerifier();
        private static readonly Verifier EncounterType = new EncounterTypeVerifier();
        private static readonly Verifier HyperTraining = new HyperTrainingVerifier();
        private static readonly Verifier Gender = new GenderVerifier();
        private static readonly Verifier PIDEC = new PIDVerifier();
        private static readonly Verifier NHarmonia = new NHarmoniaVerifier();
        private static readonly Verifier CXD = new CXDVerifier();
        private static readonly Verifier Memory = new MemoryVerifier();
        private static readonly Verifier History = new HistoryVerifier();

        private static readonly TrainerNameVerifier Trainer = new TrainerNameVerifier();
        private static readonly LevelVerifier Level = new LevelVerifier();
        private static readonly MiscVerifier Misc = new MiscVerifier();
        private static readonly TransferVerifier Transfer = new TransferVerifier();

        public static string[] MoveStrings { internal get; set; } = Util.GetMovesList(GameLanguage.DefaultLanguage);
        public static string[] SpeciesStrings { internal get; set; } = Util.GetSpeciesList(GameLanguage.DefaultLanguage);
        internal static IEnumerable<string> GetMoveNames(IEnumerable<int> moves) => moves.Select(m => (uint)m >= MoveStrings.Length ? L_AError : MoveStrings[m]);
    }
}
