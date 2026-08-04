using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Diagnostics;
using static PKHeX.Core.Species;

namespace PKHeX.Core.AutoMod;

/// <summary>
/// Miscellaneous enhancement methods
/// </summary>
public static class ModLogic
{
    // Living Dex Settings
    public static LivingDexConfig Config { get; set; } = new()
    {
        IncludeForms = false,
        SetShiny = false,
        SetAlpha = false,
        TransferVersion = GameVersion.SL,
    };

    public static bool IncludeForms { get; set; }
    public static bool SetShiny { get; set; }
    public static bool SetAlpha { get; set; }
    public static GameVersion TransferVersion { get; set; }

    /// <summary>
    /// Exports the <see cref="SaveFile.CurrentBox"/> to <see cref="ShowdownSet"/> as a single string.
    /// </summary>
    /// <param name="provider">Save File to export from</param>
    /// <returns>Concatenated string of all sets in the current box.</returns>
    public static string GetRegenSetsFromBoxCurrent(this ISaveFileProvider provider) => GetRegenSetsFromBox(provider.SAV, provider.CurrentBox);

    /// <summary>
    /// Exports the <see cref="box"/> to <see cref="ShowdownSet"/> as a single string.
    /// </summary>
    /// <param name="sav">Save File to export from</param>
    /// <param name="box">Box to export from</param>
    /// <returns>Concatenated string of all sets in the specified box.</returns>
    public static string GetRegenSetsFromBox(this SaveFile sav, int box)
    {
        Span<PKM> data = sav.GetBoxData(box);
        var sep = Environment.NewLine + Environment.NewLine;
        return data.GetRegenSets(sep);
    }

    /// <summary>
    /// Gets a living dex (one per species, not every form)
    /// </summary>
    /// <param name="sav">Save File to receive the generated <see cref="PKM"/>.</param>
    /// <param name="personal">Personal table containing species and form data.</param>
    /// <returns>Consumable list of newly generated <see cref="PKM"/> data.</returns>
    public static IEnumerable<PKM> GenerateLivingDex(this ITrainerInfo sav, IPersonalTable personal) => sav.GenerateLivingDex(personal, Config);

