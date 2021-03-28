using System.Collections.Generic;

namespace PKHeX.Core
{
    public abstract class BattleVideo : IPokeGroup
    {
        public abstract IReadOnlyList<PKM> BattlePKMs { get; }
        public abstract int Generation { get; }

        public IEnumerable<PKM> Contents => BattlePKMs;

        public static BattleVideo? GetVariantBattleVideo(byte[] data)
        {
            if (BV6.IsValid(data))
                return new BV6(data);
            if (BV7.IsValid(data))
                return new BV7(data);
            if (BV3.IsValid(data))
                return new BV3(data);
            return null;
        }

        public static bool IsValid(byte[] data)
        {
            if (BV6.IsValid(data))
                return true;
            if (BV7.IsValid(data))
                return true;
            if (BV3.IsValid(data))
                return true;
            return false;
        }
    }
}
