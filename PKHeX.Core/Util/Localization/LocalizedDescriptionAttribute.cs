using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PKHeX.Core;

[AttributeUsage(AttributeTargets.All)]
public sealed class LocalizedDescriptionAttribute(string fallback, [CallerMemberName] string? key = null)
    : DescriptionAttribute
{
    public static IReadOnlyDictionary<string, string> Localizer { private get; set; } = new Dictionary<string, string>();

    public readonly string Fallback = fallback;
    public readonly string Key = $"LocalizedDescription.{key!}";

    public override string Description => Localizer.GetValueOrDefault(Key, Fallback);
}
