# PKHeX GBA: a Generation 3 save editor on the GBA itself

A small multiboot program that edits the save of the Pokémon cartridge in the
GBA's slot, on the GBA, with no computer. It reads the save, shows the trainer,
the party and the 14 boxes, makes basic edits, and writes the save back the way
the game does.

It lives beside PKHeX in this repository, and follows PKHeX's reading of the
format (`PKHeX.Core/Saves/SAV3.cs`). It shares no code with PKHeX: PKHeX is C#
on .NET, and this program is C for a 16 MHz ARM7 with 256 KB of RAM.

## Using it

1. Build `build/editor.mb` (below), and put it on gbagate's microSD card with
   the other programs it sends.
2. Put the game cartridge in the GBA. Hold **START + SELECT** while turning it
   on, so the BIOS waits for multiboot with the cartridge in. Link it to the
   board, and send **editor.mb** (Send Program). *Not yet tried on a real GBA:*
   every test so far is in emulation.
3. The editor reads the cartridge for about three seconds: the save, and the
   names and base stats it shows, which come from the cartridge itself.
4. Menu:
   - **Party**, **Boxes** (L/R change box): A opens a Pokémon.
   - **Write to cartridge** writes every edit, after you confirm.
   - **Undo all edits** reads the save again.

On a Pokémon:

| row | edit |
|---|---|
| Name | A opens the name editor: Up/Down letter, Left/Right place, SELECT ends the name there, A done, B cancel |
| Level | Left/Right ±1, L/R ±10. The level is set as the least experience for it. |
| Item | ±1 / ±10, by item number, with the cartridge's name |
| Move 1–4 | ±1 / ±10, by move number. The PP is set to the move's base PP, and its PP Ups are cleared. Move 0 empties the slot. |
| IV, EV | A picks the stat (HP, Atk, Def, Spe, SpA, SpD), and Left/Right/L/R change it. IVs are 0–31; EVs are 0–255, at most 510 in all. |

A party Pokémon's level and six stats are worked out again after every edit,
and its HP follows the new maximum as the game does it (`CalculateMonStats`). A
box Pokémon keeps experience only, as in the game.

### Writing

The game keeps two copies of its save, and loads whichever has the higher save
count. The editor writes the copy the game did not load last, with the count one
higher. It then reads back every sector and checks the bytes, the checksums and
the footers. The copy it started from is not touched, and neither are the Hall
of Fame and the other extra sectors (28–31).

If the GBA loses power while writing, the game finds that copy broken and loads
the one before: the edits are lost, and the save is not.

## What it supports

- **Games:** Ruby, Sapphire, FireRed, LeafGreen and Emerald, found by the
  cartridge's game code. Only English names are shown. Other languages edit the
  same way, but names show as numbers (`#277`).
- **Save chips:** the 128 KB flash parts these games carry (Sanyo, Macronix).
  The flash code is gbagate's Save Tool's, which mGBA's Sanyo model tests.
- **Tested:** Emerald, end to end (below).
- **Untested:** the others, which have their own section sizes and party
  offset, built from PKHeX and pret's decompilations.

Not yet:
- a `.sav` on the board's microSD card, without a cartridge (the board would
  serve its sectors over the link);
- species, PID, shininess, OT, ribbons, contest stats, Pokérus;
- moving Pokémon between slots;
- the Pokédex flags, bag, money.

## Building and testing

```sh
make test                       # the core on the host: 59 checks
make test ROM=emerald.gba SYM=pokeemerald.sym
                                # + the ROM's tables, against pret's symbols (71)
make                            # build/editor.mb (arm-none-eabi-gcc, no devkitPro)
make mgba ROM=emerald.gba GBAGATE=/path/to/gbagate
                                # the editor on a real Emerald save, in mGBA
```

`test/test_core.c` checks the following against saves it builds from nothing:
- **Slots:** the newer one is chosen, and a counter that rolled over is
  handled. A slot with a bad checksum, a missing section or mixed counters is
  passed over, and bytes past a section's size are not checksummed.
- **Writing:**
  - the other slot gets counter + 1, with sector 0 moved on by one sector as
    the game moves it;
  - the old slot and sectors 28–31 are left untouched;
  - a sector that does not read back is caught, and so is a failed write.
- **Records:** all 24 substructure orders, their checksums and every field.
- **Edits:** levels, stats, natures, HP, the IV and EV limits, moves, names.
- **Growth:** experience at every level of every growth rate.

Given the Emerald ROM and pret's symbol file, it also finds the five tables
where pret puts them. It checks every experience value against the ROM's own
tables, too.

`test/mgba/editor_emerald.py` uses gbagate's mGBA harness:
1. Emerald starts a new game. The game's own `ScriptGiveMon` gives eight
   Pokémon, and `TrySavingData` writes the save.
2. The editor takes the GBA over, as it would after multiboot.
3. Key presses only: a nickname, ten levels, an item, two moves, IVs and EVs on
   the lead, a move on a box Pokémon, then **Write to cartridge**.
4. The script reads the flash with a reader of its own and checks every edit,
   the untouched slot and the untouched sectors.
5. A fresh Emerald boots that flash and continues. Its party in RAM has the
   edits, and Emerald's own `CalculateMonStats` gives the editor's stats.

## Where things are

| path | what |
|---|---|
| `src/` | the portable core, host-tested: `g3save` (slots, sections, checksums), `mon` (pk3 records), `edit`, `growth`, `g3text` (the character set), `romdata` (the cartridge's tables) |
| `gba/` | the GBA program: `main.c` (screens), `hw.c` (text and keys), `flash.c`, `crt0.s` and `editor.ld` (the multiboot image) |
| `tools/font.txt` | the font, 5×7, drawn for this program. `make font` makes `src/font_data.c`. |
| `test/` | the host tests and the mGBA test |

The multiboot header leaves Nintendo's logo zeroed: the sender fills it from a
ROM you own, as gbagate's board does. `crt0.s`, `editor.ld` and `flash.c` are
adapted from gbagate (GPL-3.0), as each file says. This directory is GPL-3.0,
as is PKHeX.
