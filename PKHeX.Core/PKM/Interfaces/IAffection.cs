namespace PKHeX.Core;

/// <summary>
/// Exposes <see cref="OriginalTrainerAffection"/> and <see cref="HandlingTrainerAffection"/> properties used by Gen6/7.
/// </summary>
public interface IAffection
{
    byte OriginalTrainerAffection { get; set; }
    byte HandlingTrainerAffection { get; set; }
}
