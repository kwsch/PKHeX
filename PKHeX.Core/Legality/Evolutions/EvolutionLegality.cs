namespace PKHeX.Core;

internal static class EvolutionLegality
{
    public static bool IsEvolvedFromGen1Species(ushort species) => species switch
    {
        (int)Species.Crobat => true,
        (int)Species.Bellossom => true,
        (int)Species.Politoed => true,
        (int)Species.Espeon => true,
        (int)Species.Umbreon => true,
        (int)Species.Slowking => true,
        (int)Species.Steelix => true,
        (int)Species.Scizor => true,
        (int)Species.Kingdra => true,
        (int)Species.Porygon2 => true,
        (int)Species.Blissey => true,
        (int)Species.Magnezone => true,
        (int)Species.Lickilicky => true,
        (int)Species.Rhyperior => true,
        (int)Species.Tangrowth => true,
        (int)Species.Electivire => true,
        (int)Species.Magmortar => true,
        (int)Species.Leafeon => true,
        (int)Species.Glaceon => true,
        (int)Species.PorygonZ => true,
        (int)Species.Sylveon => true,
        (int)Species.Kleavor => true,
        _ => false,
    };
}
