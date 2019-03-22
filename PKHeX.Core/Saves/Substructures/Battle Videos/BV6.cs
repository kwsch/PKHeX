using System;
using System.Linq;
using System.Text;

namespace PKHeX.Core
{
    public class BV6 : BattleVideo
    {
        internal const int SIZE = 0x2E60;

        internal new static bool IsValid(byte[] data)
        {
            if (data.Length != SIZE)
                return false;
            return BitConverter.ToUInt64(data, 0xE18) != 0 && BitConverter.ToUInt16(data, 0xE12) == 0;
        }

        public BV6(byte[] data)
        {
            Data = (byte[])data.Clone();
        }

        private readonly byte[] Data;

        public int Mode { get => Data[0x00]; set => Data[0x00] = (byte)value; }

        public int Style { get => Data[0x01]; set => Data[0x01] = (byte)value; }

        public string Debug1
        {
            get => Util.TrimFromZero(Encoding.Unicode.GetString(Data, 0x6, 24));
            set => Encoding.Unicode.GetBytes(value.PadRight(12, '\0')).CopyTo(Data, 0x6);
        }

        public string Debug2
        {
            get => Util.TrimFromZero(Encoding.Unicode.GetString(Data, 0x50, 24));
            set => Encoding.Unicode.GetBytes(value.PadRight(12, '\0')).CopyTo(Data, 0x50);
        }

        public ulong RNGConst1 { get => BitConverter.ToUInt64(Data, 0x1A0); set => BitConverter.GetBytes(value).CopyTo(Data, 0x1A0); }
        public ulong RNGConst2 { get => BitConverter.ToUInt64(Data, 0x1A4); set => BitConverter.GetBytes(value).CopyTo(Data, 0x1A4); }
        public ulong RNGSeed1 { get => BitConverter.ToUInt64(Data, 0x1A8); set => BitConverter.GetBytes(value).CopyTo(Data, 0x1A8); }
        public ulong RNGSeed2 { get => BitConverter.ToUInt64(Data, 0x1B0); set => BitConverter.GetBytes(value).CopyTo(Data, 0x1B0); }

