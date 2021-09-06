using System.Collections.Generic;

namespace PKHeX.Core
{
    public abstract class MemoryContext
    {
        public abstract IEnumerable<ushort> GetMemoryItemParams();
        public abstract IEnumerable<ushort> GetKeyItemParams();

        public abstract bool CanUseItemGeneric(int item);
        public abstract bool IsUsedKeyItemUnspecific(int item);
        public abstract bool IsUsedKeyItemSpecific(int item, int species);
        public abstract bool CanBuyItem(int item, GameVersion version);
        public abstract bool CanWinLottoID(int item);
        public virtual bool CanPlantBerry(int item) => false;
        public abstract bool CanHoldItem(int item);

        public abstract bool CanObtainMemory(int memory);
        public abstract bool CanObtainMemoryOT(GameVersion pkmVersion, int memory);
        public abstract bool CanObtainMemoryHT(GameVersion pkmVersion, int memory);

        public abstract bool HasPokeCenter(GameVersion version, int location);
        public abstract bool IsInvalidGeneralLocationMemoryValue(int memory, int variable, IEncounterTemplate enc, PKM pk);
        public abstract bool IsInvalidMiscMemory(int memory, int variable);

        public abstract bool CanHaveIntensity(int memory, int intensity);
        public abstract bool CanHaveFeeling(int memory, int feeling, int argument);
        public abstract int GetMinimumIntensity(int memory);
    }
}
