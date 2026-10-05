using System;
using FluentAssertions;
using Xunit;
using static PKHeX.Core.Species;

namespace PKHeX.Core.Tests.Editing;

/// <summary>
/// Characterizes the entity side effects of changing species or form in the PKM editor (PKMEditor.UpdateSpecies/UpdateForm/SetForms/SetAbilityList/UpdateNickname).
/// Each case runs against the replay of the WinForms handlers and against <see cref="SpeciesFormChange"/>.
/// </summary>
/// <remarks>
/// Out of scope: Gen 2 Unown (UI loop rerolling IVs until the form matches), Gen 5 Basculin-Blue's extra ability entry, HaX, form arguments, eggs, Gen 1/2 nickname trash, sprite and legality refresh.
/// </remarks>
public abstract class SpeciesFormChangeTests
{
    protected const int English = (int)LanguageID.English;
    private const uint PID = 0x12345678; // low byte 0x78: female for a PID-derived 50/50 ratio
    private const uint PIDMale = 0x123456F0; // low byte 0xF0: male for a 50/50 ratio
    private const uint EC = 0x9ABCDEF0;
    private const byte Level = 50;

    protected static readonly GameStrings Strings = GameInfo.GetStrings("en");

    /// <summary>
    /// Reselecting the current Gen 3 Unown form rerolls the PID even though it already matches.
    /// </summary>
    protected abstract bool RerollsMatchingUnownPID { get; }

    protected abstract IEditor Load(PKM pk);

