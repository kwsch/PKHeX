using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PKHeX.Core
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Parameter | AttributeTargets.Delegate | AttributeTargets.ReturnValue | AttributeTargets.GenericParameter)]
    public sealed class LocalizedDescriptionAttribute : DescriptionAttribute
    {
        public static IReadOnlyDictionary<string, string> Localizer { private get; set; } = new Dictionary<string, string>();

        public readonly string Fallback;
        public readonly string Key;

        public LocalizedDescriptionAttribute(string fallback, [CallerMemberName] string? key = null)
        {
            Key = $"LocalizedDescription.{key!}";
            Fallback = fallback;
        }

        public override string Description => Localizer.TryGetValue(Key, out var result) ? result : Fallback;
    }
}
