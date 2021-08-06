using System.Linq;

namespace PKHeX.Core
{
    public sealed class ShadowInfoTableXD
    {
        private readonly ShadowInfoEntryXD[] Entries;
        private readonly int MaxLength;
        private const int SIZE_ENTRY = ShadowInfoEntryXD.SIZE_ENTRY;
        private const int MaxCount = 128;

        public ShadowInfoTableXD(byte[] data)
        {
            MaxLength = data.Length;
            int eCount = data.Length/SIZE_ENTRY;
            Entries = new ShadowInfoEntryXD[eCount];
            for (int i = 0; i < eCount; i++)
                Entries[i] = GetEntry(data, i);
        }

        public ShadowInfoTableXD() : this(new byte[SIZE_ENTRY * MaxCount]) { }

        private static ShadowInfoEntryXD GetEntry(byte[] data, int index)
        {
            var slice = data.Slice(index * SIZE_ENTRY, SIZE_ENTRY);
            return new ShadowInfoEntryXD(slice);
        }

        public byte[] Write() => Entries.SelectMany(entry => entry.Data).Take(MaxLength).ToArray();

        public ShadowInfoEntryXD this[int index] { get => Entries[index]; set => Entries[index] = value; }
        public int Count => Entries.Length;
    }
}
