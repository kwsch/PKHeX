using System.Collections.Generic;
using static PKHeX.Core.LegalityCheckStrings;
using static PKHeX.Core.Ball;

namespace PKHeX.Core;

/// <summary>
/// Verifies the <see cref="PKM.Ball"/> value.
/// </summary>
public sealed class BallVerifier : Verifier
{
    protected override CheckIdentifier Identifier => CheckIdentifier.Ball;
    private CheckResult NONE => GetInvalid(LBallNone);

    public override void Verify(LegalityAnalysis data)
    {
        if (data.Entity.Format <= 2)
            return; // no ball info saved
        var result = VerifyBall(data);
        data.AddLine(result);
    }

    private static int IsReplacedBall(IVersion enc, PKM pk) => pk switch
    {
        PK8 when enc.Version == GameVersion.PLA => (int)Poke,
        _ => (int)None,
    };

    private CheckResult VerifyBall(LegalityAnalysis data)
    {
        var Info = data.Info;
        var enc = Info.EncounterMatch;

        var ball = IsReplacedBall(enc, data.Entity);
        if (ball != 0)
            return VerifyBallEquals(data, ball);

        // Fixed ball cases -- can be only one ball ever
        switch (enc)
        {
            case MysteryGift g:
                return VerifyBallMysteryGift(data, g);
            case EncounterTrade t:
                return VerifyBallEquals(data, t.Ball);
            case EncounterStatic {Gift: true} s:
                return VerifyBallEquals(data, s.Ball);
            case EncounterSlot8GO: // Already a strict match
                return GetResult(true);
            case EncounterSlot8b {IsMarsh: true}:
                return VerifyBallEquals(data, (int)Safari);
        }

        // Capture / Inherit cases -- can be one of many balls
        var pk = data.Entity;
        if (pk.Species == (int)Species.Shedinja && enc.Species != (int)Species.Shedinja) // Shedinja. For gen3, copy the ball from Nincada
        {
            // Only Gen3 origin Shedinja can copy the wild ball.
            // Evolution chains will indicate if it could have existed as Shedinja in Gen3.
            // The special move verifier has a similar check!
            if (pk.HGSS && pk.Ball == (int)Sport) // Can evolve in DP to retain the HG/SS ball -- not able to be captured in any other ball
                return VerifyBallEquals(data, (int)Sport);
            if (Info.Generation != 3 || Info.EvoChainsAllGens.Gen3.Length != 2)
                return VerifyBallEquals(data, (int)Poke); // Pokeball Only
        }

        if (pk.Ball == (int)Heavy && BallBreedLegality.AlolanCaptureNoHeavyBall.Contains(enc.Species) && !enc.EggEncounter && pk.SM)
            return GetInvalid(LBallHeavy); // Heavy Ball, can inherit if from egg (USUM fixed catch rate calc)

        return enc switch
        {
            EncounterStatic e => VerifyBallStatic(data, e),
            EncounterSlot w => VerifyBallWild(data, w),
            EncounterEgg => VerifyBallEgg(data),
            EncounterInvalid => VerifyBallEquals(data, pk.Ball), // ignore ball, pass whatever
            _ => VerifyBallEquals(data, (int)Poke),
        };
    }

    private CheckResult VerifyBallMysteryGift(LegalityAnalysis data, MysteryGift g)
    {
        if (g.Generation == 4 && g.Species == (int)Species.Manaphy && g.Ball == 0) // there is no ball data in Manaphy Mystery Gift from Gen4
            return VerifyBallEquals(data, (int)Poke); // Pokeball
        return VerifyBallEquals(data, g.Ball);
    }

    private CheckResult VerifyBallStatic(LegalityAnalysis data, EncounterStatic s)
    {
        if (s.Location == 75 && s.Generation == 5) // Entree Forest (Dream World)
            return VerifyBallEquals(data, BallUseLegality.DreamWorldBalls);
        return VerifyBallEquals(data, BallUseLegality.GetWildBalls(data.Info.Generation, s.Version));
    }

    private CheckResult VerifyBallWild(LegalityAnalysis data, EncounterSlot w)
    {
        var req = w.FixedBall;
        if (req != None)
            return VerifyBallEquals(data, (int) req);

        return VerifyBallEquals(data, BallUseLegality.GetWildBalls(data.Info.Generation, w.Version));
    }