    /// <summary>
    /// Gets a living dex (one per species, not every form)
    /// </summary>
    /// <param name="sav">Save File to receive the generated <see cref="PKM"/>.</param>
    /// <param name="personal">Personal table containing species and form data.</param>
    /// <param name="cfg">Configuration specifying living dex options.</param>
    /// <returns>Consumable list of newly generated <see cref="PKM"/> data.</returns>
    public static IEnumerable<PKM> GenerateLivingDex(this ITrainerInfo sav, IPersonalTable personal, LivingDexConfig cfg)
    {
        var pklist = new ConcurrentBag<PKM>();
        var tr = APILegality.UseTrainerData ? TrainerSettings.GetSavedTrainerData(sav.Version) : sav;
        var context = sav.Context;
        var generation = sav.Generation;
        TrackingCount = 0;
        Parallel.For(1, personal.MaxSpeciesID+1, id => //parallel For's end is exclusive
        {
            var s = (ushort)id;
            if (!personal.IsSpeciesInGame(s))
                return;

            var num_forms = personal[s].FormCount;
            var str = GameInfo.Strings;
            if (num_forms == 1 && cfg.IncludeForms) // Validate through form lists
                num_forms = (byte)FormConverter.GetFormList(s, str.types, str.forms, GameInfo.GenderSymbolUnicode, context).Length;
            if (s == (ushort)Alcremie)
                num_forms = (byte)(num_forms * 6);
            uint formarg = 0;
            byte acform = 0;
            for (byte f = 0; f < num_forms; f++)
            {
                var form = cfg.IncludeForms ? f : GetBaseForm((Species)s, f, sav);
                if (s == (ushort)Alcremie)
                {
                    form = acform;
                    if (f % 6 == 0)
                    {
                        acform++;
                        formarg = 0;
                    }
                    else
                    {
                        formarg++;
                    }
                }

                if (!personal.IsPresentInGame(s, form) || FormInfo.IsLordForm(s, form, context) || FormInfo.IsBattleOnlyForm(s, form, generation) || FormInfo.IsFusedForm(s, form, generation) || (FormInfo.IsTotemForm(s, form) && context is not EntityContext.Gen7))
                    continue;
                var pk = AddPKM(sav, tr, s, form, cfg.SetShiny, cfg.SetAlpha);
                if (pk is null || pklist.Any(x => x.Species == pk.Species && x.Form == pk.Form && x.Species != 869))
                    continue;

                if (s == (ushort)Alcremie)
                    pk.ChangeFormArgument(formarg);
                pklist.Add(pk);
                if (!cfg.IncludeForms)
                    break;
            }
            TrackingCount++;
        });
        return pklist.OrderBy(z => z.Species);
    }
    public static int TrackingCount { get; set; }
    /// <summary>
    /// Generates a living dex for transfer between games, considering both source and destination game restrictions.
    /// </summary>
    /// <param name="src">The source trainer information.</param>
    /// <returns>An enumerable of generated <see cref="PKM"/> objects valid for transfer.</returns>
    public static IEnumerable<PKM> GenerateTransferLivingDex(this ITrainerInfo src) => src.GenerateTransferLivingDex(Config);
    /// <summary>
    /// Generates a living dex for transfer between games, considering both source and destination game restrictions.
    /// </summary>
    /// <param name="src">The source trainer information.</param>
    /// <param name="cfg">The living dex configuration specifying transfer options.</param>
    /// <returns>An enumerable of generated <see cref="PKM"/> objects valid for transfer.</returns>
    public static IEnumerable<PKM> GenerateTransferLivingDex(this ITrainerInfo src, LivingDexConfig cfg)
    {
        var srcPersonal = GameData.GetPersonal(src.Version);

        // Destination restrictions
        var destPersonal = GameData.GetPersonal(cfg.TransferVersion);
        var context = cfg.TransferVersion.Context;
        var generation = cfg.TransferVersion.Generation;

        ConcurrentBag<PKM> pklist = [];
        var tr = APILegality.UseTrainerData ? TrainerSettings.GetSavedTrainerData(src.Version, lang: (LanguageID)src.Language) : src;

        Parallel.For(1, srcPersonal.MaxSpeciesID + 1, id => //parallel For's end is exclusive
        {
            var s = (ushort)id;
            if (!srcPersonal.IsSpeciesInGame(s))
                return;
            if (!destPersonal.IsSpeciesInGame(s))
                return;
            var num_forms = srcPersonal[s].FormCount;
            var str = GameInfo.Strings;
            if (num_forms == 1 && cfg.IncludeForms) // Validate through form lists
                num_forms = (byte)FormConverter.GetFormList(s, str.types, str.forms, GameInfo.GenderSymbolUnicode, context).Length;

            for (byte f = 0; f < num_forms; f++)
            {
                if (!destPersonal.IsPresentInGame(s, f) || FormInfo.IsLordForm(s, f, context) || FormInfo.IsBattleOnlyForm(s, f, generation) || FormInfo.IsFusedForm(s, f, generation) || (FormInfo.IsTotemForm(s, f) && context is not EntityContext.Gen7))
                    continue;
                var form = cfg.IncludeForms ? f : GetBaseForm((Species)s, f, src);
                var pk = AddPKM(src, tr, s, form, cfg.SetShiny, cfg.SetAlpha);
                if (pk is null || pklist.Any(x => x.Species == pk.Species && x.Form == pk.Form))
                    continue;

                pklist.Add(pk);
                if (!cfg.IncludeForms)
                    break;
            }
        });
        return pklist.OrderBy(z => z.Species);
    }

