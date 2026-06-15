using System;

namespace PKHeX.Core;

/// <summary>
/// All Join Avenue entities (Assistants, Fans, Visitors) share some common properties, which are abstracted in this interface.
/// </summary>
/// <remarks>
/// Even if it is nonsensical for some of these properties to be shared across all entity types (e.g. ShopLevel or Greeting/Farewell for Assistants), they are still present in the data structure.
/// </remarks>
public interface IJoinAvenueEntity5
{
    string FileExtension { get; }
    string Name { get; set; }
    string Shout { get; set; }
    string Greeting { get; set; }
    string Farewell { get; set; }

    /// <summary>
    /// <see cref="UnityTower5.LegalCountries"/>
    /// </summary>
    byte Country { get; set; }

    /// <summary>
    /// <see cref="UnityTower5.GetSubregionCount"/>
    /// </summary>
    byte Subregion { get; set; }

    byte Version { get; set; }
    byte Language { get; set; }
    byte Unknown22 { get; set; }
    byte Gender { get; set; }
    byte Unused23 { get; set; }
    ushort TID16 { get; set; }
    byte Unknown26 { get; set; }
    byte Unknown27 { get; set; }
    ushort PlayedTime { get; set; }
    ushort PlayedHours { get; set; }
    byte PlayedMinutes { get; set; }
    ushort Sprite { get; set; }

    byte MetYear { get; set; }
    byte MetMonth { get; set; }
    byte MetDay { get; set; }
    bool IsInteractedToday { get; set; }

    uint Seed { get; set; }
    ReadOnlySpan<byte> Write();
}

public static class JoinAvenueEntityConverter
{
    extension(IJoinAvenueEntity5 destination)
    {
        private void CopyCommonPropertiesFrom(IJoinAvenueEntity5 source)
        {
            destination.Name = source.Name;
            destination.Shout = source.Shout;
            destination.Greeting = source.Greeting;
            destination.Farewell = source.Farewell;
            destination.Country = source.Country;
            destination.Subregion = source.Subregion;
            destination.Version = source.Version;
            destination.Language = source.Language;
            destination.Unknown22 = source.Unknown22;
            destination.Gender = source.Gender;
            destination.Unused23 = source.Unused23;
            destination.TID16 = source.TID16;
            destination.Unknown26 = source.Unknown26;
            destination.Unknown27 = source.Unknown27;
            destination.PlayedTime = source.PlayedTime;
            destination.PlayedHours = source.PlayedHours;
            destination.PlayedMinutes = source.PlayedMinutes;
            destination.Sprite = source.Sprite;
            destination.MetYear = source.MetYear;
            destination.MetMonth = source.MetMonth;
            destination.MetDay = source.MetDay;
            destination.Seed = source.Seed;
        }

        public void CopyFrom(IJoinAvenueEntity5 source)
        {
            switch (destination, source)
            {
                case (JoinAvenueVisitor5 current, JoinAvenueVisitor5 s):
                    current.CopyFrom(s);
                    break;
                case (JoinAvenueFan5 current, JoinAvenueFan5 s):
                    current.CopyFrom(s);
                    break;
                case (JoinAvenueAssistant5 current, JoinAvenueAssistant5 s):
                    current.CopyFrom(s);
                    break;
                default:
                    destination.CopyCommonPropertiesFrom(source);
                    break;
            }
        }
    }
}
