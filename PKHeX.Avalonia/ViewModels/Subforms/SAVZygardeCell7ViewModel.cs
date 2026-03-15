using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a single Zygarde cell row.
/// </summary>
public partial class ZygardeCellModel : ObservableObject
{
    public int Number { get; }
    public string Location { get; }

    [ObservableProperty]
    private int _stateIndex;

    public ZygardeCellModel(int number, string location, int stateIndex)
    {
        Number = number;
        Location = location;
        _stateIndex = stateIndex;
    }
}

/// <summary>
/// ViewModel for the Gen 7 Zygarde cell editor.
/// </summary>
public partial class SAVZygardeCell7ViewModel : SaveEditorViewModelBase
{
    private readonly SaveFile _origin;
    private readonly SAV7 _sav;

    public ObservableCollection<ZygardeCellModel> Cells { get; } = [];
    public string[] StateNames { get; } = ["None", "Available", "Received"];

    [ObservableProperty] private int _cellsTotal;
    [ObservableProperty] private int _cellsCollected;

    public SAVZygardeCell7ViewModel(SAV7 sav) : base(sav)
    {
        _sav = (SAV7)(_origin = sav).Clone();
        var ew = _sav.EventWork;

        CellsTotal = ew.ZygardeCellTotal;
        CellsCollected = ew.ZygardeCellCount;

        var locations = _sav is SAV7SM ? LocationsSM : LocationsUSUM;
        for (int i = 0; i < ew.TotalZygardeCellCount; i++)
        {
            var cell = ew.GetZygardeCell(i);
            if (cell > 2) cell = 0;
            Cells.Add(new ZygardeCellModel(i + 1, locations[i], cell));
        }
    }

    [RelayCommand]
    private void GiveAll()
    {
        int added = 0;
        foreach (var cell in Cells)
        {
            if (cell.StateIndex != 2)
                added++;
            cell.StateIndex = 2;
        }
        CellsCollected += added;
        if (_sav is not SAV7USUM)
            CellsTotal += added;
    }

    [RelayCommand]
    private void Save()
    {
        var ew = _sav.EventWork;
        for (int i = 0; i < Cells.Count; i++)
            ew.SetZygardeCell(i, (ushort)Cells[i].StateIndex);

        ew.ZygardeCellTotal = (ushort)CellsTotal;
        ew.ZygardeCellCount = (ushort)CellsCollected;
        if (_sav is SAV7USUM)
            _sav.SetRecord(72, CellsCollected);

        _origin.CopyChangesFrom(_sav);
        Modified = true;
    }

    #region Location Data

    private static readonly string[] LocationsSM =
    [
        "Verdant Cave - Trial Site",
        "Ruins of Conflict - Outside",
        "Route 1 (Day)",
        "Route 3",
        "Route 3 (Day)",
        "Kala'e Bay",
        "Hau'oli Cemetery",
        "Route 2",
        "Route 1 - Trainer School (Night)",
        "Hau'oli City - Shopping District",
        "Route 1 - Outskirts",
        "Hau'oli City - Shopping District (Night)",
        "Route 1",
        "Iki Town (Night)",
        "Route 4",
        "Paniola Ranch (Night)",
        "Paniola Ranch (Day)",
        "Wela Volcano Park - Top",
        "Lush Jungle - Cave",
        "Route 7",
        "Akala Outskirts",
        "Royal Avenue (Day)",
        "Royal Avenue (Night)",
        "Konikoni City (Night)",
        "Heahea City (Night)",
        "Route 8",
        "Route 8 (Day)",
        "Route 5",
        "Hano Beach (Day)",
        "Heahea City",
        "Diglett's Tunnel",
        "Hano Beach",
        "Malie Garden",
        "Malie City - Community Center (Night)",
        "Malie City (Day)",
        "Malie City - Outer Cape (Day)",
        "Route 11 (Night)",
        "Route 12 (Day)",
        "Route 12",
        "Secluded Shore (Night)",
        "Blush Mountain",
        "Route 13",
        "Haina Desert",
        "Ruins of Abundance - Outside",
        "Route 14",
        "Route 14 (Night)",
        "Tapu Village",
        "Route 15",
        "Aether House (Day)",
        "Ula'ula Meadow - Boardwalk",
        "Route 16 (Day)",
        "Ula'ula Meadow - Grass",
        "Route 17 - Building",
        "Route 17 - Ledge",
        "Po Town (Night)",
        "Route 10 (Day)",
        "Hokulani Observatory (Night)",
        "Mount Lanakila - Mountainside",
        "Mount Lanakila - High Mountainside",
        "Secluded Shore (Day)",
        "Route 13 (Night)",
        "Po Town (Day)",
        "Seafolk Village - Blue Food Boat",
        "Seafolk Village - Unbuilt House",
        "Poni Wilds (Day)",
        "Poni Wilds (Night)",
        "Poni Wilds",
        "Ancient Poni Path - Near Well (Day)",
        "Ancient Poni Path (Night)",
        "Poni Breaker Coast (Day)",
        "Ruins of Hope",
        "Poni Grove - Mountain Corner",
        "Poni Grove - Near a Bush",
        "Poni Plains (Day)",
        "Poni Plains (Night)",
        "Poni Plains",
        "Poni Meadow",
        "Poni Coast (Night)",
        "Poni Coast",
        "Poni Gauntlet - On Bridge",
        "Poni Gauntlet - Island w/ Trainer",
        "Resolution Cave - 1F (Day)",
        "Resolution Cave - B1F (Night)",
        "Vast Poni Canyon - 3F",
        "Vast Poni Canyon - 2F",
        "Vast Poni Canyon - Top",
        "Vast Poni Canyon - Inside",
        "Ancient Poni Path - Brickwall (Day)",
        "Poni Breaker Coast (Night)",
        "Resolution Cave - B1F",
        "Aether Foundation B2F - Right Hallway",
        "Aether Foundation 1F - Outside - Right Side",
        "Aether Foundation 1F - Outside (Day)",
        "Aether Foundation 1F - Entrance (Night)",
        "Aether Foundation 1F - Main Building",
    ];