    private static bool IsRegionalSV(Species s) => s is Tauros or Wooper;
    private static bool IsRegionalLA(Species s) => s is Growlithe or Arcanine or Voltorb or Electrode or Typhlosion or Qwilfish or Sneasel or Samurott or Lilligant or Zorua or Zoroark or Braviary or Sliggoo or Goodra or Avalugg or Decidueye;
    private static bool IsRegionalSH(Species s) => s is Meowth or Slowpoke or Ponyta or Rapidash or Slowbro or Slowking or Farfetchd or Weezing or MrMime or Articuno or Moltres or Zapdos or Corsola or Zigzagoon or Linoone or Darumaka or Darmanitan or Yamask or Stunfisk;
    private static bool IsRegionalSM(Species s) => s is Rattata or Raticate or Raichu or Sandshrew or Sandslash or Vulpix or Ninetales or Diglett or Dugtrio or Meowth or Persian or Geodude or Graveler or Golem or Grimer or Muk or Marowak;

    public static byte GetBaseForm(Species s, byte f, ITrainerInfo sav)
    {
        var HasRegionalForm = sav.Version switch
        {
            GameVersion.VL or GameVersion.SL => IsRegionalSV(s),
            GameVersion.PLA => IsRegionalLA(s),
            GameVersion.SH or GameVersion.SW => IsRegionalSH(s),
            GameVersion.SN or GameVersion.MN or GameVersion.UM or GameVersion.US => IsRegionalSM(s),
            _ => false,
        };
        if (HasRegionalForm)
        {
            if (sav.Version is GameVersion.SW or GameVersion.SH && s is Slowbro or Meowth or Darmanitan)
                return 2;
            return 1;
        }

        return f;
    }

    private static PKM? AddPKM(ITrainerInfo sav, ITrainerInfo tr, ushort species, byte form, bool shiny, bool alpha)
    {
        if (sav.GetRandomEncounter(species, form, shiny, alpha, out var pk) && pk is { Species: not 0 })
        {
            pk.Heal();
            return pk;
        }

        // If we didn't get an encounter, we still need to consider a Gen1->Gen2 trade.
        if (sav is not { Generation: 2 })
            return null;

        tr = new SimpleTrainerInfo(GameVersion.YW) { Language = tr.Language, OT = tr.OT, TID16 = tr.TID16 };
        var enc = tr.GetRandomEncounter(species, form, shiny, alpha, out var pkm);
        if (enc && pkm is PK1 pk1)
            return pk1.ConvertToPK2();
        return null;
    }

    /// <summary>
    /// Gets a legal <see cref="PKM"/> from a random in-game encounter's data.
    /// </summary>
    /// <param name="tr">Trainer Data to use in generating the encounter</param>
    /// <param name="species">Species ID to generate</param>
    /// <param name="form">Form to generate; if left null, picks first encounter</param>
    /// <param name="shiny"></param>
    /// <param name="alpha"></param>
    /// <param name="pk">Result legal pkm</param>
    /// <returns>True if a valid result was generated, false if the result should be ignored.</returns>
    public static bool GetRandomEncounter(this ITrainerInfo tr, ushort species, byte form, bool shiny, bool alpha, out PKM? pk)
    {
        var blank = EntityBlank.GetBlank(tr);
        pk = GetRandomEncounter(blank, tr, species, form, shiny, alpha);
        if (pk is null)
            return false;

        pk = EntityConverter.ConvertToType(pk, blank.GetType(), out _);
        return pk is not null;
    }