    [Theory]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(6)]
    [InlineData(8)]
    public void SpeciesChangeRecomputesLevelFromExpThenSnapsExp(byte format)
    {
        var editor = Load(Create(format, Pikachu)); // Medium Fast
        var pk = editor.Entity;
        pk.EXP.Should().Be(125000);

        // Level is re-derived from the old EXP on the new growth curve, then EXP is reset to that level's minimum.
        editor.ChangeSpecies(Magikarp); // Slow
        pk.CurrentLevel.Should().Be(46);
        pk.EXP.Should().Be(121670);

        editor.ChangeSpecies(Chansey); // Fast
        pk.CurrentLevel.Should().Be(53);
        pk.EXP.Should().Be(119101);
    }

    [Theory]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(6)]
    [InlineData(8)]
    public void SpeciesChangeKeepsSecondAbilitySlot(byte format)
    {
        var pk = Create(format, Onix); // Rock Head/Sturdy
        SetAbilitySlot(pk, 1);
        pk.Ability.Should().Be((int)Ability.Sturdy);

        var editor = Load(pk);
        editor.ChangeSpecies(Chansey); // Natural Cure/Serene Grace
        pk.Ability.Should().Be((int)Ability.SereneGrace);
        pk.AbilityNumber.Should().Be(format == 4 ? 1 : 2); // Gen 4 ability number is PID-derived; the PID is not rerolled
    }

    [Fact]
    public void SpeciesChangeGen4KeepsSlotThroughSameAbilitySpecies()
    {
        var pk = Create(4, Onix);
        SetAbilitySlot(pk, 1);

        var editor = Load(pk);
        editor.ChangeSpecies(Bulbasaur); // Overgrow/Overgrow
        pk.Ability.Should().Be((int)Ability.Overgrow);

        editor.ChangeSpecies(Magnemite); // combo box index is carried, not re-derived from PID
        pk.Ability.Should().Be((int)Ability.Sturdy);
    }

    [Fact]
    public void SpeciesChangeGen3KeepsAbilityBitWhenTargetHasSingleAbility()
    {
        var pk = Create(3, Onix);
        SetAbilitySlot(pk, 1);

        Load(pk).ChangeSpecies(Bulbasaur); // Overgrow only; Gen 3 ability list always has 2 entries
        ((PK3)pk).AbilityBit.Should().BeTrue();
        pk.Ability.Should().Be((int)Ability.Overgrow);
    }

    [Theory]
    [InlineData(6)]
    [InlineData(8)]
    public void SpeciesChangeKeepsHiddenAbilitySlot(byte format)
    {
        var pk = Create(format, Bulbasaur);
        SetAbilitySlot(pk, 2);

        pk.Ability.Should().Be((int)Ability.Chlorophyll);

        Load(pk).ChangeSpecies(Magnemite);
        pk.AbilityNumber.Should().Be(4);
        pk.Ability.Should().Be((int)Ability.Analytic);
    }

    [Theory]
    [InlineData(6)]
    [InlineData(8)]
    public void FormChangeKeepsHiddenAbilitySlot(byte format)
    {
        var editor = Load(Create(format, Chansey));
        var pk = editor.Entity;
        editor.ChangeSpecies(Meowstic);
        SetAbilitySlot(pk, 2);
        pk.Ability.Should().Be((int)Ability.Prankster);

        editor = Load(pk);
        editor.ChangeForm(1);
        pk.AbilityNumber.Should().Be(4);
        pk.Ability.Should().Be((int)Ability.Competitive);
    }

    [Theory]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(6)]
    [InlineData(8)]
    public void SpeciesChangeDoesNotRerollPID(byte format)
    {
        var editor = Load(Create(format, Onix));
        var pk = editor.Entity;
        var ec = pk.EncryptionConstant;

        editor.ChangeSpecies(Magnemite);
        editor.ChangeSpecies(Chansey);
        pk.PID.Should().Be(PID);
        pk.EncryptionConstant.Should().Be(ec);
    }

    // Each row starts from a different fixed gender than the target.
    // Gen 3 is omitted: its gender is always derived from PID, so those rows cannot fail.
    [Theory]
    [InlineData(4, Chansey, Magnemite, 2)]
    [InlineData(6, Chansey, Magnemite, 2)]
    [InlineData(8, Chansey, Magnemite, 2)]
    [InlineData(4, Tauros, Chansey, 1)]
    [InlineData(6, Tauros, Chansey, 1)]
    [InlineData(8, Tauros, Chansey, 1)]
    [InlineData(4, Chansey, Tauros, 0)]
    [InlineData(6, Chansey, Tauros, 0)]
    [InlineData(8, Chansey, Tauros, 0)]
    public void SpeciesChangeToFixedGenderSetsGender(byte format, Species start, Species species, byte gender)
    {
        var editor = Load(Create(format, start));
        editor.ChangeSpecies(species);
        editor.Entity.Gender.Should().Be(gender);
    }

    [Theory]
    [InlineData(3, 0)] // PID-derived
    [InlineData(4, 0)] // stored female no longer matches PID, re-derived from PID
    [InlineData(6, 1)] // not PID-bound, female is still valid
    [InlineData(8, 1)]
    public void SpeciesChangeToDualGenderIsSane(byte format, byte expect)
    {
        var editor = Load(Create(format, Chansey, PIDMale));
        var pk = editor.Entity;
        pk.Gender.Should().Be(1);

        editor.ChangeSpecies(Onix); // 50/50
        pk.Gender.Should().Be(expect);
    }

    // Gen 6 Meowstic forms share a 50/50 ratio, so only the form's gender selection yields male.
    // Gen 8 gender forms are fixed-gender per form in personal data, so they would pass without the gender-form branch.
    [Fact]
    public void GenderFormSetsGender()
    {
        var editor = Load(Create(6, Chansey));
        var pk = editor.Entity;

        editor.ChangeSpecies(Meowstic);
        pk.Form.Should().Be(0);
        pk.Gender.Should().Be(0);

        editor.ChangeForm(1);
        pk.Form.Should().Be(1);
        pk.Gender.Should().Be(1);

        editor.ChangeForm(0);
        pk.Form.Should().Be(0);
        pk.Gender.Should().Be(0);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(17)]
    [InlineData(27)]
    public void Gen3UnownFormRerollsPID(byte form)
    {
        var editor = Load(Create(3, Pikachu));
        var pk = editor.Entity;

        editor.ChangeSpecies(Unown);
        pk.Form.Should().Be(0);

        editor.ChangeForm(form);
        pk.Form.Should().Be(form);

        var pid = pk.PID;
        editor.ChangeForm(form);
        if (RerollsMatchingUnownPID)
            pk.PID.Should().NotBe(pid);
        else
            pk.PID.Should().Be(pid);
        EntityPID.GetUnownForm3(pk.PID).Should().Be(form);
    }

    [Fact]
    public void Gen4UnownFormKeepsPID()
    {
        var editor = Load(Create(4, Pikachu));
        var pk = editor.Entity;

        editor.ChangeSpecies(Unown);
        editor.ChangeForm(17);
        pk.Form.Should().Be(17);
        pk.PID.Should().Be(PID);
    }

    [Theory]
    [InlineData(3, "IVYSAUR")]
    [InlineData(4, "IVYSAUR")]
    [InlineData(6, "Ivysaur")]
    [InlineData(8, "Ivysaur")]
    public void SpeciesChangeUpdatesDefaultNickname(byte format, string expect)
    {
        var editor = Load(Create(format, Bulbasaur));
        var pk = editor.Entity;
        pk.IsNicknamed.Should().BeFalse();

        editor.ChangeSpecies(Ivysaur);
        pk.Nickname.Should().Be(expect);
        pk.IsNicknamed.Should().BeFalse();
    }

    [Theory]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(6)]
    [InlineData(8)]
    public void SpeciesChangeKeepsNickname(byte format)
    {
        var pk = Create(format, Bulbasaur);
        pk.SetNickname("Bob");
        pk.IsNicknamed.Should().BeTrue();

        Load(pk).ChangeSpecies(Ivysaur);
        pk.Nickname.Should().Be("Bob");
        pk.IsNicknamed.Should().BeTrue();
    }

    [Theory]
    [InlineData(6)]
    [InlineData(8)]
    public void SpeciesChangeReplacesOtherLanguageNameWhenNotNicknamed(byte format)
    {
        var pk = Create(format, Bulbasaur);
        pk.Nickname = SpeciesName.GetSpeciesNameGeneration((ushort)Bulbasaur, (int)LanguageID.German, format);
        pk.IsNicknamed.Should().BeFalse();

        Load(pk).ChangeSpecies(Ivysaur);
        pk.Nickname.Should().Be("Ivysaur");
    }

    [Theory]
    [InlineData(6, Pyroar)]
    [InlineData(8, Chansey)]
    public void SpeciesChangeWithoutFormSelectionResetsForm(byte format, Species species)
    {
        var editor = Load(Create(format, Pumpkaboo));
        var pk = editor.Entity;
        editor.ChangeForm(2);
        pk.Form.Should().Be(2);

        editor.ChangeSpecies(species);
        pk.Form.Should().Be(0);
    }

    // Gen 3 is omitted: its form always reads 0 for species other than Unown.
    [Fact]
    public void Gen4SpeciesChangeFromUnownResetsForm()
    {
        var editor = Load(Create(4, Pikachu));
        var pk = editor.Entity;
        editor.ChangeSpecies(Unown);
        editor.ChangeForm(5);
        pk.Form.Should().Be(5);

        editor.ChangeSpecies(Pikachu);
        pk.Form.Should().Be(0);
    }

    [Theory]
    [InlineData(6)]
    [InlineData(8)]
    public void SpeciesChangeWithFormSelectionResetsToFirstForm(byte format)
    {
        var editor = Load(Create(format, Pumpkaboo));
        editor.ChangeForm(3);
        editor.ChangeSpecies(Gourgeist);
        editor.Entity.Form.Should().Be(0); // form list is rebound, selecting the first entry
    }

    [Theory]
    [InlineData(6)]
    [InlineData(8)]
    public void FormChangeKeepsPID(byte format)
    {
        var editor = Load(Create(format, Pumpkaboo));
        var pk = editor.Entity;
        editor.ChangeForm(3);
        pk.Form.Should().Be(3);
        pk.PID.Should().Be(PID);
    }

    protected static PKM Create(byte format, Species species, uint pid = PID) => Create(format switch
    {
        3 => nameof(PK3),
        4 => nameof(PK4),
        6 => nameof(PK6),
        8 => nameof(PK8),
        _ => throw new ArgumentOutOfRangeException(nameof(format)),
    }, species, pid);

    protected static PKM Create(string type, Species species, uint pid = PID)
    {
        PKM pk = type switch
        {
            nameof(PK3) => new PK3 { Version = GameVersion.E },
            nameof(CK3) => new CK3 { Version = GameVersion.COLO },
            nameof(XK3) => new XK3 { Version = GameVersion.CXD },
            nameof(PK4) => new PK4 { Version = GameVersion.Pt },
            nameof(BK4) => new BK4 { Version = GameVersion.HG },
            nameof(PK5) => new PK5 { Version = GameVersion.B2 },
            nameof(PK6) => new PK6 { Version = GameVersion.OR },
            nameof(PK7) => new PK7 { Version = GameVersion.US },
            nameof(PB7) => new PB7 { Version = GameVersion.GP },
            nameof(PK8) => new PK8 { Version = GameVersion.SW },
            nameof(PA8) => new PA8 { Version = GameVersion.PLA },
            nameof(PB8) => new PB8 { Version = GameVersion.BD },
            nameof(PK9) => new PK9 { Version = GameVersion.SL },
            nameof(PA9) => new PA9 { Version = GameVersion.ZA },
            _ => throw new ArgumentOutOfRangeException(nameof(type)),
        };
        pk.Species = (ushort)species;
        pk.Language = English;
        pk.PID = pid;
        if (pk.Format >= 6)
            pk.EncryptionConstant = EC;
        pk.CurrentLevel = Level;
        pk.Gender = pk.GetSaneGender();
        SetAbilitySlot(pk, 0);
        pk.ClearNickname();
        return pk;
    }

    /// <summary>
    /// Mirrors the ability combo box selection being saved back to the entity (LoadSave/EditPK3).
    /// </summary>
    protected static void SetAbilitySlot(PKM pk, int index)
    {
        switch (pk)
        {
            case G3PKM pk3:
                pk3.AbilityBit = index != 0;
                break;
            case G4PKM pk4:
                pk4.Ability = pk4.PersonalInfo.GetAbilityAtIndex(index);
                break;
            case PK5 pk5:
                pk5.Ability = pk5.PersonalInfo.GetAbilityAtIndex(index);
                pk5.HiddenAbility = index == 2;
                break;
            default: // Gen 6+
                pk.Ability = pk.PersonalInfo.GetAbilityAtIndex(index);
                pk.AbilityNumber = 1 << index;
                break;
        }
    }

    /// <summary>
    /// Mirrors the entity being saved after <c>SetAbilityList</c> reselects the ability combo box (EditPK3/EditPK5/SaveMisc4/SaveMisc6).
    /// </summary>
    /// <remarks>
    /// Gen 6+ <see cref="PKM.AbilityNumber"/> is saved from its text box, which is not updated while the list is rebound.
    /// Legends: Z-A saves the ability from the raw ability selector, which is not rebound.
    /// </remarks>
    private static void SaveAbilitySlot(PKM pk, int index)
    {
        switch (pk)
        {
            case PA9:
                break;
            case G3PKM or G4PKM or PK5:
                SetAbilitySlot(pk, index);
                break;
            default:
                pk.Ability = pk.PersonalInfo.GetAbilityAtIndex(index);
                break;
        }
    }

    /// <summary>
    /// Mirrors the ability combo box index loaded from the entity when the editor is populated (LoadMisc6/LoadAbility4/EditPK3).
    /// </summary>
    private static int GetAbilitySlot(PKM pk) => pk switch
    {
        G3PKM pk3 => pk3.AbilityBit ? 1 : 0,
        PK5 { HiddenAbility: true } => 2,
        G4PKM or PK5 => GetAbilityIndex4(pk),
        _ => AbilityVerifier.IsValidAbilityBits(pk.AbilityNumber) ? pk.AbilityNumber >> 1 : 0,
    };

    private static int GetAbilityIndex4(PKM pk)
    {
        var pi = pk.PersonalInfo;
        int index = pi.GetIndexOfAbility(pk.Ability);
        if (index < 0)
            return 0;
        if (index < 2 && ((IPersonalAbility12)pi).IsAbility12Same)
            return pk.PIDAbility;
        return index;
    }

    protected static IPersonalTable PersonalTable(PKM pk) => pk switch
    {
        G3PKM => Core.PersonalTable.E,
        BK4 => Core.PersonalTable.HGSS,
        G4PKM => Core.PersonalTable.Pt,
        PK5 => Core.PersonalTable.B2W2,
        PK6 => Core.PersonalTable.AO,
        PK7 => Core.PersonalTable.USUM,
        PB7 => Core.PersonalTable.GG,
        PK8 => Core.PersonalTable.SWSH,
        PA8 => Core.PersonalTable.LA,
        PB8 => Core.PersonalTable.BDSP,
        PK9 => Core.PersonalTable.SV,
        PA9 => Core.PersonalTable.ZA,
        _ => throw new ArgumentOutOfRangeException(nameof(pk)),
    };

    /// <summary>
    /// Species/form edits applied to a loaded entity.
    /// </summary>
    protected interface IEditor
    {
        PKM Entity { get; }
        void ChangeSpecies(Species value);
        void ChangeForm(byte form);
    }

    private static void EnsurePresent(PKM pk, ushort species, byte form)
    {
        if (!PersonalTable(pk).IsPresentInGame(species, form))
            throw new ArgumentException($"{(Species)species} is not present in format {pk.Format}.", nameof(species));
    }

    private static void EnsureFormSelection(PKM pk)
    {
        var pi = PersonalTable(pk)[pk.Species];
        if (!FormInfo.HasFormSelection(pi, pk.Species, pk.Format))
            throw new InvalidOperationException("The form combo box is only shown when selectable.");
    }

    /// <summary>
    /// Replays the WinForms handlers. Editor state that persists across species/form changes: the loaded entity and the ability combo box index.
    /// </summary>
    protected sealed class ReplayEditor : IEditor
    {
        public PKM Entity { get; }
        private int AbilitySlot;

        public ReplayEditor(PKM pk)
        {
            EnsurePresent(pk, pk.Species, pk.Form);
            Entity = pk;
            AbilitySlot = GetAbilitySlot(pk);
        }

        /// <summary>
        /// Replays PKMEditor.UpdateSpecies: species set, form list rebound (first entry selected), ability list refreshed, EXP recomputed from the level derived on the new growth curve, gender sanitized and default nickname refreshed.
        /// </summary>
        public void ChangeSpecies(Species value)
        {
            var pk = Entity;
            var species = (ushort)value;
            EnsurePresent(pk, species, 0);
            var nicknamed = pk.IsNicknamed; // CHK_NicknamedFlag state before the change; derived for Gen 3

            pk.Species = species;
            pk.Form = 0; // SetForms: cleared if no selection, otherwise the rebound list selects index 0
            ApplyForm();

            pk.Gender = pk.GetSaneGender();
            if (!nicknamed)
                UpdateNickname(pk);
        }

        /// <summary>
        /// Replays PKMEditor.UpdateForm when the form combo box selection changes.
        /// </summary>
        public void ChangeForm(byte form)
        {
            var pk = Entity;
            EnsureFormSelection(pk);

            pk.Form = form;
            ApplyForm();
        }

        private void ApplyForm()
        {
            var pk = Entity;
            // Entity.CurrentLevel is derived from the current EXP on the new species/form's growth curve.
            pk.EXP = Experience.GetEXP(pk.CurrentLevel, pk.PersonalInfo.EXPGrowth);

            if (pk.Format >= 3)
            {
                // List length is fixed per format for the formats covered here, so the clamp is inert (kept for fidelity).
                var count = pk.PersonalInfo.AbilityCount;
                AbilitySlot = Math.Clamp(AbilitySlot, 0, count - 1);
                SaveAbilitySlot(pk, AbilitySlot);
            }

            if (pk.Species == (ushort)Unown)
            {
                if (pk.Format == 3)
                    pk.SetPIDUnown3(pk.Form);
                // Gen 2 rerolls IVs until the form matches (out of scope); other formats leave gender untouched.
            }
            else if (TryGetFormGender(pk, out var gender, out var formCount))
            {
                if (formCount == 2) // Pumpkaboo forms in German are S,M,L,XL; M parses as a gender symbol
                {
                    pk.Gender = gender;
                    pk.Gender = pk.GetSaneGender();
                }
            }
            else
            {
                pk.Gender = pk.GetSaneGender();
            }
        }
    }

    /// <summary>
    /// Applies changes through <see cref="SpeciesFormChange"/>, carrying the ability slot across changes as an editor would.
    /// </summary>
    protected sealed class HelperEditor : IEditor
    {
        public PKM Entity { get; }
        private readonly int AbilityIndex;

        /// <summary>Flags returned by the last change.</summary>
        public SpeciesFormChangeResult LastResult { get; private set; }

        public HelperEditor(PKM pk)
        {
            EnsurePresent(pk, pk.Species, pk.Form);
            Entity = pk;
            AbilityIndex = pk.GetAbilitySlot();
        }

        public void ChangeSpecies(Species value)
        {
            var species = (ushort)value;
            EnsurePresent(Entity, species, 0);
            LastResult = Entity.ChangeSpeciesForm(species, 0, PersonalTable(Entity), AbilityIndex);
        }

        public void ChangeForm(byte form)
        {
            EnsureFormSelection(Entity);
            LastResult = Entity.ChangeSpeciesForm(Entity.Species, form, PersonalTable(Entity), AbilityIndex);
        }
    }

    /// <summary>
    /// Form combo box is enabled and the selected form text parses as a gender symbol (Meowstic, Indeedee, ...).
    /// </summary>
    /// <remarks>
    /// WinForms parses the localized form text; English strings are used here, so localized false positives are not covered.
    /// </remarks>
    private static bool TryGetFormGender(PKM pk, out byte gender, out int formCount)
    {
        gender = 0;
        formCount = 0;
        var pi = PersonalTable(pk)[pk.Species];
        if (!FormInfo.HasFormSelection(pi, pk.Species, pk.Format))
            return false;
        var forms = FormConverter.GetFormList(pk.Species, Strings.types, Strings.forms, GameInfo.GenderSymbolUnicode, pk.Context);
        formCount = forms.Length;
        if (formCount <= 1 || pk.Form >= formCount)
            return false;
        gender = EntityGender.GetFromString(forms[pk.Form]);
        return gender < 2;
    }

    /// <summary>
    /// Replays PKMEditor.UpdateNickname for a species change with the nicknamed flag unchecked.
    /// </summary>
    private static void UpdateNickname(PKM pk)
    {
        var species = pk.Species;
        if (species is 0 || species > pk.MaxSpeciesID)
        {
            pk.Nickname = string.Empty;
            return;
        }

        var current = pk.Nickname;
        if (!SpeciesName.IsNicknamedAnyLanguage(species, current, pk.Context))
            return; // already a species name in some language
        if (pk.Context == EntityContext.Gen5 && species <= 493 && !pk.Gen5
            && !SpeciesName.IsNicknamedAnyLanguage(species, current, EntityContext.Gen4))
            return; // transferred Gen 3/4 name retains capitalization

        pk.Nickname = SpeciesName.GetSpeciesNameGeneration(species, pk.Language, pk.Format);
    }
}

