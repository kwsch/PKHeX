using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class KChartViewModel : ViewModelBase
{
    private readonly SaveFile _sav;
    private readonly ISpriteRenderer _spriteRenderer;

    public ObservableCollection<KChartEntryViewModel> Entries { get; } = [];

    public KChartViewModel(SaveFile sav, ISpriteRenderer spriteRenderer)
    {
        _sav = sav;
        _spriteRenderer = spriteRenderer;
        Populate();
    }

    private void Populate()
    {
        var pt = _sav.Personal;
        var strings = GameInfo.Strings;
        var species = strings.specieslist;
        
        for (ushort s = 1; s <= pt.MaxSpeciesID; s++)
        {
            var fc = pt[s, 0].FormCount;
            var formNames = fc <= 1 ? [] : FormConverter.GetFormList(s, strings.Types, strings.forms, GameInfo.GenderSymbolASCII, _sav.Context);
            
            for (byte f = 0; f < fc; f++)
            {
                if (!pt.IsPresentInGame(s, f)) continue;

                var name = f == 0 ? species[s] : $"{species[s]}-{(f < formNames.Length ? formNames[f] : f.ToString())}";
                var entry = pt.GetFormEntry(s, f);
                
                // Sprite
                var sprite = _spriteRenderer.GetSprite(s, f, 0, 0, false, _sav.Context);

                Entries.Add(new KChartEntryViewModel(s, f, name, entry, sprite, _sav.Generation));
            }
        }
    }
}

public partial class KChartEntryViewModel : ObservableObject
{
    public ushort Species { get; }
    public byte Form { get; }
    public string Name { get; }
    public byte[]? Sprite { get; }
    public string DisplayID { get; }
    public string BST { get; }
    public string CatchRate { get; }
    
    // Stats
    public int HP { get; }
    public int ATK { get; }
    public int DEF { get; }
    public int SPA { get; }
    public int SPD { get; }
    public int SPE { get; }

    // Types
    public string Type1 { get; }
    public string Type2 { get; }

    // Abilities
    public string Ability1 { get; }
    public string Ability2 { get; }
    public string AbilityH { get; }

    public KChartEntryViewModel(ushort species, byte form, string name, IPersonalInfo info, byte[]? sprite, int generation)
    {
        Species = species;
        Form = form;
        Name = name;
        Sprite = sprite;
        DisplayID = generation >= 9 ? $"{species:0000}" : $"{species:000}" + (form > 0 ? $"-{form:00}" : "");
        BST = info.BST.ToString("000");
        CatchRate = info.CatchRate.ToString("000");
        
        HP = info.HP;
        ATK = info.ATK;
        DEF = info.DEF;
        SPA = info.SPA;
        SPD = info.SPD;
        SPE = info.SPE;

        Type1 = ((MoveType)info.Type1).ToString(); // Or localized
        Type2 = info.Type1 == info.Type2 ? "" : ((MoveType)info.Type2).ToString();
        
        var abils = GameInfo.Strings.abilitylist;
        int count = info.AbilityCount;
        Ability1 = count > 0 ? abils[info.GetAbilityAtIndex(0)] : "-";
        Ability2 = count > 1 ? abils[info.GetAbilityAtIndex(1)] : "-";
        AbilityH = count > 2 ? abils[info.GetAbilityAtIndex(2)] : "-";
    }
}
