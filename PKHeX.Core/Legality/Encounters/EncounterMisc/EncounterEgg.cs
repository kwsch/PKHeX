using System;

namespace PKHeX.Core;

/// <summary>
/// Egg Encounter Data
/// </summary>
public sealed record EncounterEgg(ushort Species, byte Form, byte Level, int Generation, GameVersion Version, EntityContext Context) : IEncounterable
{
    public string Name => "Egg";
    public string LongName => "Egg";

    public bool EggEncounter => true;
    public byte LevelMin => Level;
    public byte LevelMax => Level;
    public bool IsShiny => false;
    public int Location => 0;
    public int EggLocation => Locations.GetDaycareLocation(Generation, Version);
    public Ball FixedBall => BallBreedLegality.GetDefaultBall(Version, Species);
    public Shiny Shiny => Shiny.Random;
    public AbilityPermission Ability => AbilityPermission.Any12H;

    public bool CanHaveVoltTackle => Species is (int)Core.Species.Pichu && (Generation > 3 || Version is GameVersion.E);

    public PKM ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PKM ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int gen = Generation;
        var version = Version;
        var pk = EntityBlank.GetBlank(gen, version);

        tr.ApplyTo(pk);

        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language, version);
        pk.Species = Species;
        pk.Form = Form;
        pk.Language = lang;
        pk.Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, gen);
        pk.CurrentLevel = Level;
        pk.Version = (int)version;

        var ball = FixedBall;
        pk.Ball = ball is Ball.None ? (int)Ball.Poke : (int)ball;
        pk.OT_Friendship = pk.PersonalInfo.BaseFriendship;

        SetEncounterMoves(pk, version);
        pk.HealPP();
        SetPINGA(pk, criteria);

        if (gen <= 2)
        {
            if (version != GameVersion.C)
            {
                pk.OT_Gender = 0;
            }
            else
            {
                pk.Met_Location = Locations.HatchLocationC;
                pk.Met_Level = 1;
                ((PK2)pk).Met_TimeOfDay = Util.Rand.Next(1, 4); // Morning | Day | Night
            }
            return pk;
        }

        SetMetData(pk);

        if (gen >= 4)
            pk.SetEggMetData(version, (GameVersion)tr.Game);

        if (gen < 6)
            return pk;
        if (pk is PK6 pk6)
            pk6.SetHatchMemory6();

        SetForm(pk, tr);

        pk.SetRandomEC();
        pk.RelearnMove1 = pk.Move1;
        pk.RelearnMove2 = pk.Move2;
        pk.RelearnMove3 = pk.Move3;
        pk.RelearnMove4 = pk.Move4;
        if (pk is IScaledSize s)
        {
            s.HeightScalar = PokeSizeUtil.GetRandomScalar();
            s.WeightScalar = PokeSizeUtil.GetRandomScalar();
            if (pk is IScaledSize3 s3)
                s3.Scale = PokeSizeUtil.GetRandomScalar();
        }

        if (pk is ITeraType tera)
        {
            var type = Tera9RNG.GetTeraTypeFromPersonal(Species, Form, Util.Rand.Rand64());
            tera.TeraTypeOriginal = (MoveType)type;
            if (criteria.TeraType != -1 && type != criteria.TeraType)
                tera.SetTeraType(type); // sets the override type
        }

        return pk;
    }

    private void SetForm(PKM pk, ITrainerInfo sav)
    {
        switch (Species)
        {
            case (int)Core.Species.Minior:
                pk.Form = (byte)Util.Rand.Next(7, 14);
                break;
            case (int)Core.Species.Scatterbug or (int)Core.Species.Spewpa or (int)Core.Species.Vivillon:
                if (sav.Generation is 6 or 7 && sav is IRegionOrigin o)
                    pk.Form = Vivillon3DS.GetPattern(o.Country, o.Region);
                // else keep original value
                break;
        }
    }

    private static void SetPINGA(PKM pk, EncounterCriteria criteria)
    {
        pk.SetRandomIVs(minFlawless: 3);
        if (pk.Format <= 2)
            return;

        int gender = criteria.GetGender(-1, pk.PersonalInfo);
        int nature = (int)criteria.GetNature(Nature.Random);

        if (pk.Format <= 5)
        {
            pk.SetPIDGender(gender);
            pk.Gender = gender;
            pk.SetPIDNature(nature);
            pk.Nature = nature;
            pk.RefreshAbility(pk.PIDAbility);
        }
        else
        {
            pk.PID = Util.Rand32();
            pk.Nature = nature;
            pk.Gender = gender;
            pk.RefreshAbility(Util.Rand.Next(2));
        }
        pk.StatNature = nature;
    }

    private void SetMetData(PKM pk)
    {
        pk.Met_Level = EggStateLegality.GetEggLevelMet(Version, Generation);
        pk.Met_Location = Math.Max(0, EggStateLegality.GetEggHatchLocation(Version, Generation));

        if (pk is IObedienceLevel l)
            l.Obedience_Level = (byte)pk.Met_Level;
    }

    private void SetEncounterMoves(PKM pk, GameVersion version)
    {
        var ls = GameData.GetLearnSource(version);
        var learn = ls.GetLearnset(Species, Form);
        var initial = learn.GetBaseEggMoves(LevelMin);
        pk.SetMoves(initial);
    }
}
