using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace PKHeX.Core
{
    /// <summary>
    /// Encounter Save Data
    /// </summary>
    /// <remarks>size 0x188, struct_name ENC_SV_DATA</remarks>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class EncounterSave8b : SaveBlock
    {
        public const int COUNT_HONEYTREE = 21;
        private const int SIZE_HillBackData = 0x8;
        private const int SIZE_HoneyTree = 8 + 4 + (COUNT_HONEYTREE * HoneyTree8b.SIZE); // 0x108
        private const int SIZE_Roamer = 0x20;
        private const int SIZE_Closing = 10 + 2; // 2 padding alignment

        private const int OFS_HillBackData = 0xC;
        private const int OFS_HoneyTree = OFS_HillBackData + SIZE_HillBackData; // 0x14
        private const int OFS_SWAY = OFS_HoneyTree + SIZE_HoneyTree; // 0x11C
        private const int OFS_ZONEHISTORY = OFS_SWAY + (4 * 6); // 0x134
        private const int OFS_ROAM1 = OFS_ZONEHISTORY + 4 + 4; // 0x13C
        private const int OFS_ROAM2 = OFS_ROAM1 + SIZE_Roamer; // 0x15C
        private const int OFS_CLOSING = OFS_ROAM2 + SIZE_Roamer; // 0x17C
        private const int SIZE = OFS_CLOSING + SIZE_Closing; // 0x188

        public EncounterSave8b(SAV8BS sav, int offset) : base(sav) => Offset = offset;

        public void Clear() => Data.AsSpan(Offset, SIZE).Clear();

        public int EncounterWalkCount
        {
            get => BitConverter.ToInt32(Data, Offset + 0x00);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x00);
        }

        public uint SafariRandSeed
        {
            get => BitConverter.ToUInt32(Data, Offset + 0x04);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x04);
        }

        public uint GenerateRandSeed
        {
            get => BitConverter.ToUInt32(Data, Offset + 0x08);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x08);
        }

        // HILL_BACK_DATA
        public bool HillTalkFlag
        {
            get => BitConverter.ToUInt32(Data, Offset + OFS_HillBackData + 0x00) == 1;
            set => BitConverter.GetBytes(value ? 1u : 0).CopyTo(Data, Offset + OFS_HillBackData + 0x00);
        }
        public ushort HillEncTblIdx1
        {
            get => BitConverter.ToUInt16(Data, Offset + OFS_HillBackData + 0x04);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_HillBackData + 0x04);
        }
        public ushort HillEncTblIdx2
        {
            get => BitConverter.ToUInt16(Data, Offset + OFS_HillBackData + 0x06);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_HillBackData + 0x06);
        }

        // HONEY_TREE
        public long HoneyLastUpdateMinutes
        {
            get => BitConverter.ToInt64(Data, Offset + OFS_HoneyTree + 0x00);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_HoneyTree + 0x00);
        }
        public byte HoneyTreeNo
        {
            get => Data[Offset + OFS_HoneyTree + 0x08];
            set => Data[Offset + OFS_HoneyTree + 0x08] = value;
        }

#pragma warning disable CA1819 // Properties should not return arrays
        public HoneyTree8b[] HoneyTrees
        {
            get => GetTrees();
            set => SetTrees(value);
        }
