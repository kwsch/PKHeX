namespace PKHeX.Core
{
    public interface ITeamIndexSet
    {
        bool GetIsTeamLocked(int team);
        void SetIsTeamLocked(int team, bool value);

        void ClearBattleTeams();
        void SaveBattleTeams();
        void UnlockAllTeams();
    }
}
