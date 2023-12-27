namespace PKHeX.Core;

internal interface ILangNick
{
    string Nickname { get; }
    bool IsNicknamed { get; }
    int Language { get; }
}