#pragma warning restore CA1819 // Properties should not return arrays

        private HoneyTree8b[] GetTrees()
        {
            var result = new HoneyTree8b[COUNT_HONEYTREE];
            for (int i = 0; i < result.Length; i++)
                result[i] = new HoneyTree8b(Data, Offset + OFS_HoneyTree + 0xC + (i * HoneyTree8b.SIZE));
            return result;
        }

        private static void SetTrees(IReadOnlyList<HoneyTree8b> value)
        {
            if (value.Count != COUNT_HONEYTREE)
                throw new ArgumentOutOfRangeException(nameof(value.Count));
            // data is already hard-referencing the original byte array. This is mostly a hack for Property Grid displays.
        }

        public uint Radar1Species { get => BitConverter.ToUInt32(Data, Offset + OFS_SWAY + 0x00); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_SWAY + 0x00); }
        public uint Radar1Chain   { get => BitConverter.ToUInt32(Data, Offset + OFS_SWAY + 0x04); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_SWAY + 0x04); }
        public uint Radar2Species { get => BitConverter.ToUInt32(Data, Offset + OFS_SWAY + 0x08); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_SWAY + 0x08); }
        public uint Radar2Chain   { get => BitConverter.ToUInt32(Data, Offset + OFS_SWAY + 0x0C); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_SWAY + 0x0C); }
        public uint Radar3Species { get => BitConverter.ToUInt32(Data, Offset + OFS_SWAY + 0x10); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_SWAY + 0x10); }
        public uint Radar3Chain   { get => BitConverter.ToUInt32(Data, Offset + OFS_SWAY + 0x14); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_SWAY + 0x14); }

        public int BeforeZone { get => BitConverter.ToInt32(Data, Offset + OFS_ZONEHISTORY + 0x00); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_ZONEHISTORY + 0x00); }
        public int OldZone    { get => BitConverter.ToInt32(Data, Offset + OFS_ZONEHISTORY + 0x04); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_ZONEHISTORY + 0x04); }

        // Mesprit
        public int   Roamer1ZoneID  { get => BitConverter.ToInt32 (Data, Offset + OFS_ROAM1 + 0x00); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_ROAM1 + 0x00); }
        public ulong Roamer1Seed    { get => BitConverter.ToUInt64(Data, Offset + OFS_ROAM1 + 0x04); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_ROAM1 + 0x04); }
        public uint  Roamer1Species { get => BitConverter.ToUInt32(Data, Offset + OFS_ROAM1 + 0x0C); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_ROAM1 + 0x0C); }
        public uint  Roamer1HP      { get => BitConverter.ToUInt32(Data, Offset + OFS_ROAM1 + 0x10); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_ROAM1 + 0x10); }
        public byte  Roamer1Level   { get => Data[Offset + OFS_ROAM1 + 0x14]; set => Data[Offset + OFS_ROAM1 + 0x14] = value; }
        public uint  Roamer1Status  { get => BitConverter.ToUInt32(Data, Offset + OFS_ROAM1 + 0x18); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_ROAM1 + 0x18); }
        public byte  Roamer1Encount { get => Data[Offset + OFS_ROAM1 + 0x1C]; set => Data[Offset + OFS_ROAM1 + 0x1C] = value; }

        // Cresselia
        public int   Roamer2ZoneID  { get => BitConverter.ToInt32 (Data, Offset + OFS_ROAM2 + 0x00); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_ROAM2 + 0x00); }
        public ulong Roamer2Seed    { get => BitConverter.ToUInt64(Data, Offset + OFS_ROAM2 + 0x04); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_ROAM2 + 0x04); }
        public uint  Roamer2Species { get => BitConverter.ToUInt32(Data, Offset + OFS_ROAM2 + 0x0C); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_ROAM2 + 0x0C); }
        public uint  Roamer2HP      { get => BitConverter.ToUInt32(Data, Offset + OFS_ROAM2 + 0x10); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_ROAM2 + 0x10); }
        public byte  Roamer2Level   { get => Data[Offset + OFS_ROAM2 + 0x14]; set => Data[Offset + OFS_ROAM2 + 0x14] = value; }
        public uint  Roamer2Status  { get => BitConverter.ToUInt32(Data, Offset + OFS_ROAM2 + 0x18); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_ROAM2 + 0x18); }
        public byte  Roamer2Encount { get => Data[Offset + OFS_ROAM2 + 0x1C]; set => Data[Offset + OFS_ROAM2 + 0x1C] = value; }

        public bool GenerateValid
        {
            get => BitConverter.ToUInt32(Data, Offset + OFS_CLOSING + 0) == 1;
            set => BitConverter.GetBytes(value ? 1u : 0).CopyTo(Data, Offset + OFS_CLOSING + 0);
        }
        public short SprayCount
        {
            get => BitConverter.ToInt16(Data, Offset + OFS_CLOSING + 4);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_CLOSING + 4);
        }
        public byte SprayType       { get => Data[Offset + OFS_CLOSING + 6]; set => Data[Offset + OFS_CLOSING + 6] = value; }
        public byte VsSeekerCharge  { get => Data[Offset + OFS_CLOSING + 7]; set => Data[Offset + OFS_CLOSING + 7] = value; } // max 100
        public byte PokeRadarCharge { get => Data[Offset + OFS_CLOSING + 8]; set => Data[Offset + OFS_CLOSING + 8] = value; } // max 50
        public byte FluteType       { get => Data[Offset + OFS_CLOSING + 9]; set => Data[Offset + OFS_CLOSING + 9] = value; } // vidro
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class HoneyTree8b
    {
        public const int SIZE = 0xC;

        private readonly int Offset;
        private readonly byte[] Data;

        public HoneyTree8b(byte[] data, int offset)
        {
            Data = data;
            Offset = offset;
        }

        public bool Spreaded
        {
            get => BitConverter.ToUInt32(Data, Offset + 0x00) == 1;
            set => BitConverter.GetBytes(value ? 1u : 0).CopyTo(Data, Offset + 0x00);
        }
        public int Minutes
        {
            get => BitConverter.ToInt32(Data, Offset + 0x04);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x04);
        }
        public byte TblMonsNo
        {
            get => Data[Offset + 0x08];
            set => Data[Offset + 0x08] = value;
        }
        public byte RareLv
        {
            get => Data[Offset + 0x09];
            set => Data[Offset + 0x09] = value;
        }
        public byte SwayLv
        {
            get => Data[Offset + 0x0A];
            set => Data[Offset + 0x0A] = value;
        }
        // 0xB alignment
    }
}
