/* main.c -- the save editor on the GBA: the game cartridge's own save, read,
 * viewed, edited and written back, with the cartridge in the slot and this
 * program multibooted (doc: gba/README.md).
 *
 * Screens: the trainer and the menu; the party; the boxes; one Pokemon,
 * with its edits; a name. Edits stay in RAM until "Write to cartridge",
 * which writes the other save slot as the game would and reads it back.
 */
#include <stdint.h>

#include "../src/edit.h"
#include "../src/g3save.h"
#include "../src/g3text.h"
#include "../src/mem.h"
#include "../src/romdata.h"
#include "flash.h"
#include "hw.h"

#define ROM ((const uint8_t *)0x08000000)
#define ROM_SCAN 0x01000000u              /* 16 MB: every Generation 3 game */

/* The editor's state, where a test can find it: the screen and a write's
 * result, at a fixed address (editor.ld: 0x0203EF00). */
typedef struct {
    uint32_t magic;                       /* "PKGB" when the save is loaded */
    uint32_t screen;
    uint32_t writes;                      /* saves written */
    int32_t last_write;                   /* G3_OK or an error */
    uint32_t chip_id;
    uint32_t tables;                      /* ROM tables found */
} status_t;
__attribute__((section(".status"))) volatile status_t g_status;

static uint8_t s_data[G3_DATA_SIZE];
static uint8_t s_tmp[G3_SECTOR];
static g3_save_t s_save;
static romdata_t s_rd;
static char s_code[5];
static int s_dirty;

enum { SCR_MAIN, SCR_PARTY, SCR_BOX, SCR_MON, SCR_NAME, SCR_CONFIRM, SCR_ERROR };

static char buf[32];

/* ---- helpers ------------------------------------------------------------- */

static void header(const char *title) {
    cls();
    putn(0, 0, PAL_HEAD, title, 30);
    if (s_dirty) put(22, 0, PAL_HEAD, "edited");
}

static void footer(const char *help) {
    putn(0, 19, PAL_DIM, help, 30);
}

static int wait_key(void) {
    for (;;) {
        vsync();
        const int k = keys_poll();
        if (k) return k;
    }
}

static void fail(const char *a, const char *b) {
    header("PKHeX GBA");
    put(1, 4, PAL_WARN, a);
    if (b) put(1, 6, PAL_TEXT, b);
    g_status.screen = SCR_ERROR;
    for (;;) vsync();
}

/* A Pokemon's species name, nickname, and level (box records by experience). */
static int mon_level_of(mon_t *m) {
    if (m->party) return mon_level(m);
    species_stats_t sp;
    return romdata_species_stats(&s_rd, mon_species(m), &sp) ? edit_level(m, &sp) : 0;
}

static void mon_row(int x, int y, int pal, mon_t *m, int long_form) {
    char name[12], lv[6];
    if (!mon_present(m)) { putn(x, y, PAL_DIM, "-", long_form ? 28 : 14); return; }
    if (!m->checksum_ok) { putn(x, y, PAL_WARN, "(bad record)", long_form ? 28 : 14); return; }
    g3_decode(mon_nickname(m), 10, name);
    putn(x, y, pal, name, 11);
    fmt_u(lv, (uint32_t)mon_level_of(m), 3);
    if (long_form) {
        char sp[12];
        romdata_species_name(&s_rd, mon_species(m), sp);
        putn(x + 11, y, pal, mon_is_egg(m) ? "Egg" : sp, 11);
        putn(x + 22, y, pal, "Lv", 2);
        put(x + 24, y, pal, lv);
    } else {
        put(x + 11, y, pal, lv);
    }
}

/* ---- loading and writing ------------------------------------------------- */

static void load_or_fail(void) {
    const int r = g3_load(&s_save, flash_read, 0, s_tmp);
    if (r == G3_ERR_NO_SAVE) fail("No save on this cartridge.", "Play and save once first.");
    if (r) fail("The save could not be read.", 0);
    s_dirty = 0;
    g_status.magic = 0x42474B50;          /* "PKGB" */
}

