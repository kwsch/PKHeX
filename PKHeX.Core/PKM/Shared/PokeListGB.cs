using System;

namespace PKHeX.Core
{
    /// <summary>
    /// List of <see cref="T"/> prefixed by a count.
    /// </summary>
    /// <typeparam name="T"><see cref="PKM"/> type that inherits from <see cref="GBPKML"/>.</typeparam>
    public abstract class PokeListGB<T> where T : GBPKML
    {
        // Structure:
        // u8               Count of slots filled
        // s8[capacity+1]   GB Species ID in Slot (-1 = no species)
        // pkx[capacity]    GB PKM data (no strings)
        // str[capacity]    Trainer Name
        // str[capacity]    Nickname
        // 
        // where,
        // - str has variable size (jp/int)
        // - pkx is different size for gen1/gen2

        private readonly int StringLength;
        private readonly byte[] Data;
        private readonly byte Capacity;
        private readonly int Entry_Size;
        protected readonly bool Japanese;

        public readonly T[] Pokemon;

        /// <summary>
        /// Count of slots filled in the list
        /// </summary>
        public byte Count { get => Data[0]; private set => Data[0] = value > Capacity ? Capacity : value; }

        private const int SLOT_NONE = byte.MaxValue;

        protected PokeListGB(byte[]? d, PokeListType c = PokeListType.Single, bool jp = false)
        {
            Japanese = jp;
            Capacity = (byte)c;
            Entry_Size = GetEntrySize();
            StringLength = GetStringLength(jp);
            var data = d ?? GetEmptyList(c, jp);
            var dataSize = 1 + 1 + (Capacity * (Entry_Size + 1 + (2 * StringLength)));

            Array.Resize(ref data, dataSize);
            Data = data;

            Pokemon = Read();
        }

        protected PokeListGB(PokeListType c = PokeListType.Single, bool jp = false)
            : this(null, c, jp) => Count = 1;

        protected PokeListGB(T pk)
            : this(PokeListType.Single, pk.Japanese)
        {
            this[0] = pk;
            Count = 1;
        }

        private byte[] GetEmptyList(PokeListType c, bool jp = false)
        {
            int capacity = (byte)c;

            int size_intro = capacity + 1;
            int size_pkm = GetEntrySize() * capacity;
            int size_str = 2 * GetStringLength(jp) * capacity;

            var result = new byte[1 + size_intro + size_pkm + size_str];

            for (int i = 1; i <= size_intro; i++)
                result[i] = SLOT_NONE;

            for (int i = 1 + size_intro + size_pkm; i < result.Length; i++)
                result[i] = StringConverter12.G1TerminatorCode; // terminator

            return result;
        }

        private int GetOffsetPKMData(int base_ofs, int i) => base_ofs + (Entry_Size * i);
        private int GetOffsetPKMOT(int base_ofs, int i) => GetOffsetPKMData(base_ofs, Capacity) + (StringLength * i);
        private int GetOffsetPKMNickname(int base_ofs, int i) => GetOffsetPKMOT(base_ofs, Capacity) + (StringLength * i);

        private static int GetStringLength(bool jp) => jp ? GBPKML.StringLengthJapanese : GBPKML.StringLengthNotJapan;
        protected bool IsFormatParty => IsCapacityPartyFormat((PokeListType)Capacity);
        protected static bool IsCapacityPartyFormat(PokeListType Capacity) => Capacity is PokeListType.Single or PokeListType.Party;

        protected static int GetDataSize(PokeListType c, bool jp, int entrySize)
        {
            var entryLength = 1 + entrySize + (2 * GetStringLength(jp));
            return 2 + ((byte)c * entryLength);
        }

        protected abstract int GetEntrySize();
        protected abstract byte GetSpeciesBoxIdentifier(T pk);
        protected abstract T GetEntry(byte[] dat, byte[] otname, byte[] nick, bool egg);

        public T this[int i]
        {
            get
            {
                if ((uint)i > Capacity)
                    throw new ArgumentOutOfRangeException($"Invalid {nameof(PokeListGB<T>)} Access: {i}");
                return Pokemon[i];
            }
            set => Pokemon[i] = (T)value.Clone();
        }

        private T[] Read()
        {
            var arr = new T[Capacity];
            int base_ofs = 2 + Capacity;
            for (int i = 0; i < Capacity; i++)
                arr[i] = GetEntry(base_ofs, i);
            return arr;
        }

        public byte[] Write()
        {
            // Rebuild GB Species ID table
            int count = Array.FindIndex(Pokemon, pk => pk.Species == 0);
            Count = count < 0 ? Capacity : (byte)count;
            int base_ofs = 1 + 1 + Capacity;
            for (int i = 0; i < Count; i++)
            {
                Data[1 + i] = GetSpeciesBoxIdentifier(Pokemon[i]);
                SetEntry(base_ofs, i);
            }
            Data[1 + Count] = SLOT_NONE;
            return Data;
        }

        private T GetEntry(int base_ofs, int i)
        {
            int pkOfs = GetOffsetPKMData(base_ofs, i);
            int otOfs = GetOffsetPKMOT(base_ofs, i);
            int nkOfs = GetOffsetPKMNickname(base_ofs, i);

            var dat = Data.Slice(pkOfs, Entry_Size);
            var otname = Data.Slice(otOfs, StringLength);
            var nick = Data.Slice(nkOfs, StringLength);

            return GetEntry(dat, otname, nick, Data[1 + i] == 0xFD);
        }

        private void SetEntry(int base_ofs, int i)
        {
            int pkOfs = GetOffsetPKMData(base_ofs, i);
            int otOfs = GetOffsetPKMOT(base_ofs, i);
            int nkOfs = GetOffsetPKMNickname(base_ofs, i);
            Array.Copy(Pokemon[i].Data, 0, Data, pkOfs, Entry_Size);
            Array.Copy(Pokemon[i].OT_Trash, 0, Data, otOfs, StringLength);
            Array.Copy(Pokemon[i].Nickname_Trash, 0, Data, nkOfs, StringLength);
        }
    }
}