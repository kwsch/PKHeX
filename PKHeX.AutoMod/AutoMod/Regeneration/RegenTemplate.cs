// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

using System;
using System.Linq;
using System.Text;

namespace PKHeX.Core.AutoMod;

public sealed class RegenTemplate : IBattleTemplate
{
    public ushort Species { get; set; }
    public EntityContext Context { get; set; }
    public string Nickname { get; set; }
    public byte? Gender { get; set; }
    public int HeldItem { get; set; }
    public int Ability { get; set; }
    public byte Level { get; set; }
    public bool Shiny { get; set; }
    public byte Friendship { get; set; }
    public Nature Nature { get; set; }
    public string FormName { get; set; }
    public byte Form { get; set; }
    public sbyte HiddenPowerType { get; set; }
    public bool CanGigantamax { get; set; }
    public byte DynamaxLevel { get; set; }
    public MoveType TeraType { get; set; }

    public int[] EVs { get; }
    public int[] IVs { get; }
    public ushort[] Moves { get; set; }

    public RegenSet Regen { get; set; } = RegenSet.Default;
    public string Text => GetSummary();

    private readonly string ParentLines;

    private RegenTemplate(ShowdownSet set, byte gen = Latest.Generation, string _ = "")
    {
        Species = set.Species;
        Context = set.Context;
        Nickname = set.Nickname;
        Gender = set.Gender;
        HeldItem = set.HeldItem;
        Ability = set.Ability;
        Level = (set.Level == 50 && APILegality.ForceLevel100for50) ? (byte)100 : set.Level;
        Shiny = set.Shiny;
        Friendship = set.Friendship;
        Nature = set.Nature;
        FormName = set.FormName;
        Form = set.Form;
        EVs = SanitizeEVs(set.EVs, gen);
        IVs = set.IVs;
        HiddenPowerType = set.HiddenPowerType;
        Moves = set.Moves;
        CanGigantamax = set.CanGigantamax;
        DynamaxLevel = set.DynamaxLevel;
        TeraType = set.TeraType;

        ParentLines = set.GetText(APILegality.ExportFormat == BattleTemplateDisplayStyle.Legacy ? BattleTemplateExportSettings.CommunityStandard : BattleTemplateExportSettings.Showdown);
        SanitizeMoves(set, Moves);
    }

    public RegenTemplate(ShowdownSet set, byte gen = Latest.Generation) : this(set, gen, set.Text)
    {
        this.SanitizeForm(gen);
        this.SanitizeBattleMoves();
        this.SanitizeTeraTypes();

        var shiny = Shiny ? Core.Shiny.Always : Core.Shiny.Never;
        if (set.InvalidLines.Count == 0)
        {
            Regen.Extra.ShinyType = shiny;
            return;
        }

        Regen = new RegenSet(set.InvalidLines, gen, shiny);
        Shiny = Regen.Extra.IsShiny;
        if (Ability == -1)
            Ability = RegenUtil.GetRegenAbility(set.Species, gen, Regen.Extra.Ability);
    }

    public RegenTemplate(PKM pk, byte gen = Latest.Generation) : this(new ShowdownSet(pk), gen)
    {
        this.FixGender(pk.PersonalInfo);
        if (!pk.IsNicknamed)
            Nickname = string.Empty;

        Regen = new RegenSet(pk);
        Shiny = Regen.Extra.IsShiny;
    }

    private static int[] SanitizeEVs(ReadOnlySpan<int> evs, byte gen)
    {
        var copy = evs.ToArray();
        int maxEV = gen >= 6 ? 252 : gen >= 3 ? 255 : 65535;
        for (int i = 0; i < evs.Length; i++)
        {
            if (copy[i] > maxEV)
                copy[i] = maxEV;
        }
        return copy;
    }

    private static void SanitizeMoves<T>(T set, Span<ushort> moves) where T : IBattleTemplate
    {
        // Specified moveset, no need to sanitize
        if (moves[0] != 0)
            return;

        // Sanitize keldeo moves to avoid form mismatches
        if (set.Species == (ushort)Core.Species.Keldeo)
            moves[0] = set.Form == 0 ? (ushort)Move.AquaJet : (ushort)Move.SecretSword;
    }

    private string GetSummary()
    {
        var sb = new StringBuilder();
        var text = ParentLines;
        var regen = Regen.GetSummary();
        bool hasRegen = !string.IsNullOrWhiteSpace(regen);

        // Add Showdown content except moves
        var split = text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        var group = split.Where(z => !IsIgnored(z, Regen)).GroupBy(z => z.StartsWith("- ")).ToArray();
        if (group.Length == 0)
            return sb.ToString();

        sb.AppendJoin(Environment.NewLine, group[0]).AppendLine(); // Not Moves

        // Add non-Showdown content
        if (hasRegen)
            sb.AppendLine(regen.Trim());

        // Add Moves
        if (group.Length > 1)
            sb.AppendJoin(Environment.NewLine, group[1]).AppendLine(); // Moves

        return sb.ToString();
    }

    private static bool IsIgnored(ReadOnlySpan<char> s, RegenSet regen)
    {
        return regen.HasExtraSettings && s.StartsWith("Shiny: ");
    }
}
