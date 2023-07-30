using System;

namespace PKHeX.Core;

public interface IFixedNickname
{
    bool IsFixedNickname { get; }

    bool IsNicknameMatch(PKM pk, ReadOnlySpan<char> nickname, int language);
    string GetNickname(int language);
}
