using System;
using System.ComponentModel;

namespace PKHeX.Core
{
    /// <summary>
    /// Details about the Player's underground area state.
    /// </summary>
    /// <remarks>size: 0x27A0</remarks>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class UgSaveData8b : SaveBlock
    {
        public const int COUNT_DIGPOINTS = 10;
        public const int COUNT_ENCOUNTERS = 15;
        public const int COUNT_FRIENDS = 50; // 100 total with Friends+Others, not sure if it's evenly split 50/50, or if this is actually correct.
        public const int COUNT_OTHERS = 50;
        public const int COUNT_TRAINERS = 100;

        public const int SIZE_SECRETBASE = 0x14 + (MAX_STONE_STATUE * SIZE_STATUE) + 4; // 0x270
        public const int SIZE_PLAYERLISTENTRY = 0x28;

        // structure:
        // public int ReturnZoneID; // 0
        // int ReturnGridPosX; // 4
        // int ReturnPosY; // 8
        // int ReturnGridPosZ; // C
        // LOCATION_WORK ReturnZenmetsu_Ground; // 0x10-0x23
        // (short X, short Y)[10] DigPoints; // 0x24-0x4B
        // SerializedPokemonFull[15] EncountPokes; // 0x4C-0x1473 (PB8 party 0x158 per entry)
        // Vector3[15] EncountPokePositions; // 0x1474-0x1527 (3 floats, 0xC per entry)
        // int ReturnUgZoneID; // 0x1528

        // UGRecord ugRecord; (secret base, see below documentation), size 0x270
        // UgPlayerInfo[???] FriendPlayerList;
        // UgPlayerInfo[???] OtherPlayerList;
        // byte[100] TalkedNPCsID;

        private const int OFS_DIGPOINT = 0x24;
        private const int OFS_ENCOUNTPOKE = OFS_DIGPOINT + (COUNT_DIGPOINTS * (2 + 2)); // 0x4C
        private const int OFS_ENCOUNTPOS = OFS_ENCOUNTPOKE + (COUNT_ENCOUNTERS * PokeCrypto.SIZE_8PARTY); // 0x1474
        private const int OFS_ReturnUgZoneID = OFS_ENCOUNTPOS + (COUNT_ENCOUNTERS * (4 + 4 + 4)); // 0x1528
        private const int OFS_UgRecord = OFS_ReturnUgZoneID + 4; // 0x152C
        private const int OFS_FRIENDS = OFS_UgRecord + SIZE_SECRETBASE; // 0x179C
        private const int OFS_OTHERS = OFS_FRIENDS + (COUNT_FRIENDS * SIZE_PLAYERLISTENTRY); // 0x1F6C

        private const int OFS_NPC = OFS_OTHERS + (COUNT_OTHERS * SIZE_PLAYERLISTENTRY); // 0x273C
        public const int TOTAL_SIZE = OFS_NPC + COUNT_TRAINERS;

        public const int MAX_STONE_STATUE = 30;
        private const int SIZE_STATUE = 0x14;
        // UGRecord - Secret Base Structure:
        // uint talkPlayerDataID; // 0x0
        // uint talkPlayerCount; // 0x4
        // UgSecretBase myBase; // 0x8
        //   short zoneID; // 0x8
        //   short posX; // 0xA
        //   short posY; // 0xC
        //   byte direction; // 0xE
        //   byte expansionStatus; // 0xF
        //   
        //   int goodCount; // 0x10
        //   UgStoneStatue[30] ugStoneStatue; 0x14
        //      int StatueID, int PedestalID, int X, int Y, int Direction (size: 20 bytes)
        //   bool isEnable; // 0x26C

        public UgSaveData8b(SAV8BS sav, int offset) : base(sav) => Offset = offset;

        public int ReturnZoneID        { get => BitConverter.ToInt32(Data, Offset + 0x0); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x0); }
        public int ReturnGridPositionX { get => BitConverter.ToInt32(Data, Offset + 0x4); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x4); }
        public int ReturnGridPositionY { get => BitConverter.ToInt32(Data, Offset + 0x8); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x8); }
        public int ReturnGridPositionZ { get => BitConverter.ToInt32(Data, Offset + 0xC); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0xC); }

        public int ZenmetsuZoneID      { get => BitConverter.ToInt32 (Data, Offset + 0x10); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x10); }
        public float ZenmetsuPositionX { get => BitConverter.ToSingle(Data, Offset + 0x14); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x14); }
        public float ZenmetsuPositionY { get => BitConverter.ToSingle(Data, Offset + 0x18); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x18); }
        public float ZenmetsuPositionZ { get => BitConverter.ToSingle(Data, Offset + 0x1C); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x1C); }
        public int ZenmetsuDirection   { get => BitConverter.ToInt32 (Data, Offset + 0x20); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x20); }

        private Span<byte> GetDigPoints() => Data.AsSpan(Offset + 0x24, COUNT_DIGPOINTS);
        public void ClearDigPoints() => GetDigPoints().Fill(0xFF);

        public int GetSlotOffset(int slot)
        {
            if ((uint)slot >= COUNT_ENCOUNTERS)
                throw new ArgumentOutOfRangeException(nameof(slot));
            return Offset + OFS_ENCOUNTPOKE + (slot * PokeCrypto.SIZE_8PARTY);
        }

        public  int ReturnUgZoneID   { get => BitConverter.ToInt32 (Data, Offset + OFS_ReturnUgZoneID); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_ReturnUgZoneID); }
        public uint TalkPlayerDataID { get => BitConverter.ToUInt32(Data, Offset + OFS_UgRecord + 0x0); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_UgRecord + 0x0); }
        public uint TalkPlayerCount  { get => BitConverter.ToUInt32(Data, Offset + OFS_UgRecord + 0x4); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + OFS_UgRecord + 0x4); }

        #region Seen NPCs

        public Span<byte> GetTrainers() => Data.AsSpan(Offset + OFS_NPC, COUNT_TRAINERS);

        public void SetTrainers(ReadOnlySpan<byte> data)
        {
            if (Data.Length > COUNT_TRAINERS)
                throw new ArgumentOutOfRangeException(nameof(data.Length));
            data.CopyTo(GetTrainers());
        }

        public byte GetNPCSeen(int index) => Data[Offset + OFS_NPC + index];
        public void SetNPCSeen(int index, byte value) => Data[Offset + OFS_NPC + index] = value;

        public void ClearNPC() => GetTrainers().Fill(0);
        public void ClearNPC(int start, int count = COUNT_TRAINERS) => FillNPC(0, start, count);

        public void FillNPC(byte value, int start = 0, int count = COUNT_TRAINERS)
        {
            if ((uint)start + (uint)count > COUNT_TRAINERS)
                throw new ArgumentOutOfRangeException(nameof(count));
            if ((uint)start > COUNT_TRAINERS)
                throw new ArgumentOutOfRangeException(nameof(start));

            var ofs = Offset + OFS_NPC + start;
            for (int i = 0; i < count; i++)
                Data[ofs + i] = value;
        }

        #endregion
    }
}
