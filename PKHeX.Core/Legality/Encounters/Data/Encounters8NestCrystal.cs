using static PKHeX.Core.GameVersion;

namespace PKHeX.Core
{
    // Dynamax Crystal Distribution Nest Encounters (BCAT)
    internal static partial class Encounters8Nest
    {
        #region Dynamax Crystal Distributions
        internal static readonly EncounterStatic8NC[] Crystal_SWSH =
        {
            new(SWSH) { Species = 782, Level = 16, Ability = A3, Location = 126, IVs = new[] {31,31,31,-1,-1,-1}, DynamaxLevel = 2, Moves = new[] {033,029,525,043} }, // ★And458 Jangmo-o
            new(SWSH) { Species = 246, Level = 16, Ability = A3, Location = 126, IVs = new[] {31,31,31,-1,-1,-1}, DynamaxLevel = 2, Moves = new[] {033,157,371,044} }, // ★And15 Larvitar
            new(SWSH) { Species = 823, Level = 50, Ability = A2, Location = 126, IVs = new[] {31,31,31,-1,-1,31}, DynamaxLevel = 5, Moves = new[] {065,442,034,796}, CanGigantamax = true }, // ★And337 Gigantamax Corviknight
            new(SWSH) { Species = 875, Level = 15, Ability = A3, Location = 126, IVs = new[] {31,31,-1,31,-1,-1}, DynamaxLevel = 2, Moves = new[] {181,311,054,556} }, // ★And603 Eiscue
            new(SWSH) { Species = 874, Level = 15, Ability = A3, Location = 126, IVs = new[] {31,31,31,-1,-1,-1}, DynamaxLevel = 2, Moves = new[] {397,317,335,157} }, // ★And390 Stonjourner
            new(SWSH) { Species = 879, Level = 35, Ability = A3, Location = 126, IVs = new[] {31,31,-1, 0,31,-1}, DynamaxLevel = 4, Moves = new[] {484,174,776,583}, CanGigantamax = true }, // ★Sgr6879 Gigantamax Copperajah
            new(SWSH) { Species = 851, Level = 35, Ability = A2, Location = 126, IVs = new[] {31,31,31,-1,-1,-1}, DynamaxLevel = 5, Moves = new[] {680,679,489,438}, CanGigantamax = true }, // ★Sgr6859 Gigantamax Centiskorch
        };
        #endregion
    }
}
