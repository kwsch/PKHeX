namespace PKHeX.Core;

public readonly record struct LearnVersion(int Level, GameVersion Game = GameVersion.Any)
{
    public bool IsLevelUp => Level >= 0;
}
