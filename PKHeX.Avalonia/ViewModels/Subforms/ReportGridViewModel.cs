using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Avalonia.ViewModels.Subforms;

/// <summary>
/// Row model for a single PKM in the report grid.
/// </summary>
public sealed class ReportRowModel
{
    public string Position { get; }
    public string Species { get; }
    public byte Level { get; }
    public string Nature { get; }
    public string Ability { get; }
    public string HeldItem { get; }
    public string Move1 { get; }
    public string Move2 { get; }
    public string Move3 { get; }
    public string Move4 { get; }
    public string OT { get; }
    public int IV_HP { get; }
    public int IV_ATK { get; }
    public int IV_DEF { get; }
    public int IV_SPA { get; }
    public int IV_SPD { get; }
    public int IV_SPE { get; }
    public int EV_HP { get; }
    public int EV_ATK { get; }
    public int EV_DEF { get; }
    public int EV_SPA { get; }
    public int EV_SPD { get; }
    public int EV_SPE { get; }
    public bool IsShiny { get; }
    public bool IsEgg { get; }
    public string Ball { get; }

    public ReportRowModel(PKM pk, GameStrings strings, string position)
    {
        Position = position;
        Species = (uint)pk.Species < (uint)strings.specieslist.Length ? strings.specieslist[pk.Species] : string.Empty;
        Level = pk.CurrentLevel;
        Nature = (uint)(byte)pk.StatNature < (uint)strings.natures.Length ? strings.natures[(byte)pk.StatNature] : string.Empty;
        Ability = (uint)pk.Ability < (uint)strings.abilitylist.Length ? strings.abilitylist[pk.Ability] : string.Empty;
        var itemStrings = strings.GetItemStrings(pk.Context);
        HeldItem = (uint)pk.HeldItem < (uint)itemStrings.Length ? itemStrings[pk.HeldItem] : string.Empty;
        Move1 = (uint)pk.Move1 < (uint)strings.movelist.Length ? strings.movelist[pk.Move1] : string.Empty;
        Move2 = (uint)pk.Move2 < (uint)strings.movelist.Length ? strings.movelist[pk.Move2] : string.Empty;
        Move3 = (uint)pk.Move3 < (uint)strings.movelist.Length ? strings.movelist[pk.Move3] : string.Empty;
        Move4 = (uint)pk.Move4 < (uint)strings.movelist.Length ? strings.movelist[pk.Move4] : string.Empty;
        OT = pk.OriginalTrainerName;
        IV_HP = pk.IV_HP;
        IV_ATK = pk.IV_ATK;
        IV_DEF = pk.IV_DEF;
        IV_SPA = pk.IV_SPA;
        IV_SPD = pk.IV_SPD;
        IV_SPE = pk.IV_SPE;
        EV_HP = pk.EV_HP;
        EV_ATK = pk.EV_ATK;
        EV_DEF = pk.EV_DEF;
        EV_SPA = pk.EV_SPA;
        EV_SPD = pk.EV_SPD;
        EV_SPE = pk.EV_SPE;
        IsShiny = pk.IsShiny;
        IsEgg = pk.IsEgg;
        Ball = (uint)pk.Ball < (uint)strings.balllist.Length ? strings.balllist[pk.Ball] : string.Empty;
    }
}

/// <summary>
/// ViewModel for the ReportGrid subform. Displays PKM data in a tabular format.
/// </summary>
public partial class ReportGridViewModel : ObservableObject
{
    private readonly SaveFile _sav;

    [ObservableProperty]
    private ObservableCollection<ReportRowModel> _rows = [];

    [ObservableProperty]
    private string _statusText = string.Empty;

    [ObservableProperty]
    private bool _isAllBoxes = true;

    /// <summary>Callback for CSV export file dialog.</summary>
    public Func<Task<string?>>? GetExportPath { get; set; }

    public ReportGridViewModel(SaveFile sav)
    {
        _sav = sav;
    }

