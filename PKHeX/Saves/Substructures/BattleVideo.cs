namespace PKHeX
{
    public abstract class BattleVideo
    {
        public abstract PKM[] BattlePKMs { get; }
        public abstract int Generation { get; }

        public static BattleVideo getVariantBattleVideo(byte[] data)
        {
            if (BV6.getIsValid(data))
                return new BV6(data);
            if (BV7.getIsValid(data))
                return new BV7(data);
            return null;
        }

        public static bool getIsValid(byte[] data)
        {
            if (BV6.getIsValid(data))
                return true;
            if (BV7.getIsValid(data))
                return true;
            return false;
        }
    }
}
