/* g3save.c -- see g3save.h. The sizes and the slot rules follow the games
 * (pret's pokeemerald save.c, pokefirered, pokeruby) and PKHeX's SAV3. */
#include "g3save.h"
#include "mem.h"

static uint32_t rd32(const uint8_t *p) { return p[0] | p[1] << 8 | p[2] << 16 | (uint32_t)p[3] << 24; }
static uint16_t rd16(const uint8_t *p) { return (uint16_t)(p[0] | p[1] << 8); }
static void wr32(uint8_t *p, uint32_t v) { p[0] = (uint8_t)v; p[1] = (uint8_t)(v >> 8); p[2] = (uint8_t)(v >> 16); p[3] = (uint8_t)(v >> 24); }
static void wr16(uint8_t *p, uint16_t v) { p[0] = (uint8_t)v; p[1] = (uint8_t)(v >> 8); }

int g3_game_from_code(const char c[4]) {
    if (c[0] == 'B' && c[1] == 'P' && c[2] == 'E') return G3_GAME_E;
    if (c[0] == 'B' && c[1] == 'P' && (c[2] == 'R' || c[2] == 'G')) return G3_GAME_FRLG;
    if (c[0] == 'A' && (c[1] == 'X' && (c[2] == 'V' || c[2] == 'P'))) return G3_GAME_RS;
    return G3_GAME_UNKNOWN;
}

uint32_t g3_section_size(int game, int id) {
    if (id == 0) return game == G3_GAME_RS ? 0x890 : game == G3_GAME_FRLG ? 0xF24 : 0xF2C;
    if (id == 4) return game == G3_GAME_RS ? 0xC40 : game == G3_GAME_FRLG ? 0xEE8 : 0xF08;
    if (id == 13) return 0x7D0;
    return G3_SECTOR_DATA;
}

uint16_t g3_checksum(const uint8_t *p, uint32_t n) {
    uint32_t sum = 0;
    for (uint32_t k = 0; k + 4 <= n; k += 4) sum += rd32(p + k);
    return (uint16_t)((sum >> 16) + (sum & 0xFFFF));
}

/* One slot's footers: whether all 14 sections are there with good
 * signatures and checksums, its counter, and where section 0 is. */
static int scan_slot(g3_save_t *s, int slot, g3_read_fn rd, void *ctx, uint8_t *tmp,
                     uint32_t *counter, uint8_t *rot) {
    unsigned seen = 0;
    for (unsigned k = 0; k < G3_SECTIONS; k++) {
        if (rd(ctx, slot * G3_SECTIONS + k, tmp)) return G3_ERR_IO;
        const unsigned id = rd16(tmp + 0xFF4);
        if (rd32(tmp + 0xFF8) != G3_SIGNATURE || id >= G3_SECTIONS) return 0;
        if (rd16(tmp + 0xFF6) != g3_checksum(tmp, g3_section_size(s->game, id))) return 0;
        if (k == 0) *counter = rd32(tmp + 0xFFC);
        else if (rd32(tmp + 0xFFC) != *counter) return 0;
        if (id == 0) *rot = (uint8_t)k;
        seen |= 1u << id;
    }
    return seen == (1u << G3_SECTIONS) - 1;
}

int g3_load(g3_save_t *s, g3_read_fn rd, void *ctx, uint8_t *tmp) {
    uint8_t rot[2] = {0, 0};
    for (int slot = 0; slot < 2; slot++) {
        s->counters[slot] = 0;
        const int r = scan_slot(s, slot, rd, ctx, tmp, &s->counters[slot], &rot[slot]);
        if (r < 0) return r;
        s->valid[slot] = r;
    }
    if (!s->valid[0] && !s->valid[1]) return G3_ERR_NO_SAVE;
    /* both whole: the higher counter, as the games choose (with PKHeX's
     * reading of a counter that rolled over from 0xFFFFFFFF) */
    int slot;
    if (!s->valid[1]) slot = 0;
    else if (!s->valid[0]) slot = 1;
    else {
        const uint32_t a = s->counters[0], b = s->counters[1];
        if (a == 0xFFFFFFFFu && b == 0) slot = 1;
        else if (b == 0xFFFFFFFFu && a == 0) slot = 0;
        else slot = b > a ? 1 : 0;
    }
    s->slot = slot;
    s->counter = s->counters[slot];
    s->rot = rot[slot];
    for (unsigned k = 0; k < G3_SECTIONS; k++) {
        if (rd(ctx, slot * G3_SECTIONS + k, tmp)) return G3_ERR_IO;
        const unsigned id = rd16(tmp + 0xFF4);
        mem_copy(s->data + id * G3_SECTOR_DATA, tmp, G3_SECTOR_DATA);
    }
    return G3_OK;
}

