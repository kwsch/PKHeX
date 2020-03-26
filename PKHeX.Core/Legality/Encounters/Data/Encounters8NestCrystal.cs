namespace PKHeX.Core
{
    // Dynamax Crystal Distribution Nest Encounters (BCAT)
    internal static partial class Encounters8Nest
    {
        #region Dynamax Crystal Distributions
        internal static readonly EncounterStatic8NC[] Crystal_SWSH =
        {
            new EncounterStatic8NC { Species = 782, Level = 16, Ability = A3, Location = 126, IVs = new[] {31,31,31,-1,-1,-1}, DynamaxLevel = 2, Moves = new[] {033,029,525,043} }, // ★And458 Jangmo-o
            new EncounterStatic8NC { Species = 246, Level = 16, Ability = A3, Location = 126, IVs = new[] {31,31,31,-1,-1,-1}, DynamaxLevel = 2, Moves = new[] {033,157,371,044} }, // ★And15 Larvitar
            new EncounterStatic8NC { Species = 823, Level = 50, Ability = A2, Location = 126, IVs = new[] {31,31,31,-1,-1,31}, DynamaxLevel = 5, Moves = new[] {065,442,034,796}, CanGigantamax = true }, // ★And337 Gigantamax Corviknight
            new EncounterStatic8NC { Species = 875, Level = 15, Ability = A3, Location = 126, IVs = new[] {31,31,-1,31,-1,-1}, DynamaxLevel = 2, Moves = new[] {181,311,054,556} }, // ★And603 Eiscue
            new EncounterStatic8NC { Species = 874, Level = 15, Ability = A3, Location = 126, IVs = new[] {31,31,31,-1,-1,-1}, DynamaxLevel = 2, Moves = new[] {397,317,335,157} }, // ★And390 Stonjourner
        };
        #endregion
    }
}