/// <summary>
/// Runs the characterization cases against the replay of the WinForms handlers.
/// </summary>
public sealed class SpeciesFormChangeReplayTests : SpeciesFormChangeTests
{
    protected override bool RerollsMatchingUnownPID => true;
    protected override IEditor Load(PKM pk) => new ReplayEditor(pk);
}

/// <summary>
/// Runs the characterization cases against <see cref="SpeciesFormChange"/>, plus checks specific to the helper.
/// </summary>
public sealed class SpeciesFormChangeHelperTests : SpeciesFormChangeTests
{
    protected override bool RerollsMatchingUnownPID => false; // only rerolled when the PID does not yield the form
    protected override IEditor Load(PKM pk) => new HelperEditor(pk);

    [Fact]
    public void SpeciesChangeReportsChangedFields()
    {
        var editor = new HelperEditor(Create(6, Pikachu));
        editor.ChangeSpecies(Magikarp); // different growth rate and ability
        editor.LastResult.Should().Be(SpeciesFormChangeResult.EXP | SpeciesFormChangeResult.Ability | SpeciesFormChangeResult.Nickname);
    }

    [Fact]
    public void GenderFormChangeReportsChangedFields()
    {
        var editor = new HelperEditor(Create(6, Chansey));
        editor.ChangeSpecies(Meowstic);
        editor.ChangeForm(1); // same first ability for both forms
        editor.LastResult.Should().Be(SpeciesFormChangeResult.Form | SpeciesFormChangeResult.Gender);
    }