static void build_sector(const g3_save_t *s, unsigned id, uint32_t counter, uint8_t *tmp) {
    mem_set(tmp, 0, G3_SECTOR);
    const uint32_t n = g3_section_size(s->game, id);
    mem_copy(tmp, s->data + id * G3_SECTOR_DATA, n);
    wr16(tmp + 0xFF4, (uint16_t)id);
    wr16(tmp + 0xFF6, g3_checksum(tmp, n));
    wr32(tmp + 0xFF8, G3_SIGNATURE);
    wr32(tmp + 0xFFC, counter);
}

int g3_commit(g3_save_t *s, g3_read_fn rd, g3_write_fn wr, void *ctx, uint8_t *tmp) {
    const int slot = s->slot ^ 1;
    const uint32_t counter = s->counter + 1;
    const uint8_t rot = (uint8_t)((s->rot + 1) % G3_SECTIONS);   /* as the game moves it */
    for (unsigned id = 0; id < G3_SECTIONS; id++) {
        build_sector(s, id, counter, tmp);
        if (wr(ctx, slot * G3_SECTIONS + (id + rot) % G3_SECTIONS, tmp)) return G3_ERR_IO;
    }
    /* read back: every byte the game will read, and the footer */
    for (unsigned id = 0; id < G3_SECTIONS; id++) {
        if (rd(ctx, slot * G3_SECTIONS + (id + rot) % G3_SECTIONS, tmp)) return G3_ERR_IO;
        const uint32_t n = g3_section_size(s->game, id);
        if (rd16(tmp + 0xFF4) != id || rd32(tmp + 0xFFC) != counter || rd32(tmp + 0xFF8) != G3_SIGNATURE ||
            !mem_equal(tmp, s->data + id * G3_SECTOR_DATA, n) || rd16(tmp + 0xFF6) != g3_checksum(tmp, n))
            return G3_ERR_VERIFY;
    }
    s->valid[slot] = 1;
    s->counters[slot] = counter;
    s->slot = slot;
    s->counter = counter;
    s->rot = rot;
    return G3_OK;
}

/* ---- what is in the sections -------------------------------------------- */

uint8_t *g3_sb2(g3_save_t *s) { return s->data; }
uint8_t *g3_sb1(g3_save_t *s) { return s->data + 1 * G3_SECTOR_DATA; }
uint8_t *g3_storage(g3_save_t *s) { return s->data + 5 * G3_SECTOR_DATA; }

static uint32_t party_off(const g3_save_t *s) { return s->game == G3_GAME_FRLG ? 0x34 : 0x234; }

int g3_party_count(g3_save_t *s) {
    const int n = g3_sb1(s)[party_off(s)];
    return n > 6 ? 6 : n;
}

uint8_t *g3_party_mon(g3_save_t *s, int k) { return g3_sb1(s) + party_off(s) + 4 + k * G3_MON_PARTY; }
int g3_current_box(g3_save_t *s) { return g3_storage(s)[0] % G3_BOXES; }
uint8_t *g3_box_mon(g3_save_t *s, int box, int k) {
    return g3_storage(s) + 4 + (box * G3_BOX_SLOTS + k) * G3_MON_BOX;
}
uint8_t *g3_box_name(g3_save_t *s, int box) {
    return g3_storage(s) + 4 + G3_BOXES * G3_BOX_SLOTS * G3_MON_BOX + box * 9;
}
