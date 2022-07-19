namespace PKHeX.Core;

public interface IPersonalEgg
{
    /// <summary>
    /// First Egg Group
    /// </summary>
    int EggGroup1 { get; set; }

    /// <summary>
    /// Second Egg Group
    /// </summary>
    int EggGroup2 { get; set; }
}

public static class PersonalEggExtensions
{
    /// <summary>
    /// Checks if the entry has either egg group equal to the input type.
    /// </summary>
    /// <param name="pi">Object reference</param>
    /// <param name="group">Egg group</param>
    /// <returns>Egg is present in entry</returns>
    public static bool IsEggGroup(this IPersonalEgg pi, int group) => pi.EggGroup1 == group || pi.EggGroup2 == group;
}