static void write_save(void) {
    header("Writing the save");
    put(1, 3, PAL_TEXT, "Keep the GBA on.");
    put(1, 5, PAL_TEXT, "Slot ");
    fmt_u(buf, (uint32_t)(s_save.slot ^ 1), 1);
    put(6, 5, PAL_TEXT, buf);
    put(8, 5, PAL_TEXT, "save count ");
    fmt_u(buf, s_save.counter + 1, 1);
    put(19, 5, PAL_TEXT, buf);
    vsync();
    const int r = g3_commit(&s_save, flash_read, flash_write, 0, s_tmp);
    g_status.last_write = r;
    if (r == G3_OK) {
        g_status.writes++;
        s_dirty = 0;
        put(1, 8, PAL_GOOD, "Written and read back.");
        put(1, 10, PAL_TEXT, "The game loads it next.");
    } else {
        put(1, 8, PAL_WARN, r == G3_ERR_VERIFY ? "It did not read back." : "The flash did not answer.");
        put(1, 10, PAL_TEXT, "The last save is still");
        put(1, 11, PAL_TEXT, "there: the game loads that.");
        load_or_fail();                   /* whatever is on the flash now */
    }
    footer("A: back");
    while (!(wait_key() & (KEY_A | KEY_B))) {}
}

/* ---- one Pokemon --------------------------------------------------------- */

enum { ROW_NAME, ROW_LEVEL, ROW_ITEM, ROW_MOVE1, ROW_MOVE4 = ROW_MOVE1 + 3, ROW_IV, ROW_EV, ROWS };
static const char *const stat_names[6] = {"HP", "Atk", "Def", "Spe", "SpA", "SpD"};

static int name_editor(uint8_t name[10]) {
    uint8_t work[10];
    mem_copy(work, name, 10);
    int pos = 0;
    for (;;) {
        header("Nickname");
        char shown[11];
        for (int k = 0; k < 10; k++) shown[k] = (char)(work[k] == 0xFF ? '_' : g3_to_font(work[k]));
        shown[10] = 0;
        put(10, 6, PAL_TEXT, shown);
        put(10 + pos, 7, PAL_SEL, "^");
        footer("Up/Dn:letter <>:move SEL:end");
        put(0, 17, PAL_DIM, "A: done   B: cancel");
        g_status.screen = SCR_NAME;
        const int k = wait_key();
        int idx = 0;
        for (int c = 0; c < g3_name_chars_count; c++) if (g3_name_chars[c] == work[pos]) idx = c;
        if (k & (KEY_UP | KEY_DOWN)) {
            if (work[pos] == 0xFF) idx = (k & KEY_UP) ? 0 : g3_name_chars_count - 1;
            else idx = (idx + ((k & KEY_UP) ? 1 : g3_name_chars_count - 1)) % g3_name_chars_count;
            work[pos] = g3_name_chars[idx];
            for (int j = 0; j < pos; j++) if (work[j] == 0xFF) work[j] = 0x00;   /* no gaps */
        }
        if ((k & KEY_RIGHT) && pos < 9) pos++;
        if ((k & KEY_LEFT) && pos > 0) pos--;
        if (k & KEY_SELECT) for (int j = pos; j < 10; j++) work[j] = 0xFF;
        if (k & KEY_B) return 0;
        if ((k & (KEY_A | KEY_START)) && work[0] != 0xFF) {
            mem_copy(name, work, 10);
            return 1;
        }
    }
}

