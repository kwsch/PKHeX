using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Editor object that unpacks <see cref="EventWork{T}"/> into flags & work groups, and handles value get/set operations.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class SplitEventEditor<T> where T : struct
    {
        public readonly IList<EventVarGroup> Work;
        public readonly IList<EventVarGroup> Flag;
        public readonly IEventWork<T> Block;

        public SplitEventEditor(IEventWork<T> block, IEnumerable<string> work, IEnumerable<string> flag)
        {
            Block = block;
            // load lines
            var worklines = work.Where(z => !string.IsNullOrWhiteSpace(z) && z.Length > 5);
            Work = EventWorkUtil.GetVars(worklines, (index, t, data) => new EventWork<T>(index, t, data));
            var flaglines = flag.Where(z => !string.IsNullOrWhiteSpace(z) && z.Length > 5);
            Flag = EventWorkUtil.GetVars(flaglines, (index, t, data) => new EventFlag(index, t, data));

            // initialize lines
            foreach (var g in Work)
            {
                foreach (var item in g.Vars)
                {
                    item.RawIndex = block.GetWorkRawIndex(item.Type, item.RelativeIndex);
                    ((EventWork<T>)item).Value = block.GetWork(item.RawIndex);
                }
            }
            foreach (var g in Flag)
            {
                foreach (var item in g.Vars)
                {
                    item.RawIndex = block.GetFlagRawIndex(item.Type, item.RelativeIndex);
                    ((EventFlag)item).Flag = block.GetFlag(item.RawIndex);
                }
            }
        }

        /// <summary>
        /// Writes all of the updated event values back to the block.
        /// </summary>
        public void Save()
        {
            foreach (var g in Work)
            {
                foreach (var item in g.Vars)
                {
                    var val = ((EventWork<T>)item).Value;
                    Block.SetWork(item.RawIndex, val);
                }
            }
            foreach (var g in Flag)
            {
                foreach (var item in g.Vars)
                {
                    var val = ((EventFlag)item).Flag;
                    Block.SetFlag(item.RawIndex, val);
                }
            }
        }
    }
}
