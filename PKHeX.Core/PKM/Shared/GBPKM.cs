using System;
using System.Diagnostics.CodeAnalysis;

namespace PKHeX.Core;

/// <summary>
/// Base format for Generation 1 &amp; 2 <see cref="PKM"/> objects.
/// </summary>
/// <remarks>
/// <see cref="SK2"/> store text buffers with the rest of the data.
/// <see cref="PK1"/> and <see cref="PK2"/> store them separately; see <see cref="GBPKML"/>.
/// </remarks>
public abstract class GBPKM : PKM
{
    public sealed override int MaxBallID => -1;
    public sealed override GameVersion MinGameID => GameVersion.RD;
    public sealed override GameVersion MaxGameID => GameVersion.C;
    public sealed override int MaxIV => 15;
    public sealed override int MaxEV => EffortValues.Max12;

    public sealed override ReadOnlySpan<ushort> ExtraBytes => [];

    protected GBPKM([ConstantExpected] int size) : base(size) { }
    protected GBPKM(byte[] data) : base(data) { }

    public sealed override byte[] EncryptedPartyData => Encrypt();
    public sealed override byte[] EncryptedBoxData => Encrypt();
    public sealed override byte[] DecryptedBoxData => Encrypt();
    public sealed override byte[] DecryptedPartyData => Encrypt();

    public override bool Valid { get => true; set { } }
    public sealed override void RefreshChecksum() { }

    private bool? _isnicknamed;
    protected abstract void GetNonNickname(int language, Span<byte> data);

    public sealed override bool IsNicknamed
    {
        get
        {
            if (_isnicknamed is {} actual)
                return actual;

            var current = NicknameTrash;
            Span<byte> expect = stackalloc byte[current.Length];
            var language = GuessedLanguage();
            GetNonNickname(language, expect);
            var result = !current.SequenceEqual(expect);
            _isnicknamed = result;
            return result;
        }
        set
        {
            _isnicknamed = value;
            if (_isnicknamed == false)
                SetNotNicknamed(GuessedLanguage());
        }
    }

    protected bool IsNicknamedBank
    {
        get
        {
            var spName = SpeciesName.GetSpeciesNameGeneration(Species, GuessedLanguage(), Format);

            Span<char> nickname = stackalloc char[TrashCharCountNickname];
            int len = LoadString(NicknameTrash, nickname);
            return !nickname[..len].SequenceEqual(spName);
        }
    }

    public sealed override int Language
    {
        get
        {
            if (Japanese)
                return (int)LanguageID.Japanese;
            if (Korean)
                return (int)LanguageID.Korean;
            if (StringConverter1.IsG12German(OriginalTrainerTrash))
                return (int)LanguageID.German; // german

            Span<char> nickname = stackalloc char[TrashCharCountNickname];
            int len = StringConverter1.LoadString(NicknameTrash, nickname, false);
            int lang = SpeciesName.GetSpeciesNameLanguage(Species, nickname[..len], Format);
            if (lang > 0)
                return lang;
            return 0;
        }
        set
        {
            if (Japanese)
                return;
            if (Korean)
                return;

            if (IsNicknamed)
                return;
            SetNotNicknamed(value);
        }
    }

    public sealed override byte Gender
    {
        get
        {
            int gv = PersonalInfo.Gender;
            return gv switch
            {
                PersonalInfo.RatioMagicGenderless => 2,
                PersonalInfo.RatioMagicFemale => 1,
                PersonalInfo.RatioMagicMale => 0,
                _ => IV_ATK > gv >> 4 ? (byte)0 : (byte)1,
            };
        }
        set { }
    }