    private CheckResult VerifyBallEgg(LegalityAnalysis data)
    {
        var pk = data.Entity;
        if (data.Info.Generation < 6) // No inheriting Balls
            return VerifyBallEquals(data, (int)Poke); // Must be Pokéball -- no ball inheritance.

        return pk.Ball switch
        {
            (int)Master => GetInvalid(LBallEggMaster), // Master Ball
            (int)Cherish => GetInvalid(LBallEggCherish), // Cherish Ball
            _ => VerifyBallInherited(data),
        };
    }

    private CheckResult VerifyBallInherited(LegalityAnalysis data) => data.Info.Generation switch
    {
        6 => VerifyBallEggGen6(data), // Gen6 Inheritance Rules
        7 => VerifyBallEggGen7(data), // Gen7 Inheritance Rules
        8 => data.Entity.BDSP ? VerifyBallEggGen8BDSP(data) : VerifyBallEggGen8(data),
        _ => NONE,
    };

    private CheckResult VerifyBallEggGen6(LegalityAnalysis data)
    {
        var pk = data.Entity;
        if (pk.Ball == (int)Poke)
            return GetValid(LBallEnc); // Poké Ball

        var enc = data.EncounterMatch;
        int species = enc.Species;
        if (pk.Gender == 2 || BallBreedLegality.BreedMaleOnly6.Contains(species)) // Genderless
            return VerifyBallEquals(data, (int)Poke); // Must be Pokéball as ball can only pass via mother (not Ditto!)

        Ball ball = (Ball)pk.Ball;

        if (ball == Safari) // Safari Ball
        {
            if (!BallBreedLegality.Inherit_Safari.Contains(species))
                return GetInvalid(LBallSpecies);
            if (IsHiddenAndNotPossible(pk))
                return GetInvalid(LBallAbility);
            return GetValid(LBallSpeciesPass);
        }
        if (ball.IsApricornBall()) // Apricorn Ball
        {
            if (!BallBreedLegality.Inherit_Apricorn6.Contains(species))
                return GetInvalid(LBallSpecies);
            if (IsHiddenAndNotPossible(pk))
                return GetInvalid(LBallAbility);
            return GetValid(LBallSpeciesPass);
        }
        if (ball == Sport) // Sport Ball
        {
            if (!BallBreedLegality.Inherit_Sport.Contains(species))
                return GetInvalid(LBallSpecies);
            if (IsHiddenAndNotPossible(pk))
                return GetInvalid(LBallAbility);
            return GetValid(LBallSpeciesPass);
        }
        if (ball == Dream) // Dream Ball
        {
            if (BallBreedLegality.Ban_DreamHidden.Contains(species) && IsHiddenAndNotPossible(pk))
                return GetInvalid(LBallAbility);
            if (BallBreedLegality.Inherit_Dream.Contains(species))
                return GetValid(LBallSpeciesPass);
            return GetInvalid(LBallSpecies);
        }
        if (ball is >= Dusk and <= Quick) // Dusk Heal Quick
        {
            if (!BallBreedLegality.Ban_Gen4Ball_6.Contains(species))
                return GetValid(LBallSpeciesPass);
            return GetInvalid(LBallSpecies);
        }
        if (ball is >= Ultra and <= Premier) // Don't worry, Safari was already checked.
        {
            if (BallBreedLegality.Ban_Gen3Ball.Contains(species))
                return GetInvalid(LBallSpecies);
            if (BallBreedLegality.Ban_Gen3BallHidden.Contains(species | (enc.Form << 11)) && IsHiddenAndNotPossible(pk))
                return GetInvalid(LBallAbility);
            return GetValid(LBallSpeciesPass);
        }

        if (species > 650 && species != 700) // Sylveon
        {
            if (BallUseLegality.WildPokeballs6.Contains(pk.Ball))
                return GetValid(LBallSpeciesPass);
            return GetInvalid(LBallSpecies);
        }

        if (ball >= Dream)
            return GetInvalid(LBallUnavailable);

        return NONE;
    }

