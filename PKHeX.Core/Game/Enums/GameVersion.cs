namespace PKHeX.Core;

/// <summary>
/// Game Version ID enum shared between actual Version IDs and lumped version groupings.
/// </summary>
public enum GameVersion : byte
{
    #region Indicators for method empty arguments & result indication. Not stored values.
    Any = 0,
    Invalid = byte.MaxValue,
    #endregion

    // The following values are IDs stored within PKM data, and can also identify individual games.

    #region Gen3
    /// <summary>
    /// Pokémon Sapphire (GBA)
    /// </summary>
    S = 1,

    /// <summary>
    /// Pokémon Ruby (GBA)
    /// </summary>
    R = 2,

    /// <summary>
    /// Pokémon Emerald (GBA)
    /// </summary>
    E = 3,

    /// <summary>
    /// Pokémon FireRed (GBA)
    /// </summary>
    FR = 4,

    /// <summary>
    /// Pokémon LeafGreen (GBA)
    /// </summary>
    LG = 5,

    /// <summary>
    /// Pokémon Colosseum &amp; Pokémon XD (GameCube)
    /// </summary>
    CXD = 15,
    #endregion

    #region Gen4
    /// <summary>
    /// Pokémon Diamond (NDS)
    /// </summary>
    D = 10,

    /// <summary>
    /// Pokémon Pearl (NDS)
    /// </summary>
    P = 11,

    /// <summary>
    /// Pokémon Platinum (NDS)
    /// </summary>
    Pt = 12,

    /// <summary>
    /// Pokémon HeartGold (NDS)
    /// </summary>
    HG = 7,

    /// <summary>
    /// Pokémon SoulSilver (NDS)
    /// </summary>
    SS = 8,
    #endregion

    #region Gen5
    /// <summary>
    /// Pokémon White (NDS)
    /// </summary>
    W = 20,

    /// <summary>
    /// Pokémon Black (NDS)
    /// </summary>
    B = 21,

    /// <summary>
    /// Pokémon White 2 (NDS)
    /// </summary>
    W2 = 22,

    /// <summary>
    /// Pokémon Black 2 (NDS)
    /// </summary>
    B2 = 23,
    #endregion

    #region Gen6
    /// <summary>
    /// Pokémon X (3DS)
    /// </summary>
    X = 24,

    /// <summary>
    /// Pokémon Y (3DS)
    /// </summary>
    Y = 25,

    /// <summary>
    /// Pokémon Alpha Sapphire (3DS)
    /// </summary>
    AS = 26,

    /// <summary>
    /// Pokémon Omega Ruby (3DS)
    /// </summary>
    OR = 27,
    #endregion

    #region Gen7
    /// <summary>
    /// Pokémon Sun (3DS)
    /// </summary>
    SN = 30,

    /// <summary>
    /// Pokémon Moon (3DS)
    /// </summary>
    MN = 31,

    /// <summary>
    /// Pokémon Ultra Sun (3DS)
    /// </summary>
    US = 32,

    /// <summary>
    /// Pokémon Ultra Moon (3DS)
    /// </summary>
    UM = 33,
    #endregion

    /// <summary>
    /// Pokémon GO (GO -> Let's Go/HOME transfers)
    /// </summary>
    GO = 34,

    #region Virtual Console (3DS) Gen1
    /// <summary>
    /// Pokémon Red (3DS Virtual Console)
    /// </summary>
    RD = 35,

    /// <summary>
    /// Pokémon Green[JP]/Blue[INT] (3DS Virtual Console)
    /// </summary>
    GN = 36,

    /// <summary>
    /// Pokémon Blue[JP] (3DS Virtual Console)
    /// </summary>
    BU = 37,

    /// <summary>
    /// Pokémon Yellow (3DS Virtual Console)
    /// </summary>
    YW = 38,
    #endregion

    #region Virtual Console (3DS) Gen2
    /// <summary>
    /// Pokémon Gold (3DS Virtual Console)
    /// </summary>
    GD = 39,

    /// <summary>
    /// Pokémon Silver (3DS Virtual Console)
    /// </summary>
    SI = 40,

    /// <summary>
    /// Pokémon Crystal (3DS Virtual Console)
    /// </summary>
    C = 41,
    #endregion

    #region Nintendo Switch
    /// <summary>
    /// Pokémon: Let's Go, Pikachu! (NX)
    /// </summary>
    GP = 42,

    /// <summary>
    /// Pokémon: Let's Go, Eevee! (NX)
    /// </summary>
    GE = 43,

    /// <summary>
    /// Pokémon Sword (NX)
    /// </summary>
    SW = 44,