    #region Future, Unused Attributes
    public sealed override bool IsGenderValid() => true; // not a separate property, derived via IVs
    public sealed override uint EncryptionConstant { get => 0; set { } }
    public sealed override uint PID { get => 0; set { } }
    public sealed override Nature Nature { get => 0; set { } }
    public sealed override bool ChecksumValid => true;
    public sealed override bool FatefulEncounter { get => false; set { } }
    public sealed override uint TSV => 0x0000;
    public sealed override uint PSV => 0xFFFF;
    public sealed override int Characteristic => -1;
    public sealed override int Ability { get => -1; set { } }
    public sealed override byte CurrentHandler { get => 0; set { } }
    public sealed override ushort EggLocation { get => 0; set { } }
    public sealed override byte Ball { get => 0; set { } }
    public sealed override uint ID32 { get => TID16; set => TID16 = (ushort)value; }
    public sealed override ushort SID16 { get => 0; set { } }
    #endregion

    public sealed override bool IsShiny => IV_DEF == 10 && IV_SPE == 10 && IV_SPC == 10 && (IV_ATK & 2) == 2;
    private int HPBitValPower => ((IV_ATK & 8) >> 0) | ((IV_DEF & 8) >> 1) | ((IV_SPE & 8) >> 2) | ((IV_SPC & 8) >> 3);
    public sealed override int HPPower => (((5 * HPBitValPower) + (IV_SPC & 3)) >> 1) + 31;

    public sealed override int HPType
    {
        // Get and set values directly without multiple calls to DV16.
        get => HiddenPower.GetTypeGB(DV16);
        set => DV16 = HiddenPower.SetTypeGB(value, DV16);
    }

    public sealed override byte Form
    {
        get
        {
            if (Species != 201) // Unown
                return 0;
            return GetUnownFormValue(IV_ATK, IV_DEF, IV_SPE, IV_SPC);
        }
        set
        {
            if (Species != 201) // Unown
                return;
            if (Form == value)
                return;
            var rnd = Util.Rand;
            do DV16 = (ushort)rnd.Next();
            while (Form != value);
        }
    }

    private static byte GetUnownFormValue(int atk, int def, int spe, int spc)
    {
        ushort formeVal = 0;
        formeVal |= (ushort)((atk & 0x6) << 5);
        formeVal |= (ushort)((def & 0x6) << 3);
        formeVal |= (ushort)((spe & 0x6) << 1);
        formeVal |= (ushort)((spc & 0x6) >> 1);
        return (byte)(formeVal / 10);
    }

    public abstract int EV_SPC { get; set; }
    public sealed override int EV_SPA { get => EV_SPC; set => EV_SPC = value; }
    public sealed override int EV_SPD { get => EV_SPC; set { } }
    public abstract ushort DV16 { get; set; }
    public sealed override int IV_HP { get => ((IV_ATK & 1) << 3) | ((IV_DEF & 1) << 2) | ((IV_SPE & 1) << 1) | ((IV_SPC & 1) << 0); set { } }
    public sealed override int IV_ATK { get => (DV16 >> 12) & 0xF; set => DV16 = (ushort)((DV16 & ~(0xF << 12)) | (ushort)((value > 0xF ? 0xF : value) << 12)); }
    public sealed override int IV_DEF { get => (DV16 >> 8) & 0xF; set => DV16 = (ushort)((DV16 & ~(0xF << 8)) | (ushort)((value > 0xF ? 0xF : value) << 8)); }
    public sealed override int IV_SPE { get => (DV16 >> 4) & 0xF; set => DV16 = (ushort)((DV16 & ~(0xF << 4)) | (ushort)((value > 0xF ? 0xF : value) << 4)); }
    public int IV_SPC { get => (DV16 >> 0) & 0xF; set => DV16 = (ushort)((DV16 & ~(0xF << 0)) | (ushort)((value > 0xF ? 0xF : value) << 0)); }
    public sealed override int IV_SPA { get => IV_SPC; set => IV_SPC = value; }
    public sealed override int IV_SPD { get => IV_SPC; set { } }

    public void SetNotNicknamed() => SetNotNicknamed(GuessedLanguage());
    public abstract void SetNotNicknamed(int language);

    public bool IsSpeciesNameMatch(int language)
    {
        var expect = SpeciesName.GetSpeciesNameGeneration(Species, language, 2);
        Span<char> current = stackalloc char[TrashCharCountNickname];
        int len = LoadString(NicknameTrash, current);
        return current[..len].SequenceEqual(expect);
    }

