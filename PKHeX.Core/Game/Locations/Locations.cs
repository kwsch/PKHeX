namespace PKHeX.Core;

/// <summary>
/// Decoration and logic for Met Location IDs
/// </summary>
public static class Locations
{
    public const ushort LinkTrade4 = 2002;
    public const ushort LinkTrade5 = 30003;
    public const ushort LinkTrade6 = 30002;

    public const ushort Daycare4 = 2000;
    public const ushort Daycare5 = 60002;
    public const ushort Daycare8b = 60010;
    public const ushort Picnic9 = 30023;
    public const ushort TeraCavern9 = 30024;

    public const ushort LinkTrade2NPC = 126;
    public const ushort LinkTrade3NPC = 254;
    public const ushort LinkTrade4NPC = 2001;
    public const ushort LinkTrade5NPC = 30002;
    public const ushort LinkTrade6NPC = 30001;

    public const ushort Breeder5 = 60003;
    public const ushort Breeder6 = 60004;

    public const ushort PokeWalker4 = 233;
    public const ushort Ranger4 = 3001;
    public const ushort Faraway4 = 3002;

    /// <summary> Goldenrod City in <see cref="GameVersion.C"/> </summary>
    public const byte HatchLocationC = 16;

    /// <summary> Route 117 in <see cref="GameVersion.RSE"/> </summary>
    public const byte HatchLocationRSE = 32;

    /// <summary> Four Island in <see cref="GameVersion.FRLG"/> </summary>
    public const byte HatchLocationFRLG = 146;

    /// <summary> Solaceon Town in <see cref="GameVersion.DPPt"/> </summary>
    public const ushort HatchLocationDPPt = 4;

    /// <summary> Route 34 in <see cref="GameVersion.HGSS"/> </summary>
    public const ushort HatchLocationHGSS = 182;

    /// <summary> Skyarrow Bridge in <see cref="GameVersion.Gen5"/> </summary>
    public const ushort HatchLocation5 = 64;

    /// <summary> Route 7 in <see cref="GameVersion.XY"/> </summary>
    public const ushort HatchLocation6XY = 38;

    /// <summary> Battle Resort in <see cref="GameVersion.ORAS"/> </summary>
    public const ushort HatchLocation6AO = 318;

    /// <summary> Paniola Ranch in <see cref="GameVersion.Gen7"/> </summary>
    public const ushort HatchLocation7 = 78;

    /// <summary> Route 5 in <see cref="GameVersion.SWSH"/> </summary>
    public const ushort HatchLocation8 = 40;

    /// <summary> Solaceon Town in <see cref="GameVersion.BDSP"/> </summary>
    public const ushort HatchLocation8b = 446;

    /// <summary> South Province (Area One) in <see cref="GameVersion.SV"/> </summary>
    public const ushort HatchLocation9 = 6;

    /// <summary> Generation 1 -> Generation 7 Transfer Location (Kanto) </summary>
    public const ushort Transfer1 = 30013;

    /// <summary> Generation 2 -> Generation 7 Transfer Location (Johto) </summary>
    public const ushort Transfer2 = 30017;

    /// <summary> Generation 3 -> Generation 4 Transfer Location (Pal Park) </summary>
    public const ushort Transfer3 = 0x37;

    /// <summary> Generation 4 -> Generation 5 Transfer Location (Poké Transporter) </summary>
    public const ushort Transfer4 = 30001;

    /// <summary> Generation 4 -> Generation 5 Transfer Location (Crown Celebi - Event not activated in Gen 5) </summary>
    public const ushort Transfer4_CelebiUnused = 30010;

    /// <summary> Generation 4 -> Generation 5 Transfer Location (Crown Celebi - Event activated in Gen 5) </summary>
    public const ushort Transfer4_CelebiUsed = 30011;

    /// <summary> Generation 4 -> Generation 5 Transfer Location (Crown Beast - Event not activated in Gen 5) </summary>
    public const ushort Transfer4_CrownUnused = 30012;

    /// <summary> Generation 4 -> Generation 5 Transfer Location (Crown Beast - Event activated in Gen 5) </summary>
    public const ushort Transfer4_CrownUsed = 30013;

    /// <summary> Generation 6 Gift from Pokémon Link </summary>
    public const ushort LinkGift6 = 30011;

    /// <summary> Generation 7 Poké Pelago </summary>
    public const ushort Pelago7 = 30016;

    /// <summary> Generation 7 Transfer from GO to Pokémon LGP/E's GO Park </summary>
    public const ushort GO7 = 50;