    /// <summary>
    /// Pokémon Shield (NX)
    /// </summary>
    SH = 45,

    // HOME = 46,

    /// <summary>
    /// Pokémon Legends: Arceus (NX)
    /// </summary>
    PLA = 47,

    /// <summary>
    /// Pokémon Brilliant Diamond (NX)
    /// </summary>
    BD = 48,

    /// <summary>
    /// Pokémon Shining Pearl (NX)
    /// </summary>
    SP = 49,

    /// <summary>
    /// Pokémon Scarlet (NX)
    /// </summary>
    SL = 50,

    /// <summary>
    /// Pokémon Violet (NX)
    /// </summary>
    VL = 51,
    #endregion

    // The following values are not actually stored values in pk data,
    // These values are assigned within PKHeX as properties for various logic branching.

    #region Game Groupings (SaveFile type, roughly)
    /// <summary>
    /// Pokémon Red &amp; Blue [<see cref="SAV1"/>] identifier.
    /// </summary>
    /// <seealso cref="RD"/>
    /// <seealso cref="GN"/>
    /// <seealso cref="BU"/>
    RB,

    /// <summary>
    /// Pokémon Red/Blue/Yellow [<see cref="SAV1"/>] identifier.
    /// </summary>
    /// <see cref="RD"/>
    /// <see cref="GN"/>
    /// <see cref="BU"/>
    /// <see cref="YW"/>
    RBY,

    /// <summary>
    /// Pokémon Gold &amp; Silver [<see cref="SAV2"/>] identifier.
    /// </summary>
    /// <see cref="GD"/>
    /// <see cref="SI"/>
    GS,

    /// <summary>
    /// Pokémon Gold/Silver/Crystal [<see cref="SAV2"/>] identifier.
    /// </summary>
    /// <see cref="GD"/>
    /// <see cref="SI"/>
    /// <see cref="C"/>
    GSC,

    /// <summary>
    /// Pokémon Ruby &amp; Sapphire [<see cref="SAV3RS"/>] identifier.
    /// </summary>
    /// <see cref="R"/>
    /// <see cref="S"/>
    RS,

    /// <summary>
    /// Pokémon Ruby/Sapphire/Emerald [<see cref="SAV3"/>] identifier.
    /// </summary>
    /// <see cref="R"/>
    /// <see cref="S"/>
    /// <see cref="E"/>
    RSE,

    /// <summary>
    /// Pokémon FireRed/LeafGreen [<see cref="SAV3FRLG"/>] identifier.
    /// </summary>
    /// <see cref="FR"/>
    /// <see cref="LG"/>
    FRLG,

    /// <summary>
    /// Pokémon Box Ruby &amp; Sapphire [<see cref="SAV3RSBox"/>] identifier.
    /// </summary>
    RSBOX,

    /// <summary>
    /// Pokémon Colosseum [<see cref="SAV3Colosseum"/>] identifier.
    /// </summary>
    /// <see cref="CXD"/>
    /// <remarks>Also used to mark Colosseum-only origin data as this game shares a version ID with <see cref="XD"/></remarks>
    COLO,

    /// <summary>
    /// Pokémon XD [<see cref="SAV3XD"/>] identifier.
    /// </summary>
    /// <see cref="CXD"/>
    /// <remarks>Also used to mark XD-only origin data as this game shares a version ID with <see cref="COLO"/></remarks>
    XD,

    /// <summary>
    /// Pokémon Diamond &amp; Pearl [<see cref="SAV4DP"/>] identifier.
    /// </summary>
    /// <see cref="D"/>
    /// <see cref="P"/>
    DP,

    /// <summary>
    /// Pokémon Diamond/Pearl/Platinum version group.
    /// </summary>
    /// <remarks>Used to lump data from the associated games as data assets are shared.</remarks>
    /// <see cref="D"/>
    /// <see cref="P"/>
    /// <see cref="Pt"/>
    DPPt,

    /// <summary>
    /// Pokémon HeartGold &amp; SoulSilver [<see cref="SAV4HGSS"/>] identifier.
    /// </summary>
    /// <see cref="HG"/>
    /// <see cref="SS"/>
    HGSS,

    /// <summary>
    /// Pokémon Battle Revolution [<see cref="SAV4BR"/>] identifier.
    /// </summary>
    BATREV,

    /// <summary>
    /// Pokémon Black &amp; White version group.
    /// </summary>
    /// <remarks>Used to lump data from the associated games as data assets are shared.</remarks>
    /// <see cref="B"/>
    /// <see cref="W"/>
    BW,

