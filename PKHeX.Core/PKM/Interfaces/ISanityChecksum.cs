namespace PKHeX.Core;

/// <summary>
/// Unencrypted metadata about the entity to check if the data is not corrupt.
/// </summary>
public interface ISanityChecksum
{
    /// <summary>
    /// Used to store flags about decryption state and / or integrity of the data range.
    /// </summary>
    /// <remarks>Zero when encrypted. Dumped decrypted data does not manipulate this property, and keeps it zero.</remarks>
    ushort Sanity { get; set; }

    /// <summary>
    /// Calculated checksum of the encrypted region, used to check if the data has been manipulated externally.
    /// </summary>
    /// <remarks>
    /// Can detect if the data was modified via cheats / programs, as well as cosmic ray bit-flips.
    /// Anything with a bad checksum will be flagged in the <see cref="Sanity"/> as a bad egg.
    /// </remarks>
    ushort Checksum { get; set; }
}