/* Show and edit one Pokemon; src is its record in the save buffer. */
static void mon_screen(uint8_t *src, int party) {
    mon_t m;
    mon_load(&m, src, party);
    if (!mon_present(&m) || !m.checksum_ok) return;
    species_stats_t sp;
    const int have_stats = romdata_species_stats(&s_rd, mon_species(&m), &sp);
    int row = 0, col = 0, changed = 0;
    for (;;) {
        g_status.screen = SCR_MON;
        char name[12], spn[12];
        g3_decode(mon_nickname(&m), 10, name);
        romdata_species_name(&s_rd, mon_species(&m), spn);
        header(party ? "Party Pokemon" : "Box Pokemon");
        put(1, 1, PAL_TEXT, mon_is_egg(&m) ? "Egg" : spn);
        fmt_u(buf, mon_species(&m), 1);
        put(13, 1, PAL_DIM, "#");
        put(14, 1, PAL_DIM, buf);
        g3_decode(mon_ot_name(&m), 7, buf);
        put(1, 2, PAL_DIM, "OT");
        put(4, 2, PAL_TEXT, buf);
        put(12, 2, PAL_DIM, "ID");
        fmt_u(buf, mon_otid(&m) & 0xFFFF, 5);
        put(15, 2, PAL_TEXT, buf);
        put(21, 2, PAL_TEXT, nature_names[mon_nature(&m)]);

        const int y0 = 4;
        for (int r = 0; r < ROWS; r++) {
            const int y = y0 + r + (r >= ROW_IV ? 1 : 0);
            const int pal = r == row ? PAL_SEL : PAL_TEXT;
            switch (r) {
            case ROW_NAME: putn(0, y, pal, " Name", 8); putn(8, y, pal, name, 22); break;
            case ROW_LEVEL: {
                putn(0, y, pal, " Level", 8);
                fmt_u(buf, (uint32_t)mon_level_of(&m), 3);
                putn(8, y, pal, buf, 5);
                put(13, y, pal, "Exp");
                fmt_u(buf, mon_exp(&m), 7);
                putn(17, y, pal, buf, 13);
                break;
            }
            case ROW_ITEM: {
                char it[16];
                romdata_item_name(&s_rd, mon_item(&m), it);
                putn(0, y, pal, " Item", 8);
                putn(8, y, pal, it, 22);
                break;
            }
            case ROW_IV: case ROW_EV: {
                putn(0, y, pal, r == ROW_IV ? " IV" : " EV", 5);
                for (int s = 0; s < 6; s++) {
                    fmt_u(buf, r == ROW_IV ? mon_iv(&m, s) : mon_ev(&m, s), 3);
                    putn(5 + s * 4, y, (r == row && s == col) ? PAL_SEL : PAL_TEXT, buf, 4);
                }
                break;
            }
            default: {
                const int k = r - ROW_MOVE1;
                char mv[14];
                romdata_move_name(&s_rd, mon_move(&m, k), mv);
                buf[0] = ' '; buf[1] = 'M'; buf[2] = 'o'; buf[3] = 'v'; buf[4] = 'e'; buf[5] = ' ';
                buf[6] = (char)('1' + k); buf[7] = 0;
                putn(0, y, pal, buf, 8);
                putn(8, y, pal, mv, 14);
                if (mon_move(&m, k)) {
                    fmt_u(buf, mon_pp(&m, k), 2);
                    putn(22, y, pal, "PP", 3);
                    putn(25, y, pal, buf, 5);
                } else {
                    putn(22, y, pal, "", 8);
                }
            }
            }
        }
        /* the stat names over the IV and EV columns, and the stats */
        const int yh = y0 + ROW_IV;
        for (int s = 0; s < 6; s++) put(6 + s * 4, yh, PAL_DIM, stat_names[s]);
        fmt_u(buf, (uint32_t)mon_ev_total(&m), 3);
        put(1, y0 + ROWS + 1, PAL_DIM, "EV total");
        put(10, y0 + ROWS + 1, PAL_TEXT, buf);
        put(13, y0 + ROWS + 1, PAL_DIM, "/510");
        if (party) {
            put(1, y0 + ROWS + 2, PAL_DIM, "Stat");
            for (int s = 0; s < 6; s++) {
                fmt_u(buf, mon_stat(&m, s), 3);
                put(5 + s * 4, y0 + ROWS + 2, PAL_TEXT, buf);
            }
        }
        footer(row >= ROW_IV ? "<>:1 LR:10 A:next stat B:done" : row == ROW_NAME ? "A: change name    B: done"
                                                                      : "<>: 1   L/R: 10   B: done");
        if (!have_stats) put(0, 18, PAL_WARN, "No base stats: level fixed");

        const int k = wait_key();
        if (k & KEY_B) break;
        if (k & KEY_UP) row = (row + ROWS - 1) % ROWS;
        if (k & KEY_DOWN) row = (row + 1) % ROWS;
        int d = 0;
        if (k & KEY_RIGHT) d = 1;
        if (k & KEY_LEFT) d = -1;
        if (k & KEY_R) d = 10;
        if (k & KEY_L) d = -10;
        if ((k & KEY_A) && row >= ROW_IV) col = (col + 1) % 6;
        if ((k & KEY_A) && row == ROW_NAME) {
            uint8_t nm[10];
            mem_copy(nm, mon_nickname(&m), 10);
            if (name_editor(nm)) { edit_set_nickname(&m, nm); changed = 1; }
        }
        if (!d) continue;
        switch (row) {
        case ROW_NAME: break;
        case ROW_LEVEL:
            if (have_stats) { edit_set_level(&m, &sp, mon_level_of(&m) + d); changed = 1; }
            break;
        case ROW_ITEM: {
            int v = (int)mon_item(&m) + d;
            v = (v + RD_ITEMS_MAX + 1) % (RD_ITEMS_MAX + 1);
            mon_set_item(&m, (uint16_t)v);
            changed = 1;
            break;
        }
        case ROW_IV:
            edit_set_iv(&m, col, mon_iv(&m, col) + d);
            changed = 1;
            break;
        case ROW_EV:
            edit_set_ev(&m, col, mon_ev(&m, col) + d);
            changed = 1;
            break;
        default: {
            const int slot = row - ROW_MOVE1;
            int v = (int)mon_move(&m, slot) + d;
            v = (v + RD_MOVES_MAX + 1) % (RD_MOVES_MAX + 1);
            edit_set_move(&m, slot, (uint16_t)v, romdata_move_pp(&s_rd, (uint16_t)v));
            changed = 1;
        }
        }
        if (have_stats) edit_refresh(&m, &sp);
    }
    if (changed) {
        mon_store(&m, src);
        s_dirty = 1;
    }
}

