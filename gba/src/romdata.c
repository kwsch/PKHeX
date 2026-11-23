/* romdata.c -- see romdata.h. */
#include "romdata.h"
#include "g3text.h"

/* "BULBASAUR" etc. in the game's characters, EOS-terminated */
static const uint8_t q10[] = {0xAC, 0xAC, 0xAC, 0xAC, 0xAC, 0xAC, 0xAC, 0xAC, 0xAC, 0xAC, 0xFF};
static const uint8_t bulbasaur[] = {0xBC, 0xCF, 0xC6, 0xBC, 0xBB, 0xCD, 0xBB, 0xCF, 0xCC, 0xFF};
static const uint8_t dash[] = {0xAE, 0xFF};
static const uint8_t pound[] = {0xCA, 0xC9, 0xCF, 0xC8, 0xBE, 0xFF};
static const uint8_t q8[] = {0xAC, 0xAC, 0xAC, 0xAC, 0xAC, 0xAC, 0xAC, 0xAC, 0xFF};
static const uint8_t master_ball[] = {0xC7, 0xBB, 0xCD, 0xCE, 0xBF, 0xCC, 0x00, 0xBC, 0xBB, 0xC6, 0xC6, 0xFF};
static const uint8_t bulba_stats[] = {45, 49, 49, 45, 65, 65, 12, 3};   /* types: Grass, Poison */
/* Pound and Karate Chop: effect, power, type, accuracy, PP, effect chance */
static const uint8_t pound_move[] = {0, 40, 0, 100, 35, 0};
static const uint8_t chop_move[] = {43, 50, 1, 100, 25, 0};

static int at(const uint8_t *p, const uint8_t *s, unsigned n) {
    for (unsigned k = 0; k < n; k++) if (p[k] != s[k]) return 0;
    return 1;
}

static int zero(const uint8_t *p, unsigned n) {
    for (unsigned k = 0; k < n; k++) if (p[k]) return 0;
    return 1;
}

int romdata_find(romdata_t *rd, const uint8_t *rom, uint32_t size) {
    rd->rom = rom;
    rd->size = size;
    rd->species_names = rd->move_names = rd->items = rd->base_stats = rd->moves = 0;
    int found = 0;
    for (uint32_t off = 0x200; off + 64 <= size && found < 5; off += 4) {
        const uint8_t *p = rom + off;
        /* the cheapest test first: one byte of each table's first entry */
        if (p[0] == 0xAC) {
            if (!rd->species_names && at(p, q10, 11) && at(p + 11, bulbasaur, sizeof bulbasaur)) {
                rd->species_names = off; found++;
            } else if (!rd->items && at(p, q8, sizeof q8) && at(p + 44, master_ball, sizeof master_ball)) {
                rd->items = off; found++;
            }
        } else if (p[0] == 0xAE) {
            if (!rd->move_names && at(p, dash, 2) && at(p + 13, pound, sizeof pound)) {
                rd->move_names = off; found++;
            }
        } else if (p[0] == 0) {
            if (!rd->base_stats && at(p + 28, bulba_stats, sizeof bulba_stats) && zero(p, 8)) {
                rd->base_stats = off; found++;
            } else if (!rd->moves && zero(p, 12) && at(p + 12, pound_move, sizeof pound_move) &&
                       at(p + 24, chop_move, sizeof chop_move)) {
                rd->moves = off; found++;
            }
        }
    }
    return found;
}

static void number(char *out, unsigned v) {
    char tmp[8];
    int n = 0;
    do { tmp[n++] = (char)('0' + v % 10); v /= 10; } while (v);
    *out++ = '#';
    while (n) *out++ = tmp[--n];
    *out = 0;
}

void romdata_species_name(const romdata_t *rd, uint16_t species, char *out) {
    if (species == 412) { out[0] = 'E'; out[1] = 'g'; out[2] = 'g'; out[3] = 0; return; }
    if (!rd->species_names || species > RD_SPECIES_MAX) { number(out, species); return; }
    g3_decode(rd->rom + rd->species_names + species * 11, 10, out);
}

void romdata_move_name(const romdata_t *rd, uint16_t move, char *out) {
    if (move == 0) { out[0] = '-'; out[1] = 0; return; }
    if (!rd->move_names || move > RD_MOVES_MAX) { number(out, move); return; }
    g3_decode(rd->rom + rd->move_names + move * 13, 12, out);
}

void romdata_item_name(const romdata_t *rd, uint16_t item, char *out) {
    if (item == 0) { out[0] = '-'; out[1] = 0; return; }
    if (!rd->items || item > RD_ITEMS_MAX) { number(out, item); return; }
    g3_decode(rd->rom + rd->items + item * 44, 14, out);
}

int romdata_species_stats(const romdata_t *rd, uint16_t species, species_stats_t *out) {
    if (!rd->base_stats || species == 0 || species > RD_SPECIES_MAX) return 0;
    const uint8_t *p = rd->rom + rd->base_stats + species * 28;
    for (int k = 0; k < 6; k++) out->base[k] = p[k];
    out->growth = p[19];
    return 1;
}

uint8_t romdata_move_pp(const romdata_t *rd, uint16_t move) {
    if (!rd->moves || move == 0 || move > RD_MOVES_MAX) return 0;
    return rd->rom[rd->moves + move * 12 + 4];
}
