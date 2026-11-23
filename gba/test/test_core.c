/* test_core.c -- the editor's portable core on the host: the save format,
 * records, text and growth, with saves built here from nothing.
 *
 *   make test                       synthetic saves only
 *   make test ROM=emerald.gba SYM=pokeemerald.sym
 *                                   also the ROM tables, against pret's
 *                                   symbols for the ROM it rebuilds
 */
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include "../src/g3save.h"
#include "../src/g3text.h"
#include "../src/growth.h"
#include "../src/mon.h"
#include "../src/romdata.h"
#include "../src/edit.h"

static int fails, checks;
#define CHECK(what, ok) do { checks++; if (!(ok)) { fails++; printf("  FAIL  %s (line %d)\n", what, __LINE__); } } while (0)

/* ---- a flash in memory ---------------------------------------------------- */
static uint8_t flash[32 * G3_SECTOR];
static int writes, fail_write_at = -1, corrupt_write_at = -1;

static int rd(void *ctx, unsigned sector, uint8_t *buf) {
    (void)ctx;
    if (sector >= 32) return -1;
    memcpy(buf, flash + sector * G3_SECTOR, G3_SECTOR);
    return 0;
}
static int wr(void *ctx, unsigned sector, const uint8_t *buf) {
    (void)ctx;
    if (sector >= 32) return -1;
    if (writes++ == fail_write_at) return -1;
    memcpy(flash + sector * G3_SECTOR, buf, G3_SECTOR);
    if (writes - 1 == corrupt_write_at) flash[sector * G3_SECTOR + 100] ^= 1;
    return 0;
}

static void put32(uint8_t *p, uint32_t v) { memcpy(p, &v, 4); }
static void put16(uint8_t *p, uint16_t v) { memcpy(p, &v, 2); }
static uint32_t get32(const uint8_t *p) { uint32_t v; memcpy(&v, p, 4); return v; }
static uint16_t get16(const uint8_t *p) { uint16_t v; memcpy(&v, p, 2); return v; }

/* A slot as the game writes it, its sections filled from a seed. */
static void make_slot(int game, int slot, uint32_t counter, int rot, unsigned seed, uint8_t *sections) {
    srand(seed);
    for (int id = 0; id < G3_SECTIONS; id++) {
        uint8_t *sec = flash + (slot * G3_SECTIONS + (id + rot) % G3_SECTIONS) * G3_SECTOR;
        memset(sec, 0, G3_SECTOR);
        const uint32_t n = g3_section_size(game, id);
        for (uint32_t k = 0; k < n; k++) sec[k] = (uint8_t)rand();
        if (sections) memcpy(sections + id * G3_SECTOR_DATA, sec, G3_SECTOR_DATA);
        put16(sec + 0xFF4, (uint16_t)id);
        put16(sec + 0xFF6, g3_checksum(sec, n));
        put32(sec + 0xFF8, G3_SIGNATURE);
        put32(sec + 0xFFC, counter);
    }
}

static uint8_t data[G3_DATA_SIZE], tmp[G3_SECTOR], expect[G3_DATA_SIZE], other[G3_DATA_SIZE];

