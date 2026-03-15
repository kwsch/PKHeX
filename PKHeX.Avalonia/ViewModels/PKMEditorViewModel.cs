using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using PKHeX.Avalonia.Converters;
using PKHeX.Core;
using PKHeX.Drawing.PokeSprite.Avalonia;
using SkiaSharp;

namespace PKHeX.Avalonia.ViewModels;

/// <summary>
/// ViewModel for the PKM editor panel. Implements Pokemon data viewing and editing.
/// </summary>
public partial class PKMEditorViewModel : ObservableObject
{
    private SaveFile? _sav;

    [ObservableProperty]
    private PKM? _entity;

    [ObservableProperty]
    private Bitmap? _spriteImage;

    // Basic Info
    [ObservableProperty] private ushort _species;
    [ObservableProperty] private string _nickname = string.Empty;
    [ObservableProperty] private byte _level;
    [ObservableProperty] private byte _form;
    [ObservableProperty] private byte _gender;
    [ObservableProperty] private Nature _nature;
    [ObservableProperty] private int _ability;
    [ObservableProperty] private int _heldItem;
    [ObservableProperty] private bool _isShiny;
    [ObservableProperty] private bool _isEgg;

    // New fields
    [ObservableProperty] private string _pidHex = "00000000";
    [ObservableProperty] private uint _exp;
    [ObservableProperty] private int _friendship;
    [ObservableProperty] private int _language;

    // Stats
    [ObservableProperty] private int _hp;
    [ObservableProperty] private int _atk;
    [ObservableProperty] private int _def;
    [ObservableProperty] private int _spA;
    [ObservableProperty] private int _spD;
    [ObservableProperty] private int _spe;

    // Base Stats (read-only display)
    [ObservableProperty] private int _base_HP;
    [ObservableProperty] private int _base_ATK;
    [ObservableProperty] private int _base_DEF;
    [ObservableProperty] private int _base_SPA;
    [ObservableProperty] private int _base_SPD;
    [ObservableProperty] private int _base_SPE;

    // IVs
    [ObservableProperty] private int _iv_HP;
    [ObservableProperty] private int _iv_ATK;
    [ObservableProperty] private int _iv_DEF;
    [ObservableProperty] private int _iv_SPA;
    [ObservableProperty] private int _iv_SPD;
    [ObservableProperty] private int _iv_SPE;

    // EVs
    [ObservableProperty] private int _ev_HP;
    [ObservableProperty] private int _ev_ATK;
    [ObservableProperty] private int _ev_DEF;
    [ObservableProperty] private int _ev_SPA;
    [ObservableProperty] private int _ev_SPD;
    [ObservableProperty] private int _ev_SPE;

    // Moves
    [ObservableProperty] private ushort _move1;
    [ObservableProperty] private ushort _move2;
    [ObservableProperty] private ushort _move3;
    [ObservableProperty] private ushort _move4;

    // OT
    [ObservableProperty] private string _ot = string.Empty;
    [ObservableProperty] private ushort _tid;
    [ObservableProperty] private ushort _sid;

    [ObservableProperty]
    private bool _isInitialized;

    // ComboBox data sources — sourced from GameInfo.FilteredSources
    public IReadOnlyList<ComboItem> SpeciesList => GameInfo.FilteredSources.Species;
    public IReadOnlyList<ComboItem> NatureList => GameInfo.FilteredSources.Natures;
    public IReadOnlyList<ComboItem> HeldItemList => GameInfo.FilteredSources.Items;
    public IReadOnlyList<ComboItem> MoveList => GameInfo.FilteredSources.Moves;
    public IReadOnlyList<ComboItem> AbilityList => GameInfo.FilteredSources.Abilities;
    public IReadOnlyList<ComboItem> LanguageList => GameInfo.FilteredSources.Languages;

    // Selected ComboItem bindings
    [ObservableProperty] private ComboItem? _selectedSpecies;
    [ObservableProperty] private ComboItem? _selectedNature;
    [ObservableProperty] private ComboItem? _selectedHeldItem;
    [ObservableProperty] private ComboItem? _selectedMove1;
    [ObservableProperty] private ComboItem? _selectedMove2;
    [ObservableProperty] private ComboItem? _selectedMove3;
    [ObservableProperty] private ComboItem? _selectedMove4;
    [ObservableProperty] private ComboItem? _selectedAbility;
    [ObservableProperty] private ComboItem? _selectedLanguage;

    partial void OnSelectedSpeciesChanged(ComboItem? value)
    {
        if (value is not null)
            Species = (ushort)value.Value;
    }

    partial void OnSelectedNatureChanged(ComboItem? value)
    {
        if (value is not null)
            Nature = (Nature)value.Value;
    }

    partial void OnSelectedHeldItemChanged(ComboItem? value)
    {
        if (value is not null)
            HeldItem = value.Value;
    }

    partial void OnSelectedMove1Changed(ComboItem? value)
    {
        if (value is not null)
            Move1 = (ushort)value.Value;
    }

    partial void OnSelectedMove2Changed(ComboItem? value)
    {
        if (value is not null)
            Move2 = (ushort)value.Value;
    }

    partial void OnSelectedMove3Changed(ComboItem? value)
    {
        if (value is not null)
            Move3 = (ushort)value.Value;
    }

    partial void OnSelectedMove4Changed(ComboItem? value)
    {
        if (value is not null)
            Move4 = (ushort)value.Value;
    }

