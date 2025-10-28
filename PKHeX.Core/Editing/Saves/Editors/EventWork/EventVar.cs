namespace PKHeX.Core;

/// <summary>
/// Event variable used to determine game events.
/// </summary>
public abstract class EventVar(int Index, EventVarType Type, string Name)
{
    /// <summary>
    /// Name of event variable
    /// </summary>
    public readonly string Name = Name;

    /// <summary>
    /// Type of event variable
    /// </summary>
    public readonly EventVarType Type = Type;

    /// <summary>
    /// Unpacked structure's index.
    /// </summary>
    public readonly int RelativeIndex = Index;

    /// <summary>
    /// Raw index within the event variable (type) region.
    /// </summary>
    public int RawIndex;
}
