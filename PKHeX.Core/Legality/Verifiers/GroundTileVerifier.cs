using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core;

/// <summary>
/// Verifies the <see cref="PK4.GroundTile"/>.
/// </summary>
public sealed class GroundTileVerifier : Verifier
{
    protected override CheckIdentifier Identifier => CheckIdentifier.Encounter;

    public override void Verify(LegalityAnalysis data)
    {
        if (data.Entity is not IGroundTile e)
            return;
        var enc = data.EncounterMatch;
        bool valid = IsGroundTileValid(enc, e);
        var result = !valid ? GetInvalid(LEncTypeMismatch) : GetValid(LEncTypeMatch);
        data.AddLine(result);
    }

    /// <summary>
    /// Indicates if the <see cref="IGroundTile"/> is valid for the <see cref="IEncounterTemplate"/>.
    /// </summary>
    /// <param name="enc">Encounter Template</param>
    /// <param name="e">Entity with a stored <see cref="IGroundTile.GroundTile"/> value.</param>
    /// <returns>True if stored ground tile value is permitted.</returns>
    public static bool IsGroundTileValid(IEncounterTemplate enc, IGroundTile e)
    {
        var type = enc is IGroundTypeTile t ? t.GroundTile : GroundTileAllowed.None;
        return type.Contains(e.GroundTile);
    }
}
