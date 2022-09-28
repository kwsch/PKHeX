namespace PKHeX.Core;

/// <summary>
/// Data class for max counts denoted by the ranch level in My Pokemon Ranch
/// </summary>
public class RanchLevel
{
    public readonly int LevelIndex;
    public readonly int Level;
    public readonly int MaxMiis;
    public readonly int MaxPkm;
    public readonly int MaxToys;

    public RanchLevel(int levelIndex)
    {
        this.LevelIndex = levelIndex;
        this.Level = GetLevel();
        this.MaxMiis = GetMaxMiis();
        this.MaxPkm = GetMaxPkm();
        this.MaxToys = GetMaxToys();
    }

    private int GetLevel()
    {
        return LevelIndex + 1;
    }

    private int GetMaxMiis()
    {
        if (Level >= 11)
            return 20;
        else if (Level >= 8)
            return 15;
        else if (Level >= 4)
            return 10;
        else
            return 5;
    }

    private int GetMaxToys()
    {
        if (Level >= 25)
            return 6;
        else if (Level >= 20)
            return 5;
        else if (Level >= 15)
            return 4;
        else if (Level >= 11)
            return 3;
        else if (Level >= 8)
            return 2;
        else
            return 1;
    }

    private int GetMaxPkm()
    {
        int[] maxPkmCounts = { 20, 25, 30, 40, 50, 60, 80, 100, 150, 200, 250, 300, 350, 400, 500, 600, 700, 800, 900, 1000, 1000, 1000, 1000, 1000, 1000, 1500 };
        if (LevelIndex >= maxPkmCounts.Length)
            return maxPkmCounts[maxPkmCounts.Length - 1];
        else
            return maxPkmCounts[LevelIndex];
    }
}
