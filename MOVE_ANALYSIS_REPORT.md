# Pokémon Move Availability Analysis Report
## Based on PKHeX Data (Gen 8, BDSP, and Gen 9)

---

## Summary of Findings

- **Gen 8 (Sword/Shield):** 161 moves are unavailable
- **BDSP (Brilliant Diamond/Shining Pearl):** 314 moves are unavailable
- **Gen 9 (Scarlet/Violet):** 222 moves are unavailable

---

## Question 1: Do moves missing in Gen 8 also not exist in BDSP?

**Answer:** Not necessarily. The situation is complex:

- **149 moves** are missing in BOTH Gen 8 and BDSP
- **12 moves** are available in Gen 8 but NOT in BDSP (mostly Crown Tundra DLC moves)
- **165 moves** are available in BDSP but NOT in Gen 8

BDSP actually has significantly MORE moves unavailable than Gen 8 (314 vs 161), because BDSP is based on the original Diamond/Pearl and lacks many modern moves despite being released later.

---

## Question 2: Which specific moves returned in Gen 9?

**14 moves** that were missing in Gen 8 (Sword/Shield) returned in Gen 9 (Scarlet/Violet):

1. **Sketch** (166) - Normal-type
2. **Tail Glow** (294) - Bug-type (now called Tail Glow)
3. **Psycho Boost** (354) - Psychic-type
4. **Heart Swap** (391) - Psychic-type
5. **Judgment** (449) - Normal-type (Arceus signature move)
6. **Dark Void** (464) - Dark-type
7. **Seed Flare** (465) - Grass-type
8. **Relic Song** (547) - Normal-type
9. **Hyperspace Hole** (593) - Psychic-type
10. **Hyperspace Fury** (621) - Dark-type
11. **Ice Hammer** (665) - Ice-type
12. **Toxic Thread** (672) - Poison-type
13. **Revelation Dance** (686) - Normal-type
14. **Beak Blast** (690) - Flying-type

---

## Question 3: Did any moves return in future Sword/Shield updates?

**Answer:** The PKHeX data represents the final state of Sword/Shield after all DLC (Isle of Armor and Crown Tundra). No additional moves were added beyond what's included in the base game + DLC. The 161 missing moves remain unavailable in the final version of Sword/Shield.

---

## Question 4: Complete list of Fairy-type moves in BDSP

BDSP has **13 Fairy-type moves** available:

1. **Sweet Kiss** (186)
2. **Charm** (204)
3. **Moonlight** (236)
4. **Disarming Voice** (574)
5. **Draining Kiss** (577)
6. **Flower Shield** (579)
7. **Misty Terrain** (581)
8. **Play Rough** (583)
9. **Fairy Wind** (584)
10. **Moonblast** (585)
11. **Aromatic Mist** (597)
12. **Dazzling Gleam** (605)
13. **Baby-Doll Eyes** (608)

**Missing Fairy moves in BDSP:** Crafty Shield, Fairy Lock, Geomancy, and Light of Ruin

---

## Question 5: Moves in BDSP that weren't in original Diamond/Pearl

Original Diamond/Pearl (Gen 4) had moves 1-467. BDSP added **109 post-Gen 4 moves** (from Generations 5-8).

### Notable examples include:

**Gen 5 moves:**
- Stored Power (508)
- Acrobatics (512)
- Scald (503)
- Quiver Dance (483)
- Dragon Tail (525)
- Volt Switch (521)

**Gen 6 moves (including Fairy moves):**
- **Freeze-Dry (573)** ✓ - The move you mentioned!
- Disarming Voice (574)
- Play Rough (583)
- Moonblast (585)
- Dazzling Gleam (605)
- Sticky Web (564)
- Phantom Force (566)

**Gen 7 moves:**
- Shore Up (659) - NOT in BDSP
- First Impression (660) - NOT in BDSP
- Throat Chop (675)
- High Horsepower (667)
- Revelation Dance (686)

**Gen 8 moves:**
- Most Gen 8-specific moves (like Dynamax moves 751-782) are NOT in BDSP

---

## Freeze-Dry Specifics (Your Example)

**Freeze-Dry (Move ID 573)**
- Type: **Ice**
- Generation introduced: Gen 6 (X/Y)
- **Gen 8 (SWSH):** ✓ Available
- **BDSP:** ✓ Available
- **Gen 9 (SV):** ✓ Available

Freeze-Dry is indeed one of the Gen 6 moves that was added to BDSP despite not being in the original Diamond/Pearl!

---

## Additional Insights

### Moves available in Gen 8 but NOT in BDSP include:
- Most Gen 8 signature moves (Behemoth Blade, Bolt Beak, Fishious Rend, etc.)
- Crown Tundra legendaries' signature moves
- Dynamax/G-Max moves (751-782)
- Several Gen 7 moves

### Move availability pattern:
- **Gen 8 (SWSH)**: Modern, includes most moves through Gen 8
- **BDSP**: Faithful remake - includes Gen 1-4 moves + select Gen 5-6 moves
- **Gen 9 (SV)**: Most comprehensive, brought back several missing moves

---

## Sources

All data extracted from PKHeX Core library:
- `/PKHeX.Core/Moves/MoveInfo8.cs` (Gen 8 dummied moves)
- `/PKHeX.Core/Moves/MoveInfo8b.cs` (BDSP dummied moves)
- `/PKHeX.Core/Moves/MoveInfo9.cs` (Gen 9 dummied moves and types)
- `/PKHeX.Core/Game/Enums/Move.cs` (Move names)
