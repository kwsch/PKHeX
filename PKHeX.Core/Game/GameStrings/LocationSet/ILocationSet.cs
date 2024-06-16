using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace PKHeX.Core;

/// <summary>
/// Stores location names for a given game group.
/// </summary>
public interface ILocationSet
{
    /// <summary>
    /// Gets the location name group for the requested location bank group ID.
    /// </summary>
    ReadOnlySpan<string> GetLocationNames(int bankID);

    /// <summary>
    /// Gets the location name for the requested location ID.
    /// </summary>
    string GetLocationName(int locationID);

    /// <summary>
    /// Gets all groups -- not really useful besides unit testing.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    IEnumerable<(int Bank, ReadOnlyMemory<string> Names)> GetAll();
}
