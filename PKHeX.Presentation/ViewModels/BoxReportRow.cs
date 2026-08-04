using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

/// <summary>
/// One row of the Box Data Report: an <see cref="EntitySummary"/> that knows which
/// box/slot it came from so the report can navigate back to the source Pokémon.
/// </summary>
public sealed class BoxReportRow : EntitySummary
{
    /// <summary>Zero-based box index of the source slot.</summary>
    public int Box { get; }

    /// <summary>Zero-based slot index within the box.</summary>
    public int Slot { get; }

    public override string Position => $"B{Box + 1:00}:{Slot + 1:00}";

    public int IVTotal => IV_HP + IV_ATK + IV_DEF + IV_SPA + IV_SPD + IV_SPE;
    public int EVTotal => EV_HP + EV_ATK + EV_DEF + EV_SPA + EV_SPD + EV_SPE;

    public BoxReportRow(PKM pk, GameStrings strings, int box, int slot) : base(pk, strings)
    {
        Box = box;
        Slot = slot;
    }
}
