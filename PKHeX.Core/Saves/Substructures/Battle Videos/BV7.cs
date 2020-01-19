using System;
using System.Linq;
using System.Text;

namespace PKHeX.Core
{
    public sealed class BV7 : BattleVideo
    {
        internal const int SIZE = 0x2BC0;

        internal new static bool IsValid(byte[] data)
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
                        int offset = offsets[t] + (PokeCrypto.SIZE_6PARTY * p);
                        Teams[t][p] = new PK7(Data.Slice(offset, PokeCrypto.SIZE_6STORED)) {Identifier = $"Team {t}, Slot {p}"};
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
                        int offset = offsets[t] + (PokeCrypto.SIZE_6PARTY * p);
                        Teams[t][p].EncryptedPartyData.CopyTo(Data, offset);
                    }
                }
            }
        }

        private const string NPC = "NPC";

        public string[] PlayerNames
        {
            get
            {
                string[] trainers = new string[4];
                for (int i = 0; i < 4; i++)
                {
                    trainers[i] = Util.TrimFromZero(Encoding.Unicode.GetString(Data, 0x12C + (0x1A * i), 0x1A));
                    if (string.IsNullOrWhiteSpace(trainers[i]))
                        trainers[i] = NPC;
                }
                return trainers;
            }
            set
            {
                if (value.Length != 4)
                    return;

                for (int i = 0; i < 4; i++)
                {
                    string tr = value[i] == NPC ? string.Empty : value[i];
                    Encoding.Unicode.GetBytes(tr.PadRight(0x1A / 2)).CopyTo(Data, 0xEC + (0x1A * i));
                }
            }
        }

        private int MatchYear { get => BitConverter.ToUInt16(Data, 0x2BB0); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x2BB0); }
        private int MatchDay { get => Data[0x2BB3]; set => Data[0x2BB3] = (byte)value; }
        private int MatchMonth { get => Data[0x2BB2]; set => Data[0x2BB2] = (byte)value; }
        private int MatchHour { get => Data[0x2BB4]; set => Data[0x2BB4] = (byte)value; }
        private int MatchMinute { get => Data[0x2BB5]; set => Data[0x2BB5] = (byte)value; }
        private int MatchSecond { get => Data[0x2BB6]; set => Data[0x2BB6] = (byte)value; }

        public DateTime? MatchStamp
        {
            get
            {
                if (!Util.IsDateValid(MatchYear, MatchMonth, MatchDay))
                    return null;
                return new DateTime(MatchYear, MatchMonth, MatchDay, MatchHour, MatchMinute, MatchSecond);
            }
            set
            {
                if (value.HasValue)
                {
                    MatchYear = value.Value.Year;
                    MatchDay = value.Value.Day;
                    MatchMonth = value.Value.Month;
                    MatchHour = value.Value.Hour;
                    MatchMinute = value.Value.Minute;
                    MatchSecond = value.Value.Second;
                }
                else
                {
                    MatchYear = MatchDay = MatchMonth = MatchHour = MatchMinute = MatchSecond = 0;
                }
            }
        }

        public int MusicID { get => Data[0x21C]; set => Data[0x21C] = (byte)value; }
        public bool SilentBGM { get => MusicID == 0xFF; set => MusicID = (byte)(value ? 0xFF : MusicID); }
    }
}
