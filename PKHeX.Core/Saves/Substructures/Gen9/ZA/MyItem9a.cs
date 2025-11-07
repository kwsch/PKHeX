using System;
using System.Collections.Generic;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core;

/// <summary>
/// Player item pouches storage
/// </summary>
/// <remarks>size=0xBB80 (<see cref="ItemSaveSize"/> items)</remarks>
public sealed class MyItem9a(SAV9ZA sav, SCBlock block) : MyItem(sav, block.Raw)
{
    public const int ItemSaveSize = 3000;

    private Span<byte> GetItemSpan(ushort itemIndex) => InventoryPouch9a.GetItemSpan(Data, itemIndex);

    public uint DefaultInitPouch => ReadUInt32LittleEndian(Data); // Item 0

    /// <summary>
    /// Deletes the item at the requested <paramref name="itemIndex"/>.
    /// </summary>
    /// <remarks>
    /// Copies item 0 to the requested item index, effectively deleting it.
    /// Item 0 should always be un-tarnished, so this is a safe operation.
    /// <see cref="InventoryItem9a.PouchNone"/> for remarks on Pouch type quirks. This aims to retain consistency within the block.
    /// </remarks>
    public void DeleteItem(ushort itemIndex) => GetItemSpan(0).CopyTo(GetItemSpan(itemIndex));
    public InventoryItem9a GetItem(ushort itemIndex) => InventoryItem9a.Read(itemIndex, GetItemSpan(itemIndex));

    public uint GetItemQuantity(ushort itemIndex) => InventoryItem9a.GetItemCount(GetItemSpan(itemIndex));

    public void SetItemQuantity(ushort itemIndex, int quantity)
    {
        var pouch = GetPouchIndex(GetType(itemIndex));
        if (pouch == InventoryItem9a.PouchNone)
        {
            DeleteItem(itemIndex); // don't allow setting items that don't exist
            return;
        }
        var span = GetItemSpan(itemIndex);
        var item = InventoryItem9a.Read(itemIndex, span);
        item.Count = quantity;
        item.Pouch = GetPouchIndex(GetType(itemIndex));

        if (item.IsNewNotify && quantity != 0)
        {
            // Show popup, treat as new.
            item.IsNewNotify = false;
            item.IsNew = true;
        }

        item.Write(span);
    }

    public static InventoryType GetType(ushort itemIndex) => ItemStorage9ZA.GetInventoryPouch(itemIndex);

    public override IReadOnlyList<InventoryPouch> Inventory { get => ConvertToPouches(); set => LoadFromPouches(value); }

    private IReadOnlyList<InventoryPouch> ConvertToPouches()
    {
        InventoryPouch9a[] pouches =
        [
            MakePouch(InventoryType.Medicine),
            MakePouch(InventoryType.Balls),
            MakePouch(InventoryType.Berries),
            MakePouch(InventoryType.Items), // Other
            MakePouch(InventoryType.TMHMs),
            MakePouch(InventoryType.MegaStones),
            MakePouch(InventoryType.Treasure),
            MakePouch(InventoryType.KeyItems),
        ];
        return pouches.LoadAll(Data);
    }

    private void LoadFromPouches(IReadOnlyList<InventoryPouch> value)
    {
        value.SaveAll(Data);
        CleanIllegalSlots();
    }

    private void CleanIllegalSlots()
    {
        var types = ItemStorage9ZA.ValidTypes;
        var hashSet = new HashSet<ushort>(Legal.MaxItemID_9a);
        foreach (var type in types)
        {
            var items = ItemStorage9ZA.GetLegal(type);
            foreach (var item in items)
                hashSet.Add(item);
        }
        // even though there are 3000, just overwrite the ones that people will mess up.
        for (ushort itemIndex = 0; itemIndex < (ushort)SAV.MaxItemID; itemIndex++)
        {
            if (!hashSet.Contains(itemIndex))
                DeleteItem(itemIndex);
        }
    }

    public void ResetToDefault()
    {
        var block = Data;
        var defaultPouch = DefaultInitPouch;
        ResetToDefault(block, defaultPouch);
    }

    public static void ResetToDefault(Span<byte> block, uint defaultPouch)
    {
        block.Clear();
        for (int i = 0; i < block.Length; i += InventoryItem9a.SIZE)
            WriteUInt32LittleEndian(block[i..], defaultPouch);
    }

    private static InventoryPouch9a MakePouch(InventoryType type)
    {
        var info = ItemStorage9ZA.Instance;
        var max = info.GetMax(type);
        return new InventoryPouch9a(type, info, max, GetPouchIndex(type));
    }

    public static uint GetPouchIndex(InventoryType type) => type switch
    {
        InventoryType.Items => InventoryItem9a.PouchOther,
        InventoryType.KeyItems => InventoryItem9a.PouchKey,
        InventoryType.TMHMs => InventoryItem9a.PouchTM,
        InventoryType.Medicine => InventoryItem9a.PouchMedicine,
        InventoryType.Berries => InventoryItem9a.PouchBerry,
        InventoryType.Balls => InventoryItem9a.PouchBalls,
        InventoryType.Treasure => InventoryItem9a.PouchTreasure,
        InventoryType.MegaStones => InventoryItem9a.PouchMegaStones,
        _ => InventoryItem9a.PouchNone,
    };
}
