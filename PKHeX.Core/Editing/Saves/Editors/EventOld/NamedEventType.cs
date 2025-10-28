using System;
using static PKHeX.Core.NamedEventType;

namespace PKHeX.Core;

/// <summary>
/// Type of scripted event label
/// </summary>
public enum NamedEventType
{
    None,
    HiddenItem,
    TrainerToggle,
    StoryProgress,
    FlyToggle,
    Misc,
    Statistic,

    Achievement,
    UsefulFeature,
    EventEncounter,
    GiftAvailable,
    Rebattle = 100,
}

/// <summary>
/// Utility logic methods for <see cref="NamedEventType"/>
/// </summary>
public static class NamedEventTypeUtil
{
    public static NamedEventType GetEventType(ReadOnlySpan<char> s) => s.Length == 0 ? None : GetEventType(s[0]);

    public static NamedEventType GetEventType(char c) => c switch
    {
        'h' => HiddenItem,
        'm' => Misc,
        'f' => FlyToggle,
        't' => TrainerToggle,
        's' => StoryProgress,

        'a' => Achievement,
        '+' => Statistic,
        '*' => UsefulFeature,
        'e' => EventEncounter,
        'g' => GiftAvailable,
        'r' => Rebattle,
        _ => None,
    };
}
