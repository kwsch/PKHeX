namespace PKHeX.Core;

public interface IGymTeamInfo
{
    ushort GetBadgeVictorySpecies(uint badge, uint slot);
    void SetBadgeVictorySpecies(uint badge, uint slot, ushort species);
}

public static class GymTeamInfoExtensions
{
    public static ushort[][] GetGymTeams(this IGymTeamInfo info)
    {
        ushort[][] teams = new ushort[8][];
        for (uint badge = 0; badge < teams.Length; badge++)
            teams[badge] = GetGymTeam(info, badge);
        return teams;
    }

    private static ushort[] GetGymTeam(IGymTeamInfo info, uint badge)
    {
        var team = new ushort[6];
        for (uint slot = 0; slot < team.Length; slot++)
            team[slot] = info.GetBadgeVictorySpecies(badge, slot);
        return team;
    }

    public static void SetGymTeams(this IGymTeamInfo info, ushort[][] teams)
    {
        for (uint badge = 0; badge < teams.Length; badge++)
            info.SetGymTeam(badge, teams[badge]);
    }

    public static void SetGymTeam(this IGymTeamInfo info, uint badge, ushort[] team)
    {
        for (uint slot = 0; slot < team.Length; slot++)
            info.SetBadgeVictorySpecies(badge, slot, team[slot]);
    }
}