    [Fact]
    public void Gen3UnownFormChangeReportsPID()
    {
        var editor = new HelperEditor(Create(3, Pikachu));
        editor.ChangeSpecies(Unown);
        var form = (byte)((EntityPID.GetUnownForm3(editor.Entity.PID) + 1) % 28);
        editor.ChangeForm(form);
        editor.LastResult.Should().Be(SpeciesFormChangeResult.Form | SpeciesFormChangeResult.PID);
    }

    /// <summary>
    /// Entity types the helper supports, with the ability slots they can store.
    /// </summary>
    public static TheoryData<string, int> TypesAndSlots()
    {
        var data = new TheoryData<string, int>();
        foreach (var type in (string[])[nameof(PK3), nameof(CK3), nameof(XK3), nameof(PK4), nameof(BK4)])
        {
            data.Add(type, 0);
            data.Add(type, 1);
        }
        foreach (var type in (string[])[nameof(PK5), nameof(PK6), nameof(PK7), nameof(PB7), nameof(PK8), nameof(PA8), nameof(PB8), nameof(PK9), nameof(PA9)])
        {
            data.Add(type, 0);
            data.Add(type, 1);
            data.Add(type, 2);
        }
        return data;
    }

    /// <summary>
    /// Compares the whole entity after every species and form change in the game, so fields the characterization cases do not assert cannot drift.
    /// </summary>
    [Theory]
    [MemberData(nameof(TypesAndSlots))]
    public void HelperMatchesReplay(string type, int slot)
    {
        foreach (var language in (int[])[English, (int)LanguageID.Japanese, (int)LanguageID.German])
            CompareAllChanges(type, slot, language);
    }

