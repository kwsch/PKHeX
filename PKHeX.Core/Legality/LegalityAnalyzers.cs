namespace PKHeX.Core;

/// <summary>
/// Collection of analyzers that are used for parsing secondary details.
/// </summary>
internal static class LegalityAnalyzers
{
    public static readonly LanguageVerifier LanguageIndex = new();
    public static readonly NicknameVerifier Nickname = new();
    public static readonly EffortValueVerifier EVs = new();
    public static readonly IndividualValueVerifier IVs = new();
    public static readonly BallVerifier BallIndex = new();
    public static readonly FormVerifier FormValues = new();
    public static readonly ConsoleRegionVerifier ConsoleRegion = new();
    public static readonly AbilityVerifier AbilityValues = new();
    public static readonly MedalVerifier Medal = new();
    public static readonly RibbonVerifier Ribbon = new();
    public static readonly ItemVerifier Item = new();
    public static readonly GroundTileVerifier Gen4GroundTile = new();
    public static readonly HyperTrainingVerifier HyperTraining = new();
    public static readonly GenderVerifier GenderValues = new();
    public static readonly PIDVerifier PIDEC = new();
    public static readonly CXDVerifier CXD = new();
    public static readonly MemoryVerifier Memory = new();
    public static readonly HistoryVerifier History = new();
    public static readonly ContestStatVerifier Contest = new();
    public static readonly MarkingVerifier Marking = new();
    public static readonly MovePPVerifier MovePP = new();

    public static readonly TrainerNameVerifier Trainer = new();
    public static readonly TrainerIDVerifier TrainerID = new();
    public static readonly LevelVerifier Level = new();
    public static readonly MiscVerifier MiscValues = new();
    public static readonly TransferVerifier Transfer = new();
    public static readonly MarkVerifier Mark = new();
    public static readonly LegendsArceusVerifier Arceus = new();
    public static readonly AwakenedValueVerifier Awakening = new();
    public static readonly TrashByteVerifier Trash = new();
    public static readonly SlotTypeVerifier SlotType = new();
}