    public int GuessedLanguage(int fallback = (int)LanguageID.English)
    {
        int lang = Language;
        if (lang > 0)
            return lang;
        if (fallback is (int)LanguageID.French or (int)LanguageID.German or (int)LanguageID.Italian or (int)LanguageID.Spanish) // only other permitted besides English
            return fallback;
        return (int)LanguageID.English;
    }

    /// <summary>
    /// Tries to guess the source language ID when transferred to future generations (7+)
    /// </summary>
    /// <param name="destLanguage">Destination language ID</param>
    /// <returns>Source language ID</returns>
    protected int TransferLanguage(int destLanguage)
    {
        // if the Species name of the destination language matches the current nickname, transfer with that language.
        if (IsSpeciesNameMatch(destLanguage))
            return destLanguage;
        return GuessedLanguage(destLanguage);
    }

    public override void LoadStats(IBaseStat p, Span<ushort> stats)
    {
        var lv = CurrentLevel; // recalculate instead of checking Stat_Level
        stats[0] = (ushort)(GetStat(p.HP, IV_HP, EV_HP, lv) + (5 + lv)); // HP
        stats[1] = GetStat(p.ATK, IV_ATK, EV_ATK, lv);
        stats[2] = GetStat(p.DEF, IV_DEF, EV_DEF, lv);
        stats[3] = GetStat(p.SPE, IV_SPE, EV_SPE, lv);
        stats[4] = GetStat(p.SPA, IV_SPA, EV_SPA, lv);
        stats[5] = GetStat(p.SPD, IV_SPD, EV_SPD, lv);
    }

    protected static ushort GetStat(int baseStat, int iv, int effort, int level)
    {
        // The games store a precomputed ushort[256] i^2 table for all ushort->byte square root calculations.
        // The game then iterates to find the lowest index with a value >= input (effort).
        // With modern CPUs we can just call sqrt->ceil directly.
        // ceil(sqrt(65535)) evaluates to 256, but we're clamped to byte only.
        byte firstSquare = (byte)Math.Min(255, Math.Ceiling(Math.Sqrt(effort)));

        effort = firstSquare >> 2;
        return (ushort)((((2 * (baseStat + iv)) + effort) * level / 100) + 5);
    }

    public sealed override int GetMovePP(ushort move, int ppUpCount)
    {
        var pp = base.GetMovePP(move, 0);
        return pp + (ppUpCount * Math.Min(7, pp / 5));
    }

    public void MaxEVs() => EV_HP = EV_ATK = EV_DEF = EV_SPC = EV_SPE = MaxEV;

    /// <summary>
    /// Applies <see cref="PKM.IVs"/> to the <see cref="PKM"/> to make it shiny.
    /// </summary>
    public sealed override void SetShiny()
    {
        IV_ATK |= 2;
        IV_DEF = 10;
        IV_SPE = 10;
        IV_SPA = 10;
    }

    internal void ImportFromFuture(PKM pk)
    {
        Span<char> nickname = stackalloc char[pk.TrashCharCountNickname];
        pk.LoadString(pk.NicknameTrash, nickname);
        SetString(NicknameTrash, nickname, MaxStringLengthNickname, StringConverterOption.Clear50);

        Span<char> trainer = stackalloc char[pk.TrashCharCountTrainer];
        pk.LoadString(pk.OriginalTrainerTrash, trainer);
        SetString(OriginalTrainerTrash, nickname, MaxStringLengthTrainer, StringConverterOption.Clear50);

        IV_ATK = pk.IV_ATK / 2;
        IV_DEF = pk.IV_DEF / 2;
        IV_SPC = pk.IV_SPA / 2;
        //IV_SPD = pk.IV_ATK / 2;
        IV_SPE = pk.IV_SPE / 2;

        if (pk.HasMove((int)Move.HiddenPower))
            HPType = pk.HPType;
    }
}
