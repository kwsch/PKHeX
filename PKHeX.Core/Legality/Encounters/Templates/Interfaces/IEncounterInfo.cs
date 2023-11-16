namespace PKHeX.Core;

/// <summary>
/// Exposes simple encounter info and can be converted to a <see cref="PKM"/>.
/// </summary>
/// <remarks>Combined interface used to require multiple interfaces being present for a calling method.</remarks>
public interface IEncounterInfo : IEncounterTemplate, IEncounterConvertible;
