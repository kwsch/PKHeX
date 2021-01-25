namespace PKHeX.Core
{
    public interface IEncounterMatch
    {
        bool IsMatchExact(PKM pkm, DexLevel dl);
        EncounterMatchRating GetMatchRating(PKM pkm);
    }
}
