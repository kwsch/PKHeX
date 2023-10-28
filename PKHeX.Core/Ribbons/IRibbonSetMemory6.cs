namespace PKHeX.Core;

public interface IRibbonSetMemory6
{
    byte RibbonCountMemoryContest { get; set; }
    byte RibbonCountMemoryBattle { get; set; }
    bool HasContestMemoryRibbon { get; set; }
    bool HasBattleMemoryRibbon { get; set; }
}

public static partial class RibbonExtensions
{
    public static void CopyRibbonSetMemory6(this IRibbonSetMemory6 set, IRibbonSetMemory6 dest)
    {
        dest.HasContestMemoryRibbon = set.HasContestMemoryRibbon;
        dest.HasBattleMemoryRibbon = set.HasBattleMemoryRibbon;
        dest.RibbonCountMemoryContest = set.RibbonCountMemoryContest;
        dest.RibbonCountMemoryBattle = set.RibbonCountMemoryBattle;
    }
}