static void test_slots(void) {
    printf("save slots\n");
    g3_save_t s = {.game = G3_GAME_E, .data = data};
    CHECK("the checksum folds the word sum", ({ uint8_t b[8] = {1, 0, 1, 0, 0xFF, 0xFF, 0, 0};
          g3_checksum(b, 8) == 2; }));   /* 0x00010001 + 0x0000FFFF = 0x00020000: 2 + 0 */

    memset(flash, 0xFF, sizeof flash);
    CHECK("an erased flash has no save", g3_load(&s, rd, NULL, tmp) == G3_ERR_NO_SAVE);

    make_slot(G3_GAME_E, 0, 7, 3, 1, expect);
    make_slot(G3_GAME_E, 1, 8, 4, 2, other);
    CHECK("two whole slots load", g3_load(&s, rd, NULL, tmp) == G3_OK);
    CHECK("... the newer one (counter 8, slot 1)", s.slot == 1 && s.counter == 8 && s.rot == 4);
    CHECK("... its sections, by id, whatever the rotation", memcmp(data, other, G3_DATA_SIZE) == 0);

    make_slot(G3_GAME_E, 0, 9, 13, 1, expect);
    CHECK("the other slot when it is newer", g3_load(&s, rd, NULL, tmp) == G3_OK && s.slot == 0 &&
          memcmp(data, expect, G3_DATA_SIZE) == 0);

    make_slot(G3_GAME_E, 0, 0xFFFFFFFFu, 0, 1, NULL);
    make_slot(G3_GAME_E, 1, 0, 1, 2, NULL);
    CHECK("a counter that rolled over to 0 is the newer", g3_load(&s, rd, NULL, tmp) == G3_OK && s.slot == 1);

    make_slot(G3_GAME_E, 0, 5, 0, 1, expect);
    make_slot(G3_GAME_E, 1, 6, 1, 2, NULL);
    flash[(1 * G3_SECTIONS + 5) * G3_SECTOR + 17] ^= 0x40;            /* a bad checksum in slot 1 */
    CHECK("a slot with a bad checksum is passed over", g3_load(&s, rd, NULL, tmp) == G3_OK &&
          s.slot == 0 && !s.valid[1] && memcmp(data, expect, G3_DATA_SIZE) == 0);

    make_slot(G3_GAME_E, 1, 6, 1, 2, NULL);
    put16(flash + (1 * G3_SECTIONS + 2) * G3_SECTOR + 0xFF4, 3);       /* section 3 twice, 2 missing */
    put16(flash + (1 * G3_SECTIONS + 2) * G3_SECTOR + 0xFF6,
          g3_checksum(flash + (1 * G3_SECTIONS + 2) * G3_SECTOR, G3_SECTOR_DATA));
    CHECK("a slot missing a section is passed over", g3_load(&s, rd, NULL, tmp) == G3_OK && s.slot == 0);

    make_slot(G3_GAME_E, 1, 6, 1, 2, NULL);
    put32(flash + (1 * G3_SECTIONS + 9) * G3_SECTOR + 0xFFC, 5);       /* one sector from another save */
    CHECK("a slot with mixed counters is passed over", g3_load(&s, rd, NULL, tmp) == G3_OK && s.slot == 0);

    /* the section sizes matter only to the checksum: bytes past a section's
     * size in its sector are not summed */
    make_slot(G3_GAME_E, 0, 5, 0, 1, NULL);
    make_slot(G3_GAME_E, 1, 6, 1, 2, NULL);
    flash[(1 * G3_SECTIONS + 1) * G3_SECTOR + 0xF2C + 4] = 0x55;      /* section 0 at sector 1, past 0xF2C */
    CHECK("bytes past the section's size are not checksummed", g3_load(&s, rd, NULL, tmp) == G3_OK && s.slot == 1);

    printf("writing\n");
    make_slot(G3_GAME_E, 0, 41, 6, 1, NULL);
    make_slot(G3_GAME_E, 1, 40, 5, 2, NULL);
    uint8_t extra[4 * G3_SECTOR];
    for (int k = 0; k < (int)sizeof extra; k++) flash[28 * G3_SECTOR + k] = extra[k] = (uint8_t)(k * 7);
    CHECK("load the newer slot (0)", g3_load(&s, rd, NULL, tmp) == G3_OK && s.slot == 0);
    memcpy(expect, data, G3_DATA_SIZE);
    for (int k = 0; k < 64; k++) data[0x238 + G3_SECTOR_DATA + k] ^= 0x5A;   /* an edit in the party */
    memcpy(expect, data, G3_DATA_SIZE);
    uint8_t slot0[G3_SECTIONS * G3_SECTOR];
    memcpy(slot0, flash, sizeof slot0);
    writes = 0;
    CHECK("commit", g3_commit(&s, rd, wr, NULL, tmp) == G3_OK);
    CHECK("... 14 sectors written", writes == 14);
    CHECK("... to the other slot, counter + 1, section 0 one sector on", s.slot == 1 && s.counter == 42 && s.rot == 7);
    CHECK("... the old slot untouched (a fall-back if the new one breaks)", memcmp(flash, slot0, sizeof slot0) == 0);
    CHECK("... sectors 28..31 untouched", memcmp(flash + 28 * G3_SECTOR, extra, sizeof extra) == 0);
    g3_save_t t = {.game = G3_GAME_E, .data = other};
    CHECK("it loads back as the newer slot", g3_load(&t, rd, NULL, tmp) == G3_OK && t.slot == 1 && t.counter == 42);
    CHECK("... with the edit", memcmp(other, expect, G3_DATA_SIZE) == 0);
    CHECK("... and both slots whole", t.valid[0] && t.valid[1]);

    writes = 0; corrupt_write_at = 5;
    CHECK("a sector that does not read back is caught", g3_commit(&s, rd, wr, NULL, tmp) == G3_ERR_VERIFY);
    corrupt_write_at = -1;
    CHECK("... and the save still loads, from the last good slot", g3_load(&t, rd, NULL, tmp) == G3_OK && t.counter == 42);
    writes = 0; fail_write_at = 3;
    CHECK("a failed write is reported", g3_commit(&s, rd, wr, NULL, tmp) == G3_ERR_IO);
    fail_write_at = -1;
    CHECK("... the save still loads", g3_load(&t, rd, NULL, tmp) == G3_OK && t.counter == 42);

    /* FRLG and RS: their own sizes, their own party offset */
    make_slot(G3_GAME_FRLG, 0, 3, 0, 5, NULL);
    make_slot(G3_GAME_FRLG, 1, 2, 0, 6, NULL);
    s.game = G3_GAME_FRLG;
    CHECK("a FireRed save loads", g3_load(&s, rd, NULL, tmp) == G3_OK && s.slot == 0);
    CHECK("... its party at SaveBlock1 + 0x38", g3_party_mon(&s, 0) == data + G3_SECTOR_DATA + 0x38);
    s.game = G3_GAME_E;
    CHECK("Emerald's party at SaveBlock1 + 0x238", g3_party_mon(&s, 1) == data + G3_SECTOR_DATA + 0x238 + 100);
    CHECK("the game from its code", g3_game_from_code("BPEE") == G3_GAME_E && g3_game_from_code("BPRE") == G3_GAME_FRLG &&
          g3_game_from_code("BPGE") == G3_GAME_FRLG && g3_game_from_code("AXVE") == G3_GAME_RS &&
          g3_game_from_code("AXPE") == G3_GAME_RS && g3_game_from_code("BPEJ") == G3_GAME_E &&
          g3_game_from_code("AGBJ") == G3_GAME_UNKNOWN);
}

