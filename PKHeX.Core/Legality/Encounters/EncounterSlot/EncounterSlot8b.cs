using System;

namespace PKHeX.Core;

/// <summary>
/// Encounter Slot found in <see cref="GameVersion.BDSP"/>.
/// </summary>
/// <inheritdoc cref="EncounterSlot"/>
public sealed record EncounterSlot8b : EncounterSlot
{
    public override int Generation => 8;
    public override EntityContext Context => EntityContext.Gen8b;
    public bool IsUnderground => Area.Location is (>= 508 and <= 617);
    public bool IsMarsh => Area.Location is (>= 219 and <= 224);
    public override Ball FixedBall => IsMarsh ? Ball.Safari : Ball.None;

    public EncounterSlot8b(EncounterArea area, ushort species, byte form, byte min, byte max) : base(area, species, form, min, max)
    {
    }

    public override EncounterMatchRating GetMatchRating(PKM pk)
    {
        bool isHidden = pk.AbilityNumber == 4;
        if (isHidden && this.IsPartialMatchHidden(pk.Species, Species))
            return EncounterMatchRating.PartialMatch;
        return base.GetMatchRating(pk);
    }

    protected override void SetFormatSpecificData(PKM pk)
    {
        if (IsUnderground)
        {
            if (GetBaseEggMove(out var move1))
                pk.RelearnMove1 = move1;
        }
        pk.SetRandomEC();
    }

    public bool CanBeUndergroundMove(ushort move)
    {
        var et = PersonalTable.BDSP;
        var sf = et.GetFormEntry(Species, Form);
        var species = sf.HatchSpecies;
        var baseEgg = Legal.EggMovesBDSP[species].Moves;
        if (baseEgg.Length == 0)
            return move == 0;
        return Array.IndexOf(baseEgg, move) >= 0;
    }

    public bool GetBaseEggMove(out ushort move)
    {
        var et = PersonalTable.BDSP;
        var sf = et.GetFormEntry(Species, Form);
        var species = sf.HatchSpecies;
        var baseEgg = Legal.EggMovesBDSP[species].Moves;
        if (baseEgg.Length == 0)
        {
            move = 0;
            return false;
        }

        // Official method creates a new List<ushort>() with all the egg moves, removes all ignored, then picks a random index.
        // However, the "excluded egg moves" list was unreferenced in v1.0, so all egg moves are allowed.
        // We can't know which patch the encounter originated from, because they never added any new content.
        var rnd = Util.Rand;
        {
            var index = rnd.Next(baseEgg.Length);
            move = baseEgg[index];
            return true;
        }
    }

    public bool CanUseRadar => Area.Type is SlotType.Grass && !IsUnderground && !IsMarsh && Location switch
    {
        195 or 196 => false, // Oreburgh Mine
        203 or 204 or 205 or 208 or 209 or 210 or 211 or 212 or 213 or 214 or 215 => false, // Mount Coronet, 206/207 exterior
        >= 225 and <= 243 => false, // Solaceon Ruins
        244 or 245 or 246 or 247 or 248 or 249 => false, // Victory Road
        252 => false, // Ravaged Path
        255 or 256 => false, // Oreburgh Gate
        260 or 261 or 262 => false, // Stark Mountain, 259 exterior
        >= 264 and <= 284 => false, // Turnback Cave
        286 or 287 or 288 or 289 or 290 or 291 => false, // Snowpoint Temple
        292 or 293 => false, // Wayward Cave
        294 or 295 => false, // Ruin Maniac Cave
        296 => false, // Maniac Tunnel
        299 or 300 or 301 or 302 or 303 or 304 or 305 => false, // Iron Island, 298 exterior
        306 or 307 or 308 or 309 or 310 or 311 or 312 or 313 or 314 => false, // Old Chateau
        368 or 369 or 370 or 371 or 372 => false, // Route 209 (Lost Tower)
        _ => true,
    };

    protected override HiddenAbilityPermission IsHiddenAbilitySlot() => CanUseRadar ? HiddenAbilityPermission.Possible : HiddenAbilityPermission.Never;
}
