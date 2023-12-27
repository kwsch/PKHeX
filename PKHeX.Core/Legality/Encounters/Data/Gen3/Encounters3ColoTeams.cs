using static PKHeX.Core.Encounters3ColoShadow;

namespace PKHeX.Core;

/// <summary>
/// Permutations of teams, where a team may have multiple shadow Pokémon or alternate/re-battle scenarios.
/// </summary>
public static class Encounters3ColoTeams
{
    public static readonly TeamLock[] First = [];

    // Colo
    public static readonly TeamLock[] ColoMakuhita = [CMakuhita];
    public static readonly TeamLock[] Gligar = [CGligar];
    public static readonly TeamLock[] Murkrow = [CMurkrow];
    public static readonly TeamLock[] Heracross = [CHeracross];
    public static readonly TeamLock[] Ursaring = [CUrsaring];

    // E-Reader
    public static readonly TeamLock[] CTogepi = [ETogepi];
    public static readonly TeamLock[] CMareep = [EMareep];
    public static readonly TeamLock[] CScizor = [EScizor];
}
