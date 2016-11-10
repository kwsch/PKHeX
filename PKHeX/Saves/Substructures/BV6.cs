using System;
using System.Linq;
using System.Text;

namespace PKHeX
{
    public class BV6 : BattleVideo
    {
        internal const int SIZE = 0x2E60;
        internal new static bool getIsValid(byte[] data)
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

        private int Mode { get { return Data[0x00]; } set { Data[0x00] = (byte)value; } }
        private string[] BVmode =
        {
            "Link", "Maison", "Super Maison", "Battle Spot - Free", "Battle Spot - Rating",
            "Battle Spot - Special", "UNUSED", "JP-1", "JP-2", "BROKEN",
        };
        private int Style { get { return Data[0x01]; } set { Data[0x01] = (byte)value; } }
        private string[] BVstyle = { "Single", "Double", "Triple", "Rotation", "Multi", };
        private string Debug1
        {
            get { return Util.TrimFromZero(Encoding.Unicode.GetString(Data, 0x6, 24)); }
            set { Encoding.Unicode.GetBytes(value.PadRight(12, '\0')).CopyTo(Data, 0x6); }
        }
        private string Debug2
        {
            get { return Util.TrimFromZero(Encoding.Unicode.GetString(Data, 0x50, 24)); }
            set { Encoding.Unicode.GetBytes(value.PadRight(12, '\0')).CopyTo(Data, 0x50); }
        }
        private ulong RNGConst1 { get { return BitConverter.ToUInt64(Data, 0x1A0); } set { BitConverter.GetBytes(value).CopyTo(Data, 0x1A0); } }
        private ulong RNGConst2 { get { return BitConverter.ToUInt64(Data, 0x1A4); } set { BitConverter.GetBytes(value).CopyTo(Data, 0x1A4); } }
        private ulong RNGSeed1 { get { return BitConverter.ToUInt64(Data, 0x1A8); } set { BitConverter.GetBytes(value).CopyTo(Data, 0x1A8); } }
        private ulong RNGSeed2 { get { return BitConverter.ToUInt64(Data, 0x1B0); } set { BitConverter.GetBytes(value).CopyTo(Data, 0x1B0); } }

        private int Background { get { return BitConverter.ToInt32(Data, 0x1BC); } set { BitConverter.GetBytes(value).CopyTo(Data, 0x1BC); } }
        private int _1CE { get { return BitConverter.ToUInt16(Data, 0x1CE); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x1CE); } }
        private int IntroID { get { return BitConverter.ToUInt16(Data, 0x1E4); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x1E4); } }
        private int MusicID { get { return BitConverter.ToUInt16(Data, 0x1F0); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x1F0); } }


        public override PKM[] BattlePKMs => PlayerTeams.SelectMany(t => t).ToArray();
        public override int Generation => 6;

        private const string NPC = "NPC";
        private string[] PlayerNames
        {
            get
            {
                string[] trainers = new string[4];
                for (int i = 0; i < 4; i++)
                {
                    trainers[i] = Util.TrimFromZero(Encoding.Unicode.GetString(Data, 0xEC + 0x1A*i, 0x1A));
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
                    string tr = value[i] == NPC ? "" : value[i];
                    Encoding.Unicode.GetBytes(tr.PadRight(0x1A/2)).CopyTo(Data, 0xEC + 0x1A*i);
                }
            }
        }
        private PKM[][] PlayerTeams
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
                        int offset = start + PKX.SIZE_6PARTY*(t*6 + p);
                        offset += 8*((t*6 + p)/6); // 8 bytes padding between teams
                        Teams[t][p] = new PK6(Data.Skip(offset).Take(PKX.SIZE_6PARTY).ToArray(), $"Team {t}, Slot {p}");
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
                        int offset = start + PKX.SIZE_6PARTY*(t*6 + p);
                        offset += 8*((t*6 + p)/6); // 8 bytes padding between teams
                        Teams[t][p].EncryptedPartyData.CopyTo(Data, offset);
                    }
                }
            }
        }

        private int MatchYear { get { return BitConverter.ToUInt16(Data, 0x2E50); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x2E50); } }
        private int MatchDay { get { return Data[0x2E52]; } set { Data[0x2E52] = (byte)value; } }
        private int MatchMonth { get { return Data[0x2E53]; } set { Data[0x2E53] = (byte)value; } }
        private int MatchHour { get { return Data[0x2E54]; } set { Data[0x2E54] = (byte)value; } }
        private int MatchMinute { get { return Data[0x2E55]; } set { Data[0x2E55] = (byte)value; } }
        private int MatchSecond { get { return Data[0x2E56]; } set { Data[0x2E56] = (byte)value; } }
        private int MatchFlags { get { return Data[0x2E57]; } set { Data[0x2E57] = (byte)value; } }
        public DateTime MatchStamp
        {
            get { return new DateTime(MatchYear, MatchDay, MatchMonth, MatchHour, MatchMinute, MatchSecond); }
            set { MatchYear = value.Year; MatchDay = value.Day; MatchMonth = value.Month; MatchHour = value.Hour; MatchMinute = value.Minute; MatchSecond = value.Second; }
        }
        private int UploadYear { get { return BitConverter.ToUInt16(Data, 0x2E58); } set { BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x2E58); } }
        private int UploadDay { get { return Data[0x2E5A]; } set { Data[0x2E5A] = (byte)value; } }
        private int UploadMonth { get { return Data[0x2E5B]; } set { Data[0x2E5B] = (byte)value; } }
        private int UploadHour { get { return Data[0x2E5C]; } set { Data[0x2E5C] = (byte)value; } }
        private int UploadMinute { get { return Data[0x2E5D]; } set { Data[0x2E5D] = (byte)value; } }
        private int UploadSecond { get { return Data[0x2E5E]; } set { Data[0x2E5E] = (byte)value; } }
        private int UploadFlags { get { return Data[0x2E5F]; } set { Data[0x2E5F] = (byte)value; } }
        public DateTime UploadStamp
        {
            get { return new DateTime(UploadYear, UploadDay, UploadMonth, UploadHour, UploadMinute, UploadSecond); }
            set { UploadYear = value.Year; UploadDay = value.Day; UploadMonth = value.Month; UploadHour = value.Hour; UploadMinute = value.Minute; UploadSecond = value.Second; }
        }

        // Battle Instruction Parsing
        private string[] Action = { "0", "Fight", "2", "Switch", "Run", "5", "Rotate", "7", "MegaEvolve" };
        private string[] Target =
        {
            "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "Opposite Enemy", "B", "C", "D",
            "All except User", "Everyone"
        };
        private string[] Rotate = { "0", "Right", "Left", "3" };
    }
}