/* ---- records ------------------------------------------------------------ */

/* The order of G, A, E, M for each PID % 24, as a table independent of mon.c's:
 * the 24 permutations in lexical order of the letters G < A < E < M. */
static void perm(int n, int out[4]) {
    int left[4] = {0, 1, 2, 3}, cnt = 4;
    const int f[4] = {6, 2, 1, 1};
    for (int k = 0; k < 4; k++) {
        const int i = n / f[k];
        n %= f[k];
        out[k] = left[i];
        for (int j = i; j < cnt - 1; j++) left[j] = left[j + 1];
        cnt--;
    }
}

static void test_records(void) {
    printf("records\n");
    int bad_orders = 0;
    for (uint32_t pid_mod = 0; pid_mod < 24; pid_mod++) {
        int o[4];
        perm((int)pid_mod, o);
        /* a record whose substructure k is all byte 0x10 + k, encrypted by hand */
        uint8_t raw[80] = {0};
        const uint32_t pid = 24 * 1000 + pid_mod, otid = 0x12345678;
        put32(raw, pid);
        put32(raw + 4, otid);
        raw[19] = 2;
        uint16_t sum = 0;
        for (int slot = 0; slot < 4; slot++)
            for (int k = 0; k < 12; k++) raw[32 + slot * 12 + k] = (uint8_t)(0x10 + o[slot]);
        for (int k = 32; k < 80; k += 2) sum = (uint16_t)(sum + get16(raw + k));
        put16(raw + 28, sum);
        for (int k = 32; k < 80; k += 4) put32(raw + k, get32(raw + k) ^ pid ^ otid);
        mon_t m;
        mon_load(&m, raw, 0);
        int ok = m.checksum_ok;
        for (int s = 0; s < 4; s++)
            for (int k = 0; k < 12; k++) ok &= m.b[32 + s * 12 + k] == 0x10 + s;
        uint8_t back[80];
        mon_store(&m, back);
        ok &= memcmp(back, raw, 80) == 0;
        bad_orders += !ok;
    }
    CHECK("all 24 substructure orders decrypt, checksum and re-encrypt exactly", bad_orders == 0);

    /* fields, on a made-up record */
    uint8_t raw[100] = {0};
    put32(raw, 0xDEADBEEF);
    put32(raw + 4, 0x0001E240);
    raw[19] = 2;
    mon_t m;
    mon_load(&m, raw, 1);
    CHECK("zero bytes on the cartridge decrypt to the key", m.b[32] == ((0xDEADBEEFu ^ 0x0001E240u) & 0xFF) && m.b[35] == (0xDEADBEEFu ^ 0x0001E240u) >> 24);
    memset(m.b + 32, 0, 48);
    m.b[32] = 25; m.b[33] = 0;                      /* species */
    mon_set_item(&m, 13);
    mon_set_exp(&m, 135);
    mon_set_move(&m, 2, 85, 15);
    for (int s = 0; s < 6; s++) { mon_set_iv(&m, s, (uint8_t)(s * 5 + 1)); mon_set_ev(&m, s, (uint8_t)(s * 40)); }
    mon_store(&m, raw);
    mon_t n;
    mon_load(&n, raw, 1);
    int ivs = 1;
    for (int s = 0; s < 6; s++) ivs &= mon_iv(&n, s) == s * 5 + 1 && mon_ev(&n, s) == s * 40;
    CHECK("fields survive a store and load", n.checksum_ok && mon_species(&n) == 25 && mon_item(&n) == 13 &&
          mon_exp(&n) == 135 && mon_move(&n, 2) == 85 && mon_pp(&n, 2) == 15 && ivs);
    CHECK("an IV write leaves the egg and ability bits", ({ mon_t q = n; uint8_t *w = q.b + 32 + 36 + 4;
          w[3] |= 0xC0; mon_set_iv(&q, 5, 0); mon_ability_bit(&q) == 1 && (w[3] & 0x40); }));
    CHECK("EV total", mon_ev_total(&n) == 0 + 40 + 80 + 120 + 160 + 200);
    CHECK("nature is PID % 25", mon_nature(&n) == (int)(0xDEADBEEFu % 25));
    raw[40] ^= 1;
    mon_load(&n, raw, 1);
    CHECK("a damaged record fails its checksum", !n.checksum_ok);
    CHECK("PP Ups cleared for a replaced move", ({ mon_t q = m; q.b[32 + 8] = 0xFF;
          mon_set_move(&q, 1, 33, 35); mon_pp_bonuses(&q) == 0xF3; }));
}

