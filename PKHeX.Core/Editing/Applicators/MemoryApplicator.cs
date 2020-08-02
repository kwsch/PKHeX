namespace PKHeX.Core
{
    public static class MemoryApplicator
    {
        /// <summary>
        /// Sets all Memory related data to the default value (zero).
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        public static void ClearMemories(this PKM pk)
        {
            if (pk is IAffection a)
                a.OT_Affection = a.HT_Affection = 0;
            if (pk is IMemoryOT o)
                o.ClearMemoriesOT();
            if (pk is IMemoryHT h)
                h.ClearMemoriesHT();
        }

        /// <summary>
        /// Sets the Memory details to a Hatched Egg's memories.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        public static void SetHatchMemory6(this PKM pk)
        {
            if (pk is IMemoryOT o)
            {
                o.OT_Memory = 2;
                o.OT_Feeling = Memories.GetRandomFeeling(2);
                o.OT_Intensity = 1;
                o.OT_TextVar = pk.XY ? 43 : 27; // riverside road : battling spot
            }
            if (pk is IAffection a)
                a.OT_Affection = 0;
        }

        /// <summary>
        /// Sets a random memory specific to <see cref="GameVersion.Gen6"/> locality.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        public static void SetRandomMemory6(this PK6 pk)
        {
            // for lack of better randomization :)
            pk.OT_Memory = 63;
            pk.OT_Intensity = 6;
            pk.OT_Feeling = Memories.GetRandomFeeling(pk.OT_Memory);
        }
    }
}