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
            {
                var entry = GetEntry(data, i);
                Entries[i] = entry;
            }
        }

        public ShadowInfoTableXD() : this(new byte[SIZE_ENTRY * MaxCount]) { }

        private static ShadowInfoEntryXD GetEntry(byte[] data, int i)
        {
            var d = data.Slice(i * SIZE_ENTRY, SIZE_ENTRY);
            var entry = new ShadowInfoEntryXD(d);
            return entry;
        }

        public byte[] Write() => Entries.SelectMany(entry => entry.Data).Take(MaxLength).ToArray();

        public ShadowInfoEntryXD this[int index] { get => Entries[index]; set => Entries[index] = value; }
        public int Count => Entries.Length;
    }
}
