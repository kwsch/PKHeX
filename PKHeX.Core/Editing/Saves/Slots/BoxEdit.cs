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

        SAV.AddBoxData(CurrentContents, box, 0);
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

    public int MoveLeft(bool max = false)
    {
        int newBox = max ? 0 : (CurrentBox + SAV.BoxCount - 1) % SAV.BoxCount;
        LoadBox(newBox);
        return newBox;
    }

    public int MoveRight(bool max = false)
    {
        int newBox = max ? SAV.BoxCount - 1 : (CurrentBox + 1) % SAV.BoxCount;
        LoadBox(newBox);
        return newBox;
    }
}
