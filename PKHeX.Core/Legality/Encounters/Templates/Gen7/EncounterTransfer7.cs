using System;

namespace PKHeX.Core;

/// <summary>
/// Virtual Console Transfer Encounter Data
/// </summary>
/// <param name="Version">Version group transferred from.</param>
/// <param name="Species">Species transferred.</param>
/// <param name="Level">Level transferred at.</param>
public sealed record EncounterTransfer7(GameVersion Version, ushort Species, byte Level) : IEncounterable, IFlawlessIVCount, IFatefulEncounterReadOnly
{
    public byte Generation => 7;
    public EntityContext Context => EntityContext.Gen7;
    public Ball FixedBall => Ball.Poke;
    public bool IsEgg => false;
    public ushort EggLocation => 0;

    public byte Form => 0;
    public ushort Location { get; private init; }
    public Shiny Shiny { get; private init; }
    public AbilityPermission Ability { get; private init; }
    public bool FatefulEncounter { get; private init; }
    public byte FlawlessIVCount { get; private init; }

    public byte LevelMin => Level;
    public byte LevelMax => Level;
    public bool IsShiny => false;

    public string Name => $"Virtual Console Transfer ({Version})";
    public string LongName => Name;

    public static EncounterTransfer7 GetVC1(ushort species, byte metLevel)
    {
        bool mew = species == (int)Core.Species.Mew;
        return new EncounterTransfer7(GameVersion.RBY, species, metLevel)
        {
            Species = species,
            Ability = TransporterLogic.IsHiddenDisallowedVC1(species) ? AbilityPermission.OnlyFirst : AbilityPermission.OnlyHidden, // Hidden by default, else first
            Shiny = mew ? Shiny.Never : Shiny.Random,
            FatefulEncounter = mew,
            Location = Locations.Transfer1,
            FlawlessIVCount = mew ? (byte)5 : (byte)3,
        };
    }

    public static EncounterTransfer7 GetVC2(ushort species, byte metLevel)
    {
        bool mew = species == (int)Core.Species.Mew;
        bool fateful = mew || species == (int)Core.Species.Celebi;
        return new EncounterTransfer7(GameVersion.GSC, species, metLevel)
        {
            Ability = TransporterLogic.IsHiddenDisallowedVC2(species) ? AbilityPermission.OnlyFirst : AbilityPermission.OnlyHidden, // Hidden by default, else first
            Shiny = mew ? Shiny.Never : Shiny.Random,
            FatefulEncounter = fateful,
            Location = Locations.Transfer2,
            FlawlessIVCount = fateful ? (byte)5 : (byte)3,
        };
    }

    public PKM ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);
    public PKM ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => throw new InvalidOperationException("Conversion not supported.");
}
