using System;

namespace PKHeX.Core;

public static class EncounterUtil1
{
    public const int FormDynamic = FormVivillon;
    public const byte FormVivillon = 30;
    public const byte FormRandom = 31;

    public static ushort GetRandomDVs(Random rand) => (ushort)rand.Next(ushort.MaxValue + 1);

    public static byte GetWildCatchRate(GameVersion version, ushort species) => (byte)(version == GameVersion.YW ? PersonalTable.Y : PersonalTable.RB)[species].CatchRate;
    public static (byte Type1, byte Type2) GetTypes(GameVersion version, ushort species)
    {
        var pi = GetPersonal1(version, species);
        return (pi.Type1, pi.Type2);
    }

    public static PersonalInfo1 GetPersonal1(GameVersion version, ushort species)
    {
        var pt = version == GameVersion.YW ? PersonalTable.Y : PersonalTable.RB;
        return pt[species];
    }

    public static void SetEncounterMoves<T>(T pk, GameVersion version, byte level) where T : PKM
    {
        Span<ushort> moves = stackalloc ushort[4];
        var source = GameData.GetLearnSource(version);
        source.SetEncounterMoves(pk.Species, pk.Form, level, moves);
        pk.SetMoves(moves);
    }

    public static string GetTrainerName(ITrainerInfo tr, int lang) => lang switch
    {
        (int)LanguageID.Japanese => tr.Language == 1 ? tr.OT : "ゲーフリ",
        _ => tr.Language == 1 ? "GF" : tr.OT,
    };

    public static ushort GetDV16(in IndividualValueSet actual)
    {
        ushort result = 0;
        result |= (ushort)(actual.SPA << 0);
        result |= (ushort)(actual.SPE << 4);
        result |= (ushort)(actual.DEF << 8);
        result |= (ushort)(actual.ATK << 12);
        return result;
    }
}
