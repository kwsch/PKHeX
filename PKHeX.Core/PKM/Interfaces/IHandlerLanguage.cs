namespace PKHeX.Core;

/// <summary>
/// Exposes info about the latest handler (not OT) language.
/// </summary>
public interface IHandlerLanguage
{
    /// <summary>
    /// Trainer game language of the latest handler (not OT).
    /// </summary>
    byte HandlingTrainerLanguage { get; set; }
}
