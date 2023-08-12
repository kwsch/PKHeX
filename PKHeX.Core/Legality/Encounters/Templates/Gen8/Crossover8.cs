namespace PKHeX.Core;

public readonly record struct Crossover8(byte L1 = 0, byte L2 = 0, byte L3 = 0, byte L4 = 0, byte L5 = 0, byte L6 = 0, byte L7 = 0)
{
    public bool IsMatchLocation(byte location) => location != 0 && L1 != 0 && (
        location == L1 || location == L2 || location == L3 ||
        location == L4 || location == L5 || location == L6 || location == L7);
}
