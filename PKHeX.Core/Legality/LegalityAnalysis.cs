#define SUPPRESS

using System;
using System.Collections.Generic;
using static PKHeX.Core.LegalityAnalyzers;
using static PKHeX.Core.LegalityCheckResultCode;

namespace PKHeX.Core;

/// <summary>
/// Legality Check object containing the <see cref="CheckResult"/> data and overview values from the parse.
/// </summary>
public sealed class LegalityAnalysis
{
    /// <summary> The entity we are checking. </summary>
    internal readonly PKM Entity;

    /// <summary> The entity's <see cref="IPersonalInfo"/>, which may have been sourced from the Save File it resides on. </summary>
    /// <remarks>We store this rather than re-fetching, as some games that use the same <see cref="PKM"/> format have different values.</remarks>
    internal readonly IPersonalInfo PersonalInfo;

    private readonly List<CheckResult> Parse = new(8);

    /// <summary>
    /// Parse result list allowing view of the legality parse.
    /// </summary>
    public IReadOnlyList<CheckResult> Results => Parse;

    public bool HasResult(LegalityCheckResultCode code)
    {
        foreach (var result in Parse)
        {
            if (result.Result == code)
                return true;
        }
        return false;
    }

    public int IndexOfResult(LegalityCheckResultCode code)
    {
        for (var i = 0; i < Parse.Count; i++)
        {
            var result = Parse[i];
            if (result.Result == code)
                return i;
        }
        return -1;
    }

    /// <summary>
    /// Matched encounter data for the <see cref="Entity"/>.
    /// </summary>
    public IEncounterable EncounterMatch => Info.EncounterMatch;

    /// <summary>
    /// Original encounter data for the <see cref="Entity"/>.
    /// </summary>
    /// <remarks>
    /// Generation 1/2 <see cref="Entity"/> that are transferred forward to Generation 7 are restricted to new encounter details.
    /// By retaining their original match, more information can be provided by the parse.
    /// </remarks>
    public IEncounterable EncounterOriginal => Info.EncounterOriginal;

    /// <summary>
    /// Indicates where the <see cref="Entity"/> originated.
    /// </summary>
    public readonly StorageSlotType SlotOrigin;

    /// <summary>
    /// Indicates if all checks ran to completion.
    /// </summary>
    /// <remarks>This value is false if any checks encountered an error.</remarks>
    public readonly bool Parsed;

    /// <summary>
    /// Indicates if all checks returned a <see cref="Severity.Valid"/> result.
    /// </summary>
    public readonly bool Valid;

    /// <summary>
    /// Contains various data reused for multiple checks.
    /// </summary>
    public readonly LegalInfo Info;

    private const StorageSlotType Ignore = StorageSlotType.None;

    internal bool IsStoredSlot(StorageSlotType type) => SlotOrigin == type || SlotOrigin is Ignore;

    /// <summary>
    /// Checks the input <see cref="PKM"/> data for legality. This is the best method for checking with context, as some games do not have all Alternate Form data available.
    /// </summary>
    /// <param name="pk">Input data to check</param>
    /// <param name="table"><see cref="SaveFile"/> specific personal data</param>
    /// <param name="source">Details about where the <see cref="Entity"/> originated from.</param>
    public LegalityAnalysis(PKM pk, IPersonalTable table, StorageSlotType source = Ignore) : this(pk, table.GetFormEntry(pk.Species, pk.Form), source) { }

    /// <summary>
    /// Checks the input <see cref="PKM"/> data for legality.
    /// </summary>
    /// <param name="pk">Input data to check</param>
    /// <param name="source">Details about where the <see cref="Entity"/> originated from.</param>
    public LegalityAnalysis(PKM pk, StorageSlotType source = Ignore) : this(pk, pk.PersonalInfo, source) { }