    private static void CompareAllChanges(string type, int slot, int language)
    {
        var start = Create(type, Pikachu);
        start.Language = language;
        start.ClearNickname();
        SetAbilitySlot(start, slot);
        var replay = new ReplayEditor(start.Clone());
        var helper = new HelperEditor(start.Clone());
        var table = PersonalTable(start);

        for (ushort species = 1; species <= table.MaxSpeciesID; species++)
        {
            if (!table.IsPresentInGame(species, 0))
                continue;
            if (species is (ushort)Unown && start.Format == 3)
                continue; // the replay always rerolls the PID (see Gen3UnownFormRerollsPID)
            var because = $"{type} slot {slot} language {language} -> {(Species)species}";
            replay.ChangeSpecies((Species)species);
            helper.ChangeSpecies((Species)species);
            ShouldMatch(replay.Entity, helper.Entity, because);

            if (!FormInfo.HasFormSelection(table[species], species, start.Format))
                continue;
            var count = FormConverter.GetFormList(species, Strings.types, Strings.forms, GameInfo.GenderSymbolUnicode, start.Context).Length;
            for (int i = 1; i <= count; i++)
            {
                var form = (byte)(i % count); // each form, then back to the first
                replay.ChangeForm(form);
                helper.ChangeForm(form);
                if (species is (ushort)Meowstic && start.Context is { IsMegaContext: true, Generation: >= 9 })
                {
                    // Four-entry form list: the replay never applies the gender form; the helper always does.
                    helper.Entity.Gender.Should().Be((byte)(form & 1), because);
                    replay.Entity.Gender = helper.Entity.Gender;
                }
                ShouldMatch(replay.Entity, helper.Entity, $"{because}-{form}");
            }
        }
    }

