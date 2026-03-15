using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Model for a single species entry in the KChart.
/// </summary>
public sealed class KChartEntry
{
    public string Index { get; }
    public string Name { get; }
    public bool IsNative { get; }
    public int BST { get; }
    public int HP { get; }
    public int ATK { get; }
    public int DEF { get; }
    public int SPA { get; }
    public int SPD { get; }
    public int SPE { get; }
    public string Type1Name { get; }
    public string Type2Name { get; }
    public string Ability1 { get; }
    public string Ability2 { get; }
    public string AbilityH { get; }
    public int CatchRate { get; }

    public KChartEntry(ushort species, byte form, string name, PersonalInfo p, string[] abilities, string[] types, string numberFormat)
    {
        Index = species.ToString(numberFormat) + (form > 0 ? $"-{form:00}" : string.Empty);
        Name = name;
        IsNative = true;
        BST = p.BST;
        HP = p.HP;
        ATK = p.ATK;
        DEF = p.DEF;
        SPA = p.SPA;
        SPD = p.SPD;
        SPE = p.SPE;
        CatchRate = p.CatchRate;
        Type1Name = p.Type1 < types.Length ? types[p.Type1] : p.Type1.ToString();
        Type2Name = p.Type1 == p.Type2 ? string.Empty : (p.Type2 < types.Length ? types[p.Type2] : p.Type2.ToString());
        var abils = p.AbilityCount;
        Ability1 = abils > 0 ? abilities[p.GetAbilityAtIndex(0)] : string.Empty;
        Ability2 = abils > 1 ? abilities[p.GetAbilityAtIndex(1)] : string.Empty;
        AbilityH = abils > 2 ? abilities[p.GetAbilityAtIndex(2)] : string.Empty;
    }
}

/// <summary>
/// ViewModel for the KChart subform.
/// Displays a read-only table of species base stats.
/// </summary>
public partial class KChartViewModel : ObservableObject
{
    public ObservableCollection<KChartEntry> Entries { get; } = [];

    [ObservableProperty]
    private string _statusText = string.Empty;

    public KChartViewModel(SaveFile sav)
    {
        var pt = sav.Personal;
        var strings = GameInfo.Strings;
        var species = strings.specieslist;
        var abilities = strings.abilitylist;
        var types = strings.types;
        var numberFormat = sav.Generation >= 9 ? "0000" : "000";

        for (ushort s = 1; s <= pt.MaxSpeciesID; s++)
        {
            var fc = pt[s, 0].FormCount;
            var formNames = fc <= 1
                ? []
                : FormConverter.GetFormList(s, types, strings.forms, GameInfo.GenderSymbolASCII, sav.Context);

            for (byte f = 0; f < fc; f++)
            {
                if (!pt.IsPresentInGame(s, f))
                    continue;

                var p = pt.GetFormEntry(s, f);
                var name = f == 0 ? species[s] : $"{species[s]}-{(f < formNames.Length ? formNames[f] : f.ToString())}";
                Entries.Add(new KChartEntry(s, f, name, p, abilities, types, numberFormat));
            }
        }

        StatusText = $"{Entries.Count} entries loaded.";
    }
}
