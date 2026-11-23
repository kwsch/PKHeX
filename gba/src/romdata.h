/* romdata.h -- the names and base stats the editor shows, read from the
 * game cartridge itself, so the editor carries no game data.
 *
 * Each table is found by what its first entries hold, on a 4-byte boundary
 * (where the games' linkers put them), not at an address typed in per
 * version:
 *   species names  11-byte entries: "??????????", then "BULBASAUR"
 *   move names     13-byte entries: "-", then "POUND"
 *   items          44-byte entries (RS, FRLG and Emerald alike), the name
 *                  first: "????????", then "MASTER BALL"
 *   base stats     28-byte entries: species 1 is 45/49/49/45/65/65,
 *                  Grass/Poison; growth rate at byte 19
 * English games only, for the names; a table not found shows numbers.
 */
#ifndef ROMDATA_H
#define ROMDATA_H
#include <stdint.h>

typedef struct {
    const uint8_t *rom;
    uint32_t size;
    uint32_t species_names, move_names, items, base_stats;   /* offsets; 0: not found */
} romdata_t;

#define RD_SPECIES_MAX  411       /* internal species 1..411 (412 is the egg) */
#define RD_MOVES_MAX    354
#define RD_ITEMS_MAX    376       /* Emerald's last item; the others have fewer */

/* Find the tables in rom[0..size). Returns how many were found (0..4). */
int romdata_find(romdata_t *rd, const uint8_t *rom, uint32_t size);
/* Names, decoded to font codes into out (12 bytes for species, 13 for
 * moves, 15 for items). A table not found gives "#123". */
void romdata_species_name(const romdata_t *rd, uint16_t species, char *out);
void romdata_move_name(const romdata_t *rd, uint16_t move, char *out);
void romdata_item_name(const romdata_t *rd, uint16_t item, char *out);
/* A species' base stats and growth rate; 0 when the table is not found. */
#include "growth.h"
int romdata_species_stats(const romdata_t *rd, uint16_t species, species_stats_t *out);
#endif
