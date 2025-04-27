using System;
using System.Text;

namespace PKHeX.Core;

/// <summary>
/// Grammar and prefix/suffix tokens for <see cref="IBattleTemplate"/>.
/// </summary>
public sealed record BattleTemplateConfig
{
    public sealed record BattleTemplateTuple(BattleTemplateToken Token, string Text);

    /// <summary> Prefix tokens - e.g. Friendship: {100} </summary>
    public required BattleTemplateTuple[] Left { get; init; }

    /// <summary> Suffix tokens - e.g. {Timid} Nature </summary>
    public required BattleTemplateTuple[] Right { get; init; }

    /// <summary> Tokens that always display the same text, with no value - e.g. Shiny: Yes </summary>
    public required BattleTemplateTuple[] Center { get; init; }

    /// <summary>
    /// Stat names, ordered with speed in the middle (not last).
    /// </summary>
    public required string[] StatNames { get; init; }

    /// <summary>
    /// Tries to parse the line for a token and value, if applicable.
    /// </summary>
    /// <param name="line">Line to parse</param>
    /// <param name="value">Value for the token, if applicable</param>
    /// <returns>Token type that was found</returns>
    public BattleTemplateToken TryParse(ReadOnlySpan<char> line, out ReadOnlySpan<char> value)
    {
        value = default;
        if (line.Length == 0)
            return BattleTemplateToken.None;
        foreach (var tuple in Left)
        {
            if (!line.StartsWith(tuple.Text, StringComparison.OrdinalIgnoreCase))
                continue;
            value = line[tuple.Text.Length..];
            return tuple.Token;
        }
        foreach (var tuple in Right)
        {
            if (!line.EndsWith(tuple.Text, StringComparison.OrdinalIgnoreCase))
                continue;
            value = line[..^tuple.Text.Length];
            return tuple.Token;
        }
        foreach (var tuple in Center)
        {
            if (!line.Equals(tuple.Text, StringComparison.OrdinalIgnoreCase))
                continue;
            return tuple.Token;
        }
        return BattleTemplateToken.None;
    }

    private string GetToken(BattleTemplateToken token, out bool isLeft)
    {
        foreach (var tuple in Left)
        {
            if (tuple.Token != token)
                continue;
            isLeft = true;
            return tuple.Text;
        }
        foreach (var tuple in Right)
        {
            if (tuple.Token != token)
                continue;
            isLeft = false;
            return tuple.Text;
        }
        foreach (var tuple in Center)
        {
            if (tuple.Token != token)
                continue;
            isLeft = false;
            return tuple.Text;
        }
        throw new ArgumentException($"Token {token} not found in config");
    }

    /// <summary>
    /// Gets the string representation of the token. No value is combined with it.
    /// </summary>
    public string Push(BattleTemplateToken token) => GetToken(token, out _);

    /// <summary>
    /// Gets the string representation of the token, and combines the value with it.
    /// </summary>
    public string Push<T>(BattleTemplateToken token, T value)
    {
        var str = GetToken(token, out var isLeft);
        if (isLeft)
            return $"{str}{value}";
        return $"{value}{str}";
    }

    /// <inheritdoc cref="Push{T}(BattleTemplateToken,T)"/>
    public void Push<T>(BattleTemplateToken token, T value, StringBuilder sb)
    {
        var str = GetToken(token, out var isLeft);
        if (isLeft)
            sb.Append(str).Append(value);
        else
            sb.Append(value).Append(str);
    }
}
