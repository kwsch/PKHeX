using System;
using FluentAssertions;
using Xunit;
using static PKHeX.Core.Species;

namespace PKHeX.Core.Tests.Editing;

/// <summary>
/// Characterizes the entity side effects of changing species or form in the PKM editor
/// (PKMEditor.UpdateSpecies/UpdateForm/SetForms/SetAbilityList/UpdateNickname), replayed with Core calls.
/// </summary>
/// <remarks>
/// Out of scope: Gen 2 Unown (UI loop rerolling IVs until the form matches), Gen 5 Basculin-Blue's extra ability entry,
/// HaX, form arguments, eggs, Gen 1/2 nickname trash, sprite and legality refresh.
/// </remarks>
public class SpeciesFormChangeTests
{
    private const int English = (int)LanguageID.English;
    private const uint PID = 0x12345678; // low byte 0x78: female for a PID-derived 50/50 ratio
    private const uint PIDMale = 0x123456F0; // low byte 0xF0: male for a 50/50 ratio
    private const uint EC = 0x9ABCDEF0;
    private const byte Level = 50;

    private static readonly GameStrings Strings = GameInfo.GetStrings("en");

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

        // Reselecting the current form still rerolls the PID.
        var pid = pk.PID;
        editor.ChangeForm(form);
        pk.PID.Should().NotBe(pid);
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

    private static PKM Create(byte format, Species species, uint pid = PID)
    {
        PKM pk = format switch
        {
            3 => new PK3 { Version = GameVersion.E },
            4 => new PK4 { Version = GameVersion.Pt },
            6 => new PK6 { Version = GameVersion.OR },
            8 => new PK8 { Version = GameVersion.SW },
            _ => throw new ArgumentOutOfRangeException(nameof(format)),
        };
        pk.Species = (ushort)species;
        pk.Language = English;
        pk.PID = pid;
        if (format >= 6)
            pk.EncryptionConstant = EC;
        pk.CurrentLevel = Level;
        pk.Gender = pk.GetSaneGender();
        SetAbilitySlot(pk, 0);
        pk.ClearNickname();
        return pk;
    }

    private static Editor Load(PKM pk) => new(pk);

    /// <summary>
    /// Mirrors the ability combo box selection being saved back to the entity (LoadSave/EditPK3).
    /// </summary>
    private static void SetAbilitySlot(PKM pk, int index)
    {
        switch (pk)
        {
            case PK3 pk3:
                pk3.AbilityBit = index != 0;
                break;
            case PK4 pk4:
                pk4.Ability = pk4.PersonalInfo.GetAbilityAtIndex(index);
                break;
            default: // Gen 6+
                pk.Ability = pk.PersonalInfo.GetAbilityAtIndex(index);
                pk.AbilityNumber = 1 << index;
                break;
        }
    }

    /// <summary>
    /// Mirrors the ability combo box index loaded from the entity when the editor is populated (LoadMisc6/LoadAbility4/EditPK3).
    /// </summary>
    private static int GetAbilitySlot(PKM pk) => pk switch
    {
        PK3 pk3 => pk3.AbilityBit ? 1 : 0,
        PK4 => GetAbilityIndex4(pk),
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

    private static IPersonalTable PersonalTable(PKM pk) => pk.Format switch
    {
        3 => Core.PersonalTable.E,
        4 => Core.PersonalTable.Pt,
        6 => Core.PersonalTable.AO,
        8 => Core.PersonalTable.SWSH,
        _ => throw new ArgumentOutOfRangeException(nameof(pk)),
    };

    /// <summary>
    /// Editor state that persists across species/form changes: the loaded entity and the ability combo box index.
    /// </summary>
    private sealed class Editor
    {
        public PKM Entity { get; }
        private int AbilitySlot;

        public Editor(PKM pk)
        {
            if (!PersonalTable(pk).IsPresentInGame(pk.Species, pk.Form))
                throw new ArgumentException($"{(Species)pk.Species} is not present in format {pk.Format}.", nameof(pk));
            Entity = pk;
            AbilitySlot = GetAbilitySlot(pk);
        }

        /// <summary>
        /// Replays PKMEditor.UpdateSpecies: species set, form list rebound (first entry selected), ability list refreshed,
        /// EXP recomputed from the level derived on the new growth curve, gender sanitized and default nickname refreshed.
        /// </summary>
        public void ChangeSpecies(Species value)
        {
            var pk = Entity;
            var species = (ushort)value;
            if (!PersonalTable(pk).IsPresentInGame(species, 0))
                throw new ArgumentException($"{value} is not present in format {pk.Format}.", nameof(value));
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
            var pi = PersonalTable(pk)[pk.Species];
            if (!FormInfo.HasFormSelection(pi, pk.Species, pk.Format))
                throw new InvalidOperationException("The form combo box is only shown when selectable.");

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
                SetAbilitySlot(pk, AbilitySlot);
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
