# PKHeX - Pokémon Legends: Z-A DLC 2.0.0 Support

This fork adds support for **Pokémon Legends: Z-A version 2.0.0 (Mega Dimensions DLC)**.

> [!CAUTION]
> **BACKUP YOUR SAVE FILES BEFORE USING!**
> This is an unofficial patch that has NOT been fully tested. Always keep backups of your original save files.

> [!WARNING]
> **DONUT ITEMS DO NOT WORK!**
> The new DLC Donut/Food items (IDs 2651-2656) and item 2137 are **NOT editable** in PKHeX. They exist in save files but will not appear in the inventory editor dropdowns. This is because PKHeX's item name strings have not been updated for these new items yet.

## Changelog

### Save File Compatibility
- **`SaveUtil.cs`**: Extended `SIZE_G9ZA_Max` from `0x2F3289` (3,093,129 bytes) to `0x30A3A8` (3,186,600 bytes) to support the larger DLC 2.0.0 save files

### New Inventory Type: Donuts
The DLC introduces a new Donuts/Food item category. The following files were updated:

- **`InventoryType.cs`**: Added `Donuts` enum value
- **`InventoryItem9a.cs`**: Added `PouchDonuts = 8` constant for the new pouch index
- **`MyItem9a.cs`**: Added Donuts pouch to `ConvertToPouches()` and `GetPouchIndex()` mapping
- **`ItemStorage9ZA.cs`**: 
  - Added `Donuts` item list (placeholder - items 2651-2656)
  - Added `InventoryType.Donuts` to `ValidTypes`
  - Added Donuts cases to `GetMax()` (999) and `GetLegal()` methods
- **`SAV_Inventory.cs`**: Added `Donuts` case in `GetImage()` using `bag_candy` as placeholder icon

### Item ID Updates
- **`Legal.cs`**: Updated `MaxItemID_9a` from 2634 to 2656 to accommodate new DLC items

### Known DLC 2.0.0 Items (Not Yet Usable in UI)
The following new item IDs were discovered through save file analysis but are **not yet in the dropdown lists** until official item strings are added:

| Item ID | Category | Notes |
|---------|----------|-------|
| 2137 | Other | New DLC item |
| 2651 | Berry/Food | Meringa Baccarcade (Donut) |
| 2652 | Berry/Food | Curry Baccarosmel (Donut) |
| 2653 | Berry/Food | Unknown (placeholder) |
| 2654 | Berry/Food | Mousse Baccalga (Donut) |
| 2655 | Berry/Food | Mousse Baccastagna (Donut) |
| 2656 | Berry/Food | Unknown (placeholder) |

> **Note**: These items exist in save files but cannot be added via PKHeX UI until the item name strings are updated in PKHeX's game data.

### Files Added
- **`BlankBlocks9a.cs`**: Added from upstream (was missing in the base version)

## Files Changed Summary
```
PKHeX.Core/Items/ItemStorage9ZA.cs                        | 10 ++++++++++
PKHeX.Core/Legality/Legal.cs                              |  2 +-
PKHeX.Core/Saves/Substructures/Gen9/ZA/MyItem9a.cs        |  2 ++
PKHeX.Core/Saves/Substructures/Inventory/InventoryType.cs |  1 +
PKHeX.Core/Saves/Substructures/Inventory/Item/InventoryItem9a.cs | 1 +
PKHeX.Core/Saves/Util/SaveUtil.cs                         |  4 ++--
PKHeX.WinForms/Subforms/Save Editors/SAV_Inventory.cs     |  1 +
```

## Testing
Tested with save files from:
- Pre-DLC game version (3,093,124 bytes) ✅
- Post-DLC 2.0.0 game version (3,186,598 bytes) ✅

## Credits
- PKHeX original author: [Kurt](https://github.com/kwsch/PKHeX)
- DLC 2.0.0 compatibility patch: AI-assisted development