    partial void OnSelectedAbilityChanged(ComboItem? value)
    {
        if (value is not null)
            Ability = value.Value;
    }

    partial void OnSelectedLanguageChanged(ComboItem? value)
    {
        if (value is not null)
            Language = value.Value;
    }

    public void Initialize(SaveFile sav)
    {
        _sav = sav;
        IsInitialized = true;
    }

    public void PopulateFields(PKM pk)
    {
        Entity = pk;

        Species = pk.Species;
        Nickname = pk.Nickname;
        Level = (byte)pk.CurrentLevel;
        Form = pk.Form;
        Gender = pk.Gender;
        Nature = pk.Nature;
        Ability = pk.Ability;
        HeldItem = pk.HeldItem;
        IsShiny = pk.IsShiny;
        IsEgg = pk.IsEgg;

        // New fields
        PidHex = pk.PID.ToString("X8");
        Exp = pk.EXP;
        Friendship = pk.CurrentFriendship;
        Language = pk.Language;

        Hp = pk.Stat_HPCurrent;
        Atk = pk.Stat_ATK;
        Def = pk.Stat_DEF;
        SpA = pk.Stat_SPA;
        SpD = pk.Stat_SPD;
        Spe = pk.Stat_SPE;

        // Base stats
        var pi = pk.PersonalInfo;
        Base_HP = pi.HP;
        Base_ATK = pi.ATK;
        Base_DEF = pi.DEF;
        Base_SPA = pi.SPA;
        Base_SPD = pi.SPD;
        Base_SPE = pi.SPE;

        Iv_HP = pk.IV_HP;
        Iv_ATK = pk.IV_ATK;
        Iv_DEF = pk.IV_DEF;
        Iv_SPA = pk.IV_SPA;
        Iv_SPD = pk.IV_SPD;
        Iv_SPE = pk.IV_SPE;

        Ev_HP = pk.EV_HP;
        Ev_ATK = pk.EV_ATK;
        Ev_DEF = pk.EV_DEF;
        Ev_SPA = pk.EV_SPA;
        Ev_SPD = pk.EV_SPD;
        Ev_SPE = pk.EV_SPE;

        Move1 = pk.Move1;
        Move2 = pk.Move2;
        Move3 = pk.Move3;
        Move4 = pk.Move4;

        Ot = pk.OriginalTrainerName;
        Tid = pk.TID16;
        Sid = pk.SID16;

        // Look up ComboItems by matching Value
        SelectedSpecies = SpeciesList.FirstOrDefault(x => x.Value == pk.Species);
        SelectedNature = NatureList.FirstOrDefault(x => x.Value == (int)pk.Nature);
        SelectedHeldItem = HeldItemList.FirstOrDefault(x => x.Value == pk.HeldItem);
        SelectedMove1 = MoveList.FirstOrDefault(x => x.Value == pk.Move1);
        SelectedMove2 = MoveList.FirstOrDefault(x => x.Value == pk.Move2);
        SelectedMove3 = MoveList.FirstOrDefault(x => x.Value == pk.Move3);
        SelectedMove4 = MoveList.FirstOrDefault(x => x.Value == pk.Move4);
        SelectedAbility = AbilityList.FirstOrDefault(x => x.Value == pk.Ability);
        SelectedLanguage = LanguageList.FirstOrDefault(x => x.Value == pk.Language);

        UpdateSprite();
    }

    /// <summary>
    /// Writes the current ViewModel property values back to the underlying <see cref="Entity"/>
    /// and returns the prepared PKM. Used by "Set" operations to write editor data into a slot.
    /// </summary>
    public PKM? PreparePKM()
    {
        if (Entity is null)
            return null;

        Entity.Species = Species;
        Entity.Nickname = Nickname;
        Entity.Form = Form;
        Entity.Gender = Gender;
        Entity.Nature = Nature;
        Entity.Ability = Ability;
        Entity.HeldItem = HeldItem;

        // New fields
        if (uint.TryParse(PidHex, System.Globalization.NumberStyles.HexNumber, null, out var pid))
            Entity.PID = pid;
        Entity.EXP = Exp;
        Entity.CurrentFriendship = (byte)Math.Clamp(Friendship, 0, 255);
        Entity.Language = Language;

        Entity.IV_HP = Iv_HP;
        Entity.IV_ATK = Iv_ATK;
        Entity.IV_DEF = Iv_DEF;
        Entity.IV_SPA = Iv_SPA;
        Entity.IV_SPD = Iv_SPD;
        Entity.IV_SPE = Iv_SPE;

        Entity.EV_HP = Ev_HP;
        Entity.EV_ATK = Ev_ATK;
        Entity.EV_DEF = Ev_DEF;
        Entity.EV_SPA = Ev_SPA;
        Entity.EV_SPD = Ev_SPD;
        Entity.EV_SPE = Ev_SPE;

        Entity.Move1 = Move1;
        Entity.Move2 = Move2;
        Entity.Move3 = Move3;
        Entity.Move4 = Move4;

        Entity.OriginalTrainerName = Ot;
        Entity.TID16 = Tid;
        Entity.SID16 = Sid;

        Entity.IsEgg = IsEgg;

        return Entity;
    }

    private void UpdateSprite()
    {
        if (Entity is null)
            return;

        var sprite = Entity.Sprite();
        SpriteImage = SKBitmapToAvaloniaBitmapConverter.ToAvaloniaBitmap(sprite);
    }
}