    /// <summary>
    /// Loads PKM data from the specified box or all boxes.
    /// </summary>
    [RelayCommand]
    private void LoadData()
    {
        var strings = GameInfo.Strings;
        var list = new List<ReportRowModel>();

        if (IsAllBoxes)
        {
            for (int box = 0; box < _sav.BoxCount; box++)
            {
                var boxName = _sav is IBoxDetailNameRead n ? n.GetBoxName(box) : $"Box {box + 1}";
                for (int slot = 0; slot < _sav.BoxSlotCount; slot++)
                {
                    var pk = _sav.GetBoxSlotAtIndex(box, slot);
                    if (pk.Species == 0)
                        continue;
                    pk.Stat_Level = pk.CurrentLevel;
                    list.Add(new ReportRowModel(pk, strings, $"[{box + 1:00}] {boxName}-{slot + 1:00}"));
                }
            }
        }
        else
        {
            // Use CurrentBox if set, otherwise 0
            var box = 0;
            var boxName = _sav is IBoxDetailNameRead n ? n.GetBoxName(box) : $"Box {box + 1}";
            for (int slot = 0; slot < _sav.BoxSlotCount; slot++)
            {
                var pk = _sav.GetBoxSlotAtIndex(box, slot);
                if (pk.Species == 0)
                    continue;
                pk.Stat_Level = pk.CurrentLevel;
                list.Add(new ReportRowModel(pk, strings, $"[{box + 1:00}] {boxName}-{slot + 1:00}"));
            }
        }

        Rows = new ObservableCollection<ReportRowModel>(list);
        StatusText = $"{list.Count} entries loaded.";
    }

    /// <summary>
    /// Loads data from a specific box index.
    /// </summary>
    public void LoadCurrentBox(int boxIndex)
    {
        var strings = GameInfo.Strings;
        var list = new List<ReportRowModel>();
        var boxName = _sav is IBoxDetailNameRead n ? n.GetBoxName(boxIndex) : $"Box {boxIndex + 1}";
        for (int slot = 0; slot < _sav.BoxSlotCount; slot++)
        {
            var pk = _sav.GetBoxSlotAtIndex(boxIndex, slot);
            if (pk.Species == 0)
                continue;
            pk.Stat_Level = pk.CurrentLevel;
            list.Add(new ReportRowModel(pk, strings, $"[{boxIndex + 1:00}] {boxName}-{slot + 1:00}"));
        }

        IsAllBoxes = false;
        Rows = new ObservableCollection<ReportRowModel>(list);
        StatusText = $"{list.Count} entries loaded from {boxName}.";
    }

    /// <summary>
    /// Exports the current report data to a CSV file.
    /// </summary>
    [RelayCommand]
    private async Task ExportCsvAsync()
    {
        if (Rows.Count == 0 || GetExportPath is null)
            return;

        var path = await GetExportPath();
        if (path is null)
            return;

        try
        {
            var sb = new StringBuilder();
            // Header
            sb.AppendLine("Position,Species,Level,Nature,Ability,HeldItem,Move1,Move2,Move3,Move4,OT,IV_HP,IV_ATK,IV_DEF,IV_SPA,IV_SPD,IV_SPE,EV_HP,EV_ATK,EV_DEF,EV_SPA,EV_SPD,EV_SPE,IsShiny,IsEgg,Ball");
            foreach (var row in Rows)
            {
                sb.Append(Escape(row.Position)); sb.Append(',');
                sb.Append(Escape(row.Species)); sb.Append(',');
                sb.Append(row.Level); sb.Append(',');
                sb.Append(Escape(row.Nature)); sb.Append(',');
                sb.Append(Escape(row.Ability)); sb.Append(',');
                sb.Append(Escape(row.HeldItem)); sb.Append(',');
                sb.Append(Escape(row.Move1)); sb.Append(',');
                sb.Append(Escape(row.Move2)); sb.Append(',');
                sb.Append(Escape(row.Move3)); sb.Append(',');
                sb.Append(Escape(row.Move4)); sb.Append(',');
                sb.Append(Escape(row.OT)); sb.Append(',');
                sb.Append(row.IV_HP); sb.Append(',');
                sb.Append(row.IV_ATK); sb.Append(',');
                sb.Append(row.IV_DEF); sb.Append(',');
                sb.Append(row.IV_SPA); sb.Append(',');
                sb.Append(row.IV_SPD); sb.Append(',');
                sb.Append(row.IV_SPE); sb.Append(',');
                sb.Append(row.EV_HP); sb.Append(',');
                sb.Append(row.EV_ATK); sb.Append(',');
                sb.Append(row.EV_DEF); sb.Append(',');
                sb.Append(row.EV_SPA); sb.Append(',');
                sb.Append(row.EV_SPD); sb.Append(',');
                sb.Append(row.EV_SPE); sb.Append(',');
                sb.Append(row.IsShiny); sb.Append(',');
                sb.Append(row.IsEgg); sb.Append(',');
                sb.Append(Escape(row.Ball));
                sb.AppendLine();
            }
            await File.WriteAllTextAsync(path, sb.ToString());
            StatusText = $"Exported {Rows.Count} entries to {Path.GetFileName(path)}.";
        }
        catch (Exception ex)
        {
            StatusText = $"Export failed: {ex.Message}";
        }
    }

    private static string Escape(string value)
    {
        if (value.Contains(',') || value.Contains('"'))
            return $"\"{value.Replace("\"", "\"\"")}\"";
        return value;
    }
}