/* ---- text and growth ------------------------------------------------------ */

static void test_text(void) {
    printf("text\n");
    const uint8_t s[] = {0xCA, 0xC9, 0xC5, 0x1B, 0xC7, 0xC9, 0xC8, 0x00, 0xA2, 0xAB, 0xFF, 0xBB};
    char out[16];
    g3_decode(s, 11, out);
    CHECK("POKeMON 1!", strcmp(out, "POK\x82MON 1!") == 0);
    int round = 1;
    for (int k = 0; k < g3_name_chars_count; k++) {
        const uint8_t c = g3_name_chars[k];
        round &= g3_from_font(g3_to_font(c)) == c || c == 0xB2 || c == 0xB4;   /* closing quotes read as opening */
    }
    CHECK("every name character has a glyph and comes back", round);
    CHECK("10 characters at most, without EOS", g3_decode((const uint8_t *)"\xBB\xBB\xBB\xBB\xBB\xBB\xBB\xBB\xBB\xBB\xBB", 10, out) == 10);
}

static void test_growth(void) {
    printf("growth\n");
    CHECK("Medium Fast: n^3", growth_exp(GROWTH_MEDIUM_FAST, 50) == 125000 && growth_exp(GROWTH_MEDIUM_FAST, 100) == 1000000);
    CHECK("the totals at 100", growth_exp(GROWTH_ERRATIC, 100) == 600000 && growth_exp(GROWTH_FLUCTUATING, 100) == 1640000 &&
          growth_exp(GROWTH_MEDIUM_SLOW, 100) == 1059860 && growth_exp(GROWTH_FAST, 100) == 800000 &&
          growth_exp(GROWTH_SLOW, 100) == 1250000);
    CHECK("Medium Slow level 2 (9)", growth_exp(GROWTH_MEDIUM_SLOW, 2) == 9);
    int mono = 1;
    for (int r = 0; r < 6; r++)
        for (int l = 1; l < 100; l++) mono &= growth_exp(r, l + 1) > growth_exp(r, l);
    CHECK("every rate rises with level", mono);
    int inv = 1;
    for (int r = 0; r < 6; r++)
        for (int l = 1; l <= 100; l++) {
            inv &= growth_level(r, growth_exp(r, l)) == l;
            if (l > 1) inv &= growth_level(r, growth_exp(r, l) - 1) == l - 1;
        }
    CHECK("the level from experience, at every boundary", inv);

    /* A worked example: Garchomp's base stats are not Gen 3, so a made-up
     * species with round numbers: base 100 everywhere, level 50, IV 31,
     * EV 252 in Atk; Adamant (+Atk, -SpA). */
    species_stats_t sp = {{100, 100, 100, 100, 100, 100}, 0};
    uint8_t iv[6] = {31, 31, 31, 31, 31, 31}, ev[6] = {0, 252, 0, 0, 0, 0};
    uint16_t st[6];
    growth_stats(&sp, 1, 50, 3, iv, ev, st);
    /* HP (200+31)*50/100+60 = 175; Atk ((200+31+63)*50/100+5)*1.1 = 167;
     * Def 120; SpA 120*0.9 = 108 */
    CHECK("stats: HP 175, Atk 167, Def 120, Spe 120, SpA 108, SpD 120",
          st[0] == 175 && st[1] == 167 && st[2] == 120 && st[3] == 120 && st[4] == 108 && st[5] == 120);
    growth_stats(&sp, SPECIES_SHEDINJA, 50, 0, iv, ev, st);
    CHECK("Shedinja's HP is 1", st[0] == 1);
    CHECK("nature effects: Hardy none, Lonely +Atk -Def, Modest +SpA -Atk",
          growth_nature_effect(0, 1) == 0 && growth_nature_effect(1, 1) == 1 && growth_nature_effect(1, 2) == -1 &&
          growth_nature_effect(15, 4) == 1 && growth_nature_effect(15, 1) == -1);
    CHECK("HP follows a new max as the game does",
          growth_new_hp(1, 20, 30, 40) == 30 && growth_new_hp(1, 0, 30, 40) == 0 && growth_new_hp(1, 0, 0, 40) == 40 &&
          growth_new_hp(1, 5, 30, 10) == 1);
}

