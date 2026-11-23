/* mon.c -- see mon.h. The order table is the games' (pret's
 * GetSubstruct): for PID % 24, the substructure in each 12-byte slot. */
#include "mon.h"
#include "mem.h"

enum { G, A, E, M };
static const uint8_t order[24][4] = {
    {G, A, E, M}, {G, A, M, E}, {G, E, A, M}, {G, E, M, A}, {G, M, A, E}, {G, M, E, A},
    {A, G, E, M}, {A, G, M, E}, {A, E, G, M}, {A, E, M, G}, {A, M, G, E}, {A, M, E, G},
    {E, G, A, M}, {E, G, M, A}, {E, A, G, M}, {E, A, M, G}, {E, M, G, A}, {E, M, A, G},
    {M, G, A, E}, {M, G, E, A}, {M, A, G, E}, {M, A, E, G}, {M, E, G, A}, {M, E, A, G},
};

static uint32_t rd32(const uint8_t *p) { return p[0] | p[1] << 8 | p[2] << 16 | (uint32_t)p[3] << 24; }
static uint16_t rd16(const uint8_t *p) { return (uint16_t)(p[0] | p[1] << 8); }
static void wr32(uint8_t *p, uint32_t v) { p[0] = (uint8_t)v; p[1] = (uint8_t)(v >> 8); p[2] = (uint8_t)(v >> 16); p[3] = (uint8_t)(v >> 24); }
static void wr16(uint8_t *p, uint16_t v) { p[0] = (uint8_t)v; p[1] = (uint8_t)(v >> 8); }

#define SUB(m, s) ((m)->b + 32 + (s) * 12)

uint32_t mon_pid(const mon_t *m) { return rd32(m->b); }
uint32_t mon_otid(const mon_t *m) { return rd32(m->b + 4); }

uint16_t mon_checksum(const mon_t *m) {
    uint16_t sum = 0;
    for (int k = 32; k < 80; k += 2) sum = (uint16_t)(sum + rd16(m->b + k));
    return sum;
}

void mon_load(mon_t *m, const uint8_t *src, int party) {
    mem_set(m->b, 0, sizeof m->b);
    mem_copy(m->b, src, 32);
    if (party) mem_copy(m->b + 80, src + 80, 20);
    m->party = party;
    const uint32_t key = mon_pid(m) ^ mon_otid(m);
    const uint8_t *o = order[mon_pid(m) % 24];
    for (int slot = 0; slot < 4; slot++)
        for (int k = 0; k < 12; k += 4)
            wr32(SUB(m, o[slot]) + k, rd32(src + 32 + slot * 12 + k) ^ key);
    m->checksum_ok = mon_checksum(m) == rd16(m->b + 28);
}

void mon_store(const mon_t *m, uint8_t *dst) {
    mem_copy(dst, m->b, 32);
    wr16(dst + 28, mon_checksum(m));
    const uint32_t key = mon_pid(m) ^ mon_otid(m);
    const uint8_t *o = order[mon_pid(m) % 24];
    for (int slot = 0; slot < 4; slot++)
        for (int k = 0; k < 12; k += 4)
            wr32(dst + 32 + slot * 12 + k, rd32(SUB(m, o[slot]) + k) ^ key);
    if (m->party) mem_copy(dst + 80, m->b + 80, 20);
}

int mon_present(const mon_t *m) { return (m->b[19] & 2) && mon_species(m) != 0; }
int mon_is_egg(const mon_t *m) { return (m->b[19] & 4) || (rd32(SUB(m, M) + 4) >> 30 & 1); }
uint16_t mon_species(const mon_t *m) { return rd16(SUB(m, G)); }
uint16_t mon_item(const mon_t *m) { return rd16(SUB(m, G) + 2); }
void mon_set_item(mon_t *m, uint16_t v) { wr16(SUB(m, G) + 2, v); }
uint32_t mon_exp(const mon_t *m) { return rd32(SUB(m, G) + 4); }
void mon_set_exp(mon_t *m, uint32_t v) { wr32(SUB(m, G) + 4, v); }
uint8_t mon_pp_bonuses(const mon_t *m) { return SUB(m, G)[8]; }
uint16_t mon_move(const mon_t *m, int k) { return rd16(SUB(m, A) + 2 * k); }
uint8_t mon_pp(const mon_t *m, int k) { return SUB(m, A)[8 + k]; }
void mon_set_move(mon_t *m, int k, uint16_t move, uint8_t pp) {
    wr16(SUB(m, A) + 2 * k, move);
    SUB(m, A)[8 + k] = pp;
    /* a new move starts with no PP Ups */
    SUB(m, G)[8] = (uint8_t)(SUB(m, G)[8] & ~(3u << (2 * k)));
}
uint8_t mon_ev(const mon_t *m, int stat) { return SUB(m, E)[stat]; }
void mon_set_ev(mon_t *m, int stat, uint8_t v) { SUB(m, E)[stat] = v; }
int mon_ev_total(const mon_t *m) {
    int t = 0;
    for (int k = 0; k < 6; k++) t += mon_ev(m, k);
    return t;
}
uint8_t mon_iv(const mon_t *m, int stat) { return (uint8_t)(rd32(SUB(m, M) + 4) >> (5 * stat) & 31); }
void mon_set_iv(mon_t *m, int stat, uint8_t v) {
    uint32_t w = rd32(SUB(m, M) + 4);
    w = (w & ~(31u << (5 * stat))) | (uint32_t)(v & 31) << (5 * stat);
    wr32(SUB(m, M) + 4, w);
}
int mon_ability_bit(const mon_t *m) { return (int)(rd32(SUB(m, M) + 4) >> 31); }
int mon_nature(const mon_t *m) { return (int)(mon_pid(m) % 25); }
uint8_t *mon_nickname(mon_t *m) { return m->b + 8; }
uint8_t *mon_ot_name(mon_t *m) { return m->b + 20; }

uint8_t mon_level(const mon_t *m) { return m->b[84]; }
uint16_t mon_hp(const mon_t *m) { return rd16(m->b + 86); }
uint16_t mon_stat(const mon_t *m, int stat) {
    /* party order: max HP, Atk, Def, Speed, SpAtk, SpDef from byte 88 */
    return rd16(m->b + 88 + 2 * stat);
}
void mon_set_party_stats(mon_t *m, uint8_t level, const uint16_t stats[6], uint16_t hp) {
    m->b[84] = level;
    wr16(m->b + 86, hp);
    for (int k = 0; k < 6; k++) wr16(m->b + 88 + 2 * k, stats[k]);
}
