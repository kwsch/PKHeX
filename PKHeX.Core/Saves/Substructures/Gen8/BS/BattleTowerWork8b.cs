using System;
using System.ComponentModel;

namespace PKHeX.Core
{
    /// <summary>
    /// Stores progress within the Battle Tower for the four battle modes.
    /// </summary>
    /// <remarks>size: ???</remarks>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class BattleTowerWork8b : SaveBlock
    {
        public BattleTowerWork8b(SAV8BS sav, int offset) : base(sav) => Offset = offset;

        // Structure:
        // uint max_master_rank;
        // int play_mode;
        // int old_playmode;
        // uint btl_point;
        // uint day_challeng_cnt;
        // BTLTOWER_CLASSDATA[] class_data; // 0x18
        // uint challenge_cnt;
        public int MasterRankMax { get => BitConverter.ToInt32(Data, Offset + 0x0); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x0); } // max_master_rank
        public int PlayMode      { get => BitConverter.ToInt32(Data, Offset + 0x4); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x4); }// play_mode
        public int PlayModeOld   { get => BitConverter.ToInt32(Data, Offset + 0x8); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x8); } // old_playmode
        public uint BP           { get => BitConverter.ToUInt32(Data, Offset + 0xC); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0xC); } // btl_point

        // class_data appears to be an array of 4, sized 0x68 each. There's 2 fields at the end instead of 2, dunno.
    }
}
