namespace PKHeX.Core
{
    public static class WurmpleUtil
    {
        /// <summary>
        /// Gets the Wurmple Evolution Value for a given <see cref="PKM.EncryptionConstant"/>
        /// </summary>
        /// <param name="encryptionConstant">Encryption Constant</param>
        /// <returns>Wurmple Evolution Value</returns>
        public static uint GetWurmpleEvoVal(uint encryptionConstant)
        {
            var evoVal = encryptionConstant >> 16;
            return evoVal % 10 / 5;
        }

        /// <summary>
        /// Gets the evo chain of Wurmple
        /// </summary>
        /// <param name="species">Current species</param>
        /// <returns>-1 if not a Wurmple Evo, 0 if Silcoon chain, 1 if Cascoon chain</returns>
        public static int GetWurmpleEvoGroup(int species)
        {
            int wIndex = species - (int)Species.Silcoon;
            if ((wIndex & 3) != wIndex) // Wurmple evo, [0,3]
                return -1;
            return wIndex >> 1; // Silcoon, Cascoon
        }

        /// <summary>
        /// Gets the Wurmple <see cref="PKM.EncryptionConstant"/> for a given Evolution Value
        /// </summary>
        /// <param name="evoVal">Wurmple Evolution Value</param>
        /// <remarks>0 = Silcoon, 1 = Cascoon</remarks>
        /// <returns>Encryption Constant</returns>
        public static uint GetWurmpleEncryptionConstant(int evoVal)
        {
            uint result;
            do result = Util.Rand32();
            while (evoVal != GetWurmpleEvoVal(result));
            return result;
        }

        /// <summary>
        /// Checks to see if the input <see cref="pkm"/>, with species being that of Wurmple's evo chain, is valid.
        /// </summary>
        /// <param name="pkm">Pokémon data</param>
        /// <returns>True if valid, false if invalid</returns>
        public static bool IsWurmpleEvoValid(PKM pkm)
        {
            uint evoVal = GetWurmpleEvoVal(pkm.EncryptionConstant);
            int wIndex = GetWurmpleEvoGroup(pkm.Species);
            return evoVal == wIndex;
        }
    }
}