    /// <summary>
    /// Pokémon Black 2 &amp; White 2 version group.
    /// </summary>
    /// <remarks>Used to lump data from the associated games as data assets are shared.</remarks>
    /// <see cref="B2"/>
    /// <see cref="W2"/>
    B2W2,

    /// <summary>
    /// Pokémon X &amp; Y
    /// </summary>
    /// <remarks>Used to lump data from the associated games as data assets are shared.</remarks>
    /// <see cref="X"/>
    /// <see cref="Y"/>
    XY,

    /// <summary>
    /// Pokémon Omega Ruby &amp; Alpha Sapphire Demo [<see cref="SAV6AODemo"/>] identifier.
    /// </summary>
    /// <see cref="ORAS"/>
    ORASDEMO,

    /// <summary>
    /// Pokémon Omega Ruby &amp; Alpha Sapphire version group.
    /// </summary>
    /// <remarks>Used to lump data from the associated games as data assets are shared.</remarks>
    /// <see cref="OR"/>
    /// <see cref="AS"/>
    ORAS,

    /// <summary>
    /// Pokémon Sun &amp; Moon
    /// </summary>
    /// <remarks>Used to lump data from the associated games as data assets are shared.</remarks>
    /// <see cref="SN"/>
    /// <see cref="MN"/>
    SM,

    /// <summary>
    /// Pokémon Ultra Sun &amp; Ultra Moon
    /// </summary>
    /// <remarks>Used to lump data from the associated games as data assets are shared.</remarks>
    /// <see cref="US"/>
    /// <see cref="UM"/>
    USUM,

    /// <summary>
    /// Pokémon Let's Go Pikachu &amp; Eevee
    /// </summary>
    /// <remarks>Used to lump data from the associated games as data assets are shared.</remarks>
    /// <see cref="GP"/>
    /// <see cref="GE"/>
    GG,

    /// <summary>
    /// Pokémon Sword &amp; Shield
    /// </summary>
    /// <remarks>Used to lump data from the associated games as data assets are shared.</remarks>
    /// <see cref="SW"/>
    /// <see cref="SH"/>
    SWSH,

    /// <summary>
    /// Pokémon Brilliant Diamond &amp; Shining Pearl
    /// </summary>
    /// <remarks>Used to lump data from the associated games as data assets are shared.</remarks>
    /// <see cref="BD"/>
    /// <see cref="SP"/>
    BDSP,

    /// <summary>
    /// Pokémon Scarlet &amp; Violet
    /// </summary>
    /// <remarks>Used to lump data from the associated games as data assets are shared.</remarks>
    /// <see cref="SL"/>
    /// <see cref="VL"/>
    SV,

    /// <summary>
    /// Generation 1 Games
    /// </summary>
    /// <see cref="RBY"/>
    Gen1,

    /// <summary>
    /// Generation 2 Games
    /// </summary>
    /// <see cref="GSC"/>
    Gen2,

    /// <summary>
    /// Generation 3 Games
    /// </summary>
    /// <see cref="RSE"/>
    /// <see cref="FRLG"/>
    Gen3,

    /// <summary>
    /// Generation 4 Games
    /// </summary>
    /// <see cref="DPPt"/>
    /// <see cref="HGSS"/>
    Gen4,

    /// <summary>
    /// Generation 5 Games
    /// </summary>
    /// <see cref="BW"/>
    /// <see cref="B2W2"/>
    Gen5,

    /// <summary>
    /// Generation 6 Games
    /// </summary>
    /// <see cref="XY"/>
    /// <see cref="ORAS"/>
    Gen6,

    /// <summary>
    /// Generation 7 Games on the Nintendo 3DS
    /// </summary>
    /// <see cref="SM"/>
    /// <see cref="USUM"/>
    Gen7,

    /// <summary>
    /// Generation 7 Games on the Nintendo Switch
    /// </summary>
    /// <see cref="GG"/>
    /// <see cref="GO"/>
    Gen7b,

    /// <summary>
    /// Generation 8 Games
    /// </summary>
    /// <see cref="SWSH"/>
    /// <see cref="BDSP"/>
    /// <see cref="PLA"/>
    Gen8,

    /// <summary>
    /// Generation 9 Games
    /// </summary>
    /// <see cref="SV"/>
    Gen9,

    /// <summary>
    /// Pocket Monsters Stadium data origin identifier
    /// </summary>
    StadiumJ,

    /// <summary>
    /// Pokémon Stadium data origin identifier
    /// </summary>
    Stadium,

    /// <summary>
    /// Pokémon Stadium 2 data origin identifier
    /// </summary>
    Stadium2,
    #endregion
}
