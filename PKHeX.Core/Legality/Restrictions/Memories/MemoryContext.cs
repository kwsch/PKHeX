using System.Collections.Generic;

namespace PKHeX.Core;

public abstract class MemoryContext
{
    public abstract EntityContext Context { get; }
    public abstract IEnumerable<ushort> GetMemoryItemParams();

    public abstract bool CanUseItemGeneric(int item);
    public abstract bool IsUsedKeyItemUnspecific(int item);
    public abstract bool IsUsedKeyItemSpecific(int item, ushort species);
    public abstract bool CanBuyItem(int item, GameVersion version);
    public abstract bool CanWinLotoID(int item);
    public virtual bool CanPlantBerry(int item) => false;
    public abstract bool CanHoldItem(int item);

    public abstract bool CanObtainMemory(byte memory);
    public abstract bool CanObtainMemoryOT(GameVersion pkmVersion, byte memory);
    public abstract bool CanObtainMemoryHT(GameVersion pkmVersion, byte memory);

    public abstract bool HasPokeCenter(GameVersion version, ushort location);
    public abstract bool IsInvalidGeneralLocationMemoryValue(byte memory, ushort variable, IEncounterTemplate enc, PKM pk);
    public abstract bool IsInvalidMiscMemory(byte memory, ushort variable);

    public abstract bool CanHaveIntensity(byte memory, byte intensity);
    public abstract bool CanHaveFeeling(byte memory, byte feeling, ushort argument);
    public abstract int GetMinimumIntensity(byte memory);
}
