using System;
using System.Collections;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Iterates a generic collection with the ability to peek into the collection to see if the next element exists.
    /// </summary>
    /// <typeparam name="T">Generic Collection Element Type</typeparam>
    public sealed class PeekEnumerator<T> : IEnumerator<T> where T : class
    {
        private readonly IEnumerator<T> Enumerator;
        private T? peek;
        private bool didPeek;

        #region IEnumerator Implementation

        /// <summary>
        /// Advances the enumerator to the next element in the collection.
        /// </summary>
        /// <returns>Indication if there are more elements in the collection.</returns>
        public bool MoveNext()
        {
            if (!didPeek)
                return Enumerator.MoveNext();
            didPeek = false;
            return true;
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        public void Reset()
        {
            Enumerator.Reset();
            peek = default;
            didPeek = false;
        }

        object? IEnumerator.Current => Current;
        public void Dispose() => Enumerator.Dispose();
        public T Current => didPeek ? peek! : Enumerator.Current;

        #endregion

        public PeekEnumerator(IEnumerator<T> enumerator) => Enumerator = enumerator ?? throw new ArgumentNullException(nameof(enumerator));
        public PeekEnumerator(IEnumerable<T> enumerable) => Enumerator = enumerable.GetEnumerator();

        /// <summary>
        /// Fetch the next element, if not already performed.
        /// </summary>
        /// <returns>True/False that a Next element exists</returns>
        /// <remarks>Advances the enumerator if Next has not been peeked already</remarks>
        private bool TryFetchPeek()
        {
            if (!didPeek && (didPeek = Enumerator.MoveNext()))
                peek = Enumerator.Current;
            return didPeek;
        }

        /// <summary>
        /// Peeks to the next element
        /// </summary>
        /// <returns>Next element</returns>
        /// <remarks>Throws an exception if no element exists</remarks>
        public T Peek()
        {
            if (!TryFetchPeek())
                throw new InvalidOperationException("Enumeration already finished.");

            return peek!;
        }

        public T? PeekOrDefault() => !TryFetchPeek() ? default : peek;

        /// <summary>
        /// Checks if a Next element exists
        /// </summary>
        /// <returns>True/False that a Next element exists</returns>
        public bool PeekIsNext() => TryFetchPeek();
    }
}