    /// <summary>
    /// Checks the input <see cref="PKM"/> data for legality.
    /// </summary>
    /// <param name="pk">Input data to check</param>
    /// <param name="pi">Personal info to parse with</param>
    /// <param name="source">Details about where the <see cref="Entity"/> originated from.</param>
    public LegalityAnalysis(PKM pk, IPersonalInfo pi, StorageSlotType source = Ignore)
    {
        Entity = pk;
        PersonalInfo = pi;
        SlotOrigin = source;

        Info = new LegalInfo(pk, Parse);
#if SUPPRESS
        try
#endif
        {
            EncounterFinder.FindVerifiedEncounter(pk, Info);
            if (!pk.IsOriginValid)
                AddLine(Severity.Invalid, EncConditionBadSpecies, CheckIdentifier.GameOrigin);
            GetParseMethod(pk)();
            RunExternalVerifiers();
            Valid = AssertValid();
            if (!Valid)
                GenerateHints(pk);
            Parsed = true;
        }
#if SUPPRESS
        // We want to swallow any error from malformed input data from the user. The Valid state is all that we really need.
        catch (Exception e)
        {
            System.Diagnostics.Debug.WriteLine(e.Message);
            Valid = false;
            EnsureMovesPopulated(); // Moves and Relearn arrays can potentially be empty on error.
            AddLine(Severity.Invalid, Error, CheckIdentifier.Misc);
        }
#endif
    }

    private void GenerateHints(PKM pk)
    {
        if (Info.EncounterMatch is not EncounterInvalid)
            return;
        if (pk.IsUntraded && EvolutionTree.GetEvolutionTree(pk.Context).Reverse.GetReverse(pk.Species, pk.Form).First.Method.Method.IsTrade)
            AddLine(Severity.Invalid, EvoInvalid, CheckIdentifier.Evolution);
    }

    private void RunExternalVerifiers()
    {
        foreach (var ext in ExternalLegalityCheck.ExternalCheckers.Values)
            ext.Verify(this);
    }

    private bool AssertValid() => Parse.TrueForAll(chk => chk.Valid)
                                  && MoveResult.AllValid(Info.Moves)
                                  && MoveResult.AllValid(Info.Relearn);

    private void EnsureMovesPopulated()
    {
        foreach (ref var p in Info.Moves.AsSpan())
        {
            if (!p.IsParsed)
                p = MoveResult.Unobtainable();
        }

        foreach (ref var p in Info.Relearn.AsSpan())
        {
            if (!p.IsParsed)
                p = MoveResult.Unobtainable();
        }
    }

    private Action GetParseMethod(PKM pk) => GetParseMethod(GetParseFormat(pk));

    private Action GetParseMethod(LegalityParseFormat method) => method switch
    {
        LegalityParseFormat.GameBoy => ParsePK1,
        LegalityParseFormat.Gen3 => ParsePK3,
        LegalityParseFormat.Gen4 => ParsePK4,
        LegalityParseFormat.Gen5 => ParsePK5,
        LegalityParseFormat.Gen6 => ParsePK6,
        LegalityParseFormat.Gen7 => ParsePK7,
        LegalityParseFormat.Gen8 => ParsePK8,
        LegalityParseFormat.Gen9 => ParsePK9,
        _ => throw new ArgumentOutOfRangeException(nameof(method)),
    };

    private enum LegalityParseFormat
    {
        GameBoy = 1,
        Gen3 = 3,
        Gen4 = 4,
        Gen5 = 5,
        Gen6 = 6,
        Gen7 = 7,
        Gen8 = 8,
        Gen9 = 9,
    }

    private static LegalityParseFormat GetParseFormat(PKM pk)
    {
        // prior to storing GameVersion
        var format = pk.Format;
        if (format < 3)
            return LegalityParseFormat.GameBoy;

        var gen = pk.Generation;
        if (gen > 0)
        {
            if (gen is 1 or 2)
                gen = 7; // VC=>Gen7, treat as Gen7
            return (LegalityParseFormat)gen;
        }

        if (pk is PK9 { IsUnhatchedEgg: true })
            return LegalityParseFormat.Gen9;
        return (LegalityParseFormat)format;
    }