/* ---- the ROM's own tables --------------------------------------------- */

static uint32_t sym(const char *path, const char *name) {
    FILE *f = fopen(path, "r");
    char line[256], n[128];
    unsigned a, sz;
    char t;
    while (f && fgets(line, sizeof line, f))
        if (sscanf(line, "%x %c %x %127s", &a, &t, &sz, n) == 4 && !strcmp(n, name)) { fclose(f); return a; }
    if (f) fclose(f);
    return 0;
}

static void test_rom(const char *rom_path, const char *sym_path) {
    printf("ROM tables (%s)\n", rom_path);
    FILE *f = fopen(rom_path, "rb");
    if (!f) { CHECK("the ROM opens", 0); return; }
    static uint8_t rom[32 << 20];
    const uint32_t size = (uint32_t)fread(rom, 1, sizeof rom, f);
    fclose(f);
    romdata_t rd;
    CHECK("all five tables found", romdata_find(&rd, rom, size) == 5);
    CHECK("battle moves at gBattleMoves", rd.moves == sym(sym_path, "gBattleMoves") - 0x08000000);
    CHECK("PP: Pound 35, Psycho Boost 5", romdata_move_pp(&rd, 1) == 35 && romdata_move_pp(&rd, 354) == 5);
    CHECK("species names where pret puts gSpeciesNames", rd.species_names == sym(sym_path, "gSpeciesNames") - 0x08000000);
    CHECK("move names at gMoveNames", rd.move_names == sym(sym_path, "gMoveNames") - 0x08000000);
    CHECK("items at gItems", rd.items == sym(sym_path, "gItems") - 0x08000000);
    CHECK("base stats at gSpeciesInfo", rd.base_stats == sym(sym_path, "gSpeciesInfo") - 0x08000000);
    char s[16];
    romdata_species_name(&rd, 277, s);          /* internal 277: Treecko */
    CHECK("species 277 is TREECKO", !strcmp(s, "TREECKO"));
    romdata_move_name(&rd, 354, s);
    CHECK("move 354 is PSYCHO BOOST", !strcmp(s, "PSYCHO BOOST"));
    romdata_item_name(&rd, 1, s);
    CHECK("item 1 is MASTER BALL", !strcmp(s, "MASTER BALL"));
    romdata_item_name(&rd, 376, s);
    printf("    item %d: %s\n", RD_ITEMS_MAX, s);
    species_stats_t sp;
    CHECK("Treecko: 40/45/35/70/65/55, Medium Slow", romdata_species_stats(&rd, 277, &sp) &&
          sp.base[0] == 40 && sp.base[1] == 45 && sp.base[2] == 35 && sp.base[3] == 70 &&
          sp.base[4] == 65 && sp.base[5] == 55 && sp.growth == GROWTH_MEDIUM_SLOW);
    /* the experience tables: every rate, every level, against the ROM's */
    const uint32_t xt = sym(sym_path, "gExperienceTables") - 0x08000000;
    int same = 1;
    for (int r = 0; r < 6; r++)
        for (int l = 0; l <= 100; l++) {
            const uint32_t v = get32(rom + xt + (r * 101 + l) * 4);
            if (v != growth_exp(r, l)) {
                if (same) printf("    rate %d level %d: ROM %u, here %u\n", r, l, v, growth_exp(r, l));
                same = 0;
            }
        }
    CHECK("experience for every level and rate matches the ROM's tables", same);
}

