using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    public partial class LegalityAnalysis
    {
        private static readonly Verifier Language = new LanguageVerifier();
        private static readonly Verifier Nickname = new NicknameVerifier();
        private static readonly Verifier EffortValues = new EffortValueVerifier();
        private static readonly Verifier IndividualValues = new IndividualValueVerifier();
        private static readonly Verifier Ball = new BallVerifier();
        private static readonly Verifier Form = new FormVerifier();
        private static readonly Verifier ConsoleRegion = new ConsoleRegionVerifier();
        private static readonly Verifier Ability = new AbilityVerifier();
        private static readonly Verifier Medal = new MedalVerifier();
        private static readonly Verifier Ribbon = new RibbonVerifier();
        private static readonly Verifier Item = new ItemVerifier();
        private static readonly Verifier EncounterType = new EncounterTypeVerifier();
        private static readonly Verifier HyperTraining = new HyperTrainingVerifier();
        private static readonly Verifier Gender = new GenderVerifier();
        private static readonly Verifier PIDEC = new PIDVerifier();
        private static readonly Verifier NHarmonia = new NHarmoniaVerifier();
        private static readonly Verifier CXD = new CXDVerifier();

        private static readonly TrainerNameVerifier Trainer = new TrainerNameVerifier();
        private static readonly MemoryVerifier Memory = new MemoryVerifier();
        private static readonly LevelVerifier Level = new LevelVerifier();
        private static readonly MiscVerifier Misc = new MiscVerifier();
        private static readonly TransferVerifier Transfer = new TransferVerifier();

        private void VerifyBall() => Ball.Verify(this);
        private void VerifyForm() => Form.Verify(this);
        private void VerifyEVs() => EffortValues.Verify(this);
        private void VerifyIVs() => IndividualValues.Verify(this);
        private void VerifyHistory() => Memory.Verify(this);
        private void VerifyConsoleRegion() => ConsoleRegion.Verify(this);
        private void VerifyAbility() => Ability.Verify(this);
        private void VerifyRibbons() => Ribbon.Verify(this);
        private void VerifyItem() => Item.Verify(this);
        private void VerifyLevel() => Level.Verify(this);
        private void VerifyLevelG1() => Level.VerifyG1(this);
        private void VerifyEncounterType() => EncounterType.Verify(this);
        private void VerifyOT() => Trainer.Verify(this);
        private void VerifyHyperTraining() => HyperTraining.Verify(this);
        private void VerifyGender() => Gender.Verify(this);
        private void VerifyECPID() => PIDEC.Verify(this);
        private void VerifyCXD() => CXD.Verify(this);
        private void VerifyMedals()
        {
            if (pkm.Format >= 6)
                Medal.Verify(this);
        }
        private void VerifyNickname()
        {
            Nickname.Verify(this);
            if (pkm.Format >= 3)
                Language.Verify(this);
        }
        private void VerifyMisc()
        {
            if (Info.Generation == 5)
                NHarmonia.Verify(this);
            Misc.Verify(this);
        }

        public static string[] MoveStrings { internal get; set; } = Util.GetMovesList("en");
        public static string[] SpeciesStrings { internal get; set; } = Util.GetSpeciesList("en");
        internal static IEnumerable<string> GetMoveNames(IEnumerable<int> moves) => moves.Select(m => m >= MoveStrings.Length ? V190 : MoveStrings[m]);
    }
}
