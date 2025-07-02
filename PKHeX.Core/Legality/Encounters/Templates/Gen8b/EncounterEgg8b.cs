using System;
using static PKHeX.Core.Species;

namespace PKHeX.Core;

public sealed record EncounterEgg8b(ushort Species, byte Form, GameVersion Version) : IEncounterEgg
{
    private const ushort Location = Locations.HatchLocation8b;

    public string Name => "Egg";
    public string LongName => Name;

    public const byte Level = 1;
    public bool CanHaveVoltTackle => Species is (int)Pichu;

    public byte Generation => 8;
    public EntityContext Context => EntityContext.Gen8b;
    public bool IsShiny => false;
    public byte LevelMin => Level;
    public byte LevelMax => Level;
    ushort ILocation.EggLocation => Locations.Daycare8b;
    ushort ILocation.Location => Location;
    public AbilityPermission Ability => AbilityBreedLegality.IsHiddenPossibleHOME(Species) ? AbilityPermission.Any12H : AbilityPermission.Any12;
    public Ball FixedBall => Ball.None; // Inheritance allowed.
    public Shiny Shiny => Shiny.Random;
    public bool IsEgg => true;

    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria) => ConvertToPKM(tr, criteria);
    PKM IEncounterConvertible.ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr);
    public PB8 ConvertToPKM(ITrainerInfo tr) => ConvertToPKM(tr, EncounterCriteria.Unrestricted);

    public PB8 ConvertToPKM(ITrainerInfo tr, EncounterCriteria criteria)
    {
        int language = (int)Language.GetSafeLanguage(Generation, (LanguageID)tr.Language, Version);
        var date = EncounterDate.GetDateSwitch();
        var pi = PersonalTable.BDSP[Species, Form];
        var rnd = Util.Rand;

        var pk = new PB8
        {
            Species = Species,
            Form = Form,
            CurrentLevel = Level,
            Version = Version,
            Ball = (byte)Ball.Poke,
            TID16 = tr.TID16,
            SID16 = tr.SID16,
            OriginalTrainerGender = tr.Gender,

            // Force Hatch
            Language = language,
            OriginalTrainerName = tr.OT,
            Nickname = SpeciesName.GetSpeciesNameGeneration(Species, language, Generation),
            OriginalTrainerFriendship = 100,
            MetLevel = 1,
            MetLocation = Location,
            EggLocation = tr.Version == Version ? Locations.Daycare8b : Locations.LinkTrade6NPC,

            MetDate = date,
            EggMetDate = date,

            // Disassociated from the Egg RNG; PID only is overwritten if re-rolled.
            PID = EncounterUtil.GetRandomPID(tr, rnd, criteria.Shiny),
            HeightScalar = PokeSizeUtil.GetRandomScalar(rnd),
            WeightScalar = PokeSizeUtil.GetRandomScalar(rnd)
        };

        SetPINGA(pk, criteria, pi, rnd.Rand32());
        SetEncounterMoves(pk);

        return pk;
    }

    ILearnSource IEncounterEgg.Learn => Learn;
    public ILearnSource<PersonalInfo8BDSP> Learn => LearnSource8BDSP.Instance;

    private void SetEncounterMoves(PB8 pk)
    {
        var learn = Learn.GetLearnset(Species, Form);
        var initial = learn.GetBaseEggMoves(Level);
        pk.SetMoves(initial);
        pk.SetRelearnMoves(initial);
    }

    private const int Uninitialized = -1; // -1 indicates uninitialized IV.

    /// <summary>
    /// Applies the random values to match the criteria.
    /// </summary>
    /// <remarks>
    /// The egg is first templated by the game, unrelated to the egg seed. This determines the PID and Height/Weight.
    /// The rest of the values are then assigned, "randomly" via a sign-extended seed from the save file.
    /// We assume minimal parent held items in order to generate something legally. One parent holds an Everstone to get the desired nature, and the other holds a Destiny Knot to give us the best IV inheritance options.
    /// For inheritance, we assume the parents both have the desired IVs, so statistically we only fail 1:32 if that un-inherited IV is specified via criteria.
    /// If the generated egg is not the desired shiny state, we overwrite values to imply it was traded from a(n) (un)lucky trainer, rather than wasting time retrying with Masuda Method/Shiny Charm rolls.
    /// Purists who want to generate shiny eggs that aren't traded can revise the logic themselves; the intent is to make something legal as quick as possible, not retry the entire loop ~400x.
    /// </remarks>
    private void SetPINGA(PB8 pk, in EncounterCriteria criteria, PersonalInfo8BDSP pi, uint seed)
    {
        Span<int> ivs = stackalloc int[6];
        criteria.SetRandomIVs(pk);
        ReadOnlySpan<int> requiredIVs = [pk.IV_HP, pk.IV_ATK, pk.IV_DEF, pk.IV_SPA, pk.IV_SPD, pk.IV_SPE];

        var ratio = pi.Gender;
        var checkGender = ratio is not (PersonalInfo.RatioMagicMale or PersonalInfo.RatioMagicFemale or PersonalInfo.RatioMagicGenderless);
        var isSpeciesGender = !checkGender && (Species)Species is NidoranF or NidoranM or Illumise or Volbeat;
        var gender = checkGender ? (byte)0 : criteria.GetGender(pi); // input is ignored for random genders; this only is for fetching a value for fixed genders

        SearchAndApply(pk, criteria, requiredIVs, ivs, isSpeciesGender, checkGender, ratio, gender, seed);
    }

    /// <summary>
    /// Loops until it finds a suitable random egg that matches the criteria.
    /// </summary>
    /// <param name="pk">Entity to apply the random values to.</param>
    /// <param name="criteria">Encounter criteria to satisfy.</param>
    /// <param name="requestIVs">Requested IVs for the encounter (not random/unspecified).</param>
    /// <param name="ivs">IVs to fill in with the generated values.</param>
    /// <param name="isSpeciesGender">Is Nidoran or Volbeat/Illumise.</param>
    /// <param name="isRandomGender">Is not a single gender, and must be randomly decided.</param>
    /// <param name="ratio">Gender ratio of the species-form to generate (0-255).</param>
    /// <param name="gender">Gender, if not random.</param>
    /// <param name="seed">Random seed to use for the generation.</param>
    private void SearchAndApply(PB8 pk, in EncounterCriteria criteria, ReadOnlySpan<int> requestIVs, Span<int> ivs,
        bool isSpeciesGender, bool isRandomGender, byte ratio, byte gender, uint seed)
    {
        while (true)
        {
            seed = LCRNG.Next(seed); // arbitrary random state (for each iteration)
            var rng = new Xoroshiro128Plus8b((ulong)(int)seed);

            // Generate Species, otherwise Gender if Species is random gender.
            if (isSpeciesGender)
            {
                if (rng.NextUInt(2) != gender)
                    continue;
            }
            else if (isRandomGender)
            {
                var rand = rng.NextUInt(252) + 1;
                var randGender = rand < ratio ? (byte)1 : (byte)0;
                if (criteria.IsSpecifiedGender() && !criteria.IsSatisfiedGender(randGender))
                    continue;
                gender = randGender;
            }

            // nature
            _ = rng.NextUInt(25); // Assume one parent always carries an Everstone, so the value is overwritten by that parent.

            // ability
            var abilityRoll = rng.NextUInt(100); // Ability can be changed using Capsule/Patch (Assume parent is ability 0/1).
            if (!CheckAbilityRoll(pk, criteria, abilityRoll, out var abilityIndex))
                continue; // Generated ability cannot satisfy.

            // The game does a rand(6) to decide which IV's inheritance to check.
            // If that IV isn't marked to inherit from a parent, it does a rand(2) to pick the parent.
            // When generating egg IVs, it first randomly fills in the egg IVs with rand(32) x6, then overwrites with parent IVs based on tracking.
            // We'll assume both parents have the perfect IVs and copy over the parent IV as it's inherited, then fill in blanks afterward.

            ivs.Fill(Uninitialized); // reset
            LoadInheritedIVs(ref rng, ivs, requestIVs);

            // Roll all 6 IVs. Parent inheritance will override.
            GetFinalIVs(ref rng, ivs);

            if (!criteria.IsIVsCompatibleSpeedLast(ivs))
                continue;

            // Result found. Finish up and set the values.
            var speed = ivs[5]; // speed IV before special when set to pkm
            ivs[5] = ivs[4];
            ivs[4] = ivs[3];
            ivs[3] = speed;

            // When generating, the game first generates a template, unrelated from the egg seed.
            // This unrelated PID can be retained if the breeding does not use Masuda Method or Shiny Charm re-rolls.
            // Height and Weight are also unrelated, via the template.
            // For eggs, we'll "randomly" get the right PID via template roll before ever generating the rest of the egg.

            // Set the rest of the values as per our generating via the egg seed.
            pk.EncryptionConstant = rng.NextUInt(); // PID would be re-rolled after here, but we aren't going to have re-rolls in our hypothetical setup.
            pk.SetIVs(ivs);
            pk.StatNature = pk.Nature = criteria.GetNature(); // Everstone (see above)
            pk.Gender = gender;
            pk.RefreshAbility(abilityIndex);
            return;
        }
    }

    private bool CheckAbilityRoll(PB8 pk, EncounterCriteria criteria, uint abilityRoll, out int abilityIndex)
    {
        if (!pk.IsEgg || !criteria.IsSpecifiedAbility())
        {
            abilityIndex = criteria.GetAbilityFromNumber(Ability);
            return true;
        }

        // If still an egg, we need to ensure the generated ability matches (can't be changed yet).
        abilityIndex = -1;
        for (int i = 0; i < 3; i++) // Try all 3 parent ability indexes to inherit, if acceptable to the criteria.
        {
            int result = GetInheritedAbilityIndex(i, abilityRoll);
            if (!criteria.IsSatisfiedAbility(result))
                continue;
            if (result is 2 && Ability is AbilityPermission.Any12) // not hidden
                continue; // only for Phione.
            abilityIndex = result;
            break;
        }
        return abilityIndex != -1;
    }

    /// <summary>
    /// Parent ability inheritance (1/2/H)
    /// </summary>
    private static int GetInheritedAbilityIndex(int parentIndex, uint rand) => parentIndex switch
    {
        0 => rand < 80 ? 0 : 1, // 80% ability 0, else 1
        1 => rand < 20 ? 0 : 1, // 20% ability 0, else 2
        _ => rand switch
        {
            < 20 => 0, // 20% ability 0
            < 40 => 1, // 20% ability 1
               _ => 2, // 60% ability 2
        },
    };

    private static void LoadInheritedIVs(ref Xoroshiro128Plus8b rng, Span<int> ivs, ReadOnlySpan<int> requiredIVs)
    {
        const int inheritCount = 5; // assume other parent always has destiny knot, so that we fail this part less.
        var inherited = 0;
        while (inherited < inheritCount)
        {
            var stat = (int)rng.NextUInt(6); // Decides which IV to check.
            if (ivs[stat] != Uninitialized)
                continue;

            // Decide which parent's IV to inherit.
            // Assume both parents have the same desired IV.
            _ = rng.NextUInt(2);
            ivs[stat] = requiredIVs[stat];
            inherited++;
        }
    }

    private static void GetFinalIVs(ref Xoroshiro128Plus8b rng, Span<int> result)
    {
        // Generate base IVs, then override with inheritance.
        const int maxIV = 31; // max IV value.
        // IVs in result were already populated with parent inheritance values, actual IVs. Merge the two, prioritizing inherited.
        for (int i = 0; i < result.Length; i++)
        {
            var baseIV = rng.NextUInt(maxIV + 1);
            if (result[i] is Uninitialized)
                result[i] = (int)baseIV;
        }
    }
}
