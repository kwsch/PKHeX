using System;
using System.Collections;
using System.Collections.Generic;

namespace PKHeX.Core
{
    public class PeekEnumerator<T> : IEnumerator<T>
    {
        private readonly IEnumerator<T> Enumerator;
        private T peek;
        private bool didPeek;

        #region IEnumerator Implementation

        public bool MoveNext()
        {
            if (!didPeek)
                return Enumerator.MoveNext();
            didPeek = false;
            return true;
        }
        public void Reset()
        {
            Enumerator.Reset();
            didPeek = false;
        }

        object IEnumerator.Current => Current;
        public void Dispose() => Enumerator.Dispose();
        public T Current => didPeek ? peek : Enumerator.Current;

        #endregion
        
        public PeekEnumerator(IEnumerator<T> enumerator) => Enumerator = enumerator ?? throw new ArgumentNullException(nameof(enumerator));

        private bool TryFetchPeek()
        {
            if (!didPeek && (didPeek = Enumerator.MoveNext()))
                peek = Enumerator.Current;
            return didPeek;
        }

        public T Peek()
        {
            if (!TryFetchPeek())
                throw new InvalidOperationException("Enumeration already finished.");

            return peek;
        }
        public T PeekOrDefault()
        {
            return !TryFetchPeek() ? default(T) : peek;
        }
        public bool PeekIsNext()
        {
            return TryFetchPeek();
        }
    }
}
