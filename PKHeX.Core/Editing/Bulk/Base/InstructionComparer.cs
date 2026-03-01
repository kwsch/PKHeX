using System;
using static PKHeX.Core.InstructionComparer;

namespace PKHeX.Core;

/// <summary>
/// Value comparison type
/// </summary>
public enum InstructionComparer : byte
{
    None,
    IsEqual,
    IsNotEqual,
    IsGreaterThan,
    IsGreaterThanOrEqual,
    IsLessThan,
    IsLessThanOrEqual,
}

/// <summary>
/// Extension methods for <see cref="InstructionComparer"/>
/// </summary>
public static class InstructionComparerExtensions
{
    extension(InstructionComparer comparer)
    {
        /// <summary>
        /// Indicates if the <see cref="comparer"/> is supported by the logic.
        /// </summary>
        /// <returns>True if supported, false if unsupported.</returns>
        public bool IsSupported => comparer switch
        {
            IsEqual => true,
            IsNotEqual => true,
            IsGreaterThan => true,
            IsGreaterThanOrEqual => true,
            IsLessThan => true,
            IsLessThanOrEqual => true,
            _ => false,
        };

        /// <summary>
        /// Checks if the compare operator is satisfied by a boolean comparison result.
        /// </summary>
        /// <param name="compareResult">Result from Equals comparison</param>
        /// <returns>True if satisfied</returns>
        /// <remarks>Only use this method if the comparison is boolean only. Use the <see cref="IsCompareOperator"/> otherwise.</remarks>
        public bool IsCompareEquivalence(bool compareResult) => comparer switch
        {
            IsEqual => compareResult,
            IsNotEqual => !compareResult,
            _ => false,
        };

        /// <summary>
        /// Checks if the compare operator is satisfied by the <see cref="IComparable{T}.CompareTo"/> result.
        /// </summary>
        /// <param name="compareResult">Result from CompareTo</param>
        /// <returns>True if satisfied</returns>
        public bool IsCompareOperator(int compareResult) => comparer switch
        {
            IsEqual => compareResult is 0,
            IsNotEqual => compareResult is not 0,
            IsGreaterThan => compareResult > 0,
            IsGreaterThanOrEqual => compareResult >= 0,
            IsLessThan => compareResult < 0,
            IsLessThanOrEqual => compareResult <= 0,
            _ => false,
        };
    }
}