    private void ParsePK1()
    {
        Nickname.Verify(this);
        Level.Verify(this);
        Level.VerifyG1(this);
        Trainer.VerifyOTGB(this);
        MiscValues.VerifyMiscG12(this);
        MovePP.Verify(this);
        EVs.Verify(this);
        if (Entity.Format == 2)
            Item.Verify(this);
    }

    private void ParsePK3()
    {
        UpdateChecks();
        if (Entity.Format > 3)
            Transfer.VerifyTransferLegalityG3(this);

        if (Entity.Version == GameVersion.CXD)
            CXD.Verify(this);

        if (Entity.Format >= 8)
            Transfer.VerifyTransferLegalityG8(this);
    }

    private void ParsePK4()
    {
        UpdateChecks();
        if (Entity.Format > 4)
            Transfer.VerifyTransferLegalityG4(this);
        if (Entity.Format >= 8)
            Transfer.VerifyTransferLegalityG8(this);
    }

    private void ParsePK5()
    {
        UpdateChecks();
        if (Entity.Format >= 8)
            Transfer.VerifyTransferLegalityG8(this);
    }

    private void ParsePK6()
    {
        UpdateChecks();
        if (Entity.Format >= 8)
            Transfer.VerifyTransferLegalityG8(this);
    }

    private void ParsePK7()
    {
        if (Entity.VC)
            UpdateVCTransferInfo();
        UpdateChecks();
        if (Entity.Format >= 8)
            Transfer.VerifyTransferLegalityG8(this);
        else if (Entity is PB7)
            Awakening.Verify(this);
    }

    private void ParsePK8()
    {
        UpdateChecks();
        Transfer.VerifyTransferLegalityG8(this);
    }

    private void ParsePK9()
    {
        UpdateChecks();
        Transfer.VerifyTransferLegalityG8(this);
    }

    /// <summary>
    /// Adds a new Check parse value.
    /// </summary>
    /// <param name="s">Check severity</param>
    /// <param name="c">Check comment</param>
    /// <param name="i">Check type</param>
    internal void AddLine(Severity s, LegalityCheckResultCode c, CheckIdentifier i) => AddLine(CheckResult.Get(s, i, c));

    /// <summary>
    /// Adds a new Check parse value.
    /// </summary>
    /// <param name="chk">Check result to add.</param>
    public void AddLine(CheckResult chk) => Parse.Add(chk);

    private void UpdateVCTransferInfo()
    {
        var enc = (Info.EncounterOriginalGB = EncounterMatch);
        if (enc is EncounterInvalid)
            return;
        var vc = EncounterGenerator7.GetVCStaticTransferEncounter(Entity, enc.Species, Info.EvoChainsAllGens.Gen7);
        Info.EncounterMatch = vc;

        Transfer.VerifyVCEncounter(Entity, enc, vc, this);
        Transfer.VerifyTransferLegalityG12(this);
    }

    private void UpdateChecks()
    {
        PIDEC.Verify(this);
        LanguageIndex.Verify(this);
        Nickname.Verify(this);
        Trainer.Verify(this);
        TrainerID.Verify(this);
        IVs.Verify(this);
        EVs.Verify(this);
        Level.Verify(this);
        Ribbon.Verify(this);
        AbilityValues.Verify(this);
        BallIndex.Verify(this);
        FormValues.Verify(this);
        MiscValues.Verify(this);
        MovePP.Verify(this);
        GenderValues.Verify(this);
        Item.Verify(this);
        Contest.Verify(this);
        Marking.Verify(this);

        var format = Entity.Format;
        if (format is 4 or 5 or 6) // Gen 6->7 transfer removes this property.
            Gen4GroundTile.Verify(this);

        SlotType.Verify(this);
        if (format < 6)
            return;

        History.Verify(this);
        if (format < 8) // Gen 7->8 transfer removes these properties.
            ConsoleRegion.Verify(this);

        if (Entity is ITrainerMemories)
            Memory.Verify(this);
        if (Entity is ISuperTrain)
            Medal.Verify(this);

        if (format < 7)
            return;

        HyperTraining.Verify(this);

        Trash.Verify(this);
        if (format < 8)
            return;

        Mark.Verify(this);
    }
}
