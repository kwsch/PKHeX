# Pokéathlon Save Data Support for HGSS

> **Related issue:** [#4755](https://github.com/kwsch/PKHeX/issues/4755)
>
> **Research sources:**
> - [Project Pokémon Forums — HGSS Pokéathlon save research](https://projectpokemon.org/home/forums/topic/62989-hgss-pokeathlon-save-research-records-medals-counters-data-cards/)
> - [RetroAchievements Code Notes — Pokémon HeartGold / SoulSilver](https://retroachievements.org/codenotes.php?g=576)

---

## Overview

PKHeX currently exposes only `PokeathlonPoints` (Athlete Points, offset `0xE548`) for HGSS saves. This plan covers the full Pokéathlon save block, adding read/write support for course records, event records, participation counters, medals, Data Cards, global statistics, and the Data Card screen counters — everything shown in-game on the Pokéathlon Data Card and trophy room.

The implementation is split across two projects following existing conventions:

- **PKHeX.Core** — data model (`PokeatlonBlock`) exposed from `SAV4HGSS`
- **PKHeX.WinForms** — editor form (`SAV_Pokeathlon`) wired into the existing `SAVEditor`

---

## Save File Layout

All offsets are relative to the **General buffer** (the 0xF628-byte primary block). The game engine maintains a mirror copy at `offset + 0x40000`; PKHeX's `SAV4` base class already handles checksum recalculation and mirroring on export, so the implementation only needs to target the primary (General) buffer.

> **Note:** All offsets below were verified against HeartGold USA. SoulSilver USA shares the same General block layout; regional variants (JPN, KOR, EUR) have **not** been independently verified and may require offset deltas. A `TODO` comment should be added pending verification.

### Course Records (16-bit LE, one per course)

| Course  | Offset |
|---------|--------|
| Speed   | 0xD9DA |
| Power   | 0xDA06 |
| Skill   | 0xDA32 |
| Stamina | 0xDA5E |
| Jump    | 0xDA8A |

These are raw score values as stored by the game (higher = better). The in-game display maps these to 1–5 star ratings; PKHeX should expose the raw values and leave display interpretation to the UI.

### Per-Species Medal Bitfield

- **Base offset:** `0xDAB0`
- **Size:** one byte per species, indexed by `speciesIndex - 1` (1-based, covering all 493 Gen-4 species → 493 bytes)
- **Bit layout per byte:**

  | Bit | Medal    |
  |-----|----------|
  | 0   | Speed    |
  | 1   | Power    |
  | 2   | Skill    |
  | 3   | Stamina  |
  | 4   | Jump     |
  | 5–7 | Unused   |

### Event Records (16-bit LE) and Participation Counters (16-bit LE)

Ten Pokéathlon events. Each event has two associated fields: a best-score record and a participation count. The participation counter sits `0x28` bytes before its corresponding record.

| Event            | Record Offset | Participation Offset |
|------------------|--------------|----------------------|
| Hurdle Dash      | 0xDCA0       | 0xDCC8               |
| Pennant Capture  | 0xDCCC       | 0xDCF4               |
| Circle Push      | 0xDCF8       | 0xDD20               |
| Block Smash      | 0xDD24       | 0xDD4C               |
| Disc Catch       | 0xDD50       | 0xDD78               |
| Lamp Jump        | 0xDD7C       | 0xDDA4               |
| Relay Run        | 0xDDA8       | 0xDDD0               |
| Ring Drop        | 0xDDD4       | 0xDDFC               |
| Snow Throw       | 0xDE00       | 0xDE28               |
| Goal Roll        | 0xDE2C       | 0xDE54               |

> **Special cases:**
> - **Hurdle Dash** record is stored in 30 fps frame counts. The UI should display and accept frame counts directly; a helper may optionally convert to/from seconds for display.
> - **Relay Run** record uses a non-trivial encoding that has not been fully reverse-engineered. The field should be exposed as a raw `ushort` with a UI tooltip noting this limitation.

### Event-Side Friendship Subtotal Table (16-bit LE × 10)

- **Offset:** `0xE4C0` through `0xE4D2` (10 × 2 bytes)
- Purpose: contributes to the Pokéathlon "friendship" total shown in-game. Exact semantics are partially understood — the total friendship is the sum of course-record contributions, event-side values, and medal bitcounts. These should be exposed as read-write `ushort` values; a future validation step could cross-check consistency.

### Global Pokéathlon Statistics (16-bit LE)

These populate the in-game Data Card screen.

| Field                 | Offset |
|-----------------------|--------|
| Time spent (minutes)  | 0xE4D4 |
| Total joins           | 0xE4D8 |
| Overall 1st places    | 0xE4DC |
| Overall last places   | 0xE4E0 |
| Bonuses earned        | 0xE4E4 |
| Instructions given    | 0xE4E8 |
| Pokémon that failed   | 0xE4EC |
| Times jumped          | 0xE4F0 |
| Pokémon acquired pts  | 0xE4F4 |
| Times tackled         | 0xE4F8 |
| Pokémon fell down     | 0xE4FC |
| Times dashed          | 0xE500 |
| Times switched        | 0xE504 |
| Times self-impeded    | 0xE508 |

### Per-Event 1st-Place Counters (32-bit LE)

| Event            | Offset |
|------------------|--------|
| Hurdle Dash      | 0xE518 |
| Pennant Capture  | 0xE51C |
| Circle Push      | 0xE520 |
| Block Smash      | 0xE524 |
| Disc Catch       | 0xE528 |
| Lamp Jump        | 0xE52C |
| Relay Run        | 0xE530 |
| Ring Drop        | 0xE534 |
| Snow Throw       | 0xE538 |
| Goal Roll        | 0xE53C |

### Aggregate Last-Place Total (32-bit LE)

- **Offset:** `0xE540`

### Athlete Points (32-bit LE) — already implemented

- **Offset:** `0xE548` — currently exposed as `SAV4HGSS.PokeathlonPoints`

### Data Card Ownership Bitflags

27 Data Cards, packed into 4 consecutive bytes starting at `0xE54C`. Cards are numbered 1–27, assigned LSB-first across bytes:

| Byte offset | Cards covered |
|-------------|---------------|
| 0xE54C      | Cards 1–8     |
| 0xE54D      | Cards 9–16    |
| 0xE54E      | Cards 17–24   |
| 0xE54F      | Cards 25–27 (bits 0–2 only) |

### Apricorn Pouch

- **Offset:** `0xE558` — 7 bytes, one per apricorn type
- Already exposed via `GetApricornCount`/`SetApricornCount` on `SAV4HGSS`; no action needed.

---

## Implementation Plan

### Phase 1 — Core Data Model

**File:** `PKHeX.Core/Saves/Substructures/Gen4/PokeatlonBlock.cs` (new file)

Create a `PokeatlonBlock` class that wraps a `Span<byte>` / `Memory<byte>` slice of the General buffer:

```
public sealed class PokeatlonBlock
{
    private readonly Memory<byte> Raw;
    private Span<byte> Data => Raw.Span;

    // Number of events
    public const int EventCount = 10;
    // Number of courses
    public const int CourseCount = 5;
    // Base offset for medals (relative to General buffer start)
    private const int MedalBase = 0xDAB0;
    private const int OffsetCourseBase = 0xD9DA;
    private const int CourseStride = 0x2C;  // spacing between course record offsets
    // ... etc.
}
```

Key members:

- `GetCourseRecord(int course)` / `SetCourseRecord(int course, ushort value)` — 5 courses
- `GetEventRecord(int ev)` / `SetEventRecord(int ev, ushort value)` — 10 events
- `GetEventParticipation(int ev)` / `SetEventParticipation(int ev, ushort value)` — 10 events
- `GetEventWins(int ev)` / `SetEventWins(int ev, uint value)` — 10 events (32-bit)
- `GetMedalFlags(int speciesIndex)` / `SetMedalFlags(int speciesIndex, byte flags)` — per species (1-based, 493 species)
- All 14 global stat properties (16-bit)
- `TotalLastPlaces` (32-bit)
- `AthletePoints` (32-bit) — existing field migrated here; keep `SAV4HGSS.PokeathlonPoints` as a forwarding shim for backward compat
- `GetDataCard(int card)` / `SetDataCard(int card, bool owned)` — cards 1–27
- `GetEventFriendshipSubtotal(int ev)` / `SetEventFriendshipSubtotal(int ev, ushort value)` — 10 events

**File:** `PKHeX.Core/Saves/SAV4HGSS.cs` (modify)

Add:

```csharp
private const int OffsetPokeatlonBlock = 0xD980; // start of Pokéathlon region
private const int SizePokeatlonBlock = 0x5F0;

public PokeatlonBlock Pokeathlon => new(GeneralBuffer.Slice(OffsetPokeatlonBlock, SizePokeatlonBlock));
```

Keep `PokeathlonPoints` as a forwarding property calling `Pokeathlon.AthletePoints` to avoid breaking existing callers.

> The block is intentionally instantiated on each access (same pattern used by `Roamer4`) rather than cached, to avoid lifetime issues with `Memory<byte>` slices.

---

### Phase 2 — WinForms Editor

**File:** `PKHeX.WinForms/Subforms/Save Editors/Gen4/SAV_Pokeathlon.cs` (new file + Designer)

A tabbed `Form` following the same pattern as `SAV_Apricorn` and `SAV_Misc4`:

- Constructor clones `SAV4HGSS`, calls `LoadData()`
- "Save" button writes UI → cloned SAV, then `Origin.CopyChangesFrom(SAV)`
- "Cancel" button closes without saving

**Suggested tab layout:**

#### Tab: Courses
5 rows, one per course. `NumericUpDown` per row, labelled Speed / Power / Skill / Stamina / Jump. Range `0–65535`.

#### Tab: Events
`DataGridView` with columns: Event Name | Best Score | Times Entered. 10 rows. Relay Run row shows a tooltip/warning about the encoding. Hurdle Dash row shows "(frames)" hint in header or cell.

#### Tab: Event Wins
`DataGridView` with columns: Event Name | 1st Places. 10 rows. Per-event 32-bit counters.

#### Tab: Medals
`DataGridView` with columns: Species | Speed | Power | Skill | Stamina | Jump. One row per species (up to 493). Checkbox columns for each medal. "Give All" / "Clear All" buttons. Rows filtered to non-empty species names.

#### Tab: Data Cards
27 checkboxes laid out in a 3-column grid, labelled "Data Card 1" through "Data Card 27". "Check All" / "Uncheck All" buttons.

#### Tab: Statistics
`NumericUpDown` controls for all 14 global stat fields plus the aggregate last-place total. Laid out in two columns with descriptive labels matching in-game Data Card screen text.

#### Tab: Friendship Subtotals
10 `NumericUpDown` controls for the event-side friendship table. Labelled by event name. Display-only note explaining these are sub-components of the overall Pokéathlon friendship score.

**Wire-up in SAVEditor:**

`PKHeX.WinForms/Controls/SAV Editor/SAVEditor.cs` already has a button/click handler that opens `SAV_Apricorn`. Add an analogous handler:

```csharp
private void B_OpenPokeathlon_Click(object sender, EventArgs e)
    => OpenDialog(new SAV_Pokeathlon((SAV4HGSS)SAV));
```

And bind it to a new "Pokéathlon" button on the HGSS-specific control panel (same area as the Apricorn button), conditionally visible only when `SAV is SAV4HGSS`.

**Localization:**

Add entries to all `lang_*.txt` files under `PKHeX.WinForms/Resources/text/`:

```
SAV_Pokeathlon=Pokéathlon Editor
```

---

### Phase 3 — Validation / Consistency Helpers (stretch goal)

These are optional quality-of-life additions for a follow-up, listed here for completeness:

- **Friendship recalculation:** A "Recalculate" button on the Friendship Subtotals tab that recomputes the expected contribution from medals and course records and warns if totals appear inconsistent.
- **Hurdle Dash display:** Option to show the Hurdle Dash record as `MM:SS.ff` (from 30 fps frames) alongside the raw value.
- **Max/Zero helpers:** Per-tab "Maximize" and "Reset to Zero" buttons for convenience.

---

## Open Questions / Research Gaps

1. **SoulSilver / regional ROM offsets** — All offsets confirmed for HeartGold USA only. SoulSilver USA is believed to share the same layout, but this should be verified with a known-good SoulSilver save. Japanese and Korean versions may differ; if so, version-gated offsets (already used elsewhere in `SAV4HGSS`) should be added.

2. **Relay Run record encoding** — The 16-bit value at `0xDDA8` has a non-obvious encoding. Until it is fully understood, the field should be exposed as raw `ushort` with an in-UI advisory. If the encoding is later understood (e.g., BCD, fixed-point, frame counter), the property can be updated without breaking the data model.

3. **Event-side friendship table semantics** — The 10-entry table at `0xE4C0` contributes to the friendship total but the exact per-entry meaning is not fully understood. Exposing raw values and documenting the uncertainty is preferable to guessing.

4. **Pokémon acquired points** (`0xE4F4`) — The field exists and is visible in the Data Card screen but its exact game mechanic (what increments it) is not documented. Expose as a standard stat field.

5. **Trophy room state** — The in-game trophy room appears to derive trophy display from the underlying counters (joins, wins, etc.) rather than a persistent flag. No special handling is needed; exposing the source counters is sufficient.

6. **`PokeathlonPoints` max value** — The current `SAV_Misc4.Designer.cs` sets maximum to `9999999` for the `NUD_PokeathlonPoints` control. This should be reviewed; the field is 32-bit but the in-game maximum is likely lower (the game caps at 9,999 based on the display). The existing cap can be preserved or corrected as appropriate.

---

## Files to Create

| Path | Action |
|------|--------|
| `PKHeX.Core/Saves/Substructures/Gen4/PokeatlonBlock.cs` | **New** |
| `PKHeX.WinForms/Subforms/Save Editors/Gen4/SAV_Pokeathlon.cs` | **New** |
| `PKHeX.WinForms/Subforms/Save Editors/Gen4/SAV_Pokeathlon.Designer.cs` | **New** |

## Files to Modify

| Path | Change |
|------|--------|
| `PKHeX.Core/Saves/SAV4HGSS.cs` | Add `Pokeathlon` property; keep `PokeathlonPoints` as shim |
| `PKHeX.WinForms/Controls/SAV Editor/SAVEditor.cs` | Add click handler and HGSS-conditional button |
| `PKHeX.WinForms/Resources/text/lang_en.txt` (and all other lang files) | Add `SAV_Pokeathlon=` entry |
