using static PKHeX.Core.Species;

namespace PKHeX.Core;

/// <summary>
/// PID generating logic for Generation 4's <see cref="Ability.CuteCharm"/> mechanic.
/// </summary>
public static class CuteCharm4
{
    private const uint NatureCount = 25;

    /// <summary>
    /// Gets the PID of a Male wild encounter when the player leads a Female Pokémon with Cute Charm.
    /// </summary>
    /// <param name="genderRatio">Gender ratio of the encounter species.</param>
    /// <param name="nature">Nature of the encounter Pokémon.</param>
    public static uint GetPIDMale(byte genderRatio, uint nature)
    {
        // pid >= ratio is male
        // get the lowest PID that will be male for Hardy (0)
        var basePID = NatureCount * ((genderRatio / NatureCount) + 1);
        // add the desired nature to the base PID
        return basePID + nature;
    }

    /// <summary>
    /// Gets the PID of a Female wild encounter when the player leads a Male Pokémon with Cute Charm.
    /// </summary>
    /// <param name="nature">Nature of the encounter Pokémon.</param>
    public static uint GetPIDFemale(uint nature) => nature;

    /// <summary>
    /// Gets the Gender Ratio of the species. If the species is not in Gen 4, it will grab whatever its de-evolved species is.
    /// </summary>
    private static byte GetGenderRatio(ushort species)
    {
        return species <= Legal.MaxSpeciesID_4
            ? PersonalTable.HGSS[species].Gender
            : EntityGender.GetGenderRatio(species); // fallback (don't bother trying to devolve to Gen1-4 encounter species)
    }

    private static bool IsAzurillMale(uint pid) => pid is >= 0xC8 and <= 0xE0;

    /// <summary>
    /// Checks if the <see cref="pk"/> can have Cute Charm as its PID type. Special consideration for male Azurill encounters, which can change into female.
    /// </summary>
    /// <param name="enc">Encounter (species)</param>
    /// <param name="pk">Current entity state</param>
    /// <returns>True if the Cute Charm type is valid</returns>
    internal static bool IsValid<TEnc>(TEnc enc, PKM pk) where TEnc : ISpeciesForm
    {
        if (pk.Gender is not (0 or 1))
            return pk.Species == (ushort)Shedinja;
        if (pk.Species is not ((int)Marill or (int)Azumarill))
            return true;
        if (!IsAzurillMale(pk.EncryptionConstant)) // recognized as not Azurill
            return true;
        return enc.Species == (int)Azurill; // encounter must be male Azurill
    }

    /// <summary>
    /// Checks if the PID can arise as a result of Cute Charm.
    /// </summary>
    public static bool IsCuteCharm(PKM pk, uint pid)
    {
        if (pid > 0xFF)
            return false;

        var (species, gender) = GetGenderSpecies(pk, pid, pk.Species);

        switch (gender)
        {
            // case 2: break; // can't cute charm a genderless pk
            case 0: // male
                var gr = GetGenderRatio(species);
                if (gr >= PersonalInfo.RatioMagicFemale) // no modification for PID
                    break;
                var nature = pid % NatureCount;
                var expect = GetPIDMale(gr, nature);
                if (pid != expect)
                    break;
                return true;
            case 1: // female
                if (pid >= NatureCount)
                    break; // nope, this isn't a valid nature
                if (GetGenderRatio(species) >= PersonalInfo.RatioMagicFemale) // no modification for PID
                    break;
                return true;
        }
        return false;
    }

    /// <summary>
    /// There are some edge cases when the gender ratio changes across evolutions.
    /// </summary>
    private static (ushort Species, byte Gender) GetGenderSpecies(PKM pk, uint pid, ushort currentSpecies) => currentSpecies switch
    {
        // Nincada evo chain travels from M/F -> Genderless Shedinja
        (int)Shedinja  => ((int)Nincada, EntityGender.GetFromPID((int)Nincada, pid)),

        // These evolved species cannot be encountered with cute charm.
        // 100% fixed gender does not modify PID; override this with the encounter species for correct calculation.
        // We can assume the re-mapped species' [gender ratio] is what was encountered.
        (int)Wormadam  => ((int)Burmy,   1),
        (int)Mothim    => ((int)Burmy,   0),
        (int)Vespiquen => ((int)Combee,  1),
        (int)Gallade   => ((int)Kirlia,  0),
        (int)Froslass  => ((int)Snorunt, 1),
        // Azurill & Marill/Azumarill collision
        // Changed gender ratio (25% M -> 50% M) needs special treatment.
        // Double-check the encounter species with IsValid afterward.
        (int)Marill or (int)Azumarill when IsAzurillMale(pid) => ((int)Azurill, 0),

        // Future evolutions
        _ => (GetSpeciesGen4(currentSpecies), pk.Gender),
    };

    private static ushort GetSpeciesGen4(ushort species) => species switch
    {
        <= Legal.MaxSpeciesID_4 => species, // has a valid personal reference, all good
        (int)Sylveon    => (int)Eevee,
        (int)MrRime     => (int)MrMime,
        (int)Wyrdeer    => (int)Stantler,
        (int)Kleavor    => (int)Scyther,
        (int)Sneasler   => (int)Sneasel,
        (int)Ursaluna   => (int)Ursaring,
        (int)Annihilape => (int)Primeape,
        _ => species, // throw an exception? Hitting here is an invalid case.
    };
}