        public int Background { get => BitConverter.ToInt32(Data, 0x1BC); set => BitConverter.GetBytes(value).CopyTo(Data, 0x1BC); }
        public int _1CE { get => BitConverter.ToUInt16(Data, 0x1CE); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x1CE); }
        public int IntroID { get => BitConverter.ToUInt16(Data, 0x1E4); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x1E4); }
        public int MusicID { get => BitConverter.ToUInt16(Data, 0x1F0); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x1F0); }

        public override PKM[] BattlePKMs => PlayerTeams.SelectMany(t => t).ToArray();
        public override int Generation => 6;

        private const string NPC = "NPC";

        public string[] PlayerNames
        {
            get
            {
                string[] trainers = new string[4];
                for (int i = 0; i < 4; i++)
                {
                    trainers[i] = Util.TrimFromZero(Encoding.Unicode.GetString(Data, 0xEC + (0x1A * i), 0x1A));
                    if (string.IsNullOrWhiteSpace(trainers[i]))
                        trainers[i] = NPC;
                }
                return trainers;
            }
            set
            {
                if (value?.Length != 4)
                    return;

                for (int i = 0; i < 4; i++)
                {
                    string tr = value[i] == NPC ? string.Empty : value[i];
                    Encoding.Unicode.GetBytes(tr.PadRight(0x1A/2)).CopyTo(Data, 0xEC + (0x1A * i));
                }
            }
        }

        public PKM[][] PlayerTeams
        {
            get
            {
                var Teams = new PKM[4][];
                const int start = 0xE18;
                for (int t = 0; t < 4; t++)
                {
                    Teams[t] = new PKM[6];
                    for (int p = 0; p < 6; p++)
                    {
                        int offset = start + (PKX.SIZE_6PARTY*((t * 6) + p));
                        offset += 8*(((t * 6) + p)/6); // 8 bytes padding between teams
                        Teams[t][p] = new PK6(Data.Skip(offset).Take(PKX.SIZE_6PARTY).ToArray()) {Identifier = $"Team {t}, Slot {p}"};
                    }
                }
                return Teams;
            }
            set
            {
                var Teams = value;
                const int start = 0xE18;
                for (int t = 0; t < 4; t++)
                {
                    for (int p = 0; p < 6; p++)
                    {
                        int offset = start + (PKX.SIZE_6PARTY*((t * 6) + p));
                        offset += 8*(((t * 6) + p)/6); // 8 bytes padding between teams
                        Teams[t][p].EncryptedPartyData.CopyTo(Data, offset);
                    }
                }
            }
        }

        private int MatchYear { get => BitConverter.ToUInt16(Data, 0x2E50); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x2E50); }
        private int MatchDay { get => Data[0x2E52]; set => Data[0x2E52] = (byte)value; }
        private int MatchMonth { get => Data[0x2E53]; set => Data[0x2E53] = (byte)value; }
        private int MatchHour { get => Data[0x2E54]; set => Data[0x2E54] = (byte)value; }
        private int MatchMinute { get => Data[0x2E55]; set => Data[0x2E55] = (byte)value; }
        private int MatchSecond { get => Data[0x2E56]; set => Data[0x2E56] = (byte)value; }
        private int MatchFlags { get => Data[0x2E57]; set => Data[0x2E57] = (byte)value; }

        private int UploadYear { get => BitConverter.ToUInt16(Data, 0x2E58); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x2E58); }
        private int UploadDay { get => Data[0x2E5A]; set => Data[0x2E5A] = (byte)value; }
        private int UploadMonth { get => Data[0x2E5B]; set => Data[0x2E5B] = (byte)value; }
        private int UploadHour { get => Data[0x2E5C]; set => Data[0x2E5C] = (byte)value; }
        private int UploadMinute { get => Data[0x2E5D]; set => Data[0x2E5D] = (byte)value; }
        private int UploadSecond { get => Data[0x2E5E]; set => Data[0x2E5E] = (byte)value; }
        private int UploadFlags { get => Data[0x2E5F]; set => Data[0x2E5F] = (byte)value; }

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
                    MatchYear = MatchDay = MatchMonth = MatchHour = MatchMinute = MatchSecond = MatchFlags = 0;
                }
            }
        }

        public DateTime? UploadStamp
        {
            get
            {
                if (!Util.IsDateValid(UploadYear, UploadMonth, UploadDay))
                    return null;
                return new DateTime(UploadYear, UploadMonth, UploadDay, UploadHour, UploadMinute, UploadSecond);
            }
            set
            {
                if (value.HasValue)
                {
                    UploadYear = value.Value.Year;
                    UploadDay = value.Value.Day;
                    UploadMonth = value.Value.Month;
                    UploadHour = value.Value.Hour;
                    UploadMinute = value.Value.Minute;
                    UploadSecond = value.Value.Second;
                }
                else
                {
                    UploadYear = UploadDay = UploadMonth = UploadHour = UploadMinute = UploadSecond = UploadFlags = 0;
                }
            }
        }

        // Battle Instruction Parsing
        private static readonly string[] Action = { "0", "Fight", "2", "Switch", "Run", "5", "Rotate", "7", "MegaEvolve" };

        private static readonly string[] Target =
        {
            "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "Opposite Enemy", "11", "12", "13",
            "All except User", "Everyone"
        };

        private static readonly string[] Rotate = { "0", "Right", "Left", "3" };

        public static readonly string[] BVmode =
        {
            "Link", "Maison", "Super Maison", "Battle Spot - Free", "Battle Spot - Rating",
            "Battle Spot - Special", "UNUSED", "JP-1", "JP-2", "BROKEN",
        };

        public static readonly string[] BVstyle = { "Single", "Double", "Triple", "Rotation", "Multi", };
    }
}
