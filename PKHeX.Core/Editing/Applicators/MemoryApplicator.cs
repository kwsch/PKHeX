namespace PKHeX.Core;

/// <summary>
/// Logic for modifying the Memory parameters of a <see cref="PKM"/>.
/// </summary>
public static class MemoryApplicator
{
    extension(PKM pk)
    {
        /// <summary>
        /// Sets all Memory related data to the default value (zero).
        /// </summary>
        public void ClearMemories()
        {
            if (pk is IAffection a)
                a.OriginalTrainerAffection = a.HandlingTrainerAffection = 0;
            if (pk is IMemoryOT o)
                o.ClearMemoriesOT();
            if (pk is IMemoryHT h)
                h.ClearMemoriesHT();
        }

        /// <summary>
        /// Sets the Memory details to a Hatched Egg's memories specific to <see cref="EntityContext.Gen6"/> origin.
        /// </summary>
        public void SetHatchMemory6()
        {
            if (pk is IMemoryOT o)
            {
                o.OriginalTrainerMemory = 2;
                o.OriginalTrainerMemoryFeeling = MemoryContext6.GetRandomFeeling6(2);
                o.OriginalTrainerMemoryIntensity = 1;
                o.OriginalTrainerMemoryVariable = pk.XY ? (ushort)43 : (ushort)27; // riverside road : battling spot
            }
            if (pk is IAffection a)
                a.OriginalTrainerAffection = 0;
        }
    }

    /// <summary>
    /// Sets a random memory specific to <see cref="EntityContext.Gen6"/> origin.
    /// </summary>
    /// <param name="pk">Pokémon to modify.</param>
    public static void SetRandomMemory6(this PK6 pk)
    {
        // for lack of better randomization :)
        const byte memory = 63; // almost got lost when it explored a forest with {Trainer}
        pk.OriginalTrainerMemory = memory;
        pk.OriginalTrainerMemoryFeeling = MemoryContext6.GetRandomFeeling6(memory);
        pk.OriginalTrainerMemoryIntensity = MemoryContext6.MaxIntensity;
    }
}