    private static readonly string[] LocationsUSUM =
    [
        "Hau'oli City (Shopping) - Salon (Outside)",
        "Hau'oli City (Shopping) - Malasada Shop (Outside)",
        "Hau'oli City (Shopping) - Ilima's House (2F)",
        "Malie City - Library (1F)",
        "Hau'oli City (Marina) - Pier",
        "Route 2 - Southeast House",
        "Hau'oli City (Shopping) - Ilima's House (Outside)",
        "Hau'oli City (Shopping) - City Hall",
        "Heahea City - Hotel (3F)",
        "Route 2 - Berry Fields House",
        "Route 2 - Berry Fields House (Outside)",
        "Royal Avenue - Northeast",
        "Hau'oli City (Shopping) - Pokemon Center (Outside)",
        "Royal Avenue - South",
        "Hokulani Observatory - Room",
        "Hokulani Observatory - Reception",
        "Hau'oli City (Shopping) - City Hall (Outside)",
        "Konikoni City - Olivia's Jewelry Shop (2F)",
        "Heahea City - Surfboard (Outside)",
        "Po Town - Southwest",
        "Hano Resort Lobby - Southwest Water",
        "Hau'oli City (Shopping) - Northwest of Police Station",
        "Hau'oli City (Marina) - Ferry Terminal (Outside)",
        "Route 2 - Southeast House (Outside)",
        "Route 2 - Pokemon Center (Outside)",
        "Heahea City - West",
        "Heahea City - Hotel West (Outside)",
        "Heahea City - Hotel East (Outside)",
        "Heahea City - Research Lab East (Outside)",
        "Heahea City - Research Lab South (Outside)",
        "Heahea City - Game Freak",
        "Hokulani Observatory - Dead End",
        "Heahea City - Game Freak Building (3F)",
        "Heahea City - Research Lab",
        "Heahea City - Hotel (1F)",
        "Battle Royal Dome - 2F",
        "Paniola Town - West",
        "Paniola Town - Kiawe's House (1F)",
        "Paniola Town - Kiawe's House (2F)",
        "Paniola Ranch - Northwest",
        "Paniola Ranch - Southeast",
        "Hano Beach",
        "Hano Resort - South",
        "Hano Resort - North",
        "Konikoni City Lighthouse (Through Diglett's Tunnel)",
        "Battle Royal Dome - 1F",
        "Route 8 - Aether Base (Outside)",
        "Route 8 - Fossil Restoration Center (Outside)",
        "Konikoni City - West",
        "Konikoni City - Restaurant (1F)",
        "Iki Town - Southwest",
        "Hau'oli City (Shopping) - Ilima's House Pool",
        "Wela Volcano Park - Rocks Behind Sign",
        "Route 5 - South of Pokemon Center",
        "Hano Beach - Below Sandygast",
        "Malie City (Outer Cape) - Recycling Plant (Outside)",
        "Malie City - Ferry Terminal (Outside)",
        "Malie City - Apparel Shop (Outside)",
        "Malie City - Salon (Outside)",
        "Route 16 - Aether Base (Outside)",
        "Blush Mountain - Power Plant (Outside)",
        "Malie City - Library (2F)",
        "Malie Garden - Northeast",
        "Malie City - CommunityCenter",
        "Hokulani Observatory - Outside",
        "Mount Hokulani",
        "Blush Mountain - Power Plant",
        "Route 13",
        "Route 14 - Front of Abandoned Megamart",
        "Route 14 - North",
        "Route 15 - Islet Surfboard (Outside)",
        "Route 17 - Police Station (Outside)",
        "Route 17 - Police Station",
        "Po Town - Pokemon Center (Outside)",
        "Exeggutor Island - Under Rock",
        "Po Town - Shady House East (Outside)",
        "Po Town - Pokemon Center",
        "Po Town - Shady House (1F)",
        "Route 13 - Motel (Outside)",
        "Po Town - Shady House 2F (Outside)",
        "Route 17 - South of Po Town",
        "Ula'ula Meadow",
        "Po Town - Shady House West Rocks (Outside) 1",
        "Po Town - Shady House West Rocks (Outside) 2",
        "Po Town - Shady House West Rocks (Outside) 3",
        "Seafolk Village - Southeast Whiscash (Mina's Ship) (Outside)",
        "Seafolk Village - Southwest Huntail",
        "Seafolk Village - Southwest Huntail (Outside)",
        "Seafolk Village - Southeast Whiscash (Mina's Ship)",
        "Seafolk Village - West Wailord (Restaurant)",
        "Seafolk Village - East Steelix",
        "Poni Wilds - Southeast",
        "Ancient Poni Path - Hapu's House (Kitchen)",
        "Seafolk Village - Northeast",
        "Ancient Poni Path - Hapu's House (Bedroom)",
        "Ancient Poni Path - Southwest",
        "Ancient Poni Path - Hapu's House (Courtyard)",
        "Ancient Poni Path - Hapu's House (Outside Behind Well)",
        "Ancient Poni Path - Northeast",
        "Battle Tree - Entrance",
    ];

    #endregion
}
