namespace PKHeX.Core;

/// <summary>
/// Indicates if the object is capable of being marked by the player with simple shapes.
/// </summary>
public interface IAppliedMarkings
{
    /// <summary>
    /// Count of unique markings that can be applied to the object.
    /// </summary>
    int MarkingCount { get; }
}

public interface IAppliedMarkings<T> : IAppliedMarkings where T : unmanaged
{
    /// <summary>
    /// Gets the marking value at the given index.
    /// </summary>
    /// <param name="index">Index of the marking to get.</param>
    /// <returns>Marking value at the given index.</returns>
    T GetMarking(int index);

    /// <summary>
    /// Sets the marking value at the given index.
    /// </summary>
    /// <param name="index">Index of the marking to set.</param>
    /// <param name="value">Value to set the marking to.</param>
    void SetMarking(int index, T value);
}

/// <summary>
/// Generation 3 Markings
/// </summary>
public interface IAppliedMarkings3 : IAppliedMarkings<bool>
{
    /// <summary>
    /// Backing value for the packed bits.
    /// </summary>
    byte MarkingValue { get; set; }

    bool MarkingCircle { get; set; }
    bool MarkingTriangle { get; set; } // In generation 3, this is marking index 2
    bool MarkingSquare { get; set; } // In generation 3, this is marking index 1
    bool MarkingHeart { get; set; }
}

/// <summary>
/// Generation 4-6 Markings
/// </summary>
public interface IAppliedMarkings4 : IAppliedMarkings3
{
    bool MarkingStar { get; set; }
    bool MarkingDiamond { get; set; }
}

/// <summary>
/// Generation 7+ Markings
/// </summary>
public interface IAppliedMarkings7 : IAppliedMarkings<MarkingColor>
{
    /// <summary>
    /// Backing value for the packed bits.
    /// </summary>
    ushort MarkingValue { get; set; }

    MarkingColor MarkingCircle { get; set; }
    MarkingColor MarkingTriangle { get; set; }
    MarkingColor MarkingSquare { get; set; }
    MarkingColor MarkingHeart { get; set; }
    MarkingColor MarkingStar { get; set; }
    MarkingColor MarkingDiamond { get; set; }
}

public enum MarkingColor : byte
{
    /// <summary>
    /// Not marked.
    /// </summary>
    None = 0,

    /// <summary>
    /// Blue marking.
    /// </summary>
    Blue = 1,

    /// <summary>
    /// Pink marking.
    /// </summary>
    Pink = 2,
}
