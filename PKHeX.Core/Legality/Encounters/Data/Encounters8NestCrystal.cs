namespace PKHeX.Core
{
    // Dynamax Crystal Distribution Nest Encounters (BCAT)
    internal static partial class Encounters8Nest
    {
        #region Dynamax Crystal Distributions
        internal static readonly EncounterStatic8NC[] Crystal_SWSH =
        {
            new EncounterStatic8NC { Species = 782, Level = 16, Ability = A4, Location = 126, IVs = new []{31,31,31,-1,-1,-1}, DynamaxLevel = 2, Moves = new[] {033,029,525,043}, }, // ★And458 Jangmo-o
            new EncounterStatic8NC { Species = 246, Level = 16, Ability = A4, Location = 126, IVs = new []{31,31,31,-1,-1,-1}, DynamaxLevel = 2, Moves = new[] {033,157,371,044}, }, // ★And15 Larvitar
            new EncounterStatic8NC { Species = 823, Level = 50, Ability = A4, Location = 126, IVs = new []{31,31,31,-1,-1,31}, DynamaxLevel = 5, Moves = new[] {065,442,034,796}, CanGigantamax = true }, // ★And337 4-Star Gigantamax Corviknight
        };
        #endregion
    }
}
