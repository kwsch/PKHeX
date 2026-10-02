using PKHeX.Core;

namespace PKHeX.Web.Services;

/// <summary>
/// The stats a nature raises and lowers by a tenth, as indexes into the summary's stat order (HP, Attack, Defense, Sp. Atk, Sp. Def, Speed;
/// see <see cref="StatsFacts.Stats"/>).
/// </summary>
/// <param name="Raised">The raised stat's index, or -1 when the nature changes no stat.</param>
/// <param name="Lowered">The lowered stat's index, or -1 when the nature changes no stat.</param>
public readonly record struct NatureEffect(int Raised, int Lowered)
{
    /// <summary>A nature that changes no stat: one that raises and lowers the same stat, or a stored value outside the 25 natures.</summary>
    public static readonly NatureEffect Neutral = new(-1, -1);

    /// <summary>True when the nature changes no stat.</summary>
    public bool IsNeutral => Raised < 0;

    /// <summary>Summary indexes of Core's nature stat order: Attack, Defense, Speed, Sp. Atk, Sp. Def.</summary>
    private static ReadOnlySpan<byte> SummaryIndex => [1, 2, 5, 3, 4];

    /// <summary>The effect Core gives <paramref name="nature"/> when it calculates stats (<see cref="NatureAmp"/>).</summary>
    public static NatureEffect Of(Nature nature)
    {
        var (up, dn) = nature.GetNatureModification();
        return nature.IsNeutralOrInvalid(up, dn) ? Neutral : new NatureEffect(SummaryIndex[up], SummaryIndex[dn]);
    }
}
