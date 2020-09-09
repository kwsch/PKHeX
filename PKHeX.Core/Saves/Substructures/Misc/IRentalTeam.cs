using System.Collections.Generic;

namespace PKHeX.Core
{
    public interface IRentalTeam<T> where T : PKM
    {
        T GetSlot(int slot);
        void SetSlot(int slot, T pkm);

        T[] GetTeam();
        void SetTeam(IReadOnlyList<T> team);
    }
}