    private static void ShouldMatch(PKM expect, PKM actual, string because)
        => actual.Data.ToArray().Should().Equal(expect.Data.ToArray(), because);

    [Theory]
    [InlineData(3)]
    [InlineData(6)]
    [InlineData(8)]
    public void UnchangedSpeciesFormIsNoOp(byte format)
    {
        var pk = Create(format, Pikachu);
        pk.EXP += 500; // mid-level: a change would snap it to the level minimum
        if (format >= 6)
            pk.AbilityNumber = 0; // invalid: a change would normalize it
        var data = pk.Data.ToArray();

        pk.ChangeSpeciesForm(pk.Species, pk.Form, PersonalTable(pk)).Should().Be(SpeciesFormChangeResult.None);
        pk.Data.ToArray().Should().Equal(data);
    }

    [Theory]
    [InlineData(nameof(PK1))]
    [InlineData(nameof(PK2))]
    public void Gen12IsNotSupported(string type)
    {
        PKM pk = type == nameof(PK1) ? new PK1 { Species = (ushort)Pikachu } : new PK2 { Species = (ushort)Pikachu };
        var act = () => pk.ChangeSpeciesForm((ushort)Unown, 5, Core.PersonalTable.C);
        act.Should().Throw<NotSupportedException>();
        pk.Species.Should().Be((ushort)Pikachu);
    }

