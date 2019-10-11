namespace PKHeX.Core
{
    public abstract class BattleVideo
    {
        public abstract PKM[] BattlePKMs { get; }
        public abstract int Generation { get; }

        public static BattleVideo? GetVariantBattleVideo(byte[] data)
        {
            if (BV6.IsValid(data))
                return new BV6(data);
            if (BV7.IsValid(data))
                return new BV7(data);
            return null;
        }

        public static bool IsValid(byte[] data)
        {
            if (BV6.IsValid(data))
                return true;
            if (BV7.IsValid(data))
                return true;
            return false;
        }
    }
}