    /// <summary>
    /// Gets a legal <see cref="PKM"/> from a random in-game encounter's data.
    /// </summary>
    /// <param name="blank">Template data that will have its properties modified</param>
    /// <param name="tr">Trainer Data to use in generating the encounter</param>
    /// <param name="species">Species ID to generate</param>
    /// <param name="form">Form to generate; if left null, picks first encounter</param>
    /// <param name="shiny"></param>
    /// <param name="alpha"></param>
    /// <returns>Result legal pkm, null if data should be ignored.</returns>
    private static PKM? GetRandomEncounter(PKM blank, ITrainerInfo tr, ushort species, byte form, bool shiny, bool alpha)
    {
        blank.Species = species;
        blank.Gender = blank.GetSaneGender();
        if (species is ((ushort)Meowstic) or ((ushort)Indeedee))
        {
            blank.Gender = form;
            blank.Form = blank.Gender;
        }
        else
        {
            blank.Form = form;
        }

        var template = EntityBlank.GetBlank(tr);
        var item = GetFormSpecificItem(tr.Version, tr.Generation, blank.Species, blank.Form);
        if (item is not null)
            blank.HeldItem = (int)item;

        if (blank is { Species: (ushort)Keldeo, Form: 1 })
            blank.Move1 = (ushort)Move.SecretSword;

        if (blank.GetIsFormInvalid(tr.Generation,tr.Context, blank.Form))
            return null;

        var setText = new ShowdownSet(blank).Text.Split('\r')[0];
        if (species == (ushort)Zygarde && form == 2)
            setText += "-C";
        if (species == (ushort)Zygarde && form == 3)
            setText += "-50%-C";
        if ((shiny && !SimpleEdits.IsShinyLockedSpeciesForm(species, blank.Form)) || (shiny && tr.Generation != 6 && blank.Species != (ushort)Vivillon && blank.Form != 18))
            setText += Environment.NewLine + "Shiny: Yes";

        if (template is IAlphaReadOnly && alpha && tr.Version >= GameVersion.PLA)
            setText += Environment.NewLine + "Alpha: Yes";

        var sset = new ShowdownSet(setText);
        var set = new RegenTemplate(sset) { Nickname = string.Empty };
        template.ApplySetDetails(set);

        var t = template.Clone();
        var almres = tr.TryAPIConvert(set, t);
        var pk = almres.Created;
        var success = almres.Status;

        if (success == LegalizationResult.Regenerated)
            return pk;

        sset = new ShowdownSet(setText.Split('\r')[0]);
        set = new RegenTemplate(sset) { Nickname = string.Empty };
        template.ApplySetDetails(set);

        t = template.Clone();
        almres = tr.TryAPIConvert(set, t);
        pk = almres.Created;
        success = almres.Status;
        if (pk.Species is (ushort)Gholdengo)
        {
            pk.SetSuggestedFormArgument(pk.Species, pk.Form, pk.Context, new LegalityAnalysis(pk).Info.EvoChainsAllGens);
            pk.SetSuggestedMoves();
            success = LegalizationResult.Regenerated;
        }

        return success == LegalizationResult.Regenerated ? pk : null;
    }

    private static bool GetIsFormInvalid(this PKM pk, byte generation, EntityContext ctx, byte form)
    {
        var species = pk.Species;
        switch ((Species)species)
        {
            case Floette when form == 5 && ctx < EntityContext.Gen9a:
            case Shaymin or Furfrou or Hoopa when form != 0 && generation <= 6:
            case Arceus when generation == 4 && form == 9: // ??? form
            case Scatterbug or Spewpa when form == 19:
                return true;
        }
        if (FormInfo.IsBattleOnlyForm(species, form, generation))
            return true;

        if (form == 0)
            return false;

        if (species == 25 || SimpleEdits.AlolanOriginForms.Contains(species))
        {
            if (generation >= 7 && pk.Generation is < 7 and not 0)
                return true;
        }

        return false;
    }

    private static int? GetFormSpecificItem(GameVersion game, byte generation, ushort species, byte form)
    {
        if (game == GameVersion.PLA)
            return null;

        return species switch
        {
            (ushort)Arceus => generation != 4 || form < 9 ? SimpleEdits.GetArceusHeldItemFromForm(form) : SimpleEdits.GetArceusHeldItemFromForm(form - 1),
            (ushort)Silvally => SimpleEdits.GetSilvallyHeldItemFromForm(form),
            (ushort)Genesect => SimpleEdits.GetGenesectHeldItemFromForm(form),
            (ushort)Giratina => form == 1 && generation < 9 ? 112 : form == 1 ? 1779 : null, // Griseous Orb
            (ushort)Zacian => form == 1 ? 1103 : null, // Rusted Sword
            (ushort)Zamazenta => form == 1 ? 1104 : null, // Rusted Shield
            _ => null,
        };
    }