/* ---- lists ------------------------------------------------------------- */

static void party_screen(void) {
    int sel = 0;
    for (;;) {
        g_status.screen = SCR_PARTY;
        header("Party");
        const int n = g3_party_count(&s_save);
        for (int k = 0; k < 6; k++) {
            mon_t m;
            mon_load(&m, g3_party_mon(&s_save, k), 1);
            if (k >= n) { putn(1, 2 + 2 * k, PAL_DIM, "-", 28); continue; }
            mon_row(1, 2 + 2 * k, k == sel ? PAL_SEL : PAL_TEXT, &m, 1);
            if (k == sel) put(0, 2 + 2 * k, PAL_SEL, "\x84");
        }
        footer("A: view and edit   B: back");
        const int k = wait_key();
        if (k & KEY_B) return;
        if ((k & KEY_DOWN) && sel + 1 < n) sel++;
        if ((k & KEY_UP) && sel > 0) sel--;
        if ((k & KEY_A) && sel < n) mon_screen(g3_party_mon(&s_save, sel), 1);
    }
}

static void box_screen(void) {
    int box = g3_current_box(&s_save), sel = 0;
    for (;;) {
        g_status.screen = SCR_BOX;
        char nm[10];
        g3_decode(g3_box_name(&s_save, box), 8, nm);
        header("");
        put(0, 0, PAL_HEAD, "L");
        fmt_u(buf, (uint32_t)box + 1, 2);
        put(2, 0, PAL_HEAD, "Box");
        put(6, 0, PAL_HEAD, buf);
        put(9, 0, PAL_HEAD, nm);
        put(29, 0, PAL_HEAD, "R");
        for (int k = 0; k < G3_BOX_SLOTS; k++) {
            mon_t m;
            mon_load(&m, g3_box_mon(&s_save, box, k), 0);
            const int x = (k % 2) * 15, y = 2 + k / 2;
            mon_row(x + 1, y, k == sel ? PAL_SEL : PAL_TEXT, &m, 0);
        }
        footer("A: view and edit   B: back");
        const int k = wait_key();
        if (k & KEY_B) return;
        if (k & KEY_R) box = (box + 1) % G3_BOXES;
        if (k & KEY_L) box = (box + G3_BOXES - 1) % G3_BOXES;
        if ((k & KEY_RIGHT) && sel % 2 == 0) sel++;
        if ((k & KEY_LEFT) && sel % 2 == 1) sel--;
        if ((k & KEY_DOWN) && sel + 2 < G3_BOX_SLOTS) sel += 2;
        if ((k & KEY_UP) && sel >= 2) sel -= 2;
        if (k & KEY_A) mon_screen(g3_box_mon(&s_save, box, sel), 0);
    }
}

