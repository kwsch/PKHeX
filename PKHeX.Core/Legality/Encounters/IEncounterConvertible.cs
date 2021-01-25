namespace PKHeX.Core
{
    public interface IEncounterConvertible
    {
        PKM ConvertToPKM(ITrainerInfo sav);
        PKM ConvertToPKM(ITrainerInfo sav, EncounterCriteria criteria);
    }
}