    /// <summary>
    /// Legalizes all <see cref="PKM"/> in the specified <see cref="box"/>.
    /// </summary>
    /// <param name="sav">Save File to legalize</param>
    /// <param name="box">Box to legalize</param>
    /// <returns>Count of Pokémon that are now legal.</returns>
    public static int LegalizeBox(this SaveFile sav, int box)
    {
        if ((uint)box >= sav.BoxCount)
            return -1;

        var data = sav.GetBoxData(box);
        var ctr = sav.LegalizeAll(data);
        if (ctr > 0)
            sav.SetBoxData(data, box);

        return ctr;
    }

    /// <summary>
    /// Legalizes all <see cref="PKM"/> in all boxes.
    /// </summary>
    /// <param name="sav">Save File to legalize</param>
    /// <returns>Count of Pokémon that are now legal.</returns>
    public static int LegalizeBoxes(this SaveFile sav)
    {
        if (!sav.HasBox)
            return -1;

        var ctr = 0;
        for (int i = 0; i < sav.BoxCount; i++)
        {
            var result = sav.LegalizeBox(i);
            if (result < 0)
                return result;

            ctr += result;
        }
        return ctr;
    }

    /// <summary>
    /// Legalizes all <see cref="PKM"/> in the provided <see cref="data"/>.
    /// </summary>
    /// <param name="sav">Save File context to legalize with</param>
    /// <param name="data">Data to legalize</param>
    /// <returns>Count of Pokémon that are now legal.</returns>
    public static int LegalizeAll(this SaveFile sav, IList<PKM> data)
    {
        var ctr = 0;
        for (int i = 0; i < data.Count; i++)
        {
            var pk = data[i];
            if (pk.Species == 0 || new LegalityAnalysis(pk).Valid)
                continue;

            var result = sav.Legalize(pk);
            result.Heal();
            if (!new LegalityAnalysis(result).Valid)
                continue; // failed to legalize

            data[i] = result;
            ctr++;
        }

        return ctr;
    }
    /// <summary>
    /// Generates a team of six random legal Pokémon for the given trainer and personal table.
    /// </summary>
    /// <param name="tr">Trainer information to use for generating Pokémon.</param>
    /// <returns>An array of six legal <see cref="PKM"/> objects.</returns>
    public static PKM[] GetSixRandomMons(this ITrainerInfo tr) =>
        tr.GetSixRandomMons(GameData.GetPersonal(tr.Version));

