using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PKHeX.Core
{
    public sealed class GoParkStorage : SaveBlock, IEnumerable<GP1>
    {
        public GoParkStorage(SaveFile sav) : base(sav)
        {
            Offset = ((SAV7b)sav).Blocks.GetBlockOffset(BelugaBlockIndex.GoParkEntities);
        }

        public const int SlotsPerArea = 50;
        public const int Areas = 20;
        public const int Count = SlotsPerArea * Areas; // 1000

        public GP1 this[int index]
        {
            get
            {
                Debug.Assert(index < Count);
                return GP1.FromData(Data, Offset + (GP1.SIZE * index));
            }
            set
            {
                Debug.Assert(index < Count);
                value.WriteTo(Data, Offset + (GP1.SIZE * index));
            }
        }

        public GP1[] GetAllEntities()
        {
            var value = new GP1[Count];
            for (int i = 0; i < value.Length; i++)
                value[i] = this[i];
            return value;
        }

        public void SetAllEntities(IReadOnlyList<GP1> value)
        {
            Debug.Assert(value.Count == Count);
            for (int i = 0; i < value.Count; i++)
                this[i] = value[i];
        }

        public IEnumerable<string> DumpAll(IReadOnlyList<string> speciesNames) => GetAllEntities().Select((z, i) => new {Index = i, Entry = z}).Where(z => z.Entry.Species > 0).Select(z => z.Entry.Dump(speciesNames, z.Index));

        public IEnumerator<GP1> GetEnumerator() => (IEnumerator<GP1>)GetAllEntities().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetAllEntities().GetEnumerator();

        public void DeleteAll()
        {
            var blank = new GP1();
            for (int i = 0; i < Count; i++)
                this[i] = blank;
        }
    }
}