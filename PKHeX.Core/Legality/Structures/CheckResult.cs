namespace PKHeX.Core
{
    /// <summary>
    /// Result of a Legality Check
    /// </summary>
    public class CheckResult
    {
        public Severity Judgement { get; }
        public CheckIdentifier Identifier { get; }
        public string Comment { get; internal set; }

        public bool Valid => Judgement >= Severity.Fishy;
        public string Rating => Judgement.Description();

        internal CheckResult(CheckIdentifier i)
        {
            Judgement = Severity.Valid;
            Comment = LegalityCheckStrings.L_AValid;
            Identifier = i;
        }

        internal CheckResult(Severity s, string c, CheckIdentifier i)
        {
            Judgement = s;
            Comment = c;
            Identifier = i;
        }
    }
}
