using System.Linq;

namespace PKHeX
{
    public class BV7 : BattleVideo
    {
        internal const int SIZE = 0x2BC0;
        internal new static bool getIsValid(byte[] data)
        {
            return data.Length == SIZE;
        }

        public BV7(byte[] data)
        {
            Data = (byte[])data.Clone();
        }

        private readonly byte[] Data;
        public override PKM[] BattlePKMs => PlayerTeams.SelectMany(t => t).ToArray();
        public override int Generation => 7;

        private PKM[][] PlayerTeams
        {
            get
            {
                var Teams = new PKM[4][];
                int[] offsets = {0xE41, 0x145E, 0x1A7B, 0x2098};
                for (int t = 0; t < 4; t++)
                {
                    Teams[t] = new PKM[6];
                    for (int p = 0; p < 6; p++)
                    {
                        int offset = offsets[t] + PKX.SIZE_6PARTY * p;
                        Teams[t][p] = new PK7(Data.Skip(offset).Take(PKX.SIZE_6STORED).ToArray(), $"Team {t}, Slot {p}");
                    }
                }
                return Teams;
            }
            set
            {
                var Teams = value;
                int[] offsets = { 0xE41, 0x145E, 0x1A7B, 0x2098 };
                for (int t = 0; t < 4; t++)
                {
                    for (int p = 0; p < 6; p++)
                    {
                        int offset = offsets[t] + PKX.SIZE_6PARTY * p;
                        Teams[t][p].EncryptedPartyData.CopyTo(Data, offset);
                    }
                }
            }
        }
    }
}
