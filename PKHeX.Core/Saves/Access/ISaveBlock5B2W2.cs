namespace PKHeX.Core;

/// <summary>
/// Interface for Accessing named blocks within a Generation 5 save file.
/// </summary>
public interface ISaveBlock5B2W2
{
    PWTBlock5 PWT { get; }
    KeySystem5 Keys { get; }
    FestaBlock5 Festa { get; }
}