    [Theory]
    [InlineData(3, Unown, 28)] // Gen 3 Unown form is derived from the PID; no PID yields 28
    [InlineData(8, Pumpkaboo, 4)] // one past the last listed form
    public void InvalidFormThrows(byte format, Species species, byte form)
    {
        var pk = Create(format, Chansey);
        var act = () => pk.ChangeSpeciesForm((ushort)species, form, PersonalTable(pk));
        act.Should().Throw<ArgumentOutOfRangeException>().Which.ParamName.Should().Be("form");
        pk.Species.Should().Be((ushort)Chansey);
    }

    [Fact]
    public void FormIgnoredWithoutFormSelection()
    {
        var pk = Create(6, Chansey);
        pk.ChangeSpeciesForm((ushort)Onix, 5, PersonalTable(pk));
        pk.Form.Should().Be(0);
    }

    [Fact]
    public void InvalidSpeciesThrows()
    {
        var pk = Create(6, Chansey);
        var act = () => pk.ChangeSpeciesForm((ushort)(PersonalTable(pk).MaxSpeciesID + 1), 0, PersonalTable(pk));
        act.Should().Throw<ArgumentOutOfRangeException>().Which.ParamName.Should().Be("species");
        pk.Species.Should().Be((ushort)Chansey);
    }

