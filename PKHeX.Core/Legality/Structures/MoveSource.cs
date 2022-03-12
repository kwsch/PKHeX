namespace PKHeX.Core
{
    /// <summary>
    /// Indicates the method of learning a move
    /// </summary>
    public enum MoveSource : byte
    {
        NotParsed,
        Unknown,
        None,
        Relearn,
        Initial,
        LevelUp,
        TMHM,
        Tutor,
        EggMove,
        InheritLevelUp,
        Special,
        SpecialEgg,
        ShedinjaEvo,
        Sketch,
        Shared,
    }
}
