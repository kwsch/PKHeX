using System;

namespace PKHeX.Core;

/// <summary>
/// Represents a Box Editor that loads the contents for easy manipulation.
/// </summary>
public sealed class BoxEdit(SaveFile SAV)
{
    private readonly PKM[] CurrentContents = new PKM[SAV.BoxSlotCount];

    public void Reload() => LoadBox(CurrentBox);

    public void LoadBox(int box)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)box, (uint)SAV.BoxCount);

        SAV.GetBoxData(CurrentContents, box, 0);
        CurrentBox = box;
    }

    public PKM this[int index]
    {
        get => CurrentContents[index];
        set
        {
            CurrentContents[index] = value;
            SAV.SetBoxSlotAtIndex(value, index);
        }
    }

    public int CurrentBox { get; private set; }
    public int BoxWallpaper
    {
        get => (SAV as IBoxDetailWallpaper)?.GetBoxWallpaper(CurrentBox) ?? 0;
        set => (SAV as IBoxDetailWallpaper)?.SetBoxWallpaper(CurrentBox, value);
    }

    public string BoxName
    {
        get => (SAV as IBoxDetailNameRead)?.GetBoxName(CurrentBox) ?? BoxDetailNameExtensions.GetDefaultBoxName(CurrentBox);
        set => (SAV as IBoxDetailName)?.SetBoxName(CurrentBox, value);
    }

    /// <summary>
    /// Mimics a box viewer Left-Arrow navigation, adjusting to view the previous (n-1) box.
    /// </summary>
    /// <remarks>
    /// If the new index under-flows the box count, it wraps around to the last box.
    /// </remarks>
    /// <param name="min">Forces the box to the first box in the save, regardless of current box.</param>
    /// <returns>Box index that was loaded.</returns>
    public int MoveLeft(bool min = false)
    {
        int newBox = min ? 0 : (CurrentBox + SAV.BoxCount - 1) % SAV.BoxCount;
        LoadBox(newBox);
        return newBox;
    }

    /// <summary>
    /// Mimics a box viewer Right-Arrow navigation, adjusting to view the next (n+1) box.
    /// </summary>
    /// <remarks>
    /// If the new index over-flows the box count, it wraps around to the first box.
    /// </remarks>
    /// <param name="max">Forces the box to the last box in the save, regardless of current box.</param>
    /// <returns>Box index that was loaded.</returns>
    public int MoveRight(bool max = false)
    {
        int newBox = max ? SAV.BoxCount - 1 : (CurrentBox + 1) % SAV.BoxCount;
        LoadBox(newBox);
        return newBox;
    }
}
