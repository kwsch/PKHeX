namespace PKHeX.Core;

/// <summary>
/// Context for determining if a ball can be inherited.
/// </summary>
public interface IBallContext
{
    /// <summary>
    /// Checks if the <see cref="PKM"/> can be bred with the given <see cref="Ball"/>.
    /// </summary>
    /// <param name="species">Encounter Species</param>
    /// <param name="form">Encounter Form</param>
    /// <param name="ball">Encounter Ball</param>
    /// <param name="pk">Current Entity</param>
    BallInheritanceResult CanBreedWithBall(ushort species, byte form, Ball ball, PKM pk);

    /// <inheritdoc cref="CanBreedWithBall(ushort, byte, Ball, PKM)"/>
    bool CanBreedWithBall(ushort species, byte form, Ball ball);
}
