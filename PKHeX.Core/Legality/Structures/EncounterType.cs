namespace PKHeX.Core
{
    public enum EncounterType
    {
        Headbutt_Grass = -1, // None or TallGrass
        Headbutt_Surf = -2, // None or Surfing_Fishing
        Headbutt_GrassSurf = -3, // None, TallGrass or Surfing_Fishing
        Headbutt_CitySurf = -4, // Building_EnigmaStone or Surfing_Fishing
        Headbutt_CaveSurf = -5, // Cave_HallOfOrigin or Surfing_Fishing
        None = 0,
        RockSmash = 1,
        TallGrass = 2,
        DialgaPalkia = 4,
        Cave_HallOfOrigin = 5,
        Surfing_Fishing = 7,
        Building_EnigmaStone = 9,
        MarshSafari = 10,
        Starter_Fossil_Gift_DP = 12,
        DistortionWorld_Pt = 23,
        Starter_Fossil_Gift_Pt_DPTrio = 24,
    }

    public static class EncounterTypeExtension
    {
        public static bool Contains(this EncounterType g1, int g2)
        {
            return g1.Contains((EncounterType)g2);
        }
        private static bool Contains(this EncounterType g1, EncounterType g2)
        {
            switch (g1)
            {
                case EncounterType.Headbutt_Grass:
                    return g2 == EncounterType.None || g2 == EncounterType.TallGrass;
                case EncounterType.Headbutt_Surf:
                    return g2 == EncounterType.None || g2 == EncounterType.Surfing_Fishing;
                case EncounterType.Headbutt_GrassSurf:
                    return EncounterType.Headbutt_Grass.Contains(g2) || g2 == EncounterType.Surfing_Fishing;
                case EncounterType.Headbutt_CitySurf:
                    return g2 == EncounterType.Building_EnigmaStone || g2 == EncounterType.Surfing_Fishing;
                case EncounterType.Headbutt_CaveSurf:
                    return g2 == EncounterType.Cave_HallOfOrigin || g2 == EncounterType.Surfing_Fishing;
            }

            return g1 == g2;
        }
    }
}
