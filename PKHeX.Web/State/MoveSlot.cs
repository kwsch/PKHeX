using PKHeX.Core;

namespace PKHeX.Web.State;

/// <summary>
/// One of the four drafted move slots: the move, its current PP and PP Ups as stored, and the PP the move has with those PP Ups.
/// </summary>
/// <param name="Move">The move; 0 is an empty slot.</param>
/// <param name="Pp">Current PP, as stored. It may be above <paramref name="MaxPp"/> when stored that way, until it is edited.</param>
/// <param name="PpUps">PP Ups applied, as stored.</param>
/// <param name="MaxPp">The move's PP with those PP Ups, as Core calculates it (<see cref="PKM.GetMovePP"/>); 0 for an empty slot.</param>
/// <param name="CanTakePpUps">False for an empty slot and for a move PP Ups cannot be used on, such as Sketch (<see cref="Legal.IsPPUpAvailable(ushort)"/>).</param>
public sealed record MoveSlot(ushort Move, int Pp, int PpUps, int MaxPp, bool CanTakePpUps)
{
    /// <summary>True when the slot holds no move. Its PP and PP Ups cannot be edited.</summary>
    public bool IsEmpty => Move == 0;

    /// <summary>Reads slot <paramref name="slot"/> (0–3) of <paramref name="pk"/>.</summary>
    internal static MoveSlot Of(PK6 pk, int slot)
    {
        var move = pk.GetMove(slot);
        var ppUps = MoveSlots.GetPpUps(pk, slot);
        return new MoveSlot(move, MoveSlots.GetPp(pk, slot), ppUps, move == 0 ? 0 : pk.GetMovePP(move, ppUps), Legal.IsPPUpAvailable(move));
    }
}

/// <summary>
/// Reads and writes one move slot's PP and PP Ups by index. Core indexes the move itself (<see cref="PKM.GetMove"/>,
/// <see cref="PKM.SetMove"/>) but not its PP, and its whole-moveset helpers (<see cref="PKM.SetMoves"/>, <see cref="PKM.FixMoves"/>)
/// rewrite or reorder every slot, which an edit of one slot must not do.
/// </summary>
internal static class MoveSlots
{
    /// <summary>Current PP of slot <paramref name="slot"/>.</summary>
    public static int GetPp(PK6 pk, int slot) => slot switch
    {
        0 => pk.Move1_PP,
        1 => pk.Move2_PP,
        2 => pk.Move3_PP,
        3 => pk.Move4_PP,
        _ => throw new ArgumentOutOfRangeException(nameof(slot), slot, "The move slot must be 0–3."),
    };

    /// <summary>PP Ups of slot <paramref name="slot"/>.</summary>
    public static int GetPpUps(PK6 pk, int slot) => slot switch
    {
        0 => pk.Move1_PPUps,
        1 => pk.Move2_PPUps,
        2 => pk.Move3_PPUps,
        3 => pk.Move4_PPUps,
        _ => throw new ArgumentOutOfRangeException(nameof(slot), slot, "The move slot must be 0–3."),
    };

    /// <summary>Sets the current PP and PP Ups of slot <paramref name="slot"/>, leaving its move and every other slot as they are.</summary>
    public static void SetPp(PK6 pk, int slot, int pp, int ppUps)
    {
        switch (slot)
        {
            case 0:
                pk.Move1_PP = pp;
                pk.Move1_PPUps = ppUps;
                break;
            case 1:
                pk.Move2_PP = pp;
                pk.Move2_PPUps = ppUps;
                break;
            case 2:
                pk.Move3_PP = pp;
                pk.Move3_PPUps = ppUps;
                break;
            case 3:
                pk.Move4_PP = pp;
                pk.Move4_PPUps = ppUps;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(slot), slot, "The move slot must be 0–3.");
        }
    }
}
