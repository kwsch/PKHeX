using System.Linq;

namespace PKHeX.Core;

/// <summary>
/// Trade Encounter data with a fixed Catch Rate
/// </summary>
/// <remarks>
/// Generation 1 specific value used in detecting unmodified/un-traded Generation 1 Trade Encounter data.
/// Species &amp; Minimum level (legal) possible to acquire at.
/// </remarks>
public sealed record EncounterTrade1 : EncounterTradeGB
{
    public override int Generation => 1;
    public override EntityContext Context => EntityContext.Gen1;
    public override byte LevelMin => CanObtainMinGSC() ? LevelMinGSC : LevelMinRBY;

    private readonly byte LevelMinRBY;
    private readonly byte LevelMinGSC;
    public override int Location => 0;
    public override Shiny Shiny => Shiny.Random;

    public EncounterTrade1(ushort species, GameVersion game, byte rby, byte gsc) : base(species, gsc, game)
    {
        TrainerNames = StringConverter12.G1TradeOTName;

        LevelMinRBY = rby;
        LevelMinGSC = gsc;
    }

    public EncounterTrade1(ushort species, GameVersion game, byte rby) : this(species, game, rby, rby) { }

    public byte GetInitialCatchRate()
    {
        var pt = Version == GameVersion.YW ? PersonalTable.Y : PersonalTable.RB;
        return (byte)pt[Species].CatchRate;
    }

    protected override void ApplyDetails(ITrainerInfo sav, EncounterCriteria criteria, PKM pk)
    {
        base.ApplyDetails(sav, criteria, pk);
        var pk1 = (PK1)pk;
        pk1.Catch_Rate = GetInitialCatchRate();
    }

    internal bool IsNicknameValid(PKM pk)
    {
        var nick = pk.Nickname;
        if (pk.Format <= 2)
            return Nicknames.Contains(nick);

        // Converted string 1/2->7 to language specific value
        // Nicknames can be from any of the languages it can trade between.
        int lang = pk.Language;
        if (lang == 1)
        {
            // Special consideration for Hiragana strings that are transferred
            if (Version == GameVersion.YW && Species == (int)Core.Species.Dugtrio)
                return nick == "ぐりお";
            return nick == Nicknames[1];
        }

        return GetNicknameIndex(nick) >= 2;
    }

    internal bool IsTrainerNameValid(PKM pk)
    {
        string ot = pk.OT_Name;
        if (pk.Format <= 2)
            return ot == StringConverter12.G1TradeOTStr;

        // Converted string 1/2->7 to language specific value
        int lang = pk.Language;
        var tr = GetOT(lang);
        return ot == tr;
    }

    private int GetNicknameIndex(string nickname)
    {
        var nn = Nicknames;
        for (int i = 0; i < nn.Count; i++)
        {
            if (nn[i] == nickname)
                return i;
        }
        return -1;
    }

    private bool CanObtainMinGSC()
    {
        if (!ParseSettings.AllowGen1Tradeback)
            return false;
        if (Version == GameVersion.BU && EvolveOnTrade)
            return ParseSettings.AllowGBCartEra;
        return true;
    }

    private bool IsMatchLevel(PKM pk, int lvl)
    {
        if (pk is not PK1)
            return lvl >= LevelMinGSC;
        return lvl >= LevelMin;
    }

    public override bool IsMatchExact(PKM pk, EvoCriteria evo)
    {
        if (!IsMatchLevel(pk, pk.CurrentLevel)) // minimum required level
            return false;

        if (Version == GameVersion.BU)
        {
            // Encounters with this version have to originate from the Japanese Blue game.
            if (!pk.Japanese)
                return false;
            // Stadium 2 can transfer from GSC->RBY without a "Trade", thus allowing un-evolved outsiders
            if (EvolveOnTrade && !ParseSettings.AllowGBCartEra && pk.CurrentLevel < LevelMinRBY)
                return false;
        }

        return true;
    }

    protected override bool IsMatchPartial(PKM pk)
    {
        if (!IsTrainerNameValid(pk))
            return true;
        if (!IsNicknameValid(pk))
            return true;

        if (ParseSettings.AllowGen1Tradeback)
            return false;
        if (pk is not PK1 pk1)
            return false;

        var req = GetInitialCatchRate();
        return req != pk1.Catch_Rate;
    }
}
