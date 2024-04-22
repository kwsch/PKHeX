using System;
using System.ComponentModel;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Details about the Player's underground area state.
/// </summary>
/// <remarks>size: 0x27A0</remarks>
[TypeConverter(typeof(ExpandableObjectConverter))]
public sealed class UgSaveData8b(SAV8BS sav, Memory<byte> raw) : SaveBlock<SAV8BS>(sav, raw)
{
    public const int COUNT_DIGPOINTS = 10;
    public const int COUNT_ENCOUNTERS = 15;
    public const int COUNT_FRIENDS = 100; // 100 total with Friends+Others, savedata ctor allocates 100 for FriendPlayerList
    public const int COUNT_OTHERS = 0; // unused
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
    // UgPlayerInfo[100] FriendPlayerList;
    // UgPlayerInfo[0] OtherPlayerList; // unused
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

    public int ReturnZoneID        { get => ReadInt32LittleEndian(Data); set => WriteInt32LittleEndian(Data, value); }
    public int ReturnGridPositionX { get => ReadInt32LittleEndian(Data[0x04..]); set => WriteInt32LittleEndian(Data[0x4..], value); }
    public int ReturnGridPositionY { get => ReadInt32LittleEndian(Data[0x08..]); set => WriteInt32LittleEndian(Data[0x8..], value); }
    public int ReturnGridPositionZ { get => ReadInt32LittleEndian(Data[0x0C..]); set => WriteInt32LittleEndian(Data[0xC..], value); }

    public int ZenmetsuZoneID      { get => ReadInt32LittleEndian(Data[0x10..]); set => WriteInt32LittleEndian(Data[0x10..], value); }
    public float ZenmetsuPositionX { get => ReadSingleLittleEndian(Data[0x14..]); set => WriteSingleLittleEndian(Data[0x14..], value); }
    public float ZenmetsuPositionY { get => ReadSingleLittleEndian(Data[0x18..]); set => WriteSingleLittleEndian(Data[0x18..], value); }
    public float ZenmetsuPositionZ { get => ReadSingleLittleEndian(Data[0x1C..]); set => WriteSingleLittleEndian(Data[0x1C..], value); }
    public int ZenmetsuDirection   { get => ReadInt32LittleEndian(Data[0x20..]); set => WriteInt32LittleEndian(Data[0x20..], value); }

    private Span<byte> GetDigPoints() => Data.Slice(0x24, COUNT_DIGPOINTS);
    public void ClearDigPoints() => GetDigPoints().Fill(0xFF);

    public static int GetSlotOffset(int slot)
    {
        if ((uint)slot >= COUNT_ENCOUNTERS)
            throw new ArgumentOutOfRangeException(nameof(slot));
        return OFS_ENCOUNTPOKE + (slot * PokeCrypto.SIZE_8PARTY);
    }

    public  int ReturnUgZoneID   { get =>  ReadInt32LittleEndian(Data[OFS_ReturnUgZoneID..]); set =>  WriteInt32LittleEndian(Data[OFS_ReturnUgZoneID..], value); }
    public uint TalkPlayerDataID { get => ReadUInt32LittleEndian(Data[(OFS_UgRecord + 0x0)..]); set => WriteUInt32LittleEndian(Data[(OFS_UgRecord + 0x0)..], value); }
    public uint TalkPlayerCount  { get => ReadUInt32LittleEndian(Data[(OFS_UgRecord + 0x4)..]); set => WriteUInt32LittleEndian(Data[(OFS_UgRecord + 0x4)..], value); }

    #region Seen NPCs

    public Span<byte> GetTrainers() => Data.Slice(OFS_NPC, COUNT_TRAINERS);

    public void SetTrainers(ReadOnlySpan<byte> data)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(data.Length, COUNT_TRAINERS);
        data.CopyTo(GetTrainers());
    }

    public byte GetNPCSeen(int index) => Data[OFS_NPC + index];
    public void SetNPCSeen(int index, byte value) => Data[OFS_NPC + index] = value;

    public void ClearNPC() => GetTrainers().Clear();
    public void ClearNPC(int start, int count = COUNT_TRAINERS) => FillNPC(0, start, count);

    public int TalkedNPC // Spiritomb Trainer Count (duplicates OK)
    {
        get
        {
            var tr = GetTrainers();
            int ctr = 0;
            foreach (var t in tr)
            {
                if (t != 0)
                    ctr++;
            }
            return ctr;
        }
    }

    public void FillNPC(byte value, int start = 0, int count = COUNT_TRAINERS)
    {
        if ((uint)start + (uint)count >= COUNT_TRAINERS)
            throw new ArgumentOutOfRangeException(nameof(count));
        if ((uint)start >= COUNT_TRAINERS)
            throw new ArgumentOutOfRangeException(nameof(start));

        var ofs = OFS_NPC + start;
        for (int i = 0; i < count; i++)
            Data[ofs + i] = value;
    }

    #endregion

    public Memory<byte> this[int i] => Raw.Slice(GetSlotOffset(i), PokeCrypto.SIZE_8PARTY);
}