/* ---- the trainer and the menu --------------------------------------------- */

static int confirm(const char *q) {
    g_status.screen = SCR_CONFIRM;
    header("Confirm");
    put(1, 6, PAL_TEXT, q);
    footer("A: yes   B: no");
    for (;;) {
        const int k = wait_key();
        if (k & KEY_A) return 1;
        if (k & KEY_B) return 0;
    }
}

static void main_screen(void) {
    static const char *const items[] = {"Party", "Boxes", "Write to cartridge", "Undo all edits"};
    int sel = 0;
    for (;;) {
        g_status.screen = SCR_MAIN;
        header("PKHeX GBA: save editor");
        char t[13];
        for (int k = 0; k < 12; k++) t[k] = (char)ROM[0xA0 + k];
        t[12] = 0;
        put(1, 2, PAL_DIM, "Game");
        put(7, 2, PAL_TEXT, t);
        put(20, 2, PAL_TEXT, s_code);
        uint8_t *sb2 = g3_sb2(&s_save);
        g3_decode(sb2, 7, buf);
        put(1, 3, PAL_DIM, "OT");
        put(7, 3, PAL_TEXT, buf);
        put(15, 3, PAL_TEXT, sb2[8] ? "\x81" : "\x80");
        put(17, 3, PAL_DIM, "ID");
        fmt_u(buf, (uint32_t)(sb2[0xA] | sb2[0xB] << 8), 5);
        put(20, 3, PAL_TEXT, buf);
        put(1, 4, PAL_DIM, "Save");
        put(7, 4, PAL_TEXT, "slot");
        fmt_u(buf, (uint32_t)s_save.slot, 1);
        put(12, 4, PAL_TEXT, buf);
        put(14, 4, PAL_TEXT, "count");
        fmt_u(buf, s_save.counter, 1);
        put(20, 4, PAL_TEXT, buf);
        put(1, 5, PAL_DIM, "Flash");
        fmt_hex(buf, g_status.chip_id, 4);
        put(7, 5, PAL_TEXT, buf);
        if (s_rd.species_names) put(12, 5, PAL_DIM, "names from the ROM");
        else put(12, 5, PAL_WARN, "no names found");
        for (unsigned k = 0; k < sizeof items / sizeof items[0]; k++) {
            putn(2, 8 + 2 * (int)k, k == (unsigned)sel ? PAL_SEL : PAL_TEXT, items[k], 24);
            if (k == (unsigned)sel) put(1, 8 + 2 * (int)k, PAL_SEL, "\x84");
        }
        footer(s_dirty ? "Edits are not written yet." : "A: choose");
        const int k = wait_key();
        if ((k & KEY_DOWN) && sel < 3) sel++;
        if ((k & KEY_UP) && sel > 0) sel--;
        if (!(k & KEY_A)) continue;
        if (sel == 0) party_screen();
        if (sel == 1) box_screen();
        if (sel == 2 && confirm("Write the save to the game?")) write_save();
        if (sel == 3 && s_dirty && confirm("Undo every edit?")) load_or_fail();
    }
}

int main(void) {
    mem_set((void *)&g_status, 0, sizeof g_status);   /* outside .bss: not zeroed by crt0 */
    hw_init();
    header("PKHeX GBA");
    put(1, 3, PAL_TEXT, "Reading the cartridge...");
    vsync();
    for (int k = 0; k < 4; k++) s_code[k] = (char)ROM[0xAC + k];
    s_save.game = g3_game_from_code(s_code);
    s_save.data = s_data;
    if (s_save.game == G3_GAME_UNKNOWN)
        fail("Not a Generation 3 Pokemon game.", s_code);
    g_status.chip_id = flash_chip_id();
    if (flash_chip_size(g_status.chip_id) != 0x20000)
        fail("The save chip is not a known", "128 KB flash.");
    g_status.tables = (uint32_t)romdata_find(&s_rd, ROM, ROM_SCAN);
    load_or_fail();
    main_screen();
    return 0;
}
