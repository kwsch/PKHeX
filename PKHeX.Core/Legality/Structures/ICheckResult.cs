namespace PKHeX.Core;

public interface ICheckResult
{
    Severity Judgement { get; }
    CheckIdentifier Identifier { get; }
    string Comment { get; }

    bool Valid { get; }
    string Rating { get; }
}
