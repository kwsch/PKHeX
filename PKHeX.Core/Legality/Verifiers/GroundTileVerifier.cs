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
        var type = data.EncounterMatch is IGroundTypeTile t ? t.GroundTile : GroundTileAllowed.None;
        var result = !type.Contains(e.GroundTile) ? GetInvalid(LEncTypeMismatch) : GetValid(LEncTypeMatch);
        data.AddLine(result);
    }
}
