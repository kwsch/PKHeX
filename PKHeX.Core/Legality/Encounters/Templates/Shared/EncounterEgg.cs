using System;

namespace PKHeX.Core;

/// <summary>
/// Egg Encounter Data
/// </summary>
public sealed record EncounterEgg(ushort Species, byte Form, byte Level, byte Generation, GameVersion Version, EntityContext Context) : IEncounterable
{
    public string Name => "Egg";
    public string LongName => "Egg";

    public bool IsEgg => true;
    public byte LevelMin => Level;
    public byte LevelMax => Level;
    public bool IsShiny => false;
    public ushort Location => 0;
    public ushort EggLocation => Locations.GetDaycareLocation(Generation, Version);
    public Ball FixedBall => Generation <= 5 ? Ball.Poke : Ball.None;
    public Shiny Shiny => Shiny.Random;
    public AbilityPermission Ability => AbilityPermission.Any12H;

    public bool CanHaveVoltTackle => Species is (int)Core.Species.Pichu && (Generation > 3 || Version is GameVersion.E);

    public PKM ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PKM ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        var gen = Generation;
        var version = Version;
        var pk = EntityBlank.GetBlank(gen, version);

        tr.ApplyTo(pk);

        int lang = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language, version);
        pk.Species = Species;
        pk.Form = Form;
        pk.Language = lang;
        pk.Nickname = SpeciesName.GetSpeciesNameGeneration(Species, lang, gen);
        pk.CurrentLevel = Level;
        pk.Version = version;

        var ball = FixedBall;
        pk.Ball = ball is Ball.None ? (byte)Ball.Poke : (byte)ball;
        pk.OriginalTrainerFriendship = EggStateLegality.GetEggHatchFriendship(Context);

        SetEncounterMoves(pk, version);
        pk.HealPP();
        var rnd = Util.Rand;
        SetPINGA(pk, criteria);

        if (gen <= 2)
        {
            var pk2 = (PK2)pk;
            if (version == GameVersion.C)
            {
                // Set met data for Crystal hatch.
                pk2.MetLocation = Locations.HatchLocationC;
                pk2.MetLevel = 1;
                pk2.MetTimeOfDay = rnd.Next(1, 4); // Morning | Day | Night
            }
            else // G/S
            {
                // G/S can't set any data for Trainer Gender.
                pk2.OriginalTrainerGender = 0;
            }

            // No other revisions needed.
            return pk2;
        }

        SetMetData(pk);

        if (gen >= 4)
            pk.SetEggMetData(version, tr.Version);

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
            s.HeightScalar = PokeSizeUtil.GetRandomScalar(rnd);
            s.WeightScalar = PokeSizeUtil.GetRandomScalar(rnd);
            if (pk is IScaledSize3 s3)
                s3.Scale = PokeSizeUtil.GetRandomScalar(rnd);
        }

        if (pk is ITeraType tera)
        {
            var type = Tera9RNG.GetTeraTypeFromPersonal(Species, Form, rnd.Rand64());
            tera.TeraTypeOriginal = (MoveType)type;
            if (criteria.IsSpecifiedTeraType() && type != criteria.TeraType)
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
        criteria.SetRandomIVs(pk, 3);
        if (pk.Format <= 2)
            return;

        var gender = criteria.GetGender(pk.PersonalInfo);
        var nature = criteria.GetNature();

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
            pk.Nature = pk.StatNature = nature;
            pk.Gender = gender;
            pk.RefreshAbility(Util.Rand.Next(2));
        }
    }

    private void SetMetData(PKM pk)
    {
        pk.MetLevel = EggStateLegality.GetEggLevelMet(Version, Generation);
        pk.MetLocation = Math.Max((ushort)0, EggStateLegality.GetEggHatchLocation(Version, Generation));

        if (pk is IObedienceLevel l)
            l.ObedienceLevel = pk.MetLevel;
    }

    private void SetEncounterMoves(PKM pk, GameVersion version)
    {
        var ls = GameData.GetLearnSource(version);
        var learn = ls.GetLearnset(Species, Form);
        var initial = learn.GetBaseEggMoves(LevelMin);
        pk.SetMoves(initial);
    }
}
