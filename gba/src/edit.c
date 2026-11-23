/* edit.c -- see edit.h. */
#include "edit.h"

int edit_level(const mon_t *m, const species_stats_t *sp) {
    return growth_level(sp->growth, mon_exp(m));
}

void edit_refresh(mon_t *m, const species_stats_t *sp) {
    if (!m->party) return;
    uint8_t iv[6], ev[6];
    for (int s = 0; s < 6; s++) { iv[s] = mon_iv(m, s); ev[s] = mon_ev(m, s); }
    const int level = edit_level(m, sp);
    uint16_t st[6];
    growth_stats(sp, mon_species(m), level, mon_nature(m), iv, ev, st);
    const uint16_t hp = growth_new_hp(mon_species(m), mon_hp(m), mon_stat(m, STAT_HP), st[0]);
    mon_set_party_stats(m, (uint8_t)level, st, hp > st[0] ? st[0] : hp);
}

void edit_set_level(mon_t *m, const species_stats_t *sp, int level) {
    if (level < 1) level = 1;
    if (level > 100) level = 100;
    mon_set_exp(m, growth_exp(sp->growth, level));
    edit_refresh(m, sp);
}

int edit_set_iv(mon_t *m, int stat, int v) {
    if (v < 0) v = 0;
    if (v > 31) v = 31;
    mon_set_iv(m, stat, (uint8_t)v);
    return v;
}

int edit_set_ev(mon_t *m, int stat, int v) {
    if (v < 0) v = 0;
    if (v > 255) v = 255;
    const int others = mon_ev_total(m) - mon_ev(m, stat);
    if (others + v > EV_TOTAL_MAX) v = EV_TOTAL_MAX - others;
    mon_set_ev(m, stat, (uint8_t)v);
    return v;
}

void edit_set_move(mon_t *m, int slot, uint16_t move, uint8_t base_pp) {
    mon_set_move(m, slot, move, move ? base_pp : 0);
}

void edit_set_nickname(mon_t *m, const uint8_t name[10]) {
    uint8_t *n = mon_nickname(m);
    int end = 0;
    for (int k = 0; k < 10; k++) {
        if (!end && name[k] == 0xFF) end = 1;
        n[k] = end ? 0xFF : name[k];
    }
}
