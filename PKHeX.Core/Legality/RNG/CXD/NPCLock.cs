namespace PKHeX.Core;

/// <summary>
/// Locks associated to a given NPC PKM that appears before a <see cref="IShadow3"/>.
/// </summary>
public readonly record struct NPCLock
{
    private readonly byte Nature;
    private readonly byte Gender;
    private readonly byte Ratio;
    private readonly byte State;
    private readonly ushort Species;

    public int FramesConsumed => Seen ? 5 : 7;
    public bool Seen => State > 1;
    public bool Shadow => State != 0;
    public (Nature Nature, byte Gender) GetLock => ((Nature)Nature, Gender);

    // Not-Shadow
    public NPCLock(ushort s, byte n, byte g, byte r)
    {
        Species = s;
        Nature = n;
        Gender = g;
        Ratio = r;
    }

    // Shadow
    public NPCLock(ushort s, bool seen = false)
    {
        Species = s;
        State = seen ? (byte)2 : (byte)1;
    }

    public bool MatchesLock(uint PID)
    {
        if (Shadow && Nature == 0) // Non-locked shadow
            return true;
        if (Gender != 2 && Gender != ((PID & 0xFF) < Ratio ? 1 : 0))
            return false;
        if (Nature != PID % 25)
            return false;
        return true;
    }

#if DEBUG
    public override string ToString()
    {
        var sb = new System.Text.StringBuilder(64);
        sb.Append((Species)Species);
        if (State != 0)
            sb.Append(" (Shadow)");
        if (Seen)
            sb.Append(" [Seen]");
        sb.Append($" - Nature: {(Nature)Nature}");
        if (Gender != 2)
            sb.Append($", Gender: {Gender}");
        return sb.ToString();
    }
#endif
}
