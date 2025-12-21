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
            GetParseMethod()();

            foreach (var ext in ExternalLegalityCheck.ExternalCheckers.Values)
                ext.Verify(this);

            Valid = Parse.TrueForAll(chk => chk.Valid)
                    && MoveResult.AllValid(Info.Moves)
                    && MoveResult.AllValid(Info.Relearn);

            if (!Valid)
            {
                if (Info.EncounterMatch is EncounterInvalid && pk.IsUntraded && EvolutionTree.GetEvolutionTree(pk.Context).Reverse.GetReverse(pk.Species, pk.Form).First.Method.Method.IsTrade)
                    AddLine(Severity.Invalid, EvoInvalid, CheckIdentifier.Evolution);
                if (IsPotentiallyMysteryGift(Info, pk))
                    AddLine(Severity.Invalid, FatefulGiftMissing, CheckIdentifier.Fateful);
            }

            Parsed = true;
        }
#if SUPPRESS
        // We want to swallow any error from malformed input data from the user. The Valid state is all that we really need.
        catch (Exception e)
        {
            System.Diagnostics.Debug.WriteLine(e.Message);
            Valid = false;

            // Moves and Relearn arrays can potentially be empty on error.
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

            AddLine(Severity.Invalid, Error, CheckIdentifier.Misc);
        }
#endif
    }

    private static bool IsPotentiallyMysteryGift(LegalInfo info, PKM pk)
    {
        if (info.EncounterOriginal is not EncounterInvalid enc)
            return false;
        if (enc.Generation <= 3)
            return pk.Format <= 3;
        if (!pk.FatefulEncounter)
            return false;
        if (enc.Generation < 6)
            return true;
        if (!MoveResult.AllValid(info.Relearn))
            return true;
        return false;
    }

    private Action GetParseMethod()
    {
        if (Entity.Format <= 2) // prior to storing GameVersion
            return ParsePK1;

        var gen = GetParseFormat();
        return gen switch
        {
            3 => ParsePK3,
            4 => ParsePK4,
            5 => ParsePK5,
            6 => ParsePK6,

            1 => ParsePK7,
            2 => ParsePK7,
            7 => ParsePK7,

            8 => ParsePK8,
            9 => ParsePK9,

            _ => throw new ArgumentOutOfRangeException(nameof(gen)),
        };
    }

    private int GetParseFormat()
    {
        var gen = Entity.Generation;
        if (gen != 0)
            return gen;
        if (Entity is PK9 { IsUnhatchedEgg: true })
            return 9;
        return Entity.Format;
    }

    private void ParsePK1()
    {
        Nickname.Verify(this);
        Level.Verify(this);
        Level.VerifyG1(this);
        Trainer.VerifyOTGB(this);
        MiscValues.VerifyMiscG1(this);
        MovePP.Verify(this);
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
        Nickname.Verify(this);
        LanguageIndex.Verify(this);
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
        MiscValues.VerifyVersionEvolution(this);

        Trash.Verify(this);
        if (format < 8)
            return;

        Mark.Verify(this);
    }
}
