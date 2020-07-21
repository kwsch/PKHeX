using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Event number storage for more complex logic events.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class EventWork<T> : EventVar where T : struct
    {
        public T Value;
        public readonly IList<EventWorkVal> Options = new List<EventWorkVal> { new EventWorkVal() };

        public EventWork(int index, EventVarType t, IReadOnlyList<string> pieces) : base(index, t, pieces[1])
        {
            if (pieces.Count < 3)
                return;

            var items = pieces[2]
                .Split(',')
                .Select(z => z.Split(':'))
                .Where(z => z.Length == 2);

            foreach (var s in items)
            {
                if (int.TryParse(s[0], out var val))
                    Options.Add(new EventWorkVal(s[1], val));
            }
        }
    }
}