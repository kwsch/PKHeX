using System;

namespace PKHeX.Core;

/// <summary>
/// Provides details about box names within the save file.
/// </summary>
public interface IBoxDetailName : IBoxDetailNameRead
{
    public void SetBoxName(int box, ReadOnlySpan<char> value);
}

public interface IBoxDetailNameRead
{
    public string GetBoxName(int box);
}

public static class BoxDetailNameExtensions
{
    public static string GetDefaultBoxName(int box) => $"Box {box + 1}"; // 0-indexed
    public static string GetDefaultBoxNameCaps(int box) => $"BOX {box + 1}"; // 0-indexed
    public static string GetDefaultBoxNameJapanese(int box) => $"ボックス{box + 1}";

    public static void MoveBoxName(this IBoxDetailName obj, int box, int insertBeforeBox)
    {
        if (box == insertBeforeBox)
            return;
        var value = obj.GetBoxName(box);
        // Shift all names between the two boxes
        if (box < insertBeforeBox)
        {
            for (int i = box; i < insertBeforeBox; i++)
                obj.SetBoxName(i, obj.GetBoxName(i + 1));
        }
        else
        {
            for (int i = box; i > insertBeforeBox; i--)
                obj.SetBoxName(i, obj.GetBoxName(i - 1));
        }
        obj.SetBoxName(insertBeforeBox, value);
    }

    public static void SwapBoxName(this IBoxDetailName obj, int box1, int box2)
    {
        if (box1 == box2)
            return;
        var value1 = obj.GetBoxName(box1);
        var value2 = obj.GetBoxName(box2);
        obj.SetBoxName(box1, value2);
        obj.SetBoxName(box2, value1);
    }
}