    [Fact]
    public void PA9KeepsAbility()
    {
        var pk = Create(nameof(PA9), Pikachu);
        var ability = pk.Ability;
        var number = pk.AbilityNumber;

        var result = pk.ChangeSpeciesForm((ushort)Charmander, 0, PersonalTable(pk));
        pk.Ability.Should().Be(ability);
        pk.AbilityNumber.Should().Be(number);
        result.HasFlag(SpeciesFormChangeResult.Ability).Should().BeFalse();
    }

    [Fact]
    public void SpeciesNoneClearsNickname()
    {
        var pk = Create(6, Bulbasaur);
        pk.ChangeSpeciesForm(0, 0, PersonalTable(pk));
        pk.Nickname.Should().BeEmpty();
    }

    [Fact]
    public void InvalidAbilityNumberIsNormalizedOnChange()
    {
        var pk = Create(6, Onix);
        pk.AbilityNumber = 0; // the desktop editor keeps the invalid value from its text box

        pk.ChangeSpeciesForm((ushort)Chansey, 0, PersonalTable(pk));
        pk.AbilityNumber.Should().Be(1);
    }

    [Fact]
    public void InvalidLanguageKeepsNickname()
    {
        var pk = Create(6, Bulbasaur);
        pk.Language = 0;

        var result = pk.ChangeSpeciesForm((ushort)Ivysaur, 0, PersonalTable(pk));
        pk.Nickname.Should().Be("Bulbasaur");
        result.HasFlag(SpeciesFormChangeResult.Nickname).Should().BeFalse();
    }

    /// <summary>
    /// Forms offered by the context's form list that are missing from the save's personal table.
    /// </summary>
    [Theory]
    [InlineData(nameof(PK7), nameof(Core.PersonalTable.SM), Lycanroc, 2)] // Dusk
    [InlineData(nameof(PK7), nameof(Core.PersonalTable.SM), Marowak, 2)] // Totem
    [InlineData(nameof(PK7), nameof(Core.PersonalTable.SM), Pikachu, 7)] // Partner cap
    [InlineData(nameof(PK8), nameof(Core.PersonalTable.SWSH), Basculin, 2)] // White-Striped
    public void FormMissingFromSaveTableIsSelectable(string type, string table, Species species, byte form)
    {
        var pk = Create(type, Chansey);
        IPersonalTable personal = table == nameof(Core.PersonalTable.SM) ? Core.PersonalTable.SM : Core.PersonalTable.SWSH;
        pk.ChangeSpeciesForm((ushort)species, form, personal);
        pk.Form.Should().Be(form);
    }

    [Theory]
    [MemberData(nameof(TypesAndSlots))]
    public void GetAbilitySlotMatchesSlot(string type, int slot)
    {
        var pk = Create(type, Onix);
        SetAbilitySlot(pk, slot);
        pk.GetAbilitySlot().Should().Be(slot);
    }

    [Fact]
    public void Gen4GetAbilitySlotUsesPIDWhenAbilitiesMatch()
    {
        var pk = Create(4, Bulbasaur); // Overgrow/Overgrow
        pk.GetAbilitySlot().Should().Be(pk.PIDAbility);
    }

    [Fact]
    public void SpeciesChangeKeepsEggName()
    {
        var pk = Create(6, Bulbasaur);
        pk.IsEgg = true;
        pk.Nickname = SpeciesName.GetEggName(English, pk.Format);
        pk.IsNicknamed = false;

        var result = pk.ChangeSpeciesForm((ushort)Ivysaur, 0, PersonalTable(pk));
        pk.Nickname.Should().Be(SpeciesName.GetEggName(English, pk.Format));
        result.HasFlag(SpeciesFormChangeResult.Nickname).Should().BeFalse();
    }
}