    private CheckResult VerifyBallEggGen7(LegalityAnalysis data)
    {
        var pk = data.Entity;
        if (pk.Ball == (int)Poke)
            return GetValid(LBallEnc); // Poké Ball

        int species = data.EncounterMatch.Species;
        if (species is >= 722 and <= 730) // G7 Starters
            return VerifyBallEquals(data, (int)Poke);

        Ball ball = (Ball)pk.Ball;

        if (ball == Safari)
        {
            if (!(BallBreedLegality.Inherit_Safari.Contains(species) || BallBreedLegality.Inherit_SafariMale.Contains(species)))
                return GetInvalid(LBallSpecies);
            if (BallBreedLegality.Ban_SafariBallHidden_7.Contains(species) && IsHiddenAndNotPossible(pk))
                return GetInvalid(LBallAbility);
            return GetValid(LBallSpeciesPass);
        }
        if (ball.IsApricornBall()) // Apricorn Ball
        {
            if (!BallBreedLegality.Inherit_Apricorn7.Contains(species))
                return GetInvalid(LBallSpecies);
            if (BallBreedLegality.Ban_NoHidden7Apricorn.Contains(species | (pk.Form << 11)) && IsHiddenAndNotPossible(pk))
                return GetInvalid(LBallAbility);
            return GetValid(LBallSpeciesPass);
        }
        if (ball == Sport) // Sport Ball
        {
            if (!BallBreedLegality.Inherit_Sport.Contains(species))
                return GetInvalid(LBallSpecies);
            if ((species is (int)Species.Volbeat or (int)Species.Illumise) && IsHiddenAndNotPossible(pk)) // Volbeat/Illumise
                return GetInvalid(LBallAbility);
            return GetValid(LBallSpeciesPass);
        }
        if (ball == Dream) // Dream Ball
        {
            if (BallBreedLegality.Inherit_Dream.Contains(species) || BallBreedLegality.Inherit_DreamMale.Contains(species))
                return GetValid(LBallSpeciesPass);
            return GetInvalid(LBallSpecies);
        }
        if (ball is >= Dusk and <= Quick) // Dusk Heal Quick
        {
            if (!BallBreedLegality.Ban_Gen4Ball_7.Contains(species))
                return GetValid(LBallSpeciesPass);
            return GetInvalid(LBallSpecies);
        }
        if (ball is >= Ultra and <= Premier) // Don't worry, Safari was already checked.
        {
            if (!BallBreedLegality.Ban_Gen3Ball_7.Contains(species))
                return GetValid(LBallSpeciesPass);
            return GetInvalid(LBallSpecies);
        }

        if (ball == Beast)
        {
            if (species == (int)Species.Flabébé && pk.Form == 3 && IsHiddenAndNotPossible(pk))
                return GetInvalid(LBallAbility); // Can't obtain Flabébé-Blue with Hidden Ability in wild
            if (species == (int)Species.Voltorb && IsHiddenAndNotPossible(pk))
                return GetInvalid(LBallAbility); // Can't obtain with Hidden Ability in wild (can only breed with Ditto)
            if ((species is >= (int)Species.Pikipek and <= (int)Species.Kommoo) || BallBreedLegality.AlolanCaptureOffspring.Contains(species))
                return GetValid(LBallSpeciesPass);
            if (BallBreedLegality.PastGenAlolanScans.Contains(species))
                return GetValid(LBallSpeciesPass);
            // next statement catches all new alolans
        }

        if (species > (int)Species.Volcanion)
            return VerifyBallEquals(data, BallUseLegality.WildPokeballs7);

        if (ball > Beast)
            return GetInvalid(LBallUnavailable);

        return NONE;
    }

