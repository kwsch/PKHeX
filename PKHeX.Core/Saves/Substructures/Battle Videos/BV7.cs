using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    public sealed class BV7 : BattleVideo
    {
        internal const int SIZE = 0x2BC0;
        private const string NPC = "NPC";
        private const int PlayerCount = 4;

        public override int Generation => 7;
        private readonly byte[] Data;

        public override IReadOnlyList<PKM> BattlePKMs => PlayerTeams.SelectMany(t => t).ToArray();
        internal new static bool IsValid(byte[] data) => data.Length == SIZE;

        public BV7(byte[] data) => Data = (byte[])data.Clone();

        private static readonly int[] offsets = { 0xE41, 0x145E, 0x1A7B, 0x2098 };

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
                for (int t = 0; t < PlayerCount; t++)
                    SetTeam(value[t], t);
            }
        }

        public PKM[] GetTeam(int teamIndex)
        {
            var team = new PKM[6];
            var ofs = offsets[teamIndex];
            for (int p = 0; p < 6; p++)
            {
                int offset = ofs + (PokeCrypto.SIZE_6PARTY * p);
                team[p] = new PK7(Data.Slice(offset, PokeCrypto.SIZE_6STORED)) { Identifier = $"Team {teamIndex}, Slot {p}" };
            }

            return team;
        }

        public void SetTeam(IReadOnlyList<PKM> team, int teamIndex)
        {
            var ofs = offsets[teamIndex];
            for (int p = 0; p < 6; p++)
            {
                int offset = ofs + (PokeCrypto.SIZE_6PARTY * p);
                team[p].EncryptedPartyData.CopyTo(Data, offset);
            }
        }

        public string[] GetPlayerNames()
        {
            string[] trainers = new string[PlayerCount];
            for (int i = 0; i < PlayerCount; i++)
            {
                var str = StringConverter.GetString7(Data, 0x12C + (0x1A * i), 0x1A);
                trainers[i] = string.IsNullOrWhiteSpace(trainers[i]) ? NPC : str;
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
                StringConverter.SetString7(tr, 12, 13).CopyTo(Data, 0xEC + (0x1A * i));
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
                    MatchYear = MatchDay = MatchMonth = MatchHour = MatchMinute = MatchSecond = 0;
                }
            }
        }

        public int MusicID { get => Data[0x21C]; set => Data[0x21C] = (byte)value; }
        public bool SilentBGM { get => MusicID == 0xFF; set => MusicID = (byte)(value ? 0xFF : MusicID); }
    }
}
