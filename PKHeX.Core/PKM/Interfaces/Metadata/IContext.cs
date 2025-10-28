namespace PKHeX.Core;

public interface IContext
{
    /// <summary>
    /// The Context the data originated in.
    /// </summary>
    EntityContext Context { get; }
}