    private CheckResult VerifyBallEggGen8BDSP(LegalityAnalysis data)
    {
        int species = data.EncounterMatch.Species;
        if (species == (int)Species.Phione)
            return VerifyBallEquals(data, (int)Poke);

        if (species is (int)Species.Cranidos or (int)Species.Shieldon)
            return VerifyBallEquals(data, BallUseLegality.DreamWorldBalls);

        var pk = data.Entity;
        Ball ball = (Ball)pk.Ball;
        var balls = BallUseLegality.GetWildBalls(8, GameVersion.BDSP);
        if (balls.Contains((int)ball))
            return GetValid(LBallSpeciesPass);

        if (species is (int)Species.Spinda)
            return GetInvalid(LBallSpecies); // Can't enter or exit, needs to adhere to wild balls.

        // Cross-game inheritance
        if (IsGalarCatchAndBreed(species))
        {
            if (BallUseLegality.WildPokeballs8.Contains(pk.Ball))
                return GetValid(LBallSpeciesPass);
        }

        if (ball == Safari)
        {
            if (!(BallBreedLegality.Inherit_Safari.Contains(species) || BallBreedLegality.Inherit_SafariMale.Contains(species)))
                return GetInvalid(LBallSpecies);
            if (BallBreedLegality.Ban_SafariBallHidden_7.Contains(species) && IsHiddenAndNotPossible(pk))
                return GetInvalid(LBallAbility);
            return GetValid(LBallSpeciesPass);
        }
        if (ball.IsApricornBall()) // Apricorn Ball
        {
            if (!BallBreedLegality.Inherit_Apricorn7.Contains(species))
                return GetInvalid(LBallSpecies);
            if (BallBreedLegality.Ban_NoHidden8Apricorn.Contains(species | (pk.Form << 11)) && IsHiddenAndNotPossible(pk)) // lineage is 3->2->origin
                return GetInvalid(LBallAbility);
            return GetValid(LBallSpeciesPass);
        }
        if (ball == Sport) // Sport Ball
        {
            if (!BallBreedLegality.Inherit_Sport.Contains(species))
                return GetInvalid(LBallSpecies);
            if ((species is (int)Species.Volbeat or (int)Species.Illumise) && IsHiddenAndNotPossible(pk)) // Volbeat/Illumise
                return GetInvalid(LBallAbility);
            return GetValid(LBallSpeciesPass);
        }
        if (ball == Dream) // Dream Ball
        {
            if (BallBreedLegality.Inherit_Dream.Contains(species) || BallBreedLegality.Inherit_DreamMale.Contains(species))
                return GetValid(LBallSpeciesPass);
            return GetInvalid(LBallSpecies);
        }
        if (ball is >= Dusk and <= Quick) // Dusk Heal Quick
        {
            if (!BallBreedLegality.Ban_Gen4Ball_7.Contains(species))
                return GetValid(LBallSpeciesPass);
            return GetInvalid(LBallSpecies);
        }
        if (ball is >= Ultra and <= Premier) // Don't worry, Safari was already checked.
        {
            if (!BallBreedLegality.Ban_Gen3Ball_7.Contains(species))
                return GetValid(LBallSpeciesPass);
            return GetInvalid(LBallSpecies);
        }

        if (ball == Beast)
        {
            // Most were already caught by Galar ball logic. Check for stuff not in SW/SH.
            if (BallBreedLegality.AlolanCaptureOffspring.Contains(species))
                return GetValid(LBallSpeciesPass);
            if (BallBreedLegality.PastGenAlolanScans.Contains(species))
                return GetValid(LBallSpeciesPass);
        }

        if (ball > Beast)
            return GetInvalid(LBallUnavailable);

        return GetInvalid(LBallEncMismatch);
    }

