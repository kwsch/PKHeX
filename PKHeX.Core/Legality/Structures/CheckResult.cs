namespace PKHeX.Core
{
    /// <summary>
    /// Result of a Legality Check
    /// </summary>
    public class CheckResult
    {
        internal readonly Severity Judgement = Severity.Valid;
        internal string Comment = LegalityCheckStrings.V;
        public bool Valid => Judgement >= Severity.Fishy;
        public string Rating => Judgement.Description();
        public bool Flag;
        internal readonly CheckIdentifier Identifier;

        internal CheckResult(CheckIdentifier i) { Identifier = i; }
        internal CheckResult(Severity s, string c, CheckIdentifier i)
        {
            Judgement = s;
            Comment = c;
            Identifier = i;
        }
    }
}
