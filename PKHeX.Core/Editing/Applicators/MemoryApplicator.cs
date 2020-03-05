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
            pk.OT_Memory = pk.OT_Affection = pk.OT_Feeling = pk.OT_Intensity = pk.OT_TextVar =
                pk.HT_Memory = pk.HT_Affection = pk.HT_Feeling = pk.HT_Intensity = pk.HT_TextVar = 0;
        }

        /// <summary>
        /// Sets the Memory details to a Hatched Egg's memories.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        public static void SetHatchMemory6(this PKM pk)
        {
            pk.OT_Memory = 2;
            pk.OT_Affection = 0;
            pk.OT_Feeling = Memories.GetRandomFeeling(pk.OT_Memory);
            pk.OT_Intensity = 1;
            pk.OT_TextVar = pk.XY ? 43 : 27; // riverside road : battling spot
        }

        /// <summary>
        /// Sets a random memory specific to <see cref="GameVersion.Gen6"/> locality.
        /// </summary>
        /// <param name="pk">Pokémon to modify.</param>
        public static void SetRandomMemory6(this PKM pk)
        {
            // for lack of better randomization :)
            pk.OT_Memory = 63;
            pk.OT_Intensity = 6;
            pk.OT_Feeling = Memories.GetRandomFeeling(pk.OT_Memory);
        }
    }
}