    private CheckResult VerifyBallEggGen8(LegalityAnalysis data)
    {
        var pk = data.Entity;
        if (pk.Ball == (int)Poke)
            return GetValid(LBallEnc); // Poké Ball

        int species = data.EncounterMatch.Species;
        if (species is >= (int)Species.Grookey and <= (int)Species.Inteleon) // G8 Starters
            return VerifyBallEquals(data, (int)Poke);

        if (IsGalarCatchAndBreed(species))
        {
            if (BallUseLegality.WildPokeballs8.Contains(pk.Ball))
                return GetValid(LBallSpeciesPass);
            if (species >= (int)Species.Grookey)
                return GetInvalid(LBallSpecies);
        }

        Ball ball = (Ball)pk.Ball;

        if (ball == Safari)
        {
            if (!(BallBreedLegality.Inherit_Safari.Contains(species) || BallBreedLegality.Inherit_SafariMale.Contains(species)))
                return GetInvalid(LBallSpecies);
            if (BallBreedLegality.Ban_SafariBallHidden_7.Contains(species) && IsHiddenAndNotPossible(pk))
                return GetInvalid(LBallAbility);
            return GetValid(LBallSpeciesPass);
        }
        if (ball.IsApricornBall()) // Apricorn Ball
        {
            if (!BallBreedLegality.Inherit_Apricorn7.Contains(species))
                return GetInvalid(LBallSpecies);
            if (BallBreedLegality.Ban_NoHidden8Apricorn.Contains(species | (pk.Form << 11)) && IsHiddenAndNotPossible(pk)) // lineage is 3->2->origin
                return GetInvalid(LBallAbility);
            return GetValid(LBallSpeciesPass);
        }
        if (ball == Sport) // Sport Ball
        {
            if (!BallBreedLegality.Inherit_Sport.Contains(species))
                return GetInvalid(LBallSpecies);
            if ((species is (int)Species.Volbeat or (int)Species.Illumise) && IsHiddenAndNotPossible(pk)) // Volbeat/Illumise
                return GetInvalid(LBallAbility);
            return GetValid(LBallSpeciesPass);
        }
        if (ball == Dream) // Dream Ball
        {
            if (BallBreedLegality.Inherit_Dream.Contains(species) || BallBreedLegality.Inherit_DreamMale.Contains(species))
                return GetValid(LBallSpeciesPass);
            return GetInvalid(LBallSpecies);
        }
        if (ball is >= Dusk and <= Quick) // Dusk Heal Quick
        {
            if (!BallBreedLegality.Ban_Gen4Ball_7.Contains(species))
                return GetValid(LBallSpeciesPass);
            return GetInvalid(LBallSpecies);
        }
        if (ball is >= Ultra and <= Premier) // Don't worry, Safari was already checked.
        {
            if (!BallBreedLegality.Ban_Gen3Ball_7.Contains(species))
                return GetValid(LBallSpeciesPass);
            return GetInvalid(LBallSpecies);
        }

        if (ball == Beast)
        {
            if (species == (int)Species.Flabébé && pk.Form == 3 && IsHiddenAndNotPossible(pk))
                return GetInvalid(LBallAbility); // Can't obtain Flabébé-Blue with Hidden Ability in wild
            if ((species is >= (int)Species.Pikipek and <= (int)Species.Kommoo) || BallBreedLegality.AlolanCaptureOffspring.Contains(species))
                return GetValid(LBallSpeciesPass);
            if (BallBreedLegality.PastGenAlolanScans.Contains(species))
                return GetValid(LBallSpeciesPass);
            // next statement catches all new alolans
        }

        if (species > Legal.MaxSpeciesID_7_USUM)
            return VerifyBallEquals(data, BallUseLegality.WildPokeballs8);

        if (species > (int)Species.Volcanion)
            return VerifyBallEquals(data, BallUseLegality.WildPokeballs7);

        if (ball > Beast)
            return GetInvalid(LBallUnavailable);

        return NONE;
    }

    private static bool IsHiddenAndNotPossible(PKM pk)
    {
        if (pk.AbilityNumber != 4)
            return false;
        return !AbilityVerifier.CanAbilityPatch(pk.Format, pk.PersonalInfo.Abilities, pk.Species);
    }

    private static bool IsGalarCatchAndBreed(int species)
    {
        if (species is >= (int)Species.Grookey and <= (int)Species.Inteleon) // starter
            return false;

        // Everything breed-able that is in the Galar Dex can be captured in-game.
        var pt = PersonalTable.SWSH;
        var pi = pt.GetFormEntry(species, 0);
        if (pi.IsInDex)
            return true;

        // Foreign Captures
        if (species is >= (int)Species.Treecko and <= (int)Species.Swampert) // Dynamax Adventures
            return true;
        if (species is >= (int)Species.Rowlet and <= (int)Species.Primarina) // Distribution Raids
            return true;

        return false;
    }

    private CheckResult VerifyBallEquals(LegalityAnalysis data, int ball) => GetResult(ball == data.Entity.Ball);
    private CheckResult VerifyBallEquals(LegalityAnalysis data, HashSet<int> balls) => GetResult(balls.Contains(data.Entity.Ball));
    private CheckResult VerifyBallEquals(LegalityAnalysis data, ICollection<int> balls) => GetResult(balls.Contains(data.Entity.Ball));
    private CheckResult GetResult(bool valid) => valid ? GetValid(LBallEnc) : GetInvalid(LBallEncMismatch);
}
