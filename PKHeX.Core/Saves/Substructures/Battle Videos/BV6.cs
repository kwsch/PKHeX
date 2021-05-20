using System;
using System.Collections.Generic;
using System.Linq;
// ReSharper disable UnusedType.Local

namespace PKHeX.Core
{
    public sealed class BV6 : BattleVideo
    {
        internal const int SIZE = 0x2E60;
        private const string NPC = "NPC";
        private readonly byte[] Data;
        private const int PlayerCount = 4;

        public override IReadOnlyList<PKM> BattlePKMs => PlayerTeams.SelectMany(t => t).ToArray();
        public override int Generation => 6;

        internal new static bool IsValid(byte[] data)
        {
            if (data.Length != SIZE)
                return false;
            return BitConverter.ToUInt64(data, 0xE18) != 0 && BitConverter.ToUInt16(data, 0xE12) == 0;
        }

        public BV6(byte[] data) => Data = (byte[])data.Clone();
        public int Mode { get => Data[0x00]; set => Data[0x00] = (byte)value; }
        public int Style { get => Data[0x01]; set => Data[0x01] = (byte)value; }

        public string Debug1
        {
            get => StringConverter.GetString6(Data, 0x6, 0x1A);
            set => StringConverter.SetString6(value, 12, 13).CopyTo(Data, 0x6);
        }

        public string Debug2
        {
            get => StringConverter.GetString6(Data, 0x50, 0x1A);
            set => StringConverter.SetString6(value, 12, 13).CopyTo(Data, 0x50);
        }

        public ulong RNGConst1 { get => BitConverter.ToUInt64(Data, 0x1A0); set => BitConverter.GetBytes(value).CopyTo(Data, 0x1A0); }
        public ulong RNGConst2 { get => BitConverter.ToUInt64(Data, 0x1A4); set => BitConverter.GetBytes(value).CopyTo(Data, 0x1A4); }
        public ulong RNGSeed1 { get => BitConverter.ToUInt64(Data, 0x1A8); set => BitConverter.GetBytes(value).CopyTo(Data, 0x1A8); }
        public ulong RNGSeed2 { get => BitConverter.ToUInt64(Data, 0x1B0); set => BitConverter.GetBytes(value).CopyTo(Data, 0x1B0); }

