using System;

namespace PKHeX.Core;

/// <summary>
/// Stores the inventory of items that the player has acquired.
/// </summary>
/// <remarks>
/// Reads four separate pouch blobs: Items, Key Items, Storage, and Recipes.
/// </remarks>
public sealed class MyItem8a(SAV8LA sav, SCBlock block) : MyItem(sav, block.Raw);