    /// <summary>
    /// Generates a team of six random legal Pokémon for the given trainer and personal table.
    /// </summary>
    /// <param name="tr">Trainer information to use for generating Pokémon.</param>
    /// <param name="personal">Personal table containing species and form data.</param>
    /// <returns>An array of six legal <see cref="PKM"/> objects.</returns>
    public static PKM[] GetSixRandomMons(this ITrainerInfo tr, IPersonalTable personal)
    {
        var result = new PKM[6];
        Span<int> ivs = stackalloc int[6];
        Span<ushort> selectedSpecies = stackalloc ushort[6];
        var rng = Util.Rand;

        int ctr = 0;
        MoveType[] types = APILegality.RandTypes;
        byte destGeneration = tr.Generation;
        var destVersion = tr.Version;

        while (ctr != 6)
        {
            var spec = (ushort)rng.Next(personal.MaxSpeciesID);

            if (selectedSpecies.Contains(spec))
                continue;

            byte form = 0;
            var rough = EntityBlank.GetBlank(tr);
            rough.Species = spec;
            rough.Gender = rough.GetSaneGender();

            if (!personal.IsSpeciesInGame(spec))
                continue;

            if (types.Length != 0)
            {
                var pi = rough.PersonalInfo;
                if (!types.Contains((MoveType)pi.Type1) || !types.Contains((MoveType)pi.Type2))
                    continue;
            }

            var formnumb = personal[spec].FormCount;
            if (formnumb == 1)
                formnumb = (byte)FormConverter.GetFormList(spec, GameInfo.Strings.types, GameInfo.Strings.forms, GameInfo.GenderSymbolUnicode, tr.Context).Length;

            do
            {
                if (formnumb == 0)
                    break;
                form = rough.Form = (byte)rng.Next(formnumb);
            }
            while (!personal.IsPresentInGame(spec, form) || FormInfo.IsLordForm(spec, form, tr.Context) || FormInfo.IsBattleOnlyForm(spec, form, destGeneration) || FormInfo.IsFusedForm(spec, form, destGeneration) || (FormInfo.IsTotemForm(spec, form) && tr.Context is not EntityContext.Gen7));

            if (spec is ((ushort)Meowstic) or ((ushort)Indeedee))
            {
                rough.Gender = rough.Form;
                form = rough.Form = rough.Gender;
            }

            var item = GetFormSpecificItem(destVersion, destGeneration, spec, form);
            if (item is not null)
                rough.HeldItem = (int)item;

            if (rough is { Species: (ushort)Keldeo, Form: 1 })
                rough.Move1 = (ushort)Move.SecretSword;

            if (GetIsFormInvalid(rough, destGeneration,tr.Context, form))
                continue;

            try
            {
                var goodset = new SmogonSetGenerator(rough);
                if (goodset is { Valid: true, Sets.Count: not 0 })
                {
                    var checknull = tr.GetLegalFromSet(goodset.Sets[rng.Next(goodset.Sets.Count)]);
                    if (checknull.Status != LegalizationResult.Regenerated)
                        continue;
                    checknull.Created.ResetPartyStats();
                    selectedSpecies[ctr] = spec;
                    result[ctr++] = checknull.Created;
                    continue;
                }
            }
            catch (Exception) { Debug.Write("Smogon Issues"); }

            var showstring = new ShowdownSet(rough).Text.Split('\r')[0];
            showstring += "\nLevel: 100\n";
            ivs.Clear();
            EffortValues.SetMax(ivs, rough);
            showstring += $"EVs: {ivs[0]} HP / {ivs[1]} Atk / {ivs[2]} Def / {ivs[3]} SpA / {ivs[4]} SpD / {ivs[5]} Spe\n";
            var m = new ushort[4];
            rough.GetMoveSet(m, true);

            var moves = GameInfo.GetStrings("en").Move;
            showstring += $"- {moves[m[0]]}\n- {moves[m[1]]}\n- {moves[m[2]]}\n- {moves[m[3]]}";
            showstring += "\n\n";
            var nullcheck = tr.GetLegalFromSet(new ShowdownSet(showstring));
            if (nullcheck.Status != LegalizationResult.Regenerated)
                continue;
            var pk = nullcheck.Created;
            pk.ResetPartyStats();
            selectedSpecies[ctr] = spec;
            result[ctr++] = pk;
        }

        return result;
    }
    /// <summary>
    /// Generates a living egg dex (one egg per species) for the given trainer and personal table.
    /// </summary>
    /// <param name="sav">Trainer information to use for generating eggs.</param>
    /// <param name="personal">Personal table containing species data.</param>
    /// <returns>An enumerable of generated <see cref="PKM"/> egg objects, one per species.</returns>
    public static IEnumerable<PKM> GenerateLivingEggDex(this ITrainerInfo sav, IPersonalTable personal)
    {
        var pklist = new ConcurrentBag<PKM>();
        var tr = APILegality.UseTrainerData ? TrainerSettings.GetSavedTrainerData(sav.Version) : sav;
        Parallel.For(1, personal.MaxSpeciesID + 1, id => //parallel For's end is exclusive
        {
            var s = (Species)id;
            if (!personal.IsSpeciesInGame((ushort)s))
                return;
            var pk = tr.GenerateEgg(new RegenTemplate(new ShowdownSet(s.ToString())), out var result);
            if (result != LegalizationResult.Regenerated)
                return;
            pklist.Add(pk);
        });
        return pklist.OrderBy(z => z.Species);
    }
}