    /// <summary> Generation 8 Transfer from GO to Pokémon HOME </summary>
    public const ushort GO8 = 30012;

    /// <summary> Generation 8 Gift from Pokémon HOME </summary>
    public const ushort HOME8 = 30018;

    /// <summary> Generation 8 BD/SP Magic location for "None" since 0 is an actual met location. </summary>
    public const ushort Default8bNone = 65535;

    /// <summary>
    /// Gets the egg location value for a traded unhatched egg.
    /// </summary>
    /// <param name="generation">Generation of the egg</param>
    /// <param name="version">Game version of the egg</param>
    /// <returns>Egg Location value</returns>
    /// <remarks>Location will be set to the Met Location until it hatches, then moves to Egg Location.</remarks>
    public static ushort TradedEggLocation(byte generation, GameVersion version) => generation switch
    {
        4 => LinkTrade4,
        5 => LinkTrade5,
        8 when GameVersion.BDSP.Contains(version) => LinkTrade6NPC,
        _ => LinkTrade6,
    };

    public static bool IsPtHGSSLocation(ushort location) => location is > 111 and < 2000;
    public static bool IsPtHGSSLocationEgg(ushort location) => location is > 2010 and < 3000;
    public static bool IsEventLocation3(ushort location) => location is 255;
    public static bool IsEventLocation4(ushort location) => location is >= 3000 and <= 3076;
    public static bool IsEventLocation5(ushort location) => location is > 40000 and < 50000;

    private const int SafariLocation_RSE = 57;
    private const int SafariLocation_FRLG = 136;
    public static bool IsSafariZoneLocation3(byte loc) => loc is SafariLocation_RSE or SafariLocation_FRLG;
    public static bool IsSafariZoneLocation3RSE(byte loc) => loc == SafariLocation_RSE;

    public static bool IsEggLocationBred4(ushort loc, GameVersion version)
    {
        if (loc is Daycare4 or LinkTrade4)
            return true;
        return loc == Faraway4 && version is GameVersion.Pt or GameVersion.HG or GameVersion.SS;
    }

    public static bool IsEggLocationBred5(ushort loc) => loc is Daycare5 or LinkTrade5;
    public static bool IsEggLocationBred6(ushort loc) => loc is Daycare5 or LinkTrade6;
    public static bool IsEggLocationBred8b(ushort loc) => loc is Daycare8b or LinkTrade6NPC;
    public static bool IsEggLocationBred9(ushort loc) => loc is Picnic9 or LinkTrade6;

    public static ushort GetDaycareLocation(byte generation, GameVersion version) => generation switch
    {
        1 or 2 or 3 => 0,
        4 => Daycare4,
        5 => Daycare5,
        8 when version is GameVersion.BD or GameVersion.SP => Daycare8b,
        9 => Picnic9,
        _ => Daycare5,
    };

    public static bool IsMetLocation3RS(ushort z) => z <= 87; // Ferry
    public static bool IsMetLocation3E(ushort z) => z is <= 87 or (>= 197 and <= 212); // Trainer Hill
    public static bool IsMetLocation3FRLG(ushort z) => z is > 87 and < 197; // Celadon Dept.
    public static bool IsMetLocation4DP(ushort z) => z <= 111; // Battle Park
    public static bool IsMetLocation4Pt(ushort z) => z <= 125; // Rock Peak Ruins
    public static bool IsMetLocation4HGSS(ushort z) => z is > 125 and < 234; // Celadon Dept.
    public static bool IsMetLocation5BW(ushort z) => z <= 116; // Abyssal Ruins
    public static bool IsMetLocation6XY(ushort z) => z <= 168; // Unknown Dungeon
    public static bool IsMetLocation6AO(ushort z) => z is > 168 and <= 354; // Secret Base
    public static bool IsMetLocation7SM(ushort z) => z < 200; // Outer Cape
    public static bool IsMetLocation7USUM(ushort z) => z < 234; // Dividing Peak Tunnel
    public static bool IsMetLocation7GG(ushort z) => z <= 54; // Pokémon League
    public static bool IsMetLocation8SWSH(ushort z) => z <= 246; // Crown Tundra Station
    public static bool IsMetLocation8BDSP(ushort z) => z <= 657; // Ramanas Park (Genome Room)
    public static bool IsMetLocation8LA(ushort z) => z <= 155; // Training Grounds
    public static bool IsMetLocation9SV(ushort z) => z <= 200; // Terarium (Entry Tunnel)
}
