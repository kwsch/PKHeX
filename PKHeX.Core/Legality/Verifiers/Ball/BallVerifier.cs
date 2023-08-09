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
        // Trading from PLA origin -> SW/SH will replace the Legends: Arceus ball with a regular Poké Ball
        PK8 when enc.Version == GameVersion.PLA => (int)Poke,

        // No replacement done.
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
            case IFixedBall {FixedBall: not None} s:
                return VerifyBallEquals(data, (byte)s.FixedBall);
            case EncounterSlot8GO: // Already a strict match
                return GetResult(true);
        }

        // Capture / Inherit cases -- can be one of many balls
        var pk = data.Entity;
        if (pk.Species == (int)Species.Shedinja && enc.Species != (int)Species.Shedinja) // Shedinja. For gen3, copy the ball from Nincada
        {
            // Only a Gen3 origin Shedinja can copy the wild ball.
            // Evolution chains will indicate if it could have existed as Shedinja in Gen3.
            // The special move verifier has a similar check!
            if (pk is { HGSS: true, Ball: (int)Sport }) // Can evolve in DP to retain the HG/SS ball (separate byte) -- not able to be captured in any other ball
                return VerifyBallEquals(data, (int)Sport);
            if (Info.Generation != 3 || Info.EvoChainsAllGens.Gen3.Length != 2) // not evolved in Gen3 Nincada->Shedinja
                return VerifyBallEquals(data, (int)Poke); // Poké ball Only
        }

        // Capturing with Heavy Ball is impossible in Sun/Moon for specific species.
        if (pk is { Ball: (int)Heavy, SM: true } && enc is not EncounterEgg && BallBreedLegality.AlolanCaptureNoHeavyBall.Contains(enc.Species))
            return GetInvalid(LBallHeavy); // Heavy Ball, can inherit if from egg (US/UM fixed catch rate calc)

        return enc switch
        {
            EncounterStatic5Entree => VerifyBallEquals(data, BallUseLegality.DreamWorldBalls),
            EncounterEgg => VerifyBallEgg(data),
            EncounterInvalid => VerifyBallEquals(data, pk.Ball), // ignore ball, pass whatever
            _ => VerifyBallEquals(data, BallUseLegality.GetWildBalls(data.Info.Generation, enc.Version)),
        };
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

    private CheckResult VerifyBallInherited(LegalityAnalysis data) => data.Info.EncounterMatch.Context switch
    {
        EntityContext.Gen6 => VerifyBallEggGen6(data), // Gen6 Inheritance Rules
        EntityContext.Gen7 => VerifyBallEggGen7(data), // Gen7 Inheritance Rules
        EntityContext.Gen8 => VerifyBallEggGen8(data),
        EntityContext.Gen8b => VerifyBallEggGen8BDSP(data),
        EntityContext.Gen9 => VerifyBallEggGen9(data),
        _ => NONE,
    };

    private CheckResult VerifyBallEggGen6(LegalityAnalysis data)
    {
        var pk = data.Entity;
        if (pk.Ball == (int)Poke)
            return GetValid(LBallEnc); // Poké Ball

        var enc = data.EncounterMatch;
        var species = enc.Species;
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
            if (BallBreedLegality.Ban_Gen3BallHidden.Contains((ushort)(species | (enc.Form << 11))) && IsHiddenAndNotPossible(pk))
                return GetInvalid(LBallAbility);
            return GetValid(LBallSpeciesPass);
        }

        if (species > 650 && species != 700) // Sylveon
        {
            if (IsBallPermitted(BallUseLegality.WildPokeballs6, pk.Ball))
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

        var species = data.EncounterMatch.Species;
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
            if (BallBreedLegality.Ban_NoHidden7Apricorn.Contains((ushort)(species | (pk.Form << 11))) && IsHiddenAndNotPossible(pk))
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
        var species = data.EncounterMatch.Species;
        if (species == (int)Species.Phione)
            return VerifyBallEquals(data, (int)Poke);

        if (species is (int)Species.Cranidos or (int)Species.Shieldon)
            return VerifyBallEquals(data, BallUseLegality.DreamWorldBalls);

        var pk = data.Entity;
        Ball ball = (Ball)pk.Ball;
        var balls = BallUseLegality.GetWildBalls(8, GameVersion.BDSP);
        if (IsBallPermitted(balls, (int)ball))
            return GetValid(LBallSpeciesPass);

        if (species is (int)Species.Spinda)
            return GetInvalid(LBallSpecies); // Can't enter or exit, needs to adhere to wild balls.

        // Cross-game inheritance
        if (IsGalarCatchAndBreed(species))
        {
            if (IsBallPermitted(BallUseLegality.WildPokeballs8, (int)ball))
                return GetValid(LBallSpeciesPass);
        }
        if (IsPaldeaCatchAndBreed(species))
        {
            if (IsBallPermitted(BallUseLegality.WildPokeballs9, (int)ball))
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
            if (BallBreedLegality.Ban_NoHidden8Apricorn.Contains((ushort)(species | (pk.Form << 11))) && IsHiddenAndNotPossible(pk)) // lineage is 3->2->origin
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

        var species = data.EncounterMatch.Species;
        if (IsGalarCatchAndBreed(species))
        {
            if (IsBallPermitted(BallUseLegality.WildPokeballs8, pk.Ball))
                return GetValid(LBallSpeciesPass);
        }
        if (IsPaldeaCatchAndBreed(species))
        {
            if (IsBallPermitted(BallUseLegality.WildPokeballs9, pk.Ball))
                return GetValid(LBallSpeciesPass);
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
            if (BallBreedLegality.Ban_NoHidden8Apricorn.Contains((ushort)(species | (pk.Form << 11))) && IsHiddenAndNotPossible(pk)) // lineage is 3->2->origin
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

    private CheckResult VerifyBallEggGen9(LegalityAnalysis data)
    {
        var enc = data.EncounterMatch;
        var species = enc.Species;
        if (species is >= (int)Species.Sprigatito and <= (int)Species.Quaquaval) // G9 Starters
            return VerifyBallEquals(data, (int)Poke);

        // PLA Voltorb: Only via PLA (transfer only, not wild) and GO
        if (enc is { Species: (ushort)Species.Voltorb, Form: 1 })
            return VerifyBallEquals(data, BallUseLegality.WildPokeballs8g);

        // S/V Tauros forms > 1: Only local Wild Balls for Blaze/Aqua breeds -- can't inherit balls from Kantonian/Combat.
        if (enc is { Species: (ushort)Species.Tauros, Form: > 1 })
            return VerifyBallEquals(data, BallUseLegality.WildPokeballs9);

        var pk = data.Entity;
        if (IsPaldeaCatchAndBreed(species) && IsBallPermitted(BallUseLegality.WildPokeballs9, pk.Ball))
            return GetValid(LBallSpeciesPass);

        if (species > Legal.MaxSpeciesID_8)
            return VerifyBallEquals(data, BallUseLegality.WildPokeballs9);

        return VerifyBallEggGen8(data);
    }

    private static bool IsHiddenAndNotPossible(PKM pk)
    {
        if (pk.AbilityNumber != 4)
            return false;
        var abilities = (IPersonalAbility12H)pk.PersonalInfo;
        return !AbilityVerifier.CanAbilityPatch(pk.Format, abilities, pk.Species);
    }

    private static bool IsGalarCatchAndBreed(ushort species)
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

    private static bool IsPaldeaCatchAndBreed(ushort species)
    {
        if (species is >= (int)Species.Sprigatito and <= (int)Species.Quaquaval) // starter
            return false;
        var pt = PersonalTable.SV;
        var pi = pt.GetFormEntry(species, 0);
        if (pi.IsPresentInGame)
            return true;

        return false;
    }

    private CheckResult VerifyBallEquals(LegalityAnalysis data, int ball) => GetResult(ball == data.Entity.Ball);
    private CheckResult VerifyBallEquals(LegalityAnalysis data, ulong permit) => GetResult(IsBallPermitted(permit, data.Entity.Ball));

    private static bool IsBallPermitted(ulong permit, int ball)
    {
        if ((uint)ball >= 64)
            return false;
        return (permit & (1ul << ball)) != 0;
    }

    private CheckResult GetResult(bool valid) => valid ? GetValid(LBallEnc) : GetInvalid(LBallEncMismatch);
}