        public int Background { get => BitConverter.ToInt32(Data, 0x1BC); set => BitConverter.GetBytes(value).CopyTo(Data, 0x1BC); }
        public int Unk1CE { get => BitConverter.ToUInt16(Data, 0x1CE); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x1CE); }
        public int IntroID { get => BitConverter.ToUInt16(Data, 0x1E4); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x1E4); }
        public int MusicID { get => BitConverter.ToUInt16(Data, 0x1F0); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x1F0); }

        public string[] GetPlayerNames()
        {
            string[] trainers = new string[PlayerCount];
            for (int i = 0; i < PlayerCount; i++)
            {
                var str = StringConverter.GetString6(Data, 0xEC + (0x1A * i), 0x1A);
                trainers[i] = string.IsNullOrWhiteSpace(str) ? NPC : str;
            }
            return trainers;
        }

        public void SetPlayerNames(IReadOnlyList<string> value)
        {
            if (value.Count != PlayerCount)
                return;

            for (int i = 0; i < PlayerCount; i++)
            {
                string tr = value[i] == NPC ? string.Empty : value[i];
                StringConverter.SetString6(tr, 12, 13).CopyTo(Data, 0xEC + (0x1A * i));
            }
        }

        public IReadOnlyList<PKM[]> PlayerTeams
        {
            get
            {
                var Teams = new PKM[PlayerCount][];
                for (int t = 0; t < PlayerCount; t++)
                    Teams[t] = GetTeam(t);
                return Teams;
            }
            set
            {
                var Teams = value;
                for (int t = 0; t < PlayerCount; t++)
                    SetTeam(Teams[t], t);
            }
        }

        public PKM[] GetTeam(int t)
        {
            var team = new PKM[6];
            const int start = 0xE18;
            for (int p = 0; p < 6; p++)
            {
                int offset = start + (PokeCrypto.SIZE_6PARTY * ((t * 6) + p));
                offset += 8 * (((t * 6) + p) / 6); // 8 bytes padding between teams
                team[p] = new PK6(Data.Slice(offset, PokeCrypto.SIZE_6PARTY)) { Identifier = $"Team {t}, Slot {p}" };
            }

            return team;
        }

        public void SetTeam(IReadOnlyList<PKM> team, int t)
        {
            const int start = 0xE18;
            for (int p = 0; p < 6; p++)
            {
                int offset = start + (PokeCrypto.SIZE_6PARTY * ((t * 6) + p));
                offset += 8 * (((t * 6) + p) / 6); // 8 bytes padding between teams
                team[p].EncryptedPartyData.CopyTo(Data, offset);
            }
        }

        public int MatchYear { get => BitConverter.ToUInt16(Data, 0x2E50); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x2E50); }
        public int MatchDay { get => Data[0x2E52]; set => Data[0x2E52] = (byte)value; }
        public int MatchMonth { get => Data[0x2E53]; set => Data[0x2E53] = (byte)value; }
        public int MatchHour { get => Data[0x2E54]; set => Data[0x2E54] = (byte)value; }
        public int MatchMinute { get => Data[0x2E55]; set => Data[0x2E55] = (byte)value; }
        public int MatchSecond { get => Data[0x2E56]; set => Data[0x2E56] = (byte)value; }
        public int MatchFlags { get => Data[0x2E57]; set => Data[0x2E57] = (byte)value; }

        public int UploadYear { get => BitConverter.ToUInt16(Data, 0x2E58); set => BitConverter.GetBytes((ushort)value).CopyTo(Data, 0x2E58); }
        public int UploadDay { get => Data[0x2E5A]; set => Data[0x2E5A] = (byte)value; }
        public int UploadMonth { get => Data[0x2E5B]; set => Data[0x2E5B] = (byte)value; }
        public int UploadHour { get => Data[0x2E5C]; set => Data[0x2E5C] = (byte)value; }
        public int UploadMinute { get => Data[0x2E5D]; set => Data[0x2E5D] = (byte)value; }
        public int UploadSecond { get => Data[0x2E5E]; set => Data[0x2E5E] = (byte)value; }
        public int UploadFlags { get => Data[0x2E5F]; set => Data[0x2E5F] = (byte)value; }

        public DateTime? MatchStamp
        {
            get
            {
                if (!DateUtil.IsDateValid(MatchYear, MatchMonth, MatchDay))
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
                if (!DateUtil.IsDateValid(UploadYear, UploadMonth, UploadDay))
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

        private enum TurnAction
        {
            None = 0,
            Fight = 1,
            Unk2 = 2,
            Switch = 3,
            Run = 4,
            Unk5 = 5,
            Rotate = 6,
            Unk7 = 7,
            MegaEvolve = 8,
        }

        private enum TurnTarget
        {
            U0 = 0,
            U1 = 1,
            U2 = 2,
            U3 = 3,
            U4 = 4,
            U5 = 5,
            U6 = 6,
            U7 = 7,
            U8 = 8,
            U9 = 9,
            OppositeEnemy,
            U11 = 11,
            U12 = 12,
            U13 = 13,
            AllExceptUser = 14,
            Everyone = 15,
        }

        private enum TurnRotate
        {
            None,
            Right,
            Left,
            Unk3,
        }

        public enum BVType
        {
            Link = 0,
            Maison = 1,
            SuperMaison = 2,
            BattleSpotFree = 3,
            BattleSpotRating = 4,
            BattleSpotSpecial = 5,
            UNUSED = 6,
            JP1 = 7,
            JP2 = 8,
            BROKEN = 9,
        }

        public enum BVStyle
        {
            Single = 0,
            Double = 1,
            Triple = 2,
            Rotation = 3,
            Multi = 4,
        }
    }
}