static void test_edits(void) {
    printf("edits\n");
    species_stats_t sp = {{40, 45, 35, 70, 65, 55}, GROWTH_MEDIUM_SLOW};   /* Treecko's */
    uint8_t raw[100] = {0};
    put32(raw, 1234567);                       /* nature 1234567 % 25 = 17, Quiet: +SpA -Spe */
    put32(raw + 4, 0x0BADF00D);
    raw[19] = 2;
    mon_t m;
    mon_load(&m, raw, 1);
    memset(m.b + 32, 0, 48);
    m.b[32] = 277 & 0xFF; m.b[33] = 277 >> 8;
    edit_set_level(&m, &sp, 5);
    CHECK("level 5: experience 135 (Medium Slow), party level 5", mon_exp(&m) == 135 && mon_level(&m) == 5);
    CHECK("... and HP full from nothing (a new max)", mon_hp(&m) == mon_stat(&m, STAT_HP) && mon_hp(&m) > 0);
    for (int s = 0; s < 6; s++) edit_set_iv(&m, s, 31);
    edit_set_level(&m, &sp, 50);
    /* HP (80+31)*50/100+60 = 115; SpA (130+31)*50/100+5 = 85, Quiet: 93;
     * Speed (140+31)*50/100+5 = 90, Quiet: 81 */
    CHECK("level 50, IV 31: HP 115, SpA 93 (+), Speed 81 (-)", mon_level(&m) == 50 &&
          mon_stat(&m, STAT_HP) == 115 && mon_stat(&m, STAT_SPA) == 93 && mon_stat(&m, STAT_SPE) == 81);
    CHECK("an IV past 31 is held at 31", edit_set_iv(&m, 0, 40) == 31 && edit_set_iv(&m, 0, -3) == 0);
    edit_set_iv(&m, 0, 31);
    CHECK("EVs: 252 + 252 + 6, then nothing more fits",
          edit_set_ev(&m, 1, 252) == 252 && edit_set_ev(&m, 3, 300) == 255 && edit_set_ev(&m, 3, 252) == 252 &&
          edit_set_ev(&m, 0, 10) == 6 && edit_set_ev(&m, 5, 1) == 0 && mon_ev_total(&m) == 510);
    const uint16_t hp_before = mon_hp(&m), max_before = mon_stat(&m, STAT_HP);
    edit_refresh(&m, &sp);
    CHECK("the 6 HP EVs give one more HP, and HP follows",
          mon_stat(&m, STAT_HP) == max_before + 1 && mon_hp(&m) == hp_before + 1);
    edit_set_move(&m, 0, 1, 35);
    edit_set_move(&m, 3, 0, 20);
    CHECK("moves: Pound with 35 PP; a cleared slot has none", mon_move(&m, 0) == 1 && mon_pp(&m, 0) == 35 &&
          mon_move(&m, 3) == 0 && mon_pp(&m, 3) == 0);
    const uint8_t name[10] = {0xCE, 0xCC, 0xBF, 0xBF, 0xFF, 0xBB, 0xBB, 0xBB, 0xBB, 0xBB};
    edit_set_nickname(&m, name);
    CHECK("a nickname is padded with EOS after its end",
          !memcmp(mon_nickname(&m), "\xCE\xCC\xBF\xBF\xFF\xFF\xFF\xFF\xFF\xFF", 10));
    mon_store(&m, raw);
    mon_t n;
    mon_load(&n, raw, 1);
    CHECK("all of it stored with a good checksum", n.checksum_ok && mon_level(&n) == 50 && mon_ev_total(&n) == 510);
    mon_t box;
    mon_load(&box, raw, 0);
    edit_set_level(&box, &sp, 30);
    CHECK("a box record: level 30 is its experience alone", edit_level(&box, &sp) == 30 && box.b[84] == 0);
    edit_set_level(&box, &sp, 120);
    CHECK("level 120 is held at 100", edit_level(&box, &sp) == 100);
}

int main(int argc, char **argv) {
    test_edits();
    test_slots();
    test_records();
    test_text();
    test_growth();
    if (argc >= 3) test_rom(argv[1], argv[2]);
    printf("test_core: %d checks, %s\n", checks, fails ? "FAILED" : "all passed");
    return fails ? 1 : 0;
}
