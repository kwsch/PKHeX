
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Core;

namespace PKHeX.Presentation.ViewModels;

public partial class PokemonEditorViewModel
{
    // Group 2: Health & Status
    [ObservableProperty]
    private int _statHPCurrent;

    [ObservableProperty]
    private int _statHPMax;

    [ObservableProperty]
    private int _statAlignment;

    [ObservableProperty]
    private int _hpType;

    // IVs
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Stat_HP), nameof(Stat_ATK), nameof(Stat_DEF), nameof(Stat_SPA), nameof(Stat_SPD), nameof(Stat_SPE), nameof(IVTotal))]
    private int _ivHP;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Stat_HP), nameof(Stat_ATK), nameof(Stat_DEF), nameof(Stat_SPA), nameof(Stat_SPD), nameof(Stat_SPE), nameof(IVTotal))]
    private int _ivATK;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Stat_HP), nameof(Stat_ATK), nameof(Stat_DEF), nameof(Stat_SPA), nameof(Stat_SPD), nameof(Stat_SPE), nameof(IVTotal))]
    private int _ivDEF;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Stat_HP), nameof(Stat_ATK), nameof(Stat_DEF), nameof(Stat_SPA), nameof(Stat_SPD), nameof(Stat_SPE), nameof(IVTotal))]
    private int _ivSPA;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Stat_HP), nameof(Stat_ATK), nameof(Stat_DEF), nameof(Stat_SPA), nameof(Stat_SPD), nameof(Stat_SPE), nameof(IVTotal))]
    private int _ivSPD;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Stat_HP), nameof(Stat_ATK), nameof(Stat_DEF), nameof(Stat_SPA), nameof(Stat_SPD), nameof(Stat_SPE), nameof(IVTotal))]
    private int _ivSPE;

    // EVs
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Stat_HP), nameof(Stat_ATK), nameof(Stat_DEF), nameof(Stat_SPA), nameof(Stat_SPD), nameof(Stat_SPE), nameof(EVTotal))]
    private int _evHP;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Stat_HP), nameof(Stat_ATK), nameof(Stat_DEF), nameof(Stat_SPA), nameof(Stat_SPD), nameof(Stat_SPE), nameof(EVTotal))]
    private int _evATK;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Stat_HP), nameof(Stat_ATK), nameof(Stat_DEF), nameof(Stat_SPA), nameof(Stat_SPD), nameof(Stat_SPE), nameof(EVTotal))]
    private int _evDEF;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Stat_HP), nameof(Stat_ATK), nameof(Stat_DEF), nameof(Stat_SPA), nameof(Stat_SPD), nameof(Stat_SPE), nameof(EVTotal))]
    private int _evSPA;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Stat_HP), nameof(Stat_ATK), nameof(Stat_DEF), nameof(Stat_SPA), nameof(Stat_SPD), nameof(Stat_SPE), nameof(EVTotal))]
    private int _evSPD;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Stat_HP), nameof(Stat_ATK), nameof(Stat_DEF), nameof(Stat_SPA), nameof(Stat_SPD), nameof(Stat_SPE), nameof(EVTotal))]
    private int _evSPE;

    // Hyper Training
    [ObservableProperty]
    private bool _hyperTrainedHP;

    [ObservableProperty]
    private bool _hyperTrainedATK;

    [ObservableProperty]
    private bool _hyperTrainedDEF;

    [ObservableProperty]
    private bool _hyperTrainedSPA;

    [ObservableProperty]
    private bool _hyperTrainedSPD;

    [ObservableProperty]
    private bool _hyperTrainedSPE;

    // Computed Stats — RecalculateStats() is called in the OnChanged hooks before
    // PropertyChanged fires, so _pk is already up-to-date by the time these are read.
    public int Stat_HP => _pk.Stat_HPMax;
    public int Stat_ATK => _pk.Stat_ATK;
    public int Stat_DEF => _pk.Stat_DEF;
    public int Stat_SPA => _pk.Stat_SPA;
    public int Stat_SPD => _pk.Stat_SPD;
    public int Stat_SPE => _pk.Stat_SPE;

    // Base Stats
    public int Base_HP => _pk.PersonalInfo.HP;
    public int Base_ATK => _pk.PersonalInfo.ATK;
    public int Base_DEF => _pk.PersonalInfo.DEF;
    public int Base_SPA => _pk.PersonalInfo.SPA;
    public int Base_SPD => _pk.PersonalInfo.SPD;
    public int Base_SPE => _pk.PersonalInfo.SPE;

    public int IVTotal => IvHP + IvATK + IvDEF + IvSPA + IvSPD + IvSPE;
    public int EVTotal => EvHP + EvATK + EvDEF + EvSPA + EvSPD + EvSPE;

    /// <summary>
    /// Gen7+ formats store Hyper Training flags per stat; <see cref="IHyperTrain"/> gates access.
    /// </summary>
    public bool HasHyperTraining => _pk is IHyperTrain;

    /// <summary>
    /// Hyper Training requires the current level to be at or above the format's minimum.
    /// </summary>
    public bool CanHyperTrain => _pk is IHyperTrain t && t.IsHyperTrainingAvailable() && _pk.Context.IsHyperTrainingAvailable(_pk.CurrentLevel);

    /// <summary>
    /// Gen8+ formats store the displayed-stat alignment independently of <see cref="PKM.Nature"/>.
    /// </summary>
    public bool ShowStatAlignment => _pk.Format >= 8;

    private void RecalculateStats()
    {
        if (_isLoading) return; // Don't overwrite _pk during loading

        _pk.Stat_Level = (byte)Level;
        _pk.IV_HP = IvHP;
        _pk.IV_ATK = IvATK;
        _pk.IV_DEF = IvDEF;
        _pk.IV_SPA = IvSPA;
        _pk.IV_SPD = IvSPD;
        _pk.IV_SPE = IvSPE;
        _pk.EV_HP = EvHP;
        _pk.EV_ATK = EvATK;
        _pk.EV_DEF = EvDEF;
        _pk.EV_SPA = EvSPA;
        _pk.EV_SPD = EvSPD;
        _pk.EV_SPE = EvSPE;
        if (_pk is IHyperTrain ht)
        {
            ht.HT_HP = HyperTrainedHP;
            ht.HT_ATK = HyperTrainedATK;
            ht.HT_DEF = HyperTrainedDEF;
            ht.HT_SPA = HyperTrainedSPA;
            ht.HT_SPD = HyperTrainedSPD;
            ht.HT_SPE = HyperTrainedSPE;
        }
        _pk.ResetPartyStats();
    }

    [RelayCommand]
    private void SetMaxIVs()
    {
        IvHP = 31;
        IvATK = 31;
        IvDEF = 31;
        IvSPA = 31;
        IvSPD = 31;
        IvSPE = 31;
    }

    [RelayCommand]
    private void ClearEVs()
    {
        EvHP = 0;
        EvATK = 0;
        EvDEF = 0;
        EvSPA = 0;
        EvSPD = 0;
        EvSPE = 0;
    }

    // Stat Alignment changes the displayed stats (gen8+ stores it independently of Nature).
    partial void OnStatAlignmentChanged(int value) { if (!_isLoading) { _pk.StatAlignment = (Nature)value; RecalculateStats(); Validate(); } }

    // Recalculate once per change, then let NotifyPropertyChangedFor push the new values to the UI.
    partial void OnIvHPChanged(int value) { if (!_isLoading) { RecalculateStats(); Validate(); } }
    partial void OnIvATKChanged(int value) { if (!_isLoading) { RecalculateStats(); Validate(); } }
    partial void OnIvDEFChanged(int value) { if (!_isLoading) { RecalculateStats(); Validate(); } }
    partial void OnIvSPAChanged(int value) { if (!_isLoading) { RecalculateStats(); Validate(); } }
    partial void OnIvSPDChanged(int value) { if (!_isLoading) { RecalculateStats(); Validate(); } }
    partial void OnIvSPEChanged(int value) { if (!_isLoading) { RecalculateStats(); Validate(); } }
    partial void OnEvHPChanged(int value) { if (!_isLoading) { RecalculateStats(); Validate(); } }
    partial void OnEvATKChanged(int value) { if (!_isLoading) { RecalculateStats(); Validate(); } }
    partial void OnEvDEFChanged(int value) { if (!_isLoading) { RecalculateStats(); Validate(); } }
    partial void OnEvSPAChanged(int value) { if (!_isLoading) { RecalculateStats(); Validate(); } }
    partial void OnEvSPDChanged(int value) { if (!_isLoading) { RecalculateStats(); Validate(); } }
    partial void OnEvSPEChanged(int value) { if (!_isLoading) { RecalculateStats(); Validate(); } }
    partial void OnHyperTrainedHPChanged(bool value) { if (!_isLoading) { RecalculateStats(); Validate(); } }
    partial void OnHyperTrainedATKChanged(bool value) { if (!_isLoading) { RecalculateStats(); Validate(); } }
    partial void OnHyperTrainedDEFChanged(bool value) { if (!_isLoading) { RecalculateStats(); Validate(); } }
    partial void OnHyperTrainedSPAChanged(bool value) { if (!_isLoading) { RecalculateStats(); Validate(); } }
    partial void OnHyperTrainedSPDChanged(bool value) { if (!_isLoading) { RecalculateStats(); Validate(); } }
    partial void OnHyperTrainedSPEChanged(bool value) { if (!_isLoading) { RecalculateStats(); Validate(); } }
}
