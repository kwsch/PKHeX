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

        public static readonly TrainerNameVerifier Trainer = new();
        public static readonly LevelVerifier Level = new();
        public static readonly MiscVerifier MiscValues = new();
        public static readonly TransferVerifier Transfer = new();
        public static readonly MarkVerifier Mark = new();
    }
}
