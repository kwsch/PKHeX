using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace PKHeX.Core
{
    /// <summary>
    /// Stores progress within the Battle Tower for the four battle modes.
    /// </summary>
    /// <remarks>size: 0x1B8</remarks>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class BattleTowerWork8b : SaveBlock
    {
        private const int OFS_ClassData = 20;
        private const int COUNT_CLASSDATA = 4;

        public BattleTowerWork8b(SAV8BS sav, int offset) : base(sav) => Offset = offset;

        // Structure:
        // uint max_master_rank;
        // int play_mode;
        // int old_playmode;
        // uint btl_point;
        // uint day_challeng_cnt;
        // BTLTOWER_CLASSDATA[4] class_data;
        // uint challenge_cnt;
        public int MasterRankMax { get => BitConverter.ToInt32(Data, Offset + 0x0); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x0); } // max_master_rank
        public int PlayMode      { get => BitConverter.ToInt32(Data, Offset + 0x4); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x4); }// play_mode
        public int PlayModeOld   { get => BitConverter.ToInt32(Data, Offset + 0x8); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x8); } // old_playmode
        public uint BP           { get => BitConverter.ToUInt32(Data, Offset + 0xC); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0xC); } // btl_point

        public uint ChallengeCount { get => BitConverter.ToUInt32(Data, Offset + 0x1B4); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x1B4); } // challenge_cnt

        public BattleTowerClassData8b[] Records
        {
            get => GetRecords();
            set => SetRecords(value);
        }

        private BattleTowerClassData8b[] GetRecords()
        {
            var result = new BattleTowerClassData8b[COUNT_CLASSDATA];
            for (int i = 0; i < result.Length; i++)
                result[i] = new BattleTowerClassData8b(Data, Offset + OFS_ClassData + (i * BattleTowerClassData8b.SIZE));
            return result;
        }

        private static void SetRecords(IReadOnlyList<BattleTowerClassData8b> value)
        {
            if (value.Count != COUNT_CLASSDATA)
                throw new ArgumentOutOfRangeException(nameof(value.Count));
            // data is already hard-referencing the original byte array. This is mostly a hack for Property Grid displays.
        }
    }


    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class BattleTowerClassData8b
    {
        public const int SIZE = 0x68;

        private readonly int Offset;
        private readonly byte[] Data;

        public BattleTowerClassData8b(byte[] data, int offset)
        {
            Data = data;
            Offset = offset;
        }

        public override string ToString() => $"Rank: {Rank}, Streak: {RenshouCount} (Max {RenshouCountOld}), Wins: {TotalWins}|{TotalWinsLoop}|{TotalWinsLose}";

        public byte Cleared
        {
            get => Data[Offset + 0x00];
            set => Data[Offset]= value;
        }
        public bool Suspended
        {
            get => BitConverter.ToInt32(Data, Offset + 0x04) == 1;
            set => BitConverter.GetBytes(value ? 1u : 0u).CopyTo(Data, Offset + 0x04);
        }
        public ulong BattlePlaySeed  { get => BitConverter.ToUInt64(Data, Offset + 0x08); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x08); }
        public uint Rank             { get => BitConverter.ToUInt32(Data, Offset + 0x10); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x10); }
        public uint RankDownLose     { get => BitConverter.ToUInt32(Data, Offset + 0x14); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x14); }

        public ulong TrainerSeed1 { get => BitConverter.ToUInt64(Data, Offset + 0x1C); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x1C); }
        public ulong TrainerSeed2 { get => BitConverter.ToUInt64(Data, Offset + 0x24); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x24); }
        public ulong TrainerSeed3 { get => BitConverter.ToUInt64(Data, Offset + 0x2C); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x2C); }
        public ulong TrainerSeed4 { get => BitConverter.ToUInt64(Data, Offset + 0x34); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x34); }
        public ulong TrainerSeed5 { get => BitConverter.ToUInt64(Data, Offset + 0x3C); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x3C); }
        public ulong TrainerSeed6 { get => BitConverter.ToUInt64(Data, Offset + 0x44); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x44); }
        public ulong TrainerSeed7 { get => BitConverter.ToUInt64(Data, Offset + 0x4C); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x4C); }

        public uint TotalWins        { get => BitConverter.ToUInt32(Data, Offset + 0x54); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x54); }
        public uint TotalWinsLoop    { get => BitConverter.ToUInt32(Data, Offset + 0x58); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x58); }
        public uint TotalWinsLose    { get => BitConverter.ToUInt32(Data, Offset + 0x5C); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x5C); }
        public uint RenshouCountOld  { get => BitConverter.ToUInt32(Data, Offset + 0x60); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x60); }
        public uint RenshouCount     { get => BitConverter.ToUInt32(Data, Offset + 0x64); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x64); }
    }
}
