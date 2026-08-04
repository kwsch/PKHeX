using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Presentation.Models;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class GroupViewerViewModel : ViewModelBase
{
    private readonly SaveFile _sav;
    private readonly ISpriteRenderer _spriteRenderer;
    private readonly IReadOnlyList<SlotGroup> _groups;
    private readonly ISlotService? _slotService;

    [ObservableProperty]
    private int _currentGroupIndex;
    
    [ObservableProperty]
    private string _currentGroupName = string.Empty;

    [ObservableProperty]
    private ObservableCollection<string> _groupNames = [];

    [ObservableProperty]
    private ObservableCollection<SlotData> _slots = [];
    
    [ObservableProperty]
    private SlotData? _selectedSlot;

    public GroupViewerViewModel(SaveFile sav, IReadOnlyList<SlotGroup> groups, ISpriteRenderer spriteRenderer, ISlotService? slotService = null)
    {
        _sav = sav;
        _groups = groups;
        _spriteRenderer = spriteRenderer;
        _slotService = slotService;

        GroupNames = new ObservableCollection<string>(groups.Select(g => g.GroupName));
        
        // Find first group with content or default to 0
        int initialGroup = 0;
        for (int i = 0; i < groups.Count; i++)
        {
            if (groups[i].Slots.Any(p => p.Species != 0))
            {
                initialGroup = i;
                break;
            }
        }
        
        LoadGroup(initialGroup);
    }

    partial void OnCurrentGroupIndexChanged(int value)
    {
        LoadGroup(value);
    }

    private void LoadGroup(int index)
    {
        if (index < 0 || index >= _groups.Count) return;
        
        var group = _groups[index];
        CurrentGroupName = group.GroupName;

        Slots.Clear();

        for (int i = 0; i < group.Slots.Length; i++)
        {
            var pk = group.Slots[i];
            var isEmpty = pk.Species == 0;
            
            // We use SlotData to reuse the box slot style templates if possible, 
            // though Slot and Box properties might be irrelevant here. 
            // We can treat "Box" as the group index for now? Or just ignore it.
            
            Slots.Add(new SlotData
            {
                Slot = i, 
                Box = index, // Using Box to store Group Index
                Species = pk.Species,
                Sprite = _spriteRenderer.GetSprite(pk),
                IsEmpty = isEmpty,
                IsShiny = pk.IsShiny,
                Nickname = isEmpty ? string.Empty : pk.Nickname,
                SpeciesName = isEmpty ? string.Empty : StringResourceLookup.Species(pk.Species),
                Level = pk.CurrentLevel,
                Gender = (byte)pk.Gender,
                HeldItem = (ushort)pk.HeldItem,
                HeldItemName = pk.HeldItem > 0 ? StringResourceLookup.Item(pk.HeldItem) : string.Empty,
                IsEgg = pk.IsEgg,
                Form = pk.Form,
                Ability = (ushort)pk.Ability,
                AbilityName = StringResourceLookup.Ability(pk.Ability),
                Nature = (byte)pk.Nature,
                NatureName = StringResourceLookup.Nature((int)pk.Nature),
                ShowdownSummary = isEmpty ? string.Empty : new ShowdownSet(pk).Text,
            });
        }
    }

    [RelayCommand]
    private void NextGroup()
    {
        if (GroupNames.Count == 0) return;
        CurrentGroupIndex = (CurrentGroupIndex + 1) % GroupNames.Count;
    }

    [RelayCommand]
    private void PreviousGroup()
    {
        if (GroupNames.Count == 0) return;
        CurrentGroupIndex = (CurrentGroupIndex - 1 + GroupNames.Count) % GroupNames.Count;
    }

    [RelayCommand]
    private void ViewSlot(SlotData? slot)
    {
        if (slot == null || slot.IsEmpty) return;
        
        // Populate editor with this PKM.
        // Since these are "read-only" groups usually (unless Battle Box editing is supported),
        // we might just want to view it.
        // WinForms calls View.PopulateFields(pkm).
        
        // We can use the SlotService to request viewing, but we need to construct a location suitable for it?
        // SlotLocation usually expects Box/Slot or Party Slot.
        // These groups don't map directly to standard boxes always.
        // If they are Battle Boxes, they might be special.
        
        // However, we passed 'SlotService' which expects 'SlotLocation'.
        // If we can't map it to a standard box, we might need a direct way to load PKM into editor.
        // For now, let's expose an event or use a callback mechanism if SlotService isn't sufficient.
        
        // Actually, SlotService has ViewRequested event. But it needs coordinates.
        // If we just want to VIEW, perhaps we can fire an event that MainWindow listens to?
        
        // Let's assume for now we just fire an event locally, 
        // OR better, passed "OnView" action in constructor if we want tight coupling for this sub-window.
        // But to keep it MVVM, let's use an event.
        
        ViewRequested?.Invoke(slot.Box, slot.Slot); // Box here = Group Index
    }
    
    public event System.Action<int, int>? ViewRequested;
    
    public PKM GetPKM(int groupIndex, int slotIndex)
    {
        if (groupIndex < 0 || groupIndex >= _groups.Count) return _sav.BlankPKM;
        var group = _groups[groupIndex];
        if (slotIndex < 0 || slotIndex >= group.Slots.Length) return _sav.BlankPKM;
        return group.Slots[slotIndex];
    }
}
